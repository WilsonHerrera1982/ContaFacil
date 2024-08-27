using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Retencion
{
    public int IdRetencion { get; set; }

    public int IdEmpresa { get; set; }

    public string? Xml { get; set; }

    public int? IdFactura { get; set; }

    public int? IdProveedor { get; set; }

    public string? NumeroFactura { get; set; }

    public string? ComprobanteRetencion { get; set; }

    public string? NumeroAutorizacion { get; set; }

    public string? ClaveAcceso { get; set; }

    public string? EjercicioFiscal { get; set; }

    public decimal BaseImponible { get; set; }

    public string? Impuesto { get; set; }

    public decimal? PorcentajeRetencion { get; set; }

    public decimal? ValorRetenido { get; set; }

    public string? TipoContribuyente { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaAutorizacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
