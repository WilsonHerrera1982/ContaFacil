namespace ContaFacil.Utilities
{
    using ContaFacil.Models;
    using OfficeOpenXml;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class ExportService
    {
        public async Task<byte[]> ExportToExcelAsync(List<Factura> facturas)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Facturas");

            // Añadir encabezados
            worksheet.Cells[1, 1].Value = "Número Factura";
            worksheet.Cells[1, 2].Value = "Monto Total";
            worksheet.Cells[1, 3].Value = "Estado";
            worksheet.Cells[1, 4].Value = "Fecha Creación";
            worksheet.Cells[1, 5].Value = "Descripción SRI";
            worksheet.Cells[1, 6].Value = "Cliente";
            worksheet.Cells[1, 7].Value = "Clave Acceso";
            worksheet.Cells[1, 8].Value = "Autorización SRI";

            // Añadir datos
            int row = 2;
            foreach (var factura in facturas)
            {
                worksheet.Cells[row, 1].Value = factura.NumeroFactura;
                worksheet.Cells[row, 2].Value = factura.MontoTotal;
                worksheet.Cells[row, 3].Value = factura.Estado;
                worksheet.Cells[row, 4].Value = factura.FechaCreacion.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 5].Value = factura.DescripcionSri;
                worksheet.Cells[row, 6].Value = factura.IdClienteNavigation.IdPersonaNavigation.Nombre;
                worksheet.Cells[row, 7].Value = factura.ClaveAcceso;
                worksheet.Cells[row, 8].Value = factura.AutorizacionSri;
                row++;
            }

            // Ajustar ancho de columnas
            worksheet.Cells.AutoFitColumns();

            // Convertir el archivo a un array de bytes
            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            return stream.ToArray();
        }
    }

}
