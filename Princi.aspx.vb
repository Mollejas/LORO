Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.UI
Imports System.Web.UI.WebControls

Partial Public Class PRINCI
    Inherits System.Web.UI.Page

    ' ===== Helpers para encontrar controles dentro del ContentPlaceHolder "MainContent" =====
    Private ReadOnly Property MC As ContentPlaceHolder
        Get
            Return TryCast(Master.FindControl("MainContent"), ContentPlaceHolder)
        End Get
    End Property

    Private ReadOnly Property PnlResultadosCtl As Panel
        Get
            Return TryCast(MC?.FindControl("pnlResultados"), Panel)
        End Get
    End Property

    Private ReadOnly Property LblResultadosCtl As Label
        Get
            Return TryCast(MC?.FindControl("lblResultados"), Label)
        End Get
    End Property

    Private ReadOnly Property RptResultadosCtl As Repeater
        Get
            Return TryCast(MC?.FindControl("rptResultados"), Repeater)
        End Get
    End Property

    ' ====== MÉTRICAS (tu lógica existente) ======
    Private Sub CargarMetricas()
        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString

        Using cn As New SqlConnection(cs)
            Using cmd As New SqlCommand()
                cmd.Connection = cn

                ' WHERE dinámico según filtros (sin afectar el buscador/redirect)
                Dim whereClause As String = " WHERE 1=1 "
                If Not String.IsNullOrWhiteSpace(TextBox1.Text) Then ' Carpeta -> Expediente
                    whereClause &= " AND A.Carpeta LIKE @Carpeta "
                    cmd.Parameters.Add("@Carpeta", SqlDbType.NVarChar, 50).Value = "%" & TextBox1.Text.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(TextBox2.Text) Then ' Reporte -> SiniestroGen
                    whereClause &= " AND A.Reporte LIKE @Reporte "
                    cmd.Parameters.Add("@Reporte", SqlDbType.NVarChar, 50).Value = "%" & TextBox2.Text.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(TextBox3.Text) Then ' Placas
                    whereClause &= " AND A.Placas LIKE @Placas "
                    cmd.Parameters.Add("@Placas", SqlDbType.NVarChar, 20).Value = "%" & TextBox3.Text.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(TextBox4.Text) Then ' VIN (Serie)
                    whereClause &= " AND A.Serie LIKE @Serie "
                    cmd.Parameters.Add("@Serie", SqlDbType.NVarChar, 50).Value = "%" & TextBox4.Text.Trim() & "%"
                End If

                cmd.CommandText =
";WITH A AS (
    SELECT *
    FROM dbo.Admisiones AS A" & whereClause & "
),
Base AS (
    SELECT
        A.Estatus,
        A.VigenciaHasta,
        A.FechaCreacion,
        DATEDIFF(DAY, A.FechaCreacion, GETDATE()) AS DiasDesdeCreacion
    FROM A
)
SELECT
    SUM(CASE WHEN Estatus = 'PISO' THEN 1 ELSE 0 END)  AS PisoTotal,
    SUM(CASE WHEN Estatus = 'TRANSITO' THEN 1 ELSE 0 END) AS TransTotal,
    SUM(CASE WHEN Estatus = 'PISO' AND VigenciaHasta IS NOT NULL AND VigenciaHasta < CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS PisoVencidas,
    SUM(CASE WHEN Estatus = 'PISO' AND DiasDesdeCreacion >= 30 THEN 1 ELSE 0 END)   AS PisoMas30,
    CAST(0 AS INT) AS Piso100,
    SUM(CASE WHEN Estatus = 'TRANSITO' AND DiasDesdeCreacion >= 10 THEN 1 ELSE 0 END) AS Trans10,
    SUM(CASE WHEN Estatus = 'TRANSITO' AND DiasDesdeCreacion >= 15 THEN 1 ELSE 0 END) AS Trans15,
    SUM(CASE WHEN Estatus = 'TRANSITO' AND DiasDesdeCreacion >= 20 THEN 1 ELSE 0 END) AS Trans20,
    SUM(CASE WHEN Estatus = 'TRANSITO' AND DiasDesdeCreacion >= 30 THEN 1 ELSE 0 END) AS Trans30,
    CAST(0 AS INT) AS Trans100
FROM Base;"

                cn.Open()
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim pisoTotal As Integer = If(rd("PisoTotal") Is DBNull.Value, 0, Convert.ToInt32(rd("PisoTotal")))
                        Dim transTotal As Integer = If(rd("TransTotal") Is DBNull.Value, 0, Convert.ToInt32(rd("TransTotal")))

                        Dim pisoVencidas As Integer = If(rd("PisoVencidas") Is DBNull.Value, 0, Convert.ToInt32(rd("PisoVencidas")))
                        Dim pisoMas30 As Integer = If(rd("PisoMas30") Is DBNull.Value, 0, Convert.ToInt32(rd("PisoMas30")))
                        Dim piso100 As Integer = If(rd("Piso100") Is DBNull.Value, 0, Convert.ToInt32(rd("Piso100")))

                        Dim trans10 As Integer = If(rd("Trans10") Is DBNull.Value, 0, Convert.ToInt32(rd("Trans10")))
                        Dim trans15 As Integer = If(rd("Trans15") Is DBNull.Value, 0, Convert.ToInt32(rd("Trans15")))
                        Dim trans20 As Integer = If(rd("Trans20") Is DBNull.Value, 0, Convert.ToInt32(rd("Trans20")))
                        Dim trans30 As Integer = If(rd("Trans30") Is DBNull.Value, 0, Convert.ToInt32(rd("Trans30")))
                        Dim trans100 As Integer = If(rd("Trans100") Is DBNull.Value, 0, Convert.ToInt32(rd("Trans100")))

                        TextBox10.Text = pisoTotal.ToString()
                        TextBox14.Text = transTotal.ToString()

                        TextBox11.Text = pisoVencidas.ToString()
                        TextBox12.Text = pisoMas30.ToString()
                        TextBox13.Text = piso100.ToString()

                        TextBox15.Text = trans10.ToString()
                        TextBox16.Text = trans15.ToString()
                        TextBox17.Text = trans20.ToString()
                        TextBox18.Text = trans30.ToString()
                        TextBox20.Text = trans100.ToString()

                        TextBox19.Text = (piso100 + trans100).ToString()

                        TextBox25.Text = transTotal.ToString()
                        TextBox26.Text = pisoTotal.ToString()
                        TextBox27.Text = (transTotal + pisoTotal).ToString()
                    Else
                        LimpiarMetricas()
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub LimpiarMetricas()
        TextBox10.Text = "0" : TextBox11.Text = "0" : TextBox12.Text = "0" : TextBox13.Text = "0"
        TextBox14.Text = "0" : TextBox15.Text = "0" : TextBox16.Text = "0" : TextBox17.Text = "0" : TextBox18.Text = "0" : TextBox20.Text = "0"
        TextBox19.Text = "0"
        TextBox25.Text = "0" : TextBox26.Text = "0" : TextBox27.Text = "0"
    End Sub

    ' ====== BUSCADOR (redirigir a Hoja.aspx o listar resultados) ======
    Private Sub BuscarYRedirigir()
        ' Oculta/limpia resultados (si existen)
        If PnlResultadosCtl IsNot Nothing Then PnlResultadosCtl.Visible = False
        If LblResultadosCtl IsNot Nothing Then LblResultadosCtl.Text = ""

        Dim cs As String = ConfigurationManager.ConnectionStrings("DaytonaDB").ConnectionString

        Using cn As New SqlConnection(cs)
            Using cmd As New SqlCommand()
                cmd.Connection = cn

                ' WHERE a partir de filtros (AND si hay varios)
                Dim where As String = " WHERE 1=1 "
                If Not String.IsNullOrWhiteSpace(TextBox1.Text) Then ' Carpeta -> Expediente
                    where &= " AND A.Expediente LIKE @Expediente "
                    cmd.Parameters.Add("@Expediente", SqlDbType.NVarChar, 50).Value = "%" & TextBox1.Text.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(TextBox2.Text) Then ' Reporte -> SiniestroGen
                    where &= " AND A.SiniestroGen LIKE @Siniestro "
                    cmd.Parameters.Add("@Siniestro", SqlDbType.NVarChar, 50).Value = "%" & TextBox2.Text.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(TextBox3.Text) Then ' Placas
                    where &= " AND A.Placas LIKE @Placas "
                    cmd.Parameters.Add("@Placas", SqlDbType.NVarChar, 20).Value = "%" & TextBox3.Text.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(TextBox4.Text) Then ' VIN (Serie)
                    where &= " AND A.Serie LIKE @Serie "
                    cmd.Parameters.Add("@Serie", SqlDbType.NVarChar, 50).Value = "%" & TextBox4.Text.Trim() & "%"
                End If

                ' Nada escrito → muestra aviso y termina
                If where = " WHERE 1=1 " Then
                    If PnlResultadosCtl IsNot Nothing Then PnlResultadosCtl.Visible = True
                    If LblResultadosCtl IsNot Nothing Then LblResultadosCtl.Text = "Escribe al menos un criterio para buscar."
                    If RptResultadosCtl IsNot Nothing Then
                        RptResultadosCtl.DataSource = Nothing
                        RptResultadosCtl.DataBind()
                    End If
                    Exit Sub
                End If

                cmd.CommandText =
"SELECT TOP (50)
    A.Id,
    A.Expediente,
    A.SiniestroGen,
    A.Placas
FROM dbo.Admisiones AS A WITH (NOLOCK)
" & where & "
ORDER BY A.FechaCreacion DESC;"

                Dim dt As New DataTable()
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

                If dt.Rows.Count = 0 Then
                    If PnlResultadosCtl IsNot Nothing Then PnlResultadosCtl.Visible = True
                    If LblResultadosCtl IsNot Nothing Then LblResultadosCtl.Text = "Sin resultados para los criterios ingresados."
                    If RptResultadosCtl IsNot Nothing Then
                        RptResultadosCtl.DataSource = Nothing
                        RptResultadosCtl.DataBind()
                    End If

                ElseIf dt.Rows.Count = 1 Then
                    Dim id As Integer = Convert.ToInt32(dt.Rows(0)("Id"))
                    Response.Redirect("Hoja.aspx?id=" & id.ToString(), False)
                    Context.ApplicationInstance.CompleteRequest()

                Else
                    If PnlResultadosCtl IsNot Nothing Then PnlResultadosCtl.Visible = True
                    If LblResultadosCtl IsNot Nothing Then LblResultadosCtl.Text = "Se encontraron " & dt.Rows.Count.ToString() & " resultados. Selecciona uno:"
                    If RptResultadosCtl IsNot Nothing Then
                        RptResultadosCtl.DataSource = dt
                        RptResultadosCtl.DataBind()
                    End If
                End If
            End Using
        End Using
    End Sub

    ' ====== Ciclo de vida ======
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarMetricas()
        End If
    End Sub

    Protected Sub ButtonSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        CargarMetricas()     ' opcional: métricas con filtros
        BuscarYRedirigir()   ' abre Hoja.aspx si hay 1 match; lista si hay varios
    End Sub
End Class
