<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="ElegirEmpresa.aspx.vb" Inherits="DAYTONAMIO.ElegirEmpresa" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
  <meta charset="utf-8" />
  <title>Seleccionar Empresa - Sistema</title>
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
  <link href="https://fonts.googleapis.com/css2?family=Manrope:wght@400;600;700;800&display=swap" rel="stylesheet" />

  <style>
    :root {
      --primary: #10b981;
      --primary-hover: #059669;
      --primary-light: #ecfdf5;
      --text-header: #0b1324;
      --text-body: #1f2937;
      --bg-main: #fafafa;
      --surface: #ffffff;
      --border-color: #e5e7eb;
    }

    body {
      margin: 0;
      font-family: 'Manrope', system-ui, -apple-system, sans-serif;
      background: linear-gradient(135deg, #ecfdf5 0%, #f0fdf4 100%);
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 2rem 1rem;
    }

    .container-elegir {
      max-width: 900px;
      width: 100%;
    }

    .header-elegir {
      text-align: center;
      margin-bottom: 3rem;
    }

    .header-elegir h1 {
      font-size: 2.5rem;
      font-weight: 800;
      color: var(--text-header);
      margin-bottom: 0.5rem;
    }

    .header-elegir p {
      font-size: 1.125rem;
      color: var(--text-body);
      opacity: 0.8;
    }

    .empresa-card {
      background: white;
      border-radius: 20px;
      padding: 2.5rem;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
      border: 2px solid transparent;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      cursor: pointer;
      text-align: center;
      height: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
    }

    .empresa-card:hover {
      transform: translateY(-8px);
      box-shadow: 0 20px 40px rgba(16, 185, 129, 0.15);
      border-color: var(--primary);
    }

    .empresa-card:active {
      transform: translateY(-4px);
    }

    .empresa-icon {
      width: 80px;
      height: 80px;
      background: var(--primary-light);
      border-radius: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.5rem;
    }

    .empresa-icon i {
      font-size: 2.5rem;
      color: var(--primary);
    }

    .empresa-nombre {
      font-size: 1.5rem;
      font-weight: 800;
      color: var(--text-header);
      margin-bottom: 0.5rem;
    }

    .empresa-descripcion {
      font-size: 0.95rem;
      color: var(--text-body);
      opacity: 0.7;
    }

    .btn-empresa {
      width: 100%;
      padding: 0;
      border: none;
      background: none;
      text-decoration: none;
      display: block;
    }

    @media (max-width: 768px) {
      .header-elegir h1 {
        font-size: 2rem;
      }

      .empresa-card {
        padding: 2rem;
        margin-bottom: 1rem;
      }
    }

    .user-info {
      text-align: center;
      margin-bottom: 2rem;
      padding: 1rem;
      background: rgba(255, 255, 255, 0.6);
      border-radius: 12px;
    }

    .user-info strong {
      color: var(--primary);
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
    <div class="container-elegir">
      <div class="header-elegir">
        <h1>Seleccionar Empresa</h1>
        <p>Elige la empresa con la que deseas trabajar</p>
      </div>

      <div class="user-info" runat="server" id="divUserInfo" visible="false">
        Hola, <strong><asp:Literal ID="litUsuario" runat="server" /></strong>
      </div>

      <div class="row g-4">
        <!-- QUALITAS -->
        <div class="col-12 col-md-4">
          <asp:LinkButton ID="btnQualitas" runat="server" CssClass="btn-empresa" OnClick="btnQualitas_Click">
            <div class="empresa-card">
              <div class="empresa-icon">
                <i class="bi bi-shield-check"></i>
              </div>
              <div class="empresa-nombre">QUALITAS</div>
              <div class="empresa-descripcion">Base de datos Qualitas</div>
            </div>
          </asp:LinkButton>
        </div>

        <!-- INBURSA -->
        <div class="col-12 col-md-4">
          <asp:LinkButton ID="btnInbursa" runat="server" CssClass="btn-empresa" OnClick="btnInbursa_Click">
            <div class="empresa-card">
              <div class="empresa-icon">
                <i class="bi bi-bank"></i>
              </div>
              <div class="empresa-nombre">INBURSA</div>
              <div class="empresa-descripcion">Base de datos Inbursa</div>
            </div>
          </asp:LinkButton>
        </div>

        <!-- EXTERNOS -->
        <div class="col-12 col-md-4">
          <asp:LinkButton ID="btnExternos" runat="server" CssClass="btn-empresa" OnClick="btnExternos_Click">
            <div class="empresa-card">
              <div class="empresa-icon">
                <i class="bi bi-building"></i>
              </div>
              <div class="empresa-nombre">EXTERNOS</div>
              <div class="empresa-descripcion">Base de datos Externos</div>
            </div>
          </asp:LinkButton>
        </div>
      </div>
    </div>
  </form>
</body>
</html>
