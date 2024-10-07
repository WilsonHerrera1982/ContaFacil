using System;
using System.Collections.Generic;  
using System.ComponentModel;
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
    public class TransaccionController : NotificacionClass
    {
        private readonly ContableContext _context;

        public TransaccionController(ContableContext context)
        {
            _context = context;
        }

        // GET: Transaccion
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Transaccions.Include(t => t.IdCuentaNavigation).Include(t => t.IdEmpresaNavigation).Include(t => t.IdTipoTransaccionNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Transaccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transaccions == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccions
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .Include(t => t.IdTipoTransaccionNavigation)
                .FirstOrDefaultAsync(m => m.IdTransaccion == id);
            if (transaccion == null)
            {
                return NotFound();
            }

            return View(transaccion);
        }

        // GET: Transaccion/Create
        public IActionResult Create()
        {
            var cuentas = _context.Cuenta
    .Select(c => new
    {
        IdCuenta = c.IdCuenta,
        NombreCompleto = c.Codigo + " - " + c.Nombre // Concatenación del código y el nombre
    })
    .ToList();

            ViewData["IdCuenta"] = new SelectList(cuentas, "IdCuenta", "NombreCompleto");

            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "Nombre");
            return View();
        }

        [HttpGet]
        public JsonResult GetAccountBalance(int idCuenta)
        {
            var balance = _context.Transaccions
     .Where(t => t.IdCuenta == idCuenta)
     .Sum(t => t.EsDebito ? Math.Abs(t.Monto) : -Math.Abs(t.Monto));

            return Json(balance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateTime Fecha, string Descripcion, List<AccountEntry> Accounts)
        {
            Accounts = Accounts.Where(a => !(a.Credit > 0 && a.Debit > 0)).ToList();
            if (Accounts == null || !Accounts.Any())
            {
                return Json(new { success = false, message = "Debe ingresar al menos una cuenta." });
            }
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos. Por favor, revise el formulario." });
            }
            try
            {
                // Verificar la existencia de todas las cuentas
                var cuentasExistentes = await _context.Cuenta.Select(c => c.IdCuenta).ToListAsync();
                var cuentasInvalidas = Accounts.Select(a => a.IdCuenta).Except(cuentasExistentes).ToList();

                if (cuentasInvalidas.Any())
                {
                    return Json(new { success = false, message = $"Las siguientes cuentas no existen: {string.Join(", ", cuentasInvalidas)}" });
                }

                string numeroAsiento = ObtenerSiguienteNumeroAsiento();
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = await _context.Usuarios
                    .Where(u => u.IdUsuario == int.Parse(idUsuario))
                    .Include(p => p.IdPersonaNavigation)
                    .FirstOrDefaultAsync();
                if (usuario == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }
                Emisor emisor = await _context.Emisors
                    .FirstOrDefaultAsync(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion);
                if (emisor == null)
                {
                    return Json(new { success = false, message = "Emisor no encontrado." });
                }
                Empresa empresa = await _context.Empresas
                    .FirstOrDefaultAsync(e => e.Identificacion == emisor.Ruc);
                if (empresa == null)
                {
                    return Json(new { success = false, message = "Empresa no encontrada." });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    foreach (var account in Accounts)
                    {
                        if (account.Debit > 0)
                        {
                            var debitTransaction = new Transaccion
                            {
                                IdCuenta = account.IdCuenta,
                                Fecha = DateOnly.FromDateTime(Fecha),
                                IdTipoTransaccion = 1,
                                Monto = account.Debit,
                                Descripcion = $"{numeroAsiento} {Descripcion}",
                                Estado = true,
                                EsDebito = true,
                                UsuarioCreacion = usuario.IdUsuario,
                                FechaCreacion = DateTime.Now,
                                IdEmpresa = empresa.IdEmpresa
                            };
                            _context.Add(debitTransaction);
                        }
                        if (account.Credit > 0)
                        {
                            var creditTransaction = new Transaccion
                            {
                                IdCuenta = account.IdCuenta,
                                Fecha = DateOnly.FromDateTime(Fecha),
                                IdTipoTransaccion = 1,
                                Monto = account.Credit,
                                Descripcion = $"{numeroAsiento} {Descripcion}",
                                Estado = true,
                                EsDebito = false,
                                UsuarioCreacion = usuario.IdUsuario,
                                FechaCreacion = DateTime.Now,
                                IdEmpresa = empresa.IdEmpresa
                            };
                            _context.Add(creditTransaction);
                        }
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Json(new { success = true, message = "Asiento contable registrado correctamente" });
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception
                    //_logger.LogError(ex, "Error al guardar el asiento contable");
                    return Json(new { success = false, message = "Error al guardar el asiento contable. Por favor, verifique que todas las cuentas sean válidas." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = $"Error al guardar el asiento contable: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al procesar la solicitud: {ex.Message}" });
            }
        }
        private void PrepareViewData()
        {
            var cuentas = _context.Cuenta
                .Select(c => new
                {
                    IdCuenta = c.IdCuenta,
                    NombreCompleto = c.Codigo + " - " + c.Nombre
                })
                .ToList();

            ViewData["IdCuenta"] = new SelectList(cuentas, "IdCuenta", "NombreCompleto");
            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "Nombre");
        }
        public class AccountEntry
        {
            public int IdCuenta { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
        }

        // GET: Transaccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transaccions == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccions.FindAsync(id);
            if (transaccion == null)
            {
                return NotFound();
            }
            ViewData["IdCuenta"] = new SelectList(_context.Cuenta, "IdCuenta", "IdCuenta", transaccion.IdCuenta);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", transaccion.IdEmpresa);
            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "IdTipoTransaccion", transaccion.IdTipoTransaccion);
            return View(transaccion);
        }

        // POST: Transaccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTransaccion,IdCuenta,Fecha,IdTipoTransaccion,Monto,Descripcion,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Transaccion transaccion)
        {
            if (id != transaccion.IdTransaccion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransaccionExists(transaccion.IdTransaccion))
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
            ViewData["IdCuenta"] = new SelectList(_context.Cuenta, "IdCuenta", "IdCuenta", transaccion.IdCuenta);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", transaccion.IdEmpresa);
            ViewData["IdTipoTransaccion"] = new SelectList(_context.TipoTransaccions, "IdTipoTransaccion", "IdTipoTransaccion", transaccion.IdTipoTransaccion);
            return View(transaccion);
        }

        // GET: Transaccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transaccions == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccions
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .Include(t => t.IdTipoTransaccionNavigation)
                .FirstOrDefaultAsync(m => m.IdTransaccion == id);
            if (transaccion == null)
            {
                return NotFound();
            }

            return View(transaccion);
        }

        // POST: Transaccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transaccions == null)
            {
                return Problem("Entity set 'ContableContext.Transaccions'  is null.");
            }
            var transaccion = await _context.Transaccions.FindAsync(id);
            if (transaccion != null)
            {
                _context.Transaccions.Remove(transaccion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransaccionExists(int id)
        {
          return (_context.Transaccions?.Any(e => e.IdTransaccion == id)).GetValueOrDefault();
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
