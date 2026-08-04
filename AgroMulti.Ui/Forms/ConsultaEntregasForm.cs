using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Requests;
using AgroMulti.Ui.Services;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgroMulti.Ui.Forms
{
    public partial class ConsultaEntregasForm : Form
    {
        // ── Última consulta (para exportar exactamente lo visible) ───
        private List<EntregaDto> _ultimosResultados = new();

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

            QuestPDF.Settings.License = LicenseType.Community;

            Load += async (s, e) => await InicializarAsync();
        }

        // ── Helper genérico para consumir la API ────────────────────────
        private static async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            var response = await ApiClient.Client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<List<T>>>();
            return wrapper?.Data ?? new List<T>();
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
                var productores = (await GetListAsync<ProductorDto>("api/Productores"))
                    .OrderBy(p => p.Codigo).ToList();
                productores.Insert(0, new ProductorDto { Id = 0, Codigo = "(Todos)", Nombre = "", Apellido = "" });
                cboProductor.DataSource = productores;
                cboProductor.DisplayMember = "Codigo";
                cboProductor.ValueMember = "Id";

                var productos = (await GetListAsync<ProductoDto>("api/Productos"))
                    .OrderBy(p => p.Nombre).ToList();
                productos.Insert(0, new ProductoDto { Id = 0, Nombre = "(Todos)" });
                cboProducto.DataSource = productos;
                cboProducto.DisplayMember = "Nombre";
                cboProducto.ValueMember = "Id";

                var estados = (await GetListAsync<EstadoEntregaDto>("api/EstadoEntregas"))
                    .OrderBy(e => e.Nombre).ToList();
                estados.Insert(0, new EstadoEntregaDto { Id = 0, Nombre = "(Todos)" });
                cboEstado.DataSource = estados;
                cboEstado.DisplayMember = "Nombre";
                cboEstado.ValueMember = "Id";
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
                var cHeader = XLColor.FromArgb(38, 22, 10);
                var cSubtit = XLColor.FromArgb(201, 181, 157);
                var cMeta = XLColor.FromArgb(184, 158, 130);
                var cBg = XLColor.FromArgb(244, 239, 231);
                var cBorder = XLColor.FromArgb(216, 200, 184);
                var cLabel = XLColor.FromArgb(107, 76, 50);
                var cRowPar = XLColor.FromArgb(250, 247, 242);
                var cRowImpar = XLColor.White;
                var cFooterBg = XLColor.FromArgb(239, 231, 219);

                const int COLS = 11;

                string[] headers =
                {
                    "Número", "Fecha", "Productor", "Producto", "Subproducto",
                    "Kilos", "Estado", "Lugar en almacén", "Placa", "Conductor", "Observaciones"
                };

                int[] colWidths = { 12, 12, 26, 18, 18, 11, 14, 24, 11, 22, 30 };

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Entregas");

                for (int c = 1; c <= COLS; c++)
                    ws.Column(c).Width = colWidths[c - 1];

                ws.Row(1).Height = 23;
                var r1 = ws.Range(1, 1, 1, COLS);
                r1.Merge();
                r1.Value = "CONSULTA DE ENTREGAS";
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
                           $"Resultados encontrados: {_ultimosResultados.Count:N0}";
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

                var colsCentradas = new HashSet<int> { 0, 1, 5, 6, 8 };

                int dataRow = 6;
                int rowNum = 0;

                foreach (var en in _ultimosResultados)
                {
                    ws.Row(dataRow).Height = 15;

                    var rowBg = rowNum % 2 == 0 ? cRowImpar : cRowPar;
                    var colorEstado = ObtenerColorEstado(en.Estado.ToLowerInvariant());
                    string lugar = ObtenerLugar(en);

                    object[] valores =
                    {
                        en.NumeroEntrega,
                        en.FechaEntrega.ToString("dd/MM/yyyy"),
                        en.Productor,
                        en.Producto,
                        en.SubProducto ?? "—",
                        (object)en.Kilos,
                        en.Estado,
                        lugar,
                        en.Placa           ?? "—",
                        en.NombreConductor ?? "—",
                        string.IsNullOrWhiteSpace(en.Observaciones) ? "—" : en.Observaciones,
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

                        if (c == 5 && valores[c] is not string)
                            cell.Style.NumberFormat.Format = "#,##0.00";
                    }

                    var cellEstado = ws.Cell(dataRow, 7);
                    cellEstado.Style.Font.Bold = true;
                    cellEstado.Style.Font.FontColor = XLColor.FromArgb(
                        colorEstado.R, colorEstado.G, colorEstado.B);

                    dataRow++;
                    rowNum++;
                }

                int totalRow = dataRow;
                ws.Row(totalRow).Height = 17;

                double sumKilos = _ultimosResultados.Sum(en => (double)en.Kilos);

                var rTotLabel = ws.Range(totalRow, 1, totalRow, 5);
                rTotLabel.Merge();
                rTotLabel.Value = $"Total  —  {_ultimosResultados.Count:N0} entregas encontradas";
                rTotLabel.Style.Font.Bold = true;
                rTotLabel.Style.Font.FontSize = 9;
                rTotLabel.Style.Font.FontColor = cLabel;
                rTotLabel.Style.Fill.BackgroundColor = cFooterBg;
                rTotLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                rTotLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rTotLabel.Style.Alignment.Indent = 1;
                rTotLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rTotLabel.Style.Border.OutsideBorderColor = cBorder;

                var cTotKilos = ws.Cell(totalRow, 6);
                cTotKilos.Value = sumKilos;
                cTotKilos.Style.Font.Bold = true;
                cTotKilos.Style.Font.FontSize = 9;
                cTotKilos.Style.Font.FontColor = cHeader;
                cTotKilos.Style.Fill.BackgroundColor = cFooterBg;
                cTotKilos.Style.NumberFormat.Format = "#,##0.00";
                cTotKilos.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cTotKilos.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cTotKilos.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cTotKilos.Style.Border.OutsideBorderColor = cBorder;

                for (int c = 7; c <= COLS; c++)
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

                MessageBox.Show($"Exportado correctamente:\n{sfd.FileName}",
                    "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                var datos = _ultimosResultados;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.PageColor("#26160A");
                        page.Margin(0.5f, Unit.Centimetre);

                        page.Header()
                            .Background("#26160A")
                            .Padding(12)
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

                        page.Content()
                            .Background("#F4EFE7")
                            .PaddingHorizontal(18)
                            .PaddingVertical(14)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(1.2f);
                                    cols.RelativeColumn(1.0f);
                                    cols.RelativeColumn(1.8f);
                                    cols.RelativeColumn(1.2f);
                                    cols.RelativeColumn(1.1f);
                                    cols.RelativeColumn(0.8f);
                                    cols.RelativeColumn(1.2f);
                                    cols.RelativeColumn(1.4f);
                                    cols.RelativeColumn(0.9f);
                                    cols.RelativeColumn(1.4f);
                                    cols.RelativeColumn(2.0f);
                                });

                                table.Header(header =>
                                {
                                    foreach (string h in new[]
                                    {
                                        "Número", "Fecha", "Productor", "Producto", "Subproducto",
                                        "Kilos", "Estado", "Lugar", "Placa", "Conductor", "Observaciones"
                                    })
                                    {
                                        header.Cell().Background("#3A2612")
                                            .Padding(4).AlignCenter()
                                            .Text(h).FontColor("#FFFFFF").Bold().FontSize(8f);
                                    }
                                });

                                int index = 0;

                                foreach (var en in datos)
                                {
                                    var colorEst = ObtenerColorEstado(en.Estado.ToLowerInvariant());
                                    string colorHex = $"#{colorEst.R:X2}{colorEst.G:X2}{colorEst.B:X2}";
                                    string lugar = ObtenerLugar(en);
                                    string fondoFila = index % 2 == 0 ? "#F8F4EE" : "#EFE7DB";
                                    string placa = en.Placa ?? "—";
                                    string conductor = en.NombreConductor ?? "—";

                                    IContainer EstiloCelda(IContainer cell) =>
                                        cell.Background(fondoFila)
                                            .BorderBottom(1)
                                            .BorderColor("#D7C8B5")
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(4);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.NumeroEntrega).Bold().FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.FechaEntrega.ToString("dd/MM/yyyy")).FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.Productor).FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.Producto).FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.SubProducto ?? "—").FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .AlignRight()
                                        .Text(en.Kilos.ToString("N2")).FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(en.Estado).FontColor(colorHex).Bold().FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(lugar).FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .AlignCenter()
                                        .Text(placa).FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(conductor).FontSize(8f);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(string.IsNullOrWhiteSpace(en.Observaciones) ? "—" : en.Observaciones)
                                        .FontSize(8f);

                                    index++;
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

                var todas = await GetListAsync<EntregaDto>("api/Entregas");

                var query = todas.Where(e => e.FechaEntrega >= desde && e.FechaEntrega <= hasta);
                if (productorId > 0) query = query.Where(e => e.ProductorId == productorId);
                if (productoId > 0) query = query.Where(e => e.ProductoId == productoId);
                if (estadoId > 0) query = query.Where(e => e.EstadoEntregaId == estadoId);

                _ultimosResultados = query
                    .OrderByDescending(e => e.FechaEntrega)
                    .ThenByDescending(e => e.Id)
                    .ToList();

                dgvEntregas.Rows.Clear();

                foreach (var entrega in _ultimosResultados)
                {
                    dgvEntregas.Rows.Add(
                        entrega.Id,
                        entrega.NumeroEntrega,
                        entrega.FechaEntrega.ToString("dd/MM/yyyy"),
                        entrega.Productor,
                        entrega.Producto,
                        entrega.SubProducto ?? "",
                        entrega.Kilos.ToString("N2"),
                        entrega.Estado,
                        entrega.Placa ?? "",
                        entrega.NombreConductor ?? "",
                        entrega.Observaciones ?? ""
                    );
                }

                dgvEntregas.ClearSelection();

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
                var entrega = _ultimosResultados.FirstOrDefault(en => en.Id == entregaId);
                if (entrega == null)
                {
                    MessageBox.Show("No se encontró la entrega.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var estados = await GetListAsync<EstadoEntregaDto>("api/EstadoEntregas");
                var estadoNuevo = estados.FirstOrDefault(es =>
                    es.Nombre.Equals(nuevoEstado, StringComparison.OrdinalIgnoreCase));

                if (estadoNuevo == null)
                {
                    MessageBox.Show(
                        $"No se encontró el estado «{nuevoEstado}» en la base de datos.\nVerifique que los nombres de estados coincidan exactamente.",
                        "Estado no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var actualizarRequest = new ActualizarEntregaRequest
                {
                    NumeroEntrega = entrega.NumeroEntrega,
                    FechaEntrega = entrega.FechaEntrega,
                    ProductorId = entrega.ProductorId,
                    ProductoId = entrega.ProductoId,
                    SubProductoId = entrega.SubProductoId,
                    EstadoEntregaId = estadoNuevo.Id,
                    Placa = entrega.Placa,
                    NombreConductor = entrega.NombreConductor,
                    Kilos = entrega.Kilos,
                    Cajas = entrega.Cajas,
                    Sacos = entrega.Sacos,
                    KilosSecos = entrega.KilosSecos,
                    Pasillo = entrega.Pasillo,
                    NumeroAnaquel = entrega.NumeroAnaquel,
                    Piso = entrega.Piso,
                    Observaciones = entrega.Observaciones
                };

                var putResponse = await ApiClient.Client.PutAsJsonAsync($"api/Entregas/{entregaId}", actualizarRequest);
                if (!putResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("No se pudo actualizar la entrega.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var historicoRequest = new CrearHistoricoEstadoEntregaRequest
                {
                    EntregaId = entregaId,
                    EstadoEntregaId = estadoNuevo.Id,
                    FechaCambio = DateTime.Now,
                    Observaciones = $"Cambio de estado de '{estadoActual}' a '{nuevoEstado}'"
                };

                var historicoResponse = await ApiClient.Client.PostAsJsonAsync("api/HistoricoEstadoEntregas", historicoRequest);
                if (!historicoResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "El estado se actualizó, pero no se pudo registrar en el historial.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

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
        private static string ObtenerLugar(EntregaDto en)
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