using System.Drawing;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    partial class ProductoresForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelHeader;
        private Panel panelAccent;
        private Label lblTitulo;
        private Label lblSubtitulo;

        private Panel panelAcciones;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Button btnAgregar;
        private Button btnEditar;
        private Button btnEliminar;

        private Panel panelContenido;
        private Panel panelSombra;
        private Panel panelCard;

        private DataGridView dgvProductores;

        private DataGridViewTextBoxColumn colCodigo;
        private DataGridViewTextBoxColumn colNombre;
        private DataGridViewTextBoxColumn colApellido;
        private DataGridViewTextBoxColumn colTelefono;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            panelHeader = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            panelAcciones = new Panel();
            btnAgregar = new Button();
            btnEditar = new Button();
            btnEliminar = new Button();
            lblBuscar = new Label();
            txtBuscar = new TextBox();
            panelContenido = new Panel();
            panelCard = new Panel();
            dgvProductores = new DataGridView();
            colCodigo = new DataGridViewTextBoxColumn();
            colNombre = new DataGridViewTextBoxColumn();
            colApellido = new DataGridViewTextBoxColumn();
            colTelefono = new DataGridViewTextBoxColumn();
            panelHeader.SuspendLayout();
            panelAcciones.SuspendLayout();
            panelContenido.SuspendLayout();
            panelCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductores).BeginInit();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(lblSubtitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(22, 16, 22, 10);
            panelHeader.Size = new Size(1180, 90);
            panelHeader.TabIndex = 2;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(30, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(224, 48);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Productores";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 9F);
            lblSubtitulo.ForeColor = Color.FromArgb(185, 165, 140);
            lblSubtitulo.Location = new Point(24, 52);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(368, 25);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Gestión y control de productores registrados";
            // 
            // panelAcciones
            // 
            panelAcciones.BackColor = Color.FromArgb(58, 38, 18);
            panelAcciones.Controls.Add(btnAgregar);
            panelAcciones.Controls.Add(btnEditar);
            panelAcciones.Controls.Add(btnEliminar);
            panelAcciones.Controls.Add(lblBuscar);
            panelAcciones.Controls.Add(txtBuscar);
            panelAcciones.Dock = DockStyle.Top;
            panelAcciones.Location = new Point(0, 90);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Padding = new Padding(22, 15, 22, 10);
            panelAcciones.Size = new Size(1180, 70);
            panelAcciones.TabIndex = 1;
            // 
            // btnAgregar
            // 
            btnAgregar.BackColor = Color.FromArgb(92, 122, 42);
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.FlatStyle = FlatStyle.Flat;
            btnAgregar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAgregar.ForeColor = Color.White;
            btnAgregar.Location = new Point(22, 15);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(110, 35);
            btnAgregar.TabIndex = 0;
            btnAgregar.Text = "Agregar";
            btnAgregar.UseVisualStyleBackColor = false;
            // 
            // btnEditar
            // 
            btnEditar.BackColor = Color.FromArgb(65, 42, 22);
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.FlatStyle = FlatStyle.Flat;
            btnEditar.ForeColor = Color.White;
            btnEditar.Location = new Point(140, 15);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(95, 35);
            btnEditar.TabIndex = 1;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = false;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.FromArgb(38, 22, 10);
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.ForeColor = Color.FromArgb(185, 165, 140);
            btnEliminar.Location = new Point(245, 15);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(100, 35);
            btnEliminar.TabIndex = 2;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = false;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.ForeColor = Color.FromArgb(185, 165, 140);
            lblBuscar.Location = new Point(760, 22);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(67, 25);
            lblBuscar.TabIndex = 3;
            lblBuscar.Text = "Buscar:";
            // 
            // txtBuscar
            // 
            txtBuscar.BackColor = Color.White;
            txtBuscar.BorderStyle = BorderStyle.FixedSingle;
            txtBuscar.ForeColor = Color.FromArgb(44, 28, 16);
            txtBuscar.Location = new Point(825, 18);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(220, 31);
            txtBuscar.TabIndex = 4;
            // 
            // panelContenido
            // 
            panelContenido.BackColor = Color.FromArgb(245, 240, 232);
            panelContenido.Controls.Add(panelCard);
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Location = new Point(0, 160);
            panelContenido.Name = "panelContenido";
            panelContenido.Padding = new Padding(20);
            panelContenido.Size = new Size(1180, 460);
            panelContenido.TabIndex = 0;
            // 
            // panelCard
            // 
            panelCard.BackColor = Color.White;
            panelCard.Controls.Add(dgvProductores);
            panelCard.Dock = DockStyle.Fill;
            panelCard.Location = new Point(20, 20);
            panelCard.Name = "panelCard";
            panelCard.Padding = new Padding(10);
            panelCard.Size = new Size(1140, 420);
            panelCard.TabIndex = 0;
            // 
            // dgvProductores
            // 
            dgvProductores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductores.BackgroundColor = Color.White;
            dgvProductores.BorderStyle = BorderStyle.None;
            dgvProductores.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(58, 38, 18);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(185, 165, 140);
            dgvProductores.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvProductores.ColumnHeadersHeight = 40;
            dgvProductores.Columns.AddRange(new DataGridViewColumn[] { colCodigo, colNombre, colApellido, colTelefono });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(44, 28, 16);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(230, 218, 200);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(44, 28, 16);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvProductores.DefaultCellStyle = dataGridViewCellStyle2;
            dgvProductores.Dock = DockStyle.Fill;
            dgvProductores.EnableHeadersVisualStyles = false;
            dgvProductores.GridColor = Color.FromArgb(230, 223, 215);
            dgvProductores.Location = new Point(10, 10);
            dgvProductores.Name = "dgvProductores";
            dgvProductores.RowHeadersVisible = false;
            dgvProductores.RowHeadersWidth = 62;
            dgvProductores.RowTemplate.Height = 34;
            dgvProductores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductores.Size = new Size(1120, 400);
            dgvProductores.TabIndex = 0;
            // 
            // colCodigo
            // 
            colCodigo.HeaderText = "Código";
            colCodigo.MinimumWidth = 8;
            colCodigo.Name = "colCodigo";
            // 
            // colNombre
            // 
            colNombre.HeaderText = "Nombre";
            colNombre.MinimumWidth = 8;
            colNombre.Name = "colNombre";
            // 
            // colApellido
            // 
            colApellido.HeaderText = "Apellido";
            colApellido.MinimumWidth = 8;
            colApellido.Name = "colApellido";
            // 
            // colTelefono
            // 
            colTelefono.HeaderText = "Teléfono";
            colTelefono.MinimumWidth = 8;
            colTelefono.Name = "colTelefono";
            // 
            // FormularioProductores
            // 
            BackColor = Color.FromArgb(245, 240, 232);
            ClientSize = new Size(1180, 620);
            Controls.Add(panelContenido);
            Controls.Add(panelAcciones);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(44, 28, 16);
            Name = "FormularioProductores";
            Text = "Gestión de Productores";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelAcciones.ResumeLayout(false);
            panelAcciones.PerformLayout();
            panelContenido.ResumeLayout(false);
            panelCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProductores).EndInit();
            ResumeLayout(false);
        }
    }
}