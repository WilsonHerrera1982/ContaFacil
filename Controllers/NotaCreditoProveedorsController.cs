using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Globalization;
using ContaFacil.Logica;

namespace ContaFacil.Controllers
{
    
    public class NotaCreditoProveedorsController : NotificacionClass
    {
        string idUsuario = "";
        string idEmpresa = "";
        private readonly ContableContext _context;

        public NotaCreditoProveedorsController(ContableContext context)
        {
            _context = context;
        }

        // GET: NotaCreditoProveedors
        public async Task<IActionResult> Index()
        {
            return View(await _context.NotaCreditoProveedors.ToListAsync());
        }

        // GET: NotaCreditoProveedors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notaCreditoProveedor = await _context.NotaCreditoProveedors
                .FirstOrDefaultAsync(m => m.IdNotaCredito == id);
            if (notaCreditoProveedor == null)
            {
                return NotFound();
            }

            return View(notaCreditoProveedor);
        }

        // GET: NotaCreditoProveedors/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessXML(IFormFile xmlFile)
        {
            try
            {
                if (xmlFile == null || xmlFile.Length == 0)
                {
                    return Json(new { success = false, message = "No se ha seleccionado ningún archivo" });
                }

                using var streamReader = new StreamReader(xmlFile.OpenReadStream());
                var xmlContent = await streamReader.ReadToEndAsync();

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                var result = new NotaCreditoProveedor
                {
                    // Información Tributaria
                    Ambiente = GetXmlValue(xmlDoc, "ambiente"),
                    TipoEmision = GetXmlValue(xmlDoc, "tipoEmision"),
                    RazonSocial = GetXmlValue(xmlDoc, "razonSocial"),
                    NombreComercial = GetXmlValue(xmlDoc, "nombreComercial"),
                    Ruc = GetXmlValue(xmlDoc, "ruc"),
                    ClaveAcceso = GetXmlValue(xmlDoc, "claveAcceso"),
                    CodDoc = GetXmlValue(xmlDoc, "codDoc"),
                    Estab = GetXmlValue(xmlDoc, "estab"),
                    PtoEmi = GetXmlValue(xmlDoc, "ptoEmi"),
                    Secuencial = GetXmlValue(xmlDoc, "secuencial"),
                    DirMatriz = GetXmlValue(xmlDoc, "dirMatriz"),
                    DirEstablecimiento = GetXmlValue(xmlDoc, "dirEstablecimiento"),

                    // Información del Comprador
                    TipoIdentificacionComprador = GetXmlValue(xmlDoc, "tipoIdentificacionComprador"),
                    RazonSocialComprador = GetXmlValue(xmlDoc, "razonSocialComprador"),
                    IdentificacionComprador = GetXmlValue(xmlDoc, "identificacionComprador"),

                    // Información de Modificación
                    CodDocModificado = GetXmlValue(xmlDoc, "codDocModificado"),
                    NumDocModificado = GetXmlValue(xmlDoc, "numDocModificado"),
                    Motivo = GetXmlValue(xmlDoc, "motivo"),

                    // Fechas
                    // Opción 1: Si estás seguro que el valor nunca será null
                    FechaEmision = DateOnly.FromDateTime(ParseFecha(GetXmlValue(xmlDoc, "fechaEmision")).Value),
                    FechaEmisionDocSustento = DateOnly.FromDateTime(ParseFecha(GetXmlValue(xmlDoc, "fechaEmisionDocSustento")).Value),

                   
                    // Valores
                    TotalSinImpuestos = ParseDecimal(GetXmlValue(xmlDoc, "totalSinImpuestos")),
                    ValorModificacion = ParseDecimal(GetXmlValue(xmlDoc, "valorModificacion")),
                    Moneda = GetXmlValue(xmlDoc, "moneda")
                };
                NotaCreditoProveedor notaCredito=_context.NotaCreditoProveedors.Where(n=>n.NumDocModificado.Equals(result.NumDocModificado)).SingleOrDefault();
                if (notaCredito.IdNotaCredito != null)
                {
                    //Notificacion("Ya existe una nota credito para la factura "+ result.NumDocModificado, NotificacionTipo.Warning);
                    return Json(new { success = false, message = $"Ya existe una nota credito para la factura "+ result.NumDocModificado });
                }
                // Procesar detalles
                var detalles = new List<NotaCreditoDetalle>();
                var detallesNodes = xmlDoc.SelectNodes("//detalle");

                if (detallesNodes != null)
                {
                    foreach (XmlNode detalle in detallesNodes)
                    {
                        var detalleItem = new NotaCreditoDetalle
                        {
                            CodigoInterno = GetXmlValue(detalle, "codigoInterno"),
                            Descripcion = GetXmlValue(detalle, "descripcion"),
                            Cantidad = ParseDecimal(GetXmlValue(detalle, "cantidad")),
                            PrecioUnitario = ParseDecimal(GetXmlValue(detalle, "precioUnitario")),
                            Descuento = ParseDecimal(GetXmlValue(detalle, "descuento")),
                            PrecioTotalSinImpuesto = ParseDecimal(GetXmlValue(detalle, "precioTotalSinImpuesto"))
                        };

                        var impuesto = detalle.SelectSingleNode("impuestos/impuesto");
                        if (impuesto != null)
                        {
                            detalleItem.CodigoImpuesto = GetXmlValue(impuesto, "codigo");
                            detalleItem.CodigoPorcentaje = GetXmlValue(impuesto, "codigoPorcentaje");
                            detalleItem.Tarifa = ParseDecimal(GetXmlValue(impuesto, "tarifa"));
                            detalleItem.BaseImponible = ParseDecimal(GetXmlValue(impuesto, "baseImponible"));
                            detalleItem.ValorImpuesto = ParseDecimal(GetXmlValue(impuesto, "valor"));
                        }

                        detalles.Add(detalleItem);
                    }
                }

                // Procesar información adicional
                var infoAdicional = new List<NotaCreditoInfoAdicional>();
                var camposAdicionales = xmlDoc.SelectNodes("//campoAdicional");

                if (camposAdicionales != null)
                {
                    foreach (XmlNode campo in camposAdicionales)
                    {
                        infoAdicional.Add(new NotaCreditoInfoAdicional
                        {
                            Nombre = campo.Attributes?["nombre"]?.Value ?? "",
                            Valor = campo.InnerText
                        });
                    }
                }

                return Json(new
                {
                    success = true,
                    data = result,
                    detalles = detalles.ToArray(), // Asegúrate de que sea un array
                    infoAdicional = infoAdicional.ToArray() // Asegúrate de que sea un array
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al procesar el XML: {ex.Message}" });
            }
        }

        // POST: NotaCreditoProveedors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotaCreditoProveedor notaCreditoProveedor, List<NotaCreditoDetalle> detalles, List<NotaCreditoInfoAdicional> infoAdicional)
        {
            try
            {
                idUsuario = HttpContext.Session.GetString("_idUsuario");
                idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = new Usuario();
                notaCreditoProveedor.UsuarioCreacion = usuario.IdUsuario;
                notaCreditoProveedor.FechaCreacion = DateTime.Now;
                notaCreditoProveedor.EstadoBoolean = true;
                notaCreditoProveedor.ContribuyenteEspecial = "N";
                notaCreditoProveedor.ObligadoContabilidad = false;
                notaCreditoProveedor.Moneda = "USD";
                _context.Add(notaCreditoProveedor);
                await _context.SaveChangesAsync();
                foreach (var item in infoAdicional)
                {
                    item.EstadoBoolean = true;
                    item.FechaCreacion = DateTime.Now;
                    item.UsuarioCreacion= usuario.IdUsuario;
                    item.IdNotaCredito=notaCreditoProveedor.IdNotaCredito;
                    _context.Add(item);
                }
                foreach(var detalle in detalles)
                {
                    detalle.EstadoBoolean = true;
                    detalle.FechaCreacion= DateTime.Now;
                    detalle.UsuarioCreacion = usuario.IdUsuario;
                    detalle.IdNotaCredito = notaCreditoProveedor.IdNotaCredito;
                    _context.Add(detalle);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(notaCreditoProveedor);
            }
        }

        // GET: NotaCreditoProveedors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notaCreditoProveedor = await _context.NotaCreditoProveedors.FindAsync(id);
            if (notaCreditoProveedor == null)
            {
                return NotFound();
            }
            return View(notaCreditoProveedor);
        }

        // POST: NotaCreditoProveedors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNotaCredito,Ambiente,TipoEmision,RazonSocial,NombreComercial,Ruc,ClaveAcceso,CodDoc,Estab,PtoEmi,Secuencial,DirMatriz,FechaEmision,DirEstablecimiento,TipoIdentificacionComprador,RazonSocialComprador,IdentificacionComprador,ContribuyenteEspecial,ObligadoContabilidad,CodDocModificado,NumDocModificado,FechaEmisionDocSustento,TotalSinImpuestos,ValorModificacion,Moneda,Motivo,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] NotaCreditoProveedor notaCreditoProveedor)
        {
            if (id != notaCreditoProveedor.IdNotaCredito)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notaCreditoProveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotaCreditoProveedorExists(notaCreditoProveedor.IdNotaCredito))
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
            return View(notaCreditoProveedor);
        }

        // GET: NotaCreditoProveedors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notaCreditoProveedor = await _context.NotaCreditoProveedors
                .FirstOrDefaultAsync(m => m.IdNotaCredito == id);
            if (notaCreditoProveedor == null)
            {
                return NotFound();
            }

            return View(notaCreditoProveedor);
        }

        // POST: NotaCreditoProveedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notaCreditoProveedor = await _context.NotaCreditoProveedors.FindAsync(id);
            if (notaCreditoProveedor != null)
            {
                _context.NotaCreditoProveedors.Remove(notaCreditoProveedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotaCreditoProveedorExists(int id)
        {
            return _context.NotaCreditoProveedors.Any(e => e.IdNotaCredito == id);
        }

        private string GetXmlValue(XmlNode doc, string tagName)
        {
            return doc.SelectSingleNode($"//{tagName}")?.InnerText ?? "";
        }


        private DateTime? ParseFecha(string fecha)
        {
            if (string.IsNullOrEmpty(fecha)) return null;

            if (DateTime.TryParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return null;
        }

        private decimal ParseDecimal(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return 0;

            if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            return 0;
        }
    }
   
}