using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Factura
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdCliente { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateOnly Fecha { get; set; }

    [DisplayName("Monto Total")]
    public decimal MontoTotal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Estado { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

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
    public int? IdEmisor { get; set; }
    [DisplayName("Descripcion SRI")]
    public string? DescripcionSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? Subtotal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Xml { get; set; }
    [DisplayName("Clave Acceso")]
    public string? ClaveAcceso { get; set; }
    [DisplayName("Numero Factura")]
    public string? NumeroFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdSucursal { get; set; }
    [DisplayName("Autorizacion SRI")]
    public string? AutorizacionSri { get; set; }
    [DisplayName("Fecha Autorización SRI")]
    public DateTime? FechaAutorizacionSri { get; set; }
    [DisplayName("Crédito")]
    public Boolean Credito {  get; set; }
    public int? IdImpuesto { get; set; }

    [NotMapped]
    public virtual int? idPago { get; set; }
    public virtual ICollection<CuentaCobrar> CuentaCobrars { get; set; } = new List<CuentaCobrar>();

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Emisor? IdEmisorNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual ICollection<NotaCredito> NotaCreditos { get; set; } = new List<NotaCredito>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<SucursalFactura> SucursalFacturas { get; set; } = new List<SucursalFactura>();
}
