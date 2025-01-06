using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

/// <summary>
/// relacionar clientes con opciones
/// </summary>
public partial class OpcionesCliente
{
    public int OpcId { get; set; }

    public int OpcionId { get; set; }

    public int EmpresaId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public virtual Empresa Empresa { get; set; } = null!;

    public virtual OpcionCliente Opcion { get; set; } = null!;
}
