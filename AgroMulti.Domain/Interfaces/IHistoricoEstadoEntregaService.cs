using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Domain.Interfaces;

public interface IHistoricoEstadoEntregaService
{
    Task<ApiResponse<List<HistoricoEstadoEntregaDto>>> ObtenerTodosAsync();

    Task<ApiResponse<List<HistoricoEstadoEntregaDto>>> ObtenerPorEntregaIdAsync(int entregaId);

    Task<ApiResponse<HistoricoEstadoEntregaDto>> RegistrarAsync(CrearHistoricoEstadoEntregaRequest request);
}