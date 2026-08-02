using System.ComponentModel.DataAnnotations;

namespace AgroMulti.Domain.Requests;

public class LoginRequest
{
    [Required]
    public string Usuario { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}