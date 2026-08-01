namespace AgroMulti.Domain.DTOs;

public class HistoricoEstadoEntregaDto
{
    public int Id { get; set; }

    public int EntregaId { get; set; }

    public string NumeroEntrega { get; set; } = string.Empty;

    public int EstadoEntregaId { get; set; }

    public string Estado { get; set; } = string.Empty;

    public DateTime FechaCambio { get; set; }

    public string? Observaciones { get; set; }
}