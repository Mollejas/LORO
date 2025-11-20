<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CreateUser.aspx.vb"
    Inherits="DAYTONAMIO.CreateUser" MaintainScrollPositionOnPostBack="true" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Crear Usuario | Mi Empresa</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <style>
        :root{
            --nav-bg: #062a24;
            --primary: #10b981;
            --primary-hover: #059669;
            --primary-light: #ecfdf5;
            --text-header: #0b1324;
            --text-body: #1f2937;
            --text-muted: #6b7280;
            --bg-main: #fafafa;
            --surface: #ffffff;
            --border-color: #e5e7eb;
            --chip-active-bg: #065f46;
            --ok:#10b981; --okbg:#d1fae5; --okbd:#a7f3d0; --err:#dc2626; --errbg:#fee2e2; --errbd:#fecaca;
        }
        *{box-sizing:border-box}
        html,body{height:100%;margin:0;font-family:Calibri,"Segoe UI",Roboto,Arial,sans-serif;background:linear-gradient(135deg,var(--nav-bg),#064e3b)}
        .wrap{min-height:100%;display:grid;place-items:center;padding:20px}
        .card{width:100%;max-width:1200px;background:var(--surface);border-radius:18px;box-shadow:0 18px 56px rgba(0,0,0,.22);overflow:hidden;animation:fadeIn .35s ease-out both}
        .hdr{display:flex;gap:20px;align-items:center;padding:24px 32px;background:var(--primary-light);border-bottom:1px solid #d1fae5}
        .logo{width:64px;height:64px;object-fit:contain;background:var(--surface);border-radius:12px;border:1px solid var(--border-color)}
        .ttl{margin:0;color:var(--text-header);font-size:1.65rem;font-weight:800;letter-spacing:-.02em}
        .sub{margin:.35rem 0 0 0;color:var(--text-muted);font-size:1rem}
        .body{padding:32px 32px 24px;border-bottom:1px solid var(--border-color)}
        
        /* Layout de dos columnas */
        .two-column-layout{display:grid;grid-template-columns:1fr 1fr;gap:24px;margin-bottom:20px}
        
        /* Sección de formulario */
        .form-section{background:var(--bg-main);padding:24px;border-radius:14px;border:1px solid var(--border-color);height:100%;display:flex;flex-direction:column}
        .section-title{margin:0 0 20px 0;color:var(--text-header);font-size:1.1rem;font-weight:700;display:flex;align-items:center;gap:10px}
        .section-title::before{content:'';width:4px;height:20px;background:var(--primary);border-radius:2px}
        
        /* Stack de campos */
        .fields-stack{display:flex;flex-direction:column;gap:16px;flex:1}
        
        .grid{display:grid;grid-template-columns:repeat(12,1fr);gap:18px}
        .col-6{grid-column:span 6}
        .col-4{grid-column:span 4}
        .col-3{grid-column:span 3}
        .col-12{grid-column:span 12}
        
        .field-group{display:flex;flex-direction:column;gap:8px}
        label{display:block;color:var(--text-body);font-weight:700;font-size:.95rem}
        .label-required::after{content:' *';color:#dc2626}
        
        .inp{width:100%;border:1px solid var(--border-color);border-radius:10px;padding:12px 16px;font-size:15px;outline:none;transition:all .2s;background:var(--surface)}
        .inp:focus{border-color:var(--primary);box-shadow:0 0 0 4px rgba(16,185,129,.12)}
        .inp:hover:not(:focus){border-color:#9ca3af}
        
        /* Sección de permisos */
        .permissions-stack{display:flex;flex-direction:column;gap:12px}
        .permissions-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:14px;margin-top:4px}
        .chip{display:flex;align-items:center;gap:10px;border:2px solid var(--border-color);border-radius:12px;padding:14px 16px;background:var(--surface);transition:all .2s;cursor:pointer}
        .chip:hover{border-color:var(--primary);background:var(--primary-light)}
        .chip input[type="checkbox"]{cursor:pointer;width:18px;height:18px;accent-color:var(--primary)}
        .chip label{margin:0;cursor:pointer;font-weight:600;color:var(--text-body)}
        
        .paridad-box{background:var(--surface);border:2px solid var(--border-color);border-radius:12px;padding:18px}
        .paridad-title{font-weight:700;color:var(--text-header);margin-bottom:12px;font-size:1rem}
        .paridad-options{display:flex;gap:20px;align-items:center;flex-wrap:wrap;margin-bottom:10px}
        .paridad-item{display:flex;align-items:center;gap:8px}
        .paridad-item input[type="checkbox"]{width:18px;height:18px;accent-color:var(--primary);cursor:pointer}
        .paridad-item label{margin:0;font-weight:600;cursor:pointer}
        .paridad-note{color:var(--text-muted);font-size:.88rem;font-style:italic;display:block}
        
        .btns{display:flex;gap:12px;margin-top:24px;flex-wrap:wrap}
        .btn{border:0;border-radius:10px;padding:13px 24px;font-weight:700;cursor:pointer;background:var(--primary);color:#fff;transition:all .2s ease;font-size:1rem;box-shadow:0 2px 8px rgba(16,185,129,.25)}
        .btn:hover{transform:translateY(-1px);box-shadow:0 4px 12px rgba(16,185,129,.35);background:var(--primary-hover)}
        .btn:active{transform:translateY(0)}
        .btn.sec{background:var(--nav-bg);box-shadow:0 2px 8px rgba(6,42,36,.25)}
        .btn.sec:hover{background:#042218;box-shadow:0 4px 12px rgba(6,42,36,.35)}
        
        .msg{margin-bottom:20px;padding:14px 16px;border-radius:10px;border:2px solid transparent;display:none;font-weight:600}
        .msg.show{display:block}
        .msg.ok{background:var(--okbg);border-color:var(--okbd);color:var(--ok)}
        .msg.err{background:var(--errbg);border-color:var(--errbd);color:var(--err)}
        
        @keyframes fadeIn{from{opacity:0;transform:translateY(12px)}to{opacity:1;transform:translateY(0)}}

        /* Tabla mejorada */
        .list{padding:32px 32px 28px}
        .list-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:16px}
        .list-title{margin:0;color:var(--text-header);font-size:1.35rem;font-weight:800}
        .tbl{width:100%;border-collapse:separate;border-spacing:0;border:1px solid var(--border-color);border-radius:12px;overflow:hidden}
        .tbl th,.tbl td{padding:12px 16px;text-align:left;font-size:14px}
        .tbl th{background:var(--bg-main);color:var(--text-header);font-weight:700;border-bottom:2px solid var(--border-color)}
        .tbl td{border-bottom:1px solid var(--border-color)}
        .tbl tr:last-child td{border-bottom:none}
        .tbl tr:hover td{background:var(--primary-light)}

        .badge{display:inline-block;padding:4px 10px;font-size:11px;font-weight:700;border-radius:999px;background:var(--primary-light);color:var(--chip-active-bg);border:1px solid #a7f3d0;text-transform:uppercase;letter-spacing:.02em}
        .muted{color:var(--text-muted)}

        @media (max-width: 1024px){
            .two-column-layout{grid-template-columns:1fr}
            .grid{grid-template-columns:repeat(6,1fr)}
            .col-6,.col-4,.col-3{grid-column:span 6}
            .col-12{grid-column:span 6}
        }
        
        @media (max-width: 768px){
            .hdr{flex-direction:column;align-items:flex-start;padding:20px 24px}
            .body,.list{padding:24px 20px}
            .form-section{padding:20px 16px}
            .permissions-grid{grid-template-columns:1fr}
            .paridad-note{margin-left:0;margin-top:8px}
            .ttl{font-size:1.4rem}
        }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <div class="wrap">
        <div class="card">
            <div class="hdr">
                <img class="logo" src="images/logo1.png" alt="Logo" />
                <div>
                    <h1 class="ttl">Alta de Usuarios</h1>
                    <div class="sub">Crea nuevos usuarios y administra los existentes <span class="badge">Administración</span></div>
                </div>
            </div>

            <div class="body">
                <asp:Label ID="lblMsg" runat="server" CssClass="msg"></asp:Label>
                
                <div class="two-column-layout">
                    <div class="form-section">
                        <h2 class="section-title">Información Personal</h2>
                        <div class="fields-stack">
                            <div class="field-group">
                                <label for="txtNombre" class="label-required">Nombre completo</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="inp" MaxLength="100" placeholder="Ej: Juan Pérez García" />
                            </div>

                            <div class="field-group">
                                <label for="txtCorreo" class="label-required">Correo electrónico</label>
                                <asp:TextBox ID="txtCorreo" runat="server" CssClass="inp text-lowercase" TextMode="Email" MaxLength="150" placeholder="usuario@empresa.com" />
                            </div>

                            <div class="field-group">
                                <label for="txtTelefono">Teléfono</label>
                                <asp:TextBox ID="txtTelefono" runat="server" CssClass="inp" MaxLength="30" placeholder="(55) 1234-5678" />
                            </div>

                            <div class="field-group">
                                <label for="txtPassword" class="label-required">Contraseña</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="inp" TextMode="Password" placeholder="Mínimo 8 caracteres" />
                            </div>

                            <div class="field-group">
                                <label for="txtConfirm" class="label-required">Confirmar contraseña</label>
                                <asp:TextBox ID="txtConfirm" runat="server" CssClass="inp" TextMode="Password" placeholder="Repite la contraseña" />
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h2 class="section-title">Permisos y Configuración</h2>
                        <div class="fields-stack">
                            <div class="permissions-stack">
                                <div class="chip">
                                    <asp:CheckBox ID="chkValidador" runat="server" />
                                    <label for="<%= chkValidador.ClientID %>">Validador (activo)</label>
                                </div>
                                <div class="chip">
                                    <asp:CheckBox ID="chkAdmin" runat="server" />
                                    <label for="<%= chkAdmin.ClientID %>">Administrador</label>
                                </div>
                            </div>
                            
                            <div class="paridad-box">
                                <div class="paridad-title">Configuración de Paridad</div>
                                <div class="paridad-options">
                                    <div class="paridad-item">
                                        <asp:CheckBox ID="chkPar" runat="server" />
                                        <label for="<%= chkPar.ClientID %>">Par</label>
                                    </div>
                                    <div class="paridad-item">
                                        <asp:CheckBox ID="chkNon" runat="server" />
                                        <label for="<%= chkNon.ClientID %>">Non</label>
                                    </div>
                                </div>
                                <div class="paridad-note">Las opciones son mutuamente excluyentes</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="btns">
                    <asp:Button ID="btnGuardar" runat="server" CssClass="btn" Text="💾 Guardar usuario" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnLimpiar" runat="server" CssClass="btn sec" Text="🔄 Limpiar formulario" CausesValidation="false" OnClick="btnLimpiar_Click" />
                </div>
            </div>

            <div class="list">
                <div class="list-header">
                    <h3 class="list-title">Usuarios Registrados</h3>
                </div>
                <asp:GridView ID="gvUsuarios" runat="server"
                    AutoGenerateColumns="False"
                    CssClass="tbl"
                    AllowPaging="True" PageSize="10"
                    DataKeyNames="UsuarioId"
                    OnPageIndexChanging="gvUsuarios_PageIndexChanging"
                    OnRowEditing="gvUsuarios_RowEditing"
                    OnRowCancelingEdit="gvUsuarios_RowCancelingEdit"
                    OnRowUpdating="gvUsuarios_RowUpdating"
                    OnRowDeleting="gvUsuarios_RowDeleting">
                    <Columns>
                        <asp:BoundField DataField="UsuarioId" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="Correo" HeaderText="Correo" />
                        <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />

                        <asp:TemplateField HeaderText="Validador">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkItemValidador" runat="server" Checked='<%# Convert.ToBoolean(Eval("Validador")) %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEditValidador" runat="server" Checked='<%# Convert.ToBoolean(Eval("Validador")) %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Admin">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkItemAdmin" runat="server" Checked='<%# Convert.ToBoolean(Eval("EsAdmin")) %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEditAdmin" runat="server" Checked='<%# Convert.ToBoolean(Eval("EsAdmin")) %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Paridad">
                            <ItemTemplate><%# Eval("Paridad") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlEditParidad" runat="server" CssClass="inp">
                                    <asp:ListItem Text="PAR" Value="PAR" />
                                    <asp:ListItem Text="NON" Value="NON" />
                                    <asp:ListItem Text="(ninguna)" Value="" />
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="FechaAlta" HeaderText="Fecha Alta" DataFormatString="{0:yyyy-MM-dd HH:mm}" ReadOnly="True" />
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" EditText="✏️ Editar" DeleteText="🗑️ Eliminar" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div style="text-align:center;color:#e6f0ff;font-size:.9rem;margin-top:16px;font-weight:500">© <%: DateTime.Now.Year %> Mi Empresa — Todos los derechos reservados</div>
    </div>

    <script type="text/javascript">
        (function () {
            function bindParity() {
                var par = document.getElementById('<%= chkPar.ClientID %>');
                var non = document.getElementById('<%= chkNon.ClientID %>');
                if (!par || !non) return;
                par.addEventListener('change', function () { if (par.checked) non.checked = false; });
                non.addEventListener('change', function () { if (non.checked) par.checked = false; });
            }
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', bindParity);
            } else { bindParity(); }
        })();
    </script>
</form>
</body>
</html>