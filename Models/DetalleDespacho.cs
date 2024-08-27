﻿using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class DetalleDespacho
{
    public int IdDetalleDespacho { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdDespacho { get; set; }

    public virtual Despacho? IdDespachoNavigation { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
