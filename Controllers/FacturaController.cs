using System;
using System.Collections.Generic;  using System.ComponentModel;
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
using System.Net;
using System.Net.Mail;
using ClosedXML.Excel;
using Irony.Parsing;
using DocumentFormat.OpenXml.Office2013.Excel;
namespace ContaFacil.Controllers
{
    public class FacturaController : NotificacionClass
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        private readonly TareaRegistroTransacciones _tareaRegistroTransacciones;

        string idUsuario ="";
        string idEmpresa="";
        public FacturaController(ContableContext context, IConfiguration configuration, TareaRegistroTransacciones tareaRegistroTransacciones)
        {            
            _context = context;
            _configuration = configuration;
            _tareaRegistroTransacciones= tareaRegistroTransacciones;
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
            var contableContext = _context.Facturas.Where(f=>f.IdEmisor==emisor.IdEmisor & f.IdSucursal==usuarioSucursal.IdSucursal).Include(f => f.IdClienteNavigation).ThenInclude(c=>c.IdPersonaNavigation);
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
                .Include(f => f.IdClienteNavigation).ThenInclude(f=>f.IdPersonaNavigation)
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
             .Where(p => p.IdEmpresa == empresa.IdEmpresa && idProductos.Contains(p.IdProducto) && p.EstadoBoolean==true)
             .Include(p => p.IdImpuestoNavigation)
             .Select(p => new
             {
                 p.IdProducto,
                 p.Codigo,
                 NombreCompleto = $"{p.Nombre} - {p.Descripcion}",
                 p.Porcentaje,
                 p.Descuento
             })
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
                Factura facturaCreada;
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    idUsuario = HttpContext.Session.GetString("_idUsuario");

                    var usuario = await _context.Usuarios
                        .Where(u => u.IdUsuario == int.Parse(idUsuario))
                        .Include(p => p.IdPersonaNavigation)
                        .FirstOrDefaultAsync();

                    var emisor = await _context.Emisors
                        .FirstOrDefaultAsync(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion);

                    var empresa = await _context.Empresas
                        .FirstOrDefaultAsync(e => e.Identificacion == emisor.Ruc);

                    var us = await _context.UsuarioSucursals
                        .FirstOrDefaultAsync(u => u.IdUsuario == int.Parse(idUsuario));

                    var sucursalInventario = await _context.SucursalInventarios.Include(s=>s.IdSucursalNavigation)
                        .FirstOrDefaultAsync(s => s.IdSucursal == us.IdSucursal);

                    var ultimoFactura = await _context.Facturas
                        .Where(f => f.IdEmisor == emisor.IdEmisor && f.IdSucursal == us.IdSucursal)
                        .OrderByDescending(i => i.FechaCreacion)
                        .FirstOrDefaultAsync();

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
                        secuencialStr = partes[2];
                        emisor.Secuencial = secuencialStr;
                    }
                    else
                    {
                        factura.NumeroFactura = emisor.Establecimiento + "-" + sucursalInventario.IdSucursalNavigation.PuntoEmision + "-" + emisor.Secuencial;
                    }

                    factura.Fecha = DateOnly.FromDateTime(DateTime.Now);
                    factura.FechaCreacion = DateTime.Now;
                    factura.UsuarioCreacion = int.Parse(idUsuario);
                    factura.Estado = "Pendiente";
                    factura.EstadoBoolean = true;
                    factura.IdEmisor = emisor.IdEmisor;
                    factura.IdSucursal = sucursalInventario.IdSucursal;

                    _context.Facturas.Add(factura);
                    await _context.SaveChangesAsync();

                    foreach (var detalle in detalles)
                    {
                        var inventario = new Inventario();
                        var ultimoMovimiento = await _context.Inventarios
                            .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T" || i.TipoMovimiento == "C" || i.TipoMovimiento == "V") && i.IdProducto == detalle.IdProducto && i.IdSucursal == us.IdSucursal)
                            .OrderByDescending(i => i.FechaCreacion)
                            .FirstOrDefaultAsync();

                        var transaccionMovimiento = await _context.Inventarios
                            .Where(i => (i.TipoMovimiento == "E" || i.TipoMovimiento == "C") && i.IdProducto == detalle.IdProducto && i.IdSucursal == us.IdSucursal)
                            .OrderByDescending(i => i.FechaCreacion)
                            .FirstOrDefaultAsync();

                        if (ultimoMovimiento == null)
                        {
                            sucursalInventario = await _context.SucursalInventarios
                                .FirstOrDefaultAsync(s => s.IdInventario == ultimoMovimiento.IdInventario);

                            if (sucursalInventario == null)
                            {
                                Notificacion("No existe inventario del producto", NotificacionTipo.Warning);
                                return RedirectToAction("Index", "Factura");
                            }
                        }
                        else if (ultimoMovimiento.Stock < detalle.Cantidad)
                        {
                            Notificacion("Producto no disponible", NotificacionTipo.Warning);
                            return RedirectToAction("Index", "Factura");
                        }

                        var producto = await _context.Productos
                            .Include(p => p.IdCategoriaProductoNavigation)
                            .FirstOrDefaultAsync(p => p.IdProducto == detalle.IdProducto);

                        detalle.IdFactura = factura.IdFactura;
                        detalle.FechaCreacion = DateTime.Now;
                        detalle.UsuarioCreacion = int.Parse(idUsuario);
                        detalle.Estado = true;
                        _context.DetalleFacturas.Add(detalle);

                        var cuentum = await _context.Cuenta
                            .FirstOrDefaultAsync(c => c.Nombre.Equals(producto.IdCategoriaProductoNavigation.Nombre));

                        int stock = (ultimoMovimiento.Stock - detalle.Cantidad) ?? 0;

                        inventario.Cantidad = detalle.Cantidad;
                        inventario.IdProducto = detalle.IdProducto;
                        inventario.TipoMovimiento = "V";
                        inventario.Stock = stock;
                        inventario.FechaMovimiento = DateTime.Now;
                        inventario.UsuarioCreacion = int.Parse(idUsuario);
                        inventario.Descripcion = "EGRESO POR VENTA FACTURA " + factura.NumeroFactura;
                        inventario.IdSucursal = factura.IdSucursal;
                        inventario.IdCuentaContable = cuentum.IdCuenta;
                        inventario.NumeroDespacho = ObtenerNumeroDespacho(inventario.TipoMovimiento);

                        decimal totalDescuento = detalle.Descuento ?? 0;
                        decimal totalValorUnitario = detalle.PrecioUnitario;
                        decimal subtotal = (totalValorUnitario * detalle.Cantidad) - totalDescuento;
                        inventario.Descuento = totalDescuento;
                        inventario.Iva = subtotal * 0.15m;
                        inventario.Total = subtotal;
                        inventario.SubTotal = subtotal;
                        inventario.NumeroFactura = factura.NumeroFactura;

                        _context.Inventarios.Add(inventario);
                        await _context.SaveChangesAsync();

                        var si = new SucursalInventario
                        {
                            IdSucursal = sucursalInventario.IdSucursal,
                            IdInventario = inventario.IdInventario,
                            EstadoBoolean = true,
                            UsuarioCreacion = int.Parse(idUsuario),
                            FechaCreacion = DateTime.Now
                        };
                        _context.Add(si);
                        await _context.SaveChangesAsync();
                    }

                    var cliente = await _context.Clientes
                        .Where(c => c.IdCliente == factura.IdCliente)
                        .Include(c => c.IdPersonaNavigation)
                            .ThenInclude(p => p.IdTipoIdentificacionNavigation)
                        .FirstOrDefaultAsync();

                    emisor = await _context.Emisors.FirstOrDefaultAsync(e => e.IdEmisor == factura.IdEmisor);
                    if (!string.IsNullOrEmpty(secuencialStr))
                    {
                        emisor.Secuencial = secuencialStr;
                    }

                    var generator = new FacturaXmlGenerator(_configuration);
                    var factu = await _context.Facturas
                        .Include(f => f.Pagos)
                            .ThenInclude(pf => pf.IdTipoPagoNavigation)
                        .Include(f => f.DetalleFacturas)
                            .ThenInclude(df => df.IdProductoNavigation)
                                .ThenInclude(p => p.IdImpuestoNavigation)
                        .FirstOrDefaultAsync(f => f.IdFactura == factura.IdFactura);

                    var xmlDocument = generator.GenerateXml(factu, cliente.IdPersonaNavigation, emisor);
                    xmlDocument.Save("factura.xml");
                    string xmlString = xmlDocument.ToString();
                    string xmlFirmado = generator.FirmarXml(xmlString, emisor.CertificadoDigital, emisor.Clave);

                    XDocument xdoc = XDocument.Parse(xmlString);
                    string claveAcceso = (string)xdoc.Root
                                                .Element("infoTributaria")
                                                .Element("claveAcceso");
                    factura.ClaveAcceso = claveAcceso;
                    factura.Xml = xmlFirmado;
                    _context.Update(factura);
                    await _context.SaveChangesAsync();

                    facturaCreada = factura;
                    await transaction.CommitAsync();
                }

                // Operaciones fuera de la transacción principal
                await EnviarRIDEPorEmail(facturaCreada.IdFactura);

                // Usar un nuevo contexto para registrarTransaccionesVenta
                using (var nuevoContexto = new ContableContext())
                {
                    await registrarTransaccionesVenta(nuevoContexto, facturaCreada);
                }

                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);

                return RedirectToAction("Index", "Factura");
            }
            catch (Exception ex)
            {
                Notificacion("Error al guardar el registro: " + ex.Message, NotificacionTipo.Error);
                ViewBag.Clientes = await _context.Clientes.ToListAsync();
                ViewBag.Productos = await _context.Productos.ToListAsync();
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

            return Json(new
            {
                precioUnitario = producto.PrecioVenta,
                porcentaje = producto.Porcentaje,
                descuento = producto.Descuento
            });
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
            var result = await generator.ConsultarAutorizacionAsync(factura.ClaveAcceso, factura.IdEmisorNavigation.TipoAmbiente);

            var (est, fechaAutorizacion) = result;

            factura.AutorizacionSri = est;
            factura.FechaAutorizacionSri = fechaAutorizacion;
            _context.Update(factura);
            await _context.SaveChangesAsync();

            Notificacion("Registro guardado con exito", NotificacionTipo.Success);
            ViewBag.Clientes = _context.Clientes.Include(p => p.IdPersonaNavigation).ToList();
            ViewBag.Productos = _context.Productos.ToList();
            return RedirectToAction("Index", "Factura");
        }
        [HttpGet]
        public IActionResult VerificarStock(int idProducto, int cantidad)
        {
            var producto = _context.Productos.Include(p => p.IdImpuestoNavigation).FirstOrDefault(p => p.IdProducto == idProducto);
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.Where(s => s.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
            Inventario ultimoMovimiento = _context.Inventarios
                .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T" || i.TipoMovimiento == "V" || i.TipoMovimiento == "C") && i.IdProducto == idProducto && i.IdSucursal==usuarioSucursal.IdSucursal)
                .OrderByDescending(i => i.FechaCreacion)
                .FirstOrDefault();

            if (ultimoMovimiento == null || ultimoMovimiento.Stock < cantidad)
            {
                return Json(new { disponible = false, mensaje = "Stock no disponible o insuficiente " +producto.Nombre + ". Saldo actual " + ultimoMovimiento.Stock });
            }

            return Json(new { disponible = true, precioUnitario = producto.PrecioVenta, porcentaje = producto.Porcentaje });
        }

        [HttpPost]
        public async Task<IActionResult> EnviarRIDEPorEmail(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .ThenInclude(c => c.IdPersonaNavigation)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (factura == null)
            {
                return NotFound();
            }

            // Generar el PDF
            GeneradorRIDE generador = new GeneradorRIDE();
            var pdfBytes = generador.GenerarRIDE(factura.Xml);

            // Configurar el cliente de correo electrónico
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("wherrera.web@gmail.com", "eysx ogkx abxj uaoy"),
                EnableSsl = true,
            };

            // Crear el mensaje de correo
            var mailMessage = new MailMessage
            {
                From = new MailAddress("wherrea.web@gmail.com", "FINANSYS"),
                Subject = $"Factura RIDE - {factura.NumeroFactura}",
                Body = $"Estimado cliente,\n\nAdjunto encontrará la factura RIDE número {factura.NumeroFactura}.\n\nGracias por su preferencia.",
                IsBodyHtml = false,
            };

            // Agregar el destinatario (asumiendo que el email del cliente está en la propiedad Email)
            mailMessage.To.Add(factura.IdClienteNavigation.IdPersonaNavigation.Email);

            // Adjuntar el PDF
            var attachment = new Attachment(new MemoryStream(pdfBytes), "Factura.pdf", "application/pdf");
            mailMessage.Attachments.Add(attachment);

            try
            {
                // Enviar el correo
                await smtpClient.SendMailAsync(mailMessage);
                return Ok("Correo enviado exitosamente");
            }
            catch (Exception ex)
            {
                // Manejar cualquier error en el envío
                return StatusCode(500, $"Error al enviar el correo: {ex.Message}");
            }
            finally
            {
                // Limpiar recursos
                attachment.Dispose();
                mailMessage.Dispose();
            }
        }
        // GET: Factura/Details/5
        public async Task<IActionResult> NotaCredito(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.IdClienteNavigation).ThenInclude(f => f.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }
        // POST: Factura/Delete/5
        [HttpPost]
        public IActionResult NotaConfirmed(int id,string motivo)
        {
            var generator = new NotaCreditoXmlGenerator(_configuration);
            Factura factura=new Factura();
            factura=_context.Facturas.Include(f=>f.DetalleFacturas).ThenInclude(f=>f.IdProductoNavigation).ThenInclude(f=>f.IdImpuestoNavigation).Include(f=>f.IdEmisorNavigation). FirstOrDefault(f => f.IdFactura == id);
            Cliente cliente = new Cliente();
            cliente=_context.Clientes.Include(c=>c.IdPersonaNavigation).ThenInclude(c => c.IdTipoIdentificacionNavigation).FirstOrDefault(f => f.IdCliente == factura.IdCliente);
            var xmlDocument= generator.GenerateXml(factura,cliente.IdPersonaNavigation,factura.IdEmisorNavigation,motivo);
            xmlDocument.Save("notaCredito.xml");
            return RedirectToAction(nameof(Index));
        }
        public string ObtenerNumeroDespacho(string tipoMovimiento)
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
        private string ObtenerSiguienteNumeroAsiento()
        {
            var ultimaTransaccion =  _context.Transaccions
                .OrderByDescending(t => t.IdTransaccion)
                .FirstOrDefault();

            if (ultimaTransaccion == null)
            {
                return "AS0001";
            }

            string ultimoNumeroAsiento = ultimaTransaccion.Descripcion.Split(' ')[0];
            if (ultimoNumeroAsiento.StartsWith("AS"))
            {
                int numero = int.Parse(ultimoNumeroAsiento.Substring(2)) + 1;
                return $"AS{numero:D4}";
            }
            else
            {
                return "AS0001";
            }
        }
        public IActionResult PrincipalFactura()
        {
            // Aquí puedes agregar cualquier lógica adicional que necesites antes de devolver la vista
            // Por ejemplo, podrías cargar algunos datos desde la base de datos y pasarlos a la vista

            return View(); // Esto devolverá la vista PrincipalProducto.cshtml
        }
        [HttpPost]
        public async Task<IActionResult> ExportFacturaExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            try
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

                if (fechaFin == default)
                {
                    TempData["Error"] = "La fecha de corte es obligatoria.";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"Buscando facturas para la empresa {empresa.IdEmpresa} entre {fechaInicio} y {fechaFin}");

                // Obtener las facturas para la empresa en el período especificado
                var facturas = await _context.Facturas
                    .Where(f => f.FechaCreacion >= fechaInicio && f.FechaCreacion <= fechaFin && f.IdEmisor == emisor.IdEmisor)
                    .OrderBy(f=>f.FechaCreacion)
                    .ToListAsync();

                Console.WriteLine($"Número de facturas encontradas: {facturas.Count}");

                if (facturas.Count == 0)
                {
                    TempData["Error"] = "No se encontraron facturas para el período seleccionado.";
                    return RedirectToAction(nameof(Index));
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Reporte de Facturación");
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

                    worksheet.Cell("C1").Value = "Reporte de Facturación";
                    worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
                    worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
                    worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";
                    worksheet.Cell("C5").Value = $"Fecha de corte: {fechaFin:dd/MM/yyyy}";

                    int currentRow = 8;
                    worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Número de Factura";
                    worksheet.Cell(currentRow, 2).Value = "Fecha";
                    worksheet.Cell(currentRow, 3).Value = "Cliente";
                   // worksheet.Cell(currentRow, 4).Value = "Subtotal";
                   // worksheet.Cell(currentRow, 5).Value = "IVA";
                    worksheet.Cell(currentRow, 4).Value = "Total";
                    worksheet.Range(currentRow, 1, currentRow, 4).Style.Font.Bold = true;
                    worksheet.Range(currentRow, 1, currentRow, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                    currentRow++;

                    foreach (var factura in facturas)
                    {
                        worksheet.Cell(currentRow, 1).Value = factura.NumeroFactura;
                        worksheet.Cell(currentRow, 2).Value = factura.Fecha.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 3).Value = factura.IdEmisorNavigation.RazonSocial;
                       // worksheet.Cell(currentRow, 4).Value = factura.Subtotal;
                        //worksheet.Cell(currentRow, 5).Value = (factura.Subtotal*0.15m)+ factura.Subtotal;
                        worksheet.Cell(currentRow, 4).Value = factura.MontoTotal;

                        worksheet.Range(currentRow, 1, currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        currentRow++;
                    }

                    worksheet.Columns().AdjustToContents();
                    var rangeNumerica = worksheet.Range(9, 4, currentRow - 1, 4);
                    rangeNumerica.Style.NumberFormat.Format = "#,##0.00";

                    Console.WriteLine("Preparando el archivo Excel para descarga");

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        Console.WriteLine($"Tamaño del archivo Excel generado: {content.Length} bytes");
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteFacturacion.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el archivo Excel: {ex.Message}");
                TempData["Error"] = "Hubo un error al generar el archivo Excel.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportNotasCreditoExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            ContaFacil.Models.Sucursal sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == usuarioSucursal.IdSucursal);
            Persona persona = _context.Personas.FirstOrDefault(p => p.IdPersona == usuario.IdPersona);
            Emisor emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == persona.Identificacion);
            Empresa empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);

            if (fechaFin == default)
            {
                TempData["Error"] = "La fecha de corte es obligatoria.";
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine($"Buscando notas de crédito para la empresa {empresa.IdEmpresa} entre {fechaInicio} y {fechaFin}");

            var notasCredito = await _context.NotaCreditos
                .Where(nc => nc.FechaCreacion >= fechaInicio && nc.FechaCreacion <= fechaFin && nc.IdEmpresa == empresa.IdEmpresa)
                //.Include(nc => nc.IdFacturaNavigation)
                .OrderBy(nc=>nc.FechaCreacion)
                .ToListAsync();

            Console.WriteLine($"Número de notas de crédito encontradas: {notasCredito.Count}");

            if (notasCredito.Count == 0)
            {
                Notificacion("No se encontraron notas de crédito para el período seleccionado.", NotificacionTipo.Warning);
                return RedirectToAction(nameof(PrincipalFactura));
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Notas de Crédito");
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                var logo = worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.25);

                worksheet.Cell("C1").Value = "Notas de Crédito";
                worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
                worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
                worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";
                worksheet.Cell("C5").Value = $"Fecha de corte: {fechaFin:dd/MM/yyyy}";

                int currentRow = 8;

                worksheet.Cell(currentRow, 1).Value = "Número Nota";
                worksheet.Cell(currentRow, 2).Value = "Número Autorización";
                worksheet.Cell(currentRow, 3).Value = "Clave Acceso";
                worksheet.Cell(currentRow, 4).Value = "Motivo";
                worksheet.Cell(currentRow, 5).Value = "Descripción";
                worksheet.Cell(currentRow, 6).Value = "Fecha Creación";
                worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                worksheet.Range(currentRow, 1, currentRow, 6).Style.Fill.BackgroundColor = XLColor.LightGray;
                currentRow++;

                foreach (var notaCredito in notasCredito)
                {
                    worksheet.Cell(currentRow, 1).Value = notaCredito.NumeroNota;
                    worksheet.Cell(currentRow, 2).Value = notaCredito.NumeroAutorizacion;
                    worksheet.Cell(currentRow, 3).Value = notaCredito.ClaveAcceso;
                    worksheet.Cell(currentRow, 4).Value = notaCredito.Motivo;
                    worksheet.Cell(currentRow, 5).Value = notaCredito.Descripcion;
                    worksheet.Cell(currentRow, 6).Value = notaCredito.FechaCreacion.ToString("dd/MM/yyyy");

                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();
                var rangeNumerica = worksheet.Range(8, 1, currentRow - 1, 6);
                rangeNumerica.Style.NumberFormat.Format = "@"; // Format text fields as text

                Console.WriteLine("Preparando el archivo Excel para descarga");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    Console.WriteLine($"Tamaño del archivo Excel generado: {content.Length} bytes");
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NotasCredito.xlsx");
                }
            }
        }
        private async Task registrarTransaccionesVenta(ContableContext context, Factura factura)
        {
            var sucursal = await _context.Sucursals
             .FirstOrDefaultAsync(s => s.IdSucursal == factura.IdSucursal);
            idUsuario = HttpContext.Session.GetString("_idUsuario");
            var usuario = await _context.Usuarios
                .Where(u => u.IdUsuario == int.Parse(idUsuario))
                .Include(p => p.IdPersonaNavigation)
                .FirstOrDefaultAsync();

            var emisor = await _context.Emisors
                .FirstOrDefaultAsync(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion);

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.Identificacion == emisor.Ruc);

            string numeroAsiento = ObtenerSiguienteNumeroAsiento();
            var tipoTransaccion = await _context.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == "Venta");

            var listaInventarios = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .ThenInclude(i => i.IdCategoriaProductoNavigation)
                .Where(i => i.NumeroFactura.Equals(factura.NumeroFactura))
                .ToListAsync();

            decimal iva = listaInventarios.Sum(li => li.Iva) ?? 0;
            decimal des = listaInventarios.Sum(df => df.Descuento) ?? 0;

            var lista = await context.Pagos
                .Where(p => p.IdFactura == factura.IdFactura)
                .Include(p => p.IdTipoPagoNavigation)
                .ToListAsync();

            var listaCategorias = listaInventarios
                .Select(i => i.IdProductoNavigation.IdCategoriaProductoNavigation)
                .Distinct()
                .ToList();

            string descripcion = factura.NumeroFactura;

            // Transacción descuento
            await CrearTransaccion("Descuento en ventas", numeroAsiento + " Venta de " + descripcion, des, tipoTransaccion, empresa, usuario);

            // Transacción IVA
            await CrearTransaccion("IVA por pagar", numeroAsiento + " IVA por pagar en venta de " + descripcion, iva, tipoTransaccion, empresa, usuario);

           

            // Transacciones por tipo de pago
            foreach (var pag in lista)
            {
                string cuentaCodigo = pag.IdTipoPagoNavigation.Nombre switch
                {
                    var s when s.Contains("EFECTIVO") => "1.1.1.1",
                    var s when s.Contains("CREDITO") => "1.1.1.3",
                    var s when s.Contains("TRANSFERENCIA") => "1.1.1.2",
                    _ => throw new ArgumentException("Tipo de pago no reconocido")
                };

                await CrearTransaccion(cuentaCodigo, numeroAsiento + " Venta de " + descripcion, pag.Monto, tipoTransaccion, empresa, usuario);
            }
            // Transacciones por categoría
            foreach (var cat in listaCategorias)
            {
                var inventariosFiltrados = listaInventarios
                    .Where(i => i.IdProductoNavigation.IdCategoriaProductoNavigation.IdCategoriaProducto == cat.IdCategoriaProducto)
                    .ToList();

                decimal totalCategoria = inventariosFiltrados.Sum(i => i.Total) ?? 0;
                decimal descuentoFiltrado = inventariosFiltrados.Sum(i => i.Descuento) ?? 0;

                await CrearTransaccion("Ingreso " + cat.Nombre, numeroAsiento + " Venta de " + cat.Nombre + " " + descripcion, totalCategoria + descuentoFiltrado, tipoTransaccion, empresa, usuario);
                
                //calculo de  valor desde ultimo ingreso del kardex
               // List<DetalleFactura> detalles = factura.DetalleFacturas;
            }
            foreach (var det in factura.DetalleFacturas)
            {
                numeroAsiento = ObtenerSiguienteNumeroAsiento();
                Inventario inventario = await context.Inventarios
                .Where(i => (i.TipoMovimiento == "S" || i.TipoMovimiento == "E" || i.TipoMovimiento == "T" || i.TipoMovimiento == "C") && i.IdProducto == det.IdProducto && i.IdSucursal == factura.IdSucursal)
                .OrderByDescending(i => i.FechaCreacion).Include(i => i.IdProductoNavigation).ThenInclude(i=>i.IdCategoriaProductoNavigation)
                .FirstOrDefaultAsync();
                decimal precioCalculo = inventario.PrecioCalculo ?? 0;
                decimal cantidad = det.Cantidad;
                decimal valor = precioCalculo * cantidad;
                await CrearTransaccion("Costo " + inventario.IdProductoNavigation.IdCategoriaProductoNavigation.Nombre, numeroAsiento + " Venta de mercadería " + inventario.IdProductoNavigation.Nombre + " " + descripcion, valor, tipoTransaccion, empresa, usuario);
                await CrearTransaccion(inventario.IdProductoNavigation.IdCategoriaProductoNavigation.Nombre, numeroAsiento + " Venta de mercadería " + inventario.IdProductoNavigation.Nombre + " " + descripcion, valor, tipoTransaccion, empresa, usuario);
            }
        }

        private async Task CrearTransaccion(string cuentaNombre, string descripcion, decimal monto, TipoTransaccion tipoTransaccion, Empresa empresa, Usuario usuario)
        {
            var cuenta = await _context.Cuenta.FirstOrDefaultAsync(c => c.Nombre == cuentaNombre || c.Codigo == cuentaNombre);

            var transaccion = new Transaccion
            {
                IdCuenta = cuenta.IdCuenta,
                Descripcion = descripcion,
                IdInventario = 0,
                Monto = monto,
                IdTipoTransaccion = tipoTransaccion.IdTipoTransaccion,
                IdEmpresa = empresa.IdEmpresa,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = usuario.IdUsuario,
                Estado = true
            };

            _context.Add(transaccion);
            await _context.SaveChangesAsync();
        }
    }
}