using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class AnticipoCuentum
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdAnticipoCuenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdCuenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdAnticipo { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Valor { get; set; }

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
    public string? Trial485 { get; set; }

    public virtual Anticipo IdAnticipoNavigation { get; set; } = null!;

    public virtual CuentaCobrar IdCuentaNavigation { get; set; } = null!;
}
