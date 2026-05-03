using AgroMulti;
using AgroMulti.Data.Models;
using AgroMulti.Ui.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class ProductorDetalleForm : Form
    {
        // ── Servicio inyectado ───────────────────────────────────────
        private readonly ProductorService _productorService;

        private readonly bool _modoEdicion;
        private readonly Productor _productorExistente;

        public string CodigoGenerado { get; private set; }

        // ── Constructores ────────────────────────────────────────────

        /// <summary>
        /// Constructor para AGREGAR un nuevo productor.
        /// </summary>
        public ProductorDetalleForm()
        {
            InitializeComponent();

            _productorService = Program.ServiceProvider.GetRequiredService<ProductorService>();

            _modoEdicion = false;
            _productorExistente = null;
            Configurar();
            _ = CargarSiguienteCodigoAsync();   
        }

        /// <summary>
        /// Constructor para EDITAR un productor existente.
        /// </summary>
        public ProductorDetalleForm(Productor productor)
        {
            InitializeComponent();

            _productorService = Program.ServiceProvider.GetRequiredService<ProductorService>();

            _modoEdicion = true;
            _productorExistente = productor ?? throw new ArgumentNullException(nameof(productor));
            Configurar();
            CargarDatosProductor();
        }

        // ── Configuración de eventos y apariencia ────────────────────

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
            txtNombre.Focus();
        }

        // ── Obtención del siguiente código ────────────────────────────

        private async Task CargarSiguienteCodigoAsync()
        {
            try
            {
                
                var lista = await _productorService.GetList(p => p.Codigo.StartsWith("PROD-"));
                var ultimoCodigo = lista
                    .Select(p => p.Codigo)
                    .OrderByDescending(c => c)
                    .FirstOrDefault();

                int siguienteNumero = 1;
                if (ultimoCodigo != null)
                {
                    string numeroStr = ultimoCodigo.Replace("PROD-", "").TrimStart('0');
                    if (int.TryParse(numeroStr, out int ultimoNumero))
                        siguienteNumero = ultimoNumero + 1;
                }
                txtCodigo.Text = $"PROD-{siguienteNumero:D5}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular el siguiente código: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodigo.Text = "PROD-00001";
            }
        }

        // ── Carga de datos en modo edición ────────────────────────────

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

        // ── Eventos de botones ────────────────────────────────────────

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                bool exito;

                if (_modoEdicion)
                {
                    // ── EDITAR ──────────────────────────────────────
                    _productorExistente.Nombre = txtNombre.Text.Trim();
                    _productorExistente.Apellido = txtApellido.Text.Trim();
                    _productorExistente.Telefono = txtTelefono.Text.Trim();
                    _productorExistente.Direccion = txtDireccion.Text.Trim();

                    exito = await _productorService.Guardar(_productorExistente);
                    if (exito)
                        CodigoGenerado = _productorExistente.Codigo;
                }
                else
                {
                    // ── CREAR ──────────────────────────────────────
                    string codigoNuevo = txtCodigo.Text.Trim();

                    // Verificar que no exista un productor con ese código
                    var existentes = await _productorService.GetList(p => p.Codigo == codigoNuevo);
                    if (existentes.Any())
                    {
                        MessageBox.Show($"El código {codigoNuevo} ya existe. Intente de nuevo.",
                            "Código duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    exito = await _productorService.Guardar(nuevoProductor);
                    if (exito)
                        CodigoGenerado = nuevoProductor.Codigo;
                }

                if (!exito)
                {
                    MessageBox.Show("No se pudo guardar el productor.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
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

        // ── Validación ─────────────────────────────────────────────────

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