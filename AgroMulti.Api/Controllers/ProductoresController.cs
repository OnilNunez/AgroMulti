using AgroMulti.Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductoresController : ControllerBase
{
    private readonly AgroMultiContext _context;

    public ProductoresController(AgroMultiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var productores = await _context.Productors
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return Ok(productores);
    }
}