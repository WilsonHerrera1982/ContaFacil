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
    public class TipoPagoController : Controller
    {
        private readonly ContableContext _context;

        public TipoPagoController(ContableContext context)
        {
            _context = context;
        }

        // GET: TipoPago
        public async Task<IActionResult> Index()
        {
              return _context.TipoPagos != null ? 
                          View(await _context.TipoPagos.ToListAsync()) :
                          Problem("Entity set 'ContableContext.TipoPagos'  is null.");
        }

        // GET: TipoPago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TipoPagos == null)
            {
                return NotFound();
            }

            var tipoPago = await _context.TipoPagos
                .FirstOrDefaultAsync(m => m.IdTipoPago == id);
            if (tipoPago == null)
            {
                return NotFound();
            }

            return View(tipoPago);
        }

        // GET: TipoPago/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoPago/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoPago,Nombre")] TipoPago tipoPago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoPago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoPago);
        }

        // GET: TipoPago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TipoPagos == null)
            {
                return NotFound();
            }

            var tipoPago = await _context.TipoPagos.FindAsync(id);
            if (tipoPago == null)
            {
                return NotFound();
            }
            return View(tipoPago);
        }

        // POST: TipoPago/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoPago,Nombre")] TipoPago tipoPago)
        {
            if (id != tipoPago.IdTipoPago)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoPago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoPagoExists(tipoPago.IdTipoPago))
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
            return View(tipoPago);
        }

        // GET: TipoPago/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TipoPagos == null)
            {
                return NotFound();
            }

            var tipoPago = await _context.TipoPagos
                .FirstOrDefaultAsync(m => m.IdTipoPago == id);
            if (tipoPago == null)
            {
                return NotFound();
            }

            return View(tipoPago);
        }

        // POST: TipoPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TipoPagos == null)
            {
                return Problem("Entity set 'ContableContext.TipoPagos'  is null.");
            }
            var tipoPago = await _context.TipoPagos.FindAsync(id);
            if (tipoPago != null)
            {
                _context.TipoPagos.Remove(tipoPago);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoPagoExists(int id)
        {
          return (_context.TipoPagos?.Any(e => e.IdTipoPago == id)).GetValueOrDefault();
        }
    }
}
