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
    public class PerfilController : Controller
    {
        private readonly ContableContext _context;

        public PerfilController(ContableContext context)
        {
            _context = context;
        }

        // GET: Perfil
        public async Task<IActionResult> Index()
        {
              return _context.Perfils != null ? 
                          View(await _context.Perfils.ToListAsync()) :
                          Problem("Entity set 'ContableContext.Perfils'  is null.");
        }

        // GET: Perfil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Perfils == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfils
                .FirstOrDefaultAsync(m => m.IdPerfil == id);
            if (perfil == null)
            {
                return NotFound();
            }

            return View(perfil);
        }

        // GET: Perfil/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Perfil/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Perfil perfil)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                perfil.FechaCreacion = new DateTime();
                perfil.UsuarioCreacion=int.Parse(idUsuario);
                _context.Add(perfil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(perfil);
            }
        }

        // GET: Perfil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Perfils == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfils.FindAsync(id);
            if (perfil == null)
            {
                return NotFound();
            }
            return View(perfil);
        }

        // POST: Perfil/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPerfil,Descripcion,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Perfil perfil)
        {
            if (id != perfil.IdPerfil)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(perfil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerfilExists(perfil.IdPerfil))
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
            return View(perfil);
        }

        // GET: Perfil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Perfils == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfils
                .FirstOrDefaultAsync(m => m.IdPerfil == id);
            if (perfil == null)
            {
                return NotFound();
            }

            return View(perfil);
        }

        // POST: Perfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Perfils == null)
            {
                return Problem("Entity set 'ContableContext.Perfils'  is null.");
            }
            var perfil = await _context.Perfils.FindAsync(id);
            if (perfil != null)
            {
                _context.Perfils.Remove(perfil);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PerfilExists(int id)
        {
          return (_context.Perfils?.Any(e => e.IdPerfil == id)).GetValueOrDefault();
        }
    }
}
