Imports System
Imports System.Web
Imports System.Data.SqlClient
Imports System.Configuration

Public Class UpdateRefaccion
    Implements IHttpHandler

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"

        Dim idStr As String = context.Request("id")
        Dim field As String = If(context.Request("field"), "").Trim().ToLower()
        Dim val As String = context.Request("val")

        ' Validar parámetros
        If String.IsNullOrWhiteSpace(idStr) OrElse Not Integer.TryParse(idStr, Nothing) Then
            context.Response.Write("{""ok"":false,""error"":""ID inválido""}")
            Return
        End If

        ' Validar que field no esté vacío
        If String.IsNullOrWhiteSpace(field) Then
            context.Response.Write("{""ok"":false,""error"":""Campo vacío o no especificado""}")
            Return
        End If

        ' Solo permitir actualizar autorizado, estatus y complemento (case insensitive)
        If field <> "autorizado" AndAlso field <> "estatus" AndAlso field <> "complemento" Then
            context.Response.Write("{""ok"":false,""error"":""Campo no permitido: [" & field & "]""}")
            Return
        End If

        Try
            Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
            Using cn As New SqlConnection(cs)
                cn.Open()

                Dim sql As String
                If field = "autorizado" Then
                    sql = "UPDATE refacciones SET autorizado = @val WHERE id = @id"
                ElseIf field = "complemento" Then
                    sql = "UPDATE refacciones SET complemento = @val WHERE id = @id"
                Else
                    sql = "UPDATE refacciones SET estatus = @val WHERE id = @id"
                End If

                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(idStr))

                    If field = "autorizado" OrElse field = "complemento" Then
                        cmd.Parameters.AddWithValue("@val", Convert.ToInt32(val))
                    Else
                        cmd.Parameters.AddWithValue("@val", val)
                    End If

                    Dim rows As Integer = cmd.ExecuteNonQuery()
                    context.Response.Write("{""ok"":" & (rows > 0).ToString().ToLower() & ",""field"":""" & field & """,""val"":""" & val & """,""rows"":" & rows.ToString() & "}")
                End Using
            End Using
        Catch ex As Exception
            context.Response.Write("{""ok"":false,""error"":""" & ex.Message.Replace("""", "'") & """,""field"":""" & field & """}")
        End Try
    End Sub

End Class
