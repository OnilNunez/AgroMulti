using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CentroFermentacionSecado
{
    public partial class ConsultaEntregasForm : Form
    {
        public ConsultaEntregasForm()
        {
            InitializeComponent();
            // Sobrescribimos los items fijos del diseñador con datos reales desde la BD
            cboEstado.Items.Clear();
            CargarCombos();
            LimpiarFiltros();
            // Mostrar los resultados iniciales (último mes)
            CargarResultados();
        }

        // ── Carga de combos desde la base de datos ────────────────────
        private void CargarCombos()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    // Productores
                    var productores = db.Productors
                        .OrderBy(p => p.Codigo)
                        .ToList();
                    cboProductor.DataSource = null;
                    productores.Insert(0, new Productor { ProductorId = 0, Codigo = "", Nombre = "(Todos)", Apellido = "" });
                    cboProductor.DataSource = productores;
                    cboProductor.DisplayMember = "Codigo";           // Muestra el código en el combo
                    cboProductor.ValueMember = "ProductorId";

                    // Productos
                    var productos = db.Productos
                        .OrderBy(p => p.Nombre)
                        .ToList();
                    productos.Insert(0, new Producto { ProductoId = 0, Nombre = "(Todos)" });
                    cboProducto.DataSource = productos;
                    cboProducto.DisplayMember = "Nombre";
                    cboProducto.ValueMember = "ProductoId";

                    // Estados de entrega
                    var estados = db.EstadoEntregas
                        .OrderBy(e => e.Nombre)
                        .ToList();
                    estados.Insert(0, new EstadoEntrega { EstadoEntregaId = 0, Nombre = "(Todos)" });
                    cboEstado.DataSource = estados;
                    cboEstado.DisplayMember = "Nombre";
                    cboEstado.ValueMember = "EstadoEntregaId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las listas de filtros: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Restablecer filtros al estado inicial ────────────────────
        private void LimpiarFiltros()
        {
            dtpFechaDesde.Value = DateTime.Today.AddDays(-30);
            dtpFechaHasta.Value = DateTime.Today;
            cboProductor.SelectedIndex = 0;
            cboProducto.SelectedIndex = 0;
            cboEstado.SelectedIndex = 0;
        }

        // ── Eventos de botones ──────────────────────────────────────
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarResultados();
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            LimpiarFiltros();
            dgvEntregas.Rows.Clear();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ── Consulta y llenado del DataGridView ─────────────────────
        private void CargarResultados()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    // Obtener los valores de los filtros
                    DateOnly desde = DateOnly.FromDateTime(dtpFechaDesde.Value.Date);
                    DateOnly hasta = DateOnly.FromDateTime(dtpFechaHasta.Value.Date);
                    int productorId = cboProductor.SelectedValue != null ? (int)cboProductor.SelectedValue : 0;
                    int productoId = cboProducto.SelectedValue != null ? (int)cboProducto.SelectedValue : 0;
                    int estadoId = cboEstado.SelectedValue != null ? (int)cboEstado.SelectedValue : 0;

                    IQueryable<Entrega> query = db.Entregas
                        .Include(e => e.Productor)
                        .Include(e => e.Producto)
                        .Include(e => e.SubProducto)
                        .Include(e => e.EstadoEntrega)
                        .Where(e => e.FechaEntrega >= desde && e.FechaEntrega <= hasta);

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
                            entrega.NumeroEntrega,
                            entrega.FechaEntrega.ToString("dd/MM/yyyy"),
                            $"{entrega.Productor.Nombre} {entrega.Productor.Apellido}",
                            entrega.Producto.Nombre,
                            entrega.SubProducto?.Nombre ?? "",
                            entrega.Kilos.ToString("N2"),
                            entrega.EstadoEntrega.Nombre
                        );
                    }

                    if (entregas.Count == 0)
                    {
                        // Opcional: mostrar un mensaje discreto en el grupo de resultados
                        // Se puede dejar vacío o mostrar algo en un label.
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar entregas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
