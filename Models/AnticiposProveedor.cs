using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class AnticiposProveedor
{
    public int IdAnticipo { get; set; }

    public int IdProveedor { get; set; }

    public string NumeroAnticipo { get; set; } = null!;

    public decimal MontoAnticipo { get; set; }

    public DateOnly FechaAnticipo { get; set; }

    public string FormaPago { get; set; } = null!;

    public string? NumeroReferencia { get; set; }

    public string? Estado { get; set; }

    public string? Descripcion { get; set; }

    public int UsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<AnticipoCuentaPagar> AnticipoCuentaPagars { get; set; } = new List<AnticipoCuentaPagar>();

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}
