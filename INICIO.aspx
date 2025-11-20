<%@ Page Title="Inicio" Language="VB" MasterPageFile="~/Site1.Master" AutoEventWireup="false" CodeBehind="Inicio.aspx.vb" Inherits="DAYTONAMIO.Inicio" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .form-section {
            background: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            max-width: 800px;
            margin: auto;
        }
        .form-label {
            font-weight: 600;
        }
        .btn-search {
            width: 150px;
            font-weight: 600;
        }
        /* Forzar espacio entre últimos TextBox y botón */
        .btn-row {
            margin-top: 30px; /* <<--- aquí controlas la separación */
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="form-section">
        <h3 class="mb-4">Buscar Carpeta</h3>

        <!-- Primera fila -->
        <div class="row mb-3">
            <div class="col-md-6">
                <label for="txtCarpeta" class="form-label">No. Carpeta</label>
                <asp:TextBox ID="txtCarpeta" runat="server" CssClass="form-control" placeholder="Ingrese No. Carpeta"></asp:TextBox>
            </div>
            <div class="col-md-6">
                <label for="txtPlaca" class="form-label">Placa</label>
                <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" placeholder="Ingrese Placa"></asp:TextBox>
            </div>
        </div>

        <!-- Segunda fila -->
        <div class="row">
            <div class="col-md-6">
                <label for="txtSiniestro" class="form-label">Siniestro</label>
                <asp:TextBox ID="txtSiniestro" runat="server" CssClass="form-control" placeholder="Ingrese Siniestro"></asp:TextBox>
            </div>
            <div class="col-md-6">
                <label for="txtVIN" class="form-label">VIN</label>
                <asp:TextBox ID="txtVIN" runat="server" CssClass="form-control" placeholder="Ingrese VIN"></asp:TextBox>
            </div>
        </div>

        <!-- Botón separado con CSS forzado -->
        <div class="row btn-row">
            <div class="col text-center">
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary btn-search" />
            </div>
        </div>
    </div>
</asp:Content>
