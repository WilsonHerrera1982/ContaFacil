using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Proveedor
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdProveedor { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Email { get; set; }

    [DisplayName("Activo/Inactivo")]
    public bool? Estado { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaCreacion { get; set; }

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
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Identificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal? RetencionPorcentaje { get; set; }
    [DisplayName("Retención IVA")]
    public decimal? RetencionIva { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial492 { get; set; }

    public virtual ICollection<AnticiposProveedor> AnticiposProveedors { get; set; } = new List<AnticiposProveedor>();

    public virtual Empresa? IdEmpresaNavigation { get; set; }
}
