using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Tipocuentum
    {
        public Tipocuentum()
        {
            Cuenta = new HashSet<Cuentum>();
        }

        public int IdTipoCuenta { get; set; }
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Cuentum> Cuenta { get; set; }
    }
}
