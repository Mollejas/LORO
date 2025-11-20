

<%@ Page Title="Búsqueda (Entregado)"
    Language="vb"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="false"
    CodeBehind="BuscarEntregado.aspx.vb"
    Inherits="DAYTONAMIO.BuscarEntregado" %>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        :root{
            --brand:#0d47a1; --brand-600:#1565c0; --brand-soft:#eaf2ff;
            --text:#0f172a; --muted:#6b7280; --border:#e5e7eb; --row:#f9fafb; --bg:#ffffff;
            --shadow: 0 8px 24px rgba(0,0,0,0.08);
            --sel-bg:#e8f5e9; --sel-bb:#c8e6c9; --sel-text:#1b5e20;
        }
        *{ box-sizing:border-box }
        html,body{ height:100% }
        body{
            margin:0; padding:0; font-family:Segoe UI, Roboto, Arial, sans-serif;
            color:var(--text); background:var(--bg);
        }

        /* Más separación superior para inputs */
        .page{ max-width:1200px; margin:48px auto 48px; padding:0 18px; }
        .title{ font-size:24px; font-weight:800; margin:0 0 16px; color:var(--brand); }

        /* ===== Card de filtros ===== */
        .card{
            background:#fff; border:1px solid var(--border); border-radius:14px; box-shadow:var(--shadow);
        }
        .card-header{ padding:14px 16px; border-bottom:1px solid var(--border); display:flex; align-items:center; justify-content:space-between; }
        .card-title{ font-size:16px; font-weight:800; color:#111827; }
        .card-body{ padding:14px 16px; }

        .filters{ display:grid; grid-template-columns:repeat(12,1fr); gap:12px; }
        .fld{ grid-column:span 3; display:flex; flex-direction:column; gap:6px; }
        @media (max-width: 992px){ .fld{ grid-column:span 6; } }
        @media (max-width: 576px){ .fld{ grid-column:span 12; } }
        .filters label{ font-weight:700; font-size:13px; color:#111827; }
        .tb{
            padding:10px 12px; border:1px solid var(--border); border-radius:10px; outline:none; font-size:14px; background:#fff; width:100%;
        }
        .tb:focus{ border-color:var(--brand-600); box-shadow:0 0 0 3px var(--brand-soft); }
        .toolbar{ display:flex; gap:10px; align-items:center; justify-content:flex-end; margin-top:10px; }
        .btn{ padding:10px 16px; border:1px solid var(--brand); background:var(--brand); color:#fff; font-weight:700; border-radius:10px; cursor:pointer; }
        .btn:hover{ filter:brightness(0.96); }
        .btn-ghost{ padding:10px 16px; border:1px solid var(--border); background:#fff; color:#111827; font-weight:700; border-radius:10px; cursor:pointer; }
        .btn-ghost:hover{ background:#f8fafc; }

        /* ===== Card del grid ===== */
        .grid-card{ margin-top:18px; background:#fff; border:1px solid var(--border); border-radius:14px; box-shadow:var(--shadow); overflow:hidden; }
        .grid-head{ padding:12px 16px; border-bottom:1px solid var(--border); background:#fff; display:flex; gap:12px; align-items:center; justify-content:space-between; }
        .stats{ font-size:13px; color:var(--muted); }

        .gridwrap{ max-height:65vh; overflow:auto; }

        /* ===== Tabla sin borde negro & pro ===== */
        table.gv{ width:100%; border-collapse:separate; border-spacing:0; font-size:14px; border:0; }
        .gv thead th{
            position:sticky; top:0; z-index:1; background:#fff; color:#111827; text-align:left;
            padding:12px 14px; border-bottom:1px solid var(--border); font-weight:800;
        }
        .gv tbody td{ padding:12px 14px; border-bottom:1px solid #edf0f2; vertical-align:top; background:#fff; }
        .gv tbody tr:nth-child(even) td{ background:#fcfcfd; }
        .gv tbody tr:hover td{ background:#f6f9ff; }

        /* >>> Selección en VERDE solo para la fila clickeada <<< */
        .gv tbody tr.selected td{
            background:var(--sel-bg) !important;
            color:var(--sel-text);
            border-bottom-color:var(--sel-bb);
        }

        /* Sin bordes feos */
        .gridwrap, .gv{ border:0 !important; }
        .chip{
            display:inline-block; padding:2px 10px; border-radius:999px;
            border:1px solid #dbeafe; background:#eff6ff; color:#1d4ed8;
            font-size:12px; font-weight:800; letter-spacing:.3px;
        }
        .empty{ padding:12px 16px; color:var(--muted); display:block; }

        /* Mejor UX en móviles */
        .gv tbody tr{ cursor:pointer; -webkit-tap-highlight-color: transparent; }
    </style>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page">
        <div class="title">Búsqueda de Admisiones (solo <span class="chip">TRANSITO</span>)</div>

        <!-- ScriptManager local (Opción B) -->
        <asp:ScriptManager ID="sm" runat="server" EnablePartialRendering="true" />

        <!-- ===== Filtros (fuera del UpdatePanel para no perder foco) ===== -->
        <div class="card">
            <div class="card-header">
                <div class="card-title">Filtros</div>
                <div class="muted">Escribe y filtrará automáticamente</div>
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
                                         AutoCompleteType="Disabled" placeholder="marca, tipo, color, modelo, placas, etc."
                                         oninput="debouncedFilter();" />
                        </div>
                    </div>

                    <div class="toolbar">
                        <asp:Button ID="btnLimpiar"  runat="server" CssClass="btn-ghost" Text="Limpiar" OnClick="btnLimpiar_Click" />
                        <asp:Button ID="btnRecargar" runat="server" CssClass="btn-ghost" Text="Recargar desde BD" OnClick="btnRecargar_Click" />
                        <asp:Button ID="btnBuscar"   runat="server" CssClass="btn"       Text="Filtrar" OnClick="btnBuscar_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>

        <!-- ===== Grid (dentro del UpdatePanel) ===== -->
        <div class="grid-card">
            <div class="grid-head">
                <span class="stats">Resultados <asp:Label ID="lblCount" runat="server" Text="" /></span>
                <asp:Label ID="lblMsg" runat="server" CssClass="empty" Visible="false" />
            </div>

            <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <div class="gridwrap">
                        <asp:GridView ID="gvAdmisiones" runat="server" AutoGenerateColumns="False"
                                      CssClass="gv" GridLines="None" BorderStyle="None" BorderWidth="0"
                                      AllowPaging="True" PageSize="50" EnableTheming="false"
                                      OnPageIndexChanging="gvAdmisiones_PageIndexChanging"
                                      EmptyDataText="Sin registros para mostrar.">
                            <Columns>
                                <asp:BoundField DataField="Expediente" HeaderText="Expediente" />
                                <asp:BoundField DataField="SiniestroGen" HeaderText="Siniestro" />
                                <asp:BoundField DataField="TipoIngreso" HeaderText="Tipo ingreso" />
                                <asp:TemplateField HeaderText="Estatus">
                                    <ItemTemplate>
                                        <span class="chip"><%# Eval("Estatus") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Vehiculo" HeaderText="Vehículo (Marca/Tipo/Color/Modelo/Placas)" />
                                <asp:HyperLinkField HeaderText="Detalles" Text="Ver hoja"
                                    DataNavigateUrlFields="Id"
                                    DataNavigateUrlFormatString="Hoja.aspx?id={0}" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnBuscar"  EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnLimpiar" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnRecargar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Debounce + foco + selección de fila en verde -->
    <script type="text/javascript">
        let __filterTimer;
        function debouncedFilter() {
            clearTimeout(__filterTimer);
            __filterTimer = setTimeout(function () {
                __doPostBack('<%= btnBuscar.UniqueID %>', '');
            }, 250);
        }

        // Mantener foco tras el postback parcial
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

        // === Selección de fila (verde) ===
        function bindRowClicks() {
            var grid = document.getElementById('<%= gvAdmisiones.ClientID %>');
            if (!grid) return;
            var tbody = grid.getElementsByTagName('tbody')[0];
            if (!tbody) return;
            var rows = tbody.getElementsByTagName('tr');

            // Limpia handlers previos para evitar duplicados
            for (var i = 0; i < rows.length; i++) {
                var tr = rows[i];
                tr.style.cursor = 'pointer';
                // Remueve selección previa si existiera
                tr.classList.remove('selected');

                // Evita doble registro: clona sin eventos y reemplaza (opcional)
                var newTr = tr.cloneNode(true);
                tr.parentNode.replaceChild(newTr, tr);
            }

            // Vuelve a obtener las filas ya clonadas
            rows = tbody.getElementsByTagName('tr');

            for (var j = 0; j < rows.length; j++) {
                (function (tr) {
                    tr.addEventListener('click', function (e) {
                        // Si el click fue sobre un link, no pintar (deja navegar)
                        if (e.target && (e.target.tagName === 'A' || (e.target.closest && e.target.closest('a')))) return;

                        // Quitar selección de todas
                        for (var k = 0; k < rows.length; k++) rows[k].classList.remove('selected');
                        // Marcar seleccionada
                        tr.classList.add('selected');
                    });
                })(rows[j]);
            }
        }

        // Rebind al cargar y después de cada actualización parcial
        document.addEventListener('DOMContentLoaded', function () {
            bindRowClicks();
        });
        prm.add_endRequest(function () {
            bindRowClicks();
            restoreFocus();
        });
    </script>
</asp:Content>
