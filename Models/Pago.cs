using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Pago
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPago { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateOnly Fecha { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTipoPago { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }

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

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual TipoPago IdTipoPagoNavigation { get; set; } = null!;
}
