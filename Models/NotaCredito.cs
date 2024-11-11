using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class NotaCredito
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdNotaCredito { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdFactura { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NumeroNota { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NumeroAutorizacion { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string ClaveAcceso { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Xml { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Motivo { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaAutorizacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Descripcion { get; set; } = null!;

    [DisplayName("Activo/Inactivo")]
    public bool? EstadoBoolean { get; set; }

    [DisplayName("Fecha Creación")]  public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int UsuarioCreacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public int? UsuarioModificacion { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Factura IdFacturaNavigation { get; set; } = null!;
}
