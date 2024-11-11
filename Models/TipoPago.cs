using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class TipoPago
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTipoPago { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? CodigoSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
