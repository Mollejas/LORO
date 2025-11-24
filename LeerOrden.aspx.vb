Imports System
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports Path = System.IO.Path

Public Class LeerOrden
    Inherits System.Web.UI.Page
    Protected Sub btnProcesar_Click(sender As Object, e As EventArgs) Handles btnProcesar.Click
        lblMensaje.Text = ""

        If Not fuPdf.HasFile Then
            lblMensaje.Text = "Selecciona un archivo PDF."
            Return
        End If

        ' Carpeta temporal para guardar el PDF subido
        Dim rutaTmp As String = Server.MapPath("~/App_Data/Uploads")
        If Not Directory.Exists(rutaTmp) Then
            Directory.CreateDirectory(rutaTmp)
        End If

        Dim rutaArchivo As String = Path.Combine(rutaTmp, Path.GetFileName(fuPdf.FileName))
        fuPdf.SaveAs(rutaArchivo)

        Try
            Using reader As New PdfReader(rutaArchivo)
                Dim pagina As Integer = 1  ' Siempre es 1 en tu ejemplo

                ' ------- RECTÁNGULOS (coordenadas con origen abajo-izquierda) -------
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

            lblMensaje.ForeColor = Drawing.Color.Green
            lblMensaje.Text = "PDF procesado correctamente."
        Catch ex As Exception
            lblMensaje.ForeColor = Drawing.Color.Red
            lblMensaje.Text = "Error al leer el PDF: " & ex.Message
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

End Class