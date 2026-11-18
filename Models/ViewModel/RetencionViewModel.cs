namespace ContaFacil.Models.ViewModel
{
    public class RetencionViewModel
    {
        public string NumeroFactura { get; set; }
    public string ComprobanteRetencion { get; set; }
    public string NumeroAutorizacion { get; set; }
    public string EjercicioFiscal { get; set; }
    public decimal BaseImponibleIVA { get; set; }
    public decimal PorcentajeRetencionIVA { get; set; }
    public decimal ValorRetenidoIVA { get; set; }
    public decimal BaseImponibleRenta { get; set; }
    public decimal PorcentajeRetencionRenta { get; set; }
    public decimal ValorRetenidoRenta { get; set; }
    }
}
