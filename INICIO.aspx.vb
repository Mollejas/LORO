Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Partial Public Class Inicio
    Inherits System.Web.UI.Page

    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Dim vCarpeta = txtCarpeta.Text.Trim()
        Dim vPlaca = txtPlaca.Text.Trim()
        Dim vSiniestro = txtSiniestro.Text.Trim()

        ' Contar cuántos campos vienen con valor
        Dim filled = 0
        If vCarpeta <> "" Then filled += 1
        If vPlaca <> "" Then filled += 1
        If vSiniestro <> "" Then filled += 1

        If filled = 0 Then
            Alert("Debes ingresar un campo de búsqueda.")
            Exit Sub
        End If
        If filled > 1 Then
            Alert("Solo puedes buscar por un campo a la vez.")
            Exit Sub
        End If

        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing Then
            Alert("No se encontró la cadena de conexión DaytonaDB en Web.config.")
            Exit Sub
        End If

        Dim sql As String = "
            SELECT TOP 1 Id
            FROM dbo.Admisiones
            WHERE 1=1
        "

        Dim paramName As String = Nothing

        If vCarpeta <> "" Then
            sql &= " AND Expediente = @val"
            paramName = "@val"
        ElseIf vPlaca <> "" Then
            sql &= " AND Placas = @val"
            paramName = "@val"
        ElseIf vSiniestro <> "" Then
            ' Buscar por cualquiera de los dos siniestros almacenados
            sql &= " AND (SiniestroGen = @val OR SiniestroIdent = @val)"
            paramName = "@val"
        End If

        Try
            Using cn As New SqlConnection(cs.ConnectionString)
                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.AddWithValue(paramName, If(vCarpeta <> "", vCarpeta, If(vPlaca <> "", vPlaca, vSiniestro)))
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        Alert("No se encontró ningún expediente con ese criterio.")
                        Exit Sub
                    End If

                    Dim id As Integer = Convert.ToInt32(obj)
                    ' Redirigir a Hoja de Trabajo con el Id
                    Response.Redirect($"Hoja.aspx?id={id}", False)
                End Using
            End Using
        Catch ex As Exception
            Alert("Error al buscar: " & ex.Message)
        End Try
    End Sub

    Private Sub Alert(msg As String)
        ClientScript.RegisterStartupScript(Me.GetType(), "msg", "alert('" & msg.Replace("'", "\'") & "');", True)
    End Sub
End Class
