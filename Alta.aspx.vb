Imports System.IO
Imports System.Drawing
Imports System.Text
Imports System.Text.RegularExpressions
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports Path = System.IO.Path
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Globalization
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Web
Imports System.Linq

Public Class Alta
    Inherits System.Web.UI.Page

    ' ===== Subcarpetas estándar =====
    Private ReadOnly SubcarpetasInbursa As String() = {
        "1. DOCUMENTOS DE INGRESO",
        "2. FOTOS DIAGNOSTICO MECANICA",
        "3. FOTOS DIAGNOSTICO HOJALATERIA",
        "4. VALUACION",
        "5. REFACCIONES",
        "6. FOTOS PROCESO DE REPARACION",
        "7. FOTOS DE SALIDA",
        "8. FACTURACION",
        "9. FOTOS DE RECLAMACION",
        "10. FOTOS REINGRESO DE TRANSITO "
    }

    Private Const TEMP_DIR As String = "~/App_Data/tmp"

    ' ================================
    ' ============ EVENTS ============
    ' ================================
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Ocultar checkboxes de puertas (y contenedores)
            If rowPuertas2 IsNot Nothing Then rowPuertas2.Visible = False
            If rowPuertas4 IsNot Nothing Then rowPuertas4.Visible = False
            If chk2Puertas IsNot Nothing Then chk2Puertas.Visible = False
            If chk4Puertas IsNot Nothing Then chk4Puertas.Visible = False

            ' Sugerido en UI (no definitivo) por paridad
            SetExpedienteSugeridoPorParidad()

            ' “Creado por” desde el Master
            Dim nombreCreador As String = If(Master IsNot Nothing, Master.CurrentUserName, String.Empty)
            txtCreadoPor.Text = nombreCreador
            txtCreadoPor.ReadOnly = True
            txtCreadoPor.Attributes("readonly") = "readonly"
            txtCreadoPor.CssClass = (txtCreadoPor.CssClass & " bg-light").Trim()

            ' FECHA ACTUAL (YYYY-MM-DD) y solo lectura
            Dim ahora = DateTime.Now
            txtFechaCreacion.TextMode = TextBoxMode.DateTimeLocal
            txtFechaCreacion.Text = ahora.ToString("yyyy-MM-ddTHH:mm") ' formato HTML5
            txtFechaCreacion.ReadOnly = True
            ' ya no necesitas: txtFechaCreacion.Attributes("readonly") = "readonly"
            txtFechaCreacion.CssClass = (txtFechaCreacion.CssClass & " bg-light").Trim()

            ' Estatus en blanco visualmente
            If ddlEstatus IsNot Nothing Then ddlEstatus.ClearSelection()
        End If
    End Sub

    ' Disparado por el botón oculto tras seleccionar/soltar PDF
    Protected Sub BtnTrigger_Click(sender As Object, e As EventArgs) Handles btnTrigger.Click
        ProcesarPDFyPrellenar()
        SetExpedienteSugeridoPorParidad()
    End Sub

    Private Sub ProcesarPDFyPrellenar()
        If Not fupPDF.HasFile Then Exit Sub

        Dim msOriginal As New MemoryStream()
        fupPDF.PostedFile.InputStream.CopyTo(msOriginal)

        Dim msTexto As New MemoryStream(msOriginal.ToArray())

        Dim contenido As String = ExtraerTextoDePDF(msTexto)
        Dim lineas = contenido.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries).ToList()
        contenido = contenido.Replace(ChrW(&HA0), " ").Replace(vbCr, vbLf)

        ' === Folio/Reporte ===
        Dim idxFolioTxt As Integer = contenido.IndexOf("Folio", StringComparison.OrdinalIgnoreCase)
        If idxFolioTxt >= 0 Then
            Dim start As Integer = idxFolioTxt
            Dim length As Integer = Math.Min(300, contenido.Length - start)
            Dim bloque As String = contenido.Substring(start, length)
            Dim msNums = Regex.Matches(bloque, "\d{7,}")
            If msNums.Count > 0 Then
                txtReporte.Text = msNums(msNums.Count - 1).Value
            Else
                Dim ms6 = Regex.Matches(bloque, "\d{6,}")
                If ms6.Count > 0 Then txtReporte.Text = ms6(ms6.Count - 1).Value
            End If
        End If

        ' === Encabezado principal ===
        Dim idxEncabezado = lineas.FindIndex(Function(l) l.Contains("Emisor") AndAlso l.Contains("Número de carpeta"))
        If idxEncabezado >= 0 AndAlso idxEncabezado + 1 < lineas.Count Then
            Dim valoresLinea As String = lineas(idxEncabezado + 1).Trim()
            Dim matches = Regex.Matches(valoresLinea, "40100\s*-\s*\d+")
            If matches.Count >= 1 Then txtSiniestro.Text = matches(0).Value
            Dim partes = Regex.Split(valoresLinea, "\s+")
            If partes.Length >= 4 Then
                txtEmisor.Text = partes(0)
                txtCarpeta.Text = partes(1)
                txtPoliza.Text = partes(2)
                txtCIS.Text = partes(3)
            End If
        End If

        ' === Cobranza / Vigencias / Fecha Siniestro ===
        Dim idxCobranza = lineas.FindIndex(Function(l) l.Contains("Estado de cobranza"))
        If idxCobranza >= 0 AndAlso idxCobranza + 1 < lineas.Count Then
            Dim datos = Regex.Split(lineas(idxCobranza + 1).Trim(), "\s+")
            If datos.Length >= 5 Then
                txtEstCobranza.Text = datos(0) & " " & datos(1)
                txtVigenciaDesde.Text = datos(2)
                txtVigenciaHasta.Text = datos(3)
                FchSiniestro.Text = datos(4)
            End If
        End If

        ' === Ajustador ===
        Dim idxAjustador = lineas.FindIndex(Function(l) l.Contains("Nombre del ajustador"))
        If idxAjustador >= 0 Then
            Dim datos = Regex.Split(lineas(idxAjustador + 1).Trim(), "\s+")
            If datos.Length > 1 Then
                txtAjustador.Text = String.Join(" ", datos.Take(datos.Length - 1))
                txtClaveAjustador.Text = datos.Last()
            End If
        End If

        ' === Asegurado / Teléfono / Correo ===
        Dim idxAsegurado = lineas.FindIndex(Function(l) l.Contains("Nombre completo"))
        If idxAsegurado >= 0 Then txtAsegurado.Text = lineas(idxAsegurado + 1).Trim()
        Dim idxTel = lineas.FindIndex(Function(l) l.Contains("Teléfono celular"))
        If idxTel >= 0 Then
            Dim datosTel = Regex.Split(lineas(idxTel + 1).Trim(), "\s+")
            If datosTel.Length >= 1 Then txtTelefono.Text = datosTel(0)
            If datosTel.Length >= 2 Then txtCorreo.Text = datosTel(1)
        End If

        ' === Vehículo ===
        Dim idxVehiculo = lineas.FindIndex(Function(l) l.Contains("Marca") AndAlso l.Contains("Modelo"))
        Dim idxPlacas = lineas.FindIndex(Function(l) l.Contains("Placas") AndAlso l.Contains("Color"))
        If idxVehiculo >= 0 AndAlso idxPlacas > idxVehiculo Then
            Dim numLineas = idxPlacas - idxVehiculo - 1
            If numLineas = 3 Then
                Dim linea1 = lineas(idxVehiculo + 1).Trim()
                Dim linea2 = lineas(idxVehiculo + 2).Trim()
                Dim linea3 = lineas(idxVehiculo + 3).Trim()
                txtMarca.Text = CleanMarca(RemoveParentheses(linea1 & " " & linea3))
                Dim datos = Regex.Split(linea2, "\s+")
                If datos.Length >= 4 Then
                    txtTipo.Text = datos(0)
                    txtModelo.Text = datos(1)
                    txtMotor.Text = datos(2)
                    txtSerie.Text = datos(3)
                End If
            Else
                If idxVehiculo + 1 < lineas.Count Then
                    Dim datos = Regex.Split(lineas(idxVehiculo + 1).Trim(), "\s+")
                    If datos.Length >= 6 AndAlso datos(1).StartsWith("(") AndAlso datos(1).EndsWith(")") Then
                        txtMarca.Text = CleanMarca(RemoveParentheses(datos(0) & " " & datos(1)))
                        txtTipo.Text = datos(2)
                        txtModelo.Text = datos(3)
                        txtMotor.Text = datos(4)
                        txtSerie.Text = datos(5)
                    ElseIf datos.Length >= 5 Then
                        txtMarca.Text = CleanMarca(RemoveParentheses(datos(0)))
                        txtTipo.Text = datos(1)
                        txtModelo.Text = datos(2)
                        txtMotor.Text = datos(3)
                        txtSerie.Text = datos(4)
                    End If
                End If
            End If
        End If

        Dim idxPlacas2 = lineas.FindIndex(Function(l) l.Contains("Placas") AndAlso l.Contains("Color"))
        If idxPlacas2 >= 0 AndAlso idxPlacas2 + 1 < lineas.Count Then
            Dim datos = Regex.Split(lineas(idxPlacas2 + 1).Trim(), "\s+")
            If datos.Length >= 5 Then
                txtPlacas.Text = datos(0)
                txtColor.Text = datos(1)
                txtTransmision.Text = datos(2)
                txtKilometros.Text = datos(3)
                txtUso.Text = datos.Last()
            End If
        End If

        ' === Guardar temporal ODA.pdf para moverlo al Guardar ===
        Dim ext = Path.GetExtension(fupPDF.FileName).ToLowerInvariant()
        If ext <> ".pdf" Then Throw New ApplicationException("El archivo debe ser un PDF (.pdf).")
        Dim dirTmp = Server.MapPath(TEMP_DIR)
        If Not Directory.Exists(dirTmp) Then Directory.CreateDirectory(dirTmp)
        Dim tempName = Guid.NewGuid().ToString("N") & ".pdf"
        Dim tempPhysical = Path.Combine(dirTmp, tempName)
        fupPDF.SaveAs(tempPhysical)
        ViewState("TempPdfRel") = TEMP_DIR.TrimEnd("/"c) & "/" & tempName

        ' Siniestro (últimos 7)
        Dim ult7 As String = Ultimos7Digitos(txtSiniestro.Text)
        txtSiniestroGen.Text = ult7
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        ' === Validaciones de selección ===
        Dim tipoSel As String = If(ddlTipoIngreso.SelectedValue, String.Empty).Trim().ToUpperInvariant()
        Dim estSel As String = If(ddlEstatus.SelectedValue, String.Empty).Trim().ToUpperInvariant()

        If String.IsNullOrWhiteSpace(txtSiniestroGen.Text) OrElse
       String.IsNullOrWhiteSpace(tipoSel) OrElse
       String.IsNullOrWhiteSpace(ddlDeducible.SelectedValue) OrElse
       String.IsNullOrWhiteSpace(estSel) Then
            Alert("Completa Generación de expediente.")
            Exit Sub
        End If

        ' Reglas de negocio: TipoIngreso vs Estatus
        If (tipoSel = "GRUA" OrElse tipoSel = "PROPIO IMPULSO") AndAlso estSel <> "PISO" Then
            Alert("Si el Tipo de Ingreso es GRUA o PROPIO IMPULSO, el Estatus debe ser PISO.")
            Exit Sub
        End If
        If (tipoSel = "TRANSITO") AndAlso estSel <> "TRANSITO" Then
            Alert("Si el Tipo de Ingreso es TRANSITO, el Estatus debe ser TRANSITO.")
            Exit Sub
        End If

        ' === 1) Número DEFINITIVO por paridad (Expediente visible para el usuario) ===
        Dim paridadUsuario As String = ObtenerParidadUsuarioActual()
        Dim expedienteId As Integer
        Try
            expedienteId = ObtenerSiguienteExpedienteSeguro(paridadUsuario)
        Catch ex As Exception
            Alert("No se pudo asignar el número de expediente: " & ex.Message.Replace("'", "\'"))
            Exit Sub
        End Try
        txtExpediente.Text = expedienteId.ToString("0")

        Dim csSetting = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If csSetting Is Nothing OrElse String.IsNullOrWhiteSpace(csSetting.ConnectionString) Then
            Alert("Falta la cadena de conexión DaytonaDB en Web.config")
            Exit Sub
        End If

        Try
            ' === 2) Preparar carpeta destino (usa Marca limpia sin paréntesis) ===
            Dim idFormateado As String = expedienteId.ToString("D5")
            Dim marcaClean As String = CleanMarca(RemoveParentheses(txtMarca.Text))

            Dim partes As New List(Of String)
            If Not String.IsNullOrWhiteSpace(marcaClean) Then partes.Add(marcaClean.Trim())
            If Not String.IsNullOrWhiteSpace(txtTipo.Text) Then partes.Add(txtTipo.Text.Trim())
            If Not String.IsNullOrWhiteSpace(txtModelo.Text) Then partes.Add(txtModelo.Text.Trim())
            If Not String.IsNullOrWhiteSpace(txtColor.Text) Then partes.Add(txtColor.Text.Trim())
            If Not String.IsNullOrWhiteSpace(txtPlacas.Text) Then partes.Add(txtPlacas.Text.Trim())

            Dim carpetaNombre As String = "EXP " & idFormateado
            If partes.Count > 0 Then carpetaNombre &= " " & String.Join(" ", partes)
            carpetaNombre = SanitizeFileName(carpetaNombre)

            Dim baseVirtual = GetInbursaBaseVirtual().TrimEnd("/"c)   ' p.ej. ~/INBURSA
            Dim carpetaRel As String = (baseVirtual & "/" & carpetaNombre).Replace("//", "/")
            Dim carpetaFisica = Server.MapPath(carpetaRel)

            If Not Directory.Exists(carpetaFisica) Then Directory.CreateDirectory(carpetaFisica)
            For Each subc In SubcarpetasInbursa
                Dim rel = carpetaRel & "/" & SanitizeFileName(subc)
                Dim phy = Server.MapPath(rel)
                If Not Directory.Exists(phy) Then Directory.CreateDirectory(phy)
            Next

            ' === 3) Mover ODA.pdf desde TEMP si existe ===
            Dim tempRel = TryCast(ViewState("TempPdfRel"), String)
            If Not String.IsNullOrWhiteSpace(tempRel) Then
                Dim tempPhysical = Server.MapPath(tempRel)
                If File.Exists(tempPhysical) Then
                    Dim relDocs = carpetaRel & "/1. DOCUMENTOS DE INGRESO"
                    Dim phyDocs = Server.MapPath(relDocs)
                    If Not Directory.Exists(phyDocs) Then Directory.CreateDirectory(phyDocs)
                    Dim destino = Path.Combine(phyDocs, "ODA.pdf")
                    If File.Exists(destino) Then File.Delete(destino)
                    File.Move(tempPhysical, destino)
                    ViewState("TempPdfRel") = Nothing
                End If
            End If

            ' === 4) INSERT en Admisiones y obtener el Id (PK=Id) con OUTPUT INSERTED.Id ===
            Dim newAdmId As Integer = 0
            Using cn As New SqlConnection(csSetting.ConnectionString)
                Const sqlInsert As String = "
            INSERT INTO dbo.Admisiones
            (
                Expediente, CreadoPor, FechaCreacion, SiniestroGen, TipoIngreso, DeducibleSI_NO, Estatus,
                Asegurado, Telefono, Correo,
                Emisor, Carpeta, Poliza, CIS, SiniestroIdent, Reporte, EstCobranza, FechaSiniestro, VigenciaDesde, VigenciaHasta,
                Ajustador, ClaveAjustador,
                Marca, Tipo, Modelo, Motor, Serie, Placas, Color, Transmision, Kilometros, Uso, Puertas2, Puertas4,
                CarpetaRel
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @Expediente, @CreadoPor, @FechaCreacion, @SiniestroGen, @TipoIngreso, @DeducibleSI_NO, @Estatus,
                @Asegurado, @Telefono, @Correo,
                @Emisor, @Carpeta, @Poliza, @CIS, @SiniestroIdent, @Reporte, @EstCobranza, @FechaSiniestro, @VigenciaDesde, @VigenciaHasta,
                @Ajustador, @ClaveAjustador,
                @Marca, @Tipo, @Modelo, @Motor, @Serie, @Placas, @Color, @Transmision, @Kilometros, @Uso, @Puertas2, @Puertas4,
                @CarpetaRel
            );"

                Using cmd As New SqlCommand(sqlInsert, cn)
                    cmd.CommandType = CommandType.Text

                    ' CreadoPor desde el textbox o Master
                    Dim creadoPorNombre As String = If(Not String.IsNullOrWhiteSpace(txtCreadoPor.Text), txtCreadoPor.Text.Trim(),
                                                    If(Master IsNot Nothing, Master.CurrentUserName, String.Empty))

                    ' Generación
                    cmd.Parameters.Add("@Expediente", SqlDbType.NVarChar, 50).Value = txtExpediente.Text.Trim()
                    cmd.Parameters.Add("@CreadoPor", SqlDbType.NVarChar, 100).Value =
                    If(String.IsNullOrWhiteSpace(creadoPorNombre), DBNull.Value, CType(creadoPorNombre.Trim(), Object))
                    cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = DateTime.Now
                    cmd.Parameters.Add("@SiniestroGen", SqlDbType.NVarChar, 50).Value = txtSiniestroGen.Text.Trim()
                    cmd.Parameters.Add("@TipoIngreso", SqlDbType.NVarChar, 20).Value = ddlTipoIngreso.SelectedValue
                    cmd.Parameters.Add("@DeducibleSI_NO", SqlDbType.NVarChar, 2).Value = ddlDeducible.SelectedValue
                    cmd.Parameters.Add("@Estatus", SqlDbType.NVarChar, 20).Value = ddlEstatus.SelectedValue

                    ' Cliente (reducido)
                    cmd.Parameters.Add("@Asegurado", SqlDbType.NVarChar, 150).Value = txtAsegurado.Text.Trim()
                    cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 50).Value = txtTelefono.Text.Trim()
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = txtCorreo.Text.Trim()

                    ' Identificación del siniestro
                    cmd.Parameters.Add("@Emisor", SqlDbType.NVarChar, 100).Value = txtEmisor.Text.Trim()
                    cmd.Parameters.Add("@Carpeta", SqlDbType.NVarChar, 50).Value = txtCarpeta.Text.Trim()
                    cmd.Parameters.Add("@Poliza", SqlDbType.NVarChar, 50).Value = txtPoliza.Text.Trim()
                    cmd.Parameters.Add("@CIS", SqlDbType.NVarChar, 50).Value = txtCIS.Text.Trim()
                    cmd.Parameters.Add("@SiniestroIdent", SqlDbType.NVarChar, 50).Value = txtSiniestro.Text.Trim()
                    cmd.Parameters.Add("@Reporte", SqlDbType.NVarChar, 50).Value = txtReporte.Text.Trim()
                    cmd.Parameters.Add("@EstCobranza", SqlDbType.NVarChar, 50).Value = txtEstCobranza.Text.Trim()
                    cmd.Parameters.Add("@FechaSiniestro", SqlDbType.Date).Value = ParseDateOrDbNull(FchSiniestro.Text)
                    cmd.Parameters.Add("@VigenciaDesde", SqlDbType.Date).Value = ParseDateOrDbNull(txtVigenciaDesde.Text)
                    cmd.Parameters.Add("@VigenciaHasta", SqlDbType.Date).Value = ParseDateOrDbNull(txtVigenciaHasta.Text)
                    cmd.Parameters.Add("@Ajustador", SqlDbType.NVarChar, 100).Value = txtAjustador.Text.Trim()
                    cmd.Parameters.Add("@ClaveAjustador", SqlDbType.NVarChar, 50).Value = txtClaveAjustador.Text.Trim()

                    ' Vehículo
                    cmd.Parameters.Add("@Marca", SqlDbType.NVarChar, 50).Value = marcaClean
                    cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 50).Value = txtTipo.Text.Trim()
                    cmd.Parameters.Add("@Modelo", SqlDbType.NVarChar, 20).Value = txtModelo.Text.Trim()
                    cmd.Parameters.Add("@Motor", SqlDbType.NVarChar, 50).Value = txtMotor.Text.Trim()
                    cmd.Parameters.Add("@Serie", SqlDbType.NVarChar, 50).Value = txtSerie.Text.Trim()
                    cmd.Parameters.Add("@Placas", SqlDbType.NVarChar, 20).Value = txtPlacas.Text.Trim()
                    cmd.Parameters.Add("@Color", SqlDbType.NVarChar, 30).Value = txtColor.Text.Trim()
                    cmd.Parameters.Add("@Transmision", SqlDbType.NVarChar, 30).Value = txtTransmision.Text.Trim()
                    cmd.Parameters.Add("@Kilometros", SqlDbType.NVarChar, 20).Value = txtKilometros.Text.Trim()
                    cmd.Parameters.Add("@Uso", SqlDbType.NVarChar, 30).Value = txtUso.Text.Trim()
                    cmd.Parameters.Add("@Puertas2", SqlDbType.Bit).Value = chk2Puertas.Checked
                    cmd.Parameters.Add("@Puertas4", SqlDbType.Bit).Value = chk4Puertas.Checked

                    ' Carpeta relativa
                    cmd.Parameters.Add("@CarpetaRel", SqlDbType.NVarChar, 300).Value = carpetaRel

                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()   ' <- Id gracias a OUTPUT INSERTED.Id
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        newAdmId = Convert.ToInt32(obj)
                    Else
                        Throw New ApplicationException("No se pudo obtener el Id insertado.")
                    End If
                End Using
            End Using

            ' === 5) (Opcional) Correo de bienvenida ===
            Dim destinatario As String = If(txtCorreo IsNot Nothing, txtCorreo.Text.Trim(), String.Empty)
            If Not String.IsNullOrWhiteSpace(destinatario) AndAlso IsValidEmail(destinatario) Then
                Try
                    EnviarCorreoBienvenida(destinatario)
                Catch
                    ' Si falla el correo, no bloqueamos el flujo
                End Try
            End If

            ' === 6) Redirección a Hoja.aspx con el Id (PK) recién creado ===
            Dim url As String = ResolveUrl("Hoja.aspx?id=" & HttpUtility.UrlEncode(newAdmId.ToString()))
            Response.Redirect(url, False)
            Context.ApplicationInstance.CompleteRequest()
            Return

        Catch ex As Exception
            Alert("Error al guardar: " & ex.Message.Replace("'", "\'"))
        End Try
    End Sub



    ' ================================
    ' ============ HELPERS ===========
    ' ================================
    Private Sub SetExpedienteSugeridoPorParidad()
        Dim paridad As String = ObtenerParidadUsuarioActual()
        Dim ultimos = ObtenerUltimosParYNonExpediente()
        Dim lastPar As Integer? = If(ultimos IsNot Nothing, ultimos.Item1, CType(Nothing, Integer?))
        Dim lastNon As Integer? = If(ultimos IsNot Nothing, ultimos.Item2, CType(Nothing, Integer?))
        Dim nextPar As Integer = If(lastPar.HasValue, lastPar.Value + 2, 2)
        Dim nextNon As Integer = If(lastNon.HasValue, lastNon.Value + 2, 1)

        ViewState("NextParPreview") = nextPar
        ViewState("NextNonPreview") = nextNon

        Dim nextExp As Integer = If(paridad = "NON", nextNon, nextPar)
        txtExpediente.Text = nextExp.ToString("0")
        ViewState("NextIdPreview") = nextExp
    End Sub

    Private Function ObtenerParidadUsuarioActual() As String
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        Dim fallback As String = If(String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings("DefaultParidad")), "PAR", ConfigurationManager.AppSettings("DefaultParidad"))

        If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
            Return fallback.ToUpperInvariant().Trim()
        End If

        Dim par As String = Nothing

        ' Session("UsuarioId")
        Dim objId = Session("UsuarioId")
        Dim usuarioId As Integer
        If objId IsNot Nothing AndAlso Integer.TryParse(objId.ToString(), usuarioId) Then
            par = ObtenerParidadPorUsuarioId(usuarioId, cs.ConnectionString)
        End If

        ' Session("UsuarioCorreo")
        If String.IsNullOrWhiteSpace(par) Then
            Dim correo As String = TryCast(Session("UsuarioCorreo"), String)
            If Not String.IsNullOrWhiteSpace(correo) Then
                par = ObtenerParidadPorCorreo(correo, cs.ConnectionString)
            End If
        End If

        ' Session("UsuarioNombre")
        If String.IsNullOrWhiteSpace(par) Then
            Dim nombre As String = TryCast(Session("UsuarioNombre"), String)
            If Not String.IsNullOrWhiteSpace(nombre) Then
                par = ObtenerParidadPorNombre(nombre, cs.ConnectionString)
            End If
        End If

        par = If(par, fallback)
        par = par.ToUpperInvariant().Trim()
        If par <> "PAR" AndAlso par <> "NON" Then par = fallback.ToUpperInvariant().Trim()
        Return par
    End Function

    Private Function ObtenerParidadPorUsuarioId(usuarioId As Integer, cs As String) As String
        Const sql As String = "SELECT TOP 1 Paridad FROM dbo.Usuarios WHERE UsuarioId = @Id"
        Try
            Using cn As New SqlConnection(cs)
                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = usuarioId
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Return obj.ToString()
                    End If
                End Using
            End Using
        Catch
        End Try
        Return Nothing
    End Function

    Private Function ObtenerParidadPorCorreo(correo As String, cs As String) As String
        Const sql As String = "SELECT TOP 1 Paridad FROM dbo.Usuarios WHERE Correo = @Correo"
        Try
            Using cn As New SqlConnection(cs)
                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 256).Value = correo.Trim()
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Return obj.ToString()
                    End If
                End Using
            End Using
        Catch
        End Try
        Return Nothing
    End Function

    Private Function ObtenerParidadPorNombre(nombre As String, cs As String) As String
        Const sql As String = "SELECT TOP 1 Paridad FROM dbo.Usuarios WHERE Nombre = @Nombre"
        Try
            Using cn As New SqlConnection(cs)
                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 256).Value = nombre.Trim()
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Return obj.ToString()
                    End If
                End Using
            End Using
        Catch
        End Try
        Return Nothing
    End Function

    Private Function ObtenerUltimosParYNonExpediente() As Tuple(Of Integer?, Integer?)
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
            Return Tuple.Create(CType(Nothing, Integer?), CType(Nothing, Integer?))
        End If

        Const sql As String = "
            WITH V AS (
                SELECT TRY_CONVERT(INT, Expediente) AS v
                FROM dbo.Admisiones
                WHERE TRY_CONVERT(INT, Expediente) IS NOT NULL
            )
            SELECT
                MAX(CASE WHEN v % 2 = 0 THEN v END) AS LastPar,
                MAX(CASE WHEN v % 2 = 1 THEN v END) AS LastNon
            FROM V;"

        Try
            Using cn As New SqlConnection(cs.ConnectionString)
                Using cmd As New SqlCommand(sql, cn)
                    cn.Open()
                    Using rd = cmd.ExecuteReader()
                        If rd.Read() Then
                            Dim lastPar As Integer? = If(rd.IsDBNull(0), CType(Nothing, Integer?), rd.GetInt32(0))
                            Dim lastNon As Integer? = If(rd.IsDBNull(1), CType(Nothing, Integer?), rd.GetInt32(1))
                            Return Tuple.Create(lastPar, lastNon)
                        End If
                    End Using
                End Using
            End Using
        Catch
        End Try

        Return Tuple.Create(CType(Nothing, Integer?), CType(Nothing, Integer?))
    End Function

    Private Function ObtenerSiguienteExpedienteSeguro(paridad As String) As Integer
        paridad = If(paridad, "PAR").ToUpperInvariant().Trim()
        If paridad <> "PAR" AndAlso paridad <> "NON" Then paridad = "PAR"

        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
            Throw New ApplicationException("Falta la cadena de conexión DaytonaDB.")
        End If

        Dim recursoLock As String = If(paridad = "PAR", "EXPEDIENTE_PAR", "EXPEDIENTE_NON")
        Dim paridadMod As Integer = If(paridad = "PAR", 0, 1)

        Using cn As New SqlConnection(cs.ConnectionString)
            cn.Open()
            Using tx = cn.BeginTransaction(IsolationLevel.Serializable)
                Using lockCmd As New SqlCommand("EXEC @rc = sp_getapplock @Resource,@LockMode,@LockOwner,@LockTimeout;", cn, tx)
                    lockCmd.Parameters.Add("@rc", SqlDbType.Int).Direction = ParameterDirection.Output
                    lockCmd.Parameters.Add("@Resource", SqlDbType.NVarChar, 255).Value = recursoLock
                    lockCmd.Parameters.Add("@LockMode", SqlDbType.NVarChar, 32).Value = "Exclusive"
                    lockCmd.Parameters.Add("@LockOwner", SqlDbType.NVarChar, 32).Value = "Transaction"
                    lockCmd.Parameters.Add("@LockTimeout", SqlDbType.Int).Value = 10000
                    lockCmd.ExecuteNonQuery()
                    Dim rc As Integer = CInt(lockCmd.Parameters("@rc").Value)
                    If rc < 0 Then
                        Throw New ApplicationException("No se pudo obtener bloqueo de asignación de expediente (timeout).")
                    End If
                End Using

                Const sqlLast As String = "
                    WITH V AS (
                      SELECT TRY_CONVERT(INT, Expediente) AS v
                      FROM dbo.Admisiones WITH (UPDLOCK, HOLDLOCK)
                      WHERE TRY_CONVERT(INT, Expediente) IS NOT NULL
                    )
                    SELECT MAX(v) FROM V WHERE v % 2 = @paridad;"

                Dim lastVal As Integer? = Nothing
                Using cmd As New SqlCommand(sqlLast, cn, tx)
                    cmd.Parameters.Add("@paridad", SqlDbType.Int).Value = paridadMod
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot DBNull.Value AndAlso obj IsNot Nothing Then
                        lastVal = Convert.ToInt32(obj)
                    End If
                End Using

                Dim siguiente As Integer = If(lastVal.HasValue, lastVal.Value + 2, If(paridad = "PAR", 2, 1))
                tx.Commit()
                Return siguiente
            End Using
        End Using
    End Function

    ' -------- Utilidades varias --------
    Private Function RemoveParentheses(s As String) As String
        If String.IsNullOrWhiteSpace(s) Then Return String.Empty
        Dim t As String = s
        Dim prev As String
        Do
            prev = t
            t = Regex.Replace(t, "\s*\([^)]*\)\s*", " ", RegexOptions.Multiline)
        Loop While t <> prev
        Return t
    End Function

    Private Function CleanMarca(s As String) As String
        If String.IsNullOrWhiteSpace(s) Then Return String.Empty
        Dim t = RemoveParentheses(s)
        t = Regex.Replace(t, "\s{2,}", " ").Trim()
        t = t.TrimEnd("."c, "-"c, "_"c)
        Return t
    End Function

    Private Function Ultimos7Digitos(noSiniestro As String) As String
        If String.IsNullOrEmpty(noSiniestro) Then Return String.Empty
        Dim digits = New String(noSiniestro.Where(AddressOf Char.IsDigit).ToArray())
        If digits.Length <= 7 Then Return digits
        Return digits.Substring(digits.Length - 7)
    End Function

    Private Sub Alert(msg As String)
        ClientScript.RegisterStartupScript(Me.GetType(), "msg", "alert('" & msg.Replace("'", "\'") & "');", True)
    End Sub

    Private Function SanitizeFileName(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return String.Empty
        Dim invalid = Path.GetInvalidFileNameChars()
        Dim limpio = New String(input.Select(Function(c) If(invalid.Contains(c), "-"c, c)).ToArray())
        limpio = Regex.Replace(limpio, "\s{2,}", " ").Trim()
        limpio = limpio.TrimEnd("."c, "-"c, "_"c)
        Return limpio
    End Function

    Private Function ExtraerTextoDePDF(stream As Stream) As String
        Dim texto As New StringWriter()
        Using lector As New PdfReader(stream)
            For i As Integer = 1 To lector.NumberOfPages
                texto.WriteLine(PdfTextExtractor.GetTextFromPage(lector, i))
            Next
        End Using
        Return texto.ToString()
    End Function

    Private Function HayMarcaX(bitmap As Bitmap, region As Rectangle, umbralOscuridad As Integer, umbralPorcentaje As Double) As Boolean
        Dim pixelesOscuros As Integer = 0
        Dim totalPixeles As Integer = region.Width * region.Height

        Dim recorte As New Bitmap(region.Width, region.Height)
        Using g As Graphics = Graphics.FromImage(recorte)
            g.DrawImage(bitmap, 0, 0, region, GraphicsUnit.Pixel)
        End Using

        For y As Integer = 0 To recorte.Height - 1
            For x As Integer = 0 To recorte.Width - 1
                Dim colorPixel As Color = recorte.GetPixel(x, y)
                Dim gris As Integer = (CInt(colorPixel.R) + CInt(colorPixel.G) + CInt(colorPixel.B)) \ 3
                If gris < umbralOscuridad Then pixelesOscuros += 1
            Next
        Next

        Dim porcentajeOscuros As Double = pixelesOscuros / totalPixeles
        Return porcentajeOscuros >= umbralPorcentaje
    End Function

    Private Function ParseDateOrDbNull(s As String) As Object
        If String.IsNullOrWhiteSpace(s) Then Return DBNull.Value
        Dim dt As DateTime
        Dim formatos = {"yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy"}
        If DateTime.TryParseExact(s.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, dt) Then
            Return dt.Date
        End If
        If DateTime.TryParse(s, CultureInfo.GetCultureInfo("es-MX"), DateTimeStyles.None, dt) Then
            Return dt.Date
        End If
        Return DBNull.Value
    End Function

    Private Sub AddDec(cmd As SqlCommand, name As String, precision As Byte, scale As Byte, txt As String)
        Dim p = cmd.Parameters.Add(name, SqlDbType.Decimal)
        p.Precision = precision : p.Scale = scale
        If String.IsNullOrWhiteSpace(txt) Then
            p.Value = DBNull.Value
        Else
            Dim v As Decimal
            Dim norm = txt.Replace(",", ".")
            If Decimal.TryParse(norm, NumberStyles.Any, CultureInfo.InvariantCulture, v) Then
                p.Value = v
            Else
                p.Value = DBNull.Value
            End If
        End If
    End Sub

    Private Function GetInbursaBaseVirtual() As String
        Dim v = ConfigurationManager.AppSettings("InbursaBaseVirtual")
        If String.IsNullOrWhiteSpace(v) Then v = "~/INBURSA"
        Return v.TrimEnd("/"c)
    End Function

    ' ================================
    ' ============ CORREO ============
    ' ================================
    Private Function ObtenerTelefonoAsesor(nombreCreador As String) As String
        If String.IsNullOrWhiteSpace(nombreCreador) Then Return String.Empty
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then Return String.Empty

        Const sql As String = "SELECT TOP 1 Telefono FROM dbo.Usuarios WHERE Nombre = @Nombre"
        Try
            Using cn As New SqlConnection(cs.ConnectionString)
                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 256).Value = nombreCreador.Trim()
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Return obj.ToString().Trim()
                    End If
                End Using
            End Using
        Catch
            Return String.Empty
        End Try
        Return String.Empty
    End Function

    Private Sub EnviarCorreoBienvenida(destinatario As String)
        ' === SMTP directo (sin Web.config) ===
        Const SMTP_HOST As String = "mail.loroautomotriz.com.mx"
        Const SMTP_PORT As Integer = 587
        Const SMTP_USER As String = "no-responder@loroautomotriz.com.mx"  ' <- el que indica el hosting
        Const SMTP_PASS As String = "2K3Le3z9pqvlo~re"                 ' <- la del cPanel
        Const FROM_EMAIL As String = "no-responder@loroautomotriz.com.mx"  ' <- ideal: igual al usuario autenticado
        Const FROM_NAME As String = "LORO REPARACION AUTOMOTRIZ"

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim expediente As String = txtExpediente.Text.Trim()
        Dim asunto As String = $"Bienvenido(a) – Expediente {expediente}"

        ' Asesor = txtCreadoPor (o Master)
        Dim asesorNombre As String = If(Not String.IsNullOrWhiteSpace(txtCreadoPor.Text),
                                    txtCreadoPor.Text.Trim(),
                                    If(Master IsNot Nothing, Master.CurrentUserName, String.Empty))
        Dim asesorTelefono As String = ObtenerTelefonoAsesor(asesorNombre)

        Dim html As String = ConstruirHtmlCorreo(asesorNombre, asesorTelefono)
        Dim textoPlano As String = QuitarEtiquetas(html)

        Dim msg As New MailMessage() With {
        .From = New MailAddress(FROM_EMAIL, FROM_NAME),
        .Subject = asunto,
        .BodyEncoding = Encoding.UTF8,
        .IsBodyHtml = True
    }
        msg.To.Add(New MailAddress(destinatario))

        ' Si quieres que el cliente responda al asesor (opcional):
        ' If Not String.IsNullOrWhiteSpace(txtCorreo.Text) Then
        '     msg.ReplyToList.Clear()
        '     msg.ReplyToList.Add(New MailAddress(FROM_EMAIL, asesorNombre))
        ' End If

        ' Vista HTML con logo
        Dim logoPhysical As String = Server.MapPath("~/images/logo1.png")
        Dim viewHtml As AlternateView = CrearVistaHtmlConLogo(html, logoPhysical, "logoCID")
        msg.AlternateViews.Add(viewHtml)

        ' Texto plano (compatibilidad)
        Dim altText As AlternateView = AlternateView.CreateAlternateViewFromString(textoPlano, Encoding.UTF8, MediaTypeNames.Text.Plain)
        msg.AlternateViews.Add(altText)

        Using smtp As New SmtpClient(SMTP_HOST, SMTP_PORT)
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network
            smtp.UseDefaultCredentials = False
            smtp.Credentials = New NetworkCredential(SMTP_USER, SMTP_PASS)
            smtp.EnableSsl = False ' Implicit SSL (465)
            smtp.Timeout = 30000
            smtp.Send(msg)
        End Using
    End Sub



    Private Function ConstruirHtmlCorreo(asesorNombre As String, asesorTelefono As String) As String
        ' Utilidades E(), CleanMarca(), RemoveParentheses() asumidas como en tu proyecto
        Dim marca = E(CleanMarca(RemoveParentheses(txtMarca.Text)))
        Dim tipo = E(txtTipo.Text)
        Dim modelo = E(txtModelo.Text)
        Dim motor = E(txtMotor.Text)
        Dim serie = E(txtSerie.Text)
        Dim placas = E(txtPlacas.Text)
        Dim color = E(txtColor.Text)
        Dim transmision = E(txtTransmision.Text)
        Dim kms = E(txtKilometros.Text)
        Dim uso = E(txtUso.Text)
        Dim expediente = E(txtExpediente.Text)

        ' Datos adicionales que aparecen en el PDF
        Dim clienteNombre = E(txtAsegurado.Text)            ' e.g., "Armando Hernández"
        Dim clienteTelefono = E(txtTelefono.Text)        ' e.g., "(55) 584160684"
        '  Dim tipoCliente = E(txtTipoCliente.Text)                ' e.g., "Asegurado"
        'Dim aseguradora = E(txtAseguradora.Text)                ' e.g., "Qualitas"
        Dim reporte = E(txtReporte.Text)                        ' e.g., "042565568698"
        Dim tipoIngreso = E(ddlTipoIngreso.Text)                ' e.g., "Grúa"

        Dim recepcion = "(55) 8717-4788 / 89 Ext: 103"          ' Ajusta si lo tomas de un control
        Dim horario = "Lunes a Viernes<br/>8:00 AM-2:00 PM y de 3:00 PM– 6:00 PM" ' Igual que en el PDF
        ' Dim linkQualitas = txtLinkQualitas.Text             ' URL para “Enlace”, opcional

        Dim sb As New StringBuilder()
        sb.AppendLine("<!DOCTYPE html><html lang='es'><head><meta charset='utf-8' />")
        sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1' /></head>")
        ' Fondo azul exterior como el marco del PDF
        sb.AppendLine("<body style=""margin:0;padding:0;background:#0b5d7c;font-family:Arial,Helvetica,sans-serif;color:#111827;"">")
        sb.AppendLine("<table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background:#0b5d7c;padding:28px 12px;'><tr><td align='center'>")
        sb.AppendLine("<table role='presentation' width='740' cellspacing='0' cellpadding='0' border='0' style='background:#ffffff;border-collapse:separate;border-radius:8px;overflow:hidden;box-shadow:0 6px 28px rgba(0,0,0,.08);'>")

        ' ======= Encabezado con logo y título =======
        sb.AppendLine("<tr><td style='padding:24px 24px 10px 24px;'>")
        sb.AppendLine("<table width='100%' role='presentation' cellspacing='0' cellpadding='0'><tr>")
        sb.AppendLine("<td style='width:120px;vertical-align:top;'><img src='cid:logoCID' alt='LORO Reparación Automotriz' style='display:block;height:96px;max-width:120px;' /></td>")
        sb.AppendLine("<td style='vertical-align:middle;text-align:center;'>")
        sb.AppendLine("<div style='font-size:22px;font-weight:bold;color:#1182b3;margin-bottom:6px;'>¡Bienvenido a su centro de reparación!</div>")
        sb.AppendLine("<div style='font-size:18px;color:#1182b3;'>LORO Reparación Automotriz</div>")
        sb.AppendLine("</td><td style='width:120px;'>&nbsp;</td>")
        sb.AppendLine("</tr></table>")
        sb.AppendLine("<div style='margin-top:14px;font-size:13px;line-height:1.55;color:#374151;'>")
        sb.AppendLine("Estimado (a) cliente<br/>")
        sb.AppendLine("¡Gracias por elegirnos como su taller de reparación automotriz!<br/>")
        sb.AppendLine("Nos complace tenerlo como cliente y queremos asegurarle que estamos comprometidos en brindar un servicio de calidad.<br/>")
        sb.AppendLine("Estaremos acompañándolo en todo el proceso de su reparación para asegurarnos de que sus necesidades sean atendidas.")
        sb.AppendLine("</div>")
        sb.AppendLine("</td></tr>")

        ' Número de expediente destacado
        sb.AppendLine("<tr><td style='padding:6px 24px 0 24px;text-align:center;'>")
        sb.AppendLine("<div style='font-size:16px;color:#111827;'>Número de expediente LORO:</div>")
        sb.AppendLine("<div style='font-size:42px;line-height:1;font-weight:800;letter-spacing:.5px;margin:8px 0 14px 0;color:#111827;'>" & expediente & "</div>")
        sb.AppendLine("</td></tr>")

        ' ======= Tarjeta Asesor asignado =======
        sb.AppendLine("<tr><td style='padding:0 24px 0 24px;'>")
        sb.AppendLine("<table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='border:1px solid #e5e7eb;border-radius:6px;overflow:hidden;margin:0 0 18px 0;'>")
        sb.AppendLine("<tr><td colspan='2' style='background:#f7fbff;padding:10px 14px;font-weight:bold;color:#0b64a3;font-size:14px;'>Asesor asignado:</td></tr>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<td style='width:50%;padding:12px 14px;font-size:14px;vertical-align:top;'>")
        sb.AppendLine("<div style='color:#111827;'>" & E(asesorNombre) & "</div>")
        sb.AppendLine("<div style='margin-top:10px;'><span style='font-weight:bold;'>Horario de atención:</span><br/>" & horario & "</div>")
        sb.AppendLine("</td>")
        sb.AppendLine("<td style='width:50%;padding:12px 14px;font-size:14px;vertical-align:top;'>")
        sb.AppendLine("<div style='margin-bottom:8px;'><span style='font-weight:bold;'>Teléfono:</span><br/>" & If(String.IsNullOrWhiteSpace(asesorTelefono), "No disponible", E(asesorTelefono)) & "</div>")
        sb.AppendLine("<div><span style='font-weight:bold;'>Recepción:</span><br/>" & E(recepcion) & "</div>")
        sb.AppendLine("</td></tr></table>")
        sb.AppendLine("</td></tr>")

        ' ======= Encabezado Información General =======
        sb.AppendLine("<tr><td style='padding:0 24px;'><div style='color:#6b7280;font-weight:bold;font-size:13px;margin:6px 0 8px 0;'>Información General</div></td></tr>")

        ' ======= Datos del cliente =======
        sb.AppendLine("<tr><td style='padding:0 24px;'>")
        sb.AppendLine("<table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='border:1px solid #e5e7eb;border-radius:6px;overflow:hidden;margin:0 0 14px 0;'>")
        sb.AppendLine("<tr><td colspan='4' style='background:#f7fbff;padding:10px 14px;font-weight:bold;color:#0b64a3;font-size:14px;'>Datos del cliente</td></tr>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<td style='width:25%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Nombre:</span><br/>" & clienteNombre & "</td>")
        sb.AppendLine("<td style='width:25%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Aseguradora:</span><br/>" & clienteNombre & "</td>")
        sb.AppendLine("<td style='width:25%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Reporte:</span><br/>" & reporte & "</td>")
        sb.AppendLine("<td style='width:25%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Tipo de ingreso:</span><br/>" & tipoIngreso & "</td>")
        sb.AppendLine("</tr>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<td style='width:25%;padding:0 14px 14px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Teléfono:</span><br/>" & clienteTelefono & "</td>")
        sb.AppendLine("<td style='width:25%;padding:0 14px 14px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Tipo de cliente:</span><br/>" & clienteNombre & "</td>")
        sb.AppendLine("<td style='width:25%;padding:0 14px 14px 14px;'>&nbsp;</td>")
        sb.AppendLine("<td style='width:25%;padding:0 14px 14px 14px;'>&nbsp;</td>")
        sb.AppendLine("</tr>")
        sb.AppendLine("</table>")
        sb.AppendLine("</td></tr>")

        ' ======= Datos del vehículo =======
        sb.AppendLine("<tr><td style='padding:0 24px;'>")
        sb.AppendLine("<table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='border:1px solid #e5e7eb;border-radius:6px;overflow:hidden;margin:0 0 18px 0;'>")
        sb.AppendLine("<tr><td colspan='6' style='background:#f7fbff;padding:10px 14px;font-weight:bold;color:#0b64a3;font-size:14px;'>Datos del vehículo</td></tr>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<td style='width:16.6%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Marca:</span><br/>" & marca & "</td>")
        sb.AppendLine("<td style='width:16.6%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Tipo:</span><br/>" & tipo & "</td>")
        sb.AppendLine("<td style='width:16.6%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Modelo:</span><br/>" & modelo & "</td>")
        sb.AppendLine("<td style='width:16.6%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Motor:</span><br/>" & motor & "</td>")
        sb.AppendLine("<td style='width:16.6%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Serie (VIN):</span><br/>" & serie & "</td>")
        sb.AppendLine("<td style='width:16.6%;padding:12px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Placas:</span><br/>" & placas & "</td>")
        sb.AppendLine("</tr>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<td style='padding:0 14px 14px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Color:</span><br/>" & color & "</td>")
        sb.AppendLine("<td style='padding:0 14px 14px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Transmisión:</span><br/>" & transmision & "</td>")
        sb.AppendLine("<td style='padding:0 14px 14px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Kilómetros:</span><br/>" & kms & "</td>")
        sb.AppendLine("<td style='padding:0 14px 14px 14px;font-size:14px;vertical-align:top;'><span style='font-weight:bold;'>Uso:</span><br/>" & uso & "</td>")
        sb.AppendLine("<td style='padding:0 14px 14px 14px;'>&nbsp;</td>")
        sb.AppendLine("<td style='padding:0 14px 14px 14px;'>&nbsp;</td>")
        sb.AppendLine("</tr>")
        sb.AppendLine("</table>")
        sb.AppendLine("</td></tr>")

        ' ======= Conozca el avance de su reparación =======
        sb.AppendLine("<tr><td style='padding:0 24px 10px 24px;'>")
        sb.AppendLine("<table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='border:1px solid #e5e7eb;border-radius:6px;overflow:hidden;margin:0 0 10px 0;'>")
        sb.AppendLine("<tr><td style='background:#eef6fb;padding:14px 16px;text-align:center;font-size:18px;color:#1182b3;font-weight:700;'>Conozca el avance de su reparación</td></tr>")
        sb.AppendLine("<tr><td style='padding:14px 16px;font-size:14px;color:#374151;line-height:1.6;'>")
        sb.AppendLine("Queremos mantenerte informado sobre el avance de la reparación de tu vehículo. Consulta el estado de tu reparación de las siguientes maneras:")
        sb.AppendLine("<ul style='margin:12px 0 0 18px;padding:0;'>")
        If Not String.IsNullOrWhiteSpace(clienteNombre) Then
            sb.AppendLine("<li>A través del portal qualitas <a href='" & clienteNombre & "' style='color:#0b64a3;text-decoration:underline;'>Enlace</a></li>")
        Else
            sb.AppendLine("<li>A través del portal qualitas <span style='text-decoration:underline;'>Enlace</span></li>")
        End If
        sb.AppendLine("<li>Escaneando el código QR en tu orden de admisión</li>")
        sb.AppendLine("<li>O mediante los siguientes pasos:</li>")
        sb.AppendLine("</ul>")
        sb.AppendLine("<ol style='margin:6px 0 0 18px;padding:0;'>")
        sb.AppendLine("<li>Identifica tu número de expediente LORO dentro de su hoja de ingreso o dentro de este correo</li>")
        sb.AppendLine("<li>Ingresa a nuestra página de seguimiento Loro reparación Automotriz</li>")
        sb.AppendLine("<li>Digite su número de expediente para acceder a la información.</li>")
        sb.AppendLine("</ol>")
        sb.AppendLine("<div style='margin-top:12px;'>Si tienes alguna duda, no dudes en contactarnos. Estamos para apoyarte.</div>")
        sb.AppendLine("<div style='margin-top:10px;'>Atentamente<br/>Centro Loro reparación automotriz</div>")
        sb.AppendLine("<div style='margin-top:8px;'>Teléfonos: (55) 8717-4788 / 89 Ext: 101<br/>E-mail: atencionaclientes@loroautomotriz.com.mx</div>")
        sb.AppendLine("</td></tr></table>")
        sb.AppendLine("</td></tr>")

        ' ======= Pie de página (barra azul) =======
        sb.AppendLine("<tr><td style='background:#0b5d7c;padding:12px 24px;color:#e6f4ff;font-size:12px;text-align:center;'>")
        sb.AppendLine("© " & DateTime.Now.Year.ToString() & " LORO Reparación Automotriz. Todos los derechos reservados.")
        sb.AppendLine("</td></tr>")

        ' Cierre
        sb.AppendLine("</table></td></tr></table></body></html>")
        Return sb.ToString()
    End Function


    Private Function CrearVistaHtmlConLogo(html As String, logoPhysicalPath As String, contentId As String) As AlternateView
        Dim view As AlternateView
        Try
            If File.Exists(logoPhysicalPath) Then
                Dim ext As String = Path.GetExtension(logoPhysicalPath).ToLowerInvariant()
                Dim mediaType As String
                Select Case ext
                    Case ".png" : mediaType = "image/png"
                    Case ".jpg", ".jpeg" : mediaType = MediaTypeNames.Image.Jpeg
                    Case ".gif" : mediaType = MediaTypeNames.Image.Gif
                    Case Else : mediaType = "application/octet-stream"
                End Select

                view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html)

                Dim res As New LinkedResource(logoPhysicalPath)
                res.ContentId = contentId
                res.TransferEncoding = TransferEncoding.Base64
                res.ContentType = New ContentType(mediaType)
                res.ContentType.Name = Path.GetFileName(logoPhysicalPath)
                res.ContentLink = New Uri("cid:" & contentId)

                view.LinkedResources.Add(res)
            Else
                Dim fallbackUrl As String = "https://tu-dominio.com/images/logo-daytona.png"
                Dim htmlConUrl As String = html.Replace("cid:" & contentId, fallbackUrl)
                view = AlternateView.CreateAlternateViewFromString(htmlConUrl, Encoding.UTF8, MediaTypeNames.Text.Html)
            End If

        Catch
            view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html)
        End Try

        Return view
    End Function

    Private Function IsValidEmail(email As String) As Boolean
        Try
            Dim addr = New MailAddress(email)
            Return String.Equals(addr.Address, email, StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function

    Private Function E(s As String) As String
        If s Is Nothing Then Return String.Empty
        Return Server.HtmlEncode(s.Trim())
    End Function

    Private Function QuitarEtiquetas(html As String) As String
        If String.IsNullOrEmpty(html) Then Return String.Empty
        Dim tmp As String = System.Text.RegularExpressions.Regex.Replace(html, "<br\s*/?>", vbCrLf, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        tmp = System.Text.RegularExpressions.Regex.Replace(tmp, "<.*?>", String.Empty, System.Text.RegularExpressions.RegexOptions.Singleline)
        Return System.Web.HttpUtility.HtmlDecode(tmp)
    End Function
End Class
