using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Empresa
{
    public int IdEmpresa { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public virtual ICollection<CategoriaProducto> CategoriaProductos { get; set; } = new List<CategoriaProducto>();

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();

    public virtual ICollection<Emisor> Emisors { get; set; } = new List<Emisor>();

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
