Imports System
Imports System.IO
Imports System.Web
Imports Ionic.Zip   ' DotNetZip


Public Class DownloadDiagFotosZip
        Implements IHttpHandler

        Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
            ' ----- Leer parámetros (sin ?. ) -----
            Dim namesRaw As String = GetFormValue(context, "names")
            If String.IsNullOrWhiteSpace(namesRaw) Then
                context.Response.StatusCode = 400
                context.Response.Write("Faltan nombres (names).")
                Return
            End If

            Dim expediente As String = GetFormValue(context, "expediente")
            If String.IsNullOrWhiteSpace(expediente) Then expediente = "exp"

            ' folder = ruta virtual base (p.ej. ~/Expedientes/EXP-001/2. FOTOS DIAGNOSTICO MECANICA)
            Dim virtualFolder As String = GetFormValue(context, "folder")

            ' ----- Resolver carpeta física (si se envió) -----
            Dim physicalFolder As String = String.Empty
            If Not String.IsNullOrWhiteSpace(virtualFolder) Then
                Dim vf As String = virtualFolder
                If vf.StartsWith("~/") Then
                    physicalFolder = context.Server.MapPath(vf)
                Else
                    ' Asegurar forma ~/...
                    If vf.StartsWith("/") Then
                        vf = "~" & vf
                    Else
                        vf = "~/" & vf.TrimStart("/"c)
                    End If
                    physicalFolder = context.Server.MapPath(vf)
                End If
            End If

            ' ----- Preparar respuesta ZIP -----
            Dim safeExp As String = MakeSafeFileName(expediente)
            Dim zipName As String = $"{safeExp}_MECANICA_{DateTime.Now:yyyyMMdd_HHmm}.zip"

            context.Response.Clear()
            context.Response.BufferOutput = False
            context.Response.ContentType = "application/zip"
            context.Response.AddHeader("Content-Disposition", $"attachment; filename=""{zipName}""")

            Dim fileNames As String() = namesRaw.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)

            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary
                zip.UseUnicodeAsNecessary = True

                For Each rawName As String In fileNames
                    If String.IsNullOrWhiteSpace(rawName) Then Continue For

                    ' Nunca confiar en separadores que formen rutas
                    Dim safeName As String = rawName.Replace("\"c, "_"c).Replace("/"c, "_"c)

                    ' Intentar armar la ruta física
                    Dim fullPath As String = safeName

                    If Not String.IsNullOrWhiteSpace(physicalFolder) Then
                        fullPath = Path.Combine(physicalFolder, safeName)
                    Else
                        ' Si viene un nombre con ruta virtual completa, mapearla
                        If rawName.StartsWith("~/") OrElse rawName.StartsWith("/") Then
                            fullPath = context.Server.MapPath(rawName)
                        Else
                            ' Último recurso: tratar desde raíz de la app (no recomendado)
                            fullPath = Path.Combine(context.Server.MapPath("~"), safeName)
                        End If
                    End If

                    If File.Exists(fullPath) Then
                        ' Agregar al zip en la RAÍZ (segundo parámetro = subcarpeta dentro del zip -> "")
                        zip.AddFile(fullPath, "")
                    End If
                Next

                zip.Save(context.Response.OutputStream)
            End Using

            context.Response.Flush()
            ' No llamar Response.End() en handlers modernos
        End Sub

        ' -------- Helpers sin null-conditional --------
        Private Function GetFormValue(ctx As HttpContext, key As String) As String
            Dim v As String = Nothing
            If ctx IsNot Nothing Then
                Dim req As HttpRequest = ctx.Request
                If req IsNot Nothing AndAlso req.Form IsNot Nothing Then
                    v = req.Form(key)
                End If
            End If
            If v Is Nothing Then v = String.Empty
            Return v.Trim()
        End Function

        Private Function MakeSafeFileName(s As String) As String
            If s Is Nothing Then Return "exp"
            Dim invalid() As Char = Path.GetInvalidFileNameChars()
            Dim chars As Char() = s.ToCharArray()
            For i As Integer = 0 To chars.Length - 1
                If Array.IndexOf(invalid, chars(i)) >= 0 Then
                    chars(i) = "_"c
                End If
            Next
            Dim r As String = New String(chars)
            If r.Length = 0 Then r = "exp"
            Return r
        End Function

    End Class

