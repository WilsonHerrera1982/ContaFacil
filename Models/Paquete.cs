using System;
using System.Collections.Generic;

namespace ContaFacil.Models;

public partial class Paquete
{
    public int IdPaquete { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int CantidadEmisores { get; set; }

    public decimal Valor { get; set; }

    public bool EstadoBoolean { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int UsuarioCreacion { get; set; }

    public int? UsuarioModificacion { get; set; }

    public virtual ICollection<Comision> Comisions { get; set; } = new List<Comision>();

    public virtual ICollection<PaqueteContador> PaqueteContadors { get; set; } = new List<PaqueteContador>();

    public virtual ICollection<VentaPaquete> VentaPaquetes { get; set; } = new List<VentaPaquete>();
}
