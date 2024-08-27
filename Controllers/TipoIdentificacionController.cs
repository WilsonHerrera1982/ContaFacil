using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;

namespace ContaFacil.Controllers
{
    public class TipoIdentificacionController : NotificacionClass
    {
        private readonly ContableContext _context;

        public TipoIdentificacionController(ContableContext context)
        {
            _context = context;
        }

        // GET: TipoIdentificacion
        public async Task<IActionResult> Index()
        {
              return _context.TipoIdentificacions != null ? 
                          View(await _context.TipoIdentificacions.ToListAsync()) :
                          Problem("Entity set 'ContableContext.TipoIdentificacions'  is null.");
        }

        // GET: TipoIdentificacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TipoIdentificacions == null)
            {
                return NotFound();
            }

            var tipoIdentificacion = await _context.TipoIdentificacions
                .FirstOrDefaultAsync(m => m.IdTipoIdemtificacion == id);
            if (tipoIdentificacion == null)
            {
                return NotFound();
            }

            return View(tipoIdentificacion);
        }

        // GET: TipoIdentificacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoIdentificacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoIdentificacion tipoIdentificacion)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                tipoIdentificacion.UsuarioCreacion = int.Parse(idUsuario);
                tipoIdentificacion.FechaCreacion = new DateTime();
                tipoIdentificacion.EstadoBoolean = true;
                _context.Add(tipoIdentificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(tipoIdentificacion);
            }
        }

        // GET: TipoIdentificacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TipoIdentificacions == null)
            {
                return NotFound();
            }

            var tipoIdentificacion = await _context.TipoIdentificacions.FindAsync(id);
            if (tipoIdentificacion == null)
            {
                return NotFound();
            }
            return View(tipoIdentificacion);
        }

        // POST: TipoIdentificacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoIdemtificacion,CodigoSri,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] TipoIdentificacion tipoIdentificacion)
        {
            if (id != tipoIdentificacion.IdTipoIdemtificacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoIdentificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoIdentificacionExists(tipoIdentificacion.IdTipoIdemtificacion))
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
            return View(tipoIdentificacion);
        }

        // GET: TipoIdentificacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TipoIdentificacions == null)
            {
                return NotFound();
            }

            var tipoIdentificacion = await _context.TipoIdentificacions
                .FirstOrDefaultAsync(m => m.IdTipoIdemtificacion == id);
            if (tipoIdentificacion == null)
            {
                return NotFound();
            }

            return View(tipoIdentificacion);
        }

        // POST: TipoIdentificacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TipoIdentificacions == null)
            {
                return Problem("Entity set 'ContableContext.TipoIdentificacions'  is null.");
            }
            var tipoIdentificacion = await _context.TipoIdentificacions.FindAsync(id);
            if (tipoIdentificacion != null)
            {
                _context.TipoIdentificacions.Remove(tipoIdentificacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoIdentificacionExists(int id)
        {
          return (_context.TipoIdentificacions?.Any(e => e.IdTipoIdemtificacion == id)).GetValueOrDefault();
        }
    }
}
