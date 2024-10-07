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
    public class GeneradorRIDERetencion
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
                AgregarInformacionSujeto(pdfDoc, xmlDoc);
                AgregarDetallesRetencion(pdfDoc, xmlDoc);
                AgregarInformacionAdicional(pdfDoc, xmlDoc);

                pdfDoc.Close();
                return ms.ToArray();
            }
        }

        private void AgregarEncabezado(Document pdfDoc, XmlDocument xmlDoc, PdfWriter writer)
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 100;

            // Información de la empresa
            PdfPCell celdaInfoEmpresa = new PdfPCell();
            celdaInfoEmpresa.BorderWidth = 1f;
            celdaInfoEmpresa.BorderColor = BaseColor.BLACK;
            celdaInfoEmpresa.Padding = 5f;

            // Intentar agregar el logo
            string rutaLogo = @"C:\Users\Wilson\Documents\Wilson\Logo\logo.png";
            if (File.Exists(rutaLogo))
            {
                ITImage logo = ITImage.GetInstance(rutaLogo);
                logo.ScaleToFit(100f, 50f);
                celdaInfoEmpresa.AddElement(logo);
            }
            else
            {
                celdaInfoEmpresa.AddElement(new Paragraph("NO TIENE LOGO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
            }

            celdaInfoEmpresa.AddElement(new Paragraph(ObtenerValorXML(xmlDoc, "//razonSocial"), FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            celdaInfoEmpresa.AddElement(new Paragraph("Dirección Matriz: " + ObtenerValorXML(xmlDoc, "//dirMatriz"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoEmpresa.AddElement(new Paragraph("Dirección Sucursal: " + ObtenerValorXML(xmlDoc, "//dirEstablecimiento"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoEmpresa.AddElement(new Paragraph("OBLIGADO A LLEVAR CONTABILIDAD: " + ObtenerValorXML(xmlDoc, "//obligadoContabilidad"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            tabla.AddCell(celdaInfoEmpresa);

            // Información de la retención
            PdfPCell celdaInfoRetencion = new PdfPCell();
            celdaInfoRetencion.BorderWidth = 1f;
            celdaInfoRetencion.BorderColor = BaseColor.BLACK;
            celdaInfoRetencion.Padding = 5f;

            string claveAcceso = ObtenerValorXML(xmlDoc, "//claveAcceso");
            celdaInfoRetencion.AddElement(new Paragraph("R.U.C.: " + ObtenerValorXML(xmlDoc, "//ruc"), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
            celdaInfoRetencion.AddElement(new Paragraph("COMPROBANTE DE RETENCIÓN", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
            celdaInfoRetencion.AddElement(new Paragraph("No. " + ObtenerValorXML(xmlDoc, "//estab") + "-" + ObtenerValorXML(xmlDoc, "//ptoEmi") + "-" + ObtenerValorXML(xmlDoc, "//secuencial"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph("NÚMERO DE AUTORIZACIÓN", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph(ObtenerValorXML(xmlDoc, "//numeroAutorizacion"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph("FECHA Y HORA DE AUTORIZACIÓN", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph(ObtenerValorXML(xmlDoc, "//fechaAutorizacion"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph("AMBIENTE: " + (ObtenerValorXML(xmlDoc, "//ambiente") == "1" ? "PRUEBAS" : "PRODUCCIÓN"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph("EMISIÓN: " + (ObtenerValorXML(xmlDoc, "//tipoEmision") == "1" ? "NORMAL" : "INDISPONIBILIDAD DEL SISTEMA"), FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph("CLAVE DE ACCESO", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaInfoRetencion.AddElement(new Paragraph(claveAcceso, FontFactory.GetFont(FontFactory.HELVETICA, 8)));

            ITImage codigoBarras = GenerarCodigoBarras(claveAcceso);
            if (codigoBarras != null)
            {
                codigoBarras.ScaleAbsoluteWidth(190f);
                codigoBarras.ScaleAbsoluteHeight(15f);
                PdfPTable tablaInterna = new PdfPTable(1);
                tablaInterna.WidthPercentage = 100;
                PdfPCell celdaCodigoBarras = new PdfPCell(codigoBarras);
                celdaCodigoBarras.BorderWidth = 0;
                celdaCodigoBarras.HorizontalAlignment = Element.ALIGN_CENTER;
                tablaInterna.AddCell(celdaCodigoBarras);
                celdaInfoRetencion.AddElement(tablaInterna);
            }

            tabla.AddCell(celdaInfoRetencion);

            pdfDoc.Add(tabla);
        }

        private void AgregarInformacionSujeto(Document pdfDoc, XmlDocument xmlDoc)
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 100;

            // Alinear a la izquierda el texto en las celdas
            AgregarCeldaIzquierda(tabla, "Razón Social / Nombres y Apellidos:", ObtenerValorXML(xmlDoc, "//razonSocialSujetoRetenido"));
            AgregarCeldaIzquierda(tabla, "Identificación:", ObtenerValorXML(xmlDoc, "//identificacionSujetoRetenido"));
            AgregarCeldaIzquierda(tabla, "Fecha de Emisión:", ObtenerValorXML(xmlDoc, "//fechaEmision"));

            pdfDoc.Add(tabla);
        }

        // Método auxiliar para agregar celdas alineadas a la izquierda
        private void AgregarCeldaIzquierda(PdfPTable tabla, string etiqueta, string valor)
        {
            PdfPCell celdaEtiqueta = new PdfPCell(new Phrase(etiqueta, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaEtiqueta.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaEtiqueta.VerticalAlignment = Element.ALIGN_MIDDLE;
            tabla.AddCell(celdaEtiqueta);

            PdfPCell celdaValor = new PdfPCell(new Phrase(valor, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celdaValor.HorizontalAlignment = Element.ALIGN_LEFT;  // Alineación a la izquierda
            celdaValor.VerticalAlignment = Element.ALIGN_MIDDLE;
            tabla.AddCell(celdaValor);
        }


        private void AgregarDetallesRetencion(Document pdfDoc, XmlDocument xmlDoc)
        {
            PdfPTable tabla = new PdfPTable(new float[] { 2, 2, 2, 2, 2, 2, 2, 2 });
            tabla.WidthPercentage = 100;

            AgregarCeldaEncabezado(tabla, "Comprobante");
            AgregarCeldaEncabezado(tabla, "Número");
            AgregarCeldaEncabezado(tabla, "Fecha Emisión");
            AgregarCeldaEncabezado(tabla, "Ejercicio Fiscal");
            AgregarCeldaEncabezado(tabla, "Base Imponible");
            AgregarCeldaEncabezado(tabla, "Impuesto");
            AgregarCeldaEncabezado(tabla, "Porcentaje Retención");
            AgregarCeldaEncabezado(tabla, "Valor Retenido");

            XmlNodeList impuestos = xmlDoc.SelectNodes("//impuesto");

            // Extraer los detalles del primer comprobante para mostrar una sola vez
            string comprobante = ObtenerValorXML(impuestos[0], "codDocSustento");
            string numero = ObtenerValorXML(impuestos[0], "numDocSustento");
            string fechaEmision = ObtenerValorXML(impuestos[0], "fechaEmisionDocSustento");
            string ejercicioFiscal = ObtenerValorXML(xmlDoc, "//periodoFiscal");

            // Agregar detalles del comprobante (combinando las primeras cuatro celdas en una fila)
            PdfPCell cellComprobante = new PdfPCell(new Phrase(comprobante, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            cellComprobante.Rowspan = impuestos.Count; // Combina las celdas por el número de filas de impuestos
            cellComprobante.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.AddCell(cellComprobante);

            PdfPCell cellNumero = new PdfPCell(new Phrase(numero, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            cellNumero.Rowspan = impuestos.Count;
            cellNumero.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.AddCell(cellNumero);

            PdfPCell cellFechaEmision = new PdfPCell(new Phrase(fechaEmision, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            cellFechaEmision.Rowspan = impuestos.Count;
            cellFechaEmision.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.AddCell(cellFechaEmision);

            PdfPCell cellEjercicioFiscal = new PdfPCell(new Phrase(ejercicioFiscal, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            cellEjercicioFiscal.Rowspan = impuestos.Count;
            cellEjercicioFiscal.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.AddCell(cellEjercicioFiscal);

            // Iterar sobre los impuestos y agregar las celdas que corresponden a cada fila
            foreach (XmlNode impuesto in impuestos)
            {
                AgregarCelda(tabla, ObtenerValorXML(impuesto, "baseImponible"));
                AgregarCelda(tabla, ObtenerDescripcionImpuesto(ObtenerValorXML(impuesto, "codigo")));
                AgregarCelda(tabla, ObtenerValorXML(impuesto, "porcentajeRetener") + "%");
                AgregarCelda(tabla, ObtenerValorXML(impuesto, "valorRetenido"));
            }

            pdfDoc.Add(tabla);
        }


        private void AgregarInformacionAdicional(Document pdfDoc, XmlDocument xmlDoc)
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 100;

            XmlNodeList infoAdicional = xmlDoc.SelectNodes("//campoAdicional");
            if (infoAdicional != null && infoAdicional.Count > 0)
            {
                PdfPCell celdaTitulo = new PdfPCell(new Phrase("Información Adicional", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
                celdaTitulo.Colspan = 2;
                celdaTitulo.HorizontalAlignment = Element.ALIGN_LEFT;
                tabla.AddCell(celdaTitulo);

                foreach (XmlNode campo in infoAdicional)
                {
                    string nombre = campo.Attributes["nombre"]?.Value ?? "";
                    string valor = campo.InnerText;
                    AgregarCelda(tabla, nombre, valor);
                }

                pdfDoc.Add(tabla);
            }
        }

        private string ObtenerValorXML(XmlNode elemento, string nombreEtiqueta)
        {
            XmlNode node = elemento.SelectSingleNode(nombreEtiqueta);
            return node?.InnerText ?? string.Empty;
        }

        private void AgregarCelda(PdfPTable tabla, string texto)
        {
            PdfPCell celda = new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
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

        private string ObtenerDescripcionImpuesto(string codigoImpuesto)
        {
            switch (codigoImpuesto)
            {
                case "1": return "RENTA";
                case "2": return "IVA";
                case "3": return "ICE";
                case "6": return "ISD";
                default: return "OTRO";
            }
        }
    }
}