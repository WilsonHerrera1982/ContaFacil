using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;

namespace ContaFacil.Controllers.Reportes
{
    public class ProductoBajoStockController : Controller
    {
        private readonly ContableContext _context;

        public ProductoBajoStockController(ContableContext context)
        {
            _context = context;
        }

        // GET: ProductoBajoStock
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Productos.Where(p => p.Stock <= 10).Include(p => p.IdCategoriaProductoNavigation).Include(p => p.IdEmpresaNavigation).Include(p => p.IdUnidadMedidaNavigation);
            return View(await contableContext.ToListAsync());
        }

    }
}
