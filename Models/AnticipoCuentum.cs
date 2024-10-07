using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class AnticipoCuentum
{
    public int IdAnticipoCuenta { get; set; }

    public int IdCuenta { get; set; }

    public int IdAnticipo { get; set; }

    public decimal Valor { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Anticipo IdAnticipoNavigation { get; set; } = null!;

    public virtual CuentaCobrar IdCuentaNavigation { get; set; } = null!;
}
