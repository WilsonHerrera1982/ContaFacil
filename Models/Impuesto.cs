using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Impuesto
    {
        public Impuesto()
        {
            Productos = new HashSet<Producto>();
        }

        public int IdImpuesto { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Porcentaje { get; set; }
        public bool? EstadoBoolean { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public string? CodigoSri { get; set; }
        public string? CodigoPorcentajeSri { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
