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
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class MainMenu : Form
    {
        private int _isLoading = 0;

        // ── Colores de estado (reutilizados en exportaciones) ─────────
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

        public MainMenu()
        {
            InitializeComponent();

            QuestPDF.Settings.License = LicenseType.Community;

            Load += async (s, e) => await CargarDashboardAsync();
            Activated += async (s, e) => await CargarDashboardAsync();
        }

        
        private async Task CargarDashboardAsync()
        {
            if (Interlocked.Exchange(ref _isLoading, 1) == 1) return;

            try
            {
                var entregaService = Program.ServiceProvider.GetRequiredService<EntregaService>();
                var hoy = DateOnly.FromDateTime(DateTime.Today);

                var entregasHoy = await entregaService.GetListConRelaciones(e => e.FechaEntrega == hoy);

                decimal totalKilos = entregasHoy.Sum(e => e.Kilos);
                lblTotalKilosValue.Text = totalKilos.ToString("N0") + " kg";
                lblTotalDeliveriesValue.Text = entregasHoy.Count.ToString();

                int pendientes = entregasHoy.Count(e =>
                    e.EstadoEntrega.Nombre.IndexOf("pendiente", StringComparison.OrdinalIgnoreCase) >= 0);
                int completadas = entregasHoy.Count(e =>
                    e.EstadoEntrega.Nombre.IndexOf("completad", StringComparison.OrdinalIgnoreCase) >= 0);

                lblPendingValue.Text = pendientes.ToString();
                lblCompletedValue.Text = completadas.ToString();

                var todas = await entregaService.GetListConRelaciones(e => true);
                var recientes = todas
                    .OrderByDescending(e => e.FechaEntrega)
                    .ThenByDescending(e => e.EntregaId)
                    .Take(20)
                    .ToList();

                dgvRecentDeliveries.Rows.Clear();
                foreach (var entrega in recientes)
                {
                    dgvRecentDeliveries.Rows.Add(
                        entrega.NumeroEntrega,
                        $"{entrega.Productor.Nombre} {entrega.Productor.Apellido}",
                        entrega.Producto.Nombre,
                        entrega.FechaEntrega.ToString("dd/MM/yyyy"),
                        entrega.Kilos.ToString("N2"),
                        entrega.EstadoEntrega.Nombre
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos del dashboard: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Interlocked.Exchange(ref _isLoading, 0);
            }
        }

        // ── Navegación ────────────────────────────────────────────────
        private async void NuevaEntrega_Click(object sender, EventArgs e)
        {
            using var form = new RegistroEntregaForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
            await CargarDashboardAsync();
        }

        private async void ConsultarEntregas_Click(object sender, EventArgs e)
        {
            using var form = new ConsultaEntregasForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
            await CargarDashboardAsync();
        }

        private void ListaProductores_Click(object sender, EventArgs e)
        {
            using var form = new ProductoresForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void AgregarProductor_Click(object sender, EventArgs e)
        {
            using var form = new ProductorDetalleForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void ayudaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var frm = new AcercaDeForm();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        // ══════════════════════════════════════════════════════════════
        // EXPORTACIONES
        // ══════════════════════════════════════════════════════════════

        // ── Helpers de color para PDF ─────────────────────────────────
        private static System.Drawing.Color ObtenerColorEstado(string estadoLower)
        {
            foreach (var par in _coloresEstado)
                if (estadoLower.Contains(par.Clave))
                    return par.Color;
            return System.Drawing.Color.FromArgb(80, 55, 30);
        }

        private static string ObtenerLugarEntrega(Entrega entrega)
        {
            var partes = new List<string>(3);
            if (!string.IsNullOrWhiteSpace(entrega.Pasillo)) partes.Add(entrega.Pasillo);
            if (!string.IsNullOrWhiteSpace(entrega.NumeroAnaquel)) partes.Add(entrega.NumeroAnaquel);
            if (!string.IsNullOrWhiteSpace(entrega.Piso)) partes.Add(entrega.Piso);
            return partes.Count > 0 ? string.Join(" · ", partes) : "—";
        }

        // ─────────────────────────────────────────────────────────────
        // PRODUCTORES — Excel
        // ─────────────────────────────────────────────────────────────
        private async void ExportarProductoresExcel_Click(object sender, EventArgs e)
        {
            List<Productor> productores;
            try
            {
                var svc = Program.ServiceProvider.GetRequiredService<ProductorService>();
                productores = (await svc.GetList(_ => true)).OrderBy(p => p.Codigo).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener productores: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (productores.Count == 0)
            {
                MessageBox.Show("No hay productores para exportar.", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar productores a Excel",
                Filter = "Libro de Excel (*.xlsx)|*.xlsx",
                FileName = $"Productores_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                DefaultExt = "xlsx"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Productores");

                // Encabezado
                string[] headers = { "Código", "Nombre", "Apellido", "Teléfono", "Dirección" };
                for (int c = 0; c < headers.Length; c++)
                {
                    var cell = ws.Cell(1, c + 1);
                    cell.Value = headers[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(58, 38, 18);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Datos
                int row = 2;
                foreach (var p in productores)
                {
                    ws.Cell(row, 1).Value = p.Codigo ?? "—";
                    ws.Cell(row, 2).Value = p.Nombre ?? "—";
                    ws.Cell(row, 3).Value = p.Apellido ?? "—";
                    ws.Cell(row, 4).Value = p.Telefono ?? "—";
                    ws.Cell(row, 5).Value = p.Direccion ?? "—";

                    if (row % 2 == 0)
                    {
                        ws.Range(row, 1, row, 5)
                          .Style.Fill.BackgroundColor = XLColor.FromArgb(250, 247, 242);
                    }
                    row++;
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(sfd.FileName);

                MessageBox.Show($"Productores exportados correctamente:\n{sfd.FileName}",
                    "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // PRODUCTORES — PDF
        // ─────────────────────────────────────────────────────────────
        private async void ExportarProductoresPDF_Click(object sender, EventArgs e)
        {
            List<Productor> productores;
            try
            {
                var svc = Program.ServiceProvider.GetRequiredService<ProductorService>();
                productores = (await svc.GetList(_ => true)).OrderBy(p => p.Codigo).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener productores: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (productores.Count == 0)
            {
                MessageBox.Show("No hay productores para exportar.", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar productores a PDF",
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = $"Productores_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                DefaultExt = "pdf"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1.5f, Unit.Centimetre);
                        page.PageColor(Colors.White);

                        page.Header()
                            .Background("#26160A")
                            .Padding(12)
                            .Column(col =>
                            {
                                col.Item().Text("Lista de Productores")
                                    .FontFamily("Segoe UI").FontSize(18).Bold()
                                    .FontColor(Colors.White);
                                col.Item().Text("Centro de Fermentación y Secado")
                                    .FontSize(10).FontColor("#B9A58C");
                                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(9).FontColor("#B9A58C");
                            });

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1.2f); // Código
                                cols.RelativeColumn(1.8f); // Nombre
                                cols.RelativeColumn(1.8f); // Apellido
                                cols.RelativeColumn(1.5f); // Teléfono
                                cols.RelativeColumn(3.7f); // Dirección
                            });

                            table.Header(header =>
                            {
                                foreach (string h in new[] { "Código", "Nombre", "Apellido", "Teléfono", "Dirección" })
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text(h).FontColor("#FFFFFF").Bold().FontSize(9);
                            });

                            foreach (var p in productores)
                            {
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(p.Codigo ?? "—").Bold().FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(p.Nombre ?? "—").FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(p.Apellido ?? "—").FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(p.Telefono ?? "—").FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(p.Direccion ?? "—").FontSize(9);
                            }
                        });

                        page.Footer().AlignRight().Text(text =>
                        {
                            text.Span("Página ").FontSize(8).FontColor("#6B4C32");
                            text.CurrentPageNumber().FontSize(8).FontColor("#6B4C32");
                            text.Span(" de ").FontSize(8).FontColor("#6B4C32");
                            text.TotalPages().FontSize(8).FontColor("#6B4C32");
                        });
                    });
                }).GeneratePdf(sfd.FileName);

                MessageBox.Show($"Productores exportados correctamente:\n{sfd.FileName}",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // ENTREGAS — Excel
        // ─────────────────────────────────────────────────────────────
        private async void ExportarEntregasExcel_Click(object sender, EventArgs e)
        {
            List<Entrega> entregas;
            try
            {
                var svc = Program.ServiceProvider.GetRequiredService<EntregaService>();
                entregas = (await svc.GetListConRelaciones(_ => true))
                    .OrderByDescending(en => en.FechaEntrega)
                    .ThenByDescending(en => en.EntregaId)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener entregas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (entregas.Count == 0)
            {
                MessageBox.Show("No hay entregas para exportar.", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar entregas a Excel",
                Filter = "Libro de Excel (*.xlsx)|*.xlsx",
                FileName = $"Entregas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                DefaultExt = "xlsx"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Entregas");

                string[] headers =
                {
                    "Número", "Fecha", "Productor", "Producto", "Subproducto",
                    "Estado", "Kilos", "Cajas", "Sacos", "Kilos secos",
                    "Placa", "Conductor", "Pasillo", "Anaquel", "Piso", "Observaciones"
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

                int row = 2;
                foreach (var en in entregas)
                {
                    string estado = en.EstadoEntrega?.Nombre ?? "—";
                    var colorEstado = ObtenerColorEstado(estado.ToLowerInvariant());

                    ws.Cell(row, 1).Value = en.NumeroEntrega ?? "—";
                    ws.Cell(row, 2).Value = en.FechaEntrega.ToString("dd/MM/yyyy");
                    ws.Cell(row, 3).Value = $"{en.Productor?.Nombre} {en.Productor?.Apellido}".Trim();
                    ws.Cell(row, 4).Value = en.Producto?.Nombre ?? "—";
                    ws.Cell(row, 5).Value = en.SubProducto?.Nombre ?? "—";
                    ws.Cell(row, 6).Value = estado;
                    ws.Cell(row, 7).Value = en.Kilos;
                    ws.Cell(row, 8).Value = en.Cajas;
                    ws.Cell(row, 9).Value = en.Sacos;
                    ws.Cell(row, 10).Value = en.KilosSecos.HasValue ? en.KilosSecos.Value.ToString("N2") : "—";
                    ws.Cell(row, 11).Value = en.Placa ?? "—";
                    ws.Cell(row, 12).Value = en.NombreConductor ?? "—";
                    ws.Cell(row, 13).Value = en.Pasillo ?? "—";
                    ws.Cell(row, 14).Value = en.NumeroAnaquel ?? "—";
                    ws.Cell(row, 15).Value = en.Piso ?? "—";
                    ws.Cell(row, 16).Value = en.Observaciones ?? "—";

                    ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromArgb(colorEstado.R, colorEstado.G, colorEstado.B);
                    ws.Cell(row, 6).Style.Font.Bold = true;
                    ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00";

                    if (row % 2 == 0)
                        ws.Range(row, 1, row, 16).Style.Fill.BackgroundColor = XLColor.FromArgb(250, 247, 242);

                    row++;
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(sfd.FileName);

                MessageBox.Show($"Entregas exportadas correctamente:\n{sfd.FileName}",
                    "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // ENTREGAS — PDF
        // ─────────────────────────────────────────────────────────────
        private async void ExportarEntregasPDF_Click(object sender, EventArgs e)
        {
            List<Entrega> entregas;
            try
            {
                var svc = Program.ServiceProvider.GetRequiredService<EntregaService>();
                entregas = (await svc.GetListConRelaciones(_ => true))
                    .OrderByDescending(en => en.FechaEntrega)
                    .ThenByDescending(en => en.EntregaId)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener entregas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (entregas.Count == 0)
            {
                MessageBox.Show("No hay entregas para exportar.", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar entregas a PDF",
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = $"Entregas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                DefaultExt = "pdf"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A3.Landscape());
                        page.Margin(1.2f, Unit.Centimetre);
                        page.PageColor(Colors.White);

                        page.Header()
                            .Background("#26160A")
                            .Padding(10)
                            .Column(col =>
                            {
                                col.Item().Text("Listado de Entregas")
                                    .FontFamily("Segoe UI").FontSize(18).Bold()
                                    .FontColor(Colors.White);
                                col.Item().Text("Centro de Fermentación y Secado")
                                    .FontSize(10).FontColor("#B9A58C");
                                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  Total: {entregas.Count} registros")
                                    .FontSize(9).FontColor("#B9A58C");
                            });

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1.2f); // Número
                                cols.RelativeColumn(1.1f); // Fecha
                                cols.RelativeColumn(2.0f); // Productor
                                cols.RelativeColumn(1.5f); // Producto
                                cols.RelativeColumn(1.3f); // Estado
                                cols.RelativeColumn(1.0f); // Kilos
                                cols.RelativeColumn(1.8f); // Lugar
                            });

                            table.Header(header =>
                            {
                                foreach (string h in new[] { "Número", "Fecha", "Productor", "Producto", "Estado", "Kilos", "Lugar" })
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text(h).FontColor("#FFFFFF").Bold().FontSize(8.5f);
                            });

                            foreach (var en in entregas)
                            {
                                string estado = en.EstadoEntrega?.Nombre ?? "—";
                                var colorEst = ObtenerColorEstado(estado.ToLowerInvariant());
                                string colorHex = $"#{colorEst.R:X2}{colorEst.G:X2}{colorEst.B:X2}";
                                string productor = $"{en.Productor?.Nombre} {en.Productor?.Apellido}".Trim();

                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(en.NumeroEntrega ?? "—").Bold().FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(en.FechaEntrega.ToString("dd/MM/yyyy")).FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(productor).FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(en.Producto?.Nombre ?? "—").FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(estado).FontColor(colorHex).Bold().FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).AlignRight()
                                    .Text(en.Kilos.ToString("N2")).FontSize(8.5f);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(ObtenerLugarEntrega(en)).FontSize(8.5f);
                            }
                        });

                        page.Footer().AlignRight().Text(text =>
                        {
                            text.Span("Página ").FontSize(8).FontColor("#6B4C32");
                            text.CurrentPageNumber().FontSize(8).FontColor("#6B4C32");
                            text.Span(" de ").FontSize(8).FontColor("#6B4C32");
                            text.TotalPages().FontSize(8).FontColor("#6B4C32");
                        });
                    });
                }).GeneratePdf(sfd.FileName);

                MessageBox.Show($"Entregas exportadas correctamente:\n{sfd.FileName}",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // HISTORIAL — Excel
        // ─────────────────────────────────────────────────────────────
        private async void ExportarHistorialExcel_Click(object sender, EventArgs e)
        {
            List<HistoricoEstadoEntrega> historial;
            Dictionary<int, Entrega> entregasDict;
            try
            {
                var svcHistorico = Program.ServiceProvider.GetRequiredService<HistoricoEstadoEntregaService>();
                var svcEntregas = Program.ServiceProvider.GetRequiredService<EntregaService>();

                var entregas = await svcEntregas.GetListConRelaciones(_ => true);
                entregasDict = entregas.ToDictionary(en => en.EntregaId);
                historial = await svcHistorico.ObtenerTodosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener el historial: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (historial.Count == 0)
            {
                MessageBox.Show("No hay registros de historial para exportar.", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar historial a Excel",
                Filter = "Libro de Excel (*.xlsx)|*.xlsx",
                FileName = $"Historial_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                DefaultExt = "xlsx"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Historial");

                string[] headers = { "Fecha y hora", "Entrega", "Lugar en almacén", "Estado", "Observaciones" };
                for (int c = 0; c < headers.Length; c++)
                {
                    var cell = ws.Cell(1, c + 1);
                    cell.Value = headers[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(58, 38, 18);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                int row = 2;
                foreach (var h in historial)
                {
                    string estado = h.EstadoEntrega?.Nombre ?? "Desconocido";
                    var colorEst = ObtenerColorEstado(estado.ToLowerInvariant());

                    string lugar = entregasDict.TryGetValue(h.EntregaId, out var ent)
                        ? ObtenerLugarEntrega(ent) : "—";

                    ws.Cell(row, 1).Value = h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss");
                    ws.Cell(row, 2).Value = $"E-{h.EntregaId:D4}";
                    ws.Cell(row, 3).Value = lugar;
                    ws.Cell(row, 4).Value = estado;
                    ws.Cell(row, 5).Value = string.IsNullOrWhiteSpace(h.Observaciones) ? "—" : h.Observaciones;

                    ws.Cell(row, 1).Style.Font.FontName = "Consolas";
                    ws.Cell(row, 1).Style.Font.FontSize = 8.5;
                    ws.Cell(row, 4).Style.Font.FontColor = XLColor.FromArgb(colorEst.R, colorEst.G, colorEst.B);
                    ws.Cell(row, 4).Style.Font.Bold = true;

                    if (row % 2 == 0)
                        ws.Range(row, 1, row, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(250, 247, 242);

                    row++;
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(sfd.FileName);

                MessageBox.Show($"Historial exportado correctamente:\n{sfd.FileName}",
                    "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // HISTORIAL — PDF
        // ─────────────────────────────────────────────────────────────
        private async void ExportarHistorialPDF_Click(object sender, EventArgs e)
        {
            List<HistoricoEstadoEntrega> historial;
            Dictionary<int, Entrega> entregasDict;
            try
            {
                var svcHistorico = Program.ServiceProvider.GetRequiredService<HistoricoEstadoEntregaService>();
                var svcEntregas = Program.ServiceProvider.GetRequiredService<EntregaService>();

                var entregas = await svcEntregas.GetListConRelaciones(_ => true);
                entregasDict = entregas.ToDictionary(en => en.EntregaId);
                historial = await svcHistorico.ObtenerTodosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener el historial: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (historial.Count == 0)
            {
                MessageBox.Show("No hay registros de historial para exportar.", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar historial a PDF",
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = $"Historial_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                DefaultExt = "pdf"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                // Captura local para el lambda
                var historialLocal = historial;
                var entregasDictLocal = entregasDict;

                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1.5f, Unit.Centimetre);
                        page.PageColor(Colors.White);

                        page.Header()
                            .Background("#26160A")
                            .Padding(12)
                            .Column(col =>
                            {
                                col.Item().Text("Historial de cambios de estado")
                                    .FontFamily("Segoe UI").FontSize(18).Bold()
                                    .FontColor(Colors.White);
                                col.Item().Text("Centro de Fermentación y Secado")
                                    .FontSize(10).FontColor("#B9A58C");
                                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  Total: {historialLocal.Count} registros")
                                    .FontSize(9).FontColor("#B9A58C");
                            });

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2.2f); // Fecha
                                cols.RelativeColumn(1.3f); // Entrega
                                cols.RelativeColumn(2.0f); // Lugar
                                cols.RelativeColumn(1.5f); // Estado
                                cols.RelativeColumn(3.0f); // Observaciones
                            });

                            table.Header(header =>
                            {
                                foreach (string h in new[] { "Fecha y hora", "Entrega", "Lugar en almacén", "Estado", "Observaciones" })
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text(h).FontColor("#FFFFFF").Bold().FontSize(9);
                            });

                            foreach (var h in historialLocal)
                            {
                                string estado = h.EstadoEntrega?.Nombre ?? "Desconocido";
                                var colorEst = ObtenerColorEstado(estado.ToLowerInvariant());
                                string colorHex = $"#{colorEst.R:X2}{colorEst.G:X2}{colorEst.B:X2}";
                                string lugar = entregasDictLocal.TryGetValue(h.EntregaId, out var ent)
                                    ? ObtenerLugarEntrega(ent) : "—";

                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss"))
                                    .FontSize(8.5f).FontFamily("Consolas");
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text($"E-{h.EntregaId:D4}").Bold().FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(lugar).FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(estado).FontColor(colorHex).Bold().FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(string.IsNullOrWhiteSpace(h.Observaciones) ? "—" : h.Observaciones)
                                    .FontSize(9);
                            }
                        });

                        page.Footer().AlignRight().Text(text =>
                        {
                            text.Span("Página ").FontSize(8).FontColor("#6B4C32");
                            text.CurrentPageNumber().FontSize(8).FontColor("#6B4C32");
                            text.Span(" de ").FontSize(8).FontColor("#6B4C32");
                            text.TotalPages().FontSize(8).FontColor("#6B4C32");
                        });
                    });
                }).GeneratePdf(sfd.FileName);

                MessageBox.Show($"Historial exportado correctamente:\n{sfd.FileName}",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}