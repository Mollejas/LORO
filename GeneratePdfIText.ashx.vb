

Imports System
Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Web.Script.Serialization

Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports iTextSharp.tool.xml
Imports iTextSharp.tool.xml.pipeline.css
Imports iTextSharp.tool.xml.pipeline.html
Imports iTextSharp.tool.xml.pipeline.end
Imports iTextSharp.tool.xml.parser
Imports iTextSharp.tool.xml.html
Imports iTextSharp.tool.xml.css

Public Class GeneratePdfIText
    Implements IHttpHandler

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            context.Response.ContentType = "application/pdf"

            ' Leer JSON { html: "...", id: "opcional" }
            Dim body As String
            Using reader As New StreamReader(context.Request.InputStream, Encoding.UTF8)
                body = reader.ReadToEnd()
            End Using

            Dim payload = New JavaScriptSerializer().Deserialize(Of ReqPayload)(body)
            Dim html As String = If(payload IsNot Nothing, payload.html, Nothing)
            If String.IsNullOrWhiteSpace(html) Then Throw New Exception("No se recibió HTML.")

            Dim expedienteId As String = If(payload IsNot Nothing, payload.id, Nothing)

            ' (A) MemoryStream instanciado
            Dim ms As New MemoryStream()

            ' (B) Document de iTextSharp explícito
            Using doc As New iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 28.0F, 28.0F, 28.0F, 28.0F)
                ' (C) Writer a partir de doc + ms
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)
                writer.CloseStream = False
                doc.Open()

                ' ===== XMLWorker: CSS + HTML =====
                Dim cssResolver As New StyleAttrCSSResolver()

                Dim cssPath = context.Server.MapPath("~/css1.css")
                If File.Exists(cssPath) Then
                    Using fsCss As New FileStream(cssPath, FileMode.Open, FileAccess.Read)
                        Dim cssFile = XMLWorkerHelper.GetCSS(fsCss)
                        cssResolver.AddCss(cssFile)
                    End Using
                End If

                Dim fontProvider As New XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS)
                fontProvider.RegisterDirectories()
                Dim cssAppliers As New CssAppliersImpl(fontProvider)

                Dim htmlContext As New HtmlPipelineContext(cssAppliers)
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory())

                Dim baseUrl = GetBaseUrl(context)
                htmlContext.SetImageProvider(New BaseUriImageProvider(baseUrl))

                Dim pipeline As IPipeline =
                    New CssResolverPipeline(cssResolver,
                        New HtmlPipeline(htmlContext,
                            New PdfWriterPipeline(doc, writer)))

                Dim worker As New XMLWorker(pipeline, True)
                Dim parser As New XMLParser(True, worker, Encoding.UTF8)

                Using sr As New StringReader(html)
                    parser.Parse(sr)
                End Using

                doc.Close()
            End Using

            ' Guardado opcional
            If Not String.IsNullOrWhiteSpace(expedienteId) Then
                Dim safeId = MakeSafe(expedienteId)
                Dim dirExp = context.Server.MapPath("~/App_Data/Expedientes/" & safeId)
                If Not Directory.Exists(dirExp) Then Directory.CreateDirectory(dirExp)
                File.WriteAllBytes(Path.Combine(dirExp, "INV.pdf"), ms.ToArray())
            End If

            ' Descarga
            ms.Position = 0
            context.Response.AddHeader("Content-Disposition", "attachment; filename=inspeccion_itext_" &
                                       DateTime.Now.ToString("yyyy-MM-dd") & ".pdf")
            context.Response.BinaryWrite(ms.ToArray())
            context.Response.End()

        Catch ex As Exception
            context.Response.ContentType = "text/plain; charset=utf-8"
            context.Response.StatusCode = 500
            context.Response.Write("Error generando PDF iTextSharp: " & ex.Message & vbCrLf &
                                   "Stack: " & ex.StackTrace)
        End Try
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Private Shared Function GetBaseUrl(ctx As HttpContext) As String
        Dim app = (If(ctx.Request.ApplicationPath, String.Empty)).TrimEnd("/"c)
        If app = "/" Then app = ""
        Return ctx.Request.Url.Scheme & "://" & ctx.Request.Url.Authority & If(app = "", "/", app & "/")
    End Function

    Private Shared Function MakeSafe(input As String) As String
        Dim sb As New StringBuilder()
        For Each ch In input
            If Char.IsLetterOrDigit(ch) OrElse ch = "-"c OrElse ch = "_"c Then sb.Append(ch)
        Next
        If sb.Length = 0 Then sb.Append("expediente")
        Return sb.ToString()
    End Function

    Private Class ReqPayload
        Public Property html As String
        Public Property id As String
    End Class

    Private Class BaseUriImageProvider
        Inherits iTextSharp.tool.xml.pipeline.html.AbstractImageProvider
        Private ReadOnly _base As String
        Public Sub New(baseUri As String)
            _base = baseUri

        End Sub
        Public Overrides Function GetImageRootPath() As String
            Return _base

        End Function
    End Class
End Class
