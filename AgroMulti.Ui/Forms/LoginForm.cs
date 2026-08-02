using AgroMulti.Ui.Models;
using AgroMulti.Ui.Services;
using AgroMulti.Ui.Session;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace AgroMulti.Ui.Forms;

public partial class LoginForm : Form
{
    // Para arrastrar la ventana sin bordes
    private bool dragging = false;
    private Point startPoint = new Point(0, 0);

    public LoginForm()
    {
        InitializeComponent();
        // Suscribir eventos para mover la ventana
        panelHeader.MouseDown += PanelHeader_MouseDown;
        panelHeader.MouseMove += PanelHeader_MouseMove;
        panelHeader.MouseUp += PanelHeader_MouseUp;
    }

    private void PanelHeader_MouseDown(object? sender, MouseEventArgs e)
    {
        dragging = true;
        startPoint = new Point(e.X, e.Y);
    }

    private void PanelHeader_MouseMove(object? sender, MouseEventArgs e)
    {
        if (dragging)
        {
            Point p = PointToScreen(e.Location);
            Location = new Point(p.X - startPoint.X, p.Y - startPoint.Y);
        }
    }

    private void PanelHeader_MouseUp(object? sender, MouseEventArgs e)
    {
        dragging = false;
    }

    private void btnCerrarVentana_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    private async void btnIniciarSesion_Click(object sender, EventArgs e)
    {
        btnIniciarSesion.Enabled = false;
        btnIniciarSesion.Text = "Ingresando...";

        try
        {
            var usuario = txtUsuario.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Debes escribir usuario y contraseña.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var request = new LoginRequest
            {
                Usuario = usuario,
                Password = password
            };

            var response = await ApiClient.Client.PostAsJsonAsync("api/Auth/login", request);

            var json = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(json);

            if (loginResponse == null || !loginResponse.Success || loginResponse.Data == null)
            {
                MessageBox.Show(
                    loginResponse?.Message ?? "No fue posible iniciar sesión.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            UserSession.Token = loginResponse.Data.Token;
            UserSession.Usuario = loginResponse.Data.Usuario;
            UserSession.Rol = loginResponse.Data.Rol;

            ApiClient.SetToken(UserSession.Token);

            // --- Mensaje de bienvenida simple (sin formulario personalizado) ---
            MessageBox.Show(
                $"Bienvenido, {UserSession.Usuario}.",
                "Acceso correcto",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            // ----------------------------------------------------------------

            Hide();

            var formPrincipal = new AgroMulti.Ui.Forms.MainMenu();
            formPrincipal.ShowDialog();

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ocurrió un error al iniciar sesión: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            btnIniciarSesion.Enabled = true;
            btnIniciarSesion.Text = "Iniciar sesión";
        }
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}