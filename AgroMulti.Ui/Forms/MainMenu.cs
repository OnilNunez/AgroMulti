using AgroMulti;
using AgroMulti.Ui.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class MainMenu : Form
    {
        private int _isLoading = 0;   // 0 = no cargando, 1 = cargando

        public MainMenu()
        {
            InitializeComponent();

            Load += async (s, e) => await CargarDashboardAsync();
            Activated += async (s, e) => await CargarDashboardAsync();
        }

        private async Task CargarDashboardAsync()
        {
            // Evita ejecuciones concurrentes
            if (Interlocked.Exchange(ref _isLoading, 1) == 1) return;

            try
            {
                
                var entregaService = Program.ServiceProvider.GetRequiredService<EntregaService>();

                var hoy = DateOnly.FromDateTime(DateTime.Today);

                // Entregas del día
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

                // Últimas 20 entregas
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

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void ayudaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var frm = new AcercaDeForm();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }
    }
}