using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdFactura { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal Monto { get; set; }

    public int IdTipoPago { get; set; }

    public bool Estado { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual TipoPago IdTipoPagoNavigation { get; set; } = null!;
}
