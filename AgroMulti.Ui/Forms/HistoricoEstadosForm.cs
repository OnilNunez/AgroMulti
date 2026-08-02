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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AgroMulti.Ui
{
    public partial class HistoricoEstadosForm : Form
    {
        // ── Datos ────────────────────────────────────────────────────
        private List<HistoricoEstadoEntrega> _historial;
        private List<HistoricoEstadoEntrega> _historialFiltrado;
        private Dictionary<int, Entrega> _entregasDict;

        // ── Recursos visuales ────────────────────────────────────────
        private static readonly Font _fntEstado = new Font("Segoe UI", 9F, FontStyle.Bold);

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

        // ── Servicios ────────────────────────────────────────────────
        private readonly HistoricoEstadoEntregaService _historicoService;
        private readonly EntregaService _entregaService;

        // ── Constructor ──────────────────────────────────────────────
        public HistoricoEstadosForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgvHistorial, new object[] { true });

            _historicoService = Program.ServiceProvider.GetRequiredService<HistoricoEstadoEntregaService>();
            _entregaService = Program.ServiceProvider.GetRequiredService<EntregaService>();

            this.Load += HistoricoEstadosForm_Load;
            ConfigurarTooltips();

            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ── Carga inicial ────────────────────────────────────────────
        private async void HistoricoEstadosForm_Load(object sender, EventArgs e)
        {
            SetControlesActivos(false);
            try
            {
                var entregas = await _entregaService.GetListConRelaciones(_ => true);
                _entregasDict = entregas.ToDictionary(en => en.EntregaId);

                _historial = await _historicoService.ObtenerTodosAsync();
                _historialFiltrado = new List<HistoricoEstadoEntrega>(_historial);

                PoblarComboEstados(mantenerSeleccion: false);
                PoblarComboEntregas(mantenerSeleccion: false);
                InicializarFechas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _historial = new List<HistoricoEstadoEntrega>();
                _historialFiltrado = new List<HistoricoEstadoEntrega>();
                _entregasDict = new Dictionary<int, Entrega>();
            }
            finally
            {
                SetControlesActivos(true);
            }

            ConfigurarColumnasDgv();
            CargarHistorial();
        }

        // ── Refrescar ────────────────────────────────────────────────
        private async void BtnRefrescar_Click(object sender, EventArgs e)
        {
            SetControlesActivos(false);
            try
            {
                var entregas = await _entregaService.GetListConRelaciones(_ => true);
                _entregasDict = entregas.ToDictionary(en => en.EntregaId);
                _historial = await _historicoService.ObtenerTodosAsync();

                PoblarComboEstados(mantenerSeleccion: true);
                PoblarComboEntregas(mantenerSeleccion: true);
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al refrescar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlesActivos(true);
            }
        }

        // ── Exportar (mostrar menú) ──────────────────────────────────
        private void BtnExportar_Click(object sender, EventArgs e)
        {
            ctxExportar.Show(btnExportar, new System.Drawing.Point(0, btnExportar.Height));
        }

        private void ItemExportarExcel_Click(object sender, EventArgs e)
        {
            if (_historialFiltrado == null || _historialFiltrado.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Exportar a Excel",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar a Excel",
                Filter = "Libro de Excel (*.xlsx)|*.xlsx",
                FileName = $"HistorialEstados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
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
                var cBorder = XLColor.FromArgb(216, 200, 184);
                var cLabel = XLColor.FromArgb(107, 76, 50);
                var cRowPar = XLColor.FromArgb(250, 247, 242);
                var cRowImpar = XLColor.White;
                var cFooterBg = XLColor.FromArgb(239, 231, 219);
                var cCardBg = XLColor.FromArgb(252, 249, 244);

                const int COLS = 5;

                string[] headers = { "Fecha y hora", "Entrega", "Lugar en almacén", "Estado", "Observaciones" };
                int[] colWidths = { 20, 10, 28, 16, 45 };

                // ── Datos de resumen precalculados ────────────────────────────
                var porEstado = _historialFiltrado
                    .GroupBy(h => h.EstadoEntrega?.Nombre ?? "Desconocido")
                    .Select(g => new
                    {
                        Estado = g.Key,
                        Cantidad = g.Count(),
                        Pct = (double)g.Count() / _historialFiltrado.Count * 100,
                        Color = ObtenerColorFromName(g.Key.ToLowerInvariant())
                    })
                    .OrderByDescending(x => x.Cantidad)
                    .ToList();

                // Rango de fechas del historial filtrado
                var fechaMin = _historialFiltrado.Min(h => h.FechaCambio);
                var fechaMax = _historialFiltrado.Max(h => h.FechaCambio);

                using var wb = new XLWorkbook();


                var ws = wb.Worksheets.Add("Historial");

                for (int c = 1; c <= COLS; c++)
                    ws.Column(c).Width = colWidths[c - 1];

                // ── Filas 1-3: bloque de encabezado ───────────────────────────
                ws.Row(1).Height = 23;
                var r1 = ws.Range(1, 1, 1, COLS);
                r1.Merge();
                r1.Value = "HISTORIAL DE MOVIMIENTOS DE ESTADOS";
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
                                                 $"Período: {fechaMin:dd/MM/yyyy} – {fechaMax:dd/MM/yyyy}  ·  " +
                                                 $"Total registros: {_historialFiltrado.Count:N0}";
                r3.Style.Font.FontSize = 8;
                r3.Style.Font.FontColor = cMeta;
                r3.Style.Fill.BackgroundColor = cHeader;
                r3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                r3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fila 4 — Separador
                ws.Row(4).Height = 6;
                ws.Range(4, 1, 4, COLS).Style.Fill.BackgroundColor = cBg;

                // ── Fila 5: encabezados de columna ────────────────────────────
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

                // ── Filas de datos desde fila 6 ───────────────────────────────
                // Centradas: Fecha y hora=0, Entrega=1, Estado=3
                var colsCentradas = new HashSet<int> { 0, 1, 3 };

                int dataRow = 6;
                int rowNum = 0;

                foreach (var h in _historialFiltrado)
                {
                    ws.Row(dataRow).Height = 15;

                    var rowBg = rowNum % 2 == 0 ? cRowImpar : cRowPar;
                    string estado = h.EstadoEntrega?.Nombre ?? "Desconocido";
                    var colorEstado = ObtenerColorFromName(estado.ToLowerInvariant());

                    string[] valores =
                    {
                        h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss"),             // 0 Fecha y hora
                        $"E-{h.EntregaId:D4}",                                      // 1 Entrega
                        ObtenerLugar(h.EntregaId),                                 // 2 Lugar en almacén
                        estado,                                                    // 3 Estado
                        string.IsNullOrWhiteSpace(h.Observaciones) ? "—"
                            : h.Observaciones,                                     // 4 Observaciones
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

                        // Fecha en Consolas
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

                // ── Fila de totales ───────────────────────────────────────────
                int totalRow = dataRow;
                ws.Row(totalRow).Height = 17;

                // Desglose por estado en la etiqueta
                string resumenEstados = string.Join("  ·  ",
                    porEstado.Select(x => $"{x.Estado}: {x.Cantidad:N0}"));

                var rTotLabel = ws.Range(totalRow, 1, totalRow, 4);
                rTotLabel.Merge();
                rTotLabel.Value = $"Total  —  {_historialFiltrado.Count:N0} movimientos  ·  {resumenEstados}";
                rTotLabel.Style.Font.Bold = true;
                rTotLabel.Style.Font.FontSize = 9;
                rTotLabel.Style.Font.FontColor = cLabel;
                rTotLabel.Style.Fill.BackgroundColor = cFooterBg;
                rTotLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                rTotLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rTotLabel.Style.Alignment.Indent = 1;
                rTotLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rTotLabel.Style.Border.OutsideBorderColor = cBorder;

                var cTotVal = ws.Cell(totalRow, 5);
                cTotVal.Value = _historialFiltrado.Count;
                cTotVal.Style.Font.Bold = true;
                cTotVal.Style.Font.FontSize = 9;
                cTotVal.Style.Font.FontColor = cHeader;
                cTotVal.Style.Fill.BackgroundColor = cFooterBg;
                cTotVal.Style.NumberFormat.Format = "#,##0";
                cTotVal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cTotVal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cTotVal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cTotVal.Style.Border.OutsideBorderColor = cBorder;

                // Filtros + freeze + zoom
                ws.Range(5, 1, dataRow - 1, COLS).SetAutoFilter();
                ws.SheetView.FreezeRows(5);
                ws.SheetView.ZoomScale = 110;

                // Borde exterior
                ws.Range(5, 1, totalRow, COLS).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                ws.Range(5, 1, totalRow, COLS).Style.Border.OutsideBorderColor = cBorder;


                var wsRes = wb.Worksheets.Add("Resumen por estado");
                wsRes.Column(1).Width = 24;  // Estado
                wsRes.Column(2).Width = 14;  // Movimientos
                wsRes.Column(3).Width = 14;  // Porcentaje
                wsRes.Column(4).Width = 22;  // Barra visual

                // Encabezado hoja 2
                wsRes.Row(1).Height = 23;
                var wr1 = wsRes.Range(1, 1, 1, 4);
                wr1.Merge();
                wr1.Value = "RESUMEN DE MOVIMIENTOS POR ESTADO";
                wr1.Style.Font.Bold = true;
                wr1.Style.Font.FontSize = 14;
                wr1.Style.Font.FontColor = XLColor.White;
                wr1.Style.Fill.BackgroundColor = cHeader;
                wr1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wr1.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                wsRes.Row(2).Height = 16;
                var wr2 = wsRes.Range(2, 1, 2, 4);
                wr2.Merge();
                wr2.Value = "Centro de Fermentación y Secado";
                wr2.Style.Font.FontSize = 9;
                wr2.Style.Font.FontColor = cSubtit;
                wr2.Style.Fill.BackgroundColor = cHeader;
                wr2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wr2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                wsRes.Row(3).Height = 13;
                var wr3 = wsRes.Range(3, 1, 3, 4);
                wr3.Merge();
                wr3.Value = $"Período: {fechaMin:dd/MM/yyyy} – {fechaMax:dd/MM/yyyy}  ·  " +
                                                  $"Total registros: {_historialFiltrado.Count:N0}";
                wr3.Style.Font.FontSize = 8;
                wr3.Style.Font.FontColor = cMeta;
                wr3.Style.Fill.BackgroundColor = cHeader;
                wr3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wr3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                wsRes.Row(4).Height = 6;
                wsRes.Range(4, 1, 4, 4).Style.Fill.BackgroundColor = cBg;

                // Encabezados de columna hoja 2
                wsRes.Row(5).Height = 18;
                string[] resHeaders = { "Estado", "Movimientos", "Porcentaje", "Distribución visual" };
                for (int c = 0; c < resHeaders.Length; c++)
                {
                    var cell = wsRes.Cell(5, c + 1);
                    cell.Value = resHeaders[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontSize = 9;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = cLabel;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = cBorder;
                }

                // Filas de datos hoja 2
                int resRow = 6;
                int resNum = 0;
                foreach (var est in porEstado)
                {
                    wsRes.Row(resRow).Height = 16;
                    var resBg = resNum % 2 == 0 ? cRowImpar : cRowPar;

                    // Barra visual proporcional (max 20 bloques)
                    int bloques = (int)Math.Round(est.Pct / 100.0 * 20);
                    string barra = new string('█', bloques) + new string('░', 20 - bloques);

                    string[] resVals = { est.Estado, est.Cantidad.ToString("N0"),
                                  $"{est.Pct:N1} %", barra };

                    for (int c = 0; c < 4; c++)
                    {
                        var cell = wsRes.Cell(resRow, c + 1);
                        cell.Value = resVals[c];
                        cell.Style.Font.FontSize = c == 3 ? 8 : 9;
                        cell.Style.Font.FontColor = cHeader;
                        cell.Style.Fill.BackgroundColor = resBg;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Alignment.WrapText = false;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = cBorder;
                        cell.Style.Alignment.Horizontal = c == 0
                            ? XLAlignmentHorizontalValues.Left
                            : XLAlignmentHorizontalValues.Center;
                        if (c == 0) cell.Style.Alignment.Indent = 1;
                    }

                    // Color del estado en col 1 + fondo suave
                    var cEst = wsRes.Cell(resRow, 1);
                    cEst.Style.Font.Bold = true;
                    cEst.Style.Font.FontColor = XLColor.FromArgb(
                        est.Color.R, est.Color.G, est.Color.B);
                    cEst.Style.Fill.BackgroundColor = XLColor.FromArgb(
                        255 - (255 - est.Color.R) / 4,
                        255 - (255 - est.Color.G) / 4,
                        255 - (255 - est.Color.B) / 4);

                    // Color de la barra proporcional
                    var cBarra = wsRes.Cell(resRow, 4);
                    cBarra.Style.Font.FontColor = XLColor.FromArgb(
                        est.Color.R, est.Color.G, est.Color.B);
                    cBarra.Style.Font.FontName = "Consolas";

                    resRow++;
                    resNum++;
                }

                // Fila de total hoja 2
                wsRes.Row(resRow).Height = 17;
                var rResTotal = wsRes.Range(resRow, 1, resRow, 3);
                rResTotal.Merge();
                rResTotal.Value = $"Total general  —  {porEstado.Count} estado(s) distintos";
                rResTotal.Style.Font.Bold = true;
                rResTotal.Style.Font.FontSize = 9;
                rResTotal.Style.Font.FontColor = cLabel;
                rResTotal.Style.Fill.BackgroundColor = cFooterBg;
                rResTotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                rResTotal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rResTotal.Style.Alignment.Indent = 1;
                rResTotal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rResTotal.Style.Border.OutsideBorderColor = cBorder;

                var cResTot = wsRes.Cell(resRow, 4);
                cResTot.Value = _historialFiltrado.Count;
                cResTot.Style.Font.Bold = true;
                cResTot.Style.Font.FontSize = 9;
                cResTot.Style.Font.FontColor = cHeader;
                cResTot.Style.Fill.BackgroundColor = cFooterBg;
                cResTot.Style.NumberFormat.Format = "#,##0";
                cResTot.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cResTot.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cResTot.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cResTot.Style.Border.OutsideBorderColor = cBorder;

                // Borde exterior hoja 2 + freeze + zoom
                wsRes.Range(5, 1, resRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                wsRes.Range(5, 1, resRow, 4).Style.Border.OutsideBorderColor = cBorder;
                wsRes.SheetView.FreezeRows(5);
                wsRes.SheetView.ZoomScale = 110;

                // Hoja 1 activa al abrir
                wb.Worksheet("Historial").SetTabActive();

                wb.SaveAs(sfd.FileName);

                MessageBox.Show($"Exportado correctamente a Excel:\n{sfd.FileName}",
                    "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a Excel: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemExportarPDF_Click(object sender, EventArgs e)
        {
            if (_historialFiltrado == null || _historialFiltrado.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Exportar a PDF",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar a PDF",
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = $"HistorialEstados_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                DefaultExt = "pdf"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var historialLocal = _historialFiltrado;

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
                                    .FontFamily("Segoe UI")
                                    .FontSize(22)
                                    .Bold()
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
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2.2f); // Fecha
                                    columns.RelativeColumn(1.3f); // Entrega
                                    columns.RelativeColumn(2.0f); // Lugar
                                    columns.RelativeColumn(1.5f); // Estado
                                    columns.RelativeColumn(3.0f); // Observaciones
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text("Fecha y hora").FontColor("#FFFFFF").Bold().FontSize(9);
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text("Entrega").FontColor("#FFFFFF").Bold().FontSize(9);
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text("Lugar en almacén").FontColor("#FFFFFF").Bold().FontSize(9);
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text("Estado").FontColor("#FFFFFF").Bold().FontSize(9);
                                    header.Cell().Background("#3A2612")
                                        .Padding(4).AlignCenter()
                                        .Text("Observaciones").FontColor("#FFFFFF").Bold().FontSize(9);
                                });

                                foreach (var h in historialLocal)
                                {
                                    string estado = h.EstadoEntrega?.Nombre ?? "Desconocido";
                                    var colorEstado = ObtenerColorFromName(estado.ToLowerInvariant());
                                    string colorHex = $"#{colorEstado.R:X2}{colorEstado.G:X2}{colorEstado.B:X2}";

                                    IContainer EstiloCelda(IContainer cell)
                                    {
                                        return cell
                                            .BorderBottom(1)
                                            .BorderColor("#DED2C2")
                                            .Padding(3);
                                    }

                                    table.Cell().Element(EstiloCelda)
                                        .Text(h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss"))
                                        .FontSize(8.5f)
                                        .FontFamily("Consolas");

                                    table.Cell().Element(EstiloCelda)
                                        .Text($"E-{h.EntregaId:D4}")
                                        .Bold()
                                        .FontSize(9);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(ObtenerLugar(h.EntregaId))
                                        .FontSize(9);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(estado)
                                        .FontColor(colorHex)
                                        .Bold()
                                        .FontSize(9);

                                    table.Cell().Element(EstiloCelda)
                                        .Text(string.IsNullOrWhiteSpace(h.Observaciones) ? "—" : h.Observaciones)
                                        .FontSize(9);
                                }
                            });

                        page.Footer()
                            .Background("#26160A")
                            .PaddingVertical(8)
                            .PaddingHorizontal(20)
                            .AlignRight()
                            .Text(text =>
                            {
                                text.Span("Página ").FontSize(8).FontColor("#D8C2A5");
                                text.CurrentPageNumber().FontSize(8).FontColor("#D8C2A5");
                                text.Span(" de ").FontSize(8).FontColor("#D8C2A5");
                                text.TotalPages().FontSize(8).FontColor("#D8C2A5");
                            });
                    });
                }).GeneratePdf(sfd.FileName);

                MessageBox.Show($"Exportado correctamente a PDF:\n{sfd.FileName}",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a PDF: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Convierte nombre de estado a System.Drawing.Color
        private static System.Drawing.Color ObtenerColorFromName(string estadoLower)
        {
            foreach (var par in _coloresEstado)
                if (estadoLower.Contains(par.Clave))
                    return par.Color;
            return System.Drawing.Color.FromArgb(80, 55, 30);
        }

        // ── Filtros ──────────────────────────────────────────────────
        private void BtnFiltrar_Click(object sender, EventArgs e) => AplicarFiltros();
        private void BtnLimpiarFiltros_Click(object sender, EventArgs e) => LimpiarFiltros();

        private void AplicarFiltros()
        {
            if (_historial == null) return;

            int? entregaSeleccionadaId = null;
            if (cmbBuscarEntrega.SelectedIndex > 0)
            {
                string item = cmbBuscarEntrega.SelectedItem.ToString();
                if (item.StartsWith("E-") && int.TryParse(item.Substring(2), out int id))
                    entregaSeleccionadaId = id;
            }

            string estadoFiltro = cmbFiltroEstado.SelectedIndex > 0
                ? cmbFiltroEstado.SelectedItem?.ToString() : null;
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);

            _historialFiltrado = _historial.Where(h =>
            {
                if (h.FechaCambio < desde || h.FechaCambio > hasta)
                    return false;

                if (entregaSeleccionadaId.HasValue && h.EntregaId != entregaSeleccionadaId.Value)
                    return false;

                if (estadoFiltro != null)
                {
                    string nombre = h.EstadoEntrega?.Nombre ?? "Desconocido";
                    if (!nombre.Equals(estadoFiltro, StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                return true;
            }).ToList();

            CargarHistorial();
        }

        private void LimpiarFiltros()
        {
            cmbBuscarEntrega.SelectedIndex = 0;
            cmbFiltroEstado.SelectedIndex = 0;
            InicializarFechas();
            _historialFiltrado = new List<HistoricoEstadoEntrega>(
                _historial ?? new List<HistoricoEstadoEntrega>());
            CargarHistorial();
        }

        // ── Cerrar ───────────────────────────────────────────────────
        private void BtnCerrar_Click(object sender, EventArgs e) => this.Close();

        // ── Configuración de columnas del DGV ────────────────────────
        private void ConfigurarColumnasDgv()
        {
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorial.Columns[0].FillWeight = 18;  // Fecha y hora
            dgvHistorial.Columns[1].FillWeight = 10;  // Entrega
            dgvHistorial.Columns[2].FillWeight = 20;  // Lugar en almacén
            dgvHistorial.Columns[3].FillWeight = 14;  // Estado
            dgvHistorial.Columns[4].FillWeight = 38;  // Observaciones
        }

        // ── Carga en la grilla ───────────────────────────────────────
        private void CargarHistorial()
        {
            dgvHistorial.Rows.Clear();
            lblCardEstadoTitle.Text = "Entregas";

            var fuente = _historialFiltrado ?? _historial;

            if (fuente == null || fuente.Count == 0)
            {
                dgvHistorial.Rows.Add("—", "—", "—", "Sin registros",
                    "No hay cambios registrados para los criterios actuales.");
                dgvHistorial.Enabled = false;
                lblCardTotalValue.Text = "0";
                lblCardUltimoValue.Text = "—";
                lblCardEstadoValue.Text = "0";
                lblCardEstadoValue.ForeColor = System.Drawing.Color.FromArgb(128, 105, 82);
                lblSubtitle.Text = _historial?.Count > 0
                    ? "Registro cronológico de transiciones · Sin resultados para los filtros aplicados"
                    : "Registro cronológico de transiciones · Sin datos";
                return;
            }

            dgvHistorial.Enabled = true;

            foreach (var item in fuente)
            {
                string fecha = item.FechaCambio.ToString("dd/MM/yyyy  HH:mm:ss");
                string entrega = $"E-{item.EntregaId:D4}";
                string lugar = ObtenerLugar(item.EntregaId);
                string estado = item.EstadoEntrega?.Nombre ?? "Desconocido";
                string obs = string.IsNullOrWhiteSpace(item.Observaciones)
                                    ? "—" : item.Observaciones;
                dgvHistorial.Rows.Add(fecha, entrega, lugar, estado, obs);
            }

            lblCardTotalValue.Text = fuente.Count.ToString("N0");
            lblCardUltimoValue.Text = fuente.Last().FechaCambio.ToString("dd/MM/yyyy  HH:mm");

            int entregasDistintas = fuente.Select(h => h.EntregaId).Distinct().Count();
            lblCardEstadoValue.Text = entregasDistintas.ToString();
            lblCardEstadoValue.ForeColor = System.Drawing.Color.FromArgb(92, 122, 42);

            bool filtrosActivos = _historialFiltrado?.Count != _historial?.Count;
            lblSubtitle.Text = filtrosActivos
                ? $"Registro cronológico de transiciones · {fuente.Count} resultado(s) filtrado(s)"
                : "Registro cronológico de transiciones · Todas las entregas";
        }

        // ── Lugar en almacén ──────────────────────────────────────────
        private string ObtenerLugar(int entregaId)
        {
            if (_entregasDict == null || !_entregasDict.TryGetValue(entregaId, out var entrega))
                return "—";

            var partes = new List<string>(3);
            if (!string.IsNullOrWhiteSpace(entrega.Pasillo)) partes.Add(entrega.Pasillo);
            if (!string.IsNullOrWhiteSpace(entrega.NumeroAnaquel)) partes.Add(entrega.NumeroAnaquel);
            if (!string.IsNullOrWhiteSpace(entrega.Piso)) partes.Add(entrega.Piso);

            return partes.Count > 0 ? string.Join(" · ", partes) : "—";
        }

        // ── Helpers ──────────────────────────────────────────────────
        private void PoblarComboEstados(bool mantenerSeleccion)
        {
            string seleccionado = mantenerSeleccion
                ? cmbFiltroEstado.SelectedItem?.ToString() : null;

            var estados = _historial
                .Select(h => h.EstadoEntrega?.Nombre ?? "Desconocido")
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            cmbFiltroEstado.Items.Clear();
            cmbFiltroEstado.Items.Add(" Todos ");
            foreach (var est in estados)
                cmbFiltroEstado.Items.Add(est);

            int idx = seleccionado != null
                ? cmbFiltroEstado.Items.IndexOf(seleccionado) : -1;
            cmbFiltroEstado.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void PoblarComboEntregas(bool mantenerSeleccion)
        {
            string seleccionado = mantenerSeleccion
                ? cmbBuscarEntrega.SelectedItem?.ToString() : null;

            var ids = _historial
                .Select(h => h.EntregaId)
                .Distinct()
                .OrderBy(id => id)
                .ToList();

            cmbBuscarEntrega.Items.Clear();
            cmbBuscarEntrega.Items.Add(" Todas ");
            foreach (int id in ids)
                cmbBuscarEntrega.Items.Add($"E-{id:D4}");

            int idx = seleccionado != null
                ? cmbBuscarEntrega.Items.IndexOf(seleccionado) : -1;
            cmbBuscarEntrega.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void InicializarFechas()
        {
            dtpDesde.Value = _historial?.Count > 0
                ? _historial.Min(h => h.FechaCambio).Date
                : DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
        }

        private void SetControlesActivos(bool activo)
        {
            btnRefrescar.Enabled = activo;
            btnExportar.Enabled = activo;
            btnFiltrar.Enabled = activo;
            btnLimpiarFiltros.Enabled = activo;
            cmbBuscarEntrega.Enabled = activo;
            cmbFiltroEstado.Enabled = activo;
            dtpDesde.Enabled = activo;
            dtpHasta.Enabled = activo;
        }

        // ── CellFormatting ───────────────────────────────────────────
        private void DgvHistorial_CellFormatting(object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != dgvHistorial.Columns["colEstado"].Index) return;
            if (e.Value == null) return;

            e.CellStyle.ForeColor = ObtenerColor(e.Value.ToString());
            e.CellStyle.Font = _fntEstado;
            e.FormattingApplied = true;
        }

        private static System.Drawing.Color ObtenerColor(string estado)
        {
            string lower = estado.ToLowerInvariant();
            foreach (var par in _coloresEstado)
                if (lower.Contains(par.Clave))
                    return par.Color;
            return System.Drawing.Color.FromArgb(80, 55, 30);
        }

        // ── Tooltips ─────────────────────────────────────────────────
        private void ConfigurarTooltips()
        {
            var tt = new ToolTip { InitialDelay = 400, AutoPopDelay = 4000 };
            tt.SetToolTip(btnCerrar, "Cerrar esta ventana");
            tt.SetToolTip(btnRefrescar, "Recargar todos los datos desde la base de datos");
            tt.SetToolTip(btnExportar, "Exportar el historial filtrado a Excel o PDF");
            tt.SetToolTip(btnFiltrar, "Aplicar los filtros seleccionados");
            tt.SetToolTip(btnLimpiarFiltros, "Restablecer todos los filtros");
            tt.SetToolTip(cmbBuscarEntrega, "Seleccionar una entrega para filtrar el historial");
            tt.SetToolTip(cmbFiltroEstado, "Filtrar por un estado específico");
            tt.SetToolTip(dtpDesde, "Fecha de inicio del rango a mostrar");
            tt.SetToolTip(dtpHasta, "Fecha de fin del rango a mostrar");
            tt.SetToolTip(dgvHistorial, "Historial de cambios de estado de todas las entregas");
        }
    }
}