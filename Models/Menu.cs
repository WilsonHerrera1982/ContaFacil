﻿using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Menu
    {
        public Menu()
        {
            MenuPerfils = new HashSet<MenuPerfil>();
        }

        public int IdMenu { get; set; }
        public string Descripcion { get; set; } = null!;
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public int? MenuId { get; set; }
        public string? Url { get; set; }
        public List<Menu> subMenus { get; set; }

        public virtual ICollection<MenuPerfil> MenuPerfils { get; set; }
    }
}
