using System;

namespace AgroMulti.Data.Models
{
    public class HistoricoEstadoEntrega
    {
        public int HistoricoEstadoEntregaId { get; set; }
        public int EntregaId { get; set; }
        public int EstadoEntregaId { get; set; }
        public DateTime FechaCambio { get; set; }
        public string? Observaciones { get; set; }
        public virtual Entrega Entrega { get; set; } = null!;
        public virtual EstadoEntrega EstadoEntrega { get; set; } = null!;
    }
}