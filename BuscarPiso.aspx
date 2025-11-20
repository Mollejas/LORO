

<%@ Page Title="Búsqueda (Piso)"
    Language="vb"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="false"
    CodeBehind="BuscarPiso.aspx.vb"
    Inherits="DAYTONAMIO.BuscarPiso" %>



<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    /* ============================================================
       SCOPE LOCAL: todo bajo #transitoScope (no pisa el Site Master)
       ============================================================ */
  /* ============================================================
   SCOPE LOCAL: todo bajo #transitoScope (no pisa el Site Master)
   ============================================================ */
/* ============================================================
   SCOPE LOCAL: todo bajo #transitoScope (no pisa el Site Master)
   ============================================================ */
/* ============================================================
   SCOPE LOCAL: todo bajo #transitoScope (no pisa el Site Master)
   ============================================================ */
#transitoScope {
  --brand: #10b981;
  --brand-600: #059669;
  --brand-700: #047857;
  --brand-soft: #d1fae5;
  --brand-lighter: #ecfdf5;
  --brand-glow: rgba(16, 185, 129, 0.15);
  --text: #0f172a;
  --text-secondary: #475569;
  --muted: #6b7280;
  --border: #e5e7eb;
  --row: #f9fafb;
  --bg: #ffffff;
  --shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
  --shadow-md: 0 8px 24px rgba(0, 0, 0, 0.08);
  --shadow-lg: 0 12px 32px rgba(0, 0, 0, 0.1);
  --sel-bg: #e8f5e9;
  --sel-bb: #c8e6c9;
  --sel-text: #1b5e20;
}

/* Layout con gradiente de fondo */
#transitoScope {
  color: var(--text);
  background: linear-gradient(135deg, #f0fdf4 0%, #f9fafb 50%, #ffffff 100%);
  min-height: 100vh;
  padding: 1.5rem 0;
}

#transitoScope.page {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1rem;
  animation: fadeIn 0.5s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Título principal mejorado */
#transitoScope .title {
  font-size: 1.75rem;
  font-weight: 800;
  margin: 0 0 1.25rem;
  color: var(--brand-700);
  display: flex;
  align-items: center;
  gap: 0.75rem;
  letter-spacing: -0.02em;
}

#transitoScope .title::before {
  content: '';
  width: 6px;
  height: 36px;
  background: linear-gradient(180deg, var(--brand) 0%, var(--brand-600) 100%);
  border-radius: 999px;
  box-shadow: 0 0 12px var(--brand-glow);
}

/* Card oculto - sección de filtros removida */
#transitoScope .card {
  display: none;
}

/* Filtros ocultos - sección removida */

/* Toolbar oculto - botones de filtros removidos */
#transitoScope .toolbar {
  display: none;
}

/* Botones modernos con gradientes y efectos */
#transitoScope .btn-transito {
  padding: 0.65rem 1.5rem;
  border: none;
  background: linear-gradient(135deg, var(--brand) 0%, var(--brand-600) 100%);
  color: #fff;
  font-weight: 700;
  border-radius: 10px;
  cursor: pointer;
  font-size: 0.9rem;
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

#transitoScope .btn-transito::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  transition: left 0.5s;
}

#transitoScope .btn-transito:hover::before {
  left: 100%;
}

#transitoScope .btn-transito:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(16, 185, 129, 0.4);
}

#transitoScope .btn-transito:active {
  transform: translateY(0);
}

#transitoScope .btn-ghost-transito {
  padding: 0.65rem 1.5rem;
  border: 2px solid var(--border);
  background: #fff;
  color: var(--text);
  font-weight: 700;
  border-radius: 10px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

#transitoScope .btn-ghost-transito:hover {
  background: var(--row);
  border-color: var(--muted);
  transform: translateY(-2px);
  box-shadow: var(--shadow);
}

#transitoScope .btn-ghost-transito:active {
  transform: translateY(0);
}

/* Grid card mejorado - ahora es el elemento principal */
#transitoScope .grid-card {
  margin-top: 0;
  background: #fff;
  border: 1px solid var(--border);
  border-radius: 16px;
  box-shadow: var(--shadow-md);
  overflow: hidden;
  animation: slideUp 0.6s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

#transitoScope .grid-head {
  padding: 1rem 1.25rem;
  border-bottom: 2px solid var(--brand-lighter);
  background: linear-gradient(135deg, #ffffff 0%, #f9fafb 100%);
  display: flex;
  gap: 1rem;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}

#transitoScope .stats {
  font-size: 0.9rem;
  color: var(--text-secondary);
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

#transitoScope .stats::before {
  content: '📊';
  font-size: 1.1rem;
}

/* Tabla mejorada con efectos modernos */
#transitoScope .table-wrap {
  overflow: auto;
  max-height: calc(100vh - 250px);
  position: relative;
}

#transitoScope table.gv {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  font-size: 0.75rem;
  border: 0;
  table-layout: fixed;
}

/* Headers sticky mejorados con anchos fijos */
#transitoScope .gv thead th {
  position: sticky;
  top: 0;
  z-index: 10;
  background: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
  color: var(--text);
  text-align: left;
  padding: 0.6rem 0.5rem;
  border-bottom: 2px solid var(--brand-soft);
  font-weight: 800;
  white-space: nowrap;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  width: auto;
  min-width: 80px;
}

/* Anchos específicos para cada columna */
#transitoScope .gv thead th:nth-child(1) { width: 40px; min-width: 40px; }  /* # */
#transitoScope .gv thead th:nth-child(2) { width: 90px; min-width: 90px; }  /* EXPEDIENTE */
#transitoScope .gv thead th:nth-child(3) { width: 90px; min-width: 90px; }  /* SINIESTRO */
#transitoScope .gv thead th:nth-child(4) { width: 80px; min-width: 80px; }  /* DÍAS TRÁNSITO */
#transitoScope .gv thead th:nth-child(5) { width: 100px; min-width: 100px; } /* MARCA */
#transitoScope .gv thead th:nth-child(6) { width: 100px; min-width: 100px; } /* SUBMARCA */
#transitoScope .gv thead th:nth-child(7) { width: 70px; min-width: 70px; }  /* MODELO */
#transitoScope .gv thead th:nth-child(8) { width: 80px; min-width: 80px; }  /* COLOR */
#transitoScope .gv thead th:nth-child(9) { width: 90px; min-width: 90px; }  /* PLACAS */
#transitoScope .gv thead th:nth-child(10) { width: 120px; min-width: 120px; } /* ESTATUS DE PROCESOS */
#transitoScope .gv thead th:nth-child(11) { width: 100px; min-width: 100px; } /* TOTAL / RECIBIDAS */
#transitoScope .gv thead th:nth-child(12) { width: 100px; min-width: 100px; } /* CATEGORÍA */
#transitoScope .gv thead th:nth-child(13) { width: 90px; min-width: 90px; }  /* VER DETALLE */

#transitoScope .gv thead th:hover {
  background: var(--brand-lighter);
  color: var(--brand-700);
}

/* Links de sort mejorados */
#transitoScope .gv thead th a {
  color: var(--text);
  text-decoration: none;
  font-weight: 800;
  display: flex;
  align-items: center;
  gap: 0.35rem;
  transition: color 0.3s ease;
}

#transitoScope .gv thead th a:hover {
  color: var(--brand-600);
}

#transitoScope .gv thead th {
  cursor: pointer;
  user-select: none;
}

/* Filas con mejor diseño - reducidas a la mitad */
#transitoScope .gv tbody td {
  padding: 0.4rem 0.5rem;
  border-bottom: 1px solid #f1f3f5;
  vertical-align: middle;
  background: #fff;
  transition: all 0.2s ease;
  font-size: 0.75rem;
  color: var(--text-secondary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Anchos específicos para celdas del body (igual que headers) */
#transitoScope .gv tbody td:nth-child(1) { width: 40px; max-width: 40px; }
#transitoScope .gv tbody td:nth-child(2) { width: 90px; max-width: 90px; }
#transitoScope .gv tbody td:nth-child(3) { width: 90px; max-width: 90px; }
#transitoScope .gv tbody td:nth-child(4) { width: 80px; max-width: 80px; }
#transitoScope .gv tbody td:nth-child(5) { width: 100px; max-width: 100px; }
#transitoScope .gv tbody td:nth-child(6) { width: 100px; max-width: 100px; }
#transitoScope .gv tbody td:nth-child(7) { width: 70px; max-width: 70px; }
#transitoScope .gv tbody td:nth-child(8) { width: 80px; max-width: 80px; }
#transitoScope .gv tbody td:nth-child(9) { width: 90px; max-width: 90px; }
#transitoScope .gv tbody td:nth-child(10) { width: 120px; max-width: 120px; }
#transitoScope .gv tbody td:nth-child(11) { width: 100px; max-width: 100px; }
#transitoScope .gv tbody td:nth-child(12) { width: 100px; max-width: 100px; }
#transitoScope .gv tbody td:nth-child(13) { width: 90px; max-width: 90px; }

#transitoScope .gv tbody tr:nth-child(even) td {
  background: #fcfcfd;
}

#transitoScope .gv tbody tr:hover td {
  background: linear-gradient(90deg, #f0f9ff 0%, #f6f9ff 100%);
  transform: scale(1.002);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

/* Selección de fila mejorada */
#transitoScope .gv tbody tr.selected td {
  background: linear-gradient(90deg, var(--sel-bg) 0%, #d4f4dd 100%) !important;
  color: var(--sel-text);
  border-bottom-color: var(--sel-bb);
  font-weight: 600;
  box-shadow: inset 4px 0 0 var(--brand);
}

#transitoScope .gv tbody tr.selected td:first-child {
  border-left: 4px solid var(--brand);
}

/* Chips de estatus mejorados - más compactos */
#transitoScope .status-chip {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.65rem;
  border-radius: 999px;
  border: 1px solid #bbf7d0;
  background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%);
  color: #065f46;
  font-size: 0.7rem;
  font-weight: 800;
  letter-spacing: 0.03em;
  text-transform: uppercase;
  box-shadow: 0 2px 6px rgba(16, 185, 129, 0.15);
  transition: all 0.3s ease;
}

#transitoScope .status-chip:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.25);
}

/* Links de detalle mejorados - más compactos */
#transitoScope .gv a {
  color: var(--brand-600);
  text-decoration: none;
  font-weight: 700;
  padding: 0.3rem 0.6rem;
  border-radius: 6px;
  background: var(--brand-lighter);
  border: 1px solid var(--brand-soft);
  display: inline-block;
  transition: all 0.3s ease;
  font-size: 0.7rem;
}

#transitoScope .gv a:hover {
  background: var(--brand);
  color: white;
  transform: translateX(3px);
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
}

/* Mensaje vacío */
#transitoScope .empty {
  padding: 3rem 1.25rem;
  color: var(--muted);
  display: block;
  text-align: center;
  font-size: 1rem;
  font-weight: 500;
}

#transitoScope .empty::before {
  content: '🔍';
  display: block;
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

/* Cursor pointer en filas */
#transitoScope .gv tbody tr {
  cursor: pointer;
  -webkit-tap-highlight-color: transparent;
}

/* Scrollbar personalizado */
#transitoScope .table-wrap::-webkit-scrollbar {
  width: 10px;
  height: 10px;
}

#transitoScope .table-wrap::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 10px;
}

#transitoScope .table-wrap::-webkit-scrollbar-thumb {
  background: linear-gradient(180deg, var(--brand) 0%, var(--brand-600) 100%);
  border-radius: 10px;
  border: 2px solid #f1f5f9;
}

#transitoScope .table-wrap::-webkit-scrollbar-thumb:hover {
  background: var(--brand-700);
}

/* Responsivo mejorado */
@media (max-width: 1200px) {
  #transitoScope .table-wrap {
    overflow-x: auto;
  }
  #transitoScope table.gv {
    min-width: 1100px;
  }
}

@media (max-width: 768px) {
  #transitoScope .title {
    font-size: 1.4rem;
    flex-direction: column;
    align-items: flex-start;
  }

  #transitoScope .card-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  #transitoScope .toolbar {
    flex-direction: column;
    width: 100%;
  }

  #transitoScope .btn-transito,
  #transitoScope .btn-ghost-transito {
    width: 100%;
  }

  #transitoScope .grid-head {
    flex-direction: column;
    align-items: flex-start;
  }
}

/* Animación de carga para la tabla */
@keyframes shimmer {
  0% {
    background-position: -1000px 0;
  }
  100% {
    background-position: 1000px 0;
  }
}

/* Efecto ripple en botones */
#transitoScope .btn-transito,
#transitoScope .btn-ghost-transito {
  position: relative;
  overflow: hidden;
}

#transitoScope .btn-transito::after,
#transitoScope .btn-ghost-transito::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.5);
  transform: translate(-50%, -50%);
  transition: width 0.6s, height 0.6s;
}

#transitoScope .btn-transito:active::after,
#transitoScope .btn-ghost-transito:active::after {
  width: 300px;
  height: 300px;
}
  </style>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">
  <div id="transitoScope" class="page">
    <div class="title">Búsqueda de Admisiones (solo <span class="status-chip">PISO</span>)</div>

    <!-- ScriptManager local -->
    <asp:ScriptManager ID="sm" runat="server" EnablePartialRendering="true" />

    <!-- ===== Filtros ===== -->
    <div class="card">
      <div class="card-header">
        <div class="card-title">Filtros</div>
        <div class="muted">Escribe y filtra automáticamente</div>
      </div>
      <div class="card-body">
        <asp:Panel runat="server" DefaultButton="btnBuscar">
          <div class="filters">
            <div class="fld">
              <label for="txtCarpeta">No. Carpeta</label>
              <asp:TextBox ID="txtCarpeta" runat="server" CssClass="tb" ClientIDMode="Static"
                           AutoCompleteType="Disabled" oninput="debouncedFilter();" />
            </div>
            <div class="fld">
              <label for="txtPlaca">Placa</label>
              <asp:TextBox ID="txtPlaca" runat="server" CssClass="tb" ClientIDMode="Static"
                           AutoCompleteType="Disabled" oninput="debouncedFilter();" />
            </div>
            <div class="fld">
              <label for="txtSiniestro">Siniestro</label>
              <asp:TextBox ID="txtSiniestro" runat="server" CssClass="tb" ClientIDMode="Static"
                           AutoCompleteType="Disabled" oninput="debouncedFilter();" />
            </div>
            <div class="fld">
              <label for="txtBuscar">Búsqueda general</label>
              <asp:TextBox ID="txtBuscar" runat="server" CssClass="tb" ClientIDMode="Static"
                           AutoCompleteType="Disabled" placeholder="marca, submarca, color, modelo, placas, etc."
                           oninput="debouncedFilter();" />
            </div>
          </div>

          <div class="toolbar">
            <asp:Button ID="btnLimpiar"  runat="server" CssClass="btn-ghost-transito" Text="Limpiar" OnClick="btnLimpiar_Click" />
            <asp:Button ID="btnRecargar" runat="server" CssClass="btn-ghost-transito" Text="Recargar desde BD" OnClick="btnRecargar_Click" />
            <asp:Button ID="btnBuscar"   runat="server" CssClass="btn-transito"       Text="Filtrar" OnClick="btnBuscar_Click" />
          </div>
        </asp:Panel>
      </div>
    </div>

    <!-- ===== Grid ===== -->
    <div class="grid-card">
      <div class="grid-head">
        <span class="stats">Resultados <asp:Label ID="lblCount" runat="server" Text="" /></span>
        <asp:Label ID="lblMsg" runat="server" CssClass="empty" Visible="false" />
      </div>

      <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
          <div class="table-wrap">
            <asp:GridView ID="gvAdmisiones" runat="server"
                          AutoGenerateColumns="False"
                          CssClass="gv"
                          GridLines="None" BorderStyle="None" BorderWidth="0"
                          AllowSorting="True"
                          OnSorting="gvAdmisiones_Sorting"
                          OnRowDataBound="gvAdmisiones_RowDataBound">
              <Columns>
               
                <asp:TemplateField HeaderText="#" SortExpression="">
                  <ItemStyle Width="1%" />
                  <ItemTemplate>
                    <%# Container.DisplayIndex + 1 %>
                  </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Expediente" HeaderText="EXPEDIENTE" SortExpression="Expediente" />
                <asp:BoundField DataField="SiniestroGen" HeaderText="SINIESTRO" SortExpression="SiniestroGen" />
                <asp:BoundField DataField="DiasTransito" HeaderText="DÍAS PISO" SortExpression="DiasTransito" />
                <asp:BoundField DataField="Marca" HeaderText="MARCA" SortExpression="Marca" />
                <asp:BoundField DataField="Tipo" HeaderText="SUBMARCA" SortExpression="Tipo" />
                <asp:BoundField DataField="Modelo" HeaderText="MODELO" SortExpression="Modelo" />
                <asp:BoundField DataField="Color" HeaderText="COLOR" SortExpression="Color" />
                <asp:BoundField DataField="Placas" HeaderText="PLACAS" SortExpression="Placas" />
                <asp:BoundField DataField="EstatusProceso" HeaderText="ESTATUS DE PROCESOS" SortExpression="EstatusProceso" />
                <asp:BoundField DataField="PiezasInfo" HeaderText="TOTAL / RECIBIDAS" SortExpression="PiezasInfo" />
                <asp:BoundField DataField="Categoria" HeaderText="CATEGORÍA" SortExpression="Categoria" />

                <asp:HyperLinkField HeaderText="VER DETALLE" Text="Abrir"
                    DataNavigateUrlFields="Id"
                    DataNavigateUrlFormatString="Hoja.aspx?id={0}" />
              </Columns>
            </asp:GridView>
          </div>
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
  </div>

  <!-- JS de la página -->
  <script type="text/javascript">
      // Debounce de filtros
      let __filterTimer;
      function debouncedFilter() {
          clearTimeout(__filterTimer);
          __filterTimer = setTimeout(function () {
              __doPostBack('<%= btnBuscar.UniqueID %>', '');
          }, 250);
      }

      // Mantener foco tras el postback parcial + selección fila
      var prm = Sys.WebForms.PageRequestManager.getInstance();
      let lastFocusId = null;

      document.addEventListener('focusin', function (e) {
          if (e.target && e.target.id) lastFocusId = e.target.id;
      });

      function restoreFocus() {
          if (!lastFocusId) return;
          var el = document.getElementById(lastFocusId);
          if (el && typeof el.focus === 'function') {
              el.focus();
              try {
                  if (typeof el.selectionStart === 'number') {
                      el.selectionStart = el.value.length;
                      el.selectionEnd = el.value.length;
                  }
              } catch (err) { /* ignore */ }
          }
      }

      function bindRowClicks() {
          var grid = document.getElementById('<%= gvAdmisiones.ClientID %>');
          if (!grid) return;
          var tbody = grid.getElementsByTagName('tbody')[0];
          if (!tbody) return;
          var rows = tbody.getElementsByTagName('tr');

          for (var i = 0; i < rows.length; i++) {
              var tr = rows[i];
              tr.classList.remove('selected');
              var newTr = tr.cloneNode(true);
              tr.parentNode.replaceChild(newTr, tr);
          }
          rows = tbody.getElementsByTagName('tr');

          for (var j = 0; j < rows.length; j++) {
              (function (tr) {
                  tr.addEventListener('click', function (e) {
                      if (e.target && (e.target.tagName === 'A' || (e.target.closest && e.target.closest('a')))) return;
                      for (var k = 0; k < rows.length; k++) rows[k].classList.remove('selected');
                      tr.classList.add('selected');
                  });
              })(rows[j]);
          }
      }

      document.addEventListener('DOMContentLoaded', function () { bindRowClicks(); });
      prm.add_endRequest(function () { bindRowClicks(); restoreFocus(); });
  </script>

    <script>
        // ----- SUBIR FOTOS (modal #fotosModal) -----
        let __fmRefId = null, __fmDesc = "", __fmFiles = [];
        const fm = {
            input: () => document.getElementById('fmInput'),
            thumbs: () => document.getElementById('fmThumbs'),
            refId: () => document.getElementById('fmRefId'),
            desc: () => document.getElementById('fmDesc'),
            btn: () => document.getElementById('fmUploadBtn'),
            progWrap: () => document.getElementById('fmProgWrap'),
            prog: () => document.getElementById('fmProg'),
            msg: () => document.getElementById('fmMsg')
        };

        // Abrir modal desde botón de fila
        document.addEventListener('click', function (e) {
            const btn = e.target.closest('.btn-fotos');
            if (!btn) return;
            __fmRefId = btn.getAttribute('data-ref-id');
            __fmDesc = btn.getAttribute('data-descripcion') || '';
            fm.refId().textContent = __fmRefId || '—';
            fm.desc().textContent = __fmDesc || '—';
            __fmFiles = [];
            fm.input().value = '';
            fm.thumbs().innerHTML = '';
            fm.btn().disabled = true;
            fm.progWrap().classList.add('d-none');
            fm.msg().classList.add('d-none');
        });

        // Previews
        fm.input().addEventListener('change', function () {
            __fmFiles = Array.from(this.files || []).filter(f => f.type.startsWith('image/'));
            fm.thumbs().innerHTML = '';
            __fmFiles.forEach(f => {
                const r = new FileReader();
                r.onload = e => {
                    const img = document.createElement('img');
                    img.src = e.target.result; img.className = 'thumb';
                    fm.thumbs().appendChild(img);
                };
                r.readAsDataURL(f);
            });
            fm.btn().disabled = (__fmFiles.length < 5);
        });

        // Subir
        fm.btn().addEventListener('click', async function () {
            const expediente = document.getElementById('hfExpediente')?.value || '';
            if (!expediente) { alert('Expediente vacío.'); return; }
            if (!__fmRefId) { alert('Fila no válida.'); return; }
            if (__fmFiles.length < 5) { alert('Selecciona al menos 5 fotos.'); return; }
            fm.progWrap().classList.remove('d-none');
            fm.msg().classList.add('d-none');
            fm.prog().style.width = '10%'; fm.prog().textContent = '10%';

            const fd = new FormData();
            fd.append('refId', __fmRefId);
            fd.append('expediente', expediente);
            fd.append('descripcion', __fmDesc);
            __fmFiles.forEach((f, i) => fd.append('file' + i, f, f.name));

            try {
                const r = await fetch('<%= ResolveUrl("~/UploadDiagFotos.ashx") %>', { method: 'POST', body: fd });
          const t = await r.text();
          let j = null; try { j = JSON.parse(t); } catch { throw new Error(t); }
          if (!j.ok) throw new Error(j.msg || 'Fallo al guardar');
          fm.prog().style.width = '100%'; fm.prog().textContent = '100%';
          fm.msg().classList.remove('d-none');
      } catch (err) {
          alert('Error subiendo fotos: ' + err.message);
      }
  });
    </script>

    <script>
        // ----- GALERÍA (modal #galeriaModal) -----
        (function () {
            const galModal = new bootstrap.Modal(document.getElementById('galeriaModal'));
            const $title = document.getElementById('galTitle');
            const $thumbs = document.getElementById('galThumbs');
            const $big = document.getElementById('galBig');
            const $info = document.getElementById('galInfo');
            const $prev = document.getElementById('galPrev');
            const $next = document.getElementById('galNext');
            const $btnZip = document.getElementById('btnZip');
            const $selAll = document.getElementById('btnSelAll');
            const $selNone = document.getElementById('btnSelNone');

            var __folder = "", __prefix = "", __files = [], __idx = 0, _scale = 1;

            function bust(u) {
                if (!u) return '';
                if (u.indexOf('data:') === 0) return u;
                var url = new URL(u, window.location.origin);
                url.searchParams.set('v', Date.now().toString());
                return url.pathname + url.search;
            }
            function fullUrl(name) {
                var path = __folder.replace(/^~\//, '/').replace(/\\/g, '/');
                if (path.charAt(0) !== '/') path = '/' + path;
                if (path.charAt(path.length - 1) !== '/') path += '/';
                return bust(path + name);
            }

            function renderThumbs() {
                $thumbs.innerHTML = '';
                __files.forEach(function (name, i) {
                    var row = document.createElement('div');
                    row.className = 'gal-item';
                    row.setAttribute('data-i', String(i));
                    row.innerHTML = '<input type="checkbox" class="form-check-input gal-ch" data-name="' + name + '"><img src="' + fullUrl(name) + '" alt="">';
                    $thumbs.appendChild(row);
                });
                $info.textContent = __files.length + ' archivo(s) – prefijo: ' + __prefix;
                highlight(0);
            }

            function highlight(i) {
                $thumbs.querySelectorAll('.gal-item').forEach(function (el) { el.classList.remove('selected'); });
                var el = $thumbs.querySelector('.gal-item[data-i="' + i + '"]');
                if (el) el.classList.add('selected');
            }

            function showAt(i) {
                if (__files.length === 0) { $big.removeAttribute('src'); return; }
                if (i < 0) i = __files.length - 1;
                if (i >= __files.length) i = 0;
                __idx = i;
                _scale = 1;
                $big.style.transform = 'scale(1)';
                $big.style.cursor = 'zoom-in';
                $big.src = fullUrl(__files[__idx]);
                highlight(__idx);
            }

            $prev.addEventListener('click', function () { showAt(__idx - 1); });
            $next.addEventListener('click', function () { showAt(__idx + 1); });

            $big.addEventListener('wheel', function (e) {
                e.preventDefault();
                var d = (e.deltaY < 0) ? 0.1 : -0.1;
                _scale = Math.min(5, Math.max(1, _scale + d));
                $big.style.transform = 'scale(' + _scale + ')';
                $big.style.cursor = (_scale > 1) ? 'zoom-out' : 'zoom-in';
            }, { passive: false });

            $big.addEventListener('dblclick', function () {
                _scale = (_scale > 1) ? 1 : 2;
                $big.style.transform = 'scale(' + _scale + ')';
                $big.style.cursor = (_scale > 1) ? 'zoom-out' : 'zoom-in';
            });

            $thumbs.addEventListener('click', function (e) {
                if (e.target.classList.contains('gal-ch')) return;
                var item = e.target.closest('.gal-item');
                if (item) {
                    var i = parseInt(item.getAttribute('data-i') || '0', 10);
                    showAt(i);
                }
            });

            $selAll.addEventListener('click', function () {
                $thumbs.querySelectorAll('.gal-ch').forEach(function (cb) { cb.checked = true; });
            });
            $selNone.addEventListener('click', function () {
                $thumbs.querySelectorAll('.gal-ch').forEach(function (cb) { cb.checked = false; });
            });

            $btnZip.addEventListener('click', function () {
                var checks = Array.from($thumbs.querySelectorAll('.gal-ch:checked'));
                if (checks.length === 0) { alert('Selecciona al menos una imagen.'); return; }
                var names = checks.map(function (cb) { return cb.getAttribute('data-name'); }).join('|');

                var form = document.createElement('form');
                form.method = 'POST';
                form.action = '<%= ResolveUrl("~/DownloadDiagFotosZip.ashx") %>';

                var hidNames = document.createElement('input');
                hidNames.type = 'hidden'; hidNames.name = 'names'; hidNames.value = names; form.appendChild(hidNames);

                var expediente = (document.getElementById('hfExpediente') && document.getElementById('hfExpediente').value) || '';
                var hidId = document.createElement('input');
                hidId.type = 'hidden'; hidId.name = 'expediente'; hidId.value = expediente; form.appendChild(hidId);

                var hidFolder = document.createElement('input');
                hidFolder.type = 'hidden'; hidFolder.name = 'folder'; hidFolder.value = __folder; form.appendChild(hidFolder);

                document.body.appendChild(form);
                form.submit();
                setTimeout(function () { document.body.removeChild(form); }, 2000);
            });

            // Expuesta para el RegisterStartupScript del servidor
            window.__openGaleriaDiag = function (title, virtualFolder, prefix, filesCsv) {
                $title.textContent = title || 'Galería';
                __folder = virtualFolder || '';
                __prefix = prefix || '';
                __files = (filesCsv || '').split('|').filter(function (s) { return !!s; });
                renderThumbs();
                showAt(0);
                galModal.show();
            };
        })();
    </script>


</asp:Content>
