using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Application.Services;

public class HistoricoEstadoEntregaService : IHistoricoEstadoEntregaService
{
    private readonly AgroMultiContext _context;

    public HistoricoEstadoEntregaService(AgroMultiContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<HistoricoEstadoEntregaDto>>> ObtenerTodosAsync()
    {
        var historicos = await _context.HistoricosEstadoEntrega
            .AsNoTracking()
            .OrderByDescending(h => h.FechaCambio)
            .Select(h => new HistoricoEstadoEntregaDto
            {
                Id = h.HistoricoEstadoEntregaId,
                EntregaId = h.EntregaId,
                NumeroEntrega = h.Entrega.NumeroEntrega,
                EstadoEntregaId = h.EstadoEntregaId,
                Estado = h.EstadoEntrega.Nombre,
                FechaCambio = h.FechaCambio,
                Observaciones = h.Observaciones
            })
            .ToListAsync();

        return ApiResponse<List<HistoricoEstadoEntregaDto>>.Ok(historicos);
    }

    public async Task<ApiResponse<List<HistoricoEstadoEntregaDto>>> ObtenerPorEntregaIdAsync(int entregaId)
    {
        var historicos = await _context.HistoricosEstadoEntrega
            .AsNoTracking()
            .Where(h => h.EntregaId == entregaId)
            .OrderBy(h => h.FechaCambio)
            .Select(h => new HistoricoEstadoEntregaDto
            {
                Id = h.HistoricoEstadoEntregaId,
                EntregaId = h.EntregaId,
                NumeroEntrega = h.Entrega.NumeroEntrega,
                EstadoEntregaId = h.EstadoEntregaId,
                Estado = h.EstadoEntrega.Nombre,
                FechaCambio = h.FechaCambio,
                Observaciones = h.Observaciones
            })
            .ToListAsync();

        return ApiResponse<List<HistoricoEstadoEntregaDto>>.Ok(historicos);
    }

    public async Task<ApiResponse<HistoricoEstadoEntregaDto>> RegistrarAsync(CrearHistoricoEstadoEntregaRequest request)
    {
        var entregaExiste = await _context.Entregas.AnyAsync(e => e.EntregaId == request.EntregaId);
        if (!entregaExiste)
            return ApiResponse<HistoricoEstadoEntregaDto>.Fail("Entrega no encontrada.");

        var estadoExiste = await _context.EstadoEntregas.AnyAsync(e => e.EstadoEntregaId == request.EstadoEntregaId);
        if (!estadoExiste)
            return ApiResponse<HistoricoEstadoEntregaDto>.Fail("Estado de entrega no encontrado.");

        var historico = new HistoricoEstadoEntrega
        {
            EntregaId = request.EntregaId,
            EstadoEntregaId = request.EstadoEntregaId,
            FechaCambio = request.FechaCambio ?? DateTime.Now,
            Observaciones = request.Observaciones
        };

        _context.HistoricosEstadoEntrega.Add(historico);
        await _context.SaveChangesAsync();

        await _context.Entry(historico).Reference(h => h.Entrega).LoadAsync();
        await _context.Entry(historico).Reference(h => h.EstadoEntrega).LoadAsync();

        var dto = new HistoricoEstadoEntregaDto
        {
            Id = historico.HistoricoEstadoEntregaId,
            EntregaId = historico.EntregaId,
            NumeroEntrega = historico.Entrega.NumeroEntrega,
            EstadoEntregaId = historico.EstadoEntregaId,
            Estado = historico.EstadoEntrega.Nombre,
            FechaCambio = historico.FechaCambio,
            Observaciones = historico.Observaciones
        };

        return ApiResponse<HistoricoEstadoEntregaDto>.Ok(dto, "Histórico registrado correctamente.");
    }
}