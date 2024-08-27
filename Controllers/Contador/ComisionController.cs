using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;

namespace ContaFacil.Controllers.Contador
{
    public class ComisionController : Controller
    {
        private readonly ContableContext _context;

        public ComisionController(ContableContext context)
        {
            _context = context;
        }

        // GET: Comision
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Comisions.Include(c => c.IdPaqueteNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Comision/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comisions == null)
            {
                return NotFound();
            }

            var comision = await _context.Comisions
                .Include(c => c.IdPaqueteNavigation)
                .FirstOrDefaultAsync(m => m.IdComision == id);
            if (comision == null)
            {
                return NotFound();
            }

            return View(comision);
        }

        // GET: Comision/Create
        public IActionResult Create()
        {
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete");
            return View();
        }

        // POST: Comision/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdComision,IdPaquete,Valor,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Comision comision)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", comision.IdPaquete);
            return View(comision);
        }

        // GET: Comision/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comisions == null)
            {
                return NotFound();
            }

            var comision = await _context.Comisions.FindAsync(id);
            if (comision == null)
            {
                return NotFound();
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", comision.IdPaquete);
            return View(comision);
        }

        // POST: Comision/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdComision,IdPaquete,Valor,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Comision comision)
        {
            if (id != comision.IdComision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comision);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComisionExists(comision.IdComision))
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
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", comision.IdPaquete);
            return View(comision);
        }

        // GET: Comision/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comisions == null)
            {
                return NotFound();
            }

            var comision = await _context.Comisions
                .Include(c => c.IdPaqueteNavigation)
                .FirstOrDefaultAsync(m => m.IdComision == id);
            if (comision == null)
            {
                return NotFound();
            }

            return View(comision);
        }

        // POST: Comision/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comisions == null)
            {
                return Problem("Entity set 'ContableContext.Comisions'  is null.");
            }
            var comision = await _context.Comisions.FindAsync(id);
            if (comision != null)
            {
                _context.Comisions.Remove(comision);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComisionExists(int id)
        {
          return (_context.Comisions?.Any(e => e.IdComision == id)).GetValueOrDefault();
        }
    }
}
