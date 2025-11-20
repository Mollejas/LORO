<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="Mecanica.aspx.vb" Inherits="DAYTONAMIO.Mecanica" MaintainScrollPositionOnPostBack="true" %><!DOCTYPE html>
<html lang="es">
<head runat="server">
  <meta charset="utf-8" />
  <title>Diagnóstico – Mecánica</title>
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

    /* === Full width === */
    .shell{ width:100%; max-width:100%; margin:0 auto; }

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

    /* ====== Tablas compactas y con layout fijo ====== */
    .table-wrap{
      border:1px solid var(--neutral-200); border-radius: 12px; overflow:auto;
      max-height: 48vh; background:#fff;
    }
    .table.table-sm>:not(caption)>*>*{ padding:.35rem .45rem }
    .table td, .table th{ vertical-align:middle; }
    .table-fixed{ table-layout: fixed; }

    .col-cant{ width:60px; }
    .col-desc{ width: 100%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .col-np{ width:160px; }
    .col-obs{ width:170px; }
    .col-act{ width:44px; text-align:center; }
    .col-act i{ font-size:1rem; opacity:.9; }

    .btn-icon{ --size: 32px; width: var(--size); height: var(--size); padding: 0; display:inline-flex; align-items:center; justify-content:center; border-radius: 8px; }
    .btn-icon i{ pointer-events:none; }

    /* Resaltado sutil si cumple con mín. de fotos */
    .table tr.row-photos-ok{ box-shadow: inset 4px 0 0 #22c55e; }
    .table tr.row-photos-ok > *{
      background: linear-gradient(180deg, #effaf3, #e9f7ef) !important;
      border-top: 1px solid #d9f0e1; border-bottom: 1px solid #d9f0e1; color:#0f3b26;
    }

    .table-sticky thead th{
      position: sticky; top: 0; z-index: 2;
      background:linear-gradient(180deg, #fff, var(--neutral-100));
      border-bottom:2px solid var(--neutral-200) !important;
    }

    /* ====== Cámara (overlay temporal) ====== */
    .cam-ov{
      position:fixed; inset:0; background:rgba(0,0,0,.6); display:flex; align-items:center; justify-content:center;
      z-index: 20000;
    }
    .cam-ov-inner{
      background:#000; border-radius:16px; box-shadow: var(--shadow-lg);
      width:min(96vw, 680px); padding:10px; display:flex; flex-direction:column; gap:10px;
    }
    .cam-ov video{ width:100%; max-height:60vh; border-radius:10px; background:#000; }
    .cam-ov .row-actions{ display:flex; gap:10px; justify-content:center; }
    .fm-thumb{ width:86px; height:86px; border:1px solid var(--neutral-200); border-radius:10px; display:grid; place-items:center; overflow:hidden; }
    .fm-thumb img{ width:100%; height:100%; object-fit:cover; }

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
</head>
<body>
<form id="form1" runat="server">
  <asp:ScriptManager ID="sm1" runat="server" EnablePageMethods="true" />

  <div class="shell container-fluid py-2"><!-- full width -->

    <!-- Hero -->
    <div class="hero mb-3">
      <div class="d-flex align-items-center gap-2">
        <i class="bi bi-wrench-adjustable fs-4"></i>
        <h1>Diagnóstico – Mecánica</h1>
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
              <div class="col-6 col-sm-2">
                <label class="label-strong mb-1">Cantidad</label>
                <asp:TextBox ID="txtCantSust" runat="server" CssClass="form-control qty" MaxLength="4" />
              </div>
              <div class="col-12 col-sm-5">
                <label class="label-strong mb-1">Descripción</label>
                <asp:TextBox ID="txtDescSust" runat="server" CssClass="form-control" />
              </div>
              <div class="col-6 col-sm-3">
                <label class="label-strong mb-1">Num. de parte (opcional)</label>
                <asp:TextBox ID="txtNumParteSust" runat="server" CssClass="form-control" MaxLength="80" />
              </div>
              <div class="col-12 col-sm-2 d-grid">
                <asp:Button ID="btnAddSust" runat="server" CssClass="btn btn-brand" Text="Guardar"
                            UseSubmitBehavior="false" OnClick="btnAddSust_Click" />
              </div>
            </div>

            <div class="table-wrap mt-3">
              <asp:GridView ID="gvSust" runat="server" AutoGenerateColumns="False"
                CssClass="table table-sm table-striped table-hover table-sticky table-fixed"
                DataKeyNames="Id,Descripcion,NumParte,Observ1"
                OnRowCommand="gvSust_RowCommand"
                OnRowDataBound="gvSust_RowDataBound">
                <Columns>
                  <asp:BoundField DataField="Cantidad" HeaderText="Cant.">
                    <ItemStyle CssClass="col-cant" />
                    <HeaderStyle CssClass="col-cant" />
                  </asp:BoundField>
                  <asp:BoundField DataField="Descripcion" HeaderText="Descripción">
                    <ItemStyle CssClass="col-desc" />
                    <HeaderStyle CssClass="col-desc" />
                  </asp:BoundField>
                  <asp:BoundField DataField="NumParte" HeaderText="Num. parte">
                    <ItemStyle CssClass="col-np" />
                    <HeaderStyle CssClass="col-np" />
                  </asp:BoundField>

                  <asp:TemplateField>
                    <HeaderTemplate><i class="bi bi-cloud-upload" title="Subir fotos"></i></HeaderTemplate>
                    <ItemTemplate>
                      <button type="button" class="btn btn-outline-secondary btn-sm btn-icon btn-fotos"
                              title="Subir fotos" aria-label="Subir fotos"
                              data-ref-id='<%# Eval("Id") %>' data-descripcion='<%# Eval("Descripcion") %>'
                              data-bs-toggle="modal" data-bs-target="#fotosModal">
                        <i class="bi bi-cloud-upload"></i>
                      </button>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
                  </asp:TemplateField>

                  <asp:TemplateField>
                    <HeaderTemplate><i class="bi bi-images" title="Ver galería"></i></HeaderTemplate>
                    <ItemTemplate>
                      <asp:LinkButton runat="server" ID="btnVerSust" CommandName="VER_FOTOS"
                        CommandArgument="<%# Container.DataItemIndex %>"
                        CssClass="btn btn-outline-primary btn-sm btn-icon"
                        CausesValidation="false" UseSubmitBehavior="false" ToolTip="Ver galería" aria-label="Ver galería">
                        <i class="bi bi-images"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
                  </asp:TemplateField>

                  <asp:TemplateField>
                    <HeaderTemplate><i class="bi bi-pencil-square" title="Editar descripción"></i></HeaderTemplate>
                    <ItemTemplate>
                      <button type="button" class="btn btn-outline-secondary btn-sm btn-icon btn-edit-desc"
                              title="Editar" aria-label="Editar"
                              data-ref-id='<%# Eval("Id") %>' data-descripcion='<%# Eval("Descripcion") %>'>
                        <i class="bi bi-pencil-square"></i>
                      </button>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
                  </asp:TemplateField>

                  <asp:TemplateField HeaderText="Eliminar" Visible="False">
                    <HeaderTemplate><i class="bi bi-trash" title="Eliminar"></i></HeaderTemplate>
                    <ItemTemplate>
                      <asp:LinkButton ID="btnDelSust" runat="server"
                        CommandName="DELETE_ROW" CommandArgument='<%# Eval("Id") %>'
                        CssClass="btn btn-outline-danger btn-sm btn-icon"
                        OnClientClick='return confirm("¿Eliminar la refacción seleccionada?");'
                        CausesValidation="false" UseSubmitBehavior="false" ToolTip="Eliminar" aria-label="Eliminar">
                        <i class="bi bi-trash"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
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
              <div class="col-6 col-sm-2">
                <label class="label-strong mb-1">Cantidad</label>
                <asp:TextBox ID="txtCantRep" runat="server" CssClass="form-control qty" MaxLength="4" />
              </div>
              <div class="col-12 col-sm-5">
                <label class="label-strong mb-1">Descripción</label>
                <asp:TextBox ID="txtDescRep" runat="server" CssClass="form-control" />
              </div>
              <div class="col-12 col-sm-3">
                <label class="label-strong mb-1">Observaciones (opcional)</label>
                <asp:TextBox ID="txtObsRep" runat="server" CssClass="form-control" MaxLength="250" />
              </div>
              <div class="col-12 col-sm-2 d-grid">
                <asp:Button ID="btnAddRep" runat="server" CssClass="btn btn-brand" Text="Guardar"
                            UseSubmitBehavior="false" OnClick="btnAddRep_Click" />
              </div>
            </div>

            <div class="table-wrap mt-3">
              <asp:GridView ID="gvRep" runat="server" AutoGenerateColumns="False"
                CssClass="table table-sm table-striped table-hover table-sticky table-fixed"
                DataKeyNames="Id,Descripcion,NumParte,Observ1"
                OnRowCommand="gvRep_RowCommand"
                OnRowDataBound="gvRep_RowDataBound">
                <Columns>
                  <asp:BoundField DataField="Cantidad" HeaderText="Cant.">
                    <ItemStyle CssClass="col-cant" /><HeaderStyle CssClass="col-cant" />
                  </asp:BoundField>
                  <asp:BoundField DataField="Descripcion" HeaderText="Descripción">
                    <ItemStyle CssClass="col-desc" /><HeaderStyle CssClass="col-desc" />
                  </asp:BoundField>
                  <asp:BoundField DataField="Observ1" HeaderText="Observaciones">
                    <ItemStyle CssClass="col-obs" /><HeaderStyle CssClass="col-obs" />
                  </asp:BoundField>

                  <asp:TemplateField>
                    <HeaderTemplate><i class="bi bi-cloud-upload" title="Subir fotos"></i></HeaderTemplate>
                    <ItemTemplate>
                      <button type="button" class="btn btn-outline-secondary btn-sm btn-icon btn-fotos"
                              title="Subir fotos" aria-label="Subir fotos"
                              data-ref-id='<%# Eval("Id") %>' data-descripcion='<%# Eval("Descripcion") %>'
                              data-bs-toggle="modal" data-bs-target="#fotosModal">
                        <i class="bi bi-cloud-upload"></i>
                      </button>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
                  </asp:TemplateField>

                  <asp:TemplateField>
                    <HeaderTemplate><i class="bi bi-images" title="Ver galería"></i></HeaderTemplate>
                    <ItemTemplate>
                      <asp:LinkButton runat="server" ID="btnVerRep" CommandName="VER_FOTOS"
                        CommandArgument="<%# Container.DataItemIndex %>"
                        CssClass="btn btn-outline-primary btn-sm btn-icon"
                        CausesValidation="false" UseSubmitBehavior="false" ToolTip="Ver galería" aria-label="Ver galería">
                        <i class="bi bi-images"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
                  </asp:TemplateField>

                  <asp:TemplateField>
                    <HeaderTemplate><i class="bi bi-pencil-square" title="Editar descripción"></i></HeaderTemplate>
                    <ItemTemplate>
                      <button type="button" class="btn btn-outline-secondary btn-sm btn-icon btn-edit-desc"
                              title="Editar" aria-label="Editar"
                              data-ref-id='<%# Eval("Id") %>' data-descripcion='<%# Eval("Descripcion") %>'>
                        <i class="bi bi-pencil-square"></i>
                      </button>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
                  </asp:TemplateField>

                  <asp:TemplateField HeaderText="Eliminar" Visible="False">
                    <HeaderTemplate><i class="bi bi-trash" title="Eliminar"></i></HeaderTemplate>
                    <ItemTemplate>
                      <asp:LinkButton ID="btnDelRep" runat="server"
                        CommandName="DELETE_ROW" CommandArgument='<%# Eval("Id") %>'
                        CssClass="btn btn-outline-danger btn-sm btn-icon"
                        OnClientClick='return confirm("¿Eliminar la reparación seleccionada?");'
                        CausesValidation="false" UseSubmitBehavior="false" ToolTip="Eliminar" aria-label="Eliminar">
                        <i class="bi bi-trash"></i>
                      </asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle CssClass="col-act" /><HeaderStyle CssClass="col-act" />
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Autorizaciones -->
    <div id="autPanel" class="panel mt-3">
      <div class="panel-head">
        <i class="bi bi-person-badge text-primary fs-5"></i>
        <h3 class="ttl">Vistos Buenos (Autorizaciones)</h3>
      </div>
      <div class="panel-body">
        <div class="row g-3">
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
          Selecciona tu nombre de la lista, escribe tu contraseña y presiona <strong>Autorizar</strong>.
          Si es correcta, se marcará en Admisiones y se guardará tu nombre (<code>autmec1nombre</code>, <code>autmec2nombre</code>, <code>autmec3nombre</code>).
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
        <h5 class="modal-title"><i class="bi bi-cloud-arrow-up me-2"></i>Fotos de diagnóstico (mínimo 3)</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body">
        <div class="alert alert-info py-2">
          Se guardarán en <strong>\2. FOTOS DIAGNOSTICO MECANICA\</strong> con prefijo <strong>Id-5car</strong> y consecutivo.
        </div>
        <div class="mb-2 small text-muted d-flex flex-wrap gap-4">
          <div><span class="text-muted">Refacción Id:</span> <span id="fmRefId" class="fw-semibold">—</span></div>
          <div><span class="text-muted">Descripción:</span> <span id="fmDesc" class="fw-semibold">—</span></div>
        </div>
        <div class="row g-3">
          <div class="col-12">
            <div class="d-flex flex-wrap gap-2">
              <input id="fmInput" type="file" accept="image/*" capture="environment" class="form-control" multiple style="max-width:420px" />
              <button type="button" id="fmCamBtn" class="btn btn-outline-secondary">
                <i class="bi bi-camera"></i> Tomar con cámara
              </button>
            </div>
            <div class="form-text">Selecciona o toma al menos 3 fotos.</div>
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
    msg: () => document.getElementById('fmMsg'),
    camBtn: () => document.getElementById('fmCamBtn')
  };

  // ---------- Abrir modal de fotos ----------
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

  // ---------- Previsualización (input nativo) ----------
  fm.input().addEventListener('change', function () {
    __fmFiles = Array.from(this.files || []).filter(f => f.type.startsWith('image/'));
    refreshThumbs();
  });

  // ---------- Subir a handler .ashx ----------
  fm.btn().addEventListener('click', async function () {
    const expediente = document.getElementById('hfExpediente')?.value || '';
    if (!expediente) { alert('Expediente vacío.'); return; }
    if (!__fmRefId) { alert('Fila no válida.'); return; }
    if (__fmFiles.length < 3) { alert('Selecciona o toma al menos 3 fotos.'); return; }
    fm.progWrap().classList.remove('d-none');
    fm.msg().classList.add('d-none');
    fm.prog().style.width = '10%'; fm.prog().textContent = '10%';

    const fd = new FormData();
    fd.append('refId', __fmRefId);            // usar para prefijo {Id}-
    fd.append('expediente', expediente);
    fd.append('descripcion', __fmDesc);       // usar para 5 primeros chars sanitizados
    __fmFiles.forEach((f, i) => fd.append('file' + i, f, f.name));

    try {
      const r = await fetch('<%= ResolveUrl("~/UploadDiagFotos.ashx") %>', { method: 'POST', body: fd });
      const t = await r.text();
      let j=null; try{ j=JSON.parse(t); }catch{ throw new Error(t); }
      if(!j.ok) throw new Error(j.msg || 'Fallo al guardar');
      fm.prog().style.width = '100%'; fm.prog().textContent = '100%';
      fm.msg().classList.remove('d-none');

      // === Marcar la fila en sutil verde sin recargar ===
      PageMethods.HasMinFotos(expediente, parseInt(__fmRefId,10), __fmDesc, 3,
        function(ok){
          const fotosBtn = document.querySelector(`.btn-fotos[data-ref-id="${CSS.escape(__fmRefId)}"]`);
          const row = fotosBtn?.closest('tr');
          if(row){ row.classList.toggle('row-photos-ok', !!ok); }
        },
        function(){ /* no-op */ }
      );

    } catch(err){
      alert('Error subiendo fotos: ' + err.message);
    }
  });

  // ---------- Editar descripción (renombrar en servidor) ----------
  document.addEventListener('click', function(e){
    const btn = e.target.closest('.btn-edit-desc');
    if(!btn) return;

    const expediente = document.getElementById('hfExpediente')?.value || '';
    const id = parseInt(btn.getAttribute('data-ref-id'),10);
    const oldDesc = btn.getAttribute('data-descripcion') || '';

    const newDesc = prompt('Nueva descripción:', oldDesc);
    if(newDesc === null) return;
    const trimmed = (newDesc || '').trim();
    if(!trimmed){ alert('La descripción no puede estar vacía.'); return; }

    btn.disabled = true;
    PageMethods.UpdateDescripcionAndRename(expediente, id, oldDesc, trimmed,
      function(res){
        btn.disabled = false;
        if(!res || res.ok !== true){
          alert((res && res.msg) ? res.msg : 'No se pudo actualizar.');
          return;
        }
        const row = btn.closest('tr');

        // Actualiza celda y data-atributos
        btn.setAttribute('data-descripcion', trimmed);
        const descCell = row?.querySelector('td.col-desc');
        if(descCell) descCell.textContent = trimmed;
        const fotosBtn = row?.querySelector('.btn-fotos');
        if(fotosBtn) fotosBtn.setAttribute('data-descripcion', trimmed);

        // Re-evalúa si hay ≥3 fotos con el nuevo prefijo (Id-5car)
        PageMethods.HasMinFotos(expediente, id, trimmed, 3,
          function(ok){ if(row) row.classList.toggle('row-photos-ok', !!ok); },
          function(){}
        );

        // Mensaje
        try{
          const lbl = document.getElementById('<%= lblStatus.ClientID %>');
          if(lbl){ lbl.className = 'd-block mt-3 fw-semibold text-success'; lbl.textContent = `Descripción actualizada y ${res.renamed||0} archivo(s) renombrado(s).`; }
        }catch(_){}
      },
      function(err){
        btn.disabled = false;
        alert('Error: ' + (err && err.get_message ? err.get_message() : 'desconocido'));
      }
    );
  });

  function refreshThumbs(){
    const list = fm.thumbs();
    list.innerHTML = '';
    __fmFiles.forEach(f => {
      const r = new FileReader();
      r.onload = e => {
        const wrap = document.createElement('div');
        wrap.className = 'fm-thumb';
        const img = document.createElement('img');
        img.src = e.target.result; img.alt = f.name; img.loading = 'lazy';
        wrap.appendChild(img);
        list.appendChild(wrap);
      };
      r.readAsDataURL(f);
    });
    fm.btn().disabled = (__fmFiles.length < 3);
  }

  // ======================================================
  // ==============  CÁMARA EN VIVO (NUEVO)  ==============
  // ======================================================
  fm.camBtn().addEventListener('click', async function(){
    if (!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)) {
      // Sin cámara: abrimos selector nativo como fallback
      fm.input().click();
      return;
    }
    openCameraOverlay();
  });

  function openCameraOverlay(){
    const ov = document.createElement('div');
    ov.className = 'cam-ov';
    ov.innerHTML = `
      <div class="cam-ov-inner">
        <div class="text-white small px-1">Cámara activa</div>
        <video id="camLive" autoplay playsinline muted></video>
        <div class="row-actions">
          <button type="button" class="btn btn-light" id="camCapture"><i class="bi bi-camera-fill"></i> Capturar</button>
          <button type="button" class="btn btn-outline-light" id="camCancel">Cancelar</button>
        </div>
      </div>`;
    document.body.appendChild(ov);

    const video = ov.querySelector('#camLive');
    const btnOk = ov.querySelector('#camCapture');
    const btnCancel = ov.querySelector('#camCancel');

    let stream;

    const stopAll = () => {
      try{ stream?.getTracks()?.forEach(t => t.stop()); }catch(_){}
      ov.remove();
    };

    btnCancel.addEventListener('click', stopAll);

    navigator.mediaDevices.getUserMedia({
      video: { facingMode: { ideal: 'environment' } },
      audio: false
    }).then(s => {
      stream = s;
      video.srcObject = s;
    }).catch(_ => {
      // Permiso denegado o error -> fallback a input
      stopAll();
      fm.input().click();
    });

    btnOk.addEventListener('click', async () => {
      try{
        const blob = await captureFromVideo(video, {maxW:1600, maxH:1600, quality:0.85});
        const file = new File([blob], genCamName(), {type:'image/jpeg'});
        __fmFiles.push(file);
        refreshThumbs();
      }catch(err){
        alert('No se pudo capturar: ' + err.message);
      }finally{
        stopAll();
      }
    }, { once:true });
  }

  function captureFromVideo(video, opts){
    return new Promise((resolve, reject) => {
      try{
        const vw = video.videoWidth || 1600, vh = video.videoHeight || 1200;
        const {w, h} = fitWithin(vw, vh, opts.maxW||1600, opts.maxH||1600);
        const canvas = document.createElement('canvas');
        canvas.width = w; canvas.height = h;
        const ctx = canvas.getContext('2d');
        ctx.drawImage(video, 0, 0, w, h);
        canvas.toBlob((b)=> b ? resolve(b) : reject(new Error('blob nulo')), 'image/jpeg', opts.quality ?? 0.85);
      }catch(e){ reject(e); }
    });
  }

  function fitWithin(w, h, maxW, maxH){
    const r = Math.min(maxW / w, maxH / h, 1);
    return { w: Math.round(w*r), h: Math.round(h*r) };
  }

  function genCamName(){
    const z = (n, l=2)=> String(n).padStart(l,'0');
    const d = new Date();
    const ts = d.getFullYear()+z(d.getMonth()+1)+z(d.getDate())+z(d.getHours())+z(d.getMinutes())+z(d.getSeconds())+z(d.getMilliseconds(),3);
    return 'cam_'+ts+'.jpg';
  }
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

        // Expuesta para RegisterStartupScript del servidor
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
