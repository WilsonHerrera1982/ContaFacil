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
    public class MenusController : NotificacionClass
    {
        private readonly ContableContext _context;

        public MenusController(ContableContext context)
        {
            _context = context;
        }

        // GET: Menus
        public async Task<IActionResult> Index()
        {
              return _context.Menus != null ? 
                          View(await _context.Menus.ToListAsync()) :
                          Problem("Entity set 'ContableContext.Menus'  is null.");
        }

        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.IdMenu == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menus/Create
        public IActionResult Create()
        {
            Menu menuNinguno = new Menu
            {
                IdMenu = 0,
                Descripcion = "Ninguno"
            };

            var menus = _context.Menus.ToList();
            menus.Insert(0, menuNinguno);

            ViewData["Menus"] = new SelectList(menus, "IdMenu", "Descripcion");
            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Menu menu)
        {
            try
            {

                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                menu.Url = "#";
                menu.UsuarioCreacion = int.Parse(idUsuario);
                menu.FechaCreacion = new DateTime();
                _context.Add(menu);
                await _context.SaveChangesAsync();
                Notificacion("Registro guardardo con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Notificacion("Error al guardar el Registro "+e.Message, NotificacionTipo.Error);
                return View(menu);
            }
           
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            Menu menuNinguno = new Menu
            {
                IdMenu = 0,
                Descripcion = "Ninguno"
            };

            var menus = _context.Menus.ToList();
            menus.Insert(0, menuNinguno);

            ViewData["Menus"] = new SelectList(menus, "IdMenu", "Descripcion");
            return View(menu);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Menu menu)
        {
            if (id != menu.IdMenu)
            {
                return NotFound();
            }

            
                try
                {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                menu.FechaModificacion = new DateTime();
                menu.UsuarioModificacion=int.Parse(idUsuario);
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.IdMenu))
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

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.IdMenu == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Menus == null)
            {
                return Problem("Entity set 'ContableContext.Menus'  is null.");
            }
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
          return (_context.Menus?.Any(e => e.IdMenu == id)).GetValueOrDefault();
        }
    }
}
