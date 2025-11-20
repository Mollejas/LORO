Imports System
Imports System.Web
Imports System.IO
Imports System.Linq
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Globalization
Imports Ionic.Zip           ' DotNetZip
Imports Ionic.Zlib          ' CompressionLevel

Public Class DownloadFotosZip
    Implements IHttpHandler

    Private Const SUBFOLDER_NAME As String = "1. DOCUMENTOS DE INGRESO"

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            Dim idStr As String = context.Request("id")
            Dim namesStr As String = context.Request("names")

            ' Validaciones iniciales (antes de cualquier cabecera)
            If String.IsNullOrWhiteSpace(idStr) OrElse Not idStr.All(AddressOf Char.IsDigit) Then
                WritePlainError(context, 400, "Solicitud inválida (id).")
                Return
            End If
            If String.IsNullOrWhiteSpace(namesStr) Then
                WritePlainError(context, 400, "No hay archivos seleccionados.")
                Return
            End If

            Dim id As Integer = Convert.ToInt32(idStr)
            Dim carpetaRel As String = ObtenerCarpetaRelPorId(id)
            If String.IsNullOrWhiteSpace(carpetaRel) Then
                WritePlainError(context, 404, "Carpeta no encontrada.")
                Return
            End If

            Dim baseFolder As String = ResolverCarpetaFisica(context, carpetaRel)
            Dim fotosFolder As String = Path.Combine(baseFolder, SUBFOLDER_NAME)
            If Not Directory.Exists(fotosFolder) Then
                WritePlainError(context, 404, "Directorio de fotos no existe.")
                Return
            End If

            ' Filtra nombres y arma rutas existentes ANTES de enviar cabeceras
            Dim names = namesStr.Split("|"c).
                                Select(Function(s) (If(s, "").Trim())).
                                Where(Function(s) s.Length > 0).
                                Distinct(StringComparer.OrdinalIgnoreCase).
                                ToList()

            If names.Count = 0 Then
                WritePlainError(context, 400, "No hay archivos válidos.")
                Return
            End If

            Dim filesToZip As New List(Of String)
            For Each n In names
                Dim safeName As String = Path.GetFileName(n)
                Dim src As String = Path.Combine(fotosFolder, safeName)
                If File.Exists(src) Then filesToZip.Add(src)
            Next

            If filesToZip.Count = 0 Then
                WritePlainError(context, 404, "Ninguno de los archivos seleccionados existe en el servidor.")
                Return
            End If

            ' Construye el ZIP en memoria primero
            Dim zipLabel As String = ConstruirEtiquetaDesdeCarpetaRel(carpetaRel)
            Dim zipName As String = SanearNombreArchivo(zipLabel) & ".zip"

            Using ms As New MemoryStream()
                Using zip As New ZipFile()
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary
                    zip.CompressionLevel = CompressionLevel.BestSpeed

                    For Each f In filesToZip
                        zip.AddFile(f, "") ' raíz del zip
                    Next

                    zip.Save(ms)
                End Using

                ' Ahora sí, enviar cabeceras + cuerpo en una sola vez
                context.Response.Clear()
                ' BufferOutput por defecto = True (mejor dejarlo así)
                context.Response.ContentType = "application/zip"
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" & zipName)
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
                context.Response.Cache.SetNoStore()

                ms.Position = 0
                ms.CopyTo(context.Response.OutputStream)
                ' Opcionalmente:
                ' context.Response.Flush()
            End Using

        Catch ex As Exception
            ' Si llegamos aquí sin haber enviado cabeceras de ZIP, devolvemos texto plano con 500.
            ' Si por algún cambio futuro se enviaran cabeceras antes del fallo,
            ' el buffer (al estar activado) evitará mezclar tipos.
            WritePlainError(context, 500, "Error al generar ZIP: " & ex.Message)
        End Try
    End Sub

    Private Sub WritePlainError(ctx As HttpContext, code As Integer, msg As String)
        ctx.Response.Clear()
        ctx.Response.StatusCode = code
        ctx.Response.ContentType = "text/plain; charset=utf-8"
        ctx.Response.Write(msg)
        ' ctx.Response.End()  ' no es necesario; Return del método basta
    End Sub

    ' === NOMBRE COMPLETO desde "EXP" hasta fin de segmento ===
    Private Function ConstruirEtiquetaDesdeCarpetaRel(carpetaRel As String) As String
        If String.IsNullOrWhiteSpace(carpetaRel) Then Return "EXP_FOTOS"
        Dim norm As String = carpetaRel.Replace("\", "/")
        Dim idx As Integer = CultureInfo.InvariantCulture.CompareInfo.IndexOf(norm, "exp", CompareOptions.IgnoreCase)
        Dim etiqueta As String = Nothing

        If idx >= 0 Then
            Dim rest As String = norm.Substring(idx)
            Dim cut As Integer = rest.IndexOf("/"c)
            etiqueta = If(cut >= 0, rest.Substring(0, cut), rest)
        Else
            etiqueta = norm.Split("/"c).LastOrDefault()
            If String.IsNullOrWhiteSpace(etiqueta) Then etiqueta = "EXP_FOTOS"
        End If

        etiqueta = etiqueta.Trim()
        If etiqueta.Length = 0 Then etiqueta = "EXP_FOTOS"
        Return etiqueta
    End Function

    Private Function SanearNombreArchivo(nombre As String) As String
        Dim invalid = Path.GetInvalidFileNameChars()
        Dim sb As New System.Text.StringBuilder(nombre.Length)
        For Each ch In nombre
            If invalid.Contains(ch) Then
                sb.Append("_"c)
            Else
                sb.Append(ch)
            End If
        Next
        Dim out As String = sb.ToString().Trim().Trim("."c)
        If String.IsNullOrWhiteSpace(out) Then out = "EXP_FOTOS"
        Return out
    End Function

    Private Function ObtenerCarpetaRelPorId(id As Integer) As String
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing Then Return Nothing
        Using cn As New SqlConnection(cs.ConnectionString)
            cn.Open()
            Using cmd As New SqlCommand("SELECT carpetarel FROM admisiones WHERE Id=@Id", cn)
                cmd.Parameters.AddWithValue("@Id", id)
                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse obj Is DBNull.Value Then Return Nothing
                Return Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture).Trim()
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
