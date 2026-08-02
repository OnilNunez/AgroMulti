using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AgroMulti.Api.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EntregasController : ControllerBase
{
    private readonly IEntregaService _service;

    public EntregasController(IEntregaService service)
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
    public async Task<IActionResult> Crear(CrearEntregaRequest request)
    {
        var resultado = await _service.CrearAsync(request);

        if (!resultado.Success)
            return BadRequest(resultado);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = resultado.Data!.Id },
            resultado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, ActualizarEntregaRequest request)
    {
        var resultado = await _service.ActualizarAsync(id, request);

        if (!resultado.Success)
            return BadRequest(resultado);

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