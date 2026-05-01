using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Data.Models;

[Table("Productor")]
[Index("Codigo", Name = "UQ_Productor_Codigo", IsUnique = true)]
public partial class Productor
{
    [Key]
    public int ProductorId { get; set; }

    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    public string Apellido { get; set; } = null!;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(200)]
    public string? Direccion { get; set; }

    [InverseProperty("Productor")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
