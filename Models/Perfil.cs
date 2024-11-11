using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Perfil
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPerfil { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Descripcion { get; set; }

    [DisplayName("Activo/Inactivo")]
    public bool? Estado { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? UsuarioCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual ICollection<MenuPerfil> MenuPerfils { get; set; } = new List<MenuPerfil>();

    public virtual ICollection<UsuarioPerfil> UsuarioPerfils { get; set; } = new List<UsuarioPerfil>();
}
