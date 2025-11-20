

Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Configuration

Public Class ImgCarpeta
    Implements IHttpHandler

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim req = context.Request
        Dim res = context.Response

        Dim b64 As String = req("b64")
        Dim ver As String = req("v") ' usado para vary/caché; no se necesita aquí
        Dim doCheck As Boolean = String.Equals(req("check"), "1", StringComparison.Ordinal)

        Dim physicalPath As String = Nothing

        ' 1) Decodificar token
        Try
            If Not String.IsNullOrWhiteSpace(b64) Then
                Dim bytes = HttpServerUtility.UrlTokenDecode(b64)
                If bytes IsNot Nothing Then
                    physicalPath = Encoding.UTF8.GetString(bytes)
                End If
            End If
        Catch
            physicalPath = Nothing
        End Try

        ' 2) Resolver rutas virtuales del sitio a físicas
        If Not String.IsNullOrWhiteSpace(physicalPath) Then
            Dim pp As String = physicalPath.Trim()
            If pp.StartsWith("~\", StringComparison.Ordinal) OrElse pp.StartsWith("~/", StringComparison.Ordinal) Then
                physicalPath = context.Server.MapPath(pp.Replace("\"c, "/"c))
            ElseIf pp.StartsWith("\", StringComparison.Ordinal) OrElse pp.StartsWith("/", StringComparison.Ordinal) Then
                physicalPath = context.Server.MapPath("~" & pp.Replace("\"c, "/"c))
            End If
        End If

        ' 3) Asegurar que apunte a principal.jpg dentro de "1. DOCUMENTOS DE INGRESO"
        physicalPath = EnsureImageFullPath(physicalPath)

        ' 4) Seguridad: restringir a BaseExpRoot (si está configurado) o AppRoot
        Dim baseRoot As String = ConfigurationManager.AppSettings("BaseExpRoot")
        If Not String.IsNullOrWhiteSpace(physicalPath) Then
            Try
                Dim normPath = Path.GetFullPath(physicalPath)

                Dim appRoot = Path.GetFullPath(context.Server.MapPath("~").TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) &
                              Path.DirectorySeparatorChar

                Dim insideAllowed As Boolean = False
                If Not String.IsNullOrWhiteSpace(baseRoot) Then
                    Dim normBase = Path.GetFullPath(baseRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) &
                                   Path.DirectorySeparatorChar
                    insideAllowed = normPath.StartsWith(normBase, StringComparison.OrdinalIgnoreCase) OrElse
                                    normPath.StartsWith(appRoot, StringComparison.OrdinalIgnoreCase)
                Else
                    ' Si no hay BaseExpRoot, al menos exigimos estar dentro del AppRoot
                    insideAllowed = normPath.StartsWith(appRoot, StringComparison.OrdinalIgnoreCase)
                End If

                If Not insideAllowed Then
                    If doCheck Then
                        WritePlain(res,
                                   "DENEGADO: fuera de BaseExpRoot/AppRoot" & Environment.NewLine &
                                   "BaseExpRoot: " & If(baseRoot, "<null>") & Environment.NewLine &
                                   "AppRoot:    " & appRoot & Environment.NewLine &
                                   "Path:       " & normPath)
                        Return
                    End If
                    ServePlaceholder(context)
                    Return
                End If

                physicalPath = normPath
            Catch ex As Exception
                If doCheck Then
                    WritePlain(res, "ERROR normalizando ruta: " & ex.Message)
                    Return
                End If
                ServePlaceholder(context)
                Return
            End Try
        End If

        ' 5) Modo diagnóstico
        If doCheck Then
            Dim exists As Boolean = (Not String.IsNullOrWhiteSpace(physicalPath) AndAlso File.Exists(physicalPath))
            Dim sb As New StringBuilder()
            sb.AppendLine("PATH: " & If(physicalPath, "<null>"))
            sb.AppendLine("EXISTS: " & exists.ToString())
            If exists Then
                Dim fi As New FileInfo(physicalPath)
                sb.AppendLine("SIZE: " & fi.Length.ToString() & " bytes")
                sb.AppendLine("LASTWRITEUTC: " & fi.LastWriteTimeUtc.ToString("o"))
            End If
            WritePlain(res, sb.ToString())
            Return
        End If

        ' 6) Servir archivo o placeholder
        If String.IsNullOrWhiteSpace(physicalPath) OrElse Not File.Exists(physicalPath) Then
            ServePlaceholder(context)
            Return
        End If

        Try
            Dim fi As New FileInfo(physicalPath)
            Dim last As DateTime = fi.LastWriteTimeUtc

            ' Encabezados de caché
            res.Clear()
            res.Cache.SetCacheability(HttpCacheability.Private)
            res.Cache.SetMaxAge(TimeSpan.FromDays(7))
            res.Cache.VaryByParams("b64") = True
            res.Cache.VaryByParams("v") = True
            res.Cache.SetLastModified(last)

            ' ETag fuerte basada en ticks+length
            Dim etag As String = """" & last.Ticks.ToString("x") & "-" & fi.Length.ToString("x") & """"
            res.Cache.SetETag(etag)

            ' Condicionales (If-None-Match / If-Modified-Since)
            Dim inm As String = context.Request.Headers("If-None-Match")
            If Not String.IsNullOrEmpty(inm) AndAlso String.Equals(inm.Trim(), etag, StringComparison.Ordinal) Then
                res.StatusCode = 304
                res.SuppressContent = True
                Return
            End If

            Dim ims As String = context.Request.Headers("If-Modified-Since")
            Dim imsDt As DateTime
            If String.IsNullOrEmpty(inm) AndAlso Not String.IsNullOrEmpty(ims) AndAlso DateTime.TryParse(ims, imsDt) Then
                If last <= imsDt.ToUniversalTime().AddSeconds(1) Then
                    res.StatusCode = 304
                    res.SuppressContent = True
                    Return
                End If
            End If

            ' Content-Type por extensión
            Dim mime As String = GetMimeForExtension(Path.GetExtension(physicalPath))
            res.ContentType = mime

            ' HEAD: solo encabezados
            If String.Equals(context.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase) Then
                res.StatusCode = 200
                Return
            End If

            res.TransmitFile(physicalPath)
        Catch
            ServePlaceholder(context)
        End Try
    End Sub

    ' ------------------ Helpers ------------------

    Private Shared Function NormalizePath(p As String) As String
        If p Is Nothing Then Return String.Empty
        Dim s As String = p.Trim()
        If s.Length = 0 Then Return s
        s = s.Replace("/"c, "\"c)
        s = s.Trim(" "c, """"c)
        Do While s.Contains("\\\\")
            s = s.Replace("\\\\", "\\")
        Loop
        Return s
    End Function

    Private Shared Function EnsureImageFullPath(basePath As String) As String
        Dim p As String = NormalizePath(basePath)
        If String.IsNullOrEmpty(p) Then Return p

        Dim ext As String = Path.GetExtension(p)
        If ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) _
            OrElse ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) _
            OrElse ext.Equals(".png", StringComparison.OrdinalIgnoreCase) _
            OrElse ext.Equals(".webp", StringComparison.OrdinalIgnoreCase) Then
            Return p ' ya apunta a un archivo de imagen
        End If

        ' Si ya contiene la carpeta "1. DOCUMENTOS DE INGRESO", solo agregar principal.jpg
        If p.IndexOf("1. DOCUMENTOS DE INGRESO", StringComparison.OrdinalIgnoreCase) >= 0 Then
            Return Path.Combine(p, "principal.jpg")
        End If

        ' Caso normal: CarpetaRel + "1. DOCUMENTOS DE INGRESO" + principal.jpg
        Return Path.Combine(Path.Combine(p, "1. DOCUMENTOS DE INGRESO"), "principal.jpg")
    End Function

    Private Shared Function GetMimeForExtension(ext As String) As String
        If String.IsNullOrEmpty(ext) Then Return "image/jpeg"
        Select Case ext.ToLowerInvariant()
            Case ".png" : Return "image/png"
            Case ".webp" : Return "image/webp"
            Case ".gif" : Return "image/gif"
            Case ".bmp" : Return "image/bmp"
            Case Else : Return "image/jpeg"
        End Select
    End Function

    Private Shared Sub WritePlain(res As HttpResponse, text As String)
        res.Clear()
        res.ContentType = "text/plain; charset=utf-8"
        res.Write(text)
    End Sub

    Private Shared Sub ServePlaceholder(ctx As HttpContext)
        Dim res = ctx.Response
        Dim fallback As String = ctx.Server.MapPath("~/images/no-image.png")

        res.Clear()
        res.Cache.SetCacheability(HttpCacheability.Private)
        res.Cache.SetMaxAge(TimeSpan.FromHours(1))
        res.Cache.VaryByParams("b64") = True
        res.Cache.VaryByParams("v") = True

        If File.Exists(fallback) Then
            res.ContentType = GetMimeForExtension(Path.GetExtension(fallback))
            res.TransmitFile(fallback)
        Else
            res.ContentType = "image/svg+xml"
            res.Write("<svg xmlns='http://www.w3.org/2000/svg' width='800' height='600'><rect width='100%' height='100%' fill='#e5e7eb'/><text x='50%' y='50%' dominant-baseline='middle' text-anchor='middle' font-family='Segoe UI, Arial' font-size='22' fill='#6b7280'>SIN IMAGEN</text></svg>")
        End If
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return True
        End Get
    End Property
End Class
