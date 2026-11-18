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
            // Obtener el id de empresa del usuario logueado
            string idEmpresaStr = HttpContext.Session.GetString("_empresa");
            
            if (string.IsNullOrEmpty(idEmpresaStr))
            {
                // Si no hay empresa en sesión, redirigir al login o mostrar error
                return RedirectToAction("Login", "Account");
            }

            int idEmpresa = int.Parse(idEmpresaStr);

            // Obtener las cuentas asociadas a la empresa usando la tabla intermedia cuenta_empresa
            var cuentas = await _context.CuentaEmpresas
                .Where(ce => ce.IdEmpresa == idEmpresa && ce.Estado == true)
                .Include(ce => ce.IdCuentaNavigation)
                    .ThenInclude(c => c.IdTipoCuentaNavigation)
                .Select(ce => ce.IdCuentaNavigation)
                .Where(c => c.Estado == true)
                .ToListAsync();

            // Ordenar numéricamente por segmentos del código (ej: 6.1.10 viene después de 6.1.2)
            cuentas = cuentas.OrderBy(c => OrdenarCodigoJerarquico(c.Codigo)).ToList();

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
                string idEmpresaStr = HttpContext.Session.GetString("_empresa");
                
                if (string.IsNullOrEmpty(idEmpresaStr))
                {
                    Notificacion("No se pudo obtener la empresa del usuario", NotificacionTipo.Error);
                    return View();
                }

                int idEmpresaUsuario = int.Parse(idEmpresaStr);
                Tipocuentum tipocuentum = await _context.Tipocuenta.FirstOrDefaultAsync(m => m.Nombre == "PASIVOS");
                
                cuentum.FechaCreacion = DateTime.Now;
                cuentum.Estado = true;
                cuentum.SaldoInicial = 0;
                cuentum.SaldoActual = 0;
                cuentum.IdTipoCuenta = tipocuentum.IdTipoCuenta;
                cuentum.IdEmpresa = 18; // Valor por defecto - Empresa maestra
                cuentum.UsuarioCreacion = int.Parse(idUsuario);
                
                if (cuentum.Tipo == null)
                {
                    cuentum.Tipo = " ";
                }
                
                // Guardar la cuenta con id_empresa = 18 (cuenta maestra)
                _context.Add(cuentum);
                await _context.SaveChangesAsync();

                // Crear la relación en cuenta_empresa con la empresa real del usuario
                var cuentaEmpresa = new CuentaEmpresa
                {
                    IdCuenta = cuentum.IdCuenta,
                    IdEmpresa = idEmpresaUsuario, // Empresa real del usuario logueado
                    SaldoInicial = 0,
                    SaldoActual = 0,
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    UsuarioCreacion = int.Parse(idUsuario)
                };

                _context.CuentaEmpresas.Add(cuentaEmpresa);
                await _context.SaveChangesAsync();

                Notificacion("Registro guardado correctamente", NotificacionTipo.Success);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Notificacion($"Error al guardar el registro: {e.Message}", NotificacionTipo.Error);
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

            // Obtener el estado desde cuenta_empresa para la empresa del usuario
            string idEmpresaStr = HttpContext.Session.GetString("_empresa");
            
            if (!string.IsNullOrEmpty(idEmpresaStr))
            {
                int idEmpresaUsuario = int.Parse(idEmpresaStr);
                
                // Buscar el estado en cuenta_empresa para esta empresa
                var cuentaEmpresa = await _context.CuentaEmpresas
                    .FirstOrDefaultAsync(ce => ce.IdCuenta == id && ce.IdEmpresa == idEmpresaUsuario);
                
                if (cuentaEmpresa != null)
                {
                    // Sobrescribir el estado con el de cuenta_empresa
                    cuentum.Estado = cuentaEmpresa.Estado;
                }
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
        public async Task<IActionResult> Edit(int id, Cuentum cuentum)
        {
            if (id != cuentum.IdCuenta)
            {
                return NotFound();
            }

            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresaStr = HttpContext.Session.GetString("_empresa");

            if (string.IsNullOrEmpty(idEmpresaStr))
            {
                Notificacion("No se pudo obtener la empresa del usuario", NotificacionTipo.Error);
                return View(cuentum);
            }

            int idEmpresaUsuario = int.Parse(idEmpresaStr);
            
            try
            {
                // Actualizar la cuenta maestra (nombre, tipo, débito, crédito)
                Cuentum cue = _context.Cuenta.FirstOrDefault(c => c.IdCuenta == id);
                
                if (cue == null)
                {
                    Notificacion("Cuenta no encontrada", NotificacionTipo.Error);
                    return NotFound();
                }

                if (cue.IdTipoCuenta == 0)
                {
                    Tipocuentum tipocuentum = await _context.Tipocuenta.FirstOrDefaultAsync(m => m.Nombre == "PASIVOS");
                    cue.IdTipoCuenta = tipocuentum.IdTipoCuenta;
                }

                if (cuentum.Tipo == null)
                {
                    cuentum.Tipo = " ";
                }

                // Actualizar campos de la cuenta maestra (NO el estado)
                cue.Credito = cuentum.Credito;
                cue.Debito = cuentum.Debito;
                cue.Nombre = cuentum.Nombre;
                cue.Tipo = cuentum.Tipo;
                cue.UsuarioModificacion = int.Parse(idUsuario);
                cue.FechaModificacion = DateTime.Now;

                _context.Update(cue);

                // Actualizar el ESTADO en cuenta_empresa para la empresa del usuario
                var cuentaEmpresa = await _context.CuentaEmpresas
                    .FirstOrDefaultAsync(ce => ce.IdCuenta == id && ce.IdEmpresa == idEmpresaUsuario);

                if (cuentaEmpresa != null)
                {
                    // Actualizar estado en cuenta_empresa
                    cuentaEmpresa.Estado = cuentum.Estado ?? false;
                    cuentaEmpresa.FechaModificacion = DateTime.Now;
                    cuentaEmpresa.UsuarioModificacion = int.Parse(idUsuario);
                    
                    _context.Update(cuentaEmpresa);
                }
                else
                {
                    // Si no existe la relación, crearla
                    cuentaEmpresa = new CuentaEmpresa
                    {
                        IdCuenta = id,
                        IdEmpresa = idEmpresaUsuario,
                        SaldoInicial = 0,
                        SaldoActual = 0,
                        Estado = cuentum.Estado ?? false,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = int.Parse(idUsuario)
                    };
                    
                    _context.CuentaEmpresas.Add(cuentaEmpresa);
                }

                await _context.SaveChangesAsync();

                Notificacion("Registro actualizado correctamente", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Notificacion($"Error al actualizar el registro: {ex.Message}", NotificacionTipo.Error);
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cuentum.IdEmpresa);
                ViewData["IdTipoCuenta"] = new SelectList(_context.Tipocuenta, "IdTipoCuenta", "IdTipoCuenta", cuentum.IdTipoCuenta);
                return View(cuentum);
            }
            catch (Exception ex)
            {
                Notificacion($"Error inesperado: {ex.Message}", NotificacionTipo.Error);
                return View(cuentum);
            }
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

        /// <summary>
        /// Convierte un código jerárquico en un string ordenable numéricamente.
        /// Ejemplo: "6.1.10" → "000006.000001.000010"
        /// Esto permite ordenar correctamente: 6.1.1, 6.1.2, ..., 6.1.9, 6.1.10
        /// </summary>
        private string OrdenarCodigoJerarquico(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return string.Empty;

            try
            {
                // Dividir el código por puntos
                var partes = codigo.Split('.');
                
                // Convertir cada parte a número con padding (6 dígitos)
                var partesOrdenadas = partes.Select(p => 
                {
                    if (int.TryParse(p, out int numero))
                    {
                        return numero.ToString("D6"); // Padding de 6 dígitos: "1" → "000001"
                    }
                    return p.PadLeft(6, '0'); // Si no es número, padding con ceros
                });
                
                // Unir con puntos: "6.1.10" → "000006.000001.000010"
                return string.Join(".", partesOrdenadas);
            }
            catch
            {
                // Si hay error, devolver el código original
                return codigo;
            }
        }
      
public IActionResult ExportToExcel()
    {
        // Obtener el id de empresa del usuario logueado
        string idEmpresaStr = HttpContext.Session.GetString("_empresa");
        
        if (string.IsNullOrEmpty(idEmpresaStr))
        {
            return RedirectToAction("Login", "Account");
        }

        int idEmpresa = int.Parse(idEmpresaStr);

        // Obtener cuentas de la empresa usando cuenta_empresa
        var cuentas = _context.CuentaEmpresas
            .Where(ce => ce.IdEmpresa == idEmpresa && ce.Estado == true)
            .Include(ce => ce.IdCuentaNavigation)
            .Select(ce => ce.IdCuentaNavigation)
            .Where(c => c.Estado == true)
            .ToList();

        // Ordenar numéricamente por segmentos del código
        cuentas = cuentas.OrderBy(c => OrdenarCodigoJerarquico(c.Codigo)).ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Plan de Cuentas");

            // Añadir encabezados
            worksheet.Cell(1, 1).Value = "Código";
            worksheet.Cell(1, 2).Value = "Cuenta";
                worksheet.Cell(1, 3).Value = "Tipo";
                worksheet.Cell(1, 4).Value = "Debito";
                worksheet.Cell(1, 5).Value = "Crédito";
                int row = 2;
            foreach (var cuenta in cuentas)
            {
                worksheet.Cell(row, 1).Value = cuenta.Codigo;
                worksheet.Cell(row, 2).Value = new String(' ', (cuenta.Codigo.Split('.').Length - 1) * 2) + cuenta.Nombre;
                    worksheet.Cell(row, 3).Value = cuenta.Tipo;
                    worksheet.Cell(row, 4).Value = cuenta.Debito.HasValue && cuenta.Debito.Value ? 1 : 0;
                    worksheet.Cell(row, 5).Value = cuenta.Credito.HasValue && cuenta.Credito.Value ? 1 : 0;
                    row++;
            }

            // Ajustar el ancho de las columnas
            worksheet.Column(1).Width = 15;
            worksheet.Column(2).Width = 50;
                worksheet.Column(3).Width = 50;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 15;

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
