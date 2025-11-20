
Imports System
Imports System.Web
Imports System.IO
Imports System.Linq
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports System.Web.Script.Serialization

Public Class UploadPrincipal : Implements IHttpHandler

    Private Const MAX_SIDE As Integer = 1600
    Private Const JPEG_QUALITY As Long = 88
    Private Const SUBFOLDER_NAME As String = "1. DOCUMENTOS DE INGRESO"
    Private Const PRINCIPAL_NAME As String = "principal.jpg"

    Public Sub ProcessRequest(ctx As HttpContext) Implements IHttpHandler.ProcessRequest
        ctx.Response.ContentType = "application/json"
        Try
            If String.Equals(ctx.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) Then
                If String.Equals(ctx.Request("ping"), "1") Then
                    WriteJson(ctx, True, "ping ok (handler alcanzable)")
                Else
                    WriteJson(ctx, False, "GET sin ping. Usa ping=1 o haz POST con archivo.")
                End If
                Return
            End If

            If Not String.Equals(ctx.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) Then
                ctx.Response.StatusCode = 405
                WriteJson(ctx, False, "Método no permitido (usa POST).")
                Return
            End If

            ' --- carpetaRel directo o por id ---
            Dim carpetaRel As String = Trim(Convert.ToString(ctx.Request.Form("carpetaRel")))
            If String.IsNullOrWhiteSpace(carpetaRel) Then
                Dim idStr As String = Convert.ToString(ctx.Request.Form("id"))
                Dim idNum As Integer
                If String.IsNullOrWhiteSpace(idStr) OrElse Not Integer.TryParse(idStr, idNum) Then
                    ctx.Response.StatusCode = 400
                    WriteJson(ctx, False, "Id inválido y sin carpetaRel.")
                    Return
                End If
                carpetaRel = ObtenerCarpetaRel(idNum)
                If String.IsNullOrWhiteSpace(carpetaRel) Then
                    ctx.Response.StatusCode = 404
                    WriteJson(ctx, False, "No se encontró carpetaRel para el id " & idStr)
                    Return
                End If
            End If

            ' --- archivo ---
            If ctx.Request.Files Is Nothing OrElse ctx.Request.Files.Count = 0 Then
                ctx.Response.StatusCode = 400
                WriteJson(ctx, False, "Archivo no recibido.")
                Return
            End If

            ' RENOMBRADO para no chocar con System.IO.File
            Dim up As HttpPostedFile = ctx.Request.Files(0)
            If up Is Nothing OrElse up.ContentLength <= 0 Then
                ctx.Response.StatusCode = 400
                WriteJson(ctx, False, "Archivo vacío.")
                Return
            End If

            Dim ext As String = Path.GetExtension(up.FileName).ToLowerInvariant()
            Dim permitidas = New String() {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"}
            If Not permitidas.Contains(ext) Then
                ctx.Response.StatusCode = 415
                WriteJson(ctx, False, "Formato no permitido: " & ext)
                Return
            End If

            ' --- leer bytes ---
            Dim raw() As Byte
            Using ms As New MemoryStream()
                up.InputStream.CopyTo(ms)
                raw = ms.ToArray()
            End Using

            ' --- convertir a JPG redimensionado ---
            Dim jpg() As Byte = A_Jpeg_Redimensionado(raw, MAX_SIDE, MAX_SIDE, JPEG_QUALITY)

            ' --- guardar ---
            Dim carpetaFisica As String = ResolverCarpetaFisica(carpetaRel)
            Dim subcarpeta As String = Path.Combine(carpetaFisica, SUBFOLDER_NAME)
            If Not Directory.Exists(subcarpeta) Then Directory.CreateDirectory(subcarpeta)

            Dim destino As String = Path.Combine(subcarpeta, PRINCIPAL_NAME)
            ' **CLASE TOTALMENTE CALIFICADA** para evitar sombras
            System.IO.File.WriteAllBytes(destino, jpg)

            WriteJson(ctx, True, Nothing)
        Catch ex As Exception
            ctx.Response.StatusCode = 500
            WriteJson(ctx, False, "EX: " & ex.Message)
        End Try
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Private Sub WriteJson(ctx As HttpContext, ok As Boolean, msg As String)
        Dim js As New JavaScriptSerializer()
        ctx.Response.Write(js.Serialize(New With {.ok = ok, .msg = msg}))
    End Sub

    Private Function ObtenerCarpetaRel(id As Integer) As String
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing Then Return ""
        Using cn As New SqlConnection(cs.ConnectionString)
            cn.Open()
            Using cmd As New SqlCommand("SELECT carpetarel FROM admisiones WHERE Id=@Id", cn)
                cmd.Parameters.AddWithValue("@Id", id)
                Dim o = cmd.ExecuteScalar()
                If o Is Nothing OrElse o Is DBNull.Value Then Return ""
                Return Convert.ToString(o).Trim()
            End Using
        End Using
    End Function

    Private Function ResolverCarpetaFisica(carpetaRel As String) As String
        Dim p As String = Convert.ToString(carpetaRel).Trim()
        If String.IsNullOrEmpty(p) Then Throw New Exception("carpetaRel vacío.")
        If Path.IsPathRooted(p) Then Return p
        If p.StartsWith("~") OrElse p.StartsWith("/") OrElse p.StartsWith("\") Then
            Return HttpContext.Current.Server.MapPath(p)
        End If
        Return HttpContext.Current.Server.MapPath("~/" & p.TrimStart("/"c, "\"c))
    End Function

    Private Function A_Jpeg_Redimensionado(inputBytes As Byte(), maxW As Integer, maxH As Integer, calidad As Long) As Byte()
        Using msIn As New MemoryStream(inputBytes)
            Using src As Image = Image.FromStream(msIn)
                Dim ratioW As Double = maxW / CDbl(src.Width)
                Dim ratioH As Double = maxH / CDbl(src.Height)
                Dim ratio As Double = Math.Min(1.0, Math.Min(ratioW, ratioH))
                Dim newW As Integer = Math.Max(1, CInt(Math.Round(src.Width * ratio)))
                Dim newH As Integer = Math.Max(1, CInt(Math.Round(src.Height * ratio)))

                Using bmp As New Bitmap(newW, newH)
                    bmp.SetResolution(96, 96)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.CompositingQuality = CompositingQuality.HighQuality
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.SmoothingMode = SmoothingMode.HighQuality
                        g.DrawImage(src, 0, 0, newW, newH)
                    End Using

                    Dim codecJpg As ImageCodecInfo = ImageCodecInfo.GetImageEncoders().First(Function(c) c.FormatID = ImageFormat.Jpeg.Guid)
                    Dim encParams As New EncoderParameters(1)
                    encParams.Param(0) = New EncoderParameter(Encoder.Quality, calidad)

                    Using msOut As New MemoryStream()
                        bmp.Save(msOut, codecJpg, encParams)
                        Return msOut.ToArray()
                    End Using
                End Using
            End Using
        End Using
    End Function
End Class
