<%@ WebHandler Language="VB" Class="UploadFotoIngreso" %>

Imports System
Imports System.Web
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D

Public Class UploadFotoIngreso
    Implements System.Web.IHttpHandler

    Private Const MAX_SIDE As Integer = 1600
    Private Const JPEG_QUALITY As Long = 88
    Private Const PREFIX_PRESUP As String = "recep"

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"

        Try
            ' Obtener parámetros
            Dim carpetaRel As String = context.Request.Form("carpetaRel")
            Dim siguienteIndice As String = context.Request.Form("siguienteIndice")

            If String.IsNullOrWhiteSpace(carpetaRel) Then
                context.Response.Write("{""ok"":false,""error"":""Carpeta no especificada""}")
                Return
            End If

            Dim indice As Integer
            If Not Integer.TryParse(siguienteIndice, indice) Then
                context.Response.Write("{""ok"":false,""error"":""Índice inválido""}")
                Return
            End If

            ' Obtener archivo
            If context.Request.Files.Count = 0 Then
                context.Response.Write("{""ok"":false,""error"":""No se recibió archivo""}")
                Return
            End If

            Dim file As HttpPostedFile = context.Request.Files(0)
            If file Is Nothing OrElse file.ContentLength <= 0 Then
                context.Response.Write("{""ok"":false,""error"":""Archivo vacío""}")
                Return
            End If

            ' Validar extensión
            Dim ext As String = Path.GetExtension(file.FileName).ToLower()
            Dim permitidas = New String() {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"}
            If Not permitidas.Contains(ext) Then
                context.Response.Write("{""ok"":false,""error"":""Formato no permitido""}")
                Return
            End If

            ' Construir ruta física
            Dim carpetaBaseFisica As String = context.Server.MapPath(carpetaRel)
            Dim subcarpeta As String = Path.Combine(carpetaBaseFisica, "1. DOCUMENTOS DE INGRESO")

            If Not Directory.Exists(subcarpeta) Then
                Directory.CreateDirectory(subcarpeta)
            End If

            ' Procesar imagen
            Using ms As New MemoryStream()
                file.InputStream.CopyTo(ms)
                Dim bytes As Byte() = ms.ToArray()
                Dim salidaJpg As Byte() = RedimensionarJpeg(bytes)

                Dim nombre As String = $"{PREFIX_PRESUP}{indice}.jpg"
                Dim rutaFinal As String = Path.Combine(subcarpeta, nombre)
                System.IO.File.WriteAllBytes(rutaFinal, salidaJpg)

                context.Response.Write("{""ok"":true,""filename"":""" & nombre & """}")
            End Using

        Catch ex As Exception
            context.Response.Write("{""ok"":false,""error"":""" & ex.Message.Replace("""", "'") & """}")
        End Try
    End Sub

    Private Function RedimensionarJpeg(bytes As Byte()) As Byte()
        Using msIn As New MemoryStream(bytes)
            Using imgOriginal As Image = Image.FromStream(msIn)
                Dim w As Integer = imgOriginal.Width
                Dim h As Integer = imgOriginal.Height

                ' Calcular nuevas dimensiones manteniendo aspect ratio
                Dim newW As Integer = w
                Dim newH As Integer = h

                If w > MAX_SIDE OrElse h > MAX_SIDE Then
                    Dim ratio As Double = Math.Min(CDbl(MAX_SIDE) / w, CDbl(MAX_SIDE) / h)
                    newW = CInt(w * ratio)
                    newH = CInt(h * ratio)
                End If

                Using bmp As New Bitmap(newW, newH)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.CompositingQuality = CompositingQuality.HighQuality
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.SmoothingMode = SmoothingMode.HighQuality
                        g.DrawImage(imgOriginal, 0, 0, newW, newH)
                    End Using

                    Using msOut As New MemoryStream()
                        Dim jpegEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
                        Dim encoderParams As New EncoderParameters(1)
                        encoderParams.Param(0) = New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, JPEG_QUALITY)
                        bmp.Save(msOut, jpegEncoder, encoderParams)
                        Return msOut.ToArray()
                    End Using
                End Using
            End Using
        End Using
    End Function

    Private Function GetEncoder(format As ImageFormat) As ImageCodecInfo
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageEncoders()
        For Each codec As ImageCodecInfo In codecs
            If codec.FormatID = format.Guid Then Return codec
        Next
        Return Nothing
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
