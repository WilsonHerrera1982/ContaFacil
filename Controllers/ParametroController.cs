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
    public class ParametroController : Controller
    {
        private readonly ContableContext _context;

        public ParametroController(ContableContext context)
        {
            _context = context;
        }

        // GET: Parametro
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Parametros.Include(p => p.IdEmpresaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Parametro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parametro = await _context.Parametros
                .Include(p => p.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdParametro == id);
            if (parametro == null)
            {
                return NotFound();
            }

            return View(parametro);
        }

        // GET: Parametro/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre");
            return View();
        }

        // POST: Parametro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parametro parametro)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                parametro.UsuarioCreacion=int.Parse(idUsuario);
                parametro.FechaCreacion = new DateTime();
                parametro.EstadoBoolean = true;
                _context.Add(parametro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", parametro.IdEmpresa);
                return View(parametro);
            }
        }

        // GET: Parametro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parametro = await _context.Parametros.FindAsync(id);
            if (parametro == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", parametro.IdEmpresa);
            return View(parametro);
        }

        // POST: Parametro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdParametro,IdEmpresa,NombreParametro,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,Valor")] Parametro parametro)
        {
            if (id != parametro.IdParametro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parametro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParametroExists(parametro.IdParametro))
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
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", parametro.IdEmpresa);
            return View(parametro);
        }

        // GET: Parametro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parametro = await _context.Parametros
                .Include(p => p.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdParametro == id);
            if (parametro == null)
            {
                return NotFound();
            }

            return View(parametro);
        }

        // POST: Parametro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parametro = await _context.Parametros.FindAsync(id);
            if (parametro != null)
            {
                _context.Parametros.Remove(parametro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParametroExists(int id)
        {
            return _context.Parametros.Any(e => e.IdParametro == id);
        }
    }
}
