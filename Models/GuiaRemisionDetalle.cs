using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class GuiaRemisionDetalle
{
    public int IdGuiaRemisionDetalle { get; set; }

    public int IdGuiaRemision { get; set; }

    public int IdProducto { get; set; }

    public decimal Cantidad { get; set; }

    public string? Descripcion { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public string? CodigoPrincipal { get; set; }

    public string? CodigoAuxiliar { get; set; }

    public string? UnidadMedida { get; set; }

    public virtual GuiaRemision IdGuiaRemisionNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
