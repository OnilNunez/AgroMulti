namespace CentroFermentacionSecado
{
    partial class MainMenu
    {
        private System.ComponentModel.IContainer components = null;

        // ── Barra de menú ─────────────────────────────────────────────
        private System.Windows.Forms.MenuStrip menuStripPrincipal;
        private System.Windows.Forms.ToolStrip toolStripAccesoRapido;

        // Menú Archivo y submenús
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem exportarProductoresMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarProductoresExcelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarProductoresPDFMenuItem;

        private System.Windows.Forms.ToolStripMenuItem exportarEntregasMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarEntregasExcelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarEntregasPDFMenuItem;

        private System.Windows.Forms.ToolStripMenuItem exportarHistorialMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarHistorialExcelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarHistorialPDFMenuItem;

        private System.Windows.Forms.ToolStripSeparator separadorArchivoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;

        // Menú Entregas
        private System.Windows.Forms.ToolStripMenuItem entregasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nuevaEntregaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consultarEntregasToolStripMenuItem;

        // Menú Productores
        private System.Windows.Forms.ToolStripMenuItem productoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listaProductoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem agregarProductorToolStripMenuItem;

        // Menú Ayuda
        private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;

        // ToolStrip de acceso rápido
        private System.Windows.Forms.ToolStripButton btnToolNuevaEntrega;
        private System.Windows.Forms.ToolStripButton btnToolProductores;

        // ── Paneles / estructura ──────────────────────────────────────
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel panelAccentStrip;

        // ── Dashboard ─────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox groupSummary;
        private System.Windows.Forms.TableLayoutPanel summaryLayout;

        private System.Windows.Forms.Panel panelTotalKilos;
        private System.Windows.Forms.Label lblTotalKilosTitle;
        private System.Windows.Forms.Label lblTotalKilosValue;

        private System.Windows.Forms.Panel panelTotalDeliveries;
        private System.Windows.Forms.Label lblTotalDeliveriesTitle;
        private System.Windows.Forms.Label lblTotalDeliveriesValue;

        private System.Windows.Forms.Panel panelPending;
        private System.Windows.Forms.Label lblPendingTitle;
        private System.Windows.Forms.Label lblPendingValue;

        private System.Windows.Forms.Panel panelCompleted;
        private System.Windows.Forms.Label lblCompletedTitle;
        private System.Windows.Forms.Label lblCompletedValue;

        private System.Windows.Forms.GroupBox groupRecentDeliveries;
        private System.Windows.Forms.DataGridView dgvRecentDeliveries;

        private System.Windows.Forms.DataGridViewTextBoxColumn colDeliveryNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProducer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKilos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle styleHeaders = new DataGridViewCellStyle();
            DataGridViewCellStyle styleDefault = new DataGridViewCellStyle();

            // ── Instancias ────────────────────────────────────────────
            menuStripPrincipal = new MenuStrip();
            toolStripAccesoRapido = new ToolStrip();

            archivoToolStripMenuItem = new ToolStripMenuItem();
            exportarToolStripMenuItem = new ToolStripMenuItem();

            exportarProductoresMenuItem = new ToolStripMenuItem();
            exportarProductoresExcelMenuItem = new ToolStripMenuItem();
            exportarProductoresPDFMenuItem = new ToolStripMenuItem();

            exportarEntregasMenuItem = new ToolStripMenuItem();
            exportarEntregasExcelMenuItem = new ToolStripMenuItem();
            exportarEntregasPDFMenuItem = new ToolStripMenuItem();

            exportarHistorialMenuItem = new ToolStripMenuItem();
            exportarHistorialExcelMenuItem = new ToolStripMenuItem();
            exportarHistorialPDFMenuItem = new ToolStripMenuItem();

            separadorArchivoMenuItem = new ToolStripSeparator();
            salirToolStripMenuItem = new ToolStripMenuItem();

            entregasToolStripMenuItem = new ToolStripMenuItem();
            nuevaEntregaToolStripMenuItem = new ToolStripMenuItem();
            consultarEntregasToolStripMenuItem = new ToolStripMenuItem();

            productoresToolStripMenuItem = new ToolStripMenuItem();
            listaProductoresToolStripMenuItem = new ToolStripMenuItem();
            agregarProductorToolStripMenuItem = new ToolStripMenuItem();

            ayudaToolStripMenuItem = new ToolStripMenuItem();
            acercaDeToolStripMenuItem = new ToolStripMenuItem();

            btnToolNuevaEntrega = new ToolStripButton();
            btnToolProductores = new ToolStripButton();

            panelHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            panelAccentStrip = new Panel();

            groupSummary = new GroupBox();
            summaryLayout = new TableLayoutPanel();

            panelTotalKilos = new Panel();
            lblTotalKilosTitle = new Label();
            lblTotalKilosValue = new Label();
            panelTotalDeliveries = new Panel();
            lblTotalDeliveriesTitle = new Label();
            lblTotalDeliveriesValue = new Label();
            panelPending = new Panel();
            lblPendingTitle = new Label();
            lblPendingValue = new Label();
            panelCompleted = new Panel();
            lblCompletedTitle = new Label();
            lblCompletedValue = new Label();

            groupRecentDeliveries = new GroupBox();
            dgvRecentDeliveries = new DataGridView();
            colDeliveryNumber = new DataGridViewTextBoxColumn();
            colProducer = new DataGridViewTextBoxColumn();
            colProduct = new DataGridViewTextBoxColumn();
            colDate = new DataGridViewTextBoxColumn();
            colKilos = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();

            menuStripPrincipal.SuspendLayout();
            toolStripAccesoRapido.SuspendLayout();
            panelHeader.SuspendLayout();
            groupSummary.SuspendLayout();
            summaryLayout.SuspendLayout();
            panelTotalKilos.SuspendLayout();
            panelTotalDeliveries.SuspendLayout();
            panelPending.SuspendLayout();
            panelCompleted.SuspendLayout();
            groupRecentDeliveries.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRecentDeliveries).BeginInit();
            SuspendLayout();

            // ─────────────────────────────────────────────────────────
            // menuStripPrincipal
            // ─────────────────────────────────────────────────────────
            menuStripPrincipal.BackColor = Color.FromArgb(38, 22, 10);
            menuStripPrincipal.ForeColor = Color.FromArgb(215, 195, 170);
            menuStripPrincipal.ImageScalingSize = new Size(24, 24);
            menuStripPrincipal.Items.AddRange(new ToolStripItem[]
            {
                archivoToolStripMenuItem,
                entregasToolStripMenuItem,
                productoresToolStripMenuItem,
                ayudaToolStripMenuItem
            });
            menuStripPrincipal.Location = new Point(0, 0);
            menuStripPrincipal.Name = "menuStripPrincipal";
            menuStripPrincipal.Size = new Size(1319, 33);
            menuStripPrincipal.TabIndex = 5;

            // ── Archivo ───────────────────────────────────────────────
            archivoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                exportarToolStripMenuItem,
                separadorArchivoMenuItem,
                salirToolStripMenuItem
            });
            archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            archivoToolStripMenuItem.Size = new Size(88, 29);
            archivoToolStripMenuItem.Text = "Archivo";

            // Exportar (padre)
            exportarToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                exportarProductoresMenuItem,
                exportarEntregasMenuItem,
                exportarHistorialMenuItem
            });
            exportarToolStripMenuItem.Name = "exportarToolStripMenuItem";
            exportarToolStripMenuItem.Size = new Size(200, 34);
            exportarToolStripMenuItem.Text = "Exportar";

            // Exportar → Productores
            exportarProductoresMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                exportarProductoresExcelMenuItem,
                exportarProductoresPDFMenuItem
            });
            exportarProductoresMenuItem.Name = "exportarProductoresMenuItem";
            exportarProductoresMenuItem.Size = new Size(200, 34);
            exportarProductoresMenuItem.Text = "Productores";

            exportarProductoresExcelMenuItem.Name = "exportarProductoresExcelMenuItem";
            exportarProductoresExcelMenuItem.Size = new Size(210, 34);
            exportarProductoresExcelMenuItem.Text = "Exportar a Excel";
            exportarProductoresExcelMenuItem.Click += ExportarProductoresExcel_Click;

            exportarProductoresPDFMenuItem.Name = "exportarProductoresPDFMenuItem";
            exportarProductoresPDFMenuItem.Size = new Size(210, 34);
            exportarProductoresPDFMenuItem.Text = "Exportar a PDF";
            exportarProductoresPDFMenuItem.Click += ExportarProductoresPDF_Click;

            // Exportar → Entregas
            exportarEntregasMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                exportarEntregasExcelMenuItem,
                exportarEntregasPDFMenuItem
            });
            exportarEntregasMenuItem.Name = "exportarEntregasMenuItem";
            exportarEntregasMenuItem.Size = new Size(200, 34);
            exportarEntregasMenuItem.Text = "Entregas";

            exportarEntregasExcelMenuItem.Name = "exportarEntregasExcelMenuItem";
            exportarEntregasExcelMenuItem.Size = new Size(210, 34);
            exportarEntregasExcelMenuItem.Text = "Exportar a Excel";
            exportarEntregasExcelMenuItem.Click += ExportarEntregasExcel_Click;

            exportarEntregasPDFMenuItem.Name = "exportarEntregasPDFMenuItem";
            exportarEntregasPDFMenuItem.Size = new Size(210, 34);
            exportarEntregasPDFMenuItem.Text = "Exportar a PDF";
            exportarEntregasPDFMenuItem.Click += ExportarEntregasPDF_Click;

            // Exportar → Historial
            exportarHistorialMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                exportarHistorialExcelMenuItem,
                exportarHistorialPDFMenuItem
            });
            exportarHistorialMenuItem.Name = "exportarHistorialMenuItem";
            exportarHistorialMenuItem.Size = new Size(200, 34);
            exportarHistorialMenuItem.Text = "Historial de movimientos";

            exportarHistorialExcelMenuItem.Name = "exportarHistorialExcelMenuItem";
            exportarHistorialExcelMenuItem.Size = new Size(210, 34);
            exportarHistorialExcelMenuItem.Text = "Exportar a Excel";
            exportarHistorialExcelMenuItem.Click += ExportarHistorialExcel_Click;

            exportarHistorialPDFMenuItem.Name = "exportarHistorialPDFMenuItem";
            exportarHistorialPDFMenuItem.Size = new Size(210, 34);
            exportarHistorialPDFMenuItem.Text = "Exportar a PDF";
            exportarHistorialPDFMenuItem.Click += ExportarHistorialPDF_Click;

            // Separador + Salir
            separadorArchivoMenuItem.Name = "separadorArchivoMenuItem";
            separadorArchivoMenuItem.Size = new Size(197, 6);

            salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            salirToolStripMenuItem.Size = new Size(200, 34);
            salirToolStripMenuItem.Text = "Salir";
            salirToolStripMenuItem.Click += SalirToolStripMenuItem_Click;

            // ── Entregas ──────────────────────────────────────────────
            entregasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                nuevaEntregaToolStripMenuItem,
                consultarEntregasToolStripMenuItem
            });
            entregasToolStripMenuItem.Name = "entregasToolStripMenuItem";
            entregasToolStripMenuItem.Size = new Size(96, 29);
            entregasToolStripMenuItem.Text = "Entregas";

            nuevaEntregaToolStripMenuItem.Name = "nuevaEntregaToolStripMenuItem";
            nuevaEntregaToolStripMenuItem.Size = new Size(229, 34);
            nuevaEntregaToolStripMenuItem.Text = "Nueva entrega";
            nuevaEntregaToolStripMenuItem.Click += NuevaEntrega_Click;

            consultarEntregasToolStripMenuItem.Name = "consultarEntregasToolStripMenuItem";
            consultarEntregasToolStripMenuItem.Size = new Size(229, 34);
            consultarEntregasToolStripMenuItem.Text = "Ver entregas";
            consultarEntregasToolStripMenuItem.Click += ConsultarEntregas_Click;

            // ── Productores ───────────────────────────────────────────
            productoresToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                listaProductoresToolStripMenuItem,
                agregarProductorToolStripMenuItem
            });
            productoresToolStripMenuItem.Name = "productoresToolStripMenuItem";
            productoresToolStripMenuItem.Size = new Size(124, 29);
            productoresToolStripMenuItem.Text = "Productores";

            listaProductoresToolStripMenuItem.Name = "listaProductoresToolStripMenuItem";
            listaProductoresToolStripMenuItem.Size = new Size(276, 34);
            listaProductoresToolStripMenuItem.Text = "Lista de productores";
            listaProductoresToolStripMenuItem.Click += ListaProductores_Click;

            agregarProductorToolStripMenuItem.Name = "agregarProductorToolStripMenuItem";
            agregarProductorToolStripMenuItem.Size = new Size(276, 34);
            agregarProductorToolStripMenuItem.Text = "Agregar productor";
            agregarProductorToolStripMenuItem.Click += AgregarProductor_Click;

            // ── Ayuda ─────────────────────────────────────────────────
            ayudaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                acercaDeToolStripMenuItem
            });
            ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            ayudaToolStripMenuItem.Size = new Size(79, 29);
            ayudaToolStripMenuItem.Text = "Ayuda";
            ayudaToolStripMenuItem.Click += ayudaToolStripMenuItem_Click;

            acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            acercaDeToolStripMenuItem.Size = new Size(270, 34);
            acercaDeToolStripMenuItem.Text = "Acerca de...";

            // ─────────────────────────────────────────────────────────
            // toolStripAccesoRapido
            // ─────────────────────────────────────────────────────────
            toolStripAccesoRapido.BackColor = Color.FromArgb(58, 38, 18);
            toolStripAccesoRapido.GripStyle = ToolStripGripStyle.Hidden;
            toolStripAccesoRapido.ImageScalingSize = new Size(24, 24);
            toolStripAccesoRapido.Items.AddRange(new ToolStripItem[]
            {
                btnToolNuevaEntrega,
                btnToolProductores
            });
            toolStripAccesoRapido.Location = new Point(0, 33);
            toolStripAccesoRapido.Name = "toolStripAccesoRapido";
            toolStripAccesoRapido.Size = new Size(1319, 34);
            toolStripAccesoRapido.TabIndex = 6;

            btnToolNuevaEntrega.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnToolNuevaEntrega.ForeColor = Color.White;
            btnToolNuevaEntrega.Name = "btnToolNuevaEntrega";
            btnToolNuevaEntrega.Size = new Size(131, 29);
            btnToolNuevaEntrega.Text = "Nueva entrega";
            btnToolNuevaEntrega.Click += NuevaEntrega_Click;

            btnToolProductores.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnToolProductores.ForeColor = Color.White;
            btnToolProductores.Name = "btnToolProductores";
            btnToolProductores.Size = new Size(112, 29);
            btnToolProductores.Text = "Productores";
            btnToolProductores.Click += ListaProductores_Click;

            // ─────────────────────────────────────────────────────────
            // panelHeader
            // ─────────────────────────────────────────────────────────
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(lblSubtitle);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(panelAccentStrip);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 67);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(22, 16, 22, 3);
            panelHeader.Size = new Size(1319, 111);
            panelHeader.TabIndex = 0;

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Snow;
            lblTitle.Location = new Point(22, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(582, 48);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Centro de Fermentación y Secado";

            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(185, 165, 140);
            lblSubtitle.Location = new Point(34, 64);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(292, 25);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Dashboard general y acceso rápido";

            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 105);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(1275, 3);
            panelAccentStrip.TabIndex = 4;

            // ─────────────────────────────────────────────────────────
            // groupSummary
            // ─────────────────────────────────────────────────────────
            groupSummary.BackColor = Color.FromArgb(245, 240, 232);
            groupSummary.Controls.Add(summaryLayout);
            groupSummary.Dock = DockStyle.Top;
            groupSummary.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupSummary.ForeColor = Color.FromArgb(80, 55, 30);
            groupSummary.Location = new Point(0, 178);
            groupSummary.Name = "groupSummary";
            groupSummary.Padding = new Padding(14, 16, 14, 12);
            groupSummary.Size = new Size(1319, 196);
            groupSummary.TabIndex = 1;
            groupSummary.TabStop = false;
            groupSummary.Text = "Resumen del día";

            summaryLayout.BackColor = Color.FromArgb(245, 240, 232);
            summaryLayout.ColumnCount = 4;
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            summaryLayout.Controls.Add(panelTotalKilos, 0, 0);
            summaryLayout.Controls.Add(panelTotalDeliveries, 1, 0);
            summaryLayout.Controls.Add(panelPending, 2, 0);
            summaryLayout.Controls.Add(panelCompleted, 3, 0);
            summaryLayout.Dock = DockStyle.Fill;
            summaryLayout.Location = new Point(14, 43);
            summaryLayout.Name = "summaryLayout";
            summaryLayout.RowCount = 1;
            summaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            summaryLayout.Size = new Size(1291, 141);
            summaryLayout.TabIndex = 0;

            // Card: Total kilos
            panelTotalKilos.BackColor = Color.White;
            panelTotalKilos.BorderStyle = BorderStyle.FixedSingle;
            panelTotalKilos.Controls.Add(lblTotalKilosValue);
            panelTotalKilos.Controls.Add(lblTotalKilosTitle);
            panelTotalKilos.Dock = DockStyle.Fill;
            panelTotalKilos.Location = new Point(10, 10);
            panelTotalKilos.Margin = new Padding(10);
            panelTotalKilos.Name = "panelTotalKilos";
            panelTotalKilos.Padding = new Padding(16, 14, 16, 14);
            panelTotalKilos.Size = new Size(302, 121);
            panelTotalKilos.TabIndex = 0;

            lblTotalKilosTitle.Dock = DockStyle.Top;
            lblTotalKilosTitle.Font = new Font("Segoe UI", 9F);
            lblTotalKilosTitle.ForeColor = Color.FromArgb(128, 105, 82);
            lblTotalKilosTitle.Location = new Point(16, 14);
            lblTotalKilosTitle.Name = "lblTotalKilosTitle";
            lblTotalKilosTitle.Size = new Size(268, 24);
            lblTotalKilosTitle.TabIndex = 0;
            lblTotalKilosTitle.Text = "Total kilos hoy";

            lblTotalKilosValue.Dock = DockStyle.Fill;
            lblTotalKilosValue.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTotalKilosValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblTotalKilosValue.Location = new Point(16, 38);
            lblTotalKilosValue.Name = "lblTotalKilosValue";
            lblTotalKilosValue.Size = new Size(268, 67);
            lblTotalKilosValue.TabIndex = 1;
            lblTotalKilosValue.Text = "0";
            lblTotalKilosValue.TextAlign = ContentAlignment.MiddleLeft;

            // Card: Total entregas
            panelTotalDeliveries.BackColor = Color.White;
            panelTotalDeliveries.BorderStyle = BorderStyle.FixedSingle;
            panelTotalDeliveries.Controls.Add(lblTotalDeliveriesValue);
            panelTotalDeliveries.Controls.Add(lblTotalDeliveriesTitle);
            panelTotalDeliveries.Dock = DockStyle.Fill;
            panelTotalDeliveries.Location = new Point(332, 10);
            panelTotalDeliveries.Margin = new Padding(10);
            panelTotalDeliveries.Name = "panelTotalDeliveries";
            panelTotalDeliveries.Padding = new Padding(16, 14, 16, 14);
            panelTotalDeliveries.Size = new Size(302, 121);
            panelTotalDeliveries.TabIndex = 1;

            lblTotalDeliveriesTitle.Dock = DockStyle.Top;
            lblTotalDeliveriesTitle.Font = new Font("Segoe UI", 9F);
            lblTotalDeliveriesTitle.ForeColor = Color.FromArgb(128, 105, 82);
            lblTotalDeliveriesTitle.Location = new Point(16, 14);
            lblTotalDeliveriesTitle.Name = "lblTotalDeliveriesTitle";
            lblTotalDeliveriesTitle.Size = new Size(268, 24);
            lblTotalDeliveriesTitle.TabIndex = 0;
            lblTotalDeliveriesTitle.Text = "Total entregas";

            lblTotalDeliveriesValue.Dock = DockStyle.Fill;
            lblTotalDeliveriesValue.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTotalDeliveriesValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblTotalDeliveriesValue.Location = new Point(16, 38);
            lblTotalDeliveriesValue.Name = "lblTotalDeliveriesValue";
            lblTotalDeliveriesValue.Size = new Size(268, 67);
            lblTotalDeliveriesValue.TabIndex = 1;
            lblTotalDeliveriesValue.Text = "0";
            lblTotalDeliveriesValue.TextAlign = ContentAlignment.MiddleLeft;

            // Card: Pendientes
            panelPending.BackColor = Color.White;
            panelPending.BorderStyle = BorderStyle.FixedSingle;
            panelPending.Controls.Add(lblPendingValue);
            panelPending.Controls.Add(lblPendingTitle);
            panelPending.Dock = DockStyle.Fill;
            panelPending.Location = new Point(654, 10);
            panelPending.Margin = new Padding(10);
            panelPending.Name = "panelPending";
            panelPending.Padding = new Padding(16, 14, 16, 14);
            panelPending.Size = new Size(302, 121);
            panelPending.TabIndex = 2;

            lblPendingTitle.Dock = DockStyle.Top;
            lblPendingTitle.Font = new Font("Segoe UI", 9F);
            lblPendingTitle.ForeColor = Color.FromArgb(128, 105, 82);
            lblPendingTitle.Location = new Point(16, 14);
            lblPendingTitle.Name = "lblPendingTitle";
            lblPendingTitle.Size = new Size(268, 24);
            lblPendingTitle.TabIndex = 0;
            lblPendingTitle.Text = "Pendientes";

            lblPendingValue.Dock = DockStyle.Fill;
            lblPendingValue.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblPendingValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblPendingValue.Location = new Point(16, 38);
            lblPendingValue.Name = "lblPendingValue";
            lblPendingValue.Size = new Size(268, 67);
            lblPendingValue.TabIndex = 1;
            lblPendingValue.Text = "0";
            lblPendingValue.TextAlign = ContentAlignment.MiddleLeft;

            // Card: Completadas
            panelCompleted.BackColor = Color.White;
            panelCompleted.BorderStyle = BorderStyle.FixedSingle;
            panelCompleted.Controls.Add(lblCompletedValue);
            panelCompleted.Controls.Add(lblCompletedTitle);
            panelCompleted.Dock = DockStyle.Fill;
            panelCompleted.Location = new Point(976, 10);
            panelCompleted.Margin = new Padding(10);
            panelCompleted.Name = "panelCompleted";
            panelCompleted.Padding = new Padding(16, 14, 16, 14);
            panelCompleted.Size = new Size(305, 121);
            panelCompleted.TabIndex = 3;

            lblCompletedTitle.Dock = DockStyle.Top;
            lblCompletedTitle.Font = new Font("Segoe UI", 9F);
            lblCompletedTitle.ForeColor = Color.FromArgb(128, 105, 82);
            lblCompletedTitle.Location = new Point(16, 14);
            lblCompletedTitle.Name = "lblCompletedTitle";
            lblCompletedTitle.Size = new Size(271, 24);
            lblCompletedTitle.TabIndex = 0;
            lblCompletedTitle.Text = "Completadas";

            lblCompletedValue.Dock = DockStyle.Fill;
            lblCompletedValue.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblCompletedValue.ForeColor = Color.FromArgb(92, 122, 42);
            lblCompletedValue.Location = new Point(16, 38);
            lblCompletedValue.Name = "lblCompletedValue";
            lblCompletedValue.Size = new Size(271, 67);
            lblCompletedValue.TabIndex = 1;
            lblCompletedValue.Text = "0";
            lblCompletedValue.TextAlign = ContentAlignment.MiddleLeft;

            // ─────────────────────────────────────────────────────────
            // groupRecentDeliveries
            // ─────────────────────────────────────────────────────────
            groupRecentDeliveries.BackColor = Color.FromArgb(245, 240, 232);
            groupRecentDeliveries.Controls.Add(dgvRecentDeliveries);
            groupRecentDeliveries.Dock = DockStyle.Fill;
            groupRecentDeliveries.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupRecentDeliveries.ForeColor = Color.FromArgb(80, 55, 30);
            groupRecentDeliveries.Location = new Point(0, 374);
            groupRecentDeliveries.Name = "groupRecentDeliveries";
            groupRecentDeliveries.Padding = new Padding(14, 16, 14, 14);
            groupRecentDeliveries.Size = new Size(1319, 304);
            groupRecentDeliveries.TabIndex = 2;
            groupRecentDeliveries.TabStop = false;
            groupRecentDeliveries.Text = "Entregas recientes";

            // ─────────────────────────────────────────────────────────
            // dgvRecentDeliveries
            // ─────────────────────────────────────────────────────────
            dgvRecentDeliveries.AllowUserToAddRows = false;
            dgvRecentDeliveries.AllowUserToDeleteRows = false;
            dgvRecentDeliveries.AllowUserToResizeRows = false;
            dgvRecentDeliveries.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRecentDeliveries.BackgroundColor = Color.White;
            dgvRecentDeliveries.BorderStyle = BorderStyle.None;

            styleHeaders.Alignment = DataGridViewContentAlignment.MiddleLeft;
            styleHeaders.BackColor = Color.FromArgb(58, 38, 18);
            styleHeaders.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            styleHeaders.ForeColor = Color.White;
            styleHeaders.SelectionBackColor = Color.FromArgb(58, 38, 18);
            styleHeaders.SelectionForeColor = Color.White;
            styleHeaders.WrapMode = DataGridViewTriState.True;
            dgvRecentDeliveries.ColumnHeadersDefaultCellStyle = styleHeaders;

            dgvRecentDeliveries.ColumnHeadersHeight = 40;
            dgvRecentDeliveries.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvRecentDeliveries.Columns.AddRange(new DataGridViewColumn[]
            {
                colDeliveryNumber, colProducer, colProduct, colDate, colKilos, colStatus
            });

            styleDefault.Alignment = DataGridViewContentAlignment.MiddleLeft;
            styleDefault.BackColor = Color.White;
            styleDefault.Font = new Font("Segoe UI", 9F);
            styleDefault.ForeColor = Color.FromArgb(80, 55, 30);
            styleDefault.SelectionBackColor = Color.FromArgb(230, 218, 200);
            styleDefault.SelectionForeColor = Color.FromArgb(38, 22, 10);
            styleDefault.WrapMode = DataGridViewTriState.False;
            dgvRecentDeliveries.DefaultCellStyle = styleDefault;

            dgvRecentDeliveries.Dock = DockStyle.Fill;
            dgvRecentDeliveries.EnableHeadersVisualStyles = false;
            dgvRecentDeliveries.GridColor = Color.FromArgb(222, 210, 194);
            dgvRecentDeliveries.Location = new Point(14, 43);
            dgvRecentDeliveries.MultiSelect = false;
            dgvRecentDeliveries.Name = "dgvRecentDeliveries";
            dgvRecentDeliveries.ReadOnly = true;
            dgvRecentDeliveries.RowHeadersVisible = false;
            dgvRecentDeliveries.RowHeadersWidth = 62;
            dgvRecentDeliveries.RowTemplate.Height = 34;
            dgvRecentDeliveries.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecentDeliveries.Size = new Size(1291, 247);
            dgvRecentDeliveries.TabIndex = 0;

            colDeliveryNumber.HeaderText = "Número";
            colDeliveryNumber.MinimumWidth = 100;
            colDeliveryNumber.Name = "colDeliveryNumber";
            colDeliveryNumber.ReadOnly = true;

            colProducer.HeaderText = "Productor";
            colProducer.MinimumWidth = 160;
            colProducer.Name = "colProducer";
            colProducer.ReadOnly = true;

            colProduct.HeaderText = "Producto";
            colProduct.MinimumWidth = 130;
            colProduct.Name = "colProduct";
            colProduct.ReadOnly = true;

            colDate.HeaderText = "Fecha";
            colDate.MinimumWidth = 110;
            colDate.Name = "colDate";
            colDate.ReadOnly = true;

            colKilos.HeaderText = "Kilos";
            colKilos.MinimumWidth = 100;
            colKilos.Name = "colKilos";
            colKilos.ReadOnly = true;

            colStatus.HeaderText = "Estado";
            colStatus.MinimumWidth = 120;
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;

            // ─────────────────────────────────────────────────────────
            // MainMenu Form
            // ─────────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            ClientSize = new Size(1319, 678);
            Controls.Add(groupRecentDeliveries);
            Controls.Add(groupSummary);
            Controls.Add(panelHeader);
            Controls.Add(toolStripAccesoRapido);
            Controls.Add(menuStripPrincipal);
            Font = new Font("Segoe UI", 9F);
            MainMenuStrip = menuStripPrincipal;
            MinimumSize = new Size(1140, 720);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AgroMulti - Centro de Fermentación y Secado";

            menuStripPrincipal.ResumeLayout(false);
            menuStripPrincipal.PerformLayout();
            toolStripAccesoRapido.ResumeLayout(false);
            toolStripAccesoRapido.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            groupSummary.ResumeLayout(false);
            summaryLayout.ResumeLayout(false);
            panelTotalKilos.ResumeLayout(false);
            panelTotalDeliveries.ResumeLayout(false);
            panelPending.ResumeLayout(false);
            panelCompleted.ResumeLayout(false);
            groupRecentDeliveries.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRecentDeliveries).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}