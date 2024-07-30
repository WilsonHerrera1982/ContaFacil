using System;
using System.Xml.Linq;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature;
using FirmaXadesNet.Signature.Parameters;
using System.Xml;
using System.Net.Http;
using System.Threading.Tasks;
using ContaFacil.Models;

namespace ContaFacil.Utilities
{
    public class NotaCreditoXmlGenerator
    {
        private readonly IConfiguration _configuration;
        public NotaCreditoXmlGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public XDocument GenerateXml(Factura notaCredito, Persona cliente, Emisor emisor,string motivo)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("notaCredito",
                    new XAttribute("id", "comprobante"),
                    new XAttribute("version", "1.0.0"),
                    GenerateInfoTributaria(notaCredito, emisor),
                    GenerateInfoNotaCredito(notaCredito, cliente, emisor,motivo),
                    GenerateDetalles(notaCredito),
                    GenerateInfoAdicional(cliente)
                )
            );

            return doc;
        }

        private XElement GenerateInfoTributaria(Factura notaCredito, Emisor emisor)
        {
            return new XElement("infoTributaria",
                new XElement("ambiente", "1"),
                new XElement("tipoEmision", "1"),
                new XElement("razonSocial", emisor.RazonSocial),
                new XElement("nombreComercial", emisor.NombreComercial),
                new XElement("ruc", emisor.Ruc),
                new XElement("claveAcceso", GenerateClaveAcceso(notaCredito, emisor)),
                new XElement("codDoc", "04"),
                new XElement("estab", emisor.Establecimiento),
                new XElement("ptoEmi", emisor.PuntoEmision),
                new XElement("secuencial", emisor.Secuencial),
                new XElement("dirMatriz", emisor.Direccion)
            );
        }

        private XElement GenerateInfoNotaCredito(Factura notaCredito, Persona cliente, Emisor emisor, string motivo)
        {
            return new XElement("infoNotaCredito",
                new XElement("fechaEmision", notaCredito.Fecha.ToString("dd/MM/yyyy")),
                new XElement("dirEstablecimiento", emisor.Direccion),
                new XElement("tipoIdentificacionComprador", cliente.IdTipoIdentificacionNavigation.CodigoSri),
                new XElement("razonSocialComprador", cliente.Nombre),
                new XElement("identificacionComprador", cliente.Identificacion),
                new XElement("contribuyenteEspecial", "1234"),
                new XElement("obligadoContabilidad", "SI"),
                new XElement("codDocModificado", "01"),
                new XElement("numDocModificado", "001-001-000000001"),
                new XElement("fechaEmisionDocSustento", notaCredito.Fecha.ToString("dd/MM/yyyy")),
                new XElement("totalSinImpuestos", notaCredito.Subtotal?.ToString("F2") ?? "0.00"),
                new XElement("valorModificacion", notaCredito.MontoTotal.ToString("F2")),
                new XElement("moneda", "DOLAR"),
                new XElement("totalConImpuestos",
    notaCredito.DetalleFacturas.GroupBy(df => df.IdProductoNavigation.IdImpuestoNavigation)
        .Select(group =>
            new XElement("totalImpuesto",
                new XElement("codigo", group.Key.CodigoSri),
                new XElement("codigoPorcentaje", group.Key.CodigoPorcentajeSri),
                new XElement("baseImponible", group.Sum(df => df.Cantidad * df.PrecioUnitario).ToString("F2")),
                new XElement("valor", group.Sum(df => df.Cantidad * df.PrecioUnitario * (group.Key.Porcentaje / 100m)).ToString("F2"))
            )
        )
),
                new XElement("motivo", motivo)
            );
        }

        private XElement GenerateDetalles(Factura notaCredito)
        {
            return new XElement("detalles",
                notaCredito.DetalleFacturas.Select(detalle =>
                    new XElement("detalle",
                        new XElement("codigoInterno", detalle.IdProductoNavigation.Codigo),
                        new XElement("descripcion", detalle.Descripcion),
                        new XElement("cantidad", detalle.Cantidad.ToString()),
                        new XElement("precioUnitario", detalle.PrecioUnitario.ToString("F2")),
                        new XElement("descuento", detalle.Descuento?.ToString("F2") ?? "0.00"),
                        new XElement("precioTotalSinImpuesto", (detalle.Cantidad * detalle.PrecioUnitario).ToString("F2")),
                        new XElement("impuestos",
                            new XElement("impuesto",
                                new XElement("codigo", "2"),
                                new XElement("codigoPorcentaje", "2"),
                                new XElement("tarifa", detalle.IdProductoNavigation.IdImpuestoNavigation.Porcentaje.ToString("F2")),
                     new XElement("baseImponible", (detalle.Cantidad * detalle.PrecioUnitario).ToString("F2")),
                     new XElement("valor", (detalle.Cantidad * detalle.PrecioUnitario * (detalle.IdProductoNavigation.IdImpuestoNavigation.Porcentaje / 100m)).ToString("F2"))
                            )
                        )
                    )
                )
            );
        }

        private XElement GenerateInfoAdicional(Persona cliente)
        {
            return new XElement("infoAdicional",
                new XElement("campoAdicional",
                    new XAttribute("nombre", "Telefono"),
                    cliente.Telefono ?? "000000000"),
                new XElement("campoAdicional",
                    new XAttribute("nombre", "Email"),
                    cliente.Email ?? "xxxxx@GMAIL.COM")
            );
        }

        private string GenerateClaveAcceso(Factura notaCredito, Emisor emisor)
        {
            var fechaEmision = DateTime.Now;
            string fecha = fechaEmision.ToString("ddMMyyyy");
            string tipoComprobante = "04";
            string numeroRuc = emisor.Ruc;
            string tipoAmbiente = emisor.TipoAmbiente;
            string serie = emisor.Establecimiento + emisor.PuntoEmision;
            string numeroComprobante = emisor.Secuencial;
            string codigoNumerico = "12345678";
            string tipoEmision = "1";

            string clave = $"{fecha}{tipoComprobante}{numeroRuc}{tipoAmbiente}{serie}{numeroComprobante}{codigoNumerico}{tipoEmision}";

            clave = clave.PadRight(48, '0');
            int verificador = CalculateVerifierDigit(clave);
            clave = clave + verificador.ToString();

            return clave;
        }

        private int CalculateVerifierDigit(string code)
        {
            int[] coefficients = { 2, 3, 4, 5, 6, 7 };
            int sum = 0;
            int coefficient;

            for (int i = code.Length - 1; i >= 0; i--)
            {
                coefficient = coefficients[(code.Length - 1 - i) % 6];
                sum += int.Parse(code[i].ToString()) * coefficient;
            }

            int verifierDigit = 11 - (sum % 11);
            if (verifierDigit == 11)
                verifierDigit = 0;
            else if (verifierDigit == 10)
                verifierDigit = 1;

            return verifierDigit;
        }
    }
}
