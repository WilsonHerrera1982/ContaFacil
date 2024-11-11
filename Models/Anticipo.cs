using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Anticipo
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdAnticipo { get; set; }
    [DisplayName("Cliente")]
    public int IdCliente { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Valor { get; set; }
    [DisplayName("Número Comprobante")]
    public string? NumeroComprobante { get; set; }
    [DisplayName("Fecha Comprobante")]
    public DateTime FechaComprobante { get; set; }
    [DisplayName("Paguese a Orden")]
    public string? PagueseOrden { get; set; }
    [DisplayName("Número de Comprobante")]
    public string? NumeroCheque { get; set; }
    [DisplayName("Descripción")]
    public string? Descripcion { get; set; }
    [DisplayName("Fecha Cheque")]
    public DateTime FechaCheque { get; set; }
    [DisplayName("Activo/Inactivo")]
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
    [DisplayName("Tipo Pago")]
    public string TipoPago {  get; set; }

    public virtual ICollection<AnticipoCuentum> AnticipoCuenta { get; set; } = new List<AnticipoCuentum>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
