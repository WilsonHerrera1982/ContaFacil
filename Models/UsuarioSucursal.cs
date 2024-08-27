﻿using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

public partial class UsuarioSucursal
{
    public int IdUsuarioSucursal { get; set; }

    public int IdUsuario { get; set; }

    public int IdSucursal { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
