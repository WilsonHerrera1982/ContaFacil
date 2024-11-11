using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Usuario
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Clave { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPersona { get; set; }

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
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial482 { get; set; }

    public virtual ICollection<Despacho> Despachos { get; set; } = new List<Despacho>();

    public virtual ICollection<DetalleDespacho> DetalleDespachos { get; set; } = new List<DetalleDespacho>();

    public virtual ICollection<Emisor> Emisors { get; set; } = new List<Emisor>();

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    public virtual ICollection<PaqueteContador> PaqueteContadors { get; set; } = new List<PaqueteContador>();

    public virtual ICollection<Sucursal> Sucursals { get; set; } = new List<Sucursal>();

    public virtual ICollection<UsuarioPerfil> UsuarioPerfils { get; set; } = new List<UsuarioPerfil>();

    public virtual ICollection<UsuarioSucursal> UsuarioSucursals { get; set; } = new List<UsuarioSucursal>();

    public virtual ICollection<VentaPaquete> VentaPaquetes { get; set; } = new List<VentaPaquete>();
}
