using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class ProductoProveedor
{
    public int IdProductoProveedor { get; set; }

    public int? IdProducto { get; set; }

    public int IdProveedor { get; set; }

    public decimal PrecioCompra { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
