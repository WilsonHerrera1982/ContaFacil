namespace ContaFacil.Models.ViewModel
{
    public class InventarioReporteViewModel
    {
        public List<Inventario> Inventarios { get; set; }
        public List<Sucursal> Sucursales { get; set; }
        public List<CategoriaProducto> CategoriaProductos { get; set; }
        public List<Producto> Productos { get; set; }
        public int? IdSucursalSeleccionada { get; set; }
        public string TipoMovimientoSeleccionado { get; set; }
        public int? IdProducto {  get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
