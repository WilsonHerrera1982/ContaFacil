using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContaFacil.Models.ViewModel
{
    public class InventarioViewModel
    {
        // Propiedades de la cabecera
        public string tipoMovimiento { get; set; }
        public string numeroDespacho { get; set; }
        public string numeroFactura { get; set; }
        public int idProveedor { get; set; }
        public int sucursalDestino { get; set; }
        public string descripcion { get; set; }

        // Lista de detalles de productos
        public List<DetalleInventarioViewModel> Detalles { get; set; } = new List<DetalleInventarioViewModel>();
    }

    public class DetalleInventarioViewModel
    {
        [Required(ErrorMessage = "El producto es requerido")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal PorcentajeDescuento { get; set; }

        public decimal DescuentoUnitario { get; set; }
        public decimal PrecioUnitarioFinal { get; set; }
        public decimal SubtotalSinImpuestos { get; set; }
        public decimal TotalDescuento { get; set; }

        [Required(ErrorMessage = "El porcentaje de IVA es requerido")]
        [Range(0, 100, ErrorMessage = "El IVA debe estar entre 0 y 100")]
        public decimal PorcentajeIVA { get; set; }

        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public int Utilidad { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}