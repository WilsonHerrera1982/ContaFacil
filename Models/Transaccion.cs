using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

public partial class Transaccion
{
    public int IdTransaccion { get; set; }

    public int IdCuenta { get; set; }

    public DateOnly Fecha { get; set; }

    public int IdTipoTransaccion { get; set; }

    public decimal Monto { get; set; }

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdEmpresa { get; set; }
    public int? IdInventario { get; set; }

    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual TipoTransaccion IdTipoTransaccionNavigation { get; set; } = null!;
}
