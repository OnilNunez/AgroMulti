using AgroMulti;
using AgroMulti.Data.Models;
using AgroMulti.Ui.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class ConsultaEntregasForm : Form
    {
        private readonly ProductorService _productorService;
        private readonly ProductoService _productoService;
        private readonly EstadoEntregaService _estadoEntregaService;
        private readonly EntregaService _entregaService;
        private readonly HistoricoEstadoEntregaService _historicoService;

        public ConsultaEntregasForm()
        {
            InitializeComponent();
            cboEstado.Items.Clear();

            _productorService = Program.ServiceProvider.GetRequiredService<ProductorService>();
            _productoService = Program.ServiceProvider.GetRequiredService<ProductoService>();
            _estadoEntregaService = Program.ServiceProvider.GetRequiredService<EstadoEntregaService>();
            _entregaService = Program.ServiceProvider.GetRequiredService<EntregaService>();
            _historicoService = Program.ServiceProvider.GetRequiredService<HistoricoEstadoEntregaService>();

            Load += async (s, e) => await InicializarAsync();
            
        }

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
                var productores = (await _productorService.GetList(p => true))
                    .OrderBy(p => p.Codigo).ToList();
                productores.Insert(0, new Productor { ProductorId = 0, Codigo = "", Nombre = "(Todos)", Apellido = "" });
                cboProductor.DataSource = productores;
                cboProductor.DisplayMember = "Codigo";
                cboProductor.ValueMember = "ProductorId";

                var productos = (await _productoService.GetList(p => true))
                    .OrderBy(p => p.Nombre).ToList();
                productos.Insert(0, new Producto { ProductoId = 0, Nombre = "(Todos)" });
                cboProducto.DataSource = productos;
                cboProducto.DisplayMember = "Nombre";
                cboProducto.ValueMember = "ProductoId";

                var estados = (await _estadoEntregaService.GetList(e => true))
                    .OrderBy(e => e.Nombre).ToList();
                estados.Insert(0, new EstadoEntrega { EstadoEntregaId = 0, Nombre = "(Todos)" });
                cboEstado.DataSource = estados;
                cboEstado.DisplayMember = "Nombre";
                cboEstado.ValueMember = "EstadoEntregaId";
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

        private async void btnBuscar_Click(object sender, EventArgs e) => await CargarResultadosAsync();
        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private async void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            LimpiarFiltros();
            dgvEntregas.Rows.Clear();
        }

        
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            var formHistorial = new HistoricoEstadosForm();
            formHistorial.ShowDialog(this);
        }

        private async Task CargarResultadosAsync()
        {
            try
            {
                DateOnly desde = DateOnly.FromDateTime(dtpFechaDesde.Value.Date);
                DateOnly hasta = DateOnly.FromDateTime(dtpFechaHasta.Value.Date);
                int productorId = cboProductor.SelectedValue != null ? (int)cboProductor.SelectedValue : 0;
                int productoId = cboProducto.SelectedValue != null ? (int)cboProducto.SelectedValue : 0;
                int estadoId = cboEstado.SelectedValue != null ? (int)cboEstado.SelectedValue : 0;

                var todas = await _entregaService.GetListConRelaciones(e =>
                    e.FechaEntrega >= desde && e.FechaEntrega <= hasta);

                var query = todas.AsEnumerable();
                if (productorId > 0)
                    query = query.Where(e => e.ProductorId == productorId);
                if (productoId > 0)
                    query = query.Where(e => e.ProductoId == productoId);
                if (estadoId > 0)
                    query = query.Where(e => e.EstadoEntregaId == estadoId);

                var entregas = query
                    .OrderByDescending(e => e.FechaEntrega)
                    .ThenByDescending(e => e.EntregaId)
                    .ToList();

                dgvEntregas.Rows.Clear();

                foreach (var entrega in entregas)
                {
                    dgvEntregas.Rows.Add(
                        entrega.EntregaId,
                        entrega.NumeroEntrega,
                        entrega.FechaEntrega.ToString("dd/MM/yyyy"),
                        $"{entrega.Productor.Nombre} {entrega.Productor.Apellido}",
                        entrega.Producto.Nombre,
                        entrega.SubProducto?.Nombre ?? "",
                        entrega.Kilos.ToString("N2"),
                        entrega.EstadoEntrega.Nombre,
                        entrega.Observaciones ?? ""
                    );
                }

                dgvEntregas.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar entregas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                MessageBox.Show($"La entrega se encuentra en estado {estadoActual}.\nEste es un estado final y no puede modificarse.",
                    "Estado final", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string? nuevoEstado = MostrarDialogoCambioEstado(estadoActual, transiciones);
            if (nuevoEstado == null) return;

            try
            {
                var entrega = await _entregaService.Buscar(entregaId);
                if (entrega == null)
                {
                    MessageBox.Show("No se encontró la entrega en la base de datos.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var estadosCoincidentes = await _estadoEntregaService.GetList(es =>
                    es.Nombre.ToLower() == nuevoEstado.ToLower());
                var estadoNuevo = estadosCoincidentes.FirstOrDefault();

                if (estadoNuevo == null)
                {
                    MessageBox.Show($"No se encontró el estado «{nuevoEstado}» en la base de datos.\nVerifique que los nombres de estados coincidan exactamente.",
                        "Estado no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string estadoAnteriorNombre = estadoActual;
                entrega.EstadoEntregaId = estadoNuevo.EstadoEntregaId;
                bool guardado = await _entregaService.Guardar(entrega);

                if (!guardado)
                {
                    MessageBox.Show("No se pudo actualizar la entrega.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var nuevoHistorial = new HistoricoEstadoEntrega
                {
                    EntregaId = entregaId,
                    EstadoEntregaId = estadoNuevo.EstadoEntregaId,
                    Observaciones = $"Cambio de estado de '{estadoAnteriorNombre}' a '{nuevoEstado}'"
                };
                await _historicoService.Guardar(nuevoHistorial);

                MessageBox.Show($"Estado actualizado correctamente a {estadoNuevo.Nombre}.\nEl cambio ha sido registrado en el historial.",
                    "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await CargarResultadosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el estado: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                Size = new Size(460, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 240, 232),
                Font = new Font("Segoe UI", 9F)
            };

            var headerPanel = new Panel
            {
                BackColor = Color.FromArgb(38, 22, 10),
                Dock = DockStyle.Top,
                Height = 52
            };
            var headerLabel = new Label
            {
                Text = "Modificar estado de entrega",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(18, 14)
            };
            var accentLine = new Panel
            {
                BackColor = Color.FromArgb(92, 122, 42),
                Dock = DockStyle.Bottom,
                Height = 3
            };
            headerPanel.Controls.Add(headerLabel);
            headerPanel.Controls.Add(accentLine);

            var lblActualEtiqueta = new Label
            {
                Text = "Estado actual:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(128, 105, 82),
                AutoSize = true,
                Location = new Point(24, 76)
            };
            var lblActualValor = new Label
            {
                Text = estadoActual,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 22, 10),
                AutoSize = true,
                Location = new Point(148, 74)
            };

            var separador = new Panel
            {
                BackColor = Color.FromArgb(210, 195, 175),
                Location = new Point(24, 108),
                Size = new Size(400, 1)
            };

            var lblNuevoEtiqueta = new Label
            {
                Text = "Nuevo estado:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(128, 105, 82),
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
                BackColor = Color.FromArgb(92, 122, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(148, 185),
                Size = new Size(168, 40),
                DialogResult = DialogResult.OK,
                UseVisualStyleBackColor = false
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                BackColor = Color.White,
                ForeColor = Color.FromArgb(44, 28, 16),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(330, 185),
                Size = new Size(94, 40),
                DialogResult = DialogResult.Cancel,
                UseVisualStyleBackColor = false
            };
            btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);

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