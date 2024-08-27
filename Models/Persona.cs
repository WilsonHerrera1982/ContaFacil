﻿using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class Persona
{
    public int IdPersona { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public bool Estado { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public string? Identificacion { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdTipoIdentificacion { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual TipoIdentificacion? IdTipoIdentificacionNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
