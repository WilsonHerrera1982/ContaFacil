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
    public class CuentasPorPagarController : NotificacionClass
    {
        private readonly ContableContext _context;

        public CuentasPorPagarController(ContableContext context)
        {
            _context = context;
        }

        // GET: CuentasPorPagar
        public async Task<IActionResult> Index()
        {
            return View(await _context.CuentasPorPagars.ToListAsync());
        }

        // GET: CuentasPorPagar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentasPorPagar = await _context.CuentasPorPagars
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (cuentasPorPagar == null)
            {
                return NotFound();
            }

            return View(cuentasPorPagar);
        }

        // GET: CuentasPorPagar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CuentasPorPagar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuentasPorPagar cuentasPorPagar)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
                cuentasPorPagar.UsuarioCreacion=usuario.IdUsuario.ToString();
                cuentasPorPagar.FechaCreacion = DateTime.Now;
                _context.Add(cuentasPorPagar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(cuentasPorPagar);
            }
        }

        // GET: CuentasPorPagar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentasPorPagar = await _context.CuentasPorPagars.FindAsync(id);
            if (cuentasPorPagar == null)
            {
                return NotFound();
            }
            return View(cuentasPorPagar);
        }

        // POST: CuentasPorPagar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCuenta,TipoDocumento,TipoProveedor,RazonSocial,RucIdentificacion,NumeroDocumento,FechaEmision,FechaVencimiento,MontoTotal,MontoPendiente,Impuestos,Descuentos,FormaPago,NumeroReferencia,Categoria,Descripcion,Estado,UsuarioCreacion,FechaCreacion,UsuarioModificacion,FechaModificacion,CentroCosto,Proyecto")] CuentasPorPagar cuentasPorPagar)
        {
            if (id != cuentasPorPagar.IdCuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuentasPorPagar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentasPorPagarExists(cuentasPorPagar.IdCuenta))
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
            return View(cuentasPorPagar);
        }

        // GET: CuentasPorPagar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentasPorPagar = await _context.CuentasPorPagars
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (cuentasPorPagar == null)
            {
                return NotFound();
            }

            return View(cuentasPorPagar);
        }

        // POST: CuentasPorPagar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cuentasPorPagar = await _context.CuentasPorPagars.FindAsync(id);
            if (cuentasPorPagar != null)
            {
                _context.CuentasPorPagars.Remove(cuentasPorPagar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuentasPorPagarExists(int id)
        {
            return _context.CuentasPorPagars.Any(e => e.IdCuenta == id);
        }
    }
}
