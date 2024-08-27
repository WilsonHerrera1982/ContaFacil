using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace ContaFacil.Models;

public partial class Factura
{
    public int IdFactura { get; set; }

    public int IdCliente { get; set; }

    public DateOnly Fecha { get; set; }

    [DisplayName("Monto Total")]
    public decimal MontoTotal { get; set; }

    public string Estado { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdEmisor { get; set; }
    [DisplayName("Descripcion SRI")]
    public string? DescripcionSri { get; set; }

    public decimal? Subtotal { get; set; }

    public string? Xml { get; set; }
    [DisplayName("Clave Acceso")]
    public string? ClaveAcceso { get; set; }
    [DisplayName("Numero Factura")]
    public string? NumeroFactura { get; set; }

    public int? IdSucursal { get; set; }
    [DisplayName("Autorizacion SRI")]
    public string? AutorizacionSri { get; set; }
    [DisplayName("Fecha Autorización SRI")]
    public DateTime? FechaAutorizacionSri { get; set; }
    [DisplayName("Crédito")]
    public Boolean Credito {  get; set; }

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
