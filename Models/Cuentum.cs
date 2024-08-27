using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

public partial class Cuentum
{
    public int IdCuenta { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdTipoCuenta { get; set; }

    public decimal SaldoInicial { get; set; }

    public decimal SaldoActual { get; set; }

    public bool Estado { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdEmpresa { get; set; }

    /// <summary>
    /// id de la cuenta padre
    /// </summary>
    public int? IdIdCuenta { get; set; }

    public string? Codigo { get; set; }

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual Tipocuentum IdTipoCuentaNavigation { get; set; } = null!;

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
}
