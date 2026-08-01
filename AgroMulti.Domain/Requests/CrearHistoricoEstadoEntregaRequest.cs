namespace AgroMulti.Domain.Requests;

public class CrearHistoricoEstadoEntregaRequest
{
    public int EntregaId { get; set; }

    public int EstadoEntregaId { get; set; }

    public DateTime? FechaCambio { get; set; }

    public string? Observaciones { get; set; }
}