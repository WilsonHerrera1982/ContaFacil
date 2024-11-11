using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Transportistum
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTransportista { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string TipoIdentificacion { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NumeroIdentificacion { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string RazonSocial { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string PlacaVehiculo { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? UsuarioRegistro { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual ICollection<GuiaRemision> GuiaRemisions { get; set; } = new List<GuiaRemision>();
}
