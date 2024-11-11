using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Despacho
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdDespacho { get; set; }

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
    public int IdSucursal { get; set; }
    [DisplayName("Número Despacho")]
    public string NumeroDespacho { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

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
    [DisplayName("Estado Despacho")]
    public string? EstadoDespacho { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? IdSucursalDestino { get; set; }
    [NotMapped]
    [DisplayName("Nombre Sucursal Destino")]
    public string? NombreSucursalDestino { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<DetalleDespacho> DetalleDespachos { get; set; }
    public void CargarNombreSucursalDestino(ContableContext context)
    {
        if (IdSucursalDestino == null)
        {
            NombreSucursalDestino = string.Empty;
            return;
        }

        NombreSucursalDestino = context.Set<Sucursal>()
            .Where(s => s.IdSucursal == IdSucursalDestino)
            .Select(s => s.NombreSucursal)
            .FirstOrDefault() ?? string.Empty;
    }
}
