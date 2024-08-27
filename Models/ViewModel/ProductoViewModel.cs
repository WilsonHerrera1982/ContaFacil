namespace ContaFacil.Models.ViewModel
{
    public class ProductoViewModel
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int? IdCategoriaProducto { get; set; }
        public int? IdUnidadMedida { get; set; }
        public decimal? Stock { get; set; } = 0;
        public int? IdEmpresa { get; set; }
        public int IdProveedor { get; set; }
        public int? IdImpuesto { get; set; }
        public string? NumeroDespacho { get; set; } = "E-000001";
        public decimal? Descuento { get; set; }
    }
}
