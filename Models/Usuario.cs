using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public int IdPersona { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdEmpresa { get; set; }

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
