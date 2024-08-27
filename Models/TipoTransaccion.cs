using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

public partial class TipoTransaccion
{
    public int IdTipoTransaccion { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
}
