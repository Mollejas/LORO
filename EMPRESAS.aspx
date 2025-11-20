<%@ Page Title="Selecciona Empresa" Language="VB" MasterPageFile="~/Site1.Master" AutoEventWireup="false" %>

<asp:Content ID="cHead" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- Recursos específicos de ESTA página (opcional) -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.2.0/css/all.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;700&display=swap" rel="stylesheet" />
    <style>
        body {
            min-height: 100vh;
            margin: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(135deg, #0f2027 0%, #203a43 50%, #2c5364 100%);
            font-family: 'Montserrat', sans-serif;
            color: #fff;
        }
        .content-card {
            background: rgba(255,255,255,0.05);
            backdrop-filter: blur(10px);
            border-radius: 1rem;
            box-shadow: 0 8px 32px rgba(0,0,0,0.7);
            padding: 2rem;
            width: 100%;
            max-width: 900px;
            animation: fadeInUp 0.8s ease-out;
        }
        @keyframes fadeInUp {
            from { opacity: 0; transform: translateY(30px); }
            to   { opacity: 1; transform: translateY(0); }
        }
        .subtitle {
            text-align: center;
            background: #1abc9c;
            color: #fff;
            padding: 0.75rem 0;
            font-size: 1.5rem;
            font-weight: 700;
            border-radius: 6px;
            margin-bottom: 2rem;
        }
        .top-image { text-align: center; margin-bottom: 2rem; }
        .top-image img { max-width: 120px; transition: transform 0.3s ease, opacity 0.3s ease; }
        .top-image img:hover { transform: scale(1.1); opacity: 0.8; }
        .bottom-images { display: flex; justify-content: center; gap: 1.5rem; }
        .bottom-images .col-auto {
            width: 150px; height: 150px; flex: 0 0 auto;
            transition: transform 0.3s ease, opacity 0.3s ease;
        }
        .bottom-images .col-auto:hover { transform: scale(1.05); opacity: 0.9; }
        .bottom-images img {
            width: 100%; height: 100%; object-fit: fill;
            border-radius: 0.5rem; box-shadow: 0 4px 16px rgba(0,0,0,0.4);
        }
        @media (max-width: 768px) {
            .subtitle { font-size: 1.25rem; }
            .bottom-images .col-auto { width: 120px; height: 120px; }
        }
    </style>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-card">
        <div class="top-image">
            <img src="images/logoloro.jpg" alt="Logo Loro" />
        </div>

        <div class="subtitle">Elige Empresa</div>

        <div class="bottom-images">
            <div class="col-auto">
                <a href="princi.aspx">
                    <img src="images/gnp.jpg" alt="GNP" />
                </a>
            </div>
            <div class="col-auto">
                <a href="princi.aspx">
                    <img src="images/descarga.jpg" alt="Descarga" />
                </a>
            </div>
            <div class="col-auto">
                <a href="princi.aspx">
                    <img src="images/logoloro.jpg" alt="Loro" />
                </a>
            </div>
        </div>
    </div>
</asp:Content>
