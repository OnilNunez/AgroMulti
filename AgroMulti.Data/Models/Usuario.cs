using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMulti.Data.Models;

[Table("Usuario")]
public class Usuario
{
    [Key]
    public int UsuarioId { get; set; }

    [StringLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [StringLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [StringLength(50)]
    public string Rol { get; set; } = "Usuario";

    public bool Activo { get; set; } = true;
}