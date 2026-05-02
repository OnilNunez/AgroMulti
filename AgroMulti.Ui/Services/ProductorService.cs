using AgroMulti.Data;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroMulti.Ui.Services;

public class ProductorService(AgroMultiContext context) : IService<Productor, int>
{
    public async Task<bool> Guardar(Productor entidad)
    {
        if (!await Existe(entidad.ProductorId))
            return await Insertar(entidad);
        else
            return await Modificar(entidad);
    }

    private async Task<bool> Insertar(Productor entidad)
    {
        context.Productors.Add(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Existe(int id)
    {
        return await context.Productors.AnyAsync(p => p.ProductorId == id);
    }

    public async Task<bool> Modificar(Productor entidad)
    {
        context.Productors.Update(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Productor?> Buscar(int id)
    {
        return await context.Productors
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductorId == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        var productor = await context.Productors.FindAsync(id);
        if (productor == null)
            return false;

        context.Productors.Remove(productor);
        var cantidad = await context.SaveChangesAsync();
        return cantidad > 0;
    }

    public async Task<List<Productor>> GetList(Expression<Func<Productor, bool>> criterio)
    {
        return await context.Productors
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }
}
