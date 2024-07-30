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

                Usuario usuario = _context.Usuarios
                    .Include(u => u.IdEmpresaNavigation)
                    .FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account"); // Asume que tienes un controlador de cuenta
                }

                ViewBag.Usuario = usuario.Nombre;

                Persona persona = _context.Personas
                    .FirstOrDefault(p => p.IdPersona == usuario.IdPersona);

                Emisor emisor = _context.Emisors
                    .FirstOrDefault(e => e.Ruc == persona.Identificacion);

                ViewBag.Empresa = emisor?.RazonSocial;

                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error en el Index");
                Notificacion("Error en el Login", NotificacionTipo.Error);
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
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
            if (statusCode.HasValue)
            {
                switch (statusCode.Value)
                {
                    case 404:
                        errorViewModel.Message = "La página que buscas no existe.";
                        break;
                    case 500:
                        errorViewModel.Message = "Ha ocurrido un error interno en el servidor.";
                        break;
                    default:
                        errorViewModel.Message = "Ha ocurrido un error inesperado.";
                        break;
                }
            }

            Response.StatusCode = statusCode ?? 500;
            return View(errorViewModel);
        }
    }
}