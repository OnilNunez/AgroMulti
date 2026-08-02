using System.Text.Json;
using AgroMulti.Domain.Responses;

namespace AgroMulti.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<string>.Fail(
                "Ocurrió un error interno del servidor.");

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}