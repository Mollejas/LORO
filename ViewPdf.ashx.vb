Imports System
Imports System.Web
Imports System.IO
Imports System.Data.SqlClient
Imports System.Configuration


Public Class ViewPdf
        Implements IHttpHandler

        Private Const SUBFOLDER_DOCUMENTOS As String = "1. DOCUMENTOS DE INGRESO"
        Private Const SUBFOLDER_VALUACION As String = "4. VALUACION"

        Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
            Dim idStr As String = context.Request("id")
            Dim kind As String = Convert.ToString(context.Request("kind")).ToLowerInvariant()

            If String.IsNullOrWhiteSpace(idStr) OrElse Not idStr.All(AddressOf Char.IsDigit) Then
                context.Response.StatusCode = 400 : context.Response.Write("Solicitud inválida.") : Return
            End If
            If String.IsNullOrWhiteSpace(kind) Then
                context.Response.StatusCode = 400 : context.Response.Write("Falta 'kind'.") : Return
            End If

            Dim fileName As String
            Dim subFolder As String = SUBFOLDER_DOCUMENTOS
            Select Case kind
                Case "oda" : fileName = "ODA.pdf"
                Case "ine" : fileName = "INE.pdf"
                Case "ct" : fileName = "CT.pdf"
                Case "inv" : fileName = "inv.pdf"
                Case "inetransito" : fileName = "inetransito.pdf"
                Case "transitoaseg" : fileName = "transitoaseg.pdf"
                Case "comple" : fileName = "comple.pdf"
                Case "valsin" : fileName = "valsin.pdf" : subFolder = SUBFOLDER_VALUACION
                Case "valaut" : fileName = "valaut.pdf" : subFolder = SUBFOLDER_VALUACION
                Case Else
                    context.Response.StatusCode = 400 : context.Response.Write("Kind inválido.") : Return
            End Select

            Dim id As Integer = Convert.ToInt32(idStr)
            Dim carpetaRel As String = ObtenerCarpetaRelPorId(id)
            If String.IsNullOrWhiteSpace(carpetaRel) Then
                context.Response.StatusCode = 404 : context.Response.Write("Carpeta no encontrada.") : Return
            End If

            Dim baseFolder As String = ResolverCarpetaFisica(context, carpetaRel)
            Dim pdfPath As String = Path.Combine(baseFolder, subFolder, fileName)
            If Not File.Exists(pdfPath) Then
                context.Response.StatusCode = 404 : context.Response.Write("Archivo no encontrado.") : Return
            End If

            ' Preparar respuesta
            context.Response.Clear()
            context.Response.Buffer = False
            context.Response.ContentType = "application/pdf"
            ' Cache-busting: si viene v en query, cambia URL, pero igual damos ETag para rapidez
            Dim fi As New FileInfo(pdfPath)
            Dim etag As String = """" & fi.LastWriteTimeUtc.Ticks.ToString() & """"
            context.Response.AddHeader("ETag", etag)
            context.Response.AddHeader("Accept-Ranges", "bytes")
            context.Response.Cache.SetCacheability(HttpCacheability.Private)
            context.Response.Cache.SetMaxAge(TimeSpan.Zero)
            context.Response.Cache.SetNoStore()
            context.Response.AddHeader("Content-Disposition", "inline; filename=" & fileName)

            ' 304 si ETag coincide
            Dim inm As String = context.Request.Headers("If-None-Match")
            If Not String.IsNullOrEmpty(inm) AndAlso inm = etag Then
                context.Response.StatusCode = 304
                context.Response.End()
                Return
            End If

            ' Soporte de Range (parcial) para rapidez
            Dim fs As FileStream = Nothing
            Try
                fs = New FileStream(pdfPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim total As Long = fs.Length
                Dim start As Long = 0
                Dim [end] As Long = total - 1

                Dim range = context.Request.Headers("Range")
                If Not String.IsNullOrEmpty(range) AndAlso range.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase) Then
                    Dim parts = range.Substring(6).Split("-"c)
                    If parts.Length >= 1 AndAlso Not String.IsNullOrEmpty(parts(0)) Then
                        Long.TryParse(parts(0), start)
                    End If
                    If parts.Length = 2 AndAlso Not String.IsNullOrEmpty(parts(1)) Then
                        Long.TryParse(parts(1), [end])
                    End If
                    If start < 0 Then start = 0
                    If [end] >= total Then [end] = total - 1
                    Dim length As Long = [end] - start + 1

                    context.Response.StatusCode = 206
                    context.Response.AddHeader("Content-Range", $"bytes {start}-{[end]}/{total}")
                    context.Response.AddHeader("Content-Length", length.ToString())

                    fs.Position = start
                    CopiarStream(fs, context.Response.OutputStream, length)
                Else
                    context.Response.AddHeader("Content-Length", total.ToString())
                    fs.Position = 0
                    fs.CopyTo(context.Response.OutputStream)
                End If

                context.Response.Flush()
            Finally
                If fs IsNot Nothing Then fs.Dispose()
            End Try
        End Sub

        Private Sub CopiarStream(src As Stream, dst As Stream, maxBytes As Long)
            Dim buffer(8191) As Byte
            Dim remaining As Long = maxBytes
            While remaining > 0
                Dim toRead As Integer = If(remaining > buffer.Length, buffer.Length, CInt(remaining))
                Dim read = src.Read(buffer, 0, toRead)
                If read <= 0 Then Exit While
                dst.Write(buffer, 0, read)
                remaining -= read
            End While
        End Sub

        Private Function ObtenerCarpetaRelPorId(id As Integer) As String
            Dim cs As String = DatabaseHelper.GetConnectionString()
            If cs Is Nothing Then Return Nothing
            Using cn As New SqlConnection(cs)
                cn.Open()
                Using cmd As New SqlCommand("SELECT carpetarel FROM admisiones WHERE Id=@Id", cn)
                    cmd.Parameters.AddWithValue("@Id", id)
                    Dim obj = cmd.ExecuteScalar()
                    If obj Is Nothing OrElse obj Is DBNull.Value Then Return Nothing
                    Return Convert.ToString(obj).Trim()
                End Using
            End Using
        End Function

        Private Function ResolverCarpetaFisica(ctx As HttpContext, carpetaRel As String) As String
            Dim p As String = Convert.ToString(carpetaRel).Trim()
            If Path.IsPathRooted(p) Then Return p
            If p.StartsWith("~") OrElse p.StartsWith("/") OrElse p.StartsWith("\") Then
                Return ctx.Server.MapPath(p)
            End If
            Return ctx.Server.MapPath("~/" & p.TrimStart("/"c, "\"c))
        End Function

    End Class

