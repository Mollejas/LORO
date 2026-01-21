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
            Using cmd As New SqlCommand("SELECT expediente, marca, tipo, modelo, color, placas, carpetarel FROM admisiones WHERE id = @id", cn)
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = admId

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim expediente As String = If(rd.IsDBNull(0), "", rd.GetString(0))
                        hidExpediente.Value = expediente
                        lblExpediente.Text = expediente
                        lblMarca.Text = If(rd.IsDBNull(1), "", rd.GetString(1))
                        lblModelo.Text = If(rd.IsDBNull(2), "", rd.GetString(2))  ' Tipo es el modelo del vehículo
                        lblAnio.Text = If(rd.IsDBNull(3), "", rd.GetValue(3).ToString())  ' Modelo es el año
                        lblColor.Text = If(rd.IsDBNull(4), "", rd.GetString(4))
                        lblPlacas.Text = If(rd.IsDBNull(5), "", rd.GetString(5))

                        Dim carpetaRel As String = If(rd.IsDBNull(6), "", rd.GetString(6))

                        ' Cargar imagen principal (mismo método que Hoja de Trabajo)
                        If Not String.IsNullOrWhiteSpace(carpetaRel) Then
                            Dim baseFolder As String = ResolverCarpetaFisica(carpetaRel)
                            Dim imgPath As String = Path.Combine(baseFolder, "1. DOCUMENTOS DE INGRESO", "principal.jpg")
                            If File.Exists(imgPath) Then
                                Dim bytes = File.ReadAllBytes(imgPath)
                                imgPrincipal.ImageUrl = "data:image/jpeg;base64," & Convert.ToBase64String(bytes)
                            Else
                                imgPrincipal.ImageUrl = ""
                            End If
                        End If
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Function ResolverCarpetaFisica(carpetaRel As String) As String
        If String.IsNullOrWhiteSpace(carpetaRel) Then Return ""
        If carpetaRel.StartsWith("~") Then
            Return Server.MapPath(carpetaRel)
        Else
            Return Server.MapPath("~" & carpetaRel)
        End If
    End Function

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
