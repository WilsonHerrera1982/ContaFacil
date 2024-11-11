using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class TipoIdentificacion
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTipoIdemtificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string CodigoSri { get; set; } = null!;

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
    public string? Trial482 { get; set; }

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
