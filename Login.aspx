<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="DAYTONAMIO.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Acceso | Mi Empresa</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <style>
        :root{
            --brand: #10b981;
            --brand-600: #059669;
            --brand-700: #047857;
            --brand-soft: #d1fae5;
            --brand-lighter: #ecfdf5;
            --brand-glow: rgba(16, 185, 129, 0.15);
            --text: #ffffff;
            --text-secondary: rgba(255, 255, 255, 0.8);
            --muted: rgba(255, 255, 255, 0.6);
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        html, body {
            height: 100%;
            font-family: 'Segoe UI', Roboto, 'Inter', Arial, sans-serif;
            background: linear-gradient(135deg, #0f2027 0%, #203a43 50%, #2c5364 100%);
            position: relative;
            overflow: hidden;
        }
        .bg-particles { position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1; overflow: hidden; }
        .particle { position: absolute; background: rgba(16, 185, 129, 0.08); border-radius: 50%; animation: float 20s infinite ease-in-out; }
        .particle:nth-child(1) { width: 80px; height: 80px; left: 10%; animation-delay: 0s; }
        .particle:nth-child(2) { width: 120px; height: 120px; left: 80%; animation-delay: 2s; }
        .particle:nth-child(3) { width: 60px; height: 60px; left: 50%; animation-delay: 4s; }
        .particle:nth-child(4) { width: 100px; height: 100px; left: 30%; animation-delay: 6s; }
        .particle:nth-child(5) { width: 70px; height: 70px; left: 70%; animation-delay: 8s; }
        @keyframes float {
            0%, 100% { transform: translateY(100vh) rotate(0deg); opacity: 0; }
            10% { opacity: 0.3; }
            90% { opacity: 0.3; }
            50% { transform: translateY(-10vh) rotate(180deg); }
        }
        .center-wrap { min-height: 100vh; display: flex; flex-direction: column; align-items: center; justify-content: center; padding: 24px; position: relative; z-index: 2; }
        .card {
            width: 100%; max-width: 400px; background: rgba(255, 255, 255, 0.05);
            backdrop-filter: blur(10px); border-radius: 1rem; box-shadow: 0 8px 32px rgba(0, 0, 0, 0.7);
            overflow: hidden; animation: fadeInUp 0.8s ease-out both; border: 1px solid rgba(255, 255, 255, 0.1);
        }
        @keyframes fadeInUp { from { opacity: 0; transform: translateY(30px); } to { opacity: 1; transform: translateY(0); } }
        .card-header {
            background: rgba(16, 185, 129, 0.1); padding: 2rem 2rem 1.5rem 2rem; text-align: center;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1); position: relative; overflow: hidden;
        }
        .card-header::before {
            content: ''; position: absolute; top: -50%; left: -50%; width: 200%; height: 200%;
            background: radial-gradient(circle, rgba(16, 185, 129, 0.15) 0%, transparent 70%); animation: pulse 4s ease-in-out infinite;
        }
        @keyframes pulse { 0%, 100% { transform: scale(1); opacity: 0.5; } 50% { transform: scale(1.1); opacity: 0.8; } }
        .logo {
            width: 100%; height: 120px; object-fit: contain; background: rgba(255, 255, 255, 0.95);
            margin-bottom: 16px; border-radius: 12px; padding: 12px; position: relative; z-index: 1; animation: logoGlow 3s ease-in-out infinite;
        }
        @keyframes logoGlow { 0%, 100% { box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3); } 50% { box-shadow: 0 8px 24px rgba(16, 185, 129, 0.5); } }
        .title { margin: 0; color: #fff; font-size: 1.75rem; font-weight: 800; letter-spacing: -0.5px; position: relative; z-index: 1; animation: fadeInTitle 0.8s ease-out 0.2s both; text-shadow: 0 2px 10px rgba(0, 0, 0, 0.3); }
        @keyframes fadeInTitle { from { opacity: 0; transform: translateY(-10px); } to { opacity: 1; transform: translateY(0); } }
        .subtitle { margin: 8px 0 0 0; color: var(--text-secondary); font-size: 0.95rem; position: relative; z-index: 1; animation: fadeInTitle 0.8s ease-out 0.3s both; }
        .card-body { padding: 2rem; animation: fadeInBody 0.8s ease-out 0.4s both; }
        @keyframes fadeInBody { from { opacity: 0; } to { opacity: 1; } }
        label { display: block; margin-bottom: 8px; color: var(--text); font-weight: 600; font-size: 0.9rem; }
        .form-control {
            width: 100%; border: none; border-radius: 0.5rem; padding: 13px 16px; font-size: 15px; outline: none;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1); background: rgba(255, 255, 255, 0.1); color: #fff;
        }
        .form-control::placeholder { color: rgba(255, 255, 255, 0.5); }
        .form-control:focus { background: rgba(255, 255, 255, 0.15); box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.3); transform: translateY(-1px); }
        .form-group { margin-bottom: 18px; animation: slideInLeft 0.5s ease-out both; }
        .form-group:nth-child(1) { animation-delay: 0.5s; }
        .form-group:nth-child(2) { animation-delay: 0.6s; }
        .form-group:nth-child(3) { animation-delay: 0.7s; }
        @keyframes slideInLeft { from { opacity: 0; transform: translateX(-20px); } to { opacity: 1; transform: translateX(0); } }
        .actions { display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-top: 12px; animation: slideInLeft 0.5s ease-out 0.8s both; }
        .btn {
            display: inline-flex; align-items: center; justify-content: center; width: 100%; border: none; border-radius: 0.5rem;
            padding: 14px 16px; font-weight: 700; font-size: 1rem; cursor: pointer; background: #10b981; color: #fff;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1); box-shadow: 0 4px 12px rgba(16, 185, 129, 0.4); position: relative; overflow: hidden;
        }
        .btn::before {
            content: ''; position: absolute; top: 50%; left: 50%; width: 0; height: 0; border-radius: 50%;
            background: rgba(255, 255, 255, 0.2); transform: translate(-50%, -50%); transition: width 0.6s, height 0.6s;
        }
        .btn:hover { background: #059669; transform: translateY(-2px); box-shadow: 0 8px 20px rgba(16, 185, 129, 0.5); }
        .btn:hover::before { width: 300px; height: 300px; }
        .btn:active { transform: translateY(0); }
        .helper { font-size: 0.9rem; color: var(--muted); text-align: center; margin-top: 16px; animation: fadeIn 0.8s ease-out 0.9s both; }
        .helper a { color: #10b981; text-decoration: none; transition: all 0.3s ease; }
        .helper a:hover { text-decoration: underline; }
        @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
        .alert {
            margin: 12px 0 0 0; padding: 12px 16px; border-radius: 0.5rem; background: rgba(239, 68, 68, 0.2);
            color: #fecaca; border: 1px solid rgba(239, 68, 68, 0.3); display: none; animation: shake 0.5s ease-in-out; backdrop-filter: blur(10px);
        }
        @keyframes shake { 0%, 100% { transform: translateX(0); } 25% { transform: translateX(-10px); } 75% { transform: translateX(10px); } }
        .alert.show { display: block; }
        .footer { text-align: center; color: rgba(255, 255, 255, 0.8); font-size: 0.75rem; margin-top: 1rem; position: relative; z-index: 2; opacity: 0.8; }
        .footer a { color: #10b981; text-decoration: none; transition: all 0.3s ease; }
        .footer a:hover { text-decoration: underline; }

        /* ====== Estilos para el ojo de ver contraseña ====== */
        .input-wrap { position: relative; }
        .toggle-eye {
            position: absolute; right: 10px; top: 50%; transform: translateY(-50%);
            background: transparent; border: 0; cursor: pointer; padding: 6px; line-height: 0;
            border-radius: 8px;
        }
        .toggle-eye:hover { background: rgba(255,255,255,0.08); }
        .toggle-eye svg { width: 22px; height: 22px; }
        .hidden { display: none; }

        /* Checkbox */
        input[type="checkbox"] { width: 18px; height: 18px; cursor: pointer; margin-right: 8px; accent-color: var(--brand); }
        label[for*="chkRecordar"] { color: var(--text-secondary); font-weight: 400; display: flex; align-items: center; cursor: pointer; }
    </style>
</head>
<body>
    <div class="bg-particles">
        <div class="particle"></div>
        <div class="particle"></div>
        <div class="particle"></div>
        <div class="particle"></div>
        <div class="particle"></div>
    </div>

    <form id="form1" runat="server">
        <div class="center-wrap">
            <div class="card">
                <div class="card-header">
                    <img class="logo" src="images/logo1.png" alt="Logo" />
                    <h1 class="title">Acceso a la plataforma</h1>
                    <p class="subtitle">Ingresa con tu correo y contraseña</p>
                </div>

                <div class="card-body">
                    <asp:Label ID="lblError" runat="server" CssClass="alert" />

                    <div class="form-group">
                        <label for="txtCorreo">Correo electrónico</label>
                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email" placeholder="correo@dominio.com" />
                    </div>

                    <div class="form-group">
                        <label for="txtPassword">Contraseña</label>

                        <!-- 🔒 Contenedor con el ojo -->
                        <div class="input-wrap">
                            <!-- ClientIDMode Static para que el id sea estable en JS -->
                            <asp:TextBox ID="txtPassword" runat="server" ClientIDMode="Static"
                                         CssClass="form-control" TextMode="Password" placeholder="••••••••" />
                            <button type="button" class="toggle-eye" aria-label="Mostrar u ocultar contraseña"
                                    onclick="togglePwd('txtPassword', this)">
                                <!-- Ojo abierto -->
                                <svg class="icon-eye" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="#ffffff" opacity="0.9">
                                    <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 12a5 5 0 1 1 0-10 5 5 0 0 1 0 10z"/>
                                    <circle cx="12" cy="12" r="2.5"></circle>
                                </svg>
                                <!-- Ojo tachado -->
                                <svg class="icon-eye-off hidden" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="#ffffff" opacity="0.9">
                                    <path d="M2 4.27 3.28 3 21 20.72 19.73 22l-3.07-3.07A10.87 10.87 0 0 1 12 19C5 19 2 12 2 12a18.63 18.63 0 0 1 5.21-6.59L2 4.27zM12 7a5 5 0 0 1 5 5 4.9 4.9 0 0 1-.46 2.07l-1.51-1.51A2.99 2.99 0 0 0 12 9a3 3 0 0 0-1.83.62L8.6 8.05A4.9 4.9 0 0 1 12 7zm9.99 5s-.71 1.64-2.26 3.42l-1.43-1.43C19.59 12.81 20 12 20 12s-3-7-8-7c-1.07 0-2.06.2-2.97.55l-1.6-1.6A10.9 10.9 0 0 1 12 5c7 0 10 7 10 7z"/>
                                </svg>
                            </button>
                        </div>
                        <!-- /Contenedor -->
                    </div>

                    <div class="form-group">
                        <asp:CheckBox ID="chkRecordar" runat="server" Text="Recordar sesión en este equipo" />
                    </div>

                    <div class="actions">
                        <asp:Button ID="btnEntrar" runat="server" CssClass="btn" Text="Entrar" />
                    </div>

                    <div class="helper">
                        <small>¿Olvidaste la contraseña? <a href="#">Contacta al administrador</a></small>
                    </div>
                </div>
            </div>

            <div class="footer">© <%: DateTime.Now.Year %> LORO AUTOMOTRIZ — Todos los derechos reservados</div>
        </div>
    </form>

    <script type="text/javascript">
        // Muestra/oculta la contraseña y alterna el icono
        function togglePwd(inputId, btn) {
            var input = document.getElementById(inputId);
            if (!input) return;

            var eye = btn.querySelector('.icon-eye');
            var eyeOff = btn.querySelector('.icon-eye-off');

            if (input.type === 'password') {
                input.type = 'text';
                eye.classList.add('hidden');
                eyeOff.classList.remove('hidden');
            } else {
                input.type = 'password';
                eyeOff.classList.add('hidden');
                eye.classList.remove('hidden');
            }
        }
    </script>
</body>
</html>
