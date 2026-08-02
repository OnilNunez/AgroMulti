using AgroMulti.Application.Security;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgroMulti.Application.Services;

public class AuthService : IAuthService
{
    private readonly AgroMultiContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AgroMultiContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto request)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.NombreUsuario == request.Usuario &&
                u.Activo);

        if (usuario == null)
            return ApiResponse<LoginResponseDto>.Fail("Credenciales inválidas.");

        var passwordValida = PasswordHasher.Verify(request.Password, usuario.PasswordHash);

        if (!passwordValida)
            return ApiResponse<LoginResponseDto>.Fail("Credenciales inválidas.");

        var token = GenerarToken(usuario, out var expiracion);

        return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
        {
            Token = token,
            Expira = expiracion,
            Usuario = usuario.NombreUsuario,
            Rol = usuario.Rol
        }, "Inicio de sesión correcto.");
    }

    private string GenerarToken(Usuario usuario, out DateTime expiracion)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key no configurado.");
        var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer no configurado.");
        var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience no configurado.");

        var expirationMinutesText = jwtSection["ExpirationMinutes"];
        var expirationMinutes = 120;

        if (!string.IsNullOrWhiteSpace(expirationMinutesText) &&
            int.TryParse(expirationMinutesText, out var parsedMinutes) &&
            parsedMinutes > 0)
        {
            expirationMinutes = parsedMinutes;
        }

        expiracion = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol),
            new Claim("full_name", usuario.NombreCompleto)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiracion,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }
}