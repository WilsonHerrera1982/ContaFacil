using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Cliente
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdCliente { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPersona { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }

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
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial482 { get; set; }

    public virtual ICollection<Anticipo> Anticipos { get; set; } = new List<Anticipo>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
