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
using System.Threading.Tasks;
using System.Windows.Forms;

using DrawingColor = System.Drawing.Color;
using DrawingPoint = System.Drawing.Point;

namespace AgroMulti.Ui
{
    public partial class DashboardForm : Form
    {
        // ── Servicios ────────────────────────────────────────────────
        private readonly EntregaService _entregaService;

        // ── Datos en caché para exportación ──────────────────────────
        private List<Entrega> _entregasCache = new List<Entrega>();

        // ── Paleta del sistema ───────────────────────────────────────
        private static readonly DrawingColor[] _palette =
        {
            DrawingColor.FromArgb(92,  122, 42),
            DrawingColor.FromArgb(140, 100, 50),
            DrawingColor.FromArgb(58,   38, 18),
            DrawingColor.FromArgb(170,  140, 70),
            DrawingColor.FromArgb(200,  160, 80),
            DrawingColor.FromArgb(72,   98,  30),
            DrawingColor.FromArgb(190,  150, 90),
            DrawingColor.FromArgb(110,   75, 25),
        };

        private static readonly string[] _mesesCortos =
        {
            "Ene","Feb","Mar","Abr","May","Jun",
            "Jul","Ago","Sep","Oct","Nov","Dic"
        };

        // ── Constructor ──────────────────────────────────────────────
        public DashboardForm()
        {
            InitializeComponent();

            _entregaService = Program.ServiceProvider.GetRequiredService<EntregaService>();
            QuestPDF.Settings.License = LicenseType.Community;

            WindowState = FormWindowState.Maximized;

            int anioActual = DateTime.Now.Year;
            for (int a = anioActual; a >= anioActual - 4; a--)
                cmbAnio.Items.Add(a.ToString());
            cmbAnio.SelectedIndex = 0;

            Load += async (s, e) => await CargarDatosAsync();
        }

        // ── Carga y construcción de todos los gráficos ───────────────
        private async Task CargarDatosAsync()
        {
            SetControlesActivos(false);
            lblEstado.Text = "Cargando datos...";

            try
            {
                var entregas = await _entregaService.GetListConRelaciones(_ => true);
                _entregasCache = entregas.ToList();

                int anio = int.TryParse(cmbAnio.SelectedItem?.ToString(), out int a)
                    ? a : DateTime.Now.Year;

                var entregasAnio = _entregasCache
                    .Where(e => e.FechaEntrega.Year == anio)
                    .ToList();

                BuildChartKilosMes(entregasAnio, anio);
                BuildChartEstados(_entregasCache);
                BuildChartTopProductores(_entregasCache);
                BuildChartPorProducto(_entregasCache);
                BuildChartDiaSemana(_entregasCache);
                BuildChartKilosSecos(entregasAnio, anio);

                lblEstado.Text =
                    $"Actualizado: {DateTime.Now:dd/MM/yyyy  HH:mm}  ·  " +
                    $"{_entregasCache.Count:N0} registros totales  ·  " +
                    $"{entregasAnio.Count:N0} en {anio}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblEstado.Text = "Error al cargar los datos.";
            }
            finally
            {
                SetControlesActivos(true);
            }
        }

        // ── Gráfico 1 · Kilos recibidos por mes ──────────────────────
        private void BuildChartKilosMes(List<Entrega> entregas, int anio)
        {
            double[] pos = Enumerable.Range(0, 12).Select(i => (double)i).ToArray();
            double[] kilos = new double[12];

            foreach (var e in entregas)
                kilos[e.FechaEntrega.Month - 1] += (double)e.Kilos;

            fpKilosMes.Plot.Clear();

            if (kilos.Sum() > 0)
            {
                var bar = fpKilosMes.Plot.AddBar(kilos, pos);
                bar.FillColor = DrawingColor.FromArgb(92, 122, 42);
                bar.BorderColor = DrawingColor.FromArgb(72, 98, 30);
                bar.BorderLineWidth = 1.5f;
                fpKilosMes.Plot.XTicks(pos, _mesesCortos);
                fpKilosMes.Plot.YAxis.Label("Kilos");
                double max = kilos.Max();
                if (max > 0) fpKilosMes.Plot.SetAxisLimitsY(0, max * 1.20);
            }

            fpKilosMes.Plot.Title($"Kilos recibidos por mes ({anio})");
            AplicarEstilo(fpKilosMes);
            fpKilosMes.Refresh();
        }

        // ── Gráfico 2 · Distribución de estados ──────────────────────
        private void BuildChartEstados(List<Entrega> entregas)
        {
            fpEstados.Plot.Clear();

            var grupos = entregas
                .GroupBy(e => e.EstadoEntrega?.Nombre ?? "Sin estado")
                .OrderByDescending(g => g.Count())
                .ToList();

            if (grupos.Count > 0)
            {
                double[] vals = grupos.Select(g => (double)g.Count()).ToArray();
                string[] etiq = grupos.Select(g => g.Key).ToArray();
                var colors = GetPaletteColors(vals.Length);
                double total = vals.Sum();

                var pie = fpEstados.Plot.AddPie(vals);
                pie.SliceLabels = etiq;
                pie.ShowLabels = false;
                pie.ShowPercentages = false;
                pie.ShowValues = false;
                pie.SliceFillColors = colors;
                pie.Explode = false;
                pie.OutlineSize = 2f;
                pie.OutlineColor = DrawingColor.White;
                pie.DonutSize = 0.5;

                for (int i = 0; i < etiq.Length; i++)
                {
                    double pct = total > 0 ? vals[i] / total * 100.0 : 0;

                    var sp = fpEstados.Plot.AddScatter(new double[] { 0 }, new double[] { 0 });
                    sp.Color = colors[i];
                    sp.MarkerSize = 0;
                    sp.LineWidth = 0;
                    sp.Label = $"{etiq[i]}   {pct:N1} %  ({(int)vals[i]} entregas)";
                }

                AplicarLeyenda(fpEstados, ScottPlot.Alignment.LowerRight);
                fpEstados.Plot.Legend().FontSize = 15.5f;
            }

            //fpEstados.Plot.Title("Distribución de estados");
            AplicarEstilo(fpEstados);
            fpEstados.Refresh();
        }

        // ── Gráfico 3 · Top 5 productores por kilos ──────────────────
        private void BuildChartTopProductores(List<Entrega> entregas)
        {
            fpProductores.Plot.Clear();

            var top = entregas
                .GroupBy(e =>
                    $"{e.Productor?.Nombre ?? ""} {e.Productor?.Apellido ?? ""}".Trim())
                .Select(g => new { N = g.Key, K = g.Sum(e => (double)e.Kilos) })
                .OrderByDescending(x => x.K)
                .Take(5)
                .ToList();

            if (top.Count > 0)
            {
                double[] vals = top.Select(x => x.K).ToArray();
                double[] pos = Enumerable.Range(0, top.Count).Select(i => (double)i).ToArray();

                string[] nom = top.Select(x =>
                    x.N.Length > 14 ? x.N[..14] + "\n" + x.N[14..] : x.N).ToArray();

                var bar = fpProductores.Plot.AddBar(vals, pos);
                bar.FillColor = DrawingColor.FromArgb(140, 100, 50);
                bar.BorderColor = DrawingColor.FromArgb(110, 75, 30);
                bar.BorderLineWidth = 1.5f;

                fpProductores.Plot.XTicks(pos, nom);
                fpProductores.Plot.YAxis.Label("Kilos");
                double max = vals.Max();
                if (max > 0) fpProductores.Plot.SetAxisLimitsY(0, max * 1.20);
            }

           //fpProductores.Plot.Title("Top 5 productores por kilos");
            AplicarEstilo(fpProductores);
            fpProductores.Plot.XAxis.TickLabelStyle(
                fontName: "Segoe UI",
                fontSize: 15.625f,
                color: DrawingColor.FromArgb(38, 22, 10));
            fpProductores.Plot.XAxis.TickLabelStyle(rotation: 0);
            fpProductores.Refresh();
        }

        // ── Gráfico 4 · Volumen por producto ─────────────────────────
        private void BuildChartPorProducto(List<Entrega> entregas)
        {
            fpProductos.Plot.Clear();

            var grupos = entregas
                .GroupBy(e => e.Producto?.Nombre ?? "Sin producto")
                .Select(g => new { N = g.Key, K = g.Sum(e => (double)e.Kilos) })
                .OrderByDescending(x => x.K)
                .ToList();

            if (grupos.Count > 0)
            {
                double[] vals = grupos.Select(g => g.K).ToArray();
                string[] etiq = grupos.Select(g => g.N).ToArray();
                var colors = GetPaletteColors(vals.Length);
                double total = vals.Sum();

                var pie = fpProductos.Plot.AddPie(vals);
                pie.SliceLabels = etiq;
                pie.ShowLabels = false;
                pie.ShowPercentages = false;
                pie.ShowValues = false;
                pie.SliceFillColors = colors;
                pie.Explode = false;
                pie.OutlineSize = 2f;
                pie.OutlineColor = DrawingColor.White;
                pie.DonutSize = 0.5;

                for (int i = 0; i < etiq.Length; i++)
                {
                    double pct = total > 0 ? vals[i] / total * 100.0 : 0;

                    var sp = fpProductos.Plot.AddScatter(new double[] { 0 }, new double[] { 0 });
                    sp.Color = colors[i];
                    sp.MarkerSize = 0;
                    sp.LineWidth = 0;
                    sp.Label = $"{etiq[i]}   {pct:N1} %  ({vals[i]:N0} kg)";
                }

                AplicarLeyenda(fpProductos, ScottPlot.Alignment.LowerRight);
                fpProductos.Plot.Legend().FontSize = 15.5f;   // ← ajusta 9–12 según cuántos productos haya
            }

            //fpProductos.Plot.Title("Volumen por producto");
            AplicarEstilo(fpProductos);
            fpProductos.Refresh();
        }

        // ── Gráfico 5 · Actividad por día de la semana ───────────────
        private void BuildChartDiaSemana(List<Entrega> entregas)
        {
            string[] dias = { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };
            double[] cont = new double[7];
            double[] pos = Enumerable.Range(0, 7).Select(i => (double)i).ToArray();

            foreach (var e in entregas)
            {
                int d = e.FechaEntrega.DayOfWeek == DayOfWeek.Sunday
                    ? 6 : (int)e.FechaEntrega.DayOfWeek - 1;
                cont[d]++;
            }

            fpDiaSemana.Plot.Clear();

            if (cont.Sum() > 0)
            {
                var bar = fpDiaSemana.Plot.AddBar(cont, pos);
                bar.FillColor = DrawingColor.FromArgb(72, 52, 28);
                bar.BorderColor = DrawingColor.FromArgb(45, 30, 12);
                bar.BorderLineWidth = 1.5f;

                fpDiaSemana.Plot.XTicks(pos, dias);
                fpDiaSemana.Plot.YAxis.Label("Cantidad de entregas");
                double max = cont.Max();
                if (max > 0) fpDiaSemana.Plot.SetAxisLimitsY(0, max * 1.20);
            }

            //fpDiaSemana.Plot.Title("Actividad por día de la semana");
            AplicarEstilo(fpDiaSemana);
            fpDiaSemana.Refresh();
        }

        // ── Gráfico 6 · Kilos frescos vs secos ───────────────────────
        private void BuildChartKilosSecos(List<Entrega> entregas, int anio)
        {
            double[] xs = Enumerable.Range(1, 12).Select(i => (double)i).ToArray();
            double[] frescos = new double[12];
            double[] secos = new double[12];

            foreach (var e in entregas)
            {
                int idx = e.FechaEntrega.Month - 1;
                frescos[idx] += (double)e.Kilos;
                if (e.KilosSecos.HasValue)
                    secos[idx] += (double)e.KilosSecos.Value;
            }

            fpKilosSecos.Plot.Clear();

            if (entregas.Count > 0)
            {
                var lf = fpKilosSecos.Plot.AddScatter(xs, frescos);
                lf.Color = DrawingColor.FromArgb(92, 122, 42);
                lf.Label = "Frescos";
                lf.LineWidth = 2.5f;
                lf.MarkerSize = 8;

                var ls = fpKilosSecos.Plot.AddScatter(xs, secos);
                ls.Color = DrawingColor.FromArgb(160, 90, 20);
                ls.Label = "Secos";
                ls.LineWidth = 2.5f;
                ls.MarkerSize = 8;
                ls.LineStyle = ScottPlot.LineStyle.Dash;

                fpKilosSecos.Plot.XTicks(xs, _mesesCortos);
                fpKilosSecos.Plot.YAxis.Label("Kilos");
                AplicarLeyenda(fpKilosSecos, ScottPlot.Alignment.UpperRight);
            }

            //fpKilosSecos.Plot.Title($"Kilos frescos vs secos ({anio})");
            AplicarEstilo(fpKilosSecos);
            fpKilosSecos.Refresh();
        }

        // ── Exportar: menú contextual ─────────────────────────────────
        private void BtnExportar_Click(object sender, EventArgs e) =>
            ctxExportar.Show(btnExportar, new DrawingPoint(0, btnExportar.Height));

        // ── Exportar a Excel ──────────────────────────────────────────
        private void ItemExportarExcel_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Title = "Exportar dashboard a Excel",
                Filter = "Libro de Excel (*.xlsx)|*.xlsx",
                FileName = $"Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                DefaultExt = "xlsx"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                // ── Cálculos previos ──────────────────────────────────────────
                int anio = int.TryParse(cmbAnio.SelectedItem?.ToString(), out int a) ? a : DateTime.Now.Year;
                var entregasAnio = _entregasCache.Where(e => e.FechaEntrega.Year == anio).ToList();

                double totalKilosAnio = entregasAnio.Sum(e => (double)e.Kilos);
                double totalKilosSecosAnio = entregasAnio.Sum(e => (double)(e.KilosSecos ?? 0m));
                double promedioKilosPorEntrega = entregasAnio.Count > 0
                    ? totalKilosAnio / entregasAnio.Count : 0;
                double totalKilosTodos = _entregasCache.Sum(x => (double)x.Kilos);
                double rendimiento = totalKilosAnio > 0 ? totalKilosSecosAnio / totalKilosAnio * 100 : 0;
                double merma = totalKilosAnio - totalKilosSecosAnio;

                string mesMasActivo = "—";
                string mesMasActivoKg = "—";
                if (entregasAnio.Count > 0)
                {
                    var mej = entregasAnio
                        .GroupBy(e => e.FechaEntrega.Month)
                        .Select(g => new { Mes = g.Key, Kilos = g.Sum(x => (double)x.Kilos) })
                        .OrderByDescending(x => x.Kilos).FirstOrDefault();
                    if (mej != null)
                    {
                        mesMasActivo = _mesesCortos[mej.Mes - 1];
                        mesMasActivoKg = $"{mej.Kilos:N0} kg";
                    }
                }

                string estadoDominante = _entregasCache.Count > 0
                    ? _entregasCache.GroupBy(e => e.EstadoEntrega?.Nombre ?? "Sin estado")
                        .OrderByDescending(g => g.Count()).First().Key
                    : "—";

                string productorLider = _entregasCache.Count > 0
                    ? _entregasCache
                        .GroupBy(e => $"{e.Productor?.Nombre ?? ""} {e.Productor?.Apellido ?? ""}".Trim())
                        .Select(g => new
                        {
                            Nombre = string.IsNullOrWhiteSpace(g.Key) ? "Sin productor" : g.Key,
                            Kilos = g.Sum(x => (double)x.Kilos)
                        })
                        .OrderByDescending(x => x.Kilos).First().Nombre
                    : "—";

                // ── Datos para los paneles ────────────────────────────────────
                var estadosPanel = _entregasCache
                    .GroupBy(x => x.EstadoEntrega?.Nombre ?? "Sin estado")
                    .Select(g => new { Nombre = g.Key, Cantidad = g.Count(), Kilos = g.Sum(x => (double)x.Kilos) })
                    .OrderByDescending(x => x.Kilos).Take(4).ToList();

                var productoresPanel = _entregasCache
                    .GroupBy(x => $"{x.Productor?.Nombre ?? ""} {x.Productor?.Apellido ?? ""}".Trim())
                    .Select(g => new
                    {
                        Nombre = string.IsNullOrWhiteSpace(g.Key) ? "Sin productor" : g.Key,
                        Cantidad = g.Count(),
                        Kilos = g.Sum(x => (double)x.Kilos)
                    })
                    .OrderByDescending(x => x.Kilos).Take(4).ToList();

                var productosPanel = _entregasCache
                    .GroupBy(x => x.Producto?.Nombre ?? "Sin producto")
                    .Select(g => new { Nombre = g.Key, Cantidad = g.Count(), Kilos = g.Sum(x => (double)x.Kilos) })
                    .OrderByDescending(x => x.Kilos).Take(4).ToList();

                string[] diasCortos = { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
                var diasPanel = entregasAnio
                    .GroupBy(x => (int)x.FechaEntrega.DayOfWeek)
                    .Select(g => new
                    {
                        DiaSemana = g.Key,
                        Dia = diasCortos[g.Key],
                        Cantidad = g.Count(),
                        Kilos = g.Sum(x => (double)x.Kilos)
                    })
                    .OrderBy(x => x.DiaSemana).ToList();

                var diaMaxCantidad = diasPanel.Count > 0
                    ? diasPanel.OrderByDescending(d => d.Cantidad).First() : null;
                var diaMaxKilos = diasPanel.Count > 0
                    ? diasPanel.OrderByDescending(d => d.Kilos).First() : null;
                double promDia = diasPanel.Count > 0
                    ? (double)entregasAnio.Count / diasPanel.Count : 0;

                // ── Paleta de colores ─────────────────────────────────────────
                var cHeader = XLColor.FromArgb(38, 22, 10);
                var cSubtit = XLColor.FromArgb(201, 181, 157);
                var cMeta = XLColor.FromArgb(184, 158, 130);
                var cBg = XLColor.FromArgb(244, 239, 231);
                var cCardBg = XLColor.FromArgb(252, 249, 244);
                var cBorder = XLColor.FromArgb(216, 200, 184);
                var cLabel = XLColor.FromArgb(107, 76, 50);
                var cMuted = XLColor.FromArgb(138, 115, 95);
                var cLectura = XLColor.FromArgb(239, 231, 219);

                // ── Constantes escaladas al 85.8 % (66 % × 1.30) ────────────
                const int COLS = 8;
                const int COL_WIDTH = 14;   // 11 × 1.30
                const int IMG_W = 824;  // 634 × 1.30
                const int IMG_H = 429;  // 330 × 1.30
                const int IMG_ROW = 3;
                const int DATA_ROW = 25;

                // ── Helpers locales ───────────────────────────────────────────
                void StyleHeader(IXLRange r, string text, double fontSize,
                                 XLColor fg, XLColor bg, bool bold = false)
                {
                    r.Merge();
                    r.Value = text;
                    r.Style.Font.Bold = bold;
                    r.Style.Font.FontSize = fontSize;
                    r.Style.Font.FontColor = fg;
                    r.Style.Fill.BackgroundColor = bg;
                    r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    r.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }

                void WritePanelTitle(IXLWorksheet ws, int row, string text)
                {
                    ws.Row(row).Height = 17;   // 13 × 1.30
                    var r = ws.Range(row, 1, row, COLS);
                    r.Merge();
                    r.Value = text;
                    r.Style.Font.Bold = true;
                    r.Style.Font.FontSize = 9;   // 7 × 1.30
                    r.Style.Font.FontColor = cHeader;
                    r.Style.Fill.BackgroundColor = cCardBg;
                    r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    r.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    r.Style.Alignment.Indent = 1;
                    r.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    r.Style.Border.OutsideBorderColor = cBorder;
                }

                void WriteKpiCards(IXLWorksheet ws, int startRow,
                                   (string Label, string Value, string Sub)[] cards)
                {
                    int n = cards.Length;
                    int colsEach = COLS / n;

                    ws.Row(startRow).Height = 13;  // 10 × 1.30
                    ws.Row(startRow + 1).Height = 22;  // 17 × 1.30
                    ws.Row(startRow + 2).Height = 13;  // 10 × 1.30

                    for (int i = 0; i < n; i++)
                    {
                        int cs = i * colsEach + 1;
                        int ce = i == n - 1 ? COLS : cs + colsEach - 1;

                        // Etiqueta
                        var lbl = ws.Range(startRow, cs, startRow, ce);
                        lbl.Merge();
                        lbl.Value = cards[i].Label;
                        lbl.Style.Font.FontSize = 7;   // 5 × 1.30
                        lbl.Style.Font.FontColor = cLabel;
                        lbl.Style.Fill.BackgroundColor = XLColor.White;
                        lbl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        lbl.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        lbl.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        lbl.Style.Border.TopBorderColor = cBorder;
                        lbl.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        lbl.Style.Border.LeftBorderColor = cBorder;
                        lbl.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        lbl.Style.Border.RightBorderColor = cBorder;

                        // Valor
                        var val = ws.Range(startRow + 1, cs, startRow + 1, ce);
                        val.Merge();
                        val.Value = cards[i].Value;
                        val.Style.Font.Bold = true;
                        val.Style.Font.FontSize = 13;  // 10 × 1.30
                        val.Style.Font.FontColor = cHeader;
                        val.Style.Fill.BackgroundColor = XLColor.White;
                        val.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        val.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        val.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        val.Style.Border.LeftBorderColor = cBorder;
                        val.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        val.Style.Border.RightBorderColor = cBorder;

                        // Subtítulo
                        var sub = ws.Range(startRow + 2, cs, startRow + 2, ce);
                        sub.Merge();
                        sub.Value = cards[i].Sub;
                        sub.Style.Font.FontSize = 7;   // 5 × 1.30
                        sub.Style.Font.FontColor = cMuted;
                        sub.Style.Fill.BackgroundColor = XLColor.White;
                        sub.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sub.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        sub.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        sub.Style.Border.BottomBorderColor = cBorder;
                        sub.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        sub.Style.Border.LeftBorderColor = cBorder;
                        sub.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        sub.Style.Border.RightBorderColor = cBorder;
                    }
                }

                using var wb = new XLWorkbook();

                
                {
                    var ws = wb.Worksheets.Add("Resumen ejecutivo");
                    for (int c = 1; c <= COLS; c++) ws.Column(c).Width = COL_WIDTH;

                    ws.Row(1).Height = 23;   // 18 × 1.30
                    StyleHeader(ws.Range(1, 1, 1, COLS),
                        "DASHBOARD DE ANÁLISIS", 14, XLColor.White, cHeader, true);  // 11 × 1.30

                    ws.Row(2).Height = 16;   // 12 × 1.30
                    StyleHeader(ws.Range(2, 1, 2, COLS),
                        "Centro de Fermentación y Secado", 9, cSubtit, cHeader);     // 7 × 1.30

                    ws.Row(3).Height = 13;   // 10 × 1.30
                    StyleHeader(ws.Range(3, 1, 3, COLS),
                        $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  " +
                        $"Año analizado: {anio}  ·  Registros totales: {_entregasCache.Count:N0}",
                        8, cMeta, cHeader);                                           // 6 × 1.30

                    ws.Row(4).Height = 9;    // 7 × 1.30
                    ws.Range(4, 1, 4, COLS).Style.Fill.BackgroundColor = cBg;

                    WritePanelTitle(ws, 5, "Resumen ejecutivo");

                    WriteKpiCards(ws, 6, new (string, string, string)[]
                    {
                ("Entregas totales", _entregasCache.Count.ToString("N0"), "Registros cargados"),
                ("Kilos del año",    $"{totalKilosAnio:N0} kg",           $"Año {anio}"),
                ("Kilos secos",      $"{totalKilosSecosAnio:N0} kg",      "Acumulado anual"),
                ("Prom. / entrega",  $"{promedioKilosPorEntrega:N1} kg",  "Media del período"),
                    });

                    ws.Row(9).Height = 9;    // 7 × 1.30
                    ws.Range(9, 1, 9, COLS).Style.Fill.BackgroundColor = cBg;

                    WritePanelTitle(ws, 10, "Lectura rápida");

                    string[] lecturas =
                    {
                $"  El mes más activo fue {mesMasActivo} con {mesMasActivoKg}.",
                $"  Estado predominante: {estadoDominante}.",
                $"  Productor líder: {productorLider}.",
            };

                    for (int i = 0; i < lecturas.Length; i++)
                    {
                        int row = 11 + i;
                        ws.Row(row).Height = 14;   // 11 × 1.30
                        var lr = ws.Range(row, 1, row, COLS);
                        lr.Merge();
                        lr.Value = lecturas[i];
                        lr.Style.Font.FontSize = 8;   // 6 × 1.30
                        lr.Style.Font.FontColor = cLabel;
                        lr.Style.Fill.BackgroundColor = cLectura;
                        lr.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        lr.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        lr.Style.Border.LeftBorderColor = cBorder;
                        lr.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        lr.Style.Border.RightBorderColor = cBorder;
                        if (i == lecturas.Length - 1)
                        {
                            lr.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            lr.Style.Border.BottomBorderColor = cBorder;
                        }
                    }
                }

                
                var chartSheets = new (ScottPlot.FormsPlot Fp, string Nombre,
                                       string Titulo, string Subtitulo,
                                       string PanelTitulo,
                                       (string Label, string Value, string Sub)[] Cards)[]
                {
            (fpKilosMes, "Kilos por mes",
             "Kilos recibidos por mes",
             "Tendencia mensual de ingreso de materia prima en el año seleccionado.",
             $"Kilos recibidos por mes — año {anio}",
             new (string, string, string)[]
             {
                 ("Mes más activo",     mesMasActivo,                       mesMasActivoKg),
                 ("Kilos del año",      $"{totalKilosAnio:N0} kg",          $"Acumulado {anio}"),
                 ("Prom. / entrega",    $"{promedioKilosPorEntrega:N1} kg", "Media del período"),
                 ("Entregas en el año", entregasAnio.Count.ToString("N0"), $"Registros {anio}"),
             }),

            (fpEstados, "Estados",
             "Distribución de estados",
             "Participación relativa de los estados de entrega en el conjunto total.",
             "Distribución detallada por estado",
             estadosPanel.Select(x =>
             {
                 double pct = totalKilosTodos > 0 ? x.Kilos / totalKilosTodos * 100 : 0;
                 return (x.Nombre, $"{x.Kilos:N0} kg", $"{x.Cantidad} entregas · {pct:N1}%");
             }).ToArray()),

            (fpProductores, "Top productores",
             "Top 5 productores por kilos",
             "Productores con mayor volumen recibido en el período analizado.",
             "Ranking de productores — top 4 por volumen",
             productoresPanel.Select((x, i) =>
             {
                 string nom = x.Nombre.Length > 16 ? x.Nombre[..14] + "…" : x.Nombre;
                 return ($"#{i + 1}  {nom}", $"{x.Kilos:N0} kg", $"{x.Cantidad} entregas");
             }).ToArray()),

            (fpProductos, "Por producto",
             "Volumen por producto",
             "Comparación del peso acumulado por tipo de producto.",
             "Desglose por tipo de producto",
             productosPanel.Select(x =>
             {
                 double pct = totalKilosTodos > 0 ? x.Kilos / totalKilosTodos * 100 : 0;
                 return (x.Nombre, $"{x.Kilos:N0} kg", $"{x.Cantidad} entregas · {pct:N1}%");
             }).ToArray()),

            (fpDiaSemana, "Días semana",
             "Actividad por día de la semana",
             "Frecuencia de entregas según el día en que fueron registradas.",
             $"Actividad semanal — año {anio}",
             new (string, string, string)[]
             {
                 ("Día más activo",
                  diaMaxCantidad?.Dia ?? "—",
                  diaMaxCantidad != null ? $"{diaMaxCantidad.Cantidad} entregas" : "—"),

                 ("Día mayor volumen",
                  diaMaxKilos?.Dia ?? "—",
                  diaMaxKilos != null ? $"{diaMaxKilos.Kilos:N0} kg" : "—"),

                 ("Total entregas año",
                  entregasAnio.Count.ToString("N0"),
                  $"Prom. {promDia:N1} / día activo"),

                 ("Días con actividad",
                  diasPanel.Count.ToString(),
                  "Días distintos registrados"),
             }),

            (fpKilosSecos, "Kilos frescos-secos",
             "Kilos frescos vs secos",
             "Evolución comparativa de kilos frescos y kilos secos en el año.",
             "Resumen comparativo: frescos vs secos",
             new (string, string, string)[]
             {
                 ("Kilos frescos",  $"{totalKilosAnio:N0} kg",      $"Acumulado {anio}"),
                 ("Kilos secos",    $"{totalKilosSecosAnio:N0} kg", $"Acumulado {anio}"),
                 ("Rendimiento",    $"{rendimiento:N1} %",           "Secos / frescos × 100"),
                 ("Merma estimada", $"{merma:N0} kg",                "Frescos − secos"),
             }),
                };

                foreach (var sheet in chartSheets)
                {
                    var ws = wb.Worksheets.Add(sheet.Nombre);
                    for (int c = 1; c <= COLS; c++) ws.Column(c).Width = COL_WIDTH;

                    // ── Filas 1-2: header ──────────────────────────────────────
                    ws.Row(1).Height = 22;   // 17 × 1.30
                    StyleHeader(ws.Range(1, 1, 1, COLS),
                        sheet.Titulo, 13, XLColor.White, cHeader, true);  // 10 × 1.30

                    ws.Row(2).Height = 13;   // 10 × 1.30
                    StyleHeader(ws.Range(2, 1, 2, COLS),
                        sheet.Subtitulo, 7, cSubtit, cHeader);             // 5 × 1.30

                    // ── Fila IMG_ROW: imagen escalada al 85.8 % (824 × 429) ──
                    byte[] imgBytes = GetChartBytes(sheet.Fp, IMG_W, IMG_H);
                    using var ms = new MemoryStream(imgBytes);
                    var pic = ws.AddPicture(ms).MoveTo(ws.Cell(IMG_ROW, 1));
                    pic.Width = IMG_W;  // 824
                    pic.Height = IMG_H;  // 429

                    // ── Fila DATA_ROW: panel de datos contextual ───────────────
                    WritePanelTitle(ws, DATA_ROW, sheet.PanelTitulo);
                    WriteKpiCards(ws, DATA_ROW + 1, sheet.Cards);
                }

                wb.SaveAs(sfd.FileName);
                MessageBox.Show($"Dashboard exportado a Excel:\n{sfd.FileName}",
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
            if (_entregasCache == null || _entregasCache.Count == 0)
            {
                MessageBox.Show("No hay datos cargados para exportar el dashboard.",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar dashboard a PDF",
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = $"Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                DefaultExt = "pdf"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                int anio = int.TryParse(cmbAnio.SelectedItem?.ToString(), out int a)
                    ? a : DateTime.Now.Year;

                var entregasAnio = _entregasCache
                    .Where(e => e.FechaEntrega.Year == anio)
                    .ToList();

                double totalKilosAnio = entregasAnio.Sum(e => (double)e.Kilos);
                double totalKilosSecosAnio = entregasAnio.Sum(e => (double)(e.KilosSecos ?? 0m));
                double promedioKilosPorEntrega = entregasAnio.Count > 0
                    ? totalKilosAnio / entregasAnio.Count : 0;

                string mesMasActivo = "—";
                if (entregasAnio.Count > 0)
                {
                    var mejorMes = entregasAnio
                        .GroupBy(e => e.FechaEntrega.Month)
                        .Select(g => new { Mes = g.Key, Kilos = g.Sum(x => (double)x.Kilos) })
                        .OrderByDescending(x => x.Kilos)
                        .FirstOrDefault();
                    if (mejorMes != null)
                        mesMasActivo = $"{_mesesCortos[mejorMes.Mes - 1]} ({mejorMes.Kilos:N0} kg)";
                }

                string estadoDominante = _entregasCache.Count > 0
                    ? _entregasCache
                        .GroupBy(e => e.EstadoEntrega?.Nombre ?? "Sin estado")
                        .OrderByDescending(g => g.Count())
                        .First().Key
                    : "—";

                string productorLider = _entregasCache.Count > 0
                    ? _entregasCache
                        .GroupBy(e => $"{e.Productor?.Nombre ?? ""} {e.Productor?.Apellido ?? ""}".Trim())
                        .Select(g => new
                        {
                            Nombre = string.IsNullOrWhiteSpace(g.Key) ? "Sin productor" : g.Key,
                            Kilos = g.Sum(x => (double)x.Kilos)
                        })
                        .OrderByDescending(x => x.Kilos)
                        .First().Nombre
                    : "—";

                // ── Datos para los paneles de páginas 1-5 ────────────────────
                double totalKilosTodos = _entregasCache.Sum(x => (double)x.Kilos);

                // Página 1 – Distribución de estados
                var estadosPanel = _entregasCache
                    .GroupBy(x => x.EstadoEntrega?.Nombre ?? "Sin estado")
                    .Select(g => new
                    {
                        Nombre = g.Key,
                        Cantidad = g.Count(),
                        Kilos = g.Sum(x => (double)x.Kilos)
                    })
                    .OrderByDescending(x => x.Kilos)
                    .Take(4)
                    .ToList();

                // Página 2 – Top 5 productores
                var productoresPanel = _entregasCache
                    .GroupBy(x => $"{x.Productor?.Nombre ?? ""} {x.Productor?.Apellido ?? ""}".Trim())
                    .Select(g => new
                    {
                        Nombre = string.IsNullOrWhiteSpace(g.Key) ? "Sin productor" : g.Key,
                        Cantidad = g.Count(),
                        Kilos = g.Sum(x => (double)x.Kilos)
                    })
                    .OrderByDescending(x => x.Kilos)
                    .Take(5)
                    .ToList();

                // Página 3 – Volumen por producto
                var productosPanel = _entregasCache
                    .GroupBy(x => x.Producto?.Nombre ?? "Sin producto")
                    .Select(g => new
                    {
                        Nombre = g.Key,
                        Cantidad = g.Count(),
                        Kilos = g.Sum(x => (double)x.Kilos)
                    })
                    .OrderByDescending(x => x.Kilos)
                    .Take(5)
                    .ToList();

                // Página 4 – Actividad por día de la semana
                string[] diasCortos = { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
                var diasPanel = entregasAnio
                    .GroupBy(x => (int)x.FechaEntrega.DayOfWeek)
                    .Select(g => new
                    {
                        DiaSemana = g.Key,
                        Dia = diasCortos[g.Key],
                        Cantidad = g.Count(),
                        Kilos = g.Sum(x => (double)x.Kilos)
                    })
                    .OrderBy(x => x.DiaSemana)
                    .ToList();

                // Página 5 – Frescos vs secos
                double rendimiento = totalKilosAnio > 0
                    ? totalKilosSecosAnio / totalKilosAnio * 100 : 0;
                double merma = totalKilosAnio - totalKilosSecosAnio;

                // ── Gráficas ──────────────────────────────────────────────────
                var charts = new (string Titulo, string Subtitulo, byte[] Imagen)[]
                {
            ("Kilos recibidos por mes",
             "Tendencia mensual de ingreso de materia prima en el año seleccionado.",
             GetChartBytes(fpKilosMes,    1100, 380)),

            ("Distribución de estados",
             "Participación relativa de los estados de entrega en el conjunto total.",
             GetChartBytes(fpEstados,     1100, 380)),

            ("Top 5 productores por kilos",
             "Productores con mayor volumen recibido en el período analizado.",
             GetChartBytes(fpProductores, 1100, 380)),

            ("Volumen por producto",
             "Comparación del peso acumulado por tipo de producto.",
             GetChartBytes(fpProductos,   1100, 380)),

            ("Actividad por día de la semana",
             "Frecuencia de entregas según el día en que fueron registradas.",
             GetChartBytes(fpDiaSemana,   1100, 380)),

            ("Kilos frescos vs secos",
             "Evolución comparativa de kilos frescos y kilos secos en el año.",
             GetChartBytes(fpKilosSecos,  1100, 380)),
                };

                // Página 0 = resumen ejecutivo + charts[0]
                // Páginas 1-5 = charts[1-5] + panel de datos
                int totalPages = charts.Length; // 6

                Document.Create(container =>
                {
                    for (int p = 0; p < totalPages; p++)
                    {
                        int pageIndex = p;

                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4.Landscape());
                            page.PageColor("#26160A");
                            page.Margin(0.5f, Unit.Centimetre);

                            // ── Encabezado ────────────────────────────────────
                            page.Header()
                                .Background("#26160A")
                                .PaddingVertical(10)
                                .PaddingHorizontal(22)
                                .Column(col =>
                                {
                                    col.Item().Text("DASHBOARD DE ANÁLISIS")
                                        .FontFamily("Segoe UI").FontSize(18).Bold().FontColor(Colors.White);
                                    col.Item().PaddingTop(1)
                                        .Text("Centro de Fermentación y Secado")
                                        .FontSize(10).FontColor("#C9B59D");
                                    col.Item().PaddingTop(2)
                                        .Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  ·  " +
                                              $"Año analizado: {anio}  ·  " +
                                              $"Registros totales: {_entregasCache.Count:N0}")
                                        .FontSize(8).FontColor("#B89E82");
                                });

                            // ── Contenido ─────────────────────────────────────
                            page.Content()
                                .Background("#F4EFE7")
                                .PaddingHorizontal(18)
                                .PaddingVertical(pageIndex == 0 ? 8 : 14)
                                .Column(col =>
                                {
                                    
                                    if (pageIndex == 0)
                                    {
                                        col.Item()
                                            .Background("#FCF9F4").Border(1).BorderColor("#D8C8B8").Padding(8)
                                            .Column(box =>
                                            {
                                                box.Item().Text("Resumen ejecutivo")
                                                    .FontFamily("Segoe UI").FontSize(11).Bold().FontColor("#26160A");

                                                box.Item().PaddingTop(6).Row(row =>
                                                {
                                                    row.RelativeItem().PaddingRight(5)
                                                        .Background(Colors.White).Border(1).BorderColor("#D8C8B8").Padding(7)
                                                        .Column(card =>
                                                        {
                                                            card.Item().Text("Entregas totales").FontSize(8).FontColor("#6B4C32");
                                                            card.Item().Text(_entregasCache.Count.ToString("N0"))
                                                                .FontSize(16).Bold().FontColor("#26160A");
                                                            card.Item().Text("Registros cargados").FontSize(7.5f).FontColor("#8A735F");
                                                        });

                                                    row.RelativeItem().PaddingRight(5)
                                                        .Background(Colors.White).Border(1).BorderColor("#D8C8B8").Padding(7)
                                                        .Column(card =>
                                                        {
                                                            card.Item().Text("Kilos del año").FontSize(8).FontColor("#6B4C32");
                                                            card.Item().Text($"{totalKilosAnio:N0} kg")
                                                                .FontSize(16).Bold().FontColor("#26160A");
                                                            card.Item().Text($"Año {anio}").FontSize(7.5f).FontColor("#8A735F");
                                                        });

                                                    row.RelativeItem().PaddingRight(5)
                                                        .Background(Colors.White).Border(1).BorderColor("#D8C8B8").Padding(7)
                                                        .Column(card =>
                                                        {
                                                            card.Item().Text("Kilos secos").FontSize(8).FontColor("#6B4C32");
                                                            card.Item().Text($"{totalKilosSecosAnio:N0} kg")
                                                                .FontSize(16).Bold().FontColor("#26160A");
                                                            card.Item().Text("Acumulado anual").FontSize(7.5f).FontColor("#8A735F");
                                                        });

                                                    row.RelativeItem()
                                                        .Background(Colors.White).Border(1).BorderColor("#D8C8B8").Padding(7)
                                                        .Column(card =>
                                                        {
                                                            card.Item().Text("Promedio por entrega").FontSize(8).FontColor("#6B4C32");
                                                            card.Item().Text($"{promedioKilosPorEntrega:N1} kg")
                                                                .FontSize(16).Bold().FontColor("#26160A");
                                                            card.Item().Text("Media del período").FontSize(7.5f).FontColor("#8A735F");
                                                        });
                                                });

                                                box.Item().PaddingTop(6)
                                                    .Background("#EFE7DB").Padding(7)
                                                    .Text($"Lectura rápida: el mes más activo fue {mesMasActivo}; " +
                                                          $"el estado predominante fue {estadoDominante}; " +
                                                          $"y el productor líder fue {productorLider}.")
                                                    .FontSize(8.5f).FontColor("#6B4C32");
                                            });

                                        col.Item().PaddingTop(6);

                                        // charts[0] en la misma página
                                        col.Item()
                                            .Background(Colors.White).Border(1).BorderColor("#D8C8B8").Padding(8)
                                            .Column(card =>
                                            {
                                                card.Item().Text(charts[0].Titulo)
                                                    .FontFamily("Segoe UI").FontSize(11).Bold().FontColor("#26160A");
                                                card.Item().Text(charts[0].Subtitulo)
                                                    .FontSize(8).FontColor("#7C6550");
                                                card.Item().PaddingTop(5)
                                                    .AlignCenter()
                                                    .Width(160, Unit.Millimetre)
                                                    .Image(charts[0].Imagen).FitWidth();
                                            });
                                    }
                                    
                                    else
                                    {
                                        var chart = charts[pageIndex];

                                        // ── Tarjeta de la gráfica (ancho completo) ──
                                        col.Item()
                                            .Background(Colors.White).Border(1).BorderColor("#D8C8B8").Padding(12)
                                            .Column(card =>
                                            {
                                                card.Item().Text(chart.Titulo)
                                                    .FontFamily("Segoe UI").FontSize(12).Bold().FontColor("#26160A");
                                                card.Item().Text(chart.Subtitulo)
                                                    .FontSize(8.5f).FontColor("#7C6550");
                                                // Sin restricción de ancho → FitWidth usa todo el espacio disponible
                                                card.Item().PaddingTop(8)
                                                    .Image(chart.Imagen).FitWidth();
                                            });

                                        col.Item().PaddingTop(8);

                                        // ── Panel de datos contextual ────────────
                                        string panelTitle = pageIndex switch
                                        {
                                            1 => "Distribución detallada por estado",
                                            2 => "Ranking de productores — top 5 por volumen",
                                            3 => "Desglose por tipo de producto",
                                            4 => $"Actividad semanal — entregas del año {anio}",
                                            5 => "Resumen comparativo: frescos vs secos",
                                            _ => "Datos del gráfico"
                                        };

                                        col.Item()
                                            .Background("#FCF9F4").Border(1).BorderColor("#D8C8B8").Padding(10)
                                            .Column(panel =>
                                            {
                                                panel.Item().Text(panelTitle)
                                                    .FontSize(9).Bold().FontColor("#26160A");

                                                panel.Item().PaddingTop(6).Row(row =>
                                                {
                                                    // ── Página 1: estados ────────────────
                                                    if (pageIndex == 1)
                                                    {
                                                        for (int i = 0; i < estadosPanel.Count; i++)
                                                        {
                                                            var est = estadosPanel[i];
                                                            double pct = totalKilosTodos > 0
                                                                ? est.Kilos / totalKilosTodos * 100 : 0;
                                                            IContainer slot = i < estadosPanel.Count - 1
                                                                ? row.RelativeItem().PaddingRight(5)
                                                                : row.RelativeItem();
                                                            slot.Background(Colors.White)
                                                                .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                                .Column(c =>
                                                                {
                                                                    c.Item().Text(est.Nombre)
                                                                        .FontSize(8).Bold().FontColor("#26160A");
                                                                    c.Item().Text($"{est.Kilos:N0} kg")
                                                                        .FontSize(14).Bold().FontColor("#26160A");
                                                                    c.Item().Text($"{est.Cantidad} entregas · {pct:N1}%")
                                                                        .FontSize(7.5f).FontColor("#8A735F");
                                                                });
                                                        }
                                                    }
                                                    // ── Página 2: productores ────────────
                                                    else if (pageIndex == 2)
                                                    {
                                                        for (int i = 0; i < productoresPanel.Count; i++)
                                                        {
                                                            var prod = productoresPanel[i];
                                                            string nombreCorto = prod.Nombre.Length > 16
                                                                ? prod.Nombre.Substring(0, 14) + "…"
                                                                : prod.Nombre;
                                                            IContainer slot = i < productoresPanel.Count - 1
                                                                ? row.RelativeItem().PaddingRight(5)
                                                                : row.RelativeItem();
                                                            slot.Background(Colors.White)
                                                                .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                                .Column(c =>
                                                                {
                                                                    c.Item().Text($"#{i + 1}  {nombreCorto}")
                                                                        .FontSize(7.5f).Bold().FontColor("#26160A");
                                                                    c.Item().Text($"{prod.Kilos:N0} kg")
                                                                        .FontSize(14).Bold().FontColor("#26160A");
                                                                    c.Item().Text($"{prod.Cantidad} entregas")
                                                                        .FontSize(7.5f).FontColor("#8A735F");
                                                                });
                                                        }
                                                    }
                                                    // ── Página 3: productos ──────────────
                                                    else if (pageIndex == 3)
                                                    {
                                                        for (int i = 0; i < productosPanel.Count; i++)
                                                        {
                                                            var prd = productosPanel[i];
                                                            double pct = totalKilosTodos > 0
                                                                ? prd.Kilos / totalKilosTodos * 100 : 0;
                                                            IContainer slot = i < productosPanel.Count - 1
                                                                ? row.RelativeItem().PaddingRight(5)
                                                                : row.RelativeItem();
                                                            slot.Background(Colors.White)
                                                                .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                                .Column(c =>
                                                                {
                                                                    c.Item().Text(prd.Nombre)
                                                                        .FontSize(8).Bold().FontColor("#26160A");
                                                                    c.Item().Text($"{prd.Kilos:N0} kg")
                                                                        .FontSize(14).Bold().FontColor("#26160A");
                                                                    c.Item().Text($"{prd.Cantidad} entregas · {pct:N1}%")
                                                                        .FontSize(7.5f).FontColor("#8A735F");
                                                                });
                                                        }
                                                    }
                                                    // ── Página 4: días de la semana ──────
                                                    else if (pageIndex == 4)
                                                    {
                                                        for (int i = 0; i < diasPanel.Count; i++)
                                                        {
                                                            var dia = diasPanel[i];
                                                            IContainer slot = i < diasPanel.Count - 1
                                                                ? row.RelativeItem().PaddingRight(5)
                                                                : row.RelativeItem();
                                                            slot.Background(Colors.White)
                                                                .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                                .Column(c =>
                                                                {
                                                                    c.Item().Text(dia.Dia)
                                                                        .FontSize(9).Bold().FontColor("#26160A");
                                                                    c.Item().Text(dia.Cantidad.ToString())
                                                                        .FontSize(16).Bold().FontColor("#26160A");
                                                                    c.Item().Text($"{dia.Kilos:N0} kg")
                                                                        .FontSize(7.5f).FontColor("#8A735F");
                                                                });
                                                        }
                                                    }
                                                    // ── Página 5: frescos vs secos ───────
                                                    else if (pageIndex == 5)
                                                    {
                                                        row.RelativeItem().PaddingRight(5)
                                                            .Background(Colors.White)
                                                            .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                            .Column(c =>
                                                            {
                                                                c.Item().Text("Kilos frescos").FontSize(8).FontColor("#6B4C32");
                                                                c.Item().Text($"{totalKilosAnio:N0} kg")
                                                                    .FontSize(14).Bold().FontColor("#26160A");
                                                                c.Item().Text($"Acumulado año {anio}").FontSize(7.5f).FontColor("#8A735F");
                                                            });

                                                        row.RelativeItem().PaddingRight(5)
                                                            .Background(Colors.White)
                                                            .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                            .Column(c =>
                                                            {
                                                                c.Item().Text("Kilos secos").FontSize(8).FontColor("#6B4C32");
                                                                c.Item().Text($"{totalKilosSecosAnio:N0} kg")
                                                                    .FontSize(14).Bold().FontColor("#26160A");
                                                                c.Item().Text($"Acumulado año {anio}").FontSize(7.5f).FontColor("#8A735F");
                                                            });

                                                        row.RelativeItem().PaddingRight(5)
                                                            .Background(Colors.White)
                                                            .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                            .Column(c =>
                                                            {
                                                                c.Item().Text("Rendimiento").FontSize(8).FontColor("#6B4C32");
                                                                c.Item().Text($"{rendimiento:N1} %")
                                                                    .FontSize(14).Bold().FontColor("#26160A");
                                                                c.Item().Text("Secos / frescos × 100").FontSize(7.5f).FontColor("#8A735F");
                                                            });

                                                        row.RelativeItem()
                                                            .Background(Colors.White)
                                                            .Border(1).BorderColor("#D8C8B8").Padding(7)
                                                            .Column(c =>
                                                            {
                                                                c.Item().Text("Merma estimada").FontSize(8).FontColor("#6B4C32");
                                                                c.Item().Text($"{merma:N0} kg")
                                                                    .FontSize(14).Bold().FontColor("#26160A");
                                                                c.Item().Text("Frescos − secos").FontSize(7.5f).FontColor("#8A735F");
                                                            });
                                                    }
                                                });
                                            });
                                    }
                                });

                            // ── Pie de página ──────────────────────────────────
                            page.Footer()
                                .Background("#26160A")
                                .PaddingVertical(8)
                                .PaddingHorizontal(20)
                                .AlignRight()
                                .Text($"Página {pageIndex + 1} de {totalPages}  ·  Dashboard AgroMulti")
                                .FontSize(8)
                                .FontColor("#D8C2A5");
                        });
                    }
                }).GeneratePdf(sfd.FileName);

                MessageBox.Show($"Dashboard exportado a PDF:\n{sfd.FileName}",
                    "Exportar a PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Botones ───────────────────────────────────────────────────
        private async void BtnRefrescar_Click(object sender, EventArgs e) =>
            await CargarDatosAsync();

        private void BtnCerrar_Click(object sender, EventArgs e) => Close();

        private async void CmbAnio_SelectedIndexChanged(object sender, EventArgs e) =>
            await CargarDatosAsync();

        // ── Helpers de estilo ─────────────────────────────────────────

        /// <summary>
        /// Aplica la paleta visual del sistema a cualquier FormsPlot.
        /// Fuente 39 pt, color oscuro máximo contraste.
        /// </summary>
        private static void AplicarEstilo(ScottPlot.FormsPlot fp)
        {
            fp.Plot.Style(
                figureBackground: DrawingColor.FromArgb(245, 240, 232),
                dataBackground: DrawingColor.White,
                grid: DrawingColor.FromArgb(218, 208, 192),
                tick: DrawingColor.FromArgb(38, 22, 10),
                axisLabel: DrawingColor.FromArgb(38, 22, 10),
                titleLabel: DrawingColor.FromArgb(38, 22, 10)
            );

            fp.Plot.XAxis.TickLabelStyle(
                fontName: "Segoe UI",
                fontSize: 16.25f,
                color: DrawingColor.FromArgb(38, 22, 10));

            fp.Plot.YAxis.TickLabelStyle(
                fontName: "Segoe UI",
                fontSize: 16.25f,
                color: DrawingColor.FromArgb(38, 22, 10));

            fp.Plot.XAxis.Label(" ");
            fp.Plot.YAxis.Label(" ");
        }

        private static void AplicarLeyenda(ScottPlot.FormsPlot fp,
            ScottPlot.Alignment location = ScottPlot.Alignment.LowerRight)
        {
            var leg = fp.Plot.Legend(location: location);

            leg.FontSize = 18f;
            leg.FontColor = DrawingColor.FromArgb(38, 22, 10);
            leg.FillColor = DrawingColor.FromArgb(252, 249, 244);
            leg.OutlineColor = DrawingColor.FromArgb(200, 185, 165);
            leg.ShadowColor = DrawingColor.FromArgb(25, 0, 0, 0);
        }

        private static DrawingColor[] GetPaletteColors(int count) =>
            Enumerable.Range(0, count)
                      .Select(i => _palette[i % _palette.Length])
                      .ToArray();

        private static byte[] GetChartBytes(ScottPlot.FormsPlot fp, int w, int h)
        {
            using var bmp = fp.Plot.Render(w, h);
            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        private void SetControlesActivos(bool activo)
        {
            btnRefrescar.Enabled = activo;
            btnExportar.Enabled = activo;
            cmbAnio.Enabled = activo;
        }
    }
}