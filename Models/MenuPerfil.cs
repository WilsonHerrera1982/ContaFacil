using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class MenuPerfil
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdMenuPerfil { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdMenu { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPerfil { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }

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
    public string? Trial489 { get; set; }

    public virtual Menu IdMenuNavigation { get; set; } = null!;

    public virtual Perfil IdPerfilNavigation { get; set; } = null!;
}
