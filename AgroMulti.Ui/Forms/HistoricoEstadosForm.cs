using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using AgroMulti.Data.Models;

namespace CentroFermentacionSecado
{
    public partial class HistoricoEstadosForm : Form
    {
        private readonly List<HistoricoEstadoEntrega> _historial;

        // Fuentes pre-creadas: NUNCA instanciar Font dentro de eventos de pintado
        private static readonly Font _fntEstado = new Font("Segoe UI", 9F, FontStyle.Bold);

        // Mapa de colores por palabra clave — sin azules
        private static readonly (string Clave, Color Color)[] _coloresEstado =
        {
            ("complet", Color.FromArgb(72,  118,  28)),   // verde oliva
            ("finaliz", Color.FromArgb(72,  118,  28)),
            ("listo",   Color.FromArgb(72,  118,  28)),
            ("ferment", Color.FromArgb(160,  90,  20)),   // naranja cacao
            ("secad",   Color.FromArgb(140, 100,  30)),   // marrón medio
            ("secan",   Color.FromArgb(140, 100,  30)),
            ("control", Color.FromArgb(130,  80, 160)),   // violeta suave
            ("calidad", Color.FromArgb(130,  80, 160)),
            ("pend",    Color.FromArgb(170, 120,  40)),   // ocre
            ("espera",  Color.FromArgb(170, 120,  40)),
            ("cancel",  Color.FromArgb(180,  55,  35)),   // terracota
            ("rechaz",  Color.FromArgb(180,  55,  35)),
        };

        // ── Constructor ───────────────────────────────────────────────────────
        public HistoricoEstadosForm(List<HistoricoEstadoEntrega> historial)
        {
            InitializeComponent();

            // ── Anti-parpadeo ─────────────────────────────────────────────────
            // 1. Formulario
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
            this.UpdateStyles();

            // 2. DataGridView (propiedad interna, requiere reflexión)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgvHistorial, new object[] { true });

            _historial = historial;
            CargarHistorial();
            ConfigurarTooltips();
        }

        // ── Cerrar ────────────────────────────────────────────────────────────
        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ── Carga de datos ────────────────────────────────────────────────────
        private void CargarHistorial()
        {
            dgvHistorial.Rows.Clear();

            if (_historial == null || _historial.Count == 0)
            {
                dgvHistorial.Rows.Add("—", "Sin registros",
                    "No hay cambios registrados para esta entrega.");
                dgvHistorial.Enabled = false;
                lblCardTotalValue.Text = "0";
                lblCardUltimoValue.Text = "—";
                lblCardEstadoValue.Text = "Sin datos";
                lblCardEstadoValue.ForeColor = Color.FromArgb(128, 105, 82);
                return;
            }

            foreach (var item in _historial)
            {
                string fecha = item.FechaCambio.ToString("dd/MM/yyyy  HH:mm:ss");
                string estado = item.EstadoEntrega?.Nombre ?? "Desconocido";
                string obs = string.IsNullOrWhiteSpace(item.Observaciones)
                                ? "—" : item.Observaciones;
                dgvHistorial.Rows.Add(fecha, estado, obs);
            }

            // Tarjetas de resumen
            lblCardTotalValue.Text = _historial.Count.ToString("N0");

            var ultimo = _historial[_historial.Count - 1];
            lblCardUltimoValue.Text = ultimo.FechaCambio.ToString("dd/MM/yyyy  HH:mm");

            string estadoActual = ultimo.EstadoEntrega?.Nombre ?? "—";
            lblCardEstadoValue.Text = estadoActual;
            lblCardEstadoValue.ForeColor = ObtenerColor(estadoActual);

            // Subtítulo dinámico con número de entrega
            if (ultimo.EntregaId > 0)
                lblSubtitle.Text = "Registro cronológico de transiciones · Entrega #E-"
                                   + ultimo.EntregaId.ToString("D4");
        }

        // ── CellFormatting: colorea la columna Estado ─────────────────────────
        // Usar CellFormatting en lugar de RowPrePaint evita crear objetos
        // gráficos en cada repintado y elimina el parpadeo.
        private void DgvHistorial_CellFormatting(object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != dgvHistorial.Columns["colEstado"].Index) return;
            if (e.Value == null) return;

            e.CellStyle.ForeColor = ObtenerColor(e.Value.ToString());
            e.CellStyle.Font = _fntEstado;
            e.FormattingApplied = true;
        }

        // ── Helper: color por palabra clave ───────────────────────────────────
        private static Color ObtenerColor(string estado)
        {
            string lower = estado.ToLowerInvariant();
            foreach (var par in _coloresEstado)
                if (lower.Contains(par.Clave))
                    return par.Color;
            return Color.FromArgb(80, 55, 30);
        }

        // ── Tooltips ─────────────────────────────────────────────────────────
        private void ConfigurarTooltips()
        {
            var tt = new ToolTip { InitialDelay = 400, AutoPopDelay = 4000 };
            tt.SetToolTip(btnCerrar, "Cerrar esta ventana");
            tt.SetToolTip(dgvHistorial, "Historial de cambios de estado de la entrega");
        }
    }
}