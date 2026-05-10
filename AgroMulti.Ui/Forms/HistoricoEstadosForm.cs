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

namespace CentroFermentacionSecado
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
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Historial");

                var header = ws.Range("A1:E1");
                header.Style.Font.Bold = true;
                header.Style.Font.FontColor = XLColor.White;
                header.Style.Fill.BackgroundColor = XLColor.FromArgb(58, 38, 18);
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(1, 1).Value = "Fecha y hora";
                ws.Cell(1, 2).Value = "Entrega";
                ws.Cell(1, 3).Value = "Lugar en almacén";
                ws.Cell(1, 4).Value = "Estado";
                ws.Cell(1, 5).Value = "Observaciones";

                int row = 2;
                foreach (var h in _historialFiltrado)
                {
                    ws.Cell(row, 1).Value = h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss");
                    ws.Cell(row, 2).Value = $"E-{h.EntregaId:D4}";
                    ws.Cell(row, 3).Value = ObtenerLugar(h.EntregaId);
                    ws.Cell(row, 4).Value = h.EstadoEntrega?.Nombre ?? "Desconocido";
                    ws.Cell(row, 5).Value = string.IsNullOrWhiteSpace(h.Observaciones) ? "—" : h.Observaciones;

                    string estado = (h.EstadoEntrega?.Nombre ?? "").ToLowerInvariant();
                    var color = ObtenerColorFromName(estado);
                    ws.Cell(row, 4).Style.Font.FontColor = XLColor.FromArgb(color.R, color.G, color.B);
                    ws.Cell(row, 4).Style.Font.Bold = true;

                    row++;
                }

                ws.Columns().AdjustToContents();
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
                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1.5f, Unit.Centimetre);
                        page.PageColor(QuestPDF.Helpers.Colors.White);

                        // ── Encabezado estilo "agro" (CORREGIDO) ──
                        page.Header()
                            .Background("#26160A")
                            .Padding(12)
                            .Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Historial de cambios de estado")
                                        .FontFamily("Segoe UI")
                                        .FontSize(18)
                                        .Bold()
                                        .FontColor(QuestPDF.Helpers.Colors.White);

                                    col.Item().Text("Centro de Fermentación y Secado")
                                        .FontSize(10)
                                        .FontColor("#B9A58C");

                                    col.Item().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                                        .FontSize(9)
                                        .FontColor("#B9A58C");
                                });
                            });

                        // Tabla
                        page.Content().Table(table =>
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

                            foreach (var h in _historialFiltrado)
                            {
                                string estado = h.EstadoEntrega?.Nombre ?? "Desconocido";
                                var colorEstado = ObtenerColorFromName(estado.ToLowerInvariant());
                                string colorHex = $"#{colorEstado.R:X2}{colorEstado.G:X2}{colorEstado.B:X2}";

                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(h.FechaCambio.ToString("dd/MM/yyyy HH:mm:ss"))
                                    .FontSize(8.5f).FontFamily("Consolas");
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text($"E-{h.EntregaId:D4}").Bold().FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(ObtenerLugar(h.EntregaId)).FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(estado)
                                    .FontColor(colorHex).Bold().FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor("#DED2C2")
                                    .Padding(3).Text(string.IsNullOrWhiteSpace(h.Observaciones) ? "—" : h.Observaciones)
                                    .FontSize(9);
                            }
                        });

                        // Pie de página
                        page.Footer().AlignRight().Text(text =>
                        {
                            text.Span("Página ").FontSize(8).FontColor("#6B4C32");
                            text.CurrentPageNumber().FontSize(8).FontColor("#6B4C32");
                            text.Span(" de ").FontSize(8).FontColor("#6B4C32");
                            text.TotalPages().FontSize(8).FontColor("#6B4C32");
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