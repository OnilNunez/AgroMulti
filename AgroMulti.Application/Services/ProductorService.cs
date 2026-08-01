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
                NombreCompleto = p.Nombre + " " + p.Apellido,
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
                NombreCompleto = p.Nombre + " " + p.Apellido,
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
            NombreCompleto = productor.Nombre + " " + productor.Apellido,
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

        productor.Nombre = request.Nombre;
        productor.Apellido = request.Apellido;
        productor.Telefono = request.Telefono;
        productor.Direccion = request.Direccion;

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<bool>> EliminarAsync(int id)
    {
        var productor = await _context.Productors.FindAsync(id);

        if (productor == null)
            return ApiResponse<bool>.Fail("Productor no encontrado.");

        _context.Productors.Remove(productor);
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true);
    }
}