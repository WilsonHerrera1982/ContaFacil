using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class CategoriaProducto
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdCategoriaProducto { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Descripcion { get; set; }

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
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial485 { get; set; }

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
