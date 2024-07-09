using ContaFacil.Logica;
using ContaFacil.Models;
using ContaFacil.Models.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
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
            Usuario usuario = _context.Usuarios.Include(u =>u.UsuarioPerfils).FirstOrDefault((Usuario u) => u.Nombre == viewModel2.Username && u.Clave == viewModel2.Password);
          
            if (usuario != null)
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 256 // Aumenta la profundidad máxima si es necesario
                };

               UsuarioPerfil perfil= usuario.UsuarioPerfils.FirstOrDefault();

                List<Menu> menus = ObtenerMenusPorPerfil(perfil.IdPerfil);
                string jsonString = JsonSerializer.Serialize(menus, options);
                HttpContext.Session.SetString("menu", jsonString);
                //base.HttpContext.Session.SetString("menu", JsonSerializer.Serialize(menus));
                Notificacion("Bienvenid@, " + usuario.Nombre, NotificacionTipo.Success);
                string id = usuario.IdUsuario.ToString();
                base.HttpContext.Session.SetString("_idUsuario", id);
                base.HttpContext.Session.SetString("_usuario", usuario.Nombre);
                base.HttpContext.Session.SetString("_empresa", usuario.IdEmpresa.ToString());
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
                                          where m.MenuId == (int?)menuPrincipal.IdMenu
                                          orderby m.IdMenu
                                          select m).ToList();
                
            }
           
            return menusPrincipales;
        }
        public IActionResult Logout()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();

            // Si estás usando autenticación de cookies, también debes borrar la cookie de autenticación
            // Esto es necesario si estás usando ASP.NET Core Identity o autenticación basada en cookies
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            // Redirigir al usuario a la página de login
            return RedirectToAction("Login", "Login");
        }
    }

}
