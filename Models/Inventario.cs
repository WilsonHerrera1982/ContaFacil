using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Inventario
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdInventario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string TipoMovimiento { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Cantidad { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaMovimiento { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? NumeroDespacho { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Descripcion { get; set; }

    [DisplayName("Activo/Inactivo")]
    public bool? EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? Stock { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdSucursal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdCuentaContable { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? SubTotal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? Iva { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? Total { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? FacturaNumero { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? Descuento { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? PrecioUnitario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? PrecioUnitarioFinal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? Subtotal15 { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? PrecioCalculo { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? TransaccionRegistrada { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual ICollection<SucursalInventario> SucursalInventarios { get; set; } = new List<SucursalInventario>();
}
