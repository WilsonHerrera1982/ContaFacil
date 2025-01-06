using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// tabla para registrar el acceso a opciones de cliente
/// </summary>
public partial class OpcionCliente
{
    public int OpcionId { get; set; }

    public string OpcionNombre { get; set; } = null!;

    public string OpcionDescripcion { get; set; } = null!;

    public DateTime? FechaResgitro { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public int? Estado { get; set; }

    public virtual ICollection<OpcionesCliente> OpcionesClientes { get; set; } = new List<OpcionesCliente>();
}
