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
using Irony.Parsing;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace ContaFacil.Controllers
{
    public class CategoriaProductoController : NotificacionClass
    {
        private readonly ContableContext _context;
        private readonly ICompositeViewEngine _viewEngine;

        public CategoriaProductoController(ContableContext context, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _viewEngine = viewEngine;
        }

        // GET: CategoriaProducto
        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
            UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            return _context.CategoriaProductos != null ? 
                          View(await _context.CategoriaProductos.Where(p=>p.IdEmpresa==empresa.IdEmpresa && p.EstadoBoolean==true).ToListAsync()) :
                          Problem("Entity set 'ContableContext.CategoriaProductos'  is null.");
        }

        // GET: ExportarExcel
        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            var categorias = await _context.CategoriaProductos.Where(p => p.IdEmpresa == empresa.IdEmpresa).ToListAsync();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Categorías de Producto");
                worksheet.Cell("A1").Value = "Reporte de Categorías de Producto";
                worksheet.Cell("A2").Value = $"Empresa: {empresa.Nombre}";
                worksheet.Cell("A3").Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Range("A1:C1").Merge().Style.Font.Bold = true;
                worksheet.Range("A2:C2").Merge();
                worksheet.Range("A3:C3").Merge();
                worksheet.Cell("A5").Value = "Nombre";
                worksheet.Cell("B5").Value = "Descripción";
                worksheet.Cell("C5").Value = "Estado";
                worksheet.Range("A5:C5").Style.Font.Bold = true;
                int row = 6;
                foreach (var cat in categorias)
                {
                    worksheet.Cell(row, 1).Value = cat.Nombre;
                    worksheet.Cell(row, 2).Value = cat.Descripcion;
                    worksheet.Cell(row, 3).Value = cat.EstadoBoolean ? "Sí" : "No";
                    row++;
                }
                worksheet.Columns().AdjustToContents();
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CategoriasProducto.xlsx");
                }
            }
        }

        // GET: CategoriaProducto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos
                .FirstOrDefaultAsync(m => m.IdCategoriaProducto == id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }

            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Create
        public IActionResult Create()
        {
            return View();
        }

        // Devuelve el HTML <option> de las categorías para el dropdown
        [HttpGet]
        public IActionResult GetCategoriasSelect()
        {
            var categorias = _context.CategoriaProductos.OrderBy(c => c.Nombre).ToList();
            var options = "";
            foreach (var cat in categorias)
            {
                options += $"<option value='{cat.IdCategoriaProducto}'>{cat.Nombre}</option>";
            }
            return Content(options, "text/html");
        }

        // POST: CategoriaProducto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaProducto categoriaProducto)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
                UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
                usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                CategoriaProducto categoria = _context.CategoriaProductos.Where(c => c.Nombre.Equals(categoriaProducto.Nombre)&& c.EstadoBoolean==true).FirstOrDefault();
                if (categoria != null)
                {
                    Notificacion("Error al guardar el Registro la categoría ya existe" , NotificacionTipo.Error);
                    System.Diagnostics.Debug.WriteLine($"[CategoriaProductoController] X-Requested-With (EXISTE): {Request.Headers["X-Requested-With"]}");
                    if (Request.Headers["X-Requested-With"].ToString().Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine($"[CategoriaProductoController] X-Requested-With (EXISTE): {Request.Headers["X-Requested-With"]}");
                        var html = RenderPartialViewToString("Create", categoriaProducto);
                        return Json(new { success = false, html });
                    }
                    return View(categoriaProducto);
                }
                categoriaProducto.UsuarioCreacion = int.Parse(idUsuario);
                categoriaProducto.FechaCreacion = new DateTime();
                categoriaProducto.IdEmpresa = empresa.IdEmpresa;
                categoriaProducto.EstadoBoolean = true;
                _context.Add(categoriaProducto);
                Cuentum cuentum = new Cuentum();
                cuentum = _context.Cuenta.Where(c => c.Nombre.Equals("Inventario")).FirstOrDefault();
                // Extraer el prefijo para esta subcuenta basado en la categoría padre
                // Para subcuentas, siempre debe usar el código completo del padre como prefijo
                string prefijoPadre = cuentum.Codigo;
                var subcuentasExistentes = _context.Cuenta
                            .Where(c => c.IdIdCuenta == cuentum.IdCuenta)
                            .OrderBy(c => c.Codigo)
                            .ToList();
                // Buscar todas las categorías de productos (hijos directos de Inventario)
                var categoriasInventario = _context.Cuenta
                    .Where(c => c.IdIdCuenta == cuentum.IdCuenta)
                    .OrderBy(c => c.Codigo)
                    .ToList();

                string codigo = "";

                if (categoriasInventario.Any())
                {
                    // Verificar la secuencia de los códigos de categoría
                    var segmentosCategoria = categoriasInventario
                        .Select(c => {
                            // Extraer el último número del código (e.j., "3" de "1.1.3.2")
                            var partes = c.Codigo.Split('.');
                            if (partes.Length >= 4) // Asegurarse de que tiene suficientes partes
                            {
                                int.TryParse(partes[3], out int num);
                                return num;
                            }
                            return 0;
                        })
                        .Where(n => n > 0)
                        .OrderBy(n => n)
                        .ToList();

                    // Encontrar el primer número que falta en la secuencia
                    int siguienteNumero = 1;
                    for (int i = 0; i < segmentosCategoria.Count; i++)
                    {
                        if (segmentosCategoria[i] != i + 1)
                        {
                            siguienteNumero = i + 1;
                            break;
                        }
                        siguienteNumero = i + 2; // El siguiente después del último
                    }

                    codigo = $"1.1.3.{siguienteNumero}";
                }
                else
                {
                    codigo = "1.1.3.1"; // Primera categoría
                }
                Tipocuentum tipocuentum = _context.Tipocuenta.FirstOrDefault(c => c.Nombre == "ACTIVOS");
                int idCuenta = 0;
                int idSubcuenta = 0;
                Cuentum cuentaCat = new Cuentum();
                cuentaCat = _context.Cuenta.FirstOrDefault(c => c.Nombre == categoriaProducto.Nombre);
                if (cuentaCat == null)
                {
                    cuentaCat = new Cuentum();
                    cuentaCat.Nombre = categoriaProducto.Nombre;
                    cuentaCat.Codigo = codigo;
                    cuentaCat.IdIdCuenta = cuentum.IdCuenta;
                    cuentaCat.FechaCreacion = new DateTime();
                    cuentaCat.UsuarioCreacion = int.Parse(idUsuario);
                    cuentaCat.IdEmpresa = empresa.IdEmpresa;
                    cuentaCat.IdTipoCuenta = tipocuentum.IdTipoCuenta;
                    cuentaCat.Debito = true;
                    cuentaCat.Credito = false;
                    cuentaCat.Tipo = "Producto";
                    _context.Add(cuentaCat);
                    _context.SaveChanges();
                    idCuenta = cuentaCat.IdCuenta;
                }

                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);

                if (Request.Headers["X-Requested-With"].ToString().Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = true, message = "Registro guardado con éxito" });
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                if (Request.Headers["X-Requested-With"].ToString().Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
                {
                    var html = RenderPartialViewToString("Create", categoriaProducto);
                    return Json(new { success = false, html });
                }
                return View(categoriaProducto);
            }
        }

        // GET: CategoriaProducto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos.FindAsync(id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }
            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoriaProducto,Nombre,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] CategoriaProducto categoriaProducto)
        {
            if (id != categoriaProducto.IdCategoriaProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    categoriaProducto.UsuarioModificacion = int.Parse(idUsuario);
                    categoriaProducto.FechaModificacion = new DateTime();
                    _context.Update(categoriaProducto);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CategoriaProductoExists(categoriaProducto.IdCategoriaProducto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        Notificacion("Error al actualizar el Registro" + ex.Message, NotificacionTipo.Error);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoriaProductos == null)
            {
                return NotFound();
            }

            var categoriaProducto = await _context.CategoriaProductos
                .FirstOrDefaultAsync(m => m.IdCategoriaProducto == id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }

            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoriaProductos == null)
            {
                return Problem("Entity set 'ContableContext.CategoriaProductos'  is null.");
            }
            var categoriaProducto = await _context.CategoriaProductos.FindAsync(id);
            if (categoriaProducto != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                categoriaProducto.UsuarioModificacion = int.Parse(idUsuario);
                categoriaProducto.FechaModificacion = new DateTime();
                categoriaProducto.EstadoBoolean = false;
                _context.CategoriaProductos.Update(categoriaProducto);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaProductoExists(int id)
        {
          return (_context.CategoriaProductos?.Any(e => e.IdCategoriaProducto == id)).GetValueOrDefault();
        }

        // Método auxiliar para renderizar un PartialView a string
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext).Wait();
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
