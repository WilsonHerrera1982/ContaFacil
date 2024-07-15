using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;

namespace ContaFacil.Controllers.Contador
{
    public class PaqueteContadorsController : Controller
    {
        private readonly ContableContext _context;

        public PaqueteContadorsController(ContableContext context)
        {
            _context = context;
        }

        // GET: PaqueteContadors
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.PaqueteContadors.Include(p => p.IdPaqueteNavigation).Include(p => p.IdUsuarioNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: PaqueteContadors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PaqueteContadors == null)
            {
                return NotFound();
            }

            var paqueteContador = await _context.PaqueteContadors
                .Include(p => p.IdPaqueteNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdPaqueteContador == id);
            if (paqueteContador == null)
            {
                return NotFound();
            }

            return View(paqueteContador);
        }

        // GET: PaqueteContadors/Create
        public IActionResult Create()
        {
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: PaqueteContadors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaqueteContador,IdPaquete,IdUsuario,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] PaqueteContador paqueteContador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paqueteContador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", paqueteContador.IdPaquete);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", paqueteContador.IdUsuario);
            return View(paqueteContador);
        }

        // GET: PaqueteContadors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PaqueteContadors == null)
            {
                return NotFound();
            }

            var paqueteContador = await _context.PaqueteContadors.FindAsync(id);
            if (paqueteContador == null)
            {
                return NotFound();
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", paqueteContador.IdPaquete);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", paqueteContador.IdUsuario);
            return View(paqueteContador);
        }

        // POST: PaqueteContadors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaqueteContador,IdPaquete,IdUsuario,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] PaqueteContador paqueteContador)
        {
            if (id != paqueteContador.IdPaqueteContador)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paqueteContador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaqueteContadorExists(paqueteContador.IdPaqueteContador))
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
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", paqueteContador.IdPaquete);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", paqueteContador.IdUsuario);
            return View(paqueteContador);
        }

        // GET: PaqueteContadors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PaqueteContadors == null)
            {
                return NotFound();
            }

            var paqueteContador = await _context.PaqueteContadors
                .Include(p => p.IdPaqueteNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdPaqueteContador == id);
            if (paqueteContador == null)
            {
                return NotFound();
            }

            return View(paqueteContador);
        }

        // POST: PaqueteContadors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PaqueteContadors == null)
            {
                return Problem("Entity set 'ContableContext.PaqueteContadors'  is null.");
            }
            var paqueteContador = await _context.PaqueteContadors.FindAsync(id);
            if (paqueteContador != null)
            {
                _context.PaqueteContadors.Remove(paqueteContador);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaqueteContadorExists(int id)
        {
          return (_context.PaqueteContadors?.Any(e => e.IdPaqueteContador == id)).GetValueOrDefault();
        }
    }
}
