using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Comision
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdComision { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPaquete { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Valor { get; set; }

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
    public string? Trial485 { get; set; }

    public virtual ICollection<ComisionContador> ComisionContadors { get; set; } = new List<ComisionContador>();

    public virtual Paquete IdPaqueteNavigation { get; set; } = null!;
}
