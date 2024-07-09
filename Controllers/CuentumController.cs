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
    public class CuentumController : Controller
    {
        private readonly ContableContext _context;

        public CuentumController(ContableContext context)
        {
            _context = context;
        }

        // GET: Cuentum
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Cuenta.Include(c => c.IdEmpresaNavigation).Include(c => c.IdTipoCuentaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Cuentum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cuenta == null)
            {
                return NotFound();
            }

            var cuentum = await _context.Cuenta
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdTipoCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (cuentum == null)
            {
                return NotFound();
            }

            return View(cuentum);
        }

        // GET: Cuentum/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta");
            return View();
        }

        // POST: Cuentum/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCuenta,Nombre,IdTipoCuenta,SaldoInicial,SaldoActual,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Cuentum cuentum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cuentum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentum.IdEmpresa);
            ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta", cuentum.IdTipoCuenta);
            return View(cuentum);
        }

        // GET: Cuentum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cuenta == null)
            {
                return NotFound();
            }

            var cuentum = await _context.Cuenta.FindAsync(id);
            if (cuentum == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentum.IdEmpresa);
            ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta", cuentum.IdTipoCuenta);
            return View(cuentum);
        }

        // POST: Cuentum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCuenta,Nombre,IdTipoCuenta,SaldoInicial,SaldoActual,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Cuentum cuentum)
        {
            if (id != cuentum.IdCuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuentum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentumExists(cuentum.IdCuenta))
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
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentum.IdEmpresa);
            ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta", cuentum.IdTipoCuenta);
            return View(cuentum);
        }

        // GET: Cuentum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cuenta == null)
            {
                return NotFound();
            }

            var cuentum = await _context.Cuenta
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdTipoCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (cuentum == null)
            {
                return NotFound();
            }

            return View(cuentum);
        }

        // POST: Cuentum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cuenta == null)
            {
                return Problem("Entity set 'ContableContext.Cuenta'  is null.");
            }
            var cuentum = await _context.Cuenta.FindAsync(id);
            if (cuentum != null)
            {
                _context.Cuenta.Remove(cuentum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuentumExists(int id)
        {
          return (_context.Cuenta?.Any(e => e.IdCuenta == id)).GetValueOrDefault();
        }
    }
}
