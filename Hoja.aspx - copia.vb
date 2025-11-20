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


Partial Public Class Hoja
        Inherits System.Web.UI.Page

        Private Const MAX_SIDE As Integer = 1600
        Private Const JPEG_QUALITY As Long = 88
        Private Const SUBFOLDER_NAME As String = "1. DOCUMENTOS DE INGRESO"
        Private Const PRINCIPAL_NAME As String = "principal.jpg"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Dim sid = Request.QueryString("id")
                If String.IsNullOrWhiteSpace(sid) OrElse Not sid.All(AddressOf Char.IsDigit) Then Exit Sub

                hidId.Value = sid
                lblId.Text = sid

                CargarAdmision(CInt(sid))
                UpdateUIFromPrincipal()
            End If
        End Sub

        '====================== Datos de admisión ======================
        Private Sub CargarAdmision(id As Integer)
            Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
            If cs Is Nothing Then Exit Sub

            Using cn As New SqlConnection(cs.ConnectionString)
                cn.Open()
                Using cmd As New SqlCommand("SELECT * FROM admisiones WHERE Id=@Id", cn)
                    cmd.Parameters.AddWithValue("@Id", id)
                    Using rd = cmd.ExecuteReader()
                        If Not rd.Read() Then Exit Sub

                        Dim expediente = GetStr(rd, "Expediente")
                        Dim siniestro = CoalesceNonEmpty(GetStr(rd, "SiniestroIdent"), GetStr(rd, "SiniestroGen"))
                        Dim asegurado = CoalesceNonEmpty(GetStr(rd, "Asegurado"), GetStr(rd, "AseguradoNombre"))
                        Dim telefono = GetStr(rd, "Telefono")
                        Dim correo = GetStr(rd, "Correo")

                        Dim marca = GetStr(rd, "Marca")
                        Dim tipo = GetStr(rd, "Tipo")
                        Dim modelo = GetStr(rd, "Modelo")
                        Dim color = GetStr(rd, "Color")
                        Dim placas = GetStr(rd, "Placas")

                        Dim carpetarel = GetStr(rd, "carpetarel")

                        lblExpediente.Text = expediente
                        lblSiniestro.Text = siniestro
                        lblAsegurado.Text = asegurado
                        lblTelefono.Text = telefono
                        lblCorreo.Text = correo
                        lblVehiculo.Text = BuildVehiculo(marca, tipo, modelo, color, placas)

                        hidCarpeta.Value = carpetarel
                        lblCarpeta.Text = If(String.IsNullOrWhiteSpace(carpetarel), "(sin carpeta)", carpetarel)
                    End Using
                End Using
            End Using
        End Sub

        Private Function BuildVehiculo(marca As String, tipo As String, modelo As String, color As String, placas As String) As String
            Dim partes As New List(Of String)
            If Not String.IsNullOrWhiteSpace(marca) Then partes.Add(marca.Trim())
            If Not String.IsNullOrWhiteSpace(tipo) Then partes.Add(tipo.Trim())
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

        '====================== UI principal: mostrar/ocultar controles ======================
        Private Sub UpdateUIFromPrincipal()
            If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then
                imgPreview.ImageUrl = ""
                fuImagen.Visible = True
                btnSubirGuardar.Visible = True
                divFile.Visible = True
                divBotonSubir.Visible = True
                btnEliminarPrincipal.Visible = False
                Exit Sub
            End If

            Dim ruta = Path.Combine(ObtenerSubcarpetaDestinoFisica(), PRINCIPAL_NAME)
            If File.Exists(ruta) Then
                ' Cargar y ocultar controles de carga
                Dim bytes = File.ReadAllBytes(ruta)
                imgPreview.ImageUrl = "data:image/jpeg;base64," & Convert.ToBase64String(bytes)
                fuImagen.Visible = False
                btnSubirGuardar.Visible = False
                divFile.Visible = False
                divBotonSubir.Visible = True ' deja visible por el botón "Agregar varias"
                btnEliminarPrincipal.Visible = True
            Else
                ' No existe: mostrar controles de carga
                imgPreview.ImageUrl = ""
                fuImagen.Visible = True
                btnSubirGuardar.Visible = True
                divFile.Visible = True
                divBotonSubir.Visible = True
                btnEliminarPrincipal.Visible = False
            End If
        End Sub

        '====================== Un botón: subir + guardar principal.jpg ======================
        Protected Sub btnSubirGuardar_Click(sender As Object, e As EventArgs) Handles btnSubirGuardar.Click
            If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
            If Not fuImagen.HasFile Then Exit Sub

            Dim ext As String = Path.GetExtension(fuImagen.FileName).ToLower()
            Dim permitidas = New String() {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"}
            If Not permitidas.Contains(ext) Then Exit Sub

            Dim originalBytes As Byte()
            Using ms As New MemoryStream()
                fuImagen.PostedFile.InputStream.CopyTo(ms)
                originalBytes = ms.ToArray()
            End Using

            Dim salidaJpg As Byte() = A_Jpeg_Redimensionado(originalBytes, MAX_SIDE, MAX_SIDE, JPEG_QUALITY)

            Dim carpetaDestinoFisica As String = ObtenerSubcarpetaDestinoFisica()
            If Not Directory.Exists(carpetaDestinoFisica) Then Directory.CreateDirectory(carpetaDestinoFisica)

            Dim rutaPrincipal As String = Path.Combine(carpetaDestinoFisica, PRINCIPAL_NAME)
            File.WriteAllBytes(rutaPrincipal, salidaJpg)

            ' Actualiza UI
            UpdateUIFromPrincipal()
        End Sub

        '====================== Eliminar principal.jpg (tache) ======================
        Protected Sub btnEliminarPrincipal_Click(sender As Object, e As EventArgs)
            If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub
            Dim ruta = Path.Combine(ObtenerSubcarpetaDestinoFisica(), PRINCIPAL_NAME)
            Try
                If File.Exists(ruta) Then File.Delete(ruta)
            Catch
                ' si no se puede borrar, no rompas la UI
            End Try
            ' Refresca UI para mostrar FileUpload y botón otra vez
            UpdateUIFromPrincipal()
        End Sub

        '====================== Múltiples imágenes (modal) ======================
        Protected Sub btnGuardarMultiples_Click(sender As Object, e As EventArgs) Handles btnGuardarMultiples.Click
            If String.IsNullOrWhiteSpace(hidCarpeta.Value) Then Exit Sub

            Dim carpetaDestinoFisica As String = ObtenerSubcarpetaDestinoFisica()
            If Not Directory.Exists(carpetaDestinoFisica) Then Directory.CreateDirectory(carpetaDestinoFisica)

            Dim archivos = Request.Files
            Dim indice As Integer = ObtenerSiguienteIndiceRecep(carpetaDestinoFisica)

            For i As Integer = 0 To archivos.Count - 1
                If archivos.AllKeys(i) Is Nothing Then Continue For
                If Not archivos.AllKeys(i).Equals("fuMultiples", StringComparison.OrdinalIgnoreCase) Then Continue For

                Dim file As HttpPostedFile = archivos(i)
                If file Is Nothing OrElse file.ContentLength <= 0 Then Continue For

                Dim ext As String = Path.GetExtension(file.FileName).ToLower()
                Dim permitidas = New String() {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"}
                If Not permitidas.Contains(ext) Then Continue For

                Using ms As New MemoryStream()
                    file.InputStream.CopyTo(ms)
                    Dim bytes As Byte() = ms.ToArray()
                    Dim salidaJpg As Byte() = A_Jpeg_Redimensionado(bytes, MAX_SIDE, MAX_SIDE, JPEG_QUALITY)

                    Dim nombre As String = $"recep{indice}.jpg"
                    Dim rutaFinal As String = Path.Combine(carpetaDestinoFisica, nombre)
                    System.IO.File.WriteAllBytes(rutaFinal, salidaJpg)

                    indice += 1
                End Using
            Next

            ' No afecta al principal; no es necesario refrescar UI del principal.
        End Sub

        '====================== Utilidades ======================
        Private Function ObtenerSubcarpetaDestinoFisica() As String
            Dim carpetaBaseFisica As String = ResolverCarpetaFisica(hidCarpeta.Value)
            Return Path.Combine(carpetaBaseFisica, SUBFOLDER_NAME)
        End Function

        Private Function ObtenerSiguienteIndiceRecep(folder As String) As Integer
            If Not Directory.Exists(folder) Then Return 1
            Dim maxN As Integer = 0
            Dim regex As New Regex("^recep(\d+)\.jpg$", RegexOptions.IgnoreCase)
            For Each f In Directory.GetFiles(folder, "recep*.jpg")
                Dim name = Path.GetFileName(f)
                Dim m = regex.Match(name)
                If m.Success Then
                    Dim n As Integer
                    If Integer.TryParse(m.Groups(1).Value, n) AndAlso n > maxN Then maxN = n
                End If
            Next
            Return maxN + 1
        End Function

        Private Function A_Jpeg_Redimensionado(inputBytes As Byte(), maxW As Integer, maxH As Integer, calidad As Long) As Byte()
            Using msIn As New MemoryStream(inputBytes)
                Using src As Image = Image.FromStream(msIn)
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

                        Dim codecJpg As ImageCodecInfo = ImageCodecInfo.GetImageEncoders().
                          First(Function(c) c.FormatID = ImageFormat.Jpeg.Guid)
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

        ' Acepta física (C:\...), UNC (\\srv\share), "~", "/", "\" o relativa a la app
        Private Function ResolverCarpetaFisica(carpetaRel As String) As String
            Dim p As String = Convert.ToString(carpetaRel).Trim()
            If String.IsNullOrEmpty(p) Then Throw New Exception("carpetarel vacío.")
            If Path.IsPathRooted(p) Then Return p
            If p.StartsWith("~") OrElse p.StartsWith("/") OrElse p.StartsWith("\") Then
                Return Server.MapPath(p)
            End If
            Return Server.MapPath("~/" & p.TrimStart("/"c, "\"c))
        End Function

    End Class

