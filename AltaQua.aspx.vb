Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports Path = System.IO.Path

Public Class AltaQua
    Inherits System.Web.UI.Page

    Private Const TEMP_DIR As String = "~/App_Data/tmp"

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Sugerido en UI (no definitivo) por paridad
            SetExpedienteSugeridoPorParidad()

            ' "Creado por" desde el Master
            Dim nombreCreador As String = If(Master IsNot Nothing, Master.CurrentUserName, String.Empty)
            txtCreadoPor.Text = nombreCreador
            txtCreadoPor.ReadOnly = True
            txtCreadoPor.Attributes("readonly") = "readonly"
            txtCreadoPor.CssClass = (txtCreadoPor.CssClass & " bg-light").Trim()

            ' Fecha de creación actual
            Dim ahora = DateTime.Now
            txtFechaCreacion.TextMode = TextBoxMode.DateTimeLocal
            txtFechaCreacion.Text = ahora.ToString("yyyy-MM-ddTHH:mm")
            txtFechaCreacion.ReadOnly = True
            txtFechaCreacion.CssClass = (txtFechaCreacion.CssClass & " bg-light").Trim()

            ' Estatus en blanco visualmente
            If ddlEstatus IsNot Nothing Then ddlEstatus.ClearSelection()
        End If
    End Sub

    ' Se dispara cuando el JS hace click en btnTrigger al cargar el PDF
    Protected Sub BtnTrigger_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Not fupPDF.HasFile Then Exit Sub

        ' === Guardar temporal ODA.pdf para moverlo al Guardar ===
        Dim ext = Path.GetExtension(fupPDF.FileName).ToLowerInvariant()
        If ext <> ".pdf" Then Exit Sub
        Dim dirTmp = Server.MapPath(TEMP_DIR)
        If Not Directory.Exists(dirTmp) Then Directory.CreateDirectory(dirTmp)
        Dim tempName = Guid.NewGuid().ToString("N") & ".pdf"
        Dim tempPhysical = Path.Combine(dirTmp, tempName)
        fupPDF.SaveAs(tempPhysical)
        ViewState("TempPdfRel") = TEMP_DIR.TrimEnd("/"c) & "/" & tempName

        Try
            Using reader As New PdfReader(tempPhysical)
                Dim pagina As Integer = 1

                ' ------- RECTÁNGULOS (coordenadas con origen abajo-izquierda) -------
                ' Estos son los mismos rectángulos que en LeerOrden.aspx.vb
                Dim rNumeroReporte As New Rectangle(85.0F, 1234.69F, 140.04F, 1247.06F)
                Dim rNombreCliente As New Rectangle(23.0F, 1177.18F, 164.38F, 1188.17F)
                Dim rTelefono As New Rectangle(338.0F, 1177.18F, 386.93F, 1188.17F)
                Dim rEmail As New Rectangle(23.0F, 1150.18F, 102.69F, 1161.17F)
                Dim rMarca As New Rectangle(23.0F, 1055.18F, 55.9F, 1066.17F)
                Dim rTipo As New Rectangle(159.0F, 1055.18F, 288.36F, 1066.17F)
                Dim rModelo As New Rectangle(295.0F, 1055.18F, 312.79F, 1066.17F)
                Dim rColor As New Rectangle(331.0F, 1038.96F, 366.0F, 1051.33F)
                Dim rVin As New Rectangle(22.0F, 1025.96F, 113.55F, 1038.33F)
                Dim rPlacas As New Rectangle(331.0F, 1025.96F, 362.02F, 1038.33F)

                ' ------- LECTURA DE CADA REGIÓN -------
                txtNumeroReporte.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rNumeroReporte))
                txtNombreCliente.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rNombreCliente))
                txtTelefono.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rTelefono))
                txtEmail.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rEmail))
                txtMarca.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rMarca))
                txtTipo.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rTipo))
                txtModelo.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rModelo))
                txtColor.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rColor))
                txtVin.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rVin))
                txtPlacas.Text = Limpiar(ExtraerTextoRegion(reader, pagina, rPlacas))

                ' Copiar número de reporte a siniestro
                txtSiniestroGen.Text = txtNumeroReporte.Text
            End Using
        Catch ex As Exception
            ' Manejo de error silencioso o puedes agregar un Label para mostrar el mensaje
        End Try

        ' Recalcular expediente sugerido después de cargar PDF
        SetExpedienteSugeridoPorParidad()
    End Sub

    ''' <summary>
    ''' Extrae el texto de una región específica usando iTextSharp.
    ''' </summary>
    Private Function ExtraerTextoRegion(reader As PdfReader,
                                       numPagina As Integer,
                                       region As Rectangle) As String
        Dim filtro As New RegionTextRenderFilter(region)
        Dim estrategia As ITextExtractionStrategy =
            New FilteredTextRenderListener(New LocationTextExtractionStrategy(), filtro)

        Dim texto As String = PdfTextExtractor.GetTextFromPage(reader, numPagina, estrategia)
        Return texto
    End Function

    ''' <summary>
    ''' Limpia saltos de línea y espacios extra.
    ''' </summary>
    Private Function Limpiar(texto As String) As String
        If String.IsNullOrWhiteSpace(texto) Then
            Return ""
        End If

        texto = texto.Replace(vbCr, " ").Replace(vbLf, " ")
        While texto.Contains("  ")
            texto = texto.Replace("  ", " ")
        End While

        Return texto.Trim()
    End Function

    ' Guardado en la tabla ADMISIONES
    Protected Sub btnGuardar_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' Obtener la cadena de conexión del web.config
            Dim csSetting = ConfigurationManager.ConnectionStrings("DaytonaDB")
            If csSetting Is Nothing OrElse String.IsNullOrWhiteSpace(csSetting.ConnectionString) Then
                Alert("Falta la cadena de conexión DaytonaDB en Web.config")
                Exit Sub
            End If

            ' CreadoPor desde el textbox o Master
            Dim creadoPorNombre As String = If(Not String.IsNullOrWhiteSpace(txtCreadoPor.Text), txtCreadoPor.Text.Trim(),
                                            If(Master IsNot Nothing, Master.CurrentUserName, String.Empty))

            ' Obtener la paridad del usuario actual
            Dim paridad As String = ObtenerParidadUsuarioActual()

            ' Generar expediente seguro con sp_getapplock
            Dim expedienteId As Integer
            Try
                expedienteId = ObtenerSiguienteExpedienteSeguro(paridad, csSetting.ConnectionString)
            Catch ex As Exception
                Alert("No se pudo asignar el número de expediente: " & ex.Message.Replace("'", "\'"))
                Exit Sub
            End Try

            ' Formatear expediente y actualizar el textbox
            txtExpediente.Text = expedienteId.ToString("0")

            ' Formatear expediente a 5 dígitos: "00001", "00002", etc.
            Dim idFormateado As String = expedienteId.ToString("D5")

            ' Construir nombre de carpeta: "EXP 00001 MARCA TIPO MODELO COLOR PLACAS" (igual que Alta.aspx)
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

            ' Ruta virtual y física de la carpeta
            Dim baseVirtual = GetInbursaBaseVirtual().TrimEnd("/"c)   ' p.ej. ~/INBURSA
            Dim carpetaRel As String = (baseVirtual & "/" & carpetaNombre).Replace("//", "/")
            Dim carpetaFisica As String = Server.MapPath(carpetaRel)

            ' Crear carpeta principal
            If Not Directory.Exists(carpetaFisica) Then Directory.CreateDirectory(carpetaFisica)

            ' Crear las 10 subcarpetas estándar (igual que en Alta.aspx)
            For Each subc In SubcarpetasInbursa
                Dim rel = carpetaRel & "/" & SanitizeFileName(subc)
                Dim phy = Server.MapPath(rel)
                If Not Directory.Exists(phy) Then Directory.CreateDirectory(phy)
            Next

            ' === Mover ODA.pdf desde TEMP si existe ===
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

            ' Guardar en la base de datos con CarpetaRel
            Using cn As New SqlConnection(csSetting.ConnectionString)
                Const sqlInsert As String = "
            INSERT INTO dbo.Admisiones
            (
                Expediente, CreadoPor, FechaCreacion, SiniestroGen, TipoIngreso, DeducibleSI_NO, Estatus,
                Asegurado, Telefono, Correo,
                Reporte,
                Marca, Tipo, Modelo, Color, Serie, Placas,
                CarpetaRel
            )
            VALUES
            (
                @Expediente, @CreadoPor, @FechaCreacion, @SiniestroGen, @TipoIngreso, @DeducibleSI_NO, @Estatus,
                @Asegurado, @Telefono, @Correo,
                @Reporte,
                @Marca, @Tipo, @Modelo, @Color, @Serie, @Placas,
                @CarpetaRel
            );"

                Using cmd As New SqlCommand(sqlInsert, cn)
                    cmd.CommandType = CommandType.Text

                    ' Generación de Expediente
                    cmd.Parameters.Add("@Expediente", SqlDbType.NVarChar, 50).Value = txtExpediente.Text.Trim()
                    cmd.Parameters.Add("@CreadoPor", SqlDbType.NVarChar, 100).Value = creadoPorNombre.Trim()
                    cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = DateTime.Now
                    cmd.Parameters.Add("@SiniestroGen", SqlDbType.NVarChar, 50).Value = txtSiniestroGen.Text.Trim()
                    cmd.Parameters.Add("@TipoIngreso", SqlDbType.NVarChar, 20).Value = If(ddlTipoIngreso.SelectedValue, String.Empty)
                    cmd.Parameters.Add("@DeducibleSI_NO", SqlDbType.NVarChar, 2).Value = If(ddlDeducible.SelectedValue, String.Empty)
                    cmd.Parameters.Add("@Estatus", SqlDbType.NVarChar, 20).Value = If(ddlEstatus.SelectedValue, String.Empty)

                    ' Datos del Cliente (extraídos del PDF)
                    cmd.Parameters.Add("@Asegurado", SqlDbType.NVarChar, 150).Value = txtNombreCliente.Text.Trim()
                    cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 50).Value = txtTelefono.Text.Trim()
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = txtEmail.Text.Trim()
                    cmd.Parameters.Add("@Reporte", SqlDbType.NVarChar, 50).Value = txtNumeroReporte.Text.Trim()

                    ' Datos del Vehículo (extraídos del PDF)
                    cmd.Parameters.Add("@Marca", SqlDbType.NVarChar, 50).Value = marcaClean
                    cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 50).Value = txtTipo.Text.Trim()
                    cmd.Parameters.Add("@Modelo", SqlDbType.NVarChar, 20).Value = txtModelo.Text.Trim()
                    cmd.Parameters.Add("@Color", SqlDbType.NVarChar, 30).Value = txtColor.Text.Trim()
                    cmd.Parameters.Add("@Serie", SqlDbType.NVarChar, 50).Value = txtVin.Text.Trim()
                    cmd.Parameters.Add("@Placas", SqlDbType.NVarChar, 20).Value = txtPlacas.Text.Trim()

                    ' Carpeta relativa
                    cmd.Parameters.Add("@CarpetaRel", SqlDbType.NVarChar, 300).Value = carpetaRel

                    cn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Redirigir o mostrar mensaje de éxito
            Alert("Expediente guardado exitosamente.")
            Response.Redirect("AltaQua.aspx?success=1")

        Catch ex As Exception
            ' Mostrar mensaje de error detallado al usuario
            Alert("Error al guardar: " & ex.Message.Replace("'", "\'"))
        End Try
    End Sub

    ''' <summary>
    ''' Muestra un mensaje de alerta en el navegador usando JavaScript.
    ''' </summary>
    Private Sub Alert(msg As String)
        ClientScript.RegisterStartupScript(Me.GetType(), "msg", "alert('" & msg.Replace("'", "\'") & "');", True)
    End Sub

    ' ==================== FUNCIONES DE PARIDAD PAR/NON ====================

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

    ' ==================== FUNCIONES PARA CREACIÓN DE CARPETAS ====================

    ''' <summary>
    ''' Obtiene el siguiente expediente de forma segura usando sp_getapplock para evitar duplicados.
    ''' </summary>
    Private Function ObtenerSiguienteExpedienteSeguro(paridad As String, connectionString As String) As Integer
        Dim lockName As String = "ExpedienteSecuence_" & paridad.ToUpperInvariant()
        Dim expedienteId As Integer = 0

        Using cn As New SqlConnection(connectionString)
            cn.Open()

            Using txn = cn.BeginTransaction()
                Try
                    ' 1) Adquirir lock
                    Using cmdLock As New SqlCommand("sp_getapplock", cn, txn)
                        cmdLock.CommandType = CommandType.StoredProcedure
                        cmdLock.Parameters.Add("@Resource", SqlDbType.NVarChar, 255).Value = lockName
                        cmdLock.Parameters.Add("@LockMode", SqlDbType.NVarChar, 32).Value = "Exclusive"
                        cmdLock.Parameters.Add("@LockOwner", SqlDbType.NVarChar, 32).Value = "Transaction"
                        cmdLock.Parameters.Add("@LockTimeout", SqlDbType.Int).Value = 5000

                        Dim retLock As Object = cmdLock.ExecuteScalar()
                        Dim lockResult As Integer = If(retLock IsNot Nothing AndAlso retLock IsNot DBNull.Value,
                                                       Convert.ToInt32(retLock), -999)
                        If lockResult < 0 Then
                            Throw New InvalidOperationException("No se pudo adquirir el lock para generar expediente PAR/NON.")
                        End If
                    End Using

                    ' 2) Leer el último expediente de esa paridad
                    Dim sqlQuery As String
                    If paridad.ToUpperInvariant() = "PAR" Then
                        sqlQuery = "
                            SELECT MAX(TRY_CONVERT(INT, Expediente))
                            FROM dbo.Admisiones
                            WHERE TRY_CONVERT(INT, Expediente) IS NOT NULL
                              AND TRY_CONVERT(INT, Expediente) % 2 = 0"
                    Else
                        sqlQuery = "
                            SELECT MAX(TRY_CONVERT(INT, Expediente))
                            FROM dbo.Admisiones
                            WHERE TRY_CONVERT(INT, Expediente) IS NOT NULL
                              AND TRY_CONVERT(INT, Expediente) % 2 = 1"
                    End If

                    Using cmdMax As New SqlCommand(sqlQuery, cn, txn)
                        Dim maxVal = cmdMax.ExecuteScalar()
                        If maxVal IsNot Nothing AndAlso maxVal IsNot DBNull.Value Then
                            expedienteId = Convert.ToInt32(maxVal) + 2
                        Else
                            ' Primer expediente de esta paridad
                            expedienteId = If(paridad.ToUpperInvariant() = "PAR", 2, 1)
                        End If
                    End Using

                    ' 3) Liberar lock
                    Using cmdRelease As New SqlCommand("sp_releaseapplock", cn, txn)
                        cmdRelease.CommandType = CommandType.StoredProcedure
                        cmdRelease.Parameters.Add("@Resource", SqlDbType.NVarChar, 255).Value = lockName
                        cmdRelease.Parameters.Add("@LockOwner", SqlDbType.NVarChar, 32).Value = "Transaction"
                        cmdRelease.ExecuteNonQuery()
                    End Using

                    txn.Commit()
                Catch ex As Exception
                    Try
                        txn.Rollback()
                    Catch
                    End Try
                    Throw
                End Try
            End Using
        End Using

        Return expedienteId
    End Function

    ''' <summary>
    ''' Remueve paréntesis de un texto.
    ''' </summary>
    Private Function RemoveParentheses(text As String) As String
        If String.IsNullOrWhiteSpace(text) Then Return String.Empty
        text = text.Replace("(", "").Replace(")", "")
        Return text.Trim()
    End Function

    ''' <summary>
    ''' Limpia el texto de marca.
    ''' </summary>
    Private Function CleanMarca(text As String) As String
        If String.IsNullOrWhiteSpace(text) Then Return String.Empty
        Return text.Trim().ToUpperInvariant()
    End Function

    ''' <summary>
    ''' Sanitiza un nombre de archivo/carpeta reemplazando caracteres inválidos.
    ''' </summary>
    Private Function SanitizeFileName(name As String) As String
        If String.IsNullOrWhiteSpace(name) Then Return String.Empty

        Dim invalidos = Path.GetInvalidFileNameChars()
        For Each c In invalidos
            name = name.Replace(c, "_"c)
        Next

        Return name.Trim()
    End Function

    ''' <summary>
    ''' Obtiene la ruta virtual base de INBURSA desde Web.config o usa predeterminada.
    ''' </summary>
    Private Function GetInbursaBaseVirtual() As String
        Dim v = ConfigurationManager.AppSettings("InbursaBaseVirtual")
        If String.IsNullOrWhiteSpace(v) Then v = "~/INBURSA"
        Return v.TrimEnd("/"c)
    End Function

End Class
