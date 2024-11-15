namespace ContaFacil.Models.ViewModel
{
    public class ConstatacionFisicaViewModel
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal StockActual { get; set; }
        public int? CantidadFisica { get; set; }
        public string Descripcion { get; set; }
    }
}
