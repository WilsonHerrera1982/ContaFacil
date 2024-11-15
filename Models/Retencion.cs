using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Retencion
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdRetencion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Xml { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdProveedor { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? NumeroFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? ComprobanteRetencion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? NumeroAutorizacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? ClaveAcceso { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? EjercicioFiscal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal BaseImponible { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Impuesto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? PorcentajeRetencion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? ValorRetenido { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? TipoContribuyente { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? EstadoBoolean { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaAutorizacion { get; set; }

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
    public string? Proveedor { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
