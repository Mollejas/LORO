Imports System.Data
Imports System.IO
Imports System.Text.RegularExpressions
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports iTextSharp.text
Imports System.Linq

Public Class Valuacion
    Inherits System.Web.UI.Page

    Protected Sub btnProcesar_Click(sender As Object, e As EventArgs) Handles btnProcesar.Click

        If Not fuPdf.HasFile Then Exit Sub

        Dim lineas = LeerPdfPorLineas(fuPdf.PostedFile.InputStream)

        Dim dtRef = CrearTabla()
        Dim dtPin = CrearTabla()
        Dim dtHoj = CrearTabla()

        Dim totRef As New TotalesRubro()
        Dim totPin As New TotalesRubro()
        Dim totHoj As New TotalesRubro()

        Dim seccionActual As String = ""
        Dim capturandoConceptos As Boolean = False
        Dim capturandoTotales As Boolean = False
        Dim descripcionPendiente As String = Nothing

        For idx = 0 To lineas.Count - 1
            Dim linea = lineas(idx)
            Dim txt = linea.Trim()
            If txt = "" Then Continue For

            Dim txtU = txt.ToUpper()

            ' ==========================
            ' DETECTAR SECCIONES
            ' ==========================
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

            ' ==========================
            ' ENCABEZADO DESCRIPCION / MONTO
            ' ==========================
            If txtU.Contains("DESCRIPCION") AndAlso txtU.Contains("MONTO") Then
                capturandoConceptos = True
                Continue For
            End If

            ' ==========================
            ' INICIO DE TOTALES (CIERRA SECCION)
            ' ==========================
            If txtU = "SUBTOTAL" Then
                ' Si hay una descripción pendiente sin monto, intentar buscar su monto en las próximas líneas
                If descripcionPendiente IsNot Nothing Then
                    ' Buscar en las próximas 3 líneas si hay un monto
                    Dim indiceLimite = Math.Min(idx + 3, lineas.Count - 1)
                    For i = idx To indiceLimite
                        Dim lineaBusqueda = lineas(i)
                        Dim montoTemp As Decimal
                        If TryParseMonto(lineaBusqueda, montoTemp) Then
                            ' Encontramos el monto, insertar el concepto
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

            ' ==========================
            ' CAPTURAR SUB / IVA / TOTAL
            ' ==========================
            If capturandoTotales Then
                Dim m = Regex.Match(txt, "\$\s*([\d,]+\.\d{2})")
                If m.Success Then
                    Dim valor = Decimal.Parse(m.Groups(1).Value.Replace(",", ""))

                    Select Case seccionActual
                        Case "REF" : totRef.Asignar(valor)
                        Case "PIN" : totPin.Asignar(valor)
                        Case "HOJ" : totHoj.Asignar(valor)
                    End Select
                End If
                Continue For
            End If

            ' ==========================
            ' IGNORAR LINEAS NEUTRAS
            ' ==========================
            If txtU.StartsWith("UT") _
               Or txtU = "IVA" _
               Or txtU = "TOTAL" _
               Or txtU.Contains("NO EFECTIVO") Then Continue For

            ' ==========================
            ' CAPTURAR CONCEPTOS
            ' ==========================
            If capturandoConceptos Then

                Dim desc As String = Nothing
                Dim monto As Decimal
                Dim procesado As Boolean = False

                ' CASO ESPECIAL: Detectar TIEMPO PREPARACION DE PINTURA
                If seccionActual = "PIN" AndAlso (txtU.Contains("TIEMPO") AndAlso txtU.Contains("PREPARACION")) Then
                    ' Extraer monto si está en la misma línea
                    Dim montoMatch = Regex.Match(txt, "\$\s*([\d,]+\.\d{2})")
                    If montoMatch.Success Then
                        desc = "TIEMPO PREPARACION DE PINTURA"
                        monto = Decimal.Parse(montoMatch.Groups(1).Value.Replace(",", ""))
                        procesado = True
                    Else
                        ' Si no hay monto, guardar como descripción pendiente
                        descripcionPendiente = "TIEMPO PREPARACION DE PINTURA"
                        procesado = True
                    End If
                End If

                ' Intentar procesar normalmente si no fue procesado arriba
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
                        Case "REF"
                            dtRef.Rows.Add(desc, monto)
                        Case "PIN"
                            dtPin.Rows.Add(desc, monto)
                        Case "HOJ"
                            dtHoj.Rows.Add(desc, monto)
                    End Select
                End If
            End If

        Next

        ' ==========================
        ' FALLBACK: Buscar TIEMPO PREPARACION DE PINTURA si no fue capturado
        ' ==========================
        Dim tieneTPP As Boolean = False
        For Each row As DataRow In dtPin.Rows
            If row("Descripcion").ToString().ToUpper().Contains("TIEMPO") AndAlso
               row("Descripcion").ToString().ToUpper().Contains("PREPARACION") Then
                tieneTPP = True
                Exit For
            End If
        Next

        If Not tieneTPP Then
            ' Buscar en todas las líneas del PDF
            For Each linea In lineas
                Dim txtU = linea.ToUpper()
                If (txtU.Contains("TIEMPO") AndAlso txtU.Contains("PREPARACION")) OrElse
                   (txtU.Contains("TPP") AndAlso txtU.Contains("$")) Then
                    ' Intentar extraer el monto
                    Dim montoMatch = Regex.Match(linea, "\$\s*([\d,]+\.\d{2})")
                    If montoMatch.Success Then
                        Dim monto = Decimal.Parse(montoMatch.Groups(1).Value.Replace(",", ""))
                        dtPin.Rows.Add("TIEMPO PREPARACION DE PINTURA", monto)
                        Exit For
                    End If
                End If
            Next
        End If

        ' ==========================
        ' FALLBACK: Buscar CUALQUIER concepto después de TPP en PINTURA
        ' ==========================
        ' Encontrar el índice de la línea de TIEMPO PREPARACION DE PINTURA
        Dim indiceTPP As Integer = -1
        Dim indiceSubtotalPin As Integer = -1
        Dim dentroSeccionPintura As Boolean = False

        For i = 0 To lineas.Count - 1
            Dim txtU = lineas(i).ToUpper()

            ' Marcar cuando entramos a la sección PINTURA
            If txtU.Contains("PINTURA") AndAlso Not txtU.Contains("PREPARACION") Then
                dentroSeccionPintura = True
                Continue For
            End If

            ' Salir de la sección cuando llegamos a MANO DE OBRA HOJALATERIA
            If txtU.Contains("MANO DE OBRA HOJALATERIA") Then
                dentroSeccionPintura = False
                indiceSubtotalPin = i
                Exit For
            End If

            ' Dentro de la sección PINTURA, buscar TIEMPO PREPARACION
            If dentroSeccionPintura AndAlso ((txtU.Contains("TIEMPO") AndAlso txtU.Contains("PREPARACION")) OrElse txtU.Contains("TPP")) Then
                indiceTPP = i
            End If

            ' Buscar SUBTOTAL dentro de la sección PINTURA
            If dentroSeccionPintura AndAlso txtU.Contains("SUBTOTAL") Then
                indiceSubtotalPin = i
            End If
        Next

        ' Si encontramos TPP, buscar conceptos entre TPP y SUBTOTAL
        If indiceTPP >= 0 AndAlso indiceSubtotalPin > indiceTPP Then
            For i = indiceTPP + 1 To indiceSubtotalPin - 1
                Dim linea = lineas(i)
                Dim txtU = linea.ToUpper()

                ' Ignorar líneas que no son conceptos
                If txtU = "" OrElse txtU.StartsWith("UT") OrElse txtU = "IVA" OrElse txtU = "TOTAL" Then
                    Continue For
                End If

                ' Si la línea tiene $ (posible concepto)
                If linea.Contains("$") Then
                    Dim montoMatch As Match = Nothing
                    If TryParseMontoMatch(linea, montoMatch) Then
                        ' Extraer descripción
                        Dim textoAntes = linea.Substring(0, montoMatch.Index).Trim()

                        ' Limpiar TPP y números al final
                        Dim desc = Regex.Replace(textoAntes, "\s+TPP(\s+[\d.]+)?\s*$", "", RegexOptions.IgnoreCase).Trim()
                        desc = Regex.Replace(desc, "\s+[\d.]+\s*$", "").Trim()

                        ' Validar que sea un concepto válido
                        If desc.Length >= 3 AndAlso Not EsConceptoBloqueado(desc) Then
                            ' Verificar que no esté ya capturado
                            Dim yaCapturado As Boolean = False
                            For Each row As DataRow In dtPin.Rows
                                If row("Descripcion").ToString().Trim().ToUpper() = desc.ToUpper() Then
                                    yaCapturado = True
                                    Exit For
                                End If
                            Next

                            ' Agregar si no fue capturado
                            If Not yaCapturado Then
                                Dim monto = Decimal.Parse(montoMatch.Groups(1).Value.Replace(",", ""))
                                dtPin.Rows.Add(desc, monto)
                            End If
                        End If
                    End If
                End If
            Next
        End If

        ' ==========================
        ' BIND GRIDS
        ' ==========================
        gvRefacciones.DataSource = dtRef
        gvRefacciones.DataBind()

        gvPintura.DataSource = dtPin
        gvPintura.DataBind()

        gvHojalateria.DataSource = dtHoj
        gvHojalateria.DataBind()

        ' ==========================
        ' TOTALES ABAJO DE CADA GRID
        ' ==========================
        lblTotRef.Text = totRef.Formato()
        lblTotPintura.Text = totPin.Formato()
        lblTotHoj.Text = totHoj.Formato()

    End Sub

    ' ==========================
    ' CREA TABLA
    ' ==========================
    Private Function CrearTabla() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Descripcion", GetType(String))
        dt.Columns.Add("Monto", GetType(Decimal))
        Return dt
    End Function

    ' ==========================
    ' LECTURA PDF POR LINEAS
    ' ==========================
    Private Function LeerPdfPorLineas(stream As System.IO.Stream) As List(Of String)

        Dim reader As New PdfReader(stream)
        Dim resultado As New List(Of String)()

        For p = 1 To reader.NumberOfPages

            Dim tamano = reader.GetPageSize(p)
            Dim mitadX = tamano.Width / 2

            ' La hoja viene dividida en dos secciones (izquierda y derecha).
            ' Extraemos primero el bloque izquierdo (de arriba hacia abajo)
            ' y después el bloque derecho.
            Dim regionIzq As New Rectangle(0, 0, mitadX, tamano.Height)
            Dim regionDer As New Rectangle(mitadX, 0, tamano.Width, tamano.Height)

            resultado.AddRange(ExtraerLineasRegion(reader, p, regionIzq))
            resultado.AddRange(ExtraerLineasRegion(reader, p, regionDer))
        Next

        reader.Close()

        Return resultado
    End Function

    Private Function TryParseConcepto(linea As String, seccion As String, ByRef descripcion As String, ByRef monto As Decimal) As Boolean

        If Not linea.Contains("$") Then Return False

        Dim montoMatch As Match = Nothing
        If Not TryParseMontoMatch(linea, montoMatch) Then Return False

        Dim textoAntes = linea.Substring(0, montoMatch.Index).Trim()
        If textoAntes = "" Then Return False

        Dim textoUpper = textoAntes.ToUpper()
        If textoUpper = "TOTAL" OrElse textoUpper = "IVA" OrElse textoUpper = "UT" Then Return False

        ' Eliminar TPP y números al final (para TIEMPO PREPARACION)
        Dim desc = Regex.Replace(textoAntes, "\s+TPP(\s+[\d.]+)?\s*$", "", RegexOptions.IgnoreCase).Trim()

        ' Eliminar números decimales al final (para conceptos con UT como "TIRANTE BRAZO SUP.TRAS.SALP. 10.0")
        desc = Regex.Replace(desc, "\s+[\d.]+\s*$", "").Trim()

        If EsConceptoBloqueado(desc) Then Return False

        ' No aplicar validación EsLineaPintura en sección PINTURA - aceptar todos los conceptos
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

        ' Eliminar TPP y números al final
        Dim desc = Regex.Replace(texto, "\s+TPP(\s+[\d.]+)?\s*$", "", RegexOptions.IgnoreCase).Trim()

        ' Eliminar números decimales al final (para conceptos con UT)
        desc = Regex.Replace(desc, "\s+[\d.]+\s*$", "").Trim()

        If desc.Length < 3 Then Return False
        If EsConceptoBloqueado(desc) Then Return False

        ' No aplicar validación EsLineaPintura en sección PINTURA - aceptar todos los conceptos
        If seccion = "HOJ" AndAlso textoUpper.Contains(":PINT") Then Return False

        descripcion = desc
        Return True
    End Function

    Private Function TryParseMonto(linea As String, ByRef monto As Decimal) As Boolean

        ' Si la línea contiene TPP, es probable que sea la línea de monto para TIEMPO PREPARACION DE PINTURA
        ' que viene en línea separada, así que es válido para sección de pintura
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
        ' Primero intenta encontrar un monto con símbolo de peso
        montoMatch = Regex.Match(linea, "\$\s*([\d,]+\.\d{2})", RegexOptions.RightToLeft)
        If montoMatch.Success Then Return True

        ' Si no, busca cualquier número con formato de moneda (dígitos, comas, punto y 2 decimales)
        montoMatch = Regex.Match(linea, "([\d]{1,3}(?:,\d{3})*\.\d{2})", RegexOptions.RightToLeft)
        Return montoMatch.Success
    End Function

    Private Function EsLineaPintura(textoUpper As String) As Boolean
        Return textoUpper.Contains(":PINT") _
            OrElse textoUpper.Contains("TPP") _
            OrElse textoUpper.Contains("PINTURA") _
            OrElse textoUpper.Contains("PREPARACION")
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

    Private Function ExtraerLineasRegion(reader As PdfReader, pagina As Integer, region As Rectangle) As IEnumerable(Of String)

        Dim strategy As New FilteredTextRenderListener(New LocationTextExtractionStrategy(), New RegionTextRenderFilter(region))
        Dim texto = PdfTextExtractor.GetTextFromPage(reader, pagina, strategy)

        Return texto.
            Split({vbLf, vbCr}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(l) l.Trim()).
            Where(Function(l) l <> "")
    End Function

End Class