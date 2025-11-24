Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports Path = System.IO.Path

Public Class AltaQua
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' No hay inicialización especial necesaria
        End If
    End Sub

    ' Se dispara cuando el JS hace click en btnTrigger al cargar el PDF
    Protected Sub BtnTrigger_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Not fupPDF.HasFile Then
            Exit Sub
        End If

        Dim ext As String = Path.GetExtension(fupPDF.FileName).ToLowerInvariant()
        If ext <> ".pdf" Then
            Exit Sub
        End If

        ' Carpeta temporal para guardar el volante
        Dim tempFolder As String = Server.MapPath("~/App_Data/Uploads")
        If Not Directory.Exists(tempFolder) Then
            Directory.CreateDirectory(tempFolder)
        End If

        Dim fileName As String = Path.GetFileName(fupPDF.FileName)
        Dim fullPath As String = Path.Combine(tempFolder, fileName)
        fupPDF.SaveAs(fullPath)

        Try
            Using reader As New PdfReader(fullPath)
                Dim pagina As Integer = 1

                ' ------- RECTÁNGULOS (coordenadas con origen abajo-izquierda) -------
                ' Estos son los mismos rectángulos que en LeerOrden.aspx.vb
                Dim rNumeroReporte As New Rectangle(85.0F, 1234.69F, 140.04F, 1247.06F)
                Dim rNombreCliente As New Rectangle(23.0F, 1177.18F, 164.38F, 1188.17F)
                Dim rTelefono As New Rectangle(338.0F, 1177.18F, 386.93F, 1188.17F)
                Dim rEmail As New Rectangle(23.0F, 1150.18F, 102.69F, 1161.17F)
                Dim rMarca As New Rectangle(23.0F, 1055.18F, 55.9F, 1066.17F)
                Dim rTipo As New Rectangle(159.0F, 1055.18F, 288.36F, 1066.17F)
                Dim rModelo As New Rectangle(295.0F, 1055.18F, 312.79F, 1066.17F)
                Dim rColor As New Rectangle(331.0F, 1038.96F, 366.0F, 1051.33F)
                Dim rVin As New Rectangle(22.0F, 1025.96F, 113.55F, 1038.33F)
                Dim rPlacas As New Rectangle(331.0F, 1025.96F, 362.02F, 1038.33F)

                ' ------- LECTURA DE CADA REGIÓN -------
                txtNumeroReporte.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rNumeroReporte))
                txtNombreCliente.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rNombreCliente))
                txtTelefono.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rTelefono))
                txtEmail.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rEmail))
                txtMarca.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rMarca))
                txtTipo.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rTipo))
                txtModelo.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rModelo))
                txtColor.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rColor))
                txtVin.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rVin))
                txtPlacas.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rPlacas))
            End Using
        Catch ex As Exception
            ' Manejo de error silencioso o puedes agregar un Label para mostrar el mensaje
        End Try
    End Sub

    ''' <summary>
    ''' Extrae el texto de una región específica usando iTextSharp.
    ''' </summary>
    Private Function ExtraerTextoRegion(reader As PdfReader,
                                       numPagina As Integer,
                                       region As Rectangle) As String
        Dim filtro As New RegionTextRenderFilter(region)
        Dim estrategia As ITextExtractionStrategy =
            New FilteredTextRenderListener(New LocationTextExtractionStrategy(), filtro)

        Dim texto As String = PdfTextExtractor.GetTextFromPage(reader, numPagina, estrategia)
        Return texto
    End Function

    ''' <summary>
    ''' Limpia saltos de línea y espacios extra.
    ''' </summary>
    Private Function Limpiar(texto As String) As String
        If String.IsNullOrWhiteSpace(texto) Then
            Return ""
        End If

        texto = texto.Replace(vbCr, " ").Replace(vbLf, " ")
        While texto.Contains("  ")
            texto = texto.Replace("  ", " ")
        End While

        Return texto.Trim()
    End Function

    ' Guardado en la tabla ADMISIONES
    Protected Sub btnGuardar_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' Obtener la cadena de conexión del web.config
            Dim connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("csSetting").ConnectionString

            Using cn As New SqlConnection(connectionString)
                Const sqlInsert As String = "
            INSERT INTO dbo.Admisiones
            (
                Reporte, Asegurado, Telefono, Correo,
                Marca, Tipo, Modelo, Color, Serie, Placas
            )
            VALUES
            (
                @Reporte, @Asegurado, @Telefono, @Correo,
                @Marca, @Tipo, @Modelo, @Color, @Serie, @Placas
            );"

                Using cmd As New SqlCommand(sqlInsert, cn)
                    cmd.CommandType = CommandType.Text

                    ' Asignar parámetros desde los controles del formulario
                    cmd.Parameters.Add("@Reporte", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtNumeroReporte.Text), DBNull.Value, CType(txtNumeroReporte.Text.Trim(), Object))
                    cmd.Parameters.Add("@Asegurado", SqlDbType.NVarChar, 200).Value = If(String.IsNullOrWhiteSpace(txtNombreCliente.Text), DBNull.Value, CType(txtNombreCliente.Text.Trim(), Object))
                    cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtTelefono.Text), DBNull.Value, CType(txtTelefono.Text.Trim(), Object))
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = If(String.IsNullOrWhiteSpace(txtEmail.Text), DBNull.Value, CType(txtEmail.Text.Trim().ToLowerInvariant(), Object))

                    cmd.Parameters.Add("@Marca", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtMarca.Text), DBNull.Value, CType(txtMarca.Text.Trim(), Object))
                    cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 100).Value = If(String.IsNullOrWhiteSpace(txtTipo.Text), DBNull.Value, CType(txtTipo.Text.Trim(), Object))
                    cmd.Parameters.Add("@Modelo", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtModelo.Text), DBNull.Value, CType(txtModelo.Text.Trim(), Object))
                    cmd.Parameters.Add("@Color", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtColor.Text), DBNull.Value, CType(txtColor.Text.Trim(), Object))
                    cmd.Parameters.Add("@Serie", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtVin.Text), DBNull.Value, CType(txtVin.Text.Trim(), Object))
                    cmd.Parameters.Add("@Placas", SqlDbType.NVarChar, 20).Value = If(String.IsNullOrWhiteSpace(txtPlacas.Text), DBNull.Value, CType(txtPlacas.Text.Trim(), Object))

                    cn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Redirigir o mostrar mensaje de éxito
            Response.Redirect("AltaQua.aspx?success=1")

        Catch ex As Exception
            ' Manejo de errores - aquí puedes mostrar un mensaje al usuario
            ' Por ejemplo, agregando un Label en el .aspx y asignando el mensaje:
            ' lblError.Text = "Error al guardar: " & ex.Message
        End Try
    End Sub

End Class
