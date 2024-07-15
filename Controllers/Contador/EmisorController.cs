using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;

namespace ContaFacil.Controllers.Contador
{
    public class EmisorController : Controller
    {
        private readonly ContableContext _context;

        public EmisorController(ContableContext context)
        {
            _context = context;
        }

        // GET: Emisor
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Emisors.Include(e => e.IdEmpresaNavigation).Include(e => e.IdUsuarioNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Emisor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Emisors == null)
            {
                return NotFound();
            }

            var emisor = await _context.Emisors
                .Include(e => e.IdEmpresaNavigation)
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdEmisor == id);
            if (emisor == null)
            {
                return NotFound();
            }

            return View(emisor);
        }

        // GET: Emisor/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Emisor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmisor,IdUsuario,IdEmpresa,RazonSocial,NombreComercial,Ruc,NombreUsuario,Telefono,CorreoElectronico,Establecimiento,PuntoEmision,Secuencial,Direccion,CertificadoDigital,Clave,ObligadoContabilidad,TipoAmbiente,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Emisor emisor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emisor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", emisor.IdEmpresa);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", emisor.IdUsuario);
            return View(emisor);
        }

        // GET: Emisor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Emisors == null)
            {
                return NotFound();
            }

            var emisor = await _context.Emisors.FindAsync(id);
            if (emisor == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", emisor.IdEmpresa);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", emisor.IdUsuario);
            return View(emisor);
        }

        // POST: Emisor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmisor,IdUsuario,IdEmpresa,RazonSocial,NombreComercial,Ruc,NombreUsuario,Telefono,CorreoElectronico,Establecimiento,PuntoEmision,Secuencial,Direccion,CertificadoDigital,Clave,ObligadoContabilidad,TipoAmbiente,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Emisor emisor)
        {
            if (id != emisor.IdEmisor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emisor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmisorExists(emisor.IdEmisor))
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
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", emisor.IdEmpresa);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", emisor.IdUsuario);
            return View(emisor);
        }

        // GET: Emisor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Emisors == null)
            {
                return NotFound();
            }

            var emisor = await _context.Emisors
                .Include(e => e.IdEmpresaNavigation)
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdEmisor == id);
            if (emisor == null)
            {
                return NotFound();
            }

            return View(emisor);
        }

        // POST: Emisor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Emisors == null)
            {
                return Problem("Entity set 'ContableContext.Emisors'  is null.");
            }
            var emisor = await _context.Emisors.FindAsync(id);
            if (emisor != null)
            {
                _context.Emisors.Remove(emisor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmisorExists(int id)
        {
          return (_context.Emisors?.Any(e => e.IdEmisor == id)).GetValueOrDefault();
        }
    }
}
