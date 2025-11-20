<%@ Page Title="Hoja de trabajo" Language="VB" MasterPageFile="~/Site1.Master" AutoEventWireup="false" CodeBehind="HojaDeTrabajo.aspx.vb" Inherits="DAYTONAMIO.HojaDeTrabajo" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
  <!-- Deja SOLO este Bootstrap si tu Master no ya incluye otro. Quita duplicados. -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
  <style>
    .doc-card{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:1rem}
    .doc-actions{display:inline-flex;align-items:center;gap:16px;border:1px solid #cbd5e1;border-radius:999px;padding:.5rem .9rem;background:#fff}
    .doc-actions i{font-size:1.6rem;cursor:pointer}
    .doc-title{font-weight:700;color:#0d47a1;margin-bottom:.5rem}
    .img-panel{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:12px;display:flex;justify-content:center;align-items:center;min-height:220px}
    .img-panel img{max-width:100%;max-height:200px;border-radius:8px}
    .visually-hidden{position:absolute;width:1px;height:1px;padding:0;margin:-1px;overflow:hidden;clip:rect(0,0,0,0);white-space:nowrap;border:0}
    .disabled i{opacity:.45;pointer-events:none}
    /* Refuerzo de stacking */
    .modal{ z-index: 20000; }
    .modal-backdrop{ z-index: 19990; }
  </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="container my-3" style="max-width:900px;">
    <h3 class="mb-3">Hoja de trabajo</h3>

    <asp:HiddenField ID="hidId" runat="server" />

    <!-- ===== Imagen principal ===== -->
    <div class="doc-card mb-3">
      <div class="doc-title">Imagen principal</div>
      <div class="img-panel">
        <asp:Image ID="imgPrincipal" runat="server" ImageUrl="~/Content/placeholder.png" AlternateText="principal" />
      </div>
      <div class="text-center mt-2">
        <asp:FileUpload ID="fupPrincipal" runat="server" CssClass="visually-hidden"
          onchange="__doPostBack('<%= btnSubirPrincipal.UniqueID %>', '')" />
        <div class="doc-actions">
          <i class="bi bi-upload text-success" title="Subir imagen principal" onclick="triggerUpload('<%= fupPrincipal.ClientID %>')"></i>
          <asp:LinkButton ID="btnSubirPrincipal" runat="server" OnClick="btnSubirPrincipal_Click" CssClass="visualmente-oculto">subir</asp:LinkButton>
          <asp:LinkButton ID="btnVerPrincipal" runat="server" OnClientClick="return verDoc(this);" ToolTip="Ver">
            <i class="bi bi-eye text-primary"></i>
          </asp:LinkButton>
        </div>
        <div><small>Se guarda como <b>principal.jpg</b> en <b>1. DOCUMENTOS DE INGRESO</b>.</small></div>
      </div>
    </div>

    <!-- ===== INE ===== -->
    <div class="doc-card mb-4">
      <div class="doc-title">INE</div>
      <div class="d-flex justify-content-center">
        <asp:FileUpload ID="fupINE" runat="server" CssClass="visually-hidden"
          onchange="__doPostBack('<%= btnSubirINE.UniqueID %>', '')" />
        <div class="doc-actions">
          <i class="bi bi-upload text-success" title="Subir INE" onclick="triggerUpload('<%= fupINE.ClientID %>')"></i>
          <asp:LinkButton ID="btnSubirINE" runat="server" OnClick="btnSubirINE_Click" CssClass="visualmente-oculto">subir</asp:LinkButton>

          <asp:LinkButton ID="btnDescargarINE" runat="server" OnClick="btnDescargarINE_Click" ToolTip="Descargar INE"
                          UseSubmitBehavior="false" CausesValidation="false">
            <i class="bi bi-download text-primary"></i>
          </asp:LinkButton>

          <asp:LinkButton ID="btnVerINE" runat="server" OnClientClick="return verDoc(this);" ToolTip="Ver INE"
                          UseSubmitBehavior="false" CausesValidation="false">
            <i class="bi bi-eye text-primary"></i>
          </asp:LinkButton>
        </div>
      </div>
      <div class="text-center"><small>Se guarda como <b>INE.pdf</b> o <b>INE.jpg</b> en <b>1. DOCUMENTOS DE INGRESO</b>.</small></div>
    </div>

    <!-- Botón de PRUEBA para abrir modal (descártalo después) -->
    <div class="text-center mb-5">
      <button type="button" class="btn btn-outline-secondary" onclick="return openViewer('https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf','pdf')">
        Test: abrir modal con PDF dummy
      </button>
    </div>
  </div>

  <!-- Modales: colócalos dentro del form, se moverán a body en runtime -->
  <div class="modal fade" id="viewerModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width: 95vw;">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Vista de documento</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
        </div>
        <div class="modal-body" style="min-height:70vh;">
          <iframe id="viewerFrame" style="display:none;width:100%;height:70vh;border:0;"></iframe>
          <img id="viewerImg" style="display:none;max-width:100%;max-height:70vh;border-radius:8px;" />
        </div>
      </div>
    </div>
  </div>



    <!-- Botón de prueba SIN JS -->
<div class="text-center my-4">
  <button type="button" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#testModal">
    Probar modal simple
  </button>
</div>

<!-- Modal simple de prueba -->
<div class="modal fade" id="testModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Modal de prueba</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body">
        Si ves este contenido, Bootstrap está OK.
      </div>
    </div>
  </div>
</div>
     <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
    <script>

        document.addEventListener('DOMContentLoaded', function () {
            ['viewerModal'].forEach(function (id) {
                var el = document.getElementById(id);
                if (el && el.parentElement !== document.body) {
                    document.body.appendChild(el);
                }
            });

            // Si usas UpdatePanel, re-mover tras partial postback:
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    ['viewerModal'].forEach(function (id) {
                        var el = document.getElementById(id);
                        if (el && el.parentElement !== document.body) {
                            document.body.appendChild(el);
                        }
                    });
                });
            }
        });

        function triggerUpload(idInput) {
            var el = document.getElementById(idInput);
            if (el) { el.click(); }
        }

        function openViewer(url, type) {
            var frame = document.getElementById('viewerFrame');
            var img = document.getElementById('viewerImg');
            if (!url) { alert('No hay archivo para ver.'); return false; }
            var isPdf = (type === 'pdf') || url.toLowerCase().endsWith('.pdf');

            if (isPdf) {
                img.style.display = 'none';
                frame.src = url + (url.indexOf('#') > -1 ? '' : '#toolbar=1');
                frame.style.display = 'block';
            } else {
                frame.style.display = 'none';
                frame.src = 'about:blank';
                img.src = url;
                img.style.display = 'block';
            }

            var modalEl = document.getElementById('viewerModal');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();

            // limpiar iframe al cerrar para liberar PDF locks
            modalEl.addEventListener('hidden.bs.modal', function () {
                frame.src = 'about:blank';
                img.src = '';
            }, { once: true });

            return false;
        }

        function verDoc(btn) {
            var url = btn.getAttribute('data-url') || '';
            var type = btn.getAttribute('data-type') || 'img';
            return openViewer(url, type);
        }
    </script>


</asp:Content>


 
  
  