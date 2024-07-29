using System;
using System.Collections.Generic;

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

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public int? Stock { get; set; }

    public int? IdSucursal { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual ICollection<SucursalInventario> SucursalInventarios { get; set; } = new List<SucursalInventario>();
}
