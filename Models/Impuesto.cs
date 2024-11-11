using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Impuesto
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdImpuesto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Porcentaje { get; set; }

    [DisplayName("Activo/Inactivo")]
    public bool? EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha Modificación")]  public DateTime FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? CodigoSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? CodigoPorcentajeSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Tipo { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial485 { get; set; }

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
