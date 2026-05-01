//TOD0: Agregar que la ubiacion en el formulario sea un combobox,generar un algoritmo
//que genere automaticamnete el numero, tambien en el formulario de agregar productor
//debe de tener un codigo cin nomenclatura que sea sistematico y eficiente, que no permita
//que haya duplicados en la base de datos, y hacer el completado del form mas funcional.



using System;
using System.Drawing;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    partial class RegistroEntregaForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelHeader;
        private Panel panelContenido;
        private Panel panelInferior;
        private Panel panelAccentStrip;

        private Label lblTitulo;
        private Label lblSubtitulo;

        private TableLayoutPanel mainLayout;

        // ── Productor ──
        private GroupBox groupProductor;
        private TableLayoutPanel layoutProductor;
        private Label lblCodigoProductor;
        private ComboBox cboCodigoProductor;
        private Button btnBuscarProductor;
        private Button btnAgregarProductor;
        private DataGridView dgvProductores;

        // ── Entrega ──
        private GroupBox groupEntrega;
        private TableLayoutPanel layoutEntrega;
        private Label lblNumeroEntrega;
        private TextBox txtNumeroEntrega;   // ← ahora ReadOnly
        private Label lblFechaEntrega;
        private DateTimePicker dtpFechaEntrega;
        private Label lblProducto;
        private ComboBox cboProducto;
        private Label lblSubProducto;
        private ComboBox cboSubProducto;
        private Label lblEstadoEntrega;
        private ComboBox cboEstadoEntrega;

        // ── Vehículo y conductor ──
        private GroupBox groupVehiculo;
        private TableLayoutPanel layoutVehiculo;
        private Label lblPlaca;
        private TextBox txtPlaca;
        private Label lblConductor;
        private TextBox txtNombreConductor;

        // ── Pesaje ──
        private GroupBox groupPesaje;
        private TableLayoutPanel layoutPesaje;
        private Label lblKilos;
        private TextBox txtKilos;
        private Label lblCajas;
        private TextBox txtCajas;
        private Label lblSacos;
        private TextBox txtSacos;
        private Label lblKilosSecos;
        private TextBox txtKilosSecos;

        // ── Ubicación (almacén) — ahora ComboBoxes ──
        private GroupBox groupUbicacion;
        private TableLayoutPanel layoutUbicacion;
        private Label lblCalle;
        private ComboBox cboCalle;
        private Label lblTunel;
        private ComboBox cboFila;
        private Label lblBox;
        private ComboBox cboPosicion;

        // ── Observaciones ──
        private GroupBox groupObservaciones;
        private TableLayoutPanel layoutObservaciones;
        private TextBox txtObservaciones;
        private Label lblObservaciones;

        // ── Botones inferiores ──
        private Button btnCancelar;
        private Button btnLimpiar;
        private Button btnGuardar;

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
            panelAccentStrip = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            panelContenido = new Panel();
            mainLayout = new TableLayoutPanel();
            groupProductor = new GroupBox();
            layoutProductor = new TableLayoutPanel();
            lblCodigoProductor = new Label();
            cboCodigoProductor = new ComboBox();
            btnBuscarProductor = new Button();
            btnAgregarProductor = new Button();
            dgvProductores = new DataGridView();
            groupEntrega = new GroupBox();
            layoutEntrega = new TableLayoutPanel();
            lblNumeroEntrega = new Label();
            lblFechaEntrega = new Label();
            txtNumeroEntrega = new TextBox();
            dtpFechaEntrega = new DateTimePicker();
            lblProducto = new Label();
            lblSubProducto = new Label();
            cboProducto = new ComboBox();
            cboSubProducto = new ComboBox();
            lblEstadoEntrega = new Label();
            cboEstadoEntrega = new ComboBox();
            groupVehiculo = new GroupBox();
            layoutVehiculo = new TableLayoutPanel();
            lblPlaca = new Label();
            lblConductor = new Label();
            txtPlaca = new TextBox();
            txtNombreConductor = new TextBox();
            groupPesaje = new GroupBox();
            layoutPesaje = new TableLayoutPanel();
            lblKilos = new Label();
            lblCajas = new Label();
            lblSacos = new Label();
            lblKilosSecos = new Label();
            txtKilos = new TextBox();
            txtCajas = new TextBox();
            txtSacos = new TextBox();
            txtKilosSecos = new TextBox();
            groupUbicacion = new GroupBox();
            layoutUbicacion = new TableLayoutPanel();
            lblCalle = new Label();
            lblTunel = new Label();
            lblBox = new Label();
            cboCalle = new ComboBox();
            cboFila = new ComboBox();
            cboPosicion = new ComboBox();
            groupObservaciones = new GroupBox();
            layoutObservaciones = new TableLayoutPanel();
            txtObservaciones = new TextBox();
            lblObservaciones = new Label();
            panelInferior = new Panel();
            btnCancelar = new Button();
            btnLimpiar = new Button();
            btnGuardar = new Button();
            panelHeader.SuspendLayout();
            panelContenido.SuspendLayout();
            mainLayout.SuspendLayout();
            groupProductor.SuspendLayout();
            layoutProductor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductores).BeginInit();
            groupEntrega.SuspendLayout();
            layoutEntrega.SuspendLayout();
            groupVehiculo.SuspendLayout();
            layoutVehiculo.SuspendLayout();
            groupPesaje.SuspendLayout();
            layoutPesaje.SuspendLayout();
            groupUbicacion.SuspendLayout();
            layoutUbicacion.SuspendLayout();
            groupObservaciones.SuspendLayout();
            layoutObservaciones.SuspendLayout();
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
            panelHeader.Size = new Size(1368, 101);
            panelHeader.TabIndex = 0;
            // 
            // panelAccentStrip
            // 
            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 88);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(1324, 3);
            panelAccentStrip.TabIndex = 2;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(22, 10);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(279, 45);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Registrar entrega";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 9F);
            lblSubtitulo.ForeColor = Color.FromArgb(185, 165, 140);
            lblSubtitulo.Location = new Point(24, 55);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(315, 25);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Complete la información de la entrega";
            // 
            // panelContenido
            // 
            panelContenido.AutoScroll = true;
            panelContenido.BackColor = Color.FromArgb(245, 240, 232);
            panelContenido.Controls.Add(mainLayout);
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Location = new Point(0, 101);
            panelContenido.Name = "panelContenido";
            panelContenido.Padding = new Padding(20);
            panelContenido.Size = new Size(1368, 613);
            panelContenido.TabIndex = 1;
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 63.19F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36.81F));
            mainLayout.Controls.Add(groupProductor, 0, 0);
            mainLayout.Controls.Add(groupEntrega, 1, 0);
            mainLayout.Controls.Add(groupVehiculo, 0, 1);
            mainLayout.Controls.Add(groupPesaje, 1, 1);
            mainLayout.Controls.Add(groupUbicacion, 0, 2);
            mainLayout.Controls.Add(groupObservaciones, 1, 2);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(20, 20);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(8);
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 53F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 23.3393173F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 23.6983833F));
            mainLayout.Size = new Size(1328, 573);
            mainLayout.TabIndex = 0;
            // 
            // groupProductor
            // 
            groupProductor.BackColor = Color.White;
            groupProductor.Controls.Add(layoutProductor);
            groupProductor.Dock = DockStyle.Fill;
            groupProductor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupProductor.ForeColor = Color.FromArgb(80, 55, 30);
            groupProductor.Location = new Point(14, 14);
            groupProductor.Margin = new Padding(6);
            groupProductor.Name = "groupProductor";
            groupProductor.Padding = new Padding(14, 16, 14, 14);
            groupProductor.Size = new Size(817, 283);
            groupProductor.TabIndex = 0;
            groupProductor.TabStop = false;
            groupProductor.Text = "Información del productor";
            // 
            // layoutProductor
            // 
            layoutProductor.ColumnCount = 3;
            layoutProductor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutProductor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            layoutProductor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46F));
            layoutProductor.Controls.Add(lblCodigoProductor, 0, 0);
            layoutProductor.Controls.Add(cboCodigoProductor, 0, 1);
            layoutProductor.Controls.Add(btnBuscarProductor, 1, 1);
            layoutProductor.Controls.Add(btnAgregarProductor, 2, 1);
            layoutProductor.Controls.Add(dgvProductores, 0, 2);
            layoutProductor.Dock = DockStyle.Fill;
            layoutProductor.Location = new Point(14, 40);
            layoutProductor.Name = "layoutProductor";
            layoutProductor.RowCount = 4;
            layoutProductor.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutProductor.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layoutProductor.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutProductor.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
            layoutProductor.Size = new Size(789, 229);
            layoutProductor.TabIndex = 0;
            // 
            // lblCodigoProductor
            // 
            lblCodigoProductor.AutoSize = true;
            layoutProductor.SetColumnSpan(lblCodigoProductor, 3);
            lblCodigoProductor.Font = new Font("Segoe UI", 9F);
            lblCodigoProductor.ForeColor = Color.FromArgb(128, 105, 82);
            lblCodigoProductor.Location = new Point(3, 0);
            lblCodigoProductor.Name = "lblCodigoProductor";
            lblCodigoProductor.Size = new Size(269, 24);
            lblCodigoProductor.TabIndex = 0;
            lblCodigoProductor.Text = "Código o nombre del productor";
            // 
            // cboCodigoProductor
            // 
            cboCodigoProductor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCodigoProductor.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboCodigoProductor.Dock = DockStyle.Fill;
            cboCodigoProductor.Font = new Font("Segoe UI", 9F);
            cboCodigoProductor.Location = new Point(3, 27);
            cboCodigoProductor.Name = "cboCodigoProductor";
            cboCodigoProductor.Size = new Size(647, 33);
            cboCodigoProductor.TabIndex = 1;
            // 
            // btnBuscarProductor
            // 
            btnBuscarProductor.BackColor = Color.FromArgb(92, 122, 42);
            btnBuscarProductor.Cursor = Cursors.Hand;
            btnBuscarProductor.Dock = DockStyle.Fill;
            btnBuscarProductor.FlatAppearance.BorderSize = 0;
            btnBuscarProductor.FlatStyle = FlatStyle.Flat;
            btnBuscarProductor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBuscarProductor.ForeColor = Color.White;
            btnBuscarProductor.Location = new Point(656, 27);
            btnBuscarProductor.Name = "btnBuscarProductor";
            btnBuscarProductor.Size = new Size(84, 32);
            btnBuscarProductor.TabIndex = 2;
            btnBuscarProductor.Text = "🔍 Buscar";
            btnBuscarProductor.UseVisualStyleBackColor = false;
            // 
            // btnAgregarProductor
            // 
            btnAgregarProductor.BackColor = Color.FromArgb(92, 122, 42);
            btnAgregarProductor.Cursor = Cursors.Hand;
            btnAgregarProductor.Dock = DockStyle.Fill;
            btnAgregarProductor.FlatAppearance.BorderSize = 0;
            btnAgregarProductor.FlatStyle = FlatStyle.Flat;
            btnAgregarProductor.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnAgregarProductor.ForeColor = Color.White;
            btnAgregarProductor.Location = new Point(746, 27);
            btnAgregarProductor.Name = "btnAgregarProductor";
            btnAgregarProductor.Size = new Size(40, 32);
            btnAgregarProductor.TabIndex = 3;
            btnAgregarProductor.Text = "＋";
            btnAgregarProductor.UseVisualStyleBackColor = false;
            // 
            // dgvProductores
            // 
            dgvProductores.AllowUserToAddRows = false;
            dgvProductores.AllowUserToDeleteRows = false;
            dgvProductores.AllowUserToResizeRows = false;
            dgvProductores.BackgroundColor = Color.White;
            dgvProductores.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProductores.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 240, 232);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(80, 55, 30);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(245, 240, 232);
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvProductores.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvProductores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            layoutProductor.SetColumnSpan(dgvProductores, 3);
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(80, 55, 30);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(92, 122, 42);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvProductores.DefaultCellStyle = dataGridViewCellStyle2;
            dgvProductores.Dock = DockStyle.Fill;
            dgvProductores.EnableHeadersVisualStyles = false;
            dgvProductores.GridColor = Color.FromArgb(220, 215, 205);
            dgvProductores.Location = new Point(3, 65);
            dgvProductores.MultiSelect = false;
            dgvProductores.Name = "dgvProductores";
            dgvProductores.ReadOnly = true;
            dgvProductores.RowHeadersVisible = false;
            dgvProductores.RowHeadersWidth = 62;
            dgvProductores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductores.Size = new Size(783, 153);
            dgvProductores.TabIndex = 4;
            // 
            // groupEntrega
            // 
            groupEntrega.BackColor = Color.White;
            groupEntrega.Controls.Add(layoutEntrega);
            groupEntrega.Dock = DockStyle.Fill;
            groupEntrega.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupEntrega.ForeColor = Color.FromArgb(80, 55, 30);
            groupEntrega.Location = new Point(843, 14);
            groupEntrega.Margin = new Padding(6);
            groupEntrega.Name = "groupEntrega";
            groupEntrega.Padding = new Padding(14, 16, 14, 14);
            groupEntrega.Size = new Size(471, 283);
            groupEntrega.TabIndex = 1;
            groupEntrega.TabStop = false;
            groupEntrega.Text = "Información de la entrega";
            // 
            // layoutEntrega
            // 
            layoutEntrega.ColumnCount = 2;
            layoutEntrega.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutEntrega.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutEntrega.Controls.Add(lblNumeroEntrega, 0, 0);
            layoutEntrega.Controls.Add(lblFechaEntrega, 1, 0);
            layoutEntrega.Controls.Add(txtNumeroEntrega, 0, 1);
            layoutEntrega.Controls.Add(dtpFechaEntrega, 1, 1);
            layoutEntrega.Controls.Add(lblProducto, 0, 2);
            layoutEntrega.Controls.Add(lblSubProducto, 1, 2);
            layoutEntrega.Controls.Add(cboProducto, 0, 3);
            layoutEntrega.Controls.Add(cboSubProducto, 1, 3);
            layoutEntrega.Controls.Add(lblEstadoEntrega, 0, 4);
            layoutEntrega.Controls.Add(cboEstadoEntrega, 0, 5);
            layoutEntrega.Dock = DockStyle.Fill;
            layoutEntrega.Location = new Point(14, 40);
            layoutEntrega.Name = "layoutEntrega";
            layoutEntrega.RowCount = 6;
            layoutEntrega.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutEntrega.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layoutEntrega.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutEntrega.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layoutEntrega.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutEntrega.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layoutEntrega.Size = new Size(443, 229);
            layoutEntrega.TabIndex = 0;
            // 
            // lblNumeroEntrega
            // 
            lblNumeroEntrega.AutoSize = true;
            lblNumeroEntrega.Font = new Font("Segoe UI", 9F);
            lblNumeroEntrega.ForeColor = Color.FromArgb(128, 105, 82);
            lblNumeroEntrega.Location = new Point(3, 0);
            lblNumeroEntrega.Name = "lblNumeroEntrega";
            lblNumeroEntrega.Size = new Size(77, 24);
            lblNumeroEntrega.TabIndex = 0;
            lblNumeroEntrega.Text = "Número";
            // 
            // lblFechaEntrega
            // 
            lblFechaEntrega.AutoSize = true;
            lblFechaEntrega.Font = new Font("Segoe UI", 9F);
            lblFechaEntrega.ForeColor = Color.FromArgb(128, 105, 82);
            lblFechaEntrega.Location = new Point(224, 0);
            lblFechaEntrega.Name = "lblFechaEntrega";
            lblFechaEntrega.Size = new Size(57, 24);
            lblFechaEntrega.TabIndex = 1;
            lblFechaEntrega.Text = "Fecha";
            // 
            // txtNumeroEntrega (NUEVO: ReadOnly, BackColor, TabStop)
            // 
            txtNumeroEntrega.Dock = DockStyle.Fill;
            txtNumeroEntrega.Font = new Font("Segoe UI", 9F);
            txtNumeroEntrega.Location = new Point(3, 27);
            txtNumeroEntrega.Name = "txtNumeroEntrega";
            txtNumeroEntrega.ReadOnly = true;
            txtNumeroEntrega.BackColor = SystemColors.Control;
            txtNumeroEntrega.TabStop = false;
            txtNumeroEntrega.Size = new Size(215, 31);
            txtNumeroEntrega.TabIndex = 0;
            // 
            // dtpFechaEntrega
            // 
            dtpFechaEntrega.Dock = DockStyle.Fill;
            dtpFechaEntrega.Font = new Font("Segoe UI", 9F);
            dtpFechaEntrega.Format = DateTimePickerFormat.Short;
            dtpFechaEntrega.Location = new Point(224, 27);
            dtpFechaEntrega.Name = "dtpFechaEntrega";
            dtpFechaEntrega.Size = new Size(216, 31);
            dtpFechaEntrega.TabIndex = 1;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Font = new Font("Segoe UI", 9F);
            lblProducto.ForeColor = Color.FromArgb(128, 105, 82);
            lblProducto.Location = new Point(3, 58);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(85, 24);
            lblProducto.TabIndex = 2;
            lblProducto.Text = "Producto";
            // 
            // lblSubProducto
            // 
            lblSubProducto.AutoSize = true;
            lblSubProducto.Font = new Font("Segoe UI", 9F);
            lblSubProducto.ForeColor = Color.FromArgb(128, 105, 82);
            lblSubProducto.Location = new Point(224, 58);
            lblSubProducto.Name = "lblSubProducto";
            lblSubProducto.Size = new Size(117, 24);
            lblSubProducto.TabIndex = 3;
            lblSubProducto.Text = "Subproducto";
            // 
            // cboProducto
            // 
            cboProducto.Dock = DockStyle.Fill;
            cboProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProducto.Font = new Font("Segoe UI", 9F);
            cboProducto.FormattingEnabled = true;
            cboProducto.Location = new Point(3, 85);
            cboProducto.Name = "cboProducto";
            cboProducto.Size = new Size(215, 33);
            cboProducto.TabIndex = 2;
            // 
            // cboSubProducto
            // 
            cboSubProducto.Dock = DockStyle.Fill;
            cboSubProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSubProducto.Font = new Font("Segoe UI", 9F);
            cboSubProducto.FormattingEnabled = true;
            cboSubProducto.Location = new Point(224, 85);
            cboSubProducto.Name = "cboSubProducto";
            cboSubProducto.Size = new Size(216, 33);
            cboSubProducto.TabIndex = 3;
            // 
            // lblEstadoEntrega
            // 
            lblEstadoEntrega.AutoSize = true;
            layoutEntrega.SetColumnSpan(lblEstadoEntrega, 2);
            lblEstadoEntrega.Font = new Font("Segoe UI", 9F);
            lblEstadoEntrega.ForeColor = Color.FromArgb(128, 105, 82);
            lblEstadoEntrega.Location = new Point(3, 116);
            lblEstadoEntrega.Name = "lblEstadoEntrega";
            lblEstadoEntrega.Size = new Size(66, 24);
            lblEstadoEntrega.TabIndex = 4;
            lblEstadoEntrega.Text = "Estado";
            // 
            // cboEstadoEntrega
            // 
            layoutEntrega.SetColumnSpan(cboEstadoEntrega, 2);
            cboEstadoEntrega.Dock = DockStyle.Fill;
            cboEstadoEntrega.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEstadoEntrega.Font = new Font("Segoe UI", 9F);
            cboEstadoEntrega.FormattingEnabled = true;
            cboEstadoEntrega.Location = new Point(3, 143);
            cboEstadoEntrega.Name = "cboEstadoEntrega";
            cboEstadoEntrega.Size = new Size(437, 33);
            cboEstadoEntrega.TabIndex = 4;
            // 
            // groupVehiculo
            // 
            groupVehiculo.BackColor = Color.White;
            groupVehiculo.Controls.Add(layoutVehiculo);
            groupVehiculo.Dock = DockStyle.Fill;
            groupVehiculo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupVehiculo.ForeColor = Color.FromArgb(80, 55, 30);
            groupVehiculo.Location = new Point(14, 309);
            groupVehiculo.Margin = new Padding(6);
            groupVehiculo.Name = "groupVehiculo";
            groupVehiculo.Padding = new Padding(14, 16, 14, 14);
            groupVehiculo.Size = new Size(817, 117);
            groupVehiculo.TabIndex = 2;
            groupVehiculo.TabStop = false;
            groupVehiculo.Text = "Vehículo y conductor";
            // 
            // layoutVehiculo
            // 
            layoutVehiculo.ColumnCount = 2;
            layoutVehiculo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            layoutVehiculo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            layoutVehiculo.Controls.Add(lblPlaca, 0, 0);
            layoutVehiculo.Controls.Add(lblConductor, 1, 0);
            layoutVehiculo.Controls.Add(txtPlaca, 0, 1);
            layoutVehiculo.Controls.Add(txtNombreConductor, 1, 1);
            layoutVehiculo.Dock = DockStyle.Fill;
            layoutVehiculo.Location = new Point(14, 40);
            layoutVehiculo.Name = "layoutVehiculo";
            layoutVehiculo.RowCount = 2;
            layoutVehiculo.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutVehiculo.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layoutVehiculo.Size = new Size(789, 63);
            layoutVehiculo.TabIndex = 0;
            // 
            // lblPlaca
            // 
            lblPlaca.AutoSize = true;
            lblPlaca.Font = new Font("Segoe UI", 9F);
            lblPlaca.ForeColor = Color.FromArgb(128, 105, 82);
            lblPlaca.Location = new Point(3, 0);
            lblPlaca.Name = "lblPlaca";
            lblPlaca.Size = new Size(52, 24);
            lblPlaca.TabIndex = 0;
            lblPlaca.Text = "Placa";
            // 
            // lblConductor
            // 
            lblConductor.AutoSize = true;
            lblConductor.Font = new Font("Segoe UI", 9F);
            lblConductor.ForeColor = Color.FromArgb(128, 105, 82);
            lblConductor.Location = new Point(279, 0);
            lblConductor.Name = "lblConductor";
            lblConductor.Size = new Size(96, 24);
            lblConductor.TabIndex = 1;
            lblConductor.Text = "Conductor";
            // 
            // txtPlaca
            // 
            txtPlaca.Dock = DockStyle.Fill;
            txtPlaca.Font = new Font("Segoe UI", 9F);
            txtPlaca.Location = new Point(3, 27);
            txtPlaca.Name = "txtPlaca";
            txtPlaca.Size = new Size(270, 31);
            txtPlaca.TabIndex = 0;
            // 
            // txtNombreConductor
            // 
            txtNombreConductor.Dock = DockStyle.Fill;
            txtNombreConductor.Font = new Font("Segoe UI", 9F);
            txtNombreConductor.Location = new Point(279, 27);
            txtNombreConductor.Name = "txtNombreConductor";
            txtNombreConductor.Size = new Size(507, 31);
            txtNombreConductor.TabIndex = 1;
            // 
            // groupPesaje
            // 
            groupPesaje.BackColor = Color.White;
            groupPesaje.Controls.Add(layoutPesaje);
            groupPesaje.Dock = DockStyle.Fill;
            groupPesaje.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupPesaje.ForeColor = Color.FromArgb(80, 55, 30);
            groupPesaje.Location = new Point(843, 309);
            groupPesaje.Margin = new Padding(6);
            groupPesaje.Name = "groupPesaje";
            groupPesaje.Padding = new Padding(14, 16, 14, 14);
            groupPesaje.Size = new Size(471, 117);
            groupPesaje.TabIndex = 3;
            groupPesaje.TabStop = false;
            groupPesaje.Text = "Pesaje";
            // 
            // layoutPesaje
            // 
            layoutPesaje.ColumnCount = 4;
            layoutPesaje.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutPesaje.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutPesaje.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutPesaje.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutPesaje.Controls.Add(lblKilos, 0, 0);
            layoutPesaje.Controls.Add(lblCajas, 1, 0);
            layoutPesaje.Controls.Add(lblSacos, 2, 0);
            layoutPesaje.Controls.Add(lblKilosSecos, 3, 0);
            layoutPesaje.Controls.Add(txtKilos, 0, 1);
            layoutPesaje.Controls.Add(txtCajas, 1, 1);
            layoutPesaje.Controls.Add(txtSacos, 2, 1);
            layoutPesaje.Controls.Add(txtKilosSecos, 3, 1);
            layoutPesaje.Dock = DockStyle.Fill;
            layoutPesaje.Location = new Point(14, 40);
            layoutPesaje.Name = "layoutPesaje";
            layoutPesaje.RowCount = 2;
            layoutPesaje.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutPesaje.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layoutPesaje.Size = new Size(443, 63);
            layoutPesaje.TabIndex = 0;
            // 
            // lblKilos
            // 
            lblKilos.AutoSize = true;
            lblKilos.Font = new Font("Segoe UI", 9F);
            lblKilos.ForeColor = Color.FromArgb(128, 105, 82);
            lblKilos.Location = new Point(3, 0);
            lblKilos.Name = "lblKilos";
            lblKilos.Size = new Size(49, 24);
            lblKilos.TabIndex = 0;
            lblKilos.Text = "Kilos";
            // 
            // lblCajas
            // 
            lblCajas.AutoSize = true;
            lblCajas.Font = new Font("Segoe UI", 9F);
            lblCajas.ForeColor = Color.FromArgb(128, 105, 82);
            lblCajas.Location = new Point(113, 0);
            lblCajas.Name = "lblCajas";
            lblCajas.Size = new Size(53, 24);
            lblCajas.TabIndex = 1;
            lblCajas.Text = "Cajas";
            // 
            // lblSacos
            // 
            lblSacos.AutoSize = true;
            lblSacos.Font = new Font("Segoe UI", 9F);
            lblSacos.ForeColor = Color.FromArgb(128, 105, 82);
            lblSacos.Location = new Point(223, 0);
            lblSacos.Name = "lblSacos";
            lblSacos.Size = new Size(58, 24);
            lblSacos.TabIndex = 2;
            lblSacos.Text = "Sacos";
            // 
            // lblKilosSecos
            // 
            lblKilosSecos.AutoSize = true;
            lblKilosSecos.Font = new Font("Segoe UI", 9F);
            lblKilosSecos.ForeColor = Color.FromArgb(128, 105, 82);
            lblKilosSecos.Location = new Point(333, 0);
            lblKilosSecos.Name = "lblKilosSecos";
            lblKilosSecos.Size = new Size(98, 24);
            lblKilosSecos.TabIndex = 3;
            lblKilosSecos.Text = "Kilos secos";
            // 
            // txtKilos
            // 
            txtKilos.Dock = DockStyle.Fill;
            txtKilos.Font = new Font("Segoe UI", 9F);
            txtKilos.Location = new Point(3, 27);
            txtKilos.Name = "txtKilos";
            txtKilos.Size = new Size(104, 31);
            txtKilos.TabIndex = 0;
            // 
            // txtCajas
            // 
            txtCajas.Dock = DockStyle.Fill;
            txtCajas.Font = new Font("Segoe UI", 9F);
            txtCajas.Location = new Point(113, 27);
            txtCajas.Name = "txtCajas";
            txtCajas.Size = new Size(104, 31);
            txtCajas.TabIndex = 1;
            // 
            // txtSacos
            // 
            txtSacos.Dock = DockStyle.Fill;
            txtSacos.Font = new Font("Segoe UI", 9F);
            txtSacos.Location = new Point(223, 27);
            txtSacos.Name = "txtSacos";
            txtSacos.Size = new Size(104, 31);
            txtSacos.TabIndex = 2;
            // 
            // txtKilosSecos
            // 
            txtKilosSecos.Dock = DockStyle.Fill;
            txtKilosSecos.Font = new Font("Segoe UI", 9F);
            txtKilosSecos.Location = new Point(333, 27);
            txtKilosSecos.Name = "txtKilosSecos";
            txtKilosSecos.Size = new Size(107, 31);
            txtKilosSecos.TabIndex = 3;
            // 
            // groupUbicacion
            // 
            groupUbicacion.BackColor = Color.White;
            groupUbicacion.Controls.Add(layoutUbicacion);
            groupUbicacion.Dock = DockStyle.Fill;
            groupUbicacion.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupUbicacion.ForeColor = Color.FromArgb(80, 55, 30);
            groupUbicacion.Location = new Point(14, 438);
            groupUbicacion.Margin = new Padding(6);
            groupUbicacion.Name = "groupUbicacion";
            groupUbicacion.Padding = new Padding(14, 16, 14, 14);
            groupUbicacion.Size = new Size(817, 121);
            groupUbicacion.TabIndex = 4;
            groupUbicacion.TabStop = false;
            groupUbicacion.Text = "Ubicación en almacén";
            // 
            // layoutUbicacion
            // 
            layoutUbicacion.ColumnCount = 3;
            layoutUbicacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layoutUbicacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layoutUbicacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layoutUbicacion.Controls.Add(lblCalle, 0, 0);
            layoutUbicacion.Controls.Add(lblTunel, 1, 0);
            layoutUbicacion.Controls.Add(lblBox, 2, 0);
            layoutUbicacion.Controls.Add(cboCalle, 0, 1);
            layoutUbicacion.Controls.Add(cboFila, 1, 1);
            layoutUbicacion.Controls.Add(cboPosicion, 2, 1);
            layoutUbicacion.Dock = DockStyle.Fill;
            layoutUbicacion.Location = new Point(14, 40);
            layoutUbicacion.Name = "layoutUbicacion";
            layoutUbicacion.RowCount = 2;
            layoutUbicacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutUbicacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            layoutUbicacion.Size = new Size(789, 67);
            layoutUbicacion.TabIndex = 0;
            // 
            // lblCalle
            // 
            lblCalle.AutoSize = true;
            lblCalle.Font = new Font("Segoe UI", 9F);
            lblCalle.ForeColor = Color.FromArgb(128, 105, 82);
            lblCalle.Location = new Point(3, 0);
            lblCalle.Name = "lblCalle";
            lblCalle.Size = new Size(61, 24);
            lblCalle.TabIndex = 0;
            lblCalle.Text = "Pasillo";
            // 
            // lblTunel
            // 
            lblTunel.AutoSize = true;
            lblTunel.Font = new Font("Segoe UI", 9F);
            lblTunel.ForeColor = Color.FromArgb(128, 105, 82);
            lblTunel.Location = new Point(318, 0);
            lblTunel.Name = "lblTunel";
            lblTunel.Size = new Size(54, 24);
            lblTunel.TabIndex = 1;
            lblTunel.Text = "Túnel";
            // 
            // lblBox
            // 
            lblBox.AutoSize = true;
            lblBox.Font = new Font("Segoe UI", 9F);
            lblBox.ForeColor = Color.FromArgb(128, 105, 82);
            lblBox.Location = new Point(554, 0);
            lblBox.Name = "lblBox";
            lblBox.Size = new Size(75, 24);
            lblBox.TabIndex = 2;
            lblBox.Text = "Módulo";
            // 
            // cboCalle
            // 
            cboCalle.Dock = DockStyle.Fill;
            cboCalle.Font = new Font("Segoe UI", 9F);
            cboCalle.FormattingEnabled = true;
            cboCalle.Location = new Point(3, 27);
            cboCalle.Name = "cboCalle";
            cboCalle.Size = new Size(309, 33);
            cboCalle.TabIndex = 0;
            // 
            // cboFila
            // 
            cboFila.Dock = DockStyle.Fill;
            cboFila.Font = new Font("Segoe UI", 9F);
            cboFila.FormattingEnabled = true;
            cboFila.Location = new Point(318, 27);
            cboFila.Name = "cboFila";
            cboFila.Size = new Size(230, 33);
            cboFila.TabIndex = 1;
            // 
            // cboPosicion
            // 
            cboPosicion.Dock = DockStyle.Fill;
            cboPosicion.Font = new Font("Segoe UI", 9F);
            cboPosicion.FormattingEnabled = true;
            cboPosicion.Location = new Point(554, 27);
            cboPosicion.Name = "cboPosicion";
            cboPosicion.Size = new Size(232, 33);
            cboPosicion.TabIndex = 2;
            // 
            // groupObservaciones
            // 
            groupObservaciones.BackColor = Color.White;
            groupObservaciones.Controls.Add(layoutObservaciones);
            groupObservaciones.Dock = DockStyle.Fill;
            groupObservaciones.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupObservaciones.ForeColor = Color.FromArgb(80, 55, 30);
            groupObservaciones.Location = new Point(843, 438);
            groupObservaciones.Margin = new Padding(6);
            groupObservaciones.Name = "groupObservaciones";
            groupObservaciones.Padding = new Padding(14, 16, 14, 14);
            groupObservaciones.Size = new Size(471, 121);
            groupObservaciones.TabIndex = 5;
            groupObservaciones.TabStop = false;
            groupObservaciones.Text = "Observaciones";
            // 
            // layoutObservaciones
            // 
            layoutObservaciones.ColumnCount = 1;
            layoutObservaciones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutObservaciones.Controls.Add(txtObservaciones, 0, 1);
            layoutObservaciones.Controls.Add(lblObservaciones, 0, 0);
            layoutObservaciones.Dock = DockStyle.Fill;
            layoutObservaciones.Location = new Point(14, 40);
            layoutObservaciones.Name = "layoutObservaciones";
            layoutObservaciones.RowCount = 2;
            layoutObservaciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutObservaciones.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutObservaciones.Size = new Size(443, 67);
            layoutObservaciones.TabIndex = 0;
            // 
            // txtObservaciones
            // 
            txtObservaciones.Dock = DockStyle.Fill;
            txtObservaciones.Font = new Font("Segoe UI", 9F);
            txtObservaciones.Location = new Point(3, 27);
            txtObservaciones.Multiline = true;
            txtObservaciones.Name = "txtObservaciones";
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Size = new Size(437, 37);
            txtObservaciones.TabIndex = 0;
            // 
            // lblObservaciones
            // 
            lblObservaciones.AutoSize = true;
            lblObservaciones.Font = new Font("Segoe UI", 9F);
            lblObservaciones.ForeColor = Color.FromArgb(128, 105, 82);
            lblObservaciones.Location = new Point(3, 0);
            lblObservaciones.Name = "lblObservaciones";
            lblObservaciones.Size = new Size(128, 24);
            lblObservaciones.TabIndex = 0;
            lblObservaciones.Text = "Observaciones";
            // 
            // panelInferior
            // 
            panelInferior.BackColor = Color.White;
            panelInferior.Controls.Add(btnCancelar);
            panelInferior.Controls.Add(btnLimpiar);
            panelInferior.Controls.Add(btnGuardar);
            panelInferior.Dock = DockStyle.Bottom;
            panelInferior.Location = new Point(0, 714);
            panelInferior.Name = "panelInferior";
            panelInferior.Size = new Size(1368, 69);
            panelInferior.TabIndex = 2;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.BackColor = Color.White;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F);
            btnCancelar.ForeColor = Color.FromArgb(44, 28, 16);
            btnCancelar.Location = new Point(1041, 15);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(100, 38);
            btnCancelar.TabIndex = 0;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLimpiar.BackColor = Color.White;
            btnLimpiar.Cursor = Cursors.Hand;
            btnLimpiar.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.Font = new Font("Segoe UI", 9F);
            btnLimpiar.ForeColor = Color.FromArgb(44, 28, 16);
            btnLimpiar.Location = new Point(1147, 15);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(100, 38);
            btnLimpiar.TabIndex = 1;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGuardar.BackColor = Color.FromArgb(92, 122, 42);
            btnGuardar.Cursor = Cursors.Hand;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(1253, 15);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(100, 38);
            btnGuardar.TabIndex = 2;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // RegistroEntregaForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(245, 240, 232);
            ClientSize = new Size(1368, 783);
            Controls.Add(panelContenido);
            Controls.Add(panelInferior);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(44, 28, 16);
            MinimumSize = new Size(1240, 720);
            Name = "RegistroEntregaForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Registrar entrega";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelContenido.ResumeLayout(false);
            mainLayout.ResumeLayout(false);
            groupProductor.ResumeLayout(false);
            layoutProductor.ResumeLayout(false);
            layoutProductor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductores).EndInit();
            groupEntrega.ResumeLayout(false);
            layoutEntrega.ResumeLayout(false);
            layoutEntrega.PerformLayout();
            groupVehiculo.ResumeLayout(false);
            layoutVehiculo.ResumeLayout(false);
            layoutVehiculo.PerformLayout();
            groupPesaje.ResumeLayout(false);
            layoutPesaje.ResumeLayout(false);
            layoutPesaje.PerformLayout();
            groupUbicacion.ResumeLayout(false);
            layoutUbicacion.ResumeLayout(false);
            layoutUbicacion.PerformLayout();
            groupObservaciones.ResumeLayout(false);
            layoutObservaciones.ResumeLayout(false);
            layoutObservaciones.PerformLayout();
            panelInferior.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}