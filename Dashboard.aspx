<%@ Page Title="Dashboard"
    Language="VB"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="false"
    CodeBehind="Dashboard.aspx.vb"
    Inherits="DAYTONAMIO.Dashboard"
    ClientIDMode="Static" %>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    :root{
      --brand:#0d47a1; --brand-600:#1565c0; --brand-soft:#eaf2ff;
      --text:#0f172a; --muted:#6b7280; --border:#e5e7eb; --bg:#fff;
      --shadow:0 8px 24px rgba(0,0,0,0.08);
      --chip-bg:#eff6ff; --chip-bd:#dbeafe; --chip-tx:#1d4ed8;
    }
    *{box-sizing:border-box}
    html,body{height:100%}
    body{margin:0;font-family:Segoe UI, Roboto, Arial, sans-serif;color:var(--text);background:var(--bg)}
    .page{max-width:1200px;margin:48px auto;padding:0 18px}
    .title{font-size:24px;font-weight:800;margin:0 0 16px;color:var(--brand)}

    .card{background:#fff;border:1px solid var(--border);border-radius:14px;box-shadow:var(--shadow)}
    .card-header{padding:14px 16px;border-bottom:1px solid var(--border);display:flex;align-items:center;justify-content:space-between}
    .card-title{font-size:16px;font-weight:800;color:#111827}
    .card-body{padding:14px 16px}

    .filters{display:grid;grid-template-columns:repeat(12,1fr);gap:12px}
    .fld{grid-column:span 3;display:flex;flex-direction:column;gap:6px}
    @media (max-width: 992px){.fld{grid-column:span 6}}
    @media (max-width: 576px){.fld{grid-column:span 12}}
    .filters label{font-weight:700;font-size:13px;color:#111827}
    .tb{padding:10px 12px;border:1px solid var(--border);border-radius:10px;outline:none;font-size:14px;background:#fff;width:100%}
    .tb:focus{border-color:var(--brand-600);box-shadow:0 0 0 3px var(--brand-soft)}
    .toolbar{display:flex;gap:10px;align-items:center;justify-content:flex-end;margin-top:10px}
    .btn{padding:10px 16px;border:1px solid var(--brand);background:var(--brand);color:#fff;font-weight:700;border-radius:10px;cursor:pointer}
    .btn:hover{filter:brightness(0.96)}
    .btn-ghost{padding:10px 16px;border:1px solid var(--border);background:#fff;color:#111827;font-weight:700;border-radius:10px;cursor:pointer}
    .btn-ghost:hover{background:#f8fafc}

    .grid-card{margin-top:18px;background:#fff;border:1px solid var(--border);border-radius:14px;box-shadow:var(--shadow);overflow:hidden}
    .grid-head{padding:12px 16px;border-bottom:1px solid var(--border);background:#fff;display:flex;gap:12px;align-items:center;justify-content:space-between}
    .stats{font-size:13px;color:var(--muted)}
    .empty{padding:12px 16px;color:var(--muted);display:block}

    /* Tarjetas */
    .cards{display:grid;grid-template-columns:repeat(auto-fill,minmax(280px,1fr));gap:14px;padding:14px}
    .cardx{
      display:block;border:1px solid var(--border);border-radius:12px;overflow:hidden;background:#fff;
      text-decoration:none;color:inherit;box-shadow:var(--shadow);
      transition:transform .06s ease, box-shadow .15s ease, border-color .15s ease;
    }
    .cardx:hover{transform:translateY(-1px);box-shadow:0 10px 28px rgba(0,0,0,.10);border-color:#dbeafe}

    .imgwrap{width:100%;aspect-ratio:4/3;background:#f3f4f6;display:flex;align-items:center;justify-content:center;overflow:hidden}
    .imgwrap a.imglink{display:block;width:100%;height:100%}
    .imgwrap a.imglink img{width:100%;height:100%;object-fit:cover;display:block;cursor:pointer}
    .imgwrap a.imglink:focus{outline:3px solid var(--brand-600);outline-offset:2px}

    .info{padding:12px 12px 10px;display:grid;grid-template-columns:1fr auto;grid-row-gap:8px;align-items:center}

    /* Carpeta: fuente chica y multi-línea para que se vea completa */
    .carpeta{
      font-weight:800;
      font-size:12px;
      line-height:1.25;
      grid-column:1/-1;
      color:#111827;
      white-space:normal;
      overflow-wrap:anywhere;
      word-break:break-word;
    }

    .estatus{font-size:12px; display:flex; align-items:center; gap:10px}
    .chip{display:inline-block;padding:2px 10px;border-radius:999px;border:1px solid var(--chip-bd);background:var(--chip-bg);color:var(--chip-tx);font-size:12px;font-weight:800;letter-spacing:.3px}

    /* ===== Progreso circular (0% por ahora) =====
       Para hacerlo dinámico después, puedes setear --p:XX en línea y el texto interno. */
    .prog{
      --p: 0;                         /* porcentaje (0..100) */
      width:32px; height:32px; border-radius:50%;
      background: conic-gradient(var(--brand) calc(var(--p)*1%), #e5e7eb 0);
      display:grid; place-items:center;
      box-shadow:0 0 0 4px rgba(13,71,161,.07);
    }
    .prog-inner{
      width:70%; height:70%;
      border-radius:50%;
      background:#fff; display:grid; place-items:center;
      font-size:10px; font-weight:800; color:#111827;
      border:1px solid var(--border);
      line-height:1;
    }
  </style>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">
  <div class="page">
    <asp:ScriptManager ID="sm" runat="server" EnablePartialRendering="true" />

    <!-- ===== Filtros ===== -->
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

    <!-- ===== Tarjetas ===== -->
    <div class="grid-card">
      <div class="grid-head">
        <span class="stats">Resultados <asp:Label ID="lblCount" runat="server" Text="" /></span>
        <asp:Label ID="lblMsg" runat="server" CssClass="empty" Visible="false" />
      </div>

      <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>

          <asp:ListView ID="lvAdmisiones" runat="server" DataKeyNames="Id"
                        OnPagePropertiesChanging="lvAdmisiones_PagePropertiesChanging">
            <LayoutTemplate>
              <div class="cards">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
              </div>

              <div class="pager" style="padding:8px 12px;border-top:1px solid var(--border);display:flex;justify-content:center;gap:6px">
                <asp:DataPager ID="dpMain" runat="server" PageSize="24" PagedControlID="lvAdmisiones">
                  <Fields>
                    <asp:NextPreviousPagerField ShowFirstPageButton="true" ShowPreviousPageButton="true"
                                                ShowNextPageButton="false" ShowLastPageButton="false" ButtonType="Button" />
                    <asp:NumericPagerField ButtonType="Button" />
                    <asp:NextPreviousPagerField ShowFirstPageButton="false" ShowPreviousPageButton="false"
                                                ShowNextPageButton="true" ShowLastPageButton="true" ButtonType="Button" />
                  </Fields>
                </asp:DataPager>
              </div>
            </LayoutTemplate>

            <ItemTemplate>
              <div class="cardx">
                <div class="imgwrap">
                  <!-- Solo la imagen es clickeable -->
                  <a class="imglink" href='<%# Eval("Id", "Hoja.aspx?id={0}") %>' title="Ver hoja">
                    <img src='<%# Eval("ImagenUrl") %>' alt="principal.jpg" loading="lazy" />
                  </a>
                </div>
                <div class="info">
                  <!-- Carpeta multi-línea, tamaño pequeño -->
                  <div class="carpeta"><%# Eval("CarpetaRelLimpia") %></div>

                  <!-- Estatus + Progreso circular (0% en todos por ahora) -->
                  <div class="estatus">
                    <span class="chip"><%# Eval("Estatus") %></span>

                   
                    <div class="prog" style="--p:0" aria-label="Avance 0 por ciento" title="Avance 0%">
                      <div class="prog-inner">0%</div>
                    </div>
                  </div>
                </div>
              </div>
            </ItemTemplate>

            <EmptyDataTemplate>
              <div class="empty">Sin registros para mostrar.</div>
            </EmptyDataTemplate>
          </asp:ListView>

        </ContentTemplate>
        <Triggers>
          <asp:AsyncPostBackTrigger ControlID="btnBuscar"  EventName="Click" />
          <asp:AsyncPostBackTrigger ControlID="btnLimpiar" EventName="Click" />
          <asp:AsyncPostBackTrigger ControlID="btnRecargar" EventName="Click" />
        </Triggers>
      </asp:UpdatePanel>
    </div>
  </div>

  <!-- Debounce del filtrado -->
  <script type="text/javascript">
    let __filterTimer;
    function debouncedFilter() {
      clearTimeout(__filterTimer);
      __filterTimer = setTimeout(function () {
        __doPostBack('<%= btnBuscar.UniqueID %>', '');
      }, 250);
      }
  </script>
</asp:Content>
