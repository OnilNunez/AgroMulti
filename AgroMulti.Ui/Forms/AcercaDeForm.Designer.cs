using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Font = System.Drawing.Font;

namespace CentroFermentacionSecado
{
    partial class AcercaDeForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelHeader;
        private Label lblTitulo;
        private Panel panelAccentStrip;

        private TableLayoutPanel layoutPrincipal;
        private Label lblAppName;
        private Label lblVersion;
        private Label lblDescripcion;
        private Label lblCreditos;

        private Panel panelInferior;
        private Button btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelHeader = new Panel();
            lblTitulo = new Label();
            panelAccentStrip = new Panel();

            layoutPrincipal = new TableLayoutPanel();
            lblAppName = new Label();
            lblVersion = new Label();
            lblDescripcion = new Label();
            lblCreditos = new Label();

            panelInferior = new Panel();
            btnCerrar = new Button();

            panelHeader.SuspendLayout();
            layoutPrincipal.SuspendLayout();
            panelInferior.SuspendLayout();
            SuspendLayout();

            // ── panelHeader ──────────────────────────────────────────
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(panelAccentStrip);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(22, 16, 22, 10);
            panelHeader.Size = new Size(480, 90);
            panelHeader.TabIndex = 0;

            // lblTitulo
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(22, 16);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(157, 45);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Acerca de";

            // panelAccentStrip
            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 77);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(436, 3);
            panelAccentStrip.TabIndex = 2;

            // ── layoutPrincipal ──────────────────────────────────────
            layoutPrincipal.BackColor = Color.White;
            layoutPrincipal.ColumnCount = 1;
            layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPrincipal.Controls.Add(lblAppName, 0, 0);
            layoutPrincipal.Controls.Add(lblVersion, 0, 1);
            layoutPrincipal.Controls.Add(lblDescripcion, 0, 2);
            layoutPrincipal.Controls.Add(lblCreditos, 0, 3);
            layoutPrincipal.Dock = DockStyle.Fill;
            layoutPrincipal.Location = new Point(0, 90);
            layoutPrincipal.Name = "layoutPrincipal";
            layoutPrincipal.Padding = new Padding(30, 20, 30, 20);
            layoutPrincipal.RowCount = 4;
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPrincipal.Size = new Size(480, 280);
            layoutPrincipal.TabIndex = 1;

            // lblAppName
            lblAppName.AutoSize = true;
            lblAppName.Dock = DockStyle.Fill;
            lblAppName.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblAppName.ForeColor = Color.FromArgb(44, 28, 16);
            lblAppName.Location = new Point(33, 20);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new Size(414, 50);
            lblAppName.TabIndex = 0;
            lblAppName.Text = "AgroMulti";
            lblAppName.TextAlign = ContentAlignment.MiddleCenter;

            // lblVersion
            lblVersion.AutoSize = true;
            lblVersion.Dock = DockStyle.Fill;
            lblVersion.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblVersion.ForeColor = Color.FromArgb(80, 55, 30);
            lblVersion.Location = new Point(33, 70);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(414, 40);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "Versión 1.0.0";
            lblVersion.TextAlign = ContentAlignment.MiddleCenter;

            // lblDescripcion
            lblDescripcion.AutoSize = true;
            lblDescripcion.Dock = DockStyle.Fill;
            lblDescripcion.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblDescripcion.ForeColor = Color.FromArgb(128, 105, 82);
            lblDescripcion.Location = new Point(33, 110);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(414, 80);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Sistema integral para la gestión del Centro de Fermentación y Secado.\r\nRegistro de entregas, productores y trazabilidad.";
            lblDescripcion.TextAlign = ContentAlignment.MiddleCenter;

            // lblCreditos
            lblCreditos.AutoSize = true;
            lblCreditos.Dock = DockStyle.Fill;
            lblCreditos.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblCreditos.ForeColor = Color.FromArgb(160, 140, 120);
            lblCreditos.Location = new Point(33, 190);
            lblCreditos.Name = "lblCreditos";
            lblCreditos.Size = new Size(414, 70);
            lblCreditos.TabIndex = 3;
            lblCreditos.Text = "Desarrollado por: Elier Onil Nuñez Peña\r\nTel: 829-305-0249\r\n© 2025";
            lblCreditos.TextAlign = ContentAlignment.MiddleCenter;

            // ── panelInferior ────────────────────────────────────────
            panelInferior.BackColor = Color.White;
            panelInferior.Controls.Add(btnCerrar);
            panelInferior.Dock = DockStyle.Bottom;
            panelInferior.Location = new Point(0, 370);
            panelInferior.Name = "panelInferior";
            panelInferior.Size = new Size(480, 60);
            panelInferior.TabIndex = 2;

            // btnCerrar
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.BackColor = Color.White;
            btnCerrar.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Font = new Font("Segoe UI", 9F);
            btnCerrar.ForeColor = Color.FromArgb(44, 28, 16);
            btnCerrar.Location = new Point(368, 11);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(100, 38);
            btnCerrar.TabIndex = 0;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = false;
            btnCerrar.Click += btnCerrar_Click;

            // ── FormularioAcercaDe ────────────────────────────────────
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            ClientSize = new Size(480, 430);
            Controls.Add(layoutPrincipal);
            Controls.Add(panelInferior);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(44, 28, 16);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormularioAcercaDe";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Acerca de AgroMulti";

            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            layoutPrincipal.ResumeLayout(false);
            layoutPrincipal.PerformLayout();
            panelInferior.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}