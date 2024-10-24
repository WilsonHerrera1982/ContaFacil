﻿using System;
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

        public XDocument GenerateXml(Factura factura, Persona cliente, Emisor emisor, string motivo, List<DetalleFactura> detallesNotaCredito, decimal totalNotaCredito)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("notaCredito",
                    new XAttribute("id", "comprobante"),
                    new XAttribute("version", "1.0.0"),
                    GenerateInfoTributaria(factura, emisor),
                    GenerateInfoNotaCredito(factura, cliente, emisor, motivo, totalNotaCredito),
                    GenerateDetalles(detallesNotaCredito),
                    GenerateInfoAdicional(cliente)
                )
            );

            return doc;
        }

        private XElement GenerateInfoTributaria(Factura factura, Emisor emisor)
        {
            return new XElement("infoTributaria",
                new XElement("ambiente", emisor.TipoAmbiente),
                new XElement("tipoEmision", "1"),
                new XElement("razonSocial", emisor.RazonSocial),
                new XElement("nombreComercial", emisor.NombreComercial),
                new XElement("ruc", emisor.Ruc),
                new XElement("claveAcceso", GenerateClaveAcceso(factura, emisor)),
                new XElement("codDoc", "04"),
                new XElement("estab", emisor.Establecimiento),
                new XElement("ptoEmi", emisor.PuntoEmision),
                new XElement("secuencial", emisor.Secuencial),
                new XElement("dirMatriz", emisor.Direccion)
            );
        }

        private XElement GenerateInfoNotaCredito(Factura factura, Persona cliente, Emisor emisor, string motivo, decimal totalNotaCredito)
        {
            return new XElement("infoNotaCredito",
                new XElement("fechaEmision", DateTime.Now.ToString("dd/MM/yyyy")),
                new XElement("dirEstablecimiento", emisor.Direccion),
                new XElement("tipoIdentificacionComprador", cliente.IdTipoIdentificacionNavigation.CodigoSri),
                new XElement("razonSocialComprador", cliente.Nombre),
                new XElement("identificacionComprador", cliente.Identificacion),
                new XElement("contribuyenteEspecial", "NO"),
                new XElement("obligadoContabilidad", emisor.ObligadoContabilidad),
                new XElement("codDocModificado", "01"),
                new XElement("numDocModificado", factura.NumeroFactura),
                new XElement("fechaEmisionDocSustento", factura.Fecha.ToString("dd/MM/yyyy")),
                new XElement("totalSinImpuestos", totalNotaCredito.ToString("F2")),
                new XElement("valorModificacion", totalNotaCredito.ToString("F2")),
                new XElement("moneda", "DOLAR"),
                new XElement("totalConImpuestos", GenerateTotalConImpuestos(factura.DetalleFacturas)),
                new XElement("motivo", motivo)
            );
        }

        private XElement GenerateDetalles(List<DetalleFactura> detallesNotaCredito)
        {
            return new XElement("detalles",
                detallesNotaCredito.Select(detalle =>
                    new XElement("detalle",
                        new XElement("codigoInterno", detalle.IdProductoNavigation.Codigo),
                        new XElement("descripcion", detalle.Descripcion),
                        new XElement("cantidad", detalle.Cantidad.ToString()),
                        new XElement("precioUnitario", detalle.PrecioUnitario.ToString("F2")),
                        new XElement("descuento", detalle.Descuento?.ToString("F2") ?? "0.00"),
                        new XElement("precioTotalSinImpuesto", (detalle.Cantidad * detalle.PrecioUnitario).ToString("F2")),
                        new XElement("impuestos",
                            new XElement("impuesto",
                                new XElement("codigo", detalle.IdProductoNavigation.IdImpuestoNavigation.CodigoSri),
                                new XElement("codigoPorcentaje", detalle.IdProductoNavigation.IdImpuestoNavigation.CodigoPorcentajeSri),
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

        public string FirmarXml(string xmlString, string ruta, string clave)
        {
            // Cargar el certificado
            var rutaCertificado = ruta;
            var claveCertificado = clave;
            try
            {
                string certificadoPath = rutaCertificado;
                string certificadoPassword = claveCertificado;

                // Logging de la ruta del certificado y longitud de la clave para debugging
                Console.WriteLine($"Ruta del certificado: {certificadoPath}");
                Console.WriteLine($"Longitud de la clave del certificado: {certificadoPassword.Length}");

                // Cargar el certificado
                X509Certificate2 cert = new X509Certificate2(certificadoPath, certificadoPassword, X509KeyStorageFlags.Exportable);

                // Logging para verificar que el certificado se ha cargado correctamente
                Console.WriteLine($"Certificado cargado: {cert.Subject}");

                // Crear el servicio de firma XAdES
                XadesService xadesService = new XadesService();

                SignatureParameters parametros = new SignatureParameters
                {
                    SignaturePackaging = SignaturePackaging.ENVELOPED,
                    SignatureMethod = SignatureMethod.RSAwithSHA256, // Cambiado a SHA256
                    DigestMethod = DigestMethod.SHA256 // Cambiado a SHA256
                };

                // Configurar el certificado de firma
                parametros.Signer = new Signer(cert);

                // Convertir el XML a un MemoryStream
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
                {
                    // Logging para verificar el contenido del XML
                    Console.WriteLine($"Contenido del XML: {xmlString.Substring(0, Math.Min(xmlString.Length, 100))}...");

                    // Firmar el documento
                    var documentoFirmado = xadesService.Sign(ms, parametros);

                    XmlDocument xmlDoc = new XmlDocument();
                    // Obtener el XML firmado
                    using (MemoryStream outputMs = new MemoryStream())
                    {
                        documentoFirmado.Save(outputMs);
                        outputMs.Position = 0;
                        xmlDoc.Load(outputMs);
                        xmlDoc.Save("factura_firmada.xml");
                        return Encoding.UTF8.GetString(outputMs.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                // Logging detallado de la excepción
                Console.WriteLine($"Error al firmar el XML: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                    Console.WriteLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
                }

                throw new Exception($"Error al firmar el XML: {ex.Message}", ex);
            }
        }


        public async Task<(string Estado, string Descripcion)> EnviarXmlFirmadoYProcesarRespuesta(string ambiente, string xmlFirmado, int facturaId)
        {
            string url = "";
            // URL del servicio web
            if (ambiente.Equals("2"))
            {
                url = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
            }
            else
            {
                url = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";

            }


            // Validar el XML antes de enviarlo
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlFirmado); // Esto validará el XML
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"El XML proporcionado no es válido: {ex.Message}");
                return ("ERROR", $"El XML proporcionado no es válido: {ex.Message}");
            }

            // Codificar el XML en base64
            string xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlFirmado));

            // Crear la solicitud SOAP con el XML codificado en base64
            string soapEnvelope = $@"
    <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:ec='http://ec.gob.sri.ws.recepcion'>
       <soapenv:Header/>
       <soapenv:Body>
          <ec:validarComprobante>
             <xml>{xmlBase64}</xml>
          </ec:validarComprobante>
       </soapenv:Body>
    </soapenv:Envelope>";

            // Crear el cliente HTTP
            using (var client = new HttpClient())
            {
                // Configurar el contenido de la solicitud
                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

                try
                {
                    // Enviar la solicitud POST
                    var response = await client.PostAsync(url, content);

                    // Leer la respuesta
                    string responseString = await response.Content.ReadAsStringAsync();

                    // Verificar el estado de la respuesta
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error en la respuesta del servidor: {response.StatusCode} {response.ReasonPhrase}");
                        return ("ERROR", $"Error en la respuesta del servidor: {response.StatusCode} {response.ReasonPhrase}");
                    }

                    // Procesar la respuesta XML
                    XmlDocument responseXmlDoc = new XmlDocument();
                    responseXmlDoc.LoadXml(responseString);

                    // Extraer el estado y el mensaje
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(responseXmlDoc.NameTable);
                    nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                    nsManager.AddNamespace("ns2", "http://ec.gob.sri.ws.recepcion");
                    string estado = responseXmlDoc.SelectSingleNode("//ns2:validarComprobanteResponse//estado", nsManager)?.InnerText;
                    string mensaje = responseXmlDoc.SelectSingleNode("//ns2:validarComprobanteResponse//mensaje", nsManager)?.InnerText;
                    string informacionAdicional = responseXmlDoc.SelectSingleNode("//ns2:validarComprobanteResponse//informacionAdicional", nsManager)?.InnerText;
                    string descripcion = $"{mensaje} {informacionAdicional}".Trim();

                    return (estado, descripcion);
                }
                catch (HttpRequestException httpEx)
                {
                    // Manejar cualquier excepción relacionada con la solicitud HTTP
                    Console.WriteLine($"Error en la solicitud HTTP: {httpEx.Message}");
                    return ("ERROR", $"Error en la solicitud HTTP: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otra excepción
                    Console.WriteLine($"Error al enviar XML o procesar respuesta: {ex.Message}");
                    return ("ERROR", $"Error al procesar: {ex.Message}");
                }
            }
        }

        public async Task<(string Estado, DateTime FechaAutorizacion)> ConsultarAutorizacionAsync(string claveAcceso, string ambiente)
        {
            string url = "";
            // URL del servicio web
            if (ambiente.Equals("2"))
            {
                url = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
            }
            else
            {
                url = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";

            }
            var soapEnvelope =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">
                <soapenv:Header/>
                <soapenv:Body>
                    <ec:autorizacionComprobante>
                        <claveAccesoComprobante>{claveAcceso}</claveAccesoComprobante>
                    </ec:autorizacionComprobante>
                </soapenv:Body>
            </soapenv:Envelope>";
            using (var client = new HttpClient())
            {

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var xDoc = XDocument.Parse(responseString);

                var autorizacionElement = xDoc.Descendants("autorizacion").FirstOrDefault();
                if (autorizacionElement != null)
                {
                    var est = autorizacionElement.Element("estado")?.Value;
                    var fechaAutorizacionStr = autorizacionElement.Element("fechaAutorizacion")?.Value;

                    if (DateTime.TryParse(fechaAutorizacionStr, out DateTime fechaAutorizacion))
                    {
                        return (est, fechaAutorizacion);
                    }
                }

                throw new Exception("No se pudo obtener la información de autorización");
            }
        }
        private XElement GenerateTotalConImpuestos(ICollection<DetalleFactura> detalles)
        {
            return new XElement("totalConImpuestos",
                detalles.GroupBy(df => df.IdProductoNavigation.IdImpuestoNavigation)
                    .Select(group =>
                        new XElement("totalImpuesto",
                            new XElement("codigo", group.Key.CodigoSri),
                            new XElement("codigoPorcentaje", group.Key.CodigoPorcentajeSri),
                            new XElement("baseImponible", group.Sum(df => df.Cantidad * df.PrecioUnitario).ToString("F2")),
                            new XElement("valor", group.Sum(df => df.Cantidad * df.PrecioUnitario * (group.Key.Porcentaje / 100m)).ToString("F2"))
                        )
                    )
            );
        }
    }
}