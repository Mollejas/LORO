Imports System
Imports System.Web
Imports System.IO
Imports System.Globalization
Imports System.Collections.Generic
Imports System.Web.SessionState
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Linq
Imports System.Text.RegularExpressions

' NOTA: si también importas System.Text, no pasa nada; usamos Encoder totalmente calificado:
' System.Drawing.Imaging.Encoder.Quality

Public Class RefacFotosUpload
    Implements IHttpHandler, IRequiresSessionState

    ' ===== Config =====
    Private Const MAX_EDGE As Integer = 1600   ' Máx. lado px
    Private Const JPEG_QUALITY As Long = 88    ' 1..100

    ' Carpeta de diagnóstico por área
    Private Const SUBFOLDER_DIAG_MEC As String = "2. FOTOS DIAGNOSTICO MECANICA"
    Private Const SUBFOLDER_DIAG_HOJ As String = "3. FOTOS DIAGNOSTICO HOJALATERIA"

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json; charset=utf-8"
        Dim json As String

        Try
            ' ------ Params ------
            Dim itemId As Integer = SafeInt(context.Request("itemId"))
            Dim admisionId As Integer = SafeInt(context.Request("admisionId"))
            Dim area As String = Nz(context.Request("area")).Trim().ToUpperInvariant()
            Dim descripcion As String = Nz(context.Request("descripcion")).Trim()
            Dim carpetaRel As String = Nz(context.Request("carpetaRel")).Trim()
            Dim filePrefixRaw As String = Nz(context.Request("filePrefix")).Trim()

            If itemId <= 0 OrElse admisionId <= 0 Then
                Throw New ApplicationException("Parámetros inválidos: itemId/admisionId requeridos.")
            End If

            If String.IsNullOrEmpty(area) Then area = "MECANICA"
            If area <> "MECANICA" AndAlso area <> "HOJALATERIA" Then area = "MECANICA"

            ' Si no mandan filePrefix, lo derivamos de la descripción (o fallback "FOTO")
            Dim filePrefix As String = If(filePrefixRaw <> "", filePrefixRaw, PrefijoDesdeDescripcion(descripcion))

            ' ------ Archivos ------
            If context.Request.Files Is Nothing OrElse context.Request.Files.Count = 0 Then
                Throw New ApplicationException("No se recibieron archivos.")
            End If

            ' ------ Destino (carpeta del expediente que *ya trae* ~ si así está en BD) ------
            Dim baseExpFisico As String = ResolverCarpetaFisica(carpetaRel)
            Dim subFolder As String = If(area = "MECANICA", SUBFOLDER_DIAG_MEC, SUBFOLDER_DIAG_HOJ)
            Dim physFolder As String = Path.Combine(baseExpFisico, subFolder)
            Directory.CreateDirectory(physFolder)

            ' ------ Guardar ------
            Dim saved As New List(Of String)()
            Dim nowPrefix As String = DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)

            ' Encontrar siguiente índice según prefijo (NN de 2 dígitos)
            Dim startIdx As Integer = ObtenerSiguienteIndiceDiag(physFolder, filePrefix)

            For i As Integer = 0 To context.Request.Files.Count - 1
                Dim posted As HttpPostedFile = context.Request.Files(i)
                If posted Is Nothing OrElse posted.ContentLength <= 0 Then Continue For
                If Not IsImage(posted.FileName, posted.ContentType) Then Continue For

                Using st As Stream = posted.InputStream
                    Using img As Image = Image.FromStream(st, useEmbeddedColorManagement:=True, validateImageData:=True)
                        Using re As Image = ResizeMax(img, MAX_EDGE, MAX_EDGE)
                            Dim idx As Integer = startIdx + i
                            Dim name As String = $"{filePrefix}-{idx:00}.jpg"
                            Dim physPath As String = Path.Combine(physFolder, name)
                            SaveJpeg(re, physPath, JPEG_QUALITY)

                            ' Devolver ruta web relativa (respecta ~ si viene en BD)
                            Dim relWeb As String = CombineWebPath(NormalizeTildePrefix(carpetaRel), subFolder, name)
                            saved.Add(VirtualPathUtility.ToAbsolute(relWeb))
                        End Using
                    End Using
                End Using
            Next

            If saved.Count = 0 Then
                Throw New ApplicationException("No se pudo guardar ninguna imagen válida.")
            End If

            json = BuildJsonOk(saved.Count, saved)
        Catch ex As Exception
            json = BuildJsonError(ex.Message)
        End Try

        context.Response.Write(json)
    End Sub

    ' ======= Utilidades =======

    Private Function Nz(s As String) As String
        If s Is Nothing Then Return String.Empty
        Return s
    End Function

    Private Function SafeInt(s As String) As Integer
        Dim v As Integer
        If Integer.TryParse(Nz(s).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, v) Then
            Return v
        End If
        Return 0
    End Function

    Private Function IsImage(fileName As String, contentType As String) As Boolean
        Dim ext As String = Path.GetExtension(Nz(fileName)).ToLowerInvariant()
        If ext = ".jpg" OrElse ext = ".jpeg" OrElse ext = ".png" OrElse ext = ".bmp" OrElse ext = ".gif" OrElse ext = ".webp" Then
            Return True
        End If
        If Not String.IsNullOrEmpty(contentType) AndAlso contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If
        Return False
    End Function

    Private Function ResizeMax(src As Image, maxW As Integer, maxH As Integer) As Image
        Dim w As Integer = src.Width
        Dim h As Integer = src.Height
        Dim ratio As Double = Math.Min(maxW / Math.Max(1.0, w), maxH / Math.Max(1.0, h))
        If ratio >= 1.0 Then
            Return CType(src.Clone(), Image) ' no upscaling
        End If
        Dim nw As Integer = Math.Max(1, CInt(Math.Round(w * ratio)))
        Dim nh As Integer = Math.Max(1, CInt(Math.Round(h * ratio)))
        Dim bmp As New Bitmap(nw, nh)
        bmp.SetResolution(src.HorizontalResolution, src.VerticalResolution)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.CompositingMode = Drawing2D.CompositingMode.SourceOver
            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            Dim dest As New Rectangle(0, 0, nw, nh)
            g.DrawImage(src, dest, 0, 0, w, h, GraphicsUnit.Pixel)
        End Using
        Return bmp
    End Function

    Private Sub SaveJpeg(img As Image, path As String, quality As Long)
        Dim enc As ImageCodecInfo = ImageCodecInfo.GetImageDecoders().First(Function(c) c.FormatID = ImageFormat.Jpeg.Guid)
        Dim encParams As New EncoderParameters(1)
        ' <<< Aquí el uso TOTALMENTE CALIFICADO para evitar ambigüedad con System.Text.Encoder >>>
        encParams.Param(0) = New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Math.Max(1, Math.Min(100, quality)))
        img.Save(path, enc, encParams)
    End Sub

    ' Si BD guarda "carpetarel" sin "~", la agregamos
    Private Function NormalizeTildePrefix(rel As String) As String
        Dim p As String = Nz(rel).Trim()
        If String.IsNullOrEmpty(p) Then Return "~"
        If p.StartsWith("~") Then Return p
        If p.StartsWith("/") OrElse p.StartsWith("\") Then
            Return "~" & p
        End If
        Return "~/" & p
    End Function

    ' Combina partes en una ruta virtual web (con "/")
    Private Function CombineWebPath(ParamArray parts() As String) As String
        Dim segs As New List(Of String)()
        For Each p In parts
            If String.IsNullOrWhiteSpace(p) Then Continue For
            Dim s = p.Replace("\", "/").Trim("/"c)
            If s = "~" Then
                segs.Clear()
                segs.Add("~")
            Else
                segs.Add(s)
            End If
        Next
        If segs.Count = 0 Then Return "~"
        If segs(0) = "~" Then
            Return "~/" & String.Join("/", segs.Skip(1))
        End If
        Return "~/" & String.Join("/", segs)
    End Function

    ' Mapea carpetaRel (~ o relativa) a ruta física
    Private Function ResolverCarpetaFisica(carpetaRel As String) As String
        Dim p As String = Nz(carpetaRel).Trim()
        If String.IsNullOrEmpty(p) Then Throw New Exception("carpetarel vacío.")
        If Path.IsPathRooted(p) Then Return p
        If p.StartsWith("~") OrElse p.StartsWith("/") OrElse p.StartsWith("\") Then
            Return HttpContext.Current.Server.MapPath(p)
        End If
        Return HttpContext.Current.Server.MapPath("~/" & p.TrimStart("/"c, "\"c))
    End Function

    ' Prefijo desde descripción: primeros 5 dígitos; si no, primeros 5 alfanum; si no, "FOTO"
    Private Function PrefijoDesdeDescripcion(descripcion As String) As String
        Dim d = New String(Nz(descripcion).Where(Function(ch) Char.IsDigit(ch)).Take(5).ToArray())
        If d.Length > 0 Then Return d
        Dim an = New String(Nz(descripcion).Where(Function(ch) Char.IsLetterOrDigit(ch)).Take(5).ToArray())
        If an.Length > 0 Then Return an.ToUpperInvariant()
        Return "FOTO"
    End Function

    ' Busca el siguiente índice libre para prefijo-NN.jpg
    Private Function ObtenerSiguienteIndiceDiag(folder As String, prefijo As String) As Integer
        If Not Directory.Exists(folder) Then Return 1
        Dim maxN As Integer = 0
        Dim rx As New Regex("^" & Regex.Escape(prefijo) & "-(\d{2})\.jpg$", RegexOptions.IgnoreCase)
        For Each f In Directory.GetFiles(folder, prefijo & "-*.jpg", SearchOption.TopDirectoryOnly)
            Dim name = Path.GetFileName(f)
            Dim m = rx.Match(name)
            If m.Success Then
                Dim n As Integer
                If Integer.TryParse(m.Groups(1).Value, n) AndAlso n > maxN Then maxN = n
            End If
        Next
        Return maxN + 1
    End Function

    ' --- JSON helpers ---
    Private Function BuildJsonOk(count As Integer, files As List(Of String)) As String
        Dim sb As New System.Text.StringBuilder()
        sb.Append("{""ok"":true,""count"":").Append(count).Append(",""files"":[")
        For i As Integer = 0 To files.Count - 1
            If i > 0 Then sb.Append(","c)
            sb.Append("""").Append(JsonEscape(files(i))).Append("""")
        Next
        sb.Append("]}")
        Return sb.ToString()
    End Function

    Private Function BuildJsonError(msg As String) As String
        Return "{""ok"":false,""msg"":""" & JsonEscape(msg) & """}"
    End Function

    Private Function JsonEscape(s As String) As String
        If s Is Nothing Then Return ""
        Return s.Replace("\""", "\""").Replace("\", "\\").Replace(vbCr, "\r").Replace(vbLf, "\n")
    End Function

End Class
