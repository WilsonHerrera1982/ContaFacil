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
    public class UsuarioPerfilController : NotificacionClass
    {
        private readonly ContableContext _context;

        public UsuarioPerfilController(ContableContext context)
        {
            _context = context;
        }

        // GET: UsuarioPerfil
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.UsuarioPerfils.Include(u => u.IdPerfilNavigation).Include(u => u.IdUsuarioNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: UsuarioPerfil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioPerfils == null)
            {
                return NotFound();
            }

            var usuarioPerfil = await _context.UsuarioPerfils
                .Include(u => u.IdPerfilNavigation)
                .Include(u => u.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuarioPerfil == id);
            if (usuarioPerfil == null)
            {
                return NotFound();
            }

            return View(usuarioPerfil);
        }

        // GET: UsuarioPerfil/Create
        public IActionResult Create()
        {
            ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "Descripcion");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre");
            return View();
        }

        // POST: UsuarioPerfil/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioPerfil usuarioPerfil)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                usuarioPerfil.FechaCreacion = new DateTime();
                usuarioPerfil.UsuarioCreacion = int.Parse(idUsuario);
                usuarioPerfil.Estado = true;
                _context.Add(usuarioPerfil);
                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "IdPerfil", usuarioPerfil.IdPerfil);
                ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", usuarioPerfil.IdUsuario);
                Notificacion("Erro al guardar el Registro " + ex.Message, NotificacionTipo.Error);
                return View(usuarioPerfil);
            }
        }

        // GET: UsuarioPerfil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioPerfils == null)
            {
                return NotFound();
            }

            var usuarioPerfil = await _context.UsuarioPerfils.FindAsync(id);
            if (usuarioPerfil == null)
            {
                return NotFound();
            }
            ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "IdPerfil", usuarioPerfil.IdPerfil);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", usuarioPerfil.IdUsuario);
            return View(usuarioPerfil);
        }

        // POST: UsuarioPerfil/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,UsuarioPerfil usuarioPerfil)
        {
            if (id != usuarioPerfil.IdUsuarioPerfil)
            {
                return NotFound();
            }

            try
                {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                usuarioPerfil.UsuarioModificacion=int.Parse(idUsuario);
                usuarioPerfil.FechaModificacion = new DateTime();

                    _context.Update(usuarioPerfil);
                    await _context.SaveChangesAsync();
                Notificacion("Registro actualizado con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!UsuarioPerfilExists(usuarioPerfil.IdUsuarioPerfil))
                    {
                        return NotFound();
                    }
                    else
                    {
                    ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "IdPerfil", usuarioPerfil.IdPerfil);
                    ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", usuarioPerfil.IdUsuario);
                    Notificacion("Erro al actualizar el Registro " + e.Message, NotificacionTipo.Error);
                    return View(usuarioPerfil);
                }
                }
               
           
           
        }

        // GET: UsuarioPerfil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioPerfils == null)
            {
                return NotFound();
            }

            var usuarioPerfil = await _context.UsuarioPerfils
                .Include(u => u.IdPerfilNavigation)
                .Include(u => u.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuarioPerfil == id);
            if (usuarioPerfil == null)
            {
                return NotFound();
            }

            return View(usuarioPerfil);
        }

        // POST: UsuarioPerfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioPerfils == null)
            {
                return Problem("Entity set 'ContableContext.UsuarioPerfils'  is null.");
            }
            var usuarioPerfil = await _context.UsuarioPerfils.FindAsync(id);
            if (usuarioPerfil != null)
            {
                _context.UsuarioPerfils.Remove(usuarioPerfil);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioPerfilExists(int id)
        {
          return (_context.UsuarioPerfils?.Any(e => e.IdUsuarioPerfil == id)).GetValueOrDefault();
        }
    }
}
