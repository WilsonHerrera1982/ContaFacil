using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class NotaCreditoInfoAdicional
{
    public int IdInfoAdicional { get; set; }

    public int IdNotaCredito { get; set; }

    public string Nombre { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public bool? EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual NotaCreditoProveedor IdNotaCreditoNavigation { get; set; } = null!;
}
