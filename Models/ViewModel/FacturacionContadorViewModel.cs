using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContaFacil.Models.ViewModel
{
    public class FacturacionContadorViewModel
    {
        public IEnumerable<ContaFacil.Models.Factura> Facturas { get; set; }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Emisores { get; set; }
        public int? SelectedEmisorId { get; set; }
    }
}
