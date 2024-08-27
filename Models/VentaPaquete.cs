using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

public partial class VentaPaquete
{
    public int IdVentaPaquete { get; set; }

    public int IdPaquete { get; set; }

    public int IdUsuario { get; set; }

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Paquete IdPaqueteNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
