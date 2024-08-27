namespace ContaFacil.Models.ViewModel
{
    public class SubCuentasPartialViewModel
    {
        public IEnumerable<Cuentum> Cuentas { get; set; }
        public int? ParentId { get; set; }
    }
}
