using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CentroFermentacionSecado
{
    public partial class ProductorDetalleForm : Form
    {
        // Indica si estamos editando (true) o creando (false)
        private readonly bool _modoEdicion;
        private readonly Productor _productorExistente;

        // Propiedad pública para que otros formularios obtengan el código generado
        public string CodigoGenerado { get; private set; }

        /// <summary>
        /// Constructor para AGREGAR un nuevo productor.
        /// </summary>
        public ProductorDetalleForm()
        {
            InitializeComponent();
            _modoEdicion = false;
            _productorExistente = null;
            Configurar();
        }

        /// <summary>
        /// Constructor para EDITAR un productor existente.
        /// </summary>
        /// <param name="productor">Productor a modificar.</param>
        public ProductorDetalleForm(Productor productor)
        {
            InitializeComponent();
            _modoEdicion = true;
            _productorExistente = productor ?? throw new ArgumentNullException(nameof(productor));
            Configurar();
            CargarDatosProductor();
        }

        private void Configurar()
        {
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            // Cambiar título según el modo
            if (_modoEdicion)
            {
                lblTitulo.Text = "Editar productor";
                lblSubtitulo.Text = "Modifique los datos del productor";
                Text = "Editar productor";
            }

            // Cursores
            btnGuardar.Cursor = Cursors.Hand;
            btnCancelar.Cursor = Cursors.Hand;

            // Foco inicial
            txtCodigo.Focus();
        }

        private void CargarDatosProductor()
        {
            if (_productorExistente == null) return;

            txtCodigo.Text = _productorExistente.Codigo;
            txtNombre.Text = _productorExistente.Nombre;
            txtApellido.Text = _productorExistente.Apellido;
            txtTelefono.Text = _productorExistente.Telefono ?? string.Empty;
            txtDireccion.Text = _productorExistente.Direccion ?? string.Empty;

            // En modo edición, ya tenemos un código generado
            CodigoGenerado = _productorExistente.Codigo;
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                using (var db = new AgroMultiContext())
                {
                    if (_modoEdicion)
                    {
                        // ── EDITAR ──────────────────────────────────────────
                        // Adjuntamos el productor existente y actualizamos sus campos
                        db.Productors.Attach(_productorExistente);

                        _productorExistente.Codigo = txtCodigo.Text.Trim();
                        _productorExistente.Nombre = txtNombre.Text.Trim();
                        _productorExistente.Apellido = txtApellido.Text.Trim();
                        _productorExistente.Telefono = txtTelefono.Text.Trim();
                        _productorExistente.Direccion = txtDireccion.Text.Trim();

                        db.SaveChanges();

                        CodigoGenerado = _productorExistente.Codigo;
                    }
                    else
                    {
                        // ── CREAR ──────────────────────────────────────────
                        string codigo = txtCodigo.Text.Trim();

                        // Verificar que el código no exista ya
                        bool existe = db.Productors.Any(p => p.Codigo == codigo);
                        if (existe)
                        {
                            MessageBox.Show("Ya existe un productor con ese código. Ingrese uno diferente.",
                                "Código duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtCodigo.Focus();
                            return;
                        }

                        var nuevoProductor = new Productor
                        {
                            Codigo = codigo,
                            Nombre = txtNombre.Text.Trim(),
                            Apellido = txtApellido.Text.Trim(),
                            Telefono = txtTelefono.Text.Trim(),
                            Direccion = txtDireccion.Text.Trim()
                        };

                        db.Productors.Add(nuevoProductor);
                        db.SaveChanges();

                        CodigoGenerado = nuevoProductor.Codigo;
                    }
                }

                MessageBox.Show("Productor guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el productor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            // Código
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El código del productor es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return false;
            }

            // Nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del productor es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            // Apellido
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido del productor es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            return true;
        }
    }
}
