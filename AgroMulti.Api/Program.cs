using AgroMulti.Data.Data;
using Microsoft.EntityFrameworkCore;
using AgroMulti.Application.Services;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Application.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AgroMultiContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AgroMultiConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IProductorService, ProductorService>();

builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddScoped<ISubProductoService, SubProductoService>();

builder.Services.AddScoped<IEntregaService, EntregaService>();

builder.Services.AddScoped<IEstadoEntregaService, EstadoEntregaService>();

builder.Services.AddScoped<IHistoricoEstadoEntregaService, HistoricoEstadoEntregaService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();