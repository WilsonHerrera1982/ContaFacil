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
    public class ProveedorController : NotificacionClass
    {
        private readonly ContableContext _context;

        public ProveedorController(ContableContext context)
        {
            _context = context;
        }

        // GET: Proveedor
        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            var contableContext = _context.Proveedors.Where(e=>e.IdEmpresa==empresa.IdEmpresa).Include(p => p.IdEmpresaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Proveedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .Include(p => p.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // GET: Proveedor/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
            {
                IdImpuesto = i.Porcentaje,
                NombrePorcentaje = i.Nombre
            }), "IdImpuesto", "NombrePorcentaje");
            ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION EN LA FUENTE").Select(i => new
            {
                IdImpuesto = i.Porcentaje,
                NombrePorcentaje = i.Nombre 
            }), "IdImpuesto", "NombrePorcentaje");
            return View();
        }

        // POST: Proveedor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Proveedor proveedor)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = new Usuario();
                usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                proveedor.UsuarioCreacion = int.Parse(idUsuario);
                proveedor.FechaCreacion = new DateTime();
                proveedor.IdEmpresa=empresa.IdEmpresa;
                _context.Add(proveedor);

                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION EN LA FUENTE").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", proveedor.IdEmpresa);
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(proveedor);
            }
        }

        // GET: Proveedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", proveedor.IdEmpresa);
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
            {
                IdImpuesto = i.Porcentaje,
                NombrePorcentaje = i.Nombre
            }), "IdImpuesto", "NombrePorcentaje");
            ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Nombre.Contains("RETENCION IR")).Select(i => new
            {
                IdImpuesto = i.Porcentaje,
                NombrePorcentaje = i.Nombre
            }), "IdImpuesto", "NombrePorcentaje");
            return View(proveedor);
        }

        // POST: Proveedor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Proveedor proveedor)
        {
            if (id != proveedor.IdProveedor)
            {
                return NotFound();
            }

             try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    proveedor.UsuarioModificacion = int.Parse(idUsuario);
                    proveedor.FechaModificacion = new DateTime();
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException ex)
                {
                   
                        Notificacion("Error al actualizar el Registro" + ex.Message, NotificacionTipo.Error);
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", proveedor.IdEmpresa);
                ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION EN LA FUENTE").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                return View(proveedor);
            }
                
          
            
        }

        // GET: Proveedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .Include(p => p.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Proveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Proveedors == null)
            {
                return Problem("Entity set 'ContableContext.Proveedors'  is null.");
            }
            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                proveedor.UsuarioModificacion = int.Parse(idUsuario);
                proveedor.FechaModificacion = new DateTime();
                proveedor.Estado = false;
                _context.Proveedors.Update(proveedor);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
          return (_context.Proveedors?.Any(e => e.IdProveedor == id)).GetValueOrDefault();
        }
    }
}
