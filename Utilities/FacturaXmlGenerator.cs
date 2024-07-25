using System;
using System.Xml.Linq;
using System.Linq;
using ContaFacil.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature;
using FirmaXadesNet.Signature.Parameters;
using System.Text;
using System.Security.Cryptography;
using System.Xml;

namespace ContaFacil.Utilities
{
    public class FacturaXmlGenerator
    {
        private readonly IConfiguration _configuration;
        public FacturaXmlGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public XDocument GenerateXml(Factura factura, Persona cliente, Emisor emisor)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("factura",
                    new XAttribute("id", "comprobante"),
                    new XAttribute("version", "1.0.0"),
                    GenerateInfoTributaria(factura,emisor),
                    GenerateInfoFactura(factura, cliente,emisor),
                    GenerateDetalles(factura),
                    GenerateInfoAdicional(cliente)
                )
            );

            return doc;
        }

        private XElement GenerateInfoTributaria(Factura factura, Emisor emisor)
        {
            return new XElement("infoTributaria",
                new XElement("ambiente", "1"),
                new XElement("tipoEmision", "1"),
                new XElement("razonSocial", emisor.RazonSocial),
                new XElement("nombreComercial", emisor.NombreComercial),
                new XElement("ruc", emisor.Ruc),
                new XElement("claveAcceso", GenerateClaveAcceso(factura,emisor)),
                new XElement("codDoc", "01"),
                new XElement("estab", emisor.Establecimiento),
                new XElement("ptoEmi", emisor.PuntoEmision),
                new XElement("secuencial", emisor.Secuencial),
                new XElement("dirMatriz", emisor.Direccion)
            );
        }

        private XElement GenerateInfoFactura(Factura factura, Persona cliente,Emisor emisor)
        {
            var infoFactura = new XElement("infoFactura",
                new XElement("fechaEmision", factura.Fecha.ToString("dd/MM/yyyy")),
                new XElement("dirEstablecimiento", emisor.Direccion),
                new XElement("obligadoContabilidad", "NO"),
                new XElement("tipoIdentificacionComprador",cliente.IdTipoIdentificacionNavigation.CodigoSri),
                new XElement("razonSocialComprador", cliente.Nombre),
                new XElement("identificacionComprador", cliente.Identificacion),
                new XElement("totalSinImpuestos", factura.Subtotal.ToString("F2")),
                new XElement("totalDescuento", "0.00"),
               new XElement("totalConImpuestos",
    factura.DetalleFacturas.GroupBy(df => df.IdProductoNavigation.IdImpuestoNavigation)
        .Select(group =>
            new XElement("totalImpuesto",
                new XElement("codigo", group.Key.CodigoSri),
                new XElement("codigoPorcentaje", group.Key.CodigoPorcentajeSri),
                new XElement("baseImponible", group.Sum(df => df.Cantidad * df.PrecioUnitario).ToString("F2")),
                new XElement("valor", group.Sum(df => df.Cantidad * df.PrecioUnitario * (group.Key.Porcentaje / 100m)).ToString("F2"))
            )
        )
),
                new XElement("propina", "0.00"),
                new XElement("importeTotal", (factura.MontoTotal).ToString("F2")),
                new XElement("moneda", "DOLAR")
            );
            // Agregar información de pagos
            var pagosElement = new XElement("pagos");
            foreach (var pago in factura.Pagos)
            {
                pagosElement.Add(new XElement("pago",
                    new XElement("formaPago", pago.IdTipoPagoNavigation.CodigoSri),
                    new XElement("total", pago.Monto.ToString("F2"))
                ));
            }
            infoFactura.Add(pagosElement);

            return infoFactura;
        }

        private XElement GenerateDetalles(Factura factura)
        {
            return new XElement("detalles",
     factura.DetalleFacturas.Select(detalle =>
         new XElement("detalle",
             new XElement("codigoPrincipal", detalle.IdProductoNavigation.Codigo),
             new XElement("descripcion", detalle.Descripcion),
             new XElement("cantidad", detalle.Cantidad.ToString()),
             new XElement("precioUnitario", detalle.PrecioUnitario.ToString("F2")),
             new XElement("descuento", "0.00"),
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
                    new XAttribute("nombre", "Direccion"),
                    cliente.Direccion ?? "xxxxx"),
                new XElement("campoAdicional",
                    new XAttribute("nombre", "Telefono"),
                    cliente.Telefono ?? "000000000"),
                new XElement("campoAdicional",
                    new XAttribute("nombre", "Email"),
                    cliente.Email ?? "xxxxx@GMAIL.COM")
            );
        }

        private string GenerateClaveAcceso(Factura factura, Emisor emisor)
        {
           
            // Obtener la fecha actual
            var fechaEmision = DateTime.Now;

            // 1. Fecha de emisión (ddmmaaaa)
            string fecha = fechaEmision.ToString("ddMMyyyy");

            // 2. Tipo de comprobante (01 para factura)
            string tipoComprobante = "01";

            // 3. Número de RUC
            string numeroRuc = emisor.Ruc;

            // 4. Tipo de ambiente (1: pruebas, 2: producción)
            string tipoAmbiente = emisor.TipoAmbiente; // Asumimos producción, cámbialo si es necesario

            // 5. Serie (establecimiento y punto de emisión)
            string serie = emisor.Establecimiento+emisor.PuntoEmision; // Asumimos 001001, ajusta según tus necesidades

            // 6. Número de comprobante (secuencial de 9 dígitos)
            string numeroComprobante = factura.IdFactura.ToString("D9");

            // 7. Código numérico (8 dígitos)
            string codigoNumerico = GenerateRandomNumericCode(8);

            // 8. Tipo de emisión (1: normal)
            string tipoEmision = "1";

            // Concatenar todos los campos
            string claveAcceso = fecha + tipoComprobante + numeroRuc + tipoAmbiente + serie + numeroComprobante + codigoNumerico + tipoEmision;

            // Calcular el dígito verificador
            int digitoVerificador = CalculateVerifierDigit(claveAcceso);

            // Añadir el dígito verificador al final
            claveAcceso += digitoVerificador.ToString();

            return claveAcceso;
        }

        private string GenerateRandomNumericCode(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("0123456789", length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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


        public async Task<(string Estado, string Descripcion)> EnviarXmlFirmadoYProcesarRespuesta(string ambiente,string xmlFirmado, int facturaId)
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

    }

}