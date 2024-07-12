using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class TipoIdentificacion
    {
        public int IdTipoIdemtificacion { get; set; }
        public int CodigoSri { get; set; }
        public string Descripcion { get; set; } = null!;
        public bool? EstadoBoolean { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
