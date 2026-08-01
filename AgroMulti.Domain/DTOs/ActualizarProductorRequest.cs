namespace AgroMulti.Domain.Requests;

public class ActualizarProductorRequest
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }
}