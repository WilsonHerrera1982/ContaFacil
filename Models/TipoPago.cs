using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

public partial class TipoPago
{
    public int IdTipoPago { get; set; }

    public string Nombre { get; set; } = null!;
    public string? CodigoSri {  get; set; }
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
