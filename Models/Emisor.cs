using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Emisor
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmisor { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string RazonSocial { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NombreComercial { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Ruc { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NombreUsuario { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Telefono { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string CorreoElectronico { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Establecimiento { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string PuntoEmision { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Secuencial { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Direccion { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? CertificadoDigital { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Clave { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string ObligadoContabilidad { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string TipoAmbiente { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]
    public bool? EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial482 { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Sucursal> Sucursals { get; set; } = new List<Sucursal>();
}
