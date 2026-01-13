<%@ WebHandler Language="VB" Class="GetSiguienteIndicePresup" %>

Imports System
Imports System.Web
Imports System.IO
Imports System.Text.RegularExpressions

Public Class GetSiguienteIndicePresup
    Implements System.Web.IHttpHandler

    Private Const PREFIX_PRESUP As String = "recep"

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"

        Try
            Dim carpetaRel As String = context.Request.QueryString("carpetaRel")

            If String.IsNullOrWhiteSpace(carpetaRel) Then
                context.Response.Write("{""ok"":false,""error"":""Carpeta no especificada""}")
                Return
            End If

            ' Construir ruta física
            Dim carpetaBaseFisica As String = context.Server.MapPath(carpetaRel)
            Dim subcarpeta As String = Path.Combine(carpetaBaseFisica, "1. DOCUMENTOS DE INGRESO")

            Dim indice As Integer = ObtenerSiguienteIndice(subcarpeta)

            context.Response.Write($"{{""ok"":true,""indice"":{indice}}}")

        Catch ex As Exception
            context.Response.Write("{""ok"":false,""error"":""" & ex.Message.Replace("""", "'") & """}")
        End Try
    End Sub

    Private Function ObtenerSiguienteIndice(folder As String) As Integer
        If Not Directory.Exists(folder) Then Return 1

        Dim maxN As Integer = 0
        Dim regex As New Regex("^" & Regex.Escape(PREFIX_PRESUP) & "(\d+)\.jpg$", RegexOptions.IgnoreCase)

        For Each f In Directory.GetFiles(folder, PREFIX_PRESUP & "*.jpg")
            Dim name As String = Path.GetFileName(f)
            Dim m As Match = regex.Match(name)
            If m.Success Then
                Dim n As Integer
                If Integer.TryParse(m.Groups(1).Value, n) AndAlso n > maxN Then
                    maxN = n
                End If
            End If
        Next

        Return maxN + 1
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
