using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class ProductoProveedor
{
    public int IdProductoProveedor { get; set; }

    public int? IdProducto { get; set; }

    public int IdProveedor { get; set; }

    public decimal PrecioCompra { get; set; }
    public int Cantidad {  get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
