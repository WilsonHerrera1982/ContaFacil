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
            ViewData["IdProducto"] = new SelectList(
            _context.Productos
                .Where(e => e.IdEmpresa == empresa.IdEmpresa)
                .Select(p => new {
                    IdProducto = p.IdProducto,
                    NombreDescripcion = p.Nombre + " - " + p.Descripcion
                }),
            "IdProducto",
            "NombreDescripcion"
        );

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
                
                    ultimoMovimiento = _context.Inventarios
                     .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T" || i.TipoMovimiento == "C" || i.TipoMovimiento == "V") & i.IdProducto == inventario.idProducto & i.IdSucursal == usuarioSucursal.IdSucursal)
                     .OrderByDescending(i => i.FechaCreacion)
                     .FirstOrDefault();
                 Inventario inv = new Inventario();
                if (inventario.tipoMovimiento.Equals("E") || inventario.tipoMovimiento.Equals("C"))
                {
                    Inventario movimientoIngreso = new Inventario();

                    movimientoIngreso = _context.Inventarios
                     .Where(i => (i.TipoMovimiento == "E" || i.TipoMovimiento == "C") & i.IdProducto == inventario.idProducto & i.IdSucursal == usuarioSucursal.IdSucursal)
                     .OrderByDescending(i => i.FechaCreacion)
                     .FirstOrDefault();
                    inv.IdProducto= inventario.idProducto;
                    inv.TipoMovimiento = inventario.tipoMovimiento;
                    inv.NumeroDespacho= inventario.numeroDespacho;
                    inv.EstadoBoolean = true;
                    inv.UsuarioCreacion = int.Parse(idUsuario);
                    inv.FechaCreacion = new DateTime();
                    inv.Cantidad= inventario.cantidad;
                    inv.Descripcion = "INGRESO POR COMPRA";
                    inv.IdSucursal=usuarioSucursal.IdSucursal;
                    inv.IdCuentaContable = cuentum.IdCuenta;
                    inv.SubTotal = inventario.subtotal;
                    inv.Subtotal15= inventario.subtotal15;
                    inv.Descuento=inventario.descuento;
                    inv.PrecioUnitario = inventario.precioUnitario;
                    inv.PrecioUnitarioFinal=inventario.precioUnitarioFinal;
                    inv.Iva=inventario.iva;
                    inv.Total=inventario.total;
                    inv.NumeroFactura= inventario.numeroFactura;
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
                    if (producto.PrecioUnitario != inv.PrecioUnitarioFinal)
                    {
                        decimal precioProducto = (inv.PrecioUnitarioFinal + movimientoIngreso.PrecioUnitarioFinal) / 2??0;
                        producto.PrecioUnitario = precioProducto;
                        _context.Update(producto);
                        _context.SaveChanges();
                    }
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
                    detalleDespacho.IdDespacho=despacho.IdDespacho;
                    detalleDespacho.IdProducto=inv.IdProducto??0;
                    detalleDespacho.Cantidad = (int)inv.Cantidad;
                    detalleDespacho.IdUsuario = int.Parse(idUsuario);
                    detalleDespacho.UsuarioCreacion=int.Parse(idUsuario);
                    detalleDespacho.FechaCreacion = new DateTime();
                    detalleDespacho.EstadoBoolean= true;
                    _context.Add(detalleDespacho);
                    await  _context.SaveChangesAsync();
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
            ViewData["IdProducto"] = new SelectList(
                _context.Productos
                    .Where(e => e.IdEmpresa == empresa.IdEmpresa)
                    .Select(p => new {
                        IdProducto = p.IdProducto,
                        NombreDescripcion = p.Nombre + " - " + p.Descripcion
                    }),
                "IdProducto",
                "NombreDescripcion"
                );
            // Crear el ViewModel
            var viewModel = new InventarioReporteViewModel
            {
                Inventarios = inventarios,
                Sucursales = sucursales,
                IdSucursalSeleccionada = idSucursal,
                TipoMovimientoSeleccionado = tipoMovimiento,
                FechaInicio = new DateTime(2024, 1, 1),
                FechaFin = new DateTime(2024, 12, 31),
                CategoriaProductos =categoria,
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
                    inventario.NumeroFactura = producto.FacturaNro;
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
                    inventario.Descuento = producto.Descuento;
                    inventario.PrecioUnitario = producto.ValorUnitario;
                    inventario.PrecioUnitarioFinal = (producto.Subtotal - producto.Descuento) / producto.Cantidad;
                    inventario.Subtotal15=producto.Subtotal-producto.Descuento;
                    inventario.SubTotal=producto.Subtotal;
                    inventario.Iva = inventario.Subtotal15*0.15m;
                    inventario.Total=inventario.Subtotal15+inventario.Iva;
                    _context.Add(inventario);
                    _context.SaveChanges();
                    pro.PrecioUnitario = inventario.PrecioUnitarioFinal??0;
                    _context.Update(inventario);
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
                            PrecioUnitario=inv.PrecioUnitario,
                            PrecioUnitarioFinal= inv.PrecioUnitarioFinal,
                            Descuento = inv.Descuento,
                            Subtotal15= inv.Subtotal15,
                            Stock = inv.Stock,
                            SubTotal = inv.SubTotal,
                            Iva = inv.Iva,
                            Total = inv.Total,
                            DescripcionInventario=inv.Descripcion
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
                if (tipoMovimiento.Equals("C"))
                {
                    tipoMov = "DE " + "COMPRA";
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
                worksheet.Cell(currentRow, 5).Value = "Descripción Inventario";
                worksheet.Cell(currentRow, 6).Value = "Código Cuenta";
                worksheet.Cell(currentRow, 7).Value = "Unidad Medida";
                worksheet.Cell(currentRow, 8).Value = "Número Factura";
                worksheet.Cell(currentRow, 9).Value = "Fecha Movimiento";
                worksheet.Cell(currentRow, 10).Value = "Cantidad";
                worksheet.Cell(currentRow, 11).Value = "Stock";
                worksheet.Cell(currentRow, 12).Value = "Precio Unitario";
                worksheet.Cell(currentRow, 13).Value = "Precio Unitario Final";
                worksheet.Cell(currentRow, 14).Value = "Descuento";
                worksheet.Cell(currentRow, 15).Value = "Subtotal";
                worksheet.Cell(currentRow, 16).Value = "Subtotal 15%";
                worksheet.Cell(currentRow, 17).Value = "IVA";
                worksheet.Cell(currentRow, 18).Value = "Total";

                // Añadir datos
                foreach (var item in data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.CategoriaProducto;
                    worksheet.Cell(currentRow, 2).Value = item.CodigoProducto;
                    worksheet.Cell(currentRow, 3).Value = item.NombreProducto;
                    worksheet.Cell(currentRow, 4).Value = item.DescripcionProducto;
                    worksheet.Cell(currentRow, 5).Value = item.DescripcionInventario;
                    worksheet.Cell(currentRow, 6).Value = item.CodigoCuenta;
                    worksheet.Cell(currentRow, 7).Value = item.UnidadMedida;
                    worksheet.Cell(currentRow, 8).Value = item.NumeroFactura;
                    worksheet.Cell(currentRow, 9).Value = item.FechaMovimiento;
                    worksheet.Cell(currentRow, 10).Value = item.Cantidad;
                    worksheet.Cell(currentRow, 11).Value = item.Stock;
                    worksheet.Cell(currentRow, 12).Value = item.PrecioUnitario;
                    worksheet.Cell(currentRow, 13).Value = item.PrecioUnitarioFinal;
                    worksheet.Cell(currentRow, 14).Value = item.Descuento;
                    worksheet.Cell(currentRow, 15).Value = item.SubTotal;
                    worksheet.Cell(currentRow, 16).Value = item.Subtotal15;
                    worksheet.Cell(currentRow, 17).Value = item.Iva;
                    worksheet.Cell(currentRow, 18).Value = item.Total;
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

        public async Task<IActionResult> ExportarKardexExcel(int idProducto, DateTime fechaInicio, DateTime fechaFin)
        {
            if (idProducto == 0 || fechaInicio == null || fechaFin == null)
            {
                Notificacion("Verifica los parametros (Producto, Fecha inicio y Fecha fin son obligatorios )", NotificacionTipo.Warning);
                return RedirectToAction(nameof(Reportes));
            }

            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
            Persona persona = _context.Personas.FirstOrDefault(p => p.IdPersona == usuario.IdPersona);
            Emisor emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == persona.Identificacion);
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u=>u.IdUsuario==int.Parse(idUsuario));
            ContaFacil.Models.Sucursal sucursal = _context.Sucursals.FirstOrDefault(s=>s.IdSucursal== usuarioSucursal.IdSucursal);
            var producto = await _context.Productos
                .Include(p => p.IdCategoriaProductoNavigation)
                .Include(p => p.IdUnidadMedidaNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            var movimientos = await _context.Inventarios
                .Where(inv => inv.IdProducto == idProducto && inv.FechaMovimiento >= fechaInicio && inv.FechaMovimiento <= fechaFin)
                .OrderBy(inv => inv.FechaMovimiento)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Kardex");

                // Agregar logo y título
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                var logo = worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.2);

                worksheet.Cell("C1").Value = "KARDEX";
                worksheet.Cell("C2").Value = $"Nombre de cliente: {emisor.RazonSocial}";
                worksheet.Cell("C3").Value = $"RUC: {emisor.Ruc}";
                worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";
                worksheet.Cell("C5").Value = $"Tipo inventario: {producto.IdCategoriaProductoNavigation.Nombre}";
                worksheet.Cell("C6").Value = $"Código de producto: {producto.Codigo}";
                worksheet.Cell("C7").Value = $"Descripción del producto: {producto.Nombre}";
                worksheet.Cell("C8").Value = $"Unidad de medida: {producto.IdUnidadMedidaNavigation.Abreviatura}";
                worksheet.Cell("C9").Value = $"Código de barra: [Agregar campo de código de barra]";

                // Estilos para el encabezado
                var rangeC1C9 = worksheet.Range("C1:C9");
                rangeC1C9.Style.Font.Bold = true;
                worksheet.Cell("C1").Style.Font.FontSize = 14;

                // Encabezados de la tabla Kardex
                int currentRow = 11;
                worksheet.Cell(currentRow, 1).Value = "No.";
                worksheet.Cell(currentRow, 2).Value = "Fecha";
                worksheet.Cell(currentRow, 3).Value = "Descripción de la Transacción";
                worksheet.Cell(currentRow, 4).Value = "N° Documento";
                worksheet.Cell(currentRow, 5).Value = "INGRESOS";
                worksheet.Cell(currentRow, 6).Value = "";
                worksheet.Cell(currentRow, 7).Value = "";
                worksheet.Cell(currentRow, 8).Value = "EGRESOS";
                worksheet.Cell(currentRow, 9).Value = "";
                worksheet.Cell(currentRow, 10).Value = "";
                worksheet.Cell(currentRow, 11).Value = "SALDOS";
                worksheet.Cell(currentRow, 12).Value = "";
                worksheet.Cell(currentRow, 13).Value = "";

                currentRow++;
                worksheet.Cell(currentRow, 5).Value = "Cantidad";
                worksheet.Cell(currentRow, 6).Value = "Valor Unitario";
                worksheet.Cell(currentRow, 7).Value = "Valor Total";
                worksheet.Cell(currentRow, 8).Value = "Cantidad";
                worksheet.Cell(currentRow, 9).Value = "Valor Unitario";
                worksheet.Cell(currentRow, 10).Value = "Valor Total";
                worksheet.Cell(currentRow, 11).Value = "Cantidad";
                worksheet.Cell(currentRow, 12).Value = "Valor Unitario";
                worksheet.Cell(currentRow, 13).Value = "Valor Total";

                // Aplicar estilo a los encabezados
                var headerRange = worksheet.Range(11, 1, currentRow, 13);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Variables para el saldo
                decimal saldoCantidad = 0;
                decimal saldoValorTotal = 0;
                decimal saldoValorUnitario = 0;

                // Añadir datos del Kardex
                int numeroMovimiento = 1;
                foreach (var mov in movimientos)
                {
                    currentRow++;

                    string tipoMovimiento = mov.TipoMovimiento switch
                    {
                        "E" or "C" => "COMPRA",
                        "S" or "V" => "VENTA",
                        "T" => "TRANSFERENCIA",
                        "A" => "AJUSTE",
                        _ => "DESCONOCIDO"
                    };

                    worksheet.Cell(currentRow, 1).Value = numeroMovimiento++;
                    worksheet.Cell(currentRow, 2).Value = mov.FechaMovimiento;
                    worksheet.Cell(currentRow, 3).Value = mov.Descripcion;
                    worksheet.Cell(currentRow, 4).Value = mov.NumeroDespacho;

                    if (tipoMovimiento == "COMPRA")
                    {
                        worksheet.Cell(currentRow, 5).Value = mov.Cantidad;
                        worksheet.Cell(currentRow, 6).Value = mov.PrecioUnitarioFinal;
                        worksheet.Cell(currentRow, 7).Value = mov.Total;

                        // Actualizar saldo
                        if (saldoCantidad == 0)
                        {
                            // Primer registro
                            saldoValorUnitario = mov.PrecioUnitario??0;
                        }
                        else
                        {
                            // Más de un registro
                            saldoValorUnitario = (saldoValorUnitario + mov.PrecioUnitarioFinal) / 2 ?? 0;
                        }

                        saldoCantidad += mov.Cantidad ?? 0;
                        saldoValorTotal = saldoCantidad * saldoValorUnitario;
                    }
                    else if (tipoMovimiento == "VENTA")
                    {
                        worksheet.Cell(currentRow, 8).Value = mov.Cantidad;
                        worksheet.Cell(currentRow, 9).Value = saldoValorUnitario;
                        worksheet.Cell(currentRow, 10).Value = mov.Cantidad * saldoValorUnitario;

                        saldoCantidad -= mov.Cantidad ?? 0;
                        saldoValorTotal = saldoCantidad * saldoValorUnitario;
                    }

                    worksheet.Cell(currentRow, 11).Value = saldoCantidad;
                    worksheet.Cell(currentRow, 12).Value = saldoValorUnitario;
                    worksheet.Cell(currentRow, 13).Value = saldoValorTotal;
                }

                // Ajustar el ancho de las columnas automáticamente
                worksheet.Columns().AdjustToContents();

                // Aplicar formato de número a las columnas numéricas
                var rangeNumerica = worksheet.Range(13, 5, currentRow, 13);
                rangeNumerica.Style.NumberFormat.Format = "#,##0.00";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Kardex_{producto.Nombre}.xlsx");
                }
            }
        }
    }
}
