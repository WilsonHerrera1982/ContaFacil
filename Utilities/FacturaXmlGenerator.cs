﻿using System;
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
        public XDocument GenerateXml(Factura factura, Persona cliente, Persona emisor)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("factura",
                    new XAttribute("id", "comprobante"),
                    new XAttribute("version", "1.0.0"),
                    GenerateInfoTributaria(factura,emisor),
                    GenerateInfoFactura(factura, cliente),
                    GenerateDetalles(factura)
                )
            );

            return doc;
        }

        private XElement GenerateInfoTributaria(Factura factura, Persona emisor)
        {
            return new XElement("infoTributaria",
                new XElement("ambiente", "1"),
                new XElement("tipoEmision", "1"),
                new XElement("razonSocial", emisor.Nombre),
                new XElement("nombreComercial", emisor.Nombre),
                new XElement("ruc", emisor.Identificacion),
                new XElement("claveAcceso", GenerateClaveAcceso(factura,emisor.Identificacion)),
                new XElement("codDoc", "01"),
                new XElement("estab", "001"),
                new XElement("ptoEmi", "001"),
                new XElement("secuencial", "000000001"),
                new XElement("dirMatriz", emisor.Direccion)
            );
        }

        private XElement GenerateInfoFactura(Factura factura, Persona cliente)
        {
            return new XElement("infoFactura",
                new XElement("fechaEmision", factura.Fecha.ToString("dd/MM/yyyy")),
                new XElement("dirEstablecimiento", "Dirección Establecimiento"),
                new XElement("obligadoContabilidad", "NO"),
                new XElement("tipoIdentificacionComprador", "05"),
                new XElement("razonSocialComprador", cliente.Nombre),
                new XElement("identificacionComprador", cliente.Identificacion),
                new XElement("totalSinImpuestos", factura.MontoTotal.ToString("F2")),
                new XElement("totalDescuento", "0.00"),
                new XElement("totalConImpuestos",
                    new XElement("totalImpuesto",
                        new XElement("codigo", "2"),
                        new XElement("codigoPorcentaje", "2"),
                        new XElement("baseImponible", factura.MontoTotal.ToString("F2")),
                        new XElement("valor", (factura.MontoTotal * 0.12m).ToString("F2"))
                    )
                ),
                new XElement("propina", "0.00"),
                new XElement("importeTotal", (factura.MontoTotal * 1.12m).ToString("F2")),
                new XElement("moneda", "DOLAR")
            );
        }

        private XElement GenerateDetalles(Factura factura)
        {
            return new XElement("detalles",
                factura.DetalleFacturas.Select(detalle =>
                    new XElement("detalle",
                        new XElement("codigoPrincipal", detalle.IdDetalleFactura.ToString()),
                        new XElement("descripcion", detalle.Descripcion),
                        new XElement("cantidad", detalle.Cantidad.ToString()),
                        new XElement("precioUnitario", detalle.PrecioUnitario.ToString("F2")),
                        new XElement("descuento", "0.00"),
                        new XElement("precioTotalSinImpuesto", (detalle.Cantidad * detalle.PrecioUnitario).ToString("F2")),
                        new XElement("impuestos",
                            new XElement("impuesto",
                                new XElement("codigo", "2"),
                                new XElement("codigoPorcentaje", "2"),
                                new XElement("tarifa", "12"),
                                new XElement("baseImponible", (detalle.Cantidad * detalle.PrecioUnitario).ToString("F2")),
                                new XElement("valor", (detalle.Cantidad * detalle.PrecioUnitario * 0.12m).ToString("F2"))
                            )
                        )
                    )
                )
            );
        }

        private string GenerateClaveAcceso(Factura factura, string ruc)
        {
            // Obtener la fecha actual
            var fechaEmision = DateTime.Now;

            // 1. Fecha de emisión (ddmmaaaa)
            string fecha = fechaEmision.ToString("ddMMyyyy");

            // 2. Tipo de comprobante (01 para factura)
            string tipoComprobante = "01";

            // 3. Número de RUC
            string numeroRuc = ruc;

            // 4. Tipo de ambiente (1: pruebas, 2: producción)
            string tipoAmbiente = "2"; // Asumimos producción, cámbialo si es necesario

            // 5. Serie (establecimiento y punto de emisión)
            string serie = "001001"; // Asumimos 001001, ajusta según tus necesidades

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
        public string FirmarXml(string xmlString)
        {
            // Cargar el certificado
            var rutaCertificado = _configuration["CertificadoDigital:Ruta"];
            var claveCertificado = _configuration["CertificadoDigital:Clave"];
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


    }

}