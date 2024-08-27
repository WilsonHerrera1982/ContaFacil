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
    public class ComisionContadorController : Controller
    {
        private readonly ContableContext _context;

        public ComisionContadorController(ContableContext context)
        {
            _context = context;
        }

        // GET: ComisionContador
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.ComisionContadors.Include(c => c.IdComisionNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: ComisionContador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ComisionContadors == null)
            {
                return NotFound();
            }

            var comisionContador = await _context.ComisionContadors
                .Include(c => c.IdComisionNavigation)
                .FirstOrDefaultAsync(m => m.IdComsionContador == id);
            if (comisionContador == null)
            {
                return NotFound();
            }

            return View(comisionContador);
        }

        // GET: ComisionContador/Create
        public IActionResult Create()
        {
            ViewData["IdComision"] = new SelectList(_context.Comisions, "IdComision", "IdComision");
            return View();
        }

        // POST: ComisionContador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdComsionContador,IdComision,Valor,Estado,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] ComisionContador comisionContador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comisionContador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdComision"] = new SelectList(_context.Comisions, "IdComision", "IdComision", comisionContador.IdComision);
            return View(comisionContador);
        }

        // GET: ComisionContador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ComisionContadors == null)
            {
                return NotFound();
            }

            var comisionContador = await _context.ComisionContadors.FindAsync(id);
            if (comisionContador == null)
            {
                return NotFound();
            }
            ViewData["IdComision"] = new SelectList(_context.Comisions, "IdComision", "IdComision", comisionContador.IdComision);
            return View(comisionContador);
        }

        // POST: ComisionContador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdComsionContador,IdComision,Valor,Estado,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] ComisionContador comisionContador)
        {
            if (id != comisionContador.IdComsionContador)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comisionContador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComisionContadorExists(comisionContador.IdComsionContador))
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
            ViewData["IdComision"] = new SelectList(_context.Comisions, "IdComision", "IdComision", comisionContador.IdComision);
            return View(comisionContador);
        }

        // GET: ComisionContador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ComisionContadors == null)
            {
                return NotFound();
            }

            var comisionContador = await _context.ComisionContadors
                .Include(c => c.IdComisionNavigation)
                .FirstOrDefaultAsync(m => m.IdComsionContador == id);
            if (comisionContador == null)
            {
                return NotFound();
            }

            return View(comisionContador);
        }

        // POST: ComisionContador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ComisionContadors == null)
            {
                return Problem("Entity set 'ContableContext.ComisionContadors'  is null.");
            }
            var comisionContador = await _context.ComisionContadors.FindAsync(id);
            if (comisionContador != null)
            {
                _context.ComisionContadors.Remove(comisionContador);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComisionContadorExists(int id)
        {
          return (_context.ComisionContadors?.Any(e => e.IdComsionContador == id)).GetValueOrDefault();
        }
    }
}
