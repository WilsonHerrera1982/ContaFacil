using ContaFacil.Models;

namespace ContaFacil.Services
{
    public interface IOpcionCliente
    {
         List<OpcionCliente> GetOpcionClientes(int empresa);
    }
}
