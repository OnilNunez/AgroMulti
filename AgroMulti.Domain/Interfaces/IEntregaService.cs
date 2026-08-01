using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Domain.Interfaces;

public interface IEntregaService
{
    Task<ApiResponse<List<EntregaDto>>> ObtenerTodosAsync();

    Task<ApiResponse<EntregaDto>> ObtenerPorIdAsync(int id);

    Task<ApiResponse<EntregaDto>> CrearAsync(CrearEntregaRequest request);

    Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarEntregaRequest request);

    Task<ApiResponse<bool>> EliminarAsync(int id);
}