using System;
using System.Collections.Generic;  using System.ComponentModel;

namespace ContaFacil.Models;

public partial class Parametro
{
    public int IdParametro { get; set; }

    public int IdEmpresa { get; set; }

    public string NombreParametro { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public string? Valor { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
