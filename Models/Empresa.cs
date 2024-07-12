using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Empresa
    {
        public Empresa()
        {
            Clientes = new HashSet<Cliente>();
            Cuenta = new HashSet<Cuentum>();
            Emisors = new HashSet<Emisor>();
            Personas = new HashSet<Persona>();
            Productos = new HashSet<Producto>();
            Proveedors = new HashSet<Proveedor>();
            Transaccions = new HashSet<Transaccion>();
            Usuarios = new HashSet<Usuario>();
        }

        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }

        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Cuentum> Cuenta { get; set; }
        public virtual ICollection<Emisor> Emisors { get; set; }
        public virtual ICollection<Persona> Personas { get; set; }
        public virtual ICollection<Producto> Productos { get; set; }
        public virtual ICollection<Proveedor> Proveedors { get; set; }
        public virtual ICollection<Transaccion> Transaccions { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
