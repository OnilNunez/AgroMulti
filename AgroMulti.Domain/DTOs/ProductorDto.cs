namespace AgroMulti.Domain.DTOs;

public class ProductorDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }
}