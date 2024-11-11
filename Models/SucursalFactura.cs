using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class SucursalFactura
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdSucursalFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdSucursal { get; set; }

    [DisplayName("Activo/Inactivo")]
    public bool? EstadoBoolean { get; set; }

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
    public string? Trial492 { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;
}
