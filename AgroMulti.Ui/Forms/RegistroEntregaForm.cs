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

        // ── Prefijo fijo ───────────────────────────────────────────────
        private const string Prefijo = "PROD-";
        private const int MaxDigitos = 5;

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
            LlenarUbicacionCombos();
            await CargarSiguienteNumeroEntregaAsync();
        }

        // ── Inicialización de eventos ──────────────────────────────────
        private void ConfigurarFormulario()
        {
            // Combo productor: se puede escribir libremente
            cboCodigoProductor.AutoCompleteMode = AutoCompleteMode.None;
            cboCodigoProductor.AutoCompleteSource = AutoCompleteSource.None;
            cboCodigoProductor.DropDownStyle = ComboBoxStyle.DropDown;

            // Al hacer clic o al recibir el foco, selecciona solo la parte después del prefijo
            cboCodigoProductor.Enter += (s, e) => SeleccionarParteNumerica();
            cboCodigoProductor.Click += (s, e) => SeleccionarParteNumerica();

            cboCodigoProductor.TextChanged += CboCodigoProductor_TextChanged;
            cboCodigoProductor.KeyDown += CboCodigoProductor_KeyDown;
            cboCodigoProductor.SelectionChangeCommitted += CboCodigoProductor_SelectionChangeCommitted;

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

            cboCalle.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFila.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPosicion.DropDownStyle = ComboBoxStyle.DropDownList;

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
                cboCodigoProductor.Items.Clear();
                foreach (var p in _todosProductores)
                    cboCodigoProductor.Items.Add(p.Codigo);

                cboCodigoProductor.Text = Prefijo;
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

        private void LlenarUbicacionCombos()
        {
            cboCalle.Items.Clear();
            cboCalle.Items.AddRange(new object[] { "Pasillo A", "Pasillo B", "Pasillo C", "Pasillo D" });
            if (cboCalle.Items.Count > 0) cboCalle.SelectedIndex = 0;

            cboFila.Items.Clear();
            cboFila.Items.AddRange(new object[] { "Estante 1", "Estante 2", "Estante 3", "Estante 4", "Estante 5", "Estante 6" });
            if (cboFila.Items.Count > 0) cboFila.SelectedIndex = 0;

            cboPosicion.Items.Clear();
            cboPosicion.Items.AddRange(new object[] { "Planta baja", "Piso 1", "Piso 2", "Piso 3" });
            if (cboPosicion.Items.Count > 0) cboPosicion.SelectedIndex = 0;
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

        /// <summary> Extrae solo dígitos, pone el prefijo y limita la longitud. </summary>
        private string SanitizarCodigo(string texto)
        {
            // Si no empieza con el prefijo, forzamos el prefijo
            if (!texto.StartsWith(Prefijo))
            {
                string digitos = new string(texto.Where(char.IsDigit).ToArray());
                if (digitos.Length > MaxDigitos) digitos = digitos.Substring(0, MaxDigitos);
                return Prefijo + digitos;
            }
            else
            {
                string sufijo = texto.Substring(Prefijo.Length);
                string digitos = new string(sufijo.Where(char.IsDigit).ToArray());
                if (digitos.Length > MaxDigitos) digitos = digitos.Substring(0, MaxDigitos);
                return Prefijo + digitos;
            }
        }

        /// <summary> Selecciona solo la parte después del prefijo. </summary>
        private void SeleccionarParteNumerica()
        {
            string current = cboCodigoProductor.Text;
            if (current.StartsWith(Prefijo))
            {
                cboCodigoProductor.Select(Prefijo.Length, current.Length - Prefijo.Length);
            }
            else
            {
                // Si no tiene prefijo, lo forzamos (raro pero seguro)
                cboCodigoProductor.Text = Prefijo;
                cboCodigoProductor.Select(Prefijo.Length, 0);
            }
        }

        private void ActualizarDgvProductores(string filtro)
        {
            var query = _todosProductores.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(filtro) && filtro != Prefijo)
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
                    SeleccionarParteNumerica();   // deja seleccionada la parte numérica
                    _suppressEvents = false;
                }
            }
        }

        private void DgvProductores_CellDoubleClick(object sender, DataGridViewCellEventArgs e) { }

        // ── Manejo del Combo ─────────────────────────────────────────
        private void CboCodigoProductor_TextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            string current = cboCodigoProductor.Text;
            string sanitized = SanitizarCodigo(current);

            if (sanitized != current)
            {
                _suppressEvents = true;
                cboCodigoProductor.Text = sanitized;
                // Colocar el cursor al final de la parte numérica
                cboCodigoProductor.Select(sanitized.Length, 0);
                _suppressEvents = false;
            }

            ActualizarDgvProductores(sanitized);
        }

        private void CboCodigoProductor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnBuscarProductor_Click(sender, e);
            }
        }

        private void CboCodigoProductor_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cboCodigoProductor.SelectedItem != null)
            {
                string codigoSeleccionado = cboCodigoProductor.SelectedItem.ToString();
                var prod = _todosProductores.FirstOrDefault(p => p.Codigo.Equals(codigoSeleccionado, StringComparison.OrdinalIgnoreCase));
                if (prod != null)
                {
                    _productorSeleccionado = prod;
                    SeleccionarFilaEnDgv(prod.ProductorId);
                    _suppressEvents = true;
                    cboCodigoProductor.Text = prod.Codigo;
                    SeleccionarParteNumerica();
                    _suppressEvents = false;
                }
            }
        }

        private void BtnBuscarProductor_Click(object sender, EventArgs e)
        {
            string texto = cboCodigoProductor.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto) || texto == Prefijo) return;

            var prod = _todosProductores.FirstOrDefault(p =>
                p.Codigo.Equals(texto, StringComparison.OrdinalIgnoreCase) ||
                (p.Nombre + " " + p.Apellido).Contains(texto, StringComparison.OrdinalIgnoreCase));
            if (prod != null)
            {
                _productorSeleccionado = prod;
                SeleccionarFilaEnDgv(prod.ProductorId);
                _suppressEvents = true;
                cboCodigoProductor.Text = prod.Codigo;
                SeleccionarParteNumerica();
                _suppressEvents = false;
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
                            _suppressEvents = true;
                            cboCodigoProductor.Text = nuevo.Codigo;
                            SeleccionarParteNumerica();
                            _suppressEvents = false;
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
                    Pasillo = cboCalle.Text.Trim(),
                    NumeroAnaquel = cboFila.Text.Trim(),
                    Piso = cboPosicion.Text.Trim(),
                    Observaciones = txtObservaciones.Text.Trim()
                };

                bool guardado = await _entregaService.Guardar(nuevaEntrega);
                if (!guardado)
                {
                    MessageBox.Show("No se pudo guardar la entrega.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Entrega guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void BtnCancelar_Click(object sender, EventArgs e) => Close();

        private void BtnLimpiar_Click(object sender, EventArgs e) => LimpiarFormulario();

        private async void LimpiarFormulario()
        {
            _productorSeleccionado = null;
            _suppressEvents = true;
            cboCodigoProductor.Text = Prefijo;
            SeleccionarParteNumerica();
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

            if (cboCalle.Items.Count > 0) cboCalle.SelectedIndex = 0;
            if (cboFila.Items.Count > 0) cboFila.SelectedIndex = 0;
            if (cboPosicion.Items.Count > 0) cboPosicion.SelectedIndex = 0;

            txtObservaciones.Clear();
        }

        
        private void layoutObservaciones_Paint(object sender, PaintEventArgs e) { }
    }
}