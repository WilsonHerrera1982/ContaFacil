using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaFacil.Models;

public partial class Despacho
{
    public int IdDespacho { get; set; }

    public int IdUsuario { get; set; }

    public int IdEmpresa { get; set; }

    public int IdSucursal { get; set; }

    public string NumeroDespacho { get; set; } = null!;

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public string? EstadoDespacho { get; set; }

    public int? IdSucursalDestino { get; set; }
    [NotMapped]
    public string? NombreSucursalDestino { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<DetalleDespacho> DetalleDespachos { get; set; }
    public void CargarNombreSucursalDestino(DbContext context)
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
