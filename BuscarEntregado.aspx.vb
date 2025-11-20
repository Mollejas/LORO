Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text

Partial Public Class BuscarEntregado
    Inherits System.Web.UI.Page

    Private Const SESSION_KEY As String = "DT_TRANSITO"

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
                BindGridLocal()
            Catch ex As Exception
                ShowMsg("Error al cargar: " & ex.Message)
            End Try
        End If
    End Sub

    ' Carga ÚNICA desde SQL: solo ESTATUS=TRANSITO
    Private Sub CargarTodoATemporal()
        Dim sql As String =
"SELECT
    A.Id,
    A.Expediente,
    A.SiniestroGen,
    A.TipoIngreso,
    A.Estatus,
    A.Marca, A.Tipo, A.Color, A.Modelo, A.Placas
FROM dbo.Admisiones AS A WITH (NOLOCK)
WHERE UPPER(ISNULL(A.Estatus,'')) = 'ENTREGADO'
ORDER BY A.Id DESC;"

        Dim dt As New DataTable()
        Using cn As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, cn)
                da.Fill(dt)
            End Using
        End Using

        ' Columnas calculadas (mayúsculas para búsqueda case-insensitive)
        AddUpperColumn(dt, "Expediente", "ExpedienteText")
        AddUpperColumn(dt, "SiniestroGen", "SiniestroText")
        AddUpperColumn(dt, "Placas", "PlacasText")

        If Not dt.Columns.Contains("Vehiculo") Then dt.Columns.Add("Vehiculo", GetType(String))
        If Not dt.Columns.Contains("SearchTexto") Then dt.Columns.Add("SearchTexto", GetType(String))

        For Each r As DataRow In dt.Rows
            Dim veh As String = String.Join(" ",
                New String() {
                    Convert.ToString(r("Marca")).Trim(),
                    Convert.ToString(r("Tipo")).Trim(),
                    Convert.ToString(r("Color")).Trim(),
                    Convert.ToString(r("Modelo")).Trim(),
                    Convert.ToString(r("Placas")).Trim()
                }).Trim()
            r("Vehiculo") = veh

            Dim sb As New StringBuilder()
            sb.Append(Convert.ToString(r("Expediente"))).Append(" "c).
               Append(Convert.ToString(r("SiniestroGen"))).Append(" "c).
               Append(Convert.ToString(r("TipoIngreso"))).Append(" "c).
               Append(Convert.ToString(r("Estatus"))).Append(" "c).
               Append(veh)
            r("SearchTexto") = sb.ToString().ToUpperInvariant()
        Next

        Session(SESSION_KEY) = dt
    End Sub

    Private Sub AddUpperColumn(dt As DataTable, sourceName As String, upperName As String)
        If Not dt.Columns.Contains(upperName) Then dt.Columns.Add(upperName, GetType(String))
        For Each r As DataRow In dt.Rows
            r(upperName) = Convert.ToString(r(sourceName)).ToUpperInvariant()
        Next
    End Sub

    ' Botón Filtrar (lo dispara el debounce JS y manualmente)
    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Try
            gvAdmisiones.PageIndex = 0
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
        gvAdmisiones.PageIndex = 0
        BindGridLocal()
        lblMsg.Visible = False
    End Sub

    ' Refrescar desde BD (sin reciclar app)
    Protected Sub btnRecargar_Click(sender As Object, e As EventArgs)
        CargarTodoATemporal()
        gvAdmisiones.PageIndex = 0
        BindGridLocal()
        ShowMsg("Datos recargados desde BD.")
    End Sub

    ' Paginación local
    Protected Sub gvAdmisiones_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvAdmisiones.PageIndexChanging
        gvAdmisiones.PageIndex = e.NewPageIndex
        BindGridLocal()
    End Sub

    ' Aplica filtros sobre el DataTable en Session (DataView.RowFilter)
    Private Sub BindGridLocal()
        Dim dt As DataTable = TryCast(Session(SESSION_KEY), DataTable)
        If dt Is Nothing Then
            CargarTodoATemporal()
            dt = TryCast(Session(SESSION_KEY), DataTable)
        End If

        Dim dv As DataView = dt.DefaultView
        Dim filtros As New List(Of String)

        Dim fCarpeta As String = EscLike(txtCarpeta.Text)
        Dim fPlaca As String = EscLike(txtPlaca.Text)
        Dim fSiniestro As String = EscLike(txtSiniestro.Text)
        Dim fGeneral As String = EscLike(txtBuscar.Text)

        If fCarpeta <> "" Then filtros.Add($"ExpedienteText LIKE '%{fCarpeta.ToUpperInvariant()}%'")
        If fPlaca <> "" Then filtros.Add($"PlacasText LIKE '%{fPlaca.ToUpperInvariant()}%'")
        If fSiniestro <> "" Then filtros.Add($"SiniestroText LIKE '%{fSiniestro.ToUpperInvariant()}%'")
        If fGeneral <> "" Then filtros.Add($"SearchTexto LIKE '%{fGeneral.ToUpperInvariant()}%'")

        dv.RowFilter = String.Join(" AND ", filtros)

        gvAdmisiones.DataSource = dv
        gvAdmisiones.DataBind()

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
