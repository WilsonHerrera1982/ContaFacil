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
using ContaFacil.Models.ViewModel;
using Microsoft.Extensions.Options;
using ClosedXML.Excel;

namespace ContaFacil.Controllers
{
    public class ProductoController : NotificacionClass
    {
        private readonly ContableContext _context;

        public ProductoController(ContableContext context)
        {
            _context = context;
        }

        // GET: Producto
        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario=new Usuario();
            usuario= _context.Usuarios.Where(u=>u.IdUsuario==int.Parse(idUsuario)).Include(p=>p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor=new Emisor();
            emisor=_context.Emisors.Where(e=>e.Ruc==usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa=new Empresa();
            empresa = _context.Empresas.Where(e=>e.Identificacion==emisor.Ruc).FirstOrDefault();
            var contableContext = _context.Productos
      .Where(p => p.IdEmpresa == empresa.IdEmpresa)
      .Include(p => p.IdCategoriaProductoNavigation)
      .Include(p => p.IdEmpresaNavigation)
      .Include(p => p.IdUnidadMedidaNavigation)
      .Include(p => p.Inventarios)
      .Include(p=>p.IdImpuestoNavigation);

            foreach (var producto in contableContext)
            {
                var inventarioTask = Task.Run(() =>
                {
                    using (var newContext = new ContableContext())  // Crea un nuevo contexto
                    {
                        return newContext.Inventarios
                            .OrderByDescending(i => i.FechaCreacion)
                            .FirstOrDefault(i => i.IdProducto == producto.IdProducto);
                    }
                });

                var inventario = await inventarioTask;
                decimal porcentajeIva = ((producto.Porcentaje ?? 0) / 100m);
                decimal porcentajeUtilidad = ((producto.Utilidad ?? 0) / 100m);
                producto.Iva = porcentajeIva * producto.PrecioUnitario;
                producto.Comision = producto.PrecioUnitario *porcentajeUtilidad;
                producto.Stock = inventario.Stock;
            }

            return View(await contableContext.ToListAsync());
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaProductoNavigation)
                .Include(p => p.IdEmpresaNavigation)
                .Include(p => p.IdUnidadMedidaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario =  _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(empresa => empresa.Identificacion == emisor.Ruc).FirstOrDefault();
            ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos.Where(c=>c.IdEmpresa==empresa.IdEmpresa), "IdCategoriaProducto", "Nombre");
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre");
            ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "Nombre");
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Select(i => new
            {
                IdImpuesto = i.IdImpuesto,
                NombrePorcentaje = i.Nombre + " " + i.Porcentaje.ToString("F2") + "%"
            }), "IdImpuesto", "NombrePorcentaje");

            ViewData["IdProveedor"] = new SelectList(_context.Proveedors.Where(p => p.IdEmpresa == empresa.IdEmpresa), "IdProveedor", "Nombre");
            return View();
        }

        // POST: Producto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel producto)
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
                Empresa empresa=new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                Producto product = new Producto();
                product.Codigo = producto.Codigo;
                product.Nombre = producto.Nombre;
                product.Descripcion = producto.Descripcion;
                product.PrecioUnitario = producto.PrecioUnitario;
                product.IdCategoriaProducto = producto.IdCategoriaProducto;
                product.IdUnidadMedida = producto.IdUnidadMedida;
                product.Stock = producto.Stock;
                product.EstadoBoolean = true;
                product.FechaCreacion = new DateTime();
                product.UsuarioCreacion = int.Parse(idUsuario);
                product.IdEmpresa = empresa.IdEmpresa;
                product.IdImpuesto=producto.IdImpuesto;
                if (producto.Descuento == null)
                {
                    product.Descuento = 0;
                }
                _context.Productos.Add(product);
                 _context.SaveChanges();
                Inventario inventario = new Inventario();
                inventario.TipoMovimiento = "E";
                inventario.Cantidad = producto.Stock;
                inventario.FechaCreacion= new DateTime();
                inventario.UsuarioCreacion= int.Parse(idUsuario);
                inventario.IdProducto=product.IdProducto;
                inventario.NumeroDespacho = "E-000001";
                inventario.Stock=(int)producto.Stock;
                inventario.EstadoBoolean=true;
                inventario.Descripcion = "REGISTRO INICIAL DEL PRODUCTO";
                inventario.IdSucursal = usuarioSucursal.IdSucursal;
                _context.Inventarios.Add(inventario);
                await _context.SaveChangesAsync();
                SucursalInventario sucursalInventario = new SucursalInventario();
                sucursalInventario.IdInventario=inventario.IdInventario;
                sucursalInventario.IdSucursal = usuarioSucursal.IdSucursal;
                sucursalInventario.FechaCreacion= new DateTime();
                sucursalInventario.UsuarioCreacion=int.Parse(idUsuario);
                sucursalInventario.EstadoBoolean = true;
                _context.Add(sucursalInventario);
                await _context.SaveChangesAsync();
                ProductoProveedor productoProveedor = new ProductoProveedor();
                productoProveedor.IdProducto = product.IdProducto;
                productoProveedor.IdProveedor=producto.IdProveedor;
                productoProveedor.FechaCreacion = new DateTime();
                productoProveedor.UsuarioCreacion = int.Parse(idUsuario);
                productoProveedor.PrecioCompra=product.PrecioUnitario;
                productoProveedor.EstadoBoolean=true;
                _context.ProductoProveedors.Add(productoProveedor);
                await _context.SaveChangesAsync();
                ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos.Where(c => c.IdEmpresa == emisor.IdEmpresa), "IdCategoriaProducto", "Nombre");
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre");
                ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "Nombre");
                ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "Porcentaje");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "IdCategoriaProducto", producto.IdCategoriaProducto);
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", producto.IdEmpresa);
                ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "IdUnidadMedida", producto.IdUnidadMedida);
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(producto);
            }
            
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                Notificacion("Elemento no encontrado", NotificacionTipo.Warning);
                return NotFound();
            }

            var producto = _context.Productos.Include(p=>p.IdEmpresaNavigation).Include(p=>p.IdImpuestoNavigation).Where(p=>p.IdProducto==id).FirstOrDefault();
            if (producto == null)
            {
                Notificacion("Elemento no encontrado", NotificacionTipo.Warning);
                return NotFound();
            }
            ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "Nombre", producto.IdCategoriaProducto);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre", producto.IdEmpresa);
            ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "Nombre", producto.IdUnidadMedida);
            return View(producto);
        }

        // POST: Producto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            try
            {
                var productoExistente = await _context.Productos.FindAsync(id);
                if (productoExistente == null)
                {
                    return NotFound();
                }

                if (producto.Descuento > producto.Utilidad)
                {
                    Notificacion("El descuento es mayor que la utilidad", NotificacionTipo.Warning);
                    ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "IdCategoriaProducto", producto.IdCategoriaProducto);
                    ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", producto.IdEmpresa);
                    ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "IdUnidadMedida", producto.IdUnidadMedida);
                    return View(producto);
                }

                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                producto.FechaModificacion = DateTime.Now;
                producto.UsuarioModificacion = int.Parse(idUsuario);
                producto.IdEmpresa = productoExistente.IdEmpresa;
                producto.IdImpuesto= productoExistente.IdImpuesto;
                producto.FechaCreacion=productoExistente.FechaCreacion;
                producto.UsuarioCreacion= productoExistente.UsuarioCreacion;
                // Actualizar solo los campos modificados
                _context.Entry(productoExistente).CurrentValues.SetValues(producto);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
                Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                    ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "IdCategoriaProducto", producto.IdCategoriaProducto);
                    ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", producto.IdEmpresa);
                    ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "IdUnidadMedida", producto.IdUnidadMedida);
                    return View(producto);
               
            }
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaProductoNavigation)
                .Include(p => p.IdEmpresaNavigation)
                .Include(p => p.IdUnidadMedidaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'ContableContext.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        // GET: Producto/PrincipalProducto
        public IActionResult PrincipalProducto()
        {
            // Aquí puedes agregar cualquier lógica adicional que necesites antes de devolver la vista
            // Por ejemplo, podrías cargar algunos datos desde la base de datos y pasarlos a la vista

            return View(); // Esto devolverá la vista PrincipalProducto.cshtml
        }
        public async Task<IActionResult> ExportToExcel()
        {
            // Verificar sesión y obtener datos de la empresa
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            if (string.IsNullOrEmpty(idUsuario))
            {
                TempData["Error"] = "Usuario no autenticado.";
                return RedirectToAction(nameof(Index));
            }
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
            var usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            var sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == usuarioSucursal.IdSucursal);
            var persona = _context.Personas.FirstOrDefault(p => p.IdPersona == usuario.IdPersona);
            var emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == persona.Identificacion);
            var empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);

            var productos = _context.Productos
                .Where(p => p.IdEmpresa == empresa.IdEmpresa)
                .Include(p => p.IdCategoriaProductoNavigation)
                .Include(p => p.IdUnidadMedidaNavigation)
                .ToList();

            foreach (var producto in productos)
            {
                var inventarioTask = Task.Run(() =>
                {
                    using (var newContext = new ContableContext())  // Crea un nuevo contexto
                    {
                        return newContext.Inventarios
                            .OrderByDescending(i => i.FechaCreacion)
                            .FirstOrDefault(i => i.IdProducto == producto.IdProducto);
                    }
                });

                var inventario = await inventarioTask;
                decimal porcentajeIva = ((producto.Porcentaje ?? 0) / 100m);
                decimal porcentajeUtilidad = ((producto.Utilidad ?? 0) / 100m);
                producto.Iva = porcentajeIva * producto.PrecioUnitario;
                producto.Comision = producto.PrecioUnitario * porcentajeUtilidad;
                producto.Stock = inventario.Stock;
            }
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Productos");
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                if (System.IO.File.Exists(logoPath))
                {
                    var logo = worksheet.AddPicture(logoPath)
                        .MoveTo(worksheet.Cell("A1"))
                        .Scale(0.25);
                }
                else
                {
                    Console.WriteLine("El archivo de logo no existe en la ruta especificada.");
                }

                worksheet.Cell("D1").Value = "Reporte de Productos";
                worksheet.Cell("D2").Value = $"Nombre de cliente: {empresa.Nombre}";
                worksheet.Cell("D3").Value = $"RUC: {empresa.Identificacion}";
                worksheet.Cell("D4").Value = $"Sucursal: {sucursal.NombreSucursal}";
                worksheet.Cell("D5").Value = $"Fecha de corte: {DateTime.Now:dd/MM/yyyy}";
                var row = 9;
                // Add headers
                worksheet.Cell("A8").Value = "Código";
                worksheet.Cell("B8").Value = "Nombre";
                worksheet.Cell("C8").Value = "Descripción";
                worksheet.Cell("D8").Value = "Precio Unitario";
                worksheet.Cell("E8").Value = "Utilidad";
                worksheet.Cell("F8").Value = "Precio Venta";
                worksheet.Cell("G8").Value = "Descuento %";
                worksheet.Cell("H8").Value = "Stock";
                worksheet.Cell("I8").Value = "Categoría";
                worksheet.Cell("J8").Value = "Unidad de Medida";

                // Add data
               
                foreach (var producto in productos)
                {
                    worksheet.Cell($"A{row}").Value = producto.Codigo;
                    worksheet.Cell($"B{row}").Value = producto.Nombre;
                    worksheet.Cell($"C{row}").Value = producto.Descripcion;
                    worksheet.Cell($"D{row}").Value = producto.PrecioUnitario;
                    worksheet.Cell($"E{row}").Value = producto.Comision;
                    worksheet.Cell($"F{row}").Value = producto.PrecioVenta;
                    worksheet.Cell($"G{row}").Value = producto.Descuento;
                    worksheet.Cell($"H{row}").Value = producto.Stock;
                    worksheet.Cell($"I{row}").Value = producto.IdCategoriaProductoNavigation?.Descripcion;
                    worksheet.Cell($"J{row}").Value = producto.IdUnidadMedidaNavigation?.Abreviatura;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteProductos.xlsx");
                }
            }
        }
    }
}
