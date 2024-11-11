using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class PaqueteContador
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPaqueteContador { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPaquete { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdUsuario { get; set; }

    [DisplayName("Activo/Inactivo")]
    public bool? EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

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

    public virtual Paquete IdPaqueteNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
