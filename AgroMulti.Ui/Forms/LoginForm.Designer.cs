namespace AgroMulti.Ui.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnIniciarSesion;
        private Label lblUsuario;
        private Label lblPassword;

        
        private Panel panelHeader;
        private Panel panelAccentStrip;
        private Label lblAppName;
        private Label lblTitulo;
        private Button btnCancelar;
        private Button btnCerrarVentana;   // X 
        private Label lblIconoUsuario;
        private Label lblIconoPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtUsuario = new TextBox();
            txtPassword = new TextBox();
            btnIniciarSesion = new Button();
            btnCancelar = new Button();
            lblUsuario = new Label();
            lblPassword = new Label();
            panelHeader = new Panel();
            lblAppName = new Label();
            lblTitulo = new Label();
            btnCerrarVentana = new Button();
            panelAccentStrip = new Panel();
            lblIconoUsuario = new Label();
            lblIconoPassword = new Label();
            label1 = new Label();
            panelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // txtUsuario
            // 
            txtUsuario.BackColor = Color.White;
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            txtUsuario.Font = new Font("Segoe UI", 10F);
            txtUsuario.ForeColor = Color.FromArgb(44, 28, 16);
            txtUsuario.Location = new Point(71, 233);
            txtUsuario.Margin = new Padding(4, 5, 4, 5);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(456, 34);
            txtUsuario.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.ForeColor = Color.FromArgb(44, 28, 16);
            txtPassword.Location = new Point(71, 333);
            txtPassword.Margin = new Padding(4, 5, 4, 5);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(456, 34);
            txtPassword.TabIndex = 1;
            // 
            // btnIniciarSesion
            // 
            btnIniciarSesion.BackColor = Color.FromArgb(92, 122, 42);
            btnIniciarSesion.FlatAppearance.BorderColor = Color.FromArgb(92, 122, 42);
            btnIniciarSesion.FlatAppearance.BorderSize = 0;
            btnIniciarSesion.FlatStyle = FlatStyle.Flat;
            btnIniciarSesion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnIniciarSesion.ForeColor = Color.White;
            btnIniciarSesion.Location = new Point(71, 417);
            btnIniciarSesion.Margin = new Padding(4, 5, 4, 5);
            btnIniciarSesion.Name = "btnIniciarSesion";
            btnIniciarSesion.Size = new Size(214, 63);
            btnIniciarSesion.TabIndex = 2;
            btnIniciarSesion.Text = "Iniciar sesión";
            btnIniciarSesion.UseVisualStyleBackColor = false;
            btnIniciarSesion.Click += btnIniciarSesion_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.FromArgb(80, 55, 30);
            btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(160, 130, 95);
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 10F);
            btnCancelar.ForeColor = Color.Cornsilk;
            btnCancelar.Location = new Point(314, 417);
            btnCancelar.Margin = new Padding(4, 5, 4, 5);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(214, 63);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 10F);
            lblUsuario.ForeColor = Color.FromArgb(44, 28, 16);
            lblUsuario.Location = new Point(71, 197);
            lblUsuario.Margin = new Padding(4, 0, 4, 0);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(79, 28);
            lblUsuario.TabIndex = 1;
            lblUsuario.Text = "Usuario";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 10F);
            lblPassword.ForeColor = Color.FromArgb(44, 28, 16);
            lblPassword.Location = new Point(71, 297);
            lblPassword.Margin = new Padding(4, 0, 4, 0);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(110, 28);
            lblPassword.TabIndex = 3;
            lblPassword.Text = "Contraseña";
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(38, 22, 10);
            panelHeader.Controls.Add(label1);
            panelHeader.Controls.Add(lblAppName);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(btnCerrarVentana);
            panelHeader.Controls.Add(panelAccentStrip);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(4, 5, 4, 5);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(600, 158);
            panelHeader.TabIndex = 0;
            // 
            // lblAppName
            // 
            lblAppName.AutoSize = true;
            lblAppName.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblAppName.ForeColor = Color.Cornsilk;
            lblAppName.Location = new Point(31, 27);
            lblAppName.Margin = new Padding(4, 0, 4, 0);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new Size(193, 48);
            lblAppName.TabIndex = 0;
            lblAppName.Text = "AgroMulti";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12F);
            lblTitulo.ForeColor = Color.Cornsilk;
            lblTitulo.Location = new Point(31, 92);
            lblTitulo.Margin = new Padding(4, 0, 4, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(151, 32);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Iniciar sesión";
            // 
            // btnCerrarVentana
            // 
            btnCerrarVentana.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrarVentana.FlatAppearance.BorderSize = 0;
            btnCerrarVentana.FlatStyle = FlatStyle.Flat;
            btnCerrarVentana.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCerrarVentana.ForeColor = Color.White;
            btnCerrarVentana.Location = new Point(540, 20);
            btnCerrarVentana.Margin = new Padding(4, 5, 4, 5);
            btnCerrarVentana.Name = "btnCerrarVentana";
            btnCerrarVentana.Size = new Size(43, 50);
            btnCerrarVentana.TabIndex = 5;
            btnCerrarVentana.Text = "✕";
            btnCerrarVentana.UseVisualStyleBackColor = true;
            btnCerrarVentana.Click += btnCerrarVentana_Click;
            // 
            // panelAccentStrip
            // 
            panelAccentStrip.BackColor = Color.FromArgb(38, 22, 10);
            panelAccentStrip.Dock = DockStyle.Bottom;
            panelAccentStrip.Location = new Point(0, 153);
            panelAccentStrip.Margin = new Padding(4, 5, 4, 5);
            panelAccentStrip.Name = "panelAccentStrip";
            panelAccentStrip.Size = new Size(600, 5);
            panelAccentStrip.TabIndex = 2;
            // 
            // lblIconoUsuario
            // 
            lblIconoUsuario.AutoSize = true;
            lblIconoUsuario.Font = new Font("Segoe MDL2 Assets", 14F);
            lblIconoUsuario.ForeColor = Color.FromArgb(92, 122, 42);
            lblIconoUsuario.Location = new Point(23, 233);
            lblIconoUsuario.Margin = new Padding(4, 0, 4, 0);
            lblIconoUsuario.Name = "lblIconoUsuario";
            lblIconoUsuario.Size = new Size(40, 28);
            lblIconoUsuario.TabIndex = 6;
            lblIconoUsuario.Text = "";
            // 
            // lblIconoPassword
            // 
            lblIconoPassword.AutoSize = true;
            lblIconoPassword.Font = new Font("Segoe MDL2 Assets", 14F);
            lblIconoPassword.ForeColor = Color.FromArgb(92, 122, 42);
            lblIconoPassword.Location = new Point(23, 333);
            lblIconoPassword.Margin = new Padding(4, 0, 4, 0);
            lblIconoPassword.Name = "lblIconoPassword";
            lblIconoPassword.Size = new Size(40, 28);
            lblIconoPassword.TabIndex = 7;
            lblIconoPassword.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe MDL2 Assets", 64F);
            label1.ForeColor = Color.Cornsilk;
            label1.Location = new Point(350, 27);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(182, 128);
            label1.TabIndex = 8;
            label1.Text = "";
            // 
            // LoginForm
            // 
            AcceptButton = btnIniciarSesion;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 232);
            CancelButton = btnCancelar;
            ClientSize = new Size(600, 550);
            Controls.Add(lblIconoPassword);
            Controls.Add(lblIconoUsuario);
            Controls.Add(btnCancelar);
            Controls.Add(btnIniciarSesion);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtUsuario);
            Controls.Add(lblUsuario);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AgroMulti - Login";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
    }
}