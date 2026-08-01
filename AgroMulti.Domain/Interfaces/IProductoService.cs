using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Domain.Interfaces;

public interface IProductoService
{
    Task<ApiResponse<List<ProductoDto>>> ObtenerTodosAsync();

    Task<ApiResponse<ProductoDto>> ObtenerPorIdAsync(int id);

    Task<ApiResponse<ProductoDto>> CrearAsync(CrearProductoRequest request);

    Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarProductoRequest request);

    Task<ApiResponse<bool>> EliminarAsync(int id);
}