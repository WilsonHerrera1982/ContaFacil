using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Producto
    {
        public Producto()
        {
            Inventarios = new HashSet<Inventario>();
            ProductoProveedors = new HashSet<ProductoProveedor>();
        }

        public int IdProducto { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int? IdCategoriaProducto { get; set; }
        public int? IdUnidadMedida { get; set; }
        public decimal? Stock { get; set; }
        public bool? EstadoBoolean { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public int? IdEmpresa { get; set; }

        public virtual CategoriaProducto? IdCategoriaProductoNavigation { get; set; }
        public virtual Empresa? IdEmpresaNavigation { get; set; }
        public virtual UnidadMedidum? IdUnidadMedidaNavigation { get; set; }
        public virtual ICollection<Inventario> Inventarios { get; set; }
        public virtual ICollection<ProductoProveedor> ProductoProveedors { get; set; }
    }
}
