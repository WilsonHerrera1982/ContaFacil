using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaFacil.Models;

public partial class Producto
{
    public int IdProducto { get; set; }
    [DisplayName("Código")]
    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;
    [DisplayName("Descripción")]
    public string? Descripcion { get; set; }
    [DisplayName("Subtotal")]
    public decimal PrecioUnitario { get; set; }

    public int? IdCategoriaProducto { get; set; }

    public int? IdUnidadMedida { get; set; }

    public decimal? Stock { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdImpuesto { get; set; }
    [DisplayName("Precio Venta")]
    public decimal? PrecioVenta { get; set; }
    [DisplayName("IVA")]
    [NotMapped]
    public decimal? Iva { get; set; }
    [DisplayName("Utilidad")]
    [NotMapped]
    public decimal? Comision { get; set; }
    public int? Utilidad { get; set; }
    [NotMapped]
    public decimal? Porcentaje
    {
        get
        {
            return IdImpuestoNavigation?.Porcentaje;
        }
    }
    public decimal? Descuento { get; set; }
    public virtual ICollection<DetalleDespacho> DetalleDespachos { get; set; } = new List<DetalleDespacho>();

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual ICollection<HistoricoProducto> HistoricoProductos { get; set; } = new List<HistoricoProducto>();

    public virtual CategoriaProducto? IdCategoriaProductoNavigation { get; set; }

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual Impuesto? IdImpuestoNavigation { get; set; }

    public virtual UnidadMedidum? IdUnidadMedidaNavigation { get; set; }

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();

    public virtual ICollection<ProductoProveedor> ProductoProveedors { get; set; } = new List<ProductoProveedor>();
}
