<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Valuacion.aspx.vb" Inherits="DAYTONAMIO.Valuacion" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Valuación de Seguros</title>
    
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
            min-height: 100vh;
        }

        .container {
            max-width: 1400px;
            margin: 0 auto;
            background: white;
            border-radius: 15px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            overflow: hidden;
        }

        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }

        .header h1 {
            font-size: 2.5em;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
        }

        .header p {
            font-size: 1.1em;
            opacity: 0.9;
        }

        .upload-section {
            padding: 30px;
            background: #f8f9fa;
            border-bottom: 3px solid #e9ecef;
        }

        .upload-container {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 15px;
            flex-wrap: wrap;
        }

        .file-input-wrapper {
            position: relative;
            overflow: hidden;
            display: inline-block;
        }

        .file-input-wrapper input[type=file] {
            position: absolute;
            left: -9999px;
        }

        .file-input-label {
            display: inline-block;
            padding: 12px 30px;
            background: white;
            border: 2px dashed #667eea;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s ease;
            font-weight: 600;
            color: #667eea;
        }

        .file-input-label:hover {
            background: #667eea;
            color: white;
            border-style: solid;
        }

        .btn-procesar {
            padding: 12px 40px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
        }

        .btn-procesar:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(102, 126, 234, 0.6);
        }

        .btn-procesar:active {
            transform: translateY(0);
        }

        .content-section {
            padding: 30px;
        }

        .seccion-titulo {
            display: flex;
            align-items: center;
            margin: 30px 0 20px 0;
            padding-bottom: 10px;
            border-bottom: 3px solid;
        }

        .seccion-titulo h2 {
            font-size: 1.8em;
            margin-right: 15px;
        }

        .seccion-titulo .badge {
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 0.9em;
            font-weight: 600;
        }

        .refacciones-titulo {
            border-color: #3498db;
            color: #3498db;
        }

        .refacciones-titulo .badge {
            background: #3498db;
            color: white;
        }

        .pintura-titulo {
            border-color: #e74c3c;
            color: #e74c3c;
        }

        .pintura-titulo .badge {
            background: #e74c3c;
            color: white;
        }

        .hojalateria-titulo {
            border-color: #f39c12;
            color: #f39c12;
        }

        .hojalateria-titulo .badge {
            background: #f39c12;
            color: white;
        }

        /* ESTILOS PARA GRIDVIEW */
        .grid-wrapper {
            overflow-x: auto;
            margin-bottom: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .grid-valuacion {
            width: 100%;
            border-collapse: collapse;
            background: white;
            font-size: 14px;
        }

        .grid-valuacion th {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px;
            text-align: left;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            font-size: 13px;
            position: sticky;
            top: 0;
            z-index: 10;
        }

        .grid-valuacion td {
            padding: 12px 15px;
            border-bottom: 1px solid #e9ecef;
            color: #2c3e50;
        }

        .grid-valuacion tr:hover {
            background-color: #f8f9fa;
            transition: background-color 0.2s ease;
        }

        .grid-valuacion tr:last-child td {
            border-bottom: none;
        }

        /* Alternar colores de filas */
        .grid-valuacion tr:nth-child(even) {
            background-color: #f8f9fa;
        }

        .grid-valuacion tr:nth-child(odd) {
            background-color: white;
        }

        .grid-valuacion tr:nth-child(even):hover,
        .grid-valuacion tr:nth-child(odd):hover {
            background-color: #e3f2fd;
        }

        /* Columna de descripción más ancha */
        .grid-valuacion td:first-child {
            font-weight: 500;
            color: #34495e;
        }

        /* Columna de monto alineada a la derecha */
        .grid-valuacion td:last-child {
            text-align: right;
            font-weight: 600;
            color: #27ae60;
            font-family: 'Courier New', monospace;
        }

        /* TOTALES */
        .totales-box {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 10px;
            margin-top: 15px;
            box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
        }

        .totales-box h3 {
            margin-bottom: 15px;
            font-size: 1.2em;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .totales-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
        }

        .total-item {
            background: rgba(255, 255, 255, 0.2);
            padding: 15px;
            border-radius: 8px;
            backdrop-filter: blur(10px);
        }

        .total-item .label {
            font-size: 0.9em;
            opacity: 0.9;
            margin-bottom: 5px;
        }

        .total-item .valor {
            font-size: 1.8em;
            font-weight: 700;
            font-family: 'Courier New', monospace;
        }

        /* Mensaje cuando no hay datos */
        .no-data {
            text-align: center;
            padding: 40px;
            color: #95a5a6;
            font-style: italic;
        }

        /* Responsive */
        @media (max-width: 768px) {
            .header h1 {
                font-size: 1.8em;
            }

            .upload-container {
                flex-direction: column;
            }

            .grid-valuacion {
                font-size: 12px;
            }

            .grid-valuacion th,
            .grid-valuacion td {
                padding: 8px 10px;
            }

            .totales-grid {
                grid-template-columns: 1fr;
            }
        }

        /* Animación de carga */
        @keyframes pulse {
            0%, 100% { opacity: 1; }
            50% { opacity: 0.5; }
        }

        .loading {
            animation: pulse 1.5s infinite;
        }

        /* Scrollbar personalizado */
        .grid-wrapper::-webkit-scrollbar {
            height: 8px;
        }

        .grid-wrapper::-webkit-scrollbar-track {
            background: #f1f1f1;
            border-radius: 10px;
        }

        .grid-wrapper::-webkit-scrollbar-thumb {
            background: #667eea;
            border-radius: 10px;
        }

        .grid-wrapper::-webkit-scrollbar-thumb:hover {
            background: #764ba2;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            
            <!-- HEADER -->
            <div class="header">
                <h1>📋 Valuación de Seguros</h1>
                <p>Sistema de Procesamiento de PDF - Qualitas</p>
            </div>

            <!-- SECCIÓN DE CARGA -->
            <div class="upload-section">
                <div class="upload-container">
                    <div class="file-input-wrapper">
                        <label class="file-input-label" for="fuPdf">
                            📁 Seleccionar PDF
                        </label>
                        <asp:FileUpload ID="fuPdf" runat="server" accept=".pdf" ClientIDMode="Static" />
                    </div>
                    <asp:Button ID="btnProcesar" runat="server" Text="🚀 Procesar Valuación" CssClass="btn-procesar" />
                </div>
            </div>

            <!-- CONTENIDO -->
            <div class="content-section">

                <!-- REFACCIONES -->
                <div class="seccion-titulo refacciones-titulo">
                    <h2>🔧 Refacciones</h2>
                    <span class="badge">Partes y Componentes</span>
                </div>
                <div class="grid-wrapper">
                    <asp:GridView ID="gvRefacciones" runat="server" 
                        CssClass="grid-valuacion" 
                        AutoGenerateColumns="False"
                        EmptyDataText="No hay datos de refacciones">
                        <Columns>
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:C2}" />
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="totales-box">
                    <h3>💰 Totales Refacciones</h3>
                    <asp:Label ID="lblTotRef" runat="server" CssClass="total-valor"></asp:Label>
                </div>

                <!-- PINTURA -->
                <div class="seccion-titulo pintura-titulo">
                    <h2>🎨 Pintura</h2>
                    <span class="badge">Trabajos de Pintura</span>
                </div>
                <div class="grid-wrapper">
                    <asp:GridView ID="gvPintura" runat="server" 
                        CssClass="grid-valuacion" 
                        AutoGenerateColumns="False"
                        EmptyDataText="No hay datos de pintura">
                        <Columns>
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:C2}" />
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="totales-box">
                    <h3>💰 Totales Pintura</h3>
                    <asp:Label ID="lblTotPintura" runat="server" CssClass="total-valor"></asp:Label>
                </div>

                <!-- HOJALATERÍA -->
                <div class="seccion-titulo hojalateria-titulo">
                    <h2>🔨 Hojalatería</h2>
                    <span class="badge">Mano de Obra</span>
                </div>
                <div class="grid-wrapper">
                    <asp:GridView ID="gvHojalateria" runat="server" 
                        CssClass="grid-valuacion" 
                        AutoGenerateColumns="False"
                        EmptyDataText="No hay datos de hojalatería">
                        <Columns>
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:C2}" />
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="totales-box">
                    <h3>💰 Totales Hojalatería</h3>
                    <asp:Label ID="lblTotHoj" runat="server" CssClass="total-valor"></asp:Label>
                </div>

            </div>
        </div>

        <script>
            // Mostrar nombre del archivo seleccionado
            document.getElementById('fuPdf').addEventListener('change', function(e) {
                var fileName = e.target.files[0]?.name || 'Seleccionar PDF';
                var label = document.querySelector('.file-input-label');
                if (e.target.files[0]) {
                    label.textContent = '✅ ' + fileName;
                    label.style.background = '#27ae60';
                    label.style.color = 'white';
                    label.style.borderColor = '#27ae60';
                } else {
                    label.textContent = '📁 Seleccionar PDF';
                    label.style.background = 'white';
                    label.style.color = '#667eea';
                    label.style.borderColor = '#667eea';
                }
            });
        </script>
    </form>
</body>
</html>