using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public int IdPersona { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdEmpresa { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
