namespace AgroMulti.Domain.DTOs;

public class EntregaDto
{
    public int Id { get; set; }

    public string NumeroEntrega { get; set; } = string.Empty;

    public DateOnly FechaEntrega { get; set; }

    public int ProductorId { get; set; }

    public string Productor { get; set; } = string.Empty;

    public int ProductoId { get; set; }

    public string Producto { get; set; } = string.Empty;

    public int? SubProductoId { get; set; }

    public string? SubProducto { get; set; }

    public int EstadoEntregaId { get; set; }

    public string Estado { get; set; } = string.Empty;

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