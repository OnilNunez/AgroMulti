using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AgroMulti.Data.Data;       // Asegúrate de tener la referencia al proyecto de datos
using Microsoft.EntityFrameworkCore;

namespace CentroFermentacionSecado
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CargarDashboard();
        }

        private void ConfigureForm()
        {
            // La navegación se gestiona desde el menú y el ToolStrip.
            dgvRecentDeliveries.ClearSelection();
        }

        // ── Carga de datos del dashboard ──────────────────────────────
        private void CargarDashboard()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    // Fecha de hoy para los filtros del resumen
                    var hoy = DateOnly.FromDateTime(DateTime.Today);

                    // ── Resumen del día ──────────────────────────────
                    var entregasHoy = db.Entregas
                        .Include(e => e.EstadoEntrega)
                        .Where(e => e.FechaEntrega == hoy)
                        .ToList();

                    // Total kilos (sumamos Kilos, pero si hay KilosSecos puedes usarlo)
                    decimal totalKilos = entregasHoy.Sum(e => e.Kilos);
                    lblTotalKilosValue.Text = totalKilos.ToString("N0") + " kg";

                    // Total entregas
                    lblTotalDeliveriesValue.Text = entregasHoy.Count.ToString();

                    // Pendientes: busca estados que contengan "pendiente"
                    int pendientes = entregasHoy.Count(e =>
                        e.EstadoEntrega.Nombre.IndexOf("pendiente", StringComparison.OrdinalIgnoreCase) >= 0);
                    lblPendingValue.Text = pendientes.ToString();

                    // Completadas: busca estados que contengan "completad" (así cubre "completado", "completada")
                    int completadas = entregasHoy.Count(e =>
                        e.EstadoEntrega.Nombre.IndexOf("completad", StringComparison.OrdinalIgnoreCase) >= 0);
                    lblCompletedValue.Text = completadas.ToString();

                    // ── Entregas recientes (últimas 20) ──────────────
                    var recientes = db.Entregas
                        .Include(e => e.Productor)
                        .Include(e => e.Producto)
                        .Include(e => e.EstadoEntrega)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos del dashboard: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Eventos del menú y ToolStrip (se mantienen igual) ────────

        private void NuevaEntrega_Click(object sender, EventArgs e)
        {
            using var form = new RegistroEntregaForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
            // Recargar datos al cerrar el formulario (opcional)
            CargarDashboard();
        }

        private void ConsultarEntregas_Click(object sender, EventArgs e)
        {
            using var form = new ConsultaEntregasForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
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

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void groupRecentDeliveries_Enter(object sender, EventArgs e) { }

        private void ayudaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new AcercaDeForm())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();
            }
        }
    }
}
