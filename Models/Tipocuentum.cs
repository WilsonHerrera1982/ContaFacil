using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Tipocuentum
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTipoCuenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial485 { get; set; }

    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();
}
