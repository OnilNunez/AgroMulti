using AgroMulti;
using AgroMulti.Data.Models;
using AgroMulti.Ui.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class HistoricoEstadosForm : Form
    {
        private List<HistoricoEstadoEntrega> _historial;

        // Fuentes pre‑creadas
        private static readonly Font _fntEstado = new Font("Segoe UI", 9F, FontStyle.Bold);

        // Mapa de colores por palabra clave
        private static readonly (string Clave, Color Color)[] _coloresEstado =
        {
            ("complet", Color.FromArgb(72,  118,  28)),
            ("finaliz", Color.FromArgb(72,  118,  28)),
            ("listo",   Color.FromArgb(72,  118,  28)),
            ("ferment", Color.FromArgb(160,  90,  20)),
            ("secad",   Color.FromArgb(140, 100,  30)),
            ("secan",   Color.FromArgb(140, 100,  30)),
            ("control", Color.FromArgb(130,  80, 160)),
            ("calidad", Color.FromArgb(130,  80, 160)),
            ("pend",    Color.FromArgb(170, 120,  40)),
            ("espera",  Color.FromArgb(170, 120,  40)),
            ("cancel",  Color.FromArgb(180,  55,  35)),
            ("rechaz",  Color.FromArgb(180,  55,  35)),
        };

        // ── Servicio inyectado ─────────────────────────────────────
        private readonly HistoricoEstadoEntregaService _historicoService;

        // ── Constructor ─────────────────────────────────────────────
        public HistoricoEstadosForm()
        {
            InitializeComponent();

            // Anti‑parpadeo
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
            this.UpdateStyles();

            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgvHistorial, new object[] { true });

            // Obtener servicio
            _historicoService = Program.ServiceProvider.GetRequiredService<HistoricoEstadoEntregaService>();

            this.Load += HistoricoEstadosForm_Load;
            ConfigurarTooltips();
        }

        // ── Carga asíncrona ─────────────────────────────────────────
        private async void HistoricoEstadosForm_Load(object sender, EventArgs e)
        {
            try
            {
                _historial = await _historicoService.ObtenerTodosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _historial = new List<HistoricoEstadoEntrega>();
            }
            CargarHistorial();
        }

        // ── Cerrar ──────────────────────────────────────────────────
        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ── Carga de datos en la interfaz ───────────────────────────
        private void CargarHistorial()
        {
            dgvHistorial.Rows.Clear();
            lblCardEstadoTitle.Text = "Entregas";

            if (_historial == null || _historial.Count == 0)
            {
                dgvHistorial.Rows.Add("—", "—", "Sin registros",
                    "No hay cambios registrados en el sistema.");
                dgvHistorial.Enabled = false;
                lblCardTotalValue.Text = "0";
                lblCardUltimoValue.Text = "—";
                lblCardEstadoValue.Text = "0";
                lblCardEstadoValue.ForeColor = Color.FromArgb(128, 105, 82);
                lblSubtitle.Text = "Registro cronológico de transiciones · Sin datos";
                return;
            }

            foreach (var item in _historial)
            {
                string fecha = item.FechaCambio.ToString("dd/MM/yyyy  HH:mm:ss");
                string entrega = $"E-{item.EntregaId:D4}";
                string estado = item.EstadoEntrega?.Nombre ?? "Desconocido";
                string obs = string.IsNullOrWhiteSpace(item.Observaciones)
                                ? "—" : item.Observaciones;
                dgvHistorial.Rows.Add(fecha, entrega, estado, obs);
            }

            lblCardTotalValue.Text = _historial.Count.ToString("N0");

            var ultimo = _historial.Last();
            lblCardUltimoValue.Text = ultimo.FechaCambio.ToString("dd/MM/yyyy  HH:mm");

            int entregasDistintas = _historial.Select(h => h.EntregaId).Distinct().Count();
            lblCardEstadoValue.Text = entregasDistintas.ToString();
            lblCardEstadoValue.ForeColor = Color.FromArgb(92, 122, 42);

            lblSubtitle.Text = "Registro cronológico de transiciones · Todas las entregas";
        }

        // ── CellFormatting: colorea la columna Estado ─────────────────
        private void DgvHistorial_CellFormatting(object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != dgvHistorial.Columns["colEstado"].Index) return;
            if (e.Value == null) return;

            e.CellStyle.ForeColor = ObtenerColor(e.Value.ToString());
            e.CellStyle.Font = _fntEstado;
            e.FormattingApplied = true;
        }

        // ── Helper: color por palabra clave ──────────────────────────
        private static Color ObtenerColor(string estado)
        {
            string lower = estado.ToLowerInvariant();
            foreach (var par in _coloresEstado)
                if (lower.Contains(par.Clave))
                    return par.Color;
            return Color.FromArgb(80, 55, 30);
        }

        // ── Tooltips ─────────────────────────────────────────────────
        private void ConfigurarTooltips()
        {
            var tt = new ToolTip { InitialDelay = 400, AutoPopDelay = 4000 };
            tt.SetToolTip(btnCerrar, "Cerrar esta ventana");
            tt.SetToolTip(dgvHistorial, "Historial de cambios de estado de todas las entregas");
        }
    }
}