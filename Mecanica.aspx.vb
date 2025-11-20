Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports System.Web.Services
Imports System.Web.Script.Services

Public Class Mecanica
    Inherits System.Web.UI.Page

    ' ====== Config / Const ======
    Private ReadOnly Property CS As String
        Get
            Return ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        End Get
    End Property

    Private Const AREA As String = "MECANICA"
    Private Const SUBFOLDER_MECA As String = "2. FOTOS DIAGNOSTICO MECANICA"

    ' ====== Page ======
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim exp = Request.QueryString("expediente")
            If Not String.IsNullOrWhiteSpace(exp) Then
                txtExpediente.Text = exp
                hfExpediente.Value = exp
            End If
            txtSiniestro.Text = Request.QueryString("siniestro")
            txtVehiculo.Text = Request.QueryString("vehiculo")

            Dim cr = Request.QueryString("carpeta")
            If Not String.IsNullOrWhiteSpace(cr) Then
                hfCarpetaRel.Value = cr
            End If

            BindAll()
            LoadAdmins()
            PaintAutFlags()
        End If

        ' Eventos botones de autorización (vistos buenos)
        AddHandler btnAutorizarMec1.Click, AddressOf btnAutorizarMec1_Click
        AddHandler btnAutorizarMec2.Click, AddressOf btnAutorizarMec2_Click
        AddHandler btnAutorizarMec3.Click, AddressOf btnAutorizarMec3_Click
    End Sub

    Protected Overrides Sub OnPreRender(e As EventArgs)
        MyBase.OnPreRender(e)
        ' Mostrar u ocultar la columna "Eliminar" a admins
        Dim isAdm = IsCurrentUserAdmin()
        EnsureDeleteColumnVisibility(gvSust, isAdm)
        EnsureDeleteColumnVisibility(gvRep, isAdm)
    End Sub

    Private Sub EnsureDeleteColumnVisibility(gv As GridView, isAdm As Boolean)
        For Each col As DataControlField In gv.Columns
            If String.Equals(col.HeaderText, "Eliminar", StringComparison.OrdinalIgnoreCase) Then
                col.Visible = isAdm
                Exit For
            End If
        Next
    End Sub

    ' ====== Bind grids ======
    Private Sub BindAll()
        BindGrid(gvSust, "SUSTITUCION")
        BindGrid(gvRep, "REPARACION")
    End Sub

    Private Sub BindGrid(gv As GridView, categoria As String)
        Dim dt As New DataTable()
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                SELECT Id, Cantidad, Descripcion, NumParte, Observ1
                FROM dbo.Refacciones
                WHERE Area=@Area AND Categoria=@Categoria AND Expediente=@Expediente
                ORDER BY Id DESC;", cn)
                cmd.Parameters.AddWithValue("@Area", AREA)
                cmd.Parameters.AddWithValue("@Categoria", categoria)
                cmd.Parameters.AddWithValue("@Expediente", GetExpediente())
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        gv.DataSource = dt
        gv.DataBind()
    End Sub

    Private Function GetExpediente() As String
        Dim v As String = If(hfExpediente IsNot Nothing, hfExpediente.Value, Nothing)
        If String.IsNullOrWhiteSpace(v) Then v = txtExpediente.Text
        Return If(v, String.Empty).Trim()
    End Function

    ' ====== Insert ======
    Private Function ParseCantidad(txt As TextBox) As Integer
        Dim n As Integer = 0
        If txt IsNot Nothing AndAlso Integer.TryParse(txt.Text.Trim(), n) Then
            If n > 0 AndAlso n <= 9999 Then Return n
        End If
        Return 0
    End Function

    Private Sub InsertRefaccion(categoria As String,
                                cantTxt As TextBox,
                                descTxt As TextBox,
                                Optional numParteTxt As TextBox = Nothing,
                                Optional obsTxt As TextBox = Nothing)
        Dim expediente = GetExpediente()
        If String.IsNullOrWhiteSpace(expediente) Then
            ShowStatus("Expediente vacío. No se puede guardar.", isOk:=False) : Exit Sub
        End If

        Dim cant = ParseCantidad(cantTxt)
        Dim desc = If(descTxt.Text, "").Trim()
        If cant <= 0 Then
            ShowStatus("Cantidad inválida (1-9999).", isOk:=False) : Exit Sub
        End If
        If String.IsNullOrWhiteSpace(desc) Then
            ShowStatus("Descripción requerida.", isOk:=False) : Exit Sub
        End If

        Dim numParte As String = If(numParteTxt IsNot Nothing, (If(numParteTxt.Text, "")).Trim(), "")
        Dim observ As String = If(obsTxt IsNot Nothing, (If(obsTxt.Text, "")).Trim(), "")

        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                INSERT INTO dbo.Refacciones
                (AdmisionId, Expediente, Area, Categoria, Cantidad, Descripcion, NumParte, Observ1, Autorizado, Checar, Aldo, AldoDateTime)
                VALUES
                (@AdmisionId, @Expediente, @Area, @Categoria, @Cantidad, @Descripcion, @NumParte, @Observ1, 0, 0, 0, NULL);", cn)
                cmd.Parameters.AddWithValue("@AdmisionId", DBNull.Value)
                cmd.Parameters.AddWithValue("@Expediente", expediente)
                cmd.Parameters.AddWithValue("@Area", AREA)
                cmd.Parameters.AddWithValue("@Categoria", categoria)
                cmd.Parameters.AddWithValue("@Cantidad", cant)
                cmd.Parameters.AddWithValue("@Descripcion", desc)
                If String.Equals(categoria, "SUSTITUCION", StringComparison.OrdinalIgnoreCase) Then
                    cmd.Parameters.AddWithValue("@NumParte", If(String.IsNullOrWhiteSpace(numParte), CType(DBNull.Value, Object), numParte))
                    cmd.Parameters.AddWithValue("@Observ1", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@NumParte", DBNull.Value)
                    cmd.Parameters.AddWithValue("@Observ1", If(String.IsNullOrWhiteSpace(observ), CType(DBNull.Value, Object), observ))
                End If
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        cantTxt.Text = ""
        descTxt.Text = ""
        If numParteTxt IsNot Nothing Then numParteTxt.Text = ""
        If obsTxt IsNot Nothing Then obsTxt.Text = ""
        ShowStatus("Guardado correctamente.")
    End Sub

    Protected Sub btnAddSust_Click(sender As Object, e As EventArgs)
        InsertRefaccion("SUSTITUCION", txtCantSust, txtDescSust, numParteTxt:=txtNumParteSust)
        BindGrid(gvSust, "SUSTITUCION")
    End Sub

    Protected Sub btnAddRep_Click(sender As Object, e As EventArgs)
        InsertRefaccion("REPARACION", txtCantRep, txtDescRep, obsTxt:=txtObsRep)
        BindGrid(gvRep, "REPARACION")
    End Sub

    ' ====== RowCommand: Sustitución ======
    Protected Sub gvSust_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvSust.RowCommand
        Select Case e.CommandName
            Case "VER_FOTOS"
                Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
                Dim id As Integer = Convert.ToInt32(gvSust.DataKeys(rowIndex).Values("Id"))
                Dim descripcion As String = TryCast(gvSust.DataKeys(rowIndex).Values("Descripcion"), String)
                OpenGaleriaForRow("Sustitución", id, descripcion)

            Case "DELETE_ROW"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                DeleteRefaccion(id)
                BindGrid(gvSust, "SUSTITUCION")
        End Select
    End Sub

    ' ====== RowCommand: Reparación ======
    Protected Sub gvRep_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvRep.RowCommand
        Select Case e.CommandName
            Case "VER_FOTOS"
                Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
                Dim id As Integer = Convert.ToInt32(gvRep.DataKeys(rowIndex).Values("Id"))
                Dim descripcion As String = TryCast(gvRep.DataKeys(rowIndex).Values("Descripcion"), String)
                OpenGaleriaForRow("Reparación", id, descripcion)

            Case "DELETE_ROW"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                DeleteRefaccion(id)
                BindGrid(gvRep, "REPARACION")
        End Select
    End Sub

    ' ====== RowDataBound: pintar sutil si hay ≥3 fotos ======
    Protected Sub gvSust_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvSust.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        Dim drv = TryCast(e.Row.DataItem, DataRowView)
        If drv Is Nothing Then Return

        Dim descripcion As String = Convert.ToString(drv("Descripcion"))
        Dim id As Integer = Convert.ToInt32(drv("Id"))
        Dim exp As String = GetExpediente()
        Dim virtualFolder As String = CombineVirtual(GetCarpetaRelVirtual(exp), SUBFOLDER_MECA)
        Dim physicalFolder As String = Server.MapPath(NormalizeVirtual(virtualFolder))
        Dim combinedPrefix As String = BuildRowPrefix(id, descripcion)
        Dim legacyPrefix As String = BuildSafePrefix(descripcion)

        Dim count As Integer = GetFilesByAnyPrefix(physicalFolder, New List(Of String) From {combinedPrefix, legacyPrefix}).Count
        If count >= 3 Then
            e.Row.CssClass = (e.Row.CssClass & " row-photos-ok").Trim()
        End If
    End Sub

    Protected Sub gvRep_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvRep.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        Dim drv = TryCast(e.Row.DataItem, DataRowView)
        If drv Is Nothing Then Return

        Dim descripcion As String = Convert.ToString(drv("Descripcion"))
        Dim id As Integer = Convert.ToInt32(drv("Id"))
        Dim exp As String = GetExpediente()
        Dim virtualFolder As String = CombineVirtual(GetCarpetaRelVirtual(exp), SUBFOLDER_MECA)
        Dim physicalFolder As String = Server.MapPath(NormalizeVirtual(virtualFolder))
        Dim combinedPrefix As String = BuildRowPrefix(id, descripcion)
        Dim legacyPrefix As String = BuildSafePrefix(descripcion)

        Dim count As Integer = GetFilesByAnyPrefix(physicalFolder, New List(Of String) From {combinedPrefix, legacyPrefix}).Count
        If count >= 3 Then
            e.Row.CssClass = (e.Row.CssClass & " row-photos-ok").Trim()
        End If
    End Sub

    ' ====== Eliminar ======
    Private Sub DeleteRefaccion(id As Integer)
        If Not IsCurrentUserAdmin() Then
            ShowStatus("No autorizado para eliminar.", False)
            Exit Sub
        End If

        Dim expediente = GetExpediente()
        If String.IsNullOrWhiteSpace(expediente) Then
            ShowStatus("Expediente vacío.", False)
            Exit Sub
        End If

        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                DELETE FROM dbo.Refacciones
                WHERE Id = @Id AND Expediente = @Expediente;", cn)
                cmd.Parameters.AddWithValue("@Id", id)
                cmd.Parameters.AddWithValue("@Expediente", expediente)
                cn.Open()
                Dim n = cmd.ExecuteNonQuery()
                If n > 0 Then
                    ShowStatus("Registro eliminado.")
                Else
                    ShowStatus("No se encontró el registro a eliminar.", False)
                End If
            End Using
        End Using
    End Sub

    ' ====== Usuarios admin para Vistos Buenos ======
    Private Sub LoadAdmins()
        Dim dt As New DataTable()
        Using cn As New SqlConnection(CS)
            Dim sql As String =
                "SELECT UsuarioId, COALESCE(Nombre, Correo) AS Nombre " &
                "FROM dbo.Usuarios " &
                "WHERE EsAdmin = 1 " &
                "ORDER BY Nombre;"
            Using da As New SqlDataAdapter(sql, cn)
                da.Fill(dt)
            End Using
        End Using

        BindAdminDDL(ddlAutMec1, dt)
        BindAdminDDL(ddlAutMec2, dt)
        BindAdminDDL(ddlAutMec3, dt)
    End Sub

    Private Sub BindAdminDDL(ddl As DropDownList, dt As DataTable)
        ddl.Items.Clear()
        ddl.AppendDataBoundItems = True
        ddl.Items.Add(New ListItem("-- Selecciona usuario --", ""))
        ddl.DataSource = dt
        ddl.DataTextField = "Nombre"
        ddl.DataValueField = "UsuarioId"
        ddl.DataBind()
    End Sub

    ' ====== Detección de admin actual ======
    Private Function CurrentUserId() As Integer?
        Dim o = Session("UsuarioId")
        Dim id As Integer
        If o IsNot Nothing AndAlso Integer.TryParse(o.ToString(), id) Then
            Return id
        End If
        Dim qs = Request.QueryString("uid")
        If Not String.IsNullOrWhiteSpace(qs) AndAlso Integer.TryParse(qs, id) Then
            Return id
        End If
        Return Nothing
    End Function

    Private Function IsCurrentUserAdmin() As Boolean
        If ViewState("IsAdminFlag") IsNot Nothing Then
            Return CBool(ViewState("IsAdminFlag"))
        End If
        Dim uid = CurrentUserId()
        If Not uid.HasValue Then
            ViewState("IsAdminFlag") = False
            Return False
        End If
        Dim isAdm As Boolean = False
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("SELECT TOP 1 ISNULL(EsAdmin,0) FROM dbo.Usuarios WHERE UsuarioId=@Id;", cn)
                cmd.Parameters.AddWithValue("@Id", uid.Value)
                cn.Open()
                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    isAdm = Convert.ToBoolean(obj)
                End If
            End Using
        End Using
        ViewState("IsAdminFlag") = isAdm
        Return isAdm
    End Function

    ' ====== Clicks de autorización ======
    Private Sub btnAutorizarMec1_Click(sender As Object, e As EventArgs)
        HandleAuthorization(ddlAutMec1, txtPassMec1, "autmec1", litAutMec1)
    End Sub
    Private Sub btnAutorizarMec2_Click(sender As Object, e As EventArgs)
        HandleAuthorization(ddlAutMec2, txtPassMec2, "autmec2", litAutMec2)
    End Sub
    Private Sub btnAutorizarMec3_Click(sender As Object, e As EventArgs)
        HandleAuthorization(ddlAutMec3, txtPassMec3, "autmec3", litAutMec3)
    End Sub

    Private Sub HandleAuthorization(ddl As DropDownList, txtPass As TextBox, fieldName As String, lit As Literal)
        Dim expediente = GetExpediente()
        If String.IsNullOrWhiteSpace(expediente) Then
            ShowStatus("Expediente vacío.", False) : Exit Sub
        End If
        If String.IsNullOrWhiteSpace(ddl.SelectedValue) Then
            ShowStatus("Selecciona un usuario.", False) : Exit Sub
        End If

        Dim userId As Integer
        If Not Integer.TryParse(ddl.SelectedValue, userId) Then
            ShowStatus("Usuario inválido.", False) : Exit Sub
        End If

        Dim pass As String = If(txtPass.Text, "").Trim()
        If pass = "" Then
            ShowStatus("Ingresa tu contraseña.", False) : Exit Sub
        End If

        If Not ValidateUserById(userId, pass) Then
            ShowStatus("Credenciales inválidas.", False) : Exit Sub
        End If

        ' === Nombre a guardar ===
        Dim authName As String = ddl.SelectedItem.Text

        ' Actualizar bit + nombre en Admisiones
        If Not UpdateAdmisionAuth(expediente, fieldName, True, authName) Then
            ShowStatus("No se pudo actualizar la autorización.", False) : Exit Sub
        End If

        ShowStatus("Autorización registrada.", True)
        PaintAutFlags()

        ' Deshabilitar entrada
        ddl.Enabled = False : txtPass.Enabled = False

        ' Mostrar explícitamente en el literal (nombre + badge)
        Try
            Dim safeName = HttpUtility.HtmlEncode(authName)
            lit.Text = "<span class='badge bg-success me-2'>Autorizado</span>" &
                       $"<span class='text-success'><i class='bi bi-person-check me-1'></i>{safeName}</span>"
        Catch
        End Try
    End Sub

    ' ====== Validación contra VARBINARY (SHA-256) ======
    Private Function ValidateUserById(userId As Integer, password As String) As Boolean
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                SELECT TOP 1 PasswordHash, PasswordSalt, ISNULL(Validador,1) AS Validador
                FROM dbo.Usuarios
                WHERE UsuarioId = @Id;", cn)
                cmd.Parameters.AddWithValue("@Id", userId)
                cn.Open()
                Using rd = cmd.ExecuteReader()
                    If Not rd.Read() Then Return False

                    Dim enabled As Boolean = Convert.ToBoolean(rd("Validador"))
                    If Not enabled Then Return False

                    Dim dbHash As Byte() = TryCast(rd("PasswordHash"), Byte())
                    Dim salt As Byte() = TryCast(rd("PasswordSalt"), Byte())
                    If dbHash Is Nothing OrElse salt Is Nothing Then Return False

                    Dim passBytes = Encoding.UTF8.GetBytes(password)
                    Dim toHash As Byte() = CombineBytes(salt, passBytes)
                    Dim calc As Byte() = SHA256.Create().ComputeHash(toHash)

                    Return BytesEqual(calc, dbHash)
                End Using
            End Using
        End Using
    End Function

    Private Function CombineBytes(a As Byte(), b As Byte()) As Byte()
        If a Is Nothing Then a = New Byte() {}
        If b Is Nothing Then b = New Byte() {}
        Dim res(a.Length + b.Length - 1) As Byte
        If a.Length > 0 Then Array.Copy(a, 0, res, 0, a.Length)
        If b.Length > 0 Then Array.Copy(b, 0, res, a.Length, b.Length)
        Return res
    End Function


    Private Function BytesEqual(a As Byte(), b As Byte()) As Boolean
        If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then Return False
        Dim diff As Integer = 0
        For i As Integer = 0 To a.Length - 1
            diff = diff Or (a(i) Xor b(i))
        Next
        Return diff = 0
    End Function

    ' ====== Actualizar bit + nombre en Admisiones ======
    ' ====== Actualizar bit + nombre en Admisiones (con timbrado de finmec) ======
    Private Function UpdateAdmisionAuth(expediente As String, fieldName As String, value As Boolean, authName As String) As Boolean
        Dim allowed = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {"autmec1", "autmec2", "autmec3"}
        If Not allowed.Contains(fieldName) Then Return False

        Dim nameField As String = fieldName & "nombre" ' autmec1 -> autmec1nombre
        expediente = (If(expediente, String.Empty)).Trim()

        Using cn As New SqlConnection(CS)
            cn.Open()
            Using tr = cn.BeginTransaction()
                Dim ok As Boolean

                ' 1) Actualiza el bit y el nombre
                Using cmd As New SqlCommand($"UPDATE dbo.Admisiones SET {fieldName}=@v, {nameField}=@n WHERE Expediente=@exp;", cn, tr)
                    cmd.Parameters.Add("@v", SqlDbType.Bit).Value = If(value, 1, 0)
                    cmd.Parameters.Add("@n", SqlDbType.NVarChar, 200).Value =
                    If(String.IsNullOrWhiteSpace(authName), CType(DBNull.Value, Object), authName.Trim())
                    cmd.Parameters.Add("@exp", SqlDbType.NVarChar, 50).Value = expediente
                    ok = (cmd.ExecuteNonQuery() > 0)
                End Using

                ' 2) Si quedó en True, intenta TIMBRAR finmec SOLO LA PRIMERA VEZ y SOLO si ya se cumple todo
                If ok AndAlso value Then
                    Using cmd2 As New SqlCommand("
                    UPDATE dbo.Admisiones
                       SET finmec = COALESCE(finmec, GETDATE())
                     WHERE Expediente = @exp
                       AND ISNULL(mecsi,0)  = 1
                       AND ISNULL(autmec1,0)= 1
                       AND ISNULL(autmec2,0)= 1
                       AND ISNULL(autmec3,0)= 1
                       AND finmec IS NULL;", cn, tr)
                        cmd2.Parameters.Add("@exp", SqlDbType.NVarChar, 50).Value = expediente
                        cmd2.ExecuteNonQuery()
                    End Using
                End If

                tr.Commit()
                Return ok
            End Using
        End Using
    End Function


    ' ====== Badges de estado ======
    Private Sub PaintAutFlags()
        Dim expediente = GetExpediente()
        If String.IsNullOrWhiteSpace(expediente) Then
            litAutMec1.Text = "" : litAutMec2.Text = "" : litAutMec3.Text = ""
            Exit Sub
        End If

        Dim a1 As Boolean = False, a2 As Boolean = False, a3 As Boolean = False
        Dim n1 As String = "", n2 As String = "", n3 As String = ""

        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                SELECT TOP 1 
                    ISNULL(autmec1,0) AS autmec1, ISNULL(autmec1nombre,'') AS autmec1nombre,
                    ISNULL(autmec2,0) AS autmec2, ISNULL(autmec2nombre,'') AS autmec2nombre,
                    ISNULL(autmec3,0) AS autmec3, ISNULL(autmec3nombre,'') AS autmec3nombre
                FROM dbo.Admisiones WHERE Expediente=@exp;", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                cn.Open()
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        a1 = Convert.ToBoolean(rd("autmec1")) : n1 = Convert.ToString(rd("autmec1nombre"))
                        a2 = Convert.ToBoolean(rd("autmec2")) : n2 = Convert.ToString(rd("autmec2nombre"))
                        a3 = Convert.ToBoolean(rd("autmec3")) : n3 = Convert.ToString(rd("autmec3nombre"))
                    End If
                End Using
            End Using
        End Using

        Dim s1 = HttpUtility.HtmlEncode(n1)
        Dim s2 = HttpUtility.HtmlEncode(n2)
        Dim s3 = HttpUtility.HtmlEncode(n3)

        litAutMec1.Text = If(a1,
            "<span class='badge bg-success me-2'>Autorizado</span><span class='text-success'><i class='bi bi-person-check me-1'></i>" & s1 & "</span>",
            "<span class='badge bg-secondary me-2'>Pendiente</span><span class='text-muted'><i class='bi bi-person me-1'></i>—</span>"
        )
        litAutMec2.Text = If(a2,
            "<span class='badge bg-success me-2'>Autorizado</span><span class='text-success'><i class='bi bi-person-check me-1'></i>" & s2 & "</span>",
            "<span class='badge bg-secondary me-2'>Pendiente</span><span class='text-muted'><i class='bi bi-person me-1'></i>—</span>"
        )
        litAutMec3.Text = If(a3,
            "<span class='badge bg-success me-2'>Autorizado</span><span class='text-success'><i class='bi bi-person-check me-1'></i>" & s3 & "</span>",
            "<span class='badge bg-secondary me-2'>Pendiente</span><span class='text-muted'><i class='bi bi-person me-1'></i>—</span>"
        )

        ddlAutMec1.Enabled = Not a1 : txtPassMec1.Enabled = Not a1 : btnAutorizarMec1.Enabled = Not a1
        ddlAutMec2.Enabled = Not a2 : txtPassMec2.Enabled = Not a2 : btnAutorizarMec2.Enabled = Not a2
        ddlAutMec3.Enabled = Not a3 : txtPassMec3.Enabled = Not a3 : btnAutorizarMec3.Enabled = Not a3
    End Sub

    ' ====== Galería / archivos ======
    Private Sub OpenGaleriaForRow(area As String, id As Integer, descripcion As String)
        Dim exp As String = GetExpediente()
        Dim carpetaRelVirt As String = GetCarpetaRelVirtual(exp)
        If String.IsNullOrWhiteSpace(carpetaRelVirt) Then
            ShowStatus("No se encontró CarpetaRel para el expediente.", False)
            Exit Sub
        End If
        Dim virtualFolder As String = CombineVirtual(carpetaRelVirt, SUBFOLDER_MECA)
        Dim physicalFolder As String = Server.MapPath(NormalizeVirtual(virtualFolder))
        Dim combinedPrefix As String = BuildRowPrefix(id, descripcion)
        Dim legacyPrefix As String = BuildSafePrefix(descripcion)

        Dim files As List(Of String) = GetFilesByAnyPrefix(physicalFolder, New List(Of String) From {combinedPrefix, legacyPrefix})
        Dim csv As String = String.Join("|", files)
        Dim title As String = $"{area} · #{id} · {exp} · {combinedPrefix}"

        Dim js As String =
            "window.addEventListener('load', function(){" &
            "  if (window.__openGaleriaDiag) {" &
            "    window.__openGaleriaDiag(" & JsStr(title) & "," & JsStr(NormalizeVirtual(virtualFolder)) & "," & JsStr(combinedPrefix) & "," & JsStr(csv) & ");" &
            "  } else {" &
            "    setTimeout(function(){ if(window.__openGaleriaDiag){ window.__openGaleriaDiag(" & JsStr(title) & "," & JsStr(NormalizeVirtual(virtualFolder)) & "," & JsStr(combinedPrefix) & "," & JsStr(csv) & "); } }, 120);" &
            "  }" &
            "});"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "openGal" & Guid.NewGuid().ToString("N"), js, True)
    End Sub

    Private Function GetCarpetaRelVirtual(expediente As String) As String
        Dim v = If(hfCarpetaRel IsNot Nothing, hfCarpetaRel.Value, Nothing)
        If Not String.IsNullOrWhiteSpace(v) Then Return NormalizeVirtual(v)
        Try
            Using cn As New SqlConnection(CS)
                Using cmd As New SqlCommand("SELECT TOP 1 CarpetaRel FROM dbo.Admisiones WHERE Expediente=@Exp;", cn)
                    cmd.Parameters.AddWithValue("@Exp", expediente)
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Dim cr = Convert.ToString(obj)
                        If Not String.IsNullOrWhiteSpace(cr) Then
                            Return NormalizeVirtual(cr)
                        End If
                    End If
                End Using
            End Using
        Catch
        End Try
        Return NormalizeVirtual($"~/Expedientes/{expediente}")
    End Function

    Private Function CombineVirtual(baseVirt As String, subFolder As String) As String
        Dim p = NormalizeVirtual(baseVirt).TrimEnd("/"c)
        Return p & "/" & subFolder
    End Function

    Private Function NormalizeVirtual(v As String) As String
        Dim p = (If(v, "")).Replace("\", "/")
        If Not p.StartsWith("~/") Then
            If p.StartsWith("/") Then p = "~" & p Else p = "~/" & p
        End If
        Return p
    End Function

    ' ---- Prefijos ----
    Private Function BuildSafePrefix(descripcion As String) As String
        Dim raw As String = If(descripcion, "").Trim()
        If raw.Length > 5 Then raw = raw.Substring(0, 5)
        raw = Regex.Replace(raw, "[^A-Za-z0-9]", "")
        If String.IsNullOrWhiteSpace(raw) Then raw = "REF"
        Return raw
    End Function

    Private Function BuildRowPrefix(id As Integer, descripcion As String) As String
        Return $"{id}-{BuildSafePrefix(descripcion)}"
    End Function

    Private Function GetFilesByPrefix(folderPhysical As String, prefix As String) As List(Of String)
        Dim list As New List(Of String)
        If Not Directory.Exists(folderPhysical) Then Return list
        Dim exts = New String() {"*.jpg", "*.jpeg", "*.png", "*.webp", "*.bmp"}
        Dim all As New List(Of String)
        For Each pattern In exts
            all.AddRange(Directory.GetFiles(folderPhysical, pattern))
        Next
        For Each f In all
            Dim name = Path.GetFileName(f)
            If String.IsNullOrEmpty(prefix) OrElse name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                list.Add(name)
            End If
        Next
        list.Sort(StringComparer.OrdinalIgnoreCase)
        Return list
    End Function

    Private Function GetFilesByAnyPrefix(folderPhysical As String, prefixes As List(Of String)) As List(Of String)
        Dim setOut As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If Not Directory.Exists(folderPhysical) Then Return setOut.ToList()
        Dim exts = New String() {"*.jpg", "*.jpeg", "*.png", "*.webp", "*.bmp"}
        For Each pattern In exts
            For Each f In Directory.GetFiles(folderPhysical, pattern)
                Dim name = Path.GetFileName(f)
                For Each pfx In prefixes
                    If Not String.IsNullOrEmpty(pfx) AndAlso name.StartsWith(pfx, StringComparison.OrdinalIgnoreCase) Then
                        setOut.Add(name)
                        Exit For
                    End If
                Next
            Next
        Next
        Dim res = setOut.ToList()
        res.Sort(StringComparer.OrdinalIgnoreCase)
        Return res
    End Function

    Private Function JsStr(s As String) As String
        If s Is Nothing Then s = ""
        s = s.Replace("\", "\\").Replace("""", "\""").Replace(vbCr, "").Replace(vbLf, "\n")
        Return """" & s & """"
    End Function

    ' ====== UI ======
    Private Sub ShowStatus(msg As String, Optional isOk As Boolean = True)
        If lblStatus Is Nothing Then Return
        lblStatus.Text = msg
        lblStatus.CssClass = "d-block mt-3 fw-semibold " & If(isOk, "text-success", "text-danger")
    End Sub

    ' =====================================================================
    ' ===================  WEBMETHODS (AJAX PageMethods)  ==================
    ' =====================================================================

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function HasMinFotos(expediente As String, id As Integer, descripcion As String, min As Integer) As Boolean
        Try
            Dim carpetaRelVirt As String = GetCarpetaRelVirtualByExp(expediente)
            Dim virtualFolder As String = CombineVirtualStatic(carpetaRelVirt, SUBFOLDER_MECA)
            Dim physicalFolder As String = HttpContext.Current.Server.MapPath(NormalizeVirtualStatic(virtualFolder))
            Dim combinedPrefix As String = BuildRowPrefixStatic(id, descripcion)
            Dim legacyPrefix As String = BuildSafePrefixStatic(descripcion)
            Dim count As Integer = GetFilesByAnyPrefixStatic(physicalFolder, New List(Of String) From {combinedPrefix, legacyPrefix}).Count
            Return (count >= min)
        Catch
            Return False
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function UpdateDescripcionAndRename(expediente As String, id As Integer, oldDescripcion As String, newDescripcion As String) As Object
        Dim newDesc As String = If(newDescripcion, "").Trim()
        If String.IsNullOrWhiteSpace(newDesc) Then
            Return New With {.ok = False, .msg = "La descripción no puede estar vacía."}
        End If

        Dim carpetaRelVirt As String = GetCarpetaRelVirtualByExp(expediente)
        Dim virtualFolder As String = CombineVirtualStatic(carpetaRelVirt, SUBFOLDER_MECA)
        Dim physicalFolder As String = HttpContext.Current.Server.MapPath(NormalizeVirtualStatic(virtualFolder))

        Dim oldPrefixComb As String = BuildRowPrefixStatic(id, oldDescripcion) ' Id-Old5
        Dim oldPrefixLegacy As String = BuildSafePrefixStatic(oldDescripcion)  ' Old5 (sin Id, compatibilidad)
        Dim newPrefix As String = BuildRowPrefixStatic(id, newDesc)            ' Id-New5

        Dim files As List(Of String) = GetFilesByAnyPrefixStatic(physicalFolder, New List(Of String) From {oldPrefixComb, oldPrefixLegacy})

        Dim tempMap As New List(Of Tuple(Of String, String))()
        Dim seq As Integer = 1
        Try
            ' 1) A temporales
            For Each name In files
                ' Evita renombrar si YA coinciden con el nuevo prefijo
                If name.StartsWith(newPrefix, StringComparison.OrdinalIgnoreCase) Then Continue For

                Dim ext = Path.GetExtension(name)
                Dim tmp = $"__tmp_{id}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{ext}"
                File.Move(Path.Combine(physicalFolder, name), Path.Combine(physicalFolder, tmp))
                tempMap.Add(Tuple.Create(tmp, $"{newPrefix}{seq:000}{ext}"))
                seq += 1
            Next

            ' 2) A destino final (evitando colisiones)
            Dim renamed As Integer = 0
            For Each pair In tempMap
                Dim src As String = Path.Combine(physicalFolder, pair.Item1)
                Dim dst As String = Path.Combine(physicalFolder, pair.Item2)

                Dim k As Integer = 0
                Dim finalDst As String = dst
                While File.Exists(finalDst)
                    k += 1
                    finalDst = Path.Combine(physicalFolder, $"{Path.GetFileNameWithoutExtension(dst)}_{k}{Path.GetExtension(dst)}")
                End While

                File.Move(src, finalDst)
                renamed += 1
            Next

            ' 3) Actualizar descripción en BD
            Using cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString)
                Using cmd As New SqlCommand("UPDATE dbo.Refacciones SET Descripcion=@d WHERE Id=@id AND Expediente=@exp;", cn)
                    cmd.Parameters.AddWithValue("@d", newDesc)
                    cmd.Parameters.AddWithValue("@id", id)
                    cmd.Parameters.AddWithValue("@exp", expediente)
                    cn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Return New With {.ok = True, .renamed = renamed, .newPrefix = newPrefix}
        Catch ex As Exception
            Return New With {.ok = False, .msg = "Error al actualizar: " & ex.Message}
        End Try
    End Function

    ' --------- Helpers estáticos para WebMethods ---------
    Private Shared Function GetCarpetaRelVirtualByExp(expediente As String) As String
        Try
            Using cn As New SqlConnection(ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString)
                Using cmd As New SqlCommand("SELECT TOP 1 CarpetaRel FROM dbo.Admisiones WHERE Expediente=@Exp;", cn)
                    cmd.Parameters.AddWithValue("@Exp", expediente)
                    cn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Dim cr = Convert.ToString(obj)
                        If Not String.IsNullOrWhiteSpace(cr) Then
                            Return NormalizeVirtualStatic(cr)
                        End If
                    End If
                End Using
            End Using
        Catch
        End Try
        Return NormalizeVirtualStatic($"~/Expedientes/{expediente}")
    End Function

    Private Shared Function NormalizeVirtualStatic(v As String) As String
        Dim p = (If(v, "")).Replace("\", "/")
        If Not p.StartsWith("~/") Then
            If p.StartsWith("/") Then p = "~" & p Else p = "~/" & p
        End If
        Return p
    End Function

    Private Shared Function CombineVirtualStatic(baseVirt As String, subFolder As String) As String
        Dim p = NormalizeVirtualStatic(baseVirt).TrimEnd("/"c)
        Return p & "/" & subFolder
    End Function

    Private Shared Function BuildSafePrefixStatic(descripcion As String) As String
        Dim raw As String = If(descripcion, "").Trim()
        If raw.Length > 5 Then raw = raw.Substring(0, 5)
        raw = Regex.Replace(raw, "[^A-Za-z0-9]", "")
        If String.IsNullOrWhiteSpace(raw) Then raw = "REF"
        Return raw
    End Function

    Private Shared Function BuildRowPrefixStatic(id As Integer, descripcion As String) As String
        Return $"{id}-{BuildSafePrefixStatic(descripcion)}"
    End Function

    Private Shared Function GetFilesByPrefixStatic(folderPhysical As String, prefix As String) As List(Of String)
        Dim list As New List(Of String)
        If Not Directory.Exists(folderPhysical) Then Return list
        Dim exts = New String() {"*.jpg", "*.jpeg", "*.png", "*.webp", "*.bmp"}
        Dim all As New List(Of String)
        For Each pattern In exts
            all.AddRange(Directory.GetFiles(folderPhysical, pattern))
        Next
        For Each f In all
            Dim name = Path.GetFileName(f)
            If String.IsNullOrEmpty(prefix) OrElse name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                list.Add(name)
            End If
        Next
        list.Sort(StringComparer.OrdinalIgnoreCase)
        Return list
    End Function

    Private Shared Function GetFilesByAnyPrefixStatic(folderPhysical As String, prefixes As List(Of String)) As List(Of String)
        Dim setOut As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If Not Directory.Exists(folderPhysical) Then Return setOut.ToList()
        Dim exts = New String() {"*.jpg", "*.jpeg", "*.png", "*.webp", "*.bmp"}
        For Each pattern In exts
            For Each f In Directory.GetFiles(folderPhysical, pattern)
                Dim name = Path.GetFileName(f)
                For Each pfx In prefixes
                    If Not String.IsNullOrEmpty(pfx) AndAlso name.StartsWith(pfx, StringComparison.OrdinalIgnoreCase) Then
                        setOut.Add(name)
                        Exit For
                    End If
                Next
            Next
        Next
        Dim res = setOut.ToList()
        res.Sort(StringComparer.OrdinalIgnoreCase)
        Return res
    End Function

    Private Function TryStampFinMec(expediente As String) As Boolean
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
            UPDATE dbo.Admisiones
            SET finmec = COALESCE(finmec, GETDATE())
            WHERE Expediente = @exp
              AND ISNULL(mecsi,0) = 1
              AND ISNULL(autmec1,0) = 1
              AND ISNULL(autmec2,0) = 1
              AND ISNULL(autmec3,0) = 1
              AND finmec IS NULL;", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                cn.Open()
                Return (cmd.ExecuteNonQuery() > 0)
            End Using
        End Using
    End Function
    Private Function UpdateAdmisionAuthBit(expediente As String, fieldName As String, value As Boolean) As Boolean
        Dim allowed = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {"autmec1", "autmec2", "autmec3"}
        If Not allowed.Contains(fieldName) Then Return False

        Using cn As New SqlConnection(CS)
            cn.Open()
            Using tr = cn.BeginTransaction()
                Dim ok As Boolean

                ' 1) Actualiza el bit solicitado (autmec1/2/3)
                Using cmd As New SqlCommand($"UPDATE dbo.Admisiones SET {fieldName}=@v WHERE Expediente=@exp;", cn, tr)
                    cmd.Parameters.AddWithValue("@v", If(value, 1, 0))
                    cmd.Parameters.AddWithValue("@exp", expediente)
                    ok = (cmd.ExecuteNonQuery() > 0)
                End Using

                ' 2) Si se puso en True, intenta timbrar finmec si ya se cumple la condición global
                If ok AndAlso value Then
                    Using cmd2 As New SqlCommand("
                    UPDATE dbo.Admisiones
                    SET finmec = COALESCE(finmec, GETDATE())
                    WHERE Expediente = @exp
                      AND ISNULL(mecsi,0) = 1
                      AND ISNULL(autmec1,0) = 1
                      AND ISNULL(autmec2,0) = 1
                      AND ISNULL(autmec3,0) = 1
                      AND finmec IS NULL;", cn, tr)
                        cmd2.Parameters.AddWithValue("@exp", expediente)
                        cmd2.ExecuteNonQuery()
                    End Using
                End If

                tr.Commit()
                Return ok
            End Using
        End Using
    End Function


End Class
