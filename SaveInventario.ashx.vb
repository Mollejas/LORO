

Imports System
Imports System.Web
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Public Class SaveInventario : Implements IHttpHandler

    Private Const SUBFOLDER_NAME As String = "1. DOCUMENTOS DE INGRESO"
    Private Const OUTPUT_FILE As String = "INV.pdf"

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"

        Try
            Dim idStr As String = context.Request("id")
            If String.IsNullOrWhiteSpace(idStr) OrElse Not idStr.All(AddressOf Char.IsDigit) Then
                Throw New Exception("Parámetro 'id' inválido.")
            End If
            Dim id As Integer = Convert.ToInt32(idStr)

            Dim dataUrl As String = context.Request("pdfDataUrl")
            If String.IsNullOrWhiteSpace(dataUrl) Then
                Throw New Exception("No llegó el PDF (pdfDataUrl).")
            End If

            Dim pdfBytes As Byte() = DataUrlToBytes(dataUrl)
            If pdfBytes Is Nothing OrElse pdfBytes.Length = 0 Then
                Throw New Exception("PDF vacío o corrupto.")
            End If

            ' 1) Obtener carpeta relativa desde la BD
            Dim carpetaRel As String = GetCarpetaRelById(id)
            If String.IsNullOrWhiteSpace(carpetaRel) Then
                Throw New Exception("No se encontró 'carpetarel' para el id=" & id)
            End If

            ' 2) Resolver carpeta física de destino: <carpeta>/1. DOCUMENTOS DE INGRESO
            Dim basePath As String = ResolverCarpetaFisica(carpetaRel)
            Dim targetDir As String = Path.Combine(basePath, SUBFOLDER_NAME)
            If Not Directory.Exists(targetDir) Then Directory.CreateDirectory(targetDir)

            ' 3) Guardar INV.pdf
            Dim outPath As String = Path.Combine(targetDir, OUTPUT_FILE)
            File.WriteAllBytes(outPath, pdfBytes)

            context.Response.Write("{""ok"":true,""file"":""INV.pdf""}")
        Catch ex As Exception
            context.Response.StatusCode = 400
            Dim msg = ex.Message.Replace("""", "\""")
            context.Response.Write("{""ok"":false,""error"":""" & msg & """}")
        End Try
    End Sub

    Private Function GetCarpetaRelById(id As Integer) As String
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing Then Return Nothing
        Using cn As New SqlConnection(cs.ConnectionString)
            cn.Open()
            Using cmd As New SqlCommand("SELECT carpetarel FROM admisiones WHERE Id=@Id", cn)
                cmd.Parameters.AddWithValue("@Id", id)
                Dim o = cmd.ExecuteScalar()
                If o Is Nothing OrElse o Is DBNull.Value Then Return Nothing
                Return Convert.ToString(o).Trim()
            End Using
        End Using
    End Function

    Private Function ResolverCarpetaFisica(carpetaRel As String) As String
        Dim p As String = Convert.ToString(carpetaRel).Trim()
        If String.IsNullOrEmpty(p) Then Throw New Exception("carpetarel vacío.")
        If Path.IsPathRooted(p) Then Return p
        If p.StartsWith("~") OrElse p.StartsWith("/") OrElse p.StartsWith("\") Then
            Return HttpContext.Current.Server.MapPath(p)
        End If
        Return HttpContext.Current.Server.MapPath("~/" & p.TrimStart("/"c, "\"c))
    End Function

    Private Function DataUrlToBytes(dataUrl As String) As Byte()
        Dim idx As Integer = dataUrl.IndexOf("base64,", StringComparison.OrdinalIgnoreCase)
        If idx < 0 Then Return Nothing
        Dim b64 As String = dataUrl.Substring(idx + 7)
        Return Convert.FromBase64String(b64)
    End Function

End Class
