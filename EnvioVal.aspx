<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="EnvioVal.aspx.vb" Inherits="DAYTONAMIO.EnvioVal" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
  <meta charset="utf-8" />
  <title>Creacion de Valuacion</title>
  <meta name="viewport" content="width=device-width, initial-scale=1" />

  <!-- Bootstrap + Icons -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

  <style>
    body {
      background-color: #f8f9fa;
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
    }
    .header-section {
      background: white;
      padding: 20px;
      margin-bottom: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .section-title {
      color: #0d6efd;
      font-weight: 600;
      margin-bottom: 15px;
      padding-bottom: 10px;
      border-bottom: 2px solid #0d6efd;
    }
    .section-title.warning {
      color: #ffc107;
      border-bottom-color: #ffc107;
    }
    .grid-section {
      background: white;
      padding: 20px;
      margin-bottom: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .subsection-title {
      color: #6c757d;
      font-weight: 500;
      margin-bottom: 10px;
      font-size: 0.95rem;
    }
    .table-wrapper {
      margin-bottom: 25px;
    }
    .precio-input {
      width: 120px;
      text-align: right;
    }
    .btn-save-main {
      position: fixed;
      bottom: 30px;
      right: 30px;
      z-index: 1000;
      padding: 15px 30px;
      font-size: 1.1rem;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }
    @media print {
      .btn-save-main { display: none; }
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager ID="sm1" runat="server" EnablePageMethods="true" />

    <div class="container-fluid py-4">
      <h2 class="mb-4"><i class="bi bi-calculator"></i> Creacion de Valuacion</h2>

      <!-- Datos del Vehiculo -->
      <div class="header-section">
        <div class="row">
          <div class="col-md-8">
            <h5 class="fw-bold mb-3">Datos del Vehiculo</h5>
            <table class="table table-sm table-bordered">
              <tr>
                <th style="width:120px;">Expediente</th>
                <td><asp:Label ID="lblExpediente" runat="server" /></td>
                <th style="width:100px;">Ano</th>
                <td><asp:Label ID="lblAnio" runat="server" /></td>
              </tr>
              <tr>
                <th>Marca</th>
                <td><asp:Label ID="lblMarca" runat="server" /></td>
                <th>Color</th>
                <td><asp:Label ID="lblColor" runat="server" /></td>
              </tr>
              <tr>
                <th>Modelo</th>
                <td><asp:Label ID="lblModelo" runat="server" /></td>
                <th>Placas</th>
                <td><asp:Label ID="lblPlacas" runat="server" /></td>
              </tr>
            </table>
          </div>
          <div class="col-md-4 text-center">
            <asp:Image ID="imgPrincipal" runat="server" CssClass="img-fluid rounded" style="max-height:150px;" AlternateText="Imagen del vehiculo" />
          </div>
        </div>
      </div>

      <!-- Mecanica -->
      <div class="grid-section">
        <h5 class="section-title"><i class="bi bi-wrench"></i> Mecanica</h5>

        <!-- Mecanica - Sustitucion -->
        <div class="table-wrapper">
          <h6 class="subsection-title">Sustitucion</h6>
          <asp:GridView ID="gvMecSustitucion" runat="server" CssClass="table table-sm table-striped table-bordered"
                        AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id"
                        OnRowEditing="gv_RowEditing" OnRowCancelingEdit="gv_RowCancelingEdit" OnRowUpdating="gvMecSust_RowUpdating">
            <Columns>
              <asp:BoundField DataField="id" HeaderText="ID" Visible="false" />
              <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="60px" ItemStyle-CssClass="text-center" ReadOnly="true" />
              <asp:BoundField DataField="descripcion" HeaderText="Descripcion" ReadOnly="true" />
              <asp:BoundField DataField="numparte" HeaderText="Num. Parte" ItemStyle-Width="120px" ReadOnly="true" />
              <asp:TemplateField HeaderText="Precio">
                <ItemTemplate>
                  <%# String.Format("{0:C2}", If(IsDBNull(Eval("precio")), 0, Eval("precio"))) %>
                </ItemTemplate>
                <EditItemTemplate>
                  <asp:TextBox ID="txtPrecio" runat="server" Text='<%# Bind("precio") %>' CssClass="form-control form-control-sm precio-input" />
                </EditItemTemplate>
                <ItemStyle Width="130px" CssClass="text-end" />
              </asp:TemplateField>
              <asp:CommandField ShowEditButton="True" ButtonType="Button"
                                ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                EditText="Editar" UpdateText="Guardar" CancelText="Cancelar" />
            </Columns>
          </asp:GridView>
        </div>

        <!-- Mecanica - Reparacion -->
        <div class="table-wrapper">
          <h6 class="subsection-title">Reparacion</h6>
          <asp:GridView ID="gvMecReparacion" runat="server" CssClass="table table-sm table-striped table-bordered"
                        AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id"
                        OnRowEditing="gv_RowEditing" OnRowCancelingEdit="gv_RowCancelingEdit" OnRowUpdating="gvMecRep_RowUpdating">
            <Columns>
              <asp:BoundField DataField="id" HeaderText="ID" Visible="false" />
              <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="60px" ItemStyle-CssClass="text-center" ReadOnly="true" />
              <asp:BoundField DataField="descripcion" HeaderText="Descripcion" ReadOnly="true" />
              <asp:BoundField DataField="observ1" HeaderText="Observaciones" ReadOnly="true" />
              <asp:TemplateField HeaderText="Precio">
                <ItemTemplate>
                  <%# String.Format("{0:C2}", If(IsDBNull(Eval("precio")), 0, Eval("precio"))) %>
                </ItemTemplate>
                <EditItemTemplate>
                  <asp:TextBox ID="txtPrecio" runat="server" Text='<%# Bind("precio") %>' CssClass="form-control form-control-sm precio-input" />
                </EditItemTemplate>
                <ItemStyle Width="130px" CssClass="text-end" />
              </asp:TemplateField>
              <asp:CommandField ShowEditButton="True" ButtonType="Button"
                                ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                EditText="Editar" UpdateText="Guardar" CancelText="Cancelar" />
            </Columns>
          </asp:GridView>
        </div>
      </div>

      <!-- Hojalateria -->
      <div class="grid-section">
        <h5 class="section-title warning"><i class="bi bi-tools"></i> Hojalateria</h5>

        <!-- Hojalateria - Sustitucion -->
        <div class="table-wrapper">
          <h6 class="subsection-title">Sustitucion</h6>
          <asp:GridView ID="gvHojSustitucion" runat="server" CssClass="table table-sm table-striped table-bordered"
                        AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id"
                        OnRowEditing="gv_RowEditing" OnRowCancelingEdit="gv_RowCancelingEdit" OnRowUpdating="gvHojSust_RowUpdating">
            <Columns>
              <asp:BoundField DataField="id" HeaderText="ID" Visible="false" />
              <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="60px" ItemStyle-CssClass="text-center" ReadOnly="true" />
              <asp:BoundField DataField="descripcion" HeaderText="Descripcion" ReadOnly="true" />
              <asp:BoundField DataField="numparte" HeaderText="Num. Parte" ItemStyle-Width="120px" ReadOnly="true" />
              <asp:TemplateField HeaderText="Precio">
                <ItemTemplate>
                  <%# String.Format("{0:C2}", If(IsDBNull(Eval("precio")), 0, Eval("precio"))) %>
                </ItemTemplate>
                <EditItemTemplate>
                  <asp:TextBox ID="txtPrecio" runat="server" Text='<%# Bind("precio") %>' CssClass="form-control form-control-sm precio-input" />
                </EditItemTemplate>
                <ItemStyle Width="130px" CssClass="text-end" />
              </asp:TemplateField>
              <asp:CommandField ShowEditButton="True" ButtonType="Button"
                                ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                EditText="Editar" UpdateText="Guardar" CancelText="Cancelar" />
            </Columns>
          </asp:GridView>
        </div>

        <!-- Hojalateria - Reparacion -->
        <div class="table-wrapper">
          <h6 class="subsection-title">Reparacion</h6>
          <asp:GridView ID="gvHojReparacion" runat="server" CssClass="table table-sm table-striped table-bordered"
                        AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id"
                        OnRowEditing="gv_RowEditing" OnRowCancelingEdit="gv_RowCancelingEdit" OnRowUpdating="gvHojRep_RowUpdating">
            <Columns>
              <asp:BoundField DataField="id" HeaderText="ID" Visible="false" />
              <asp:BoundField DataField="cantidad" HeaderText="Cant" ItemStyle-Width="60px" ItemStyle-CssClass="text-center" ReadOnly="true" />
              <asp:BoundField DataField="descripcion" HeaderText="Descripcion" ReadOnly="true" />
              <asp:BoundField DataField="observ1" HeaderText="Observaciones" ReadOnly="true" />
              <asp:TemplateField HeaderText="Precio">
                <ItemTemplate>
                  <%# String.Format("{0:C2}", If(IsDBNull(Eval("precio")), 0, Eval("precio"))) %>
                </ItemTemplate>
                <EditItemTemplate>
                  <asp:TextBox ID="txtPrecio" runat="server" Text='<%# Bind("precio") %>' CssClass="form-control form-control-sm precio-input" />
                </EditItemTemplate>
                <ItemStyle Width="130px" CssClass="text-end" />
              </asp:TemplateField>
              <asp:CommandField ShowEditButton="True" ButtonType="Button"
                                ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                EditText="Editar" UpdateText="Guardar" CancelText="Cancelar" />
            </Columns>
          </asp:GridView>
        </div>
      </div>

      <!-- Hidden fields -->
      <asp:HiddenField ID="hidId" runat="server" />
      <asp:HiddenField ID="hidExpediente" runat="server" />
    </div>
  </form>

  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
