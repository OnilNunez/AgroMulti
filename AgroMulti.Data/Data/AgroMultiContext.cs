using System;
using System.Collections.Generic;
using AgroMulti.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Data.Data;

public partial class AgroMultiContext : DbContext
{
    public AgroMultiContext()
    {
    }

    public AgroMultiContext(DbContextOptions<AgroMultiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Entrega> Entregas { get; set; }

    public virtual DbSet<EstadoEntrega> EstadoEntregas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Productor> Productors { get; set; }

    public DbSet<HistoricoEstadoEntrega> HistoricosEstadoEntrega { get; set; }

    public virtual DbSet<SubProducto> SubProductos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=AgroMulti;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.HasOne(d => d.EstadoEntrega).WithMany(p => p.Entregas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entrega_EstadoEntrega");

            entity.HasOne(d => d.Producto).WithMany(p => p.Entregas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entrega_Producto");

            entity.HasOne(d => d.Productor).WithMany(p => p.Entregas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entrega_Productor");

            entity.HasOne(d => d.SubProducto).WithMany(p => p.Entregas).HasConstraintName("FK_Entrega_SubProducto");
        });

        modelBuilder.Entity<SubProducto>(entity =>
        {
            entity.HasOne(d => d.Producto).WithMany(p => p.SubProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubProducto_Producto");
        });

        // ========== CONFIGURACIÓN DE HistoricoEstadoEntrega ==========
        modelBuilder.Entity<HistoricoEstadoEntrega>(entity =>
        {
            // Clave primaria
            entity.HasKey(e => e.HistoricoEstadoEntregaId);

            // Propiedades
            entity.Property(e => e.HistoricoEstadoEntregaId)
                  .UseIdentityColumn(); // IDENTITY(1,1)

            entity.Property(e => e.FechaCambio)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("GETDATE()"); // Valor por defecto al insertar

            entity.Property(e => e.Observaciones)
                  .HasMaxLength(500)
                  .IsRequired(false);

            // Relación con Entrega (1 a N)
            entity.HasOne(e => e.Entrega)
                  .WithMany(e => e.HistoricosEstado) // Debes tener ICollection<HistoricoEstadoEntrega> en Entrega
                  .HasForeignKey(e => e.EntregaId)
                  .OnDelete(DeleteBehavior.Cascade)  // Si se borra la entrega, se borra su historial
                  .HasConstraintName("FK_HistoricoEstadoEntrega_Entrega");

            // Relación con EstadoEntrega (1 a N, pero sin colección inversa en EstadoEntrega)
            entity.HasOne(e => e.EstadoEntrega)
                  .WithMany()  // No hay propiedad de navegación en EstadoEntrega (evita modificar esa clase)
                  .HasForeignKey(e => e.EstadoEntregaId)
                  .OnDelete(DeleteBehavior.Restrict) // No permitir borrar un estado si tiene historial
                  .HasConstraintName("FK_HistoricoEstadoEntrega_EstadoEntrega");

            
            entity.HasIndex(e => e.EntregaId)
                  .HasDatabaseName("IX_HistoricoEstadoEntrega_EntregaId");
            entity.HasIndex(e => e.FechaCambio)
                  .HasDatabaseName("IX_HistoricoEstadoEntrega_FechaCambio");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}