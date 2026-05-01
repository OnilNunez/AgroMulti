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
        private bool _suppressEvents = false;   // evita loops entre combo ↔ DGV

        // ── ViewModel para el DGV (columnas explícitas) ─────────────────
        private class ProductorDisplay
        {
            public int ProductorId { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
        }

        // ── Constructor ────────────────────────────────────────────────
        public RegistroEntregaForm()
        {
            InitializeComponent();
            ConfigurarFormulario();
            ConfigurarDgvProductores();
            CargarCombos();
            CargarProductores();
            CargarUbicacionCombos();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN INICIAL
        // ═══════════════════════════════════════════════════════════════

        private void ConfigurarFormulario()
        {
            // Eventos del productor
            cboCodigoProductor.SelectedIndexChanged += CboCodigoProductor_SelectedIndexChanged;
            cboCodigoProductor.TextChanged += CboCodigoProductor_TextChanged;
            cboCodigoProductor.KeyDown += CboCodigoProductor_KeyDown;
            btnBuscarProductor.Click += BtnBuscarProductor_Click;
            btnAgregarProductor.Click += BtnAgregarProductor_Click;

            // DGV
            dgvProductores.SelectionChanged += DgvProductores_SelectionChanged;
            dgvProductores.CellDoubleClick += DgvProductores_CellDoubleClick;

            // Botones inferiores
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

        /// <summary>
        /// Define las columnas del DGV de productores con nombres y anchos.
        /// AutoGenerateColumns = false para control total.
        /// </summary>
        private void ConfigurarDgvProductores()
        {
            dgvProductores.AutoGenerateColumns = false;

            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colProductorId",
                DataPropertyName = "ProductorId",
                HeaderText = "ID",
                Visible = false
            });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCodigo",
                DataPropertyName = "Codigo",
                HeaderText = "Código",
                Width = 90,
                ReadOnly = true
            });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                DataPropertyName = "Nombre",
                HeaderText = "Nombre",
                Width = 140,
                ReadOnly = true
            });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colApellido",
                DataPropertyName = "Apellido",
                HeaderText = "Apellido",
                Width = 140,
                ReadOnly = true
            });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTelefono",
                DataPropertyName = "Telefono",
                HeaderText = "Teléfono",
                Width = 110,
                ReadOnly = true
            });
            dgvProductores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDireccion",
                DataPropertyName = "Direccion",
                HeaderText = "Dirección",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // CARGA DE DATOS
        // ═══════════════════════════════════════════════════════════════

        private void CargarProductores()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    _todosProductores = db.Productors
                        .OrderBy(p => p.Codigo)
                        .ToList();
                }

                // Poblar el ComboBox (dropdown) con opción vacía inicial
                _suppressEvents = true;

                var listaCombo = _todosProductores
                    .Select(p => new
                    {
                        p.ProductorId,
                        p.Codigo,
                        NombreCompleto = p.Nombre + " " + p.Apellido
                    })
                    .ToList<dynamic>();

                // Opción vacía al inicio
                var vacio = new { ProductorId = 0, Codigo = "", NombreCompleto = "" };
                var listaFinal = new List<dynamic> { vacio };
                listaFinal.AddRange(listaCombo);

                cboCodigoProductor.DataSource = listaFinal;
                cboCodigoProductor.DisplayMember = "Codigo";
                cboCodigoProductor.ValueMember = "ProductorId";
                cboCodigoProductor.SelectedIndex = 0;

                _suppressEvents = false;

                // Poblar el DGV con todos los productores
                ActualizarDgvProductores(string.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productores: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarCombos()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    // Productos
                    var productos = db.Productos.OrderBy(p => p.Nombre).ToList();
                    productos.Insert(0, new Producto { ProductoId = 0, Nombre = " Seleccione " });
                    cboProducto.DataSource = productos;
                    cboProducto.DisplayMember = "Nombre";
                    cboProducto.ValueMember = "ProductoId";

                    // Estados de entrega
                    var estados = db.EstadoEntregas.OrderBy(e => e.Nombre).ToList();
                    estados.Insert(0, new EstadoEntrega { EstadoEntregaId = 0, Nombre = " Seleccione " });
                    cboEstadoEntrega.DataSource = estados;
                    cboEstadoEntrega.DisplayMember = "Nombre";
                    cboEstadoEntrega.ValueMember = "EstadoEntregaId";

                    // Subproducto inicial vacío
                    cboSubProducto.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar listas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carga valores distintos existentes en la BD para los ComboBox de ubicación.
        /// Al ser DropDown (editable) el usuario también puede escribir valores nuevos.
        /// </summary>
        private void CargarUbicacionCombos()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    // Pasillo (campo Calle en el modelo)
                    var pasillos = db.Entregas
                        .Where(e => e.Calle != null && e.Calle != "")
                        .Select(e => e.Calle)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    cboCalle.Items.Clear();
                    cboCalle.Items.AddRange(pasillos.ToArray());

                    // Túnel (campo Zona en el modelo)
                    var tuneles = db.Entregas
                        .Where(e => e.Zona != null && e.Zona != "")
                        .Select(e => e.Zona)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    cboFila.Items.Clear();
                    cboFila.Items.AddRange(tuneles.ToArray());

                    // Módulo (campo Seccion en el modelo)
                    var modulos = db.Entregas
                        .Where(e => e.Seccion != null && e.Seccion != "")
                        .Select(e => e.Seccion)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    cboPosicion.Items.Clear();
                    cboPosicion.Items.AddRange(modulos.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar ubicaciones: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducto.SelectedValue is int idProducto && idProducto > 0)
                CargarSubProductos(idProducto);
            else
                cboSubProducto.DataSource = null;
        }

        private void CargarSubProductos(int productoId)
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    var subs = db.SubProductos
                        .Where(s => s.ProductoId == productoId)
                        .OrderBy(s => s.Nombre)
                        .ToList();

                    subs.Insert(0, new SubProducto { SubProductoId = 0, Nombre = " Seleccione " });

                    cboSubProducto.DataSource = subs;
                    cboSubProducto.DisplayMember = "Nombre";
                    cboSubProducto.ValueMember = "SubProductoId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar subproductos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // DGV — filtrado y selección
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Filtra el DGV en memoria según el texto del combo.
        /// </summary>
        private void ActualizarDgvProductores(string filtro)
        {
            IEnumerable<Productor> query = _todosProductores;

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(p =>
                    (p.Codigo ?? "").Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    ((p.Nombre ?? "") + " " + (p.Apellido ?? ""))
                        .Contains(filtro, StringComparison.OrdinalIgnoreCase));
            }

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

        /// <summary>
        /// Resalta en el DGV la fila del productor con el ID indicado.
        /// </summary>
        private void SeleccionarFilaEnDgv(int productorId)
        {
            _suppressEvents = true;
            try
            {
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
            }
            finally
            {
                _suppressEvents = false;
            }
        }

        /// <summary>
        /// Selección de fila en el DGV: actualiza productor seleccionado y el combo.
        /// </summary>
        private void DgvProductores_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (dgvProductores.SelectedRows.Count == 0) return;

            if (dgvProductores.SelectedRows[0].DataBoundItem is ProductorDisplay display)
            {
                var productor = _todosProductores
                    .FirstOrDefault(p => p.ProductorId == display.ProductorId);

                if (productor == null) return;

                _productorSeleccionado = productor;

                // Sincronizar el texto del combo sin disparar filtro
                _suppressEvents = true;
                cboCodigoProductor.Text = productor.Codigo;
                _suppressEvents = false;
            }
        }

        /// <summary>
        /// Doble clic en una fila confirma la selección visualmente (opcional).
        /// </summary>
        private void DgvProductores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // La selección ya ocurrió en SelectionChanged
        }

        // ═══════════════════════════════════════════════════════════════
        // EVENTOS DEL COMBO DE PRODUCTOR
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Filtrado en tiempo real mientras el usuario escribe en el combo.
        /// </summary>
        private void CboCodigoProductor_TextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            ActualizarDgvProductores(cboCodigoProductor.Text.Trim());
        }

        /// <summary>
        /// Cuando el usuario elige un elemento del desplegable,
        /// se selecciona automáticamente en el DGV.
        /// </summary>
        private void CboCodigoProductor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (!(cboCodigoProductor.SelectedValue is int id) || id == 0) return;

            var productor = _todosProductores.FirstOrDefault(p => p.ProductorId == id);
            if (productor == null) return;

            _productorSeleccionado = productor;
            SeleccionarFilaEnDgv(id);
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
            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Ingrese un código o nombre de productor.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BuscarYMostrarProductor(texto);
        }

        private void BuscarYMostrarProductor(string texto)
        {
            // Búsqueda en memoria (ya cargado)
            var productor = _todosProductores.FirstOrDefault(p =>
                string.Equals(p.Codigo, texto, StringComparison.OrdinalIgnoreCase) ||
                ((p.Nombre ?? "") + " " + (p.Apellido ?? ""))
                    .Contains(texto, StringComparison.OrdinalIgnoreCase));

            if (productor != null)
            {
                _productorSeleccionado = productor;
                SeleccionarFilaEnDgv(productor.ProductorId);
            }
            else
            {
                _productorSeleccionado = null;

                DialogResult respuesta = MessageBox.Show(
                    "El código o nombre ingresado no existe. ¿Desea agregar un nuevo productor?",
                    "Productor no encontrado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                    AbrirFormularioAgregarProductor();
            }
        }

        private void BtnAgregarProductor_Click(object sender, EventArgs e)
        {
            AbrirFormularioAgregarProductor();
        }

        private void AbrirFormularioAgregarProductor()
        {
            try
            {
                using (var frm = new ProductorDetalleForm())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // Recargar toda la lista para incluir el nuevo productor
                        CargarProductores();

                        if (!string.IsNullOrEmpty(frm.CodigoGenerado))
                        {
                            // Buscar el nuevo productor y seleccionarlo
                            var nuevo = _todosProductores
                                .FirstOrDefault(p => p.Codigo == frm.CodigoGenerado);

                            if (nuevo != null)
                            {
                                _productorSeleccionado = nuevo;
                                ActualizarDgvProductores(string.Empty);
                                SeleccionarFilaEnDgv(nuevo.ProductorId);

                                _suppressEvents = true;
                                cboCodigoProductor.Text = nuevo.Codigo;
                                _suppressEvents = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir formulario de productor: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // GUARDADO
        // ═══════════════════════════════════════════════════════════════

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                using (var db = new AgroMultiContext())
                {
                    var nuevaEntrega = new Entrega
                    {
                        NumeroEntrega = txtNumeroEntrega.Text.Trim(),
                        FechaEntrega = DateOnly.FromDateTime(dtpFechaEntrega.Value),
                        ProductorId = _productorSeleccionado.ProductorId,
                        ProductoId = (int)cboProducto.SelectedValue,
                        SubProductoId = cboSubProducto.SelectedValue is int subId && subId > 0
                                            ? subId : (int?)null,
                        EstadoEntregaId = (int)cboEstadoEntrega.SelectedValue,
                        Placa = txtPlaca.Text.Trim(),
                        NombreConductor = txtNombreConductor.Text.Trim(),
                        Kilos = decimal.Parse(txtKilos.Text),
                        Cajas = int.Parse(txtCajas.Text),
                        Sacos = int.Parse(txtSacos.Text),
                        KilosSecos = string.IsNullOrWhiteSpace(txtKilosSecos.Text)
                                            ? null
                                            : decimal.Parse(txtKilosSecos.Text),
                        // Ubicación almacén — los ComboBox son editables
                        Calle = cboCalle.Text.Trim(),
                        Zona = cboFila.Text.Trim(),
                        Seccion = cboPosicion.Text.Trim(),
                        Observaciones = txtObservaciones.Text.Trim()
                    };

                    db.Entregas.Add(nuevaEntrega);
                    db.SaveChanges();
                }

                MessageBox.Show("Entrega guardada correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar opciones de ubicación por si se añadieron valores nuevos
                CargarUbicacionCombos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la entrega: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (_productorSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un productor válido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCodigoProductor.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNumeroEntrega.Text))
            {
                MessageBox.Show("Ingrese el número de entrega.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumeroEntrega.Focus();
                return false;
            }

            if (cboProducto.SelectedValue == null || (int)cboProducto.SelectedValue == 0)
            {
                MessageBox.Show("Seleccione un producto.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboProducto.Focus();
                return false;
            }

            if (cboEstadoEntrega.SelectedValue == null || (int)cboEstadoEntrega.SelectedValue == 0)
            {
                MessageBox.Show("Seleccione un estado.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboEstadoEntrega.Focus();
                return false;
            }

            if (!decimal.TryParse(txtKilos.Text, out _))
            {
                MessageBox.Show("Ingrese un valor numérico válido en Kilos.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKilos.Focus();
                return false;
            }

            if (!int.TryParse(txtCajas.Text, out _))
            {
                MessageBox.Show("Ingrese un número entero en Cajas.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCajas.Focus();
                return false;
            }

            if (!int.TryParse(txtSacos.Text, out _))
            {
                MessageBox.Show("Ingrese un número entero en Sacos.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSacos.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtKilosSecos.Text) &&
                !decimal.TryParse(txtKilosSecos.Text, out _))
            {
                MessageBox.Show("Ingrese un valor decimal válido en Kilos Secos.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKilosSecos.Focus();
                return false;
            }

            return true;
        }

        // ═══════════════════════════════════════════════════════════════
        // BOTONES INFERIORES
        // ═══════════════════════════════════════════════════════════════

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            // Productor
            _productorSeleccionado = null;

            _suppressEvents = true;
            cboCodigoProductor.SelectedIndex = 0;
            _suppressEvents = false;

            ActualizarDgvProductores(string.Empty);

            // Entrega
            txtNumeroEntrega.Clear();
            dtpFechaEntrega.Value = DateTime.Today;
            cboProducto.SelectedIndex = 0;
            cboSubProducto.DataSource = null;
            cboEstadoEntrega.SelectedIndex = 0;

            // Vehículo
            txtPlaca.Clear();
            txtNombreConductor.Clear();

            // Pesaje
            txtKilos.Clear();
            txtCajas.Clear();
            txtSacos.Clear();
            txtKilosSecos.Clear();

            // Ubicación almacén (ComboBoxes editables)
            cboCalle.Text = string.Empty;
            cboFila.Text = string.Empty;
            cboPosicion.Text = string.Empty;

            // Observaciones
            txtObservaciones.Clear();
        }
    }
}
