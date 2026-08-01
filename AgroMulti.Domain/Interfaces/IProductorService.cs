using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Domain.Interfaces;

public interface IProductorService
{
    Task<ApiResponse<List<ProductorDto>>> ObtenerTodosAsync();

    Task<ApiResponse<ProductorDto>> ObtenerPorIdAsync(int id);

    Task<ApiResponse<ProductorDto>> CrearAsync(CrearProductorRequest request);

    Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarProductorRequest request);

    Task<ApiResponse<bool>> EliminarAsync(int id);
}