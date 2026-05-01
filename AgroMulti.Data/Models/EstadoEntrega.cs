using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Data.Models;

[Table("EstadoEntrega")]
public partial class EstadoEntrega
{
    [Key]
    public int EstadoEntregaId { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("EstadoEntrega")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
