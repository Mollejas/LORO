Imports System
Imports System.Web

Partial Public Class ElegirEmpresa
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' Verificar que el usuario esté autenticado
        If Session("UsuarioId") Is Nothing OrElse String.IsNullOrWhiteSpace(TryCast(Session("Nombre"), String)) Then
            Response.Redirect("~/Login.aspx", False)
            Context.ApplicationInstance.CompleteRequest()
            Return
        End If

        If Not IsPostBack Then
            ' Mostrar nombre del usuario
            Dim nombre As String = TryCast(Session("Nombre"), String)
            If Not String.IsNullOrWhiteSpace(nombre) Then
                litUsuario.Text = Server.HtmlEncode(nombre)
                divUserInfo.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnQualitas_Click(sender As Object, e As EventArgs)
        ' Guardar selección en sesión (DatabaseHelper usará esto para determinar la base de datos)
        Session("EmpresaSeleccionada") = "QUALITAS"

        ' Redirigir a página principal
        Response.Redirect("~/princi.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Protected Sub btnInbursa_Click(sender As Object, e As EventArgs)
        ' Guardar selección en sesión (DatabaseHelper usará esto para determinar la base de datos)
        Session("EmpresaSeleccionada") = "INBURSA"

        ' Redirigir a página principal
        Response.Redirect("~/princi.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Protected Sub btnExternos_Click(sender As Object, e As EventArgs)
        ' Guardar selección en sesión (DatabaseHelper usará esto para determinar la base de datos)
        Session("EmpresaSeleccionada") = "EXTERNOS"

        ' Redirigir a página principal
        Response.Redirect("~/princi.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

End Class
