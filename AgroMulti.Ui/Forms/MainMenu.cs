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

        // Exportaciones ────────────────────────────────────────────────

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
                
                var cHeader = XLColor.FromArgb(38, 22, 10);
                var cSubtit = XLColor.FromArgb(201, 181, 157);
                var cMeta = XLColor.FromArgb(184, 158, 130);
                var cBg = XLColor.FromArgb(244, 239, 231);
                var cCardBg = XLColor.FromArgb(252, 249, 244);
                var cBorder = XLColor.FromArgb(216, 200, 184);
                var cLabel = XLColor.FromArgb(107, 76, 50);
                var cMuted = XLColor.FromArgb(138, 115, 95);
                var cRowPar = XLColor.FromArgb(250, 247, 242);   // fila par
                var cRowImpar = XLColor.White;                      // fila impar
                var cFooterBg = XLColor.FromArgb(239, 231, 219);

                const int COLS = 5;

                string[] headers = { "Código", "Nombre", "Apellido", "Teléfono", "Dirección" };
                int[] colWidths = { 12, 22, 22, 16, 40 };

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Productores");

                
                for (int c = 1; c <= COLS; c++)
                    ws.Column(c).Width = colWidths[c - 1];

                // Filas 1-3 — Bloque de encabezados

                // Fila 1 — Título principal
                ws.Row(1).Height = 23;
                var r1 = ws.Range(1, 1, 1, COLS);
                r1.Merge();
                r1.Value = "LISTADO DE PRODUCTORES";
                r1.Style.Font.Bold = true;
                r1.Style.Font.FontSize = 14;
                r1.Style.Font.FontColor = XLColor.White;
                r1.Style.Fill.BackgroundColor = cHeader;
                r1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r1.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fila 2 — Subtítulo
                ws.Row(2).Height = 16;
                var r2 = ws.Range(2, 1, 2, COLS);
                r2.Merge();
                r2.Value = "Centro de Fermentación y Secado";
                r2.Style.Font.FontSize = 9;
                r2.Style.Font.FontColor = cSubtit;
                r2.Style.Fill.BackgroundColor = cHeader;
                r2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fila 3 — Meta: fecha + total registros
                ws.Row(3).Height = 13;
                var r3 = ws.Range(3, 1, 3, COLS);
                r3.Merge();
                r3.Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  " +
                                                 $"Total de productores: {productores.Count:N0}";
                r3.Style.Font.FontSize = 8;
                r3.Style.Font.FontColor = cMeta;
                r3.Style.Fill.BackgroundColor = cHeader;
                r3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fila 4 — Separador visual
                ws.Row(4).Height = 6;
                ws.Range(4, 1, 4, COLS).Style.Fill.BackgroundColor = cBg;

                // Fila 5 — Encabezados de columna
               
                ws.Row(5).Height = 18;
                for (int c = 0; c < COLS; c++)
                {
                    var cell = ws.Cell(5, c + 1);
                    cell.Value = headers[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontSize = 9;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = cLabel;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = cBorder;
                }

                // Filas de datos — a partir de fila 6
                
                int dataRow = 6;
                int rowNum = 0;
                foreach (var p in productores)
                {
                    ws.Row(dataRow).Height = 15;

                    var rowBg = rowNum % 2 == 0 ? cRowImpar : cRowPar;

                    string[] valores =
                    {
                p.Codigo    ?? "—",
                p.Nombre    ?? "—",
                p.Apellido  ?? "—",
                p.Telefono  ?? "—",
                p.Direccion ?? "—",
            };

                    for (int c = 0; c < COLS; c++)
                    {
                        var cell = ws.Cell(dataRow, c + 1);
                        cell.Value = valores[c];
                        cell.Style.Font.FontSize = 9;
                        cell.Style.Font.FontColor = cHeader;
                        cell.Style.Fill.BackgroundColor = rowBg;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = cBorder;

                        
                        cell.Style.Alignment.Horizontal = (c == 0 || c == 3)
                            ? XLAlignmentHorizontalValues.Center
                            : XLAlignmentHorizontalValues.Left;

                        // Padding izquierdo para columnas de texto
                        if (c != 0 && c != 3)
                            cell.Style.Alignment.Indent = 1;
                    }

                    dataRow++;
                    rowNum++;
                }

                // Fila de totales
                
                int totalRow = dataRow;
                ws.Row(totalRow).Height = 16;

                // Celda fusionada "Total" en columnas 1-4
                var rTotLabel = ws.Range(totalRow, 1, totalRow, 4);
                rTotLabel.Merge();
                rTotLabel.Value = $"Total de productores registrados";
                rTotLabel.Style.Font.Bold = true;
                rTotLabel.Style.Font.FontSize = 9;
                rTotLabel.Style.Font.FontColor = cLabel;
                rTotLabel.Style.Fill.BackgroundColor = cFooterBg;
                rTotLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                rTotLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rTotLabel.Style.Alignment.Indent = 1;
                rTotLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rTotLabel.Style.Border.OutsideBorderColor = cBorder;

                // Celda con el número
                var cTotVal = ws.Cell(totalRow, 5);
                cTotVal.Value = productores.Count;
                cTotVal.Style.Font.Bold = true;
                cTotVal.Style.Font.FontSize = 10;
                cTotVal.Style.Font.FontColor = cHeader;
                cTotVal.Style.Fill.BackgroundColor = cFooterBg;
                cTotVal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cTotVal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cTotVal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cTotVal.Style.Border.OutsideBorderColor = cBorder;


                // Filtros en la fila de encabezados (fila 5)
                ws.Range(5, 1, dataRow - 1, COLS).SetAutoFilter();

                // Congelar encabezado: filas 1-5 fijas al hacer scroll
                ws.SheetView.FreezeRows(5);

                // Zoom al 110 % para mejor legibilidad
                ws.SheetView.ZoomScale = 110;

                var tableRange = ws.Range(5, 1, totalRow, COLS);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                tableRange.Style.Border.OutsideBorderColor = cBorder;

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
                // ── Paleta ────────────────────────────────────────────────────
                var cHeader = XLColor.FromArgb(38, 22, 10);
                var cSubtit = XLColor.FromArgb(201, 181, 157);
                var cMeta = XLColor.FromArgb(184, 158, 130);
                var cBg = XLColor.FromArgb(244, 239, 231);
                var cCardBg = XLColor.FromArgb(252, 249, 244);
                var cBorder = XLColor.FromArgb(216, 200, 184);
                var cLabel = XLColor.FromArgb(107, 76, 50);
                var cMuted = XLColor.FromArgb(138, 115, 95);
                var cRowPar = XLColor.FromArgb(250, 247, 242);
                var cRowImpar = XLColor.White;
                var cFooterBg = XLColor.FromArgb(239, 231, 219);

                const int COLS = 16;

                string[] headers =
                {
            "Número", "Fecha", "Productor", "Producto", "Subproducto",
            "Estado", "Kilos", "Cajas", "Sacos", "Kilos secos",
            "Placa", "Conductor", "Pasillo", "Anaquel", "Piso", "Observaciones"
        };

                int[] colWidths =
                {
            12,  // Número
            12,  // Fecha
            26,  // Productor
            18,  // Producto
            18,  // Subproducto
            14,  // Estado
            11,  // Kilos
            9,   // Cajas
            9,   // Sacos
            12,  // Kilos secos
            11,  // Placa
            22,  // Conductor
            10,  // Pasillo
            10,  // Anaquel
            9,   // Piso
            30,  // Observaciones
        };

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Entregas");

                for (int c = 1; c <= COLS; c++)
                    ws.Column(c).Width = colWidths[c - 1];

                

                ws.Row(1).Height = 23;
                var r1 = ws.Range(1, 1, 1, COLS);
                r1.Merge();
                r1.Value = "LISTADO DE ENTREGAS";
                r1.Style.Font.Bold = true;
                r1.Style.Font.FontSize = 14;
                r1.Style.Font.FontColor = XLColor.White;
                r1.Style.Fill.BackgroundColor = cHeader;
                r1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r1.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Row(2).Height = 16;
                var r2 = ws.Range(2, 1, 2, COLS);
                r2.Merge();
                r2.Value = "Centro de Fermentación y Secado";
                r2.Style.Font.FontSize = 9;
                r2.Style.Font.FontColor = cSubtit;
                r2.Style.Fill.BackgroundColor = cHeader;
                r2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Row(3).Height = 13;
                var r3 = ws.Range(3, 1, 3, COLS);
                r3.Merge();
                r3.Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  " +
                                                 $"Total de entregas: {entregas.Count:N0}";
                r3.Style.Font.FontSize = 8;
                r3.Style.Font.FontColor = cMeta;
                r3.Style.Fill.BackgroundColor = cHeader;
                r3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Row(4).Height = 6;
                ws.Range(4, 1, 4, COLS).Style.Fill.BackgroundColor = cBg;

                ws.Row(5).Height = 18;
                for (int c = 0; c < COLS; c++)
                {
                    var cell = ws.Cell(5, c + 1);
                    cell.Value = headers[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontSize = 9;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = cLabel;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = cBorder;
                }

                
                var colsCentradas = new HashSet<int> { 0, 1, 5, 6, 7, 8, 9, 10, 12, 13, 14 };

                int dataRow = 6;
                int rowNum = 0;

                foreach (var en in entregas)
                {
                    ws.Row(dataRow).Height = 15;

                    var rowBg = rowNum % 2 == 0 ? cRowImpar : cRowPar;
                    string estado = en.EstadoEntrega?.Nombre ?? "—";
                    var colorEstado = ObtenerColorEstado(estado.ToLowerInvariant());

                    // ── Corrección: Cajas y Sacos son int no-nullable, sin ?? ──
                    object[] valores =
                    {
                en.NumeroEntrega ?? "—",                                    // 0  Número
                en.FechaEntrega.ToString("dd/MM/yyyy"),                     // 1  Fecha
                $"{en.Productor?.Nombre} {en.Productor?.Apellido}".Trim(), // 2  Productor
                en.Producto?.Nombre    ?? "—",                              // 3  Producto
                en.SubProducto?.Nombre ?? "—",                              // 4  Subproducto
                estado,                                                      // 5  Estado
                (object)en.Kilos,                                            // 6  Kilos
                (object)en.Cajas,                                            // 7  Cajas  
                (object)en.Sacos,                                            // 8  Sacos  
                en.KilosSecos.HasValue ? (object)en.KilosSecos.Value : "—", // 9  Kilos secos
                en.Placa           ?? "—",                                   // 10 Placa
                en.NombreConductor ?? "—",                                   // 11 Conductor
                en.Pasillo         ?? "—",                                   // 12 Pasillo
                en.NumeroAnaquel   ?? "—",                                   // 13 Anaquel
                en.Piso            ?? "—",                                   // 14 Piso
                en.Observaciones   ?? "—",                                   // 15 Observaciones
            };

                    for (int c = 0; c < COLS; c++)
                    {
                        var cell = ws.Cell(dataRow, c + 1);
                        cell.Value = XLCellValue.FromObject(valores[c]);

                        cell.Style.Font.FontSize = 9;
                        cell.Style.Font.FontColor = cHeader;
                        cell.Style.Fill.BackgroundColor = rowBg;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Alignment.WrapText = false;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = cBorder;

                        cell.Style.Alignment.Horizontal = colsCentradas.Contains(c)
                            ? XLAlignmentHorizontalValues.Center
                            : XLAlignmentHorizontalValues.Left;

                        if (!colsCentradas.Contains(c))
                            cell.Style.Alignment.Indent = 1;

                        if (c == 6 && valores[c] is not string)
                            cell.Style.NumberFormat.Format = "#,##0.00";
                        if (c == 9 && valores[c] is not string)
                            cell.Style.NumberFormat.Format = "#,##0.00";
                    }

                    
                    var cellEstado = ws.Cell(dataRow, 6);
                    cellEstado.Style.Font.Bold = true;
                    cellEstado.Style.Font.FontColor = XLColor.FromArgb(
                        colorEstado.R, colorEstado.G, colorEstado.B);
                    cellEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(
                        255 - (255 - colorEstado.R) / 4,
                        255 - (255 - colorEstado.G) / 4,
                        255 - (255 - colorEstado.B) / 4);

                    dataRow++;
                    rowNum++;
                }

                
                int totalRow = dataRow;
                ws.Row(totalRow).Height = 17;

                double sumKilos = entregas.Sum(en => (double)en.Kilos);
                double sumCajas = entregas.Sum(en => (double)en.Cajas);      
                double sumSacos = entregas.Sum(en => (double)en.Sacos);      
                double sumKilosSecos = entregas.Sum(en => (double)(en.KilosSecos ?? 0));

                var rTotLabel = ws.Range(totalRow, 1, totalRow, 6);
                rTotLabel.Merge();
                rTotLabel.Value = $"Total  —  {entregas.Count:N0} entregas registradas";
                rTotLabel.Style.Font.Bold = true;
                rTotLabel.Style.Font.FontSize = 9;
                rTotLabel.Style.Font.FontColor = cLabel;
                rTotLabel.Style.Fill.BackgroundColor = cFooterBg;
                rTotLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                rTotLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rTotLabel.Style.Alignment.Indent = 1;
                rTotLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rTotLabel.Style.Border.OutsideBorderColor = cBorder;

                var totalesNum = new (int Col, double Val, string Fmt)[]
                {
            (7,  sumKilos,      "#,##0.00"),
            (8,  sumCajas,      "#,##0"),
            (9,  sumSacos,      "#,##0"),
            (10, sumKilosSecos, "#,##0.00"),
                };

                foreach (var (col, val, fmt) in totalesNum)
                {
                    var ct = ws.Cell(totalRow, col);
                    ct.Value = val;
                    ct.Style.Font.Bold = true;
                    ct.Style.Font.FontSize = 9;
                    ct.Style.Font.FontColor = cHeader;
                    ct.Style.Fill.BackgroundColor = cFooterBg;
                    ct.Style.NumberFormat.Format = fmt;
                    ct.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ct.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ct.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ct.Style.Border.OutsideBorderColor = cBorder;
                }

                // Celdas vacías del footer cols 11-16
                for (int c = 11; c <= COLS; c++)
                {
                    var ct = ws.Cell(totalRow, c);
                    ct.Style.Fill.BackgroundColor = cFooterBg;
                    ct.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ct.Style.Border.OutsideBorderColor = cBorder;
                }

                
                ws.Range(5, 1, dataRow - 1, COLS).SetAutoFilter();
                ws.SheetView.FreezeRows(5);
                ws.SheetView.ZoomScale = 110;

                ws.Range(5, 1, totalRow, COLS).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                ws.Range(5, 1, totalRow, COLS).Style.Border.OutsideBorderColor = cBorder;

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
                var cHeader = XLColor.FromArgb(38, 22, 10);
                var cSubtit = XLColor.FromArgb(201, 181, 157);
                var cMeta = XLColor.FromArgb(184, 158, 130);
                var cBg = XLColor.FromArgb(244, 239, 231);
                var cBorder = XLColor.FromArgb(216, 200, 184);
                var cLabel = XLColor.FromArgb(107, 76, 50);
                var cMuted = XLColor.FromArgb(138, 115, 95);
                var cRowPar = XLColor.FromArgb(250, 247, 242);
                var cRowImpar = XLColor.White;
                var cFooterBg = XLColor.FromArgb(239, 231, 219);
                var cCardBg = XLColor.FromArgb(252, 249, 244);

                const int COLS = 5;

                string[] headers = { "Fecha y hora", "Entrega", "Lugar en almacén", "Estado", "Observaciones" };
                int[] colWidths = { 20, 10, 28, 16, 45 };

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Historial");

                for (int c = 1; c <= COLS; c++)
                    ws.Column(c).Width = colWidths[c - 1];

               
                // Fila 1 — Título principal
                ws.Row(1).Height = 23;
                var r1 = ws.Range(1, 1, 1, COLS);
                r1.Merge();
                r1.Value = "HISTORIAL DE ESTADOS DE ENTREGA";
                r1.Style.Font.Bold = true;
                r1.Style.Font.FontSize = 14;
                r1.Style.Font.FontColor = XLColor.White;
                r1.Style.Fill.BackgroundColor = cHeader;
                r1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r1.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fila 2 — Subtítulo
                ws.Row(2).Height = 16;
                var r2 = ws.Range(2, 1, 2, COLS);
                r2.Merge();
                r2.Value = "Centro de Fermentación y Secado";
                r2.Style.Font.FontSize = 9;
                r2.Style.Font.FontColor = cSubtit;
                r2.Style.Fill.BackgroundColor = cHeader;
                r2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fila 3 — Meta: fecha + total registros
                ws.Row(3).Height = 13;
                var r3 = ws.Range(3, 1, 3, COLS);
                r3.Merge();
                r3.Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  " +
                                                 $"Total de registros: {historial.Count:N0}";
                r3.Style.Font.FontSize = 8;
                r3.Style.Font.FontColor = cMeta;
                r3.Style.Fill.BackgroundColor = cHeader;
                r3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fila 4 — Separador visual
                ws.Row(4).Height = 6;
                ws.Range(4, 1, 4, COLS).Style.Fill.BackgroundColor = cBg;

                
                ws.Row(5).Height = 18;
                for (int c = 0; c < COLS; c++)
                {
                    var cell = ws.Cell(5, c + 1);
                    cell.Value = headers[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontSize = 9;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = cLabel;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = cBorder;
                }

                // Centradas: Fecha y hora=0, Entrega=1, Estado=3
                // Izquierda con indent: Lugar=2, Observaciones=4
                var colsCentradas = new HashSet<int> { 0, 1, 3 };

                int dataRow = 6;
                int rowNum = 0;

                foreach (var h in historial)
                {
                    ws.Row(dataRow).Height = 15;

                    var rowBg = rowNum % 2 == 0 ? cRowImpar : cRowPar;
                    string estado = h.EstadoEntrega?.Nombre ?? "Desconocido";
                    var colorEstado = ObtenerColorEstado(estado.ToLowerInvariant());
                    string lugar = entregasDict.TryGetValue(h.EntregaId, out var ent)
                        ? ObtenerLugarEntrega(ent) : "—";

                    string[] valores =
                    {
                h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss"),              // 0 Fecha y hora
                $"E-{h.EntregaId:D4}",                                      // 1 Entrega
                lugar,                                                       // 2 Lugar en almacén
                estado,                                                      // 3 Estado
                string.IsNullOrWhiteSpace(h.Observaciones) ? "—"
                    : h.Observaciones,                                       // 4 Observaciones
            };

                    for (int c = 0; c < COLS; c++)
                    {
                        var cell = ws.Cell(dataRow, c + 1);
                        cell.Value = valores[c];
                        cell.Style.Font.FontSize = 9;
                        cell.Style.Font.FontColor = cHeader;
                        cell.Style.Fill.BackgroundColor = rowBg;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Alignment.WrapText = false;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = cBorder;

                        cell.Style.Alignment.Horizontal = colsCentradas.Contains(c)
                            ? XLAlignmentHorizontalValues.Center
                            : XLAlignmentHorizontalValues.Left;

                        if (!colsCentradas.Contains(c))
                            cell.Style.Alignment.Indent = 1;

                        // Fecha y hora en Consolas para mejor legibilidad
                        if (c == 0)
                        {
                            cell.Style.Font.FontName = "Consolas";
                            cell.Style.Font.FontSize = 8.5;
                        }
                    }

                    // Color de estado: texto coloreado + fondo suave al 25 %
                    var cellEstado = ws.Cell(dataRow, 4);
                    cellEstado.Style.Font.Bold = true;
                    cellEstado.Style.Font.FontColor = XLColor.FromArgb(
                        colorEstado.R, colorEstado.G, colorEstado.B);
                    cellEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(
                        255 - (255 - colorEstado.R) / 4,
                        255 - (255 - colorEstado.G) / 4,
                        255 - (255 - colorEstado.B) / 4);

                    dataRow++;
                    rowNum++;
                }

                int totalRow = dataRow;
                ws.Row(totalRow).Height = 17;

                // Conteo de registros por estado
                var porEstado = historial
                    .GroupBy(h => h.EstadoEntrega?.Nombre ?? "Desconocido")
                    .OrderByDescending(g => g.Count())
                    .ToList();

                string resumenEstados = string.Join("  ·  ",
                    porEstado.Select(g => $"{g.Key}: {g.Count():N0}"));

                // Etiqueta fusionada cols 1-4
                var rTotLabel = ws.Range(totalRow, 1, totalRow, 4);
                rTotLabel.Merge();
                rTotLabel.Value = $"Total  —  {historial.Count:N0} registros  ·  {resumenEstados}";
                rTotLabel.Style.Font.Bold = true;
                rTotLabel.Style.Font.FontSize = 9;
                rTotLabel.Style.Font.FontColor = cLabel;
                rTotLabel.Style.Fill.BackgroundColor = cFooterBg;
                rTotLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                rTotLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rTotLabel.Style.Alignment.Indent = 1;
                rTotLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rTotLabel.Style.Border.OutsideBorderColor = cBorder;

                // Celda numérica total en col 5
                var cTotVal = ws.Cell(totalRow, 5);
                cTotVal.Value = historial.Count;
                cTotVal.Style.Font.Bold = true;
                cTotVal.Style.Font.FontSize = 9;
                cTotVal.Style.Font.FontColor = cHeader;
                cTotVal.Style.Fill.BackgroundColor = cFooterBg;
                cTotVal.Style.NumberFormat.Format = "#,##0";
                cTotVal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cTotVal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cTotVal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cTotVal.Style.Border.OutsideBorderColor = cBorder;

                ws.Range(5, 1, dataRow - 1, COLS).SetAutoFilter();
                ws.SheetView.FreezeRows(5);
                ws.SheetView.ZoomScale = 110;

                ws.Range(5, 1, totalRow, COLS).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                ws.Range(5, 1, totalRow, COLS).Style.Border.OutsideBorderColor = cBorder;

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