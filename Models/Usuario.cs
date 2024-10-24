﻿using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            UsuarioPerfils = new HashSet<UsuarioPerfil>();
        }

        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public int IdPersona { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public int? IdEmpresa { get; set; }

        public virtual Empresa? IdEmpresaNavigation { get; set; }
        public virtual Persona IdPersonaNavigation { get; set; } = null!;
        public virtual ICollection<UsuarioPerfil> UsuarioPerfils { get; set; }
    }
}
