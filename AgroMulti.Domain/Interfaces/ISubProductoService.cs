using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Domain.Interfaces;

public interface ISubProductoService
{
    Task<ApiResponse<List<SubProductoDto>>> ObtenerTodosAsync();

    Task<ApiResponse<SubProductoDto>> ObtenerPorIdAsync(int id);

    Task<ApiResponse<SubProductoDto>> CrearAsync(CrearSubProductoRequest request);

    Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarSubProductoRequest request);

    Task<ApiResponse<bool>> EliminarAsync(int id);
}