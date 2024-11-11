using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Transaccion
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTransaccion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdCuenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateOnly Fecha { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTipoTransaccion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaModificacion { get; set; }

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
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdInventario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? EsDebito { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial492 { get; set; }

    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual TipoTransaccion IdTipoTransaccionNavigation { get; set; } = null!;
}
