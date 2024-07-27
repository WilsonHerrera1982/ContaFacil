using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using ContaFacil.Utilities;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Serialization;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Components.Forms;
using iTextSharp.text.pdf.codec.wmf;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace ContaFacil.Controllers
{
    public class FacturaController : NotificacionClass
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        string idUsuario="";
        string idEmpresa="";
        public FacturaController(ContableContext context, IConfiguration configuration)
        {            
            _context = context;
            _configuration = configuration;
        }
        // GET: Factura
        public async Task<IActionResult> Index()
        {
            idUsuario = HttpContext.Session.GetString("_idUsuario");
            idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = new Usuario();
            usuario=_context.Usuarios.Where(u=>u.IdUsuario==int.Parse(idUsuario)).Include(p=>p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();   
            emisor=_context.Emisors.Where(e=>e.Ruc==usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
            var contableContext = _context.Facturas.Where(f=>f.IdEmisor==emisor.IdEmisor & f.IdSucursal==usuarioSucursal.IdSucursal).Include(f => f.IdClienteNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Factura/Details/5
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
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            UsuarioSucursal usuarioSucursal= new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(e => e.IdUsuario==usuario.IdUsuario).FirstOrDefault();

            List<SucursalInventario> sucursalInventario = _context.SucursalInventarios.Where(s => s.IdSucursal == usuarioSucursal.IdSucursal).
                Include(i=>i.IdInventarioNavigation).Where(i=> (i.IdInventarioNavigation.TipoMovimiento == "S" || i.IdInventarioNavigation.TipoMovimiento == "E") & i.IdInventarioNavigation.Stock >= 0).ToList();
            // Obtener la lista de IdProducto de sucursalInventario
            // Obtener la lista de IdProducto de sucursalInventario, filtrando los valores nulos
            List<int> idProductos = sucursalInventario
                .Select(si => si.IdInventarioNavigation.IdProducto)
                .Where(id => id.HasValue)  // Filtrar nulos
                .Select(id => id.Value)  // Convertir de int? a int
                .Distinct()
                .ToList();

            // Filtrar la lista de productos con los IdProducto obtenidos y la empresa específica
            ViewBag.Productos = _context.Productos
                .Where(p => p.IdEmpresa == empresa.IdEmpresa && idProductos.Contains(p.IdProducto))
                .Include(p => p.IdImpuestoNavigation)
                .ToList();
            ViewBag.Clientes = _context.Clientes.Include(p=>p.IdPersonaNavigation).Where(p=>p.IdEmpresa==empresa.IdEmpresa).ToList();
            ViewData["IdTipoPago"] = new SelectList(_context.TipoPagos, "IdTipoPago", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Factura factura, List<DetalleFactura> detalles, List<Pago> pagos)
        {
            try
            {
                idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = new Usuario();
                usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                UsuarioSucursal us=new UsuarioSucursal();
                us = _context.UsuarioSucursals.Where(u => u.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
                SucursalInventario sucursalInventario = new SucursalInventario();
                sucursalInventario=_context.SucursalInventarios.Where(s => s.IdSucursal==us.IdSucursal).FirstOrDefault();
                Factura ultimoFactura = _context.Facturas
                  .Where(f => f.IdEmisor == emisor.IdEmisor)
                  .OrderByDescending(i => i.FechaCreacion)
                  .FirstOrDefault();
                var secuencialStr="";
                Inventario inventario = new Inventario();
                if (ultimoFactura != null && ultimoFactura.NumeroFactura!=null)
                {
                    
                   string numero= ObtenerSiguienteNumeroFactura(ultimoFactura.NumeroFactura);
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
                    factura.NumeroFactura = emisor.Establecimiento+"-"+emisor.PuntoEmision+"-"+emisor.Secuencial;
                }

                factura.Fecha = DateOnly.FromDateTime(DateTime.Now);
                factura.FechaCreacion = DateTime.Now;
                factura.UsuarioCreacion = int.Parse(idUsuario);
                factura.Estado = "Pendiente";
                factura.EstadoBoolean = true;
                factura.IdEmisor = emisor.IdEmisor;
                factura.IdSucursal = sucursalInventario.IdSucursal;
                _context.Facturas.Add(factura);
                _context.SaveChanges();
                
                    foreach (var detalle in detalles)
                {
                    Inventario ultimoMovimiento = _context.Inventarios
   .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T") & i.IdProducto==detalle.IdProducto)
   .OrderByDescending(i => i.FechaCreacion)
   .FirstOrDefault();
                    if(ultimoMovimiento == null)
                    {
                        sucursalInventario=_context.SucursalInventarios.Where(s=>s.IdInventario==ultimoMovimiento.IdInventario).FirstOrDefault();
                        if (sucursalInventario == null)
                        {
                            Notificacion("No existe inventario del producto", NotificacionTipo.Warning);
                            ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).ToList();
                            ViewBag.Productos = _context.Productos.ToList();
                            return RedirectToAction("Index", "Factura");
                        }
                    }
                    else if (ultimoMovimiento.Stock<detalle.Cantidad)
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

                   // producto.Stock= ultimoMovimiento.Stock-detalle.Cantidad;
                    ultimoMovimiento.Stock= ultimoMovimiento.Stock - detalle.Cantidad;

                    // _context.Productos.Update(producto);
                    int stock = ultimoMovimiento.Stock ?? 0; // Si Stock es null, usar 0 como valor por defecto
                    int nuevoStock = stock - detalle.Cantidad;
                    
                    inventario.Cantidad=detalle.Cantidad;
                    inventario.IdProducto= detalle.IdProducto;
                    inventario.TipoMovimiento = "S";
                    inventario.Stock= nuevoStock;
                    inventario.FechaMovimiento = new DateTime();
                    inventario.UsuarioCreacion=int.Parse(idUsuario);
                    inventario.Descripcion = "EGRESO POR VENTA FACTURA "+factura.IdFactura;
                    _context.Inventarios.Add(inventario);
                }

                _context.SaveChanges();
                SucursalInventario si = new SucursalInventario();
                si.IdSucursal=sucursalInventario.IdSucursal;
                si.IdInventario= inventario.IdInventario;
                si.EstadoBoolean = true;
                si.UsuarioCreacion = int.Parse(idUsuario);
                si.FechaCreacion = new DateTime();
                _context.Add(si);
                _context.SaveChanges();
                 Cliente cliente = new Cliente();
                 cliente = _context.Clientes
                .Where(c => c.IdCliente == factura.IdCliente)
                .Include(c => c.IdPersonaNavigation)
                    .ThenInclude(p => p.IdTipoIdentificacionNavigation)
                .FirstOrDefault();
                emisor =_context.Emisors.Where(e=>e.IdEmisor==factura.IdEmisor).FirstOrDefault();
                if (!secuencialStr.Equals(""))
                {
                    emisor.Secuencial= secuencialStr;
                }
                var generator = new FacturaXmlGenerator(_configuration);
                var factu = _context.Facturas.Include(f=>f.Pagos).ThenInclude(pf=>pf.IdTipoPagoNavigation)
    .Include(f => f.DetalleFacturas)
        .ThenInclude(df => df.IdProductoNavigation)
            .ThenInclude(p => p.IdImpuestoNavigation)
    .FirstOrDefault(f => f.IdFactura == factura.IdFactura);
                var xmlDocument = generator.GenerateXml(factura, cliente.IdPersonaNavigation, emisor);

                // Para guardar el XML en un archivo:
                xmlDocument.Save("factura.xml");

                // Para obtener el XML como string:
                string xmlString = xmlDocument.ToString();
                string xmlFirmado=generator.FirmarXml(xmlString,emisor.CertificadoDigital,emisor.Clave);
                /*var (estado, descripcion) = await generator.EnviarXmlFirmadoYProcesarRespuesta(emisor.TipoAmbiente, xmlFirmado, factura.IdFactura);
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
                */
                factura.Xml = xmlFirmado;
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
                Notificacion("Error al guardar el registro",NotificacionTipo.Error);
                return View(factura);
            }
        }

       
        // GET: Factura/Delete/5
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

        // POST: Factura/Delete/5
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
        public IActionResult GetProductoDetails(int idProducto)
        {
            var producto = _context.Productos.Include(p => p.IdImpuestoNavigation).FirstOrDefault(p => p.IdProducto == idProducto);

            if (producto == null)
            {
                return NotFound();
            }

            return Json(new { precioUnitario = producto.PrecioUnitario, porcentaje = producto.Porcentaje });
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
        [HttpPost]
        public IActionResult GenerarXml(int id)
        {
            var factura = _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .ThenInclude(c => c.IdPersonaNavigation)
                .FirstOrDefault(f => f.IdFactura == id);

            if (factura == null)
            {
                return NotFound();
            }

            // Obtener el contenido del XML de la factura
            string xmlContent = factura.Xml;

            // Convertir el contenido del XML a un array de bytes
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(xmlContent);

            // Retorna el archivo como descarga
            return File(textBytes, "text/plain", "Factura.xml");
        }

        [HttpPost]
        public async Task<IActionResult>  ReenviarSri(int id)
        {
            var generator = new FacturaXmlGenerator(_configuration);
            Factura factura=_context.Facturas.Where(f => f.IdFactura==id).Include(f=>f.IdEmisorNavigation).FirstOrDefault();
            var (estado, descripcion) = await generator.EnviarXmlFirmadoYProcesarRespuesta(factura.IdEmisorNavigation.TipoAmbiente, factura.Xml, factura.IdFactura);
            factura.DescripcionSri = descripcion;
            factura.Estado = estado;            
            _context.Update(factura);
            _context.SaveChanges();
            Notificacion("Registro guardado con exito", NotificacionTipo.Success);
            ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).ToList();
            ViewBag.Productos = _context.Productos.ToList();
            return RedirectToAction("Index", "Factura");
        }
        [HttpGet]
        public IActionResult VerificarStock(int idProducto, int cantidad)
        {
            var producto = _context.Productos.Include(p => p.IdImpuestoNavigation).FirstOrDefault(p => p.IdProducto == idProducto);

            Inventario ultimoMovimiento = _context.Inventarios
                .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E") && i.IdProducto == idProducto)
                .OrderByDescending(i => i.FechaCreacion)
                .FirstOrDefault();

            if (ultimoMovimiento == null || ultimoMovimiento.Stock < cantidad)
            {
                return Json(new { disponible = false, mensaje = "Stock no disponible o insuficiente " +producto.Nombre });
            }

            return Json(new { disponible = true, precioUnitario = producto.PrecioUnitario, porcentaje = producto.Porcentaje });
        }

    }
}