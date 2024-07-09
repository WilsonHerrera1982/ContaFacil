using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Persona
    {
        public Persona()
        {
            Clientes = new HashSet<Cliente>();
            Usuarios = new HashSet<Usuario>();
        }

        public int IdPersona { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public string? Identificacion { get; set; }
        public int? IdEmpresa { get; set; }

        public virtual Empresa? IdEmpresaNavigation { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
