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
    public class InventarioController : NotificacionClass
    {
        private readonly ContableContext _context;

        public InventarioController(ContableContext context)
        {
            _context = context;
        }

        // GET: Inventario
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Inventarios.Include(i => i.IdProductoNavigation).AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(i => i.FechaMovimiento >= startDate.Value && i.FechaMovimiento <= endDate.Value);
            }

            var inventarios = await query.ToListAsync();

            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            return View(inventarios);
        }

        // GET: Inventario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // GET: Inventario/Create
        public IActionResult Create()
        {
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto");
            return View();
        }

        // POST: Inventario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventario inventario)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                inventario.UsuarioCreacion = int.Parse(idUsuario);
                inventario.FechaCreacion = new DateTime();
                _context.Add(inventario);

                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.IdProducto);
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(inventario);
            }
        }

        // GET: Inventario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.IdProducto);
            return View(inventario);
        }

        // POST: Inventario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInventario,IdProducto,TipoMovimiento,Cantidad,FechaMovimiento,NumeroDespacho,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Inventario inventario)
        {
            if (id != inventario.IdInventario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    inventario.UsuarioModificacion = int.Parse(idUsuario);
                    inventario.FechaModificacion = new DateTime();
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!InventarioExists(inventario.IdInventario))
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
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.IdProducto);
            return View(inventario);
        }

        // GET: Inventario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventarios == null)
            {
                return Problem("Entity set 'ContableContext.Inventarios'  is null.");
            }
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                inventario.UsuarioModificacion = int.Parse(idUsuario);
                inventario.FechaModificacion = new DateTime();
                inventario.EstadoBoolean = false;
                _context.Inventarios.Update(inventario);

                Producto producto = new Producto();
                producto = _context.Productos.Where(p => p.IdProducto == inventario.IdProducto).FirstOrDefault();
                if (producto != null)
                {
                    producto.Stock = 0;
                    _context.Update(producto);
                }
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(int id)
        {
          return (_context.Inventarios?.Any(e => e.IdInventario == id)).GetValueOrDefault();
        }
    }
}
