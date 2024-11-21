using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class CuentasPorPagar
{
    public int IdCuenta { get; set; }

    public string TipoDocumento { get; set; } = null!;

    public string TipoProveedor { get; set; } = null!;

    public string RazonSocial { get; set; } = null!;

    public string RucIdentificacion { get; set; } = null!;

    public string NumeroDocumento { get; set; } = null!;

    public DateOnly FechaEmision { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal MontoTotal { get; set; }

    public decimal MontoPendiente { get; set; }

    public decimal? Impuestos { get; set; }

    public decimal? Descuentos { get; set; }

    public string FormaPago { get; set; } = null!;

    public string? NumeroReferencia { get; set; }

    public string? Categoria { get; set; }

    public string? Descripcion { get; set; }

    public string Estado { get; set; } = null!;

    public string UsuarioCreacion { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? CentroCosto { get; set; }

    public string? Proyecto { get; set; }

    public virtual ICollection<AnticipoCuentaPagar> AnticipoCuentaPagars { get; set; } = new List<AnticipoCuentaPagar>();
}
