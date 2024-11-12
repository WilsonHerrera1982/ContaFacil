using ContaFacil.Models;

namespace ContaFacil.Services.Impl
{
    public class OpcionClienteImpl : IOpcionCliente
    {
        private readonly ContableContext _context;

        public OpcionClienteImpl(ContableContext context)
        {

            _context = context;
        }

        public List<OpcionCliente> GetOpcionClientes(int empresa)
        {
           List<OpcionCliente> opcionClientes = new List<OpcionCliente>();
            opcionClientes=_context.OpcionClientes.Where(o=>o.EmpresaId==empresa && o.Estado==1).ToList();
            return opcionClientes;
        }
    }
}
