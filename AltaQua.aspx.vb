Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports Path = System.IO.Path


Public Class AltaQua
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Compatible con TextMode="DateTimeLocal"
            txtFechaCreacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm")
        End If
    End Sub

    ' Se dispara cuando el JS hace click en btnTrigger al cargar el PDF
    Protected Sub BtnTrigger_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Not fupPDF.HasFile Then
            ' Aquí puedes mostrar un mensaje en un Label si quieres
            Exit Sub
        End If

        Dim ext As String = Path.GetExtension(fupPDF.FileName).ToLowerInvariant()
        If ext <> ".pdf" Then
            ' Mensaje de error: solo PDF
            Exit Sub
        End If

        ' Carpeta temporal para guardar el volante
        Dim tempFolder As String = Server.MapPath("~/TmpVolantes")
        If Not Directory.Exists(tempFolder) Then
            Directory.CreateDirectory(tempFolder)
        End If

        Dim fileName As String = Guid.NewGuid().ToString("N") & ".pdf"
        Dim fullPath As String = Path.Combine(tempFolder, fileName)
        fupPDF.SaveAs(fullPath)

        ' Extraer texto del PDF
        Dim textoPdf As String = ExtraerTextoPdf(fullPath)

        ' Llenar los campos de la pantalla
        RellenarCamposDesdePdf(textoPdf)
    End Sub

    Private Function ExtraerTextoPdf(ruta As String) As String
        Dim sb As New StringBuilder()

        Using reader As New PdfReader(ruta)
            For i As Integer = 1 To reader.NumberOfPages
                Dim strategy As ITextExtractionStrategy = New SimpleTextExtractionStrategy()
                Dim pageText As String = PdfTextExtractor.GetTextFromPage(reader, i, strategy)
                sb.AppendLine(pageText)
            Next
        End Using

        Return sb.ToString()
    End Function

    Private Sub RellenarCamposDesdePdf(pdf As String)
        ' Normalizamos por líneas
        Dim lineas = pdf.Split({vbCrLf, vbLf, vbCr}, StringSplitOptions.RemoveEmptyEntries)

        '========================
        '  ENCABEZADO VOLANTE
        '========================

        ' FOLIO ELECTRÓNICO 25VE00831368
        Dim folioElectronico As String = GetValueSameLine(pdf, "FOLIO ELECTRÓNICO")

        ' N°. REPORTE 04251684532
        Dim noReporte As String = GetValueSameLine(pdf, "N°. REPORTE")

        ' N°. SINIESTRO -> en el PDF de muestra el número está una línea ANTERIOR
        Dim noSiniestro As String = GetRelativeLineValue(lineas, "N°. SINIESTRO", -1)

        ' FECHA / ...  -> siguiente línea trae la fecha 03/10/2025
        Dim fechaSiniestro As String = GetRelativeLineValue(lineas, "FECHA /", 1)
        ' HORA /  -> siguiente línea trae la hora 08:32 HRS
        Dim horaSiniestro As String = GetRelativeLineValue(lineas, "HORA /", 1)

        ' ASEGURADO /   (línea 16)
        ' TERCERO Q /   (línea 17)
        ' RAFAEL ROJAS MORENO (línea 18)
        Dim nombreAsegurado As String = GetRelativeLineValue(lineas, "ASEGURADO /", 2)

        ' NOMBRE O RAZÓN SOCIAL DEL CLIENTE /  -> siguiente línea
        Dim razonSocialCliente As String = GetRelativeLineValue(lineas, "NOMBRE O RAZÓN SOCIAL DEL CLIENTE", 1)

        ' TELÉFONO / -> siguiente línea (asegurado)
        Dim telAsegurado As String = GetRelativeLineValue(lineas, "TELÉFONO /", 1)

        ' E-MAIL / -> siguiente línea
        Dim mailAsegurado As String = GetRelativeLineValue(lineas, "E-MAIL /", 1)

        ' TELÉFONO   (segunda aparición, CDR) -> siguiente línea
        Dim telCDR As String = GetRelativeLineValueExact(lineas, "TELÉFONO", 1)

        ' RESPONSABLE -> siguiente línea
        Dim responsableCDR As String = GetRelativeLineValueExact(lineas, "RESPONSABLE", 1)

        ' DOMICILIO / COBERTURA / -> en el ejemplo el domicilio viene en la línea anterior
        Dim domicilioCobertura As String = GetRelativeLineValue(lineas, "DOMICILIO / COBERTURA /", -1)

        '========================
        '  VEHÍCULO
        '========================

        ' En el volante el valor está una línea ANTES de la etiqueta
        Dim marca As String = GetRelativeLineValue(lineas, "MARCA /", -1)            ' TOYOTA
        Dim tipo As String = GetRelativeLineValue(lineas, "TIPO /", -1)             ' TY TOYOTA PRIUS C...
        Dim modelo As String = GetRelativeLineValue(lineas, "MODELO (AÑO) / KILOMETRAJE /", -1) ' 2020

        ' Serie / VIN -> viene en la MISMA línea que "N°. DE SERIE /TRANSMISIÓN /"
        Dim serie As String = ExtraerSerieVin(lineas)  ' JTDKDTB34L1634173

        ' Placas y color -> línea anterior a "LICENSE PLATES", p.ej. "74J322BLANCO"
        Dim placas As String = String.Empty
        Dim color As String = String.Empty
        ExtraerPlacasYColor(lineas, placas, color)

        ' Ajustador (clave + nombre) -> parte baja del documento
        Dim claveAjustador As String = String.Empty
        Dim nombreAjustador As String = String.Empty
        ExtraerDatosAjustador(pdf, claveAjustador, nombreAjustador)

        '========================
        '  ASIGNACIÓN A CONTROLES
        '========================

        ' --- Columna 1: generación expediente / cliente ---
        txtExpediente.Text = noSiniestro          ' puedes usar el siniestro como consecutivo
        txtCreadoPor.Text = ""                    ' se captura manual
        txtFechaCreacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm")
        txtSiniestroGen.Text = noSiniestro

        ddlTipoIngreso.SelectedIndex = 0          ' TRANSITO/GRUA/PROPIO IMPULSO -> se define internamente
        ddlDeducible.SelectedIndex = 0            ' SI/NO -> no viene en volante
        ddlEstatus.SelectedIndex = 0              ' se ajusta por lógica de negocio

        txtAsegurado.Text = nombreAsegurado
        txtTelefono.Text = telAsegurado
        txtCorreo.Text = mailAsegurado.ToLowerInvariant()

        ' --- Columna 2: identificación del siniestro ---
        txtEmisor.Text = "QUÁLITAS COMPAÑÍA DE SEGUROS S.A. DE C.V."
        txtCarpeta.Text = folioElectronico            ' puedes decidir si aquí va el folio electrónico
        txtPoliza.Text = ""                           ' en el PDF de muestra no se ve el número
        txtCIS.Text = ""
        txtSiniestro.Text = noSiniestro
        txtReporte.Text = noReporte
        txtEstCobranza.Text = ""

        FchSiniestro.Text = fechaSiniestro            ' 03/10/2025
        ' si quieres guardar también la hora, crea un TextBox txtHoraSiniestro y asígnalo:
        ' txtHoraSiniestro.Text = horaSiniestro

        txtVigenciaDesde.Text = ""
        txtVigenciaHasta.Text = ""

        txtAjustador.Text = nombreAjustador
        txtClaveAjustador.Text = claveAjustador

        ' --- Columna 3: datos del vehículo ---
        txtMarca.Text = marca
        txtTipo.Text = tipo
        txtModelo.Text = modelo
        txtMotor.Text = ""                            ' no viene en el volante
        txtSerie.Text = serie
        txtPlacas.Text = placas
        txtColor.Text = color
        txtTransmision.Text = ""                      ' en el volante son casillas AUTOMÁTICA / MANUAL
        txtKilometros.Text = ""                       ' no se imprime en el ejemplo
        txtUso.Text = ""                              ' no viene en el volante

        ' Puertas: no vienen en el volante, las dejamos en false
        chk2Puertas.Checked = False
        chk4Puertas.Checked = False
    End Sub

    ' ------------------ HELPERS ------------------

    ' Valor en la MISMA línea que la etiqueta (ej: "FOLIO ELECTRÓNICO 25VE00831368")
    Private Function GetValueSameLine(pdf As String, etiqueta As String) As String
        Dim pattern As String = Regex.Escape(etiqueta) & "\s+([A-Z0-9 ]+)"
        Dim m As Match = Regex.Match(pdf, pattern, RegexOptions.IgnoreCase)
        If m.Success Then
            Return m.Groups(1).Value.Trim()
        End If
        Return String.Empty
    End Function

    ' Devuelve la línea relativa (offset) a donde se encuentre la etiqueta (busca con Contains)
    Private Function GetRelativeLineValue(lineas As String(), etiqueta As String, offset As Integer) As String
        For i As Integer = 0 To lineas.Length - 1
            If lineas(i).ToUpper().Contains(etiqueta.ToUpper()) Then
                Dim idx As Integer = i + offset
                If idx >= 0 AndAlso idx < lineas.Length Then
                    Return lineas(idx).Trim()
                End If
            End If
        Next
        Return String.Empty
    End Function

    ' Igual que la anterior pero exige coincidencia EXACTA de la línea con la etiqueta
    Private Function GetRelativeLineValueExact(lineas As String(), etiqueta As String, offset As Integer) As String
        For i As Integer = 0 To lineas.Length - 1
            If String.Equals(lineas(i).Trim(), etiqueta, StringComparison.OrdinalIgnoreCase) Then
                Dim idx As Integer = i + offset
                If idx >= 0 AndAlso idx < lineas.Length Then
                    Return lineas(idx).Trim()
                End If
            End If
        Next
        Return String.Empty
    End Function

    ' Busca la sección "NOMBRE, APELLIDOS, CLAVE Y FIRMA DEL AJUSTADOR / ABOGADO"
    ' y en las siguientes líneas extrae: CLAVE + NOMBRE
    Private Sub ExtraerDatosAjustador(pdf As String, ByRef clave As String, ByRef nombre As String)
        clave = String.Empty
        nombre = String.Empty

        Dim lineas = pdf.Split({vbCrLf, vbLf, vbCr}, StringSplitOptions.RemoveEmptyEntries)

        For i As Integer = 0 To lineas.Length - 1
            If lineas(i).ToUpper().Contains("NOMBRE, APELLIDOS, CLAVE Y FIRMA DEL AJUSTADOR") Then
                For j As Integer = i + 1 To Math.Min(lineas.Length - 1, i + 6)
                    Dim l As String = lineas(j).Trim()
                    ' Buscar una línea con un número de 4+ dígitos seguido de texto
                    Dim m As Match = Regex.Match(l, "(\d{4,})\s+(.+)")
                    If m.Success Then
                        clave = m.Groups(1).Value.Trim()
                        nombre = m.Groups(2).Value.Trim()
                        Exit Sub
                    End If
                Next
            End If
        Next
    End Sub

    ' --------- NUEVOS HELPERS SOLO PARA SERIE / PLACAS / COLOR ---------

    ' Extrae el VIN de la línea que contiene "N°. DE SERIE /TRANSMISIÓN /"
    Private Function ExtraerSerieVin(lineas As String()) As String
        For Each l As String In lineas
            Dim upper As String = l.ToUpperInvariant()
            Dim idx As Integer = upper.IndexOf("N°. DE SERIE")
            If idx > 0 Then
                ' Todo lo que está ANTES del texto "N°. DE SERIE" es el VIN
                Dim before As String = l.Substring(0, idx).Trim()

                ' Dejamos solo caracteres A-Z y 0-9
                Dim soloAlfanumerico As String = Regex.Replace(before.ToUpperInvariant(), "[^A-Z0-9]", "")

                ' Un VIN normal son 17 caracteres
                If soloAlfanumerico.Length >= 10 Then
                    If soloAlfanumerico.Length >= 17 Then
                        Return soloAlfanumerico.Substring(soloAlfanumerico.Length - 17)
                    Else
                        Return soloAlfanumerico
                    End If
                End If
            End If
        Next
        Return String.Empty
    End Function

    ' Ubica la línea anterior a "LICENSE PLATES" y separa placas + color (ej: "74J322BLANCO")
    Private Sub ExtraerPlacasYColor(lineas As String(), ByRef placas As String, ByRef color As String)
        placas = String.Empty
        color = String.Empty

        For i As Integer = 0 To lineas.Length - 1
            Dim upper As String = lineas(i).ToUpperInvariant()
            If upper.Contains("LICENSE PLATES") Then
                Dim idxLineaDatos As Integer = i - 1
                If idxLineaDatos >= 0 Then
                    Dim concatenado As String = lineas(idxLineaDatos).Trim()
                    SplitPlacasColor(concatenado, placas, color)
                End If
                Exit Sub
            End If
        Next
    End Sub

    ' A partir de algo como "74J322BLANCO" lo separa en:
    '   placas = "74J322"
    '   color  = "BLANCO"
    Private Sub SplitPlacasColor(cadenaCruda As String, ByRef placas As String, ByRef color As String)
        placas = String.Empty
        color = String.Empty

        If String.IsNullOrWhiteSpace(cadenaCruda) Then Exit Sub

        ' Quitamos espacios y pasamos a mayúsculas
        Dim s As String = Regex.Replace(cadenaCruda.ToUpperInvariant(), "\s+", "")

        ' Buscamos un punto de corte donde la cola sean solo letras (el color)
        For i As Integer = 1 To s.Length - 2
            Dim tail As String = s.Substring(i)
            If Regex.IsMatch(tail, "^[A-Z]+$") Then
                placas = s.Substring(0, i)
                color = tail
                Exit Sub
            End If
        Next

        ' Fallback: últimas letras como color
        Dim m As Match = Regex.Match(s, "^(.+?)([A-Z]{3,})$")
        If m.Success Then
            placas = m.Groups(1).Value
            color = m.Groups(2).Value
        Else
            placas = s
        End If
    End Sub

    ' Aquí iría tu lógica de guardado a BD
    Protected Sub btnGuardar_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' TODO: guardar en tu tabla de expedientes, usando los TextBox / DropDownList ya llenados.
    End Sub

End Class
