<%@ Page Title="Alta de Expediente QUALITAS"
    Language="VB"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="false"
    CodeBehind="AltaQua.aspx.vb"
    Inherits="DAYTONAMIO.AltaQua" %>
<%@ MasterType VirtualPath="~/Site1.Master" %>

<asp:Content ID="cHead" ContentPlaceHolderID="HeadContent" runat="server">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
  <style>
    :root {
      --primary: #10b981;
      --primary-dark: #059669;
      --primary-light: #d1fae5;
      --primary-glow: rgba(16, 185, 129, 0.15);
      --secondary: #3b82f6;
      --border-color: #e5e7eb;
      --text-body: #111827;
      --text-muted: #6b7280;
      --bg-light: #f9fafb;
      --shadow-sm: 0 2px 8px rgba(0, 0, 0, 0.04);
      --shadow-md: 0 8px 24px rgba(17, 24, 39, 0.08);
      --shadow-lg: 0 20px 40px rgba(0, 0, 0, 0.1);
    }

    #altaScope {
      font-size: 14px;
      color: var(--text-body);
      padding: 1rem 1rem;
      background: linear-gradient(135deg, #f0fdf4 0%, #ecfdf5 50%, #f9fafb 100%);
      min-height: 100vh;
      max-height: 100vh;
      overflow: hidden;
    }

    .content-card {
      background: #fff;
      border: 1px solid var(--border-color);
      border-radius: 20px;
      box-shadow: var(--shadow-lg);
      overflow: hidden;
      animation: slideUp 0.5s ease-out;
      height: calc(100vh - 2rem);
      display: flex;
      flex-direction: column;
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

    .card-header {
      background: linear-gradient(135deg, #ffffff 0%, #f9fafb 100%);
      border-bottom: 2px solid var(--primary-light);
      padding: 1rem 1.2rem;
      backdrop-filter: blur(10px);
      flex-shrink: 0;
    }

    .card-body {
      padding: 1rem;
      background: linear-gradient(to bottom, #ffffff 0%, #fafbfc 100%);
      flex: 1;
      overflow-y: auto;
      overflow-x: hidden;
    }

    .top-actions {
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
      margin-bottom: 0.75rem;
    }

    .btn-primary {
      background: linear-gradient(135deg, var(--primary) 0%, var(--primary-dark) 100%);
      border: none;
      padding: 0.5rem 1.2rem;
      font-weight: 600;
      border-radius: 10px;
      box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
      transition: all 0.3s ease;
      position: relative;
      overflow: hidden;
      font-size: 0.9rem;
    }

    .btn-primary::before {
      content: '';
      position: absolute;
      top: 0;
      left: -100%;
      width: 100%;
      height: 100%;
      background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
      transition: left 0.5s;
    }

    .btn-primary:hover::before {
      left: 100%;
    }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(16, 185, 129, 0.4);
    }

    .btn-primary:active {
      transform: translateY(0);
    }

    .btn-outline-secondary {
      border: 2px solid var(--border-color);
      background: white;
      color: var(--text-body);
      padding: 0.45rem 1.2rem;
      font-weight: 600;
      border-radius: 10px;
      transition: all 0.3s ease;
      font-size: 0.9rem;
    }

    .btn-outline-secondary:hover {
      background: var(--bg-light);
      border-color: var(--text-muted);
      transform: translateY(-2px);
      box-shadow: var(--shadow-sm);
    }

    .dz-wrap {
      display: flex;
      gap: 1rem;
      align-items: center;
      flex-wrap: wrap;
      background: linear-gradient(135deg, var(--primary-light) 0%, #ecfdf5 100%);
      border: 2px dashed var(--primary);
      padding: 0.75rem 1rem;
      border-radius: 16px;
      margin-bottom: 0.75rem;
      position: relative;
      overflow: hidden;
      transition: all 0.3s ease;
    }

    .dz-wrap::before {
      content: '';
      position: absolute;
      top: -50%;
      left: -50%;
      width: 200%;
      height: 200%;
      background: radial-gradient(circle, rgba(16, 185, 129, 0.1) 0%, transparent 70%);
      animation: pulse 3s ease-in-out infinite;
    }

    @keyframes pulse {
      0%, 100% { transform: scale(1); opacity: 0.5; }
      50% { transform: scale(1.1); opacity: 0.8; }
    }

    .dz-cta {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      color: #065f46;
      font-weight: 700;
      font-size: 0.95rem;
      position: relative;
      z-index: 1;
    }

    .dz-cta i {
      font-size: 1.3rem;
      animation: bounce 2s ease-in-out infinite;
    }

    @keyframes bounce {
      0%, 100% { transform: translateY(0); }
      50% { transform: translateY(-5px); }
    }

    .dz-zone {
      flex: 1 1 280px;
      min-height: 70px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: white;
      border: 2px dashed var(--border-color);
      border-radius: 12px;
      cursor: pointer;
      user-select: none;
      padding: 0.75rem 1rem;
      text-align: center;
      transition: all 0.3s ease;
      position: relative;
      z-index: 1;
    }

    .dz-zone:hover {
      border-color: var(--primary);
      background: var(--primary-light);
      box-shadow: 0 0 0 4px var(--primary-glow);
      transform: scale(1.02);
    }

    .dz-zone.has-file {
      border-style: solid;
      background: linear-gradient(135deg, #f0fdf4 0%, #d1fae5 100%);
      border-color: #10b981;
      box-shadow: 0 4px 12px rgba(16, 185, 129, 0.2);
    }

    .dz-zone.has-file::after {
      content: '✓';
      position: absolute;
      top: 10px;
      right: 10px;
      width: 30px;
      height: 30px;
      background: var(--primary);
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: bold;
      animation: checkmark 0.5s ease-out;
    }

    @keyframes checkmark {
      0% { transform: scale(0) rotate(-180deg); }
      100% { transform: scale(1) rotate(0); }
    }

    .dz-title {
      font-weight: 700;
      color: #0b5f46;
      font-size: 0.9rem;
      margin-bottom: 0.2rem;
    }

    .dz-sub {
      font-size: 0.8rem;
      color: var(--text-muted);
    }

    .form-section {
      background: rgba(255, 255, 255, 0.9);
      backdrop-filter: blur(10px);
      border: 1px solid rgba(229, 231, 235, 0.8);
      border-radius: 16px;
      padding: 0.9rem;
      height: 100%;
      box-shadow: var(--shadow-md);
      transition: all 0.3s ease;
      position: relative;
      overflow: hidden;
    }

    .form-section::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      height: 4px;
      background: linear-gradient(90deg, var(--primary) 0%, var(--secondary) 100%);
      transform: scaleX(0);
      transition: transform 0.3s ease;
    }

    .form-section:hover {
      box-shadow: 0 12px 32px rgba(17, 24, 39, 0.12);
      transform: translateY(-2px);
    }

    .form-section:hover::before {
      transform: scaleX(1);
    }

    .form-section-header {
      font-weight: 700;
      font-size: 0.95rem;
      color: var(--primary-dark);
      margin-bottom: 0.6rem;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid var(--primary-light);
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .form-section-header::before {
      content: '';
      width: 8px;
      height: 8px;
      background: var(--primary);
      border-radius: 50%;
      box-shadow: 0 0 8px var(--primary);
      animation: glow 2s ease-in-out infinite;
    }

    @keyframes glow {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.5; }
    }

    .form-label {
      font-weight: 600;
      color: #334155;
      margin-bottom: 0.2rem;
      text-align: left;
      font-size: 0.8rem;
      display: flex;
      align-items: center;
      gap: 0.25rem;
    }

    .form-control,
    .form-select {
      background: #fff;
      border: 2px solid var(--border-color);
      border-radius: 10px;
      padding: 0.35rem 0.6rem;
      transition: all 0.3s ease;
      font-size: 0.85rem;
    }

    .form-control:focus,
    .form-select:focus {
      border-color: var(--primary);
      background: var(--primary-light);
      box-shadow: 0 0 0 4px var(--primary-glow);
      outline: none;
    }

    .form-control::placeholder {
      color: #d1d5db;
    }

    .text-muted,
    small.text-muted {
      color: var(--text-muted);
      font-size: 0.75rem;
      margin-top: 0.2rem;
      display: block;
    }

    .form-check-input {
      width: 1.25rem;
      height: 1.25rem;
      border: 2px solid var(--border-color);
      border-radius: 6px;
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .form-check-input:checked {
      background-color: var(--primary);
      border-color: var(--primary);
      box-shadow: 0 0 0 4px var(--primary-glow);
    }

    .row.g-3 > [class*="col-"] {
      display: flex;
      flex-direction: column;
    }

    @media (max-width: 991px) {
      #altaScope {
        padding: 1rem 0.5rem;
      }

      .card-header {
        padding: 1rem;
      }

      .card-body {
        padding: 1rem;
      }

      .form-section {
        margin-bottom: 1rem;
      }
    }

    @media (max-width: 576px) {
      .top-actions {
        flex-direction: column;
      }

      .btn-primary,
      .btn-outline-secondary {
        width: 100%;
      }

      .dz-wrap {
        flex-direction: column;
      }

      .dz-cta {
        width: 100%;
        justify-content: center;
      }
    }

    .form-control:valid:not(:placeholder-shown),
    .form-select:valid {
      border-color: #10b981;
    }

    .btn {
      position: relative;
      overflow: hidden;
    }

    .btn::after {
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

    .btn:active::after {
      width: 300px;
      height: 300px;
    }
  </style>

  <script type="text/javascript">
    document.addEventListener('DOMContentLoaded', function () {
      var fup = document.getElementById('<%= fupPDF.ClientID %>');
      var dz = document.getElementById('dropPDF');
      var btn = document.getElementById('<%= btnTrigger.ClientID %>');

      var ddlTipo = document.getElementById('<%= ddlTipoIngreso.ClientID %>');
      var ddlStatus = document.getElementById('<%= ddlEstatus.ClientID %>');

      function schedulePostBack() {
        if (dz) dz.classList.add('has-file');
        if (btn) btn.click(); // postback completo SOLO al cargar PDF
      }

      // --- PDF DnD ---
      if (fup) {
        fup.addEventListener('change', function () {
          if (fup.files && fup.files.length > 0) {
            var file = fup.files[0];
            var isPdf = file && (file.type === 'application/pdf' || file.name.toLowerCase().endsWith('.pdf'));
            if (!isPdf) { alert('Solo se permiten archivos PDF.'); fup.value = ''; return; }
            schedulePostBack();
          }
        });
      }
      if (dz) {
        dz.addEventListener('click', function () { if (fup) fup.click(); });
        ['dragenter', 'dragover'].forEach(function (ev) {
          dz.addEventListener(ev, function (e) { e.preventDefault(); e.stopPropagation(); dz.classList.add('is-dragover'); });
        });
        ['dragleave', 'dragend', 'drop'].forEach(function (ev) {
          dz.addEventListener(ev, function (e) { e.preventDefault(); e.stopPropagation(); dz.classList.remove('is-dragover'); });
        });
        dz.addEventListener('drop', function (e) {
          var files = e.dataTransfer ? e.dataTransfer.files : null;
          if (files && files.length > 0 && fup) {
            try { var dt = new DataTransfer(); dt.items.add(files[0]); fup.files = dt.files; } catch (_) { }
            schedulePostBack();
          }
        });
      }

      // --- Estatus dinámico sin postback ni agregar/quitar options ---
      function selectValue(ddl, val) {
        var found = false;
        for (var i = 0; i < ddl.options.length; i++) {
          if (ddl.options[i].value === val) { ddl.selectedIndex = i; found = true; break; }
        }
        if (!found) ddl.selectedIndex = 0; // cae a vacío
      }
      function disableExcept(ddl, allowedValues) {
        for (var i = 0; i < ddl.options.length; i++) {
          var opt = ddl.options[i];
          var allowed = allowedValues.indexOf(opt.value) !== -1;
          opt.disabled = !allowed;
          opt.hidden = !allowed;
        }
        if (ddl.options[ddl.selectedIndex] && ddl.options[ddl.selectedIndex].disabled) {
          selectValue(ddl, allowedValues[0] || '');
        }
      }
      function setStatusForTipo() {
        if (!ddlTipo || !ddlStatus) return;

        // reset visual
        for (var i = 0; i < ddlStatus.options.length; i++) {
          ddlStatus.options[i].disabled = false;
          ddlStatus.options[i].hidden = false;
        }
        var t = (ddlTipo.value || '').toUpperCase();

        if (t === '') {
          selectValue(ddlStatus, '');
          ddlStatus.disabled = true;
        } else if (t === 'TRANSITO') {
          ddlStatus.disabled = false;
          disableExcept(ddlStatus, ['', 'TRANSITO']);
          selectValue(ddlStatus, 'TRANSITO');
        } else if (t === 'GRUA' || t === 'PROPIO IMPULSO') {
          ddlStatus.disabled = false;
          disableExcept(ddlStatus, ['', 'PISO']);
          selectValue(ddlStatus, 'PISO');
        } else {
          selectValue(ddlStatus, '');
          ddlStatus.disabled = true;
        }
      }

      if (ddlStatus) {
        selectValue(ddlStatus, '');
        ddlStatus.disabled = true;
      }
      if (ddlTipo) ddlTipo.addEventListener('change', setStatusForTipo);
      setStatusForTipo();
    });
  </script>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">
  <div id="altaScope" class="container-fluid">
    <div class="content-card">
      <div class="card-header">
        <div class="top-actions">
          <asp:Button ID="btnGuardar" runat="server"
                      Text="Guardar expediente"
                      CssClass="btn btn-primary"
                      OnClick="btnGuardar_Click" />

          <asp:Button ID="btnLimpiar" runat="server"
                      Text="Limpiar formulario"
                      CssClass="btn btn-outline-secondary"
                      UseSubmitBehavior="false"
                      OnClientClick="window.location.href='Alta.aspx'; return false;" />
        </div>

        <!-- Drag & drop -->
        <div class="dz-wrap">
          <div class="dz-cta">
            <i class="bi bi-cloud-arrow-up"></i>
            <span>Cargar volante de admisión (PDF)</span>
          </div>

          <div id="dropPDF" class="dz-zone" role="button" tabindex="0">
            <div>
              <div class="dz-title">Arrastra y suelta el PDF aquí</div>
              <div class="dz-sub">o haz clic para seleccionar el archivo</div>
            </div>
          </div>

          <asp:FileUpload ID="fupPDF" runat="server" CssClass="d-none" accept=".pdf,application/pdf" />
          <asp:Button ID="btnTrigger" runat="server" Text="trigger" OnClick="BtnTrigger_Click"
                      Style="display:none" UseSubmitBehavior="true" CausesValidation="false" />
        </div>
      </div>

      <div class="card-body">
        <div class="row g-3">
          <!-- Col 1: Generación de expediente + datos básicos de cliente -->
          <div class="col-12 col-lg-4">
            <div class="form-section h-100">
              <div class="form-section-header">Generación de expediente</div>
              <div class="row g-3">
                <div class="col-12">
                  <label class="form-label">N° de expediente (interno)</label>
                  <asp:TextBox ID="txtExpediente" runat="server" CssClass="form-control" />
                  <small class="text-muted">Consecutivo interno sugerido.</small>
                </div>
                <div class="col-12">
                  <label class="form-label">Creado por</label>
                  <asp:TextBox ID="txtCreadoPor" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Fecha de creación</label>
                  <asp:TextBox ID="txtFechaCreacion" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                </div>
                <div class="col-12">
                  <label class="form-label">N°. de siniestro (general)</label>
                  <asp:TextBox ID="txtSiniestroGen" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                  <label class="form-label">Tipo de ingreso del vehículo</label>
                  <asp:DropDownList ID="ddlTipoIngreso" runat="server" CssClass="form-select">
                    <asp:ListItem Text="" Value="" />
                    <asp:ListItem Text="TRANSITO" Value="TRANSITO" />
                    <asp:ListItem Text="GRUA" Value="GRUA" />
                    <asp:ListItem Text="PROPIO IMPULSO" Value="PROPIO IMPULSO" />
                  </asp:DropDownList>
                  <small class="text-muted">Equivalente a GRÚA / TRÁNSITO / PROPIO IMPULSO del volante.</small>
                </div>
                <div class="col-12">
                  <label class="form-label">Aplica deducible</label>
                  <asp:DropDownList ID="ddlDeducible" runat="server" CssClass="form-select">
                    <asp:ListItem Text="SI" Value="SI" />
                    <asp:ListItem Text="NO" Value="NO" />
                  </asp:DropDownList>
                </div>
                <div class="col-12">
                  <label class="form-label">Situación del vehículo</label>
                  <asp:DropDownList ID="ddlEstatus" runat="server" CssClass="form-select">
                    <asp:ListItem Text="" Value="" />
                    <asp:ListItem Text="PISO" Value="PISO" />
                    <asp:ListItem Text="TRANSITO" Value="TRANSITO" />
                  </asp:DropDownList>
                  <small class="text-muted">Se habilita según el tipo de ingreso (PISO / TRÁNSITO).</small>
                </div>

                <!-- Cliente (alineado con volante) -->
                <div class="col-12">
                  <label class="form-label">Nombre o razón social del cliente</label>
                  <asp:TextBox ID="txtAsegurado" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Teléfono del asegurado</label>
                  <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">E-mail del asegurado</label>
                  <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control text-lowercase" />
                </div>
              </div>
            </div>
          </div>

          <!-- Col 2: Identificación del siniestro (volante) -->
          <div class="col-12 col-lg-4">
            <div class="form-section h-100">
              <div class="form-section-header">Identificación del siniestro</div>
              <div class="row g-3">
                <div class="col-12">
                  <label class="form-label">Aseguradora / emisor</label>
                  <asp:TextBox ID="txtEmisor" runat="server" CssClass="form-control" />
                  <small class="text-muted">Ejemplo: QUÁLITAS COMPAÑÍA DE SEGUROS, S.A. DE C.V.</small>
                </div>
                <div class="col-12">
                  <label class="form-label">Folio electrónico</label>
                  <asp:TextBox ID="txtCarpeta" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">N°. de póliza</label>
                  <asp:TextBox ID="txtPoliza" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">N°. CIS</label>
                  <asp:TextBox ID="txtCIS" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                  <label class="form-label">N°. de siniestro</label>
                  <asp:TextBox ID="txtSiniestro" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">N°. de reporte</label>
                  <asp:TextBox ID="txtReporte" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Estado de cobranza</label>
                  <asp:TextBox ID="txtEstCobranza" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                  <label class="form-label">Fecha del siniestro</label>
                  <asp:TextBox ID="FchSiniestro" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Vigencia desde</label>
                  <asp:TextBox ID="txtVigenciaDesde" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Vigencia hasta</label>
                  <asp:TextBox ID="txtVigenciaHasta" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                  <label class="form-label">Nombre del ajustador</label>
                  <asp:TextBox ID="txtAjustador" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Clave del ajustador</label>
                  <asp:TextBox ID="txtClaveAjustador" runat="server" CssClass="form-control" />
                </div>
              </div>
            </div>
          </div>

          <!-- Col 3: Datos del vehículo (como en el volante) -->
          <div class="col-12 col-lg-4">
            <div class="form-section h-100">
              <div class="form-section-header">Datos del vehículo</div>
              <div class="row g-3">
                <div class="col-12">
                  <label class="form-label">Marca</label>
                  <asp:TextBox ID="txtMarca" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Tipo / submarca</label>
                  <asp:TextBox ID="txtTipo" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Modelo (año)</label>
                  <asp:TextBox ID="txtModelo" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Motor</label>
                  <asp:TextBox ID="txtMotor" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                  <label class="form-label">N°. de serie / VIN</label>
                  <asp:TextBox ID="txtSerie" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Placas</label>
                  <asp:TextBox ID="txtPlacas" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Color</label>
                  <asp:TextBox ID="txtColor" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Transmisión</label>
                  <asp:TextBox ID="txtTransmision" runat="server" CssClass="form-control" />
                  <small class="text-muted">Ejemplo: AUTOMÁTICA / MANUAL.</small>
                </div>

                <div class="col-12">
                  <label class="form-label">Kilometraje</label>
                  <asp:TextBox ID="txtKilometros" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                  <label class="form-label">Uso del vehículo</label>
                  <asp:TextBox ID="txtUso" runat="server" CssClass="form-control" />
                </div>

                <div id="rowPuertas2" runat="server" class="col-12 d-flex align-items-center">
                  <asp:CheckBox ID="chk2Puertas" runat="server"
                                Text="2 puertas"
                                CssClass="form-check-input me-2" />
                </div>
                <div id="rowPuertas4" runat="server" class="col-12 d-flex align-items-center">
                  <asp:CheckBox ID="chk4Puertas" runat="server"
                                Text="4 puertas"
                                CssClass="form-check-input me-2" />
                </div>
              </div>
            </div>
          </div>

        </div> <!-- /row -->
      </div> <!-- /card-body -->

    </div>
  </div>
</asp:Content>
