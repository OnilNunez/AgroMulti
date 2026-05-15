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

namespace CentroFermentacionSecado
{
    public partial class DashboardForm : Form
    {
        // ── Servicios ────────────────────────────────────────────────
        private readonly EntregaService _entregaService;

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

                int anio = int.TryParse(cmbAnio.SelectedItem?.ToString(), out int a)
                    ? a : DateTime.Now.Year;
                var entregasAnio = entregas.Where(e => e.FechaEntrega.Year == anio).ToList();

                BuildChartKilosMes(entregasAnio, anio);
                BuildChartEstados(entregas);
                BuildChartTopProductores(entregas);
                BuildChartPorProducto(entregas);
                BuildChartDiaSemana(entregas);
                BuildChartKilosSecos(entregasAnio, anio);

                lblEstado.Text =
                    $"Actualizado: {DateTime.Now:dd/MM/yyyy  HH:mm}  ·  " +
                    $"{entregas.Count:N0} registros totales  ·  " +
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
            string[] meses = { "Ene","Feb","Mar","Abr","May","Jun",
                                "Jul","Ago","Sep","Oct","Nov","Dic" };

            foreach (var e in entregas)
                kilos[e.FechaEntrega.Month - 1] += (double)e.Kilos;

            fpKilosMes.Plot.Clear();

            if (kilos.Sum() > 0)
            {
                var bar = fpKilosMes.Plot.AddBar(kilos, pos);
                bar.FillColor = DrawingColor.FromArgb(92, 122, 42);
                bar.BorderColor = DrawingColor.FromArgb(72, 98, 30);
                bar.BorderLineWidth = 1.5f;
                fpKilosMes.Plot.XTicks(pos, meses);
                fpKilosMes.Plot.YAxis.Label("Kilos");
                double max = kilos.Max();
                if (max > 0) fpKilosMes.Plot.SetAxisLimitsY(0, max * 1.20);
            }

            
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
            }

            fpEstados.Plot.Title("");
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
            }

            fpProductos.Plot.Title("");
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

            
            AplicarEstilo(fpDiaSemana);
            fpDiaSemana.Refresh();
        }

        // ── Gráfico 6 · Kilos frescos vs secos ───────────────────────
        private void BuildChartKilosSecos(List<Entrega> entregas, int anio)
        {
            double[] xs = Enumerable.Range(1, 12).Select(i => (double)i).ToArray();
            double[] frescos = new double[12];
            double[] secos = new double[12];
            string[] meses = { "Ene","Feb","Mar","Abr","May","Jun",
                                  "Jul","Ago","Sep","Oct","Nov","Dic" };

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

                fpKilosSecos.Plot.XTicks(xs, meses);
                fpKilosSecos.Plot.YAxis.Label("Kilos");
                AplicarLeyenda(fpKilosSecos, ScottPlot.Alignment.UpperRight);
            }

            fpKilosSecos.Plot.Title($"Kilos frescos vs secos ({anio})");
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
                using var wb = new XLWorkbook();

                var charts = new (ScottPlot.FormsPlot Fp, string Nombre)[]
                {
                    (fpKilosMes,    "Kilos por mes"),
                    (fpEstados,     "Estados"),
                    (fpProductores, "Top productores"),
                    (fpProductos,   "Por producto"),
                    (fpDiaSemana,   "Dias semana"),
                    (fpKilosSecos,  "Kilos frescos-secos"),
                };

                foreach (var (fp, nombre) in charts)
                {
                    var ws = wb.Worksheets.Add(nombre);

                    ws.Cell(1, 1).Value = nombre;
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontColor = XLColor.White;
                    ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(58, 38, 18);
                    ws.Cell(1, 1).Style.Font.FontSize = 13;
                    ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(1, 1, 1, 10).Merge();
                    ws.Row(1).Height = 22;

                    byte[] imgBytes = GetChartBytes(fp, 960, 500);
                    using var ms = new MemoryStream(imgBytes);
                    var pic = ws.AddPicture(ms).MoveTo(ws.Cell(2, 1));
                    pic.Width = 960;
                    pic.Height = 500;

                    ws.Column(1).Width = 135;
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
                var charts = new (ScottPlot.FormsPlot Fp, string Titulo)[]
                {
                    (fpKilosMes,    "Kilos recibidos por mes"),
                    (fpEstados,     ""),
                    (fpProductores, "Top 5 productores por kilos"),
                    (fpProductos,   ""),
                    (fpDiaSemana,   "Actividad por día de la semana"),
                    (fpKilosSecos,  "Kilos frescos vs secos"),
                };

                var imagenes = charts.Select(c => GetChartBytes(c.Fp, 980, 340)).ToArray();

                QuestPDF.Fluent.Document.Create(container =>
                {
                    for (int p = 0; p < 3; p++)
                    {
                        int pageIdx = p;
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4.Landscape());
                            page.Margin(1.2f, Unit.Centimetre);
                            page.PageColor(Colors.White);

                            page.Header()
                                .Background("#26160A")
                                .Padding(10)
                                .Column(col =>
                                {
                                    col.Item()
                                        .Text("Dashboard de Análisis — Centro de Fermentación y Secado")
                                        .FontFamily("Segoe UI").FontSize(15).Bold()
                                        .FontColor(Colors.White);
                                    col.Item()
                                        .Text($"Generado: {DateTime.Now:dd/MM/yyyy  HH:mm}")
                                        .FontSize(9).FontColor("#B9A58C");
                                });

                            page.Content().Column(col =>
                            {
                                int idxA = pageIdx * 2;
                                int idxB = pageIdx * 2 + 1;

                                col.Item().PaddingTop(6)
                                    .Text(charts[idxA].Titulo)
                                    .Bold().FontSize(11).FontColor("#26160A");
                                col.Item().Image(imagenes[idxA]).FitWidth();

                                col.Item().PaddingTop(10)
                                    .Text(charts[idxB].Titulo)
                                    .Bold().FontSize(11).FontColor("#26160A");
                                col.Item().Image(imagenes[idxB]).FitWidth();
                            });

                            page.Footer().AlignRight().Text(text =>
                            {
                                text.Span($"Página {pageIdx + 1} de 3  ·  Dashboard AgroMulti")
                                    .FontSize(8).FontColor("#6B4C32");
                            });
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

        /// <summary>
        /// Activa y estiliza la leyenda de un gráfico con la paleta del sistema.
        /// </summary>


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