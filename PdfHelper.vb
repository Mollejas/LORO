Imports System.Drawing
Imports System.IO
Imports PdfiumViewer

Public Class PdfHelper
    Public Shared Function ConvertirPaginaAPngDesdeStream(pdfStream As Stream, pagina As Integer, Optional dpi As Integer = 200) As Bitmap
        pdfStream.Position = 0
        Try
            Using documento = PdfDocument.Load(pdfStream)
                Return New Bitmap(documento.Render(pagina - 1, dpi, dpi, True))
            End Using
        Catch ex As Exception
            Throw New ApplicationException("Error al renderizar la página del PDF.", ex)
        End Try
    End Function
End Class
