using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class TipoPago
    {
        public TipoPago()
        {
            Pagos = new HashSet<Pago>();
        }

        public int IdTipoPago { get; set; }
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Pago> Pagos { get; set; }
    }
}
