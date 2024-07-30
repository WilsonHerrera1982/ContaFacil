using System.Drawing;
using System.Security.Permissions;

namespace ContaFacil.Models.ViewModel
{
    public class InventarioViewModel { 
    
        public int cantidad {  get; set; }
        public int idProducto {  get; set; }
        public string tipoMovimiento {  get; set; }
        public string numeroDespacho {  get; set; }
        public string descripcion {  get; set; }
        public int sucursalDestino {  get; set; }
        public string numeroFactura {  get; set; }
        public decimal subtotal {  get; set; }
        public decimal iva {  get; set; }
        public decimal descuento {  get; set; }
        public decimal total { get; set; }

    }
}
