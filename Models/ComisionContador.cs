using System;
using System.Collections.Generic; 
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class ComisionContador
{
    public int IdComsionContador { get; set; }

    public int IdComision { get; set; }

    public decimal Valor { get; set; }

    public string Estado { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Comision IdComisionNavigation { get; set; } = null!;
}
