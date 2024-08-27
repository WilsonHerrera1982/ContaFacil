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
    public class VentaPaqueteController : Controller
    {
        private readonly ContableContext _context;

        public VentaPaqueteController(ContableContext context)
        {
            _context = context;
        }

        // GET: VentaPaquete
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.VentaPaquetes.Include(v => v.IdPaqueteNavigation).Include(v => v.IdUsuarioNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: VentaPaquete/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VentaPaquetes == null)
            {
                return NotFound();
            }

            var ventaPaquete = await _context.VentaPaquetes
                .Include(v => v.IdPaqueteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdVentaPaquete == id);
            if (ventaPaquete == null)
            {
                return NotFound();
            }

            return View(ventaPaquete);
        }

        // GET: VentaPaquete/Create
        public IActionResult Create()
        {
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: VentaPaquete/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVentaPaquete,IdPaquete,IdUsuario,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] VentaPaquete ventaPaquete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ventaPaquete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", ventaPaquete.IdPaquete);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", ventaPaquete.IdUsuario);
            return View(ventaPaquete);
        }

        // GET: VentaPaquete/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VentaPaquetes == null)
            {
                return NotFound();
            }

            var ventaPaquete = await _context.VentaPaquetes.FindAsync(id);
            if (ventaPaquete == null)
            {
                return NotFound();
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", ventaPaquete.IdPaquete);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", ventaPaquete.IdUsuario);
            return View(ventaPaquete);
        }

        // POST: VentaPaquete/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVentaPaquete,IdPaquete,IdUsuario,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] VentaPaquete ventaPaquete)
        {
            if (id != ventaPaquete.IdVentaPaquete)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ventaPaquete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaPaqueteExists(ventaPaquete.IdVentaPaquete))
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
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "IdPaquete", ventaPaquete.IdPaquete);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", ventaPaquete.IdUsuario);
            return View(ventaPaquete);
        }

        // GET: VentaPaquete/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VentaPaquetes == null)
            {
                return NotFound();
            }

            var ventaPaquete = await _context.VentaPaquetes
                .Include(v => v.IdPaqueteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdVentaPaquete == id);
            if (ventaPaquete == null)
            {
                return NotFound();
            }

            return View(ventaPaquete);
        }

        // POST: VentaPaquete/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VentaPaquetes == null)
            {
                return Problem("Entity set 'ContableContext.VentaPaquetes'  is null.");
            }
            var ventaPaquete = await _context.VentaPaquetes.FindAsync(id);
            if (ventaPaquete != null)
            {
                _context.VentaPaquetes.Remove(ventaPaquete);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaPaqueteExists(int id)
        {
          return (_context.VentaPaquetes?.Any(e => e.IdVentaPaquete == id)).GetValueOrDefault();
        }
    }
}
