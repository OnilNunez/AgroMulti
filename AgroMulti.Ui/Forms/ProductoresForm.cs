using AgroMulti.Domain.DTOs;
using AgroMulti.Ui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgroMulti.Ui.Forms
{
    public partial class ProductoresForm : Form
    {
        public ProductoresForm()
        {
            InitializeComponent();

            Configurar();
            _ = CargarProductoresAsync();
        }

        // ── Helper genérico para consumir la API ────────────────────────
        private static async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            var response = await ApiClient.Client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<List<T>>>();
            return wrapper?.Data ?? new List<T>();
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
        private async Task CargarProductoresAsync(string filtro = "")
        {
            try
            {
                var todos = await GetListAsync<ProductorDto>("api/Productores");
                var query = todos.AsEnumerable();

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

                var productores = query.OrderBy(p => p.Codigo).ToList();

                dgvProductores.Rows.Clear();
                foreach (var p in productores)
                {
                    int rowIndex = dgvProductores.Rows.Add(
                        p.Codigo,
                        p.Nombre,
                        p.Apellido,
                        p.Telefono ?? ""
                    );
                    dgvProductores.Rows[rowIndex].Tag = p;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productores: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            await CargarProductoresAsync(txtBuscar.Text);
        }

        // ── Operaciones CRUD ──────────────────────────────────────────
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new ProductorDetalleForm())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        _ = CargarProductoresAsync(txtBuscar.Text);
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

            var productor = dgvProductores.SelectedRows[0].Tag as ProductorDto;
            if (productor == null) return;

            try
            {
                using (var frm = new ProductorDetalleForm(productor))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        _ = CargarProductoresAsync(txtBuscar.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de edición: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un productor para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var productor = dgvProductores.SelectedRows[0].Tag as ProductorDto;
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
                var entregas = await GetListAsync<EntregaDto>("api/Entregas");
                bool tieneEntregas = entregas.Any(e => e.ProductorId == productor.Id);

                if (tieneEntregas)
                {
                    MessageBox.Show(
                        "No se puede eliminar el productor porque tiene entregas registradas.\n" +
                        "Elimine primero las entregas asociadas.",
                        "Eliminación no permitida",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                var deleteResponse = await ApiClient.Client.DeleteAsync($"api/Productores/{productor.Id}");
                if (!deleteResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("No se pudo eliminar el productor.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Productor eliminado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CargarProductoresAsync(txtBuscar.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el productor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}