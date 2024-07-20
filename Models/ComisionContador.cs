using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class ComisionContador
{
    public int IdComsionContador { get; set; }

    public int IdComision { get; set; }

    public decimal Valor { get; set; }

    public string Estado { get; set; } = null!;

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Comision IdComisionNavigation { get; set; } = null!;
}
