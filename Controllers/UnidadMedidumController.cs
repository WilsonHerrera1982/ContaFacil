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
    public class UnidadMedidumController : Controller
    {
        private readonly ContableContext _context;

        public UnidadMedidumController(ContableContext context)
        {
            _context = context;
        }

        // GET: UnidadMedidum
        public async Task<IActionResult> Index()
        {
              return _context.UnidadMedida != null ? 
                          View(await _context.UnidadMedida.ToListAsync()) :
                          Problem("Entity set 'ContableContext.UnidadMedida'  is null.");
        }

        // GET: UnidadMedidum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UnidadMedida == null)
            {
                return NotFound();
            }

            var unidadMedidum = await _context.UnidadMedida
                .FirstOrDefaultAsync(m => m.IdUnidadMedida == id);
            if (unidadMedidum == null)
            {
                return NotFound();
            }

            return View(unidadMedidum);
        }

        // GET: UnidadMedidum/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UnidadMedidum/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUnidadMedida,Nombre,Abreviatura,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] UnidadMedidum unidadMedidum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unidadMedidum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unidadMedidum);
        }

        // GET: UnidadMedidum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UnidadMedida == null)
            {
                return NotFound();
            }

            var unidadMedidum = await _context.UnidadMedida.FindAsync(id);
            if (unidadMedidum == null)
            {
                return NotFound();
            }
            return View(unidadMedidum);
        }

        // POST: UnidadMedidum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUnidadMedida,Nombre,Abreviatura,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] UnidadMedidum unidadMedidum)
        {
            if (id != unidadMedidum.IdUnidadMedida)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unidadMedidum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnidadMedidumExists(unidadMedidum.IdUnidadMedida))
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
            return View(unidadMedidum);
        }

        // GET: UnidadMedidum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UnidadMedida == null)
            {
                return NotFound();
            }

            var unidadMedidum = await _context.UnidadMedida
                .FirstOrDefaultAsync(m => m.IdUnidadMedida == id);
            if (unidadMedidum == null)
            {
                return NotFound();
            }

            return View(unidadMedidum);
        }

        // POST: UnidadMedidum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UnidadMedida == null)
            {
                return Problem("Entity set 'ContableContext.UnidadMedida'  is null.");
            }
            var unidadMedidum = await _context.UnidadMedida.FindAsync(id);
            if (unidadMedidum != null)
            {
                _context.UnidadMedida.Remove(unidadMedidum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnidadMedidumExists(int id)
        {
          return (_context.UnidadMedida?.Any(e => e.IdUnidadMedida == id)).GetValueOrDefault();
        }
    }
}
