
Imports System
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D

Public Class ImageThumb
    Implements IHttpHandler

    ' Subcarpetas en disco (ajústalas si cambian)
    Private Const SUBFOLDER_RECEP As String = "1. DOCUMENTOS DE INGRESO"
    Private Const SUBFOLDER_PRESUP As String = "3. FOTOS DE PRESUPUESTO"

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        ' ---- Inputs ----
        Dim id As String = (context.Request("id") & "").Trim()
        Dim name As String = (context.Request("name") & "").Trim()
        Dim sizeKey As String = (context.Request("s") & "").Trim().ToLower()    ' "t" o "m"
        Dim carpetaRel As String = (context.Request("carpetaRel") & "").Trim()   ' llega desde Hoja.aspx

        If String.IsNullOrWhiteSpace(id) OrElse String.IsNullOrWhiteSpace(name) Then
            context.Response.StatusCode = 400
            context.Response.Write("Missing parameters.")
            Return
        End If

        Try
            ' ---- Resolver carpeta base física ----
            If String.IsNullOrWhiteSpace(carpetaRel) Then
                ' Fallback: si NO te pasan carpetaRel, descomenta e implementa tu lookup a DB:
                ' carpetaRel = ObtenerCarpetaRelPorId(id)
                Throw New ApplicationException("carpetaRel no proporcionada.")
            End If

            Dim carpetaBaseFisica As String = ResolverCarpetaFisica(context, carpetaRel)

            ' ---- Elegir subcarpeta según el prefijo del archivo ----
            Dim subFolder As String = If(name.StartsWith("presup", StringComparison.OrdinalIgnoreCase),
                                         SUBFOLDER_PRESUP, SUBFOLDER_RECEP)

            Dim fullPath As String = Path.Combine(carpetaBaseFisica, subFolder, name)
            If Not File.Exists(fullPath) Then
                context.Response.StatusCode = 404
                context.Response.Write("Not found.")
                Return
            End If

            ' ---- Tamaño de salida ----
            Dim maxSide As Integer = If(sizeKey = "t", 200, 1200)

            ' ---- Generar y devolver JPG ----
            context.Response.Clear()
            context.Response.ContentType = "image/jpeg"
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
            context.Response.Cache.SetNoStore()
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1))

            Using src As Image = Image.FromFile(fullPath)
                Using bmp As Bitmap = ResizeToBox(src, maxSide, maxSide)
                    Dim encJpg As ImageCodecInfo = ImageCodecInfo.GetImageEncoders().First(Function(c) c.FormatID = ImageFormat.Jpeg.Guid)
                    Using ep As New EncoderParameters(1)
                        ep.Param(0) = New EncoderParameter(Encoder.Quality, 85L)
                        Using ms As New MemoryStream()
                            bmp.Save(ms, encJpg, ep)
                            context.Response.BinaryWrite(ms.ToArray())
                        End Using
                    End Using
                End Using
            End Using

        Catch ex As Exception
            context.Response.StatusCode = 500
            context.Response.Write("Server error: " & ex.Message)
        End Try
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    ' ================= Helpers =================

    Private Function ResolverCarpetaFisica(ctx As HttpContext, carpetaRel As String) As String
        Dim p As String = (carpetaRel & "").Trim()
        If Path.IsPathRooted(p) Then Return p
        If p.StartsWith("~") OrElse p.StartsWith("/") OrElse p.StartsWith("\") Then
            Return ctx.Server.MapPath(p)
        End If
        Return ctx.Server.MapPath("~/" & p.TrimStart("/"c, "\"c))
    End Function

    Private Function ResizeToBox(src As Image, maxW As Integer, maxH As Integer) As Bitmap
        Dim ratioW As Double = maxW / CDbl(src.Width)
        Dim ratioH As Double = maxH / CDbl(src.Height)
        Dim ratio As Double = Math.Min(1.0, Math.Min(ratioW, ratioH)) ' no ampliar
        Dim w As Integer = Math.Max(1, CInt(Math.Round(src.Width * ratio)))
        Dim h As Integer = Math.Max(1, CInt(Math.Round(src.Height * ratio)))

        Dim bmp As New Bitmap(w, h)
        bmp.SetResolution(96, 96)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.CompositingQuality = CompositingQuality.HighQuality
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.DrawImage(src, 0, 0, w, h)
        End Using
        Return bmp
    End Function

    ' Si quisieras fallback a DB:
    'Private Function ObtenerCarpetaRelPorId(id As String) As String
    '    ' TODO: Consulta a tu tabla "admisiones" para obtener "carpetarel" por @Id
    '    ' Return "RUTA/RELATIVA/DESDE/DB"
    '    Throw New NotImplementedException()
    'End Function
End Class
