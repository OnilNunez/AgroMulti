using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Data.Models;

[Table("Producto")]
public partial class Producto
{
    [Key]
    public int ProductoId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("Producto")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    [InverseProperty("Producto")]
    public virtual ICollection<SubProducto> SubProductos { get; set; } = new List<SubProducto>();
}
