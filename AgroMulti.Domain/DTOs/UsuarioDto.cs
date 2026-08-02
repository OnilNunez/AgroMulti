namespace AgroMulti.Domain.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;
}