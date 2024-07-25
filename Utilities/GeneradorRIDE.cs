using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ZXing;
using ZXing.QrCode;
using DrawingRectangle = System.Drawing.Rectangle;
using ITextRectangle = iTextSharp.text.Rectangle;
using ITImage = iTextSharp.text.Image;

namespace ContaFacil.Utilities
{
    public class GeneradorRIDE
    {
        public byte[] GenerarRIDE(string xmlString)
        {
            using (var ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 36, 36, 54, 36);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                AgregarEncabezado(pdfDoc, xmlDoc, writer);
                AgregarInformacionComprador(pdfDoc, xmlDoc);
                AgregarDetallesFactura(pdfDoc, xmlDoc);
                AgregarInformacionAdicional(pdfDoc, xmlDoc);
                AgregarTotales(pdfDoc, xmlDoc);
                AgregarFormasDePago(pdfDoc, xmlDoc);

                pdfDoc.Close();
                return ms.ToArray();
            }
        }

        private void AgregarEncabezado(Document pdfDoc, XmlDocument xmlDoc, PdfWriter writer)
{
    PdfPTable tabla = new PdfPTable(2); // Usar solo 2 columnas
    tabla.WidthPercentage = 100;

    // Información de la empresa
    PdfPCell celdaInfoEmpresa = new PdfPCell();
    celdaInfoEmpresa.BorderWidth = 1f; // Borde de 1 milímetro
    celdaInfoEmpresa.BorderColor = BaseColor.BLACK;
    celdaInfoEmpresa.Padding = 5f; // Espaciado dentro de la celda

    // Intentar agregar el logo
    string rutaLogo = @"C:\Users\Wilson\Documents\Wilson\Logo\logo.png";
    if (File.Exists(rutaLogo))
    {
        ITImage logo = ITImage.GetInstance(rutaLogo);
        logo.ScaleToFit(100f, 50f); // Ajustar el tamaño del logo según sea necesario
        celdaInfoEmpresa.AddElement(logo);
    }
    else
    {
        celdaInfoEmpresa.AddElement(new Paragraph("NO TIENE LOGO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
    }
    
    // Información adicional de la empresa
    celdaInfoEmpresa.AddElement(new Paragraph(ObtenerValorXML(xmlDoc, "//razonSocial"), FontFactory.GetFont(FontFactory.HELVETICA, 10)));
    celdaInfoEmpresa.AddElement(new Paragraph("Dirección Matriz: " + ObtenerValorXML(xmlDoc, "//dirMatriz"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoEmpresa.AddElement(new Paragraph("Dirección Sucursal: " + ObtenerValorXML(xmlDoc, "//dirEstablecimiento"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoEmpresa.AddElement(new Paragraph("Contribuyente Especial Nro: " + ObtenerValorXML(xmlDoc, "//contribuyenteEspecial"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoEmpresa.AddElement(new Paragraph("OBLIGADO A LLEVAR CONTABILIDAD: " + ObtenerValorXML(xmlDoc, "//obligadoContabilidad"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    tabla.AddCell(celdaInfoEmpresa);

    // Información de la factura
    PdfPCell celdaInfoFactura = new PdfPCell();
    celdaInfoFactura.BorderWidth = 1f; // Borde de 1 milímetro
    celdaInfoFactura.BorderColor = BaseColor.BLACK;
    celdaInfoFactura.Padding = 5f; // Espaciado dentro de la celda

    // Añadir número de clave de acceso
    string claveAcceso = ObtenerValorXML(xmlDoc, "//claveAcceso");
    celdaInfoFactura.AddElement(new Paragraph("R.U.C.: " + ObtenerValorXML(xmlDoc, "//ruc"), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
    celdaInfoFactura.AddElement(new Paragraph("FACTURA", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
    celdaInfoFactura.AddElement(new Paragraph("No. " + ObtenerValorXML(xmlDoc, "//estab") + "-" + ObtenerValorXML(xmlDoc, "//ptoEmi") + "-" + ObtenerValorXML(xmlDoc, "//secuencial"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph("NÚMERO DE AUTORIZACIÓN", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph(ObtenerValorXML(xmlDoc, "//numeroAutorizacion"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph("FECHA Y HORA DE AUTORIZACIÓN", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph(ObtenerValorXML(xmlDoc, "//fechaAutorizacion"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph("AMBIENTE: " + (ObtenerValorXML(xmlDoc, "//ambiente") == "1" ? "PRUEBAS" : "PRODUCCIÓN"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph("EMISIÓN: " + (ObtenerValorXML(xmlDoc, "//tipoEmision") == "1" ? "NORMAL" : "INDISPONIBILIDAD DEL SISTEMA"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph("CLAVE DE ACCESO", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
    celdaInfoFactura.AddElement(new Paragraph(claveAcceso, FontFactory.GetFont(FontFactory.HELVETICA, 8)));

    // Generar el código de barras y agregarlo encima del número de clave de acceso
    ITImage codigoBarras = GenerarCodigoBarras(claveAcceso);

    if (codigoBarras != null)
    {
        codigoBarras.ScaleAbsoluteWidth(190f); // Ajustar el ancho
        codigoBarras.ScaleAbsoluteHeight(15f); // Ajustar la altura
        PdfPTable tablaInterna = new PdfPTable(1);
        tablaInterna.WidthPercentage = 100;
        PdfPCell celdaCodigoBarras = new PdfPCell(codigoBarras);
        celdaCodigoBarras.BorderWidth = 0; // Sin borde
        celdaCodigoBarras.HorizontalAlignment = Element.ALIGN_CENTER;
        tablaInterna.AddCell(celdaCodigoBarras);
        celdaInfoFactura.AddElement(tablaInterna);
    }

    tabla.AddCell(celdaInfoFactura);

    pdfDoc.Add(tabla);
}

        private void AgregarInformacionComprador(Document pdfDoc, XmlDocument xmlDoc)
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 100;

            AgregarCelda(tabla, "Razón Social / Nombres y Apellidos:", ObtenerValorXML(xmlDoc, "//razonSocialComprador"));
            AgregarCelda(tabla, "Identificación:", ObtenerValorXML(xmlDoc, "//identificacionComprador"));
            AgregarCelda(tabla, "Fecha:", ObtenerValorXML(xmlDoc, "//fechaEmision"));
            pdfDoc.Add(tabla);
        }

        private void AgregarDetallesFactura(Document pdfDoc, XmlDocument xmlDoc)
        {
            PdfPTable tabla = new PdfPTable(new float[] { 2, 1, 6, 2, 2, 2, 2 });
            tabla.WidthPercentage = 100;

            AgregarCeldaEncabezado(tabla, "Cod. Principal");
            AgregarCeldaEncabezado(tabla, "Cod. Auxiliar");
            AgregarCeldaEncabezado(tabla, "Descripción");
            AgregarCeldaEncabezado(tabla, "Cantidad");
            AgregarCeldaEncabezado(tabla, "Precio Unitario");
            AgregarCeldaEncabezado(tabla, "Descuento");
            AgregarCeldaEncabezado(tabla, "Precio Total");

            XmlNodeList detalles = xmlDoc.SelectNodes("//detalle");
            foreach (XmlNode detalle in detalles)
            {
                AgregarCelda(tabla, ObtenerValorXML(detalle, "codigoPrincipal"));
                AgregarCelda(tabla, ObtenerValorXML(detalle, "codigoAuxiliar"));
                AgregarCelda(tabla, ObtenerValorXML(detalle, "descripcion"));
                AgregarCelda(tabla, ObtenerValorXML(detalle, "cantidad"));
                AgregarCelda(tabla, ObtenerValorXML(detalle, "precioUnitario"));
                AgregarCelda(tabla, ObtenerValorXML(detalle, "descuento"));
                AgregarCelda(tabla, ObtenerValorXML(detalle, "precioTotalSinImpuesto"));
            }

            pdfDoc.Add(tabla);
        }

        private void AgregarTotales(Document pdfDoc, XmlDocument xmlDoc)
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 50;
            tabla.HorizontalAlignment = Element.ALIGN_RIGHT;

            AgregarCelda(tabla, "SUBTOTAL 15%:", ObtenerValorXML(xmlDoc, "//totalConImpuestos/totalImpuesto[codigo='2']/baseImponible"));
            AgregarCelda(tabla, "SUBTOTAL 0%:", ObtenerValorXML(xmlDoc, "//totalConImpuestos/totalImpuesto[codigo='0']/baseImponible"));
            AgregarCelda(tabla, "SUBTOTAL NO OBJETO DE IVA:", ObtenerValorXML(xmlDoc, "//totalSinImpuestos"));
            AgregarCelda(tabla, "SUBTOTAL EXENTO DE IVA:", "0.00");
            AgregarCelda(tabla, "SUBTOTAL SIN IMPUESTOS:", ObtenerValorXML(xmlDoc, "//totalSinImpuestos"));
            AgregarCelda(tabla, "TOTAL DESCUENTO:", ObtenerValorXML(xmlDoc, "//totalDescuento"));
            AgregarCelda(tabla, "ICE:", "0.00");
            AgregarCelda(tabla, "IVA 15%:", ObtenerValorXML(xmlDoc, "//totalConImpuestos/totalImpuesto[codigo='2']/valor"));
            AgregarCelda(tabla, "IRBPNR:", "0.00");
            AgregarCelda(tabla, "PROPINA:", ObtenerValorXML(xmlDoc, "//propina"));
            AgregarCelda(tabla, "VALOR TOTAL:", ObtenerValorXML(xmlDoc, "//importeTotal"));

            pdfDoc.Add(tabla);
        }

        private void AgregarFormasDePago(Document pdfDoc, XmlDocument xmlDoc)
        {
            Paragraph titulo = new Paragraph("Formas de Pago", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
            pdfDoc.Add(titulo);

            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 100;

            XmlNodeList formasPago = xmlDoc.SelectNodes("//pago");
            foreach (XmlNode pago in formasPago)
            {
                string formaPago = ObtenerValorXML(pago, "formaPago");
                string total = ObtenerValorXML(pago, "total");
                AgregarCelda(tabla, ObtenerDescripcionFormaPago(formaPago), total);
            }

            pdfDoc.Add(tabla);
        }

        private string ObtenerValorXML(XmlNode elemento, string nombreEtiqueta)
        {
            XmlNode node = elemento.SelectSingleNode(nombreEtiqueta);
            return node?.InnerText ?? string.Empty;
        }

        private void AgregarCelda(PdfPTable tabla, string texto)
        {
            PdfPCell celda = new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_MIDDLE;
            celda.MinimumHeight = 15f;
            tabla.AddCell(celda);
        }

        private void AgregarCelda(PdfPTable tabla, string etiqueta, string valor)
        {
            AgregarCelda(tabla, etiqueta);
            AgregarCelda(tabla, valor);
        }

        private void AgregarCeldaEncabezado(PdfPTable tabla, string texto)
        {
            PdfPCell celda = new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)));
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_MIDDLE;
            celda.BackgroundColor = BaseColor.LIGHT_GRAY;
            celda.MinimumHeight = 20f;
            tabla.AddCell(celda);
        }

        private void AgregarInformacionAdicional(Document pdfDoc, XmlDocument xmlDoc)
        {
            Paragraph titulo = new Paragraph("Información Adicional", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
            pdfDoc.Add(titulo);

            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 50;

            XmlNodeList infoAdicional = xmlDoc.SelectNodes("//infoAdicional/campoAdicional");
            foreach (XmlNode campo in infoAdicional)
            {
                string nombre = campo.Attributes["nombre"]?.Value ?? "";
                string valor = campo.InnerText;
                AgregarCelda(tabla, nombre, valor);
            }

            pdfDoc.Add(tabla);
        }

        private ITImage GenerarCodigoBarras(string claveAcceso)
        {
            BarcodeWriterPixelData barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 0
                }
            };

            var pixelData = barcodeWriter.Write(claveAcceso);

            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new DrawingRectangle(0, 0, pixelData.Width, pixelData.Height),
                        ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ITImage.GetInstance(ms.ToArray());
                }
            }
        }

        private string ObtenerDescripcionFormaPago(string codigoFormaPago)
        {
            switch (codigoFormaPago)
            {
                case "01": return "Sin utilización del sistema financiero";
                case "02": return "Cheque propio";
                case "03": return "Cheque certificado";
                case "04": return "Cheque de gerencia";
                case "05": return "Cheque del exterior";
                case "06": return "Débito de cuenta";
                case "07": return "Transferencia propio banco";
                case "08": return "Transferencia otro banco nacional";
                case "09": return "Transferencia banco exterior";
                case "10": return "Tarjeta de crédito nacional";
                case "11": return "Tarjeta de crédito internacional";
                case "12": return "Giro";
                case "13": return "Deposito en cuenta (corriente/ahorros)";
                case "14": return "Endoso de inversión";
                case "15": return "Compensación de deudas";
                case "16": return "Tarjeta de débito";
                case "17": return "Dinero electrónico";
                case "18": return "Tarjeta prepago";
                case "19": return "Tarjeta de crédito";
                case "20": return "Otros con utilización del sistema financiero";
                case "21": return "Endoso de títulos";
                default: return "Forma de pago desconocida";
            }
        }
    }
}