using System.ComponentModel.DataAnnotations;

namespace AgroMulti.Domain.Requests;

public class CrearSubProductoRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public int ProductoId { get; set; }
}