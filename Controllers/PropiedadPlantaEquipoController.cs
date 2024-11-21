using ContaFacil.Models;
using ContaFacil.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ContaFacil.Controllers
{
    public class PropiedadPlantaEquipoController : Controller
    {
        private readonly ContableContext _context;
        private readonly IOpcionCliente _opcionCliente;
        string idUsuario = "";
        string idEmpresa = "";

        public PropiedadPlantaEquipoController(ContableContext context, IOpcionCliente opcionCliente)
        {
            _context = context;
            _opcionCliente = opcionCliente;
        }
        public IActionResult Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            // Aquí puedes agregar cualquier lógica adicional que necesites antes de devolver la vista
            // Por ejemplo, podrías cargar algunos datos desde la base de datos y pasarlos a la vista
            List<OpcionCliente> opcionClientes = _opcionCliente.GetOpcionClientes(usuario.IdEmpresa ?? 0);
            ViewBag.OpcionClientes = opcionClientes;
            return View();
        }
    }
}
