Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports System.Linq

Public Class Hojalateria
    Inherits System.Web.UI.Page

    ' ====== Config / Const ======
    Private ReadOnly Property CS As String
        Get
            Return ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString
        End Get
    End Property

    Private Const AREA As String = "HOJALATERIA"
    Private Const SUBFOLDER_HOJA As String = "3. FOTOS DIAGNOSTICO HOJALATERIA"

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

        AddHandler btnAutorizarMec1.Click, AddressOf btnAutorizarMec1_Click
        AddHandler btnAutorizarMec2.Click, AddressOf btnAutorizarMec2_Click
        AddHandler btnAutorizarMec3.Click, AddressOf btnAutorizarMec3_Click
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
                SELECT Id, Cantidad, Descripcion, Autorizado, Checar, Aldo, AldoDateTime
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

    Private Sub InsertRefaccion(categoria As String, cantTxt As TextBox, descTxt As TextBox)
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

        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                INSERT INTO dbo.Refacciones (AdmisionId, Expediente, Area, Categoria, Cantidad, Descripcion, Autorizado, Checar, Aldo, AldoDateTime)
                VALUES (@AdmisionId, @Expediente, @Area, @Categoria, @Cantidad, @Descripcion, 0, 0, 0, NULL);", cn)
                cmd.Parameters.AddWithValue("@AdmisionId", DBNull.Value)
                cmd.Parameters.AddWithValue("@Expediente", expediente)
                cmd.Parameters.AddWithValue("@Area", AREA)
                cmd.Parameters.AddWithValue("@Categoria", categoria)
                cmd.Parameters.AddWithValue("@Cantidad", cant)
                cmd.Parameters.AddWithValue("@Descripcion", desc)
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        cantTxt.Text = ""
        descTxt.Text = ""
        ShowStatus("Guardado correctamente.")
    End Sub

    Protected Sub btnAddSust_Click(sender As Object, e As EventArgs)
        InsertRefaccion("SUSTITUCION", txtCantSust, txtDescSust)
        BindGrid(gvSust, "SUSTITUCION")
    End Sub

    Protected Sub btnAddRep_Click(sender As Object, e As EventArgs)
        InsertRefaccion("REPARACION", txtCantRep, txtDescRep)
        BindGrid(gvRep, "REPARACION")
    End Sub

    ' ====== Pintado de ícono único autorización ======
    Protected Sub gv_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        Dim drv As DataRowView = TryCast(e.Row.DataItem, DataRowView)
        If drv Is Nothing Then Return

        Dim autorizado As Boolean = False
        If Not Convert.IsDBNull(drv("Autorizado")) Then autorizado = Convert.ToBoolean(drv("Autorizado"))

        Dim btnToggle = TryCast(e.Row.FindControl("btnToggleAuto"), LinkButton)
        If btnToggle IsNot Nothing Then
            Dim ico = TryCast(btnToggle.FindControl("icoAuto"), HtmlGenericControl)
            If ico IsNot Nothing Then
                If autorizado Then
                    ico.Attributes("class") = "bi bi-check-circle-fill text-success"
                    btnToggle.ToolTip = "Actualmente: AUTORIZADO (clic para marcar NO AUTORIZADO)"
                Else
                    ico.Attributes("class") = "bi bi-x-octagon-fill text-danger"
                    btnToggle.ToolTip = "Actualmente: NO AUTORIZADO (clic para marcar AUTORIZADO)"
                End If
            End If
        End If
    End Sub

    ' ====== RowCommand: Sustitución ======
    Protected Sub gvSust_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvSust.RowCommand
        Select Case e.CommandName
            Case "TOGGLE_AUTO"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                ToggleAutorizado(id)
                BindGrid(gvSust, "SUSTITUCION")
            Case "TOGGLE_CHECAR"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                ToggleChecar(id)
                BindGrid(gvSust, "SUSTITUCION")
            Case "TOGGLE_ALDO"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                ToggleAldo(id)
                BindGrid(gvSust, "SUSTITUCION")
            Case "VER_FOTOS"
                Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
                Dim id As Integer = Convert.ToInt32(gvSust.DataKeys(rowIndex).Values("Id"))
                Dim descripcion As String = TryCast(gvSust.DataKeys(rowIndex).Values("Descripcion"), String)
                OpenGaleriaForRow("Sustitución", id, descripcion)
        End Select
    End Sub

    ' ====== RowCommand: Reparación ======
    Protected Sub gvRep_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvRep.RowCommand
        Select Case e.CommandName
            Case "TOGGLE_AUTO"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                ToggleAutorizado(id)
                BindGrid(gvRep, "REPARACION")
            Case "TOGGLE_CHECAR"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                ToggleChecar(id)
                BindGrid(gvRep, "REPARACION")
            Case "TOGGLE_ALDO"
                Dim id As Integer = Convert.ToInt32(e.CommandArgument)
                ToggleAldo(id)
                BindGrid(gvRep, "REPARACION")
            Case "VER_FOTOS"
                Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
                Dim id As Integer = Convert.ToInt32(gvRep.DataKeys(rowIndex).Values("Id"))
                Dim descripcion As String = TryCast(gvRep.DataKeys(rowIndex).Values("Descripcion"), String)
                OpenGaleriaForRow("Reparación", id, descripcion)
        End Select
    End Sub

    ' ====== Toggles ======
    Private Sub ToggleAutorizado(id As Integer)
        Dim cur As Boolean = GetBoolField("Autorizado", id)
        UpdateAutorizado(id, Not cur)
    End Sub

    Private Sub ToggleChecar(id As Integer)
        Dim cur As Boolean = GetBoolField("Checar", id)
        UpdateChecar(id, Not cur)
    End Sub

    Private Sub ToggleAldo(id As Integer)
        Dim cur As Boolean = GetBoolField("Aldo", id)
        UpdateAldo(id, Not cur)
    End Sub

    Private Function GetBoolField(fieldName As String, id As Integer) As Boolean
        Dim v As Object = Nothing
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand($"SELECT TOP 1 {fieldName} FROM dbo.Refacciones WHERE Id=@Id;", cn)
                cmd.Parameters.AddWithValue("@Id", id)
                cn.Open()
                v = cmd.ExecuteScalar()
            End Using
        End Using
        If v Is Nothing OrElse v Is DBNull.Value Then Return False
        Return Convert.ToBoolean(v)
    End Function

    Private Sub UpdateAutorizado(id As Integer, value As Boolean)
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("UPDATE dbo.Refacciones SET Autorizado=@v WHERE Id=@Id;", cn)
                cmd.Parameters.AddWithValue("@v", If(value, 1, 0))
                cmd.Parameters.AddWithValue("@Id", id)
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub UpdateChecar(id As Integer, value As Boolean)
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("UPDATE dbo.Refacciones SET Checar=@v WHERE Id=@Id;", cn)
                cmd.Parameters.AddWithValue("@v", If(value, 1, 0))
                cmd.Parameters.AddWithValue("@Id", id)
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub UpdateAldo(id As Integer, value As Boolean)
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                UPDATE dbo.Refacciones
                SET Aldo=@v,
                    AldoDateTime = CASE WHEN @v=1 THEN GETDATE() ELSE NULL END
                WHERE Id=@Id;", cn)
                cmd.Parameters.AddWithValue("@v", If(value, 1, 0))
                cmd.Parameters.AddWithValue("@Id", id)
                cn.Open()
                cmd.ExecuteNonQuery()
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

        If Not UpdateAdmisionAuthBit(expediente, fieldName, True) Then
            ShowStatus("No se pudo actualizar la autorización.", False) : Exit Sub
        End If

        ShowStatus("Autorización registrada.", True)
        PaintAutFlags()
        ddl.Enabled = False : txtPass.Enabled = False
    End Sub

    ' ====== Validación contra VARBINARY (SHA-256) ======
    ' PasswordHash = SHA256( PasswordSalt || UTF8(password) )
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
        If a Is Nothing Then a = Array.Empty(Of Byte)()
        If b Is Nothing Then b = Array.Empty(Of Byte)()
        Dim res(a.Length + b.Length - 1) As Byte
        System.Buffer.BlockCopy(a, 0, res, 0, a.Length)
        System.Buffer.BlockCopy(b, 0, res, a.Length, b.Length)
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

    ' ====== Actualizar bit en Admisiones ======
    Private Function UpdateAdmisionAuthBit(expediente As String, fieldName As String, value As Boolean) As Boolean
        Dim allowed = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {"autmec1", "autmec2", "autmec3"}
        If Not allowed.Contains(fieldName) Then Return False

        Dim sql As String = $"UPDATE dbo.Admisiones SET {fieldName}=@v WHERE Expediente=@exp;"
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@v", If(value, 1, 0))
                cmd.Parameters.AddWithValue("@exp", expediente)
                cn.Open()
                Return (cmd.ExecuteNonQuery() > 0)
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
        Using cn As New SqlConnection(CS)
            Using cmd As New SqlCommand("
                SELECT TOP 1 
                    ISNULL(autmec1,0) AS autmec1,
                    ISNULL(autmec2,0) AS autmec2,
                    ISNULL(autmec3,0) AS autmec3
                FROM dbo.Admisiones WHERE Expediente=@exp;", cn)
                cmd.Parameters.AddWithValue("@exp", expediente)
                cn.Open()
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        a1 = Convert.ToBoolean(rd("autmec1"))
                        a2 = Convert.ToBoolean(rd("autmec2"))
                        a3 = Convert.ToBoolean(rd("autmec3"))
                    End If
                End Using
            End Using
        End Using

        litAutMec1.Text = If(a1, "<span class='badge bg-success'>Autorizado</span>", "<span class='badge bg-secondary'>Pendiente</span>")
        litAutMec2.Text = If(a2, "<span class='badge bg-success'>Autorizado</span>", "<span class='badge bg-secondary'>Pendiente</span>")
        litAutMec3.Text = If(a3, "<span class='badge bg-success'>Autorizado</span>", "<span class='badge bg-secondary'>Pendiente</span>")

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
        Dim virtualFolder As String = CombineVirtual(carpetaRelVirt, SUBFOLDER_HOJA)
        Dim physicalFolder As String = Server.MapPath(NormalizeVirtual(virtualFolder))
        Dim prefix As String = BuildSafePrefix(descripcion)
        Dim files As List(Of String) = GetFilesByPrefix(physicalFolder, prefix)
        Dim csv As String = String.Join("|", files)
        Dim title As String = $"{area} · #{id} · {exp} · {prefix}"

        Dim js As String =
            "window.addEventListener('load', function(){" &
            "  if (window.__openGaleriaDiag) {" &
            "    window.__openGaleriaDiag(" & JsStr(title) & "," & JsStr(NormalizeVirtual(virtualFolder)) & "," & JsStr(prefix) & "," & JsStr(csv) & ");" &
            "  } else {" &
            "    setTimeout(function(){ if(window.__openGaleriaDiag){ window.__openGaleriaDiag(" & JsStr(title) & "," & JsStr(NormalizeVirtual(virtualFolder)) & "," & JsStr(prefix) & "," & JsStr(csv) & "); } }, 120);" &
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

    Private Function BuildSafePrefix(descripcion As String) As String
        Dim raw As String = If(descripcion, "").Trim()
        If raw.Length > 5 Then raw = raw.Substring(0, 5)
        raw = Regex.Replace(raw, "[^A-Za-z0-9]", "")
        Return raw
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

End Class
