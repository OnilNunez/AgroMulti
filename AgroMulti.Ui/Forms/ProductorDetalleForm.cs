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
        private readonly bool _modoEdicion;
        private readonly Productor _productorExistente;

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
            CargarSiguienteCodigo();   // consulta el último código en BD
        }

        /// <summary>
        /// Constructor para EDITAR un productor existente.
        /// </summary>
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

            if (_modoEdicion)
            {
                lblTitulo.Text = "Editar productor";
                lblSubtitulo.Text = "Modifique los datos del productor";
                Text = "Editar productor";
            }

            btnGuardar.Cursor = Cursors.Hand;
            btnCancelar.Cursor = Cursors.Hand;

            // El foco inicia en el primer campo editable
            txtNombre.Focus();
        }

        /// <summary>
        /// Obtiene el siguiente código consultando el mayor código existente en la tabla Productor.
        /// </summary>
        private void CargarSiguienteCodigo()
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    // Obtener todos los códigos que empiecen con "PROD-" y extraer el número
                    var ultimoCodigo = db.Productors
                        .Where(p => p.Codigo.StartsWith("PROD-"))
                        .OrderByDescending(p => p.Codigo)  // orden alfabético, que coincide con numérico gracias al padding
                        .Select(p => p.Codigo)
                        .FirstOrDefault();

                    int siguienteNumero = 1;
                    if (ultimoCodigo != null)
                    {
                        // Extraer la parte numérica
                        string numeroStr = ultimoCodigo.Replace("PROD-", "").TrimStart('0');
                        if (int.TryParse(numeroStr, out int ultimoNumero))
                        {
                            siguienteNumero = ultimoNumero + 1;
                        }
                    }

                    txtCodigo.Text = $"PROD-{siguienteNumero:D5}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular el siguiente código: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodigo.Text = "PROD-00001"; // valor por defecto en caso de error
            }
        }

        private void CargarDatosProductor()
        {
            if (_productorExistente == null) return;

            txtCodigo.Text = _productorExistente.Codigo;
            txtNombre.Text = _productorExistente.Nombre;
            txtApellido.Text = _productorExistente.Apellido;
            txtTelefono.Text = _productorExistente.Telefono ?? string.Empty;
            txtDireccion.Text = _productorExistente.Direccion ?? string.Empty;

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
                        db.Productors.Attach(_productorExistente);
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
                        string codigoNuevo = txtCodigo.Text.Trim();

                        // Verificar que no exista ya (protección extra)
                        bool existe = db.Productors.Any(p => p.Codigo == codigoNuevo);
                        if (existe)
                        {
                            MessageBox.Show($"El código {codigoNuevo} ya existe. Intente de nuevo.", "Código duplicado",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            // Podríamos recalcular el siguiente código, pero es un caso muy raro.
                            return;
                        }

                        var nuevoProductor = new Productor
                        {
                            Codigo = codigoNuevo,
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
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del productor es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

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
