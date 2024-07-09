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
    public class TipocuentumController : Controller
    {
        private readonly ContableContext _context;

        public TipocuentumController(ContableContext context)
        {
            _context = context;
        }

        // GET: Tipocuentum
        public async Task<IActionResult> Index()
        {
              return _context.Tipocuenta != null ? 
                          View(await _context.Tipocuenta.ToListAsync()) :
                          Problem("Entity set 'ContableContext.Tipocuenta'  is null.");
        }

        // GET: Tipocuentum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tipocuenta == null)
            {
                return NotFound();
            }

            var tipocuentum = await _context.Tipocuenta
                .FirstOrDefaultAsync(m => m.IdTipoCuenta == id);
            if (tipocuentum == null)
            {
                return NotFound();
            }

            return View(tipocuentum);
        }

        // GET: Tipocuentum/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tipocuentum/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoCuenta,Nombre")] Tipocuentum tipocuentum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipocuentum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipocuentum);
        }

        // GET: Tipocuentum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tipocuenta == null)
            {
                return NotFound();
            }

            var tipocuentum = await _context.Tipocuenta.FindAsync(id);
            if (tipocuentum == null)
            {
                return NotFound();
            }
            return View(tipocuentum);
        }

        // POST: Tipocuentum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoCuenta,Nombre")] Tipocuentum tipocuentum)
        {
            if (id != tipocuentum.IdTipoCuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipocuentum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipocuentumExists(tipocuentum.IdTipoCuenta))
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
            return View(tipocuentum);
        }

        // GET: Tipocuentum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tipocuenta == null)
            {
                return NotFound();
            }

            var tipocuentum = await _context.Tipocuenta
                .FirstOrDefaultAsync(m => m.IdTipoCuenta == id);
            if (tipocuentum == null)
            {
                return NotFound();
            }

            return View(tipocuentum);
        }

        // POST: Tipocuentum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tipocuenta == null)
            {
                return Problem("Entity set 'ContableContext.Tipocuenta'  is null.");
            }
            var tipocuentum = await _context.Tipocuenta.FindAsync(id);
            if (tipocuentum != null)
            {
                _context.Tipocuenta.Remove(tipocuentum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipocuentumExists(int id)
        {
          return (_context.Tipocuenta?.Any(e => e.IdTipoCuenta == id)).GetValueOrDefault();
        }
    }
}
