using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class Empresa
{
    public int IdEmpresa { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public bool Estado { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public virtual ICollection<CategoriaProducto> CategoriaProductos { get; set; } = new List<CategoriaProducto>();

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();

    public virtual ICollection<Despacho> Despachos { get; set; } = new List<Despacho>();

    public virtual ICollection<Emisor> Emisors { get; set; } = new List<Emisor>();

    public virtual ICollection<HistoricoProducto> HistoricoProductos { get; set; } = new List<HistoricoProducto>();

    public virtual ICollection<NotaCredito> NotaCreditos { get; set; } = new List<NotaCredito>();

    public virtual ICollection<Parametro> Parametros { get; set; } = new List<Parametro>();

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
