using ContaFacil.Logica;
using ContaFacil.Models;
using ContaFacil.Models.Dto;
using ContaFacil.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using ClosedXML.Excel;
namespace ContaFacil.Controllers
{
    public class InventarioController : NotificacionClass
    {
        private readonly ContableContext _context;

        public InventarioController(ContableContext context)
        {
            _context = context;
        }

        // GET: Inventario
        public async Task<IActionResult> Index(int? idSucursal, DateTime? startDate, DateTime? endDate)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            UsuarioSucursal usuarioSucursal= new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(s => s.IdUsuario == usuario.IdUsuario).FirstOrDefault();

            var query = _context.Inventarios
             .Include(i => i.IdProductoNavigation)
             .Include(i => i.SucursalInventarios)
             .Where(i => i.SucursalInventarios.Any(s => s.IdSucursal == usuarioSucursal.IdSucursal))
             .AsQueryable();

            if (idSucursal.HasValue)
            {
                query = query.Where(i => i.SucursalInventarios.Any(si => si.IdSucursal == idSucursal.Value));
            }

            if (startDate.HasValue)
            {
                query = query.Where(i => i.FechaMovimiento >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.FechaMovimiento <= endDate.Value);
            }

            var inventarios = await query.OrderByDescending(i => i.FechaMovimiento).ToListAsync();

            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            // Asegúrese de que esto esté configurado correctamente
            ViewBag.IdSucursal = new SelectList(_context.Sucursals, "IdSucursal", "NombreSucursal", idSucursal);
            ViewBag.SelectedSucursal = idSucursal;

            return View(inventarios);
        }

        // GET: Inventario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // GET: Inventario/Create
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
            usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
            Despacho despacho = new Despacho();
            despacho = _context.Despachos.Where(e => e.IdSucursal==usuarioSucursal.IdSucursal).FirstOrDefault();
            ViewData["IdProducto"] = new SelectList(_context.Productos.Where(e=>e.IdEmpresa==empresa.IdEmpresa), "IdProducto", "Nombre");
            ViewData["IdSucursal"] = new SelectList(_context.Sucursals.Where(s => s.IdEmisor == emisor.IdEmisor & !s.NombreSucursal.Equals("Sucursal Principal")), "IdSucursal", "NombreSucursal");
            return View();
        }

        [HttpGet]
        public IActionResult ObtenerNumeroDespacho(string tipoMovimiento)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.Where(s => s.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
            var ultimoMovimiento = _context.Inventarios
                .Where(i => i.TipoMovimiento == tipoMovimiento & i.IdSucursal==usuarioSucursal.IdSucursal)
                .OrderByDescending(i => i.FechaCreacion)
                .FirstOrDefault();

            string nuevoNumeroDespacho;

            if (ultimoMovimiento != null && !string.IsNullOrEmpty(ultimoMovimiento.NumeroDespacho))
            {
                int ultimoNumero = int.Parse(ultimoMovimiento.NumeroDespacho.Split('-')[1]);
                nuevoNumeroDespacho = $"{tipoMovimiento}-{(ultimoNumero + 1).ToString("D6")}";
            }
            else
            {
                nuevoNumeroDespacho = $"{tipoMovimiento}-000001";
            }

            return Content(nuevoNumeroDespacho);
        }

        // POST: Inventario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventarioViewModel inventario)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Despacho despacho = new Despacho();
                UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
                usuarioSucursal = _context.UsuarioSucursals.Where(s => s.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
                SucursalInventario  sucursalInventario = new SucursalInventario();
                Producto producto = new Producto();
                producto=_context.Productos.Where(p => p.IdProducto==inventario.idProducto).FirstOrDefault();
                Cuentum cuentum = new Cuentum();
                cuentum = _context.Cuenta.Where(c => c.Nombre.Equals("Inventario")).FirstOrDefault();
                Inventario ultimoMovimiento=new Inventario();
                if (inventario.tipoMovimiento.Equals("T"))
                {
                            ultimoMovimiento = _context.Inventarios
                       .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T") & i.IdProducto == inventario.idProducto & i.IdSucursal == usuarioSucursal.IdSucursal)
                       .OrderByDescending(i => i.FechaCreacion)
                       .FirstOrDefault();
                }
                if (inventario.tipoMovimiento.Equals("S"))
                {
                    ultimoMovimiento = _context.Inventarios
                     .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T") & i.IdProducto == inventario.idProducto & i.IdSucursal == usuarioSucursal.IdSucursal)
                     .OrderByDescending(i => i.FechaCreacion)
                     .FirstOrDefault();
                }
                else if(inventario.tipoMovimiento.Equals("E"))
                {
                    ultimoMovimiento = _context.Inventarios
                     .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T") & i.IdProducto == inventario.idProducto & i.IdSucursal == usuarioSucursal.IdSucursal)
                     .OrderByDescending(i => i.FechaCreacion)
                     .FirstOrDefault();
                }
                Inventario inv = new Inventario();
                if (inventario.tipoMovimiento.Equals("E"))
                {
                    inv.IdProducto= inventario.idProducto;
                    inv.TipoMovimiento = inventario.tipoMovimiento;
                    inv.NumeroDespacho= inventario.numeroDespacho;
                    inv.EstadoBoolean = true;
                    inv.UsuarioCreacion = int.Parse(idUsuario);
                    inv.FechaCreacion = new DateTime();
                    inv.Cantidad= inventario.cantidad;
                    inv.Descripcion = "ENTRADA";
                    inv.IdSucursal=usuarioSucursal.IdSucursal;
                    inv.IdCuentaContable = cuentum.IdCuenta;
                    if (ultimoMovimiento != null)
                    {
                        inv.Stock = ultimoMovimiento.Stock + inventario.cantidad;
                    }
                    else
                    {
                        inv.Stock = 0 + inventario.cantidad;
                    }
                    _context.Add(inv);
                    await _context.SaveChangesAsync();
                    producto.Stock= ultimoMovimiento.Stock+inventario.cantidad;
                    sucursalInventario.EstadoBoolean = true;
                    sucursalInventario.IdInventario = inv.IdInventario;
                    sucursalInventario.IdSucursal = usuarioSucursal.IdSucursal;
                    sucursalInventario.FechaCreacion= new DateTime();
                    sucursalInventario.UsuarioCreacion= int.Parse(idUsuario);
                    _context.Add(sucursalInventario);
                    await _context.SaveChangesAsync();                   
                }
                else if(inventario.tipoMovimiento.Equals("S") && ultimoMovimiento.Stock>=0 && ultimoMovimiento.Stock>inventario.cantidad)
                {
                    inv.IdProducto = inventario.idProducto;
                    inv.TipoMovimiento = inventario.tipoMovimiento;
                    inv.NumeroDespacho = inventario.numeroDespacho;
                    inv.EstadoBoolean = true;
                    inv.UsuarioCreacion = int.Parse(idUsuario);
                    inv.FechaCreacion = new DateTime();
                    inv.Stock = ultimoMovimiento.Stock - inventario.cantidad;
                    inv.Cantidad = inventario.cantidad;
                    inv.Descripcion = "SALIDA";
                    inv.IdSucursal = usuarioSucursal.IdSucursal;
                    inv.IdCuentaContable = cuentum.IdCuenta;
                    _context.Add(inv);
                    await _context.SaveChangesAsync();
                    //producto.Stock = producto.Stock - inventario.cantidad;
                    sucursalInventario.EstadoBoolean = true;
                    sucursalInventario.IdInventario = inv.IdInventario;
                    sucursalInventario.IdSucursal = usuarioSucursal.IdSucursal;
                    sucursalInventario.FechaCreacion = new DateTime();
                    sucursalInventario.UsuarioCreacion = int.Parse(idUsuario);
                    _context.Add(sucursalInventario);
                    await _context.SaveChangesAsync();
                }
                else if (inventario.tipoMovimiento.Equals("T") && ultimoMovimiento.Stock >= 0 && ultimoMovimiento.Stock > inventario.cantidad)
                {
                    inv.IdProducto = inventario.idProducto;
                    inv.TipoMovimiento = inventario.tipoMovimiento;
                    inv.NumeroDespacho = inventario.numeroDespacho;
                    inv.EstadoBoolean = true;
                    inv.UsuarioCreacion = int.Parse(idUsuario);
                    inv.FechaCreacion = new DateTime();
                    inv.Cantidad=inventario.cantidad;
                    inv.Descripcion = "TRANSFERENCIA";
                    inv.IdSucursal = usuarioSucursal.IdSucursal;
                    inv.IdCuentaContable = cuentum.IdCuenta;
                    if (ultimoMovimiento != null)
                    {
                        int nuevoStock = ultimoMovimiento.Stock - inventario.cantidad??0;
                        inv.Stock =nuevoStock;
                    }
                    else
                    {
                        inv.Stock = inventario.cantidad;
                    }
                    
                    _context.Add(inv);
                    await _context.SaveChangesAsync();
                   // producto.Stock = producto.Stock - inventario.cantidad;
                    sucursalInventario.EstadoBoolean = true;
                    sucursalInventario.IdInventario = inv.IdInventario;
                    sucursalInventario.IdSucursal = usuarioSucursal.IdSucursal;
                    sucursalInventario.FechaCreacion = new DateTime();
                    sucursalInventario.UsuarioCreacion = int.Parse(idUsuario);
                    _context.Add(sucursalInventario);
                    await _context.SaveChangesAsync();
                    despacho.EstadoBoolean = true;
                    despacho.IdSucursal = usuarioSucursal.IdSucursal;
                    despacho.IdSucursalDestino=inventario.sucursalDestino;
                    despacho.IdUsuario=int.Parse(idUsuario);
                    despacho.IdEmpresa=int.Parse(idEmpresa);
                    despacho.EstadoDespacho = "PENDIENTE";
                    despacho.EstadoBoolean=true;
                    despacho.NumeroDespacho=inventario.numeroDespacho;
                    despacho.UsuarioCreacion = int.Parse(idUsuario);
                    despacho.FechaCreacion=new DateTime();  
                    _context.Add(despacho);
                    await _context.SaveChangesAsync();
                    DetalleDespacho detalleDespacho= new DetalleDespacho();
                    //detalleDespacho.id
                }
                else
                {
                    Notificacion("Revisar la cantidad del despacho", NotificacionTipo.Warning);
                    return RedirectToAction(nameof(Index));
                }
                _context.Update(producto);
                await _context.SaveChangesAsync();                
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.idProducto);
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(inventario);
            }
        }

        // GET: Inventario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.IdProducto);
            return View(inventario);
        }

        // POST: Inventario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInventario,IdProducto,TipoMovimiento,Cantidad,FechaMovimiento,NumeroDespacho,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Inventario inventario)
        {
            if (id != inventario.IdInventario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    inventario.UsuarioModificacion = int.Parse(idUsuario);
                    inventario.FechaModificacion = new DateTime();
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!InventarioExists(inventario.IdInventario))
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
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.IdProducto);
            return View(inventario);
        }

        // GET: Inventario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventarios == null)
            {
                return Problem("Entity set 'ContableContext.Inventarios'  is null.");
            }
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                inventario.UsuarioModificacion = int.Parse(idUsuario);
                inventario.FechaModificacion = new DateTime();
                inventario.EstadoBoolean = false;
                _context.Inventarios.Update(inventario);

                Producto producto = new Producto();
                producto = _context.Productos.Where(p => p.IdProducto == inventario.IdProducto).FirstOrDefault();
                if (producto != null)
                {
                    producto.Stock = 0;
                    _context.Update(producto);
                }
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(int id)
        {
          return (_context.Inventarios?.Any(e => e.IdInventario == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Reportes(int? idSucursal, string tipoMovimiento, DateTime? fechaInicio, DateTime? fechaFin, int? idProducto)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario=_context.Usuarios.Where(u=>u.IdUsuario==int.Parse(idUsuario)).FirstOrDefault(); 
            // Crear una lista vacía de inventarios
            List<Inventario> inventarios = new List<Inventario>();

            // Verificar si al menos uno de los parámetros de búsqueda tiene un valor
            if (idSucursal.HasValue || !string.IsNullOrEmpty(tipoMovimiento) || fechaInicio.HasValue || fechaFin.HasValue)
            {
                // Crear la consulta si hay al menos un parámetro de búsqueda
                var query = _context.Inventarios
                    .Include(i => i.IdProductoNavigation)
                    .Include(i => i.IdSucursalNavigation)
                    .AsQueryable();

                if (idSucursal.HasValue)
                {
                    query = query.Where(i => i.IdSucursal == idSucursal);
                }
                if (idProducto.HasValue)
                {
                    query = query.Where(i => i.IdProducto == idProducto);
                }
                if (!string.IsNullOrEmpty(tipoMovimiento))
                {
                    query = query.Where(i => i.TipoMovimiento == tipoMovimiento);
                }

                if (fechaInicio.HasValue)
                {
                    query = query.Where(i => i.FechaMovimiento >= fechaInicio.Value);
                }

                if (fechaFin.HasValue)
                {
                    query = query.Where(i => i.FechaMovimiento <= fechaFin.Value);
                }

                // Ejecutar la consulta
                inventarios = await query.ToListAsync();
            }
            Persona persona = new Persona();
            persona = _context.Personas.Where(p => p.IdPersona == usuario.IdPersona).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == persona.Identificacion).FirstOrDefault();       
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e=>e.Identificacion==emisor.Ruc).FirstOrDefault();
            var sucursales = await _context.Sucursals.Where(s=>s.IdEmisor==emisor.IdEmisor).ToListAsync();
            var categoria = await _context.CategoriaProductos.Where(c=>c.IdEmpresa==empresa.IdEmpresa).ToListAsync();
            var productos = await _context.Productos.Where(c => c.IdEmpresa == empresa.IdEmpresa).ToListAsync();
            // Crear el ViewModel
            var viewModel = new InventarioReporteViewModel
            {
                Inventarios = inventarios,
                Sucursales = sucursales,
                IdSucursalSeleccionada = idSucursal,
                TipoMovimientoSeleccionado = tipoMovimiento,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                CategoriaProductos=categoria,
                Productos=productos,
                IdProducto=idProducto,
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult CargarExcel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CargarExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                Notificacion("Archivo no seleccionado", NotificacionTipo.Warning);
                return View();
            }

            var listProductos = new List<ProductoDTO>();
            var listProductosRegistrados = new List<ProductoDTO>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string nombreProducto = worksheet.Cells[row, 3].Value?.ToString();
                        string descripcionProducto = worksheet.Cells[row, 4].Value?.ToString();
                        Producto product = new Producto();
                        product = _context.Productos.Where(p => p.Nombre == nombreProducto & p.Descripcion.Equals(descripcionProducto)).FirstOrDefault();
                        if (product == null)
                        {
                            var producto = new ProductoDTO
                            {
                                CodigoProducto = worksheet.Cells[row, 1].Value?.ToString(),
                                Categoria = worksheet.Cells[row, 2].Value?.ToString(),
                                NombreProducto = worksheet.Cells[row, 3].Value?.ToString(),
                                DescripcionProducto = worksheet.Cells[row, 4].Value?.ToString(),
                                UnidadMedida = worksheet.Cells[row, 5].Value?.ToString(),
                                FacturaNro = worksheet.Cells[row, 6].Value?.ToString(),
                                Proveedor = worksheet.Cells[row, 7].Value?.ToString(),
                                Cantidad = decimal.Parse(worksheet.Cells[row, 8].Value?.ToString()),
                                ValorUnitario = decimal.Parse(worksheet.Cells[row, 9].Value?.ToString()),
                                Descuento = decimal.Parse(worksheet.Cells[row, 10].Value?.ToString()),
                                Subtotal = decimal.Parse(worksheet.Cells[row, 11].Value?.ToString()),
                                IVA = decimal.Parse(worksheet.Cells[row, 12].Value?.ToString()),
                                Total = decimal.Parse(worksheet.Cells[row, 13].Value?.ToString())
                            };

                            listProductos.Add(producto);
                        }
                        else
                        {
                            var producto = new ProductoDTO
                            {
                                CodigoProducto = worksheet.Cells[row, 1].Value?.ToString(),
                                Categoria = worksheet.Cells[row, 2].Value?.ToString(),
                                NombreProducto = worksheet.Cells[row, 3].Value?.ToString(),
                                DescripcionProducto = worksheet.Cells[row, 4].Value?.ToString(),
                                UnidadMedida = worksheet.Cells[row, 5].Value?.ToString(),
                                FacturaNro = worksheet.Cells[row, 6].Value?.ToString(),
                                Proveedor = worksheet.Cells[row, 7].Value?.ToString(),
                                Cantidad = decimal.Parse(worksheet.Cells[row, 8].Value?.ToString()),
                                ValorUnitario = decimal.Parse(worksheet.Cells[row, 9].Value?.ToString()),
                                Descuento = decimal.Parse(worksheet.Cells[row, 10].Value?.ToString()),
                                Subtotal = decimal.Parse(worksheet.Cells[row, 11].Value?.ToString()),
                                IVA = decimal.Parse(worksheet.Cells[row, 12].Value?.ToString()),
                                Total = decimal.Parse(worksheet.Cells[row, 13].Value?.ToString())
                            };

                            listProductosRegistrados.Add(producto);
                        }
                    }
                }
            }

            // Aquí puedes hacer lo que necesites con la lista de productos
            // Por ejemplo, guardarla en la base de datos o procesarla de alguna manera
            TempData["ListaProductos"] = JsonConvert.SerializeObject(listProductos);
            return View("Resultado", (listProductos, listProductosRegistrados));
        
    }

        [HttpPost]
        public IActionResult RegistrarInventario()
        {
            List<ProductoDTO> lista;
            if (TempData["ListaProductos"] is string jsonLista)
            {
                lista = JsonConvert.DeserializeObject<List<ProductoDTO>>(jsonLista);
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
                UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
                usuarioSucursal = _context.UsuarioSucursals.Where(u=>u.IdUsuario==usuario.IdUsuario).FirstOrDefault();
                Persona persona = new Persona();
                persona = _context.Personas.Where(p => p.IdPersona == usuario.IdPersona).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == persona.Identificacion).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                Cuentum cuentum = new Cuentum();
                cuentum=_context.Cuenta.Where(c=>c.Nombre.Equals("Inventario")).FirstOrDefault();
                Impuesto impuesto = new Impuesto();
                impuesto = _context.Impuestos.FirstOrDefault(i => i.Porcentaje == 15.0m);
                UnidadMedidum unidadMedidum = new UnidadMedidum();
                 // Procesa la lista               
                foreach (ProductoDTO producto in lista)
                {
                    unidadMedidum = _context.UnidadMedida.Where(u => u.Abreviatura.Equals(producto.UnidadMedida)).FirstOrDefault();
                    CategoriaProducto categoriaProducto = new CategoriaProducto();
                    categoriaProducto = _context.CategoriaProductos.Where(c => c.Nombre.Equals(producto.Categoria)).FirstOrDefault();
                    if (categoriaProducto == null)
                    {
                        categoriaProducto = new CategoriaProducto();
                        categoriaProducto.Descripcion = producto.Categoria;
                        categoriaProducto.Nombre = producto.Categoria;
                        categoriaProducto.EstadoBoolean = true;
                        categoriaProducto.IdEmpresa = empresa.IdEmpresa;
                        categoriaProducto.FechaCreacion = new DateTime();
                        categoriaProducto.UsuarioCreacion = int.Parse(idUsuario);
                        _context.Add(categoriaProducto);
                        _context.SaveChanges();
                    }
                    Producto pro=new Producto();
                    pro.Nombre = producto.NombreProducto;
                    pro.Descripcion=producto.DescripcionProducto;
                    pro.Codigo = producto.CodigoProducto;
                    pro.PrecioUnitario = producto.ValorUnitario;
                    pro.IdEmpresa=empresa.IdEmpresa;    
                    pro.UsuarioCreacion=int.Parse(idUsuario);
                    pro.FechaCreacion=new DateTime();
                    pro.EstadoBoolean=true;
                    pro.IdImpuesto = impuesto.IdImpuesto;
                    pro.IdUnidadMedida = unidadMedidum.IdUnidadMedida;
                    pro.IdCategoriaProducto=categoriaProducto.IdCategoriaProducto;
                    _context.Add(pro);
                    _context.SaveChanges();
                    Inventario inventario=new Inventario();
                    inventario.Cantidad=producto.Cantidad;
                    inventario.Descripcion = "INGRESO CARGA INICIAL";
                    inventario.NumeroDespacho = ObtenerNumeroDes("E");
                    inventario.IdCuentaContable = cuentum.IdCuenta;
                    inventario.IdProducto=pro.IdProducto;
                    inventario.IdSucursal = usuarioSucursal.IdSucursal;
                    inventario.FechaMovimiento = new DateTime();
                    inventario.Stock = (int)producto.Cantidad;
                    inventario.EstadoBoolean = true;
                    inventario.UsuarioCreacion = int.Parse(idUsuario);
                    inventario.FechaCreacion = new DateTime();
                    inventario.TipoMovimiento = "E";
                    inventario.SubTotal=producto.Subtotal;
                    inventario.Iva = producto.IVA;
                    inventario.Total=producto.Total;
                    _context.Add(inventario);
                    _context.SaveChanges();
                    SucursalInventario sucursalInventario = new SucursalInventario();
                    sucursalInventario.IdInventario=inventario.IdInventario;
                    sucursalInventario.IdSucursal = usuarioSucursal.IdSucursal;
                    sucursalInventario.EstadoBoolean = true;
                    sucursalInventario.UsuarioCreacion=int.Parse(idUsuario);
                    sucursalInventario.FechaCreacion=new DateTime();
                    _context.Add(sucursalInventario);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public string ObtenerNumeroDes(string tipoMovimiento)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.Where(s => s.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
            var ultimoMovimiento = _context.Inventarios
                .Where(i => i.TipoMovimiento == tipoMovimiento & i.IdSucursal == usuarioSucursal.IdSucursal)
                .OrderByDescending(i => i.FechaCreacion)
                .FirstOrDefault();

            string nuevoNumeroDespacho;

            if (ultimoMovimiento != null && !string.IsNullOrEmpty(ultimoMovimiento.NumeroDespacho))
            {
                int ultimoNumero = int.Parse(ultimoMovimiento.NumeroDespacho.Split('-')[1]);
                nuevoNumeroDespacho = $"{tipoMovimiento}-{(ultimoNumero + 1).ToString("D6")}";
            }
            else
            {
                nuevoNumeroDespacho = $"{tipoMovimiento}-000001";
            }

            return nuevoNumeroDespacho;
        }

        public async Task<IActionResult> ExportarExcel(int? idSucursal, string tipoMovimiento, DateTime? fechaInicio, DateTime? fechaFin, int? idProducto)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
            Persona persona = new Persona();
            persona=_context.Personas.Where(p=>p.IdPersona==usuario.IdPersona).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor=_context.Emisors.Where(e=>e.Ruc==persona.Identificacion).FirstOrDefault();
            var query = from pro in _context.Productos
                        join inv in _context.Inventarios on pro.IdProducto equals inv.IdProducto
                        join si in _context.SucursalInventarios on inv.IdInventario equals si.IdInventario
                        join su in _context.Sucursals on si.IdSucursal equals su.IdSucursal
                        join cue in _context.Cuenta on inv.IdCuentaContable equals cue.IdCuenta
                        join cp in _context.CategoriaProductos on pro.IdCategoriaProducto equals cp.IdCategoriaProducto
                        join um in _context.UnidadMedida on pro.IdUnidadMedida equals um.IdUnidadMedida
                        where (idSucursal == null || su.IdSucursal == idSucursal)
                           && (idProducto == null || pro.IdProducto == idProducto)
                           && (string.IsNullOrEmpty(tipoMovimiento) || inv.TipoMovimiento == tipoMovimiento)
                           && (!fechaInicio.HasValue || inv.FechaMovimiento >= fechaInicio)
                           && (!fechaFin.HasValue || inv.FechaMovimiento <= fechaFin)
                        select new
                        {
                            CategoriaProducto = cp.Nombre,
                            CodigoProducto = pro.Codigo,
                            NombreProducto = pro.Nombre,
                            DescripcionProducto = pro.Descripcion,
                            CodigoCuenta = cue.Codigo,
                            UnidadMedida = um.Abreviatura,
                            NumeroFactura = inv.NumeroFactura,
                            FechaMovimiento = inv.FechaMovimiento,
                            Cantidad = inv.Cantidad,
                            Stock = inv.Stock,
                            PrecioUnitario = pro.PrecioUnitario,
                            SubTotal = inv.SubTotal,
                            Iva = inv.Iva,
                            Total = inv.Total
                        };

            var data = await query.ToListAsync();
            string categoriaProducto = "";
            string tipoMov = "";
            if (tipoMovimiento != null)
            {
                if (tipoMovimiento.Equals("E"))
                {
                    tipoMov = "DE " + "INGRESO";
                }
                if (tipoMovimiento.Equals("S"))
                {
                    tipoMov = "DE " + "SALIDA";
                }
                if (tipoMovimiento.Equals("T"))
                {
                    tipoMov = "DE " + "TRANSFERENCIA";
                }
                if (tipoMovimiento.Equals("A"))
                {
                    tipoMov = "DE " + "AJUSTE";
                }
            }
            foreach (var item in data)
            {
                categoriaProducto = item.CategoriaProducto;
            }
                // Crear el archivo Excel
                using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Inventario");
                 // Agregar logo
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                var logo = worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.25); // Ajusta el tamaño según sea necesario
                                 // Agregar título y detalles
                worksheet.Cell("C1").Value = "REPORTE DE INVENTARIO "+ tipoMov;
                worksheet.Cell("C2").Value = "";
                worksheet.Cell("C3").Value = "Nombre del cliente: " +emisor.RazonSocial; // Implementa este método
                worksheet.Cell("C4").Value = "RUC: " + emisor.Ruc; // Implementa este método
                worksheet.Cell("C5").Value = "Cuenta contable: 1.1.2 Inventario de consumo de materiales";
                worksheet.Cell("C6").Value = "Tipo de Inventario: " + categoriaProducto;

                // Estilo para el título y detalles
                var rangeC1C6 = worksheet.Range("C1:C6");
                rangeC1C6.Style.Font.Bold = true;
                worksheet.Cell("C1").Style.Font.FontSize = 14;

                // Comenzar la tabla de datos desde la fila 8
                var currentRow = 8;

                // Añadir encabezados
                worksheet.Cell(currentRow, 1).Value = "Categoría Producto";
                worksheet.Cell(currentRow, 2).Value = "Código Producto";
                worksheet.Cell(currentRow, 3).Value = "Nombre Producto";
                worksheet.Cell(currentRow, 4).Value = "Descripción Producto";
                worksheet.Cell(currentRow, 5).Value = "Código Cuenta";
                worksheet.Cell(currentRow, 6).Value = "Unidad Medida";
                worksheet.Cell(currentRow, 7).Value = "Número Factura";
                worksheet.Cell(currentRow, 8).Value = "Fecha Movimiento";
                worksheet.Cell(currentRow, 9).Value = "Cantidad";
                worksheet.Cell(currentRow, 10).Value = "Stock";
                worksheet.Cell(currentRow, 11).Value = "Precio Unitario";
                worksheet.Cell(currentRow, 12).Value = "Subtotal";
                worksheet.Cell(currentRow, 13).Value = "IVA";
                worksheet.Cell(currentRow, 14).Value = "Total";

                // Añadir datos
                foreach (var item in data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.CategoriaProducto;
                    worksheet.Cell(currentRow, 2).Value = item.CodigoProducto;
                    worksheet.Cell(currentRow, 3).Value = item.NombreProducto;
                    worksheet.Cell(currentRow, 4).Value = item.DescripcionProducto;
                    worksheet.Cell(currentRow, 5).Value = item.CodigoCuenta;
                    worksheet.Cell(currentRow, 6).Value = item.UnidadMedida;
                    worksheet.Cell(currentRow, 7).Value = item.NumeroFactura;
                    worksheet.Cell(currentRow, 8).Value = item.FechaMovimiento;
                    worksheet.Cell(currentRow, 9).Value = item.Cantidad;
                    worksheet.Cell(currentRow, 10).Value = item.Stock;
                    worksheet.Cell(currentRow, 11).Value = item.PrecioUnitario;
                    worksheet.Cell(currentRow, 12).Value = item.SubTotal;
                    worksheet.Cell(currentRow, 13).Value = item.Iva;
                    worksheet.Cell(currentRow, 14).Value = item.Total;
                }

                // Ajustar el ancho de las columnas automáticamente
                worksheet.Columns().AdjustToContents();

                // Aplicar estilo a los encabezados
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteInventario.xlsx");
                }
            }
        }
    }
}
