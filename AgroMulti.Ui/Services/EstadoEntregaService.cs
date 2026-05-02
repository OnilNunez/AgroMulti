using AgroMulti.Data;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroMulti.Ui.Services;

public class EstadoEntregaService(AgroMultiContext context) : IService<EstadoEntrega, int>
{
    public async Task<bool> Guardar(EstadoEntrega entidad)
    {
        if (!await Existe(entidad.EstadoEntregaId))
            return await Insertar(entidad);
        else
            return await Modificar(entidad);
    }

    private async Task<bool> Insertar(EstadoEntrega entidad)
    {
        context.EstadoEntregas.Add(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Existe(int id)
    {
        return await context.EstadoEntregas.AnyAsync(e => e.EstadoEntregaId == id);
    }

    public async Task<bool> Modificar(EstadoEntrega entidad)
    {
        context.EstadoEntregas.Update(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<EstadoEntrega?> Buscar(int id)
    {
        return await context.EstadoEntregas
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EstadoEntregaId == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        var estado = await context.EstadoEntregas.FindAsync(id);
        if (estado == null)
            return false;

        context.EstadoEntregas.Remove(estado);
        var cantidad = await context.SaveChangesAsync();
        return cantidad > 0;
    }

    public async Task<List<EstadoEntrega>> GetList(Expression<Func<EstadoEntrega, bool>> criterio)
    {
        return await context.EstadoEntregas
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }
}
