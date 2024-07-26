﻿using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class SucursalFactura
{
    public int IdSucursalFactura { get; set; }

    public int IdFactura { get; set; }

    public int IdSucursal { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;
}
