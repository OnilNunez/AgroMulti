using AgroMulti.Data;
using AgroMulti.Data.Data;
using AgroMulti.Ui.Forms;
using AgroMulti.Ui.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace AgroMulti
{
    internal static class Program
    {
        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            IniciarApi();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            System.Windows.Forms.Application.Run(new LoginForm());
        }

        private static bool ApiEstaCorriendo()
        {
            try
            {
                using var cliente = new TcpClient();
                cliente.Connect("localhost", 5126);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void IniciarApi()
        {
            if (ApiEstaCorriendo())
                return;

            try
            {
                // Busca la carpeta de la solución
                string solucion = Path.GetFullPath(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\.."));

                string proyectoApi = Path.Combine(solucion, "AgroMulti.Api");

                if (!Directory.Exists(proyectoApi))
                {
                    MessageBox.Show(
                        $"No se encontró el proyecto AgroMulti.Api.\n\nRuta buscada:\n{proyectoApi}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run",
                    WorkingDirectory = proyectoApi,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                for (int i = 0; i < 30; i++)
                {
                    if (ApiEstaCorriendo())
                        return;

                    Thread.Sleep(500);
                }

                MessageBox.Show(
                    "La API no respondió después de 15 segundos.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible iniciar la API.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            const string connectionString =
                "Server=DESKTOP-8J5PA5E\\SQLEXPRESS;Database=AgroMultiDB;Trusted_Connection=True;TrustServerCertificate=True;";

            services.AddDbContext<AgroMultiContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            services.AddTransient<EntregaService>();
            services.AddTransient<ProductorService>();
            services.AddTransient<ProductoService>();
            services.AddTransient<SubProductoService>();
            services.AddTransient<EstadoEntregaService>();
            services.AddTransient<HistoricoEstadoEntregaService>();
            services.AddTransient<AuthService>();
        }
    }
}