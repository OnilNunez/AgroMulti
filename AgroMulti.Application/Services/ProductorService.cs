using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Application.Services;

public class ProductorService : IProductorService
{
    private readonly AgroMultiContext _context;

    public ProductorService(AgroMultiContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<ProductorDto>>> ObtenerTodosAsync()
    {
        var productores = await _context.Productors
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .Select(p => new ProductorDto
            {
                Id = p.ProductorId,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Telefono = p.Telefono,
                Direccion = p.Direccion
            })
            .ToListAsync();

        return ApiResponse<List<ProductorDto>>.Ok(productores);
    }

    public async Task<ApiResponse<ProductorDto>> ObtenerPorIdAsync(int id)
    {
        var productor = await _context.Productors
            .AsNoTracking()
            .Where(p => p.ProductorId == id)
            .Select(p => new ProductorDto
            {
                Id = p.ProductorId,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Telefono = p.Telefono,
                Direccion = p.Direccion
            })
            .FirstOrDefaultAsync();

        if (productor == null)
            return ApiResponse<ProductorDto>.Fail("Productor no encontrado.");

        return ApiResponse<ProductorDto>.Ok(productor);
    }

    public async Task<ApiResponse<ProductorDto>> CrearAsync(CrearProductorRequest request)
    {
        var existeCodigo = await _context.Productors
            .AnyAsync(p => p.Codigo == request.Codigo);

        if (existeCodigo)
            return ApiResponse<ProductorDto>.Fail("Ya existe un productor con ese código.");

        var productor = new Productor
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Telefono = request.Telefono,
            Direccion = request.Direccion
        };

        _context.Productors.Add(productor);
        await _context.SaveChangesAsync();

        var dto = new ProductorDto
        {
            Id = productor.ProductorId,
            Codigo = productor.Codigo,
            Nombre = productor.Nombre,
            Apellido = productor.Apellido,
            Telefono = productor.Telefono,
            Direccion = productor.Direccion
        };

        return ApiResponse<ProductorDto>.Ok(dto);
    }

    public async Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarProductorRequest request)
    {
        var productor = await _context.Productors.FindAsync(id);

        if (productor == null)
            return ApiResponse<bool>.Fail("Productor no encontrado.");

        var codigoEnUso = await _context.Productors
            .AnyAsync(p => p.Codigo == request.Codigo && p.ProductorId != id);

        if (codigoEnUso)
            return ApiResponse<bool>.Fail("Ya existe otro productor con ese código.");

        productor.Codigo = request.Codigo;
        productor.Nombre = request.Nombre;
        productor.Apellido = request.Apellido;
        productor.Telefono = request.Telefono;
        productor.Direccion = request.Direccion;

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Productor actualizado correctamente.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(int id)
    {
        var productor = await _context.Productors
            .Include(p => p.Entregas)
            .FirstOrDefaultAsync(p => p.ProductorId == id);

        if (productor == null)
            return ApiResponse<bool>.Fail("Productor no encontrado.");

        if (productor.Entregas.Any())
            return ApiResponse<bool>.Fail("No se puede eliminar porque tiene entregas asociadas.");

        _context.Productors.Remove(productor);
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Productor eliminado correctamente.");
    }
}