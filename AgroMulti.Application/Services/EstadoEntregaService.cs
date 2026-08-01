using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Application.Services;

public class EstadoEntregaService : IEstadoEntregaService
{
    private readonly AgroMultiContext _context;

    public EstadoEntregaService(AgroMultiContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<EstadoEntregaDto>>> ObtenerTodosAsync()
    {
        var datos = await _context.EstadoEntregas
            .AsNoTracking()
            .OrderBy(e => e.Nombre)
            .Select(e => new EstadoEntregaDto
            {
                Id = e.EstadoEntregaId,
                Nombre = e.Nombre
            })
            .ToListAsync();

        return ApiResponse<List<EstadoEntregaDto>>.Ok(datos);
    }

    public async Task<ApiResponse<EstadoEntregaDto>> ObtenerPorIdAsync(int id)
    {
        var estado = await _context.EstadoEntregas
            .AsNoTracking()
            .Where(e => e.EstadoEntregaId == id)
            .Select(e => new EstadoEntregaDto
            {
                Id = e.EstadoEntregaId,
                Nombre = e.Nombre
            })
            .FirstOrDefaultAsync();

        if (estado == null)
            return ApiResponse<EstadoEntregaDto>.Fail("Estado no encontrado.");

        return ApiResponse<EstadoEntregaDto>.Ok(estado);
    }

    public async Task<ApiResponse<EstadoEntregaDto>> CrearAsync(CrearEstadoEntregaRequest request)
    {
        var estado = new EstadoEntrega
        {
            Nombre = request.Nombre
        };

        _context.EstadoEntregas.Add(estado);

        await _context.SaveChangesAsync();

        return ApiResponse<EstadoEntregaDto>.Ok(new EstadoEntregaDto
        {
            Id = estado.EstadoEntregaId,
            Nombre = estado.Nombre
        });
    }

    public async Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarEstadoEntregaRequest request)
    {
        var estado = await _context.EstadoEntregas.FindAsync(id);

        if (estado == null)
            return ApiResponse<bool>.Fail("Estado no encontrado.");

        estado.Nombre = request.Nombre;

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<bool>> EliminarAsync(int id)
    {
        var estado = await _context.EstadoEntregas.FindAsync(id);

        if (estado == null)
            return ApiResponse<bool>.Fail("Estado no encontrado.");

        _context.EstadoEntregas.Remove(estado);

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true);
    }
}