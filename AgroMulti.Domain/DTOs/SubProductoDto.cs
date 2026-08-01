namespace AgroMulti.Domain.DTOs;

public class SubProductoDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int ProductoId { get; set; }

    public string Producto { get; set; } = string.Empty;
}