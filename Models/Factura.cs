using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaFacil.Models;

public partial class Factura
{
    public int IdFactura { get; set; }

    public int IdCliente { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal MontoTotal { get; set; }

    public string Estado { get; set; } = null!;

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdEmisor { get; set; }

    public string? DescripcionSri { get; set; }
    public decimal Subtotal { get; set; }
    public string? Xml { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? NumeroFactura {  get; set; }
    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Emisor? IdEmisorNavigation { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    [NotMapped]
   public virtual int? idPago {  get; set; }
}
