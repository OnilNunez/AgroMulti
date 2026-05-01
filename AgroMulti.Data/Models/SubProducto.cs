using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Data.Models;

[Table("SubProducto")]
public partial class SubProducto
{
    [Key]
    public int SubProductoId { get; set; }

    public int ProductoId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("SubProducto")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    [ForeignKey("ProductoId")]
    [InverseProperty("SubProductos")]
    public virtual Producto Producto { get; set; } = null!;
}
