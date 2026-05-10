using System.Drawing;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    partial class HistoricoEstadosForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Estructura principal ──────────────────────────────────────
        private Panel panelHeader;
        private Panel panelAccentStrip;
        private Panel panelSummary;
        private Panel panelFiltros;
        private GroupBox groupHistorial;
        private Panel panelFooter;

        // ── Header ────────────────────────────────────────────────────
        private Label lblTitle;
        private Label lblSubtitle;

        // ── Tarjetas de resumen ───────────────────────────────────────
        private Panel panelCardTotal;
        private Label lblCardTotalTitle;
        private Label lblCardTotalValue;

        private Panel panelCardUltimo;
        private Label lblCardUltimoTitle;
        private Label lblCardUltimoValue;

        private Panel panelCardEstado;
        private Label lblCardEstadoTitle;
        private Label lblCardEstadoValue;

        // ── Panel de filtros ──────────────────────────────────────────
        private Label lblBuscar;
        private ComboBox cmbBuscarEntrega;
        private Label lblEstado;
        private ComboBox cmbFiltroEstado;
        private Label lblDesde;
        private DateTimePicker dtpDesde;
        private Label lblHasta;
        private DateTimePicker dtpHasta;
        private Button btnFiltrar;
        private Button btnLimpiarFiltros;

        // ── Grid ──────────────────────────────────────────────────────
        private DataGridView dgvHistorial;
        private DataGridViewTextBoxColumn colFecha;
        private DataGridViewTextBoxColumn colEntrega;
        private DataGridViewTextBoxColumn colLugar;
        private DataGridViewTextBoxColumn colEstado;
        private DataGridViewTextBoxColumn colObservacion;

        // ── Footer ────────────────────────────────────────────────────
        private Button btnRefrescar;
        private Button btnExportar;
        private Button btnCerrar;

        // ── Menú contextual de exportación ────────────────────────────
        private ContextMenuStrip ctxExportar;
        private ToolStripMenuItem itemExportarExcel;
        private ToolStripMenuItem itemExportarPDF;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
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
            panelFiltros = new Panel();
            lblBuscar = new Label();
            cmbBuscarEntrega = new ComboBox();
            lblEstado = new Label();
            cmbFiltroEstado = new ComboBox();
            lblDesde = new Label();
            dtpDesde = new DateTimePicker();
            lblHasta = new Label();
            dtpHasta = new DateTimePicker();
            btnFiltrar = new Button();
            btnLimpiarFiltros = new Button();
            groupHistorial = new GroupBox();
            dgvHistorial = new DataGridView();
            colFecha = new DataGridViewTextBoxColumn();
            colEntrega = new DataGridViewTextBoxColumn();
            colLugar = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            colObservacion = new DataGridViewTextBoxColumn();
            panelFooter = new Panel();
            btnRefrescar = new Button();
            btnExportar = new Button();
            btnCerrar = new Button();
            ctxExportar = new ContextMenuStrip(components);
            itemExportarExcel = new ToolStripMenuItem();
            itemExportarPDF = new ToolStripMenuItem();
            panelHeader.SuspendLayout();
            panelSummary.SuspendLayout();
            panelCardTotal.SuspendLayout();
            panelCardUltimo.SuspendLayout();
            panelCardEstado.SuspendLayout();
            panelFiltros.SuspendLayout();
            groupHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).BeginInit();
            panelFooter.SuspendLayout();
            ctxExportar.SuspendLayout();
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
            panelHeader.Size = new Size(1143, 100);
            panelHeader.TabIndex = 5;
            // 
            // panelAccentStrip
            // 
            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 94);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(1099, 3);
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
            lblTitle.Location = new Point(22, 14);
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
            panelSummary.Size = new Size(1143, 143);
            panelSummary.TabIndex = 4;
            // 
            // panelCardTotal
            // 
            panelCardTotal.BackColor = Color.White;
            panelCardTotal.BorderStyle = BorderStyle.FixedSingle;
            panelCardTotal.Controls.Add(lblCardTotalValue);
            panelCardTotal.Controls.Add(lblCardTotalTitle);
            panelCardTotal.Location = new Point(81, 14);
            panelCardTotal.Name = "panelCardTotal";
            panelCardTotal.Padding = new Padding(16, 14, 16, 14);
            panelCardTotal.Size = new Size(250, 116);
            panelCardTotal.TabIndex = 0;
            // 
            // lblCardTotalValue
            // 
            lblCardTotalValue.Dock = DockStyle.Fill;
            lblCardTotalValue.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblCardTotalValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblCardTotalValue.Location = new Point(16, 38);
            lblCardTotalValue.Name = "lblCardTotalValue";
            lblCardTotalValue.Size = new Size(216, 62);
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
            lblCardTotalTitle.Size = new Size(216, 24);
            lblCardTotalTitle.TabIndex = 1;
            lblCardTotalTitle.Text = "Total de registros";
            // 
            // panelCardUltimo
            // 
            panelCardUltimo.BackColor = Color.White;
            panelCardUltimo.BorderStyle = BorderStyle.FixedSingle;
            panelCardUltimo.Controls.Add(lblCardUltimoValue);
            panelCardUltimo.Controls.Add(lblCardUltimoTitle);
            panelCardUltimo.Location = new Point(387, 14);
            panelCardUltimo.Name = "panelCardUltimo";
            panelCardUltimo.Padding = new Padding(16, 14, 16, 14);
            panelCardUltimo.Size = new Size(350, 116);
            panelCardUltimo.TabIndex = 1;
            // 
            // lblCardUltimoValue
            // 
            lblCardUltimoValue.Dock = DockStyle.Fill;
            lblCardUltimoValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCardUltimoValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblCardUltimoValue.Location = new Point(16, 38);
            lblCardUltimoValue.Name = "lblCardUltimoValue";
            lblCardUltimoValue.Size = new Size(316, 62);
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
            lblCardUltimoTitle.Size = new Size(316, 24);
            lblCardUltimoTitle.TabIndex = 1;
            lblCardUltimoTitle.Text = "Último cambio";
            // 
            // panelCardEstado
            // 
            panelCardEstado.BackColor = Color.White;
            panelCardEstado.BorderStyle = BorderStyle.FixedSingle;
            panelCardEstado.Controls.Add(lblCardEstadoValue);
            panelCardEstado.Controls.Add(lblCardEstadoTitle);
            panelCardEstado.Location = new Point(800, 14);
            panelCardEstado.Name = "panelCardEstado";
            panelCardEstado.Padding = new Padding(16, 14, 16, 14);
            panelCardEstado.Size = new Size(290, 116);
            panelCardEstado.TabIndex = 2;
            // 
            // lblCardEstadoValue
            // 
            lblCardEstadoValue.Dock = DockStyle.Fill;
            lblCardEstadoValue.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblCardEstadoValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblCardEstadoValue.Location = new Point(16, 38);
            lblCardEstadoValue.Name = "lblCardEstadoValue";
            lblCardEstadoValue.Size = new Size(256, 62);
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
            lblCardEstadoTitle.Size = new Size(256, 24);
            lblCardEstadoTitle.TabIndex = 1;
            lblCardEstadoTitle.Text = "Entregas";
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.FromArgb(232, 224, 210);
            panelFiltros.Controls.Add(lblBuscar);
            panelFiltros.Controls.Add(cmbBuscarEntrega);
            panelFiltros.Controls.Add(lblEstado);
            panelFiltros.Controls.Add(cmbFiltroEstado);
            panelFiltros.Controls.Add(lblDesde);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(lblHasta);
            panelFiltros.Controls.Add(dtpHasta);
            panelFiltros.Controls.Add(btnFiltrar);
            panelFiltros.Controls.Add(btnLimpiarFiltros);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 243);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Padding = new Padding(14, 8, 14, 8);
            panelFiltros.Size = new Size(1143, 96);
            panelFiltros.TabIndex = 3;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Font = new Font("Segoe UI", 9F);
            lblBuscar.ForeColor = Color.FromArgb(80, 55, 30);
            lblBuscar.Location = new Point(14, 12);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(76, 25);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Entrega:";
            // 
            // cmbBuscarEntrega
            // 
            cmbBuscarEntrega.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBuscarEntrega.FlatStyle = FlatStyle.System;
            cmbBuscarEntrega.Font = new Font("Segoe UI", 9F);
            cmbBuscarEntrega.Location = new Point(101, 8);
            cmbBuscarEntrega.Name = "cmbBuscarEntrega";
            cmbBuscarEntrega.Size = new Size(236, 33);
            cmbBuscarEntrega.TabIndex = 1;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Font = new Font("Segoe UI", 9F);
            lblEstado.ForeColor = Color.FromArgb(80, 55, 30);
            lblEstado.Location = new Point(338, 12);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(70, 25);
            lblEstado.TabIndex = 2;
            lblEstado.Text = "Estado:";
            // 
            // cmbFiltroEstado
            // 
            cmbFiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroEstado.FlatStyle = FlatStyle.System;
            cmbFiltroEstado.Font = new Font("Segoe UI", 9F);
            cmbFiltroEstado.Location = new Point(413, 8);
            cmbFiltroEstado.Name = "cmbFiltroEstado";
            cmbFiltroEstado.Size = new Size(215, 33);
            cmbFiltroEstado.TabIndex = 3;
            // 
            // lblDesde
            // 
            lblDesde.AutoSize = true;
            lblDesde.Font = new Font("Segoe UI", 9F);
            lblDesde.ForeColor = Color.FromArgb(80, 55, 30);
            lblDesde.Location = new Point(14, 54);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(66, 25);
            lblDesde.TabIndex = 4;
            lblDesde.Text = "Desde:";
            // 
            // dtpDesde
            // 
            dtpDesde.Font = new Font("Segoe UI", 9F);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(81, 50);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(165, 31);
            dtpDesde.TabIndex = 5;
            // 
            // lblHasta
            // 
            lblHasta.AutoSize = true;
            lblHasta.Font = new Font("Segoe UI", 9F);
            lblHasta.ForeColor = Color.FromArgb(80, 55, 30);
            lblHasta.Location = new Point(252, 54);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(61, 25);
            lblHasta.TabIndex = 6;
            lblHasta.Text = "Hasta:";
            // 
            // dtpHasta
            // 
            dtpHasta.Font = new Font("Segoe UI", 9F);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(319, 50);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(165, 31);
            dtpHasta.TabIndex = 7;
            // 
            // btnFiltrar
            // 
            btnFiltrar.BackColor = Color.FromArgb(92, 122, 42);
            btnFiltrar.Cursor = Cursors.Hand;
            btnFiltrar.FlatAppearance.BorderSize = 0;
            btnFiltrar.FlatAppearance.MouseDownBackColor = Color.FromArgb(72, 98, 30);
            btnFiltrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(110, 145, 50);
            btnFiltrar.FlatStyle = FlatStyle.Flat;
            btnFiltrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnFiltrar.ForeColor = Color.White;
            btnFiltrar.Location = new Point(671, 50);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(115, 30);
            btnFiltrar.TabIndex = 8;
            btnFiltrar.Text = "Filtrar";
            btnFiltrar.UseVisualStyleBackColor = false;
            btnFiltrar.Click += BtnFiltrar_Click;
            // 
            // btnLimpiarFiltros
            // 
            btnLimpiarFiltros.BackColor = Color.FromArgb(140, 100, 50);
            btnLimpiarFiltros.Cursor = Cursors.Hand;
            btnLimpiarFiltros.FlatAppearance.BorderSize = 0;
            btnLimpiarFiltros.FlatAppearance.MouseDownBackColor = Color.FromArgb(110, 75, 30);
            btnLimpiarFiltros.FlatAppearance.MouseOverBackColor = Color.FromArgb(165, 120, 65);
            btnLimpiarFiltros.FlatStyle = FlatStyle.Flat;
            btnLimpiarFiltros.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLimpiarFiltros.ForeColor = Color.White;
            btnLimpiarFiltros.Location = new Point(671, 12);
            btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            btnLimpiarFiltros.Size = new Size(115, 30);
            btnLimpiarFiltros.TabIndex = 9;
            btnLimpiarFiltros.Text = "✕  Limpiar";
            btnLimpiarFiltros.UseVisualStyleBackColor = false;
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;
            // 
            // groupHistorial
            // 
            groupHistorial.BackColor = Color.FromArgb(245, 240, 232);
            groupHistorial.Controls.Add(dgvHistorial);
            groupHistorial.Dock = DockStyle.Fill;
            groupHistorial.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupHistorial.ForeColor = Color.FromArgb(80, 55, 30);
            groupHistorial.Location = new Point(0, 339);
            groupHistorial.Name = "groupHistorial";
            groupHistorial.Padding = new Padding(14, 16, 14, 14);
            groupHistorial.Size = new Size(1143, 394);
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
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
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
            dgvHistorial.Columns.AddRange(new DataGridViewColumn[] { colFecha, colEntrega, colLugar, colEstado, colObservacion });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = Color.FromArgb(80, 55, 30);
            dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(230, 218, 200);
            dataGridViewCellStyle5.SelectionForeColor = Color.FromArgb(38, 22, 10);
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            dgvHistorial.DefaultCellStyle = dataGridViewCellStyle5;
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
            dgvHistorial.Size = new Size(1115, 337);
            dgvHistorial.TabIndex = 0;
            dgvHistorial.CellFormatting += DgvHistorial_CellFormatting;
            // 
            // colFecha
            // 
            dataGridViewCellStyle3.Font = new Font("Consolas", 8.5F);
            colFecha.DefaultCellStyle = dataGridViewCellStyle3;
            colFecha.FillWeight = 17F;
            colFecha.HeaderText = "Fecha y hora";
            colFecha.MinimumWidth = 155;
            colFecha.Name = "colFecha";
            colFecha.ReadOnly = true;
            colFecha.Width = 155;
            // 
            // colEntrega
            // 
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colEntrega.DefaultCellStyle = dataGridViewCellStyle4;
            colEntrega.FillWeight = 8F;
            colEntrega.HeaderText = "Entrega";
            colEntrega.MinimumWidth = 75;
            colEntrega.Name = "colEntrega";
            colEntrega.ReadOnly = true;
            colEntrega.Width = 112;
            // 
            // colLugar
            // 
            colLugar.FillWeight = 22F;
            colLugar.HeaderText = "Lugar en almacén";
            colLugar.MinimumWidth = 180;
            colLugar.Name = "colLugar";
            colLugar.ReadOnly = true;
            colLugar.Width = 196;
            // 
            // colEstado
            // 
            colEstado.FillWeight = 17F;
            colEstado.HeaderText = "Estado";
            colEstado.MinimumWidth = 130;
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            colEstado.Width = 130;
            // 
            // colObservacion
            // 
            colObservacion.FillWeight = 36F;
            colObservacion.HeaderText = "Observaciones";
            colObservacion.MinimumWidth = 210;
            colObservacion.Name = "colObservacion";
            colObservacion.ReadOnly = true;
            colObservacion.Width = 210;
            // 
            // panelFooter
            // 
            panelFooter.BackColor = Color.Transparent;
            panelFooter.Controls.Add(btnRefrescar);
            panelFooter.Controls.Add(btnExportar);
            panelFooter.Controls.Add(btnCerrar);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 733);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(16, 8, 16, 8);
            panelFooter.Size = new Size(1143, 50);
            panelFooter.TabIndex = 1;
            // 
            // btnRefrescar
            // 
            btnRefrescar.BackColor = Color.FromArgb(58, 38, 18);
            btnRefrescar.Cursor = Cursors.Hand;
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 22, 10);
            btnRefrescar.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 55, 28);
            btnRefrescar.FlatStyle = FlatStyle.Flat;
            btnRefrescar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRefrescar.ForeColor = Color.White;
            btnRefrescar.Location = new Point(16, 8);
            btnRefrescar.Name = "btnRefrescar";
            btnRefrescar.Size = new Size(115, 34);
            btnRefrescar.TabIndex = 1;
            btnRefrescar.Text = "⟳  Refrescar";
            btnRefrescar.UseVisualStyleBackColor = false;
            btnRefrescar.Click += BtnRefrescar_Click;
            // 
            // btnExportar
            // 
            btnExportar.BackColor = Color.FromArgb(130, 90, 35);
            btnExportar.Cursor = Cursors.Hand;
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 65, 20);
            btnExportar.FlatAppearance.MouseOverBackColor = Color.FromArgb(158, 112, 50);
            btnExportar.FlatStyle = FlatStyle.Flat;
            btnExportar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportar.ForeColor = Color.White;
            btnExportar.Location = new Point(141, 8);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(130, 34);
            btnExportar.TabIndex = 2;
            btnExportar.Text = "↓  Exportar";
            btnExportar.UseVisualStyleBackColor = false;
            btnExportar.Click += BtnExportar_Click;
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
            btnCerrar.Location = new Point(1027, 8);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(100, 34);
            btnCerrar.TabIndex = 0;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = false;
            btnCerrar.Click += BtnCerrar_Click;
            // 
            // ctxExportar
            // 
            ctxExportar.ImageScalingSize = new Size(24, 24);
            ctxExportar.Items.AddRange(new ToolStripItem[] { itemExportarExcel, itemExportarPDF });
            ctxExportar.Name = "ctxExportar";
            ctxExportar.Size = new Size(208, 68);
            // 
            // itemExportarExcel
            // 
            itemExportarExcel.Name = "itemExportarExcel";
            itemExportarExcel.Size = new Size(207, 32);
            itemExportarExcel.Text = "Exportar a Excel";
            itemExportarExcel.Click += ItemExportarExcel_Click;
            // 
            // itemExportarPDF
            // 
            itemExportarPDF.Name = "itemExportarPDF";
            itemExportarPDF.Size = new Size(207, 32);
            itemExportarPDF.Text = "Exportar a PDF";
            itemExportarPDF.Click += ItemExportarPDF_Click;
            // 
            // HistoricoEstadosForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            CancelButton = btnCerrar;
            ClientSize = new Size(1143, 783);
            Controls.Add(groupHistorial);
            Controls.Add(panelFooter);
            Controls.Add(panelFiltros);
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
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            groupHistorial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).EndInit();
            panelFooter.ResumeLayout(false);
            ctxExportar.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}