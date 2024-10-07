using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Utilities;
using System.Xml.Linq;
using ContaFacil.Logica;
using ContaFacil.Models.ViewModel;

namespace ContaFacil.Controllers
{
    public class RetencionController : NotificacionClass
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        public RetencionController(ContableContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Retencion
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Retencions.Include(r => r.IdEmpresaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Retencion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retencion = await _context.Retencions
                .Include(r => r.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdRetencion == id);
            if (retencion == null)
            {
                return NotFound();
            }

            return View(retencion);
        }

        // GET: Retencion/Create
        public async Task<IActionResult> Create()
        {
            var impuestosRetencionIVA = await _context.Impuestos
                .Where(i => i.Nombre.ToUpper().Contains("RETENCION") && i.Nombre.ToUpper().Contains("IVA"))
                .ToListAsync();

            var impuestosRetencionRenta = await _context.Impuestos
                .Where(i => i.Nombre.ToUpper().Contains("RETENCION") && !i.Nombre.ToUpper().Contains("IVA"))
                .ToListAsync();

            ViewData["PorcentajeRetencionIVA"] = new SelectList(impuestosRetencionIVA, "Porcentaje", "Nombre");
            ViewData["PorcentajeRetencionRT"] = new SelectList(impuestosRetencionRenta, "Porcentaje", "Nombre");

            return View(new RetencionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> BuscarFactura(string numeroFactura)
        {
            var factura = await _context.Facturas
                .Include(f => f.IdClienteNavigation.IdPersonaNavigation)
                .ThenInclude(p => p.IdEmpresaNavigation)
                .FirstOrDefaultAsync(f => f.NumeroFactura == numeroFactura);

            if (factura == null)
            {
                return Json(new { success = false, message = "Factura no encontrada" });
            }

            var inventarios = await _context.Inventarios
                .Where(i => i.NumeroFactura == numeroFactura)
                .ToListAsync();

            if (!inventarios.Any())
            {
                return Json(new { success = false, message = "Inventario no encontrado para esta factura" });
            }

            decimal totalIva = inventarios.Sum(i => i.Iva ?? 0);
            decimal totalBase = inventarios.Sum(i => i.Total ?? 0);

            var retencionViewModel = new RetencionViewModel
            {
                NumeroFactura = factura.NumeroFactura,
                EjercicioFiscal = "", // You might want to set this based on the factura data
                BaseImponibleIVA = totalIva,
                BaseImponibleRT = totalBase,
                PorcentajeRetencionIVA = factura.IdClienteNavigation.IdPersonaNavigation.RetencionIva,
                PorcentajeRetencionRT = factura.IdClienteNavigation.IdPersonaNavigation.RetencionPorcentaje,
                ValorRetenidoIVA = totalIva * (factura.IdClienteNavigation.IdPersonaNavigation.RetencionIva / 100),
                ValorRetenidoRT = totalBase * (factura.IdClienteNavigation.IdPersonaNavigation.RetencionPorcentaje / 100)
            };

            return Json(new { success = true, data = retencionViewModel });
        }
        // POST: Retencion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RetencionViewModel retencion)
        {
            try
            {
                Factura factura = _context.Facturas.Include(f=>f.IdClienteNavigation.IdPersonaNavigation).ThenInclude(f=>f.IdEmpresaNavigation).FirstOrDefault(f=>f.NumeroFactura.Equals(retencion.NumeroFactura));
                Empresa empresa = factura.IdClienteNavigation.IdEmpresaNavigation;
                Cliente cliente = factura.IdClienteNavigation;
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
                Emisor emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == factura.IdClienteNavigation.IdEmpresaNavigation.Identificacion);
                Retencion ret = _context.Retencions.Where(r => r.IdEmpresa == empresa.IdEmpresa).OrderByDescending(r => r.FechaCreacion).FirstOrDefault();
                List<Retencion> retencions=new List<Retencion>();
                 ContableContext ctx = new ContableContext();
                    Inventario inventario = ctx.Inventarios.FirstOrDefault(i => i.NumeroFactura.Equals(factura.NumeroFactura));
                    var retencionXml = new RetencionXmlClienteGenerator(_configuration);
                string numeroRetencion = "";
                if (ret != null)
                {
                    numeroRetencion = GenerarNumeroComprobanteRetencion(ret.ComprobanteRetencion);
                }
                else
                {
                    numeroRetencion = "001-001-000000001";
                }
                string numeroAsiento = ObtenerSiguienteNumeroAsiento();
                if (retencion.ValorRetenidoRT > 0)
                    {
                        
                        var retencionRenta = new Retencion
                        {
                            IdEmpresa = empresa.IdEmpresa,
                            IdFactura = factura.IdFactura,
                            IdProveedor = factura.IdCliente,
                            NumeroFactura = factura.NumeroFactura,
                            ComprobanteRetencion = numeroRetencion,
                            EjercicioFiscal = DateTime.Now.Year.ToString(),
                            BaseImponible = factura.Subtotal ?? 0,
                            EstadoBoolean = true,
                            FechaCreacion = DateTime.Now,
                            UsuarioCreacion = usuario.IdUsuario,
                            Impuesto = "RENTA",
                            PorcentajeRetencion = cliente.IdPersonaNavigation.RetencionPorcentaje,
                            ValorRetenido = (factura.Subtotal ?? 0) * (cliente.IdPersonaNavigation.RetencionPorcentaje / 100)
                        };

                        
                        await ctx.Retencions.AddAsync(retencionRenta);
                        
                        cliente.IdPersonaNavigation.RetencionPorcentaje = Math.Truncate(cliente.IdPersonaNavigation.RetencionPorcentaje);
                        Cuentum cuent = ctx.Cuenta.FirstOrDefault(c => c.Nombre.Contains("Retención IR") && c.Nombre.Contains(cliente.IdPersonaNavigation.RetencionPorcentaje.ToString()) && c.Codigo.Contains("1.1.3."));
                        var tipoTransaccion = await ctx.TipoTransaccions
                        .FirstOrDefaultAsync(t => t.Nombre == "Venta");
                        string descripcion = factura.NumeroFactura;
                        await CrearTransaccion(cuent.Codigo, numeroAsiento + " Venta de " + descripcion, retencionRenta.ValorRetenido ?? 0, tipoTransaccion, empresa, usuario,false);
                    retencions.Add(retencionRenta);
                    }

                    if (retencion.ValorRetenidoIVA > 0)
                    {
                       
                        var retencionIva = new Retencion
                        {
                            IdEmpresa = empresa.IdEmpresa,
                            IdFactura = factura.IdFactura,
                            IdProveedor = factura.IdCliente,
                            NumeroFactura = factura.NumeroFactura,
                            ComprobanteRetencion = numeroRetencion,
                            EjercicioFiscal = DateTime.Now.Year.ToString(),
                            BaseImponible = inventario.Iva ?? 0,
                            EstadoBoolean = true,
                            FechaCreacion = DateTime.Now,
                            UsuarioCreacion = usuario.IdUsuario,
                            Impuesto = "IVA",
                            PorcentajeRetencion = cliente.IdPersonaNavigation.RetencionIva,
                            ValorRetenido = (inventario.Iva ?? 0) * (cliente.IdPersonaNavigation.RetencionIva / 100)
                        };
                        
                        await ctx.Retencions.AddAsync(retencionIva);
                        cliente.IdPersonaNavigation.RetencionPorcentaje = Math.Truncate(cliente.IdPersonaNavigation.RetencionPorcentaje);
                        Cuentum cuent = ctx.Cuenta.FirstOrDefault(c => c.Nombre.Contains("Retención IVA") && c.Nombre.Contains(cliente.IdPersonaNavigation.RetencionPorcentaje.ToString()) && c.Codigo.Contains("1.1.3."));
                        var tipoTransaccion = await ctx.TipoTransaccions
                        .FirstOrDefaultAsync(t => t.Nombre == "Venta");
                        string descripcion = factura.NumeroFactura;
                        await CrearTransaccion(cuent.Codigo, numeroAsiento + " Venta de " + descripcion, retencionIva.ValorRetenido ?? 0, tipoTransaccion, empresa, usuario,false);
                        retencions.Add(retencionIva);
                }
                
                await ctx.SaveChangesAsync();  
                decimal sumRetencion = retencions.Sum(r => r.ValorRetenido)??0;
                Cuentum cuent2 = ctx.Cuenta.FirstOrDefault(c => c.Nombre.Contains("Cuentas por cobrar"));
                var tipoTransaccion2 = await ctx.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == "Venta");
                string descripcion2 = factura.IdClienteNavigation.IdPersonaNavigation.Nombre+" "+ factura.NumeroFactura;
                await CrearTransaccion(cuent2.Codigo, numeroAsiento + " Cuentas por cobrar " + descripcion2, sumRetencion, tipoTransaccion2, empresa, usuario,true);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                   return View(retencion);
            }
            
        }

        // GET: Retencion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retencion = await _context.Retencions.FindAsync(id);
            if (retencion == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", retencion.IdEmpresa);
            return View(retencion);
        }

        // POST: Retencion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRetencion,IdEmpresa,Xml,IdFactura,IdProveedor,NumeroFactura,ComprobanteRetencion,NumeroAutorizacion,ClaveAcceso,EjercicioFiscal,BaseImponible,Impuesto,PorcentajeRetencion,ValorRetenido,TipoContribuyente,EstadoBoolean,FechaAutorizacion,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Retencion retencion)
        {
            if (id != retencion.IdRetencion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(retencion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RetencionExists(retencion.IdRetencion))
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
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", retencion.IdEmpresa);
            return View(retencion);
        }

        // GET: Retencion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retencion = await _context.Retencions
                .Include(r => r.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdRetencion == id);
            if (retencion == null)
            {
                return NotFound();
            }

            return View(retencion);
        }

        // POST: Retencion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var retencion = await _context.Retencions.FindAsync(id);
            if (retencion != null)
            {
                _context.Retencions.Remove(retencion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RetencionExists(int id)
        {
            return _context.Retencions.Any(e => e.IdRetencion == id);
        }
        [HttpPost]
        public IActionResult GenerarRIDE(int id)
        {
            var retencion = _context.Retencions
                .FirstOrDefault(f => f.IdRetencion == id);

            if (retencion == null)
            {
                return NotFound();
            }



            // Llamar al método GenerarRIDE
            GeneradorRIDERetencion generador = new GeneradorRIDERetencion();
            generador.GenerarRIDE(retencion.Xml);
            // Genera el PDF y obtén los bytes
            var pdfBytes = generador.GenerarRIDE(retencion.Xml);

            // Retorna el archivo como descarga
            return File(pdfBytes, "application/pdf", "Retencion.pdf");
        }
        [HttpPost]
        public IActionResult GenerarXml(int id)
        {
            var retencion = _context.Retencions
                .FirstOrDefault(f => f.IdRetencion == id);

            if (retencion == null)
            {
                return NotFound();
            }

            // Obtener el contenido del XML de la factura
            string xmlContent = retencion.Xml;

            // Convertir el contenido del XML a un array de bytes
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(xmlContent);

            // Retorna el archivo como descarga
            return File(textBytes, "text/plain", "Retencion.xml");
        }
        public string GenerarNumeroComprobanteRetencion(string ultimoNumero)
        {
            // Divide el número de factura en partes
            var partes = ultimoNumero.Split('-');

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
        private string ObtenerSiguienteNumeroAsiento()
        {
            var ultimaTransaccion = _context.Transaccions
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
        private async Task CrearTransaccion(string cuentaNombre, string descripcion, decimal monto, TipoTransaccion tipoTransaccion, Empresa empresa, Usuario usuario,Boolean esDebito)
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
                Estado = true,
                EsDebito = esDebito
            };

            _context.Add(transaccion);
            await _context.SaveChangesAsync();
        }

        public IActionResult PrincipalRetencion()
        {
            // Aquí puedes agregar cualquier lógica adicional que necesites antes de devolver la vista
            // Por ejemplo, podrías cargar algunos datos desde la base de datos y pasarlos a la vista

            return View(); // Esto devolverá la vista PrincipalProducto.cshtml
        }
    }
}
