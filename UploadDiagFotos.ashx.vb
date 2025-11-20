

Imports System
Imports System.Web
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Public Class UploadDiagFotos : Implements IHttpHandler

    ' ====== PARÁMETROS DE COMPRESIÓN ======
    Private Const MAX_SIDE As Integer = 1600      ' Máximo ancho/alto final
    Private Const JPEG_QUALITY As Long = 88       ' 80–92 es buen rango

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Private ReadOnly Property CS As String
        Get
            Return ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json; charset=utf-8"

        Try
            ' ===== Lectura segura de campos =====
            Dim expediente As String = SafeForm(context, "expediente")
            Dim descripcion As String = SafeForm(context, "descripcion")
            Dim refIdStr As String = SafeForm(context, "refId")
            Dim areaForm As String = SafeForm(context, "area") ' opcional

            If String.IsNullOrWhiteSpace(expediente) Then
                WriteJson(context, False, "Expediente vacío.") : Return
            End If

            Dim refId As Integer = 0
            Integer.TryParse(refIdStr, refId)
            If refId <= 0 Then
                WriteJson(context, False, "refId inválido.") : Return
            End If

            ' === Determinar subcarpeta destino según área ===
            Dim areaDet As String = Nothing

            ' 1) Si viene en el form, úsalo
            If Not String.IsNullOrWhiteSpace(areaForm) Then
                areaDet = areaForm.Trim().ToUpperInvariant()
            End If

            ' 2) Si no, intenta inferir por refId en dbo.Refacciones
            If String.IsNullOrWhiteSpace(areaDet) AndAlso refId > 0 Then
                areaDet = GetAreaByRefId(refId)
            End If

            ' 3) Mapea área -> subcarpeta (fallback MECANICA)
            Dim subFolderName As String = MapAreaToSubfolder(areaDet)

            ' === Resolver ruta física base (CarpetaRel de Admisiones) ===
            Dim carpetaRel As String = GetCarpetaRel(expediente)
            If String.IsNullOrWhiteSpace(carpetaRel) Then
                WriteJson(context, False, "No se encontró CarpetaRel para el expediente.") : Return
            End If

            Dim targetAbs As String = ResolveAbsolutePath(context, carpetaRel, subFolderName)
            If String.IsNullOrWhiteSpace(targetAbs) Then
                WriteJson(context, False, "No fue posible resolver la ruta física de destino.") : Return
            End If
            If Not Directory.Exists(targetAbs) Then
                Directory.CreateDirectory(targetAbs)
            End If

            ' === Prefijo combinado: {Id}-{Safe5} (idéntico a tu página) ===
            Dim combinedPrefix As String = BuildRowPrefix(refId, descripcion)

            Dim saved As New List(Of String)()
            Dim total As Integer = If(context.Request.Files IsNot Nothing, context.Request.Files.Count, 0)

            For i As Integer = 0 To total - 1
                Dim file As HttpPostedFile = context.Request.Files(i)
                If file Is Nothing OrElse file.ContentLength <= 0 Then Continue For

                ' Siguiente consecutivo de 3 dígitos basado en archivos existentes con el prefijo combinado
                Dim nextIndex As Integer = GetNextConsecutive3Digits(targetAbs, combinedPrefix)

                ' Nombre final forzado a .jpg => {Id}-{Safe5}{###}.jpg
                Dim saveName As String = $"{combinedPrefix}{nextIndex:000}.jpg"
                saveName = SanitizeFileName(saveName)
                Dim savePath As String = Path.Combine(targetAbs, saveName)

                ' === Compresión/Optimización ===
                Using inStream As Stream = file.InputStream
                    SaveOptimizedJpeg(inStream, savePath, MAX_SIDE, JPEG_QUALITY)
                End Using

                saved.Add(saveName)
            Next

            If saved.Count = 0 Then
                WriteJson(context, False, "No se recibieron imágenes válidas") : Return
            End If

            WriteJson(context, True, "OK", saved)

        Catch ex As Exception
            WriteJson(context, False, "Error: " & ex.Message)
        End Try
    End Sub

    ' -------- Determinar área / subcarpeta --------

    Private Function GetAreaByRefId(id As Integer) As String
        Try
            Using cn As New SqlConnection(CS)
                Using cmd As New SqlCommand("SELECT TOP 1 Area FROM dbo.Refacciones WHERE Id=@Id;", cn)
                    cmd.Parameters.AddWithValue("@Id", id)
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Return Convert.ToString(obj).Trim().ToUpperInvariant()
                    End If
                End Using
            End Using
        Catch
            ' ignorar, usamos fallback
        End Try
        Return Nothing
    End Function

    Private Function MapAreaToSubfolder(area As String) As String
        If String.Equals(area, "HOJALATERIA", StringComparison.OrdinalIgnoreCase) Then
            Return "3. FOTOS DIAGNOSTICO HOJALATERIA"
        End If
        ' Default/fallback: mecánica
        Return "2. FOTOS DIAGNOSTICO MECANICA"
    End Function

    ' -------- Helpers de negocio --------

    Private Function GetCarpetaRel(expediente As String) As String
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                SELECT TOP 1 CarpetaRel
                FROM dbo.Admisiones
                WHERE Expediente = @Exp", cn)
                cmd.Parameters.AddWithValue("@Exp", expediente)
                cn.Open()
                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    Return Convert.ToString(obj).Trim()
                End If
            End Using
        End Using
        Return Nothing
    End Function

    Private Function ResolveAbsolutePath(ctx As HttpContext, carpetaRel As String, subFolder As String) As String
        Dim basePath As String = carpetaRel
        If Not String.IsNullOrEmpty(basePath) Then basePath = basePath.Replace("/", "\").Trim()

        If Not String.IsNullOrEmpty(basePath) AndAlso Path.IsPathRooted(basePath) Then
            Return Path.Combine(basePath, subFolder)
        End If

        Dim v As String = If(String.IsNullOrEmpty(basePath), "", basePath.TrimStart("~"c, "/"c, "\"c))
        Dim virtualPath As String = If(String.IsNullOrEmpty(v),
                                       "~/" & subFolder,
                                       "~/" & v.Replace("\", "/") & "/" & subFolder.Replace("\", "/"))
        Return ctx.Server.MapPath(virtualPath)
    End Function

    ' ===== Prefijos (en sintonía con tu página) =====
    Private Function BuildSafePrefix(desc As String) As String
        Dim raw As String = If(desc, "").Trim()
        If raw.Length > 5 Then raw = raw.Substring(0, 5)
        raw = Regex.Replace(raw, "[^A-Za-z0-9]", "")
        If String.IsNullOrWhiteSpace(raw) Then raw = "REF"
        Return raw
    End Function

    Private Function BuildRowPrefix(id As Integer, desc As String) As String
        Return $"{id}-{BuildSafePrefix(desc)}"
    End Function

    ' ===== Consecutivo de 3 dígitos para {Id}-{Safe5}XXX.jpg =====
    Private Function GetNextConsecutive3Digits(folderAbs As String, combinedPrefix As String) As Integer
        Dim seq As Integer = 1
        If Not Directory.Exists(folderAbs) Then Return seq

        Dim re As New Regex("^" & Regex.Escape(combinedPrefix) & "(?<num>\d{3})(?:_.+)?\.jpg$", RegexOptions.IgnoreCase)

        For Each f In Directory.GetFiles(folderAbs, "*.jpg", SearchOption.TopDirectoryOnly)
            Dim name = Path.GetFileName(f)
            Dim m = re.Match(name)
            If m.Success Then
                Dim n As Integer
                If Integer.TryParse(m.Groups("num").Value, n) AndAlso n >= seq Then
                    seq = n + 1
                End If
            End If
        Next
        Return seq
    End Function

    ' -------- Helpers utilitarios --------

    Private Function SafeForm(ctx As HttpContext, key As String) As String
        Dim v As String = Nothing
        If ctx.Request.Form IsNot Nothing Then v = ctx.Request.Form(key)
        If v Is Nothing Then v = ""
        Return v.Trim()
    End Function

    Private Function SanitizeFileName(name As String) As String
        Dim invalid As Char() = Path.GetInvalidFileNameChars()
        For Each ch In invalid
            name = name.Replace(ch, "_"c)
        Next
        Return name
    End Function

    Private Sub WriteJson(ctx As HttpContext, ok As Boolean, msg As String, Optional files As List(Of String) = Nothing)
        Dim sb As New StringBuilder()
        sb.Append("{""ok"":").Append(If(ok, "true", "false")).Append(",""msg"":")
        sb.Append(""""c).Append(msg.Replace("""", "\""")).Append("""")
        If files IsNot Nothing Then
            sb.Append(",""files"": [")
            For i = 0 To files.Count - 1
                If i > 0 Then sb.Append(","c)
                sb.Append(""""c).Append(files(i).Replace("""", "\""")).Append("""")
            Next
            sb.Append("]")
        End If
        sb.Append("}")
        ctx.Response.Write(sb.ToString())
    End Sub

    ' -------- Compresión / Optimización de imagen --------

    Private Sub SaveOptimizedJpeg(input As Stream, destPath As String, maxSide As Integer, quality As Long)
        Using src As Image = Image.FromStream(input)
            Dim w As Integer = src.Width
            Dim h As Integer = src.Height

            Dim dw As Integer = w
            Dim dh As Integer = h
            If w > maxSide OrElse h > maxSide Then
                Dim ratio As Double = Math.Min(maxSide / CDbl(w), maxSide / CDbl(h))
                dw = CInt(Math.Round(w * ratio))
                dh = CInt(Math.Round(h * ratio))
            End If

            Using canvas As New Bitmap(dw, dh)
                canvas.SetResolution(96.0F, 96.0F)

                Using g As Graphics = Graphics.FromImage(canvas)
                    g.SmoothingMode = SmoothingMode.HighQuality
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.CompositingQuality = CompositingQuality.HighQuality
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality

                    g.DrawImage(src, New Rectangle(0, 0, dw, dh), New Rectangle(0, 0, w, h), GraphicsUnit.Pixel)
                End Using

                Dim jpgEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
                If jpgEncoder Is Nothing Then
                    canvas.Save(destPath, ImageFormat.Jpeg)
                Else
                    Using encParams As New EncoderParameters(1)
                        encParams.Param(0) = New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality)
                        canvas.Save(destPath, jpgEncoder, encParams)
                    End Using
                End If
            End Using
        End Using
    End Sub

    Private Function GetEncoder(format As ImageFormat) As ImageCodecInfo
        Dim codecs = ImageCodecInfo.GetImageDecoders()
        For Each c In codecs
            If c.FormatID = format.Guid Then Return c
        Next
        Return Nothing
    End Function

End Class
