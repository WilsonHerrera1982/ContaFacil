using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;

namespace ContaFacil.Controllers
{
    public class ImpuestoController : NotificacionClass
    {
        private readonly ContableContext _context;

        public ImpuestoController(ContableContext context)
        {
            _context = context;
        }

        // GET: Impuesto
        public async Task<IActionResult> Index()
        {
              return _context.Impuestos != null ? 
                          View(await _context.Impuestos.ToListAsync()) :
                          Problem("Entity set 'ContableContext.Impuestos'  is null.");
        }

        // GET: Impuesto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Impuestos == null)
            {
                return NotFound();
            }

            var impuesto = await _context.Impuestos
                .FirstOrDefaultAsync(m => m.IdImpuesto == id);
            if (impuesto == null)
            {
                return NotFound();
            }

            return View(impuesto);
        }

        // GET: Impuesto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Impuesto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Impuesto impuesto)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                impuesto.UsuarioCreacion = int.Parse(idUsuario);
                impuesto.FechaCreacion = new DateTime();
                impuesto.EstadoBoolean = true;
                _context.Add(impuesto);

                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(impuesto);
            }
        }

        // GET: Impuesto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Impuestos == null)
            {
                return NotFound();
            }

            var impuesto = await _context.Impuestos.FindAsync(id);
            if (impuesto == null)
            {
                return NotFound();
            }
            return View(impuesto);
        }

        // POST: Impuesto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdImpuesto,Nombre,Porcentaje,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Impuesto impuesto)
        {
            if (id != impuesto.IdImpuesto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    impuesto.UsuarioModificacion = int.Parse(idUsuario);
                    impuesto.FechaModificacion = new DateTime();
                    _context.Update(impuesto);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ImpuestoExists(impuesto.IdImpuesto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        Notificacion("Error al actualizar el Registro" + ex.Message, NotificacionTipo.Error);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(impuesto);
        }

        // GET: Impuesto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Impuestos == null)
            {
                return NotFound();
            }

            var impuesto = await _context.Impuestos
                .FirstOrDefaultAsync(m => m.IdImpuesto == id);
            if (impuesto == null)
            {
                return NotFound();
            }

            return View(impuesto);
        }

        // POST: Impuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Impuestos == null)
            {
                return Problem("Entity set 'ContableContext.Impuestos'  is null.");
            }
            var impuesto = await _context.Impuestos.FindAsync(id);
            if (impuesto != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                impuesto.UsuarioModificacion = int.Parse(idUsuario);
                impuesto.FechaModificacion = new DateTime();
                impuesto.EstadoBoolean = false;
                _context.Impuestos.Update(impuesto);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool ImpuestoExists(int id)
        {
          return (_context.Impuestos?.Any(e => e.IdImpuesto == id)).GetValueOrDefault();
        }
    }
}
