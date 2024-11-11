using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Menu
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdMenu { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }
        [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }
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
    public int? MenuId { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Controller { get; set; }
        public List<Menu> subMenus { get; set; }

    public virtual ICollection<MenuPerfil> MenuPerfils { get; set; } = new List<MenuPerfil>();
}
