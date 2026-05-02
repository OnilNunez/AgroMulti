using AgroMulti;
using AgroMulti.Data.Models;
using AgroMulti.Ui.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class RegistroEntregaForm : Form
    {
        // ── Servicios ────────────────────────────────────────────────
        private readonly ProductorService _productorService;
        private readonly ProductoService _productoService;
        private readonly EstadoEntregaService _estadoEntregaService;
        private readonly SubProductoService _subProductoService;
        private readonly EntregaService _entregaService;

        // ── Estado ─────────────────────────────────────────────────────
        private Productor _productorSeleccionado = null;
        private List<Productor> _todosProductores = new List<Productor>();
        private bool _suppressEvents = false;

        private class ProductorDisplay
        {
            public int ProductorId { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
        }

        public RegistroEntregaForm()
        {
            InitializeComponent();

            // Obtener todos los servicios desde el contenedor global
            _productorService = Program.ServiceProvider.GetRequiredService<ProductorService>();
            _productoService = Program.ServiceProvider.GetRequiredService<ProductoService>();
            _estadoEntregaService = Program.ServiceProvider.GetRequiredService<EstadoEntregaService>();
            _subProductoService = Program.ServiceProvider.GetRequiredService<SubProductoService>();
            _entregaService = Program.ServiceProvider.GetRequiredService<EntregaService>();

            ConfigurarFormulario();
            ConfigurarDgvProductores();
            CargarDatosIniciales();
        }

        private async void CargarDatosIniciales()
        {
            await CargarCombosAsync();
            await CargarProductoresAsync();
            await CargarUbicacionCombosAsync();
            await CargarSiguienteNumeroEntregaAsync();
        }

        // ── Inicialización de eventos ────────────────────────────────
        private void ConfigurarFormulario()
        {
            cboCodigoProductor.SelectedIndexChanged += CboCodigoProductor_SelectedIndexChanged;
            cboCodigoProductor.TextChanged += CboCodigoProductor_TextChanged;
            cboCodigoProductor.KeyDown += CboCodigoProductor_KeyDown;
            btnBuscarProductor.Click += BtnBuscarProductor_Click;
            btnAgregarProductor.Click += BtnAgregarProductor_Click;

            dgvProductores.SelectionChanged += DgvProductores_SelectionChanged;
            dgvProductores.CellDoubleClick += DgvProductores_CellDoubleClick;

            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            btnLimpiar.Click += BtnLimpiar_Click;

            btnBuscarProductor.Cursor = Cursors.Hand;
            btnAgregarProductor.Cursor = Cursors.Hand;
            btnGuardar.Cursor = Cursors.Hand;
            btnCancelar.Cursor = Cursors.Hand;
            btnLimpiar.Cursor = Cursors.Hand;

            dtpFechaEntrega.Format = DateTimePickerFormat.Short;
            dtpFechaEntrega.Value = DateTime.Today;

            cboProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSubProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEstadoEntrega.DropDownStyle = ComboBoxStyle.DropDownList;

            cboProducto.SelectedIndexChanged += CboProducto_SelectedIndexChanged;
        }

        private void ConfigurarDgvProductores()
        {
            dgvProductores.AutoGenerateColumns = false;
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProductorId", DataPropertyName = "ProductorId", HeaderText = "ID", Visible = false });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", DataPropertyName = "Codigo", HeaderText = "Código", Width = 90, ReadOnly = true });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 140, ReadOnly = true });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn { Name = "colApellido", DataPropertyName = "Apellido", HeaderText = "Apellido", Width = 140, ReadOnly = true });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTelefono", DataPropertyName = "Telefono", HeaderText = "Teléfono", Width = 110, ReadOnly = true });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDireccion", DataPropertyName = "Direccion", HeaderText = "Dirección", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
        }

        // ── Carga de datos ────────────────────────────────────────────
        private async Task CargarProductoresAsync()
        {
            try
            {
                var lista = await _productorService.GetList(p => true);
                _todosProductores = lista.OrderBy(p => p.Codigo).ToList();

                _suppressEvents = true;
                var vacio = new { ProductorId = 0, Codigo = "", NombreCompleto = "" };
                var listaMostrar = _todosProductores
                    .Select(p => new { p.ProductorId, p.Codigo, NombreCompleto = p.Nombre + " " + p.Apellido })
                    .ToList();
                var final = new List<dynamic> { vacio };
                final.AddRange(listaMostrar.Cast<dynamic>());
                cboCodigoProductor.DataSource = final;
                cboCodigoProductor.DisplayMember = "Codigo";
                cboCodigoProductor.ValueMember = "ProductorId";
                cboCodigoProductor.SelectedIndex = 0;
                _suppressEvents = false;
                ActualizarDgvProductores(string.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarCombosAsync()
        {
            try
            {
                var productos = (await _productoService.GetList(p => true))
                    .OrderBy(p => p.Nombre).ToList();
                productos.Insert(0, new Producto { ProductoId = 0, Nombre = " Seleccione " });
                cboProducto.DataSource = productos;
                cboProducto.DisplayMember = "Nombre";
                cboProducto.ValueMember = "ProductoId";

                var estados = (await _estadoEntregaService.GetList(e => true))
                    .OrderBy(e => e.Nombre).ToList();
                estados.Insert(0, new EstadoEntrega { EstadoEntregaId = 0, Nombre = " Seleccione " });
                cboEstadoEntrega.DataSource = estados;
                cboEstadoEntrega.DisplayMember = "Nombre";
                cboEstadoEntrega.ValueMember = "EstadoEntregaId";

                cboSubProducto.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar listas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarUbicacionCombosAsync()
        {
            try
            {
                var entregasConCalle = await _entregaService.GetList(e => e.Calle != null && e.Calle != "");
                var calles = entregasConCalle.Select(e => e.Calle).Distinct().OrderBy(x => x).ToList();
                cboCalle.Items.Clear();
                cboCalle.Items.AddRange(calles.ToArray());

                var entregasConZona = await _entregaService.GetList(e => e.Zona != null && e.Zona != "");
                var zonas = entregasConZona.Select(e => e.Zona).Distinct().OrderBy(x => x).ToList();
                cboFila.Items.Clear();
                cboFila.Items.AddRange(zonas.ToArray());

                var entregasConSeccion = await _entregaService.GetList(e => e.Seccion != null && e.Seccion != "");
                var secciones = entregasConSeccion.Select(e => e.Seccion).Distinct().OrderBy(x => x).ToList();
                cboPosicion.Items.Clear();
                cboPosicion.Items.AddRange(secciones.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar ubicaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarSiguienteNumeroEntregaAsync()
        {
            try
            {
                var ultimas = await _entregaService.GetList(e => e.NumeroEntrega.StartsWith("E-"));
                var ultimoNumero = ultimas
                    .OrderByDescending(e => e.NumeroEntrega)
                    .Select(e => e.NumeroEntrega)
                    .FirstOrDefault();

                int siguiente = 1;
                if (ultimoNumero != null)
                {
                    string numeroStr = ultimoNumero.Replace("E-", "").TrimStart('0');
                    if (int.TryParse(numeroStr, out int ultimo))
                        siguiente = ultimo + 1;
                }
                txtNumeroEntrega.Text = $"E-{siguiente:D5}";
            }
            catch (Exception)
            {
                txtNumeroEntrega.Text = "E-00001";
            }
        }

        // ── Subproductos ──────────────────────────────────────────────
        private async void CboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducto.SelectedValue is int idProducto && idProducto > 0)
            {
                try
                {
                    var subs = (await _subProductoService.GetList(s => s.ProductoId == idProducto))
                        .OrderBy(s => s.Nombre).ToList();
                    subs.Insert(0, new SubProducto { SubProductoId = 0, Nombre = " Seleccione " });
                    cboSubProducto.DataSource = subs;
                    cboSubProducto.DisplayMember = "Nombre";
                    cboSubProducto.ValueMember = "SubProductoId";
                }
                catch (Exception) { /* ignorar */ }
            }
            else
            {
                cboSubProducto.DataSource = null;
            }
        }

        // ── DGV y Combo de productor ─────────────────────────────────
        private void ActualizarDgvProductores(string filtro)
        {
            var query = _todosProductores.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(filtro))
                query = query.Where(p => (p.Codigo ?? "").Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                         (p.Nombre + " " + p.Apellido).Contains(filtro, StringComparison.OrdinalIgnoreCase));
            var datos = query.Select(p => new ProductorDisplay
            {
                ProductorId = p.ProductorId,
                Codigo = p.Codigo ?? "",
                Nombre = p.Nombre ?? "",
                Apellido = p.Apellido ?? "",
                Telefono = p.Telefono ?? "",
                Direccion = p.Direccion ?? ""
            }).ToList();
            dgvProductores.DataSource = datos;
        }

        private void SeleccionarFilaEnDgv(int productorId)
        {
            _suppressEvents = true;
            dgvProductores.ClearSelection();
            foreach (DataGridViewRow row in dgvProductores.Rows)
            {
                if (row.DataBoundItem is ProductorDisplay d && d.ProductorId == productorId)
                {
                    row.Selected = true;
                    dgvProductores.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
            _suppressEvents = false;
        }

        private void DgvProductores_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressEvents || dgvProductores.SelectedRows.Count == 0) return;
            if (dgvProductores.SelectedRows[0].DataBoundItem is ProductorDisplay display)
            {
                _productorSeleccionado = _todosProductores.FirstOrDefault(p => p.ProductorId == display.ProductorId);
                if (_productorSeleccionado != null)
                {
                    _suppressEvents = true;
                    cboCodigoProductor.Text = _productorSeleccionado.Codigo;
                    _suppressEvents = false;
                }
            }
        }

        private void DgvProductores_CellDoubleClick(object sender, DataGridViewCellEventArgs e) { }

        private void CboCodigoProductor_TextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            ActualizarDgvProductores(cboCodigoProductor.Text.Trim());
        }

        private void CboCodigoProductor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (cboCodigoProductor.SelectedValue is int id && id > 0)
            {
                var prod = _todosProductores.FirstOrDefault(p => p.ProductorId == id);
                if (prod != null)
                {
                    _productorSeleccionado = prod;
                    SeleccionarFilaEnDgv(id);
                }
            }
        }

        private void CboCodigoProductor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnBuscarProductor_Click(sender, e);
            }
        }

        private void BtnBuscarProductor_Click(object sender, EventArgs e)
        {
            string texto = cboCodigoProductor.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto)) return;
            var prod = _todosProductores.FirstOrDefault(p =>
                p.Codigo.Equals(texto, StringComparison.OrdinalIgnoreCase) ||
                (p.Nombre + " " + p.Apellido).Contains(texto, StringComparison.OrdinalIgnoreCase));
            if (prod != null)
            {
                _productorSeleccionado = prod;
                SeleccionarFilaEnDgv(prod.ProductorId);
            }
            else if (MessageBox.Show("No encontrado. ¿Desea agregar uno nuevo?", "Productor", MessageBoxButtons.YesNo) == DialogResult.Yes)
                AbrirFormularioAgregarProductor();
        }

        private void BtnAgregarProductor_Click(object sender, EventArgs e) => AbrirFormularioAgregarProductor();

        private async void AbrirFormularioAgregarProductor()
        {
            using (var frm = new ProductorDetalleForm())
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    await CargarProductoresAsync();
                    if (!string.IsNullOrEmpty(frm.CodigoGenerado))
                    {
                        var nuevo = _todosProductores.FirstOrDefault(p => p.Codigo == frm.CodigoGenerado);
                        if (nuevo != null)
                        {
                            _productorSeleccionado = nuevo;
                            SeleccionarFilaEnDgv(nuevo.ProductorId);
                        }
                    }
                }
        }

        // ── Guardado ─────────────────────────────────────────────────
        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;
            try
            {
                var nuevaEntrega = new Entrega
                {
                    NumeroEntrega = txtNumeroEntrega.Text.Trim(),
                    FechaEntrega = DateOnly.FromDateTime(dtpFechaEntrega.Value),
                    ProductorId = _productorSeleccionado.ProductorId,
                    ProductoId = (int)cboProducto.SelectedValue,
                    SubProductoId = cboSubProducto.SelectedValue is int sid && sid > 0 ? sid : (int?)null,
                    EstadoEntregaId = (int)cboEstadoEntrega.SelectedValue,
                    Placa = txtPlaca.Text.Trim(),
                    NombreConductor = txtNombreConductor.Text.Trim(),
                    Kilos = decimal.Parse(txtKilos.Text),
                    Cajas = int.Parse(txtCajas.Text),
                    Sacos = int.Parse(txtSacos.Text),
                    KilosSecos = string.IsNullOrWhiteSpace(txtKilosSecos.Text) ? null : decimal.Parse(txtKilosSecos.Text),
                    Calle = cboCalle.Text.Trim(),
                    Zona = cboFila.Text.Trim(),
                    Seccion = cboPosicion.Text.Trim(),
                    Observaciones = txtObservaciones.Text.Trim()
                };

                bool guardado = await _entregaService.Guardar(nuevaEntrega);
                if (!guardado)
                {
                    MessageBox.Show("No se pudo guardar la entrega.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Entrega guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CargarUbicacionCombosAsync();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (_productorSeleccionado == null) { MessageBox.Show("Seleccione un productor."); cboCodigoProductor.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtNumeroEntrega.Text)) { MessageBox.Show("El número de entrega es obligatorio."); return false; }
            if (cboProducto.SelectedValue == null || (int)cboProducto.SelectedValue == 0) { MessageBox.Show("Seleccione un producto."); cboProducto.Focus(); return false; }
            if (cboEstadoEntrega.SelectedValue == null || (int)cboEstadoEntrega.SelectedValue == 0) { MessageBox.Show("Seleccione un estado."); cboEstadoEntrega.Focus(); return false; }
            if (!decimal.TryParse(txtKilos.Text, out _)) { MessageBox.Show("Kilos inválido."); txtKilos.Focus(); return false; }
            if (!int.TryParse(txtCajas.Text, out _)) { MessageBox.Show("Cajas inválido."); txtCajas.Focus(); return false; }
            if (!int.TryParse(txtSacos.Text, out _)) { MessageBox.Show("Sacos inválido."); txtSacos.Focus(); return false; }
            if (!string.IsNullOrWhiteSpace(txtKilosSecos.Text) && !decimal.TryParse(txtKilosSecos.Text, out _)) { MessageBox.Show("Kilos secos inválido."); txtKilosSecos.Focus(); return false; }
            return true;
        }

        // ── Botones inferiores ───────────────────────────────────────
        private void BtnCancelar_Click(object sender, EventArgs e) => Close();

        private void BtnLimpiar_Click(object sender, EventArgs e) => LimpiarFormulario();

        private async void LimpiarFormulario()
        {
            _productorSeleccionado = null;
            _suppressEvents = true;
            cboCodigoProductor.SelectedIndex = 0;
            _suppressEvents = false;
            ActualizarDgvProductores(string.Empty);

            await CargarSiguienteNumeroEntregaAsync();

            dtpFechaEntrega.Value = DateTime.Today;
            cboProducto.SelectedIndex = 0;
            cboSubProducto.DataSource = null;
            cboEstadoEntrega.SelectedIndex = 0;

            txtPlaca.Clear();
            txtNombreConductor.Clear();
            txtKilos.Clear();
            txtCajas.Clear();
            txtSacos.Clear();
            txtKilosSecos.Clear();

            cboCalle.Text = string.Empty;
            cboFila.Text = string.Empty;
            cboPosicion.Text = string.Empty;
            txtObservaciones.Clear();
        }
    }
}