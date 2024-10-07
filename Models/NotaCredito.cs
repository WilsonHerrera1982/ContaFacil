using System;
using System.Collections.Generic;  
using System.ComponentModel;
namespace ContaFacil.Models;

public partial class NotaCredito
{
    public int IdNotaCredito { get; set; }

    public int IdEmpresa { get; set; }

    public int IdFactura { get; set; }

    public string NumeroNota { get; set; } = null!;

    public string NumeroAutorizacion { get; set; } = null!;

    public string ClaveAcceso { get; set; } = null!;

    public string Xml { get; set; } = null!;

    public string Motivo { get; set; } = null!;

    public DateTime? FechaAutorizacion { get; set; }

    public string Descripcion { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]  public bool EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

}
