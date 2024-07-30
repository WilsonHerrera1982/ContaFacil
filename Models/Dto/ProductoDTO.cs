namespace ContaFacil.Models.Dto
{
    public class ProductoDTO
    {
        public string CodigoProducto { get; set; }
        public string Categoria { get; set; }
        public string NombreProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public string CuentaContable { get; set; }
        public string UsuarioCreacion { get; set; }
        public string UnidadMedida { get; set; }
        public string FacturaNro { get; set; }
        public string IdProveedor { get; set; }
        public string Proveedor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public bool CargaInicial { get; set; }
    }
}
