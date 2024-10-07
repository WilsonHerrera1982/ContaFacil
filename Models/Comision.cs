using System;
using System.Collections.Generic; 
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class Comision
{
    public int IdComision { get; set; }

    public int IdPaquete { get; set; }

    public decimal Valor { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual ICollection<ComisionContador> ComisionContadors { get; set; } = new List<ComisionContador>();

    public virtual Paquete IdPaqueteNavigation { get; set; } = null!;
}
