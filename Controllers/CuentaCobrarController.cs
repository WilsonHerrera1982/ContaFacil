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
    public class CuentaCobrarController : Controller
    {
        private readonly ContableContext _context;

        public CuentaCobrarController(ContableContext context)
        {
            _context = context;
        }

        // GET: CuentaCobrar
        public async Task<IActionResult> Index()
        {
            var cuentasCobrar = await _context.CuentaCobrars
                .Include(c => c.IdFacturaNavigation)
                    .ThenInclude(f => f.IdClienteNavigation)
                        .ThenInclude(cl => cl.IdPersonaNavigation)
                .Include(c => c.IdFacturaNavigation)
                    .ThenInclude(f => f.IdClienteNavigation)
                        .ThenInclude(cl => cl.Anticipos)
                .Include(c => c.AnticipoCuenta)
                    .ThenInclude(ac => ac.IdAnticipoNavigation).Where(c=>c.EstadoCobro.Equals("PENDIENTE"))
                .ToListAsync();

            return View(cuentasCobrar);
        }

        // GET: CuentaCobrar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentaCobrar = await _context.CuentaCobrars
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdFacturaNavigation)
                .FirstOrDefaultAsync(m => m.IdCuentaCobrar == id);
            if (cuentaCobrar == null)
            {
                return NotFound();
            }

            return View(cuentaCobrar);
        }

        // GET: CuentaCobrar/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdFactura"] = new SelectList(_context.Facturas, "IdFactura", "IdFactura");
            return View();
        }

        // POST: CuentaCobrar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCuentaCobrar,IdEmpresa,IdFactura,PlazoDias,PrecioUnitarioFinal,Impuesto,EstadoCobro,PrecioVenta,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] CuentaCobrar cuentaCobrar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cuentaCobrar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentaCobrar.IdEmpresa);
            ViewData["IdFactura"] = new SelectList(_context.Facturas, "IdFactura", "IdFactura", cuentaCobrar.IdFactura);
            return View(cuentaCobrar);
        }

        // GET: CuentaCobrar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentaCobrar = await _context.CuentaCobrars.FindAsync(id);
            if (cuentaCobrar == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentaCobrar.IdEmpresa);
            ViewData["IdFactura"] = new SelectList(_context.Facturas, "IdFactura", "IdFactura", cuentaCobrar.IdFactura);
            return View(cuentaCobrar);
        }

        // POST: CuentaCobrar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,CuentaCobrar cuentaCobrar)
        {
            if (id != cuentaCobrar.IdCuentaCobrar)
            {
                return NotFound();
            }
            CuentaCobrar cuenta = _context.CuentaCobrars.FirstOrDefault(c=>c.IdCuentaCobrar==id);
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            try
                {
                cuenta.UsuarioModificacion = usuario.IdUsuario;
                cuenta.FechaModificacion = new DateTime();
                cuenta.PlazoDias = cuentaCobrar.PlazoDias;
                cuenta.EstadoCobro=cuentaCobrar.EstadoCobro;
                    _context.Update(cuenta);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentaCobrar.IdEmpresa);
                ViewData["IdFactura"] = new SelectList(_context.Facturas, "IdFactura", "IdFactura", cuentaCobrar.IdFactura);
                return View(cuentaCobrar);
            }
               
            
            
        }

        // GET: CuentaCobrar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentaCobrar = await _context.CuentaCobrars
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdFacturaNavigation)
                .FirstOrDefaultAsync(m => m.IdCuentaCobrar == id);
            if (cuentaCobrar == null)
            {
                return NotFound();
            }

            return View(cuentaCobrar);
        }

        // POST: CuentaCobrar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cuentaCobrar = await _context.CuentaCobrars.FindAsync(id);
            if (cuentaCobrar != null)
            {
                _context.CuentaCobrars.Remove(cuentaCobrar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuentaCobrarExists(int id)
        {
            return _context.CuentaCobrars.Any(e => e.IdCuentaCobrar == id);
        }
        public async Task<IActionResult> GetAnticipos(int idCuentaCobrar)
        {
            var cuentaCobrar = await _context.CuentaCobrars
                .Include(c => c.IdFacturaNavigation)
                    .ThenInclude(f => f.IdClienteNavigation)
                        .ThenInclude(cl => cl.Anticipos)
                .FirstOrDefaultAsync(c => c.IdCuentaCobrar == idCuentaCobrar);

            if (cuentaCobrar == null)
            {
                return NotFound();
            }

            /* var anticipos = cuentaCobrar.IdFacturaNavigation.IdClienteNavigation.Anticipos
                 .Where(a => a.EstadoBoolean && a.Valor > 0 && a.IdCliente==cuentaCobrar.IdFacturaNavigation.IdClienteNavigation.IdCliente)
                 .Select(a => new { a.IdAnticipo, a.Descripcion, a.Valor });*/
            // Obtener la lista completa de anticipos para calcular la suma total
            var anticiposQuery = _context.Anticipos
                .Where(a => a.IdCliente == cuentaCobrar.IdFacturaNavigation.IdClienteNavigation.IdCliente && a.EstadoBoolean);

            // Calcular la suma total de los anticipos
            var totalAnticipos = anticiposQuery.Sum(a => a.Valor);

            // Filtrar los anticipos con saldo positivo
            var anticiposConSaldoPositivo = anticiposQuery
                .Where(a => a.Valor > 0)
                .Select(a => new
                {
                    a.IdAnticipo,
                    a.Descripcion,
                    a.Valor,
                    TotalAnticipos = totalAnticipos // Incluimos el total de anticipos
                })
                .ToList();

            return Json(anticiposConSaldoPositivo);


        }

    }
}
