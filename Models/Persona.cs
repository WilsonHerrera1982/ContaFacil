using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Persona
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPersona { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Email { get; set; }

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
    public string? Identificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdTipoIdentificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? RetencionIva { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? RetencionPorcentaje { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial482 { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual TipoIdentificacion? IdTipoIdentificacionNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
