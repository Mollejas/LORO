<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LeerOrden.aspx.vb" Inherits="TEST1.LeerOrden" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lectura de Orden de Admisión - Qualitas</title>
    <meta charset="utf-8" />
    <style type="text/css">
        /* Fondo general */
        body {
            margin: 0;
            padding: 0;
            font-family: "Segoe UI", Arial, sans-serif;
            background: linear-gradient(135deg, #4e54c8, #8f94fb);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* Tarjeta principal */
        .card {
            background: #ffffff;
            width: 900px;
            max-width: 95%;
            border-radius: 18px;
            box-shadow: 0 18px 40px rgba(0, 0, 0, 0.25);
            padding: 25px 35px 30px 35px;
            box-sizing: border-box;
            position: relative;
            overflow: hidden;
        }

        /* Cinta decorativa en la esquina */
        .card::before {
            content: "";
            position: absolute;
            top: -70px;
            right: -70px;
            width: 180px;
            height: 180px;
            background: radial-gradient(circle at center, #ff9800, #f44336);
            opacity: 0.28;
            transform: rotate(15deg);
        }

        h2 {
            margin: 0 0 5px 0;
            font-size: 26px;
            color: #333;
        }

        .subtitulo {
            margin-bottom: 20px;
            color: #777;
            font-size: 13px;
            letter-spacing: .5px;
            text-transform: uppercase;
        }

        hr {
            border: none;
            border-top: 1px solid #eee;
            margin: 15px 0 20px 0;
        }

        .fila {
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            flex-wrap: wrap;
        }

        .fila > * {
            margin-bottom: 4px;
        }

        .lblCampo {
            display: inline-block;
            width: 160px;
            font-weight: 600;
            color: #333;
            font-size: 13px;
        }

        input[type="text"], 
        .aspNetDisabled[type="text"] {
            width: 260px;
            padding: 6px 8px;
            border-radius: 6px;
            border: 1px solid #ccc;
            font-size: 13px;
            transition: all .2s ease;
            box-sizing: border-box;
        }

        input[type="text"]:focus {
            outline: none;
            border-color: #4e54c8;
            box-shadow: 0 0 0 2px rgba(78, 84, 200, 0.15);
        }

        /* Estilo FileUpload */
        .upload-container {
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 10px;
        }

        .upload-container input[type="file"] {
            font-size: 13px;
        }

        /* Botón principal */
        .btn-primary {
            background: linear-gradient(135deg, #ff9800, #f44336);
            border: none;
            color: #fff;
            padding: 8px 18px;
            border-radius: 50px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 8px 18px rgba(244, 67, 54, 0.35);
            text-transform: uppercase;
            letter-spacing: .7px;
            transition: all .2s ease;
        }

        .btn-primary:hover {
            transform: translateY(-1px);
            box-shadow: 0 12px 25px rgba(244, 67, 54, 0.45);
        }

        .btn-primary:active {
            transform: translateY(0);
            box-shadow: 0 6px 14px rgba(244, 67, 54, 0.30);
        }

        /* Mensajes */
        #lblMensaje {
            display: block;
            font-size: 13px;
            margin-top: 4px;
            min-height: 18px;
        }

        /* Secciones de datos */
        .seccion-titulo {
            font-size: 14px;
            font-weight: 700;
            color: #4e54c8;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin: 15px 0 8px 0;
        }

        .grid-datos {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            column-gap: 20px;
        }

        .grid-datos .fila {
            margin-bottom: 8px;
        }

        /* Responsive */
        @media (max-width: 600px) {
            .lblCampo {
                width: 100%;
                margin-bottom: 4px;
            }
            input[type="text"] {
                width: 100%;
            }
            .upload-container {
                flex-direction: column;
                align-items: flex-start;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="card">
            <h2>Lectura de Orden de Admisión</h2>
            <div class="subtitulo">Extracción automática de datos desde PDF de Qualitas</div>

            <div class="upload-container">
                <asp:FileUpload ID="fuPdf" runat="server" />
                <asp:Button ID="btnProcesar" runat="server"
                            Text="Procesar PDF"
                            CssClass="btn-primary"
                            OnClick="btnProcesar_Click" />
            </div>
            <asp:Label ID="lblMensaje" runat="server"></asp:Label>

            <hr />

            <div class="seccion-titulo">Datos del reporte</div>
            <div class="grid-datos">
                <div class="fila">
                    <span class="lblCampo">Número de reporte:</span>
                    <asp:TextBox ID="txtNumeroReporte" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">Nombre cliente:</span>
                    <asp:TextBox ID="txtNombreCliente" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">Teléfono:</span>
                    <asp:TextBox ID="txtTelefono" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">Email:</span>
                    <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                </div>
            </div>

            <div class="seccion-titulo">Datos del vehículo</div>
            <div class="grid-datos">
                <div class="fila">
                    <span class="lblCampo">Marca:</span>
                    <asp:TextBox ID="txtMarca" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">Tipo:</span>
                    <asp:TextBox ID="txtTipo" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">Modelo:</span>
                    <asp:TextBox ID="txtModelo" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">Color:</span>
                    <asp:TextBox ID="txtColor" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">VIN:</span>
                    <asp:TextBox ID="txtVin" runat="server"></asp:TextBox>
                </div>

                <div class="fila">
                    <span class="lblCampo">Placas:</span>
                    <asp:TextBox ID="txtPlacas" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
    </form>
</body>
</html>