using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Application.Services;

public class ProductoService : IProductoService
{
    private readonly AgroMultiContext _context;

    public ProductoService(AgroMultiContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<ProductoDto>>> ObtenerTodosAsync()
    {
        var productos = await _context.Productos
            .AsNoTracking()
            .OrderBy(x => x.Nombre)
            .Select(x => new ProductoDto
            {
                Id = x.ProductoId,
                Nombre = x.Nombre
            })
            .ToListAsync();

        return ApiResponse<List<ProductoDto>>.Ok(productos, "Productos obtenidos correctamente.");
    }

    public async Task<ApiResponse<ProductoDto>> ObtenerPorIdAsync(int id)
    {
        var producto = await _context.Productos
            .AsNoTracking()
            .Where(x => x.ProductoId == id)
            .Select(x => new ProductoDto
            {
                Id = x.ProductoId,
                Nombre = x.Nombre
            })
            .FirstOrDefaultAsync();

        if (producto == null)
            return ApiResponse<ProductoDto>.Fail("Producto no encontrado.");

        return ApiResponse<ProductoDto>.Ok(producto);
    }

    public async Task<ApiResponse<ProductoDto>> CrearAsync(CrearProductoRequest request)
    {
        var producto = new Producto
        {
            Nombre = request.Nombre
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        var dto = new ProductoDto
        {
            Id = producto.ProductoId,
            Nombre = producto.Nombre
        };

        return ApiResponse<ProductoDto>.Ok(dto, "Producto creado correctamente.");
    }

    public async Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarProductoRequest request)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            return ApiResponse<bool>.Fail("Producto no encontrado.");

        producto.Nombre = request.Nombre;

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Producto actualizado correctamente.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            return ApiResponse<bool>.Fail("Producto no encontrado.");

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Producto eliminado correctamente.");
    }
}