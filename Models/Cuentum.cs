using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Cuentum
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdCuenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTipoCuenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal SaldoInicial { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal SaldoActual { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Estado { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

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
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdIdCuenta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Codigo { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Debito { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? Credito { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial485 { get; set; }

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual Tipocuentum IdTipoCuentaNavigation { get; set; } = null!;

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
}
