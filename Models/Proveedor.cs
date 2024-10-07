using System;
using System.Collections.Generic;  
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public bool Estado { get; set; }

    public string Identificacion {  get; set; }
    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdEmpresa { get; set; }
    [DisplayName("Retención en la Fuente")]
    public decimal? RetencionPorcentaje { get; set; }
    [DisplayName("Retención IVA")]
    public decimal? RetencionIva { get; set; }

    public virtual Empresa? IdEmpresaNavigation { get; set; }
}
