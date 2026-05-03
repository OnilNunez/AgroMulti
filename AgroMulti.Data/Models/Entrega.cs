using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Data.Models;

[Table("Entrega")]
[Index("NumeroEntrega", Name = "UQ_Entrega_NumeroEntrega", IsUnique = true)]
public partial class Entrega
{
    [Key]
    public int EntregaId { get; set; }

    [StringLength(50)]
    public string NumeroEntrega { get; set; } = null!;

    public DateOnly FechaEntrega { get; set; }

    public int ProductorId { get; set; }

    public int ProductoId { get; set; }

    public int? SubProductoId { get; set; }

    public int EstadoEntregaId { get; set; }

    [StringLength(20)]
    public string? Placa { get; set; }

    [StringLength(100)]
    public string? NombreConductor { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Kilos { get; set; }

    public int Cajas { get; set; }

    public int Sacos { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? KilosSecos { get; set; }

    [StringLength(50)]
    public string? Pasillo { get; set; }

    [StringLength(50)]
    public string? NumeroAnaquel { get; set; }

    [StringLength(50)]
    public string? Piso { get; set; }

    public string? Observaciones { get; set; }

    [ForeignKey("EstadoEntregaId")]
    [InverseProperty("Entregas")]
    public virtual EstadoEntrega EstadoEntrega { get; set; } = null!;

    [ForeignKey("ProductoId")]
    [InverseProperty("Entregas")]
    public virtual Producto Producto { get; set; } = null!;

    [ForeignKey("ProductorId")]
    [InverseProperty("Entregas")]
    public virtual Productor Productor { get; set; } = null!;

    [ForeignKey("SubProductoId")]
    [InverseProperty("Entregas")]
    public virtual SubProducto? SubProducto { get; set; }

    public virtual ICollection<HistoricoEstadoEntrega> HistoricosEstado { get; set; } = new List<HistoricoEstadoEntrega>();
}