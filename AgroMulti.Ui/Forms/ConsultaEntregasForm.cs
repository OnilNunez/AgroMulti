using AgroMulti;
using AgroMulti.Data.Models;
using AgroMulti.Ui.Services;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class ConsultaEntregasForm : Form
    {
        // ── Servicios ────────────────────────────────────────────────
        private readonly ProductorService _productorService;
        private readonly ProductoService _productoService;
        private readonly EstadoEntregaService _estadoEntregaService;
        private readonly EntregaService _entregaService;
        private readonly HistoricoEstadoEntregaService _historicoService;

        // ── Última consulta (para exportar exactamente lo visible) ───
        private List<Entrega> _ultimosResultados = new();

        // ── Colores de estado ─────────────────────────────────────────
        private static readonly (string Clave, System.Drawing.Color Color)[] _coloresEstado =
        {
            ("complet", System.Drawing.Color.FromArgb(72,  118,  28)),
            ("finaliz", System.Drawing.Color.FromArgb(72,  118,  28)),
            ("listo",   System.Drawing.Color.FromArgb(72,  118,  28)),
            ("ferment", System.Drawing.Color.FromArgb(160,  90,  20)),
            ("secad",   System.Drawing.Color.FromArgb(140, 100,  30)),
            ("secan",   System.Drawing.Color.FromArgb(140, 100,  30)),
            ("control", System.Drawing.Color.FromArgb(130,  80, 160)),
            ("calidad", System.Drawing.Color.FromArgb(130,  80, 160)),
            ("pend",    System.Drawing.Color.FromArgb(170, 120,  40)),
            ("espera",  System.Drawing.Color.FromArgb(170, 120,  40)),
            ("cancel",  System.Drawing.Color.FromArgb(180,  55,  35)),
            ("rechaz",  System.Drawing.Color.FromArgb(180,  55,  35)),
        };

        // ── Constructor ──────────────────────────────────────────────
        public ConsultaEntregasForm()
        {
            InitializeComponent();
            cboEstado.Items.Clear();

            _productorService = Program.ServiceProvider.GetRequiredService<ProductorService>();
            _productoService = Program.ServiceProvider.GetRequiredService<ProductoService>();
            _estadoEntregaService = Program.ServiceProvider.GetRequiredService<EstadoEntregaService>();
            _entregaService = Program.ServiceProvider.GetRequiredService<EntregaService>();
            _historicoService = Program.ServiceProvider.GetRequiredService<HistoricoEstadoEntregaService>();

            QuestPDF.Settings.License = LicenseType.Community;

            Load += async (s, e) => await InicializarAsync();
        }

        // ── Inicialización ────────────────────────────────────────────
        private async Task InicializarAsync()
        {
            await CargarCombosAsync();
            LimpiarFiltros();
            await CargarResultadosAsync();
        }

        private async Task CargarCombosAsync()
        {
            try
            {
                var productores = (await _productorService.GetList(p => true))
                    .OrderBy(p => p.Codigo).ToList();
                productores.Insert(0, new Productor { ProductorId = 0, Codigo = "", Nombre = "(Todos)", Apellido = "" });
                cboProductor.DataSource = productores;
                cboProductor.DisplayMember = "Codigo";
                cboProductor.ValueMember = "ProductorId";

                var productos = (await _productoService.GetList(p => true))
                    .OrderBy(p => p.Nombre).ToList();
                productos.Insert(0, new Producto { ProductoId = 0, Nombre = "(Todos)" });
                cboProducto.DataSource = productos;
                cboProducto.DisplayMember = "Nombre";
                cboProducto.ValueMember = "ProductoId";

                var estados = (await _estadoEntregaService.GetList(e => true))
                    .OrderBy(e => e.Nombre).ToList();
                estados.Insert(0, new EstadoEntrega { EstadoEntregaId = 0, Nombre = "(Todos)" });
                cboEstado.DataSource = estados;
                cboEstado.DisplayMember = "Nombre";
                cboEstado.ValueMember = "EstadoEntregaId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las listas de filtros: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFiltros()
        {
            dtpFechaDesde.Value = DateTime.Today.AddDays(-30);
            dtpFechaHasta.Value = DateTime.Today;
            cboProductor.SelectedIndex = 0;
            cboProducto.SelectedIndex = 0;
            cboEstado.SelectedIndex = 0;
        }

        // ── Botones de acción ────────────────────────────────────────
        private async void btnBuscar_Click(object sender, EventArgs e) => await CargarResultadosAsync();
        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private async void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            LimpiarFiltros();
            dgvEntregas.Rows.Clear();
            _ultimosResultados.Clear();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            var formHistorial = new HistoricoEstadosForm();
            formHistorial.ShowDialog(this);
        }

        // ── Exportar: muestra el menú contextual ─────────────────────
        private void BtnExportar_Click(object sender, EventArgs e)
        {
            ctxExportar.Show(btnExportar, new System.Drawing.Point(0, btnExportar.Height));
        }

        // ── Exportar a Excel ──────────────────────────────────────────
        private void ItemExportarExcel_Click(object sender, EventArgs e)
        {
            if (_ultimosResultados == null || _ultimosResultados.Count == 0)
            {
                MessageBox.Show("No hay resultados para exportar. Realice una búsqueda primero.",
                    "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar entregas a Excel",
                Filter = "Libro de Excel (*.xlsx)|*.xlsx",
                FileName = $"ConsultaEntregas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                DefaultExt = "xlsx"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Entregas");

                // ── Encabezados ───────────────────────────────────────
                string[] headers =
                {
                    "Número", "Fecha", "Productor", "Producto",
                    "Subproducto", "Kilos", "Estado", "Lugar en almacén", "Observaciones"
                };

                for (int c = 0; c < headers.Length; c++)
                {
                    var cell = ws.Cell(1, c + 1);
                    cell.Value = headers[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(58, 38, 18);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // ── Datos ─────────────────────────────────────────────
                int row = 2;
                foreach (var en in _ultimosResultados)
                {
                    string estado = en.EstadoEntrega?.Nombre ?? "—";
                    var colorEst = ObtenerColorEstado(estado.ToLowerInvariant());
                    string productor = $"{en.Productor?.Nombre} {en.Productor?.Apellido}".Trim();
                    string lugar = ObtenerLugar(en);

                    ws.Cell(row, 1).Value = en.NumeroEntrega ?? "—";
                    ws.Cell(row, 2).Value = en.FechaEntrega.ToString("dd/MM/yyyy");
                    ws.Cell(row, 3).Value = productor;
                    ws.Cell(row, 4).Value = en.Producto?.Nombre ?? "—";
                    ws.Cell(row, 5).Value = en.SubProducto?.Nombre ?? "—";
                    ws.Cell(row, 6).Value = en.Kilos;
                    ws.Cell(row, 7).Value = estado;
                    ws.Cell(row, 8).Value = lugar;
                    ws.Cell(row, 9).Value = string.IsNullOrWhiteSpace(en.Observaciones) ? "—" : en.Observaciones;

                    ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromArgb(colorEst.R, colorEst.G, colorEst.B);
                    ws.Cell(row, 7).Style.Font.Bold = true;

                    if (row % 2 == 0)
                        ws.Range(row, 1, row, 9).Style.Fill.BackgroundColor =
                            XLColor.FromArgb(250, 247, 242);

                    row++;
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(sfd.FileName);

                MessageBox.Show($"Exportado correctamente:\n{sfd.FileName}",
                    "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Exportar a PDF ────────────────────────────────────────────
        private void ItemExportarPDF_Click(object sender, EventArgs e)
        {
            if (_ultimosResultados == null || _ultimosResultados.Count == 0)
            {
                MessageBox.Show("No hay resultados para exportar. Realice una búsqueda primero.",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar entregas a PDF",
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = $"ConsultaEntregas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                DefaultExt = "pdf"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var datos = _ultimosResultados; // captura local para el lambda

                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A3.Landscape());
                        page.Margin(1.2f, Unit.Centimetre);
                        page.PageColor(Colors.White);

                        // ── Encabezado ─────────────────────────────────
                        page.Header()
                            .Background("#26160A")
                            .Padding(10)
                            .Column(col =>
                            {
                                col.Item().Text("Consulta de Entregas")
                                    .FontFamily("Segoe UI").FontSize(18).Bold()
                                    .FontColor(Colors.White);
                                col.Item().Text("Centro de Fermentación y Secado")
                                    .FontSize(10).FontColor("#B9A58C");
                                col.Item()
                                    .Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  Total: {datos.Count} registros")
                                    .FontSize(9).FontColor("#B9A58C");
                            });

                        // ── Tabla ──────────────────────────────────────
                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1.3f); // Número
                                cols.RelativeColumn(1.1f); // Fecha
                                cols.RelativeColumn(2.0f); // Productor
                                cols.RelativeColumn(1.4f); // Producto
                                cols.RelativeColumn(1.2f); // Subproducto
                                cols.RelativeColumn(0.9f); // Kilos
                                cols.RelativeColumn(1.4f); // Estado
                                cols.RelativeColumn(1.8f); // Lugar
                                cols.RelativeColumn(2.9f); // Observaciones
                            });

                            table.Header(header =>
                            {
                                foreach (string h in new[]
                                {
                                    "Número", "Fecha", "Productor", "Producto",
                                    "Subproducto", "Kilos", "Estado", "Lugar", "Observaciones"
                                })
                                {
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text(h).FontColor("#FFFFFF").Bold().FontSize(8.5f);
                                }
                            });

                            foreach (var en in datos)
                            {
                                string estado = en.EstadoEntrega?.Nombre ?? "—";
                                var colorEst = ObtenerColorEstado(estado.ToLowerInvariant());
                                string colorHex = $"#{colorEst.R:X2}{colorEst.G:X2}{colorEst.B:X2}";
                                string productor = $"{en.Productor?.Nombre} {en.Productor?.Apellido}".Trim();
                                string lugar = ObtenerLugar(en);

                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(en.NumeroEntrega ?? "—").Bold().FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(en.FechaEntrega.ToString("dd/MM/yyyy")).FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(productor).FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(en.Producto?.Nombre ?? "—").FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(en.SubProducto?.Nombre ?? "—").FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).AlignRight()
                                    .Text(en.Kilos.ToString("N2")).FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(estado).FontColor(colorHex).Bold().FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(lugar).FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3)
                                    .Text(string.IsNullOrWhiteSpace(en.Observaciones) ? "—" : en.Observaciones)
                                    .FontSize(8.5f);
                            }
                        });

                        // ── Pie de página ──────────────────────────────
                        page.Footer().AlignRight().Text(text =>
                        {
                            text.Span("Página ").FontSize(8).FontColor("#6B4C32");
                            text.CurrentPageNumber().FontSize(8).FontColor("#6B4C32");
                            text.Span(" de ").FontSize(8).FontColor("#6B4C32");
                            text.TotalPages().FontSize(8).FontColor("#6B4C32");
                        });
                    });
                }).GeneratePdf(sfd.FileName);

                MessageBox.Show($"Exportado correctamente:\n{sfd.FileName}",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Carga de resultados ───────────────────────────────────────
        private async Task CargarResultadosAsync()
        {
            try
            {
                DateOnly desde = DateOnly.FromDateTime(dtpFechaDesde.Value.Date);
                DateOnly hasta = DateOnly.FromDateTime(dtpFechaHasta.Value.Date);
                int productorId = cboProductor.SelectedValue != null ? (int)cboProductor.SelectedValue : 0;
                int productoId = cboProducto.SelectedValue != null ? (int)cboProducto.SelectedValue : 0;
                int estadoId = cboEstado.SelectedValue != null ? (int)cboEstado.SelectedValue : 0;

                var todas = await _entregaService.GetListConRelaciones(e =>
                    e.FechaEntrega >= desde && e.FechaEntrega <= hasta);

                var query = todas.AsEnumerable();
                if (productorId > 0) query = query.Where(e => e.ProductorId == productorId);
                if (productoId > 0) query = query.Where(e => e.ProductoId == productoId);
                if (estadoId > 0) query = query.Where(e => e.EstadoEntregaId == estadoId);

                _ultimosResultados = query
                    .OrderByDescending(e => e.FechaEntrega)
                    .ThenByDescending(e => e.EntregaId)
                    .ToList();

                dgvEntregas.Rows.Clear();

                foreach (var entrega in _ultimosResultados)
                {
                    dgvEntregas.Rows.Add(
                        entrega.EntregaId,
                        entrega.NumeroEntrega,
                        entrega.FechaEntrega.ToString("dd/MM/yyyy"),
                        $"{entrega.Productor.Nombre} {entrega.Productor.Apellido}",
                        entrega.Producto.Nombre,
                        entrega.SubProducto?.Nombre ?? "",
                        entrega.Kilos.ToString("N2"),
                        entrega.EstadoEntrega.Nombre,
                        entrega.Observaciones ?? ""
                    );
                }

                dgvEntregas.ClearSelection();

                // Habilita/deshabilita exportar según haya datos
                btnExportar.Enabled = _ultimosResultados.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar entregas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Modificar estado ──────────────────────────────────────────
        private async void btnModificarEstado_Click(object sender, EventArgs e)
        {
            if (dgvEntregas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una entrega de la lista para modificar su estado.",
                    "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgvEntregas.SelectedRows[0];
            int entregaId = Convert.ToInt32(row.Cells["colEntregaId"].Value);
            string estadoActual = row.Cells["colEstado"].Value?.ToString() ?? "";

            string[] transiciones = ObtenerTransicionesValidas(estadoActual);

            if (transiciones.Length == 0)
            {
                MessageBox.Show(
                    $"La entrega se encuentra en estado {estadoActual}.\nEste es un estado final y no puede modificarse.",
                    "Estado final", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string? nuevoEstado = MostrarDialogoCambioEstado(estadoActual, transiciones);
            if (nuevoEstado == null) return;

            try
            {
                var entrega = await _entregaService.Buscar(entregaId);
                if (entrega == null)
                {
                    MessageBox.Show("No se encontró la entrega en la base de datos.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var estadosCoincidentes = await _estadoEntregaService.GetList(es =>
                    es.Nombre.ToLower() == nuevoEstado.ToLower());
                var estadoNuevo = estadosCoincidentes.FirstOrDefault();

                if (estadoNuevo == null)
                {
                    MessageBox.Show(
                        $"No se encontró el estado «{nuevoEstado}» en la base de datos.\nVerifique que los nombres de estados coincidan exactamente.",
                        "Estado no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string estadoAnteriorNombre = estadoActual;
                entrega.EstadoEntregaId = estadoNuevo.EstadoEntregaId;
                bool guardado = await _entregaService.Guardar(entrega);

                if (!guardado)
                {
                    MessageBox.Show("No se pudo actualizar la entrega.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var nuevoHistorial = new HistoricoEstadoEntrega
                {
                    EntregaId = entregaId,
                    EstadoEntregaId = estadoNuevo.EstadoEntregaId,
                    Observaciones = $"Cambio de estado de '{estadoAnteriorNombre}' a '{nuevoEstado}'"
                };
                await _historicoService.Guardar(nuevoHistorial);

                MessageBox.Show(
                    $"Estado actualizado correctamente a {estadoNuevo.Nombre}.\nEl cambio ha sido registrado en el historial.",
                    "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await CargarResultadosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el estado: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────
        private static string ObtenerLugar(Entrega en)
        {
            var partes = new List<string>(3);
            if (!string.IsNullOrWhiteSpace(en.Pasillo)) partes.Add(en.Pasillo);
            if (!string.IsNullOrWhiteSpace(en.NumeroAnaquel)) partes.Add(en.NumeroAnaquel);
            if (!string.IsNullOrWhiteSpace(en.Piso)) partes.Add(en.Piso);
            return partes.Count > 0 ? string.Join(" · ", partes) : "—";
        }

        private static System.Drawing.Color ObtenerColorEstado(string estadoLower)
        {
            foreach (var par in _coloresEstado)
                if (estadoLower.Contains(par.Clave))
                    return par.Color;
            return System.Drawing.Color.FromArgb(80, 55, 30);
        }

        private static string[] ObtenerTransicionesValidas(string estadoActual) =>
            estadoActual.Trim().ToLower() switch
            {
                "pendiente" => new[] { "En proceso" },
                "en proceso" => new[] { "Completado", "Pendiente" },
                "completado" => Array.Empty<string>(),
                _ => Array.Empty<string>()
            };

        private string? MostrarDialogoCambioEstado(string estadoActual, string[] opciones)
        {
            using var dlg = new Form
            {
                Text = "Modificar estado",
                Size = new System.Drawing.Size(460, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = System.Drawing.Color.FromArgb(245, 240, 232),
                Font = new Font("Segoe UI", 9F)
            };

            var headerPanel = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(38, 22, 10),
                Dock = DockStyle.Top,
                Height = 52
            };
            var headerLabel = new Label
            {
                Text = "Modificar estado de entrega",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Location = new Point(18, 14)
            };
            var accentLine = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(92, 122, 42),
                Dock = DockStyle.Bottom,
                Height = 3
            };
            headerPanel.Controls.Add(headerLabel);
            headerPanel.Controls.Add(accentLine);

            var lblActualEtiqueta = new Label
            {
                Text = "Estado actual:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = System.Drawing.Color.FromArgb(128, 105, 82),
                AutoSize = true,
                Location = new Point(24, 76)
            };
            var lblActualValor = new Label
            {
                Text = estadoActual,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(38, 22, 10),
                AutoSize = true,
                Location = new Point(148, 74)
            };

            var separador = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(210, 195, 175),
                Location = new Point(24, 108),
                Size = new System.Drawing.Size(400, 1)
            };

            var lblNuevoEtiqueta = new Label
            {
                Text = "Nuevo estado:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = System.Drawing.Color.FromArgb(128, 105, 82),
                AutoSize = true,
                Location = new Point(24, 122)
            };
            var cboNuevo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(148, 119),
                Width = 276,
                Height = 32
            };
            cboNuevo.Items.AddRange(opciones);
            cboNuevo.SelectedIndex = 0;

            var btnOk = new Button
            {
                Text = "Confirmar cambio",
                BackColor = System.Drawing.Color.FromArgb(92, 122, 42),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(148, 185),
                Size = new System.Drawing.Size(168, 40),
                DialogResult = DialogResult.OK,
                UseVisualStyleBackColor = false
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.FromArgb(44, 28, 16),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(330, 185),
                Size = new System.Drawing.Size(94, 40),
                DialogResult = DialogResult.Cancel,
                UseVisualStyleBackColor = false
            };
            btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(160, 130, 95);
            dlg.Controls.AddRange(new Control[]
            {
                headerPanel,
                lblActualEtiqueta, lblActualValor,
                separador,
                lblNuevoEtiqueta, cboNuevo,
                btnOk, btnCancelar
            });

            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancelar;

            return dlg.ShowDialog(this) == DialogResult.OK
                ? cboNuevo.SelectedItem?.ToString()
                : null;
        }
    }
}