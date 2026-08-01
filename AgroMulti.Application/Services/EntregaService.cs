using AgroMulti.Data.Data;
using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AgroMulti.Domain.Interfaces;
using AgroMulti.Domain.Requests;
using AgroMulti.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Application.Services;

public class EntregaService : IEntregaService
{
    private readonly AgroMultiContext _context;
    private readonly IHistoricoEstadoEntregaService _historicoService;

    public EntregaService(
        AgroMultiContext context,
        IHistoricoEstadoEntregaService historicoService)
    {
        _context = context;
        _historicoService = historicoService;
    }

    public async Task<ApiResponse<List<EntregaDto>>> ObtenerTodosAsync()
    {
        var entregas = await _context.Entregas
            .AsNoTracking()
            .OrderByDescending(e => e.FechaEntrega)
            .Select(e => new EntregaDto
            {
                Id = e.EntregaId,
                NumeroEntrega = e.NumeroEntrega,
                FechaEntrega = e.FechaEntrega,
                ProductorId = e.ProductorId,
                Productor = e.Productor.Nombre + " " + e.Productor.Apellido,
                ProductoId = e.ProductoId,
                Producto = e.Producto.Nombre,
                SubProductoId = e.SubProductoId,
                SubProducto = e.SubProducto != null ? e.SubProducto.Nombre : null,
                EstadoEntregaId = e.EstadoEntregaId,
                Estado = e.EstadoEntrega.Nombre,
                Placa = e.Placa,
                NombreConductor = e.NombreConductor,
                Kilos = e.Kilos,
                Cajas = e.Cajas,
                Sacos = e.Sacos,
                KilosSecos = e.KilosSecos,
                Pasillo = e.Pasillo,
                NumeroAnaquel = e.NumeroAnaquel,
                Piso = e.Piso,
                Observaciones = e.Observaciones
            })
            .ToListAsync();

        return ApiResponse<List<EntregaDto>>.Ok(entregas);
    }

    public async Task<ApiResponse<EntregaDto>> ObtenerPorIdAsync(int id)
    {
        var entrega = await _context.Entregas
            .AsNoTracking()
            .Where(e => e.EntregaId == id)
            .Select(e => new EntregaDto
            {
                Id = e.EntregaId,
                NumeroEntrega = e.NumeroEntrega,
                FechaEntrega = e.FechaEntrega,
                ProductorId = e.ProductorId,
                Productor = e.Productor.Nombre + " " + e.Productor.Apellido,
                ProductoId = e.ProductoId,
                Producto = e.Producto.Nombre,
                SubProductoId = e.SubProductoId,
                SubProducto = e.SubProducto != null ? e.SubProducto.Nombre : null,
                EstadoEntregaId = e.EstadoEntregaId,
                Estado = e.EstadoEntrega.Nombre,
                Placa = e.Placa,
                NombreConductor = e.NombreConductor,
                Kilos = e.Kilos,
                Cajas = e.Cajas,
                Sacos = e.Sacos,
                KilosSecos = e.KilosSecos,
                Pasillo = e.Pasillo,
                NumeroAnaquel = e.NumeroAnaquel,
                Piso = e.Piso,
                Observaciones = e.Observaciones
            })
            .FirstOrDefaultAsync();

        if (entrega == null)
            return ApiResponse<EntregaDto>.Fail("Entrega no encontrada.");

        return ApiResponse<EntregaDto>.Ok(entrega);
    }

    public async Task<ApiResponse<EntregaDto>> CrearAsync(CrearEntregaRequest request)
    {
        var validacion = await ValidarReferenciasAsync(
            request.ProductorId,
            request.ProductoId,
            request.SubProductoId,
            request.EstadoEntregaId);

        if (validacion != null)
            return ApiResponse<EntregaDto>.Fail(validacion);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var entrega = new Entrega
        {
            NumeroEntrega = request.NumeroEntrega,
            FechaEntrega = request.FechaEntrega,
            ProductorId = request.ProductorId,
            ProductoId = request.ProductoId,
            SubProductoId = request.SubProductoId,
            EstadoEntregaId = request.EstadoEntregaId,
            Placa = request.Placa,
            NombreConductor = request.NombreConductor,
            Kilos = request.Kilos,
            Cajas = request.Cajas,
            Sacos = request.Sacos,
            KilosSecos = request.KilosSecos,
            Pasillo = request.Pasillo,
            NumeroAnaquel = request.NumeroAnaquel,
            Piso = request.Piso,
            Observaciones = request.Observaciones
        };

        _context.Entregas.Add(entrega);
        await _context.SaveChangesAsync();

        var historial = await _historicoService.RegistrarAsync(new CrearHistoricoEstadoEntregaRequest
        {
            EntregaId = entrega.EntregaId,
            EstadoEntregaId = entrega.EstadoEntregaId,
            Observaciones = entrega.Observaciones
        });

        if (!historial.Success)
        {
            await transaction.RollbackAsync();
            return ApiResponse<EntregaDto>.Fail(historial.Message);
        }

        await transaction.CommitAsync();

        return await ObtenerPorIdAsync(entrega.EntregaId);
    }

    public async Task<ApiResponse<bool>> ActualizarAsync(int id, ActualizarEntregaRequest request)
    {
        var entrega = await _context.Entregas.FindAsync(id);

        if (entrega == null)
            return ApiResponse<bool>.Fail("Entrega no encontrada.");

        var validacion = await ValidarReferenciasAsync(
            request.ProductorId,
            request.ProductoId,
            request.SubProductoId,
            request.EstadoEntregaId);

        if (validacion != null)
            return ApiResponse<bool>.Fail(validacion);

        var estadoAnterior = entrega.EstadoEntregaId;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        entrega.NumeroEntrega = request.NumeroEntrega;
        entrega.FechaEntrega = request.FechaEntrega;
        entrega.ProductorId = request.ProductorId;
        entrega.ProductoId = request.ProductoId;
        entrega.SubProductoId = request.SubProductoId;
        entrega.EstadoEntregaId = request.EstadoEntregaId;
        entrega.Placa = request.Placa;
        entrega.NombreConductor = request.NombreConductor;
        entrega.Kilos = request.Kilos;
        entrega.Cajas = request.Cajas;
        entrega.Sacos = request.Sacos;
        entrega.KilosSecos = request.KilosSecos;
        entrega.Pasillo = request.Pasillo;
        entrega.NumeroAnaquel = request.NumeroAnaquel;
        entrega.Piso = request.Piso;
        entrega.Observaciones = request.Observaciones;

        await _context.SaveChangesAsync();

        if (estadoAnterior != request.EstadoEntregaId)
        {
            var historial = await _historicoService.RegistrarAsync(new CrearHistoricoEstadoEntregaRequest
            {
                EntregaId = entrega.EntregaId,
                EstadoEntregaId = request.EstadoEntregaId,
                Observaciones = request.Observaciones
            });

            if (!historial.Success)
            {
                await transaction.RollbackAsync();
                return ApiResponse<bool>.Fail(historial.Message);
            }
        }

        await transaction.CommitAsync();

        return ApiResponse<bool>.Ok(true, "Entrega actualizada correctamente.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(int id)
    {
        var entrega = await _context.Entregas.FindAsync(id);

        if (entrega == null)
            return ApiResponse<bool>.Fail("Entrega no encontrada.");

        _context.Entregas.Remove(entrega);
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Entrega eliminada correctamente.");
    }

    private async Task<string?> ValidarReferenciasAsync(
        int productorId,
        int productoId,
        int? subProductoId,
        int estadoEntregaId)
    {
        var productorExiste = await _context.Productors.AnyAsync(p => p.ProductorId == productorId);
        if (!productorExiste)
            return "Productor no encontrado.";

        var productoExiste = await _context.Productos.AnyAsync(p => p.ProductoId == productoId);
        if (!productoExiste)
            return "Producto no encontrado.";

        var estadoExiste = await _context.EstadoEntregas.AnyAsync(e => e.EstadoEntregaId == estadoEntregaId);
        if (!estadoExiste)
            return "Estado de entrega no encontrado.";

        if (subProductoId.HasValue)
        {
            var subProducto = await _context.SubProductos
                .FirstOrDefaultAsync(s => s.SubProductoId == subProductoId.Value);

            if (subProducto == null)
                return "Subproducto no encontrado.";

            if (subProducto.ProductoId != productoId)
                return "El subproducto no pertenece al producto seleccionado.";
        }

        return null;
    }
}