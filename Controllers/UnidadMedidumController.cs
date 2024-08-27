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
    public class UnidadMedidumController : NotificacionClass
    {
        private readonly ContableContext _context;

        public UnidadMedidumController(ContableContext context)
        {
            _context = context;
        }

        // GET: UnidadMedidum
        public async Task<IActionResult> Index()
        {
              return _context.UnidadMedida != null ? 
                          View(await _context.UnidadMedida.ToListAsync()) :
                          Problem("Entity set 'ContableContext.UnidadMedida'  is null.");
        }

        // GET: UnidadMedidum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UnidadMedida == null)
            {
                return NotFound();
            }

            var unidadMedidum = await _context.UnidadMedida
                .FirstOrDefaultAsync(m => m.IdUnidadMedida == id);
            if (unidadMedidum == null)
            {
                return NotFound();
            }

            return View(unidadMedidum);
        }

        // GET: UnidadMedidum/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UnidadMedidum/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnidadMedidum unidadMedidum)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                unidadMedidum.UsuarioCreacion = int.Parse(idUsuario);
                unidadMedidum.FechaCreacion = new DateTime();
                _context.Add(unidadMedidum);

                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(unidadMedidum);
            }
        }

        // GET: UnidadMedidum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UnidadMedida == null)
            {
                return NotFound();
            }

            var unidadMedidum = await _context.UnidadMedida.FindAsync(id);
            if (unidadMedidum == null)
            {
                return NotFound();
            }
            return View(unidadMedidum);
        }

        // POST: UnidadMedidum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUnidadMedida,Nombre,Abreviatura,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] UnidadMedidum unidadMedidum)
        {
            if (id != unidadMedidum.IdUnidadMedida)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    unidadMedidum.UsuarioModificacion = int.Parse(idUsuario);
                    unidadMedidum.FechaModificacion = new DateTime();
                    _context.Update(unidadMedidum);

                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!UnidadMedidumExists(unidadMedidum.IdUnidadMedida))
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
            return View(unidadMedidum);
        }

        // GET: UnidadMedidum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UnidadMedida == null)
            {
                return NotFound();
            }

            var unidadMedidum = await _context.UnidadMedida
                .FirstOrDefaultAsync(m => m.IdUnidadMedida == id);
            if (unidadMedidum == null)
            {
                return NotFound();
            }

            return View(unidadMedidum);
        }

        // POST: UnidadMedidum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UnidadMedida == null)
            {
                return Problem("Entity set 'ContableContext.UnidadMedida'  is null.");
            }
            var unidadMedidum = await _context.UnidadMedida.FindAsync(id);
            if (unidadMedidum != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                unidadMedidum.UsuarioModificacion = int.Parse(idUsuario);
                unidadMedidum.FechaModificacion = new DateTime();
                unidadMedidum.EstadoBoolean = false;
                _context.UnidadMedida.Update(unidadMedidum);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool UnidadMedidumExists(int id)
        {
          return (_context.UnidadMedida?.Any(e => e.IdUnidadMedida == id)).GetValueOrDefault();
        }
    }
}
