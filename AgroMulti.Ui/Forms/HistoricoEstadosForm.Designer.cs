using System.Drawing;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    partial class HistoricoEstadosForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Estructura principal ──────────────────────────────────────────────
        private Panel panelHeader;
        private Panel panelAccentStrip;
        private Panel panelSummary;
        private GroupBox groupHistorial;
        private Panel panelFooter;

        // ── Header ────────────────────────────────────────────────────────────
        private Label lblTitle;
        private Label lblSubtitle;

        // ── Tarjetas de resumen (igual que MainMenu groupSummary) ─────────────
        private Panel panelCardTotal;
        private Label lblCardTotalTitle;
        private Label lblCardTotalValue;

        private Panel panelCardUltimo;
        private Label lblCardUltimoTitle;
        private Label lblCardUltimoValue;

        private Panel panelCardEstado;
        private Label lblCardEstadoTitle;
        private Label lblCardEstadoValue;

        // ── Grid ──────────────────────────────────────────────────────────────
        private DataGridView dgvHistorial;
        private DataGridViewTextBoxColumn colFecha;
        private DataGridViewTextBoxColumn colEstado;
        private DataGridViewTextBoxColumn colObservacion;

        // ── Footer ────────────────────────────────────────────────────────────
        private Button btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            panelHeader = new Panel();
            panelAccentStrip = new Panel();
            lblSubtitle = new Label();
            lblTitle = new Label();
            panelSummary = new Panel();
            panelCardTotal = new Panel();
            lblCardTotalValue = new Label();
            lblCardTotalTitle = new Label();
            panelCardUltimo = new Panel();
            lblCardUltimoValue = new Label();
            lblCardUltimoTitle = new Label();
            panelCardEstado = new Panel();
            lblCardEstadoValue = new Label();
            lblCardEstadoTitle = new Label();
            groupHistorial = new GroupBox();
            dgvHistorial = new DataGridView();
            colFecha = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            colObservacion = new DataGridViewTextBoxColumn();
            panelFooter = new Panel();
            btnCerrar = new Button();
            panelHeader.SuspendLayout();
            panelSummary.SuspendLayout();
            panelCardTotal.SuspendLayout();
            panelCardUltimo.SuspendLayout();
            panelCardEstado.SuspendLayout();
            groupHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).BeginInit();
            panelFooter.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(panelAccentStrip);
            panelHeader.Controls.Add(lblSubtitle);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(22, 16, 22, 3);
            panelHeader.Size = new Size(875, 100);
            panelHeader.TabIndex = 3;
            // 
            // panelAccentStrip
            // 
            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 94);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(831, 3);
            panelAccentStrip.TabIndex = 0;
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(185, 165, 140);
            lblSubtitle.Location = new Point(34, 62);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(408, 25);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Registro cronológico de transiciones de la entrega";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Snow;
            lblTitle.Location = new Point(22, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(536, 48);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "Historial de cambios de estado";
            // 
            // panelSummary
            // 
            panelSummary.BackColor = Color.FromArgb(245, 240, 232);
            panelSummary.Controls.Add(panelCardTotal);
            panelSummary.Controls.Add(panelCardUltimo);
            panelSummary.Controls.Add(panelCardEstado);
            panelSummary.Dock = DockStyle.Top;
            panelSummary.Location = new Point(0, 100);
            panelSummary.Name = "panelSummary";
            panelSummary.Padding = new Padding(14, 14, 14, 10);
            panelSummary.Size = new Size(875, 143);
            panelSummary.TabIndex = 2;
            // 
            // panelCardTotal
            // 
            panelCardTotal.BackColor = Color.White;
            panelCardTotal.BorderStyle = BorderStyle.FixedSingle;
            panelCardTotal.Controls.Add(lblCardTotalValue);
            panelCardTotal.Controls.Add(lblCardTotalTitle);
            panelCardTotal.Location = new Point(14, 14);
            panelCardTotal.Name = "panelCardTotal";
            panelCardTotal.Padding = new Padding(16, 14, 16, 14);
            panelCardTotal.Size = new Size(220, 116);
            panelCardTotal.TabIndex = 0;
            // 
            // lblCardTotalValue
            // 
            lblCardTotalValue.Dock = DockStyle.Fill;
            lblCardTotalValue.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblCardTotalValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblCardTotalValue.Location = new Point(16, 38);
            lblCardTotalValue.Name = "lblCardTotalValue";
            lblCardTotalValue.Size = new Size(186, 62);
            lblCardTotalValue.TabIndex = 0;
            lblCardTotalValue.Text = "0";
            lblCardTotalValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCardTotalTitle
            // 
            lblCardTotalTitle.Dock = DockStyle.Top;
            lblCardTotalTitle.Font = new Font("Segoe UI", 9F);
            lblCardTotalTitle.ForeColor = Color.FromArgb(128, 105, 82);
            lblCardTotalTitle.Location = new Point(16, 14);
            lblCardTotalTitle.Name = "lblCardTotalTitle";
            lblCardTotalTitle.Size = new Size(186, 24);
            lblCardTotalTitle.TabIndex = 1;
            lblCardTotalTitle.Text = "Total de registros";
            // 
            // panelCardUltimo
            // 
            panelCardUltimo.BackColor = Color.White;
            panelCardUltimo.BorderStyle = BorderStyle.FixedSingle;
            panelCardUltimo.Controls.Add(lblCardUltimoValue);
            panelCardUltimo.Controls.Add(lblCardUltimoTitle);
            panelCardUltimo.Location = new Point(248, 14);
            panelCardUltimo.Name = "panelCardUltimo";
            panelCardUltimo.Padding = new Padding(16, 14, 16, 14);
            panelCardUltimo.Size = new Size(310, 116);
            panelCardUltimo.TabIndex = 1;
            // 
            // lblCardUltimoValue
            // 
            lblCardUltimoValue.Dock = DockStyle.Fill;
            lblCardUltimoValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCardUltimoValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblCardUltimoValue.Location = new Point(16, 38);
            lblCardUltimoValue.Name = "lblCardUltimoValue";
            lblCardUltimoValue.Size = new Size(276, 62);
            lblCardUltimoValue.TabIndex = 0;
            lblCardUltimoValue.Text = "—";
            lblCardUltimoValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCardUltimoTitle
            // 
            lblCardUltimoTitle.Dock = DockStyle.Top;
            lblCardUltimoTitle.Font = new Font("Segoe UI", 9F);
            lblCardUltimoTitle.ForeColor = Color.FromArgb(128, 105, 82);
            lblCardUltimoTitle.Location = new Point(16, 14);
            lblCardUltimoTitle.Name = "lblCardUltimoTitle";
            lblCardUltimoTitle.Size = new Size(276, 24);
            lblCardUltimoTitle.TabIndex = 1;
            lblCardUltimoTitle.Text = "Último cambio";
            // 
            // panelCardEstado
            // 
            panelCardEstado.BackColor = Color.White;
            panelCardEstado.BorderStyle = BorderStyle.FixedSingle;
            panelCardEstado.Controls.Add(lblCardEstadoValue);
            panelCardEstado.Controls.Add(lblCardEstadoTitle);
            panelCardEstado.Location = new Point(572, 14);
            panelCardEstado.Name = "panelCardEstado";
            panelCardEstado.Padding = new Padding(16, 14, 16, 14);
            panelCardEstado.Size = new Size(274, 116);
            panelCardEstado.TabIndex = 2;
            // 
            // lblCardEstadoValue
            // 
            lblCardEstadoValue.Dock = DockStyle.Fill;
            lblCardEstadoValue.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblCardEstadoValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblCardEstadoValue.Location = new Point(16, 38);
            lblCardEstadoValue.Name = "lblCardEstadoValue";
            lblCardEstadoValue.Size = new Size(240, 62);
            lblCardEstadoValue.TabIndex = 0;
            lblCardEstadoValue.Text = "—";
            lblCardEstadoValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCardEstadoTitle
            // 
            lblCardEstadoTitle.Dock = DockStyle.Top;
            lblCardEstadoTitle.Font = new Font("Segoe UI", 9F);
            lblCardEstadoTitle.ForeColor = Color.FromArgb(128, 105, 82);
            lblCardEstadoTitle.Location = new Point(16, 14);
            lblCardEstadoTitle.Name = "lblCardEstadoTitle";
            lblCardEstadoTitle.Size = new Size(240, 24);
            lblCardEstadoTitle.TabIndex = 1;
            lblCardEstadoTitle.Text = "Estado actual";
            // 
            // groupHistorial
            // 
            groupHistorial.BackColor = Color.FromArgb(245, 240, 232);
            groupHistorial.Controls.Add(dgvHistorial);
            groupHistorial.Dock = DockStyle.Fill;
            groupHistorial.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupHistorial.ForeColor = Color.FromArgb(80, 55, 30);
            groupHistorial.Location = new Point(0, 243);
            groupHistorial.Name = "groupHistorial";
            groupHistorial.Padding = new Padding(14, 16, 14, 14);
            groupHistorial.Size = new Size(875, 395);
            groupHistorial.TabIndex = 0;
            groupHistorial.TabStop = false;
            groupHistorial.Text = "Historial de cambios";
            // 
            // dgvHistorial
            // 
            dgvHistorial.AllowUserToAddRows = false;
            dgvHistorial.AllowUserToDeleteRows = false;
            dgvHistorial.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(250, 247, 242);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(80, 55, 30);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(230, 218, 200);
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(38, 22, 10);
            dgvHistorial.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorial.BackgroundColor = Color.White;
            dgvHistorial.BorderStyle = BorderStyle.None;
            dgvHistorial.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHistorial.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(58, 38, 18);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(58, 38, 18);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvHistorial.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvHistorial.ColumnHeadersHeight = 40;
            dgvHistorial.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvHistorial.Columns.AddRange(new DataGridViewColumn[] { colFecha, colEstado, colObservacion });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.White;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(80, 55, 30);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(230, 218, 200);
            dataGridViewCellStyle4.SelectionForeColor = Color.FromArgb(38, 22, 10);
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvHistorial.DefaultCellStyle = dataGridViewCellStyle4;
            dgvHistorial.Dock = DockStyle.Fill;
            dgvHistorial.EnableHeadersVisualStyles = false;
            dgvHistorial.GridColor = Color.FromArgb(222, 210, 194);
            dgvHistorial.Location = new Point(14, 43);
            dgvHistorial.MultiSelect = false;
            dgvHistorial.Name = "dgvHistorial";
            dgvHistorial.ReadOnly = true;
            dgvHistorial.RowHeadersVisible = false;
            dgvHistorial.RowHeadersWidth = 62;
            dgvHistorial.RowTemplate.Height = 34;
            dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorial.Size = new Size(847, 338);
            dgvHistorial.TabIndex = 0;
            dgvHistorial.CellFormatting += DgvHistorial_CellFormatting;
            // 
            // colFecha
            // 
            dataGridViewCellStyle3.Font = new Font("Consolas", 8.5F);
            colFecha.DefaultCellStyle = dataGridViewCellStyle3;
            colFecha.FillWeight = 27F;
            colFecha.HeaderText = "Fecha y hora";
            colFecha.MinimumWidth = 160;
            colFecha.Name = "colFecha";
            colFecha.ReadOnly = true;
            // 
            // colEstado
            // 
            colEstado.FillWeight = 23F;
            colEstado.HeaderText = "Estado";
            colEstado.MinimumWidth = 140;
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            // 
            // colObservacion
            // 
            colObservacion.FillWeight = 50F;
            colObservacion.HeaderText = "Observaciones";
            colObservacion.MinimumWidth = 260;
            colObservacion.Name = "colObservacion";
            colObservacion.ReadOnly = true;
            // 
            // panelFooter
            // 
            panelFooter.BackColor = Color.Transparent;
            panelFooter.Controls.Add(btnCerrar);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 638);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(16, 8, 16, 8);
            panelFooter.Size = new Size(875, 50);
            panelFooter.TabIndex = 1;
            // 
            // btnCerrar
            // 
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.BackColor = Color.FromArgb(92, 122, 42);
            btnCerrar.Cursor = Cursors.Hand;
            btnCerrar.DialogResult = DialogResult.Cancel;
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseDownBackColor = Color.FromArgb(72, 98, 30);
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(110, 145, 50);
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.Location = new Point(759, 8);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(100, 34);
            btnCerrar.TabIndex = 0;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = false;
            btnCerrar.Click += BtnCerrar_Click;
            // 
            // HistoricoEstadosForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            CancelButton = btnCerrar;
            ClientSize = new Size(875, 688);
            Controls.Add(groupHistorial);
            Controls.Add(panelFooter);
            Controls.Add(panelSummary);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HistoricoEstadosForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Historial de cambios de estado";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelSummary.ResumeLayout(false);
            panelCardTotal.ResumeLayout(false);
            panelCardUltimo.ResumeLayout(false);
            panelCardEstado.ResumeLayout(false);
            groupHistorial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).EndInit();
            panelFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}