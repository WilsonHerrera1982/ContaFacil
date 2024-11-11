using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class CuentaCobrar
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdCuentaCobrar { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int PlazoDias { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal PrecioUnitarioFinal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Impuesto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? EstadoCobro { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal PrecioVenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? EstadoBoolean { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaCreacion { get; set; }

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
    public string? Trial485 { get; set; }

    public virtual ICollection<AnticipoCuentum> AnticipoCuenta { get; set; } = new List<AnticipoCuentum>();

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Factura IdFacturaNavigation { get; set; } = null!;
}
