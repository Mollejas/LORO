Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.UI.WebControls

Public Class CreateUser
    Inherits System.Web.UI.Page

    ' === Tamaños EXACTOS según tu tabla ===
    Private Const HASH_LEN As Integer = 64   ' dbo.Usuarios.PasswordHash varbinary(64)
    Private Const SALT_LEN As Integer = 32   ' dbo.Usuarios.PasswordSalt varbinary(32)
    Private Const PBKDF2_ITER As Integer = 100000

    Private ReadOnly Property Cs As String
        Get
            Dim csSetting = ConfigurationManager.ConnectionStrings("DaytonaDB")
            If csSetting Is Nothing OrElse String.IsNullOrWhiteSpace(csSetting.ConnectionString) Then
                Throw New Exception("No se encontró la cadena de conexión 'DaytonaDB' en web.config.")
            End If
            Return csSetting.ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    ' ========= UTIL =========
    Private Sub ShowMsg(text As String, Optional ok As Boolean = False)
        lblMsg.Text = text
        lblMsg.CssClass = "msg show " & If(ok, "ok", "err")
    End Sub

    ' Salt aleatorio (32 bytes)
    Private Function GenerateSalt() As Byte()
        Dim salt(SALT_LEN - 1) As Byte
        Using rng = RandomNumberGenerator.Create()
            rng.GetBytes(salt)
        End Using
        Return salt
    End Function

    ' Derivar clave/hash (64 bytes) con PBKDF2 usando el salt
    Private Function DeriveHash(password As String, salt As Byte()) As Byte()
        If String.IsNullOrEmpty(password) Then Return Nothing
        Using kdf As New Rfc2898DeriveBytes(password, salt, PBKDF2_ITER, HashAlgorithmName.SHA256)
            Return kdf.GetBytes(HASH_LEN)
        End Using
    End Function

    Private Function GetParidadFromChecks() As String
        If chkPar.Checked AndAlso Not chkNon.Checked Then Return "PAR"
        If chkNon.Checked AndAlso Not chkPar.Checked Then Return "NON"
        Return "" ' Sin selección
    End Function

    Private Sub ClearForm()
        txtNombre.Text = ""
        txtCorreo.Text = ""
        txtTelefono.Text = ""
        txtPassword.Text = ""
        txtConfirm.Text = ""
        chkValidador.Checked = False
        chkAdmin.Checked = False
        chkPar.Checked = False
        chkNon.Checked = False
        lblMsg.Text = ""
        lblMsg.CssClass = "msg"
    End Sub

    ' ========= DATA =========
    Private Sub BindGrid()
        Using cn As New SqlConnection(Cs)
            Using da As New SqlDataAdapter("
                SELECT UsuarioId, Nombre, Correo, Telefono, Validador, EsAdmin, Paridad, FechaAlta
                FROM dbo.Usuarios
                ORDER BY UsuarioId DESC", cn)
                Dim dt As New DataTable()
                da.Fill(dt)
                gvUsuarios.DataSource = dt
                gvUsuarios.DataBind()
            End Using
        End Using
    End Sub

    ' ========= CREATE =========
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)
        Try
            Dim nombre As String = Convert.ToString(txtNombre.Text).Trim()
            Dim correo As String = Convert.ToString(txtCorreo.Text).Trim().ToLowerInvariant()
            Dim telefono As String = Convert.ToString(txtTelefono.Text).Trim()
            Dim pwd As String = Convert.ToString(txtPassword.Text)
            Dim confirm As String = Convert.ToString(txtConfirm.Text)

            If String.IsNullOrWhiteSpace(nombre) Then
                ShowMsg("El nombre es obligatorio.") : Exit Sub
            End If
            If String.IsNullOrWhiteSpace(correo) OrElse Not correo.Contains("@") Then
                ShowMsg("Ingresa un correo válido.") : Exit Sub
            End If
            If String.IsNullOrEmpty(pwd) OrElse pwd.Length < 6 Then
                ShowMsg("La contraseña debe tener al menos 6 caracteres.") : Exit Sub
            End If
            If Not String.Equals(pwd, confirm, StringComparison.Ordinal) Then
                ShowMsg("Las contraseñas no coinciden.") : Exit Sub
            End If

            Dim paridadSel As String = GetParidadFromChecks()
            ' Tu columna Paridad es NOT NULL (NVARCHAR(3)).
            ' Si no eligen, guardamos "NA" para cumplir el NOT NULL.
            Dim paridad As String = If(String.IsNullOrEmpty(paridadSel), "NA", paridadSel)

            Dim validador As Boolean = chkValidador.Checked
            Dim esAdmin As Boolean = chkAdmin.Checked

            ' === Hash con SALT (requerido por tu esquema) ===
            Dim salt As Byte() = GenerateSalt()                   ' 32 bytes
            Dim hash As Byte() = DeriveHash(pwd, salt)            ' 64 bytes

            Using cn As New SqlConnection(Cs)
                cn.Open()
                ' Evitar correos duplicados
                Using chkCmd As New SqlCommand("SELECT COUNT(1) FROM dbo.Usuarios WHERE Correo=@Correo", cn)
                    chkCmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo
                    Dim exists = Convert.ToInt32(chkCmd.ExecuteScalar()) > 0
                    If exists Then
                        ShowMsg("El correo ya está registrado.") : Exit Sub
                    End If
                End Using

                Using cmd As New SqlCommand("
                    INSERT INTO dbo.Usuarios
                        (Nombre, Correo, Telefono, PasswordHash, PasswordSalt, Validador, EsAdmin, Paridad, FechaAlta)
                    VALUES
                        (@Nombre, @Correo, @Telefono, @PasswordHash, @PasswordSalt, @Validador, @EsAdmin, @Paridad, SYSDATETIME());
                    SELECT SCOPE_IDENTITY();", cn)

                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo
                    cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 30).Value =
                        If(String.IsNullOrWhiteSpace(telefono), DBNull.Value, CType(telefono, Object))

                    ' Coinciden con tu DDL:
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, HASH_LEN).Value = hash
                    cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, SALT_LEN).Value = salt

                    cmd.Parameters.Add("@Validador", SqlDbType.Bit).Value = validador
                    cmd.Parameters.Add("@EsAdmin", SqlDbType.Bit).Value = esAdmin
                    cmd.Parameters.Add("@Paridad", SqlDbType.NVarChar, 3).Value = paridad

                    Dim newId = Convert.ToInt32(Convert.ToDecimal(cmd.ExecuteScalar()))
                End Using
            End Using

            BindGrid()
            ClearForm()
            ShowMsg("Usuario creado correctamente.", True)
        Catch ex As Exception
            ShowMsg("Error al guardar: " & ex.Message)
        End Try
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        ClearForm()
    End Sub

    ' ========= GRID =========
    Protected Sub gvUsuarios_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvUsuarios.PageIndex = e.NewPageIndex
        BindGrid()
    End Sub

    Protected Sub gvUsuarios_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvUsuarios.EditIndex = e.NewEditIndex
        BindGrid()
    End Sub

    Protected Sub gvUsuarios_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvUsuarios.EditIndex = -1
        BindGrid()
    End Sub

    Protected Sub gvUsuarios_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        Try
            Dim row As GridViewRow = gvUsuarios.Rows(e.RowIndex)
            Dim id As Integer = Convert.ToInt32(gvUsuarios.DataKeys(e.RowIndex).Value)

            Dim txtNombreEdit As TextBox = TryCast(row.Cells(1).Controls(0), TextBox)
            Dim txtCorreoEdit As TextBox = TryCast(row.Cells(2).Controls(0), TextBox)
            Dim txtTelefonoEdit As TextBox = TryCast(row.Cells(3).Controls(0), TextBox)

            Dim nombre As String = Convert.ToString(If(txtNombreEdit?.Text, "")).Trim()
            Dim correo As String = Convert.ToString(If(txtCorreoEdit?.Text, "")).Trim().ToLowerInvariant()
            Dim telefono As String = Convert.ToString(If(txtTelefonoEdit?.Text, "")).Trim()

            Dim chkV As CheckBox = TryCast(row.FindControl("chkEditValidador"), CheckBox)
            Dim chkA As CheckBox = TryCast(row.FindControl("chkEditAdmin"), CheckBox)
            Dim ddlP As DropDownList = TryCast(row.FindControl("ddlEditParidad"), DropDownList)

            Dim validador As Boolean = If(chkV IsNot Nothing, chkV.Checked, False)
            Dim esAdmin As Boolean = If(chkA IsNot Nothing, chkA.Checked, False)
            Dim paridad As String = If(ddlP IsNot Nothing AndAlso Not String.IsNullOrEmpty(ddlP.SelectedValue), ddlP.SelectedValue, "NA")

            If String.IsNullOrWhiteSpace(nombre) Then
                ShowMsg("El nombre es obligatorio.") : Exit Sub
            End If
            If String.IsNullOrWhiteSpace(correo) OrElse Not correo.Contains("@") Then
                ShowMsg("Correo inválido.") : Exit Sub
            End If

            Using cn As New SqlConnection(Cs)
                cn.Open()
                Using chkCmd As New SqlCommand("
                    SELECT COUNT(1) FROM dbo.Usuarios WHERE Correo=@Correo AND UsuarioId<>@Id", cn)
                    chkCmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo
                    chkCmd.Parameters.Add("@Id", SqlDbType.Int).Value = id
                    If Convert.ToInt32(chkCmd.ExecuteScalar()) > 0 Then
                        ShowMsg("Ese correo ya pertenece a otro usuario.") : Exit Sub
                    End If
                End Using

                Using cmd As New SqlCommand("
                    UPDATE dbo.Usuarios
                    SET Nombre=@Nombre, Correo=@Correo, Telefono=@Telefono,
                        Validador=@Validador, EsAdmin=@EsAdmin, Paridad=@Paridad
                    WHERE UsuarioId=@Id", cn)

                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo
                    cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 30).Value =
                        If(String.IsNullOrWhiteSpace(telefono), DBNull.Value, CType(telefono, Object))
                    cmd.Parameters.Add("@Validador", SqlDbType.Bit).Value = validador
                    cmd.Parameters.Add("@EsAdmin", SqlDbType.Bit).Value = esAdmin
                    cmd.Parameters.Add("@Paridad", SqlDbType.NVarChar, 3).Value = paridad
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            gvUsuarios.EditIndex = -1
            BindGrid()
            ShowMsg("Usuario actualizado.", True)
        Catch ex As Exception
            ShowMsg("Error al actualizar: " & ex.Message)
        End Try
    End Sub

    Protected Sub gvUsuarios_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Try
            Dim id As Integer = Convert.ToInt32(gvUsuarios.DataKeys(e.RowIndex).Value)
            Using cn As New SqlConnection(Cs)
                cn.Open()
                Using cmd As New SqlCommand("DELETE FROM dbo.Usuarios WHERE UsuarioId=@Id", cn)
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            BindGrid()
            ShowMsg("Usuario eliminado.", True)
        Catch ex As Exception
            ShowMsg("Error al eliminar: " & ex.Message)
        End Try
    End Sub
End Class
