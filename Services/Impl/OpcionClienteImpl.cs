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
            /* List<OpcionCliente> opcionClientes = new List<OpcionCliente>();
              opcionClientes=_context.OpcionClientes.Where(o=> o.Estado==1).ToList();*/
            var opcionClientes = (from oc in _context.OpcionesClientes
                                  join o in _context.OpcionClientes
                                  on oc.OpcionId equals o.OpcionId
                                  where oc.EmpresaId == empresa
                                  && o.Estado == 1
                                  select o).ToList();
            return opcionClientes;
        }
    }
}
