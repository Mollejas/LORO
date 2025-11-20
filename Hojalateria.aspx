<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="Hojalateria.aspx.vb" Inherits="DAYTONAMIO.Hojalateria" MaintainScrollPositionOnPostBack="true" %><!DOCTYPE html>
<html lang="es">
<head runat="server">
  <meta charset="utf-8" />
  <title>Diagnóstico – Hojalatería</title>
  <meta name="viewport" content="width=device-width, initial-scale=1" />

  <!-- Bootstrap + Icons -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

  <style>
    :root{
      --brand-950:#081a39; --brand-900:#0a1f44; --brand-800:#163560; --brand-700:#1e4976;
      --brand-600:#2563eb; --brand-500:#3b82f6; --brand-400:#60a5fa;
      --neutral-900:#0f172a; --neutral-800:#1e293b; --neutral-700:#334155; --neutral-600:#475569;
      --neutral-300:#cbd5e1; --neutral-200:#e2e8f0; --neutral-100:#f1f5f9; --neutral-050:#f8fafc;
      --radius-lg:16px; --shadow-md:0 6px 18px rgba(0,0,0,.08); --shadow-lg:0 20px 40px rgba(2,6,23,.18);
    }

    body{
      background:
        radial-gradient(1200px 600px at -10% -10%, rgba(37,99,235,.08), transparent 60%),
        radial-gradient(1000px 500px at 110% 0%, rgba(96,165,250,.08), transparent 60%),
        linear-gradient(180deg, var(--neutral-050), #fff 45%, var(--neutral-050) 100%);
      color: var(--neutral-800);
    }
    .shell{ max-width: 1400px; margin: 0 auto; }

    .hero{
      position: relative; border-radius: var(--radius-lg); padding: 18px 20px;
      background: linear-gradient(135deg, var(--brand-900), var(--brand-700) 55%, var(--brand-600));
      color:#fff; box-shadow: var(--shadow-lg); overflow:hidden;
    }
    .hero h1{ margin:0; font-size:1.35rem; font-weight:800; letter-spacing:.2px; }
    .hero-sub{opacity:.9; font-size:.95rem}
    .chipbar{ display:flex; flex-wrap:wrap; gap:8px; margin-top:10px; }
    .chip{
      display:inline-flex; align-items:center; gap:8px; padding:6px 10px; border-radius:999px;
      border:1px solid rgba(255,255,255,.25);
      background:linear-gradient(180deg, rgba(255,255,255,.16), rgba(255,255,255,.06));
      font-weight:600; font-size:.85rem;
    }

    .panel{
      border:1px solid var(--neutral-200); border-radius: 16px; background:#fff;
      box-shadow: var(--shadow-md); overflow:hidden;
    }
    .panel-head{
      display:flex; align-items:center; gap:10px; padding:14px 16px;
      background:linear-gradient(180deg, #fff, var(--neutral-100)); border-bottom:1px solid var(--neutral-200);
    }
    .panel-head .ttl{ margin:0; font-weight:800; color:var(--brand-900); letter-spacing:.3px; }
    .panel-body{ padding:16px }

    .label-strong{ font-weight:700; color:var(--neutral-700); }
    .input-group .input-group-text{
      background:linear-gradient(180deg, #fff, var(--neutral-100));
      border-color: var(--neutral-200);
    }
    .form-control{ border-color: var(--neutral-200); border-radius: 10px; }
    .form-control:focus{ border-color: var(--brand-500); box-shadow: 0 0 0 .2rem rgba(37,99,235,.12); }
    .qty{ max-width: 110px; }

    .btn-brand{
      border:none; background:linear-gradient(135deg, var(--brand-700), var(--brand-600));
      color:#fff; font-weight:700; letter-spacing:.2px; border-radius: 12px;
      box-shadow: 0 8px 18px rgba(37,99,235,.25);
    }
    .btn-ghost{ background:#fff; color:var(--brand-700); border:2px solid var(--brand-600); font-weight:700; border-radius:12px; }

    .table-wrap{
      border:1px solid var(--neutral-200); border-radius: 12px; overflow:auto;
      max-height: 48vh; background:#fff;
    }
    .table-sticky thead th{
      position: sticky; top: 0; z-index: 2;
      background:linear-gradient(180deg, #fff, var(--neutral-100));
      border-bottom:2px solid var(--neutral-200) !important;
    }
    .table.table-sm>:not(caption)>*>*{ padding:.55rem .6rem }

    .ck-head{ white-space:nowrap; }
    .btn-icon{
      width:34px; height:34px; padding:0; display:inline-grid; place-items:center;
      border-radius:10px; border:1px solid #e5e7eb; background:#fff;
    }
    .btn-icon:hover{ background:#f8fafc; }

    /* Galería */
    .gal-wrap{ display:flex; flex-direction:column; gap:14px; min-height: 60vh }
    .gal-stage{
      width:100%; max-width:1180px; height:56vh; margin:0 auto; position:relative;
      border:1px solid var(--neutral-200); border-radius: 14px; background:#fff;
      display:grid; place-items:center; overflow:hidden;
    }
    .gal-big{ width:100%; height:100%; object-fit:contain; user-select:none; transition:transform .15s ease; cursor:zoom-in; }
    .gal-arrows{ position:absolute; inset:0; display:flex; justify-content:space-between; align-items:center; pointer-events:none; padding: 0 8px; }
    .gal-arrows .btn{ pointer-events:auto; opacity:.95; border-radius:999px; }
    .gal-thumbs{ display:flex; flex-wrap:wrap; gap:10px; justify-content:center; max-height: 28vh; overflow:auto; }
    .gal-item{ position:relative; width:88px; height:88px; border:1px solid var(--neutral-200); border-radius:10px; background:#fff; display:grid; place-items:center; }
    .gal-item img{ width:82px; height:82px; object-fit:cover; border-radius:8px; cursor:pointer; }
    .gal-item .gal-ch{ position:absolute; top:6px; left:6px; width:16px; height:16px; margin:0; cursor:pointer; }
    .gal-item.selected img{ outline:2px solid var(--brand-500); }

    .modal-header{ background: linear-gradient(135deg, var(--brand-700), var(--brand-600)); color:#fff; border-bottom:none; }
    .btn-close{ filter: invert(1); opacity:.9 }
    .btn-close:hover{ opacity:1 }

    #autPanel .badge{ font-weight:700; }
  </style>
  <style>
    /* ====== EXPANDIR EN MODAL/IFRAME ====== */
    html, body{ height:100%; }

    body.embed .shell,
    body.embed .container,
    body.embed .container-xxl{
      max-width: 100% !important;
      width: 100% !important;
      padding-left: 8px;
      padding-right: 8px;
    }

    body.embed .row.grids{
      height: calc(92vh - 320px);
      min-height: 420px;
    }

    body.embed .row.g-3 > [class*="col-"]{
      display: flex;
      flex-direction: column;
    }

    body.embed .panel.h-100{
      height: 100%;
      display: flex;
      flex-direction: column;
      min-height: 0;
    }

    body.embed .panel-body{
      flex: 1 1 auto;
      display: flex;
      flex-direction: column;
      min-height: 0;
    }

    body.embed .table-wrap{
      height: calc(100% - 150px);
      max-height: none;
      overflow: auto;
    }

    body.embed .table-sticky thead th{
      position: sticky; top: 0; z-index: 2;
    }

    @media (max-width: 992px){
      body.embed .row.g-3{ height: auto; }
      body.embed .panel.h-100{ min-height: 420px; }
      body.embed .table-wrap{ height: calc(100% - 180px); }
    }

    .hero{ padding: 12px 14px; }
    .hero.mb-2{ margin-bottom: .5rem !important; }
    .row.grids{ margin-top: 0 !important; }

    body.embed .row.grids{
      height: calc(92vh - 110px);
      min-height: 420px;
    }

    .shell.container-xxl{ padding-top: .5rem !important; padding-bottom: .75rem !important; }

    .panel-body{ display:flex; flex-direction:column; min-height:0; }
    .table-wrap{ flex: 1 1 auto; max-height:none; overflow:auto; }

    /* ====== Thumbs del fotosModal ====== */
    #fmThumbs{
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      max-height: 38vh;
      overflow: auto;
    }
    .fm-thumb{
      width: 96px;
      height: 96px;
      border: 1px solid #e5e7eb;
      border-radius: 10px;
      background: #fff;
      display: grid;
      place-items: center;
      box-shadow: 0 1px 2px rgba(0,0,0,.04);
    }
    .fm-thumb img{
      width: 100%;
      height: 100%;
      object-fit: cover;
      border-radius: 10px;
    }

    /* Panel compacto de Datos del expediente */
    .panel.compact .panel-head{ padding: 8px 10px; }
    .panel.compact .panel-head .ttl{ font-size: 1rem; margin: 0; }
    .panel.compact .panel-body{ padding: 10px 12px; }
    .panel.compact .row.g-3{ --bs-gutter-y: .35rem; --bs-gutter-x: .5rem; }
    .panel.compact .input-group .input-group-text{ padding: .25rem .45rem; }
    .panel.compact .form-control{ padding: .25rem .5rem; height: 2rem; font-size: .9rem; }

    .hero{ padding: 10px 12px; }
    .shell.container-xxl{ padding-top: .5rem !important; padding-bottom: .75rem !important; }
  </style>

</head>
<body>
<form id="form1" runat="server">
  <asp:ScriptManager ID="sm1" runat="server" />

  <div class="shell container-xxl py-2">
    <!-- Hero -->
    <div class="hero mb-3">
      <div class="d-flex align-items-center gap-2">
        <i class="bi bi-hammer fs-4"></i>
        <h1>Diagnóstico – Hojalatería</h1>
      </div>
      <div class="hero-sub">Captura y control de refacciones por sustitución y reparación</div>
      <div class="chipbar">
        <span class="chip"><i class="bi bi-folder2-open"></i> <strong class="me-1">Expediente:</strong> <span id="chipExp">—</span></span>
        <span class="chip"><i class="bi bi-shield-check"></i> <strong class="me-1">Siniestro:</strong> <span id="chipSin">—</span></span>
        <span class="chip"><i class="bi bi-car-front"></i> <strong class="me-1">Vehículo:</strong> <span id="chipVeh">—</span></span>
      </div>
    </div>

    <!-- Datos expediente -->
    <div class="panel mb-2 compact">
      <div class="panel-head">
        <i class="bi bi-info-circle text-primary fs-5"></i>
        <h2 class="ttl">Datos del expediente</h2>
      </div>
      <div class="panel-body">
        <asp:HiddenField ID="hfId" runat="server" />
        <asp:HiddenField ID="hfExpediente" runat="server" />
        <asp:HiddenField ID="hfCarpetaRel" runat="server" ClientIDMode="Static" />

        <div class="row g-3">
          <div class="col-md-4">
            <label class="label-strong mb-1">Expediente</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-folder2-open"></i></span>
              <asp:TextBox ID="txtExpediente" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
          </div>
          <div class="col-md-4">
            <label class="label-strong mb-1">Siniestro</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-shield-check"></i></span>
              <asp:TextBox ID="txtSiniestro" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
          </div>
          <div class="col-md-4">
            <label class="label-strong mb-1">Vehículo</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-car-front"></i></span>
              <asp:TextBox ID="txtVehiculo" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Paneles principales (Sustitución / Reparación) -->
    <div class="row g-3">
      <!-- Sustitución -->
      <div class="col-12 col-lg-6">
        <div class="panel h-100">
          <div class="panel-head">
            <i class="bi bi-arrow-repeat text-primary fs-5"></i>
            <h3 class="ttl">Sustitución</h3>
          </div>
          <div class="panel-body">
            <div class="row g-2 align-items-end">
              <div class="col-4 col-sm-3">
                <label class="label-strong mb-1">Cantidad</label>
                <asp:TextBox ID="txtCantSust" runat="server" CssClass="form-control qty" MaxLength="4" />
              </div>
              <div class="col-8 col-sm-7">
                <label class="label-strong mb-1">Descripción</label>
                <asp:TextBox ID="txtDescSust" runat="server" CssClass="form-control" />
              </div>
              <div class="col-12 col-sm-2 d-grid">
                <asp:Button ID="btnAddSust" runat="server" CssClass="btn btn-brand" Text="Guardar"
                            UseSubmitBehavior="false" OnClick="btnAddSust_Click" />
              </div>
            </div>

            <div class="table-wrap mt-3">
              <asp:GridView ID="gvSust" runat="server" AutoGenerateColumns="False"
                CssClass="table table-sm table-striped table-hover table-sticky"
                DataKeyNames="Id,Descripcion"
                OnRowDataBound="gv_RowDataBound"
                OnRowCommand="gvSust_RowCommand">
                <Columns>
                  <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" />
                  <asp:BoundField DataField="Cantidad" HeaderText="Cant." />
                  <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                  <asp:TemplateField HeaderText="Autorización" HeaderStyle-CssClass="ck-head">
                    <ItemTemplate>
                      <asp:LinkButton ID="btnToggleAuto" runat="server"
                        CommandName="TOGGLE_AUTO"
                        CommandArgument='<%# Eval("Id") %>'
                        CssClass="btn-icon" ToolTip="Cambiar autorización"
                        CausesValidation="false" UseSubmitBehavior="false">
                        <i id="icoAuto" runat="server" class="bi"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="Estatus">
                    <ItemTemplate>
                      <asp:LinkButton ID="btnChecar" runat="server"
                        CommandName="TOGGLE_CHECAR" CommandArgument='<%# Eval("Id") %>'
                        ToolTip="Cambiar estatus" CssClass="btn-icon"
                        CausesValidation="false" UseSubmitBehavior="false">
                        <i class="bi bi-power"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="Aut">
                    <ItemTemplate>
                      <asp:LinkButton ID="btnAldo" runat="server"
                        CommandName="TOGGLE_ALDO" CommandArgument='<%# Eval("Id") %>'
                        ToolTip="Marcar" CssClass="btn-icon"
                        CausesValidation="false" UseSubmitBehavior="false">
                        <i class="bi bi-person-check"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:BoundField DataField="AldoDateTime" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" HtmlEncode="False" />
                  <asp:TemplateField HeaderText="Fotos">
                    <ItemTemplate>
                      <button type="button" class="btn btn-outline-secondary btn-sm btn-fotos"
                              data-ref-id='<%# Eval("Id") %>' data-descripcion='<%# Eval("Descripcion") %>'
                              data-bs-toggle="modal" data-bs-target="#fotosModal">
                        <i class="bi bi-cloud-upload"></i> Fotos
                      </button>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="Ver">
                    <ItemTemplate>
                      <asp:Button runat="server" ID="btnVerSust" Text="Ver"
                        CommandName="VER_FOTOS" CommandArgument="<%# Container.DataItemIndex %>"
                        CssClass="btn btn-outline-primary btn-sm" CausesValidation="false" UseSubmitBehavior="false" />
                    </ItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
          </div>
        </div>
      </div>

      <!-- Reparación -->
      <div class="col-12 col-lg-6">
        <div class="panel h-100">
          <div class="panel-head">
            <i class="bi bi-tools text-primary fs-5"></i>
            <h3 class="ttl">Reparación</h3>
          </div>
          <div class="panel-body">
            <div class="row g-2 align-items-end">
              <div class="col-4 col-sm-3">
                <label class="label-strong mb-1">Cantidad</label>
                <asp:TextBox ID="txtCantRep" runat="server" CssClass="form-control qty" MaxLength="4" />
              </div>
              <div class="col-8 col-sm-7">
                <label class="label-strong mb-1">Descripción</label>
                <asp:TextBox ID="txtDescRep" runat="server" CssClass="form-control" />
              </div>
              <div class="col-12 col-sm-2 d-grid">
                <asp:Button ID="btnAddRep" runat="server" CssClass="btn btn-brand" Text="Guardar"
                            UseSubmitBehavior="false" OnClick="btnAddRep_Click" />
              </div>
            </div>

            <div class="table-wrap mt-3">
              <asp:GridView ID="gvRep" runat="server" AutoGenerateColumns="False"
                CssClass="table table-sm table-striped table-hover table-sticky"
                DataKeyNames="Id,Descripcion"
                OnRowDataBound="gv_RowDataBound"
                OnRowCommand="gvRep_RowCommand">
                <Columns>
                  <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" />
                  <asp:BoundField DataField="Cantidad" HeaderText="Cant." />
                  <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                  <asp:TemplateField HeaderText="Autorización" HeaderStyle-CssClass="ck-head">
                    <ItemTemplate>
                      <asp:LinkButton ID="btnToggleAuto" runat="server"
                        CommandName="TOGGLE_AUTO"
                        CommandArgument='<%# Eval("Id") %>'
                        CssClass="btn-icon" ToolTip="Cambiar autorización"
                        CausesValidation="false" UseSubmitBehavior="false">
                        <i id="icoAuto" runat="server" class="bi"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="Estatus">
                    <ItemTemplate>
                      <asp:LinkButton ID="btnChecar" runat="server"
                        CommandName="TOGGLE_CHECAR" CommandArgument='<%# Eval("Id") %>'
                        ToolTip="Cambiar estatus" CssClass="btn-icon"
                        CausesValidation="false" UseSubmitBehavior="false">
                        <i class="bi bi-power"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="Aut">
                    <ItemTemplate>
                      <asp:LinkButton ID="btnAldo" runat="server"
                        CommandName="TOGGLE_ALDO" CommandArgument='<%# Eval("Id") %>'
                        ToolTip="Marcar Aldo" CssClass="btn-icon"
                        CausesValidation="false" UseSubmitBehavior="false">
                        <i class="bi bi-person-check"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:BoundField DataField="AldoDateTime" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" HtmlEncode="False" />
                  <asp:TemplateField HeaderText="Fotos">
                    <ItemTemplate>
                      <button type="button" class="btn btn-outline-secondary btn-sm btn-fotos"
                              data-ref-id='<%# Eval("Id") %>' data-descripcion='<%# Eval("Descripcion") %>'
                              data-bs-toggle="modal" data-bs-target="#fotosModal">
                        <i class="bi bi-cloud-upload"></i> Fotos
                      </button>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="Ver">
                    <ItemTemplate>
                      <asp:Button runat="server" ID="btnVerRep" Text="Ver"
                        CommandName="VER_FOTOS" CommandArgument="<%# Container.DataItemIndex %>"
                        CssClass="btn btn-outline-primary btn-sm" CausesValidation="false" UseSubmitBehavior="false" />
                    </ItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- === Vistos buenos (autorizaciones) === -->
    <div id="autPanel" class="panel mt-3">
      <div class="panel-head">
        <i class="bi bi-person-badge text-primary fs-5"></i>
        <h3 class="ttl">Vistos Buenos (Autorizaciones)</h3>
      </div>
      <div class="panel-body">
        <div class="row g-3">
          <!-- Aut 1 -->
          <div class="col-md-4">
            <div class="card border-0 shadow-sm">
              <div class="card-body">
                <div class="mb-2 fw-bold">Autorización 1</div>
                <asp:DropDownList ID="ddlAutMec1" runat="server" CssClass="form-select"></asp:DropDownList>
                <asp:TextBox ID="txtPassMec1" runat="server" CssClass="form-control mt-2" TextMode="Password" placeholder="Contraseña" />
                <div class="d-grid mt-2">
                  <asp:Button ID="btnAutorizarMec1" runat="server" CssClass="btn btn-brand" Text="Autorizar" UseSubmitBehavior="false" />
                </div>
                <div class="mt-2">
                  <asp:Literal ID="litAutMec1" runat="server" />
                </div>
              </div>
            </div>
          </div>

          <!-- Aut 2 -->
          <div class="col-md-4">
            <div class="card border-0 shadow-sm">
              <div class="card-body">
                <div class="mb-2 fw-bold">Autorización 2</div>
                <asp:DropDownList ID="ddlAutMec2" runat="server" CssClass="form-select"></asp:DropDownList>
                <asp:TextBox ID="txtPassMec2" runat="server" CssClass="form-control mt-2" TextMode="Password" placeholder="Contraseña" />
                <div class="d-grid mt-2">
                  <asp:Button ID="btnAutorizarMec2" runat="server" CssClass="btn btn-brand" Text="Autorizar" UseSubmitBehavior="false" />
                </div>
                <div class="mt-2">
                  <asp:Literal ID="litAutMec2" runat="server" />
                </div>
              </div>
            </div>
          </div>

          <!-- Aut 3 -->
          <div class="col-md-4">
            <div class="card border-0 shadow-sm">
              <div class="card-body">
                <div class="mb-2 fw-bold">Autorización 3</div>
                <asp:DropDownList ID="ddlAutMec3" runat="server" CssClass="form-select"></asp:DropDownList>
                <asp:TextBox ID="txtPassMec3" runat="server" CssClass="form-control mt-2" TextMode="Password" placeholder="Contraseña" />
                <div class="d-grid mt-2">
                  <asp:Button ID="btnAutorizarMec3" runat="server" CssClass="btn btn-brand" Text="Autorizar" UseSubmitBehavior="false" />
                </div>
                <div class="mt-2">
                  <asp:Literal ID="litAutMec3" runat="server" />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="small text-muted mt-2">
          Selecciona tu nombre de la lista, escribe tu contraseña y presiona <strong>Autorizar</strong>. Si es correcta, se marcará en Admisiones (<code>autmec1</code>, <code>autmec2</code>, <code>autmec3</code>).
        </div>
      </div>
    </div>

    <asp:Label ID="lblStatus" runat="server" CssClass="d-block mt-3 text-success fw-semibold" />
  </div>
</form>

<!-- ===================== MODAL: SUBIR FOTOS ===================== -->
<div class="modal fade" id="fotosModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-lg modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title"><i class="bi bi-cloud-arrow-up me-2"></i>Fotos de diagnóstico (mínimo 5)</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body">
        <div class="alert alert-info py-2">
          Se guardarán en <strong>\3. FOTOS DIAGNOSTICO HOJALATERIA\</strong> con prefijo de 5 caracteres de la descripción y consecutivo.
        </div>
        <div class="mb-2 small text-muted d-flex flex-wrap gap-4">
          <div><span class="text-muted">Refacción Id:</span> <span id="fmRefId" class="fw-semibold">—</span></div>
          <div><span class="text-muted">Descripción:</span> <span id="fmDesc" class="fw-semibold">—</span></div>
        </div>
        <div class="row g-3">
          <div class="col-12">
            <input id="fmInput" type="file" accept="image/*" capture="environment" class="form-control" multiple />
            <div class="form-text">Selecciona o toma al menos 5 fotos.</div>
          </div>
          <div class="col-12 d-flex flex-wrap gap-2" id="fmThumbs"></div>
          <div class="col-12 d-none" id="fmProgWrap">
            <div class="progress">
              <div id="fmProg" class="progress-bar progress-bar-striped" style="width:0%">0%</div>
            </div>
            <div id="fmMsg" class="small mt-2 text-success d-none">Fotos guardadas.</div>
          </div>
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-ghost" data-bs-dismiss="modal">Cancelar</button>
        <button type="button" id="fmUploadBtn" class="btn btn-brand" disabled><i class="bi bi-cloud-upload"></i> Subir fotos</button>
      </div>
    </div>
  </div>
</div>

<!-- ===================== MODAL: GALERÍA ===================== -->
<div class="modal fade" id="galeriaModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width:96vw">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="galTitle"><i class="bi bi-images me-2"></i>Galería</h5>
        <div class="ms-auto d-flex flex-wrap gap-2">
          <button class="btn btn-sm btn-outline-light" id="btnSelAll" type="button"><i class="bi bi-check2-square me-1"></i>Seleccionar todo</button>
          <button class="btn btn-sm btn-outline-light" id="btnSelNone" type="button"><i class="bi bi-square me-1"></i>Quitar selección</button>
          <button class="btn btn-sm btn-dark" id="btnZip" type="button"><i class="bi bi-file-zip me-1"></i>Descargar ZIP</button>
          <button type="button" class="btn btn-outline-light btn-sm" data-bs-dismiss="modal">Cerrar</button>
        </div>
      </div>
      <div class="modal-body">
        <div class="gal-wrap">
          <div class="gal-stage">
            <img id="galBig" class="gal-big" alt="imagen" />
            <div class="gal-arrows">
              <button class="btn btn-light btn-sm" id="galPrev" type="button"><i class="bi bi-chevron-left"></i></button>
              <button class="btn btn-light btn-sm" id="galNext" type="button"><i class="bi bi-chevron-right"></i></button>
            </div>
          </div>
          <div class="gal-thumbs" id="galThumbs"></div>
        </div>
        <div class="small mt-2 text-muted" id="galInfo"></div>
      </div>
    </div>
  </div>
</div>

<!-- ===== Scripts ===== -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

<script>
  // Chips de cabecera
  function setVal(id, v) { const el = document.getElementById(id); if (!el) return; if ('value' in el) el.value = v || ''; else el.textContent = v || ''; }
  function setChipValues() {
      document.getElementById('chipExp').textContent = document.getElementById('txtExpediente')?.value || '—';
      document.getElementById('chipSin').textContent = document.getElementById('txtSiniestro')?.value || '—';
      document.getElementById('chipVeh').textContent = document.getElementById('txtVehiculo')?.value || '—';
  }
  window.addEventListener('message', function (e) {
      if (e.origin !== window.location.origin) return;
      if (!e.data || e.data.type !== 'EXP_PREFILL') return;
      const d = e.data.payload || {};
      setVal('hfId', d.id);
      setVal('hfExpediente', d.expediente);
      setVal('txtExpediente', d.expediente);
      setVal('txtSiniestro', d.siniestro);
      setVal('txtVehiculo', d.vehiculo);
      if (d.carpeta) setVal('hfCarpetaRel', d.carpeta);
      setChipValues();
  });
  window.addEventListener('load', function () {
      try { window.parent && window.parent.postMessage({ type: 'EXP_REQUEST' }, window.location.origin); } catch (_) { }
      setTimeout(setChipValues, 150);
  });
</script>

<script>
  // Sugerir tamaño al padre (cuando está en iframe/modal)
  function __resizeForModal() {
      try { window.parent && window.parent.postMessage({ type: 'IFR_RESIZE', heightVh: 92 }, window.location.origin); } catch (_) { }
      const h = Math.max(document.documentElement.scrollHeight, document.body.scrollHeight);
      try { window.parent && window.parent.postMessage({ type: 'IFR_CONTENT_HEIGHT', px: h }, window.location.origin); } catch (_) { }
  }
  window.addEventListener('load', __resizeForModal);
  window.addEventListener('resize', function () { clearTimeout(window.__rto); window.__rto = setTimeout(__resizeForModal, 120); });

  (function () {
      var set = function () { document.body.classList.add('embed'); };
      try {
          if (window.self !== window.top) {
              if (document.readyState === 'loading') {
                  document.addEventListener('DOMContentLoaded', set);
              } else { set(); }
          }
      } catch (e) {
          if (document.readyState === 'loading') {
              document.addEventListener('DOMContentLoaded', set);
          } else { set(); }
      }
  })();
</script>

<!-- ===== SUBIR FOTOS (fotosModal) ===== -->
<script>
  let __fmRefId=null, __fmDesc="", __fmFiles=[];
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

  document.addEventListener('click', function (e) {
    const btn = e.target.closest('.btn-fotos');
    if (!btn) return;
    __fmRefId = btn.getAttribute('data-ref-id');
    __fmDesc  = btn.getAttribute('data-descripcion') || '';
    fm.refId().textContent = __fmRefId || '—';
    fm.desc().textContent  = __fmDesc  || '—';
    __fmFiles = [];
    fm.input().value = '';
    fm.thumbs().innerHTML = '';
    fm.btn().disabled = true;
    fm.progWrap().classList.add('d-none');
    fm.msg().classList.add('d-none');
  });

  fm.input().addEventListener('change', function () {
    __fmFiles = Array.from(this.files || []).filter(f => f.type.startsWith('image/'));
    fm.thumbs().innerHTML = '';

    __fmFiles.forEach(f => {
      const r = new FileReader();
      r.onload = e => {
        const wrap = document.createElement('div');
        wrap.className = 'fm-thumb';
        const img = document.createElement('img');
        img.src = e.target.result;
        img.alt = f.name;
        img.loading = 'lazy';
        wrap.appendChild(img);
        fm.thumbs().appendChild(wrap);
      };
      r.readAsDataURL(f);
    });

    fm.btn().disabled = (__fmFiles.length < 5);
  });

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
      let j=null; try{ j=JSON.parse(t); }catch{ throw new Error(t); }
      if(!j.ok) throw new Error(j.msg || 'Fallo al guardar');
      fm.prog().style.width = '100%'; fm.prog().textContent = '100%';
      fm.msg().classList.remove('d-none');
    } catch(err){
      alert('Error subiendo fotos: ' + err.message);
    }
  });
</script>

<!-- ===== GALERÍA (galeriaModal) ===== -->
<script>
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

        window.__openGaleriaDiag = function (title, virtualFolder, prefix, filesCsv) {
            $title.textContent = title || 'Galería';
            __folder = virtualFolder || '';
            __prefix = prefix || '';
            __files = (filesCsv || '').split('|').filter(Boolean);
            renderThumbs();
            showAt(0);
            galModal.show();
        };
    })();
</script>

</body>
</html>
