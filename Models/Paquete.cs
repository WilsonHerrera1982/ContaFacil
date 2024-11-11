using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class Paquete
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdPaquete { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public int CantidadEmisores { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public decimal Valor { get; set; }

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
    public string? Trial485 { get; set; }

    public virtual ICollection<Comision> Comisions { get; set; } = new List<Comision>();

    public virtual ICollection<PaqueteContador> PaqueteContadors { get; set; } = new List<PaqueteContador>();

    public virtual ICollection<VentaPaquete> VentaPaquetes { get; set; } = new List<VentaPaquete>();
}
