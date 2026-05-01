using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CentroFermentacionSecado
{
    public partial class RegistroEntregaForm : Form
    {
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
            ConfigurarFormulario();
            ConfigurarDgvProductores();
            CargarCombos();
            CargarProductores();
            CargarUbicacionCombos();
            CargarSiguienteNumeroEntrega();   // ← Asigna el número automático
        }

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
        private void CargarProductores()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    _todosProductores = db.Productors.OrderBy(p => p.Codigo).ToList();
                }
                _suppressEvents = true;
                var vacio = new { ProductorId = 0, Codigo = "", NombreCompleto = "" };
                var lista = _todosProductores.Select(p => new { p.ProductorId, p.Codigo, NombreCompleto = p.Nombre + " " + p.Apellido }).ToList();
                var final = new List<dynamic> { vacio };
                final.AddRange(lista.Cast<dynamic>());
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

        private void CargarCombos()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    var productos = db.Productos.OrderBy(p => p.Nombre).ToList();
                    productos.Insert(0, new Producto { ProductoId = 0, Nombre = " Seleccione " });
                    cboProducto.DataSource = productos;
                    cboProducto.DisplayMember = "Nombre";
                    cboProducto.ValueMember = "ProductoId";

                    var estados = db.EstadoEntregas.OrderBy(e => e.Nombre).ToList();
                    estados.Insert(0, new EstadoEntrega { EstadoEntregaId = 0, Nombre = " Seleccione " });
                    cboEstadoEntrega.DataSource = estados;
                    cboEstadoEntrega.DisplayMember = "Nombre";
                    cboEstadoEntrega.ValueMember = "EstadoEntregaId";

                    cboSubProducto.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar listas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarUbicacionCombos()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    var pasillos = db.Entregas.Where(e => e.Calle != null && e.Calle != "").Select(e => e.Calle).Distinct().OrderBy(x => x).ToList();
                    var tuneles = db.Entregas.Where(e => e.Zona != null && e.Zona != "").Select(e => e.Zona).Distinct().OrderBy(x => x).ToList();
                    var modulos = db.Entregas.Where(e => e.Seccion != null && e.Seccion != "").Select(e => e.Seccion).Distinct().OrderBy(x => x).ToList();

                    cboCalle.Items.Clear(); cboCalle.Items.AddRange(pasillos.ToArray());
                    cboFila.Items.Clear(); cboFila.Items.AddRange(tuneles.ToArray());
                    cboPosicion.Items.Clear(); cboPosicion.Items.AddRange(modulos.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar ubicaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSiguienteNumeroEntrega()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    var ultimoNumero = db.Entregas
                        .Where(e => e.NumeroEntrega.StartsWith("E-"))
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
            }
            catch (Exception ex)
            {
                txtNumeroEntrega.Text = "E-00001";
            }
        }

        // ── Subproductos ──────────────────────────────────────────────
        private void CboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducto.SelectedValue is int idProducto && idProducto > 0)
            {
                try
                {
                    using (var db = new AgroMultiContext())
                    {
                        var subs = db.SubProductos.Where(s => s.ProductoId == idProducto).OrderBy(s => s.Nombre).ToList();
                        subs.Insert(0, new SubProducto { SubProductoId = 0, Nombre = " Seleccione " });
                        cboSubProducto.DataSource = subs;
                        cboSubProducto.DisplayMember = "Nombre";
                        cboSubProducto.ValueMember = "SubProductoId";
                    }
                }
                catch (Exception ex) { /* ignorar */ }
            }
            else
                cboSubProducto.DataSource = null;
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
            var prod = _todosProductores.FirstOrDefault(p => p.Codigo.Equals(texto, StringComparison.OrdinalIgnoreCase)
                                                         || (p.Nombre + " " + p.Apellido).Contains(texto, StringComparison.OrdinalIgnoreCase));
            if (prod != null) { _productorSeleccionado = prod; SeleccionarFilaEnDgv(prod.ProductorId); }
            else if (MessageBox.Show("No encontrado. ¿Desea agregar uno nuevo?", "Productor", MessageBoxButtons.YesNo) == DialogResult.Yes)
                AbrirFormularioAgregarProductor();
        }

        private void BtnAgregarProductor_Click(object sender, EventArgs e) => AbrirFormularioAgregarProductor();

        private void AbrirFormularioAgregarProductor()
        {
            using (var frm = new ProductorDetalleForm())
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarProductores();
                    if (!string.IsNullOrEmpty(frm.CodigoGenerado))
                    {
                        var nuevo = _todosProductores.FirstOrDefault(p => p.Codigo == frm.CodigoGenerado);
                        if (nuevo != null) { _productorSeleccionado = nuevo; SeleccionarFilaEnDgv(nuevo.ProductorId); }
                    }
                }
        }

        // ── Guardado ─────────────────────────────────────────────────
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;
            try
            {
                using (var db = new AgroMultiContext())
                {
                    db.Entregas.Add(new Entrega
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
                    });
                    db.SaveChanges();
                }
                MessageBox.Show("Entrega guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarUbicacionCombos();
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

        private void LimpiarFormulario()
        {
            _productorSeleccionado = null;
            _suppressEvents = true;
            cboCodigoProductor.SelectedIndex = 0;
            _suppressEvents = false;
            ActualizarDgvProductores(string.Empty);

            // Recargar número de entrega automático (siguiente disponible)
            CargarSiguienteNumeroEntrega();

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