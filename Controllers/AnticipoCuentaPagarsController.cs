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
    public class AnticipoCuentaPagarsController : Controller
    {
        private readonly ContableContext _context;

        public AnticipoCuentaPagarsController(ContableContext context)
        {
            _context = context;
        }

        // GET: AnticipoCuentaPagars
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.AnticipoCuentaPagars.Include(a => a.IdAnticipoNavigation).Include(a => a.IdCuentaPorPagarNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: AnticipoCuentaPagars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipoCuentaPagar = await _context.AnticipoCuentaPagars
                .Include(a => a.IdAnticipoNavigation)
                .Include(a => a.IdCuentaPorPagarNavigation)
                .FirstOrDefaultAsync(m => m.IdRelacion == id);
            if (anticipoCuentaPagar == null)
            {
                return NotFound();
            }

            return View(anticipoCuentaPagar);
        }

        // GET: AnticipoCuentaPagars/Create
        public IActionResult Create()
        {
            ViewData["IdAnticipo"] = new SelectList(_context.AnticiposProveedors, "IdAnticipo", "IdAnticipo");
            ViewData["IdCuentaPorPagar"] = new SelectList(_context.CuentasPorPagars, "IdCuenta", "IdCuenta");
            return View();
        }

        // POST: AnticipoCuentaPagars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRelacion,IdAnticipo,IdCuentaPorPagar,UsuarioCreacion,FechaCreacion")] AnticipoCuentaPagar anticipoCuentaPagar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(anticipoCuentaPagar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAnticipo"] = new SelectList(_context.AnticiposProveedors, "IdAnticipo", "IdAnticipo", anticipoCuentaPagar.IdAnticipo);
            ViewData["IdCuentaPorPagar"] = new SelectList(_context.CuentasPorPagars, "IdCuenta", "IdCuenta", anticipoCuentaPagar.IdCuentaPorPagar);
            return View(anticipoCuentaPagar);
        }

        // GET: AnticipoCuentaPagars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipoCuentaPagar = await _context.AnticipoCuentaPagars.FindAsync(id);
            if (anticipoCuentaPagar == null)
            {
                return NotFound();
            }
            ViewData["IdAnticipo"] = new SelectList(_context.AnticiposProveedors, "IdAnticipo", "IdAnticipo", anticipoCuentaPagar.IdAnticipo);
            ViewData["IdCuentaPorPagar"] = new SelectList(_context.CuentasPorPagars, "IdCuenta", "IdCuenta", anticipoCuentaPagar.IdCuentaPorPagar);
            return View(anticipoCuentaPagar);
        }

        // POST: AnticipoCuentaPagars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRelacion,IdAnticipo,IdCuentaPorPagar,UsuarioCreacion,FechaCreacion")] AnticipoCuentaPagar anticipoCuentaPagar)
        {
            if (id != anticipoCuentaPagar.IdRelacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anticipoCuentaPagar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnticipoCuentaPagarExists(anticipoCuentaPagar.IdRelacion))
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
            ViewData["IdAnticipo"] = new SelectList(_context.AnticiposProveedors, "IdAnticipo", "IdAnticipo", anticipoCuentaPagar.IdAnticipo);
            ViewData["IdCuentaPorPagar"] = new SelectList(_context.CuentasPorPagars, "IdCuenta", "IdCuenta", anticipoCuentaPagar.IdCuentaPorPagar);
            return View(anticipoCuentaPagar);
        }

        // GET: AnticipoCuentaPagars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipoCuentaPagar = await _context.AnticipoCuentaPagars
                .Include(a => a.IdAnticipoNavigation)
                .Include(a => a.IdCuentaPorPagarNavigation)
                .FirstOrDefaultAsync(m => m.IdRelacion == id);
            if (anticipoCuentaPagar == null)
            {
                return NotFound();
            }

            return View(anticipoCuentaPagar);
        }

        // POST: AnticipoCuentaPagars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anticipoCuentaPagar = await _context.AnticipoCuentaPagars.FindAsync(id);
            if (anticipoCuentaPagar != null)
            {
                _context.AnticipoCuentaPagars.Remove(anticipoCuentaPagar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnticipoCuentaPagarExists(int id)
        {
            return _context.AnticipoCuentaPagars.Any(e => e.IdRelacion == id);
        }
    }
}
