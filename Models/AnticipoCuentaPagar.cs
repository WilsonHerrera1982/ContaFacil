using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class AnticipoCuentaPagar
{
    public int IdRelacion { get; set; }

    public int IdAnticipo { get; set; }

    public int IdCuentaPorPagar { get; set; }

    public int UsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual AnticiposProveedor IdAnticipoNavigation { get; set; } = null!;

    public virtual CuentasPorPagar IdCuentaPorPagarNavigation { get; set; } = null!;
}
