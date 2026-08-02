using AgroMulti.Api.Configuration;
using AgroMulti.Api.Middleware;
using AgroMulti.Application.Mapping;
using AgroMulti.Application.Services;
using AgroMulti.Data.Data;
using AgroMulti.Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AgroMultiContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AgroMultiConnection"));
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductorService, ProductorService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ISubProductoService, SubProductoService>();
builder.Services.AddScoped<IEntregaService, EntregaService>();
builder.Services.AddScoped<IEstadoEntregaService, EstadoEntregaService>();
builder.Services.AddScoped<IHistoricoEstadoEntregaService, HistoricoEstadoEntregaService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection["Key"]!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<
    AgroMulti.Application.Validators.CrearProductorRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AgroMultiContext>();
    await DatabaseSeeder.SeedAsync(context);
}

app.MapControllers();

app.Run();