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
    public class DespachoController : NotificacionClass
    {
        private readonly ContableContext _context;

        public DespachoController(ContableContext context)
        {
            _context = context;
        }

        // GET: Despacho
        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = await _context.Usuarios
                .Where(u => u.IdUsuario == int.Parse(idUsuario))
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync();

            UsuarioSucursal usuarioSucursal = await _context.UsuarioSucursals
                .Where(u => u.IdUsuario == usuario.IdUsuario)
                .FirstOrDefaultAsync();

            Emisor emisor = await _context.Emisors
                .Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion)
                .FirstOrDefaultAsync();

            Empresa empresa = await _context.Empresas
                .Where(e => e.Identificacion == emisor.Ruc)
                .FirstOrDefaultAsync();

            var despachos = await _context.Despachos
                .Where(d => d.IdSucursal == usuarioSucursal.IdSucursal || d.IdSucursalDestino==usuarioSucursal.IdSucursal)
                .Include(d => d.IdEmpresaNavigation)
                .Include(d => d.IdSucursalNavigation)
                .Include(d => d.IdUsuarioNavigation)
                .ToListAsync();

            // Cargar los nombres de las sucursales de destino
            foreach (var despacho in despachos)
            {
                despacho.CargarNombreSucursalDestino(_context);
            }
            ViewBag.SucursalId = usuarioSucursal.IdSucursal;

            return View(despachos);
        }

        // GET: Despacho/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var despacho = await _context.Despachos
                .Include(d => d.IdEmpresaNavigation)
                .Include(d => d.IdSucursalNavigation)
                .Include(d => d.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdDespacho == id);
            if (despacho == null)
            {
                return NotFound();
            }

            return View(despacho);
        }

        // GET: Despacho/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdSucursal"] = new SelectList(_context.Sucursals, "IdSucursal", "IdSucursal");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Despacho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDespacho,IdUsuario,IdEmpresa,IdSucursal,NumeroDespacho,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,EstadoDespacho,IdSucursalDestino")] Despacho despacho)
        {
            if (ModelState.IsValid)
            {
                _context.Add(despacho);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", despacho.IdEmpresa);
            ViewData["IdSucursal"] = new SelectList(_context.Sucursals, "IdSucursal", "IdSucursal", despacho.IdSucursal);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", despacho.IdUsuario);
            return View(despacho);
        }

        // GET: Despacho/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var despacho = await _context.Despachos.FindAsync(id);
            if (despacho == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", despacho.IdEmpresa);
            ViewData["IdSucursal"] = new SelectList(_context.Sucursals, "IdSucursal", "IdSucursal", despacho.IdSucursal);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", despacho.IdUsuario);
            return View(despacho);
        }

        // POST: Despacho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDespacho,IdUsuario,IdEmpresa,IdSucursal,NumeroDespacho,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,EstadoDespacho,IdSucursalDestino")] Despacho despacho)
        {
            if (id != despacho.IdDespacho)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(despacho);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DespachoExists(despacho.IdDespacho))
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
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", despacho.IdEmpresa);
            ViewData["IdSucursal"] = new SelectList(_context.Sucursals, "IdSucursal", "IdSucursal", despacho.IdSucursal);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", despacho.IdUsuario);
            return View(despacho);
        }

        // GET: Despacho/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var despacho = await _context.Despachos
                .Include(d => d.IdEmpresaNavigation)
                .Include(d => d.IdSucursalNavigation)
                .Include(d => d.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdDespacho == id);
            if (despacho == null)
            {
                return NotFound();
            }

            return View(despacho);
        }

        // POST: Despacho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var despacho = await _context.Despachos.FindAsync(id);
            if (despacho != null)
            {
                _context.Despachos.Remove(despacho);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DespachoExists(int id)
        {
            return _context.Despachos.Any(e => e.IdDespacho == id);
        }

        [HttpPost]
        public async Task<IActionResult> AceptarDespacho(int id)
        {
            Despacho despacho=_context.Despachos.Find(id);
            if (despacho != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                despacho.FechaModificacion = new DateTime();
                despacho.UsuarioModificacion=int.Parse(idUsuario);
                despacho.EstadoDespacho = "ACEPTADO";
                _context.Add(despacho);
                _context.SaveChanges();
                Inventario inventario = new Inventario();
                inventario = _context.Inventarios
                       .Where(i=>i.NumeroDespacho == despacho.NumeroDespacho)
                       .OrderByDescending(i => i.FechaCreacion)
                       .FirstOrDefault();
                Inventario ultimoMovimiento = new Inventario();
                ultimoMovimiento = _context.Inventarios
                       .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T") & i.IdProducto == inventario.IdProducto)
                       .OrderByDescending(i => i.FechaCreacion)
                       .FirstOrDefault();

                Notificacion("Despacho aceptado con exito",NotificacionTipo.Success);
                return View();
            }
            else
            {
                Notificacion("Despacho no econtrado!",NotificacionTipo.Warning);
                return View();
            }
           
        }
        [HttpPost]
        public async Task<IActionResult> RechazarDespacho(int id)
        {
            Despacho despacho = _context.Despachos.Find(id);
            if (despacho != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                despacho.FechaModificacion = new DateTime();
                despacho.UsuarioModificacion = int.Parse(idUsuario);
                despacho.EstadoDespacho = "RECHAZADO";
                _context.Add(despacho);
                _context.SaveChanges();
                Notificacion("Despacho rechazado con exito", NotificacionTipo.Success);
                return View();
            }
            else
            {
                Notificacion("Despacho no econtrado!", NotificacionTipo.Warning);
                return View();
            }

        }
    }
}
