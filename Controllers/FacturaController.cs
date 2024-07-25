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
            var contableContext = _context.Facturas.Where(f=>f.IdEmisor==emisor.IdEmisor).Include(f => f.IdClienteNavigation);
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
            ViewBag.Clientes = _context.Clientes.Include(p=>p.IdPersonaNavigation).Where(p=>p.IdEmpresa==empresa.IdEmpresa).ToList();
            ViewBag.Productos = _context.Productos.Where(p=>p.IdEmpresa==empresa.IdEmpresa & p.Stock>0).Include(p => p.IdImpuestoNavigation).ToList();
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
               _context.Facturas.Add(factura);
                _context.SaveChanges();
                foreach (var pago in pagos)
                {
                    Pago pago1 = new Pago();
                    pago1.IdFactura = factura.IdFactura;
                    pago1.IdTipoPago = pago.IdTipoPago;
                    pago1.Monto=pago.Monto;
                    pago1.Fecha = new DateOnly();
                    pago1.UsuarioCreacion=int.Parse(idUsuario);
                    pago1.FechaCreacion=DateTime.Now;
                    _context.Add(pago1);
                    _context.SaveChanges();
                }
                    foreach (var detalle in detalles)
                {
                    Inventario ultimoMovimiento = _context.Inventarios
   .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E") & i.IdProducto==detalle.IdProducto)
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
                    producto.Stock= ultimoMovimiento.Stock-detalle.Cantidad;
                    ultimoMovimiento.Stock= ultimoMovimiento.Stock - detalle.Cantidad;
                    _context.Productos.Update(producto);
                    Inventario inventario = new Inventario();
                    inventario.Cantidad=detalle.Cantidad;
                    inventario.IdProducto= detalle.IdProducto;
                    inventario.TipoMovimiento = "S";
                    inventario.Stock= ultimoMovimiento.Stock;
                    inventario.FechaMovimiento = new DateTime();
                    inventario.UsuarioCreacion=int.Parse(idUsuario);
                    inventario.Descripcion = "EGRESO POR VENTA FACTURA "+factura.IdFactura;
                    _context.Inventarios.Add(inventario);
                }

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
    }
}