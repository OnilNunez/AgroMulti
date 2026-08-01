namespace AgroMulti.Domain.Requests;

public class CrearEntregaRequest
{
    public string NumeroEntrega { get; set; } = string.Empty;

    public DateOnly FechaEntrega { get; set; }

    public int ProductorId { get; set; }

    public int ProductoId { get; set; }

    public int? SubProductoId { get; set; }

    public int EstadoEntregaId { get; set; }

    public string? Placa { get; set; }

    public string? NombreConductor { get; set; }

    public decimal Kilos { get; set; }

    public int Cajas { get; set; }

    public int Sacos { get; set; }

    public decimal? KilosSecos { get; set; }

    public string? Pasillo { get; set; }

    public string? NumeroAnaquel { get; set; }

    public string? Piso { get; set; }

    public string? Observaciones { get; set; }
}