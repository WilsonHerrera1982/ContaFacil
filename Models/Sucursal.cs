using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Sucursal
{
    public int IdSucursal { get; set; }

    public int IdUsuario { get; set; }

    public int IdEmisor { get; set; }

    public string NombreSucursal { get; set; } = null!;

    public string Usuario { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string DireccionSucursal { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string PuntoEmision { get; set; } = null!;

    public string Secuencial { get; set; } = null!;

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual ICollection<Despacho> Despachos { get; set; } = new List<Despacho>();

    public virtual Emisor IdEmisorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<SucursalInventario> SucursalInventarios { get; set; } = new List<SucursalInventario>();
    public virtual ICollection<SucursalFactura> SucursalFacturas { get; set; } = new List<SucursalFactura>();


    public virtual ICollection<UsuarioSucursal> UsuarioSucursals { get; set; } = new List<UsuarioSucursal>();
}
