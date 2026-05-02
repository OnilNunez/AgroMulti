using AgroMulti.Data;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroMulti.Ui.Services;

public class SubProductoService(AgroMultiContext context) : IService<SubProducto, int>
{
    public async Task<bool> Guardar(SubProducto entidad)
    {
        if (!await Existe(entidad.SubProductoId))
            return await Insertar(entidad);
        else
            return await Modificar(entidad);
    }

    private async Task<bool> Insertar(SubProducto entidad)
    {
        context.SubProductos.Add(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Existe(int id)
    {
        return await context.SubProductos.AnyAsync(s => s.SubProductoId == id);
    }

    public async Task<bool> Modificar(SubProducto entidad)
    {
        context.SubProductos.Update(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<SubProducto?> Buscar(int id)
    {
        return await context.SubProductos
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SubProductoId == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        var subProducto = await context.SubProductos.FindAsync(id);
        if (subProducto == null)
            return false;

        context.SubProductos.Remove(subProducto);
        var cantidad = await context.SaveChangesAsync();
        return cantidad > 0;
    }

    public async Task<List<SubProducto>> GetList(Expression<Func<SubProducto, bool>> criterio)
    {
        return await context.SubProductos
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }
}
