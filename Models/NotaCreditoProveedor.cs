using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class NotaCreditoProveedor
{
    public int IdNotaCredito { get; set; }

    public string Ambiente { get; set; } = null!;

    public string TipoEmision { get; set; } = null!;

    public string RazonSocial { get; set; } = null!;

    public string? NombreComercial { get; set; }

    public string Ruc { get; set; } = null!;

    public string ClaveAcceso { get; set; } = null!;

    public string CodDoc { get; set; } = null!;

    public string Estab { get; set; } = null!;

    public string PtoEmi { get; set; } = null!;

    public string Secuencial { get; set; } = null!;

    public string DirMatriz { get; set; } = null!;

    public DateOnly FechaEmision { get; set; }

    public string? DirEstablecimiento { get; set; }

    public string TipoIdentificacionComprador { get; set; } = null!;

    public string RazonSocialComprador { get; set; } = null!;

    public string IdentificacionComprador { get; set; } = null!;

    public string? ContribuyenteEspecial { get; set; }

    public bool? ObligadoContabilidad { get; set; }

    public string CodDocModificado { get; set; } = null!;

    public string NumDocModificado { get; set; } = null!;

    public DateOnly FechaEmisionDocSustento { get; set; }

    public decimal TotalSinImpuestos { get; set; }

    public decimal ValorModificacion { get; set; }

    public string Moneda { get; set; } = null!;

    public string Motivo { get; set; } = null!;

    public bool? EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual ICollection<NotaCreditoDetalle> NotaCreditoDetalles { get; set; } = new List<NotaCreditoDetalle>();

    public virtual ICollection<NotaCreditoInfoAdicional> NotaCreditoInfoAdicionals { get; set; } = new List<NotaCreditoInfoAdicional>();
}
