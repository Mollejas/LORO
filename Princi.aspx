<%@ Page Title="Home Page"
    Language="VB"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="true"
    CodeFile="PRINCI.aspx.vb"
    Inherits="PRINCI" %>

<asp:Content ID="HeadBlock" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- Bootstrap Icons (para el título) -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
  <!-- Chart.js -->
  <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.4/dist/chart.umd.min.js"></script>

  <style>
    :root{
      --brand: var(--primary, #10b981);
      --brand-600: var(--primary-hover, #059669);
      --text: var(--text-body, #1f2937);
      --muted: var(--text-muted, #6b7280);
      --bg: var(--bg-main, #fafafa);
      --card: var(--surface, #ffffff);
      --card-border: var(--border-color, #e5e7eb);
      --chip: var(--primary-light, #ecfdf5);
    }

    .page-wrapper{ min-height: 100vh; display:flex; align-items:flex-start; justify-content:center; background: var(--bg); color: var(--text); padding: 1rem 0 3rem 0; }
    .content-card{ width:100%; max-width: 1400px; background: var(--card); border: 1px solid var(--card-border); border-radius: 18px; box-shadow: 0 8px 28px rgba(2,8,23,.06); padding: clamp(16px, 2vw, 28px); }

    .section-title{ display:inline-flex; align-items:center; gap:.55rem; font-weight: 800; letter-spacing:.2px; font-size:1rem; color: var(--brand-600); background: var(--chip); border: 1px solid rgba(16,185,129,.18); padding:.45rem .85rem; border-radius: 999px; margin: 1.25rem 0 .85rem 0; }
    .section-title i{ font-size: 1rem; }

    /* ====== Filtros destacados en verde ====== */
    .filters-wrap{ background: linear-gradient(0deg, rgba(16,185,129,.12), rgba(16,185,129,.12)); border: 1px solid rgba(16,185,129,.25); box-shadow: 0 10px 28px rgba(16,185,129,.10); border-radius: 14px; padding: 14px; margin-bottom: 8px; }
    .filters-wrap .form-label{ color:#065f46; font-weight:800; letter-spacing:.2px; text-transform:uppercase; font-size:.9rem; }
    .filters-wrap .form-control{ background:#fff; border:1px solid var(--card-border); border-radius:12px; height:44px; font-weight:700; letter-spacing:.3px; }
    .filters-wrap .form-control:focus{ border-color: var(--brand); box-shadow: 0 0 0 .25rem rgba(16,185,129,.18); }
    .filters-wrap .btn-primary{ background: var(--brand); border-color: var(--brand); font-weight:800; height:44px; }
    .filters-wrap .btn-primary:hover{ background: var(--brand-600); border-color: var(--brand-600); }

    /* ===== Resultados de búsqueda ===== */
    .results-card{ background:#fff; border:1px solid var(--card-border); border-radius:14px; padding:12px 14px; box-shadow: 0 6px 22px rgba(2,8,23,.06); }
    .results-item{ display:flex; align-items:center; justify-content:space-between; padding:10px 8px; border-bottom:1px dashed #e5e7eb; }
    .results-item:last-child{ border-bottom:0; }
    .results-meta{ font-size:.9rem; color:#374151; font-weight:700; }
    .results-link a{ font-weight:800; text-decoration:none; background: var(--chip); color: var(--brand-600); padding:.35rem .7rem; border-radius:8px; border:1px solid rgba(16,185,129,.25); display:inline-block; }
    .results-link a:hover{ background: var(--brand); color:#fff; border-color: var(--brand); }

    .divider{ height:1px; width:100%; background: linear-gradient(90deg, transparent, var(--card-border), transparent); margin: 1rem 0 1.25rem 0; }

    .kpi-card{ background: #fff; border: 1px solid var(--card-border); border-radius: 18px; padding: 16px; height: 100%; box-shadow: 0 6px 22px rgba(2,8,23,.06); }
    .kpi-card.kpi-green{ position: relative; background: linear-gradient(0deg, rgba(16,185,129,0.10), rgba(16,185,129,0.10)), #ffffff; border: 1px solid var(--card-border); box-shadow: 0 12px 30px rgba(16,185,129,.12); }
    .kpi-card.kpi-green::before{ content:""; position:absolute; inset:0 0 auto 0; height:6px; border-radius:18px 18px 0 0; background: linear-gradient(90deg, var(--brand), #34d399); }
    .kpi-title{ font-size:.95rem; color: var(--muted); font-weight:800; letter-spacing:.2px; text-transform:uppercase; }
    .kpi-card.kpi-green .kpi-title{ color:#065f46; }
    .kpi-value{ font-size: 2.2rem; font-weight: 800; line-height:1; color:#0b1324; letter-spacing:.3px; }
    .kpi-card.kpi-green .kpi-value{ color:#052e28; }
    .badge-soft{ background: var(--chip); color: var(--brand-600); border: 1px solid rgba(16,185,129,.25); font-weight: 800; letter-spacing:.2px; }
    .kpi-card.kpi-green .badge-soft{ background: rgba(16,185,129,.13); color: var(--brand-600); border-color: rgba(16,185,129,.35); }
    .muted{ color: var(--muted); font-weight:700; }
    .kpi-card.kpi-green .muted{ color:#065f46; opacity:.85; }

    .chart-card{ background: #fff; border: 1px solid var(--card-border); border-radius: 16px; padding: 16px; box-shadow: 0 6px 20px rgba(2,8,23,.06); }
    .tabla-personalizada th, .tabla-personalizada td{ vertical-align: middle; }
    .turquoise-bg-table{ background: var(--chip) !important; }
    .form-label{ font-weight:700; color:#0b1324; }
    .form-control{ background:#fff; color:#0b1324; border:1px solid var(--card-border); border-radius:12px; }
    .form-control:focus{ border-color: var(--brand); box-shadow: 0 0 0 .25rem rgba(16,185,129,.15); }
    .form-control[disabled]{ background: #f3f4f6; color:#6b7280; }

    .progress{ height:10px; background:#eef2f7; border-radius:999px; }
    .progress-bar{ background: var(--brand); }
    .btn-primary{ font-weight:800; letter-spacing:.2px; }
    .card-title{ margin:0; font-weight:800; color:#0b1324; }
    @media (max-width: 768px){ .kpi-value{ font-size: 1.9rem; } }
  </style>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-wrapper">
    <div class="content-card">

      <!-- ====== Filtros (fondo verde) ====== -->
      <div class="filters-wrap">
        <div class="row g-3 align-items-end">
          <div class="col-md-3">
            <label for="TextBox1" class="form-label">No. Carpeta (Expediente)</label>
            <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
          </div>
          <div class="col-md-3">
            <label for="TextBox2" class="form-label">Reporte (Siniestro)</label>
            <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
          </div>
          <div class="col-md-3">
            <label for="TextBox3" class="form-label">Placa</label>
            <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
          </div>
          <div class="col-md-2">
            <label for="TextBox4" class="form-label">VIN (Serie)</label>
            <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
          </div>
          <div class="col-md-1 d-grid">
            <asp:Button ID="ButtonSearch" runat="server" Text="Buscar" CssClass="btn btn-primary btn-lg" OnClick="ButtonSearch_Click" />
          </div>
        </div>
      </div>

      <!-- ====== Resultados de búsqueda (si hay >1) ====== -->
      <asp:Panel ID="pnlResultados" runat="server" CssClass="results-card" Visible="false">
        <div class="section-title"><i class="bi bi-search"></i> Resultados</div>
        <asp:Label ID="lblResultados" runat="server" CssClass="mb-2 d-block"></asp:Label>
        <asp:Repeater ID="rptResultados" runat="server">
          <HeaderTemplate>
            <div class="list-group">
          </HeaderTemplate>
          <ItemTemplate>
            <div class="results-item">
              <div class="results-meta">
                <div><strong>Expediente:</strong> <%# Eval("Expediente") %></div>
                <div><strong>Reporte:</strong> <%# Eval("SiniestroGen") %></div>
                <div><strong>Placas:</strong> <%# Eval("Placas") %></div>
              </div>
              <div class="results-link">
                <a href='<%# "Hoja.aspx?id=" & Eval("Id") %>' title="Abrir detalle">Abrir</a>
              </div>
            </div>
          </ItemTemplate>
          <FooterTemplate>
            </div>
          </FooterTemplate>
        </asp:Repeater>
      </asp:Panel>

      <!-- ====== KPI cards (tu dashboard existente, no depende de la búsqueda) ====== -->
      <div class="divider"></div>
      <div class="row g-3">
        <div class="col-md-3">
          <div class="kpi-card kpi-green">
            <div class="d-flex justify-content-between align-items-center">
              <span class="kpi-title">Unidades en Piso</span>
              <span class="badge badge-soft rounded-pill px-2">PISO</span>
            </div>
            <div class="kpi-value mt-2" id="kpiPiso">0</div>
            <div class="muted">Total</div>
          </div>
        </div>

        <div class="col-md-3">
          <div class="kpi-card kpi-green">
            <div class="d-flex justify-content-between align-items-center">
              <span class="kpi-title">Unidades en Tránsito</span>
              <span class="badge badge-soft rounded-pill px-2">TRÁNSITO</span>
            </div>
            <div class="kpi-value mt-2" id="kpiTransito">0</div>
            <div class="muted">Total</div>
          </div>
        </div>

        <div class="col-md-3">
          <div class="kpi-card kpi-green">
            <div class="d-flex justify-content-between align-items-center">
              <span class="kpi-title">Refacciones</span>
              <span class="badge badge-soft rounded-pill px-2">REFACCIONES</span>
            </div>
            <div class="kpi-value mt-2" id="kpiRefa">0</div>
            <div class="muted">Total</div>
          </div>
        </div>

        <div class="col-md-3">
          <div class="kpi-card kpi-green">
            <div class="d-flex justify-content-between align-items-center">
              <span class="kpi-title">Total Unidades</span>
              <span class="badge badge-soft rounded-pill px-2">PISO + TRÁNSITO</span>
            </div>
            <div class="kpi-value mt-2" id="kpiTotal">0</div>
            <div class="muted">Suma (Piso + Tránsito)</div>
          </div>
        </div>
      </div>

      <!-- ====== Gráficas ====== -->
      <div class="section-title mt-4"><i class="bi bi-speedometer"></i> Dashboard</div>
      <div class="row g-4">
        <div class="col-lg-7">
          <div class="chart-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <h5 class="card-title">Tránsito por antigüedad</h5>
              <small class="muted">10, 15, 20 y 30+ días</small>
            </div>
            <canvas id="chartTransito" style="max-height:340px"></canvas>
          </div>
        </div>
        <div class="col-lg-5">
          <div class="chart-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <h5 class="card-title">Piso por categoría</h5>
            </div>
            <canvas id="chartPiso" style="max-height:340px"></canvas>
          </div>
        </div>
      </div>

      <!-- ====== Barras de progreso y relación ====== -->
      <div class="row g-4 mt-1">
        <div class="col-lg-7">
          <div class="chart-card">
            <h5 class="card-title mb-3">Avance de Refacciones</h5>

            <div class="mb-2 d-flex justify-content-between">
              <span class="muted">Tránsito con 100% refacciones</span>
              <strong><span id="lblTrans100">0</span></strong>
            </div>
            <div class="progress mb-3"><div id="barTrans100" class="progress-bar" role="progressbar" style="width:0%"></div></div>

            <div class="mb-2 d-flex justify-content-between">
              <span class="muted">Piso con 100% refacciones</span>
              <strong><span id="lblPiso100">0</span></strong>
            </div>
            <div class="progress"><div id="barPiso100" class="progress-bar" role="progressbar" style="width:0%"></div></div>

            <small class="d-block mt-2 muted">Calculado contra los totales mostrados.</small>
          </div>
        </div>

        <div class="col-lg-5">
          <div class="chart-card">
            <h5 class="card-title mb-3">Relación Tránsito / Piso</h5>

            <div class="mb-1 d-flex justify-content-between"><span class="muted">Tránsito</span><strong id="relTrans">0</strong></div>
            <div class="progress mb-3"><div id="barRelTrans" class="progress-bar" style="width:0%"></div></div>

            <div class="mb-1 d-flex justify-content-between"><span class="muted">Piso</span><strong id="relPiso">0</strong></div>
            <div class="progress mb-3"><div id="barRelPiso" class="progress-bar" style="width:0%"></div></div>

            <div class="mb-1 d-flex justify-content-between"><span class="muted">Total</span><strong id="relTotal">0</strong></div>
            <div class="progress"><div id="barRelTotal" class="progress-bar" style="width:0%"></div></div>
          </div>
        </div>
      </div>

      <!-- ====== Tablas (IDs intactos) ====== -->
      <div class="section-title"><i class="bi bi-building-check"></i> Unidades en Piso</div>
      <table class="table tabla-personalizada turquoise-bg-table mb-4">
        <tbody>
          <tr>
            <th class="w-75">Unidades en Piso</th>
            <td class="w-25"><asp:TextBox ID="TextBox10" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox></td>
          </tr>
        </tbody>
      </table>

      <table class="table tabla-personalizada mb-5">
        <thead class="table-light"><tr><th>Categoría</th><th>Unidades</th></tr></thead>
        <tbody>
          <tr><td>UNIDADES SIN TERMINAR CON FECHA DE ENTREGA VENCIDA</td><td><asp:TextBox ID="TextBox11" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>UNIDADES CON MAS DE 30 DIAS SIN TERMINAR</td><td><asp:TextBox ID="TextBox12" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>UNIDADES EN PISO CON 100% DE REFACCIONES</td><td><asp:TextBox ID="TextBox13" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
        </tbody>
      </table>

      <div class="section-title"><i class="bi bi-tools"></i> Refacciones</div>
      <table class="table tabla-personalizada turquoise-bg-table mb-4">
        <tbody>
          <tr>
            <th class="w-75">Refacciones</th>
            <td class="w-25"><asp:TextBox ID="TextBox19" runat="server" CssClass="form-control" Enabled="false" Text="0"></asp:TextBox></td>
          </tr>
        </tbody>
      </table>

      <div class="section-title"><i class="bi bi-truck"></i> Unidades en Tránsito</div>
      <table class="table tabla-personalizada turquoise-bg-table mb-4">
        <tbody>
          <tr>
            <th class="w-75">Unidades en Tránsito</th>
            <td class="w-25"><asp:TextBox ID="TextBox14" runat="server" CssClass="form-control" Enabled="false" Text="0"></asp:TextBox></td>
          </tr>
        </tbody>
      </table>

      <table class="table tabla-personalizada mb-5">
        <thead class="table-light"><tr><th>Categoría</th><th>Unidades</th></tr></thead>
        <tbody>
          <tr><td>UNIDADES CON MAS DE 10 DIAS EN TRANSITO</td><td><asp:TextBox ID="TextBox15" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>UNIDADES CON MAS DE 15 DIAS EN TRANSITO</td><td><asp:TextBox ID="TextBox16" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>UNIDADES CON MAS DE 20 DIAS EN TRANSITO</td><td><asp:TextBox ID="TextBox17" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>UNIDADES CON MAS DE 30 DIAS EN TRANSITO</td><td><asp:TextBox ID="TextBox18" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>UNIDADES EN TRANSITO CON EL 100% DE REFACCIONES</td><td><asp:TextBox ID="TextBox20" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
        </tbody>
      </table>

      <div class="section-title"><i class="bi bi-diagram-3"></i> Relación Tránsito / Piso</div>
      <table class="table tabla-personalizada mb-0">
        <thead class="table-light"><tr><th>Relación</th><th>Unidades</th></tr></thead>
        <tbody>
          <tr><td>TRÁNSITO</td><td><asp:TextBox ID="TextBox25" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>PISO</td><td><asp:TextBox ID="TextBox26" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
          <tr><td>TOTAL</td><td><asp:TextBox ID="TextBox27" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td></tr>
        </tbody>
      </table>
    </div>
  </div>

  <!-- ====== Lógica simple para las gráficas con los TextBox actuales ====== -->
  <script type="text/javascript">
      function gv(clientId) {
          var el = document.getElementById(clientId);
          if (!el) return 0;
          var v = (el.value || el.textContent || "").toString().replace(/[, ]+/g, "").trim();
          var n = parseFloat(v);
          return isNaN(n) ? 0 : n;
      }

      document.addEventListener('DOMContentLoaded', function () {
          const ids = {
              pisoTotal: '<%= TextBox10.ClientID %>',
          pisoVencidas: '<%= TextBox11.ClientID %>',
          pisoMas30: '<%= TextBox12.ClientID %>',
          piso100: '<%= TextBox13.ClientID %>',
          transTotal: '<%= TextBox14.ClientID %>',
        trans10: '<%= TextBox15.ClientID %>',
        trans15:  '<%= TextBox16.ClientID %>',
        trans20:  '<%= TextBox17.ClientID %>',
        trans30:  '<%= TextBox18.ClientID %>',
        refaccionesTotal: '<%= TextBox19.ClientID %>',
        trans100: '<%= TextBox20.ClientID %>',
        relTrans: '<%= TextBox25.ClientID %>',
        relPiso:  '<%= TextBox26.ClientID %>',
        relTotal: '<%= TextBox27.ClientID %>'
        };

        const v = {
            pisoTotal: gv(ids.pisoTotal),
            pisoVencidas: gv(ids.pisoVencidas),
            pisoMas30: gv(ids.pisoMas30),
            piso100: gv(ids.piso100),
            transTotal: gv(ids.transTotal),
            trans10: gv(ids.trans10),
            trans15: gv(ids.trans15),
            trans20: gv(ids.trans20),
            trans30: gv(ids.trans30),
            refaTotal: gv(ids.refaccionesTotal),
            trans100: gv(ids.trans100),
            relTrans: gv(ids.relTrans),
            relPiso: gv(ids.relPiso),
            relTotal: gv(ids.relTotal)
        };

        // KPIs
        document.getElementById('kpiPiso').textContent = v.pisoTotal;
        document.getElementById('kpiTransito').textContent = v.transTotal;
        document.getElementById('kpiRefa').textContent = v.refaTotal;
        document.getElementById('kpiTotal').textContent = (v.pisoTotal + v.transTotal);

        // Progresos
        document.getElementById('lblTrans100').textContent = v.trans100;
        document.getElementById('lblPiso100').textContent = v.piso100;
        var pctTrans100 = v.transTotal > 0 ? (v.trans100 / v.transTotal) * 100 : 0;
        var pctPiso100 = v.pisoTotal > 0 ? (v.piso100 / v.pisoTotal) * 100 : 0;
        document.getElementById('barTrans100').style.width = pctTrans100.toFixed(0) + '%';
        document.getElementById('barPiso100').style.width = pctPiso100.toFixed(0) + '%';

        // Relación
        var sumRel = v.relTrans + v.relPiso;
        var pRelTrans = sumRel > 0 ? (v.relTrans / sumRel) * 100 : 0;
        var pRelPiso = sumRel > 0 ? (v.relPiso / sumRel) * 100 : 0;
        var pRelTotal = v.relTotal > 0 ? 100 : 0;
        document.getElementById('barRelTrans') && (document.getElementById('barRelTrans').style.width = pRelTrans.toFixed(0) + '%');
        document.getElementById('barRelPiso') && (document.getElementById('barRelPiso').style.width = pRelPiso.toFixed(0) + '%');
        document.getElementById('barRelTotal') && (document.getElementById('barRelTotal').style.width = pRelTotal.toFixed(0) + '%');

        // Charts
        const gridColor = '#e5e7eb', tickColor = '#4b5563';
        const ctxT = document.getElementById('chartTransito');
        if (ctxT) {
            new Chart(ctxT, {
                type: 'bar',
                data: {
                    labels: ['+10 días', '+15 días', '+20 días', '+30 días', '100% refacciones'],
                    datasets: [{ label: 'Unidades', data: [v.trans10, v.trans15, v.trans20, v.trans30, v.trans100], borderWidth: 0 }]
                },
                options: {
                    responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } },
                    scales: {
                        x: { grid: { display: false }, ticks: { color: tickColor } },
                        y: { grid: { color: gridColor }, ticks: { color: tickColor }, beginAtZero: true }
                    }
                }
            });
        }
        const ctxP = document.getElementById('chartPiso');
        if (ctxP) {
            new Chart(ctxP, {
                type: 'pie',
                data: { labels: ['Vencidas', '+30 días', '100% refacciones'], datasets: [{ data: [v.pisoVencidas, v.pisoMas30, v.piso100] }] },
                options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom', labels: { color: tickColor } } } }
            });
        }
    });
  </script>
</asp:Content>
