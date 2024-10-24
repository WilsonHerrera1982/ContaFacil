using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Transportistum
{
    public int IdTransportista { get; set; }

    public string TipoIdentificacion { get; set; } = null!;

    public string NumeroIdentificacion { get; set; } = null!;

    public string RazonSocial { get; set; } = null!;

    public string PlacaVehiculo { get; set; } = null!;

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public bool? Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? UsuarioRegistro { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual ICollection<GuiaRemision> GuiaRemisions { get; set; } = new List<GuiaRemision>();
}
