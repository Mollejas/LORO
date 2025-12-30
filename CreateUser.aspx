<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CreateUser.aspx.vb"
    Inherits="DAYTONAMIO.CreateUser" MaintainScrollPositionOnPostBack="true"
    MasterPageFile="~/Site1.Master" %>

<asp:Content ID="HeadBlock" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* Estilos específicos para CreateUser.aspx */
        .cu-card{width:100%;max-width:1400px;margin:0 auto;background:var(--surface);border-radius:12px;box-shadow:0 4px 20px rgba(0,0,0,.08);overflow:hidden}
        .cu-header{display:flex;gap:12px;align-items:center;padding:12px 20px;background:var(--nav-bg);color:#fff}
        .cu-logo{width:40px;height:40px;object-fit:contain;background:var(--surface);border-radius:8px}
        .cu-title{margin:0;color:#fff;font-size:1.2rem;font-weight:700}
        .cu-subtitle{margin:0;color:rgba(255,255,255,.7);font-size:.8rem}
        .cu-body{padding:16px 20px 12px}

        /* Layout de 3 columnas */
        .cu-layout{display:grid;grid-template-columns:1fr 1fr 1fr;gap:16px;margin-bottom:12px}
        @media (max-width: 1200px){.cu-layout{grid-template-columns:1fr 1fr}}
        @media (max-width: 768px){.cu-layout{grid-template-columns:1fr}}

        /* Secciones de formulario */
        .cu-section{background:var(--bg-main);padding:14px;border-radius:10px;border:1px solid var(--border-color)}
        .cu-section-title{margin:0 0 10px 0;color:var(--text-header);font-size:.9rem;font-weight:700;padding-bottom:8px;border-bottom:2px solid var(--primary)}
        .cu-fields{display:flex;flex-direction:column;gap:8px}

        /* Campos de formulario */
        .cu-field{display:flex;flex-direction:column;gap:3px}
        .cu-label{display:block;color:var(--text-body);font-weight:600;font-size:.75rem}
        .cu-label-required::after{content:' *';color:#dc2626}

        /* Permisos checkboxes */
        .cu-perms{display:flex;flex-wrap:wrap;gap:8px}
        .cu-perm-box{display:flex;align-items:center;gap:6px;border:1px solid var(--border-color);border-radius:6px;padding:6px 10px;background:var(--surface);transition:all .2s;cursor:pointer;font-size:.75rem}
        .cu-perm-box:hover{border-color:var(--primary);background:var(--primary-light)}

        /* Paridad y roles */
        .cu-config-box{background:var(--surface);border:1px solid var(--border-color);border-radius:8px;padding:10px}
        .cu-config-title{font-weight:700;color:var(--text-header);margin-bottom:8px;font-size:.8rem}
        .cu-config-items{display:flex;gap:12px;align-items:center;flex-wrap:wrap}
        .cu-config-item{display:flex;align-items:center;gap:4px}

        /* Botones */
        .cu-buttons{display:flex;gap:8px;justify-content:center;padding:10px 20px;background:var(--bg-main);border-top:1px solid var(--border-color)}

        /* Mensajes */
        .cu-msg{margin-bottom:12px;padding:8px 12px;border-radius:6px;border:1px solid transparent;display:none;font-weight:600;font-size:.85rem}
        .cu-msg.show{display:block}
        .cu-msg.ok{background:#d1fae5;border-color:#a7f3d0;color:#10b981}
        .cu-msg.err{background:#fee2e2;border-color:#fecaca;color:#dc2626}

        /* Tabla de usuarios */
        .cu-list{padding:12px 20px 16px}
        .cu-list-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:8px}
        .cu-list-title{margin:0;color:var(--text-header);font-size:1rem;font-weight:700}
        .cu-table{width:100%;border-collapse:collapse;font-size:12px}
        .cu-table th,.cu-table td{padding:6px 8px;text-align:left;border-bottom:1px solid var(--border-color)}
        .cu-table th{background:var(--nav-bg);color:#fff;font-weight:600;font-size:11px;text-transform:uppercase}
        .cu-table tr:hover td{background:var(--primary-light)}
        .cu-table tr:nth-child(even) td{background:#f9fafb}
        .cu-table tr:nth-child(even):hover td{background:var(--primary-light)}

        .cu-badge{display:inline-block;padding:2px 6px;font-size:9px;font-weight:700;border-radius:4px;background:var(--primary);color:#fff;text-transform:uppercase;margin-left:8px}
    </style>
</asp:Content>

<asp:Content ID="BodyBlock" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cu-card">
        <div class="cu-header">
            <img class="cu-logo" src="images/logo1.png" alt="Logo" />
            <div>
                <h1 class="cu-title">Alta de Usuarios <span class="cu-badge">Admin</span></h1>
                <div class="cu-subtitle">Gestiona usuarios del sistema</div>
            </div>
        </div>

        <div class="cu-body">
            <asp:Label ID="lblMsg" runat="server" CssClass="cu-msg"></asp:Label>

            <div class="cu-layout">
                <div class="cu-section">
                    <h2 class="cu-section-title">Datos Personales</h2>
                    <div class="cu-fields">
                        <div class="cu-field">
                            <label for="txtNombre" class="cu-label cu-label-required">Nombre</label>
                            <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control form-control-sm" MaxLength="100" placeholder="Juan Pérez" />
                        </div>
                        <div class="cu-field">
                            <label for="txtCorreo" class="cu-label cu-label-required">Correo</label>
                            <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control form-control-sm text-lowercase" TextMode="Email" MaxLength="150" placeholder="usuario@empresa.com" />
                        </div>
                        <div class="cu-field">
                            <label for="txtTelefono" class="cu-label">Teléfono</label>
                            <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control form-control-sm" MaxLength="30" placeholder="(55) 1234-5678" />
                        </div>
                    </div>
                </div>

                <div class="cu-section">
                    <h2 class="cu-section-title">Seguridad</h2>
                    <div class="cu-fields">
                        <div class="cu-field">
                            <label for="txtPassword" class="cu-label cu-label-required">Contraseña</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control form-control-sm" TextMode="Password" placeholder="Min. 8 caracteres" />
                        </div>
                        <div class="cu-field">
                            <label for="txtConfirm" class="cu-label cu-label-required">Confirmar</label>
                            <asp:TextBox ID="txtConfirm" runat="server" CssClass="form-control form-control-sm" TextMode="Password" placeholder="Repetir contraseña" />
                        </div>
                        <div class="cu-perms">
                            <div class="cu-perm-box">
                                <asp:CheckBox ID="chkValidador" runat="server" CssClass="form-check-input" />
                                <label for="<%= chkValidador.ClientID %>" class="cu-label">Validador</label>
                            </div>
                            <div class="cu-perm-box">
                                <asp:CheckBox ID="chkAdmin" runat="server" CssClass="form-check-input" />
                                <label for="<%= chkAdmin.ClientID %>" class="cu-label">Admin</label>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="cu-section">
                    <h2 class="cu-section-title">Configuración</h2>
                    <div class="cu-fields">
                        <div class="cu-config-box">
                            <div class="cu-config-title">Rol de Jefe</div>
                            <div class="cu-config-items">
                                <div class="cu-config-item">
                                    <asp:CheckBox ID="chkJefeServicio" runat="server" CssClass="form-check-input" />
                                    <label for="<%= chkJefeServicio.ClientID %>" class="cu-label">Servicio</label>
                                </div>
                                <div class="cu-config-item">
                                    <asp:CheckBox ID="chkJefeRefacciones" runat="server" CssClass="form-check-input" />
                                    <label for="<%= chkJefeRefacciones.ClientID %>" class="cu-label">Refacc.</label>
                                </div>
                                <div class="cu-config-item">
                                    <asp:CheckBox ID="chkJefeAdministracion" runat="server" CssClass="form-check-input" />
                                    <label for="<%= chkJefeAdministracion.ClientID %>" class="cu-label">Admin.</label>
                                </div>
                                <div class="cu-config-item">
                                    <asp:CheckBox ID="chkJefeTaller" runat="server" CssClass="form-check-input" />
                                    <label for="<%= chkJefeTaller.ClientID %>" class="cu-label">Taller</label>
                                </div>
                            </div>
                        </div>
                        <div class="cu-config-box">
                            <div class="cu-config-title">Paridad</div>
                            <div class="cu-config-items">
                                <div class="cu-config-item">
                                    <asp:CheckBox ID="chkPar" runat="server" CssClass="form-check-input" />
                                    <label for="<%= chkPar.ClientID %>" class="cu-label">Par</label>
                                </div>
                                <div class="cu-config-item">
                                    <asp:CheckBox ID="chkNon" runat="server" CssClass="form-check-input" />
                                    <label for="<%= chkNon.ClientID %>" class="cu-label">Non</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="cu-buttons">
            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primary" Text="Guardar" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnLimpiar" runat="server" CssClass="btn btn-outline-primary" Text="Limpiar" CausesValidation="false" OnClick="btnLimpiar_Click" />
        </div>

        <div class="cu-list">
            <div class="cu-list-header">
                <h3 class="cu-list-title">Usuarios Registrados</h3>
            </div>
            <asp:GridView ID="gvUsuarios" runat="server"
                AutoGenerateColumns="False"
                CssClass="cu-table"
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
                            <asp:CheckBox ID="chkItemValidador" runat="server" Checked='<%# Convert.ToBoolean(Eval("Validador")) %>' Enabled="false" CssClass="form-check-input" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkEditValidador" runat="server" Checked='<%# Convert.ToBoolean(Eval("Validador")) %>' CssClass="form-check-input" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Admin">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkItemAdmin" runat="server" Checked='<%# Convert.ToBoolean(Eval("EsAdmin")) %>' Enabled="false" CssClass="form-check-input" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkEditAdmin" runat="server" Checked='<%# Convert.ToBoolean(Eval("EsAdmin")) %>' CssClass="form-check-input" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Jefe">
                        <ItemTemplate>
                            <%# GetJefeDisplay(Eval("JefeServicio"), Eval("JefeRefacciones"), Eval("JefeAdministracion"), Eval("JefeTaller")) %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlEditJefe" runat="server" CssClass="form-select form-select-sm">
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
                            <asp:DropDownList ID="ddlEditParidad" runat="server" CssClass="form-select form-select-sm">
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