using System;
using System.Xml.Linq;
using System.Linq;
using ContaFacil.Models;

namespace ContaFacil.Utilities
{
    public class RetencionXmlClienteGenerator
    {
        private readonly IConfiguration _configuration;

        public RetencionXmlClienteGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public XDocument GenerateXml(List<Retencion> retenciones, Persona cliente, Emisor emisor)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("comprobanteRetencion",
                    new XAttribute("id", "comprobante"),
                    new XAttribute("version", "1.0.0"),
                    GenerateInfoTributaria(retenciones.First(), emisor),
                    GenerateInfoCompRetencion(retenciones.First(), cliente, emisor),
                    GenerateImpuestos(retenciones),
                    GenerateInfoAdicional(cliente)
                )
            );

            return doc;
        }

        private XElement GenerateImpuestos(List<Retencion> retenciones)
        {
            return new XElement("impuestos",
                retenciones.Select(retencion =>
                    new XElement("impuesto",
                        new XElement("codigo", DeterminarCodigoImpuesto(retencion.Impuesto)),
                        new XElement("codigoRetencion", DeterminarCodigoRetencion(retencion.Impuesto, retencion.PorcentajeRetencion ?? 0)),
                        new XElement("baseImponible", retencion.BaseImponible.ToString("F2")),
                        new XElement("porcentajeRetener", retencion.PorcentajeRetencion ?? 0),
                       new XElement("valorRetenido", (retencion.ValorRetenido ?? 0).ToString("F2")),
                        new XElement("codDocSustento", "01"),
                        new XElement("numDocSustento", retencion.NumeroFactura),
                        new XElement("fechaEmisionDocSustento", retencion.FechaCreacion.ToString("dd/MM/yyyy"))
                    )
                )
            );
        }

        private XElement GenerateInfoCompRetencion(Retencion retencion, Persona cliente, Emisor emisor)
        {
            return new XElement("infoCompRetencion",
                new XElement("fechaEmision", retencion.FechaCreacion.ToString("dd/MM/yyyy")),
                new XElement("dirEstablecimiento", emisor.Direccion),
                new XElement("contribuyenteEspecial", "NO"),
                new XElement("obligadoContabilidad", emisor.ObligadoContabilidad.Equals("true") ? "SI" : "NO"),
                new XElement("tipoIdentificacionSujetoRetenido", DeterminarTipoIdentificacion(cliente.Identificacion)),
                new XElement("razonSocialSujetoRetenido", cliente.Nombre),
                new XElement("identificacionSujetoRetenido", cliente.Identificacion),
                new XElement("periodoFiscal", retencion.FechaCreacion.ToString("MM/yyyy"))
            );
        }

       

        private XElement GenerateInfoAdicional(Persona cliente)
        {
            return new XElement("infoAdicional",
                new XElement("campoAdicional",
                    new XAttribute("nombre", "Email"),
                    cliente.Email ?? "N/A")
            );
        }

        private XElement GenerateInfoTributaria(Retencion retencion, Emisor emisor)
        {
            return new XElement("infoTributaria",
                new XElement("ambiente", emisor.TipoAmbiente),
                new XElement("tipoEmision", "1"),
                new XElement("razonSocial", emisor.RazonSocial),
                new XElement("nombreComercial", emisor.NombreComercial),
                new XElement("ruc", emisor.Ruc),
                new XElement("claveAcceso", GenerateClaveAcceso(retencion, emisor)),
                new XElement("codDoc", "07"),
                new XElement("estab", emisor.Establecimiento),
                new XElement("ptoEmi", emisor.PuntoEmision),
                new XElement("secuencial", emisor.Secuencial),
                new XElement("dirMatriz", emisor.Direccion)
            );
        }

        private string GenerateClaveAcceso(Retencion retencion, Emisor emisor)
        {
            var fechaEmision = retencion.FechaCreacion;
            string fecha = fechaEmision.ToString("ddMMyyyy");
            string tipoComprobante = "07";
            string numeroRuc = emisor.Ruc;
            string tipoAmbiente = emisor.TipoAmbiente;
            string serie = emisor.Establecimiento + emisor.PuntoEmision;
            string numeroComprobante = retencion.IdRetencion.ToString("D9");
            string codigoNumerico = GenerateRandomNumericCode(8);
            string tipoEmision = "1";

            string claveAcceso = fecha + tipoComprobante + numeroRuc + tipoAmbiente + serie + numeroComprobante + codigoNumerico + tipoEmision;
            int digitoVerificador = CalcularDigitoVerificador(claveAcceso);
            return claveAcceso + digitoVerificador.ToString();
        }

        private string GenerateRandomNumericCode(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("0123456789", length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private int CalcularDigitoVerificador(string clave)
        {
            int[] coeficientes = { 2, 3, 4, 5, 6, 7 };
            int suma = 0;

            for (int i = 0; i < clave.Length; i++)
            {
                suma += int.Parse(clave[i].ToString()) * coeficientes[i % 6];
            }

            int digito = 11 - (suma % 11);
            return (digito == 11) ? 0 : (digito == 10) ? 1 : digito;
        }

        private string DeterminarTipoIdentificacion(string identificacion)
        {
            return identificacion.Length == 13 ? "04" : "05";
        }

        private string DeterminarCodigoImpuesto(string impuesto)
        {
            return impuesto.ToUpper() == "RENTA" ? "1" : "2";
        }

        private string DeterminarCodigoRetencion(string impuesto, decimal porcentaje)
        {
            if (impuesto.ToUpper() == "RENTA")
            {
                // Aquí deberías tener una lógica para determinar el código de retención
                // basado en el porcentaje de retención de renta
                return "303"; // Este es un ejemplo, debes ajustarlo según tus necesidades
            }
            else if (impuesto.ToUpper() == "IVA")
            {
                if (porcentaje == 30) return "9";
                if (porcentaje == 70) return "10";
                if (porcentaje == 100) return "11";
            }
            return "0"; // Código por defecto si no se encuentra una coincidencia
        }
    }
}