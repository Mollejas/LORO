Imports System
Imports System.Web
Imports System.Web.Security

Namespace DAYTONAMIO
    ''' <summary>
    ''' HttpModule que valida que exista sesión de empresa (EmpresaSeleccionada)
    ''' y redirige a Login.aspx si no existe
    ''' </summary>
    Public Class SessionValidationModule
        Implements IHttpModule

        Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
            ' Registrar evento que se ejecuta después de la autenticación
            AddHandler context.PostAuthenticateRequest, AddressOf OnPostAuthenticateRequest
        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
            ' No hay recursos que liberar
        End Sub

        Private Sub OnPostAuthenticateRequest(sender As Object, e As EventArgs)
            Dim app As HttpApplication = DirectCast(sender, HttpApplication)
            Dim context As HttpContext = app.Context

            ' Ignorar si es una petición a Login.aspx o Logout.aspx
            Dim path As String = context.Request.Path.ToLowerInvariant()
            If path.EndsWith("login.aspx") OrElse path.EndsWith("logout.aspx") Then
                Return
            End If

            ' Ignorar recursos estáticos (imágenes, css, js, etc.)
            If path.Contains("/images/") OrElse
               path.Contains("/content/") OrElse
               path.Contains("/scripts/") OrElse
               path.EndsWith(".css") OrElse
               path.EndsWith(".js") OrElse
               path.EndsWith(".png") OrElse
               path.EndsWith(".jpg") OrElse
               path.EndsWith(".jpeg") OrElse
               path.EndsWith(".gif") OrElse
               path.EndsWith(".ico") OrElse
               path.EndsWith(".svg") Then
                Return
            End If

            ' Ignorar handlers (.ashx, .axd)
            If path.EndsWith(".ashx") OrElse path.EndsWith(".axd") Then
                Return
            End If

            ' Si el usuario está autenticado (FormsAuthentication ya validó)
            If context.User IsNot Nothing AndAlso context.User.Identity.IsAuthenticated Then
                ' Verificar que exista la sesión de empresa
                If context.Session IsNot Nothing Then
                    Dim empresa As Object = context.Session("EmpresaSeleccionada")

                    ' Si no existe EmpresaSeleccionada, redirigir a Login
                    If empresa Is Nothing OrElse String.IsNullOrWhiteSpace(empresa.ToString()) Then
                        ' Cerrar sesión y redirigir a login
                        context.Session.Clear()
                        FormsAuthentication.SignOut()

                        Dim loginUrl As String = FormsAuthentication.LoginUrl
                        If Not loginUrl.StartsWith("~/") AndAlso Not loginUrl.StartsWith("/") Then
                            loginUrl = "~/" & loginUrl
                        End If

                        context.Response.Redirect(loginUrl, True)
                    End If
                End If
            End If
        End Sub
    End Class
End Namespace