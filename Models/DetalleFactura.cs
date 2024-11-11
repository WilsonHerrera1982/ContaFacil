using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class DetalleFactura
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdDetalleFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? Descuento { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdImpuesto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual Impuesto? IdImpuestoNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
