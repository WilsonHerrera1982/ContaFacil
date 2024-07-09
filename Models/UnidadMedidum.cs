using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class UnidadMedidum
    {
        public UnidadMedidum()
        {
            Productos = new HashSet<Producto>();
        }

        public int IdUnidadMedida { get; set; }
        public string Nombre { get; set; } = null!;
        public string Abreviatura { get; set; } = null!;
        public bool? EstadoBoolean { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
