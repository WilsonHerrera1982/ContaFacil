using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class Impuesto
{
    public int IdImpuesto { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Porcentaje { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int UsuarioModificacion { get; set; }

    public string? CodigoSri { get; set; }

    public string? CodigoPorcentajeSri { get; set; }

    public string? Tipo { get; set; }

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
