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

        ' Campos permitidos (case insensitive)
        Dim allowedFields As String() = {"autorizado", "estatus", "complemento", "nivel_rep_l", "nivel_rep_m", "nivel_rep_f", "nivel_rep_pint_l", "nivel_rep_pint_m", "nivel_rep_pint_f"}
        If Not allowedFields.Contains(field) Then
            context.Response.Write("{""ok"":false,""error"":""Campo no permitido: [" & field & "]""}")
            Return
        End If

        Try
            Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
            Using cn As New SqlConnection(cs)
                cn.Open()

                Dim sql As String
                ' Todos los campos excepto estatus son BIT/INT
                If field = "estatus" Then
                    sql = "UPDATE refacciones SET estatus = @val WHERE id = @id"
                Else
                    ' Para autorizado, complemento, y todos los nivel_rep_* usar el nombre del campo dinámicamente
                    sql = "UPDATE refacciones SET " & field & " = @val WHERE id = @id"
                End If

                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(idStr))

                    If field = "estatus" Then
                        cmd.Parameters.AddWithValue("@val", val)
                    Else
                        ' Todos los demás campos son BIT/INT
                        cmd.Parameters.AddWithValue("@val", Convert.ToInt32(val))
                    End If

                    Dim rows As Integer = cmd.ExecuteNonQuery()
                    context.Response.Write("{""ok"":" & (rows > 0).ToString().ToLower() & "}")
                End Using
            End Using
        Catch ex As Exception
            context.Response.Write("{""ok"":false,""error"":""" & ex.Message.Replace("""", "'") & """}")
        End Try
    End Sub

End Class
