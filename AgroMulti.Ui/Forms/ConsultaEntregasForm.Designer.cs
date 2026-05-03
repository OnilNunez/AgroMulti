using System.Drawing;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace CentroFermentacionSecado
{
    partial class ConsultaEntregasForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Cabecera ────────────────────────────────────────────────────
        private Panel panelHeader;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel panelAccentStrip;

        // ── Contenedor principal ────────────────────────────────────────
        private TableLayoutPanel layoutPrincipal;

        // ── Grupo de filtros ────────────────────────────────────────────
        private GroupBox groupFiltros;
        private TableLayoutPanel layoutFiltros;
        private Label lblFechaDesde;
        private DateTimePicker dtpFechaDesde;
        private Label lblFechaHasta;
        private DateTimePicker dtpFechaHasta;
        private Label lblProductor;
        private ComboBox cboProductor;
        private Label lblProducto;
        private ComboBox cboProducto;
        private Label lblEstado;
        private ComboBox cboEstado;
        private Panel panelBotonesDerecha;
        private TableLayoutPanel layoutBotonesDerecha;
        private Button btnBuscar;
        private Button btnLimpiarFiltros;
        private Button btnModificarEstado;

        // ── Grupo de resultados ─────────────────────────────────────────
        private GroupBox groupResultados;
        private DataGridView dgvEntregas;
        private DataGridViewTextBoxColumn colEntregaId;
        private DataGridViewTextBoxColumn colNumero;
        private DataGridViewTextBoxColumn colFecha;
        private DataGridViewTextBoxColumn colProductor;
        private DataGridViewTextBoxColumn colProducto;
        private DataGridViewTextBoxColumn colSubproducto;
        private DataGridViewTextBoxColumn colKilos;
        private DataGridViewTextBoxColumn colEstado;
        private DataGridViewTextBoxColumn colObservaciones;
        

        // ── Panel inferior ──────────────────────────────────────────────
        private Panel panelInferior;
        private Panel panelSeparadorInferior;
        private Button btnCerrar;
        private Button btnHistorial;         

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
            panelAccentStrip = new Panel();
            layoutPrincipal = new TableLayoutPanel();
            groupFiltros = new GroupBox();
            layoutFiltros = new TableLayoutPanel();
            lblFechaDesde = new Label();
            dtpFechaDesde = new DateTimePicker();
            lblFechaHasta = new Label();
            dtpFechaHasta = new DateTimePicker();
            lblProductor = new Label();
            cboProductor = new ComboBox();
            lblProducto = new Label();
            cboProducto = new ComboBox();
            lblEstado = new Label();
            cboEstado = new ComboBox();
            panelBotonesDerecha = new Panel();
            layoutBotonesDerecha = new TableLayoutPanel();
            btnBuscar = new Button();
            btnLimpiarFiltros = new Button();
            btnModificarEstado = new Button();
            groupResultados = new GroupBox();
            dgvEntregas = new DataGridView();
            colEntregaId = new DataGridViewTextBoxColumn();
            colNumero = new DataGridViewTextBoxColumn();
            colFecha = new DataGridViewTextBoxColumn();
            colProductor = new DataGridViewTextBoxColumn();
            colProducto = new DataGridViewTextBoxColumn();
            colSubproducto = new DataGridViewTextBoxColumn();
            colKilos = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            colObservaciones = new DataGridViewTextBoxColumn();
            panelInferior = new Panel();
            btnCerrar = new Button();
            btnHistorial = new Button();
            panelSeparadorInferior = new Panel();
            panelHeader.SuspendLayout();
            layoutPrincipal.SuspendLayout();
            groupFiltros.SuspendLayout();
            layoutFiltros.SuspendLayout();
            panelBotonesDerecha.SuspendLayout();
            layoutBotonesDerecha.SuspendLayout();
            groupResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEntregas).BeginInit();
            panelInferior.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(lblSubtitulo);
            panelHeader.Controls.Add(panelAccentStrip);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(22, 16, 22, 10);
            panelHeader.Size = new Size(1200, 95);
            panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(22, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(301, 45);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Consultar entregas";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 9F);
            lblSubtitulo.ForeColor = Color.FromArgb(185, 165, 140);
            lblSubtitulo.Location = new Point(24, 55);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(317, 25);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Filtre y visualice el historial de entregas";
            // 
            // panelAccentStrip
            // 
            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 82);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(1156, 3);
            panelAccentStrip.TabIndex = 2;
            // 
            // layoutPrincipal
            // 
            layoutPrincipal.ColumnCount = 1;
            layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPrincipal.Controls.Add(groupFiltros, 0, 0);
            layoutPrincipal.Controls.Add(groupResultados, 0, 1);
            layoutPrincipal.Dock = DockStyle.Fill;
            layoutPrincipal.Location = new Point(0, 95);
            layoutPrincipal.Name = "layoutPrincipal";
            layoutPrincipal.Padding = new Padding(20);
            layoutPrincipal.RowCount = 2;
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPrincipal.Size = new Size(1200, 512);
            layoutPrincipal.TabIndex = 1;
            // 
            // groupFiltros
            // 
            groupFiltros.BackColor = Color.White;
            groupFiltros.Controls.Add(layoutFiltros);
            groupFiltros.Dock = DockStyle.Fill;
            groupFiltros.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupFiltros.ForeColor = Color.FromArgb(80, 55, 30);
            groupFiltros.Location = new Point(23, 23);
            groupFiltros.Margin = new Padding(3, 3, 3, 10);
            groupFiltros.Name = "groupFiltros";
            groupFiltros.Padding = new Padding(14, 16, 14, 14);
            groupFiltros.Size = new Size(1154, 187);
            groupFiltros.TabIndex = 0;
            groupFiltros.TabStop = false;
            groupFiltros.Text = "Filtros de búsqueda";
            // 
            // layoutFiltros
            // 
            layoutFiltros.ColumnCount = 3;
            layoutFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layoutFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layoutFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            layoutFiltros.Controls.Add(lblFechaDesde, 0, 0);
            layoutFiltros.Controls.Add(dtpFechaDesde, 0, 1);
            layoutFiltros.Controls.Add(lblFechaHasta, 1, 0);
            layoutFiltros.Controls.Add(dtpFechaHasta, 1, 1);
            layoutFiltros.Controls.Add(lblProductor, 2, 0);
            layoutFiltros.Controls.Add(cboProductor, 2, 1);
            layoutFiltros.Controls.Add(lblProducto, 0, 2);
            layoutFiltros.Controls.Add(cboProducto, 0, 3);
            layoutFiltros.Controls.Add(lblEstado, 1, 2);
            layoutFiltros.Controls.Add(cboEstado, 1, 3);
            layoutFiltros.Controls.Add(panelBotonesDerecha, 2, 2);
            layoutFiltros.Dock = DockStyle.Fill;
            layoutFiltros.Location = new Point(14, 40);
            layoutFiltros.Name = "layoutFiltros";
            layoutFiltros.RowCount = 4;
            layoutFiltros.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutFiltros.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layoutFiltros.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layoutFiltros.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layoutFiltros.Size = new Size(1126, 133);
            layoutFiltros.TabIndex = 0;
            // 
            // lblFechaDesde
            // 
            lblFechaDesde.AutoSize = true;
            lblFechaDesde.Dock = DockStyle.Fill;
            lblFechaDesde.Font = new Font("Segoe UI", 9F);
            lblFechaDesde.ForeColor = Color.FromArgb(128, 105, 82);
            lblFechaDesde.Location = new Point(3, 0);
            lblFechaDesde.Name = "lblFechaDesde";
            lblFechaDesde.Size = new Size(369, 24);
            lblFechaDesde.TabIndex = 0;
            lblFechaDesde.Text = "Fecha desde";
            // 
            // dtpFechaDesde
            // 
            dtpFechaDesde.Dock = DockStyle.Fill;
            dtpFechaDesde.Font = new Font("Segoe UI", 9F);
            dtpFechaDesde.Format = DateTimePickerFormat.Short;
            dtpFechaDesde.Location = new Point(3, 27);
            dtpFechaDesde.Name = "dtpFechaDesde";
            dtpFechaDesde.Size = new Size(369, 31);
            dtpFechaDesde.TabIndex = 0;
            // 
            // lblFechaHasta
            // 
            lblFechaHasta.AutoSize = true;
            lblFechaHasta.Dock = DockStyle.Fill;
            lblFechaHasta.Font = new Font("Segoe UI", 9F);
            lblFechaHasta.ForeColor = Color.FromArgb(128, 105, 82);
            lblFechaHasta.Location = new Point(378, 0);
            lblFechaHasta.Name = "lblFechaHasta";
            lblFechaHasta.Size = new Size(369, 24);
            lblFechaHasta.TabIndex = 1;
            lblFechaHasta.Text = "Fecha hasta";
            // 
            // dtpFechaHasta
            // 
            dtpFechaHasta.Dock = DockStyle.Fill;
            dtpFechaHasta.Font = new Font("Segoe UI", 9F);
            dtpFechaHasta.Format = DateTimePickerFormat.Short;
            dtpFechaHasta.Location = new Point(378, 27);
            dtpFechaHasta.Name = "dtpFechaHasta";
            dtpFechaHasta.Size = new Size(369, 31);
            dtpFechaHasta.TabIndex = 1;
            // 
            // lblProductor
            // 
            lblProductor.AutoSize = true;
            lblProductor.Dock = DockStyle.Fill;
            lblProductor.Font = new Font("Segoe UI", 9F);
            lblProductor.ForeColor = Color.FromArgb(128, 105, 82);
            lblProductor.Location = new Point(753, 0);
            lblProductor.Name = "lblProductor";
            lblProductor.Size = new Size(370, 24);
            lblProductor.TabIndex = 2;
            lblProductor.Text = "Productor";
            // 
            // cboProductor
            // 
            cboProductor.Dock = DockStyle.Fill;
            cboProductor.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProductor.Font = new Font("Segoe UI", 9F);
            cboProductor.FormattingEnabled = true;
            cboProductor.Location = new Point(753, 27);
            cboProductor.Name = "cboProductor";
            cboProductor.Size = new Size(370, 33);
            cboProductor.TabIndex = 2;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Dock = DockStyle.Fill;
            lblProducto.Font = new Font("Segoe UI", 9F);
            lblProducto.ForeColor = Color.FromArgb(128, 105, 82);
            lblProducto.Location = new Point(3, 66);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(369, 24);
            lblProducto.TabIndex = 3;
            lblProducto.Text = "Producto";
            // 
            // cboProducto
            // 
            cboProducto.Dock = DockStyle.Fill;
            cboProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProducto.Font = new Font("Segoe UI", 9F);
            cboProducto.FormattingEnabled = true;
            cboProducto.Location = new Point(3, 93);
            cboProducto.Name = "cboProducto";
            cboProducto.Size = new Size(369, 33);
            cboProducto.TabIndex = 3;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Dock = DockStyle.Fill;
            lblEstado.Font = new Font("Segoe UI", 9F);
            lblEstado.ForeColor = Color.FromArgb(128, 105, 82);
            lblEstado.Location = new Point(378, 66);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(369, 24);
            lblEstado.TabIndex = 4;
            lblEstado.Text = "Estado";
            // 
            // cboEstado
            // 
            cboEstado.Dock = DockStyle.Fill;
            cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEstado.Font = new Font("Segoe UI", 9F);
            cboEstado.FormattingEnabled = true;
            cboEstado.Location = new Point(378, 93);
            cboEstado.Name = "cboEstado";
            cboEstado.Size = new Size(369, 33);
            cboEstado.TabIndex = 4;
            // 
            // panelBotonesDerecha
            // 
            panelBotonesDerecha.Controls.Add(layoutBotonesDerecha);
            panelBotonesDerecha.Dock = DockStyle.Fill;
            panelBotonesDerecha.Location = new Point(753, 69);
            panelBotonesDerecha.Name = "panelBotonesDerecha";
            layoutFiltros.SetRowSpan(panelBotonesDerecha, 2);
            panelBotonesDerecha.Size = new Size(370, 61);
            panelBotonesDerecha.TabIndex = 5;
            // 
            // layoutBotonesDerecha
            // 
            layoutBotonesDerecha.ColumnCount = 3;
            layoutBotonesDerecha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layoutBotonesDerecha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layoutBotonesDerecha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            layoutBotonesDerecha.Controls.Add(btnBuscar, 0, 0);
            layoutBotonesDerecha.Controls.Add(btnLimpiarFiltros, 1, 0);
            layoutBotonesDerecha.Controls.Add(btnModificarEstado, 2, 0);
            layoutBotonesDerecha.Dock = DockStyle.Fill;
            layoutBotonesDerecha.Location = new Point(0, 0);
            layoutBotonesDerecha.Name = "layoutBotonesDerecha";
            layoutBotonesDerecha.Padding = new Padding(6, 8, 6, 8);
            layoutBotonesDerecha.RowCount = 1;
            layoutBotonesDerecha.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutBotonesDerecha.Size = new Size(370, 61);
            layoutBotonesDerecha.TabIndex = 0;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = Color.FromArgb(92, 122, 42);
            btnBuscar.Dock = DockStyle.Fill;
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBuscar.ForeColor = Color.White;
            btnBuscar.Location = new Point(9, 11);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(113, 39);
            btnBuscar.TabIndex = 5;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = false;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // btnLimpiarFiltros
            // 
            btnLimpiarFiltros.BackColor = Color.FromArgb(160, 80, 20);
            btnLimpiarFiltros.Dock = DockStyle.Fill;
            btnLimpiarFiltros.FlatAppearance.BorderSize = 0;
            btnLimpiarFiltros.FlatStyle = FlatStyle.Flat;
            btnLimpiarFiltros.Font = new Font("Segoe UI", 9F);
            btnLimpiarFiltros.ForeColor = Color.White;
            btnLimpiarFiltros.Location = new Point(128, 11);
            btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            btnLimpiarFiltros.Size = new Size(113, 39);
            btnLimpiarFiltros.TabIndex = 6;
            btnLimpiarFiltros.Text = "Limpiar filtros";
            btnLimpiarFiltros.UseVisualStyleBackColor = false;
            btnLimpiarFiltros.Click += btnLimpiarFiltros_Click;
            // 
            // btnModificarEstado
            // 
            btnModificarEstado.BackColor = Color.FromArgb(58, 38, 18);
            btnModificarEstado.Dock = DockStyle.Fill;
            btnModificarEstado.FlatAppearance.BorderSize = 0;
            btnModificarEstado.FlatStyle = FlatStyle.Flat;
            btnModificarEstado.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnModificarEstado.ForeColor = Color.White;
            btnModificarEstado.Location = new Point(247, 11);
            btnModificarEstado.Name = "btnModificarEstado";
            btnModificarEstado.Size = new Size(114, 39);
            btnModificarEstado.TabIndex = 7;
            btnModificarEstado.Text = "Modificar estado";
            btnModificarEstado.UseVisualStyleBackColor = false;
            btnModificarEstado.Click += btnModificarEstado_Click;
            // 
            // groupResultados
            // 
            groupResultados.BackColor = Color.White;
            groupResultados.Controls.Add(dgvEntregas);
            groupResultados.Dock = DockStyle.Fill;
            groupResultados.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupResultados.ForeColor = Color.FromArgb(80, 55, 30);
            groupResultados.Location = new Point(23, 223);
            groupResultados.Margin = new Padding(3, 3, 3, 10);
            groupResultados.Name = "groupResultados";
            groupResultados.Padding = new Padding(14, 16, 14, 14);
            groupResultados.Size = new Size(1154, 259);
            groupResultados.TabIndex = 1;
            groupResultados.TabStop = false;
            groupResultados.Text = "Resultados";
            // 
            // dgvEntregas
            // 
            dgvEntregas.AllowUserToAddRows = false;
            dgvEntregas.AllowUserToDeleteRows = false;
            dgvEntregas.AllowUserToResizeRows = false;
            dgvEntregas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEntregas.BackgroundColor = Color.White;
            dgvEntregas.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(58, 38, 18);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(58, 38, 18);
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvEntregas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvEntregas.ColumnHeadersHeight = 40;
            dgvEntregas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvEntregas.Columns.AddRange(new DataGridViewColumn[] { colEntregaId, colNumero, colFecha, colProductor, colProducto, colSubproducto, colKilos, colEstado, colObservaciones });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(80, 55, 30);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(230, 218, 200);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(38, 22, 10);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvEntregas.DefaultCellStyle = dataGridViewCellStyle2;
            dgvEntregas.Dock = DockStyle.Fill;
            dgvEntregas.EnableHeadersVisualStyles = false;
            dgvEntregas.GridColor = Color.FromArgb(222, 210, 194);
            dgvEntregas.Location = new Point(14, 43);
            dgvEntregas.MultiSelect = false;
            dgvEntregas.Name = "dgvEntregas";
            dgvEntregas.ReadOnly = true;
            dgvEntregas.RowHeadersVisible = false;
            dgvEntregas.RowHeadersWidth = 62;
            dgvEntregas.RowTemplate.Height = 34;
            dgvEntregas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEntregas.Size = new Size(1126, 202);
            dgvEntregas.TabIndex = 0;
            // 
            // colEntregaId
            // 
            colEntregaId.HeaderText = "ID";
            colEntregaId.MinimumWidth = 6;
            colEntregaId.Name = "colEntregaId";
            colEntregaId.ReadOnly = true;
            colEntregaId.Visible = false;
            // 
            // colNumero
            // 
            colNumero.FillWeight = 80F;
            colNumero.HeaderText = "Número";
            colNumero.MinimumWidth = 95;
            colNumero.Name = "colNumero";
            colNumero.ReadOnly = true;
            // 
            // colFecha
            // 
            colFecha.FillWeight = 80F;
            colFecha.HeaderText = "Fecha";
            colFecha.MinimumWidth = 95;
            colFecha.Name = "colFecha";
            colFecha.ReadOnly = true;
            // 
            // colProductor
            // 
            colProductor.FillWeight = 160F;
            colProductor.HeaderText = "Productor";
            colProductor.MinimumWidth = 150;
            colProductor.Name = "colProductor";
            colProductor.ReadOnly = true;
            // 
            // colProducto
            // 
            colProducto.FillWeight = 110F;
            colProducto.HeaderText = "Producto";
            colProducto.MinimumWidth = 110;
            colProducto.Name = "colProducto";
            colProducto.ReadOnly = true;
            // 
            // colSubproducto
            // 
            colSubproducto.FillWeight = 110F;
            colSubproducto.HeaderText = "Subproducto";
            colSubproducto.MinimumWidth = 110;
            colSubproducto.Name = "colSubproducto";
            colSubproducto.ReadOnly = true;
            // 
            // colKilos
            // 
            colKilos.FillWeight = 75F;
            colKilos.HeaderText = "Kilos";
            colKilos.MinimumWidth = 85;
            colKilos.Name = "colKilos";
            colKilos.ReadOnly = true;
            // 
            // colEstado
            // 
            colEstado.FillWeight = 95F;
            colEstado.HeaderText = "Estado";
            colEstado.MinimumWidth = 105;
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            // 
            // colObservaciones
            // 
            colObservaciones.FillWeight = 200F;
            colObservaciones.HeaderText = "Observaciones";
            colObservaciones.MinimumWidth = 150;
            colObservaciones.Name = "colObservaciones";
            colObservaciones.ReadOnly = true;
            // 
            // panelInferior
            // 
            panelInferior.BackColor = Color.White;
            panelInferior.Controls.Add(btnCerrar);
            panelInferior.Controls.Add(btnHistorial);
            panelInferior.Controls.Add(panelSeparadorInferior);
            panelInferior.Dock = DockStyle.Bottom;
            panelInferior.Location = new Point(0, 607);
            panelInferior.Name = "panelInferior";
            panelInferior.Size = new Size(1200, 61);
            panelInferior.TabIndex = 2;
            // 
            // btnCerrar
            // 
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.BackColor = Color.FromArgb(92, 122, 42);
            btnCerrar.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.Location = new Point(1088, 14);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(100, 38);
            btnCerrar.TabIndex = 0;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = false;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // btnHistorial
            // 
            btnHistorial.BackColor = Color.FromArgb(58, 38, 18);
            btnHistorial.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);
            btnHistorial.FlatStyle = FlatStyle.Flat;
            btnHistorial.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnHistorial.ForeColor = Color.White;
            btnHistorial.Location = new Point(20, 14);
            btnHistorial.Name = "btnHistorial";
            btnHistorial.Size = new Size(269, 38);
            btnHistorial.TabIndex = 1;
            btnHistorial.Text = "Historial de movimientos";
            btnHistorial.UseVisualStyleBackColor = false;
            btnHistorial.Click += btnHistorial_Click;
            // 
            // panelSeparadorInferior
            // 
            panelSeparadorInferior.BackColor = Color.FromArgb(210, 195, 175);
            panelSeparadorInferior.Dock = DockStyle.Top;
            panelSeparadorInferior.Location = new Point(0, 0);
            panelSeparadorInferior.Name = "panelSeparadorInferior";
            panelSeparadorInferior.Size = new Size(1200, 1);
            panelSeparadorInferior.TabIndex = 3;
            // 
            // ConsultaEntregasForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            ClientSize = new Size(1200, 668);
            Controls.Add(layoutPrincipal);
            Controls.Add(panelInferior);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(44, 28, 16);
            MinimumSize = new Size(1000, 600);
            Name = "ConsultaEntregasForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Consultar entregas";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            layoutPrincipal.ResumeLayout(false);
            groupFiltros.ResumeLayout(false);
            layoutFiltros.ResumeLayout(false);
            layoutFiltros.PerformLayout();
            panelBotonesDerecha.ResumeLayout(false);
            layoutBotonesDerecha.ResumeLayout(false);
            groupResultados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvEntregas).EndInit();
            panelInferior.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}