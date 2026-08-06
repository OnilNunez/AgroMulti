using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroMulti.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SubProductosController : ControllerBase
{
    private readonly ISubProductoService _service;

    public SubProductosController(ISubProductoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _service.ObtenerTodosAsync();
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _service.ObtenerPorIdAsync(id);

        if (!resultado.Success)
            return NotFound(resultado);

        return Ok(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearSubProductoRequest request)
    {
        var resultado = await _service.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = resultado.Data!.Id },
            resultado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, ActualizarSubProductoRequest request)
    {
        var resultado = await _service.ActualizarAsync(id, request);

        if (!resultado.Success)
            return NotFound(resultado);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _service.EliminarAsync(id);

        if (!resultado.Success)
            return NotFound(resultado);

        return NoContent();
    }
}