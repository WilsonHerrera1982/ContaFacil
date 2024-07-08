using ContaFacil.Logica;
using ContaFacil.Models;
using ContaFacil.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace ContaFacil.Controllers
{
    public class LoginController : NotificacionClass
    {
        private readonly ContableContext _context;

        public const string idUsuario = "_idUsuario";

        public IActionResult Index()
        {
            return View();
        }

        public LoginController(ContableContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            LoginViewModel viewModel2 = viewModel;
            Usuario usuario = _context.Usuarios.FirstOrDefault((Usuario u) => u.Nombre == viewModel2.Username && u.Clave == viewModel2.Password);
            if (usuario != null)
            {
                int perfilId = 5;
                List<Menu> menus = ObtenerMenusPorPerfil(perfilId);
               // base.HttpContext.Session.SetString("menu", JsonSerializer.Serialize(menus));
                Notificacion("Bienvenid@, " + usuario.Nombre, NotificacionTipo.Success);
                string id = usuario.IdUsuario.ToString();
                //base.HttpContext.Session.SetString("_idUsuario", id);
                return RedirectToAction("Index", "Home");
            }
            base.ViewBag.ErrorMessage = "Usuario o contraseña incorrectos.";
            Notificacion("Usuario o contraseña incorrectos.", NotificacionTipo.Warning);
            return View();
        }

        public List<Menu> ObtenerMenusPorPerfil(int perfilId)
        {
            List<Menu> menusPrincipales = (from m in _context.Menus
                                           where m.MenuPerfils.Any((MenuPerfil mp) => mp.IdPerfil == perfilId)
                                           orderby m.IdMenu
                                           select m).ToList();
            foreach (Menu menuPrincipal in menusPrincipales)
            {
                menuPrincipal.subMenus = (from m in _context.Menus
                                          where m.IdMenu == (int?)menuPrincipal.IdMenu
                                          orderby m.IdMenu
                                          select m).ToList();
            }
            return menusPrincipales;
        }
    }
}
