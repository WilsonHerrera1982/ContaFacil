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
    public class UsuariosController : NotificacionClass
    {
        private readonly ContableContext _context;

        public UsuariosController(ContableContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Usuarios.Include(u => u.IdEmpresaNavigation).Include(u => u.IdPersonaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdEmpresaNavigation)
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre");
            ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Usuario usuario)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                usuario.FechaCreacion = new DateTime();
                usuario.UsuarioCreacion = int.Parse(idUsuario);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                Notificacion("Registro guardardo con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", usuario.IdEmpresa);
                ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Direccion", usuario.IdPersona);
                Notificacion("Erro al guardar el Registro " + ex.Message, NotificacionTipo.Error);
                return View(usuario);
            }
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre", usuario.IdEmpresa);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Nombre", usuario.IdPersona);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            
                try
                {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                usuario.FechaModificacion = new DateTime();
                usuario.UsuarioCreacion= int.Parse(idUsuario);
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                Notificacion("Registro actualizado con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!UsuarioExists(usuario.IdUsuario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        Notificacion("Error al actualizar el Registro " + e.Message, NotificacionTipo.Error);
                    ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", usuario.IdEmpresa);
                    ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Direccion", usuario.IdPersona);
                    return View(usuario);                    
                    }
                }
                
            
            
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdEmpresaNavigation)
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'ContableContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }
    }
}
