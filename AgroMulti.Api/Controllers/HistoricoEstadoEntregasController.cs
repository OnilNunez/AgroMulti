using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AgroMulti.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoricoEstadoEntregasController : ControllerBase
{
    private readonly IHistoricoEstadoEntregaService _service;

    public HistoricoEstadoEntregasController(IHistoricoEstadoEntregaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _service.ObtenerTodosAsync();
        return Ok(resultado);
    }

    [HttpGet("entrega/{entregaId}")]
    public async Task<IActionResult> ObtenerPorEntregaId(int entregaId)
    {
        var resultado = await _service.ObtenerPorEntregaIdAsync(entregaId);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(CrearHistoricoEstadoEntregaRequest request)
    {
        var resultado = await _service.RegistrarAsync(request);

        if (!resultado.Success)
            return BadRequest(resultado);

        return Ok(resultado);
    }
}