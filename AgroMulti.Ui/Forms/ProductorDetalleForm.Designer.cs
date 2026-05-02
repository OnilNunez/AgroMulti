using System;
using System.Drawing;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    partial class ProductorDetalleForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel panelHeader;
        private Panel panelAccentStrip;
        private Label lblTitulo;
        private Label lblSubtitulo;

        // Contenido
        private Panel panelContenido;
        private Panel panelCard;
        private TableLayoutPanel layoutCampos;

        private Label lblCodigo;
        private TextBox txtCodigo;       
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblApellido;
        private TextBox txtApellido;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblDireccion;
        private TextBox txtDireccion;

        // Botones inferiores
        private Panel panelInferior;
        private Button btnCancelar;
        private Button btnGuardar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelHeader = new Panel();
            panelAccentStrip = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            panelContenido = new Panel();
            panelCard = new Panel();
            layoutCampos = new TableLayoutPanel();
            lblCodigo = new Label();
            txtCodigo = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblApellido = new Label();
            txtApellido = new TextBox();
            lblTelefono = new Label();
            txtTelefono = new TextBox();
            lblDireccion = new Label();
            txtDireccion = new TextBox();
            panelInferior = new Panel();
            btnCancelar = new Button();
            btnGuardar = new Button();
            panelHeader.SuspendLayout();
            panelContenido.SuspendLayout();
            panelCard.SuspendLayout();
            layoutCampos.SuspendLayout();
            panelInferior.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(panelAccentStrip);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(lblSubtitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(22, 16, 22, 10);
            panelHeader.Size = new Size(640, 96);
            panelHeader.TabIndex = 0;
            // 
            // panelAccentStrip
            // 
            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 83);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(596, 3);
            panelAccentStrip.TabIndex = 2;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(22, 10);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(304, 45);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Agregar productor";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 9F);
            lblSubtitulo.ForeColor = Color.FromArgb(185, 165, 140);
            lblSubtitulo.Location = new Point(24, 50);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(281, 25);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Complete los datos del productor";
            // 
            // panelContenido
            // 
            panelContenido.BackColor = Color.FromArgb(245, 240, 232);
            panelContenido.Controls.Add(panelCard);
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Location = new Point(0, 96);
            panelContenido.Name = "panelContenido";
            panelContenido.Padding = new Padding(20);
            panelContenido.Size = new Size(640, 310);
            panelContenido.TabIndex = 1;
            // 
            // panelCard
            // 
            panelCard.BackColor = Color.White;
            panelCard.Controls.Add(layoutCampos);
            panelCard.Dock = DockStyle.Fill;
            panelCard.Location = new Point(20, 20);
            panelCard.Name = "panelCard";
            panelCard.Padding = new Padding(16);
            panelCard.Size = new Size(600, 270);
            panelCard.TabIndex = 0;
            // 
            // layoutCampos
            // 
            layoutCampos.ColumnCount = 2;
            layoutCampos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            layoutCampos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            layoutCampos.Controls.Add(lblCodigo, 0, 0);
            layoutCampos.Controls.Add(txtCodigo, 0, 1);
            layoutCampos.Controls.Add(lblNombre, 1, 0);
            layoutCampos.Controls.Add(txtNombre, 1, 1);
            layoutCampos.Controls.Add(lblApellido, 0, 2);
            layoutCampos.Controls.Add(txtApellido, 0, 3);
            layoutCampos.Controls.Add(lblTelefono, 1, 2);
            layoutCampos.Controls.Add(txtTelefono, 1, 3);
            layoutCampos.Controls.Add(lblDireccion, 0, 4);
            layoutCampos.Controls.Add(txtDireccion, 0, 5);
            layoutCampos.Dock = DockStyle.Fill;
            layoutCampos.Location = new Point(16, 16);
            layoutCampos.Name = "layoutCampos";
            layoutCampos.RowCount = 6;
            layoutCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            layoutCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layoutCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            layoutCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layoutCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            layoutCampos.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutCampos.Size = new Size(568, 238);
            layoutCampos.TabIndex = 0;
            // 
            // lblCodigo
            // 
            lblCodigo.AutoSize = true;
            lblCodigo.Font = new Font("Segoe UI", 9F);
            lblCodigo.ForeColor = Color.FromArgb(128, 105, 82);
            lblCodigo.Location = new Point(3, 0);
            lblCodigo.Name = "lblCodigo";
            lblCodigo.Size = new Size(71, 25);
            lblCodigo.TabIndex = 0;
            lblCodigo.Text = "Código";
            // 
            // txtCodigo
            // 
            txtCodigo.BackColor = SystemColors.Control;
            txtCodigo.Dock = DockStyle.Fill;
            txtCodigo.Font = new Font("Segoe UI", 9F);
            txtCodigo.Location = new Point(3, 29);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.ReadOnly = true;
            txtCodigo.Size = new Size(209, 31);
            txtCodigo.TabIndex = 0;
            txtCodigo.TabStop = false;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 9F);
            lblNombre.ForeColor = Color.FromArgb(128, 105, 82);
            lblNombre.Location = new Point(218, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(78, 25);
            lblNombre.TabIndex = 1;
            lblNombre.Text = "Nombre";
            // 
            // txtNombre
            // 
            txtNombre.Dock = DockStyle.Fill;
            txtNombre.Font = new Font("Segoe UI", 9F);
            txtNombre.Location = new Point(218, 29);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(347, 31);
            txtNombre.TabIndex = 1;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Font = new Font("Segoe UI", 9F);
            lblApellido.ForeColor = Color.FromArgb(128, 105, 82);
            lblApellido.Location = new Point(3, 64);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(78, 25);
            lblApellido.TabIndex = 2;
            lblApellido.Text = "Apellido";
            // 
            // txtApellido
            // 
            txtApellido.Dock = DockStyle.Fill;
            txtApellido.Font = new Font("Segoe UI", 9F);
            txtApellido.Location = new Point(3, 93);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(209, 31);
            txtApellido.TabIndex = 2;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Font = new Font("Segoe UI", 9F);
            lblTelefono.ForeColor = Color.FromArgb(128, 105, 82);
            lblTelefono.Location = new Point(218, 64);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(79, 25);
            lblTelefono.TabIndex = 3;
            lblTelefono.Text = "Teléfono";
            // 
            // txtTelefono
            // 
            txtTelefono.Dock = DockStyle.Fill;
            txtTelefono.Font = new Font("Segoe UI", 9F);
            txtTelefono.Location = new Point(218, 93);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(347, 31);
            txtTelefono.TabIndex = 3;
            // 
            // lblDireccion
            // 
            lblDireccion.AutoSize = true;
            layoutCampos.SetColumnSpan(lblDireccion, 2);
            lblDireccion.Font = new Font("Segoe UI", 9F);
            lblDireccion.ForeColor = Color.FromArgb(128, 105, 82);
            lblDireccion.Location = new Point(3, 128);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(85, 25);
            lblDireccion.TabIndex = 4;
            lblDireccion.Text = "Dirección";
            // 
            // txtDireccion
            // 
            layoutCampos.SetColumnSpan(txtDireccion, 2);
            txtDireccion.Dock = DockStyle.Fill;
            txtDireccion.Font = new Font("Segoe UI", 9F);
            txtDireccion.Location = new Point(3, 157);
            txtDireccion.Multiline = true;
            txtDireccion.Name = "txtDireccion";
            txtDireccion.ScrollBars = ScrollBars.Vertical;
            txtDireccion.Size = new Size(562, 78);
            txtDireccion.TabIndex = 4;
            // 
            // panelInferior
            // 
            panelInferior.BackColor = Color.White;
            panelInferior.Controls.Add(btnCancelar);
            panelInferior.Controls.Add(btnGuardar);
            panelInferior.Dock = DockStyle.Bottom;
            panelInferior.Location = new Point(0, 406);
            panelInferior.Name = "panelInferior";
            panelInferior.Size = new Size(640, 64);
            panelInferior.TabIndex = 2;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.BackColor = Color.White;
            btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F);
            btnCancelar.ForeColor = Color.FromArgb(44, 28, 16);
            btnCancelar.Location = new Point(416, 13);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(100, 38);
            btnCancelar.TabIndex = 0;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGuardar.BackColor = Color.FromArgb(92, 122, 42);
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(524, 13);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(100, 38);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // ProductorDetalleForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            ClientSize = new Size(640, 470);
            Controls.Add(panelContenido);
            Controls.Add(panelInferior);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(44, 28, 16);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProductorDetalleForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Agregar productor";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelContenido.ResumeLayout(false);
            panelCard.ResumeLayout(false);
            layoutCampos.ResumeLayout(false);
            layoutCampos.PerformLayout();
            panelInferior.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}