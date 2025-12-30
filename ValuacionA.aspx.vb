Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Configuration
Imports System.Text.RegularExpressions
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports iTextSharp.text
Imports System.Linq
Imports System.Web.Script.Serialization

Public Class ValuacionA
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim admId As String = Request.QueryString("id")
            If String.IsNullOrWhiteSpace(admId) Then
                lblMensaje.Text = "ID de admisión no especificado"
                lblMensaje.CssClass = "alert alert-danger d-block"
                lblMensaje.Visible = True
                Return
            End If

            CargarExpediente(admId)
            CargarRefacciones(admId)
            CargarConceptosPDF(admId)
        End If
    End Sub

    Private Sub CargarExpediente(admId As String)
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        Using cn As New SqlConnection(cs)
            Using cmd As New SqlCommand("SELECT expediente FROM admisiones WHERE id = @id", cn)
                cmd.Parameters.AddWithValue("@id", admId)
                cn.Open()
                Dim exp = cmd.ExecuteScalar()
                If exp IsNot Nothing Then
                    lblExpediente.Text = exp.ToString()
                End If
            End Using
        End Using
    End Sub

    Private Sub CargarRefacciones(admId As String)
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        Dim expediente As String = lblExpediente.Text

        Using cn As New SqlConnection(cs)
            cn.Open()

            ' Mecánica - Reparación
            Dim dtMecRep As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, observ1 FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'MECANICA' AND UPPER(categoria) = 'REPARACION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtMecRep)
                End Using
            End Using
            gvMecReparacion.DataSource = dtMecRep
            gvMecReparacion.DataBind()

            ' Mecánica - Sustitución
            Dim dtMecSus As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, numparte FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'MECANICA' AND UPPER(categoria) = 'SUSTITUCION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtMecSus)
                End Using
            End Using
            gvMecSustitucion.DataSource = dtMecSus
            gvMecSustitucion.DataBind()

            ' Hojalatería - Reparación
            Dim dtHojRep As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, observ1 FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'HOJALATERIA' AND UPPER(categoria) = 'REPARACION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtHojRep)
                End Using
            End Using
            gvHojReparacion.DataSource = dtHojRep
            gvHojReparacion.DataBind()

            ' Hojalatería - Sustitución
            Dim dtHojSus As New DataTable()
            Using cmd As New SqlCommand("SELECT id, cantidad, descripcion, numparte FROM refacciones WHERE expediente = @exp AND UPPER(area) = 'HOJALATERIA' AND UPPER(categoria) = 'SUSTITUCION' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtHojSus)
                End Using
            End Using
            gvHojSustitucion.DataSource = dtHojSus
            gvHojSustitucion.DataBind()
        End Using
    End Sub

    Private Sub CargarConceptosPDF(admId As String)
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        Dim expediente As String = lblExpediente.Text

        Using cn As New SqlConnection(cs)
            cn.Open()

            ' Cargar conceptos guardados
            Dim dtRef As New DataTable()
            Using cmd As New SqlCommand("SELECT id, concepto, importe FROM ConceptosValuacion WHERE expediente = @exp AND seccion = 'REF' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtRef)
                End Using
            End Using
            gvPDFRefacciones.DataSource = dtRef
            gvPDFRefacciones.DataBind()

            Dim dtPin As New DataTable()
            Using cmd As New SqlCommand("SELECT id, concepto, importe FROM ConceptosValuacion WHERE expediente = @exp AND seccion = 'PIN' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtPin)
                End Using
            End Using
            gvPDFPintura.DataSource = dtPin
            gvPDFPintura.DataBind()

            Dim dtHoj As New DataTable()
            Using cmd As New SqlCommand("SELECT id, concepto, importe FROM ConceptosValuacion WHERE expediente = @exp AND seccion = 'HOJ' ORDER BY id", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtHoj)
                End Using
            End Using
            gvPDFHojalateria.DataSource = dtHoj
            gvPDFHojalateria.DataBind()
        End Using
    End Sub

    Protected Sub btnProcesarPDF_Click(sender As Object, e As EventArgs)
        Dim admId As String = Request.QueryString("id")
        If String.IsNullOrWhiteSpace(admId) Then Return

        Dim expediente As String = lblExpediente.Text
        If String.IsNullOrWhiteSpace(expediente) Then Return

        ' Obtener ruta del PDF valaut.pdf
        Dim pdfPath As String = ObtenerRutaPDF(admId)
        If String.IsNullOrWhiteSpace(pdfPath) OrElse Not File.Exists(pdfPath) Then
            lblMensaje.Text = "No se encontró el archivo valaut.pdf"
            lblMensaje.CssClass = "alert alert-warning d-block"
            lblMensaje.Visible = True
            Return
        End If

        Try
            ' Extraer conceptos del PDF
            Dim lineas = LeerPdfPorLineas(pdfPath)

            Dim dtRef = CrearTabla()
            Dim dtPin = CrearTabla()
            Dim dtHoj = CrearTabla()

            ProcesarLineasPDF(lineas, dtRef, dtPin, dtHoj)

            ' Guardar en base de datos
            GuardarConceptosEnDB(expediente, dtRef, dtPin, dtHoj)

            ' Recargar grids
            CargarConceptosPDF(admId)

            lblMensaje.Text = $"Se extrajeron {dtRef.Rows.Count + dtPin.Rows.Count + dtHoj.Rows.Count} conceptos del PDF exitosamente"
            lblMensaje.CssClass = "alert alert-success d-block"
            lblMensaje.Visible = True

        Catch ex As Exception
            lblMensaje.Text = "Error al procesar PDF: " & ex.Message
            lblMensaje.CssClass = "alert alert-danger d-block"
            lblMensaje.Visible = True
        End Try
    End Sub

    Private Function ObtenerRutaPDF(admId As String) As String
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        Dim carpetaRel As String = Nothing

        Using cn As New SqlConnection(cs)
            Using cmd As New SqlCommand("SELECT carpeta FROM admisiones WHERE id = @id", cn)
                cmd.Parameters.AddWithValue("@id", admId)
                cn.Open()
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    carpetaRel = result.ToString()
                End If
            End Using
        End Using

        If String.IsNullOrWhiteSpace(carpetaRel) Then Return Nothing

        Dim basePath As String = ConfigurationManager.AppSettings("carpeta_pdfs")
        If String.IsNullOrWhiteSpace(basePath) Then basePath = "C:\ERP\PDFS"

        Dim fullPath As String = Path.Combine(basePath, carpetaRel, "4. VALUACION", "valaut.pdf")
        Return fullPath
    End Function

    Private Sub GuardarConceptosEnDB(expediente As String, dtRef As DataTable, dtPin As DataTable, dtHoj As DataTable)
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString

        Using cn As New SqlConnection(cs)
            cn.Open()

            ' Eliminar conceptos anteriores del expediente
            Using cmd As New SqlCommand("DELETE FROM ConceptosValuacion WHERE expediente = @exp", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                cmd.ExecuteNonQuery()
            End Using

            ' Insertar conceptos de Refacciones
            For Each row As DataRow In dtRef.Rows
                Using cmd As New SqlCommand("INSERT INTO ConceptosValuacion (expediente, concepto, importe, seccion) VALUES (@exp, @concepto, @importe, @seccion)", cn)
                    cmd.Parameters.AddWithValue("@exp", expediente)
                    cmd.Parameters.AddWithValue("@concepto", row("Descripcion").ToString())
                    cmd.Parameters.AddWithValue("@importe", Convert.ToDecimal(row("Monto")))
                    cmd.Parameters.AddWithValue("@seccion", "REF")
                    cmd.ExecuteNonQuery()
                End Using
            Next

            ' Insertar conceptos de Pintura
            For Each row As DataRow In dtPin.Rows
                Using cmd As New SqlCommand("INSERT INTO ConceptosValuacion (expediente, concepto, importe, seccion) VALUES (@exp, @concepto, @importe, @seccion)", cn)
                    cmd.Parameters.AddWithValue("@exp", expediente)
                    cmd.Parameters.AddWithValue("@concepto", row("Descripcion").ToString())
                    cmd.Parameters.AddWithValue("@importe", Convert.ToDecimal(row("Monto")))
                    cmd.Parameters.AddWithValue("@seccion", "PIN")
                    cmd.ExecuteNonQuery()
                End Using
            Next

            ' Insertar conceptos de Hojalatería
            For Each row As DataRow In dtHoj.Rows
                Using cmd As New SqlCommand("INSERT INTO ConceptosValuacion (expediente, concepto, importe, seccion) VALUES (@exp, @concepto, @importe, @seccion)", cn)
                    cmd.Parameters.AddWithValue("@exp", expediente)
                    cmd.Parameters.AddWithValue("@concepto", row("Descripcion").ToString())
                    cmd.Parameters.AddWithValue("@importe", Convert.ToDecimal(row("Monto")))
                    cmd.Parameters.AddWithValue("@seccion", "HOJ")
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)
        ' Obtener el JSON de relaciones desde el hidden field
        Dim jsonRelaciones As String = Request.Form("hfRelaciones")
        If String.IsNullOrWhiteSpace(jsonRelaciones) Then
            lblMensaje.Text = "No hay relaciones para guardar"
            lblMensaje.CssClass = "alert alert-warning d-block"
            lblMensaje.Visible = True
            Return
        End If

        Try
            Dim serializer As New JavaScriptSerializer()
            Dim relaciones As Dictionary(Of String, List(Of String)) = serializer.Deserialize(Of Dictionary(Of String, List(Of String)))(jsonRelaciones)

            Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
            Using cn As New SqlConnection(cs)
                cn.Open()

                ' Actualizar refaccion_id en ConceptosValuacion
                ' Primero limpiar todas las relaciones existentes
                Using cmd As New SqlCommand("UPDATE ConceptosValuacion SET refaccion_id = NULL WHERE expediente = @exp", cn)
                    cmd.Parameters.AddWithValue("@exp", lblExpediente.Text)
                    cmd.ExecuteNonQuery()
                End Using

                ' Ahora asignar las nuevas relaciones
                For Each kvp In relaciones
                    Dim refaccionId As Integer = Convert.ToInt32(kvp.Key)
                    For Each conceptoId In kvp.Value
                        Using cmd As New SqlCommand("UPDATE ConceptosValuacion SET refaccion_id = @refid WHERE id = @conceptoid", cn)
                            cmd.Parameters.AddWithValue("@refid", refaccionId)
                            cmd.Parameters.AddWithValue("@conceptoid", Convert.ToInt32(conceptoId))
                            cmd.ExecuteNonQuery()
                        End Using
                    Next
                Next
            End Using

            lblMensaje.Text = "Relaciones guardadas exitosamente"
            lblMensaje.CssClass = "alert alert-success d-block"
            lblMensaje.Visible = True

        Catch ex As Exception
            lblMensaje.Text = "Error al guardar relaciones: " & ex.Message
            lblMensaje.CssClass = "alert alert-danger d-block"
            lblMensaje.Visible = True
        End Try
    End Sub

    ' ========== FUNCIONES DE EXTRACCIÓN DE PDF (copiadas de Valuacion.aspx) ==========

    Private Function CrearTabla() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Descripcion", GetType(String))
        dt.Columns.Add("Monto", GetType(Decimal))
        Return dt
    End Function

    Private Function LeerPdfPorLineas(pdfPath As String) As List(Of String)
        Using stream As New FileStream(pdfPath, FileMode.Open, FileAccess.Read)
            Dim reader As New PdfReader(stream)
            Dim resultado As New List(Of String)()

            For p = 1 To reader.NumberOfPages
                Dim tamano = reader.GetPageSize(p)
                Dim mitadX = tamano.Width / 2

                Dim regionIzq As New Rectangle(0, 0, mitadX, tamano.Height)
                Dim regionDer As New Rectangle(mitadX, 0, tamano.Width, tamano.Height)

                resultado.AddRange(ExtraerLineasRegion(reader, p, regionIzq))
                resultado.AddRange(ExtraerLineasRegion(reader, p, regionDer))
            Next

            reader.Close()
            Return resultado
        End Using
    End Function

    Private Function ExtraerLineasRegion(reader As PdfReader, pagina As Integer, region As Rectangle) As IEnumerable(Of String)
        Dim strategy As New FilteredTextRenderListener(New LocationTextExtractionStrategy(), New RegionTextRenderFilter(region))
        Dim texto = PdfTextExtractor.GetTextFromPage(reader, pagina, strategy)

        Return texto.
            Split({vbLf, vbCr}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(l) l.Trim()).
            Where(Function(l) l <> "")
    End Function

    Private Sub ProcesarLineasPDF(lineas As List(Of String), dtRef As DataTable, dtPin As DataTable, dtHoj As DataTable)
        Dim seccionActual As String = ""
        Dim capturandoConceptos As Boolean = False
        Dim capturandoTotales As Boolean = False
        Dim descripcionPendiente As String = Nothing

        For idx = 0 To lineas.Count - 1
            Dim linea = lineas(idx)
            Dim txt = linea.Trim()
            If txt = "" Then Continue For

            Dim txtU = txt.ToUpper()

            ' Detectar secciones
            If txtU.Contains("REFACCIONES") Then
                seccionActual = "REF"
                capturandoConceptos = False
                capturandoTotales = False
                descripcionPendiente = Nothing
                Continue For
            End If

            If txtU.Contains("PINTURA") Then
                seccionActual = "PIN"
                capturandoConceptos = False
                capturandoTotales = False
                descripcionPendiente = Nothing
                Continue For
            End If

            If txtU.Contains("MANO DE OBRA HOJALATERIA") Then
                seccionActual = "HOJ"
                capturandoConceptos = False
                capturandoTotales = False
                descripcionPendiente = Nothing
                Continue For
            End If

            ' Encabezado DESCRIPCION / MONTO
            If txtU.Contains("DESCRIPCION") AndAlso txtU.Contains("MONTO") Then
                capturandoConceptos = True
                Continue For
            End If

            ' SUBTOTAL (cierra sección)
            If txtU = "SUBTOTAL" Then
                If descripcionPendiente IsNot Nothing Then
                    Dim indiceLimite = Math.Min(idx + 3, lineas.Count - 1)
                    For i = idx To indiceLimite
                        Dim lineaBusqueda = lineas(i)
                        Dim montoTemp As Decimal
                        If TryParseMonto(lineaBusqueda, montoTemp) Then
                            Select Case seccionActual
                                Case "REF" : dtRef.Rows.Add(descripcionPendiente, montoTemp)
                                Case "PIN" : dtPin.Rows.Add(descripcionPendiente, montoTemp)
                                Case "HOJ" : dtHoj.Rows.Add(descripcionPendiente, montoTemp)
                            End Select
                            Exit For
                        End If
                    Next
                End If
                capturandoConceptos = False
                capturandoTotales = True
                descripcionPendiente = Nothing
                Continue For
            End If

            ' Ignorar líneas neutras
            If txtU.StartsWith("UT") Or txtU = "IVA" Or txtU = "TOTAL" Or txtU.Contains("NO EFECTIVO") Then
                Continue For
            End If

            ' Capturar conceptos
            If capturandoConceptos Then
                Dim desc As String = Nothing
                Dim monto As Decimal
                Dim procesado As Boolean = False

                ' CASO ESPECIAL: TIEMPO PREPARACION DE PINTURA
                If seccionActual = "PIN" AndAlso (txtU.Contains("TIEMPO") AndAlso txtU.Contains("PREPARACION")) Then
                    Dim montoMatch = Regex.Match(txt, "\$\s*([\d,]+\.\d{2})")
                    If montoMatch.Success Then
                        desc = "TIEMPO PREPARACION DE PINTURA"
                        monto = Decimal.Parse(montoMatch.Groups(1).Value.Replace(",", ""))
                        procesado = True
                    Else
                        descripcionPendiente = "TIEMPO PREPARACION DE PINTURA"
                        procesado = True
                    End If
                End If

                If Not procesado Then
                    If TryParseConcepto(txt, seccionActual, desc, monto) Then
                        descripcionPendiente = Nothing
                    ElseIf descripcionPendiente IsNot Nothing AndAlso TryParseMonto(txt, monto) Then
                        desc = descripcionPendiente
                        descripcionPendiente = Nothing
                    ElseIf TryParseDescripcionSinMonto(txt, seccionActual, desc) Then
                        descripcionPendiente = desc
                    End If
                End If

                If desc IsNot Nothing Then
                    Select Case seccionActual
                        Case "REF" : dtRef.Rows.Add(desc, monto)
                        Case "PIN" : dtPin.Rows.Add(desc, monto)
                        Case "HOJ" : dtHoj.Rows.Add(desc, monto)
                    End Select
                End If
            End If
        Next
    End Sub

    Private Function TryParseConcepto(linea As String, seccion As String, ByRef descripcion As String, ByRef monto As Decimal) As Boolean
        If Not linea.Contains("$") Then Return False

        Dim montoMatch As Match = Nothing
        If Not TryParseMontoMatch(linea, montoMatch) Then Return False

        Dim textoAntes = linea.Substring(0, montoMatch.Index).Trim()
        If textoAntes = "" Then Return False

        Dim textoUpper = textoAntes.ToUpper()
        If textoUpper = "TOTAL" OrElse textoUpper = "IVA" OrElse textoUpper = "UT" Then Return False

        Dim desc = Regex.Replace(textoAntes, "\s+TPP(\s+[\d.]+)?\s*$", "", RegexOptions.IgnoreCase).Trim()
        desc = Regex.Replace(desc, "\s+[\d.]+\s*$", "").Trim()

        If EsConceptoBloqueado(desc) Then Return False
        If seccion = "HOJ" AndAlso textoUpper.Contains(":PINT") Then Return False
        If desc.Length < 3 Then Return False

        descripcion = desc
        monto = Decimal.Parse(montoMatch.Groups(1).Value.Replace(",", ""))
        Return True
    End Function

    Private Function TryParseDescripcionSinMonto(linea As String, seccion As String, ByRef descripcion As String) As Boolean
        If linea.Contains("$") Then Return False

        Dim texto = linea.Trim()
        If texto = "" Then Return False

        Dim textoUpper = texto.ToUpper()
        If textoUpper = "TOTAL" OrElse textoUpper = "IVA" OrElse textoUpper = "UT" Then Return False

        Dim desc = Regex.Replace(texto, "\s+TPP(\s+[\d.]+)?\s*$", "", RegexOptions.IgnoreCase).Trim()
        desc = Regex.Replace(desc, "\s+[\d.]+\s*$", "").Trim()

        If desc.Length < 3 Then Return False
        If EsConceptoBloqueado(desc) Then Return False
        If seccion = "HOJ" AndAlso textoUpper.Contains(":PINT") Then Return False

        descripcion = desc
        Return True
    End Function

    Private Function TryParseMonto(linea As String, ByRef monto As Decimal) As Boolean
        If linea.ToUpper().Contains("TPP") Then
            Dim montoMatch As Match = Nothing
            If Not TryParseMontoMatch(linea, montoMatch) Then Return False
            monto = Decimal.Parse(montoMatch.Groups(1).Value.Replace(",", ""))
            Return True
        End If

        Dim montoMatch2 As Match = Nothing
        If Not TryParseMontoMatch(linea, montoMatch2) Then Return False
        monto = Decimal.Parse(montoMatch2.Groups(1).Value.Replace(",", ""))
        Return True
    End Function

    Private Function TryParseMontoMatch(linea As String, ByRef montoMatch As Match) As Boolean
        montoMatch = Regex.Match(linea, "\$\s*([\d,]+\.\d{2})", RegexOptions.RightToLeft)
        If montoMatch.Success Then Return True

        montoMatch = Regex.Match(linea, "([\d]{1,3}(?:,\d{3})*\.\d{2})", RegexOptions.RightToLeft)
        Return montoMatch.Success
    End Function

    Private Function EsConceptoBloqueado(descripcion As String) As Boolean
        Dim descUpper = descripcion.ToUpper()
        Dim descSinAcentos = descUpper.Replace("É", "E")

        Dim bloqueados As String() = {
            "SUMA TOTAL SIN IVA",
            "16% IVA",
            "SUMA TOTAL VAL CON IVA",
            "DEDUCIBLE",
            "DEMERITO",
            "SUBTOTAL",
            "IVA",
            "TOTAL"
        }

        Return bloqueados.Any(Function(b) descSinAcentos.Contains(b))
    End Function

    Private Class TotalesRubro
        Public Sub1 As Decimal
        Public Iva As Decimal
        Public Tot As Decimal
        Private idx As Integer = 0

        Public Sub Asignar(valor As Decimal)
            Select Case idx
                Case 0 : Sub1 = valor
                Case 1 : Iva = valor
                Case 2 : Tot = valor
            End Select
            idx += 1
        End Sub

        Public Function Formato() As String
            Return String.Format("Subtotal: {0:C2} | IVA: {1:C2} | Total: {2:C2}", Sub1, Iva, Tot)
        End Function
    End Class

End Class
