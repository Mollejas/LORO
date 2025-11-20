Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.IO
Imports System.Linq
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.Script.Serialization
' PDFsharp
Imports PdfSharp.Pdf
Imports PdfSharp
Imports PdfSharp.Drawing
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.UI.HtmlControls

' Evitar conflicto con iTextSharp Image
Imports DrawingImage = System.Drawing.Image
Imports System.Globalization

Partial Public Class Hoja
    Inherits System.Web.UI.Page

    ' ===================== API: Gate de diagnóstico (Mec/Hoj) =====================
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json), WebMethod()>
    Public Shared Function SetDiagGate(admisionId As Integer, area As String, enabled As Boolean) As Object
        Try
            Dim col As String = If(area, "").Trim().ToLowerInvariant()
            Dim field As String = Nothing
            If col = "mec" OrElse col = "mecanica" Then
                field = "MecSi"
            ElseIf col = "hoja" OrElse col = "hojalateria" Then
                field = "HojaSi"
            Else
                Return New With {.ok = False, .msg = "Área inválida (use MEC/HOJA)."}
            End If

            Using cn As New SqlConnection(GetCs())
                cn.Open()
                Dim sql As String = $"UPDATE dbo.admisiones SET {field}=@v WHERE Id=@id;"
                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.AddWithValue("@v", If(enabled, 1, 0))
                    cmd.Parameters.AddWithValue("@id", admisionId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Return New With {.ok = True}
        Catch ex As Exception
            Return New With {.ok = False, .msg = ex.Message}
        End Try
    End Function
    ' ===================== /API: Gate de diagnóstico =====================




    ' ===================== API REFACCIONES (PageMethods) =====================
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json), WebMethod()>
    Public Shared Function RefacLoadItems(admisionId As Integer, area As String, categoria As String) As Object
        Try
            EnsureRefaccionesTable() ' si ya existe, no hace nada

            Dim items As New List(Of RefacItem)()

            Using cn As New SqlConnection(GetCs())
                cn.Open()

                Using cmd As New SqlCommand("
                SELECT Id,
                       Expediente,
                       AdmisionId,
                       Area,
                       Categoria,
                       Cantidad,
                       Descripcion,
                       Autorizado,
                       Checar,
                       Aldo,
                       AldoDateTime
                FROM dbo.refacciones
                WHERE AdmisionId = @id
                  AND UPPER(Area) = UPPER(@area)
                  AND UPPER(Categoria) = UPPER(@cat)
                ORDER BY Id DESC;", cn)

                    cmd.Parameters.AddWithValue("@id", admisionId)
                    cmd.Parameters.AddWithValue("@area", area)
                    cmd.Parameters.AddWithValue("@cat", categoria)

                    Using rd = cmd.ExecuteReader()
                        While rd.Read()
                            items.Add(New RefacItem With {
                            .id = CInt(rd("Id")),
                            .expediente = Convert.ToString(rd("Expediente")),
                            .admision_id = CInt(rd("AdmisionId")),                    ' ← nombre correcto
                            .area = Convert.ToString(rd("Area")),
                            .categoria = Convert.ToString(rd("Categoria")),
                            .cantidad = If(rd.IsDBNull(rd.GetOrdinal("Cantidad")), 0, Convert.ToInt32(rd("Cantidad"))),
                            .descripcion = Convert.ToString(rd("Descripcion")),
                            .autorizado = If(rd.IsDBNull(rd.GetOrdinal("Autorizado")), False, Convert.ToBoolean(rd("Autorizado"))),
                            .checar = If(rd.IsDBNull(rd.GetOrdinal("Checar")), False, Convert.ToBoolean(rd("Checar"))),
                            .aldo = If(rd.IsDBNull(rd.GetOrdinal("Aldo")), False, Convert.ToBoolean(rd("Aldo"))),
                            .aldo_datetime = If(rd.IsDBNull(rd.GetOrdinal("AldoDateTime")), CType(Nothing, DateTime?), CType(rd("AldoDateTime"), DateTime)) ' ← nombre correcto
                        })
                        End While
                    End Using
                End Using
            End Using

            Return New With {.ok = True, .items = items}
        Catch ex As Exception
            Return New With {.ok = False, .msg = ex.Message}
        End Try
    End Function



    <ScriptMethod(ResponseFormat:=ResponseFormat.Json), WebMethod()>
    Public Shared Function RefacAddItem(admisionId As Integer,
                                    area As String,
                                    categoria As String,
                                    cantidad As Integer,
                                    descripcion As String) As Object
        Try
            EnsureRefaccionesTable()

            ' Obtiene el Expediente de admisiones
            Dim expediente As String = ""
            Using cn As New SqlConnection(GetCs())
                cn.Open()
                Using c1 As New SqlCommand("SELECT Expediente FROM admisiones WHERE Id=@Id", cn)
                    c1.Parameters.AddWithValue("@Id", admisionId)
                    expediente = Convert.ToString(c1.ExecuteScalar())
                End Using

                If String.IsNullOrWhiteSpace(expediente) Then
                    Return New With {.ok = False, .msg = "No se encontró el expediente de la admisión."}
                End If

                Using cmd As New SqlCommand("
                INSERT INTO dbo.refacciones
                    (expediente, admisionid, area, categoria, cantidad, descripcion, autorizado, checar, aldo, aldodatetime)
                VALUES
                    (@exp, @adm, UPPER(@area), UPPER(@cat), @cant, @desc, 0, 0, 0, NULL);
                SELECT SCOPE_IDENTITY();", cn)

                    cmd.Parameters.AddWithValue("@exp", expediente)
                    cmd.Parameters.AddWithValue("@adm", admisionId)
                    cmd.Parameters.AddWithValue("@area", area)
                    cmd.Parameters.AddWithValue("@cat", categoria)
                    cmd.Parameters.AddWithValue("@cant", cantidad)
                    cmd.Parameters.AddWithValue("@desc", descripcion)

                    Dim newId = Convert.ToInt32(Convert.ToDecimal(cmd.ExecuteScalar()))
                    Return New With {.ok = True, .id = newId}
                End Using
            End Using
        Catch ex As Exception
            Return New With {.ok = False, .msg = ex.Message}
        End Try
    End Function

    <ScriptMethod(ResponseFormat:=ResponseFormat.Json), WebMethod()>
    Public Shared Function RefacUpdateFlags(itemId As Integer,
                                        autorizado As Nullable(Of Boolean),
                                        checar As Nullable(Of Boolean),
                                        aldo As Nullable(Of Boolean)) As Object
        Try
            EnsureRefaccionesTable()

            Dim sets As New List(Of String)
            If autorizado.HasValue Then sets.Add("autorizado=@aut")
            If checar.HasValue Then sets.Add("checar=@chk")
            If aldo.HasValue Then
                sets.Add("aldo=@ald")
                If aldo.Value Then
                    sets.Add("aldodatetime=GETDATE()")
                Else
                    sets.Add("aldodatetime=NULL")
                End If
            End If

            If sets.Count = 0 Then
                Return New With {.ok = True}
            End If

            Using cn As New SqlConnection(GetCs())
                cn.Open()
                Dim sql As String = "UPDATE dbo.refacciones SET " & String.Join(", ", sets) & " WHERE id=@id;"
                Using cmd As New SqlCommand(sql, cn)
                    cmd.Parameters.AddWithValue("@id", itemId)
                    If autorizado.HasValue Then cmd.Parameters.AddWithValue("@aut", If(autorizado.Value, 1, 0))
                    If checar.HasValue Then cmd.Parameters.AddWithValue("@chk", If(checar.Value, 1, 0))
                    If aldo.HasValue Then cmd.Parameters.AddWithValue("@ald", If(aldo.Value, 1, 0))
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Return New With {.ok = True}
        Catch ex As Exception
            Return New With {.ok = False, .msg = ex.Message}
        End Try
    End Function

    ' ----------------- Helpers y modelos (Shared) -----------------
    Private Shared Function GetCs() As String
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
            Throw New Exception("No se encontró la cadena de conexión 'DaytonaDB'.")
        End If
        Return cs.ConnectionString
    End Function

    <ScriptMethod(ResponseFormat:=ResponseFormat.Json), WebMethod()>
    Public Shared Function RefacDelete(id As Integer) As Object
        Try
            Using cn As New SqlConnection(GetCs())
                cn.Open()
                Using cmd As New SqlCommand("DELETE FROM dbo.refacciones WHERE Id=@id;", cn)
                    cmd.Parameters.AddWithValue("@id", id)
                    Dim rows = cmd.ExecuteNonQuery()
                    Return New With {.ok = (rows > 0), .deleted = rows}
                End Using
            End Using
        Catch ex As Exception
            Return New With {.ok = False, .msg = ex.Message}
        End Try
    End Function


    Private Shared Sub EnsureRefaccionesTable()
        Using cn As New SqlConnection(GetCs())
            cn.Open()
            Dim sql As String =
"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='refacciones' AND schema_id=SCHEMA_ID('dbo'))
BEGIN
  CREATE TABLE dbo.refacciones (
      id             INT IDENTITY(1,1) PRIMARY KEY,
      expediente     NVARCHAR(50) NOT NULL,
      admisionid    INT          NOT NULL,
      area           NVARCHAR(20) NOT NULL,
      categoria      NVARCHAR(20) NOT NULL,
      cantidad       INT          NOT NULL,
      descripcion    NVARCHAR(400) NOT NULL,
      autorizado     BIT          NOT NULL DEFAULT(0),
      checar         BIT          NOT NULL DEFAULT(0),
      aldo           BIT          NOT NULL DEFAULT(0),
      aldodatetime  DATETIME     NULL,
      created_at     DATETIME     NOT NULL DEFAULT (GETDATE()),
      created_by     NVARCHAR(100) NULL
  );
  CREATE INDEX IX_refacciones_adm_area_cat ON dbo.refacciones(admision_id, area, categoria);
  CREATE INDEX IX_refacciones_exp ON dbo.refacciones(expediente);
END"
            Using cmd As New SqlCommand(sql, cn)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Class RefacItem
        Public Property id As Integer
        Public Property expediente As String
        Public Property admision_id As Integer
        Public Property area As String
        Public Property categoria As String
        Public Property cantidad As Integer
        Public Property descripcion As String
        Public Property autorizado As Boolean
        Public Property checar As Boolean
        Public Property aldo As Boolean
        Public Property aldo_datetime As DateTime?
    End Class
    ' ===================== /API REFACCIONES =====================


    Private Const MAX_SIDE As Integer = 1600
    Private Const JPEG_QUALITY As Long = 88
    Private Const SUBFOLDER_NAME As String = "1. DOCUMENTOS DE INGRESO"
    Private Const PRINCIPAL_NAME As String = "principal.jpg"
    Private Const SUBFOLDER_PRESUP As String = "1. DOCUMENTOS DE INGRESO" ' 👈 antes "3. FOTOS DE PRESUPUESTO"
    Private Const PREFIX_PRESUP As String = "recep"


    Private Const SUBFOLDER_INGRESO As String = "1. DOCUMENTOS DE INGRESO"
    Private Const ODA_FILENAME As String = "oda.pdf"

    Private _fechaCreacion As Date? = Nothing

    ' ===== Helpers =====
    Private Function FindControlRecursive(root As Control, id As String) As Control
        If root Is Nothing Then Return Nothing
        Dim ctl As Control = root.FindControl(id)
        If ctl IsNot Nothing Then Return ctl
        For Each c As Control In root.Controls
            Dim t = FindControlRecursive(c, id)
            If t IsNot Nothing Then Return t
        Next
        Return Nothing
    End Function

    Private Function ObtenerSubcarpetaPresupuestoFisica() As String
        Dim carpetaBaseFisica As String = ResolverCarpetaFisica(hidCarpeta.Value)
        Return Path.Combine(carpetaBaseFisica, SUBFOLDER_PRESUP) ' 👈 ahora apunta a "1. DOCUMENTOS DE INGRESO"
    End Function

    Private Function ObtenerSiguienteIndicePorPrefijo(folder As String, prefijo As String) As Integer
        If Not Directory.Exists(folder) Then Return 1
        Dim maxN As Integer = 0
        Dim regex As New Regex("^" & Regex.Escape(prefijo) & "(\d+)\.jpg$", RegexOptions.IgnoreCase)
        For Each f In Directory.GetFiles(folder, prefijo & "*.jpg")
            Dim name = Path.GetFileName(f)
            Dim m = regex.Match(name)
            If m.Success Then
                Dim n As Integer
                If Integer.TryParse(m.Groups(1).Value, n) AndAlso n > maxN Then maxN = n
            End If
        Next
        Return maxN + 1
    End Function

    Protected Sub btnGuardarMultiplesPresup_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub

        Dim carpetaDestinoFisica As String = ObtenerSubcarpetaPresupuestoFisica()
        If Not Directory.Exists(carpetaDestinoFisica) Then Directory.CreateDirectory(carpetaDestinoFisica)

        Dim archivos = Request.Files
        Dim indice As Integer = ObtenerSiguienteIndicePorPrefijo(carpetaDestinoFisica, PREFIX_PRESUP)

        For i As Integer = 0 To archivos.Count - 1
            If archivos.AllKeys(i) Is Nothing Then Continue For
            If Not archivos.AllKeys(i).Equals("fuMultiplesPresup", StringComparison.OrdinalIgnoreCase) Then Continue For

            Dim file As HttpPostedFile = archivos(i)
            If file Is Nothing OrElse file.ContentLength <= 0 Then Continue For

            Dim ext As String = Path.GetExtension(file.FileName).ToLower()
            Dim permitidas = New String() {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"}
            If Not permitidas.Contains(ext) Then Continue For

            Using ms As New MemoryStream()
                file.InputStream.CopyTo(ms)
                Dim bytes As Byte() = ms.ToArray()
                Dim salidaJpg As Byte() = A_Jpeg_Redimensionado(bytes, MAX_SIDE, MAX_SIDE, JPEG_QUALITY)
                Dim nombre As String = $"{PREFIX_PRESUP}{indice}.jpg"
                Dim rutaFinal As String = Path.Combine(carpetaDestinoFisica, nombre)
                System.IO.File.WriteAllBytes(rutaFinal, salidaJpg)
                indice += 1
            End Using
        Next

        UpdateBottomWidgets()

        ' Reabrir modal y mostrar 100%
        Dim js As String =
          "(function(){var el=document.getElementById('modalMultiplesPresup');if(!el)return;var m=bootstrap.Modal.getOrCreateInstance(el);m.show();" &
          "var wrap=document.getElementById('fotosPresupProgressWrap');var bar=document.getElementById('fotosPresupProgressBar');var msg=document.getElementById('fotosPresupStatus');" &
          "if(wrap)wrap.classList.remove('d-none');if(bar){bar.classList.remove('progress-bar-animated');bar.style.width='100%';bar.setAttribute('aria-valuenow','100');bar.textContent='100%';}" &
          "if(msg){msg.classList.remove('d-none');msg.textContent='Fotos guardadas exitosamente';}" &
          "var inpt=document.getElementById('fuMultiplesPresup');if(inpt)inpt.value='';var thumbs=document.getElementById('thumbsPresup');if(thumbs)thumbs.innerHTML='';})();"
        EmitStartupScript("fotosPresupSaved", js)
    End Sub

    Private Function GetFU(id As String) As FileUpload
        Return TryCast(FindControlRecursive(Me, id), FileUpload)
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim sid As String = Request.QueryString("id")
            If String.IsNullOrWhiteSpace(sid) OrElse Not sid.All(AddressOf Char.IsDigit) Then Exit Sub

            hidId.Value = sid
            lblId.Text = sid

            CargarAdmision(Convert.ToInt32(sid))
            UpdateBottomWidgets()
            PrefillCtModal()
            UpdateInvGruaButtons()
            PintarTilesRecepcion()
        Else
            If ViewState("FechaCreacion") IsNot Nothing Then
                _fechaCreacion = CType(ViewState("FechaCreacion"), DateTime)
            End If
            UpdateInvGruaButtons()
        End If

        UpdateUIFromPrincipal()
        UpdateBottomWidgets()
        UpdateMetaLabels()

        ' >>> ADD: Pintar tile MECÁNICA en cada carga (usa id de admisión actual)
        Dim admId As Integer
        Dim idStr As String = If(Not IsPostBack, Request.QueryString("id"), hidId.Value)
        If Integer.TryParse(idStr, admId) Then
            PintarTileMecanica(admId)
            CargarFinesDiagnostico(admId)
        End If
        ' <<< ADD

        ' Accept para INV GRÚA / INE
        Dim fuInv As FileUpload = TryCast(FindControlRecursive(Me, "fuInvGruaPdf"), FileUpload)
        If fuInv IsNot Nothing Then fuInv.Attributes("accept") = "application/pdf"

        Dim fuPdf As FileUpload = GetFU("fuInePdf")
        If fuPdf IsNot Nothing Then fuPdf.Attributes("accept") = "application/pdf"

        Dim fuFront As FileUpload = GetFU("fuIneFront")
        If fuFront IsNot Nothing Then
            fuFront.Attributes("accept") = "image/*"
            fuFront.Attributes("capture") = "environment"
        End If

        Dim fuBack As FileUpload = GetFU("fuIneBack")
        If fuBack IsNot Nothing Then
            fuBack.Attributes("accept") = "image/*"
            fuBack.Attributes("capture") = "environment"
        End If

        Dim fuCompl As FileUpload = TryCast(FindControlRecursive(Me, "fuComplementos"), FileUpload)
        If fuCompl IsNot Nothing Then fuCompl.Attributes("accept") = "application/pdf"
    End Sub


    '====================== Datos de admisión ======================
    Private _marca As String = "", _version As String = "", _anio As String = "", _placas As String = ""
    Private _telefono As String = "", _correo As String = "", _siniestro As String = ""
    Private lblInvGruaInfo As Object

    Private Sub CargarAdmision(id As Integer)
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing Then Exit Sub

        Using cn As New SqlConnection(cs.ConnectionString)
            cn.Open()
            Using cmd As New SqlCommand("SELECT * FROM admisiones WHERE Id=@Id", cn)
                cmd.Parameters.AddWithValue("@Id", id)
                Using rd = cmd.ExecuteReader()
                    If Not rd.Read() Then Exit Sub

                    Dim expediente As String = GetStr(rd, "Expediente")
                    _siniestro = CoalesceNonEmpty(GetStr(rd, "SiniestroIdent"), GetStr(rd, "SiniestroGen"))
                    Dim asegurado As String = CoalesceNonEmpty(GetStr(rd, "Asegurado"), GetStr(rd, "AseguradoNombre"))
                    _telefono = GetStr(rd, "Telefono")
                    _correo = GetStr(rd, "Correo")

                    Dim reporte As String = GetStr(rd, "Reporte")

                    _marca = GetStr(rd, "Marca")
                    _version = GetStr(rd, "Tipo")
                    _anio = GetStr(rd, "Modelo")
                    Dim color As String = GetStr(rd, "Color")
                    _placas = GetStr(rd, "Placas")

                    Dim carpetarel As String = GetStr(rd, "carpetarel")

                    ViewState("marca") = _marca
                    ViewState("version") = _version
                    ViewState("anio") = _anio
                    ViewState("color") = color
                    ViewState("placas") = _placas

                    lblExpediente.Text = expediente
                    lblSiniestro.Text = _siniestro
                    lblAsegurado.Text = asegurado
                    lblTelefono.Text = _telefono
                    lblCorreo.Text = _correo
                    lblVehiculo.Text = BuildVehiculo(_marca, _version, _anio, color, _placas)
                    lblReporte.Text = reporte

                    hidCarpeta.Value = carpetarel
                    lblCarpeta.Text = If(String.IsNullOrWhiteSpace(carpetarel), "(sin carpeta)", carpetarel)

                    Dim fcRaw As String = GetStr(rd, "FechaCreacion")
                    If Not String.IsNullOrWhiteSpace(fcRaw) Then
                        Dim fc As DateTime
                        If DateTime.TryParse(fcRaw, fc) Then
                            _fechaCreacion = fc
                            ViewState("FechaCreacion") = fc
                        End If
                    End If

                    ' --- AGREGAR: leer flags de diagnóstico y pre-checar los checkboxes ---
                    Dim mecSi As Boolean = SafeReadBool(rd, "MecSi", "mecsi")
                    Dim hojaSi As Boolean = SafeReadBool(rd, "HojaSi", "hojasi")

                    Dim chkM As CheckBox = TryCast(FindControlRecursive(Me, "chkMecSi"), CheckBox)
                    If chkM IsNot Nothing Then chkM.Checked = mecSi

                    Dim chkH As CheckBox = TryCast(FindControlRecursive(Me, "chkHojaSi"), CheckBox)
                    If chkH IsNot Nothing Then chkH.Checked = hojaSi
                    ' --- /AGREGAR ---


                End Using
            End Using
        End Using
    End Sub

    Private Function SafeReadBool(rd As IDataRecord, ParamArray colNames() As String) As Boolean
        For Each col In colNames
            Try
                Dim i = rd.GetOrdinal(col)
                If Not rd.IsDBNull(i) Then
                    Dim v = rd.GetValue(i)
                    If TypeOf v Is Boolean Then Return CType(v, Boolean)
                    If TypeOf v Is Integer OrElse TypeOf v Is Int16 OrElse TypeOf v Is Int32 Then
                        Return Convert.ToInt32(v) <> 0
                    End If
                    Dim s As String = Convert.ToString(v).Trim().ToLowerInvariant()
                    If s = "1" OrElse s = "true" OrElse s = "sí" OrElse s = "si" Then Return True
                    If s = "0" OrElse s = "false" Then Return False
                End If
            Catch
                ' intenta con el siguiente nombre
            End Try
        Next
        Return False
    End Function

    Private Sub UpdateMetaLabels()
        Dim lblFC As Label = TryCast(FindControlRecursive(Me, "lblFechaCreacion"), Label)
        Dim lblDias As Label = TryCast(FindControlRecursive(Me, "lblDiasTranscurridos"), Label)
        Dim lblExtra As Label = TryCast(FindControlRecursive(Me, "lblMeta3"), Label)

        If _fechaCreacion.HasValue Then
            If lblFC IsNot Nothing Then lblFC.Text = _fechaCreacion.Value.ToString("dd/MM/yyyy HH:mm")
            Dim ts As TimeSpan = DateTime.Now.Date - _fechaCreacion.Value.Date
            Dim dias As Integer = Math.Max(0, CInt(Math.Floor(ts.TotalDays)))
            If lblDias IsNot Nothing Then
                lblDias.Text = If(dias = 0, "hoy", If(dias = 1, "1 día", dias & " días"))
            End If
        Else
            If lblFC IsNot Nothing Then lblFC.Text = "—"
            If lblDias IsNot Nothing Then lblDias.Text = "—"
        End If

        If lblExtra IsNot Nothing Then lblExtra.Text = "—"
    End Sub

    Private Sub PrefillCtModal()
        Dim tb As TextBox

        tb = TryCast(FindControlRecursive(Me, "txtCtFecha"), TextBox)
        If tb IsNot Nothing Then tb.Text = Date.Now.ToString("yyyy-MM-dd")

        tb = TryCast(FindControlRecursive(Me, "txtCtSiniestro"), TextBox)
        If tb IsNot Nothing Then tb.Text = _siniestro

        tb = TryCast(FindControlRecursive(Me, "txtCtMarca"), TextBox)
        If tb IsNot Nothing Then tb.Text = _marca

        tb = TryCast(FindControlRecursive(Me, "txtCtVersion"), TextBox)
        If tb IsNot Nothing Then tb.Text = _version

        tb = TryCast(FindControlRecursive(Me, "txtCtAnio"), TextBox)
        If tb IsNot Nothing Then tb.Text = _anio

        tb = TryCast(FindControlRecursive(Me, "txtCtPlacas"), TextBox)
        If tb IsNot Nothing Then tb.Text = _placas

        tb = TryCast(FindControlRecursive(Me, "txtCtTel"), TextBox)
        If tb IsNot Nothing Then tb.Text = _telefono

        tb = TryCast(FindControlRecursive(Me, "txtCtCel"), TextBox)
        If tb IsNot Nothing Then tb.Text = _telefono

        tb = TryCast(FindControlRecursive(Me, "txtCtCorreo"), TextBox)
        If tb IsNot Nothing Then tb.Text = _correo
    End Sub

    Private Function BuildVehiculo(marca As String, version As String, modelo As String, color As String, placas As String) As String
        Dim partes As New List(Of String)
        If Not String.IsNullOrWhiteSpace(marca) Then partes.Add(marca.Trim())
        If Not String.IsNullOrWhiteSpace(version) Then partes.Add(version.Trim())
        If Not String.IsNullOrWhiteSpace(modelo) Then partes.Add(modelo.Trim())
        If Not String.IsNullOrWhiteSpace(color) Then partes.Add(color.Trim())
        If Not String.IsNullOrWhiteSpace(placas) Then partes.Add("Placas: " & placas.Trim())
        Return String.Join(" ", partes)
    End Function

    Private Function GetStr(rd As IDataRecord, col As String) As String
        Dim ordinal As Integer
        Try
            ordinal = rd.GetOrdinal(col)
        Catch
            Return ""
        End Try
        If rd.IsDBNull(ordinal) Then Return ""
        Return Convert.ToString(rd.GetValue(ordinal)).Trim()
    End Function

    Private Function CoalesceNonEmpty(ParamArray vals() As String) As String
        For Each v In vals
            If Not String.IsNullOrWhiteSpace(v) Then Return v
        Next
        Return ""
    End Function

    '====================== principal.jpg ======================
    Private Sub UpdateUIFromPrincipal()
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            imgPreview.ImageUrl = ""
            btnEliminarPrincipal.Visible = False
            Exit Sub
        End If

        Dim ruta As String = Path.Combine(ObtenerSubcarpetaDestinoFisica(), "principal.jpg")
        If System.IO.File.Exists(ruta) Then
            Dim bytes = System.IO.File.ReadAllBytes(ruta)
            imgPreview.ImageUrl = "data:image/jpeg;base64," & Convert.ToBase64String(bytes)
            btnEliminarPrincipal.Visible = True
        Else
            imgPreview.ImageUrl = ""
            btnEliminarPrincipal.Visible = False
        End If
    End Sub

    Protected Sub btnEliminarPrincipal_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
        Dim ruta As String = Path.Combine(ObtenerSubcarpetaDestinoFisica(), PRINCIPAL_NAME)
        Try
            If System.IO.File.Exists(ruta) Then System.IO.File.Delete(ruta)
        Catch
        End Try
        UpdateUIFromPrincipal()
        UpdateBottomWidgets()
    End Sub

    '====================== Barra inferior: estados ======================
    Private Sub UpdateBottomWidgets()
        ' ====== ODA ======
        Dim enableVerOda As Boolean = False
        If Not String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            enableVerOda = System.IO.File.Exists(Path.Combine(ObtenerSubcarpetaDestinoFisica(), "ODA.pdf"))
        End If
        btnVerODA.Enabled = enableVerOda : ToggleCss(btnVerODA, "disabled", Not enableVerOda)

        ' ====== INE ======
        Dim enableVerIne As Boolean = False
        If Not String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            enableVerIne = System.IO.File.Exists(Path.Combine(ObtenerSubcarpetaDestinoFisica(), "INE.pdf"))
        End If
        btnVerINE.Enabled = enableVerIne : ToggleCss(btnVerINE, "disabled", Not enableVerIne)

        ' ====== CT ======
        Dim enableVerCT As Boolean = False
        If Not String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            enableVerCT = System.IO.File.Exists(Path.Combine(ObtenerSubcarpetaDestinoFisica(), "CT.pdf"))
        End If
        btnVerCT.Enabled = enableVerCT : ToggleCss(btnVerCT, "disabled", Not enableVerCT)

        ' ====== INV (PDF de inventario del taller) ======
        Dim enableVerInv As Boolean = False
        If Not String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            enableVerInv = System.IO.File.Exists(Path.Combine(ObtenerSubcarpetaDestinoFisica(), "inv.pdf"))
        End If
        btnVerINV.Enabled = enableVerInv : ToggleCss(btnVerINV, "disabled", Not enableVerInv)

        ' ====== COMPLEMENTOS (inetransito.pdf) ======
        Dim enableVerCompl As Boolean = False
        If Not String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            enableVerCompl = System.IO.File.Exists(Path.Combine(ObtenerSubcarpetaDestinoFisica(), "inetransito.pdf"))
        End If

        ' Botón visor: btnVerInetransito
        Dim _btnVerCompl As LinkButton = TryCast(FindControlRecursive(Me, "btnVerInetransito"), LinkButton)
        If _btnVerCompl IsNot Nothing Then
            _btnVerCompl.Enabled = enableVerCompl
            ToggleCss(_btnVerCompl, "disabled", Not enableVerCompl)
        End If

        ' ====== Tiles (referencias) ======
        Dim tileComplCtrl As HtmlGenericControl = TryCast(FindControlRecursive(Me, "tileComplementos"), HtmlGenericControl)
        Dim tileOdaCtrl As HtmlGenericControl = TryCast(FindControlRecursive(Me, "tileODA"), HtmlGenericControl)
        Dim tileFotosCtrl As HtmlGenericControl = TryCast(FindControlRecursive(Me, "TileFotos"), HtmlGenericControl)
        Dim tileIneCtrl As HtmlGenericControl = TryCast(FindControlRecursive(Me, "tileINE"), HtmlGenericControl)
        Dim tileCtCtrl As HtmlGenericControl = TryCast(FindControlRecursive(Me, "tileCT"), HtmlGenericControl)
        Dim tileInvCtrl As HtmlGenericControl = TryCast(FindControlRecursive(Me, "tileINV"), HtmlGenericControl)

        ' Pintar OK de Complementos por disponibilidad real
        SetTileOk(tileComplCtrl, enableVerCompl)

        ' ====== Fotos ingreso (presupuesto) ======
        Dim canViewFotosPresup As Boolean = False
        If Not String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            Dim fPresup As String = ObtenerSubcarpetaPresupuestoFisica()
            If Directory.Exists(fPresup) Then
                Dim countPresup As Integer = Directory.GetFiles(fPresup, PREFIX_PRESUP & "*.jpg", SearchOption.TopDirectoryOnly).Length
                canViewFotosPresup = (countPresup > 0)
            End If
        End If
        btnVerFotosPresup.Enabled = canViewFotosPresup
        ToggleCss(btnVerFotosPresup, "disabled", Not canViewFotosPresup)

        ' ====== INV GRÚA (estado ya calculado en otros flujos) ======
        Dim invGruaOk As Boolean = btnVerInvGrua.Enabled

        ' ====== Estatus de Admisión: TRANSITO o PISO ======
        Dim admId As Integer, estatus As String = ""
        If Integer.TryParse(lblId.Text, admId) Then
            estatus = GetAdmEstatusById(admId)
        End If
        Dim isTransito As Boolean =
        estatus.Equals("TRANSITO", StringComparison.OrdinalIgnoreCase) _
        OrElse estatus.Equals("TRÁNSITO", StringComparison.OrdinalIgnoreCase)

        ' Helper: habilitar/deshabilitar visualmente un tile
        Dim enableTile As Action(Of HtmlGenericControl, Boolean, String, String) =
        Sub(tile As HtmlGenericControl, enable As Boolean, titleOn As String, titleOff As String)
            If tile Is Nothing Then Exit Sub
            Dim cls As String = If(tile.Attributes("class"), "")
            If enable Then
                If cls.Contains("tile-disabled") Then
                    tile.Attributes("class") = cls.Replace("tile-disabled", "").Trim()
                End If
                tile.Attributes("aria-disabled") = "false"
                If Not String.IsNullOrEmpty(titleOn) Then
                    tile.Attributes("title") = titleOn
                Else
                    tile.Attributes.Remove("title")
                End If
            Else
                If Not cls.Contains("tile-disabled") Then
                    tile.Attributes("class") = (cls & " tile-disabled").Trim()
                End If
                tile.Attributes("aria-disabled") = "true"
                If Not String.IsNullOrEmpty(titleOff) Then
                    tile.Attributes("title") = titleOff
                End If
                SetTileOk(tile, False) ' al deshabilitar, quita OK
            End If
        End Sub

        ' ====== Reglas de habilitación de TILES por estatus ======
        If isTransito Then
            ' TRANSITO: habilitados ODA, FOTOS, INE, CT, COMPLEMENTOS; deshabilitado INV
            enableTile(tileOdaCtrl, True, "Disponible", "No disponible")
            enableTile(tileFotosCtrl, True, "Disponible", "No disponible")
            enableTile(tileIneCtrl, True, "Disponible", "No disponible")
            enableTile(tileCtCtrl, True, "Disponible", "No disponible")
            enableTile(tileComplCtrl, True, "Disponible", "No disponible")

            enableTile(tileInvCtrl, False, "", "INV no requerido (estatus: TRÁNSITO)")
            btnVerINV.Enabled = False : ToggleCss(btnVerINV, "disabled", True)
        Else
            ' PISO: único tile deshabilitado es CT; todos los demás habilitados
            enableTile(tileOdaCtrl, True, "Disponible", "No disponible")
            enableTile(tileFotosCtrl, True, "Disponible", "No disponible")
            enableTile(tileIneCtrl, True, "Disponible", "No disponible")
            enableTile(tileComplCtrl, True, "Disponible", "No disponible")
            enableTile(tileInvCtrl, True, "Disponible", "No disponible")

            enableTile(tileCtCtrl, False, "", "CT deshabilitado (estatus: PISO)")
            ' En PISO no forzamos btnVerINV; se respeta su estado real
        End If

        ' ====== Condición para MOSTRAR DIAGNÓSTICO ======
        Dim diagOk As Boolean
        If isTransito Then
            ' TRANSITO: ODA + FOTOS + INE + CT
            diagOk = (enableVerOda AndAlso canViewFotosPresup AndAlso enableVerIne AndAlso enableVerCT)
        Else
            ' PISO: ODA + FOTOS + INE + INV (acepta INV o INV GRÚA)
            diagOk = (enableVerOda AndAlso canViewFotosPresup AndAlso enableVerIne AndAlso (enableVerInv OrElse invGruaOk))
        End If

        ' ====== Blink/badge: rojo si NO cumple condiciones ======
        Dim jsBlink As String =
        "try{ setRecepcionBlink(" & If(diagOk, "false", "true") & "); }catch(e){ if(window.__refreshRecepVisualState) __refreshRecepVisualState(); }"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "recepBlink_" & Guid.NewGuid().ToString("N"), jsBlink, True)

        ' ====== Inyecta flags al cliente (Estatus + booleans + override) ======
        Dim jsFlags As New Text.StringBuilder()
        jsFlags.Append("window.__isTransito=").Append(If(isTransito, "true", "false")).Append(";"c)
        jsFlags.Append("window.__diagSrv={")
        jsFlags.Append("oda:").Append(If(enableVerOda, "true", "false")).Append(","c)
        jsFlags.Append("fotos:").Append(If(canViewFotosPresup, "true", "false")).Append(","c)
        jsFlags.Append("ine:").Append(If(enableVerIne, "true", "false")).Append(","c)
        jsFlags.Append("ct:").Append(If(enableVerCT, "true", "false")).Append(","c)
        jsFlags.Append("inv:").Append(If((enableVerInv OrElse invGruaOk), "true", "false"))
        jsFlags.Append("};")
        jsFlags.Append("window.__forceDiagVisible=").Append(If(diagOk, "true", "false")).Append(";"c)

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "diagStateFlags_" & Guid.NewGuid().ToString("N"), jsFlags.ToString(), True)

        ' ====== Mostrar/Ocultar “Tira de Diagnóstico” (robusto para postback parcial) ======
        Dim showStr As String = If(diagOk, "true", "false")
        Dim jsDiag As String =
        "try{" &
        "  if (typeof setDiagStripVisible==='function'){" &
        "    setDiagStripVisible(" & showStr & ");" &
        "  } else {" &
        "    window.__forceDiagVisible=" & showStr & ";" &
        "    var wrap=document.getElementById('diagSection');" &
        "    var el=document.getElementById('stripDiag');" &
        "    if(wrap){ if(" & showStr & "){ wrap.classList.remove('d-none'); } else { wrap.classList.add('d-none'); } }" &
        "    if(el){" &
        "      if(" & showStr & "){" &
        "        el.classList.remove('d-none'); el.classList.remove('is-collapsed'); el.style.maxHeight='';" &
        "      } else {" &
        "        el.classList.add('d-none');" &
        "      }" &
        "    }" &
        "  }" &
        "}catch(_){};" &
        "try{ if (window.__refreshDiagVisibility) { window.__refreshDiagVisibility(); } }catch(e){};" &
        "setTimeout(function(){ try{ if (window.__refreshDiagVisibility) { window.__refreshDiagVisibility(); } }catch(e){} }, 80);"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "recepDiagToggle_" & Guid.NewGuid().ToString("N"), jsDiag, True)

        ' ====== IniDiag + Labels ======
        If admId > 0 Then
            If diagOk Then
                EnsureIniDiagRecordedIfNeeded(admId)
            End If
            LoadDiagLabels(admId)
        End If

        ' ====== Pintar "OK" por disponibilidad real ======
        SetTileOk(tileOdaCtrl, enableVerOda)
        SetTileOk(tileFotosCtrl, canViewFotosPresup)
        SetTileOk(tileIneCtrl, enableVerIne)
        SetTileOk(tileCtCtrl, enableVerCT)

        ' INV OK: en TRANSITO no se pinta OK; en PISO depende (INV o grúa)
        Dim invOk As Boolean = (enableVerInv OrElse invGruaOk)
        If isTransito Then
            SetTileOk(tileInvCtrl, False)
        Else
            SetTileOk(tileInvCtrl, invOk)
        End If

        ' Complementos OK ya seteado al inicio con SetTileOk(tileComplCtrl, enableVerCompl)
    End Sub








    Protected Sub btnInvHtml_Click(sender As Object, e As EventArgs) Handles btnInvHtml.Click
        Dim url As String = ResolveUrl("~/inventario.html?id=" & Server.UrlEncode(lblId.Text))

        Dim vMarca As String = CStr(ViewState("marca"))
        Dim vVersion As String = CStr(ViewState("version"))
        Dim vAnio As String = CStr(ViewState("anio"))
        Dim vColor As String = CStr(ViewState("color"))
        Dim vPlacas As String = CStr(ViewState("placas"))

        Dim payload = New With {
            .expediente = lblExpediente.Text.Trim(),
            .siniestro = lblSiniestro.Text.Trim(),
            .asegurado = lblAsegurado.Text.Trim(),
            .telefono = lblTelefono.Text.Trim(),
            .correo = lblCorreo.Text.Trim(),
            .reporte = lblReporte.Text.Trim(),
            .vehiculo = lblVehiculo.Text.Trim(),
            .marca = If(vMarca, String.Empty).Trim(),
            .modelo = If(vVersion, String.Empty).Trim(),
            .anio = If(vAnio, String.Empty).Trim(),
            .auto_color = If(vColor, String.Empty).Trim(),
            .placas = If(vPlacas, String.Empty).Trim()
        }
        Dim json As String = New JavaScriptSerializer().Serialize(payload)

        Dim script As String =
$"(function(){{
   var iframe = document.getElementById('invFrame');
   var modal  = new bootstrap.Modal(document.getElementById('invModal'));
   iframe.src = '{url.Replace("'", "\'")}';
   iframe.onload = function(){{
     iframe.contentWindow.postMessage({{ type: 'INV_PREFILL', payload: {json} }}, window.location.origin);
   }};
   modal.show();
}})();"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "openInvPM", script, True)
    End Sub

    ' === Galería (se usa para “presup”) ===
    Private Sub AbrirGaleria(pattern As String)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub

        Dim baseFolder As String
        If pattern.StartsWith("presup", StringComparison.OrdinalIgnoreCase) Then
            baseFolder = ObtenerSubcarpetaPresupuestoFisica()
        Else
            baseFolder = ObtenerSubcarpetaDestinoFisica()
        End If
        If Not Directory.Exists(baseFolder) Then Exit Sub

        Dim files = Directory.GetFiles(baseFolder, pattern & "*.jpg", SearchOption.TopDirectoryOnly) _
                        .OrderBy(Function(p) p) _
                        .ToList()
        If files.Count = 0 Then
            UpdateBottomWidgets()
            Exit Sub
        End If

        Const MAX_IMGS As Integer = 300
        If files.Count > MAX_IMGS Then files = files.Take(MAX_IMGS).ToList()

        Dim sb As New System.Text.StringBuilder()
        Dim idText As String = lblId.Text
        Dim encCarp As String = System.Web.HttpUtility.UrlEncode(hidCarpeta.Value)

        For Each p In files
            Dim fileName As String = Path.GetFileName(p)
            Dim encName As String = System.Web.HttpUtility.UrlEncode(fileName)

            Dim thumbUrl As String = ResolveUrl(
                "~/ImageThumb.ashx?id=" & idText &
                "&name=" & encName &
                "&s=t" &
                "&carpetaRel=" & encCarp
            )

            Dim fullUrl As String = ResolveUrl(
                "~/ImageThumb.ashx?id=" & idText &
                "&name=" & encName &
                "&s=m" &
                "&carpetaRel=" & encCarp
            )

            sb.Append("<div class='grid-item'>")
            sb.Append("<div class='grid-check-wrap'>")
            sb.Append("<input type='checkbox' class='grid-check' data-name='").Append(HttpUtility.HtmlAttributeEncode(fileName)).Append("' />")
            sb.Append("</div>")
            sb.Append("<img class='grid-thumb' src='").Append(thumbUrl).Append("' data-full='").Append(fullUrl).Append("' alt='' />")
            sb.Append("</div>")
        Next

        litFotosGrid.Text = sb.ToString()

        Dim js As String =
            "var first=document.querySelector('#fotosGrid .grid-thumb');" &
            "if(first){document.getElementById('galleryBigImg').src=first.getAttribute('data-full')||first.src;}" &
            "new bootstrap.Modal(document.getElementById('fotosModal')).show();"
        EmitStartupScript("openGaleria", js)
    End Sub

    Protected Sub btnVerFotosPresup_Click(sender As Object, e As EventArgs)
        AbrirGaleria(PREFIX_PRESUP)
    End Sub

    Private Sub ToggleCss(ctrl As System.Web.UI.Control, cls As String, addClass As Boolean)
        Dim wb = TryCast(ctrl, System.Web.UI.WebControls.WebControl)
        If wb Is Nothing Then Return
        Dim cur As String = If(wb.CssClass, String.Empty).Trim()
        Dim parts = cur.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries).ToList()
        Dim has As Boolean = parts.Contains(cls)
        If addClass AndAlso Not has Then parts.Add(cls)
        If Not addClass AndAlso has Then parts.Remove(cls)
        wb.CssClass = String.Join(" ", parts)
    End Sub

    '====================== Versión PDFs ======================
    Private Function GetPdfVersion(kind As String) As String
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Return String.Empty
        Dim baseFolder As String = ObtenerSubcarpetaDestinoFisica()
        Dim pdfName As String
        Select Case kind.ToLowerInvariant()
            Case "ine" : pdfName = "INE.pdf"
            Case "oda" : pdfName = "ODA.pdf"
            Case "ct" : pdfName = "CT.pdf"
            Case "inv" : pdfName = "inv.pdf"
            Case "inetransito" : pdfName = "inetransito.pdf"
            Case Else : Return String.Empty
        End Select
        Dim pdfPath As String = Path.Combine(baseFolder, pdfName)
        If Not System.IO.File.Exists(pdfPath) Then Return String.Empty
        Dim fi As New System.IO.FileInfo(pdfPath)
        Return fi.LastWriteTimeUtc.ToString("yyyyMMddHHmmssffff")
    End Function

    '====================== ODA: Ver ======================
    Protected Sub btnVerODA_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
        Dim v As String = GetPdfVersion("oda")
        Dim baseUrl As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=oda"
        Dim url As String = ResolveUrl(baseUrl & If(v <> "", "&v=" & v, ""))
        EmitStartupScript("openPdfOda", "openSmartViewer('" & url.Replace("'", "\'") & "');")
    End Sub

    '====================== INE: Subir PDF directo ======================
    Protected Sub btnUploadInePdfGo_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
        Dim fuPdf As FileUpload = GetFU("fuInePdf")
        If fuPdf Is Nothing OrElse Not fuPdf.HasFile Then Exit Sub
        If Path.GetExtension(fuPdf.FileName).ToLower() <> ".pdf" Then Exit Sub

        Dim folder As String = ObtenerSubcarpetaDestinoFisica()
        If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)
        Dim destino As String = Path.Combine(folder, "INE.pdf")
        fuPdf.SaveAs(destino)
        UpdateBottomWidgets()
        Dim v As String = GetPdfVersion("ine")
        Dim baseUrl As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=ine"
        Dim url As String = ResolveUrl(baseUrl & If(v <> "", "&v=" & v, ""))
        EmitStartupScript("autoOpenIne", "openSmartViewer('" & url.Replace("'", "\'") & "');")
    End Sub

    '====================== INE: 2 fotos -> INE.pdf ======================
    Protected Sub btnIneCamaraGuardar_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub

        Dim fuFront As FileUpload = GetFU("fuIneFront")
        Dim fuBack As FileUpload = GetFU("fuIneBack")

        Dim frontBytes As Byte() = Nothing
        Dim backBytes As Byte() = Nothing

        If fuFront IsNot Nothing AndAlso fuFront.HasFile Then frontBytes = LeerComoJpeg(fuFront)
        If fuBack IsNot Nothing AndAlso fuBack.HasFile Then backBytes = LeerComoJpeg(fuBack)

        If (frontBytes Is Nothing OrElse frontBytes.Length = 0) AndAlso (backBytes Is Nothing OrElse backBytes.Length = 0) Then Exit Sub

        Dim folder As String = ObtenerSubcarpetaDestinoFisica()
        If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)
        Dim destino As String = Path.Combine(folder, "INE.pdf")

        GuardarDosImagenesEnUnaHojaPdf_PdfSharp(frontBytes, backBytes, destino)

        UpdateBottomWidgets()

        EmitStartupScript("hideIneCam",
            "var m=bootstrap.Modal.getInstance(document.getElementById('modalIneCamara')); if(m){m.hide();}" &
            "var wrap=document.getElementById('ineProgressWrap');var bar=document.getElementById('ineProgressBar');var msg=document.getElementById('ineStatus');" &
            "if(wrap)wrap.classList.remove('d-none');" &
            "if(bar){bar.classList.remove('progress-bar-animated');bar.style.width='100%';bar.setAttribute('aria-valuenow','100');bar.textContent='100%';}" &
            "if(msg){msg.classList.remove('d-none');msg.textContent='INE.pdf generado correctamente';}"
        )

        Dim v2 As String = GetPdfVersion("ine")
        Dim baseUrl2 As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=ine"
        Dim url2 As String = ResolveUrl(baseUrl2 & If(v2 <> "", "&v=" & v2, ""))
        EmitStartupScript("autoOpenIneFromCam", "openSmartViewer('" & url2.Replace("'", "\'") & "');")
    End Sub

    '====================== INE: Ver ======================
    Protected Sub btnVerINE_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
        Dim v As String = GetPdfVersion("ine")
        Dim baseUrl As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=ine"
        Dim url As String = ResolveUrl(baseUrl & If(v <> "", "&v=" & v, ""))
        EmitStartupScript("openIneViewer", "openSmartViewer('" & url.Replace("'", "\'") & "');")
    End Sub

    '====================== CT ======================
    Protected Sub btnCtGuardar_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub

        Dim fecha As Date = Date.Now
        Dim txt As TextBox

        txt = TryCast(FindControlRecursive(Me, "txtCtFecha"), TextBox)
        If txt IsNot Nothing Then
            Dim d As Date
            If Date.TryParse(txt.Text, d) Then fecha = d
        End If

        Dim siniestro As String = TryGetText("txtCtSiniestro", _siniestro)
        Dim marca As String = TryGetText("txtCtMarca", _marca)
        Dim version As String = TryGetText("txtCtVersion", _version)
        Dim anio As String = TryGetText("txtCtAnio", _anio)
        Dim placas As String = TryGetText("txtCtPlacas", _placas)
        Dim tel As String = TryGetText("txtCtTel", _telefono)
        Dim cel As String = TryGetText("txtCtCel", _telefono)
        Dim correo As String = TryGetText("txtCtCorreo", _correo)

        Dim hfCli = TryCast(FindControlRecursive(Me, "hfFirmaCliente"), HiddenField)
        Dim hfSup = TryCast(FindControlRecursive(Me, "hfFirmaSupervisor"), HiddenField)
        Dim firmaClienteDataUrl As String = If(hfCli IsNot Nothing, Request.Form(hfCli.UniqueID), Nothing)
        Dim firmaSupDataUrl As String = If(hfSup IsNot Nothing, Request.Form(hfSup.UniqueID), Nothing)

        Dim firmaCliente As Byte() = DataUrlToBytes(firmaClienteDataUrl)
        Dim firmaSupervisor As Byte() = DataUrlToBytes(firmaSupDataUrl)

        Dim folder As String = ObtenerSubcarpetaDestinoFisica()
        If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)
        Dim destino As String = Path.Combine(folder, "CT.pdf")

        GuardarCartaTransitoPdf(destino, fecha, siniestro, marca, version, anio, placas, tel, cel, correo, firmaCliente, firmaSupervisor)

        UpdateBottomWidgets()
        EmitStartupScript("hideCt", "var m=bootstrap.Modal.getInstance(document.getElementById('modalCt')); if(m){m.hide();}")

        Dim vct As String = GetPdfVersion("ct")
        Dim baseCt As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=ct"
        Dim urlCt As String = ResolveUrl(baseCt & If(vct <> "", "&v=" & vct, ""))
        EmitStartupScript("autoOpenCt", "openSmartViewer('" & urlCt.Replace("'", "\'") & "');")
    End Sub

    Protected Sub btnVerCT_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
        Dim v As String = GetPdfVersion("ct")
        Dim baseUrl As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=ct"
        Dim url As String = ResolveUrl(baseUrl & If(v <> "", "&v=" & v, ""))
        EmitStartupScript("openCtViewer", "openSmartViewer('" & url.Replace("'", "\'") & "');")
    End Sub

    Private Function TryGetText(id As String, fallback As String) As String
        Dim t As TextBox = TryCast(FindControlRecursive(Me, id), TextBox)
        If t IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(t.Text) Then Return t.Text.Trim()
        Return fallback
    End Function

    '====================== PDF makers ======================
    Private Sub GuardarDosImagenesEnUnaHojaPdf_PdfSharp(frontBytes As Byte(), backBytes As Byte(), rutaPdf As String)
        Dim folder As String = Path.GetDirectoryName(rutaPdf)
        If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)

        Dim doc As New PdfDocument()
        doc.Info.Title = "INE"
        Dim page As PdfPage = doc.AddPage()
        page.Size = PageSize.A4

        Using gfx As XGraphics = XGraphics.FromPdfPage(page)
            Dim left As Double = 20.0, right As Double = 20.0, top As Double = 20.0, bottom As Double = 20.0, gap As Double = 12.0
            Dim pageWidth As Double = page.Width.Point, pageHeight As Double = page.Height.Point
            Dim availW As Double = pageWidth - left - right
            Dim availH As Double = pageHeight - top - bottom

            If (frontBytes IsNot Nothing) AndAlso (backBytes IsNot Nothing) Then
                Dim halfH As Double = (availH - gap) / 2.0
                DibujarImagenCentradaEnRegion(gfx, frontBytes, left, top, availW, halfH)
                DibujarImagenCentradaEnRegion(gfx, backBytes, left, top + halfH + gap, availW, halfH)
            Else
                Dim onlyBytes As Byte() = If(frontBytes, backBytes)
                DibujarImagenCentradaEnRegion(gfx, onlyBytes, left, top, availW, availH)
            End If
        End Using

        doc.Save(rutaPdf)
        doc.Close()
    End Sub

    Private Sub GuardarCartaTransitoPdf(rutaPdf As String,
                                    fecha As Date,
                                    siniestro As String,
                                    marca As String,
                                    version As String,
                                    anio As String,
                                    placas As String,
                                    tel As String,
                                    cel As String,
                                    correo As String,
                                    firmaCliente As Byte(),
                                    firmaSupervisor As Byte())

        Dim folder As String = Path.GetDirectoryName(rutaPdf)
        If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)

        Dim doc As New PdfDocument()
        doc.Info.Title = "Carta de Tránsito"
        Dim page As PdfPage = doc.AddPage()
        page.Size = PageSize.A4

        ' Fuentes (no son IDisposable)
        Dim fontTitle As New XFont("Arial", 18, XFontStyle.Bold)
        Dim fontH As New XFont("Arial", 12, XFontStyle.Bold)
        Dim fontB As New XFont("Arial", 11, XFontStyle.Regular)
        Dim fontS As New XFont("Arial", 9, XFontStyle.Regular)

        ' Pinceles/colores (NO usar Using; XSolidBrush NO es IDisposable)
        Dim brushText As XBrush = XBrushes.Black
        Dim brushBrand As XBrush = New XSolidBrush(XColor.FromArgb(0, 59, 120))
        Dim penLight As New XPen(XColors.LightGray, 0.8)
        Dim penDark As New XPen(XColors.Gray, 0.8)

        Using gfx As XGraphics = XGraphics.FromPdfPage(page)

            ' ====== Encabezado con logo ======
            Dim xMargin As Double = 40.0
            Dim y As Double = 35.0
            Dim usableW As Double = page.Width.Point - (xMargin * 2)

            ' Logo (si existe)
            Try
                Dim logoPath As String = Server.MapPath("~/images/logoinbur.png")
                If System.IO.File.Exists(logoPath) Then
                    Using xi As XImage = XImage.FromFile(logoPath)
                        Dim logoH As Double = 40.0
                        Dim scale As Double = logoH / xi.PointHeight
                        Dim logoW As Double = xi.PointWidth * scale
                        gfx.DrawImage(xi, xMargin, y, logoW, logoH)
                    End Using
                End If
            Catch
                ' Si falla el logo, continuamos sin interrumpir
            End Try

            ' Título y fecha
            gfx.DrawString("Carta de Tránsito", fontTitle, brushBrand, New XRect(xMargin, y, usableW, 28), XStringFormats.TopCenter)
            y += 46
            gfx.DrawString("Fecha: " & fecha.ToString("dd/MM/yyyy"), fontB, brushText, New XRect(xMargin, y, usableW, 18), XStringFormats.TopRight)
            y += 8

            ' Separador
            gfx.DrawLine(penDark, xMargin, y, xMargin + usableW, y)
            y += 16

            ' ====== Cuerpo del texto ======
            Dim pText1 As String =
"Por medio de la presente, hago constar el retiro voluntario del vehículo que, como parte del proceso de atención a la reclamación de mi seguro, se encontrara en espera del dictamen correspondiente. En mi calidad de Asegurado o Apoderado Legal, asumo la responsabilidad por los daños adicionales que pudiera sufrir el vehículo en mi posesión y me comprometo a reingresarlo al Centro de Reparación asignado en un plazo no mayor a 2 días hábiles, a partir de la notificación que reciba por parte de la aseguradora o del Centro de Reparación."
            y = DrawParagraph(gfx, pText1, fontB, brushText, xMargin, y, usableW, 16)

            y += 6
            Dim pText2 As String =
"Asimismo, me comprometo a no realizar reparaciones fuera del Centro de Reparación asignado. Estoy enterado de que el incumplimiento de lo anterior puede derivar en la cancelación del surtido de refacciones asignadas."
            y = DrawParagraph(gfx, pText2, fontB, brushText, xMargin, y, usableW, 16)

            y += 18

            ' ====== Tabla de datos del auto ======
            gfx.DrawString("Datos del vehículo", fontH, brushBrand, New XRect(xMargin, y, usableW, 16), XStringFormats.TopLeft)
            y += 10

            Dim rowH As Double = 22.0
            Dim colW() As Double = {usableW * 0.25, usableW * 0.25, usableW * 0.25, usableW * 0.25}
            Dim colX(3) As Double
            colX(0) = xMargin
            For i = 1 To 3
                colX(i) = colX(i - 1) + colW(i - 1)
            Next

            ' Cabeceras
            Dim headers() As String = {"Siniestro", "Marca", "Versión", "Año"}
            Dim values1() As String = {siniestro, marca, version, anio}
            Dim headers2() As String = {"Placas", "Teléfono", "Celular", "Correo"}
            Dim values2() As String = {placas, tel, cel, correo}

            ' Dibuja una fila (header + valores)
            Dim functionRow = Sub(hdr() As String, vals() As String)
                                  ' Rectángulos
                                  For c = 0 To 3
                                      gfx.DrawRectangle(penLight, New XRect(colX(c), y, colW(c), rowH))
                                  Next
                                  ' Encabezados
                                  For c = 0 To 3
                                      gfx.DrawString(hdr(c), fontS, brushBrand, New XRect(colX(c) + 4, y + 3, colW(c) - 8, rowH - 6), XStringFormats.TopLeft)
                                  Next
                                  y += rowH

                                  For c = 0 To 3
                                      gfx.DrawRectangle(penLight, New XRect(colX(c), y, colW(c), rowH))
                                  Next
                                  For c = 0 To 3
                                      gfx.DrawString(vals(c), fontB, brushText, New XRect(colX(c) + 4, y + 3, colW(c) - 8, rowH - 6), XStringFormats.TopLeft)
                                  Next
                                  y += rowH + 6
                              End Sub

            functionRow(headers, values1)
            functionRow(headers2, values2)

            y += 10

            ' ====== Firmas ======
            Dim boxH As Double = 90
            Dim gap As Double = 40
            Dim fw As Double = (usableW - gap) / 2.0

            ' Cliente
            gfx.DrawRectangle(penLight, New XRect(xMargin, y, fw, boxH))
            gfx.DrawString("Firma del Cliente", fontS, brushText, New XRect(xMargin, y + boxH + 4, fw, 12), XStringFormats.TopCenter)
            If firmaCliente IsNot Nothing AndAlso firmaCliente.Length > 0 Then
                Using ms As New MemoryStream(firmaCliente)
                    Using xi As XImage = XImage.FromStream(ms)
                        Dim scale As Double = Math.Min((fw - 10) / xi.PointWidth, (boxH - 10) / xi.PointHeight)
                        If scale > 1 Then scale = 1
                        Dim dw As Double = xi.PointWidth * scale, dh As Double = xi.PointHeight * scale
                        Dim dx As Double = xMargin + (fw - dw) / 2, dy As Double = y + (boxH - dh) / 2
                        gfx.DrawImage(xi, dx, dy, dw, dh)
                    End Using
                End Using
            End If

            ' Supervisor
            Dim x2 As Double = xMargin + fw + gap
            gfx.DrawRectangle(penLight, New XRect(x2, y, fw, boxH))
            gfx.DrawString("Firma del Asesor", fontS, brushText, New XRect(x2, y + boxH + 4, fw, 12), XStringFormats.TopCenter)
            If firmaSupervisor IsNot Nothing AndAlso firmaSupervisor.Length > 0 Then
                Using ms As New MemoryStream(firmaSupervisor)
                    Using xi As XImage = XImage.FromStream(ms)
                        Dim scale As Double = Math.Min((fw - 10) / xi.PointWidth, (boxH - 10) / xi.PointHeight)
                        If scale > 1 Then scale = 1
                        Dim dw As Double = xi.PointWidth * scale, dh As Double = xi.PointHeight * scale
                        Dim dx As Double = x2 + (fw - dw) / 2, dy As Double = y + (boxH - dh) / 2
                        gfx.DrawImage(xi, dx, dy, dw, dh)
                    End Using
                End Using
            End If
        End Using

        doc.Save(rutaPdf)
        doc.Close()
    End Sub



    Private Function DrawParagraph(gfx As XGraphics, text As String, font As XFont, brush As XBrush, x As Double, y As Double, w As Double, lineH As Double) As Double
        Dim words = text.Split(" "c)
        Dim line As String = ""
        For Each word In words
            Dim test = If(line.Length = 0, word, line & " " & word)
            Dim size = gfx.MeasureString(test, font)
            If size.Width > w Then
                gfx.DrawString(line, font, brush, New XRect(x, y, w, lineH), XStringFormats.TopLeft)
                y += lineH
                line = word
            Else
                line = test
            End If
        Next
        If line.Length > 0 Then
            gfx.DrawString(line, font, brush, New XRect(x, y, w, lineH), XStringFormats.TopLeft)
            y += lineH
        End If
        Return y
    End Function

    '====================== Utilidades ======================
    Private Function DataUrlToBytes(dataUrl As String) As Byte()
        If String.IsNullOrWhiteSpace(dataUrl) Then Return Nothing
        Dim p As Integer = dataUrl.IndexOf("base64,", StringComparison.OrdinalIgnoreCase)
        If p < 0 Then Return Nothing
        Dim b64 As String = dataUrl.Substring(p + 7)
        Try
            Return Convert.FromBase64String(b64)
        Catch
            Return Nothing
        End Try
    End Function

    Private Function LeerComoJpeg(fu As FileUpload) As Byte()
        If fu Is Nothing OrElse Not fu.HasFile Then Return Nothing
        Dim ext As String = Path.GetExtension(fu.FileName).ToLower()
        Dim permitidas = New String() {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"}
        If Not permitidas.Contains(ext) Then Return Nothing
        Using ms As New MemoryStream()
            fu.PostedFile.InputStream.CopyTo(ms)
            Dim raw = ms.ToArray()
            Return A_Jpeg_Redimensionado(raw, MAX_SIDE, MAX_SIDE, JPEG_QUALITY)
        End Using
    End Function

    Private Sub DibujarImagenCentradaEnRegion(gfx As XGraphics, imgBytes As Byte(), x As Double, y As Double, regionW As Double, regionH As Double)
        If imgBytes Is Nothing OrElse imgBytes.Length = 0 Then Return
        Using ms As New MemoryStream(imgBytes)
            Using xi As XImage = XImage.FromStream(ms)
                Dim scale As Double = Math.Min(regionW / xi.PointWidth, regionH / xi.PointHeight)
                If scale > 1.0 Then scale = 1.0
                Dim dw As Double = xi.PointWidth * scale
                Dim dh As Double = xi.PointHeight * scale
                Dim dx As Double = x + (regionW - dw) / 2.0
                Dim dy As Double = y + (regionH - dh) / 2.0
                gfx.DrawImage(xi, dx, dy, dw, dh)
            End Using
        End Using
    End Sub

    Private Function ObtenerSubcarpetaDestinoFisica() As String
        Dim carpetaBaseFisica As String = ResolverCarpetaFisica(hidCarpeta.Value)
        Return Path.Combine(carpetaBaseFisica, SUBFOLDER_NAME)
    End Function

    Private Function A_Jpeg_Redimensionado(inputBytes As Byte(), maxW As Integer, maxH As Integer, calidad As Long) As Byte()
        Using msIn As New MemoryStream(inputBytes)
            Using src As DrawingImage = DrawingImage.FromStream(msIn)
                Dim ratioW As Double = maxW / CDbl(src.Width)
                Dim ratioH As Double = maxH / CDbl(src.Height)
                Dim ratio As Double = Math.Min(1.0, Math.Min(ratioW, ratioH))
                Dim newW As Integer = Math.Max(1, CInt(Math.Round(src.Width * ratio)))
                Dim newH As Integer = Math.Max(1, CInt(Math.Round(src.Height * ratio)))

                Using bmp As New Bitmap(newW, newH)
                    bmp.SetResolution(96, 96)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.CompositingQuality = CompositingQuality.HighQuality
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.SmoothingMode = SmoothingMode.HighQuality
                        g.DrawImage(src, 0, 0, newW, newH)
                    End Using

                    Dim codecJpg As ImageCodecInfo = ImageCodecInfo.GetImageEncoders().First(Function(c) c.FormatID = ImageFormat.Jpeg.Guid)
                    Dim encParams As New EncoderParameters(1)
                    encParams.Param(0) = New EncoderParameter(Encoder.Quality, calidad)

                    Using msOut As New MemoryStream()
                        bmp.Save(msOut, codecJpg, encParams)
                        Return msOut.ToArray()
                    End Using
                End Using
            End Using
        End Using
    End Function

    Private Function ResolverCarpetaFisica(carpetaRel As String) As String
        Dim p As String = Convert.ToString(carpetaRel).Trim()
        If String.IsNullOrEmpty(p) Then Throw New Exception("carpetarel vacío.")
        If Path.IsPathRooted(p) Then Return p
        If p.StartsWith("~") OrElse p.StartsWith("/") OrElse p.StartsWith("\") Then
            Return Server.MapPath(p)
        End If
        Return Server.MapPath("~/" & p.TrimStart("/"c, "\"c))
    End Function

    Private Sub EmitStartupScript(key As String, js As String)
        Dim sm = ScriptManager.GetCurrent(Me.Page)
        If sm IsNot Nothing Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), key, js, True)
        Else
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), key, "<script>" & js & "</script>")
        End If
    End Sub

    '====================== INV Grúa helpers ======================
    Private Function GetInvGruaDiskPath() As String
        Return System.IO.Path.Combine(ObtenerSubcarpetaDestinoFisica(), "invgrua.pdf")
    End Function

    Private Function GetInvGruaWebPath() As String
        Dim rel As String = Convert.ToString(hidCarpeta.Value).Trim()
        If String.IsNullOrWhiteSpace(rel) Then Return String.Empty
        Dim baseRel As String = If(rel.StartsWith("~") OrElse rel.StartsWith("/"), rel, "~/" & rel)
        Dim webRel As String = (baseRel.TrimEnd("/"c) & "/" & SUBFOLDER_NAME & "/invgrua.pdf")
        Return ResolveUrl(webRel)
    End Function

    Private Sub UpdateInvGruaButtons()
        Dim exists As Boolean = False
        If Not String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            exists = System.IO.File.Exists(GetInvGruaDiskPath())
        End If
        btnVerInvGrua.Enabled = exists
        ToggleCss(btnVerInvGrua, "disabled", Not exists)
    End Sub

    Protected Sub btnInvGrua_Click(sender As Object, e As EventArgs) Handles btnInvGrua.Click
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "openInvGruaModal",
            "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalInvGrua')).show();", True)
    End Sub

    ' (legacy) ya no se usa, pero devolvemos el control real para compatibilidad
    Private Function fuInvGrua() As FileUpload
        Return TryCast(FindControlRecursive(Me, "fuInvGruaPdf"), FileUpload)
    End Function

    Protected Sub btnVerInvGrua_Click(sender As Object, e As EventArgs) Handles btnVerInvGrua.Click
        Dim disk As String = GetInvGruaDiskPath()
        If Not System.IO.File.Exists(disk) Then
            UpdateInvGruaButtons()
            Exit Sub
        End If

        Dim webUrl As String = GetInvGruaWebPath()
        If String.IsNullOrWhiteSpace(webUrl) Then Exit Sub

        Dim js As String =
            "document.getElementById('invGruaFrame').src='" & webUrl.Replace("'", "\'") & "';" &
            "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalVerInvGrua')).show();"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showInvGrua", js, True)
    End Sub

    Protected Sub btnUploadInvGruaGo_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "retryInvGrua",
                "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalInvGrua')).show();", True)
            Exit Sub
        End If

        Dim fu As FileUpload = TryCast(FindControlRecursive(Me, "fuInvGruaPdf"), FileUpload)
        If fu Is Nothing OrElse Not fu.HasFile Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "retryInvGrua2",
                "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalInvGrua')).show();", True)
            Exit Sub
        End If

        Dim ext As String = System.IO.Path.GetExtension(fu.FileName).ToLowerInvariant()
        If ext <> ".pdf" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "retryInvGrua3",
                "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalInvGrua')).show();", True)
            Exit Sub
        End If

        Dim folder As String = ObtenerSubcarpetaDestinoFisica()
        If Not System.IO.Directory.Exists(folder) Then System.IO.Directory.CreateDirectory(folder)

        Dim savePath As String = GetInvGruaDiskPath()
        fu.SaveAs(savePath)

        UpdateInvGruaButtons()

        Dim webUrl As String = GetInvGruaWebPath()
        If Not String.IsNullOrWhiteSpace(webUrl) Then
            Dim js As String =
                "var m=bootstrap.Modal.getInstance(document.getElementById('modalInvGrua')); if(m){m.hide();}" &
                "document.getElementById('invGruaFrame').src='" & webUrl.Replace("'", "\'") & "';" &
                "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalVerInvGrua')).show();"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "openViewInvGrua", js, True)
        End If
    End Sub

    Protected Sub btnVerINV_Click(sender As Object, e As EventArgs) Handles btnVerINV.Click
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
        Dim v As String = GetPdfVersion("inv")
        If v = String.Empty Then
            UpdateBottomWidgets()
            Exit Sub
        End If
        Dim baseUrl As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=inv"
        Dim url As String = ResolveUrl(baseUrl & If(v <> "", "&v=" & v, ""))
        EmitStartupScript("openInvPdf", "openSmartViewer('" & url.Replace("'", "\'") & "');")
    End Sub

    ' ====== BOTONES ELIMINAR EN MODALES ======

    ' Visor genérico (INE/ODA/CT/INV) -> usa hidViewerSrc
    Protected Sub btnDeleteViewer_Click(sender As Object, e As EventArgs)
        If Not IsCurrentUserAdmin Then Exit Sub
        Dim src As String = Convert.ToString(hidViewerSrc.Value)
        If String.IsNullOrWhiteSpace(src) Then Exit Sub

        Dim diskPath As String = MapWebOrHandlerUrlToDiskPath(src)
        If String.IsNullOrWhiteSpace(diskPath) Then Exit Sub
        If Not SafePathWithinExpediente(diskPath) Then Exit Sub

        SafeDeleteFile(diskPath)

        ' Cierra modal y actualiza “ojos”
        EmitStartupScript("closeViewerAfterDelete",
            "var m=bootstrap.Modal.getInstance(document.getElementById('viewerModal')); if(m){m.hide();}" &
            "setTimeout(function(){ __tryRefreshEyes && __tryRefreshEyes(); }, 80);")
        UpdateBottomWidgets()
    End Sub

    ' Ver INV Grúa -> usa hidInvGruaSrc (aunque conocemos el path)
    Protected Sub btnDeleteInvGrua_Click(sender As Object, e As EventArgs)
        If Not IsCurrentUserAdmin Then Exit Sub
        Dim diskPath As String = GetInvGruaDiskPath()
        If Not SafePathWithinExpediente(diskPath) Then Exit Sub

        SafeDeleteFile(diskPath)
        UpdateInvGruaButtons()

        EmitStartupScript("closeInvGruaAfterDelete",
            "var m=bootstrap.Modal.getInstance(document.getElementById('modalVerInvGrua')); if(m){m.hide();}" &
            "setTimeout(function(){ __tryRefreshEyes && __tryRefreshEyes(); }, 80);")
        UpdateBottomWidgets()
    End Sub

    ' Inventario (si muestra un PDF) -> usa hidInvSrc
    Protected Sub btnDeleteInv_Click(sender As Object, e As EventArgs)
        If Not IsCurrentUserAdmin Then Exit Sub
        Dim src As String = Convert.ToString(hidInvSrc.Value)
        If String.IsNullOrWhiteSpace(src) Then Exit Sub

        Dim diskPath As String = MapWebOrHandlerUrlToDiskPath(src)
        If String.IsNullOrWhiteSpace(diskPath) Then Exit Sub
        If Not SafePathWithinExpediente(diskPath) Then Exit Sub

        SafeDeleteFile(diskPath)

        EmitStartupScript("closeInvAfterDelete",
            "var m=bootstrap.Modal.getInstance(document.getElementById('invModal')); if(m){m.hide();}" &
            "setTimeout(function(){ __tryRefreshEyes && __tryRefreshEyes(); }, 80);")
        UpdateBottomWidgets()
    End Sub

    ' ====== Mapeo URL -> ruta física segura ======
    Private Function MapWebOrHandlerUrlToDiskPath(url As String) As String
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Return String.Empty
        Dim baseFolder As String = ObtenerSubcarpetaDestinoFisica()

        ' Si viene del handler ViewPdf.ashx?id=..&kind=ct|ine|oda|inv
        If url.IndexOf("ViewPdf.ashx", StringComparison.OrdinalIgnoreCase) >= 0 Then
            Dim kind As String = GetQueryStringValue(url, "kind").ToLowerInvariant()
            Dim name As String = ""
            Select Case kind
                Case "ct" : name = "CT.pdf"
                Case "ine" : name = "INE.pdf"
                Case "oda" : name = "ODA.pdf"
                Case "inv" : name = "inv.pdf"
                Case "inetransito" : name = "inetransito.pdf"
                Case Else : Return String.Empty
            End Select
            Return Path.Combine(baseFolder, name)
        End If

        ' Si es una URL directa a un archivo dentro de la carpeta del expediente.
        Try
            Dim appRel As String
            If url.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                Dim u As New Uri(url, UriKind.Absolute)
                appRel = "~" & u.AbsolutePath
            Else
                appRel = url
            End If
            Dim phys As String = Server.MapPath(appRel)
            Return phys
        Catch
            Return String.Empty
        End Try
    End Function

    Private Function GetQueryStringValue(fullUrl As String, key As String) As String
        Try
            Dim u As Uri
            If fullUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                u = New Uri(fullUrl, UriKind.Absolute)
            Else
                ' Construir con base del sitio
                Dim baseUri As New Uri(Request.Url.GetLeftPart(UriPartial.Authority))
                u = New Uri(baseUri, ResolveUrl(fullUrl))
            End If
            Dim qs = HttpUtility.ParseQueryString(u.Query)
            Return Convert.ToString(qs(key))
        Catch
            Return ""
        End Try
    End Function

    Private Function SafePathWithinExpediente(candidate As String) As Boolean
        Try
            Dim expedienteRoot As String = ResolverCarpetaFisica(hidCarpeta.Value)
            Dim fullRoot = Path.GetFullPath(expedienteRoot & Path.DirectorySeparatorChar)
            Dim fullCand = Path.GetFullPath(candidate)
            Return fullCand.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function

    Private Sub SafeDeleteFile(path As String)
        Try
            If System.IO.File.Exists(path) Then
                System.IO.File.Delete(path)
            End If
        Catch
            ' swallow
        End Try
    End Sub

    ' ====== Admin flag ======
    Public ReadOnly Property IsCurrentUserAdmin As Boolean
        Get
            Dim m = TryCast(Me.Master, Site1)
            Return (m IsNot Nothing AndAlso m.IsAdmin)
        End Get
    End Property

    Protected Overrides Sub OnPreRender(e As EventArgs)
        MyBase.OnPreRender(e)
        ' Expón un flag JS global + un refresher suave para “ojos”
        Dim js As String =
            "window.__isAdmin = " & If(IsCurrentUserAdmin, "true", "false") & ";" &
            "window.__tryRefreshEyes = function(){ try{" &
            "  var strip=document.querySelector('.card-pane.doc-strip.compacto');" &
            "  if(!strip) return; var evt=new Event('click', {bubbles:true}); document.body.dispatchEvent(evt);" &
            "}catch(_){} };"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "adminFlag", js, True)
    End Sub

    ' ====== DIAGNÓSTICO (nuevas carpetas) ======
    Private Const SUBFOLDER_DIAG_MEC As String = "2. FOTOS DIAGNOSTICO MECANICA"
    Private Const SUBFOLDER_DIAG_HOJA As String = "3. FOTOS DIAGNOSTICO HOJALATERIA"
    ' === COMPLEMENTOS ===
    Private Const INETRANSITO_FILENAME As String = "inetransito.pdf"



    Private Function ObtenerSubcarpetaDiagnosticoFisica(area As String) As String
        Dim carpetaBaseFisica As String = ResolverCarpetaFisica(hidCarpeta.Value)
        Dim subf As String = If(area IsNot Nothing AndAlso area.Trim().ToLower() = "mecanica",
                            SUBFOLDER_DIAG_MEC,
                            SUBFOLDER_DIAG_HOJA)
        Return Path.Combine(carpetaBaseFisica, subf)
    End Function

    ' Prefijo = primeros 5 dígitos de la descripción. Si no hay dígitos,
    ' usa los primeros 5 alfanuméricos; si aun así faltan, usa "FOTO".
    Private Function PrefijoDesdeDescripcion(descripcion As String) As String
        If descripcion Is Nothing Then descripcion = ""
        Dim digits = New String(descripcion.Where(Function(ch) Char.IsDigit(ch)).Take(5).ToArray())
        If digits.Length < 1 Then
            Dim alnum = New String(descripcion.Where(Function(ch) Char.IsLetterOrDigit(ch)).Take(5).ToArray())
            If alnum.Length > 0 Then Return alnum.ToUpper()
            Return "FOTO"
        End If
        Return digits
    End Function

    ' Busca el siguiente índice libre para prefijo + sufijo -NN
    Private Function ObtenerSiguienteIndiceDiag(folder As String, prefijo As String) As Integer
        If Not Directory.Exists(folder) Then Return 1
        Dim maxN As Integer = 0
        Dim regex As New Regex("^" & Regex.Escape(prefijo) & "-(\d{2})\.jpg$", RegexOptions.IgnoreCase)
        For Each f In Directory.GetFiles(folder, prefijo & "-*.jpg", SearchOption.TopDirectoryOnly)
            Dim name = Path.GetFileName(f)
            Dim m = regex.Match(name)
            If m.Success Then
                Dim n As Integer
                If Integer.TryParse(m.Groups(1).Value, n) AndAlso n > maxN Then maxN = n
            End If
        Next
        Return maxN + 1
    End Function


    ' ====== Stub para el botón admin del modal Diagnóstico ======
    Protected Sub btnDeleteDiag_Click(sender As Object, e As EventArgs) Handles btnDeleteDiag.Click
        ' TODO: Aquí la acción administrativa que necesites
        ' Por ahora solo mostramos un aviso y cerramos el modal si quieres
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "diagAdminOk",
        "alert('Acción administrativa ejecutada (stub).');", True)
    End Sub
    ' === DIAGNÓSTICO: helpers de fechas ===
    Private Sub LoadDiagLabels(admisionId As Integer)
        Dim ini As DateTime? = Nothing
        Dim fin As DateTime? = Nothing

        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing Then GoTo Paint

        Using cn As New SqlConnection(cs.ConnectionString)
            cn.Open()
            Using cmd As New SqlCommand("SELECT IniDiag, FinDiag FROM dbo.admisiones WHERE Id=@Id", cn)
                cmd.Parameters.AddWithValue("@Id", admisionId)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        ini = SafeReadNullableDate(rd, "IniDiag")
                        fin = SafeReadNullableDate(rd, "FinDiag")
                    End If
                End Using
            End Using
        End Using

Paint:
        Dim lblIni As Label = TryCast(FindControlRecursive(Me, "lblDiagInicio"), Label)
        Dim lblFin As Label = TryCast(FindControlRecursive(Me, "lblDiagFin"), Label)
        If lblIni IsNot Nothing Then lblIni.Text = If(ini.HasValue, ini.Value.ToString("dd/MM/yyyy HH:mm"), "—")
        If lblFin IsNot Nothing Then lblFin.Text = If(fin.HasValue, fin.Value.ToString("dd/MM/yyyy HH:mm"), "—")
    End Sub

    Private Sub EnsureIniDiagRecordedIfNeeded(admisionId As Integer)
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        If cs Is Nothing Then Exit Sub

        Using cn As New SqlConnection(cs.ConnectionString)
            cn.Open()

            ' ¿Ya tiene IniDiag?
            Dim currentIni As Object
            Using cSel As New SqlCommand("SELECT IniDiag FROM dbo.admisiones WHERE Id=@Id", cn)
                cSel.Parameters.AddWithValue("@Id", admisionId)
                currentIni = cSel.ExecuteScalar()
            End Using

            If currentIni Is DBNull.Value OrElse currentIni Is Nothing Then
                ' Setear solo si estaba NULL
                Using cUpd As New SqlCommand("UPDATE dbo.admisiones SET IniDiag = GETDATE() WHERE Id=@Id", cn)
                    cUpd.Parameters.AddWithValue("@Id", admisionId)
                    cUpd.ExecuteNonQuery()
                End Using
            End If
        End Using
    End Sub

    Private Function SafeReadNullableDate(rd As IDataRecord, col As String) As DateTime?
        Try
            Dim i = rd.GetOrdinal(col)
            If rd.IsDBNull(i) Then Return Nothing
            Return CType(rd.GetValue(i), DateTime)
        Catch
            Return Nothing
        End Try
    End Function

    Private Sub PintarTilesRecepcion()
        ' Referencia al tile de Complementos (una sola vez)
        Dim tileCompl As HtmlGenericControl = TryCast(FindControlRecursive(Me, "tileCOMPL"), HtmlGenericControl)

        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            ' Limpia "ok" en todas las tarjetas si no hay carpeta
            SetTileOk(tileODA, False)
            SetTileOk(TileFotos, False)
            SetTileOk(tileINE, False)
            SetTileOk(tileCT, False)
            SetTileOk(tileINV, False)
            SetTileOk(tileCompl, False) ' NUEVO: Complementos
            Exit Sub
        End If

        Dim baseFolder As String = ObtenerSubcarpetaDestinoFisica()
        Dim presupFolder As String = ObtenerSubcarpetaPresupuestoFisica()

        ' Existencias
        Dim hasODA As Boolean = File.Exists(Path.Combine(baseFolder, "ODA.pdf"))
        Dim hasINE As Boolean = File.Exists(Path.Combine(baseFolder, "INE.pdf"))
        Dim hasCT As Boolean = File.Exists(Path.Combine(baseFolder, "CT.pdf"))
        Dim hasINV As Boolean = File.Exists(Path.Combine(baseFolder, "inv.pdf"))
        Dim hasCompl As Boolean = File.Exists(Path.Combine(baseFolder, "inetransito.pdf")) ' NUEVO

        Dim hasFotos As Boolean = False
        If Directory.Exists(presupFolder) Then
            hasFotos = Directory.GetFiles(presupFolder, PREFIX_PRESUP & "*.jpg", SearchOption.TopDirectoryOnly).Length > 0
        End If

        ' Pinta/depinta "ok"
        SetTileOk(tileODA, hasODA)
        SetTileOk(TileFotos, hasFotos)
        SetTileOk(tileINE, hasINE)
        SetTileOk(tileCT, hasCT)

        ' INV: ok si existe inv.pdf o existe invgrua.pdf (reflejado por botón habilitado)
        Dim invOk As Boolean = hasINV OrElse btnVerInvGrua.Enabled
        SetTileOk(tileINV, invOk)

        ' Complementos
        SetTileOk(tileCompl, hasCompl)
    End Sub



    ' ===== Reemplaza COMPLETO =====
    Private Sub SetTileOk(tile As HtmlGenericControl, flag As Boolean)
        If tile Is Nothing Then Exit Sub
        Dim cls As String = If(tile.Attributes("class"), String.Empty)
        Dim parts = cls.Split({" "c}, StringSplitOptions.RemoveEmptyEntries) _
                   .Where(Function(c) c <> "is-ready").ToList()
        If flag Then parts.Add("is-ready")
        tile.Attributes("class") = String.Join(" ", parts)
    End Sub



    Private Function GetOdaPhysicalPath() As String
        ' Si pasas ?id=1027 en la URL (Hoja.aspx?id=1027)
        Dim idStr As String = Request.QueryString("id")
        Dim id As Integer
        If Integer.TryParse(idStr, id) Then
            Dim rootExp As String = Server.MapPath("~/Expedientes")
            Dim expPath As String = Path.Combine(rootExp, id.ToString())
            Return Path.Combine(expPath, SUBFOLDER_INGRESO, ODA_FILENAME)
        End If

        ' Si manejas CarpetaRel, cambia esto por tu variable/hiddenfield
        'Dim carpetaRel As String = hfCarpetaRel.Value
        'Return Path.Combine(Server.MapPath("~"), carpetaRel, SUBFOLDER_INGRESO, ODA_FILENAME)

        ' Fallback (opcional)
        Return Path.Combine(Server.MapPath("~"), SUBFOLDER_INGRESO, ODA_FILENAME)
    End Function

    Private Function GetInetransitoDiskPath() As String
        Return System.IO.Path.Combine(ObtenerSubcarpetaDestinoFisica(), INETRANSITO_FILENAME)
    End Function

    Private Function GetInetransitoWebPath() As String
        ' Si decides abrir directo sin handler:
        Dim rel As String = Convert.ToString(hidCarpeta.Value).Trim()
        If String.IsNullOrWhiteSpace(rel) Then Return String.Empty
        Dim baseRel As String = If(rel.StartsWith("~") OrElse rel.StartsWith("/"), rel, "~/" & rel)
        Dim webRel As String = (baseRel.TrimEnd("/"c) & "/" & SUBFOLDER_NAME & "/" & INETRANSITO_FILENAME)
        Return ResolveUrl(webRel)
    End Function

    ' === SUBIR COMPLEMENTOS (inetransito.pdf) ===
    Protected Sub btnUploadComplementosGo_Click(sender As Object, e As EventArgs)
        ' Configura mensaje por defecto
        lblComplementosMsg.Visible = True
        lblComplementosMsg.CssClass = "text-danger small"
        lblComplementosMsg.Text = ""

        ' 0) Validación de carpeta de expediente
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
            lblComplementosMsg.Text = "No hay carpeta de expediente configurada."
            Exit Sub
        End If

        ' 1) Obtener el FileUpload
        Dim fu As FileUpload = TryCast(FindControlRecursive(Me, "fuComplementos"), FileUpload)
        If fu Is Nothing OrElse Not fu.HasFile Then
            lblComplementosMsg.Text = "Selecciona un archivo PDF."
            Exit Sub
        End If

        ' 2) Validaciones del archivo
        Dim ext As String = System.IO.Path.GetExtension(fu.FileName).ToLowerInvariant()
        If ext <> ".pdf" Then
            lblComplementosMsg.Text = "El archivo debe ser PDF (.pdf)."
            Exit Sub
        End If

        ' (Opcional) validar ContentType y tamaño
        Const MAX_BYTES As Integer = 30 * 1024 * 1024 ' 30 MB
        Dim ct As String = fu.PostedFile.ContentType
        If Not String.IsNullOrEmpty(ct) AndAlso Not ct.ToLowerInvariant().Contains("pdf") Then
            lblComplementosMsg.Text = "El archivo no parece ser un PDF válido."
            Exit Sub
        End If
        If fu.PostedFile.ContentLength <= 0 OrElse fu.PostedFile.ContentLength > MAX_BYTES Then
            lblComplementosMsg.Text = "El tamaño del PDF es inválido o excede el límite."
            Exit Sub
        End If

        Try
            ' 3) Asegurar carpeta destino
            Dim folder As String = ObtenerSubcarpetaDestinoFisica()
            If Not System.IO.Directory.Exists(folder) Then
                System.IO.Directory.CreateDirectory(folder)
            End If

            ' 4) Ruta final (inetransito.pdf)
            Dim savePath As String = GetInetransitoDiskPath() ' Debe apuntar a ...\inetransito.pdf
            Dim saveDir As String = System.IO.Path.GetDirectoryName(savePath)
            If Not String.IsNullOrEmpty(saveDir) AndAlso Not System.IO.Directory.Exists(saveDir) Then
                System.IO.Directory.CreateDirectory(saveDir)
            End If

            ' 5) Reemplazo seguro si ya existe
            If System.IO.File.Exists(savePath) Then
                System.IO.File.Delete(savePath)
            End If

            ' 6) Guardar archivo
            fu.SaveAs(savePath)

            ' 7) UI: éxito y refresco de widgets
            lblComplementosMsg.CssClass = "text-success small"
            lblComplementosMsg.Text = "Archivo guardado como inetransito.pdf."
            UpdateBottomWidgets()

            ' 8) Abrir visor (cache-busting con versión si aplica)
            Dim v As String = GetPdfVersion("inetransito")
            Dim baseUrl As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=inetransito"
            Dim url As String = ResolveUrl(baseUrl & If(v <> "", "&v=" & v, ""))

            ' Usa tu helper si existe; si no, usa ScriptManager directamente
            Dim js As String = "try{ openSmartViewer('" & url.Replace("'", "\'") & "'); }catch(e){}"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "openInetransito", js, True)

        Catch ex As UnauthorizedAccessException
            lblComplementosMsg.CssClass = "text-danger small"
            lblComplementosMsg.Text = "Permiso denegado al guardar el PDF. Verifica permisos de la carpeta de destino."
        Catch ex As IOException
            lblComplementosMsg.CssClass = "text-danger small"
            lblComplementosMsg.Text = "No se pudo escribir el archivo (¿está en uso?). Intenta de nuevo."
        Catch ex As Exception
            lblComplementosMsg.CssClass = "text-danger small"
            lblComplementosMsg.Text = "Error al guardar el PDF."
            ' Log interno recomendado: ex.ToString()
        End Try
    End Sub


    ' === VER COMPLEMENTOS (inetransito.pdf) ===
    Protected Sub btnVerInetransito_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
        Dim exists As Boolean = System.IO.File.Exists(GetInetransitoDiskPath())
        If Not exists Then
            UpdateBottomWidgets()
            Exit Sub
        End If
        Dim v As String = GetPdfVersion("inetransito")
        Dim baseUrl As String = "~/ViewPdf.ashx?id=" & lblId.Text & "&kind=inetransito"
        Dim url As String = ResolveUrl(baseUrl & If(v <> "", "&v=" & v, ""))
        EmitStartupScript("openInetransitoView", "openSmartViewer('" & url.Replace("'", "\'") & "');")
    End Sub
    Private Sub PintarTileMecanica(admId As Integer)
        Dim a1 As Boolean = False, a2 As Boolean = False, a3 As Boolean = False

        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        Using cn As New SqlConnection(cs)
            Using cmd As New SqlCommand("SELECT autmec1, autmec2, autmec3 FROM admisiones WHERE id = @id", cn)
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = admId
                cn.Open()
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        a1 = Not rd.IsDBNull(0) AndAlso Convert.ToBoolean(rd(0))
                        a2 = Not rd.IsDBNull(1) AndAlso Convert.ToBoolean(rd(1))
                        a3 = Not rd.IsDBNull(2) AndAlso Convert.ToBoolean(rd(2))
                    End If
                End Using
            End Using
        End Using

        Dim allOk As Boolean = a1 AndAlso a2 AndAlso a3

        ' Asegura estado visual del tile
        Dim cls As String = tileMec.Attributes("class")
        If allOk Then
            If Not cls.Contains(" ok") Then tileMec.Attributes("class") = cls & " ok"
            flagMec.Attributes("class") = "diag-flag on"
            icoMec.Attributes("class") = "bi bi-check-circle-fill"
            chkMecSi.Checked = True
        Else
            tileMec.Attributes("class") = cls.Replace(" ok", "")
            flagMec.Attributes("class") = "diag-flag off"
            icoMec.Attributes("class") = "bi bi-x-circle-fill"
            chkMecSi.Checked = False
        End If
    End Sub
    Private Sub CargarFinesDiagnostico(admId As Integer)
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        Dim finMecObj As Object = Nothing
        Dim finColObj As Object = Nothing

        Using cn As New SqlConnection(cs)
            Using cmd As New SqlCommand("SELECT finmec, fincol FROM admisiones WHERE id = @id", cn)
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = admId
                cn.Open()
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        finMecObj = If(rd.IsDBNull(0), Nothing, rd.GetValue(0))
                        finColObj = If(rd.IsDBNull(1), Nothing, rd.GetValue(1))
                    End If
                End Using
            End Using
        End Using

        lblDiagFinMecanica.Text = FormatearFechaCortaHora(finMecObj)
        lblDiagFinColision.Text = FormatearFechaCortaHora(finColObj)
    End Sub

    Private Function FormatearFechaCortaHora(val As Object) As String
        If val Is Nothing OrElse val Is DBNull.Value Then Return "—"
        Dim dt As DateTime
        If DateTime.TryParse(val.ToString(), dt) Then
            ' Formato típico MX: dd/MM/yyyy HH:mm
            Return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("es-MX"))
        End If
        Return "—"
    End Function

    ' Helper para leer Estatus (TRANSITO / PISO)
    Private Function GetAdmEstatusById(admId As Integer) As String
        If admId <= 0 Then Return String.Empty
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        Using cn As New SqlConnection(cs)
            Using cmd As New SqlCommand("SELECT TOP 1 Estatus FROM dbo.Admisiones WHERE Id=@Id;", cn)
                cmd.Parameters.AddWithValue("@Id", admId)
                cn.Open()
                Dim o = cmd.ExecuteScalar()
                If o Is Nothing OrElse o Is DBNull.Value Then Return String.Empty
                Return Convert.ToString(o).Trim()
            End Using
        End Using
    End Function


End Class


