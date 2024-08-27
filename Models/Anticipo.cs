using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Anticipo
{
    public int IdAnticipo { get; set; }

    public int IdCliente { get; set; }

    public int IdEmpresa { get; set; }

    public decimal Valor { get; set; }

    public string? NumeroComprobante { get; set; }

    public DateTime FechaComprobante { get; set; }

    public string? PagueseOrden { get; set; }

    public string? NumeroCheque { get; set; }

    public string? Descripcion { get; set; }

    public DateTime FechaCheque { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual ICollection<AnticipoCuentum> AnticipoCuenta { get; set; } = new List<AnticipoCuentum>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
