using System.Collections.Generic;
using System.Linq;
using ContaFacil.Models; // Ajusta el namespace según tu proyecto
using ContaFacil.Models; // Asegúrate de incluir el DbContext si está en un namespace diferente
using Microsoft.EntityFrameworkCore;

namespace ContaFacil.Services.Impl
{
    public class MenuService : IMenuService
    {
        private readonly ContableContext _context;

        public MenuService(ContableContext context)
        {
            _context = context;
        }

        public List<Menu> GetMenusByPerfilId(int perfilId)
        {
            return (from m in _context.Menus
                    where m.MenuPerfils.Any(mp => mp.IdPerfil == perfilId && mp.Estado == true && m.Controller.Equals("OPCION"))
                    orderby m.Orden
                    select m).ToList();
        }
    }
}
