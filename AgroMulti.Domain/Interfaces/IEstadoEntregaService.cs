using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Domain.Interfaces;

public interface IEstadoEntregaService
{
    Task<ApiResponse<List<EstadoEntregaDto>>> ObtenerTodosAsync();

    Task<ApiResponse<EstadoEntregaDto>> ObtenerPorIdAsync(int id);

    Task<ApiResponse<EstadoEntregaDto>> CrearAsync(CrearEstadoEntregaRequest request);

    Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarEstadoEntregaRequest request);

    Task<ApiResponse<bool>> EliminarAsync(int id);
}