using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class ComisionContador
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdComsionContador { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdComision { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Estado { get; set; } = null!;

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

    public virtual Comision IdComisionNavigation { get; set; } = null!;
}
