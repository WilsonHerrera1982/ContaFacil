namespace ContaFacil.Models.ViewModel
{
    public class ReportesViewModel
    {
        public List<VentasPorDia> VentasPorDia { get; set; }
        public List<VentasPorSucursal> VentasPorSucursal { get; set; }
        public List<VentasPorProducto> VentasPorProducto { get; set; }
        public List<VentasPorTipoPago> VentasPorTipoPago { get; set; }
        public List<VentasPromocion> VentasPromocion { get; set; }
    }

    public class VentasPorDia
    {
        public DateOnly Fecha { get; set; }
        public decimal Total { get; set; }
    }

    public class VentasPorSucursal
    {
        public string Sucursal { get; set; }
        public decimal Total { get; set; }
    }

    public class VentasPorProducto
    {
        public string Producto { get; set; }
        public decimal Total { get; set; }
    }
    public class VentasPromocion
    {
        public string Producto { get; set; }
        public decimal Total { get; set; }
    }

    public class VentasPorTipoPago
    {
        public string TipoPago { get; set; }
        public decimal Total { get; set; }
    }
}
