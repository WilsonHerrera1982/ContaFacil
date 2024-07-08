using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class TipoTransaccion
    {
        public TipoTransaccion()
        {
            Transaccions = new HashSet<Transaccion>();
        }

        public int IdTipoTransaccion { get; set; }
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Transaccion> Transaccions { get; set; }
    }
}
