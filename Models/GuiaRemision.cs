using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// TRIAL
/// </summary>
public partial class GuiaRemision
{
    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdGuiaRemision { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string NumeroGuia { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaEmision { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaInicioTraslado { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaFinTraslado { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string PuntoPartida { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string PuntoLlegada { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string MotivoTraslado { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public int IdTransportista { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string PlacaVehiculo { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Ruta { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public bool? EstadoBoolean { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime FechaCreacion { get; set; }

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
    public int? IdSucursal { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? NumeroAutorizacionSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public DateTime? FechaAutorizacionSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? EstadoSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? ClaveAcceso { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string AmbienteSri { get; set; } = null!;

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? XmlSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? RespuestaSri { get; set; }

    /// <summary>
    /// TRIAL
    /// </summary>
    public string? Trial489 { get; set; }

    public virtual ICollection<GuiaRemisionDetalle> GuiaRemisionDetalles { get; set; } = new List<GuiaRemisionDetalle>();

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual Transportistum IdTransportistaNavigation { get; set; } = null!;
}
