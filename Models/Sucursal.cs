using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Sucursal
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdSucursal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmisor { get; set; }
    [DisplayName("Nombre Sucursal")]
    public string NombreSucursal { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Usuario { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Clave { get; set; } = null!;
    [DisplayName("Dirección Sucursal")]
    public string DireccionSucursal { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Telefono { get; set; } = null!;
    [DisplayName("Punto Emisión")]
    public string PuntoEmision { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Secuencial { get; set; } = null!;

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
    public string? Trial482 { get; set; }

    public virtual ICollection<Despacho> Despachos { get; set; } = new List<Despacho>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<GuiaRemision> GuiaRemisions { get; set; } = new List<GuiaRemision>();

    public virtual Emisor IdEmisorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();

    public virtual ICollection<SucursalFactura> SucursalFacturas { get; set; } = new List<SucursalFactura>();

    public virtual ICollection<SucursalInventario> SucursalInventarios { get; set; } = new List<SucursalInventario>();

    public virtual ICollection<UsuarioSucursal> UsuarioSucursals { get; set; } = new List<UsuarioSucursal>();
}
