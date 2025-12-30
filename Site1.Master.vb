Imports System
Imports System.Data.SqlClient
Imports System.Configuration

Partial Public Class Site1
    Inherits System.Web.UI.MasterPage

    ' === Propiedades públicas para que las páginas hijas puedan saber si el usuario es admin y su nombre ===
    Public ReadOnly Property IsAdmin As Boolean
        Get
            ' Usar directamente Session("EsAdmin") que se establece en Login.aspx
            If Session("EsAdmin") IsNot Nothing Then
                Return CBool(Session("EsAdmin"))
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property CurrentUserName As String
        Get
            ' 1) Preferir el nombre que guardas en sesión
            Dim nombreSesion As String = TryCast(Session("Nombre"), String)
            If Not String.IsNullOrWhiteSpace(nombreSesion) Then Return nombreSesion.Trim()

            ' 2) Como respaldo, lo que ya pintas en litUser
            If litUser IsNot Nothing Then Return (If(litUser.Text, "")).Trim()

            Return String.Empty
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' Obliga sesión en cualquier página que use este Master
        RequireAuth()

        If Not Page.IsPostBack Then
            Dim nombre As String = TryCast(Session("Nombre"), String)
            litUser.Text = Server.HtmlEncode(If(nombre, ""))

            ' Calcula y cachea en ViewState si el usuario es admin
            ViewState("IsAdminFlag") = LookupIsAdmin(If(nombre, ""))

            ' Mostrar icono de configuración solo si es admin (desktop y móvil)
            pnlAdminConfig.Visible = IsAdmin
            pnlAdminConfigMobile.Visible = IsAdmin

            ' Configurar logo y links según empresa
            ConfigurarEmpresa()
        End If
    End Sub

    ' Configura logo y links según la empresa seleccionada en sesión
    Private Sub ConfigurarEmpresa()
        Dim empresa As String = DatabaseHelper.GetEmpresaSeleccionada()

        ' Configurar logo2 según empresa
        Select Case empresa.ToUpperInvariant()
            Case "QUALITAS"
                imgLogo2.ImageUrl = "~/images/logoqua.png"
                lnkCarpeta.NavigateUrl = "~/altaqua.aspx"
                lnkCarpetaMobile.NavigateUrl = "~/altaqua.aspx"
            Case "INBURSA"
                imgLogo2.ImageUrl = "~/images/logoinbur.png"
                lnkCarpeta.NavigateUrl = "~/alta.aspx"
                lnkCarpetaMobile.NavigateUrl = "~/alta.aspx"
            Case Else
                ' Default: INBURSA
                imgLogo2.ImageUrl = "~/images/logoinbur.png"
                lnkCarpeta.NavigateUrl = "~/alta.aspx"
                lnkCarpetaMobile.NavigateUrl = "~/alta.aspx"
        End Select
    End Sub

    Private Sub RequireAuth()
        ' Considera NO usar este Master en páginas públicas (Login, Recuperar, etc.)
        Dim noAutenticado As Boolean =
            (Session("UsuarioId") Is Nothing) OrElse String.IsNullOrWhiteSpace(TryCast(Session("Nombre"), String))

        If noAutenticado Then
            Dim ret As String = Request.RawUrl
            Response.Redirect("~/Login.aspx?returnUrl=" & Server.UrlEncode(ret), False)
            Context.ApplicationInstance.CompleteRequest()
        End If
    End Sub

    ' === Consulta a BD para saber si el usuario es admin (usuarios.esadmin=1) ===
    Private Function LookupIsAdmin(nombre As String) As Boolean
        If String.IsNullOrWhiteSpace(nombre) Then Return False

        Dim cs As String = DatabaseHelper.GetConnectionString()
        If cs Is Nothing Then Return False

        Using cn As New SqlConnection(cs)
            cn.Open()
            ' Búsqueda case-insensitive usando UPPER
            Using cmd As New SqlCommand("SELECT esadmin FROM usuarios WHERE UPPER(nombre) = UPPER(@n)", cn)
                cmd.Parameters.AddWithValue("@n", nombre.Trim())
                Dim o = cmd.ExecuteScalar()
                If o Is Nothing OrElse o Is DBNull.Value Then Return False
                ' Manejar tanto BIT (boolean) como INT
                If TypeOf o Is Boolean Then
                    Return CBool(o)
                Else
                    Return Convert.ToInt32(o) = 1
                End If
            End Using
        End Using
    End Function

End Class