using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContaFacil.Models;

/// <summary>
/// Tabla intermedia para la relación muchos a muchos entre Cuenta y Empresa.
/// Permite que una cuenta pueda ser usada por múltiples empresas con saldos independientes.
/// </summary>
public partial class CuentaEmpresa
{
    /// <summary>
    /// ID único de la relación cuenta-empresa
    /// </summary>
    [Key]
    public int IdCuentaEmpresa { get; set; }

    /// <summary>
    /// ID de la cuenta
    /// </summary>
    [Required]
    public int IdCuenta { get; set; }

    /// <summary>
    /// ID de la empresa
    /// </summary>
    [Required]
    public int IdEmpresa { get; set; }

    /// <summary>
    /// Saldo inicial de la cuenta para esta empresa
    /// </summary>
    [DisplayName("Saldo Inicial")]
    [DisplayFormat(DataFormatString = "{0:N2}")]
    public decimal SaldoInicial { get; set; }

    /// <summary>
    /// Saldo actual de la cuenta para esta empresa
    /// </summary>
    [DisplayName("Saldo Actual")]
    [DisplayFormat(DataFormatString = "{0:N2}")]
    public decimal SaldoActual { get; set; }

    /// <summary>
    /// Estado de la cuenta para esta empresa: 1=Activo, 0=Inactivo
    /// </summary>
    [DisplayName("Estado")]
    public bool Estado { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [DisplayName("Fecha Creación")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha de última modificación del registro
    /// </summary>
    [DisplayName("Fecha Modificación")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// ID del usuario que creó el registro
    /// </summary>
    [Required]
    public int UsuarioCreacion { get; set; }

    /// <summary>
    /// ID del usuario que modificó el registro por última vez
    /// </summary>
    public int? UsuarioModificacion { get; set; }

    // ========================================
    // Propiedades de Navegación
    // ========================================

    /// <summary>
    /// Navegación a la cuenta relacionada
    /// </summary>
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    /// <summary>
    /// Navegación a la empresa relacionada
    /// </summary>
    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
