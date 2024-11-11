using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class GuiaRemisionDetalle
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdGuiaRemisionDetalle { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdGuiaRemision { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Cantidad { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Descripcion { get; set; }

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
    public string? CodigoPrincipal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? CodigoAuxiliar { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? UnidadMedida { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual GuiaRemision IdGuiaRemisionNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
