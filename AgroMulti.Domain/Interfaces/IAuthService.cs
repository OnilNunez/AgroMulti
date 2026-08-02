using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Domain.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto request);
}