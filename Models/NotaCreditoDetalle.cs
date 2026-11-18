using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class NotaCreditoDetalle
{
    public int IdNotaCreditoDetalle { get; set; }

    public int IdNotaCredito { get; set; }

    public string CodigoInterno { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Descuento { get; set; }

    public decimal PrecioTotalSinImpuesto { get; set; }

    public string CodigoImpuesto { get; set; } = null!;

    public string CodigoPorcentaje { get; set; } = null!;

    public decimal Tarifa { get; set; }

    public decimal BaseImponible { get; set; }

    public decimal ValorImpuesto { get; set; }

    public bool? EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual NotaCreditoProveedor IdNotaCreditoNavigation { get; set; } = null!;
}
