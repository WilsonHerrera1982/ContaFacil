using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class DetalleDespacho
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdDetalleDespacho { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? EstadoBoolean { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaCreacion { get; set; }

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
    public int? IdDespacho { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual Despacho? IdDespachoNavigation { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
