using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Perfil
    {
        public Perfil()
        {
            MenuPerfils = new HashSet<MenuPerfil>();
            UsuarioPerfils = new HashSet<UsuarioPerfil>();
        }

        public int IdPerfil { get; set; }
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }

        public virtual ICollection<MenuPerfil> MenuPerfils { get; set; }
        public virtual ICollection<UsuarioPerfil> UsuarioPerfils { get; set; }
    }
}
