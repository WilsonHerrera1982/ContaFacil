using System;
using System.Collections.Generic;

namespace ContaFacil.Models
{
    public partial class Emisor
    {
        public Emisor()
        {
            Facturas = new HashSet<Factura>();
        }

        public int IdEmisor { get; set; }
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; } = null!;
        public string NombreComercial { get; set; } = null!;
        public string Ruc { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public string Establecimiento { get; set; } = null!;
        public string PuntoEmision { get; set; } = null!;
        public string Secuencial { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string? CertificadoDigital { get; set; }
        public string Clave { get; set; } = null!;
        public string ObligadoContabilidad { get; set; } = null!;
        public string TipoAmbiente { get; set; } = null!;
        public bool? EstadoBoolean { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int? UsuarioModificacion { get; set; }

        public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<Factura> Facturas { get; set; }
    }
}
