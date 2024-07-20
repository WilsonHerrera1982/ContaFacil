using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class DetalleFactura
{
    public int IdDetalleFactura { get; set; }

    public int IdFactura { get; set; }

    public string? Descripcion { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdProducto { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual Producto? IdProductoNavigation { get; set; }
}
