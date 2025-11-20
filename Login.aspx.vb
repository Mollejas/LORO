Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Security.Cryptography
Imports System.Text

Public Class Login
    Inherits System.Web.UI.Page

    ' === Deben coincidir con CreateUser ===
    Private Const HASH_LEN As Integer = 64            ' PasswordHash VARBINARY(64)
    Private Const PBKDF2_ITER As Integer = 100000     ' Iteraciones PBKDF2

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session.Clear()

            ' Rellenar correo recordado (si existe cookie)
            Dim c = Request.Cookies("CorreoRecordado")
            If c IsNot Nothing AndAlso Not String.IsNullOrEmpty(c.Value) Then
                txtCorreo.Text = c.Value
                chkRecordar.Checked = True
            End If
        End If

        ' Asegura el handler en cada carga (como lo tenías)
        AddHandler btnEntrar.Click, AddressOf Me.btnEntrar_Click
    End Sub

    Private Sub MostrarError(mensaje As String)
        lblError.Text = Server.HtmlEncode(mensaje)
        lblError.CssClass = "alert show"
    End Sub

    ' Comparación en tiempo constante para evitar timing attacks
    Private Function SlowEquals(a As Byte(), b As Byte()) As Boolean
        If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then Return False
        Dim diff As Integer = 0
        For i As Integer = 0 To a.Length - 1
            diff = diff Or (a(i) Xor b(i))
        Next
        Return diff = 0
    End Function

    ' Verifica contraseña admitiendo:
    ' - Nuevo: PBKDF2-SHA256 100k (64 bytes)
    ' - Legado: SHA256( salt + password ) (32 bytes)
    Private Function VerifyPassword(pass As String, salt As Byte(), stored As Byte()) As Boolean
        If String.IsNullOrEmpty(pass) OrElse salt Is Nothing OrElse stored Is Nothing Then Return False

        Dim calc() As Byte

        If stored.Length = HASH_LEN Then
            ' === PBKDF2-SHA256 (como CreateUser) ===
            Using kdf As New Rfc2898DeriveBytes(pass, salt, PBKDF2_ITER, HashAlgorithmName.SHA256)
                calc = kdf.GetBytes(HASH_LEN) ' 64 bytes
            End Using

        ElseIf stored.Length = 32 Then
            ' === Compatibilidad con usuarios viejos ===
            Dim passBytes = Encoding.UTF8.GetBytes(pass)
            Dim mix(salt.Length + passBytes.Length - 1) As Byte
            System.Buffer.BlockCopy(salt, 0, mix, 0, salt.Length)
            System.Buffer.BlockCopy(passBytes, 0, mix, salt.Length, passBytes.Length)
            Using sha As SHA256 = SHA256.Create()
                calc = sha.ComputeHash(mix) ' 32 bytes
            End Using

        Else
            ' Tamaño inesperado: formato no reconocido
            Return False
        End If

        Return SlowEquals(calc, stored)
    End Function

    Private Sub btnEntrar_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Normaliza el correo (CreateUser guarda en minúsculas)
        Dim correo As String = If(txtCorreo.Text, String.Empty).Trim().ToLowerInvariant()
        Dim pass As String = If(txtPassword.Text, String.Empty)

        If String.IsNullOrWhiteSpace(correo) OrElse String.IsNullOrWhiteSpace(pass) Then
            MostrarError("Por favor, ingresa tu correo y contraseña.")
            Exit Sub
        End If

        Try
            Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString

            Using cn As New SqlConnection(cs)
                Using cmd As New SqlCommand("dbo.usp_Usuarios_Login", cn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo

                    cn.Open()
                    Using rd As SqlDataReader = cmd.ExecuteReader(CommandBehavior.SingleRow)
                        If Not rd.Read() Then
                            MostrarError("Usuario o contraseña inválidos.")
                            Exit Sub
                        End If

                        ' Campos básicos
                        Dim usuarioId As Integer = rd.GetInt32(rd.GetOrdinal("UsuarioId"))
                        Dim nombre As String = rd.GetString(rd.GetOrdinal("Nombre"))
                        Dim esAdmin As Boolean = rd.GetBoolean(rd.GetOrdinal("EsAdmin"))
                        Dim validador As Boolean = rd.GetBoolean(rd.GetOrdinal("Validador"))

                        ' === OJO: leer por los alias del SP ===
                        ' U.PasswordSalt  AS Salt
                        ' U.PasswordHash  AS ContrasenaHash
                        Dim saltOrdinal As Integer = rd.GetOrdinal("Salt")
                        Dim hashOrdinal As Integer = rd.GetOrdinal("ContrasenaHash")

                        ' Lee VARBINARY con GetBytes
                        Dim saltLen As Integer = CInt(rd.GetBytes(saltOrdinal, 0, Nothing, 0, 0))
                        Dim saltBytes(saltLen - 1) As Byte
                        rd.GetBytes(saltOrdinal, 0, saltBytes, 0, saltLen)

                        Dim hashLen As Integer = CInt(rd.GetBytes(hashOrdinal, 0, Nothing, 0, 0))
                        Dim hashBytes(hashLen - 1) As Byte
                        rd.GetBytes(hashOrdinal, 0, hashBytes, 0, hashLen)

                        ' Verificación de contraseña (PBKDF2 o legado)
                        If Not VerifyPassword(pass, saltBytes, hashBytes) Then
                            MostrarError("Usuario o contraseña inválidos.")
                            Exit Sub
                        End If

                        ' Validación de cuenta
                        If Not validador Then
                            MostrarError("Tu cuenta aún no ha sido validada.")
                            Exit Sub
                        End If

                        ' Sesión
                        Session("UsuarioId") = usuarioId
                        Session("Nombre") = nombre
                        Session("Correo") = correo
                        Session("EsAdmin") = esAdmin
                    End Using
                End Using
            End Using

            ' Recordar correo
            If chkRecordar.Checked Then
                Dim cookie As New HttpCookie("CorreoRecordado", correo) With {
                    .HttpOnly = True,
                    .Secure = Request.IsSecureConnection,
                    .Expires = DateTime.UtcNow.AddDays(14)
                }
                Response.Cookies.Add(cookie)
            Else
                If Request.Cookies("CorreoRecordado") IsNot Nothing Then
                    Dim c = New HttpCookie("CorreoRecordado") With {
                        .Expires = DateTime.UtcNow.AddDays(-1)
                    }
                    Response.Cookies.Add(c)
                End If
            End If

            ' Redirect
            Dim ret As String = Request.QueryString("returnUrl")
            If Not String.IsNullOrWhiteSpace(ret) Then
                Response.Redirect(ret, False)
            Else
                Response.Redirect("princi.aspx", False)
            End If

        Catch ex As Exception
            MostrarError("Ocurrió un error al iniciar sesión.")
            ' TODO: registrar ex.ToString() en log interno
        End Try
    End Sub
End Class
