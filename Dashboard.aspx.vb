Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls


Partial Public Class Dashboard
        Inherits System.Web.UI.Page

        Private Const SESSION_KEY As String = "DT_TRANSITO"

        '==================== Helpers para ubicar controles por ID (tipados) ====================
        Private Function FindInMainContent(Of T As Class)(id As String) As T
            Dim ctrl As Control = Nothing

            ' 1) Dentro del ContentPlaceHolder de la MasterPage (MainContent)
            If Me.Master IsNot Nothing Then
                Dim cph As ContentPlaceHolder = TryCast(Me.Master.FindControl("MainContent"), ContentPlaceHolder)
                If cph IsNot Nothing Then
                    ctrl = cph.FindControl(id)
                    If ctrl IsNot Nothing Then Return TryCast(ctrl, T)
                End If
            End If

            ' 2) Búsqueda directa en la página
            ctrl = Me.FindControl(id)
            Return TryCast(ctrl, T)
        End Function

        Private Function LV() As ListView
            Return FindInMainContent(Of ListView)("lvAdmisiones")
        End Function

        Private Function DP() As DataPager
            Dim lvCtl As ListView = LV()
            If lvCtl Is Nothing Then Return Nothing
            Return TryCast(lvCtl.FindControl("dpMain"), DataPager)
        End Function

        ' OJO: nombres distintos a lblCount/lblMsg para no chocar con el .designer
        Private Function FindLblCount() As Label
            Return FindInMainContent(Of Label)("lblCount")
        End Function

        Private Function FindLblMsg() As Label
            Return FindInMainContent(Of Label)("lblMsg")
        End Function

        Private Function TB(id As String) As TextBox
            Return FindInMainContent(Of TextBox)(id)
        End Function

        '==================== Cadena de conexión ====================
        Private ReadOnly Property ConnStr As String
            Get
                Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
                If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
                    Throw New ApplicationException("No se encontró la cadena de conexión 'DaytonaDB' en Web.config.")
                End If
                Return cs.ConnectionString
            End Get
        End Property

        '==================== Ciclo de vida ====================
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Try
                    CargarTodoATemporal()
                    BindLocal()
                Catch ex As Exception
                    ShowMsg("Error al cargar: " & ex.Message)
                End Try
            End If
        End Sub

        '==================== Carga a memoria (solo ESTATUS=TRANSITO) ====================
        Private Sub CargarTodoATemporal()
            Dim sql As String =
"SELECT
    A.Id,
    A.Expediente,
    A.SiniestroGen,
    A.TipoIngreso,
    A.Estatus,
    A.Marca, A.Tipo, A.Color, A.Modelo, A.Placas,
    A.CarpetaRel
FROM dbo.Admisiones AS A WITH (NOLOCK)
WHERE UPPER(ISNULL(A.Estatus,'')) = 'TRANSITO'
ORDER BY A.Id DESC;"

            Dim dt As New DataTable()
            Using cn As New SqlConnection(ConnStr)
                Using da As New SqlDataAdapter(sql, cn)
                    da.Fill(dt)
                End Using
            End Using

            ' Evita problemas de mayúsculas en RowFilter
            dt.CaseSensitive = False

            ' Columnas auxiliares (búsqueda case-insensitive)
            AddUpperColumn(dt, "Expediente", "ExpedienteText")
            AddUpperColumn(dt, "SiniestroGen", "SiniestroText")
            AddUpperColumn(dt, "Placas", "PlacasText")

            If Not dt.Columns.Contains("Vehiculo") Then dt.Columns.Add("Vehiculo", GetType(String))
            If Not dt.Columns.Contains("SearchTexto") Then dt.Columns.Add("SearchTexto", GetType(String))
            If Not dt.Columns.Contains("CarpetaRelLimpia") Then dt.Columns.Add("CarpetaRelLimpia", GetType(String))
            If Not dt.Columns.Contains("ImagenUrl") Then dt.Columns.Add("ImagenUrl", GetType(String))

            For Each r As DataRow In dt.Rows
                ' Vehículo (solo para búsqueda general)
                Dim vehParts As String() = {
                    Convert.ToString(r("Marca")).Trim(),
                    Convert.ToString(r("Tipo")).Trim(),
                    Convert.ToString(r("Color")).Trim(),
                    Convert.ToString(r("Modelo")).Trim(),
                    Convert.ToString(r("Placas")).Trim()
                }
                r("Vehiculo") = String.Join(" ", vehParts).Trim()

                ' CarpetaRel limpia (quita 10 primeros y a MAYÚSCULAS)
                Dim rawCarpeta As String = Convert.ToString(r("CarpetaRel")).Trim()
                Dim recortada As String = If(rawCarpeta.Length > 10, rawCarpeta.Substring(10), String.Empty)
                r("CarpetaRelLimpia") = recortada.ToUpperInvariant()

                ' URL imagen via handler (principal.jpg)
                r("ImagenUrl") = BuildImageHandlerUrl(rawCarpeta)

                ' Texto de búsqueda compuesto (incluye CarpetaRelLimpia)
                Dim sb As New StringBuilder()
                sb.Append(Convert.ToString(r("Expediente"))).Append(" "c).
                   Append(Convert.ToString(r("SiniestroGen"))).Append(" "c).
                   Append(Convert.ToString(r("TipoIngreso"))).Append(" "c).
                   Append(Convert.ToString(r("Estatus"))).Append(" "c).
                   Append(Convert.ToString(r("Vehiculo"))).Append(" "c).
                   Append(recortada)
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

        '==================== Botones (OnClick en .aspx) ====================
        Protected Sub btnBuscar_Click(sender As Object, e As EventArgs)
            Try
                ResetPagerToFirstPage()
                BindLocal()
            Catch ex As Exception
                ShowMsg("Error al filtrar: " & ex.Message)
            End Try
        End Sub

        Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
            Dim t As TextBox

            t = TB("txtCarpeta") : If t IsNot Nothing Then t.Text = ""
            t = TB("txtPlaca") : If t IsNot Nothing Then t.Text = ""
            t = TB("txtSiniestro") : If t IsNot Nothing Then t.Text = ""
            t = TB("txtBuscar") : If t IsNot Nothing Then t.Text = ""

            ResetPagerToFirstPage()
            BindLocal()

            Dim lm As Label = FindLblMsg()
            If lm IsNot Nothing Then lm.Visible = False
        End Sub

        Protected Sub btnRecargar_Click(sender As Object, e As EventArgs)
            CargarTodoATemporal()
            ResetPagerToFirstPage()
            BindLocal()
            ShowMsg("Datos recargados desde BD.")
        End Sub

        ' ListView: paginación (OnPagePropertiesChanging en .aspx)
        Protected Sub lvAdmisiones_PagePropertiesChanging(sender As Object, e As PagePropertiesChangingEventArgs)
            Dim pager As DataPager = DP()
            If pager IsNot Nothing Then
                pager.SetPageProperties(e.StartRowIndex, e.MaximumRows, False)
                BindLocal()
            End If
        End Sub

        Private Sub ResetPagerToFirstPage()
            Dim pager As DataPager = DP()
            If pager IsNot Nothing Then
                pager.SetPageProperties(0, pager.PageSize, False)
            End If
        End Sub

        '==================== Binding local con filtros ====================
        Private Sub BindLocal()
            Dim dt As DataTable = TryCast(Session(SESSION_KEY), DataTable)
            If dt Is Nothing Then
                CargarTodoATemporal()
                dt = TryCast(Session(SESSION_KEY), DataTable)
            End If

            Dim lc As Label = FindLblCount()
            Dim lm As Label = FindLblMsg()

            If dt Is Nothing Then
                ShowMsg("No se pudo cargar la tabla temporal.")
                If lc IsNot Nothing Then lc.Text = "0"
                Exit Sub
            End If

            Dim dv As DataView = dt.DefaultView
            If dv Is Nothing Then
                ShowMsg("No se pudo crear la vista de datos.")
                If lc IsNot Nothing Then lc.Text = "0"
                Exit Sub
            End If

            ' ====== Filtros ======
            Dim filtros As New List(Of String)()
            Dim t As TextBox
            Dim v As String

            ' No. Carpeta -> AHORA filtra por CarpetaRelLimpia (también acepta Expediente por compatibilidad)
            t = TB("txtCarpeta")
            If t IsNot Nothing Then v = t.Text.Trim() Else v = ""
            If v.Length <> 0 Then
                Dim val As String = EscLike(v).ToUpperInvariant()
                filtros.Add($"(CarpetaRelLimpia LIKE '%{val}%' OR ExpedienteText LIKE '%{val}%')")
            End If

            ' Placa
            t = TB("txtPlaca")
            If t IsNot Nothing Then v = t.Text.Trim() Else v = ""
            If v.Length <> 0 Then
                filtros.Add($"PlacasText LIKE '%{EscLike(v).ToUpperInvariant()}%'")
            End If

            ' Siniestro
            t = TB("txtSiniestro")
            If t IsNot Nothing Then v = t.Text.Trim() Else v = ""
            If v.Length <> 0 Then
                filtros.Add($"SiniestroText LIKE '%{EscLike(v).ToUpperInvariant()}%'")
            End If

            ' Búsqueda general (marca/tipo/color/modelo/placas/estatus/carpeta...)
            t = TB("txtBuscar")
            If t IsNot Nothing Then v = t.Text.Trim() Else v = ""
            If v.Length <> 0 Then
                filtros.Add($"SearchTexto LIKE '%{EscLike(v).ToUpperInvariant()}%'")
            End If

            ApplyRowFilter(dv, filtros)

            ' Bind al ListView
            Dim lvCtl As ListView = LV()
            If lvCtl Is Nothing Then
                Throw New InvalidOperationException("No se encontró el ListView 'lvAdmisiones' en el .aspx.")
            End If

            lvCtl.DataSource = dv
            lvCtl.DataBind()

            If lc IsNot Nothing Then lc.Text = dv.Count.ToString()
            If lm IsNot Nothing Then
                lm.Visible = (dv.Count = 0)
                lm.Text = If(dv.Count = 0, "No se encontraron registros.", "")
            End If
        End Sub

        Private Sub ApplyRowFilter(dv As DataView, filtros As List(Of String))
            If filtros Is Nothing OrElse filtros.Count = 0 Then
                dv.RowFilter = String.Empty
            Else
                dv.RowFilter = String.Join(" AND ", filtros.ToArray())
            End If
        End Sub

        '==================== Utilidades de filtro y rutas ====================
        Private Function EscLike(input As String) As String
            If String.IsNullOrEmpty(input) Then Return ""
            Dim s As String = input.Trim()
            s = s.Replace("'", "''")    ' comillas
            s = s.Replace("[", "[[]")   ' [
            s = s.Replace("%", "[%]")   ' %
            s = s.Replace("*", "[*]")   ' *
            Return s
        End Function

        Private Function NormalizeFolderPath(p As String) As String
            If p Is Nothing Then Return String.Empty
            Dim s As String = p.Trim()
            If s.Length = 0 Then Return s
            s = s.Replace("/"c, "\"c)        ' usa backslash
            s = s.Trim(" "c, """"c)          ' quita comillas y espacios extremos
            Do While s.Contains("\\\\")      ' colapsa slashes dobles
                s = s.Replace("\\\\", "\\")
            Loop
            Return s
        End Function

        Private Function Combine2(a As String, b As String) As String
            If String.IsNullOrWhiteSpace(a) Then Return b
            If String.IsNullOrWhiteSpace(b) Then Return a
            Return Path.Combine(a, b)
        End Function

        Private Function EnsureImageFullPath(basePath As String) As String
            Dim p As String = NormalizeFolderPath(basePath)
            If String.IsNullOrEmpty(p) Then Return p

            ' Si ya es un archivo (termina .jpg/.jpeg/.png), regresarlo tal cual
            Dim ext As String = Path.GetExtension(p)
            If ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) _
                OrElse ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) _
                OrElse ext.Equals(".png", StringComparison.OrdinalIgnoreCase) Then
                Return p
            End If

            ' Si ya trae la carpeta "1. DOCUMENTOS DE INGRESO", solo agregar principal.jpg
            If p.IndexOf("1. DOCUMENTOS DE INGRESO", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return Combine2(p, "principal.jpg")
            End If

            ' Caso normal: CarpetaRel + "1. DOCUMENTOS DE INGRESO" + principal.jpg
            Return Combine2(Combine2(p, "1. DOCUMENTOS DE INGRESO"), "principal.jpg")
        End Function

    ' === Construye la URL para ImgCarpeta.ashx (mapea ~/ y / a físico) ===
    Private Function BuildImageHandlerUrl(rawCarpetaRel As String) As String
        If String.IsNullOrWhiteSpace(rawCarpetaRel) Then
            Return "ImgCarpeta.ashx?b64="
        End If

        Dim carpeta As String = NormalizeFolderPath(rawCarpetaRel)

        ' 1) Resolver a ruta física
        Dim folderPath As String
        If carpeta.StartsWith("~\", StringComparison.Ordinal) OrElse carpeta.StartsWith("~/", StringComparison.Ordinal) Then
            folderPath = Server.MapPath(carpeta.Replace("\"c, "/"c))
        ElseIf carpeta.StartsWith("\", StringComparison.Ordinal) OrElse carpeta.StartsWith("/", StringComparison.Ordinal) Then
            folderPath = Server.MapPath("~" & carpeta.Replace("\"c, "/"c))
        Else
            Dim baseRoot As String = ConfigurationManager.AppSettings("BaseExpRoot")
            If Not String.IsNullOrWhiteSpace(baseRoot) AndAlso Not Path.IsPathRooted(carpeta) Then
                folderPath = Combine2(NormalizeFolderPath(baseRoot), carpeta)
            Else
                folderPath = carpeta
            End If
        End If

        ' 2) Asegura apuntar a principal.jpg
        Dim fullImg As String = EnsureImageFullPath(folderPath)

        ' 3) Tokenizar para el handler
        Dim token As String = ToUrlToken(fullImg)

        ' 4) Parámetro de versión basado en la fecha de modificación (solo cambia cuando cambia el archivo)
        Dim ver As String = ""
        Try
            Dim fi As New FileInfo(fullImg)
            If fi.Exists Then
                ver = "&v=" & fi.LastWriteTimeUtc.Ticks.ToString()
            End If
        Catch
            ' si algo falla, omitimos el versionado (seguirá funcionando con el placeholder)
        End Try

        Return "ImgCarpeta.ashx?b64=" & token & ver
    End Function

    Private Function ToUrlToken(s As String) As String
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(s)
            Return HttpServerUtility.UrlTokenEncode(bytes)
        End Function

        '==================== Mensajería ====================
        Private Sub ShowMsg(msg As String)
            Dim lm As Label = FindLblMsg()
            If lm IsNot Nothing Then
                lm.Visible = True
                lm.Text = msg
            End If
        End Sub

    End Class

