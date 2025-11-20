Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text

Partial Public Class BuscarTransito
    Inherits System.Web.UI.Page

    Private Const SESSION_KEY As String = "DT_TRANSITO"
    Private Const VS_SORT_EXPR As String = "SortExpr"
    Private Const VS_SORT_DIR As String = "SortDir" ' "ASC" / "DESC"

    Private ReadOnly Property ConnStr As String
        Get
            Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
            If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
                Throw New ApplicationException("No se encontró la cadena de conexión 'DaytonaDB' en Web.config.")
            End If
            Return cs.ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                CargarTodoATemporal()
                ' Orden inicial: mayor Días Tránsito primero
                ViewState(VS_SORT_EXPR) = "DiasTransito"
                ViewState(VS_SORT_DIR) = "DESC"
                BindGridLocal()
            Catch ex As Exception
                ShowMsg("Error al cargar: " & ex.Message)
            End Try
        End If
    End Sub

    ' ==== CARGA DESDE BD (solo ESTATUS=TRANSITO) ====
    Private Sub CargarTodoATemporal()
        Dim sql As String =
"SELECT
    A.Id,
    A.Expediente,
    A.SiniestroGen,
    A.Tipo       AS Tipo,         -- Submarca
    A.Marca      AS Marca,
    A.Modelo     AS Modelo,
    A.Color      AS Color,
    A.Placas     AS Placas,
    A.Estatus    AS Estatus,
    A.FechaCreacion
FROM dbo.Admisiones AS A WITH (NOLOCK)
WHERE UPPER(ISNULL(A.Estatus,'')) = 'TRANSITO';"

        Dim dt As New DataTable()
        Using cn As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, cn)
                da.Fill(dt)
            End Using
        End Using

        ' ===== Columnas requeridas por el Grid =====
        EnsureColumn(Of Integer)(dt, "DiasTransito")
        EnsureColumn(Of String)(dt, "EstatusProceso")
        EnsureColumn(Of String)(dt, "PiezasInfo")
        EnsureColumn(Of String)(dt, "Categoria")

        ' ===== Helpers para filtros =====
        EnsureColumn(Of String)(dt, "ExpedienteText")
        EnsureColumn(Of String)(dt, "SiniestroText")
        EnsureColumn(Of String)(dt, "PlacasText")
        EnsureColumn(Of String)(dt, "SearchTexto")

        ' ===== Relleno / Cálculos =====
        For Each r As DataRow In dt.Rows
            ' Días tránsito = hoy - FechaCreacion
            Dim dias As Integer = 0
            If Not IsDBNull(r("FechaCreacion")) Then
                Dim f As Date = Convert.ToDateTime(r("FechaCreacion"))
                dias = CInt((DateTime.Now - f).TotalDays)
                If dias < 0 Then dias = 0
            End If
            r("DiasTransito") = dias

            ' Texto proceso (placeholder si aún no tienes proceso)
            If IsDBNull(r("EstatusProceso")) OrElse String.IsNullOrWhiteSpace(Convert.ToString(r("EstatusProceso"))) Then
                r("EstatusProceso") = "-"
            End If

            ' Piezas total/recibidas (placeholder hasta tener tabla real)
            If IsDBNull(r("PiezasInfo")) OrElse String.IsNullOrWhiteSpace(Convert.ToString(r("PiezasInfo"))) Then
                r("PiezasInfo") = "0 / 0"
            End If

            ' Categoría (placeholder)
            If IsDBNull(r("Categoria")) Then r("Categoria") = ""

            ' Helpers mayúsculas
            r("ExpedienteText") = Convert.ToString(r("Expediente")).ToUpperInvariant()
            r("SiniestroText") = Convert.ToString(r("SiniestroGen")).ToUpperInvariant()
            r("PlacasText") = Convert.ToString(r("Placas")).ToUpperInvariant()

            ' SearchTexto
            Dim sb As New StringBuilder()
            sb.Append(Convert.ToString(r("Expediente"))).Append(" "c).
               Append(Convert.ToString(r("SiniestroGen"))).Append(" "c).
               Append(Convert.ToString(r("Marca"))).Append(" "c).
               Append(Convert.ToString(r("Tipo"))).Append(" "c).
               Append(Convert.ToString(r("Modelo"))).Append(" "c).
               Append(Convert.ToString(r("Color"))).Append(" "c).
               Append(Convert.ToString(r("Placas"))).Append(" "c).
               Append(Convert.ToString(r("EstatusProceso"))).Append(" "c).
               Append(Convert.ToString(r("Categoria")))
            r("SearchTexto") = sb.ToString().ToUpperInvariant()
        Next

        ' Guarda en sesión
        Session(SESSION_KEY) = dt
    End Sub

    Private Sub EnsureColumn(Of T)(dt As DataTable, colName As String)
        If Not dt.Columns.Contains(colName) Then
            dt.Columns.Add(colName, GetType(T))
            ' Inicializa valores por defecto
            For Each r As DataRow In dt.Rows
                If GetType(T) Is GetType(String) Then
                    r(colName) = ""
                ElseIf GetType(T) Is GetType(Integer) OrElse GetType(T) Is GetType(Decimal) Then
                    r(colName) = 0
                ElseIf GetType(T) Is GetType(Date) Then
                    r(colName) = DBNull.Value
                Else
                    r(colName) = DBNull.Value
                End If
            Next
        End If
    End Sub

    ' ==== FILTROS ====
    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Try
            BindGridLocal()
        Catch ex As Exception
            ShowMsg("Error al filtrar: " & ex.Message)
        End Try
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        txtCarpeta.Text = ""
        txtPlaca.Text = ""
        txtSiniestro.Text = ""
        txtBuscar.Text = ""
        BindGridLocal()
        lblMsg.Visible = False
    End Sub

    Protected Sub btnRecargar_Click(sender As Object, e As EventArgs)
        CargarTodoATemporal()
        BindGridLocal()
        ShowMsg("Datos recargados desde BD.")
    End Sub

    ' ==== ORDENAMIENTO ====
    Protected Sub gvAdmisiones_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs)
        ' Si el usuario hace click en la misma columna, alterna ASC/DESC.
        Dim currentExpr As String = Convert.ToString(ViewState(VS_SORT_EXPR))
        Dim currentDir As String = Convert.ToString(ViewState(VS_SORT_DIR))
        Dim newExpr As String = e.SortExpression

        If String.Equals(currentExpr, newExpr, StringComparison.OrdinalIgnoreCase) Then
            ViewState(VS_SORT_DIR) = If(currentDir = "ASC", "DESC", "ASC")
        Else
            ViewState(VS_SORT_EXPR) = newExpr
            ViewState(VS_SORT_DIR) = "ASC"
        End If

        BindGridLocal()
    End Sub

    ' (Opcional) dar formato especial a alguna celda
    Protected Sub gvAdmisiones_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs)
        ' Puedes aplicar estilos condicionales aquí si quieres
    End Sub

    ' ==== BINDEO LOCAL (filtros + orden) ====
    Private Sub BindGridLocal()
        Dim dt As DataTable = TryCast(Session(SESSION_KEY), DataTable)
        If dt Is Nothing Then
            CargarTodoATemporal()
            dt = TryCast(Session(SESSION_KEY), DataTable)
        End If

        Dim dv As DataView = dt.DefaultView

        ' ===== Filtros =====
        Dim filtros As New List(Of String)
        Dim fCarpeta As String = EscLike(txtCarpeta.Text).ToUpperInvariant()
        Dim fPlaca As String = EscLike(txtPlaca.Text).ToUpperInvariant()
        Dim fSiniestro As String = EscLike(txtSiniestro.Text).ToUpperInvariant()
        Dim fGeneral As String = EscLike(txtBuscar.Text).ToUpperInvariant()

        If fCarpeta <> "" Then filtros.Add($"ExpedienteText LIKE '%{fCarpeta}%'")
        If fPlaca <> "" Then filtros.Add($"PlacasText LIKE '%{fPlaca}%'")
        If fSiniestro <> "" Then filtros.Add($"SiniestroText LIKE '%{fSiniestro}%'")
        If fGeneral <> "" Then filtros.Add($"SearchTexto LIKE '%{fGeneral}%'")

        dv.RowFilter = String.Join(" AND ", filtros)

        ' ===== Orden =====
        Dim sortExpr As String = Convert.ToString(ViewState(VS_SORT_EXPR))
        Dim sortDir As String = Convert.ToString(ViewState(VS_SORT_DIR))

        If Not String.IsNullOrEmpty(sortExpr) AndAlso dt.Columns.Contains(sortExpr) Then
            dv.Sort = sortExpr & " " & If(String.IsNullOrEmpty(sortDir), "ASC", sortDir)
        Else
            ' Fallback seguro: siempre aplica DiasTransito DESC si no hay sort válido
            dv.Sort = "DiasTransito DESC"
            ViewState(VS_SORT_EXPR) = "DiasTransito"
            ViewState(VS_SORT_DIR) = "DESC"
        End If

        gvAdmisiones.DataSource = dv
        gvAdmisiones.DataBind()

        lblCount.Text = dv.Count.ToString()
        lblMsg.Visible = (dv.Count = 0)
        lblMsg.Text = If(dv.Count = 0, "No se encontraron registros.", "")
    End Sub


    ' Escapa caracteres especiales de RowFilter
    Private Function EscLike(input As String) As String
        If String.IsNullOrEmpty(input) Then Return ""
        Dim s = input.Trim()
        s = s.Replace("'", "''")    ' comillas
        s = s.Replace("[", "[[]")   ' [
        s = s.Replace("%", "[%]")   ' %
        s = s.Replace("*", "[*]")   ' *
        Return s
    End Function

    Private Sub ShowMsg(msg As String)
        lblMsg.Visible = True
        lblMsg.Text = msg
    End Sub

End Class
