﻿namespace ContaFacil.Models.ViewModel
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
    }
}