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
using System.Threading.Tasks;
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
            ("espera",   System.Drawing.Color.FromArgb(170, 120,  40)),
            ("cancel",   System.Drawing.Color.FromArgb(180,  55,  35)),
            ("rechaz",   System.Drawing.Color.FromArgb(180,  55,  35)),
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

        private void DashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new DashboardForm();
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
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.PageColor("#26160A");
                        page.Margin(0.5f, Unit.Centimetre);

                        page.Header()
                            .Background("#26160A")
                            .PaddingVertical(18)
                            .PaddingHorizontal(24)
                            .Column(col =>
                            {
                                col.Item().Text("LISTA DE PRODUCTORES")
                                    .FontFamily("Segoe UI").FontSize(22).Bold()
                                    .FontColor(Colors.White);

                                col.Item().PaddingTop(2)
                                    .Text("Centro de Fermentación y Secado")
                                    .FontSize(11)
                                    .FontColor("#C9B59D");

                                col.Item().PaddingTop(4)
                                    .Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(9)
                                    .FontColor("#B89E82");
                            });

                        page.Content()
                            .Background("#F4EFE7")
                            .PaddingHorizontal(18)
                            .PaddingVertical(14)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(1.2f);
                                    cols.RelativeColumn(1.8f);
                                    cols.RelativeColumn(1.8f);
                                    cols.RelativeColumn(1.5f);
                                    cols.RelativeColumn(3.7f);
                                });

                                table.Header(header =>
                                {
                                    foreach (string h in new[] { "Código", "Nombre", "Apellido", "Teléfono", "Dirección" })
                                    {
                                        header.Cell()
                                            .Background("#3A2612")
                                            .BorderBottom(2)
                                            .BorderColor("#1C1007")
                                            .PaddingVertical(8)
                                            .PaddingHorizontal(5)
                                            .AlignCenter()
                                            .Text(h)
                                            .FontColor("#FFFFFF")
                                            .Bold()
                                            .FontSize(9.5f);
                                    }
                                });

                                int index = 0;

                                foreach (var p in productores)
                                {
                                    string fondoFila = index % 2 == 0 ? "#F8F4EE" : "#EFE7DB";

                                    IContainer EstiloCelda(IContainer cell)
                                    {
                                        return cell
                                            .Background(fondoFila)
                                            .BorderBottom(1)
                                            .BorderColor("#D7C8B5")
                                            .PaddingVertical(6)
                                            .PaddingHorizontal(5);
                                    }

                                    table.Cell().Element(EstiloCelda)
                                        .Text(p.Codigo ?? "—")
                                        .Bold()
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(p.Nombre ?? "—")
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(p.Apellido ?? "—")
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(p.Telefono ?? "—")
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(p.Direccion ?? "—")
                                        .FontSize(8.8f);

                                    index++;
                                }
                            });

                        page.Footer()
                            .Background("#26160A")
                            .PaddingVertical(8)
                            .PaddingHorizontal(20)
                            .AlignRight()
                            .Text(text =>
                            {
                                text.Span("Página ").FontSize(9).FontColor("#D8C2A5");
                                text.CurrentPageNumber().FontSize(9).Bold().FontColor(Colors.White);
                                text.Span(" de ").FontSize(9).FontColor("#D8C2A5");
                                text.TotalPages().FontSize(9).Bold().FontColor(Colors.White);
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
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A3.Landscape());
                        page.PageColor("#26160A");
                        page.Margin(0.5f, Unit.Centimetre);

                        page.Header()
                            .Background("#26160A")
                            .PaddingVertical(18)
                            .PaddingHorizontal(24)
                            .Column(col =>
                            {
                                col.Item().Text("LISTADO DE ENTREGAS")
                                    .FontFamily("Segoe UI").FontSize(22).Bold()
                                    .FontColor(Colors.White);

                                col.Item().PaddingTop(2)
                                    .Text("Centro de Fermentación y Secado")
                                    .FontSize(11)
                                    .FontColor("#C9B59D");

                                col.Item().PaddingTop(4)
                                    .Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  Total: {entregas.Count} registros")
                                    .FontSize(9)
                                    .FontColor("#B89E82");
                            });

                        page.Content()
                            .Background("#F4EFE7")
                            .PaddingHorizontal(18)
                            .PaddingVertical(14)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(1.2f);
                                    cols.RelativeColumn(1.1f);
                                    cols.RelativeColumn(2.0f);
                                    cols.RelativeColumn(1.5f);
                                    cols.RelativeColumn(1.3f);
                                    cols.RelativeColumn(1.0f);
                                    cols.RelativeColumn(1.8f);
                                });

                                table.Header(header =>
                                {
                                    foreach (string h in new[] { "Número", "Fecha", "Productor", "Producto", "Estado", "Kilos", "Lugar" })
                                    {
                                        header.Cell()
                                            .Background("#3A2612")
                                            .BorderBottom(2)
                                            .BorderColor("#1C1007")
                                            .PaddingVertical(8)
                                            .PaddingHorizontal(5)
                                            .AlignCenter()
                                            .Text(h)
                                            .FontColor("#FFFFFF")
                                            .Bold()
                                            .FontSize(9.5f);
                                    }
                                });

                                int index = 0;

                                foreach (var en in entregas)
                                {
                                    string estado = en.EstadoEntrega?.Nombre ?? "—";
                                    var colorEst = ObtenerColorEstado(estado.ToLowerInvariant());
                                    string colorHex = $"#{colorEst.R:X2}{colorEst.G:X2}{colorEst.B:X2}";
                                    string productor = $"{en.Productor?.Nombre} {en.Productor?.Apellido}".Trim();
                                    string fondoFila = index % 2 == 0 ? "#F8F4EE" : "#EFE7DB";

                                    IContainer EstiloCelda(IContainer cell)
                                    {
                                        return cell
                                            .Background(fondoFila)
                                            .BorderBottom(1)
                                            .BorderColor("#D7C8B5")
                                            .PaddingVertical(6)
                                            .PaddingHorizontal(5);
                                    }

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.NumeroEntrega ?? "—")
                                        .Bold()
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.FechaEntrega.ToString("dd/MM/yyyy"))
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(productor)
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.Producto?.Nombre ?? "—")
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(estado)
                                        .FontColor(colorHex)
                                        .Bold()
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .AlignRight()
                                        .Text(en.Kilos.ToString("N2"))
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(ObtenerLugarEntrega(en))
                                        .FontSize(8.8f);

                                    index++;
                                }
                            });

                        page.Footer()
                            .Background("#26160A")
                            .PaddingVertical(8)
                            .PaddingHorizontal(20)
                            .AlignRight()
                            .Text(text =>
                            {
                                text.Span("Página ").FontSize(9).FontColor("#D8C2A5");
                                text.CurrentPageNumber().FontSize(9).Bold().FontColor(Colors.White);
                                text.Span(" de ").FontSize(9).FontColor("#D8C2A5");
                                text.TotalPages().FontSize(9).Bold().FontColor(Colors.White);
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
                var historialLocal = historial;
                var entregasDictLocal = entregasDict;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.PageColor("#26160A");
                        page.Margin(0.5f, Unit.Centimetre);

                        page.Header()
                            .Background("#26160A")
                            .PaddingVertical(18)
                            .PaddingHorizontal(24)
                            .Column(col =>
                            {
                                col.Item().Text("HISTORIAL DE CAMBIOS DE ESTADO")
                                    .FontFamily("Segoe UI").FontSize(22).Bold()
                                    .FontColor(Colors.White);

                                col.Item().PaddingTop(2)
                                    .Text("Centro de Fermentación y Secado")
                                    .FontSize(11)
                                    .FontColor("#C9B59D");

                                col.Item().PaddingTop(4)
                                    .Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  Total: {historialLocal.Count} registros")
                                    .FontSize(9)
                                    .FontColor("#B89E82");
                            });

                        page.Content()
                            .Background("#F4EFE7")
                            .PaddingHorizontal(18)
                            .PaddingVertical(14)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(2.2f);
                                    cols.RelativeColumn(1.3f);
                                    cols.RelativeColumn(2.0f);
                                    cols.RelativeColumn(1.5f);
                                    cols.RelativeColumn(3.0f);
                                });

                                table.Header(header =>
                                {
                                    foreach (string h in new[] { "Fecha y hora", "Entrega", "Lugar en almacén", "Estado", "Observaciones" })
                                    {
                                        header.Cell()
                                            .Background("#3A2612")
                                            .BorderBottom(2)
                                            .BorderColor("#1C1007")
                                            .PaddingVertical(8)
                                            .PaddingHorizontal(5)
                                            .AlignCenter()
                                            .Text(h)
                                            .FontColor("#FFFFFF")
                                            .Bold()
                                            .FontSize(9.5f);
                                    }
                                });

                                int index = 0;

                                foreach (var h in historialLocal)
                                {
                                    string estado = h.EstadoEntrega?.Nombre ?? "Desconocido";
                                    var colorEst = ObtenerColorEstado(estado.ToLowerInvariant());
                                    string colorHex = $"#{colorEst.R:X2}{colorEst.G:X2}{colorEst.B:X2}";
                                    string lugar = entregasDictLocal.TryGetValue(h.EntregaId, out var ent)
                                        ? ObtenerLugarEntrega(ent) : "—";
                                    string fondoFila = index % 2 == 0 ? "#F8F4EE" : "#EFE7DB";

                                    IContainer EstiloCelda(IContainer cell)
                                    {
                                        return cell
                                            .Background(fondoFila)
                                            .BorderBottom(1)
                                            .BorderColor("#D7C8B5")
                                            .PaddingVertical(6)
                                            .PaddingHorizontal(5);
                                    }

                                    table.Cell().Element(EstiloCelda)
                                        .Text(h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss"))
                                        .FontSize(8.8f)
                                        .FontFamily("Consolas");

                                    table.Cell().Element(EstiloCelda)
                                        .Text($"E-{h.EntregaId:D4}")
                                        .Bold()
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(lugar)
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(estado)
                                        .FontColor(colorHex)
                                        .Bold()
                                        .FontSize(8.8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(string.IsNullOrWhiteSpace(h.Observaciones) ? "—" : h.Observaciones)
                                        .FontSize(8.8f);

                                    index++;
                                }
                            });

                        page.Footer()
                            .Background("#26160A")
                            .PaddingVertical(8)
                            .PaddingHorizontal(20)
                            .AlignRight()
                            .Text(text =>
                            {
                                text.Span("Página ").FontSize(9).FontColor("#D8C2A5");
                                text.CurrentPageNumber().FontSize(9).Bold().FontColor(Colors.White);
                                text.Span(" de ").FontSize(9).FontColor("#D8C2A5");
                                text.TotalPages().FontSize(9).Bold().FontColor(Colors.White);
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