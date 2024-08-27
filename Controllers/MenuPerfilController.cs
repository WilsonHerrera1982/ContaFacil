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
    public class MenuPerfilController : NotificacionClass
    {
        private readonly ContableContext _context;

        public MenuPerfilController(ContableContext context)
        {
            _context = context;
        }

        // GET: MenuPerfil
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.MenuPerfils.Include(m => m.IdMenuNavigation).Include(m => m.IdPerfilNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: MenuPerfil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MenuPerfils == null)
            {
                return NotFound();
            }

            var menuPerfil = await _context.MenuPerfils
                .Include(m => m.IdMenuNavigation)
                .Include(m => m.IdPerfilNavigation)
                .FirstOrDefaultAsync(m => m.IdMenuPerfil == id);
            if (menuPerfil == null)
            {
                return NotFound();
            }

            return View(menuPerfil);
        }

        // GET: MenuPerfil/Create
        public IActionResult Create()
        {
            ViewData["IdMenu"] = new SelectList(_context.Menus, "IdMenu", "Descripcion");
            ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "Descripcion");
            return View();
        }

        // POST: MenuPerfil/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuPerfil menuPerfil)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                menuPerfil.FechaCreacion = new DateTime();
                menuPerfil.UsuarioCreacion=int.Parse(idUsuario);
                menuPerfil.Estado = true;
                _context.Add(menuPerfil);
                await _context.SaveChangesAsync();
                Notificacion("Registro guardardo con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["IdMenu"] = new SelectList(_context.Menus, "IdMenu", "Descripcion", menuPerfil.IdMenu);
                ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "Descripcion", menuPerfil.IdPerfil);
                Notificacion("Erro al guardar el Registro " + e.Message, NotificacionTipo.Error);
                return View(menuPerfil);
            }
        }

        // GET: MenuPerfil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MenuPerfils == null)
            {
                return NotFound();
            }

            var menuPerfil = await _context.MenuPerfils.FindAsync(id);
            if (menuPerfil == null)
            {
                return NotFound();
            }
            ViewData["IdMenu"] = new SelectList(_context.Menus, "IdMenu", "Descripcion", menuPerfil.IdMenu);
            ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "Descripcion", menuPerfil.IdPerfil);
            return View(menuPerfil);
        }

        // POST: MenuPerfil/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,MenuPerfil menuPerfil)
        {
               try
                {
                    _context.Update(menuPerfil);
                    await _context.SaveChangesAsync();
                Notificacion("Registro guardado con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                ViewData["IdMenu"] = new SelectList(_context.Menus, "IdMenu", "Descripcion", menuPerfil.IdMenu);
                ViewData["IdPerfil"] = new SelectList(_context.Perfils, "IdPerfil", "Descripcion", menuPerfil.IdPerfil);
                Notificacion("Error al actualizar el registro",NotificacionTipo.Error);
                return View(menuPerfil);
            }
           
        }

        // GET: MenuPerfil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MenuPerfils == null)
            {
                return NotFound();
            }

            var menuPerfil = await _context.MenuPerfils
                .Include(m => m.IdMenuNavigation)
                .Include(m => m.IdPerfilNavigation)
                .FirstOrDefaultAsync(m => m.IdMenuPerfil == id);
            if (menuPerfil == null)
            {
                return NotFound();
            }

            return View(menuPerfil);
        }

        // POST: MenuPerfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MenuPerfils == null)
            {
                return Problem("Entity set 'ContableContext.MenuPerfils'  is null.");
            }
            var menuPerfil = await _context.MenuPerfils.FindAsync(id);
            if (menuPerfil != null)
            {
                _context.MenuPerfils.Remove(menuPerfil);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuPerfilExists(int id)
        {
          return (_context.MenuPerfils?.Any(e => e.IdMenuPerfil == id)).GetValueOrDefault();
        }
    }
}
