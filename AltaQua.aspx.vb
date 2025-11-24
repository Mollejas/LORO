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

    ' Subcarpetas estándar para cada expediente en INBURSA
    Private Shared ReadOnly SubcarpetasInbursa As String() = {
        "1. DOCUMENTOS DE INGRESO",
        "2. FOTOS DIAGNOSTICO MECANICA",
        "3. FOTOS DIAGNOSTICO HOJALATERIA",
        "4. VALUACION",
        "5. REFACCIONES",
        "6. FOTOS PROCESO DE REPARACION",
        "7. FOTOS DE SALIDA",
        "8. FACTURACION",
        "9. FOTOS DE RECLAMACION",
        "10. FOTOS REINGRESO DE TRANSITO"
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
        If Not fupPDF.HasFile Then
            Exit Sub
        End If

        Dim ext As String = Path.GetExtension(fupPDF.FileName).ToLowerInvariant()
        If ext <> ".pdf" Then
            Exit Sub
        End If

        ' Carpeta temporal para guardar el volante
        Dim tempFolder As String = Server.MapPath("~/App_Data/Uploads")
        If Not Directory.Exists(tempFolder) Then
            Directory.CreateDirectory(tempFolder)
        End If

        Dim fileName As String = Path.GetFileName(fupPDF.FileName)
        Dim fullPath As String = Path.Combine(tempFolder, fileName)
        fupPDF.SaveAs(fullPath)

        ' Guardar la ruta del PDF temporal en ViewState para usarla al guardar
        ViewState("TempPdfPath") = fullPath

        Try
            Using reader As New PdfReader(fullPath)
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
                Exit Sub
            End If

            ' CreadoPor desde el textbox o Master
            Dim creadoPorNombre As String = If(Not String.IsNullOrWhiteSpace(txtCreadoPor.Text), txtCreadoPor.Text.Trim(),
                                            If(Master IsNot Nothing, Master.CurrentUserName, String.Empty))

            ' Obtener la paridad del usuario actual
            Dim paridad As String = ObtenerParidadUsuarioActual()

            ' Generar expediente seguro con sp_getapplock
            Dim expedienteId As Integer = ObtenerSiguienteExpedienteSeguro(paridad, csSetting.ConnectionString)

            ' Formatear expediente a 5 dígitos: "00001", "00002", etc.
            Dim idFormateado As String = expedienteId.ToString("D5")

            ' Construir nombre de carpeta: "EXP 00001 MARCA TIPO MODELO COLOR PLACAS"
            Dim marcaClean As String = CleanMarca(RemoveParentheses(txtMarca.Text))
            Dim partes As New List(Of String)()
            If Not String.IsNullOrWhiteSpace(marcaClean) Then partes.Add(marcaClean)
            If Not String.IsNullOrWhiteSpace(txtTipo.Text) Then partes.Add(txtTipo.Text.Trim().ToUpperInvariant())
            If Not String.IsNullOrWhiteSpace(txtModelo.Text) Then partes.Add(txtModelo.Text.Trim().ToUpperInvariant())
            If Not String.IsNullOrWhiteSpace(txtColor.Text) Then partes.Add(txtColor.Text.Trim().ToUpperInvariant())
            If Not String.IsNullOrWhiteSpace(txtPlacas.Text) Then partes.Add(txtPlacas.Text.Trim().ToUpperInvariant())

            Dim carpetaNombre As String = "EXP " & idFormateado & " " & String.Join(" ", partes)
            carpetaNombre = SanitizeFileName(carpetaNombre)

            ' Ruta virtual y física de la carpeta
            Dim baseVirtual As String = GetInbursaBaseVirtual().TrimEnd("/"c)
            Dim carpetaRel As String = (baseVirtual & "/" & carpetaNombre).Replace("//", "/")
            Dim carpetaFisica As String = Server.MapPath(carpetaRel)

            ' Crear carpeta principal
            If Not Directory.Exists(carpetaFisica) Then
                Directory.CreateDirectory(carpetaFisica)
            End If

            ' Crear las 10 subcarpetas estándar
            For Each subcarpeta In SubcarpetasInbursa
                Dim rutaSub As String = Path.Combine(carpetaFisica, subcarpeta)
                If Not Directory.Exists(rutaSub) Then
                    Directory.CreateDirectory(rutaSub)
                End If
            Next

            ' Mover el PDF temporal a "1. DOCUMENTOS DE INGRESO/ODA.pdf"
            Dim tempPdfPath As String = TryCast(ViewState("TempPdfPath"), String)
            If Not String.IsNullOrWhiteSpace(tempPdfPath) AndAlso File.Exists(tempPdfPath) Then
                Dim destinoFolder As String = Path.Combine(carpetaFisica, "1. DOCUMENTOS DE INGRESO")
                Dim destinoPdf As String = Path.Combine(destinoFolder, "ODA.pdf")

                ' Si ya existe ODA.pdf, eliminar
                If File.Exists(destinoPdf) Then
                    File.Delete(destinoPdf)
                End If

                File.Move(tempPdfPath, destinoPdf)
                ViewState("TempPdfPath") = Nothing
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
                    cmd.Parameters.Add("@Expediente", SqlDbType.NVarChar, 50).Value = idFormateado
                    cmd.Parameters.Add("@CreadoPor", SqlDbType.NVarChar, 100).Value = If(String.IsNullOrWhiteSpace(creadoPorNombre), DBNull.Value, CType(creadoPorNombre.Trim(), Object))
                    cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = DateTime.Now
                    cmd.Parameters.Add("@SiniestroGen", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtSiniestroGen.Text), DBNull.Value, CType(txtSiniestroGen.Text.Trim(), Object))
                    cmd.Parameters.Add("@TipoIngreso", SqlDbType.NVarChar, 20).Value = If(String.IsNullOrWhiteSpace(ddlTipoIngreso.SelectedValue), DBNull.Value, CType(ddlTipoIngreso.SelectedValue, Object))
                    cmd.Parameters.Add("@DeducibleSI_NO", SqlDbType.NVarChar, 10).Value = If(String.IsNullOrWhiteSpace(ddlDeducible.SelectedValue), DBNull.Value, CType(ddlDeducible.SelectedValue, Object))
                    cmd.Parameters.Add("@Estatus", SqlDbType.NVarChar, 20).Value = If(String.IsNullOrWhiteSpace(ddlEstatus.SelectedValue), DBNull.Value, CType(ddlEstatus.SelectedValue, Object))

                    ' Datos del Cliente (extraídos del PDF)
                    cmd.Parameters.Add("@Asegurado", SqlDbType.NVarChar, 200).Value = If(String.IsNullOrWhiteSpace(txtNombreCliente.Text), DBNull.Value, CType(txtNombreCliente.Text.Trim(), Object))
                    cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtTelefono.Text), DBNull.Value, CType(txtTelefono.Text.Trim(), Object))
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = If(String.IsNullOrWhiteSpace(txtEmail.Text), DBNull.Value, CType(txtEmail.Text.Trim().ToLowerInvariant(), Object))
                    cmd.Parameters.Add("@Reporte", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtNumeroReporte.Text), DBNull.Value, CType(txtNumeroReporte.Text.Trim(), Object))

                    ' Datos del Vehículo (extraídos del PDF)
                    cmd.Parameters.Add("@Marca", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtMarca.Text), DBNull.Value, CType(txtMarca.Text.Trim(), Object))
                    cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 100).Value = If(String.IsNullOrWhiteSpace(txtTipo.Text), DBNull.Value, CType(txtTipo.Text.Trim(), Object))
                    cmd.Parameters.Add("@Modelo", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtModelo.Text), DBNull.Value, CType(txtModelo.Text.Trim(), Object))
                    cmd.Parameters.Add("@Color", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtColor.Text), DBNull.Value, CType(txtColor.Text.Trim(), Object))
                    cmd.Parameters.Add("@Serie", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtVin.Text), DBNull.Value, CType(txtVin.Text.Trim(), Object))
                    cmd.Parameters.Add("@Placas", SqlDbType.NVarChar, 20).Value = If(String.IsNullOrWhiteSpace(txtPlacas.Text), DBNull.Value, CType(txtPlacas.Text.Trim(), Object))

                    ' Carpeta relativa
                    cmd.Parameters.Add("@CarpetaRel", SqlDbType.NVarChar, 500).Value = carpetaRel

                    cn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Redirigir o mostrar mensaje de éxito
            Response.Redirect("AltaQua.aspx?success=1")

        Catch ex As Exception
            ' Manejo de errores - aquí puedes mostrar un mensaje al usuario
            ' Por ejemplo, agregando un Label en el .aspx y asignando el mensaje:
            ' lblError.Text = "Error al guardar: " & ex.Message
        End Try
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
        Dim baseVirtual As String = ConfigurationManager.AppSettings("InbursaBaseVirtual")
        If String.IsNullOrWhiteSpace(baseVirtual) Then
            baseVirtual = "~/INBURSA"
        End If
        Return baseVirtual
    End Function

End Class
