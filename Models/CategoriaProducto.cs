using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class CategoriaProducto
    {
        public CategoriaProducto()
        {
            Productos = new HashSet<Producto>();
        }

        public int IdCategoriaProducto { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool? EstadoBoolean { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
