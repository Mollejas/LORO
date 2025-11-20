<%@ Page Title="Hoja de Trabajo"
    Language="VB"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="false"
    CodeBehind="Hoja.aspx.vb"
    Inherits="DAYTONAMIO.Hoja" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <style>
    :root{
      --brand-900:#0b3a7e; --brand-700:#1558b3; --brand-050:#f5f9ff;
      --ink-900:#0f172a; --ink-600:#334155; --line:#e6eefc;
    }
    body{ background: linear-gradient(180deg, var(--brand-050), #ffffff 35%); }
    .page-header{ background:linear-gradient(90deg,var(--brand-900),var(--brand-700)); color:#fff; border-radius:14px; padding:18px 20px; box-shadow:0 8px 22px rgba(16,42,112,.25); }
    .page-header h3{ margin:0; font-weight:700; }
    .page-sub{ opacity:.9; font-size:.95rem; }
    .card-pane{ background:#fff; border:1px solid var(--line); border-radius:14px; box-shadow:0 10px 24px rgba(2,34,89,.06); }
    .card-pane .card-title{ color:var(--brand-900); font-weight:700; border-bottom:1px solid var(--line); padding-bottom:.5rem; margin-bottom:1rem; }
    .field-label{ color:var(--ink-600); font-weight:600; font-size:.9rem; }
    .value{ color:var(--ink-900); font-weight:600; }
    .img-frame{
      position:relative;
      width:100%; height:420px;
      background:linear-gradient(180deg,#f7faff,#fdfefe);
      border:1px dashed #cfe0ff; border-radius:12px;
      display:flex; align-items:center; justify-content:center;
      overflow:hidden;
    }
    .img-frame img{ max-width:100%; max-height:100%; object-fit:contain; display:block; }
    /* Botón eliminar (tache) */
    .img-delete{
      position:absolute; top:10px; right:10px;
      background:#fff; color:#dc3545; border:1px solid #e5e7eb;
      border-radius:999px; padding:6px 10px; line-height:1; font-weight:700;
      box-shadow:0 6px 16px rgba(0,0,0,.15);
    }
    .img-delete:hover{ background:#fff5f5; color:#b02a37; text-decoration:none; }
    .btn-brand{ background:var(--brand-700); border-color:var(--brand-700); color:#fff; font-weight:600; }
    .btn-brand:hover{ background:var(--brand-900); border-color:var(--brand-900); color:#fff; }
    .btn-ghost{ border:1px solid var(--brand-700); color:var(--brand-700); background:#fff; font-weight:600; }
    .btn-ghost:hover{ background:var(--brand-050); color:var(--brand-900); border-color:var(--brand-900); }
    #thumbs{ display:flex; flex-wrap:wrap; gap:10px; margin-top:10px; }
    .thumb{ width:110px; height:110px; object-fit:cover; border-radius:10px; border:1px solid var(--line); }
    .thumb-wrap{ display:flex; flex-direction:column; align-items:center; font-size:.75rem; width:110px; }
    .thumb-name{ margin-top:4px; text-align:center; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; width:100%; }
  </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="container py-4">
    <div class="page-header mb-4">
      <h3>Hoja de trabajo</h3>
      <div class="page-sub">Gestión de expediente e imágenes</div>
    </div>

    <div class="row g-4">
      <!-- IZQUIERDA: Imagen -->
      <div class="col-12 col-lg-5">
        <div class="p-3 card-pane">
          <div class="card-title h5">Imágenes del expediente</div>

          <div class="img-frame mb-3">
            <!-- Botón eliminar sobre la imagen -->
            <asp:LinkButton ID="btnEliminarPrincipal" runat="server" CssClass="img-delete"
                            OnClick="btnEliminarPrincipal_Click" Visible="false"
                            ToolTip="Eliminar imagen principal (principal.jpg)">✕</asp:LinkButton>
            <asp:Image ID="imgPreview" runat="server" AlternateText="Sin imagen" />
          </div>

          <div class="mb-3" id="divFile" runat="server">
            <label class="form-label">Selecciona una imagen</label>
            <asp:FileUpload ID="fuImagen" runat="server" CssClass="form-control" />
            <small class="text-muted">JPG/PNG/GIF/BMP/WEBP (recomendado &lt; 10 MB).</small>
          </div>

          <div class="d-flex flex-wrap gap-2" id="divBotonSubir" runat="server">
            <asp:Button ID="btnSubirGuardar" runat="server"
                        Text="Subir y guardar (principal.jpg)"
                        CssClass="btn btn-brand"
                        OnClick="btnSubirGuardar_Click" />
            <button type="button" class="btn btn-ghost" data-bs-toggle="modal" data-bs-target="#modalMultiples">
              Agregar varias
            </button>
          </div>
        </div>
      </div>

      <!-- DERECHA: Datos -->
      <div class="col-12 col-lg-7">
        <div class="p-3 card-pane">
          <div class="card-title h5">Datos del expediente</div>

          <div class="row g-3">
            <div class="col-12 col-md-6">
              <div class="field-label">Expediente</div>
              <div class="value"><asp:Label ID="lblExpediente" runat="server" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label">Siniestro</div>
              <div class="value"><asp:Label ID="lblSiniestro" runat="server" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label">Asegurado</div>
              <div class="value"><asp:Label ID="lblAsegurado" runat="server" /></div>
            </div>
            <div class="col-12 col-md-6">
              <div class="field-label">Teléfono</div>
              <div class="value"><asp:Label ID="lblTelefono" runat="server" /></div>
            </div>
            <div class="col-12">
              <div class="field-label">Correo</div>
              <div class="value"><asp:Label ID="lblCorreo" runat="server" /></div>
            </div>
            <div class="col-12">
              <div class="field-label">Vehículo</div>
              <div class="value"><asp:Label ID="lblVehiculo" runat="server" /></div>
            </div>
          </div>
        </div>

        <div class="mt-3 small text-muted">
          <span class="me-2">ID:</span><asp:Label ID="lblId" runat="server" />
          <span class="ms-3 me-2">Carpeta destino:</span><asp:Label ID="lblCarpeta" runat="server" />
        </div>
      </div>
    </div>

    <!-- Hidden helpers -->
    <asp:HiddenField ID="hidId" runat="server" />
    <asp:HiddenField ID="hidCarpeta" runat="server" />

    <!-- Modal: selección múltiple -->
    <div class="modal fade" id="modalMultiples" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Cargar varias fotos</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
          </div>
          <div class="modal-body">
            <p class="mb-2">Se guardarán como <code>recep1.jpg</code>, <code>recep2.jpg</code>, etc.</p>
            <input id="fuMultiples" name="fuMultiples" type="file" class="form-control" accept="image/*" multiple />
            <div id="thumbs"></div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
            <asp:Button ID="btnGuardarMultiples" runat="server" Text="Guardar fotos"
                        CssClass="btn btn-brand"
                        OnClick="btnGuardarMultiples_Click"
                        Enabled="false" />
          </div>
        </div>
      </div>
    </div>
  </div>

  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
  <script>
      // Preview miniaturas en el modal
      const inputMulti = document.getElementById('fuMultiples');
      const thumbs = document.getElementById('thumbs');
      const btnGuardarMultiples = document.getElementById('<%= btnGuardarMultiples.ClientID %>');

      inputMulti?.addEventListener('change', function () {
          thumbs.innerHTML = '';
          const files = this.files || [];
          btnGuardarMultiples.disabled = files.length === 0;

          Array.from(files).forEach(file => {
              if (!file.type.startsWith('image/')) return;
              const reader = new FileReader();
              reader.onload = function (e) {
                  const wrap = document.createElement('div');
                  wrap.className = 'thumb-wrap';
                  const img = document.createElement('img');
                  img.className = 'thumb';
                  img.src = e.target.result;
                  const name = document.createElement('div');
                  name.className = 'thumb-name';
                  name.textContent = file.name;
                  wrap.appendChild(img); wrap.appendChild(name);
                  thumbs.appendChild(wrap);
              };
              reader.readAsDataURL(file);
          });
      });
  </script>
</asp:Content>
