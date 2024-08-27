using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;

namespace ContaFacil.Models;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int? IdProducto { get; set; }

    public string? TipoMovimiento { get; set; }

    public decimal? Cantidad { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public string? NumeroDespacho { get; set; }

    public string? Descripcion { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public int? Stock { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdCuentaContable { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Iva { get; set; }

    public decimal? Total { get; set; }

    public string? NumeroFactura {  get; set; }
    public decimal? Descuento { get; set; }
    public decimal? PrecioUnitario { get; set; }
    public decimal? PrecioUnitarioFinal { get; set; }
    public decimal? Subtotal15 { get; set; }
    public decimal? PrecioCalculo { get; set; }
    public bool TransaccionRegistrada { get; set; }
    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual ICollection<SucursalInventario> SucursalInventarios { get; set; } = new List<SucursalInventario>();
}
