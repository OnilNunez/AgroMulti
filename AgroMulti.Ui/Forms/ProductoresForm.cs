using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CentroFermentacionSecado
{
    public partial class ProductoresForm : Form
    {
        public ProductoresForm()
        {
            InitializeComponent();
            Configurar();
            CargarProductores();
        }

        private void Configurar()
        {
            btnAgregar.Click += BtnAgregar_Click;
            btnEditar.Click += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            txtBuscar.TextChanged += TxtBuscar_TextChanged;

            btnAgregar.Cursor = Cursors.Hand;
            btnEditar.Cursor = Cursors.Hand;
            btnEliminar.Cursor = Cursors.Hand;

            txtBuscar.BorderStyle = BorderStyle.FixedSingle;
        }

        // ── Carga de datos ────────────────────────────────────────────
        private void CargarProductores(string filtro = "")
        {
            try
            {
                using (var db = new AgroMultiContext())
                {
                    var query = db.Productors.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        string termino = filtro.Trim().ToLower();
                        query = query.Where(p =>
                            p.Codigo.ToLower().Contains(termino) ||
                            p.Nombre.ToLower().Contains(termino) ||
                            p.Apellido.ToLower().Contains(termino) ||
                            (p.Telefono != null && p.Telefono.ToLower().Contains(termino))
                        );
                    }

                    var productores = query
                        .OrderBy(p => p.Codigo)
                        .ToList();

                    dgvProductores.Rows.Clear();
                    foreach (var p in productores)
                    {
                        int rowIndex = dgvProductores.Rows.Add(
                            p.Codigo,
                            p.Nombre,
                            p.Apellido,
                            p.Telefono ?? ""
                        );
                        // Guardamos el objeto Productor en el Tag de la fila para editar/eliminar
                        dgvProductores.Rows[rowIndex].Tag = p;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productores: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarProductores(txtBuscar.Text);
        }

        // ── Operaciones CRUD ──────────────────────────────────────────
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // Asumimos que FormularioAgregarProductor se abrirá en modo "nuevo"
                using (var frm = new ProductorDetalleForm())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // Si el formulario de agregar devuelve OK, recargamos
                        CargarProductores(txtBuscar.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProductores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un productor para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var productor = dgvProductores.SelectedRows[0].Tag as Productor;
            if (productor == null) return;

            try
            {
                // Abrimos el mismo formulario en modo edición (asumimos que acepta un Productor)
                using (var frm = new ProductorDetalleForm(productor))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        CargarProductores(txtBuscar.Text);
                    }
                }
            }
            catch (MissingMethodException)
            {
                // Si el constructor con Productor no existe todavía, mostramos mensaje
                MessageBox.Show(
                    "El formulario de edición aún no está implementado.\n" +
                    "Agregue un constructor en 'FormularioAgregarProductor' que reciba un objeto 'Productor'.",
                    "Funcionalidad pendiente", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de edición: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un productor para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var productor = dgvProductores.SelectedRows[0].Tag as Productor;
            if (productor == null) return;

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de eliminar al productor '{productor.Nombre} {productor.Apellido}'?\n\n" +
                "Esta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                using (var db = new AgroMultiContext())
                {
                    // Verificamos que no tenga entregas asociadas (opcional)
                    bool tieneEntregas = db.Entregas.Any(e => e.ProductorId == productor.ProductorId);
                    if (tieneEntregas)
                    {
                        MessageBox.Show(
                            "No se puede eliminar el productor porque tiene entregas registradas.\n" +
                            "Elimine primero las entregas asociadas.",
                            "Eliminación no permitida",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    // Adjuntamos y eliminamos
                    db.Productors.Attach(productor);
                    db.Productors.Remove(productor);
                    db.SaveChanges();
                }

                MessageBox.Show("Productor eliminado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarProductores(txtBuscar.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el productor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
