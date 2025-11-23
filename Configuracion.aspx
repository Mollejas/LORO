<%@ Page Title="Configuración"
    Language="VB"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="false"
    CodeBehind="Configuracion.aspx.vb"
    Inherits="DAYTONAMIO.Configuracion"
    ClientIDMode="Static" %>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .config-page {
      max-width: 1200px;
      margin: 48px auto;
      padding: 0 18px;
    }

    .page-header {
      text-align: center;
      margin-bottom: 48px;
    }

    .page-title {
      font-size: 32px;
      font-weight: 800;
      color: #0b1324;
      margin: 0 0 8px;
    }

    .page-subtitle {
      font-size: 16px;
      color: #6b7280;
      margin: 0;
    }

    .config-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 24px;
      justify-content: center;
    }

    .config-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 48px 32px;
      background: #fff;
      border: 2px solid #e5e7eb;
      border-radius: 20px;
      box-shadow: 0 8px 24px rgba(0,0,0,0.06);
      text-decoration: none;
      color: inherit;
      transition: all 0.2s ease;
      cursor: pointer;
      min-height: 280px;
    }

    .config-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 16px 40px rgba(16, 185, 129, 0.15);
      border-color: #10b981;
    }

    .config-card:hover .card-icon {
      transform: scale(1.1);
      color: #059669;
    }

    .card-icon {
      font-size: 72px;
      color: #10b981;
      margin-bottom: 24px;
      transition: all 0.2s ease;
      line-height: 1;
    }

    .card-title {
      font-size: 22px;
      font-weight: 800;
      color: #0b1324;
      margin: 0 0 8px;
      text-align: center;
    }

    .card-description {
      font-size: 14px;
      color: #6b7280;
      text-align: center;
      margin: 0;
      line-height: 1.5;
    }

    /* Colores específicos para cada tarjeta */
    .config-card.proveedores .card-icon {
      color: #3b82f6;
    }
    .config-card.proveedores:hover {
      border-color: #3b82f6;
      box-shadow: 0 16px 40px rgba(59, 130, 246, 0.15);
    }
    .config-card.proveedores:hover .card-icon {
      color: #2563eb;
    }

    .config-card.usuarios .card-icon {
      color: #8b5cf6;
    }
    .config-card.usuarios:hover {
      border-color: #8b5cf6;
      box-shadow: 0 16px 40px rgba(139, 92, 246, 0.15);
    }
    .config-card.usuarios:hover .card-icon {
      color: #7c3aed;
    }

    @media (max-width: 576px) {
      .config-page {
        margin: 24px auto;
      }
      .page-title {
        font-size: 24px;
      }
      .config-card {
        padding: 32px 24px;
        min-height: 220px;
      }
      .card-icon {
        font-size: 56px;
      }
      .card-title {
        font-size: 18px;
      }
    }
  </style>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">
  <div class="config-page">

    <!-- Encabezado -->
    <div class="page-header">
      <h1 class="page-title">Configuración</h1>
      <p class="page-subtitle">Administra los catálogos y usuarios del sistema</p>
    </div>

    <!-- Grid de opciones -->
    <div class="config-grid">

      <!-- Catálogo de Proveedores -->
      <a href="Proveedores.aspx" class="config-card proveedores">
        <i class="bi bi-building card-icon"></i>
        <h2 class="card-title">Catálogo de Proveedores</h2>
        <p class="card-description">Gestiona los proveedores registrados en el sistema</p>
      </a>

      <!-- Crear Usuario -->
      <a href="CreateUser.aspx" class="config-card usuarios">
        <i class="bi bi-person-plus card-icon"></i>
        <h2 class="card-title">Crear Usuario</h2>
        <p class="card-description">Registra nuevos usuarios con acceso al sistema</p>
      </a>

    </div>

  </div>
</asp:Content>
