using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class HistoricoProducto
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdHistoricoProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NumeroDespacho { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal PrecioUnitarioFinal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Impuesto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal PrecioVenta { get; set; }

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
    public string? Trial489 { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
