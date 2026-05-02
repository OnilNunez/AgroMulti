using AgroMulti.Data;
using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroMulti.Ui.Services;

public class ProductoService(AgroMultiContext context) : IService<Producto, int>
{
    public async Task<bool> Guardar(Producto entidad)
    {
        if (!await Existe(entidad.ProductoId))
            return await Insertar(entidad);
        else
            return await Modificar(entidad);
    }

    private async Task<bool> Insertar(Producto entidad)
    {
        context.Productos.Add(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Existe(int id)
    {
        return await context.Productos.AnyAsync(p => p.ProductoId == id);
    }

    public async Task<bool> Modificar(Producto entidad)
    {
        context.Productos.Update(entidad);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Producto?> Buscar(int id)
    {
        return await context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductoId == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        var producto = await context.Productos.FindAsync(id);
        if (producto == null)
            return false;

        context.Productos.Remove(producto);
        var cantidad = await context.SaveChangesAsync();
        return cantidad > 0;
    }

    public async Task<List<Producto>> GetList(Expression<Func<Producto, bool>> criterio)
    {
        return await context.Productos
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }
}