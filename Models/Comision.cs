using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Comision
    {
        public Comision()
        {
            ComisionContadors = new HashSet<ComisionContador>();
        }

        public int IdComision { get; set; }
        public int IdPaquete { get; set; }
        public decimal Valor { get; set; }
        public bool? EstadoBoolean { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }

        public virtual Paquete IdPaqueteNavigation { get; set; } = null!;
        public virtual ICollection<ComisionContador> ComisionContadors { get; set; }
    }
}
