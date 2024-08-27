using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using ContaFacil.Utilities;
using ContaFacil.Models.ViewModel;
using System.Xml.Linq;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace ContaFacil.Controllers.Contador
{
    public class FacturacionContadorController : NotificacionClass
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        string idUsuario = "";
        string idEmpresa = "";
        public FacturacionContadorController(ContableContext context, IConfiguration configuration)
        {
            _configuration = configuration;

            _context = context;
        }

        // GET: FacturacionContador
        public async Task<IActionResult> Index(int? IdEmisor)
        {
            idEmpresa = HttpContext.Session.GetString("_empresa");
            var contableContext = _context.Facturas
                .Where(f => f.IdEmisorNavigation.IdEmpresa == int.Parse(idEmpresa));

            if (IdEmisor != null)
            {
                contableContext = contableContext.Where(f => f.IdEmisor == IdEmisor);
            }

            // Apply the Include methods after all Where clauses
            contableContext = contableContext
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.IdEmisorNavigation);

            var emisores = await _context.Emisors
                .Where(e => e.IdEmpresa == int.Parse(idEmpresa))
                .Select(e => new SelectListItem { Value = e.IdEmisor.ToString(), Text = e.RazonSocial })
                .ToListAsync();

            var viewModel = new FacturacionContadorViewModel
            {
                Facturas = await contableContext.ToListAsync(),
                Emisores = emisores,
                SelectedEmisorId = IdEmisor
            };

            return View(viewModel);
        }
        // GET: FacturacionContador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        public IActionResult Create()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.IdEmpresa == int.Parse(idEmpresa)).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).Where(p => p.IdEmpresa == empresa.IdEmpresa).ToList();
            ViewBag.Productos = _context.Productos.Where(p => p.IdEmpresa == empresa.IdEmpresa & p.Stock > 0).Include(p => p.IdImpuestoNavigation).ToList();
            ViewData["IdEmisor"] = new SelectList(_context.Emisors.Where(e => e.IdEmpresa == int.Parse(idEmpresa)), "IdEmisor", "NombreComercial");
            ViewData["IdTipoPago"] = new SelectList(_context.TipoPagos, "IdTipoPago", "Nombre");
            return View();
        }
        // POST: FacturacionContador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(Factura factura, List<DetalleFactura> detalles, List<Pago> pagos)
        {
            try
            {
                idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = new Usuario();
                usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.IdEmisor == factura.IdEmisor).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                ContaFacil.Models.Sucursal sucursal = new ContaFacil.Models.Sucursal();
                sucursal = _context.Sucursals.Where(s => s.IdEmisor == emisor.IdEmisor).FirstOrDefault();
                UsuarioSucursal us = new UsuarioSucursal();
                us = _context.UsuarioSucursals.Where(u => u.IdSucursal == sucursal.IdSucursal).FirstOrDefault();
                SucursalInventario sucursalInventario = new SucursalInventario();
                sucursalInventario = _context.SucursalInventarios.Where(s => s.IdSucursal == us.IdSucursal).FirstOrDefault();
                Factura ultimoFactura = _context.Facturas
                  .Where(f => f.IdEmisor == emisor.IdEmisor)
                  .OrderByDescending(i => i.FechaCreacion)
                  .FirstOrDefault();
                var secuencialStr = "";
                if (ultimoFactura != null && ultimoFactura.NumeroFactura != null)
                {

                    string numero = ObtenerSiguienteNumeroFactura(ultimoFactura.NumeroFactura);
                    var partes = numero.Split('-');

                    if (partes.Length != 3)
                    {
                        throw new ArgumentException("Formato de número de factura inválido.");
                    }
                    factura.NumeroFactura = numero;
                    // La parte secuencial es la última parte
                    secuencialStr = partes[2];
                    emisor.Secuencial = secuencialStr;

                }
                else
                {
                    factura.NumeroFactura = emisor.Establecimiento + "-" + emisor.PuntoEmision + "-" + emisor.Secuencial;
                }

                factura.Fecha = DateOnly.FromDateTime(DateTime.Now);
                factura.FechaCreacion = DateTime.Now;
                factura.UsuarioCreacion = int.Parse(idUsuario);
                factura.Estado = "Pendiente";
                factura.EstadoBoolean = true;
                factura.IdEmisor = emisor.IdEmisor;
                _context.Facturas.Add(factura);
                _context.SaveChanges();
                foreach (var detalle in detalles)
                {
                    Inventario ultimoMovimiento = _context.Inventarios
   .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T" || i.TipoMovimiento == "C" || i.TipoMovimiento == "V") & i.IdProducto == detalle.IdProducto & i.IdSucursal == us.IdSucursal)
   .OrderByDescending(i => i.FechaCreacion)
   .FirstOrDefault();
                    if (ultimoMovimiento == null)
                    {
                        Notificacion("No existe inventario del producto", NotificacionTipo.Warning);
                        ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).ToList();
                        ViewBag.Productos = _context.Productos.ToList();
                        return RedirectToAction("Index", "Factura");
                    }
                    else if (ultimoMovimiento.Stock < detalle.Cantidad)
                    {
                        Notificacion("Producto no disponible", NotificacionTipo.Warning);
                        ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).ToList();
                        ViewBag.Productos = _context.Productos.ToList();
                        return RedirectToAction("Index", "Factura");
                    }
                    Producto producto = new Producto();
                    producto = _context.Productos.FirstOrDefault(p => p.IdProducto == detalle.IdProducto);
                    detalle.IdFactura = factura.IdFactura;
                    detalle.FechaCreacion = DateTime.Now;
                    detalle.UsuarioCreacion = int.Parse(idUsuario);
                    detalle.Estado = true;
                    _context.DetalleFacturas.Add(detalle);
                    producto.Stock = ultimoMovimiento.Stock - detalle.Cantidad;
                    ultimoMovimiento.Stock = ultimoMovimiento.Stock - detalle.Cantidad;
                    _context.Productos.Update(producto);
                    Inventario inventario = new Inventario();
                    inventario.Cantidad = detalle.Cantidad;
                    inventario.IdProducto = detalle.IdProducto;
                    inventario.TipoMovimiento = "S";
                    inventario.Stock = ultimoMovimiento.Stock;
                    inventario.FechaMovimiento = new DateTime();
                    inventario.UsuarioCreacion = int.Parse(idUsuario);
                    inventario.Descripcion = "EGRESO POR VENTA FACTURA " + factura.IdFactura;
                    _context.Inventarios.Add(inventario);
                }

                _context.SaveChanges();
                Cliente cliente = new Cliente();
                cliente = _context.Clientes
               .Where(c => c.IdCliente == factura.IdCliente)
               .Include(c => c.IdPersonaNavigation)
                   .ThenInclude(p => p.IdTipoIdentificacionNavigation)
               .FirstOrDefault();
                emisor = _context.Emisors.Where(e => e.IdEmisor == factura.IdEmisor).FirstOrDefault();
                if (!secuencialStr.Equals(""))
                {
                    emisor.Secuencial = secuencialStr;
                }
                var generator = new FacturaXmlGenerator(_configuration);
                var factu = _context.Facturas
    .Include(f => f.DetalleFacturas)
        .ThenInclude(df => df.IdProductoNavigation)
            .ThenInclude(p => p.IdImpuestoNavigation)
    .FirstOrDefault(f => f.IdFactura == factura.IdFactura);
                var xmlDocument = generator.GenerateXml(factura, cliente.IdPersonaNavigation, emisor);

                // Para guardar el XML en un archivo:
                xmlDocument.Save("factura.xml");

                // Para obtener el XML como string:
                string xmlString = xmlDocument.ToString();
                string xmlFirmado = generator.FirmarXml(xmlString, emisor.CertificadoDigital, emisor.Clave);
                var (estado, descripcion) = await generator.EnviarXmlFirmadoYProcesarRespuesta(emisor.TipoAmbiente, xmlFirmado, factura.IdFactura);
                factura.DescripcionSri = descripcion;
                factura.Estado = estado;
                factura.Xml = xmlFirmado;
                XDocument xdoc = XDocument.Parse(xmlString);

                // Extraer la clave de acceso
                string claveAcceso = (string)xdoc.Root
                                            .Element("infoTributaria")
                                            .Element("claveAcceso");
                factura.ClaveAcceso = claveAcceso;
                _context.Update(factura);
                _context.SaveChanges();
                Notificacion("Registro guardado con exito", NotificacionTipo.Success);
                ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).ToList();
                ViewBag.Productos = _context.Productos.ToList();
                return RedirectToAction("Index", "Factura");
            }
            catch (Exception ex)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Productos = _context.Productos.ToList();
                Notificacion("Error al guardar el registro", NotificacionTipo.Error);
                return View(factura);
            }
        }


        // GET: FacturacionContador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", factura.IdCliente);
            return View(factura);
        }

        // POST: FacturacionContador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFactura,IdCliente,Fecha,MontoTotal,Estado,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Factura factura)
        {
            if (id != factura.IdFactura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.IdFactura))
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
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", factura.IdCliente);
            return View(factura);
        }

        // GET: FacturacionContador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // POST: FacturacionContador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Facturas == null)
            {
                return Problem("Entity set 'ContableContext.Facturas'  is null.");
            }
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
          return (_context.Facturas?.Any(e => e.IdFactura == id)).GetValueOrDefault();
        }
        [HttpGet]
        public IActionResult GetClientesPorEmisor(int idEmisor)
        {
            var emisor = _context.Emisors.Find(idEmisor);
            if (emisor == null)
                return Json(new SelectList(Enumerable.Empty<SelectListItem>()));

            var empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);
            if (empresa == null)
                return Json(new SelectList(Enumerable.Empty<SelectListItem>()));

            var clientes = _context.Clientes
                .Where(c => c.IdEmpresa == empresa.IdEmpresa)
                .Select(c => new { c.IdCliente, c.IdPersonaNavigation.Nombre })
                .ToList();

            return Json(new SelectList(clientes, "IdCliente", "Nombre"));
        }
        [HttpPost]
        public IActionResult GenerarRIDE(int id)
        {
            var factura = _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .ThenInclude(c => c.IdPersonaNavigation)
                .FirstOrDefault(f => f.IdFactura == id);

            if (factura == null)
            {
                return NotFound();
            }



            // Llamar al método GenerarRIDE
            GeneradorRIDE generador = new GeneradorRIDE();
            generador.GenerarRIDE(factura.Xml);
            // Genera el PDF y obtén los bytes
            var pdfBytes = generador.GenerarRIDE(factura.Xml);

            // Retorna el archivo como descarga
            return File(pdfBytes, "application/pdf", "Factura.pdf");

            return RedirectToAction(nameof(Index));
        }
        public string ObtenerSiguienteNumeroFactura(string ultimoNumeroFactura)
        {
            // Divide el número de factura en partes
            var partes = ultimoNumeroFactura.Split('-');

            if (partes.Length != 3)
            {
                throw new ArgumentException("Formato de número de factura inválido.");
            }

            // La parte secuencial es la última parte
            var secuencialStr = partes[2];

            // Convierte la parte secuencial en un número entero
            if (!int.TryParse(secuencialStr, out int secuencial))
            {
                throw new ArgumentException("Parte secuencial inválida.");
            }

            // Incrementa el número secuencial
            secuencial++;

            // Formatea el número secuencial con ceros a la izquierda (mantener 9 dígitos)
            var siguienteSecuencialStr = secuencial.ToString("D9");

            // Crea el nuevo número de factura
            var nuevoNumeroFactura = $"{partes[0]}-{partes[1]}-{siguienteSecuencialStr}";

            return nuevoNumeroFactura;
        }
        public IActionResult GetProductoDetails(int idProducto)
        {
            var producto = _context.Productos.Include(p => p.IdImpuestoNavigation).FirstOrDefault(p => p.IdProducto == idProducto);
            if (producto == null)
            {
                return NotFound();
            }

            return Json(new { precioUnitario = producto.PrecioUnitario, porcentaje = producto.Porcentaje });
        }
    }
}
