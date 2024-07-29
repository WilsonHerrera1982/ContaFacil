using ContaFacil.Logica;
using ContaFacil.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ContaFacil.Controllers
{
    public class HomeController : NotificacionClass
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ContableContext _context;

        public HomeController(ContableContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
       

        public IActionResult Index()
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = _context.Usuarios.Include(u => u.IdEmpresaNavigation).FirstOrDefault((Usuario u) => u.IdUsuario == int.Parse(idUsuario));
                ViewBag.Usuario = usuario.Nombre;
                Persona persona = new Persona();
                persona=_context.Personas.Where(p=>p.IdPersona==usuario.IdPersona).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == persona.Identificacion).FirstOrDefault();
                ViewBag.Empresa = emisor.RazonSocial;
                return View();
            }
            catch (Exception e)
            {
                Notificacion("Error en el Login",NotificacionTipo.Error);
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode
            };

            // Aquí puedes añadir lógica adicional basada en el statusCode si lo deseas

            return View(errorViewModel);
        }
    }
}
