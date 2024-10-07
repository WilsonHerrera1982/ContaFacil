using System;
using System.Collections.Generic; 
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class HistoricoProducto
{
    public int IdHistoricoProducto { get; set; }

    public int IdProducto { get; set; }

    public int IdEmpresa { get; set; }

    public string NumeroDespacho { get; set; } = null!;

    public decimal PrecioUnitarioFinal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal PrecioVenta { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
