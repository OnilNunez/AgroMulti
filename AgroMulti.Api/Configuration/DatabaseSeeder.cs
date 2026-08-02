using AgroMulti.Application.Security;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Api.Configuration;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AgroMultiContext context)
    {
        if (await context.Usuarios.AnyAsync())
            return;

        context.Usuarios.Add(new Usuario
        {
            NombreUsuario = "admin",
            PasswordHash = PasswordHasher.Hash("Admin123!"),
            NombreCompleto = "Administrador del Sistema",
            Rol = "Administrador",
            Activo = true
        });

        await context.SaveChangesAsync();
    }
}