using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroMulti.Ui.Services;

public class EntregaService(AgroMultiContext context) : IService<Entrega, int>
{

    public async Task<Entrega?> Buscar(int id)
    {
        return await context.Entregas
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EntregaId == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        var entrega = await context.Entregas.FindAsync(id);
        if (entrega == null)
            return false;

        context.Entregas.Remove(entrega);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<List<Entrega>> GetList(Expression<Func<Entrega, bool>> criterio)
    {
        return await context.Entregas
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<bool> Guardar(Entrega entidad)
    {
        if (!await Existe(entidad.EntregaId))
            return await Insertar(entidad);
        else
            return await Modificar(entidad);
    }

    private async Task<bool> Insertar(Entrega entidad)
    {
        context.Entregas.Add(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Existe(int id)
    {
        return await context.Entregas.AnyAsync(e => e.EntregaId == id);
    }

    public async Task<bool> Modificar(Entrega entidad)
    {
        context.Entregas.Update(entidad);
        return await context.SaveChangesAsync() > 0;
    }

   

    /// <summary>
    /// Obtiene una entrega con sus relaciones de Productor, Producto y EstadoEntrega.
    /// </summary>
   

    /// <summary>
    /// Obtiene una lista de entregas que cumplen el criterio, incluyendo relaciones.
    /// </summary>
    public async Task<List<Entrega>> GetListConRelaciones(Expression<Func<Entrega, bool>> criterio)
    {
        return await context.Entregas
            .AsNoTracking()
            .Include(e => e.Productor)
            .Include(e => e.Producto)
            .Include(e => e.EstadoEntrega)
            .Include(e => e.SubProducto)
            .Where(criterio)
            .ToListAsync();
    }
}