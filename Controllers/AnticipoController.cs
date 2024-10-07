using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using DocumentFormat.OpenXml.InkML;

namespace ContaFacil.Controllers
{
    public class AnticipoController : NotificacionClass
    {
        private readonly ContableContext _context;

        public AnticipoController(ContableContext context)
        {
            _context = context;
        }

        // GET: Anticipo
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Anticipos.Include(a => a.IdClienteNavigation).ThenInclude(a=>a.IdPersonaNavigation).Include(a => a.IdEmpresaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Anticipo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipo = await _context.Anticipos
                .Include(a => a.IdClienteNavigation)
                .Include(a => a.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdAnticipo == id);
            if (anticipo == null)
            {
                return NotFound();
            }

            return View(anticipo);
        }

        // GET: Anticipo/Create
        public IActionResult Create()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(e => e.IdUsuario == usuario.IdUsuario).FirstOrDefault();
            ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).Where(p => p.IdEmpresa == empresa.IdEmpresa).ToList();

             ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            return View();
        }

        // POST: Anticipo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Anticipo anticipo)
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
                Cliente cliente = _context.Clientes.Include(c => c.IdPersonaNavigation).FirstOrDefault(c => c.IdCliente == anticipo.IdCliente);

                anticipo.FechaCreacion = new DateTime();
                anticipo.UsuarioCreacion = usuario.IdUsuario;
                anticipo.EstadoBoolean=true;
                anticipo.IdEmpresa = empresa.IdEmpresa;
                _context.Add(anticipo);
                await _context.SaveChangesAsync();
                string numeroAsiento = ObtenerSiguienteNumeroAsiento();
                Cuentum cuentum = _context.Cuenta.FirstOrDefault(c => c.Nombre == "Anticipos de clientes");
                var tipoTransaccion = await _context.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == "Venta");
                string descripcion= cliente.IdPersonaNavigation.Nombre + " " + cliente.IdPersonaNavigation.Identificacion;
                await CrearTransaccion(cuentum.Codigo, numeroAsiento + " Anticipo de clientes " + descripcion, anticipo.Valor, tipoTransaccion, empresa, usuario,false);
                if (anticipo.TipoPago.Equals("EFECTIVO"))
                {
                    cuentum= _context.Cuenta.FirstOrDefault(c => c.Nombre == "Caja");

                    await CrearTransaccion(cuentum.Codigo, numeroAsiento + " Anticipo de clientes " + descripcion, anticipo.Valor, tipoTransaccion, empresa, usuario,false);

                }
                else
                {
                    cuentum = _context.Cuenta.FirstOrDefault(c => c.Nombre == "Bancos");
                    await CrearTransaccion(cuentum.Codigo, numeroAsiento + " Anticipo de clientes " + descripcion, anticipo.Valor, tipoTransaccion, empresa, usuario,false);

                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e) {
                ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", anticipo.IdCliente);
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", anticipo.IdEmpresa);
                return View(anticipo);
            }
        }

        // GET: Anticipo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            if (id == null)
            {
                return NotFound();
            }

            var anticipo = await _context.Anticipos.FindAsync(id);
            if (anticipo == null)
            {
                return NotFound();
            }
            ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).Where(p => p.IdEmpresa == empresa.IdEmpresa).ToList();

            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", anticipo.IdEmpresa);
            return View(anticipo);
        }

        // POST: Anticipo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAnticipo,IdCliente,IdEmpresa,Valor,NumeroComprobante,FechaComprobante,PagueseOrden,NumeroCheque,Descripcion,FechaCheque,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Anticipo anticipo)
        {
            if (id != anticipo.IdAnticipo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anticipo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnticipoExists(anticipo.IdAnticipo))
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
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", anticipo.IdCliente);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", anticipo.IdEmpresa);
            return View(anticipo);
        }

        // GET: Anticipo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipo = await _context.Anticipos
                .Include(a => a.IdClienteNavigation)
                .Include(a => a.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdAnticipo == id);
            if (anticipo == null)
            {
                return NotFound();
            }

            return View(anticipo);
        }

        // POST: Anticipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anticipo = await _context.Anticipos.FindAsync(id);
            if (anticipo != null)
            {
                _context.Anticipos.Remove(anticipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnticipoExists(int id)
        {
            return _context.Anticipos.Any(e => e.IdAnticipo == id);
        }
        private async Task CrearTransaccion(string cuentaNombre, string descripcion, decimal monto, TipoTransaccion tipoTransaccion, Empresa empresa, Usuario usuario, bool esdebito)
        {
            var cuenta = await _context.Cuenta.FirstOrDefaultAsync(c => c.Nombre == cuentaNombre || c.Codigo == cuentaNombre);

            var transaccion = new Transaccion
            {
                IdCuenta = cuenta.IdCuenta,
                Descripcion = descripcion,
                IdInventario = 0,
                Monto = monto,
                IdTipoTransaccion = tipoTransaccion.IdTipoTransaccion,
                IdEmpresa = empresa.IdEmpresa,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = usuario.IdUsuario,
                Estado = true,
                EsDebito=esdebito
            };

            _context.Add(transaccion);
            await _context.SaveChangesAsync();
        }
        private string ObtenerSiguienteNumeroAsiento()
        {
            var ultimaTransaccion = _context.Transaccions
                .OrderByDescending(t => t.IdTransaccion)
                .FirstOrDefault();

            if (ultimaTransaccion == null)
            {
                return "AS0001";
            }

            string ultimoNumeroAsiento = ultimaTransaccion.Descripcion.Split(' ')[0];
            if (ultimoNumeroAsiento.StartsWith("AS"))
            {
                int numero = int.Parse(ultimoNumeroAsiento.Substring(2)) + 1;
                return $"AS{numero:D4}";
            }
            else
            {
                return "AS0001";
            }
        }
    }
}
