using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ContaFacil.Models;
using ContaFacil.Utilities;

namespace ContaFacil.Utilities
{
    public class RetencionService
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RetencionService(ContableContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Retencion>> ProcesarRetenciones(
            Inventario nuevoInventario, 
            Proveedor proveedor, 
            Empresa empresa, 
            Usuario usuario, 
            Producto producto, 
            string numeroAsiento,
            string idUsuario)
        {
            List<Retencion> retencions = new List<Retencion>();

            if (proveedor.RetencionPorcentaje > 0 || proveedor.RetencionIva > 0)
            {
                Retencion ret = _context.Retencions
                    .Where(r => r.IdEmpresa == empresa.IdEmpresa)
                    .OrderByDescending(r => r.FechaCreacion)
                    .FirstOrDefault();

                var retencionXml = new RetencionXmlGenerator(_configuration);

                // Procesar retención de renta
                if (proveedor.RetencionPorcentaje > 0)
                {
                    var retencionRenta = await ProcesarRetencionRenta(
                        nuevoInventario, proveedor, empresa, usuario, producto, 
                        numeroAsiento, idUsuario, ret);
                    
                    if (retencionRenta != null)
                    {
                        retencions.Add(retencionRenta);
                    }
                }

                // Procesar retención de IVA
                if (proveedor.RetencionIva > 0)
                {
                    var retencionIva = await ProcesarRetencionIva(
                        nuevoInventario, proveedor, empresa, usuario, producto, 
                        numeroAsiento, idUsuario, ret);
                    
                    if (retencionIva != null)
                    {
                        retencions.Add(retencionIva);
                    }
                }

                // Generar XML y actualizar retenciones
                if (retencions.Any())
                {
                    await GenerarXmlYActualizarRetenciones(retencions, proveedor, empresa, retencionXml);
                    
                    // Crear transacción final de pago a proveedor
                    await CrearTransaccionPagoProveedor(nuevoInventario, producto, numeroAsiento, empresa, usuario, retencions);
                }

                await _context.SaveChangesAsync();
            }

            return retencions;
        }

        private async Task<Retencion> ProcesarRetencionRenta(
            Inventario nuevoInventario, 
            Proveedor proveedor, 
            Empresa empresa, 
            Usuario usuario, 
            Producto producto, 
            string numeroAsiento,
            string idUsuario, 
            Retencion ret)
        {
            string numeroRetencion = ret != null 
                ? GenerarNumeroComprobanteRetencion(ret.ComprobanteRetencion)
                : "001-001-000000001";

            var retencionRenta = new Retencion
            {
                IdEmpresa = empresa.IdEmpresa,
                IdFactura = nuevoInventario.IdInventario,
                IdProveedor = nuevoInventario.IdProveedor,
                NumeroFactura = nuevoInventario.FacturaNumero,
                ComprobanteRetencion = numeroRetencion,
                EjercicioFiscal = DateTime.Now.Year.ToString(),
                BaseImponible = nuevoInventario.Subtotal15 ?? 0,
                EstadoBoolean = true,
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = int.Parse(idUsuario),
                Impuesto = "RENTA",
                PorcentajeRetencion = proveedor.RetencionPorcentaje,
                ValorRetenido = (nuevoInventario.SubTotal ?? 0) * (proveedor.RetencionPorcentaje / 100)
            };

            await _context.Retencions.AddAsync(retencionRenta);

            // Crear transacción contable para retención de renta
            proveedor.RetencionPorcentaje = Math.Truncate(proveedor.RetencionPorcentaje ?? 0);
            Cuentum cuent2 = _context.Cuenta.FirstOrDefault(c => 
                c.Nombre.Contains("Retención IR") && 
                c.Nombre.Contains(proveedor.RetencionPorcentaje.ToString()) && 
                c.Codigo.Contains("2.1.3.3."));

            var tipoTransaccion3 = await _context.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == "Compra");

            string descripcion2 = producto.Nombre + " " + nuevoInventario.FacturaNumero;
            await CrearTransaccion(cuent2.Codigo, numeroAsiento + " Compra de " + descripcion2, 
                retencionRenta.ValorRetenido ?? 0, tipoTransaccion3, empresa, usuario, false);

            // Crear cuenta por pagar
            await CrearCuentaPorPagar(nuevoInventario, proveedor, descripcion2);

            return retencionRenta;
        }

        private async Task<Retencion> ProcesarRetencionIva(
            Inventario nuevoInventario, 
            Proveedor proveedor, 
            Empresa empresa, 
            Usuario usuario, 
            Producto producto, 
            string numeroAsiento,
            string idUsuario, 
            Retencion rete)
        {
            string numeroRetencion = rete != null 
                ? GenerarNumeroComprobanteRetencion(rete.ComprobanteRetencion)
                : "001-001-000000001";

            var retencionIva = new Retencion
            {
                IdEmpresa = empresa.IdEmpresa,
                IdFactura = nuevoInventario.IdInventario,
                IdProveedor = nuevoInventario.IdProveedor,
                NumeroFactura = nuevoInventario.FacturaNumero,
                ComprobanteRetencion = numeroRetencion,
                EjercicioFiscal = DateTime.Now.Year.ToString(),
                BaseImponible = nuevoInventario.Iva ?? 0,
                EstadoBoolean = true,
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = int.Parse(idUsuario),
                Impuesto = "IVA",
                PorcentajeRetencion = proveedor.RetencionIva,
                ValorRetenido = (nuevoInventario.Iva ?? 0) * (proveedor.RetencionIva / 100)
            };

            await _context.Retencions.AddAsync(retencionIva);

            // Crear transacción contable para retención de IVA
            proveedor.RetencionPorcentaje = Math.Truncate(proveedor.RetencionPorcentaje ?? 0);
            Cuentum cuent1 = _context.Cuenta.FirstOrDefault(c => 
                c.Nombre.Contains("Retención IVA") && 
                c.Nombre.Contains(proveedor.RetencionPorcentaje.ToString()) && 
                c.Codigo.Contains("2.1.3.2"));

            var tipoTransaccion1 = await _context.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == "Compra");

            string descripcion1 = producto.Nombre + " " + nuevoInventario.FacturaNumero;
            await CrearTransaccion(cuent1.Codigo, numeroAsiento + " Compra de " + descripcion1, 
                retencionIva.ValorRetenido ?? 0, tipoTransaccion1, empresa, usuario, false);

            return retencionIva;
        }

        private async Task GenerarXmlYActualizarRetenciones(
            List<Retencion> retencions, 
            Proveedor proveedor, 
            Empresa empresa, 
            RetencionXmlGenerator retencionXml)
        {
            string idUsuario = _httpContextAccessor.HttpContext?.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario ?? "1")).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            
            // Generar el XML para todas las retenciones
            var xmlDocumentRenta = retencionXml.GenerateXml(retencions, proveedor, emisor);

            // Obtener la clave de acceso del XML
            string xmlString = xmlDocumentRenta.ToString();
            XDocument xdoc = XDocument.Parse(xmlString);
            string claveAcceso = (string)xdoc.Root
                                        .Element("infoTributaria")
                                        .Element("claveAcceso");

            // Actualizar cada retención con el XML y la clave de acceso
            foreach (var retencion in retencions)
            {
                retencion.Xml = xmlDocumentRenta.ToString();
                retencion.ClaveAcceso = claveAcceso;
            }
        }

        private async Task CrearTransaccionPagoProveedor(
            Inventario nuevoInventario, 
            Producto producto, 
            string numeroAsiento, 
            Empresa empresa, 
            Usuario usuario, 
            List<Retencion> retencions)
        {
            var sumaRetencion = retencions.Sum(r => r.ValorRetenido);
            Cuentum cuent = _context.Cuenta.FirstOrDefault(c => c.Nombre.Contains("Proveedores nacionales"));
            
            var tipoTransaccion = await _context.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == "Compra");
            
            string descripcion = producto.Nombre + " " + nuevoInventario.FacturaNumero;
            var pagoTotal = nuevoInventario.Total - sumaRetencion;
            
            await CrearTransaccion(cuent.Codigo, numeroAsiento + $" Compra de " + descripcion, 
                pagoTotal ?? 0, tipoTransaccion, empresa, usuario, false);
        }

        private async Task CrearCuentaPorPagar(Inventario nuevoInventario, Proveedor proveedor, string descripcion)
        {
            CuentasPorPagar cuentasPorPagar = new CuentasPorPagar
            {
                Descripcion = nuevoInventario.Descripcion + " " + descripcion,
                Descuentos = nuevoInventario.Descuento,
                Impuestos = nuevoInventario.Iva,
                TipoDocumento = "FACTURA",
                NumeroDocumento = nuevoInventario.FacturaNumero,
                Estado = "PENDIENTE",
                FechaCreacion = DateTime.Now,
                FechaEmision = new DateOnly(),
                MontoTotal = nuevoInventario.SubTotal ?? 0,
                MontoPendiente = nuevoInventario.SubTotal ?? 0,
                RazonSocial = proveedor.Nombre,
                RucIdentificacion = proveedor.Identificacion,
                TipoProveedor = "PROVEEDOR",
                FormaPago = "OTRO",
                UsuarioCreacion = nuevoInventario.UsuarioCreacion.ToString()
            };

            _context.Add(cuentasPorPagar);
            _context.SaveChanges();
        }

        private async Task CrearTransaccion(
            string codigoCuenta, 
            string descripcion, 
            decimal monto, 
            TipoTransaccion tipoTransaccion, 
            Empresa empresa, 
            Usuario usuario, 
            bool esDebito)
        {
            var transaccion = new Transaccion
            {
                IdCuenta = await ObtenerIdCuenta(codigoCuenta),
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                IdTipoTransaccion = tipoTransaccion.IdTipoTransaccion,
                Monto = monto,
                Descripcion = descripcion,
                Estado = true,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                UsuarioCreacion = usuario.IdUsuario,
                IdEmpresa = empresa.IdEmpresa,
                EsDebito = esDebito
            };

            _context.Transaccions.Add(transaccion);
        }

        private async Task<int> ObtenerIdCuenta(string codigoCuenta)
        {
            var cuenta = await _context.Cuenta.FirstOrDefaultAsync(c => c.Codigo == codigoCuenta);
            return cuenta?.IdCuenta ?? 0;
        }

        private string GenerarNumeroComprobanteRetencion(string ultimoComprobante)
        {
            if (string.IsNullOrEmpty(ultimoComprobante))
                return "001-001-000000001";

            var partes = ultimoComprobante.Split('-');
            if (partes.Length != 3)
                return "001-001-000000001";

            if (int.TryParse(partes[2], out int numero))
            {
                numero++;
                return $"{partes[0]}-{partes[1]}-{numero:D9}";
            }

            return "001-001-000000001";
        }
    }
}
