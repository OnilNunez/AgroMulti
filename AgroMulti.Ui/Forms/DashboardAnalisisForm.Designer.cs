using System.Drawing;
using System.Windows.Forms;

namespace AgroMulti.Ui.Forms
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Estructura ────────────────────────────────────────────────
        private Panel panelHeader;
        private Panel panelAccentStrip;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel panelContenido;
        private TableLayoutPanel tableLayoutCharts;

        // ── 6 gráficos ────────────────────────────────────────────────
        private GroupBox groupKilosMes;
        private ScottPlot.FormsPlot fpKilosMes;
        private GroupBox groupEstados;
        private ScottPlot.FormsPlot fpEstados;
        private GroupBox groupProductores;
        private ScottPlot.FormsPlot fpProductores;
        private GroupBox groupProductos;
        private ScottPlot.FormsPlot fpProductos;
        private GroupBox groupDiaSemana;
        private ScottPlot.FormsPlot fpDiaSemana;
        private GroupBox groupKilosSecos;
        private ScottPlot.FormsPlot fpKilosSecos;

        // ── Footer ────────────────────────────────────────────────────
        private Panel panelFooter;
        private Label lblAnio;
        private ComboBox cmbAnio;
        private Button btnRefrescar;
        private Button btnExportar;
        private Label lblEstado;
        private Button btnCerrar;

        // ── Menú contextual ───────────────────────────────────────────
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

            panelHeader = new Panel();
            panelAccentStrip = new Panel();
            lblSubtitle = new Label();
            lblTitle = new Label();
            panelContenido = new Panel();
            tableLayoutCharts = new TableLayoutPanel();

            groupKilosMes = new GroupBox();
            fpKilosMes = new ScottPlot.FormsPlot();
            groupEstados = new GroupBox();
            fpEstados = new ScottPlot.FormsPlot();
            groupProductores = new GroupBox();
            fpProductores = new ScottPlot.FormsPlot();
            groupProductos = new GroupBox();
            fpProductos = new ScottPlot.FormsPlot();
            groupDiaSemana = new GroupBox();
            fpDiaSemana = new ScottPlot.FormsPlot();
            groupKilosSecos = new GroupBox();
            fpKilosSecos = new ScottPlot.FormsPlot();

            panelFooter = new Panel();
            lblAnio = new Label();
            cmbAnio = new ComboBox();
            btnRefrescar = new Button();
            btnExportar = new Button();
            lblEstado = new Label();
            btnCerrar = new Button();

            ctxExportar = new ContextMenuStrip(components);
            itemExportarExcel = new ToolStripMenuItem();
            itemExportarPDF = new ToolStripMenuItem();

            panelHeader.SuspendLayout();
            panelContenido.SuspendLayout();
            tableLayoutCharts.SuspendLayout();
            groupKilosMes.SuspendLayout();
            groupEstados.SuspendLayout();
            groupProductores.SuspendLayout();
            groupProductos.SuspendLayout();
            groupDiaSemana.SuspendLayout();
            groupKilosSecos.SuspendLayout();
            panelFooter.SuspendLayout();
            ctxExportar.SuspendLayout();
            SuspendLayout();

            // ─────────────────────────────────────────────────────────
            // panelHeader
            // ─────────────────────────────────────────────────────────
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(panelAccentStrip);
            panelHeader.Controls.Add(lblSubtitle);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(22, 14, 22, 3);
            panelHeader.Size = new Size(1600, 94);
            panelHeader.TabIndex = 5;

            panelAccentStrip.BackColor = Color.FromArgb(92, 122, 42);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(22, 88);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(1556, 3);
            panelAccentStrip.TabIndex = 0;

            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Snow;
            lblTitle.Location = new Point(22, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(367, 46);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "Dashboard de análisis";

            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(185, 165, 140);
            lblSubtitle.Location = new Point(34, 57);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(445, 25);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Indicadores de entregas, productores y procesamiento";

            // ─────────────────────────────────────────────────────────
            // panelContenido
            // ─────────────────────────────────────────────────────────
            panelContenido.BackColor = Color.FromArgb(245, 240, 232);
            panelContenido.Controls.Add(tableLayoutCharts);
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Location = new Point(0, 94);
            panelContenido.Name = "panelContenido";
            panelContenido.Padding = new Padding(10);
            panelContenido.Size = new Size(1600, 894);
            panelContenido.TabIndex = 0;

            // ─────────────────────────────────────────────────────────
            // tableLayoutCharts  (2 cols × 3 filas, todas Fill)
            // ─────────────────────────────────────────────────────────
            tableLayoutCharts.ColumnCount = 2;
            tableLayoutCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutCharts.Controls.Add(groupKilosMes, 0, 0);
            tableLayoutCharts.Controls.Add(groupEstados, 1, 0);
            tableLayoutCharts.Controls.Add(groupProductores, 0, 1);
            tableLayoutCharts.Controls.Add(groupProductos, 1, 1);
            tableLayoutCharts.Controls.Add(groupDiaSemana, 0, 2);
            tableLayoutCharts.Controls.Add(groupKilosSecos, 1, 2);
            tableLayoutCharts.Dock = DockStyle.Fill;
            tableLayoutCharts.Location = new Point(10, 10);
            tableLayoutCharts.Name = "tableLayoutCharts";
            tableLayoutCharts.Padding = new Padding(2);
            tableLayoutCharts.RowCount = 3;
            tableLayoutCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tableLayoutCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tableLayoutCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));
            tableLayoutCharts.Size = new Size(1580, 874);
            tableLayoutCharts.TabIndex = 0;

            // ─────────────────────────────────────────────────────────
            // Gráfico 1 · Kilos por mes
            // ─────────────────────────────────────────────────────────
            fpKilosMes.BackColor = Color.FromArgb(245, 240, 232);
            fpKilosMes.Dock = DockStyle.Fill;
            fpKilosMes.Location = new Point(6, 28);
            fpKilosMes.Margin = new Padding(6, 5, 6, 5);
            fpKilosMes.Name = "fpKilosMes";
            fpKilosMes.Size = new Size(756, 244);
            fpKilosMes.TabIndex = 0;

            groupKilosMes.BackColor = Color.White;
            groupKilosMes.Controls.Add(fpKilosMes);
            groupKilosMes.Dock = DockStyle.Fill;
            groupKilosMes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupKilosMes.ForeColor = Color.FromArgb(80, 55, 30);
            groupKilosMes.Location = new Point(7, 7);
            groupKilosMes.Margin = new Padding(5);
            groupKilosMes.Name = "groupKilosMes";
            groupKilosMes.Padding = new Padding(6, 4, 6, 6);
            groupKilosMes.Size = new Size(768, 278);
            groupKilosMes.TabIndex = 0;
            groupKilosMes.TabStop = false;
            groupKilosMes.Text = "Kilos recibidos por mes";

            // ─────────────────────────────────────────────────────────
            // Gráfico 2 · Estados
            // ─────────────────────────────────────────────────────────
            fpEstados.BackColor = Color.FromArgb(245, 240, 232);
            fpEstados.Dock = DockStyle.Fill;
            fpEstados.Location = new Point(6, 28);
            fpEstados.Margin = new Padding(6, 5, 6, 5);
            fpEstados.Name = "fpEstados";
            fpEstados.Size = new Size(756, 244);
            fpEstados.TabIndex = 0;

            groupEstados.BackColor = Color.White;
            groupEstados.Controls.Add(fpEstados);
            groupEstados.Dock = DockStyle.Fill;
            groupEstados.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupEstados.ForeColor = Color.FromArgb(80, 55, 30);
            groupEstados.Location = new Point(785, 7);
            groupEstados.Margin = new Padding(5);
            groupEstados.Name = "groupEstados";
            groupEstados.Padding = new Padding(6, 4, 6, 6);
            groupEstados.Size = new Size(768, 278);
            groupEstados.TabIndex = 1;
            groupEstados.TabStop = false;
            groupEstados.Text = "Distribución de estados";

            // ─────────────────────────────────────────────────────────
            // Gráfico 3 · Top 5 productores
            // ─────────────────────────────────────────────────────────
            fpProductores.BackColor = Color.FromArgb(245, 240, 232);
            fpProductores.Dock = DockStyle.Fill;
            fpProductores.Location = new Point(6, 28);
            fpProductores.Margin = new Padding(6, 5, 6, 5);
            fpProductores.Name = "fpProductores";
            fpProductores.Size = new Size(756, 244);
            fpProductores.TabIndex = 0;

            groupProductores.BackColor = Color.White;
            groupProductores.Controls.Add(fpProductores);
            groupProductores.Dock = DockStyle.Fill;
            groupProductores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupProductores.ForeColor = Color.FromArgb(80, 55, 30);
            groupProductores.Location = new Point(7, 295);
            groupProductores.Margin = new Padding(5);
            groupProductores.Name = "groupProductores";
            groupProductores.Padding = new Padding(6, 4, 6, 6);
            groupProductores.Size = new Size(768, 278);
            groupProductores.TabIndex = 2;
            groupProductores.TabStop = false;
            groupProductores.Text = "Top 5 productores por kilos";

            // ─────────────────────────────────────────────────────────
            // Gráfico 4 · Volumen por producto
            // ─────────────────────────────────────────────────────────
            fpProductos.BackColor = Color.FromArgb(245, 240, 232);
            fpProductos.Dock = DockStyle.Fill;
            fpProductos.Location = new Point(6, 28);
            fpProductos.Margin = new Padding(6, 5, 6, 5);
            fpProductos.Name = "fpProductos";
            fpProductos.Size = new Size(756, 244);
            fpProductos.TabIndex = 0;

            groupProductos.BackColor = Color.White;
            groupProductos.Controls.Add(fpProductos);
            groupProductos.Dock = DockStyle.Fill;
            groupProductos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupProductos.ForeColor = Color.FromArgb(80, 55, 30);
            groupProductos.Location = new Point(785, 295);
            groupProductos.Margin = new Padding(5);
            groupProductos.Name = "groupProductos";
            groupProductos.Padding = new Padding(6, 4, 6, 6);
            groupProductos.Size = new Size(768, 278);
            groupProductos.TabIndex = 3;
            groupProductos.TabStop = false;
            groupProductos.Text = "Volumen por producto";

            // ─────────────────────────────────────────────────────────
            // Gráfico 5 · Actividad por día de la semana
            // ─────────────────────────────────────────────────────────
            fpDiaSemana.BackColor = Color.FromArgb(245, 240, 232);
            fpDiaSemana.Dock = DockStyle.Fill;
            fpDiaSemana.Location = new Point(6, 28);
            fpDiaSemana.Margin = new Padding(6, 5, 6, 5);
            fpDiaSemana.Name = "fpDiaSemana";
            fpDiaSemana.Size = new Size(756, 248);
            fpDiaSemana.TabIndex = 0;

            groupDiaSemana.BackColor = Color.White;
            groupDiaSemana.Controls.Add(fpDiaSemana);
            groupDiaSemana.Dock = DockStyle.Fill;
            groupDiaSemana.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupDiaSemana.ForeColor = Color.FromArgb(80, 55, 30);
            groupDiaSemana.Location = new Point(7, 583);
            groupDiaSemana.Margin = new Padding(5);
            groupDiaSemana.Name = "groupDiaSemana";
            groupDiaSemana.Padding = new Padding(6, 4, 6, 6);
            groupDiaSemana.Size = new Size(768, 282);
            groupDiaSemana.TabIndex = 4;
            groupDiaSemana.TabStop = false;
            groupDiaSemana.Text = "Actividad por día de la semana";

            // ─────────────────────────────────────────────────────────
            // Gráfico 6 · Kilos frescos vs secos
            // ─────────────────────────────────────────────────────────
            fpKilosSecos.BackColor = Color.FromArgb(245, 240, 232);
            fpKilosSecos.Dock = DockStyle.Fill;
            fpKilosSecos.Location = new Point(6, 28);
            fpKilosSecos.Margin = new Padding(6, 5, 6, 5);
            fpKilosSecos.Name = "fpKilosSecos";
            fpKilosSecos.Size = new Size(756, 248);
            fpKilosSecos.TabIndex = 0;

            groupKilosSecos.BackColor = Color.White;
            groupKilosSecos.Controls.Add(fpKilosSecos);
            groupKilosSecos.Dock = DockStyle.Fill;
            groupKilosSecos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupKilosSecos.ForeColor = Color.FromArgb(80, 55, 30);
            groupKilosSecos.Location = new Point(785, 583);
            groupKilosSecos.Margin = new Padding(5);
            groupKilosSecos.Name = "groupKilosSecos";
            groupKilosSecos.Padding = new Padding(6, 4, 6, 6);
            groupKilosSecos.Size = new Size(768, 282);
            groupKilosSecos.TabIndex = 5;
            groupKilosSecos.TabStop = false;
            groupKilosSecos.Text = "Kilos frescos vs secos";

            // ─────────────────────────────────────────────────────────
            // panelFooter
            // ─────────────────────────────────────────────────────────
            panelFooter.BackColor = Color.White;
            panelFooter.Controls.Add(lblAnio);
            panelFooter.Controls.Add(cmbAnio);
            panelFooter.Controls.Add(btnRefrescar);
            panelFooter.Controls.Add(btnExportar);
            panelFooter.Controls.Add(lblEstado);
            panelFooter.Controls.Add(btnCerrar);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 988);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(16, 8, 16, 8);
            panelFooter.Size = new Size(1600, 52);
            panelFooter.TabIndex = 1;

            lblAnio.AutoSize = true;
            lblAnio.Font = new Font("Segoe UI", 9F);
            lblAnio.ForeColor = Color.FromArgb(80, 55, 30);
            lblAnio.Location = new Point(16, 17);
            lblAnio.Name = "lblAnio";
            lblAnio.Size = new Size(49, 25);
            lblAnio.TabIndex = 0;
            lblAnio.Text = "Año:";

            cmbAnio.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAnio.FlatStyle = FlatStyle.System;
            cmbAnio.Font = new Font("Segoe UI", 9F);
            cmbAnio.Location = new Point(89, 11);
            cmbAnio.Name = "cmbAnio";
            cmbAnio.Size = new Size(90, 33);
            cmbAnio.TabIndex = 1;
            cmbAnio.SelectedIndexChanged += CmbAnio_SelectedIndexChanged;

            btnRefrescar.BackColor = Color.FromArgb(58, 38, 18);
            btnRefrescar.Cursor = Cursors.Hand;
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 22, 10);
            btnRefrescar.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 55, 28);
            btnRefrescar.FlatStyle = FlatStyle.Flat;
            btnRefrescar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRefrescar.ForeColor = Color.White;
            btnRefrescar.Location = new Point(202, 9);
            btnRefrescar.Name = "btnRefrescar";
            btnRefrescar.Size = new Size(115, 34);
            btnRefrescar.TabIndex = 2;
            btnRefrescar.Text = "⟳  Refrescar";
            btnRefrescar.UseVisualStyleBackColor = false;
            btnRefrescar.Click += BtnRefrescar_Click;

            btnExportar.BackColor = Color.FromArgb(130, 90, 35);
            btnExportar.Cursor = Cursors.Hand;
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 65, 20);
            btnExportar.FlatAppearance.MouseOverBackColor = Color.FromArgb(158, 112, 50);
            btnExportar.FlatStyle = FlatStyle.Flat;
            btnExportar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportar.ForeColor = Color.White;
            btnExportar.Location = new Point(332, 9);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(130, 34);
            btnExportar.TabIndex = 3;
            btnExportar.Text = "↓  Exportar";
            btnExportar.UseVisualStyleBackColor = false;
            btnExportar.Click += BtnExportar_Click;

            lblEstado.AutoSize = false;
            lblEstado.Font = new Font("Segoe UI", 8.5F);
            lblEstado.ForeColor = Color.FromArgb(128, 105, 82);
            lblEstado.Location = new Point(478, 15);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(1030, 24);
            lblEstado.TabIndex = 4;
            lblEstado.Text = "—";

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
            btnCerrar.Location = new Point(1484, 9);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(100, 34);
            btnCerrar.TabIndex = 5;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = false;
            btnCerrar.Click += BtnCerrar_Click;

            // ─────────────────────────────────────────────────────────
            // ctxExportar
            // ─────────────────────────────────────────────────────────
            ctxExportar.ImageScalingSize = new Size(24, 24);
            ctxExportar.Items.AddRange(new ToolStripItem[]
            {
                itemExportarExcel, itemExportarPDF
            });
            ctxExportar.Name = "ctxExportar";
            ctxExportar.Size = new Size(208, 68);

            itemExportarExcel.Name = "itemExportarExcel";
            itemExportarExcel.Size = new Size(207, 32);
            itemExportarExcel.Text = "Exportar a Excel";
            itemExportarExcel.Click += ItemExportarExcel_Click;

            itemExportarPDF.Name = "itemExportarPDF";
            itemExportarPDF.Size = new Size(207, 32);
            itemExportarPDF.Text = "Exportar a PDF";
            itemExportarPDF.Click += ItemExportarPDF_Click;

            // ─────────────────────────────────────────────────────────
            // DashboardForm
            // ─────────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            CancelButton = btnCerrar;
            ClientSize = new Size(1600, 1040);
            // Orden crítico: Fill → Bottom → Top
            Controls.Add(panelContenido);
            Controls.Add(panelFooter);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            Name = "DashboardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Dashboard de análisis";
            WindowState = FormWindowState.Maximized;

            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelContenido.ResumeLayout(false);
            tableLayoutCharts.ResumeLayout(false);
            groupKilosMes.ResumeLayout(false);
            groupEstados.ResumeLayout(false);
            groupProductores.ResumeLayout(false);
            groupProductos.ResumeLayout(false);
            groupDiaSemana.ResumeLayout(false);
            groupKilosSecos.ResumeLayout(false);
            panelFooter.ResumeLayout(false);
            panelFooter.PerformLayout();
            ctxExportar.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}