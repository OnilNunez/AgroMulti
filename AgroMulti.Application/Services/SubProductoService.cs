using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Application.Services;

public class SubProductoService : ISubProductoService
{
    private readonly AgroMultiContext _context;

    public SubProductoService(AgroMultiContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<SubProductoDto>>> ObtenerTodosAsync()
    {
        var subProductos = await _context.SubProductos
            .AsNoTracking()
            .Include(s => s.Producto)
            .OrderBy(s => s.Nombre)
            .Select(s => new SubProductoDto
            {
                Id = s.SubProductoId,
                Nombre = s.Nombre,
                ProductoId = s.ProductoId,
                Producto = s.Producto.Nombre
            })
            .ToListAsync();

        return ApiResponse<List<SubProductoDto>>.Ok(subProductos);
    }

    public async Task<ApiResponse<SubProductoDto>> ObtenerPorIdAsync(int id)
    {
        var subProducto = await _context.SubProductos
            .AsNoTracking()
            .Include(s => s.Producto)
            .Where(s => s.SubProductoId == id)
            .Select(s => new SubProductoDto
            {
                Id = s.SubProductoId,
                Nombre = s.Nombre,
                ProductoId = s.ProductoId,
                Producto = s.Producto.Nombre
            })
            .FirstOrDefaultAsync();

        if (subProducto == null)
            return ApiResponse<SubProductoDto>.Fail("Subproducto no encontrado.");

        return ApiResponse<SubProductoDto>.Ok(subProducto);
    }

    public async Task<ApiResponse<SubProductoDto>> CrearAsync(CrearSubProductoRequest request)
    {
        var subProducto = new SubProducto
        {
            Nombre = request.Nombre,
            ProductoId = request.ProductoId
        };

        _context.SubProductos.Add(subProducto);
        await _context.SaveChangesAsync();

        await _context.Entry(subProducto)
            .Reference(s => s.Producto)
            .LoadAsync();

        var dto = new SubProductoDto
        {
            Id = subProducto.SubProductoId,
            Nombre = subProducto.Nombre,
            ProductoId = subProducto.ProductoId,
            Producto = subProducto.Producto.Nombre
        };

        return ApiResponse<SubProductoDto>.Ok(dto);
    }

    public async Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarSubProductoRequest request)
    {
        var subProducto = await _context.SubProductos.FindAsync(id);

        if (subProducto == null)
            return ApiResponse<bool>.Fail("Subproducto no encontrado.");

        subProducto.Nombre = request.Nombre;
        subProducto.ProductoId = request.ProductoId;

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<bool>> EliminarAsync(int id)
    {
        var subProducto = await _context.SubProductos.FindAsync(id);

        if (subProducto == null)
            return ApiResponse<bool>.Fail("Subproducto no encontrado.");

        _context.SubProductos.Remove(subProducto);

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true);
    }
}