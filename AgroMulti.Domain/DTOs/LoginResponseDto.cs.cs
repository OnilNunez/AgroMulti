namespace AgroMulti.Domain.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime Expira { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;
}