using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// tabla para el registro de constatacion fisica de productos por empresa
/// </summary>
public partial class ConstatacionFisica
{
    public int ConstatacionId { get; set; }

    public string? ConstatacionDescripcion { get; set; }

    public int EmpresaId { get; set; }

    public int ProductoId { get; set; }

    public int ProductoStockActual { get; set; }

    public int? CantidadFisica { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int Estado { get; set; }

    public virtual Empresa Empresa { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
