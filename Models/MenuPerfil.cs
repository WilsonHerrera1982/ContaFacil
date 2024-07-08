using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class MenuPerfil
    {
        public int IdMenuPerfil { get; set; }
        public int IdMenu { get; set; }
        public int IdPerfil { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }

        public virtual Menu IdMenuNavigation { get; set; } = null!;
        public virtual Perfil IdPerfilNavigation { get; set; } = null!;
    }
}
