Imports System


Public Class Logout
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            ' Limpia la sesión y redirige al Login
            Session.Clear()
            Session.Abandon()

            ' Si usaras FormsAuth, aquí también expirarías la cookie de auth.
            ' Response.Cookies(FormsAuthentication.FormsCookieName).Expires = DateTime.UtcNow.AddDays(-1)

            Response.Redirect("~/Login.aspx", False)
            Context.ApplicationInstance.CompleteRequest()
        End Sub
    End Class
