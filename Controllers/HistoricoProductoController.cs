using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;

namespace ContaFacil.Controllers
{
    public class HistoricoProductoController : Controller
    {
        private readonly ContableContext _context;

        public HistoricoProductoController(ContableContext context)
        {
            _context = context;
        }

        // GET: HistoricoProducto
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.HistoricoProductos.Include(h => h.IdEmpresaNavigation).Include(h => h.IdProductoNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: HistoricoProducto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoProducto = await _context.HistoricoProductos
                .Include(h => h.IdEmpresaNavigation)
                .Include(h => h.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdHistoricoProducto == id);
            if (historicoProducto == null)
            {
                return NotFound();
            }

            return View(historicoProducto);
        }

        // GET: HistoricoProducto/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto");
            return View();
        }

        // POST: HistoricoProducto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHistoricoProducto,IdProducto,IdEmpresa,NumeroDespacho,PrecioUnitarioFinal,Impuesto,PrecioVenta,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] HistoricoProducto historicoProducto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historicoProducto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", historicoProducto.IdEmpresa);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", historicoProducto.IdProducto);
            return View(historicoProducto);
        }

        // GET: HistoricoProducto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoProducto = await _context.HistoricoProductos.FindAsync(id);
            if (historicoProducto == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", historicoProducto.IdEmpresa);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", historicoProducto.IdProducto);
            return View(historicoProducto);
        }

        // POST: HistoricoProducto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHistoricoProducto,IdProducto,IdEmpresa,NumeroDespacho,PrecioUnitarioFinal,Impuesto,PrecioVenta,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] HistoricoProducto historicoProducto)
        {
            if (id != historicoProducto.IdHistoricoProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historicoProducto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoricoProductoExists(historicoProducto.IdHistoricoProducto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", historicoProducto.IdEmpresa);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", historicoProducto.IdProducto);
            return View(historicoProducto);
        }

        // GET: HistoricoProducto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoProducto = await _context.HistoricoProductos
                .Include(h => h.IdEmpresaNavigation)
                .Include(h => h.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdHistoricoProducto == id);
            if (historicoProducto == null)
            {
                return NotFound();
            }

            return View(historicoProducto);
        }

        // POST: HistoricoProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historicoProducto = await _context.HistoricoProductos.FindAsync(id);
            if (historicoProducto != null)
            {
                _context.HistoricoProductos.Remove(historicoProducto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoricoProductoExists(int id)
        {
            return _context.HistoricoProductos.Any(e => e.IdHistoricoProducto == id);
        }
    }
}
