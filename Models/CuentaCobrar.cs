using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class CuentaCobrar
{
    public int IdCuentaCobrar { get; set; }

    public int IdEmpresa { get; set; }

    public int IdFactura { get; set; }

    public int PlazoDias { get; set; }

    public decimal PrecioUnitarioFinal { get; set; }

    public decimal Impuesto { get; set; }

    public string? EstadoCobro { get; set; }

    public decimal PrecioVenta { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Factura IdFacturaNavigation { get; set; } = null!;
}
