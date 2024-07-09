using ContaFacil.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [CustomRequired]
        public string Nombre { get; set; } = null!;
        [CustomRequired]
        public string? Direccion { get; set; }
        [CustomRequired]
        [RegularExpression(@"^\d+$", ErrorMessage = "Este campo debe contener solo dígitos.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El número de teléfono debe tener 10 dígitos")]
        public string? Telefono { get; set; }
        [CustomRequired]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string? Email { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        [CustomRequired]
        [RegularExpression(@"^\d+$", ErrorMessage = "Este campo debe contener solo dígitos.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El número de identificación debe tener 10 dígitos")]
        public string? Identificacion { get; set; }
        public int? IdEmpresa { get; set; }

        public virtual Empresa? IdEmpresaNavigation { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
