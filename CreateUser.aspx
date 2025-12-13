<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CreateUser.aspx.vb"
    Inherits="DAYTONAMIO.CreateUser" MaintainScrollPositionOnPostBack="true"
    MasterPageFile="~/Site1.Master" %>

<asp:Content ID="HeadBlock" ContentPlaceHolderID="HeadContent" runat="server">
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
        .wrap{padding:10px}
        .card{width:100%;max-width:1400px;margin:0 auto;background:var(--surface);border-radius:12px;box-shadow:0 4px 20px rgba(0,0,0,.08);overflow:hidden}
        .hdr{display:flex;gap:12px;align-items:center;padding:12px 20px;background:var(--nav-bg);color:#fff}
        .logo{width:40px;height:40px;object-fit:contain;background:var(--surface);border-radius:8px}
        .ttl{margin:0;color:#fff;font-size:1.2rem;font-weight:700}
        .sub{margin:0;color:rgba(255,255,255,.7);font-size:.8rem}
        .body{padding:16px 20px 12px}

        /* Layout principal de 3 columnas */
        .main-layout{display:grid;grid-template-columns:1fr 1fr 1fr;gap:16px;margin-bottom:12px}

        /* Sección de formulario */
        .form-section{background:var(--bg-main);padding:14px;border-radius:10px;border:1px solid var(--border-color)}
        .section-title{margin:0 0 10px 0;color:var(--text-header);font-size:.9rem;font-weight:700;padding-bottom:8px;border-bottom:2px solid var(--primary)}

        /* Stack de campos */
        .fields-stack{display:flex;flex-direction:column;gap:8px}

        .field-group{display:flex;flex-direction:column;gap:3px}
        .createuser-label{display:block;color:var(--text-body);font-weight:600;font-size:.75rem}
        .label-required::after{content:' *';color:#dc2626}

        .inp{width:100%;border:1px solid var(--border-color);border-radius:6px;padding:6px 10px;font-size:13px;outline:none;transition:all .2s;background:var(--surface)}
        .inp:focus{border-color:var(--primary);box-shadow:0 0 0 2px rgba(16,185,129,.12)}

        /* Permisos inline */
        .permissions-inline{display:flex;flex-wrap:wrap;gap:8px}
        .chip{display:flex;align-items:center;gap:6px;border:1px solid var(--border-color);border-radius:6px;padding:6px 10px;background:var(--surface);transition:all .2s;cursor:pointer;font-size:.75rem}
        .chip:hover{border-color:var(--primary);background:var(--primary-light)}
        .chip input[type="checkbox"]{cursor:pointer;width:14px;height:14px;accent-color:var(--primary)}
        .chip label{margin:0;cursor:pointer;font-weight:600;color:var(--text-body);font-size:.75rem}

        .paridad-box{background:var(--surface);border:1px solid var(--border-color);border-radius:8px;padding:10px}
        .paridad-title{font-weight:700;color:var(--text-header);margin-bottom:8px;font-size:.8rem}
        .paridad-options{display:flex;gap:12px;align-items:center;flex-wrap:wrap}
        .paridad-item{display:flex;align-items:center;gap:4px}
        .paridad-item input[type="checkbox"]{width:14px;height:14px;accent-color:var(--primary);cursor:pointer}
        .paridad-item label{margin:0;font-weight:600;cursor:pointer;font-size:.75rem}
        .paridad-note{color:var(--text-muted);font-size:.7rem;font-style:italic;margin-top:6px}

        .btns{display:flex;gap:8px;justify-content:center;padding:10px 20px;background:var(--bg-main);border-top:1px solid var(--border-color)}
        .btn-cu{border:0;border-radius:6px;padding:8px 16px;font-weight:700;cursor:pointer;background:var(--primary);color:#fff;transition:all .2s ease;font-size:.85rem}
        .btn-cu:hover{background:var(--primary-hover)}
        .btn-cu.sec{background:var(--nav-bg)}
        .btn-cu.sec:hover{background:#042218}

        .msg{margin-bottom:12px;padding:8px 12px;border-radius:6px;border:1px solid transparent;display:none;font-weight:600;font-size:.85rem}
        .msg.show{display:block}
        .msg.ok{background:var(--okbg);border-color:var(--okbd);color:var(--ok)}
        .msg.err{background:var(--errbg);border-color:var(--errbd);color:var(--err)}

        /* Tabla compacta */
        .list{padding:12px 20px 16px}
        .list-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:8px}
        .list-title{margin:0;color:var(--text-header);font-size:1rem;font-weight:700}
        .tbl{width:100%;border-collapse:collapse;font-size:12px}
        .tbl th,.tbl td{padding:6px 8px;text-align:left;border-bottom:1px solid var(--border-color)}
        .tbl th{background:var(--nav-bg);color:#fff;font-weight:600;font-size:11px;text-transform:uppercase}
        .tbl tr:hover td{background:var(--primary-light)}
        .tbl tr:nth-child(even) td{background:#f9fafb}
        .tbl tr:nth-child(even):hover td{background:var(--primary-light)}

        .badge{display:inline-block;padding:2px 6px;font-size:9px;font-weight:700;border-radius:4px;background:var(--primary);color:#fff;text-transform:uppercase;margin-left:8px}
        .muted{color:var(--text-muted)}

        @media (max-width: 1200px){
            .main-layout{grid-template-columns:1fr 1fr}
        }

        @media (max-width: 768px){
            .main-layout{grid-template-columns:1fr}
            .hdr{flex-direction:column;align-items:flex-start;padding:12px 16px}
            .body,.list{padding:12px 16px}
        }
    </style>
</asp:Content>

<asp:Content ID="BodyBlock" ContentPlaceHolderID="MainContent" runat="server">
    <div class="wrap">
        <div class="card">
            <div class="hdr">
                <img class="logo" src="images/logo1.png" alt="Logo" />
                <div>
                    <h1 class="ttl">Alta de Usuarios <span class="badge">Admin</span></h1>
                    <div class="sub">Gestiona usuarios del sistema</div>
                </div>
            </div>

            <div class="body">
                <asp:Label ID="lblMsg" runat="server" CssClass="msg"></asp:Label>

                <div class="main-layout">
                    <div class="form-section">
                        <h2 class="section-title">Datos Personales</h2>
                        <div class="fields-stack">
                            <div class="field-group">
                                <label for="txtNombre" class="createuser-label label-required">Nombre</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="inp" MaxLength="100" placeholder="Juan Pérez" />
                            </div>
                            <div class="field-group">
                                <label for="txtCorreo" class="createuser-label label-required">Correo</label>
                                <asp:TextBox ID="txtCorreo" runat="server" CssClass="inp text-lowercase" TextMode="Email" MaxLength="150" placeholder="usuario@empresa.com" />
                            </div>
                            <div class="field-group">
                                <label for="txtTelefono" class="createuser-label">Teléfono</label>
                                <asp:TextBox ID="txtTelefono" runat="server" CssClass="inp" MaxLength="30" placeholder="(55) 1234-5678" />
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h2 class="section-title">Seguridad</h2>
                        <div class="fields-stack">
                            <div class="field-group">
                                <label for="txtPassword" class="createuser-label label-required">Contraseña</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="inp" TextMode="Password" placeholder="Min. 8 caracteres" />
                            </div>
                            <div class="field-group">
                                <label for="txtConfirm" class="createuser-label label-required">Confirmar</label>
                                <asp:TextBox ID="txtConfirm" runat="server" CssClass="inp" TextMode="Password" placeholder="Repetir contraseña" />
                            </div>
                            <div class="permissions-inline">
                                <div class="chip">
                                    <asp:CheckBox ID="chkValidador" runat="server" />
                                    <label for="<%= chkValidador.ClientID %>">Validador</label>
                                </div>
                                <div class="chip">
                                    <asp:CheckBox ID="chkAdmin" runat="server" />
                                    <label for="<%= chkAdmin.ClientID %>">Admin</label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h2 class="section-title">Configuración</h2>
                        <div class="fields-stack">
                            <div class="paridad-box">
                                <div class="paridad-title">Rol de Jefe</div>
                                <div class="paridad-options">
                                    <div class="paridad-item">
                                        <asp:CheckBox ID="chkJefeServicio" runat="server" />
                                        <label for="<%= chkJefeServicio.ClientID %>">Servicio</label>
                                    </div>
                                    <div class="paridad-item">
                                        <asp:CheckBox ID="chkJefeRefacciones" runat="server" />
                                        <label for="<%= chkJefeRefacciones.ClientID %>">Refacc.</label>
                                    </div>
                                    <div class="paridad-item">
                                        <asp:CheckBox ID="chkJefeAdministracion" runat="server" />
                                        <label for="<%= chkJefeAdministracion.ClientID %>">Admin.</label>
                                    </div>
                                    <div class="paridad-item">
                                        <asp:CheckBox ID="chkJefeTaller" runat="server" />
                                        <label for="<%= chkJefeTaller.ClientID %>">Taller</label>
                                    </div>
                                </div>
                            </div>
                            <div class="paridad-box">
                                <div class="paridad-title">Paridad</div>
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
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="btns">
                <asp:Button ID="btnGuardar" runat="server" CssClass="btn-cu" Text="Guardar" OnClick="btnGuardar_Click" />
                <asp:Button ID="btnLimpiar" runat="server" CssClass="btn-cu sec" Text="Limpiar" CausesValidation="false" OnClick="btnLimpiar_Click" />
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

                        <asp:TemplateField HeaderText="Jefe">
                            <ItemTemplate>
                                <%# GetJefeDisplay(Eval("JefeServicio"), Eval("JefeRefacciones"), Eval("JefeAdministracion"), Eval("JefeTaller")) %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlEditJefe" runat="server" CssClass="inp">
                                    <asp:ListItem Text="(ninguno)" Value="" />
                                    <asp:ListItem Text="Servicio" Value="Servicio" />
                                    <asp:ListItem Text="Refacciones" Value="Refacciones" />
                                    <asp:ListItem Text="Administración" Value="Administracion" />
                                    <asp:ListItem Text="Taller" Value="Taller" />
                                </asp:DropDownList>
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

            function bindJefes() {
                var jefeServicio = document.getElementById('<%= chkJefeServicio.ClientID %>');
                var jefeRefacciones = document.getElementById('<%= chkJefeRefacciones.ClientID %>');
                var jefeAdmin = document.getElementById('<%= chkJefeAdministracion.ClientID %>');
                var jefeTaller = document.getElementById('<%= chkJefeTaller.ClientID %>');

                var jefes = [jefeServicio, jefeRefacciones, jefeAdmin, jefeTaller];

                jefes.forEach(function(chk) {
                    if (chk) {
                        chk.addEventListener('change', function() {
                            if (this.checked) {
                                jefes.forEach(function(other) {
                                    if (other && other !== chk) {
                                        other.checked = false;
                                    }
                                });
                            }
                        });
                    }
                });
            }

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', function() {
                    bindParity();
                    bindJefes();
                });
            } else {
                bindParity();
                bindJefes();
            }
        })();
    </script>
</asp:Content>