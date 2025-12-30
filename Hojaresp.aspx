<%@ Page Title="Hoja de Trabajo"
    Language="VB"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="false"
    CodeBehind="Hoja.aspx.vb"
    Inherits="DAYTONAMIO.Hoja"
    MaintainScrollPositionOnPostBack="true" %>

<%@ MasterType VirtualPath="~/Site1.Master" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

  <!-- ====== ESTILOS BASE (tuyos) ====== -->
  <style>
    /* === VARIABLES DE COLOR EMPRESARIALES === */
    :root {
      --brand-900:#0a1f44;--brand-800:#163560;--brand-700:#1e4976;--brand-600:#2563eb;--brand-500:#3b82f6;--brand-400:#60a5fa;--brand-300:#93c5fd;--brand-200:#bfdbfe;--brand-100:#dbeafe;--brand-050:#eff6ff;
      --neutral-900:#0f172a;--neutral-800:#1e293b;--neutral-700:#334155;--neutral-600:#475569;--neutral-500:#64748b;--neutral-400:#94a3b8;--neutral-300:#cbd5e1;--neutral-200:#e2e8f0;--neutral-100:#f1f5f9;--neutral-050:#f8fafc;
      --success:#10b981;--warning:#f59e0b;--danger:#ef4444;--danger-light:#fee2e2;
      --shadow-sm:0 1px 2px 0 rgba(0,0,0,.05);--shadow-md:0 4px 6px -1px rgba(0,0,0,.1),0 2px 4px -1px rgba(0,0,0,.06);
      --shadow-lg:0 10px 15px -3px rgba(0,0,0,.1),0 4px 6px -2px rgba(0,0,0,.05);--shadow-xl:0 20px 25px -5px rgba(0,0,0,.1),0 10px 10px -5px rgba(0,0,0,.04);
      --border-radius-sm:4px;--border-radius-md:6px;--border-radius-lg:8px;--border-radius-xl:10px;
    }
    html,body{height:100%;font-family:'Segoe UI',system-ui,-apple-system,sans-serif;line-height:1.6;}
    body{background:linear-gradient(135deg,var(--brand-050) 0%,var(--neutral-050) 50%,#fff 100%);color:var(--neutral-800);overflow-x:hidden;min-height:100vh;}
    .page-header{background:linear-gradient(135deg,var(--brand-900) 0%,var(--brand-700) 50%,var(--brand-600) 100%);color:white;border-radius:var(--border-radius-lg);padding:24px 28px;box-shadow:var(--shadow-xl);position:relative;overflow:hidden;}
    .page-header::before{content:'';position:absolute;top:0;right:0;width:200px;height:200px;background:radial-gradient(circle,rgba(255,255,255,.1) 0%,transparent 70%);border-radius:50%;transform:translate(50%,-50%);}
    .page-header h3{margin:0;font-weight:700;font-size:1.75rem;letter-spacing:-.025em;position:relative;z-index:2;}
    .page-sub{opacity:.95;font-size:1rem;font-weight:500;margin-top:4px;position:relative;z-index:2;}
    .card-pane{background:white;border:1px solid var(--neutral-300);border-radius:var(--border-radius-lg);box-shadow:var(--shadow-lg);transition:all .3s ease;backdrop-filter:blur(10px);animation:fadeInUp .6s ease-out;}
    .card-pane:hover{box-shadow:var(--shadow-xl);transform:translateY(-2px);}
    .card-title{color:var(--brand-900);font-weight:700;font-size:1.25rem;border-bottom:2px solid var(--neutral-200);padding-bottom:12px;margin-bottom:20px;position:relative;}
    .card-title::after{content:'';position:absolute;bottom:-2px;left:0;width:60px;height:2px;background:linear-gradient(90deg,var(--brand-600),var(--brand-400));border-radius:2px;}
    .field-label{color:var(--neutral-600);font-weight:600;font-size:.875rem;text-transform:uppercase;letter-spacing:.5px;margin-bottom:4px;display:flex;align-items:center;gap:6px;}
    .field-label::before{font-family:'bootstrap-icons';font-size:1rem;color:var(--brand-600);}
    .field-label[data-field="expediente"]::before{content:'\f2db';}
    .field-label[data-field="siniestro"]::before{content:'\f33a';}
    .field-label[data-field="asegurado"]::before{content:'\f4da';}
    .field-label[data-field="telefono"]::before{content:'\f4b2';}
    .field-label[data-field="correo"]::before{content:'\f32f';}
    .field-label[data-field="vehiculo"]::before{content:'\f28c';}
    .field-label[data-field="id"]::before{content:'\f471';}
    .field-label[data-field="carpeta"]::before{content:'\f2dc';}
    .field-label[data-field="reporte"]::before{content:'\f471';}
    .value{color:var(--neutral-900);font-weight:600;font-size:1rem;padding:12px 16px;background:var(--neutral-100);border-radius:var(--border-radius-sm);border-left:4px solid var(--brand-500);transition:all .3s ease;position:relative;overflow:hidden;}
    .value::before{content:'';position:absolute;top:0;left:0;width:4px;height:100%;background:linear-gradient(180deg,var(--brand-500),var(--brand-400));}
    .value:hover{background:var(--brand-050);border-left-color:var(--brand-600);transform:translateX(2px);}

    .img-frame{position:relative;width:100%;height:420px;background:linear-gradient(135deg,var(--neutral-050),var(--brand-050));border:2px dashed var(--brand-300);border-radius:var(--border-radius-md);display:flex;align-items:center;justify-content:center;overflow:hidden;transition:all .3s ease;cursor:pointer;}
    .img-frame:hover{border-color:var(--brand-500);background:linear-gradient(135deg,var(--brand-050),var(--neutral-050));}
    .img-frame img{max-width:100%;max-height:100%;object-fit:contain;display:block;border-radius:var(--border-radius-sm);}
    .img-delete{position:absolute;top:12px;right:12px;background:white;color:var(--danger);border:1px solid var(--neutral-300);border-radius:50%;padding:8px 12px;line-height:1;font-weight:700;box-shadow:var(--shadow-md);transition:all .3s ease;z-index:10;}
    .img-delete:hover{background:var(--danger-light);color:var(--danger);text-decoration:none;transform:scale(1.1);}

    .btn-brand{background:linear-gradient(135deg,var(--brand-700),var(--brand-600));border:none;color:white;font-weight:600;padding:12px 24px;border-radius:var(--border-radius-sm);box-shadow:var(--shadow-md);transition:all .3s ease;text-transform:uppercase;letter-spacing:.5px;font-size:.875rem;}
    .btn-brand:hover{background:linear-gradient(135deg,var(--brand-800),var(--brand-700));color:white;transform:translateY(-2px);box-shadow:var(--shadow-lg);}
    .btn-ghost{border:2px solid var(--brand-600);color:var(--brand-700);background:white;font-weight:600;padding:10px 22px;border-radius:var(--border-radius-sm);transition:all .3s ease;text-transform:uppercase;letter-spacing:.5px;font-size:.875rem;}
    .btn-ghost:hover{background:var(--brand-600);color:white;border-color:var(--brand-700);transform:translateY(-2px);}

    .doc-strip{background:linear-gradient(135deg,white,var(--neutral-050));border:1px solid var(--neutral-300);border-radius:var(--border-radius-lg);}
    .tile{border:1px solid #e5e7eb;border-radius:12px;padding:14px 16px;background:#fff;display:flex;flex-direction:column;align-items:center;justify-content:center;text-align:center;animation:fadeInUp .6s ease-out;}
    .doc-strip .title{font-weight:700;color:var(--brand-900);font-size:1rem;text-transform:uppercase;letter-spacing:1px;}
    .icon-row{display:inline-flex;flex-wrap:wrap;gap:12px;align-items:center;justify-content:center;margin-top:10px;}
    .icon-btn{display:inline-flex;align-items:center;justify-content:center;width:44px;height:44px;border:1px solid #e5e7eb;border-radius:10px;background:#f8fafc;padding:0;line-height:1;}
    .icon-btn i{font-size:20px;}
    .icon-btn.disabled,.icon-btn[disabled]{pointer-events:none;opacity:.5;}
    .icon-row.inv{display:grid;grid-template-columns:repeat(2,44px);gap:12px 16px;justify-content:center;}
    @media (min-width: 992px){.icon-row.inv{grid-template-columns:repeat(4,44px);gap:12px;}}

    .gallery-big{height:60vh;display:flex;align-items:center;justify-content:center;background:linear-gradient(135deg,var(--neutral-050),var(--brand-050));border:1px solid var(--neutral-300);border-radius:var(--border-radius-md);overflow:hidden;box-shadow:var(--shadow-md);}
    .gallery-big img{max-width:100%;max-height:100%;object-fit:contain;border-radius:var(--border-radius-sm);}
    .gallery-grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(160px,1fr));gap:16px;margin-top:16px;}
    .grid-item{position:relative;border-radius:var(--border-radius-sm);overflow:hidden;transition:all .3s ease;}
    .grid-item:hover{transform:scale(1.05);box-shadow:var(--shadow-lg);}
    .grid-thumb{width:100%;height:140px;object-fit:cover;border-radius:var(--border-radius-sm);border:1px solid var(--neutral-300);cursor:pointer;transition:all .3s ease;}
    .grid-check-wrap{position:absolute;top:8px;left:8px;background:rgba(255,255,255,.95);padding:6px 8px;border-radius:var(--border-radius-sm);box-shadow:var(--shadow-sm);backdrop-filter:blur(5px);}

    .modal-content{border:none;border-radius:var(--border-radius-lg);box-shadow:var(--shadow-xl);backdrop-filter:blur(10px);}
    .modal-header{background:linear-gradient(135deg,var(--brand-700),var(--brand-600));color:white;border-radius:var(--border-radius-lg) var(--border-radius-lg) 0 0;border-bottom:none;padding:20px 24px;}
    .modal-title{font-weight:700;font-size:1.25rem;}
    .btn-close{filter:invert(1);opacity:.8;}
    .btn-close:hover{opacity:1;}

    /* Contenedores de thumbs (base) */
    #thumbs,#thumbsPresup{display:flex;flex-wrap:wrap;gap:12px;margin-top:16px;}
    .thumb{width:120px;height:120px;object-fit:cover;border-radius:var(--border-radius-sm);border:1px solid var(--neutral-300);box-shadow:var(--shadow-sm);transition:all .3s ease;}
    .thumb:hover{transform:scale(1.05);box-shadow:var(--shadow-md);}
    .thumb-wrap{display:flex;flex-direction:column;align-items:center;font-size:.75rem;width:120px;color:var(--neutral-600);}
    .thumb-name{margin-top:8px;text-align:center;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;width:100%;font-weight:500;}

    .progress{height:8px;border-radius:10px;background-color:var(--neutral-200);overflow:hidden;}
    .progress-bar{background:linear-gradient(90deg,var(--brand-600),var(--brand-500));border-radius:10px;transition:width .6s ease;}

    .form-control{border:1px solid var(--neutral-300);border-radius:var(--border-radius-sm);padding:12px 16px;font-size:.95rem;transition:all .3s ease;background-color:white;}
    .form-control:focus{border-color:var(--brand-500);box-shadow:0 0 0 3px rgba(93,125,214,.1);background-color:var(--brand-050);}
    .form-label{font-weight:600;color:var(--neutral-700);margin-bottom:8px;font-size:.9rem;}

    canvas{border:2px solid var(--neutral-300)!important;border-radius:var(--border-radius-sm)!important;background:white;transition:all .3s ease;}
    canvas:hover{border-color:var(--brand-400)!important;}

    .info-strip{background:var(--neutral-100);border:1px solid var(--neutral-200);border-radius:var(--border-radius-sm);padding:12px 16px;font-size:.875rem;color:var(--neutral-600);}
    .info-strip .field-label{display:inline-flex;margin-bottom:0;margin-right:4px;font-size:.875rem;}

    @media (max-width:768px){
      .page-header{padding:20px 24px;}
      .page-header h3{font-size:1.5rem;}
      .card-pane{margin-bottom:20px;}
      .gallery-grid{grid-template-columns:repeat(auto-fill,minmax(120px,1fr));gap:12px;}
      .doc-strip .tile{padding:16px 12px;}
    }

    @keyframes fadeInUp{from{opacity:0;transform:translateY(20px);}to{opacity:1;transform:translateY(0);}}

    .gallery-nav-btn{position:absolute;top:50%;transform:translateY(-50%);border:none;background:rgba(255,255,255,.9);width:44px;height:44px;border-radius:50%;display:flex;align-items:center;justify-content:center;box-shadow:var(--shadow-md);cursor:pointer;transition:all .2s ease;}
    .gallery-nav-btn:hover{background:#fff;transform:translateY(-50%) scale(1.05);}
    .gallery-prev{left:12px;}
    .gallery-next{right:12px;}
    .gallery-nav-btn i{font-size:22px;line-height:1;color:var(--brand-700);}

    .dnd-dropzone{border:2px dashed var(--brand-300);background:linear-gradient(135deg,var(--neutral-050),var(--brand-050));border-radius:var(--border-radius-md);padding:28px;text-align:center;transition:all .2s ease;cursor:pointer;}
    .dnd-dropzone:hover{border-color:var(--brand-500);background:linear-gradient(135deg,var(--brand-050),var(--neutral-050));}
    .dnd-dropzone.dnd-over{border-color:var(--brand-600);box-shadow:var(--shadow-md);transform:translateY(-1px);}
    .dnd-icon{font-size:42px;color:var(--brand-600);}
    .dnd-title{font-weight:700;color:var(--brand-900);margin-top:6px;}
    .dnd-sub{color:var(--neutral-600);font-size:.95rem;}

    .meta-bar{display:flex;gap:10px;align-items:center;justify-content:flex-start;padding:6px 10px;margin-bottom:6px;}
    .meta-chip{display:inline-flex;align-items:center;gap:6px;font-size:.80rem;color:var(--neutral-700);border:1px solid var(--neutral-200);background:linear-gradient(135deg,#fff,var(--neutral-050));border-radius:999px;padding:4px 10px;box-shadow:var(--shadow-sm);}
    .meta-chip .bi{font-size:14px;color:var(--brand-700);}
    .meta-chip .meta-caption{text-transform:uppercase;font-weight:700;letter-spacing:.4px;color:var(--neutral-600);}
    .meta-chip .meta-value{font-weight:700;color:var(--brand-900);}

    /* ====== STRIP COMPACTO ====== */
    .doc-strip.compacto{padding:8px!important;border-radius:var(--border-radius-lg);border:1px solid var(--neutral-300);background:linear-gradient(135deg,#fff,var(--neutral-050));box-shadow:var(--shadow-md);}
    .doc-strip-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:10px;}
    .tile.compacto{padding:10px 12px;border:1px solid var(--neutral-200);border-radius:10px;background:#fff;min-height:92px;display:grid;grid-template-rows:auto 1fr;align-items:center;justify-items:center;text-align:center;box-shadow:var(--shadow-sm);transition:transform .15s ease,box-shadow .15s ease,border-color .15s ease;}
    .tile.compacto:hover{transform:translateY(-1px);box-shadow:var(--shadow-md);border-color:var(--brand-300);}
    .tile.compacto .title{margin:0 0 6px 0;font-size:.92rem;letter-spacing:.4px;color:var(--brand-900);font-weight:700;}
    .icon-row.compacto{display:inline-flex;gap:8px;flex-wrap:wrap;align-items:center;justify-content:center;}
    .icon-btn.compacto{width:36px;height:36px;border:1px solid var(--neutral-300);background:#f8fafc;display:inline-flex;align-items:center;justify-content:center;line-height:1;padding:0;box-shadow:var(--shadow-sm);}
    .icon-btn.compacto i{font-size:18px;}
    .icon-btn.compacto:hover{background:var(--brand-600);color:#fff;border-color:var(--brand-700);transform:translateY(-1px);}
    .icon-btn.compacto.disabled,.icon-btn.compacto[disabled]{pointer-events:none;opacity:.45;}
    .icon-row.inv.compacto{display:grid!important;grid-template-columns:repeat(2,36px);grid-auto-rows:36px;gap:8px 10px;justify-content:center;align-content:center;}
    @media (min-width: 992px){.icon-row.inv.compacto{grid-template-columns:repeat(4,36px);}}
    .icon-row.inv.compacto .icon-btn{width:36px;height:36px;}
    .icon-row.inv.compacto .icon-btn i{font-size:18px;}
    .doc-strip.compacto.alert-pulse{animation:blinkBorder 1.1s ease-in-out infinite;}
    @keyframes blinkBorder{0%{box-shadow:0 0 0 0 rgba(239,68,68,.55);border-color:#fecaca;}50%{box-shadow:0 0 0 6px rgba(239,68,68,0);border-color:#ef4444;}100%{box-shadow:0 0 0 0 rgba(239,68,68,.55);border-color:#fecaca;}}

    /* Blink para botón PROCESO DE RECEPCION */
    #btnToggleStrip.blink-danger{animation:blinkBtnDanger 1.1s ease-in-out infinite!important;background:#fee2e2!important;border-color:#ef4444!important;color:#dc2626!important;}
    @keyframes blinkBtnDanger{0%{box-shadow:0 0 0 0 rgba(239,68,68,.55);}50%{box-shadow:0 0 0 8px rgba(239,68,68,0);}100%{box-shadow:0 0 0 0 rgba(239,68,68,.55);}}
    #btnToggleStrip.blink-success{background:#dcfce7!important;border-color:#16a34a!important;color:#15803d!important;}

    /* Blink para botón PROCESO DE DIAGNOSTICO */
    #btnToggleStripDiag.blink-danger{animation:blinkBtnDanger 1.1s ease-in-out infinite!important;background:#fee2e2!important;border-color:#ef4444!important;color:#dc2626!important;}
    #btnToggleStripDiag.blink-success{background:#dcfce7!important;border-color:#16a34a!important;color:#15803d!important;}

    /* Blink para botón PROCESO VALUACIÓN */
    #btnToggleStripVal.blink-danger{animation:blinkBtnDanger 1.1s ease-in-out infinite!important;background:#fee2e2!important;border-color:#ef4444!important;color:#dc2626!important;}
    #btnToggleStripVal.blink-success{background:#dcfce7!important;border-color:#16a34a!important;color:#15803d!important;}

    /* Estilos para el contenedor stripVal - parpadeo del borde */
    #stripVal.strip-danger{animation:blinkBorder 1.1s ease-in-out infinite;}

    /* Estilos para el contenedor stripDiag - solo parpadeo del borde */
    #stripDiag.strip-danger{animation:blinkBorder 1.1s ease-in-out infinite;}
    #strip{overflow:hidden;transition:max-height .35s ease,opacity .25s ease,transform .35s ease;}
    #strip.is-collapsed{max-height:0!important;opacity:0;transform:translateY(6px);pointer-events:none;margin-top:0!important;margin-bottom:0!important;border-width:0;}

    /* Transición y colapso para todas las tiras */
    #strip, #stripDiag, #stripVal{ overflow:hidden; transition:max-height .35s ease, opacity .25s ease, transform .35s ease; }
    #strip.is-collapsed, #stripDiag.is-collapsed, #stripVal.is-collapsed{ max-height:0!important; min-height:0!important; height:0!important; opacity:0; transform:translateY(6px); pointer-events:none; margin:0!important; padding:0!important; border-width:0; }

    /* Tiles de fechas en valuación */
    .fecha-tile{min-height:60px!important;padding:8px!important;display:flex!important;flex-direction:column!important;justify-content:center!important;}
    .fecha-tile small{font-size:.7rem;line-height:1.2;margin-bottom:4px;}
    .fecha-valor{font-size:.8rem;color:var(--brand-900);}

    /* Asegurar mismo tamaño para todos los toggle buttons */
    .btn-toggle-strip{min-width:280px;max-width:320px;padding:10px 20px;font-size:.9rem;display:inline-flex;align-items:center;justify-content:center;gap:8px;box-sizing:border-box;}

    /* Contenedores de las secciones - heredar mismo layout */
    #diagSection, #valSection{width:100%;padding:0;margin:0;}

    /* Espaciado consistente entre toggles - juntos con pequeño espacio */
    #diagSection, #valSection{margin-top:0.5rem!important;}
    #diagSection > .d-flex, #valSection > .d-flex{margin-top:0!important;}

    /* Todas las tiras con mismo estilo base y altura consistente */
    #strip, #stripDiag, #stripVal{
      max-width:100%;
      box-sizing:border-box;
      margin-left:auto;
      margin-right:auto;
      min-height:120px;
    }
  </style>

  <!-- Pinta verde la tarjeta ODA si el LinkButton existe/habilitado -->
  <style>
/* Verde solo cuando el tile tenga la clase .is-ready */
.tile.compacto.is-ready{
  border-color:#16a34a; background:#ecfdf5;
  box-shadow:0 0 0 3px rgba(22,163,74,.15) inset;
  transition:background .2s, box-shadow .2s, border-color .2s;
}
.tile.ok {
  background-color: #ecfdf5 !important; /* success-subtle */
  border: 1px solid #198754;            /* success */
  }

/* Contenedor de inicio centrado */
.diag-inicio {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: .5rem;
  font-weight: 600;
}

/* Cabecera (fin diagnóstico) dentro de cada tile */
.tile .tile-top {
  margin-top: .25rem;
  margin-bottom: .25rem;
  font-size: .95rem;
}
.tile .tile-top .meta-caption { opacity: .85; margin-left: .25rem; margin-right: .25rem; }

  </style>

  <!-- Ajustes visuales cámara + miniaturas compactas (override) -->
  <style>
    #camBlockPresup .cam-body { position: relative; }
    #camVideoPresup, #camPreviewPresup {
      display: block;
      width: 100%;
      max-height: 60vh;
      object-fit: contain;
      border-radius: 6px;
    }

    /* OVERRIDE para thumbs: más compactos y consistentes */
    #thumbsPresup { --thumb-size: 96px; gap: 10px; overflow: visible; }
    @media (min-width: 992px){
      #thumbsPresup { --thumb-size: 120px; }
    }
    #thumbsPresup .thumb-wrap { width: var(--thumb-size); position: relative; overflow: visible; }
    #thumbsPresup img.thumb {
      display: block;
      width: var(--thumb-size);
      height: var(--thumb-size);
      object-fit: cover;
      border-radius: 6px;
      border: 1px solid var(--neutral-300);
      box-shadow: var(--shadow-sm);
    }
    #thumbsPresup .thumb-delete {
      position: absolute;
      top: 2px;
      right: 2px;
      width: 24px;
      height: 24px;
      border-radius: 50%;
      background: rgba(220, 53, 69, 0.9);
      color: #fff;
      border: none;
      font-size: 18px;
      font-weight: bold;
      line-height: 22px;
      text-align: center;
      cursor: pointer;
      box-shadow: 0 2px 4px rgba(0,0,0,0.4);
      z-index: 10;
      transition: all 0.2s ease;
    }
    #thumbsPresup .thumb-delete:hover {
      background: rgba(200, 35, 51, 1);
      transform: scale(1.15);
    }
    #thumbsPresup .thumb-name{
      margin-top: 6px;
      max-width: 100%;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      font-size: .75rem;
      color: var(--neutral-600);
      text-align: center;
    }

    /* en tus estilos, junto a .gallery-big */
.gallery-big img#galleryBigImg{ cursor: zoom-in; }
.gallery-big img#galleryBigImg.fs{ cursor: zoom-out; }

/* Zoom/Pan en fullscreen */
#galleryBigImg.fs{
  width:100%;
  height:100%;
  object-fit:contain;
  transform-origin:center center;
}
#galleryBigImg.zooming{ cursor: grab; }
#galleryBigImg.dragging{ cursor: grabbing; }

/* Overlay de pantalla completa */
.fullscreen-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.95);
  z-index: 9999;
  display: flex;
  flex-direction: column;
}
.fullscreen-overlay .fs-header {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 10001;
}
.fullscreen-overlay .fs-close-btn {
  background: rgba(255,255,255,0.9);
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  font-size: 20px;
  cursor: pointer;
  box-shadow: 0 2px 8px rgba(0,0,0,0.3);
}
.fullscreen-overlay .fs-close-btn:hover {
  background: #fff;
  transform: scale(1.1);
}
.fullscreen-overlay .fs-image-container {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  padding: 20px;
}
.fullscreen-overlay #fsImage {
  max-width: 90vw;
  max-height: 85vh;
  object-fit: contain;
  cursor: grab;
  transform-origin: center center;
}
.fullscreen-overlay #fsImage.dragging {
  cursor: grabbing;
}
.fullscreen-overlay #fsImage.zoomed {
  cursor: move;
}
.fullscreen-overlay .fs-zoom-controls {
  position: absolute;
  bottom: 20px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 10px;
  align-items: center;
  background: rgba(255,255,255,0.9);
  border-radius: 999px;
  padding: 10px 16px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.3);
}
.fullscreen-overlay .fs-zoom-btn {
  background: #007bff;
  color: #fff;
  border: none;
  border-radius: 50%;
  width: 36px;
  height: 36px;
  font-size: 18px;
  cursor: pointer;
}
.fullscreen-overlay .fs-zoom-btn:hover {
  background: #0056b3;
}
.fullscreen-overlay #fsZoomRange {
  width: 150px;
}

  </style>
    <style>
  .zoom-toolbar{
    position:absolute;
    bottom:12px;
    left:50%;
    transform:translateX(-50%);
    display:flex;
    gap:10px;
    align-items:center;
    background:rgba(255,255,255,.9);
    border:1px solid var(--neutral-300);
    border-radius:999px;
    padding:8px 12px;
    box-shadow:var(--shadow-md);
    z-index:5;
  }
  .zoom-toolbar .zoom-btn{
    border:1px solid var(--neutral-300);
    background:#fff;
    border-radius:8px;
    padding:6px 10px;
    line-height:1;
    cursor:pointer;
    user-select:none;
  }
  .zoom-toolbar input[type="range"]{ width:200px; }
  @media (max-width:576px){
    .zoom-toolbar input[type="range"]{ width:140px; }
  }

  /* Cursores y estados del zoom (reusa tus clases existentes) */
  .gallery-big img#galleryBigImg{ cursor: zoom-in; transition: transform .05s linear; }
  .gallery-big img#galleryBigImg.zooming{ cursor: grab; }
  .gallery-big img#galleryBigImg.dragging{ cursor: grabbing; }
</style>

    <style>
  /* Cursores existentes */
  .gallery-big img#galleryBigImg{ cursor: zoom-in; transition: transform .05s linear; }
  .gallery-big img#galleryBigImg.fs{ cursor: zoom-out; }
  .gallery-big img#galleryBigImg.zooming{ cursor: grab; }
  .gallery-big img#galleryBigImg.dragging{ cursor: grabbing; }

  /* Barra de zoom visible SOLO en pantalla completa */
  .fs-zoom-toolbar{
    position: fixed;
    left: 50%;
    bottom: 16px;
    transform: translateX(-50%);
    display: flex; gap: 10px; align-items: center;
    background: rgba(255,255,255,.95);
    border: 1px solid #e2e8f0;
    border-radius: 999px;
    padding: 8px 12px;
    box-shadow: 0 4px 12px rgba(0,0,0,.12);
    z-index: 2147483647;
    user-select: none;
    -webkit-user-select: none;
  }
  .fs-zoom-toolbar .zoom-btn{
    border: 1px solid #e2e8f0;
    background: #fff;
    border-radius: 8px;
    padding: 6px 10px;
    font-weight: 700;
    line-height: 1;
  }
  .fs-zoom-toolbar .zoom-btn:active{ transform: translateY(1px); }
  .fs-zoom-toolbar .zoom-range{
    width: 180px;
    accent-color: #2563eb; /* azul de tu paleta */
  }
  .fs-zoom-toolbar .zoom-label{
    font-size: .85rem; color: #334155; min-width: 48px; text-align: right;
  }
</style>

    <style>
  /* === Toggle de diagnóstico (Mec/Hoj) === */
  .diag-flag{
    display:inline-flex; align-items:center; justify-content:center;
    gap:8px; margin-bottom:8px;
    background:linear-gradient(135deg,#fff,var(--neutral-050));
    border:1px solid var(--neutral-200);
    border-radius:999px; padding:6px 10px; box-shadow:var(--shadow-sm);
  }
  .diag-flag .form-check-input{ margin:0; cursor:pointer; }
  .diag-flag .bi{ font-size:18px; }
  .diag-flag .state{ font-size:.85rem; font-weight:700; }
  .diag-flag.on  .bi{ color:var(--success); }
  .diag-flag.off .bi{ color:var(--danger); }

  /* Ya tienes esta regla, la reutilizamos para bloquear clicks */
  /* .icon-btn.compacto.disabled,.icon-btn.compacto[disabled]{pointer-events:none;opacity:.45;} */

 /* Switch con tu paleta */
.form-switch .form-check-input {
  width: 2.7rem; height: 1.4rem;
  background-color: var(--neutral-300);
  border-color: var(--neutral-400);
}

.form-switch .form-check-input:checked {
  background-color: var(--brand-600);
  border-color: var(--brand-600);
}

.form-switch .form-check-input:focus {
  box-shadow: 0 0 0 .20rem rgba(37, 99, 235, .18);
}
.tile-disabled{ opacity:.55; pointer-events:none; filter:grayscale(.5); }

/* === Estilos para Hoja de Trabajo Grid === */
.ht-grid { font-size: 0.85rem; }
.ht-grid th { vertical-align: middle !important; }
.ht-toggle {
  cursor: pointer;
  display: inline-block;
  min-width: 20px;
  min-height: 20px;
  line-height: 20px;
  font-weight: bold;
  user-select: none;
  text-align: center;
}
.ht-toggle:hover { background: #f0f0f0; border-radius: 3px; }
.ht-toggle:empty::after { content: "○"; color: #ccc; } /* placeholder cuando está vacío */
.ht-si { color: #16a34a; } /* verde */
.ht-no { color: #dc2626; } /* rojo */
.ht-status { color: #2563eb; } /* azul */

</style>




</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="container py-4">
    <div class="page-header mb-4">
      <h3>Hoja de trabajo</h3>
      <div class="page-sub">Gestión de expediente e imágenes</div>
    </div>

    <div class="row g-4">
      <!-- IZQUIERDA: Imagen principal -->
      <div class="col-12 col-lg-5">
        <div class="p-3 card-pane">
          <div class="card-title h5">Imágenes del expediente</div>
          <div id="dropZone" class="img-frame mb-3" role="button" title="Click o arrastra para subir (te preguntaré si quieres cámara)">
            <asp:LinkButton ID="btnEliminarPrincipal" runat="server" CssClass="img-delete" ClientIDMode="Static"
              OnClick="btnEliminarPrincipal_Click" Visible="false"
              ToolTip="Eliminar imagen principal (principal.jpg)">✕</asp:LinkButton>
            <asp:Image ID="imgPreview" runat="server" AlternateText="Sin imagen" ClientIDMode="Static" />
            <input id="fileDrop" type="file" accept="image/*" capture="environment" class="d-none" />
          </div>
        </div>
      </div>

      <!-- DERECHA: Datos -->
      <div class="col-12 col-lg-7">
        <div class="p-3 card-pane">
          <div class="card-title h5">Datos del expediente</div>
          <div class="row g-3">
            <div class="col-12 col-md-6">
              <div class="field-label" data-field="expediente">Expediente</div>
              <div class="value"><asp:Label ID="lblExpediente" runat="server" ClientIDMode="Static" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label" data-field="siniestro">Siniestro</div>
              <div class="value"><asp:Label ID="lblSiniestro" runat="server" ClientIDMode="Static" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label" data-field="asegurado">Asegurado</div>
              <div class="value"><asp:Label ID="lblAsegurado" runat="server" ClientIDMode="Static" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label" data-field="telefono">Teléfono</div>
              <div class="value"><asp:Label ID="lblTelefono" runat="server" ClientIDMode="Static" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label" data-field="correo">Correo</div>
              <div class="value"><asp:Label ID="lblCorreo" runat="server" ClientIDMode="Static" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label" data-field="reporte">Reporte</div>
              <div class="value"><asp:Label ID="lblReporte" runat="server" ClientIDMode="Static" /></div>
            </div>
            <div class="col-12">
              <div class="field-label" data-field="vehiculo">Vehículo</div>
              <div class="value"><asp:Label ID="lblVehiculo" runat="server" ClientIDMode="Static" /></div>
            </div>
          </div>
        </div>

        <div class="mt-3 info-strip">
          <span class="field-label" data-field="id">ID:</span><asp:Label ID="lblId" runat="server" ClientIDMode="Static" />
          <span class="field-label ms-4" data-field="carpeta">Carpeta destino:</span><asp:Label ID="lblCarpeta" runat="server" ClientIDMode="Static" />
        </div>
      </div>
    </div>

    <!-- ====== META ====== -->
    <div class="card-pane doc-strip p-2 mb-2">
      <div class="meta-bar">
        <div class="meta-chip">
          <i class="bi bi-calendar3"></i>
          <span class="meta-caption">Creado:</span>
          <span class="meta-value"><asp:Label ID="lblFechaCreacion" runat="server" Text="—" ClientIDMode="Static" /></span>
        </div>
        <div class="meta-chip">
          <i class="bi bi-hourglass-split"></i>
          <span class="meta-caption">Días:</span>
          <span class="meta-value"><asp:Label ID="lblDiasTranscurridos" runat="server" Text="—" ClientIDMode="Static" /></span>
        </div>
        <div class="meta-chip d-none d-sm-inline-flex">
          <i class="bi bi-info-circle"></i>
          <span class="meta-caption">Extra:</span>
          <span class="meta-value"><asp:Label ID="lblMeta3" runat="server" Text="—" ClientIDMode="Static" /></span>
        </div>
      </div>
    </div>

<!-- ====== TOGGLE: Tira inferior ====== -->
<div class="d-flex justify-content-center mt-3">
  <button type="button" id="btnToggleStrip" class="btn-toggle-strip" title="Mostrar/Ocultar documentos">
    <i class="bi bi-layers"></i>
    PROCESO DE RECEPCION
    <i class="bi bi-chevron-down chev"></i>
  </button>
</div>

<!-- ====== TIRA INFERIOR COMPACTA ====== -->
<div id="strip" class="card-pane doc-strip compacto p-2 mt-4">
  <div class="doc-strip-grid">

    <!-- ODA -->
    <div id="tileODA" runat="server" class="tile compacto">
      <div class="title">ODA</div>
      <div class="icon-row compacto">
        <a href="#" class="icon-btn compacto disabled" title="Subir ODA (pendiente)">
          <i class="bi bi-cloud-upload"></i>
        </a>
        <asp:LinkButton ID="btnVerODA" runat="server" CssClass="icon-btn compacto" ToolTip="Ver ODA.pdf" OnClick="btnVerODA_Click">
          <i class="bi bi-eye"></i>
        </asp:LinkButton>
      </div>
    </div>

    <!-- Fotos ingreso -->
    <div id="TileFotos" runat="server" class="tile compacto">
      <div class="title">Fotos ingreso</div>
      <div class="icon-row compacto">
        <a href="#" class="icon-btn compacto" id="btnSubirFotosIngreso" data-bs-toggle="modal" data-bs-target="#modalMultiplesPresup" title="Subir varias fotos de ingreso">
          <i class="bi bi-cloud-upload"></i>
        </a>
        <asp:LinkButton ID="btnVerFotosPresup" runat="server" CssClass="icon-btn compacto" ToolTip="Ver galería ingreso" OnClick="btnVerFotosPresup_Click">
          <i class="bi bi-eye"></i>
        </asp:LinkButton>
      </div>
    </div>

    <!-- INE -->
    <div id="tileINE" runat="server" class="tile compacto">
      <div class="title">INE</div>
      <div class="icon-row compacto">
        <a href="#" class="icon-btn compacto" id="btnSubirInePdf" data-bs-toggle="modal" data-bs-target="#modalInePdf" title="Subir INE.pdf">
          <i class="bi bi-cloud-upload"></i>
        </a>
        <a href="#" class="icon-btn compacto" id="btnSubirIneCamara" data-bs-toggle="modal" data-bs-target="#modalIneCamara" title="Tomar/Subir 2 fotos del INE">
          <i class="bi bi-camera"></i>
        </a>
        <asp:LinkButton ID="btnVerINE" runat="server" CssClass="icon-btn compacto" ToolTip="Ver INE.pdf" OnClick="btnVerINE_Click">
          <i class="bi bi-eye"></i>
        </asp:LinkButton>
      </div>
    </div>

    <!-- CT -->
    <div id="tileCT" runat="server" class="tile compacto">
      <div class="title">CT</div>
      <div class="icon-row compacto">
        <a href="#" class="icon-btn compacto" id="btnSubirCt" data-bs-toggle="modal" data-bs-target="#modalCt" title="Llenar y firmar CT">
          <i class="bi bi-cloud-upload"></i>
        </a>
        <asp:LinkButton ID="btnVerCT" runat="server" CssClass="icon-btn compacto" ToolTip="Ver CT.pdf" OnClick="btnVerCT_Click">
          <i class="bi bi-eye"></i>
        </asp:LinkButton>
      </div>
    </div>

    <!-- INV -->
    <div id="tileINV" runat="server" class="tile compacto">
      <div class="title">INV</div>
      <div class="icon-row inv compacto">
        <asp:LinkButton ID="btnInvHtml" runat="server" CssClass="icon-btn compacto" ToolTip="Abrir inventario.html" OnClick="btnInvHtml_Click" aria-label="Abrir inventario">
          <i class="bi bi-cloud-upload"></i>
        </asp:LinkButton>

        <asp:LinkButton ID="btnVerINV" runat="server" CssClass="icon-btn compacto disabled" ToolTip="(Reservado)" Enabled="False" aria-label="Ver inventario (reservado)">
          <i class="bi bi-eye"></i>
        </asp:LinkButton>

        <asp:LinkButton ID="btnInvGrua" runat="server" CssClass="icon-btn compacto" ToolTip="Subir inventario de grúa (PDF)" OnClick="btnInvGrua_Click" aria-label="Subir inventario grúa">
          <i class="bi bi-truck-front"></i>
        </asp:LinkButton>

        <asp:LinkButton ID="btnVerInvGrua" runat="server" CssClass="icon-btn compacto" ToolTip="Ver inventario de grúa" OnClick="btnVerInvGrua_Click" aria-label="Ver inventario grúa">
          <i class="bi bi-eye"></i>
        </asp:LinkButton>
      </div>
    </div>


    <div id="tileComplementos" runat="server" class="tile compacto">
      <div class="title">Complementos</div>
      <div class="icon-row compacto">

        <a href="#" class="icon-btn compacto" id="btnSubirComplementos" data-bs-toggle="modal" data-bs-target="#modalComplementos" title="Subir PDFs Complementos">
          <i class="bi bi-cloud-upload"></i>
        </a>


        <a href="#" class="icon-btn compacto" data-bs-toggle="modal" data-bs-target="#modalVerComplementos" title="Ver Complementos">
          <i class="bi bi-eye"></i>
        </a>
      </div>
    </div>

  </div>
</div>

<!-- ========== MODAL: Complementos (3 archivos PDF) ========== -->
<div class="modal fade" id="modalComplementos" tabindex="-1" aria-labelledby="lblComplementos" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header">
        <h5 id="lblComplementos" class="modal-title">Subir PDFs de Complementos</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body">
        <!-- INE TRANSITO -->
        <div class="card mb-3" id="cardIneTransito" runat="server">
          <div class="card-header d-flex justify-content-between align-items-center">
            <strong>1. INE TRANSITO</strong>
            <span id="badgeIneTransito" runat="server" class="badge bg-secondary">Sin archivo</span>
          </div>
          <div class="card-body">
            <asp:FileUpload ID="fuIneTransito" runat="server" CssClass="form-control" accept=".pdf" />
            <asp:Button ID="btnSubirIneTransito" runat="server" CssClass="btn btn-primary btn-sm mt-2"
              Text="Subir INE Transito" OnClick="btnSubirIneTransito_Click" UseSubmitBehavior="false" />
          </div>
        </div>

        <!-- TRANSITO ASEGURADORA -->
        <div class="card mb-3" id="cardTransitoAseg" runat="server">
          <div class="card-header d-flex justify-content-between align-items-center">
            <strong>2. TRANSITO ASEGURADORA</strong>
            <span id="badgeTransitoAseg" runat="server" class="badge bg-secondary">Sin archivo</span>
          </div>
          <div class="card-body">
            <asp:FileUpload ID="fuTransitoAseg" runat="server" CssClass="form-control" accept=".pdf" />
            <asp:Button ID="btnSubirTransitoAseg" runat="server" CssClass="btn btn-primary btn-sm mt-2"
              Text="Subir Transito Aseguradora" OnClick="btnSubirTransitoAseg_Click" UseSubmitBehavior="false" />
          </div>
        </div>

        <!-- COMPLE -->
        <div class="card mb-3" id="cardComple" runat="server">
          <div class="card-header d-flex justify-content-between align-items-center">
            <strong>3. COMPLE</strong>
            <span id="badgeComple" runat="server" class="badge bg-secondary">Sin archivo</span>
          </div>
          <div class="card-body">
            <asp:FileUpload ID="fuComple" runat="server" CssClass="form-control" accept=".pdf" />
            <asp:Button ID="btnSubirComple" runat="server" CssClass="btn btn-primary btn-sm mt-2"
              Text="Subir Comple" OnClick="btnSubirComple_Click" UseSubmitBehavior="false" />
          </div>
        </div>

        <asp:Label ID="lblComplementosMsg" runat="server" CssClass="small" Visible="False"></asp:Label>
      </div>
     <div class="modal-footer">
    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
        Cerrar
    </button>
      </div>
    </div>
  </div>
</div>

<!-- ========== MODAL: Ver Complementos (3 archivos PDF) ========== -->
<div class="modal fade" id="modalVerComplementos" tabindex="-1" aria-labelledby="lblVerComplementos" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header">
        <h5 id="lblVerComplementos" class="modal-title">Ver PDFs de Complementos</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body">
        <!-- INE TRANSITO -->
        <div class="card mb-3">
          <div class="card-header d-flex justify-content-between align-items-center">
            <strong>1. INE TRANSITO</strong>
            <span id="badgeVerIneTransito" runat="server" class="badge bg-secondary">Sin archivo</span>
          </div>
          <div class="card-body text-center">
            <asp:LinkButton ID="btnVerIneTransitoPdf" runat="server" CssClass="btn btn-outline-primary btn-sm"
              ToolTip="Ver INE Transito" OnClick="btnVerIneTransito_Click">
              <i class="bi bi-file-pdf me-1"></i> Ver PDF
            </asp:LinkButton>
          </div>
        </div>

        <!-- TRANSITO ASEGURADORA -->
        <div class="card mb-3">
          <div class="card-header d-flex justify-content-between align-items-center">
            <strong>2. TRANSITO ASEGURADORA</strong>
            <span id="badgeVerTransitoAseg" runat="server" class="badge bg-secondary">Sin archivo</span>
          </div>
          <div class="card-body text-center">
            <asp:LinkButton ID="btnVerTransitoAsegPdf" runat="server" CssClass="btn btn-outline-primary btn-sm"
              ToolTip="Ver Transito Aseguradora" OnClick="btnVerTransitoAseg_Click">
              <i class="bi bi-file-pdf me-1"></i> Ver PDF
            </asp:LinkButton>
          </div>
        </div>

        <!-- COMPLE -->
        <div class="card mb-3">
          <div class="card-header d-flex justify-content-between align-items-center">
            <strong>3. COMPLE</strong>
            <span id="badgeVerComple" runat="server" class="badge bg-secondary">Sin archivo</span>
          </div>
          <div class="card-body text-center">
            <asp:LinkButton ID="btnVerComplePdf" runat="server" CssClass="btn btn-outline-primary btn-sm"
              ToolTip="Ver Comple" OnClick="btnVerComple_Click">
              <i class="bi bi-file-pdf me-1"></i> Ver PDF
            </asp:LinkButton>
          </div>
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
      </div>
    </div>
  </div>
</div>

    <!-- ====== CONTENEDOR DIAGNÓSTICO (inicia oculto) ====== -->
   <div id="diagSection" class="d-none">
  <div class="d-flex justify-content-center mt-3">
    <button type="button" id="btnToggleStripDiag" class="btn-toggle-strip" data-target="#stripDiag" title="Mostrar/Ocultar documentos">
      <i class="bi bi-clipboard2-pulse"></i>
      PROCESO DE DIAGNOSTICO
      <i class="bi bi-chevron-down chev"></i>
    </button>
  </div>

  <div id="stripDiag" class="card-pane doc-strip compacto p-2 mt-4">
    <!-- === BARRA CENTRAL DE INICIO (centrada en el contenedor) === -->
    <div class="diag-inicio text-center mb-2">
      <i class="bi bi-play-circle"></i>
      <span class="meta-caption">Inicio diagnóstico:</span>
      <span class="meta-value">
        <asp:Label ID="lblDiagInicio" runat="server" Text="—" ClientIDMode="Static" />
      </span>
    </div>

    <div class="doc-strip-grid">
      <!-- === TILE MECÁNICA === -->
      <div id="tileMec" runat="server" ClientIDMode="Static" class="tile compacto">
        <div class="title text-center">Mecánica</div>

        <!-- Fin diagnóstico Mecánica (arriba y centrado) -->
        <div class="tile-top text-center">
          <i class="bi bi-flag"></i>
          <span class="meta-caption">Fin diagnóstico:</span>
          <span class="meta-value">
            <asp:Label ID="lblDiagFinMecanica" runat="server" Text="—" ClientIDMode="Static" />
          </span>
        </div>

        <div class="d-flex align-items-center justify-content-center gap-2">
          <div id="flagMec" runat="server" ClientIDMode="Static" class="diag-flag off">
            <asp:CheckBox ID="chkMecSi" runat="server" ClientIDMode="Static" CssClass="form-check-input" />
            <i id="icoMec" runat="server" ClientIDMode="Static" class="bi bi-toggle-off fs-4" aria-hidden="true"></i>
          </div>
          <asp:LinkButton ID="btnDiagnosticoMecanica" runat="server" CssClass="icon-btn compacto"
            ToolTip="Abrir módulo de diagnóstico mecánico"
            UseSubmitBehavior="false"
            OnClientClick="openDiagPage('Mecanica.aspx'); return false;"
            aria-label="Diagnóstico Mecánica">
            <i class="bi bi-wrench-adjustable"></i>
          </asp:LinkButton>
        </div>
      </div>

      <!-- === TILE COLISIÓN === -->
      <div id="tileCol" runat="server" ClientIDMode="Static" class="tile compacto">
        <div class="title text-center">Colisión</div>

        <!-- Fin diagnóstico Colisión (arriba y centrado) -->
        <div class="tile-top text-center">
          <i class="bi bi-flag"></i>
          <span class="meta-caption">Fin diagnóstico:</span>
          <span class="meta-value">
            <asp:Label ID="lblDiagFinColision" runat="server" Text="—" ClientIDMode="Static" />
          </span>
        </div>

        <div class="d-flex align-items-center justify-content-center gap-2">
          <div id="flagHoja" runat="server" ClientIDMode="Static" class="diag-flag off">
            <asp:CheckBox ID="chkHojaSi" runat="server" ClientIDMode="Static" CssClass="form-check-input" />
            <i id="icoHoja" runat="server" ClientIDMode="Static" class="bi bi-toggle-off fs-4" aria-hidden="true"></i>
          </div>
          <asp:LinkButton ID="btnDiagnosticoHojalateria" runat="server" CssClass="icon-btn compacto"
            ToolTip="Abrir módulo de diagnóstico de hojalatería"
            UseSubmitBehavior="false"
            OnClientClick="openDiagPage('Hojalateria.aspx'); return false;"
            aria-label="Diagnóstico Hojalatería">
            <i class="bi bi-hammer"></i>
          </asp:LinkButton>
        </div>
      </div>
    </div>
  </div>

    <asp:HiddenField ID="hidDiagSrc" runat="server" ClientIDMode="Static" />
  </div>

  <!-- ====== CONTENEDOR VALUACIÓN ====== -->
  <div id="valSection" class="d-none">
    <div class="d-flex justify-content-center mt-3">
      <button type="button" id="btnToggleStripVal" class="btn-toggle-strip" data-target="#stripVal" title="Mostrar/Ocultar valuación">
        <i class="bi bi-calculator"></i>
        PROCESO VALUACIÓN
        <i class="bi bi-chevron-down chev"></i>
      </button>
    </div>

    <div id="stripVal" class="card-pane doc-strip compacto p-2 mt-4">
      <!-- === FILA DE FECHAS === -->
      <div class="doc-strip-grid mb-2">
        <div class="tile compacto fecha-tile">
          <small class="text-muted d-block">Fecha Inicio Valuación</small>
          <asp:Label ID="lblFechaIniVal" runat="server" Text="—" ClientIDMode="Static" CssClass="fw-bold fecha-valor" />
        </div>
        <div class="tile compacto fecha-tile">
          <small class="text-muted d-block">Fecha límite envío</small>
          <asp:Label ID="lblFechaLimEnvVal" runat="server" Text="—" ClientIDMode="Static" CssClass="fw-bold fecha-valor" />
        </div>
        <div class="tile compacto fecha-tile">
          <small class="text-muted d-block">Fecha envío valuación</small>
          <asp:Label ID="lblFechaEnvVal" runat="server" Text="—" ClientIDMode="Static" CssClass="fw-bold fecha-valor" />
        </div>
        <div class="tile compacto fecha-tile">
          <small class="text-muted d-block">Fecha autorización</small>
          <asp:Label ID="lblFechaAutVal" runat="server" Text="—" ClientIDMode="Static" CssClass="fw-bold fecha-valor" />
        </div>
        <div class="tile compacto fecha-tile">
          <small class="text-muted d-block">Fecha límite autoriz.</small>
          <asp:Label ID="lblFechaLimAutVal" runat="server" Text="—" ClientIDMode="Static" CssClass="fw-bold fecha-valor" />
        </div>
      </div>

      <!-- === TILES DE DOCUMENTOS === -->
      <div class="doc-strip-grid">
        <!-- Hoja de trabajo sin autorizar -->
        <div id="tileHojaTrabajo" runat="server" class="tile compacto">
          <div class="title">Hoja trabajo sin autorizar</div>
          <div class="icon-row compacto">
            <asp:LinkButton ID="btnVerHojaTrabajo" runat="server" CssClass="icon-btn compacto" ToolTip="Ver hoja de trabajo" aria-label="Ver hoja de trabajo" OnClick="btnVerHojaTrabajo_Click">
              <i class="bi bi-eye"></i>
            </asp:LinkButton>
          </div>
        </div>

        <!-- Valuación sin autorizar PDF -->
        <div id="tileValSinAut" runat="server" class="tile compacto">
          <div class="title">Valuación sin autorizar</div>
          <div class="icon-row compacto">
            <a href="#" class="icon-btn compacto" id="btnSubirValSinAut" data-bs-toggle="modal" data-bs-target="#modalValSinAut" title="Subir valuación sin autorizar">
              <i class="bi bi-cloud-upload"></i>
            </a>
            <asp:LinkButton ID="btnVerValSinAut" runat="server" CssClass="icon-btn compacto" ToolTip="Ver valuación sin autorizar" aria-label="Ver valuación sin autorizar" OnClick="btnVerValSinAut_Click">
              <i class="bi bi-eye"></i>
            </asp:LinkButton>
          </div>
        </div>

        <!-- Valuación autorizada PDF -->
        <div id="tileValAutPdf" runat="server" class="tile compacto">
          <div class="title">Valuación autorizada PDF</div>
          <div class="icon-row compacto">
            <a href="#" class="icon-btn compacto" id="btnSubirValAutPdf" data-bs-toggle="modal" data-bs-target="#modalValAutPdf" title="Subir valuación autorizada PDF">
              <i class="bi bi-cloud-upload"></i>
            </a>
            <asp:LinkButton ID="btnVerValAutPdf" runat="server" CssClass="icon-btn compacto" ToolTip="Ver valuación autorizada PDF" aria-label="Ver valuación autorizada PDF" OnClick="btnVerValAutPdf_Click">
              <i class="bi bi-eye"></i>
            </asp:LinkButton>
          </div>
        </div>

        <!-- Hoja de trabajo autorizada -->
        <div id="tileHojaTrabajoAut" runat="server" class="tile compacto">
          <div class="title">Hoja trabajo autorizada</div>
          <div class="icon-row compacto">
            <a href="#" class="icon-btn compacto" id="btnSubirHojaTrabajoAut" title="Subir hoja de trabajo autorizada">
              <i class="bi bi-cloud-upload"></i>
            </a>
            <asp:LinkButton ID="btnVerHojaTrabajoAut" runat="server" CssClass="icon-btn compacto" ToolTip="Ver hoja de trabajo autorizada" aria-label="Ver hoja de trabajo autorizada">
              <i class="bi bi-eye"></i>
            </asp:LinkButton>
          </div>
        </div>

        <!-- Seguimiento a complementos -->
        <div id="tileSeguimientoCompl" runat="server" class="tile compacto">
          <div class="title">Seguimiento complementos</div>
          <div class="icon-row compacto">
            <a href="#" class="icon-btn compacto" id="btnSubirSeguimientoCompl" title="Subir seguimiento a complementos">
              <i class="bi bi-cloud-upload"></i>
            </a>
            <asp:LinkButton ID="btnVerSeguimientoCompl" runat="server" CssClass="icon-btn compacto" ToolTip="Ver seguimiento a complementos" aria-label="Ver seguimiento a complementos">
              <i class="bi bi-eye"></i>
            </asp:LinkButton>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Hidden helpers -->
  <asp:HiddenField ID="hidId" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hidCarpeta" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hidViewerSrc" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hidInvGruaSrc" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hidInvSrc" runat="server" ClientIDMode="Static" />

  <!-- ===================== MODAL: Subir INV GRÚA (PDF) ===================== -->
  <div class="modal fade" id="modalInvGrua" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-md modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Subir Inventario de Grúa (PDF)</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">Seleccione el archivo PDF</label>
            <asp:FileUpload ID="fuInvGruaPdf" runat="server" CssClass="form-control" ClientIDMode="Static" />
            <div class="form-text">
              Se guardará como <strong>invgrua.pdf</strong> en <em>carpetarel/1. DOCUMENTOS DE INGRESO</em>.
            </div>
          </div>
          <div id="invGruaStatus" class="small text-muted d-none"></div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
          <asp:LinkButton ID="btnUploadInvGruaGo" runat="server" CssClass="btn btn-primary" OnClick="btnUploadInvGruaGo_Click">Guardar PDF</asp:LinkButton>
        </div>
      </div>
    </div>
  </div>

  <!-- =============== MODAL: Ver INV GRÚA ================= -->
  <div class="modal fade" id="modalVerInvGrua" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width:98vw;">
      <div class="modal-content" style="min-height:90vh;">
        <div class="modal-header">
          <h5 class="modal-title">Inventario de Grúa</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body p-0" style="height: calc(90vh - 60px);">
          <iframe id="invGruaFrame" style="width:100%;height:100%;border:0;"></iframe>
        </div>
        <div class="modal-footer">
          <% If Master.IsAdmin Then %>
            <asp:LinkButton ID="btnDeleteInvGrua" runat="server" CssClass="btn btn-danger"
              OnClientClick="return confirm('¿Eliminar el archivo actual (INV Grúa) definitivamente?');"
              OnClick="btnDeleteInvGrua_Click">
              <i class="bi bi-trash"></i> Eliminar PDF
            </asp:LinkButton>
          <% End If %>
          <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cerrar</button>
        </div>
      </div>
    </div>
  </div>

  <!-- ================ MODAL MULTIPLES PRESUP ================ -->
  <div class="modal fade" id="modalMultiplesPresup" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-fullscreen-md-down modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Cargar varias fotos de ingreso</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>

        <div class="modal-body">
          <p class="mb-2">
            Se guardarán como <code>presup1.jpg</code>, <code>presup2.jpg</code>, etc. en
            <code>3. FOTOS DE PRESUPUESTO</code>.
          </p>

          <!-- BLOQUE DE CÁMARA (oculto hasta activar) -->
          <div id="camBlockPresup" class="cam-block d-none">
            <div class="cam-body">
              <video id="camVideoPresup" autoplay playsinline muted></video>
              <img id="camPreviewPresup" alt="captura" />
              <canvas id="camCanvasPresup" style="display:none;"></canvas>
            </div>
            <div class="cam-footer d-flex justify-content-between align-items-center mt-2">
              <div class="d-flex gap-2">
                <button id="btnCambiarCamPresup" type="button" class="btn btn-outline-dark">Cambiar cámara</button>
              </div>
              <div class="d-flex align-items-center gap-2">
                <button id="btnRepetirPresup" type="button" class="btn btn-secondary d-none">Repetir</button>
                <button id="btnUsarPresup" type="button" class="btn btn-success d-none">Usar foto</button>
                <button id="btnCapturarPresup" type="button" class="btn btn-capture" title="Capturar">Capturar</button>
              </div>
            </div>
            <hr class="my-3">
          </div>

          <!-- Dropzone + input -->
          <div id="dropZonePresup" class="dnd-dropzone">
            <div class="dnd-icon"><i class="bi bi-cloud-arrow-up"></i></div>
            <div class="dnd-title">Arrastra tus imágenes aquí</div>
            <div class="dnd-sub">o haz click para seleccionarlas</div>
            <input id="fuMultiplesPresup" name="fuMultiplesPresup" type="file" accept="image/*" multiple class="d-none" />
          </div>

          <!-- Miniaturas -->
          <div id="thumbsPresup" class="mt-3 d-flex flex-wrap gap-2"></div>

          <div id="fotosPresupProgressWrap" class="d-none mt-3">
            <div class="progress">
              <div id="fotosPresupProgressBar" class="progress-bar progress-bar-striped" style="width:0%"
                   aria-valuenow="0" aria-valuenmin="0" aria-valuemax="100">0%</div>
            </div>
            <div id="fotosPresupStatus" class="mt-2 text-success fw-bold d-none">Fotos guardadas exitosamente</div>
          </div>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
          <button type="button" id="btnToggleCamPresup" class="btn btn-outline-primary">Abrir cámara</button>
          <asp:Button ID="btnGuardarMultiplesPresup" runat="server" Text="Guardar fotos ingreso"
            CssClass="btn btn-brand" OnClick="btnGuardarMultiplesPresup_Click"
            Enabled="false" ClientIDMode="Static" />
        </div>
      </div>
    </div>
  </div>

  <!-- Modal: galería Fotos -->
 <div class="modal fade" id="fotosModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width:95vw;">
    <div class="modal-content">
      <div class="modal-header">
        <div>
          <h5 class="modal-title">Galería de fotos</h5>
          <div class="small text-muted">Selecciona y descarga en .zip</div>
        </div>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>

      <div class="modal-body">
        <div class="d-flex flex-wrap gap-2 mb-3">
          <button type="button" class="btn btn-ghost btn-sm" onclick="toggleAllFotos(true)">Seleccionar todas</button>
          <button type="button" class="btn btn-ghost btn-sm" onclick="toggleAllFotos(false)">Deseleccionar todas</button>
          <button type="button" id="btnZipSel" class="btn btn-brand btn-sm" onclick="downloadSelectedZip()">Descargar seleccionadas (.zip)</button>
        </div>

        <div class="gallery-big mb-3 position-relative">
          <button type="button" class="gallery-nav-btn gallery-prev" aria-label="Anterior" title="Anterior">
            <i class="bi bi-chevron-left"></i>
          </button>

          <img id="galleryBigImg" alt="" />

          <button type="button" class="gallery-nav-btn gallery-next" aria-label="Siguiente" title="Siguiente">
            <i class="bi bi-chevron-right"></i>
          </button>

          <!-- Barra de zoom -->
          <div class="zoom-toolbar">
            <button type="button" class="zoom-btn" data-zoom="out" title="Alejar">−</button>
            <input type="range" id="zoomRange" min="1" max="6" step="0.1" value="1" />
            <button type="button" class="zoom-btn" data-zoom="in" title="Acercar">+</button>
            <button type="button" class="zoom-btn" data-zoom="reset" title="Restablecer">⟳</button>
            <button type="button" class="zoom-btn" data-zoom="fs" title="Pantalla completa">⛶</button>
          </div>
        </div>

        <div id="fotosGrid" class="gallery-grid">
          <asp:Literal ID="litFotosGrid" runat="server" />
        </div>
      </div>
    </div>
  </div>
</div>

<!-- Modal de pantalla completa con zoom -->
<div class="modal fade" id="fullscreenModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-fullscreen">
    <div class="modal-content" style="background: rgba(0,0,0,0.95);">
      <div class="modal-header border-0 position-absolute" style="top:10px; right:10px; z-index:10;">
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body d-flex align-items-center justify-content-center p-0" id="fsImageContainer" style="overflow:hidden;">
        <img id="fsImage" alt="Imagen en pantalla completa" style="max-width:90vw; max-height:85vh; object-fit:contain; transform-origin:center center; cursor:grab;" />
      </div>
      <div class="modal-footer border-0 justify-content-center position-absolute" style="bottom:10px; left:0; right:0;">
        <div class="d-flex gap-2 align-items-center bg-white rounded-pill px-3 py-2 shadow">
          <button type="button" class="btn btn-primary btn-sm rounded-circle" style="width:36px;height:36px;" id="fsZoomOut">−</button>
          <input type="range" id="fsZoomRange" min="1" max="6" step="0.1" value="1" style="width:120px;" />
          <button type="button" class="btn btn-primary btn-sm rounded-circle" style="width:36px;height:36px;" id="fsZoomIn">+</button>
          <button type="button" class="btn btn-secondary btn-sm rounded-circle" style="width:36px;height:36px;" id="fsZoomReset">⟳</button>
        </div>
      </div>
    </div>
  </div>
</div>
  <div class="modal fade" id="viewerModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width:95vw;">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Visor</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body" style="min-height:70vh;">
          <iframe id="viewerFrame" style="width:100%;height:70vh;border:0;" allow="autoplay"></iframe>
        </div>
        <div class="modal-footer">
          <% If Master.IsAdmin Then %>
            <asp:LinkButton ID="btnDeleteViewerFile" runat="server" CssClass="btn btn-danger"
              OnClientClick="return confirm('¿Eliminar el archivo actual definitivamente?');"
              OnClick="btnDeleteViewer_Click">
              <i class="bi bi-trash"></i> Eliminar archivo
            </asp:LinkButton>
          <% End If %>
          <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cerrar</button>
        </div>
      </div>
    </div>
  </div>

  <!-- Modal: CT (form + firmas) -->
  <div class="modal fade" id="modalCt" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Carta de Tránsito</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body">
          <div class="row g-3">
            <div class="col-12 col-md-6">
              <label class="form-label">Fecha</label>
              <asp:TextBox ID="txtCtFecha" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label">Siniestro</label>
              <asp:TextBox ID="txtCtSiniestro" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12 col-md-4">
              <label class="form-label">Marca</label>
              <asp:TextBox ID="txtCtMarca" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12 col-md-4">
              <label class="form-label">Versión</label>
              <asp:TextBox ID="txtCtVersion" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12 col-md-4">
              <label class="form-label">Año Modelo</label>
              <asp:TextBox ID="txtCtAnio" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label">Placas</label>
              <asp:TextBox ID="txtCtPlacas" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label">Teléfono</label>
              <asp:TextBox ID="txtCtTel" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label">Celular</label>
              <asp:TextBox ID="txtCtCel" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label">Correo electrónico</label>
              <asp:TextBox ID="txtCtCorreo" runat="server" CssClass="form-control" />
            </div>
          </div>
          <hr class="my-3" />
          <div class="row g-3">
            <div class="col-12 col-md-6">
              <label class="form-label">Firma del Cliente</label>
              <canvas id="sigCli" width="500" height="150" class="border rounded w-100"></canvas>
              <div class="mt-2 d-flex gap-2">
                <button type="button" class="btn btn-light btn-sm" onclick="clearCanvas('sigCli')">Borrar</button>
              </div>
              <asp:HiddenField ID="hfFirmaCliente" runat="server" ClientIDMode="Static" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label">Firma del Asesor</label>
              <canvas id="sigSup" width="500" height="150" class="border rounded w-100"></canvas>
              <div class="mt-2 d-flex gap-2">
                <button type="button" class="btn btn-light btn-sm" onclick="clearCanvas('sigSup')">Borrar</button>
              </div>
              <asp:HiddenField ID="hfFirmaSupervisor" runat="server" ClientIDMode="Static" />
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
          <asp:Button ID="btnCtGuardar" runat="server" Text="Guardar CT.pdf"
            CssClass="btn btn-brand"
            OnClientClick="return pushCtSignatures();"
            OnClick="btnCtGuardar_Click" />
        </div>
      </div>
    </div>
  </div>

  <!-- Modal: INE (2 fotos -> 1 PDF) -->
  <div class="modal fade" id="modalIneCamara" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">INE - Tomar/Subir 2 fotos</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body">
          <div class="row g-3">
            <div class="col-12 col-md-6">
              <label class="form-label">Frente</label>
              <asp:FileUpload ID="fuIneFront" runat="server" CssClass="form-control" ClientIDMode="Static" />
              <img id="prevIneFront" alt="" class="mt-2 w-100 border rounded" style="max-height:220px; object-fit:contain; display:none;" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label">Reverso</label>
              <asp:FileUpload ID="fuIneBack" runat="server" CssClass="form-control" ClientIDMode="Static" />
              <img id="prevIneBack" alt="" class="mt-2 w-100 border rounded" style="max-height:220px; object-fit:contain; display:none;" />
            </div>
          </div>
          <div id="ineProgressWrap" class="d-none mt-3">
            <div class="progress">
              <div id="ineProgressBar" class="progress-bar progress-bar-striped" style="width:0%" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">0%</div>
            </div>
            <div id="ineStatus" class="mt-2 text-success fw-bold d-none">INE.pdf generado</div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
          <asp:Button ID="btnIneCamaraGuardar" runat="server" Text="Guardar INE.pdf" CssClass="btn btn-brand" OnClick="btnIneCamaraGuardar_Click" Enabled="false" ClientIDMode="Static" />
        </div>
      </div>
    </div>
  </div>

  <!-- Modal: INV (HTML/PDF a pantalla grande) -->
  <div class="modal fade" id="invModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width:98vw;">
      <div class="modal-content" style="min-height:90vh;">
        <div class="modal-header">
          <h5 class="modal-title">Inventario</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body p-0" style="height: calc(90vh - 60px);">
          <iframe id="invFrame" style="width:100%;height:100%;border:0;"></iframe>
        </div>
        <div class="modal-footer">
          <% If Master.IsAdmin Then %>
            <asp:LinkButton ID="btnDeleteInv" runat="server" CssClass="btn btn-danger"
              OnClientClick="return confirm('¿Eliminar el archivo de inventario mostrado?');"
              OnClick="btnDeleteInv_Click">
              <i class="bi bi-trash"></i> Eliminar archivo
            </asp:LinkButton>
          <% End If %>
          <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cerrar</button>
        </div>
      </div>
    </div>
  </div>

  <!-- ===================== MODAL: Subir INE (PDF) ===================== -->
  <div class="modal fade" id="modalInePdf" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-md modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Subir INE (PDF)</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">Seleccione INE.pdf</label>
            <asp:FileUpload ID="fuInePdf" runat="server" CssClass="form-control" ClientIDMode="Static" />
            <div class="form-text">Se guardará como <strong>INE.pdf</strong> en <em>1. DOCUMENTOS DE INGRESO</em>.</div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
          <asp:LinkButton ID="btnUploadInePdfGo" runat="server" CssClass="btn btn-brand" UseSubmitBehavior="true" OnClick="btnUploadInePdfGo_Click">Subir PDF</asp:LinkButton>
        </div>
      </div>
    </div>
  </div>

  <!-- ===================== MODAL: Hoja de Trabajo Sin Autorizar ===================== -->
  <div class="modal fade" id="modalHojaTrabajo" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-fullscreen">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Hoja de Trabajo Sin Autorizar</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body">
          <!-- Encabezado con datos del auto e imagen -->
          <div class="row mb-4">
            <div class="col-md-8">
              <h6 class="fw-bold mb-3">Datos del Vehículo</h6>
              <table class="table table-sm table-bordered">
                <tr>
                  <th style="width:120px;">Expediente</th>
                  <td><asp:Label ID="lblHTExpediente" runat="server" /></td>
                  <th style="width:100px;">Año</th>
                  <td><asp:Label ID="lblHTAnio" runat="server" /></td>
                </tr>
                <tr>
                  <th>Marca</th>
                  <td><asp:Label ID="lblHTMarca" runat="server" /></td>
                  <th>Color</th>
                  <td><asp:Label ID="lblHTColor" runat="server" /></td>
                </tr>
                <tr>
                  <th>Modelo</th>
                  <td><asp:Label ID="lblHTModelo" runat="server" /></td>
                  <th>Placas</th>
                  <td><asp:Label ID="lblHTPlacas" runat="server" /></td>
                </tr>
              </table>
            </div>
            <div class="col-md-4 text-center">
              <asp:Image ID="imgHTPrincipal" runat="server" CssClass="img-fluid rounded" style="max-height:150px;" AlternateText="Imagen del vehículo" />
            </div>
          </div>

          <!-- GridViews de Mecánica -->
          <h6 class="fw-bold text-primary mb-2"><i class="bi bi-wrench"></i> Mecánica</h6>
          <div class="row mb-4">
            <div class="col-lg-6">
              <h6 class="text-muted">Reparación</h6>
              <asp:GridView ID="gvMecReparacion" runat="server" CssClass="table table-sm table-striped table-bordered ht-grid" AutoGenerateColumns="False" EmptyDataText="Sin registros" OnRowDataBound="gvHT_RowDataBound">
                <Columns>
                  <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="40px" ItemStyle-CssClass="text-center" />
                  <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                  <asp:BoundField DataField="observ1" HeaderText="Observaciones" />
                  <asp:TemplateField HeaderText="Si" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-si" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="1"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 1, "✓", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="No" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-no" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="0"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 0, "✗", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="P" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="P"><%# IIf(Convert.ToString(Eval("estatus")) = "P", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="E" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="E"><%# IIf(Convert.ToString(Eval("estatus")) = "E", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="D" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="D"><%# IIf(Convert.ToString(Eval("estatus")) = "D", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
            <div class="col-lg-6">
              <h6 class="text-muted">Sustitución</h6>
              <asp:GridView ID="gvMecSustitucion" runat="server" CssClass="table table-sm table-striped table-bordered ht-grid" AutoGenerateColumns="False" EmptyDataText="Sin registros" OnRowDataBound="gvHT_RowDataBound">
                <Columns>
                  <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="40px" ItemStyle-CssClass="text-center" />
                  <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                  <asp:BoundField DataField="numparte" HeaderText="Num. Parte" ItemStyle-Width="100px" />
                  <asp:TemplateField HeaderText="Si" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-si" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="1"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 1, "✓", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="No" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-no" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="0"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 0, "✗", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="P" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="P"><%# IIf(Convert.ToString(Eval("estatus")) = "P", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="E" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="E"><%# IIf(Convert.ToString(Eval("estatus")) = "E", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="D" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="D"><%# IIf(Convert.ToString(Eval("estatus")) = "D", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
          </div>

          <!-- GridViews de Hojalatería -->
          <h6 class="fw-bold text-warning mb-2"><i class="bi bi-tools"></i> Hojalatería</h6>
          <div class="row">
            <div class="col-lg-6">
              <h6 class="text-muted">Reparación</h6>
              <asp:GridView ID="gvHojReparacion" runat="server" CssClass="table table-sm table-striped table-bordered ht-grid" AutoGenerateColumns="False" EmptyDataText="Sin registros" OnRowDataBound="gvHT_RowDataBound">
                <Columns>
                  <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="40px" ItemStyle-CssClass="text-center" />
                  <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                  <asp:BoundField DataField="observ1" HeaderText="Observaciones" />
                  <asp:TemplateField HeaderText="Si" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-si" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="1"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 1, "✓", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="No" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-no" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="0"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 0, "✗", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="P" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="P"><%# IIf(Convert.ToString(Eval("estatus")) = "P", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="E" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="E"><%# IIf(Convert.ToString(Eval("estatus")) = "E", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="D" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="D"><%# IIf(Convert.ToString(Eval("estatus")) = "D", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
            <div class="col-lg-6">
              <h6 class="text-muted">Sustitución</h6>
              <asp:GridView ID="gvHojSustitucion" runat="server" CssClass="table table-sm table-striped table-bordered ht-grid" AutoGenerateColumns="False" EmptyDataText="Sin registros" OnRowDataBound="gvHT_RowDataBound">
                <Columns>
                  <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="40px" ItemStyle-CssClass="text-center" />
                  <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                  <asp:BoundField DataField="numparte" HeaderText="Num. Parte" ItemStyle-Width="100px" />
                  <asp:TemplateField HeaderText="Si" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-si" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="1"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 1, "✓", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="No" ItemStyle-Width="30px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-no" data-id='<%# Eval("id") %>' data-field="autorizado" data-val="0"><%# IIf(Convert.ToInt32(Eval("autorizado")) = 0, "✗", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="P" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="P"><%# IIf(Convert.ToString(Eval("estatus")) = "P", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="E" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="E"><%# IIf(Convert.ToString(Eval("estatus")) = "E", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="D" ItemStyle-Width="25px" ItemStyle-CssClass="text-center">
                    <ItemTemplate><span class="ht-toggle ht-status" data-id='<%# Eval("id") %>' data-field="estatus" data-val="D"><%# IIf(Convert.ToString(Eval("estatus")) = "D", "●", "") %></span></ItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
        </div>
      </div>
    </div>
  </div>

  <!-- ===================== MODAL: Subir Valuación Sin Autorizar (PDF) ===================== -->
  <div class="modal fade" id="modalValSinAut" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-md modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Subir Valuación Sin Autorizar</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">Seleccione el archivo PDF</label>
            <asp:FileUpload ID="fuValSinAut" runat="server" CssClass="form-control" ClientIDMode="Static" accept=".pdf" />
            <div class="form-text">Se guardará como <strong>valsin.pdf</strong> en <em>4. VALUACION</em>.</div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
          <asp:LinkButton ID="btnUploadValSinAut" runat="server" CssClass="btn btn-primary" OnClick="btnUploadValSinAut_Click">Subir PDF</asp:LinkButton>
        </div>
      </div>
    </div>
  </div>

  <!-- ===================== MODAL: Ver Valuación Sin Autorizar ===================== -->
  <div class="modal fade" id="modalVerValSinAut" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered">
      <div class="modal-content" style="height:90vh;">
        <div class="modal-header">
          <h5 class="modal-title">Valuación Sin Autorizar</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body p-0">
          <iframe id="iframeValSinAut" style="width:100%;height:100%;border:0;"></iframe>
        </div>
      </div>
    </div>
  </div>

  <!-- ===================== MODAL: Subir Valuación Autorizada (PDF) ===================== -->
  <div class="modal fade" id="modalValAutPdf" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-md modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Subir Valuación Autorizada</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">Seleccione el archivo PDF</label>
            <asp:FileUpload ID="fuValAutPdf" runat="server" CssClass="form-control" ClientIDMode="Static" accept=".pdf" />
            <div class="form-text">Se guardará como <strong>valaut.pdf</strong> en <em>4. VALUACION</em>.</div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
          <asp:LinkButton ID="btnUploadValAutPdf" runat="server" CssClass="btn btn-primary" OnClick="btnUploadValAutPdf_Click">Subir PDF</asp:LinkButton>
        </div>
      </div>
    </div>
  </div>

  <!-- ===================== MODAL: Ver Valuación Autorizada ===================== -->
  <div class="modal fade" id="modalVerValAutPdf" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered">
      <div class="modal-content" style="height:90vh;">
        <div class="modal-header">
          <h5 class="modal-title">Valuación Autorizada</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body p-0">
          <iframe id="iframeValAutPdf" style="width:100%;height:100%;border:0;"></iframe>
        </div>
      </div>
    </div>
  </div>

  <!-- ===================== MODAL: Diagnóstico (pantalla completa) ===================== -->
  <div class="modal fade" id="diagModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width:98vw;">
      <div class="modal-content" style="min-height:90vh;">
        <div class="modal-header">
          <h5 class="modal-title">Diagnóstico</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body p-0" style="height: calc(90vh - 60px);">
          <iframe id="diagFrame" style="width:100%;height:100%;border:0;"></iframe>
        </div>
        <div class="modal-footer">
          <% If Master.IsAdmin Then %>
            <asp:LinkButton ID="btnDeleteDiag" runat="server" CssClass="btn btn-danger"
              OnClientClick="return confirm('¿Acción administrativa en el diagnóstico?');"
              OnClick="btnDeleteDiag_Click">
              <i class="bi bi-shield-exclamation"></i> Acción admin (stub)
            </asp:LinkButton>
          <% End If %>
       <button type="button" id="btnCerrarHoja" class="btn btn-outline-secondary" data-bs-dismiss="modal">
  Cerrar
</button>

        </div>
      </div>
    </div>
  </div>

  <!-- ====== SCRIPTS ====== -->
  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

  <script>
      /* 1) Helpers Inventario (prefill + visor) */
      function getLblText(id) {
          const el = document.getElementById(id);
          return el ? (el.textContent || el.innerText || '').trim() : '';
      }
      function getInventarioData() {
          return {
              expediente: getLblText('lblExpediente'),
              siniestro: getLblText('lblSiniestro'),
              asegurado: getLblText('lblAsegurado'),
              telefono: getLblText('lblTelefono'),
              correo: getLblText('lblCorreo'),
              reporte: getLblText('lblReporte'),
              vehiculo: getLblText('lblVehiculo')
          };
      }
      function abrirInventarioPM() {
          const iframe = document.getElementById('invFrame');
          const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('invModal'));
          iframe.src = 'inventario.html';
          iframe.onload = () => {
              iframe.contentWindow.postMessage(
                  { type: 'INV_PREFILL', payload: getInventarioData() },
                  window.location.origin
              );
          };
          modal.show();
      }
      window.addEventListener('message', (e) => {
          if (e.origin !== window.location.origin) return;
          if (e.data?.type === 'INV_REQUEST') {
              const iframe = document.getElementById('invFrame');
              iframe?.contentWindow?.postMessage(
                  { type: 'INV_PREFILL', payload: getInventarioData() },
                  window.location.origin
              );
          }
      });
      function openInvViewer(src) {
          const f = document.getElementById('invFrame');
          f.src = src;
          const m = bootstrap.Modal.getOrCreateInstance(document.getElementById('invModal'));
          m.show();
          const hid = document.getElementById('hidInvSrc');
          if (hid) hid.value = src;
      }
  </script>

  <script>
      /* 2) INE (2 fotos -> 1 PDF) previews + botón */
      (function () {
          const fuFront = document.getElementById('fuIneFront');
          const fuBack = document.getElementById('fuIneBack');
          const prevF = document.getElementById('prevIneFront');
          const prevB = document.getElementById('prevIneBack');
          const btnSave = document.getElementById('btnIneCamaraGuardar');
          const wrap = document.getElementById('ineProgressWrap');
          const bar = document.getElementById('ineProgressBar');
          const msg = document.getElementById('ineStatus');

          function preview(input, imgEl) {
              const f = input?.files?.[0];
              if (!f || !f.type.startsWith('image/')) { imgEl.style.display = 'none'; imgEl.src = ''; return; }
              const r = new FileReader();
              r.onload = e => { imgEl.src = e.target.result; imgEl.style.display = 'block'; };
              r.readAsDataURL(f);
          }
          function updateBtn() {
              const ok = (fuFront?.files?.length || 0) + (fuBack?.files?.length || 0) > 0;
              if (btnSave) btnSave.disabled = !ok;
              if (ok && wrap) {
                  wrap.classList.remove('d-none');
                  if (bar) { bar.classList.add('progress-bar-animated'); bar.style.width = '30%'; bar.setAttribute('aria-valuenow', '30'); bar.textContent = '30%'; }
                  if (msg) msg.classList.add('d-none');
              }
          }
          fuFront?.addEventListener('change', function () { preview(this, prevF); updateBtn(); });
          fuBack?.addEventListener('change', function () { preview(this, prevB); updateBtn(); });
          document.getElementById('modalIneCamara')?.addEventListener('shown.bs.modal', function () {
              if (fuFront) fuFront.value = '';
              if (fuBack) fuBack.value = '';
              if (prevF) { prevF.src = ''; prevF.style.display = 'none'; }
              if (prevB) { prevB.src = ''; prevB.style.display = 'none'; }
              if (btnSave) btnSave.disabled = true;
              if (wrap) wrap.classList.add('d-none');
          });
      })();
  </script>

  <script>
      /* 3) Visor PDF genérico + Galería + ZIP */
      function openSmartViewer(src) {
          const f = document.getElementById('viewerFrame');
          f.src = src;
          const hid = document.getElementById('hidViewerSrc');
          if (hid) hid.value = src;
          const m = bootstrap.Modal.getOrCreateInstance(document.getElementById('viewerModal'));
          m.show();
      }

      let __galleryThumbs = [];
      let __galleryIndex = 0;

      function __rebuildGalleryState() {
          __galleryThumbs = Array.from(document.querySelectorAll('#fotosGrid .grid-thumb'));
          const big = document.getElementById('galleryBigImg');
          if (!big) return;
          const currentSrc = big.getAttribute('src') || '';
          const idx = __galleryThumbs.findIndex(t => (t.getAttribute('data-full') || t.src) === currentSrc);
          __galleryIndex = idx >= 0 ? idx : 0;
      }
      function __showGalleryAt(i) {
          if (__galleryThumbs.length === 0) return;
          if (i < 0) i = __galleryThumbs.length - 1;
          if (i >= __galleryThumbs.length) i = 0;
          __galleryIndex = i;
          const big = document.getElementById('galleryBigImg');
          const t = __galleryThumbs[__galleryIndex];
          const src = t.getAttribute('data-full') || t.src;
          if (big) big.src = src;
      }

      document.addEventListener('click', function (e) {
          const t = e.target;
          if (t && t.matches('.grid-thumb')) {
              __rebuildGalleryState();
              const idx = __galleryThumbs.indexOf(t);
              if (idx >= 0) __galleryIndex = idx;
              const big = document.getElementById('galleryBigImg');
              if (big) big.src = t.getAttribute('data-full') || t.src;
          }
          const btnPrev = e.target.closest?.('.gallery-prev');
          const btnNext = e.target.closest?.('.gallery-next');
          if (btnPrev) { __rebuildGalleryState(); __showGalleryAt(__galleryIndex - 1); }
          if (btnNext) { __rebuildGalleryState(); __showGalleryAt(__galleryIndex + 1); }
      });

      document.getElementById('fotosModal')?.addEventListener('shown.bs.modal', function () {
          __rebuildGalleryState();
          if (__galleryThumbs.length && !document.getElementById('galleryBigImg').src) {
              __showGalleryAt(0);
          }
      });

      function toggleAllFotos(sel) {
          document.querySelectorAll('#fotosGrid .grid-check').forEach(cb => cb.checked = !!sel);
      }
      function downloadSelectedZip() {
          const checks = Array.from(document.querySelectorAll('#fotosGrid .grid-check:checked'));
          if (checks.length === 0) { alert('Selecciona al menos una imagen.'); return; }
          const names = checks.map(cb => cb.getAttribute('data-name')).join('|');
          const form = document.createElement('form');
          form.method = 'POST';
          form.action = '<%= ResolveUrl("~/DownloadFotosZip.ashx") %>';
        const hid = document.createElement('input');
        hid.type = 'hidden'; hid.name = 'id'; hid.value = '<%= lblId.Text %>';
      form.appendChild(hid);
      const hn = document.createElement('input');
      hn.type = 'hidden'; hn.name = 'names'; hn.value = names;
      form.appendChild(hn);
      document.body.appendChild(form);
      form.submit();
      setTimeout(() => document.body.removeChild(form), 2000);
      }

  </script>

    <!-- Script de fullscreen nativo eliminado - ahora se usa modal -->


  <script>
    /* 4) Dropzone Principal (principal.jpg) con EXIF y resize */
    async function readOrientation(file) {
      return new Promise((resolve) => {
        const fr = new FileReader();
        fr.onload = function (e) {
          const view = new DataView(e.target.result);
          if (view.getUint16(0, false) !== 0xFFD8) return resolve(1);
          let offset = 2, length = view.byteLength;
          while (offset < length) {
            const marker = view.getUint16(offset, false); offset += 2;
            if (marker === 0xFFE1) {
              const exifLength = view.getUint16(offset, false); offset += 2;
              const exifStart = offset;
              if (view.getUint32(exifStart, false) !== 0x45786966) return resolve(1);
              const tiffOffset = exifStart + 6;
              const little = view.getUint16(tiffOffset, false) === 0x4949;
              const ifdOffset = view.getUint32(tiffOffset + 4, little);
              let dirStart = tiffOffset + ifdOffset;
              const entries = view.getUint16(dirStart, little); dirStart += 2;
              for (let i = 0; i < entries; i++) {
                const entry = dirStart + i * 12;
                const tag = view.getUint16(entry, little);
                if (tag === 0x0112) {
                  const val = view.getUint16(entry + 8, little);
                  return resolve(val || 1);
                }
              }
              return resolve(1);
            } else if ((marker & 0xFF00) !== 0xFF00) { break; }
            else { offset += view.getUint16(offset, false); }
          }
          resolve(1);
        };
        fr.readAsArrayBuffer(file.slice(0, 128 * 1024));
      });
    }
    async function imageToCanvasFixed(file) {
      const orientation = await readOrientation(file).catch(() => 1);
      const bitmap = await createImageBitmap(file).catch(() => new Promise((res, rej) => {
        const img = new Image(); img.onload = () => res(img); img.onerror = rej;
        img.src = URL.createObjectURL(file);
      }));
      const w = bitmap.width || bitmap.naturalWidth;
      const h = bitmap.height || bitmap.naturalHeight;
      let cw = w, ch = h;
      if (orientation === 6 || orientation === 8) { cw = h; ch = w; }
      const canvas = document.createElement('canvas');
      canvas.width = cw; canvas.height = ch;
      const ctx = canvas.getContext('2d');
      switch (orientation) {
        case 3: ctx.translate(cw, ch); ctx.rotate(Math.PI); break;
        case 6: ctx.translate(cw, 0); ctx.rotate(Math.PI / 2); break;
        case 8: ctx.translate(0, ch); ctx.rotate(-Math.PI / 2); break;
      }
      ctx.drawImage(bitmap, 0, 0, w, h);
      return canvas;
    }
    function canvasToJpegBlob(canvas, quality = 0.88) {
      return new Promise((resolve) => canvas.toBlob(b => resolve(b), 'image/jpeg', quality));
    }
    function bust(src) {
      if (!src) return '';
      if (src.startsWith('data:')) return src;
      const u = new URL(src, window.location.origin);
      u.searchParams.set('v', Date.now().toString());
      return u.pathname + u.search;
    }

    (function () {
      const zone = document.getElementById('dropZone');
      const input = document.getElementById('fileDrop');
      const img = document.getElementById('imgPreview');
      const delBtn = document.getElementById('btnEliminarPrincipal');
      if (!zone || !input || !img) return;

      input.setAttribute('accept', 'image/*');
      input.setAttribute('capture', 'environment');

      const uploadUrl = '<%= ResolveUrl("~/UploadPrincipal.ashx") %>';
      const expedienteId = '<%= lblId.Text %>';
      const carpetaRel  = '<%= hidCarpeta.Value %>';

      function setHover(on) {
        zone.style.borderColor = on ? 'var(--brand-500)' : 'var(--brand-300)';
        zone.style.background  = on ? 'linear-gradient(135deg, var(--brand-050), var(--neutral-050))'
                                    : 'linear-gradient(135deg, var(--neutral-050), var(--brand-050))';
      }
      async function uploadBlobAsPrincipal(blob) {
        const fd = new FormData();
        fd.append('id', expedienteId || '');
        fd.append('carpetaRel', carpetaRel || '');
        fd.append('file', blob, 'principal.jpg');

        document.querySelectorAll('.modal.show').forEach(m => bootstrap.Modal.getInstance(m)?.hide());
        const r = await fetch(uploadUrl, { method:'POST', body:fd });
        const text = await r.text();
        let j; try { j = JSON.parse(text); } catch { throw new Error('Respuesta no-JSON: ' + text); }
        if (!j.ok) throw new Error(j.msg || 'Fallo al guardar');
        return j.url || null;
      }
      async function processAndUpload(file) {
        if (!file?.type?.startsWith('image/')) { alert('Selecciona una imagen.'); return; }
        if (file.size > 10 * 1024 * 1024)      { alert('Máximo 10 MB.'); return; }
        try {
          const canvas = await imageToCanvasFixed(file);
          const MAX = 1600;
          let outCanvas = canvas;
          if (canvas.width > MAX || canvas.height > MAX) {
            const ratio = Math.min(MAX / canvas.width, MAX / canvas.height);
            const c2 = document.createElement('canvas');
            c2.width  = Math.round(canvas.width  * ratio);
            c2.height = Math.round(canvas.height * ratio);
            c2.getContext('2d').drawImage(canvas, 0, 0, c2.width, c2.height);
            outCanvas = c2;
          }
          const previewDataUrl = outCanvas.toDataURL('image/jpeg', 0.88);
          img.src = previewDataUrl;
          const blob = await canvasToJpegBlob(outCanvas, 0.88);
          const serverUrl = await uploadBlobAsPrincipal(blob);
          if (serverUrl) { img.src = bust(serverUrl); }
          if (delBtn) { delBtn.style.display = ''; delBtn.classList.remove('d-none'); }
        } catch (err) {
          console.error(err);
          alert('Error subiendo la imagen: ' + err.message);
        }
      }

      zone.addEventListener('click', () => input.click());
      zone.addEventListener('dragenter', e => { e.preventDefault(); setHover(true);  });
      zone.addEventListener('dragover',  e => { e.preventDefault(); setHover(true);  });
      zone.addEventListener('dragleave', e => { e.preventDefault(); setHover(false); });
      zone.addEventListener('drop', e => {
        e.preventDefault(); setHover(false);
        if (e.dataTransfer?.files?.length) processAndUpload(e.dataTransfer.files[0]);
      });
      input.addEventListener('change', function(){
        if (this.files && this.files.length) processAndUpload(this.files[0]);
        this.value='';
      });
    })();
  </script>

<script>
    /* Toggle Recepción + Diagnóstico con auto-activación */
    (function () {
        // ---------- Helpers de estado ----------
        function isBtnLikeDisabled(btn) {
            if (!btn) return true;
            const ariaDisabled = (btn.getAttribute('aria-disabled') || '').toLowerCase() === 'true';
            return btn.classList.contains('disabled')
                || btn.classList.contains('aspNetDisabled')
                || btn.hasAttribute('disabled')
                || ariaDisabled;
        }

        function isBtnEnabledById(selector, scope) {
            if (!scope) scope = document;
            const b = scope.querySelector(selector);
            if (!b) return false;
            return !isBtnLikeDisabled(b);
        }

        function areEnabled(selectors, scope) {
            return selectors.every(sel => isBtnEnabledById(sel, scope));
        }

        // ---------- Condición de diagnóstico ----------
        function getDiagOk() {
            if (typeof window.__forceDiagVisible === 'boolean') {
                return !!window.__forceDiagVisible;
            }
            if (window.__isTransito) {
                // TRANSITO: ODA + FOTOS + INE + CT
                return areEnabled(['#btnVerODA', '#btnVerFotosPresup', '#btnVerINE', '#btnVerCT']);
            } else {
                // PISO: ODA + FOTOS + INE + INV
                return areEnabled(['#btnVerODA', '#btnVerFotosPresup', '#btnVerINE', '#btnVerINV']);
            }
        }

        // ---------- Indicadores ----------
        function refreshIndicatorsFor(stripEl, btnEl) {
            if (!stripEl || !btnEl) return;
            let ok;
            if (stripEl.id === 'stripDiag') {
                ok = getDiagOk();
            } else if (stripEl.id === 'strip') {
                // Usar los valores del servidor si están disponibles
                if (window.__diagSrv) {
                    if (window.__isTransito) {
                        // TRANSITO: ODA + FOTOS + INE + CT
                        ok = window.__diagSrv.oda && window.__diagSrv.fotos && window.__diagSrv.ine && window.__diagSrv.ct;
                    } else {
                        // PISO: ODA + FOTOS + INE + INV (sin CT)
                        ok = window.__diagSrv.oda && window.__diagSrv.fotos && window.__diagSrv.ine && window.__diagSrv.inv;
                    }
                } else {
                    // Fallback desde DOM
                    if (window.__isTransito) {
                        ok = areEnabled(['#btnVerODA', '#btnVerFotosPresup', '#btnVerINE', '#btnVerCT']);
                    } else {
                        const baseOk = areEnabled(['#btnVerODA', '#btnVerFotosPresup', '#btnVerINE']);
                        const invOk = isBtnEnabledById('#btnVerINV') || isBtnEnabledById('#btnVerInvGrua');
                        ok = baseOk && invOk;
                    }
                }
            } else if (stripEl.id === 'stripVal') {
                // Verificar los 5 tiles de valuación
                ok = areEnabled([
                    '#btnVerHojaTrabajo',
                    '#btnVerValSinAut',
                    '#btnVerValAutPdf',
                    '#btnVerHojaTrabajoAut',
                    '#btnVerSeguimientoCompl'
                ]);
            } else {
                ok = eyesAllEnabled(stripEl);
            }
            stripEl.classList.toggle('alert-pulse', !ok);
            stripEl.classList.toggle('strip-danger', !ok);
            btnEl.classList.toggle('danger', !ok);
            btnEl.title = ok ? 'Todos los visores habilitados' : 'Faltan visores por habilitar';

            // Aplicar blink al botón
            if (stripEl.id === 'strip' || stripEl.id === 'stripVal') {
                btnEl.classList.toggle('blink-danger', !ok);
                btnEl.classList.toggle('blink-success', ok);
            }
        }

        function eyesAllEnabled(scopeEl) {
            if (!scopeEl) return true;
            const eyeIcons = scopeEl.querySelectorAll('.icon-btn i.bi-eye');
            if (eyeIcons.length === 0) return true;
            return Array.from(eyeIcons).every(icon => {
                const b = icon.closest('.icon-btn, a, button, .btn');
                return b && !isBtnLikeDisabled(b);
            });
        }

        // ---------- Animación colapsable ----------
        function measureScrollHeight(el) {
            if (!el) return 0;
            const wasHidden = el.offsetParent === null || getComputedStyle(el).display === 'none';
            if (!wasHidden) return el.scrollHeight;
            const chain = [];
            let n = el;
            while (n && (n === el || (n.offsetParent === null || getComputedStyle(n).display === 'none'))) {
                chain.push([n, n.style.display]);
                n.style.display = 'block';
                n = n.parentElement;
            }
            const h = el.scrollHeight;
            chain.forEach(([node, d]) => node.style.display = d || '');
            return h;
        }

        function scrollIntoViewWithOffset(el, offset = 90) {
            if (!el) return;
            const prefersReduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
            const rect = el.getBoundingClientRect();
            const targetTop = rect.top + window.pageYOffset - offset - Math.max(0, (window.innerHeight - rect.height) / 2);
            window.scrollTo({ top: Math.max(0, targetTop), behavior: prefersReduced ? 'auto' : 'smooth' });
        }

        function collapse(stripEl, btnEl, shouldCollapse) {
            if (!stripEl || !btnEl) return;
            if (shouldCollapse) {
                const h = measureScrollHeight(stripEl);
                stripEl.style.maxHeight = h + 'px';
                stripEl.getBoundingClientRect();
                stripEl.classList.add('is-collapsed');
                stripEl.style.maxHeight = '0px';
                btnEl.classList.add('collapsed');
            } else {
                const diagWrap = document.getElementById('diagSection');
                const isDiag = (stripEl.id === 'stripDiag');
                if (isDiag && diagWrap && diagWrap.classList.contains('d-none')) {
                    diagWrap.classList.remove('d-none');
                }
                stripEl.classList.remove('is-collapsed');
                const h = measureScrollHeight(stripEl);
                stripEl.style.maxHeight = h + 'px';
                stripEl.getBoundingClientRect();
                setTimeout(() => { stripEl.style.maxHeight = ''; }, 360);
                btnEl.classList.remove('collapsed');

                scrollIntoViewWithOffset(btnEl, 80);
                requestAnimationFrame(() => {
                    setTimeout(() => scrollIntoViewWithOffset(stripEl, 80), 80);
                });
            }
        }

        // ---------- Mostrar/ocultar diagnóstico ----------
        function refreshDiagVisibility() {
            const diagWrap = document.getElementById('diagSection');
            const diagBtn = document.getElementById('btnToggleStripDiag');
            const diagStrip = document.getElementById('stripDiag');
            if (!diagWrap) return;

            const ok = getDiagOk();
            if (ok) {
                diagWrap.classList.remove('d-none');
                if (diagBtn && diagStrip) refreshIndicatorsFor(diagStrip, diagBtn);
            } else {
                if (diagBtn && diagStrip && !diagStrip.classList.contains('is-collapsed')) {
                    collapse(diagStrip, diagBtn, true);
                }
                diagWrap.classList.add('d-none');
            }
        }

        // API pública para que el servidor fuerce
        window.setDiagStripVisible = function (show) {
            try {
                var wrap = document.getElementById('diagSection');
                var el = document.getElementById('stripDiag');
                if (wrap) wrap.classList.toggle('d-none', !show);
                if (el) {
                    el.classList.toggle('d-none', !show);
                    if (show) { el.classList.remove('is-collapsed'); el.style.maxHeight = ''; }
                }
                refreshDiagVisibility();
            } catch (e) { }
        };

        // ---------- Mostrar/ocultar valuación (visible cuando diagnóstico completo) ----------
        function refreshValVisibility() {
            const valWrap = document.getElementById('valSection');
            const valBtn = document.getElementById('btnToggleStripVal');
            const valStrip = document.getElementById('stripVal');
            if (!valWrap) return;

            // Mostrar valuación cuando ambos tiles de diagnóstico están en verde
            const tileMec = document.getElementById('tileMec');
            const tileCol = document.getElementById('tileCol');
            const chkMec = document.getElementById('chkMecSi');
            const chkHoja = document.getElementById('chkHojaSi');

            // Mostrar si: (tileMec verde Y chkMecSi) O (tileCol verde Y chkHojaSi)
            const mecOk = tileMec && tileMec.classList.contains('ok') && chkMec && chkMec.checked;
            const hojaOk = tileCol && tileCol.classList.contains('ok') && chkHoja && chkHoja.checked;

            if (mecOk || hojaOk) {
                valWrap.classList.remove('d-none');
                if (valBtn && valStrip) refreshIndicatorsFor(valStrip, valBtn);
            } else {
                if (valBtn && valStrip && !valStrip.classList.contains('is-collapsed')) {
                    collapse(valStrip, valBtn, true);
                }
                valWrap.classList.add('d-none');
            }
        }

        // Expone refresco de valuación
        window.__refreshValVisibility = refreshValVisibility;

        // Expone refresco para que el servidor pueda llamarlo
        window.__refreshDiagVisibility = refreshDiagVisibility;

        // ---------- Observadores sobre botones/tiles que disparan el refresco ----------
        const OBS_TARGETS = ['#btnVerODA', '#btnVerFotosPresup', '#btnVerINE', '#btnVerCT', '#btnVerINV', '#tileCT', '#tileINV', '#tileINE', '#tileODA', '#TileFotos'];
        function hookObservers() {
            const opts = { subtree: false, attributes: true, attributeFilter: ['class', 'disabled', 'aria-disabled'] };
            OBS_TARGETS.forEach(sel => {
                document.querySelectorAll(sel).forEach(el => {
                    if (el._diagObs) return;
                    const mo = new MutationObserver(() => refreshDiagVisibility());
                    mo.observe(el, opts);
                    el._diagObs = mo;
                });
            });
        }

        // ---------- Botón toggle genérico (si lo usas) ----------
        function initAllToggles() {
            document.querySelectorAll('.btn-toggle-strip').forEach(btn => {
                const sel = btn.getAttribute('data-target') || '#strip';
                const strip = document.querySelector(sel) || document.getElementById('strip');

                // Indicadores
                refreshIndicatorsFor(strip, btn);

                // Click
                if (!btn._hooked) {
                    btn.addEventListener('click', () => {
                        const isCollapsed = strip.classList.contains('is-collapsed');
                        collapse(strip, btn, !isCollapsed);
                    });
                    btn._hooked = true;
                }

                // Observa cambios en esa tira
                const mo = new MutationObserver(() => refreshIndicatorsFor(strip, btn));
                mo.observe(strip, { subtree: true, attributes: true, attributeFilter: ['class', 'disabled', 'aria-disabled'] });
            });

            hookObservers();
            refreshDiagVisibility();
        }

        // ---------- Auto-check (cuando los archivos llegan sin postback) ----------
        let __diagTimer = null;
        function ensureDiagTimer() {
            if (__diagTimer) return;
            __diagTimer = setInterval(() => {
                if (getDiagOk()) {
                    setDiagStripVisible(true);
                    clearInterval(__diagTimer);
                    __diagTimer = null;
                }
            }, 1500); // cada 1.5s mientras no se cumpla
        }

        document.addEventListener('DOMContentLoaded', function () {
            initAllToggles();
            ensureDiagTimer();
            refreshValVisibility();
        });
        window.addEventListener('pageshow', function () {
            initAllToggles();
            refreshDiagVisibility();
            refreshValVisibility();
            ensureDiagTimer();
        });

        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            try {
                const prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_endRequest(() => setTimeout(() => {
                    initAllToggles();
                    refreshDiagVisibility();
                    refreshValVisibility();
                    ensureDiagTimer();
                }, 60));
            } catch (e) { }
        }
    })();
</script>





  <script>
    /* 7) Diagnóstico: abrir páginas hijas + postMessage */
    function getExpedienteData() {
      function gt(id) { const el = document.getElementById(id); return el ? (el.textContent || el.innerText || '').trim() : ''; }
      return {
        id: gt('lblId'),
        carpeta: (document.getElementById('hidCarpeta')?.value || gt('lblCarpeta')),
        expediente: gt('lblExpediente'),
        siniestro: gt('lblSiniestro'),
        asegurado: gt('lblAsegurado'),
        telefono: gt('lblTelefono'),
        correo: gt('lblCorreo'),
        reporte: gt('lblReporte'),
        vehiculo: gt('lblVehiculo'),
        fechaCreacion: gt('lblFechaCreacion'),
        diasTranscurridos: gt('lblDiasTranscurridos')
      };
    }
    function openDiagPage(pageUrl) {
      // Destruir TODOS los modales (no solo los visibles) antes de abrir diagModal
      document.querySelectorAll('.modal').forEach(function(m) {
        if (m.id === 'diagModal') return; // No destruir el que vamos a abrir
        var instance = bootstrap.Modal.getInstance(m);
        if (instance) {
          instance.dispose();
        }
        m.classList.remove('show');
        m.style.display = '';
        m.removeAttribute('aria-modal');
        m.removeAttribute('role');
      });
      // Limpiar backdrops huérfanos
      document.querySelectorAll('.modal-backdrop').forEach(function(b) { b.remove(); });
      document.body.classList.remove('modal-open');
      document.body.style.overflow = '';
      document.body.style.paddingRight = '';

      const iframe = document.getElementById('diagFrame');
      const modalEl = document.getElementById('diagModal');
      const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

      const d = getExpedienteData();
      const qs = new URLSearchParams(d).toString();
      const finalUrl = pageUrl + '?' + qs;

      iframe.src = finalUrl;
      iframe.onload = () => {
        try {
          iframe.contentWindow.postMessage({ type: 'EXP_PREFILL', payload: d }, window.location.origin);
        } catch (e) { console.warn('postMessage falló:', e); }
      };

      modal.show();
      const hid = document.getElementById('hidDiagSrc');
      if (hid) hid.value = finalUrl;
    }
    window.addEventListener('message', (e) => {
      if (e.origin !== window.location.origin) return;
      if (e.data?.type === 'EXP_REQUEST') {
        const frame = document.getElementById('diagFrame');
        frame?.contentWindow?.postMessage({ type: 'EXP_PREFILL', payload: getExpedienteData() }, window.location.origin);
      }
    });
  </script>

  <script>
    // === Firmas: dibujar con mouse/touch ===
    (function () {
      function setupSigCanvas(id) {
        const c = document.getElementById(id);
        if (!c) return;
        const ctx = c.getContext('2d');
        ctx.lineWidth = 2.0;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';

        let drawing = false, lastX = 0, lastY = 0;

        function pos(e) {
          const r = c.getBoundingClientRect();
          if (e.touches && e.touches.length) {
            return { x: e.touches[0].clientX - r.left, y: e.touches[0].clientY - r.top };
          }
          return { x: e.clientX - r.left, y: e.clientY - r.top };
        }

        function start(e) { e.preventDefault(); drawing = true; const p = pos(e); lastX = p.x; lastY = p.y; }
        function move(e) {
          if (!drawing) return;
          const p = pos(e);
          ctx.beginPath();
          ctx.moveTo(lastX, lastY);
          ctx.lineTo(p.x, p.y);
          ctx.stroke();
          lastX = p.x; lastY = p.y;
        }
        function end() { drawing = false; }

        c.addEventListener('mousedown', start);
        c.addEventListener('mousemove', move);
        c.addEventListener('mouseup', end);
        c.addEventListener('mouseleave', end);

        c.addEventListener('touchstart', start, { passive: false });
        c.addEventListener('touchmove', move, { passive: false });
        c.addEventListener('touchend', end);
      }

      window.clearCanvas = function (id) {
        const c = document.getElementById(id);
        if (!c) return;
        const ctx = c.getContext('2d');
        ctx.clearRect(0, 0, c.width, c.height);
      };

      document.addEventListener('DOMContentLoaded', function () {
        setupSigCanvas('sigCli');
        setupSigCanvas('sigSup');
      });

      window.pushCtSignatures = function () {
        try {
          const c1 = document.getElementById('sigCli');
          const c2 = document.getElementById('sigSup');
          const hf1 = document.getElementById('hfFirmaCliente');
          const hf2 = document.getElementById('hfFirmaSupervisor');

          function isBlank(canvas) {
            const ctx = canvas.getContext('2d');
            const data = ctx.getImageData(0, 0, canvas.width, canvas.height).data;
            for (let i = 3; i < data.length; i += 4) { if (data[i] !== 0) return false; }
            return true;
          }

          if (hf1) hf1.value = (c1 && !isBlank(c1)) ? c1.toDataURL('image/png') : '';
          if (hf2) hf2.value = (c2 && !isBlank(c2)) ? c2.toDataURL('image/png') : '';
        } catch (e) { console.warn(e); }
        return true;
      };
    })();
  </script>

  <!-- ===== Pantalla completa en móvil para el modal de múltiples ===== -->
  <script>
    (() => {
      const modal = document.getElementById('modalMultiplesPresup');
      if (!modal) return;
      const dialog = modal.querySelector('.modal-dialog');

      function isHandheld() {
        return window.matchMedia('(pointer:coarse)').matches ||
               /Android|iPhone|iPad|iPod|IEMobile|Opera Mini/i.test(navigator.userAgent);
      }

      modal.addEventListener('show.bs.modal', () => {
        if (isHandheld() && dialog) dialog.classList.add('modal-fullscreen');
      });
      modal.addEventListener('hidden.bs.modal', () => {
        dialog?.classList.remove('modal-fullscreen');
      });
    })();
  </script>

  <!-- ===== FOTOS INGRESO: DnD + File Picker + limpieza + API para cámara ===== -->
  <script>
    (() => {
      const zone         = document.getElementById('dropZonePresup');
      const input        = document.getElementById('fuMultiplesPresup');
      const thumbs       = document.getElementById('thumbsPresup');
      const btnSave      = document.getElementById('btnGuardarMultiplesPresup');
      const progressWrap = document.getElementById('fotosPresupProgressWrap');
      const progressBar  = document.getElementById('fotosPresupProgressBar');
      const statusOk     = document.getElementById('fotosPresupStatus');
      const modal        = document.getElementById('modalMultiplesPresup');

      if (!zone || !input || !thumbs) return;
      if (input.dataset.hooked === '1') return;
      input.dataset.hooked = '1';

      function enableSave() { btnSave && (btnSave.disabled = !(input.files && input.files.length)); }
      function showProgress30() {
        if (!progressWrap || !progressBar) return;
        progressWrap.classList.remove('d-none');
        progressBar.classList.add('progress-bar-animated');
        progressBar.style.width = '30%';
        progressBar.setAttribute('aria-valuenow', '30');
        progressBar.textContent = '30%';
        statusOk?.classList.add('d-none');
      }
      function resetProgress() {
        if (!progressWrap || !progressBar) return;
        progressBar.classList.remove('progress-bar-animated');
        progressBar.style.width = '0%';
        progressBar.setAttribute('aria-valuenow', '0');
        progressBar.textContent = '0%';
        progressWrap.classList.add('d-none');
        statusOk?.classList.add('d-none');
      }

      // Acumulador de archivos
      let accumulatedFiles = [];
      let isUpdatingInput = false;

      function updateInputFromAccumulator() {
        isUpdatingInput = true;
        const dt = new DataTransfer();
        accumulatedFiles.forEach(f => dt.items.add(f));
        input.files = dt.files;
        isUpdatingInput = false;
      }

      function removeFileAtIndex(index) {
        accumulatedFiles.splice(index, 1);
        updateInputFromAccumulator();
        rebuildThumbs(accumulatedFiles);
        enableSave();
        if (accumulatedFiles.length === 0) resetProgress();
      }
      function rebuildThumbs(fileList) {
        thumbs.innerHTML = '';
        Array.from(fileList || []).forEach((file, i) => {
          if (!file.type || !file.type.startsWith('image/')) return;

          const wrap = document.createElement('div');
          wrap.className = 'thumb-wrap';

          const deleteBtn = document.createElement('span');
          deleteBtn.className = 'thumb-delete';
          deleteBtn.innerHTML = '&times;';
          deleteBtn.title = 'Eliminar';
          deleteBtn.onclick = (e) => {
            e.stopPropagation();
            removeFileAtIndex(i);
          };

          const img = document.createElement('img');
          img.className = 'thumb';
          img.alt = file.name || `presup${i + 1}.jpg`;
          img.src = URL.createObjectURL(file);
          img.onload = () => URL.revokeObjectURL(img.src);

          const name = document.createElement('div');
          name.className = 'thumb-name';
          name.textContent = file.name || `presup${i + 1}.jpg`;

          wrap.appendChild(deleteBtn);
          wrap.appendChild(img);
          wrap.appendChild(name);
          thumbs.appendChild(wrap);
        });
      }
      function filesFromDataTransfer(dt) {
        if (dt.items && dt.items.length) {
          const out = [];
          for (const it of dt.items) {
            if (it.kind === 'file') {
              const f = it.getAsFile();
              if (f) out.push(f);
            }
          }
          return out;
        }
        return Array.from(dt.files || []);
      }
      function appendFiles(newFiles) {
        Array.from(newFiles || []).forEach(f => {
          if (f && f.type && f.type.startsWith('image/')) {
            accumulatedFiles.push(f);
          }
        });
        updateInputFromAccumulator();
        rebuildThumbs(accumulatedFiles);
        enableSave();
        showProgress30();
      }

      // File picker - acumular archivos
      input.addEventListener('change', () => {
        if (isUpdatingInput) return; // Evitar duplicados por cambio programático
        const newFiles = Array.from(input.files || []);
        if (newFiles.length === 0) return;
        appendFiles(newFiles);
      });

      // DnD Global (evitar que el navegador abra la imagen)
      ['dragover','drop'].forEach(evt =>
        document.addEventListener(evt, e => e.preventDefault(), false)
      );

      // DnD zona
      zone.addEventListener('click', () => input.click());
      ['dragenter','dragover'].forEach(ev => zone.addEventListener(ev, e => {
        e.preventDefault();
        if (e.dataTransfer) e.dataTransfer.dropEffect = 'copy';
        zone.classList.add('dnd-over');
      }));
      ['dragleave','drop'].forEach(ev => zone.addEventListener(ev, e => {
        e.preventDefault();
        zone.classList.remove('dnd-over');
      }));
      zone.addEventListener('drop', e => {
        const files = filesFromDataTransfer(e.dataTransfer || {});
        if (files.length) appendFiles(files);
      });

      // Limpieza al cerrar
      modal?.addEventListener('hidden.bs.modal', () => {
        thumbs.innerHTML = '';
        accumulatedFiles = [];
        resetProgress();
        input.value = '';
        enableSave();
      });

      // API pública para la cámara
      window.__appendFotoIngreso = function(file) {
        appendFiles([file]);
      };

      // Detectar cuando las fotos se guardaron exitosamente (después del postback)
      if (statusOk && !statusOk.classList.contains('d-none')) {
        // Ocultar el status inmediatamente para evitar re-disparos
        statusOk.classList.add('d-none');

        // Cerrar modal si está abierto
        setTimeout(() => {
          const modalInstance = bootstrap.Modal.getInstance(modal);
          if (modalInstance) {
            modalInstance.hide();
          }
          alert('FOTOS GUARDADAS EXITOSAMENTE');
        }, 100);
      }
    })();
  </script>

  <!-- ===== CÁMARA: usa la API unificada para agregar fotos ===== -->
  <script>
      (() => {
          const $modal = document.getElementById('modalMultiplesPresup');
          const $video = document.getElementById('camVideoPresup');
          const $preview = document.getElementById('camPreviewPresup');
          const $canvas = document.getElementById('camCanvasPresup');

          const $btnToggle = document.getElementById('btnToggleCamPresup');
          const $btnFlip = document.getElementById('btnCambiarCamPresup');
          const $btnCap = document.getElementById('btnCapturarPresup');
          const $btnUsar = document.getElementById('btnUsarPresup');
          const $btnRep = document.getElementById('btnRepetirPresup');

          const $fuMain = document.getElementById('fuMultiplesPresup');

          let stream = null;
          let useBack = true;
          let camOpen = false;

          async function startCamera() {
              stopCamera();
              const constraints = {
                  audio: false,
                  video: {
                      facingMode: useBack ? { ideal: 'environment' } : { ideal: 'user' },
                      width: { ideal: 1920 }, height: { ideal: 1080 }
                  }
              };
              try {
                  stream = await navigator.mediaDevices.getUserMedia(constraints);
                  $video.srcObject = stream;
                  $video.style.display = 'block';
                  $preview.style.display = 'none';
                  $btnCap.classList.remove('d-none');
                  $btnUsar.classList.add('d-none');
                  $btnRep.classList.add('d-none');
              } catch (err) {
                  console.error('No se pudo abrir la cámara', err);
                  alert('No se pudo abrir la cámara. Revisa permisos (HTTPS/localhost) y otorga acceso.');
                  toggleCamera(false);
              }
          }
          function stopCamera() {
              if (stream) {
                  stream.getTracks().forEach(t => t.stop());
                  stream = null;
              }
          }
          async function takeSnapshot() {
              if (!$video.videoWidth || !$video.videoHeight) {
                  setTimeout(takeSnapshot, 40);
                  return;
              }
              const vw = $video.videoWidth, vh = $video.videoHeight;
              const MAX_SIDE = 2000;
              let tw = vw, th = vh;
              if (Math.max(vw, vh) > MAX_SIDE) {
                  const s = MAX_SIDE / Math.max(vw, vh);
                  tw = Math.round(vw * s);
                  th = Math.round(vh * s);
              }
              $canvas.width = tw; $canvas.height = th;
              const ctx = $canvas.getContext('2d');
              ctx.drawImage($video, 0, 0, tw, th);

              // Agregar foto directamente al preview sin paso intermedio
              const dataURL = $canvas.toDataURL('image/jpeg', 0.9);
              const blob = await (await fetch(dataURL)).blob();
              const index = ($fuMain.files?.length || 0) + 1;
              const file = new File([blob], `presup${index}.jpg`, { type: 'image/jpeg' });

              if (typeof window.__appendFotoIngreso === 'function') {
                  window.__appendFotoIngreso(file);
              }
          }
          async function useSnapshot() {
              const dataURL = $canvas.toDataURL('image/jpeg', 0.9);
              const blob = await (await fetch(dataURL)).blob();
              const index = ($fuMain.files?.length || 0) + 1;
              const file = new File([blob], `presup${index}.jpg`, { type: 'image/jpeg' });

              if (typeof window.__appendFotoIngreso === 'function') {
                  window.__appendFotoIngreso(file);
              }

              // preparar para otra foto
              $preview.style.display = 'none';
              $video.style.display = 'block';
              $btnCap.classList.remove('d-none');
              $btnUsar.classList.add('d-none');
              $btnRep.classList.add('d-none');
          }
          function repeatShot() {
              $preview.style.display = 'none';
              $video.style.display = 'block';
              $btnCap.classList.remove('d-none');
              $btnUsar.classList.add('d-none');
              $btnRep.classList.add('d-none');
          }
          async function flipCamera() {
              useBack = !useBack;
              await startCamera();
          }
          async function toggleCamera(force) {
              if (typeof force === 'boolean') camOpen = force; else camOpen = !camOpen;
              const block = document.getElementById('camBlockPresup');
              const dropZone = document.getElementById('dropZonePresup');

              if (camOpen) {
                  block?.classList.remove('d-none');
                  dropZone?.classList.add('d-none');
                  $btnToggle.textContent = 'Cerrar cámara';
                  await startCamera();
              } else {
                  stopCamera();
                  block?.classList.add('d-none');
                  dropZone?.classList.remove('d-none');
                  $btnToggle.textContent = 'Abrir cámara';
                  repeatShot();
              }
          }

          // Eventos
          $btnToggle.addEventListener('click', () => toggleCamera());
          $btnFlip.addEventListener('pointerup', flipCamera);
          $btnCap.addEventListener('pointerup', takeSnapshot);
          $btnUsar.addEventListener('pointerup', useSnapshot);
          $btnRep.addEventListener('pointerup', repeatShot);

          $modal.addEventListener('hidden.bs.modal', () => toggleCamera(false));
      })();
  </script>

    <script>
        (function () {
            function isVisibleEnabledLink(sel) {
                const el = document.querySelector(sel);
                if (!el) return false;
                // si es un <span> (LinkButton deshabilitado) => no cuenta
                if (el.tagName !== 'A') return false;
                // si tiene clase disabled o atributo disabled => no cuenta
                if (el.classList.contains('disabled') || el.hasAttribute('disabled')) return false;
                // si está oculto => no cuenta
                const st = getComputedStyle(el);
                if (st.display === 'none' || st.visibility === 'hidden') return false;
                if (el.offsetParent === null) return false;
                return true;
            }

            function setReady(tileId, linkSel) {
                const tile = document.getElementById(tileId);
                if (!tile) return;
                tile.classList.toggle('is-ready', isVisibleEnabledLink(linkSel));
            }

            function refreshTiles() {
                setReady('tileODA',      'a#<%= btnVerODA.ClientID %>');
      setReady('TileFotos',    'a#<%= btnVerFotosPresup.ClientID %>');
      setReady('tileINE',      'a#<%= btnVerINE.ClientID %>');
    setReady('tileCT',       'a#<%= btnVerCT.ClientID %>');
    // INV: verde si hay algún visor activo (inventario general o grúa)
    const okInv  = isVisibleEnabledLink('a#<%= btnVerINV.ClientID %>') ||
                   isVisibleEnabledLink('a#<%= btnVerInvGrua.ClientID %>');
                const invTile = document.getElementById('tileINV');
                if (invTile) invTile.classList.toggle('is-ready', okInv);
            }

            document.addEventListener('DOMContentLoaded', refreshTiles);

            // Re-evaluar después de cada postback parcial (UpdatePanel)
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                try {
                    const prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_endRequest(() => setTimeout(refreshTiles, 0));
                } catch (e) { }
            }
        })();
    </script>

    <script>
        (function () {
            const img = document.getElementById('galleryBigImg');
            const toolbar = document.querySelector('#fotosModal .zoom-toolbar');
            const zoomRange = document.getElementById('zoomRange');

            if (!img || !toolbar || !zoomRange) return;

            // Estado de zoom/pan
            let scale = 1, tx = 0, ty = 0;
            const MIN = 1, MAX = 6;
            let dragging = false, lastX = 0, lastY = 0;

            // Aplicar transform
            function apply() {
                img.style.transform = `translate(${tx}px, ${ty}px) scale(${scale})`;
                img.style.transformOrigin = 'center center';
                img.classList.toggle('zooming', scale > 1);
            }
            function reset() {
                scale = 1; tx = 0; ty = 0; dragging = false;
                img.classList.remove('dragging', 'zooming');
                img.style.transform = '';
                zoomRange.value = '1';
            }
            function clampPan() {
                // Limita el pan según el tamaño actual del contenedor
                const rect = img.getBoundingClientRect();
                const cont = img.parentElement.getBoundingClientRect();
                const vw = cont.width, vh = cont.height;
                const baseW = rect.width / scale, baseH = rect.height / scale;

                // Máximo desplazamiento permisible para seguir viendo bordes
                const maxX = Math.max(0, (baseW * scale - vw) / 2);
                const maxY = Math.max(0, (baseH * scale - vh) / 2);
                tx = Math.max(-maxX, Math.min(maxX, tx));
                ty = Math.max(-maxY, Math.min(maxY, ty));
            }

            // Escalados
            function setScaleAbs(val) {
                const newScale = Math.max(MIN, Math.min(MAX, val));
                if (newScale <= 1.0001) { tx = 0; ty = 0; }
                scale = newScale;
                clampPan(); apply();
                zoomRange.value = String(newScale);
            }
            function setScaleRelative(delta) {
                setScaleAbs(scale + delta);
            }

            // Slider
            zoomRange.addEventListener('input', () => setScaleAbs(parseFloat(zoomRange.value || "1")));

            // Botones
            toolbar.addEventListener('click', (e) => {
                const btn = e.target.closest('.zoom-btn');
                if (!btn) return;
                const act = btn.getAttribute('data-zoom');
                switch (act) {
                    case 'in': setScaleRelative(0.2); break;
                    case 'out': setScaleRelative(-0.2); break;
                    case 'reset': reset(); break;
                    case 'fs': {
                        // Abrir overlay de pantalla completa
                        openFullscreenOverlay(img.src);
                        break;
                    }
                }
            });

            // Rueda del mouse: zoom relativo al centro del contenedor
            img.addEventListener('wheel', (e) => {
                e.preventDefault();
                const factor = (e.deltaY > 0) ? -0.2 : 0.2;
                setScaleRelative(factor);
            }, { passive: false });

            // Pan con mouse
            img.addEventListener('mousedown', (e) => {
                if (scale <= 1) return;
                dragging = true;
                lastX = e.clientX; lastY = e.clientY;
                img.classList.add('dragging');
            });
            window.addEventListener('mousemove', (e) => {
                if (!dragging) return;
                tx += (e.clientX - lastX);
                ty += (e.clientY - lastY);
                lastX = e.clientX; lastY = e.clientY;
                clampPan(); apply();
            });
            window.addEventListener('mouseup', () => {
                if (dragging) { dragging = false; img.classList.remove('dragging'); }
            });

            // Touch: pinch + pan
            let tStartDist = 0, startScale = 1, tLastX = 0, tLastY = 0;
            function dist(p1, p2) { const dx = p1.clientX - p2.clientX, dy = p1.clientY - p2.clientY; return Math.hypot(dx, dy); }
            img.addEventListener('touchstart', (e) => {
                if (e.touches.length === 1) {
                    tLastX = e.touches[0].clientX; tLastY = e.touches[0].clientY;
                } else if (e.touches.length >= 2) {
                    tStartDist = dist(e.touches[0], e.touches[1]);
                    startScale = scale;
                }
                img.classList.toggle('zooming', scale > 1);
            }, { passive: false });

            img.addEventListener('touchmove', (e) => {
                e.preventDefault();
                if (e.touches.length === 1 && scale > 1) {
                    const x = e.touches[0].clientX, y = e.touches[0].clientY;
                    tx += (x - tLastX); ty += (y - tLastY);
                    tLastX = x; tLastY = y;
                    clampPan(); apply();
                } else if (e.touches.length >= 2) {
                    const d = dist(e.touches[0], e.touches[1]);
                    const newScale = Math.max(MIN, Math.min(MAX, startScale * (d / tStartDist)));
                    setScaleAbs(newScale);
                }
            }, { passive: false });

            // Doble click: abrir modal de pantalla completa
            img.addEventListener('dblclick', (e) => {
                e.preventDefault();
                if (typeof window.openFullscreenOverlay === 'function') {
                    window.openFullscreenOverlay(img.src);
                }
            });

            // Manejar cambios de fullscreen
            function onFullscreenChange() {
                const isFS = document.fullscreenElement || document.webkitFullscreenElement;
                if (isFS === img) {
                    img.classList.add('fs');
                    // Mantener el zoom actual en fullscreen
                    apply();
                } else {
                    img.classList.remove('fs');
                    // Resetear al salir de fullscreen
                    reset();
                }
            }
            document.addEventListener('fullscreenchange', onFullscreenChange);
            document.addEventListener('webkitfullscreenchange', onFullscreenChange);

            // Al abrir/cerrar el modal resetea el zoom para la siguiente foto
            document.getElementById('fotosModal')?.addEventListener('shown.bs.modal', () => {
                reset();
            });
            document.getElementById('fotosModal')?.addEventListener('hidden.bs.modal', () => {
                reset();
            });

            // Si cambias de imagen con las flechas, resetea zoom para evitar “arrastres”
            document.addEventListener('click', (e) => {
                if (e.target.closest?.('.gallery-prev') || e.target.closest?.('.gallery-next')) {
                    reset();
                }
            });
        })();
    </script>

    <!-- Modal de pantalla completa con zoom -->
    <script>
        (function() {
            const modal = document.getElementById('fullscreenModal');
            const fsImage = document.getElementById('fsImage');
            const fsZoomRange = document.getElementById('fsZoomRange');
            const fsZoomIn = document.getElementById('fsZoomIn');
            const fsZoomOut = document.getElementById('fsZoomOut');
            const fsZoomReset = document.getElementById('fsZoomReset');

            if (!modal || !fsImage) return;

            let scale = 1, tx = 0, ty = 0;
            const MIN = 1, MAX = 6;
            let dragging = false, lastX = 0, lastY = 0;

            function apply() {
                fsImage.style.transform = `translate(${tx}px, ${ty}px) scale(${scale})`;
                fsImage.style.cursor = scale > 1 ? 'move' : 'grab';
            }

            function reset() {
                scale = 1; tx = 0; ty = 0; dragging = false;
                fsImage.style.transform = '';
                fsImage.style.cursor = 'grab';
                if (fsZoomRange) fsZoomRange.value = '1';
            }

            function clampPan() {
                const rect = fsImage.getBoundingClientRect();
                const cont = fsImage.parentElement.getBoundingClientRect();
                const baseW = rect.width / scale, baseH = rect.height / scale;
                const maxX = Math.max(0, (baseW * scale - cont.width) / 2);
                const maxY = Math.max(0, (baseH * scale - cont.height) / 2);
                tx = Math.max(-maxX, Math.min(maxX, tx));
                ty = Math.max(-maxY, Math.min(maxY, ty));
            }

            function setScale(val) {
                scale = Math.max(MIN, Math.min(MAX, val));
                if (scale <= 1.0001) { tx = 0; ty = 0; }
                clampPan(); apply();
                if (fsZoomRange) fsZoomRange.value = String(scale);
            }

            // Abrir modal
            window.openFullscreenOverlay = function(src) {
                fsImage.src = src;
                reset();
                const bsModal = bootstrap.Modal.getOrCreateInstance(modal);
                bsModal.show();
            };

            // Reset al cerrar
            modal.addEventListener('hidden.bs.modal', reset);

            // Botones de zoom
            fsZoomIn?.addEventListener('click', () => setScale(scale + 0.5));
            fsZoomOut?.addEventListener('click', () => setScale(scale - 0.5));
            fsZoomReset?.addEventListener('click', reset);

            // Slider de zoom
            fsZoomRange?.addEventListener('input', () => setScale(parseFloat(fsZoomRange.value || "1")));

            // Zoom con rueda del mouse
            fsImage.addEventListener('wheel', (e) => {
                e.preventDefault();
                setScale(scale + (e.deltaY > 0 ? -0.3 : 0.3));
            }, { passive: false });

            // Pan con mouse
            fsImage.addEventListener('mousedown', (e) => {
                if (scale <= 1) return;
                dragging = true;
                lastX = e.clientX; lastY = e.clientY;
                fsImage.style.cursor = 'grabbing';
                e.preventDefault();
            });

            window.addEventListener('mousemove', (e) => {
                if (!dragging) return;
                tx += (e.clientX - lastX);
                ty += (e.clientY - lastY);
                lastX = e.clientX; lastY = e.clientY;
                clampPan(); apply();
            });

            window.addEventListener('mouseup', () => {
                if (dragging) {
                    dragging = false;
                    fsImage.style.cursor = scale > 1 ? 'move' : 'grab';
                }
            });

            // Doble click para zoom 2x
            fsImage.addEventListener('dblclick', (e) => {
                e.preventDefault();
                setScale(scale > 1 ? 1 : 2.5);
            });

            // Touch: pinch + pan
            let tStartDist = 0, startScale = 1, tLastX = 0, tLastY = 0;
            function dist(p1, p2) {
                return Math.hypot(p1.clientX - p2.clientX, p1.clientY - p2.clientY);
            }

            fsImage.addEventListener('touchstart', (e) => {
                if (e.touches.length === 1) {
                    tLastX = e.touches[0].clientX;
                    tLastY = e.touches[0].clientY;
                } else if (e.touches.length >= 2) {
                    tStartDist = dist(e.touches[0], e.touches[1]);
                    startScale = scale;
                }
            }, { passive: true });

            fsImage.addEventListener('touchmove', (e) => {
                e.preventDefault();
                if (e.touches.length === 1 && scale > 1) {
                    const x = e.touches[0].clientX, y = e.touches[0].clientY;
                    tx += (x - tLastX); ty += (y - tLastY);
                    tLastX = x; tLastY = y;
                    clampPan(); apply();
                } else if (e.touches.length >= 2) {
                    const d = dist(e.touches[0], e.touches[1]);
                    setScale(startScale * (d / tStartDist));
                }
            }, { passive: false });
        })();
    </script>

    <script>
        /* === MEC/HOJA: UI, persistencia y bloqueo === */
        (function () {
            function setFlagUI(flagId, iconId, checked) {
                const flag = document.getElementById(flagId);
                const ico = document.getElementById(iconId);
                const txt = flag?.querySelector('.state');
                if (!flag || !ico || !txt) return;

                flag.classList.toggle('on', !!checked);
                flag.classList.toggle('off', !checked);
                ico.classList.toggle('bi-toggle-on', !!checked);
                ico.classList.toggle('bi-toggle-off', !checked);
                txt.textContent = checked ? 'Habilitado' : 'Deshabilitado';
            }

            function applyDiagGateUI() {
                const mec = document.getElementById('chkMecSi')?.checked;
                const hoja = document.getElementById('chkHojaSi')?.checked;
                setFlagUI('flagMec', 'icoMec', !!mec);
                setFlagUI('flagHoja', 'icoHoja', !!hoja);

                // Bloquear/permitir clic visualmente (pointer-events: none ya existe en .disabled)
                const lnkM = document.querySelector('a#<%= btnDiagnosticoMecanica.ClientID %>');
                const lnkH = document.querySelector('a#<%= btnDiagnosticoHojalateria.ClientID %>');
                if(lnkM) lnkM.classList.toggle('disabled', !mec);
                if(lnkH) lnkH.classList.toggle('disabled', !hoja);

                // Actualizar estado del botón PROCESO DE DIAGNOSTICO y el contenedor
                const btnDiag = document.getElementById('btnToggleStripDiag');
                const stripDiag = document.getElementById('stripDiag');
                const tileMec = document.getElementById('tileMec');
                const tileCol = document.getElementById('tileCol');

                const mecOk = tileMec && tileMec.classList.contains('ok');
                const hojaOk = tileCol && tileCol.classList.contains('ok');

                // Si algún checkbox está activo pero su tile no está verde -> parpadear rojo
                const mecPending = mec && !mecOk;
                const hojaPending = hoja && !hojaOk;
                const anyPending = mecPending || hojaPending;

                // Si hay al menos un checkbox activo y todos los activos están verdes -> verde
                const anyActive = mec || hoja;
                const allActiveOk = (!mec || mecOk) && (!hoja || hojaOk) && anyActive;

                if (btnDiag) {
                    btnDiag.classList.toggle('blink-danger', anyPending);
                    btnDiag.classList.toggle('blink-success', allActiveOk && !anyPending);
                }

                if (stripDiag) {
                    stripDiag.classList.toggle('strip-danger', anyPending);
                }
            }

  function saveGate(area, enabled){
    const idTxt = (document.getElementById('lblId')?.textContent || document.getElementById('hidId')?.value || '0').trim();
    const admId = parseInt(idTxt, 10) || 0;
    if(!admId) return;

    // Si hay PageMethods, úsalo. Si no, fallback con fetch al WebMethod.
    if (window.PageMethods && typeof PageMethods.SetDiagGate === 'function'){
      PageMethods.SetDiagGate(admId, area, !!enabled,
        function(r){ if(!r || !r.ok){ console.warn('No se pudo guardar'); } },
        function(e){ alert('No se pudo guardar el estado.'); console.warn(e); }
      );
    }else{
      fetch('<%= ResolveUrl("~/Hoja.aspx/SetDiagGate") %>', {
                        method: 'POST', headers: { 'Content-Type': 'application/json; charset=utf-8' },
                        body: JSON.stringify({ admisionId: admId, area: area, enabled: !!enabled })
                    }).then(r => r.json()).catch(() => alert('No se pudo guardar el estado.'));
                }
            }

            // Listeners
            document.addEventListener('change', function (e) {
                if (e.target?.id === 'chkMecSi') { applyDiagGateUI(); saveGate('MEC', e.target.checked); }
                if (e.target?.id === 'chkHojaSi') { applyDiagGateUI(); saveGate('HOJA', e.target.checked); }
            });

            document.addEventListener('DOMContentLoaded', applyDiagGateUI);

            // Exponer globalmente para que pueda ser llamada desde otros scripts
            window.applyDiagGateUI = applyDiagGateUI;

            // Endurecemos openDiagPage: si está en rojo, no abrimos
            const __origOpenDiagPage = window.openDiagPage;
            window.openDiagPage = function (pageUrl) {
                const isMec = /Mecanica\.aspx$/i.test(pageUrl);
                const isHoja = /Hojalateria\.aspx$/i.test(pageUrl);
                const allowMec = !!document.getElementById('chkMecSi')?.checked;
                const allowHoja = !!document.getElementById('chkHojaSi')?.checked;
                if ((isMec && !allowMec) || (isHoja && !allowHoja)) {
                    alert('Este módulo está bloqueado (rojo). Activa el switch para continuar.');
                    return false;
                }
                if (typeof __origOpenDiagPage === 'function') return __origOpenDiagPage(pageUrl);
            };
        })();
    </script>

   <script>
       document.addEventListener('DOMContentLoaded', function () {
           const modalEl = document.getElementById('diagModal');
           if (!modalEl) return;
           modalEl.addEventListener('hidden.bs.modal', function () {
               // Limpiar backdrops huérfanos
               document.querySelectorAll('.modal-backdrop').forEach(function(b) { b.remove(); });
               // Limpiar body
               document.body.classList.remove('modal-open');
               document.body.style.overflow = '';
               document.body.style.paddingRight = '';
               // NO recargar la página - quedarse donde está
           });

           // (opcional) si el hijo avisa:
           window.addEventListener('message', function (e) {
               if (e.origin !== window.location.origin) return;
               if (e.data && e.data.type === 'MECA_UPDATED') {
                   // Actualizar el label de fin diagnóstico sin recargar
                   const lbl = document.getElementById('lblDiagFinMecanica');
                   if (lbl && e.data.finmec) {
                       lbl.textContent = e.data.finmec;
                   }
                   // Pintar el tile de verde (usar clase ok como los demás)
                   const tile = document.getElementById('tileMec');
                   if (tile && e.data.finmec) {
                       if (!tile.classList.contains('ok')) {
                           tile.classList.add('ok');
                       }
                   }
                   // Actualizar estado del botón de diagnóstico
                   if (typeof applyDiagGateUI === 'function') {
                       applyDiagGateUI();
                   }
                   // Refrescar visibilidad de valuación
                   if (typeof refreshValVisibility === 'function') {
                       refreshValVisibility();
                   }
               }
               if (e.data && e.data.type === 'HOJA_UPDATED') {
                   // Actualizar el label de fin diagnóstico sin recargar
                   const lbl = document.getElementById('lblDiagFinColision');
                   if (lbl && e.data.fincol) {
                       lbl.textContent = e.data.fincol;
                   }
                   // Pintar el tile de verde (usar clase ok como los demás)
                   const tile = document.getElementById('tileCol');
                   if (tile && e.data.fincol) {
                       if (!tile.classList.contains('ok')) {
                           tile.classList.add('ok');
                       }
                   }
                   // Actualizar estado visual del flag
                   const flag = document.getElementById('flagHoja');
                   const ico = document.getElementById('icoHoja');
                   const chk = document.getElementById('chkHojaSi');
                   if (flag) flag.className = 'diag-flag on';
                   if (ico) ico.className = 'bi bi-toggle-on fs-4';
                   if (chk) chk.checked = true;
                   // Actualizar estado del botón de diagnóstico
                   if (typeof applyDiagGateUI === 'function') {
                       applyDiagGateUI();
                   }
                   // Refrescar visibilidad de valuación
                   if (typeof refreshValVisibility === 'function') {
                       refreshValVisibility();
                   }
               }
           });
       });
   </script>

   <script>
       // Toggle handlers para Hoja de Trabajo
       document.addEventListener('click', function (e) {
           const toggle = e.target.closest('.ht-toggle');
           if (!toggle) {
               // Debug: ver si click llega pero no encuentra toggle
               if (e.target.closest('#modalHojaTrabajo')) {
                   console.log('Click en modal pero no en toggle:', e.target);
               }
               return;
           }

           console.log('Toggle encontrado:', toggle);

           const id = toggle.dataset.id;
           const field = toggle.dataset.field;
           const val = toggle.dataset.val;

           console.log('Datos:', { id, field, val });

           // Encontrar la fila
           const row = toggle.closest('tr');
           if (!row) {
               console.log('No se encontró la fila');
               return;
           }

           if (field === 'autorizado') {
               // Toggle Si/No - mutuamente excluyente
               const siSpan = row.querySelector('.ht-si');
               const noSpan = row.querySelector('.ht-no');

               if (val === '1') {
                   siSpan.textContent = '✓';
                   noSpan.textContent = '';
               } else {
                   siSpan.textContent = '';
                   noSpan.textContent = '✗';
               }
           } else if (field === 'estatus') {
               // Toggle P/E/D - mutuamente excluyente
               const statusSpans = row.querySelectorAll('.ht-status');
               statusSpans.forEach(span => {
                   span.textContent = span.dataset.val === val ? '●' : '';
               });
           }

           // Guardar en la base de datos
           fetch('UpdateRefaccion.ashx?id=' + id + '&field=' + field + '&val=' + val)
               .then(r => r.json())
               .then(data => {
                   if (!data.ok) {
                       console.error('Error al guardar:', data.error);
                   }
               })
               .catch(err => console.error('Error:', err));
       });
   </script>


</asp:Content>