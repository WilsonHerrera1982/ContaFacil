using ContaFacil.Models.Dto;

namespace ContaFacil.Models.Interfaces
{
    public interface IReporteMayorizacionService
    {
        Task<byte[]> GenerarReporteMayorizacion(MayorizacionParametros parametros);
        Task<ContextoEmpresarial> ObtenerContextoEmpresarial(string idUsuario);
    }
}
