using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroMulti.Ui.Services;

public class HistoricoEstadoEntregaService(AgroMultiContext context) : IService<HistoricoEstadoEntrega, int>
{
    public async Task<bool> Guardar(HistoricoEstadoEntrega entidad)
    {
        if (!await Existe(entidad.HistoricoEstadoEntregaId))
            return await Insertar(entidad);
        else
            return await Modificar(entidad);
    }

    private async Task<bool> Insertar(HistoricoEstadoEntrega entidad)
    {
        // Asignar fecha actual si no viene definida
        if (entidad.FechaCambio == default)
            entidad.FechaCambio = DateTime.Now;

        context.HistoricosEstadoEntrega.Add(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Existe(int id)
    {
        return await context.HistoricosEstadoEntrega
            .AnyAsync(e => e.HistoricoEstadoEntregaId == id);
    }

    public async Task<bool> Modificar(HistoricoEstadoEntrega entidad)
    {
        context.HistoricosEstadoEntrega.Update(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<HistoricoEstadoEntrega?> Buscar(int id)
    {
        return await context.HistoricosEstadoEntrega
            .AsNoTracking()
            .Include(h => h.EstadoEntrega)
            .FirstOrDefaultAsync(h => h.HistoricoEstadoEntregaId == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        var historico = await context.HistoricosEstadoEntrega.FindAsync(id);
        if (historico == null)
            return false;

        context.HistoricosEstadoEntrega.Remove(historico);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<List<HistoricoEstadoEntrega>> GetList(Expression<Func<HistoricoEstadoEntrega, bool>> criterio)
    {
        return await context.HistoricosEstadoEntrega
            .AsNoTracking()
            .Include(h => h.EstadoEntrega)
            .Where(criterio)
            .OrderByDescending(h => h.FechaCambio)
            .ToListAsync();
    }
}