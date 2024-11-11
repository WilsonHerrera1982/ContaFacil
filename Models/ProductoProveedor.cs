using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class ProductoProveedor
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdProductoProveedor { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdProveedor { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal PrecioCompra { get; set; }

    [DisplayName("Activo/Inactivo")]
    public bool? EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? Cantidad { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
