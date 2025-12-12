Imports System
Imports System.Web.Security

Public Class Logout
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            ' Limpia la sesión y redirige al Login
            Session.Clear()
            Session.Abandon()

            ' Cerrar sesión de autenticación por formularios
            FormsAuthentication.SignOut()

            Response.Redirect("~/Login.aspx", False)
            Context.ApplicationInstance.CompleteRequest()
        End Sub
    End Class
