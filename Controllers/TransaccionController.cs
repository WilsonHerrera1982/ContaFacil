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
    public class TransaccionController : Controller
    {
        private readonly ContableContext _context;

        public TransaccionController(ContableContext context)
        {
            _context = context;
        }

        // GET: Transaccion
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Transaccions.Include(t => t.IdCuentaNavigation).Include(t => t.IdEmpresaNavigation).Include(t => t.IdTipoTransaccionNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Transaccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transaccions == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccions
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .Include(t => t.IdTipoTransaccionNavigation)
                .FirstOrDefaultAsync(m => m.IdTransaccion == id);
            if (transaccion == null)
            {
                return NotFound();
            }

            return View(transaccion);
        }

        // GET: Transaccion/Create
        public IActionResult Create()
        {
            ViewData["IdCuenta"] = new SelectList(_context.Cuenta, "IdCuenta", "IdCuenta");
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "IdTipoTransaccion");
            return View();
        }

        // POST: Transaccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTransaccion,IdCuenta,Fecha,IdTipoTransaccion,Monto,Descripcion,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Transaccion transaccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCuenta"] = new SelectList(_context.Cuenta, "IdCuenta", "IdCuenta", transaccion.IdCuenta);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", transaccion.IdEmpresa);
            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "IdTipoTransaccion", transaccion.IdTipoTransaccion);
            return View(transaccion);
        }

        // GET: Transaccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transaccions == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccions.FindAsync(id);
            if (transaccion == null)
            {
                return NotFound();
            }
            ViewData["IdCuenta"] = new SelectList(_context.Cuenta, "IdCuenta", "IdCuenta", transaccion.IdCuenta);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", transaccion.IdEmpresa);
            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "IdTipoTransaccion", transaccion.IdTipoTransaccion);
            return View(transaccion);
        }

        // POST: Transaccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTransaccion,IdCuenta,Fecha,IdTipoTransaccion,Monto,Descripcion,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Transaccion transaccion)
        {
            if (id != transaccion.IdTransaccion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransaccionExists(transaccion.IdTransaccion))
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
            ViewData["IdCuenta"] = new SelectList(_context.Cuenta, "IdCuenta", "IdCuenta", transaccion.IdCuenta);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", transaccion.IdEmpresa);
            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "IdTipoTransaccion", transaccion.IdTipoTransaccion);
            return View(transaccion);
        }

        // GET: Transaccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transaccions == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccions
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .Include(t => t.IdTipoTransaccionNavigation)
                .FirstOrDefaultAsync(m => m.IdTransaccion == id);
            if (transaccion == null)
            {
                return NotFound();
            }

            return View(transaccion);
        }

        // POST: Transaccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transaccions == null)
            {
                return Problem("Entity set 'ContableContext.Transaccions'  is null.");
            }
            var transaccion = await _context.Transaccions.FindAsync(id);
            if (transaccion != null)
            {
                _context.Transaccions.Remove(transaccion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransaccionExists(int id)
        {
          return (_context.Transaccions?.Any(e => e.IdTransaccion == id)).GetValueOrDefault();
        }
    }
}
