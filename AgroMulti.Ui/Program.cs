using System;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgroMulti.Data.Data;
using AgroMulti.Ui.Services;
using CentroFermentacionSecado;

namespace AgroMulti
{
    internal static class Program
    {
        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            Application.Run(new CentroFermentacionSecado.MainMenu());
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["AgroMultiConnection"].ConnectionString;

           
            services.AddDbContext<AgroMultiContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            // Tus servicios (ya son Transient por defecto)
            services.AddTransient<EntregaService>();
            services.AddTransient<ProductorService>();
            services.AddTransient<ProductoService>();
            services.AddTransient<EstadoEntregaService>();
            services.AddTransient<SubProductoService>();
            services.AddTransient<HistoricoEstadoEntregaService>();
        }
    }
}