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
    public class CategoriaProductoController : NotificacionClass
    {
        private readonly ContableContext _context;

        public CategoriaProductoController(ContableContext context)
        {
            _context = context;
        }

        // GET: CategoriaProducto
        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
            UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            return _context.CategoriaProductos != null ? 
                          View(await _context.CategoriaProductos.Where(p=>p.IdEmpresa==empresa.IdEmpresa).ToListAsync()) :
                          Problem("Entity set 'ContableContext.CategoriaProductos'  is null.");
        }

        // GET: CategoriaProducto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos
                .FirstOrDefaultAsync(m => m.IdCategoriaProducto == id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }

            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaProducto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaProducto categoriaProducto)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
                UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
                usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                categoriaProducto.UsuarioCreacion = int.Parse(idUsuario);
                categoriaProducto.FechaCreacion = new DateTime();
                categoriaProducto.IdEmpresa = empresa.IdEmpresa;
                categoriaProducto.EstadoBoolean = true;
                _context.Add(categoriaProducto);

                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(categoriaProducto);
            }
            
        }

        // GET: CategoriaProducto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos.FindAsync(id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }
            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoriaProducto,Nombre,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] CategoriaProducto categoriaProducto)
        {
            if (id != categoriaProducto.IdCategoriaProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    categoriaProducto.UsuarioModificacion = int.Parse(idUsuario);
                    categoriaProducto.FechaModificacion = new DateTime();
                    _context.Update(categoriaProducto);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CategoriaProductoExists(categoriaProducto.IdCategoriaProducto))
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
            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos
                .FirstOrDefaultAsync(m => m.IdCategoriaProducto == id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }

            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoriaProductos == null)
            {
                return Problem("Entity set 'ContableContext.CategoriaProductos'  is null.");
            }
            var categoriaProducto = await _context.CategoriaProductos.FindAsync(id);
            if (categoriaProducto != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                categoriaProducto.UsuarioModificacion = int.Parse(idUsuario);
                categoriaProducto.FechaModificacion = new DateTime();
                categoriaProducto.EstadoBoolean = false;
                _context.CategoriaProductos.Update(categoriaProducto);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaProductoExists(int id)
        {
          return (_context.CategoriaProductos?.Any(e => e.IdCategoriaProducto == id)).GetValueOrDefault();
        }
    }
}
