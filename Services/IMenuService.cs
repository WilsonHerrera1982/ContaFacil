using System.Collections.Generic;
using ContaFacil.Models; // Asegúrate de ajustar el namespace según corresponda

namespace ContaFacil.Services
{
    public interface IMenuService
    {
        List<Menu> GetMenusByPerfilId(int perfilId);
    }
}
