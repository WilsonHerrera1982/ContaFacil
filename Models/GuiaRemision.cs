using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class GuiaRemision
{
    public int IdGuiaRemision { get; set; }

    public string NumeroGuia { get; set; } = null!;

    public DateTime FechaEmision { get; set; }

    public DateTime FechaInicioTraslado { get; set; }

    public DateTime FechaFinTraslado { get; set; }

    public string PuntoPartida { get; set; } = null!;

    public string PuntoLlegada { get; set; } = null!;

    public string MotivoTraslado { get; set; } = null!;

    public int IdTransportista { get; set; }

    public string PlacaVehiculo { get; set; } = null!;

    public string? Ruta { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? IdSucursal { get; set; }

    public string? NumeroAutorizacionSri { get; set; }

    public DateTime? FechaAutorizacionSri { get; set; }

    public string? EstadoSri { get; set; }

    public string? ClaveAcceso { get; set; }

    public char AmbienteSri { get; set; }

    public string? XmlSri { get; set; }

    public string? RespuestaSri { get; set; }

    public virtual ICollection<GuiaRemisionDetalle> GuiaRemisionDetalles { get; set; } = new List<GuiaRemisionDetalle>();

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual Transportistum IdTransportistaNavigation { get; set; } = null!;
}
