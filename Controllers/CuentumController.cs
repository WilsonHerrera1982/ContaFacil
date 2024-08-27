using System;
using System.Collections.Generic;  
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Models.ViewModel;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using ContaFacil.Logica;

namespace ContaFacil.Controllers
{
    public class CuentumController : NotificacionClass
    {
        private readonly ContableContext _context;

        public CuentumController(ContableContext context)
        {
            _context = context;
        }

        // GET: Cuentum
        public async Task<IActionResult> Index()
        {
            var cuentas = await _context.Cuenta
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdTipoCuentaNavigation).OrderBy(c=>c.Codigo)
                .ToListAsync();

            var viewModel = new CuentumIndexViewModel
            {
                Cuentas = cuentas
            };
            return View(viewModel);
        }

        // GET: Cuentum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cuenta == null)
            {
                return NotFound();
            }

            var cuentum = await _context.Cuenta
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdTipoCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (cuentum == null)
            {
                return NotFound();
            }

            return View(cuentum);
        }

        // GET: Cuentum/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta");
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([FromForm] Cuentum cuentum)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                Empresa empresa = _context.Empresas.FirstOrDefault(e=>e.Identificacion=="0401253950001");
                Tipocuentum tipocuentum = await _context.Tipocuenta.FirstOrDefaultAsync(m => m.Nombre == "PASIVOS");
                cuentum.FechaCreacion = DateTime.Now;
                cuentum.Estado = true;
                cuentum.SaldoInicial = 0;
                cuentum.SaldoActual = 0;
                cuentum.IdTipoCuenta = tipocuentum.IdTipoCuenta; // Set appropriate default value
                cuentum.IdEmpresa = empresa.IdEmpresa; // Set appropriate default value
                cuentum.UsuarioCreacion = int.Parse(idUsuario); // Set appropriate default value
                cuentum.FechaCreacion = new DateTime();
                _context.Add(cuentum);
                await _context.SaveChangesAsync();
                Notificacion("Registro guardado correctamente",NotificacionTipo.Success);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Notificacion("Error al guardar el registro", NotificacionTipo.Error);
                return View();
            }
            
        }

        // GET: Cuentum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cuenta == null)
            {
                return NotFound();
            }

            var cuentum = await _context.Cuenta.FindAsync(id);
            if (cuentum == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentum.IdEmpresa);
            ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta", cuentum.IdTipoCuenta);
            return View(cuentum);
        }

        // POST: Cuentum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCuenta,Nombre,IdTipoCuenta,SaldoInicial,SaldoActual,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Cuentum cuentum)
        {
            if (id != cuentum.IdCuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuentum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentumExists(cuentum.IdCuenta))
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
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentum.IdEmpresa);
            ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta", cuentum.IdTipoCuenta);
            return View(cuentum);
        }

        // GET: Cuentum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cuenta == null)
            {
                return NotFound();
            }

            var cuentum = await _context.Cuenta
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdTipoCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (cuentum == null)
            {
                return NotFound();
            }

            return View(cuentum);
        }

        // POST: Cuentum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cuenta == null)
            {
                return Problem("Entity set 'ContableContext.Cuenta'  is null.");
            }
            var cuentum = await _context.Cuenta.FindAsync(id);
            if (cuentum != null)
            {
                _context.Cuenta.Remove(cuentum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuentumExists(int id)
        {
          return (_context.Cuenta?.Any(e => e.IdCuenta == id)).GetValueOrDefault();
        }
      
public IActionResult ExportToExcel()
    {
        var cuentas = _context.Cuenta.OrderBy(c => c.Codigo).ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Plan de Cuentas");

            // Añadir encabezados
            worksheet.Cell(1, 1).Value = "Código";
            worksheet.Cell(1, 2).Value = "Cuenta";

            int row = 2;
            foreach (var cuenta in cuentas)
            {
                worksheet.Cell(row, 1).Value = cuenta.Codigo;
                worksheet.Cell(row, 2).Value = new String(' ', (cuenta.Codigo.Split('.').Length - 1) * 2) + cuenta.Nombre;
                row++;
            }

            // Ajustar el ancho de las columnas
            worksheet.Column(1).Width = 15;
            worksheet.Column(2).Width = 50;

            // Estilo para los encabezados
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "plan_de_cuentas.xlsx");
            }
        }
    }
}
}
