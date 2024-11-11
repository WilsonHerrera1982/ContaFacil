using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Parametro
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdParametro { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NombreParametro { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Descripcion { get; set; } = null!;

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
    public string? Valor { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
