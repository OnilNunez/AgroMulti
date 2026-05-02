using AgroMulti.Data.Data;

namespace AgroMulti.Ui.Services
{
    /// <summary>
    /// Fábrica de servicios. Oculta la creación del contexto de datos.
    /// </summary>
    public static class ServiceFactory
    {
        public static EntregaService CrearEntregaService()
        {
            var context = new AgroMultiContext();
            return new EntregaService(context);
        }

        public static ProductorService CrearProductorService()
        {
            var context = new AgroMultiContext();
            return new ProductorService(context);
        }

        public static ProductoService CrearProductoService()
        {
            var context = new AgroMultiContext();
            return new ProductoService(context);
        }

        public static EstadoEntregaService CrearEstadoEntregaService()
        {
            var context = new AgroMultiContext();
            return new EstadoEntregaService(context);
        }

        public static SubProductoService CrearSubProductoService()
        {
            var context = new AgroMultiContext();
            return new SubProductoService(context);
        }
    }
}