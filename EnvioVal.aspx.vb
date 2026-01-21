Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.IO
Imports System.Web

Partial Public Class EnvioVal
    Inherits System.Web.UI.Page

    Private Function GetCs() As String
        Return DatabaseHelper.GetConnectionString()
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim sid As String = Request.QueryString("id")
            If String.IsNullOrWhiteSpace(sid) Then
                Response.Write("ID de admisión no especificado")
                Return
            End If

            Dim admId As Integer
            If Not Integer.TryParse(sid, admId) Then
                Response.Write("ID de admisión inválido")
                Return
            End If

            hidId.Value = sid
            CargarDatosVehiculo(admId)
            CargarRefacciones()
        End If
    End Sub

    Private Sub CargarDatosVehiculo(admId As Integer)
        Dim cs As String = GetCs()
        If String.IsNullOrWhiteSpace(cs) Then Return

        Using cn As New SqlConnection(cs)
            cn.Open()
            Using cmd As New SqlCommand("SELECT expediente, anio, marca, modelo, color, placas, carpeta FROM admisiones WHERE id = @id", cn)
                cmd.Parameters.AddWithValue("@id", admId)

                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Dim expediente As String = If(reader("expediente")?.ToString(), "")
                        hidExpediente.Value = expediente
                        lblExpediente.Text = expediente
                        lblAnio.Text = If(reader("anio")?.ToString(), "—")
                        lblMarca.Text = If(reader("marca")?.ToString(), "—")
                        lblModelo.Text = If(reader("modelo")?.ToString(), "—")
                        lblColor.Text = If(reader("color")?.ToString(), "—")
                        lblPlacas.Text = If(reader("placas")?.ToString(), "—")

                        ' Cargar imagen principal
                        Dim carpeta As String = If(reader("carpeta")?.ToString(), "")
                        If Not String.IsNullOrWhiteSpace(carpeta) Then
                            Dim carpetaFisica As String = Server.MapPath(carpeta)
                            Dim subFolder As String = Path.Combine(carpetaFisica, "1. DOCUMENTOS DE INGRESO")
                            Dim principalPath As String = Path.Combine(subFolder, "principal.jpg")

                            If File.Exists(principalPath) Then
                                Dim relativePath As String = carpeta.TrimStart("~"c) & "/1. DOCUMENTOS DE INGRESO/principal.jpg"
                                imgPrincipal.ImageUrl = relativePath & "?v=" & DateTime.Now.Ticks.ToString()
                            Else
                                imgPrincipal.Visible = False
                            End If
                        End If
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub CargarRefacciones()
        Dim expediente As String = hidExpediente.Value
        If String.IsNullOrWhiteSpace(expediente) Then Return

        Dim cs As String = GetCs()
        Using cn As New SqlConnection(cs)
            cn.Open()

            ' Mecánica - Sustitución
            Dim dtMecSust As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, numparte, ISNULL(precio, 0) as precio FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'MECANICA' AND UPPER(categoria) = 'SUSTITUCION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtMecSust)
                End Using
            End Using
            gvMecSustitucion.DataSource = dtMecSust
            gvMecSustitucion.DataBind()

            ' Mecánica - Reparación
            Dim dtMecRep As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, observ1, ISNULL(precio, 0) as precio FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'MECANICA' AND UPPER(categoria) = 'REPARACION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtMecRep)
                End Using
            End Using
            gvMecReparacion.DataSource = dtMecRep
            gvMecReparacion.DataBind()

            ' Hojalatería - Sustitución
            Dim dtHojSust As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, numparte, ISNULL(precio, 0) as precio FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'HOJALATERIA' AND UPPER(categoria) = 'SUSTITUCION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtHojSust)
                End Using
            End Using
            gvHojSustitucion.DataSource = dtHojSust
            gvHojSustitucion.DataBind()

            ' Hojalatería - Reparación
            Dim dtHojRep As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, observ1, ISNULL(precio, 0) as precio FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'HOJALATERIA' AND UPPER(categoria) = 'REPARACION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtHojRep)
                End Using
            End Using
            gvHojReparacion.DataSource = dtHojRep
            gvHojReparacion.DataBind()
        End Using
    End Sub

    ' ========== EVENTOS DE GRIDVIEW ==========
    Protected Sub gv_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs)
        Dim gv As GridView = CType(sender, GridView)
        gv.EditIndex = e.NewEditIndex
        CargarRefacciones()
    End Sub

    Protected Sub gv_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs)
        Dim gv As GridView = CType(sender, GridView)
        gv.EditIndex = -1
        CargarRefacciones()
    End Sub

    Protected Sub gvMecSust_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs)
        ActualizarPrecio(gvMecSustitucion, e)
    End Sub

    Protected Sub gvMecRep_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs)
        ActualizarPrecio(gvMecReparacion, e)
    End Sub

    Protected Sub gvHojSust_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs)
        ActualizarPrecio(gvHojSustitucion, e)
    End Sub

    Protected Sub gvHojRep_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs)
        ActualizarPrecio(gvHojReparacion, e)
    End Sub

    Private Sub ActualizarPrecio(gv As GridView, e As System.Web.UI.WebControls.GridViewUpdateEventArgs)
        Try
            Dim row As GridViewRow = gv.Rows(e.RowIndex)
            Dim id As Integer = Convert.ToInt32(gv.DataKeys(e.RowIndex).Value)
            Dim txtPrecio As TextBox = TryCast(row.FindControl("txtPrecio"), TextBox)

            If txtPrecio IsNot Nothing Then
                Dim precio As Decimal = 0
                Decimal.TryParse(txtPrecio.Text.Trim(), precio)

                Dim cs As String = GetCs()
                Using cn As New SqlConnection(cs)
                    Using cmd As New SqlCommand("UPDATE refacciones SET precio = @precio WHERE id = @id", cn)
                        cmd.Parameters.AddWithValue("@precio", precio)
                        cmd.Parameters.AddWithValue("@id", id)
                        cn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
            End If

            gv.EditIndex = -1
            CargarRefacciones()

        Catch ex As Exception
            ' Log error
            Response.Write("Error al actualizar precio: " & ex.Message)
        End Try
    End Sub

End Class
