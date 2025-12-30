<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ValuacionA.aspx.vb" Inherits="DaytonaERP.ValuacionA" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Hoja de Trabajo Autorizada - Relacionar Conceptos</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"/>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css"/>
    <style>
        body { font-family: Arial, sans-serif; font-size: 13px; }
        .concepto-row { cursor: pointer; transition: background 0.2s; }
        .concepto-row:hover { background: #f8f9fa; }
        .concepto-selected { background: #e7f3ff !important; border-left: 3px solid #0d6efd; }
        .refaccion-row { cursor: pointer; }
        .refaccion-active { background: #fff3cd; border-left: 3px solid #ffc107; }
        .concepto-asignado { background: #d1e7dd; }
        .badge-count { font-size: 11px; }
        .btn-sm-custom { padding: 0.25rem 0.5rem; font-size: 0.75rem; }
        .section-header { background: #f8f9fa; padding: 0.5rem; margin-bottom: 0.5rem; border-left: 4px solid #0d6efd; }
        .grid-container { max-height: calc(100vh - 200px); overflow-y: auto; }
        .sticky-header { position: sticky; top: 0; z-index: 10; background: white; }

        /* Estilos para cascada de conceptos */
        .cascada-container {
            margin-left: 20px;
            margin-top: 5px;
            margin-bottom: 10px;
            padding-left: 15px;
            border-left: 3px solid #0d6efd;
            background: #f8f9fa;
        }
        .cascada-item {
            padding: 5px 10px;
            margin: 3px 0;
            background: white;
            border-left: 3px solid #28a745;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 12px;
            animation: slideIn 0.3s ease-out;
        }
        @keyframes slideIn {
            from { opacity: 0; transform: translateX(-10px); }
            to { opacity: 1; transform: translateX(0); }
        }
        .cascada-item-concepto { flex-grow: 1; }
        .cascada-item-importe { font-weight: bold; color: #28a745; margin: 0 10px; }
        .cascada-item-remove {
            color: #dc3545;
            cursor: pointer;
            font-size: 14px;
            padding: 2px 6px;
        }
        .cascada-item-remove:hover { color: #bb2d3b; }
        .concepto-hidden { display: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid p-3">

            <!-- Encabezado -->
            <div class="row mb-3">
                <div class="col">
                    <h5 class="mb-1">Hoja de Trabajo Autorizada - Relacionar Conceptos</h5>
                    <p class="text-muted small mb-0">Expediente: <asp:Label ID="lblExpediente" runat="server" CssClass="fw-bold"></asp:Label></p>
                </div>
                <div class="col-auto">
                    <asp:Button ID="btnProcesarPDF" runat="server" CssClass="btn btn-primary btn-sm" Text="Extraer Conceptos del PDF" OnClick="btnProcesarPDF_Click" />
                    <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-success btn-sm" Text="Guardar Relaciones" OnClick="btnGuardar_Click" />
                </div>
            </div>

            <asp:Label ID="lblMensaje" runat="server" CssClass="alert alert-info d-block" Visible="False"></asp:Label>

            <!-- Dos columnas -->
            <div class="row">

                <!-- COLUMNA IZQUIERDA: Refacciones -->
                <div class="col-md-6 border-end">
                    <div class="section-header">
                        <h6 class="mb-0"><i class="bi bi-list-check"></i> Conceptos de Refacciones</h6>
                    </div>

                    <div class="grid-container">
                        <!-- Mecánica - Reparación -->
                        <h6 class="text-primary mt-3"><i class="bi bi-wrench"></i> Mecánica - Reparación</h6>
                        <asp:GridView ID="gvMecReparacion" runat="server" CssClass="table table-sm table-hover table-bordered"
                            AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                                <asp:TemplateField HeaderText="Conceptos" ItemStyle-Width="100px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="badge bg-secondary badge-count" data-refid='<%# Eval("id") %>'>0</span>
                                        <button type="button" class="btn btn-sm btn-outline-primary btn-sm-custom ms-1 btn-select-ref" data-refid='<%# Eval("id") %>'>
                                            <i class="bi bi-link"></i>
                                        </button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <!-- Mecánica - Sustitución -->
                        <h6 class="text-primary mt-3"><i class="bi bi-wrench"></i> Mecánica - Sustitución</h6>
                        <asp:GridView ID="gvMecSustitucion" runat="server" CssClass="table table-sm table-hover table-bordered"
                            AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="numparte" HeaderText="Num. Parte" ItemStyle-Width="120px" />
                                <asp:TemplateField HeaderText="Conceptos" ItemStyle-Width="100px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="badge bg-secondary badge-count" data-refid='<%# Eval("id") %>'>0</span>
                                        <button type="button" class="btn btn-sm btn-outline-primary btn-sm-custom ms-1 btn-select-ref" data-refid='<%# Eval("id") %>'>
                                            <i class="bi bi-link"></i>
                                        </button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <!-- Hojalatería - Reparación -->
                        <h6 class="text-warning mt-3"><i class="bi bi-tools"></i> Hojalatería - Reparación</h6>
                        <asp:GridView ID="gvHojReparacion" runat="server" CssClass="table table-sm table-hover table-bordered"
                            AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                                <asp:TemplateField HeaderText="Conceptos" ItemStyle-Width="100px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="badge bg-secondary badge-count" data-refid='<%# Eval("id") %>'>0</span>
                                        <button type="button" class="btn btn-sm btn-outline-primary btn-sm-custom ms-1 btn-select-ref" data-refid='<%# Eval("id") %>'>
                                            <i class="bi bi-link"></i>
                                        </button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <!-- Hojalatería - Sustitución -->
                        <h6 class="text-warning mt-3"><i class="bi bi-tools"></i> Hojalatería - Sustitución</h6>
                        <asp:GridView ID="gvHojSustitucion" runat="server" CssClass="table table-sm table-hover table-bordered"
                            AutoGenerateColumns="False" EmptyDataText="Sin registros" DataKeyNames="id">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="numparte" HeaderText="Num. Parte" ItemStyle-Width="120px" />
                                <asp:TemplateField HeaderText="Conceptos" ItemStyle-Width="100px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <span class="badge bg-secondary badge-count" data-refid='<%# Eval("id") %>'>0</span>
                                        <button type="button" class="btn btn-sm btn-outline-primary btn-sm-custom ms-1 btn-select-ref" data-refid='<%# Eval("id") %>'>
                                            <i class="bi bi-link"></i>
                                        </button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <!-- COLUMNA DERECHA: Conceptos del PDF -->
                <div class="col-md-6">
                    <div class="section-header">
                        <h6 class="mb-0"><i class="bi bi-file-pdf"></i> Conceptos del PDF de Valuación Autorizada</h6>
                        <small class="text-muted">Selecciona una refacción y haz clic en los conceptos del PDF para relacionarlos</small>
                    </div>

                    <div class="alert alert-warning" id="alertSeleccionar" style="display:none;">
                        <i class="bi bi-info-circle"></i> Selecciona una refacción de la izquierda primero
                    </div>

                    <div class="grid-container">
                        <asp:HiddenField ID="hfRefaccionSeleccionada" runat="server" ClientIDMode="Static" Value="" />

                        <!-- Refacciones del PDF -->
                        <h6 class="text-success mt-2"><i class="bi bi-gear"></i> Refacciones</h6>
                        <asp:GridView ID="gvPDFRefacciones" runat="server" CssClass="table table-sm table-hover table-bordered"
                            AutoGenerateColumns="False" EmptyDataText="Sin conceptos extraídos">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="concepto" HeaderText="Concepto" />
                                <asp:BoundField DataField="importe" HeaderText="Importe" ItemStyle-Width="100px" ItemStyle-CssClass="text-end" DataFormatString="{0:C2}" />
                                <asp:TemplateField HeaderText="Acción" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <button type="button" class="btn btn-sm btn-outline-success btn-sm-custom btn-asignar-concepto"
                                            data-conceptoid='<%# Eval("id") %>' data-seccion="REF">
                                            <i class="bi bi-check-lg"></i> Asignar
                                        </button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <!-- Pintura del PDF -->
                        <h6 class="text-success mt-3"><i class="bi bi-paint-bucket"></i> Pintura</h6>
                        <asp:GridView ID="gvPDFPintura" runat="server" CssClass="table table-sm table-hover table-bordered"
                            AutoGenerateColumns="False" EmptyDataText="Sin conceptos extraídos">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="concepto" HeaderText="Concepto" />
                                <asp:BoundField DataField="importe" HeaderText="Importe" ItemStyle-Width="100px" ItemStyle-CssClass="text-end" DataFormatString="{0:C2}" />
                                <asp:TemplateField HeaderText="Acción" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <button type="button" class="btn btn-sm btn-outline-success btn-sm-custom btn-asignar-concepto"
                                            data-conceptoid='<%# Eval("id") %>' data-seccion="PIN">
                                            <i class="bi bi-check-lg"></i> Asignar
                                        </button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <!-- Hojalatería del PDF -->
                        <h6 class="text-success mt-3"><i class="bi bi-hammer"></i> Hojalatería</h6>
                        <asp:GridView ID="gvPDFHojalateria" runat="server" CssClass="table table-sm table-hover table-bordered"
                            AutoGenerateColumns="False" EmptyDataText="Sin conceptos extraídos">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="concepto" HeaderText="Concepto" />
                                <asp:BoundField DataField="importe" HeaderText="Importe" ItemStyle-Width="100px" ItemStyle-CssClass="text-end" DataFormatString="{0:C2}" />
                                <asp:TemplateField HeaderText="Acción" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <button type="button" class="btn btn-sm btn-outline-success btn-sm-custom btn-asignar-concepto"
                                            data-conceptoid='<%# Eval("id") %>' data-seccion="HOJ">
                                            <i class="bi bi-check-lg"></i> Asignar
                                        </button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Objeto para almacenar relaciones temporales
        var relaciones = {}; // { refaccion_id: [concepto_id1, concepto_id2, ...] }
        var conceptosData = {}; // { concepto_id: { concepto: 'texto', importe: 123.45 } }
        var refaccionSeleccionada = null;

        document.addEventListener('DOMContentLoaded', function() {

            // Inicializar contenedores de cascada
            inicializarCascadas();

            // Cargar datos de conceptos
            cargarDatosConceptos();

            // Cargar relaciones existentes
            cargarRelacionesExistentes();

            // Click en botón "Seleccionar" refacción
            document.querySelectorAll('.btn-select-ref').forEach(btn => {
                btn.addEventListener('click', function(e) {
                    e.preventDefault();
                    const refId = this.dataset.refid;
                    seleccionarRefaccion(refId);
                });
            });

            // Click en fila de refacción
            document.querySelectorAll('.table tbody tr').forEach(row => {
                row.addEventListener('click', function(e) {
                    if (e.target.closest('.btn-select-ref')) return;
                    const btn = this.querySelector('.btn-select-ref');
                    if (btn) {
                        const refId = btn.dataset.refid;
                        seleccionarRefaccion(refId);
                    }
                });
            });

            // Click en botón "Asignar" concepto
            document.querySelectorAll('.btn-asignar-concepto').forEach(btn => {
                btn.addEventListener('click', function(e) {
                    e.preventDefault();
                    if (!refaccionSeleccionada) {
                        document.getElementById('alertSeleccionar').style.display = 'block';
                        setTimeout(() => {
                            document.getElementById('alertSeleccionar').style.display = 'none';
                        }, 3000);
                        return;
                    }
                    const conceptoId = this.dataset.conceptoid;
                    asignarConcepto(refaccionSeleccionada, conceptoId);
                });
            });
        });

        function inicializarCascadas() {
            // Agregar contenedores de cascada después de cada tabla de refacciones
            const tables = document.querySelectorAll('.col-md-6.border-end .table');
            tables.forEach(table => {
                const refIds = [];
                table.querySelectorAll('.btn-select-ref').forEach(btn => {
                    refIds.push(btn.dataset.refid);
                });

                refIds.forEach(refId => {
                    const btn = table.querySelector('.btn-select-ref[data-refid="' + refId + '"]');
                    if (btn) {
                        const row = btn.closest('tr');
                        if (row && !row.nextElementSibling?.classList.contains('cascada-row')) {
                            const cascadaRow = document.createElement('tr');
                            cascadaRow.className = 'cascada-row';
                            cascadaRow.innerHTML = '<td colspan="100"><div class="cascada-container" id="cascada-' + refId + '"></div></td>';
                            row.parentNode.insertBefore(cascadaRow, row.nextSibling);
                        }
                    }
                });
            });
        }

        function cargarDatosConceptos() {
            // Extraer datos de todos los conceptos de los grids del PDF
            document.querySelectorAll('.btn-asignar-concepto').forEach(btn => {
                const conceptoId = btn.dataset.conceptoid;
                const row = btn.closest('tr');
                if (row) {
                    const cells = row.querySelectorAll('td');
                    if (cells.length >= 3) {
                        const concepto = cells[1].textContent.trim();
                        const importeText = cells[2].textContent.trim();
                        conceptosData[conceptoId] = {
                            concepto: concepto,
                            importe: importeText
                        };
                    }
                }
            });
        }

        function seleccionarRefaccion(refId) {
            // Remover selección anterior
            document.querySelectorAll('.refaccion-active').forEach(el => el.classList.remove('refaccion-active'));

            // Marcar como seleccionada
            refaccionSeleccionada = refId;
            document.getElementById('hfRefaccionSeleccionada').value = refId;

            // Encontrar y marcar la fila
            const btn = document.querySelector('.btn-select-ref[data-refid="' + refId + '"]');
            if (btn) {
                const row = btn.closest('tr');
                if (row) row.classList.add('refaccion-active');
            }

            console.log('Refacción seleccionada:', refId);
        }

        function asignarConcepto(refId, conceptoId) {
            if (!relaciones[refId]) relaciones[refId] = [];

            // Verificar si ya está asignado
            if (relaciones[refId].includes(conceptoId)) {
                // Desasignar
                relaciones[refId] = relaciones[refId].filter(id => id !== conceptoId);
            } else {
                // Asignar
                relaciones[refId].push(conceptoId);
            }

            actualizarUI(refId, conceptoId);
            console.log('Relaciones actuales:', relaciones);
        }

        function actualizarUI(refId, conceptoId) {
            // Actualizar contador de badges
            const badge = document.querySelector('.badge-count[data-refid="' + refId + '"]');
            if (badge) {
                const count = relaciones[refId] ? relaciones[refId].length : 0;
                badge.textContent = count;
                badge.className = count > 0 ? 'badge bg-success badge-count' : 'badge bg-secondary badge-count';
            }

            // Obtener la fila del concepto
            const btnConcepto = document.querySelector('.btn-asignar-concepto[data-conceptoid="' + conceptoId + '"]');
            if (btnConcepto) {
                const row = btnConcepto.closest('tr');
                if (row) {
                    const estaAsignado = relaciones[refId] && relaciones[refId].includes(conceptoId);

                    if (estaAsignado) {
                        // OCULTAR la fila del grid
                        row.classList.add('concepto-hidden');
                        // AGREGAR a cascada
                        agregarACascada(refId, conceptoId);
                    } else {
                        // MOSTRAR la fila del grid
                        row.classList.remove('concepto-hidden');
                        // QUITAR de cascada
                        quitarDeCascada(refId, conceptoId);
                    }
                }
            }
        }

        function agregarACascada(refId, conceptoId) {
            const cascada = document.getElementById('cascada-' + refId);
            if (!cascada) return;

            const datos = conceptosData[conceptoId];
            if (!datos) return;

            // Crear elemento de cascada
            const item = document.createElement('div');
            item.className = 'cascada-item';
            item.id = 'cascada-item-' + conceptoId;
            item.innerHTML = `
                <span class="cascada-item-concepto">${datos.concepto}</span>
                <span class="cascada-item-importe">${datos.importe}</span>
                <span class="cascada-item-remove" onclick="removerDeCascada(${refId}, ${conceptoId})" title="Desasignar">
                    <i class="bi bi-x-circle-fill"></i>
                </span>
            `;

            cascada.appendChild(item);
        }

        function quitarDeCascada(refId, conceptoId) {
            const item = document.getElementById('cascada-item-' + conceptoId);
            if (item) {
                item.remove();
            }
        }

        function removerDeCascada(refId, conceptoId) {
            // Desasignar el concepto
            asignarConcepto(refId, conceptoId);
        }

        function cargarRelacionesExistentes() {
            // Cargar relaciones desde el servidor
            if (window.relacionesIniciales) {
                relaciones = window.relacionesIniciales;

                // Aplicar las relaciones cargadas a la UI
                for (const refId in relaciones) {
                    const conceptos = relaciones[refId];
                    conceptos.forEach(conceptoId => {
                        actualizarUI(refId, conceptoId);
                    });
                }

                console.log('Relaciones cargadas desde BD:', relaciones);
            }
        }

        // Función para preparar datos antes de guardar
        function prepararDatosGuardar() {
            // Convertir objeto relaciones a JSON y guardarlo en un hidden field
            const jsonRelaciones = JSON.stringify(relaciones);
            console.log('Guardando relaciones:', jsonRelaciones);

            // Crear un hidden field dinámico para enviar los datos
            let hfRelaciones = document.getElementById('hfRelaciones');
            if (!hfRelaciones) {
                hfRelaciones = document.createElement('input');
                hfRelaciones.type = 'hidden';
                hfRelaciones.id = 'hfRelaciones';
                hfRelaciones.name = 'hfRelaciones';
                document.forms[0].appendChild(hfRelaciones);
            }
            hfRelaciones.value = jsonRelaciones;
        }

        // Agregar evento al botón guardar
        const btnGuardar = document.querySelector('[id$="btnGuardar"]');
        if (btnGuardar) {
            btnGuardar.addEventListener('click', function(e) {
                prepararDatosGuardar();
            });
        }
    </script>
</body>
</html>
