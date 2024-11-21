using ContaFacil.Models.Dto;
namespace ContaFacil.Models.Interfaces
{
    public interface ILibroDiarioService
    {
        Task<byte[]> GenerarLibroDiario(LibroDiarioParametros parametros);
    }
}
