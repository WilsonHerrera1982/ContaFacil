using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;

namespace ContaFacil.Controllers
{
    public class CategoriaProductoController : Controller
    {
        private readonly ContableContext _context;

        public CategoriaProductoController(ContableContext context)
        {
            _context = context;
        }

        // GET: CategoriaProducto
        public async Task<IActionResult> Index()
        {
              return _context.CategoriaProductos != null ? 
                          View(await _context.CategoriaProductos.ToListAsync()) :
                          Problem("Entity set 'ContableContext.CategoriaProductos'  is null.");
        }

        // GET: CategoriaProducto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos
                .FirstOrDefaultAsync(m => m.IdCategoriaProducto == id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }

            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaProducto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategoriaProducto,Nombre,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] CategoriaProducto categoriaProducto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaProducto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos.FindAsync(id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }
            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoriaProducto,Nombre,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] CategoriaProducto categoriaProducto)
        {
            if (id != categoriaProducto.IdCategoriaProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaProducto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaProductoExists(categoriaProducto.IdCategoriaProducto))
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
            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos
                .FirstOrDefaultAsync(m => m.IdCategoriaProducto == id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }

            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoriaProductos == null)
            {
                return Problem("Entity set 'ContableContext.CategoriaProductos'  is null.");
            }
            var categoriaProducto = await _context.CategoriaProductos.FindAsync(id);
            if (categoriaProducto != null)
            {
                _context.CategoriaProductos.Remove(categoriaProducto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaProductoExists(int id)
        {
          return (_context.CategoriaProductos?.Any(e => e.IdCategoriaProducto == id)).GetValueOrDefault();
        }
    }
}
