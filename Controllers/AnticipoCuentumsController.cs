using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using Microsoft.AspNetCore.Mvc;

namespace ContaFacil.Controllers
{
    public class AnticipoCuentumsController : NotificacionClass
    {
        private readonly ContableContext _context;

        public AnticipoCuentumsController(ContableContext context)
        {
            _context = context;
        }

        // GET: AnticipoCuentums
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.AnticipoCuenta.Include(a => a.IdAnticipoNavigation).Include(a => a.IdCuentaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: AnticipoCuentums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipoCuentum = await _context.AnticipoCuenta
                .Include(a => a.IdAnticipoNavigation)
                .Include(a => a.IdCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdAnticipoCuenta == id);
            if (anticipoCuentum == null)
            {
                return NotFound();
            }

            return View(anticipoCuentum);
        }

        // GET: AnticipoCuentums/Create
        public IActionResult Create()
        {
            ViewData["IdAnticipo"] = new SelectList(_context.Anticipos, "IdAnticipo", "IdAnticipo");
            ViewData["IdCuenta"] = new SelectList(_context.CuentaCobrars, "IdCuentaCobrar", "IdCuentaCobrar");
            return View();
        }

        // POST: AnticipoCuentums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(AnticipoCuentum anticipoCuenta)
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
                Anticipo anticipo = _context.Anticipos.FirstOrDefault(a => a.IdAnticipo == anticipoCuenta.IdAnticipo);
                CuentaCobrar cuentaCobrar = _context.CuentaCobrars.Include(c=>c.IdFacturaNavigation).FirstOrDefault(c => c.IdCuentaCobrar == anticipoCuenta.IdCuenta);
                anticipo.Valor = anticipo.Valor - anticipoCuenta.Valor;
                List<AnticipoCuentum> anticipoCuentum = _context.AnticipoCuenta.Where(a => a.IdCuenta == cuentaCobrar.IdCuentaCobrar).ToList();
                // Suma del campo Valor en la lista de anticipos
                decimal sumaValor = anticipoCuentum.Sum(a => a.Valor);
                decimal saldo = cuentaCobrar.PrecioVenta-sumaValor;
                if (anticipo.Valor < 0)
                {
                    Notificacion("No se puede registrar el anticipo revise los valores",NotificacionTipo.Warning);
                    return Json(new { success = false, message = "Error al registrar el anticipo" });
                }
                anticipo.FechaModificacion = new DateTime();
                anticipo.UsuarioModificacion = usuario.IdUsuario;
                _context.Update(anticipo);
                await _context.SaveChangesAsync();
                if (saldo == anticipoCuenta.Valor)
                {
                    cuentaCobrar.EstadoCobro = "CANCELADA";
                    cuentaCobrar.FechaModificacion=new DateTime();
                    cuentaCobrar.UsuarioModificacion=usuario.IdUsuario;
                    _context.Update(cuentaCobrar);
                    await _context.SaveChangesAsync();
                }
                anticipoCuenta.FechaCreacion = DateTime.Now;
                anticipoCuenta.UsuarioCreacion = usuario.IdUsuario;
                _context.Add(anticipoCuenta);
                await _context.SaveChangesAsync();

                //crear cuentas 
                string numeroAsiento = ObtenerSiguienteNumeroAsiento();
                Cliente cliente = _context.Clientes.Include(c=>c.IdPersonaNavigation).FirstOrDefault(c=>c.IdCliente==anticipo.IdCliente);
                Cuentum cuentum = _context.Cuenta.FirstOrDefault(c => c.Nombre == "Anticipos de clientes");
                var tipoTransaccion = await _context.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == "Venta");
                string descripcion = cliente.IdPersonaNavigation.Nombre + " " + cuentaCobrar.IdFacturaNavigation.NumeroFactura; 
                await CrearTransaccion(cuentum.Codigo, numeroAsiento + " Anticipo de clientes " + descripcion, anticipoCuenta.Valor, tipoTransaccion, empresa, usuario, true);
                cuentum = _context.Cuenta.FirstOrDefault(c => c.Nombre == "Cuentas por cobrar");
                await CrearTransaccion(cuentum.Codigo, numeroAsiento + " Cuentas por cobrar " + descripcion, anticipoCuenta.Valor, tipoTransaccion, empresa, usuario, true);
                return Json(new { success = true });
            }
            catch (Exception e) {
                return Json(new { success = false, message = "Erro al registrar el anticipo" });
            }
        }

        // GET: AnticipoCuentums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipoCuentum = await _context.AnticipoCuenta.FindAsync(id);
            if (anticipoCuentum == null)
            {
                return NotFound();
            }
            ViewData["IdAnticipo"] = new SelectList(_context.Anticipos, "IdAnticipo", "IdAnticipo", anticipoCuentum.IdAnticipo);
            ViewData["IdCuenta"] = new SelectList(_context.CuentaCobrars, "IdCuentaCobrar", "IdCuentaCobrar", anticipoCuentum.IdCuenta);
            return View(anticipoCuentum);
        }

        // POST: AnticipoCuentums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAnticipoCuenta,IdCuenta,IdAnticipo,Valor,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] AnticipoCuentum anticipoCuentum)
        {
            if (id != anticipoCuentum.IdAnticipoCuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anticipoCuentum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnticipoCuentumExists(anticipoCuentum.IdAnticipoCuenta))
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
            ViewData["IdAnticipo"] = new SelectList(_context.Anticipos, "IdAnticipo", "IdAnticipo", anticipoCuentum.IdAnticipo);
            ViewData["IdCuenta"] = new SelectList(_context.CuentaCobrars, "IdCuentaCobrar", "IdCuentaCobrar", anticipoCuentum.IdCuenta);
            return View(anticipoCuentum);
        }

        // GET: AnticipoCuentums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anticipoCuentum = await _context.AnticipoCuenta
                .Include(a => a.IdAnticipoNavigation)
                .Include(a => a.IdCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdAnticipoCuenta == id);
            if (anticipoCuentum == null)
            {
                return NotFound();
            }

            return View(anticipoCuentum);
        }

        // POST: AnticipoCuentums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anticipoCuentum = await _context.AnticipoCuenta.FindAsync(id);
            if (anticipoCuentum != null)
            {
                _context.AnticipoCuenta.Remove(anticipoCuentum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnticipoCuentumExists(int id)
        {
            return _context.AnticipoCuenta.Any(e => e.IdAnticipoCuenta == id);
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
                EsDebito = esdebito
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
