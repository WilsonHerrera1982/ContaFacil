using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Models.ViewModel;
using ClosedXML.Excel;
using System.Security.Claims;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml;
using DocumentFormat.OpenXml.InkML;
namespace ContaFacil.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ContableContext _context;

        public ReporteController(ContableContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerarReporte(DateTime fechaInicio, DateTime fechaFin)
        {
            var facturas = await _context.Facturas
                .Include(f => f.IdSucursalNavigation)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(df => df.IdProductoNavigation)
                .Include(f => f.Pagos)
                    .ThenInclude(p => p.IdTipoPagoNavigation)
                .Where(f => f.Fecha >= DateOnly.FromDateTime(fechaInicio) && f.Fecha <= DateOnly.FromDateTime(fechaFin))
                .ToListAsync();

            var ventasPorDia = facturas
                .GroupBy(f => f.Fecha)
                .Select(g => new VentasPorDia { Fecha = g.Key, Total = g.Sum(f => f.MontoTotal) })
                .OrderBy(x => x.Fecha)
                .ToList();

            var ventasPorSucursal = facturas
                .GroupBy(f => f.IdSucursalNavigation.NombreSucursal)
                .Select(g => new VentasPorSucursal { Sucursal = g.Key, Total = g.Sum(f => f.MontoTotal) })
                .OrderByDescending(x => x.Total)
                .ToList();

            var ventasPorProducto = facturas
                .SelectMany(f => f.DetalleFacturas)
                .GroupBy(df => df.IdProductoNavigation.Nombre + " - " + df.IdProductoNavigation.Descripcion)
                .Select(g => new VentasPorProducto { Producto = g.Key, Total = g.Sum(df => df.PrecioUnitario * df.Cantidad) })
                .OrderByDescending(x => x.Total)
                .Take(10)
                .ToList();

            var ventasPorTipoPago = facturas
                .SelectMany(f => f.Pagos)
                .GroupBy(p => p.IdTipoPagoNavigation.Nombre)
                .Select(g => new VentasPorTipoPago { TipoPago = g.Key, Total = g.Sum(p => p.Monto) })
                .OrderByDescending(x => x.Total)
                .ToList();

            var ventasConDescuento = facturas
            .SelectMany(f => f.DetalleFacturas)
            .Where(df => df.Descuento > 0)  // Filtrar solo los detalles con descuento mayor a 0
            .GroupBy(df => df.IdProductoNavigation.Nombre + " - " + df.IdProductoNavigation.Descripcion)
            .Select(g => new VentasPromocion
            {
                Producto = g.Key,
                Total = g.Sum(df => df.PrecioUnitario * df.Cantidad)
            })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToList();

            var viewModel = new ReportesViewModel
            {
                VentasPorDia = ventasPorDia,
                VentasPorSucursal = ventasPorSucursal,
                VentasPorProducto = ventasPorProducto,
                VentasPorTipoPago = ventasPorTipoPago,
                VentasPromocion=ventasConDescuento,
            };
            ViewBag.FechaInicio = fechaInicio.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.ToString("yyyy-MM-dd");

            return View("Index", viewModel);
        }
       
public async Task<ActionResult> ExportarExcel(DateTime fechaInicio, DateTime fechaFin)
    {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            if (string.IsNullOrEmpty(idUsuario))
            {
                TempData["Error"] = "Usuario no autenticado.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
            var usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            var sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == usuarioSucursal.IdSucursal);
            var persona = _context.Personas.FirstOrDefault(p => p.IdPersona == usuario.IdPersona);
            var emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == persona.Identificacion);
            var empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);
            // Obtén los datos necesarios (similar a tu método GenerarReporte)
            var facturas = await _context.Facturas
                 .Include(f => f.IdSucursalNavigation)
                 .Include(f => f.DetalleFacturas)
                     .ThenInclude(df => df.IdProductoNavigation)
                 .Include(f => f.Pagos)
                     .ThenInclude(p => p.IdTipoPagoNavigation)
                 .Where(f => f.Fecha >= DateOnly.FromDateTime(fechaInicio) && f.Fecha <= DateOnly.FromDateTime(fechaFin))
                 .ToListAsync();

        var ventasPorDia = facturas
            .GroupBy(f => f.Fecha)
            .Select(g => new VentasPorDia { Fecha = g.Key, Total = g.Sum(f => f.MontoTotal) })
            .OrderBy(x => x.Fecha)
            .ToList();

        var ventasPorSucursal = facturas
            .GroupBy(f => f.IdSucursalNavigation.NombreSucursal)
            .Select(g => new VentasPorSucursal { Sucursal = g.Key, Total = g.Sum(f => f.MontoTotal) })
            .OrderByDescending(x => x.Total)
            .ToList();

        var ventasPorProducto = facturas
            .SelectMany(f => f.DetalleFacturas)
            .GroupBy(df => df.IdProductoNavigation.Nombre + " - " + df.IdProductoNavigation.Descripcion)
            .Select(g => new VentasPorProducto { Producto = g.Key, Total = g.Sum(df => df.PrecioUnitario * df.Cantidad) })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToList();

        var ventasPorTipoPago = facturas
            .SelectMany(f => f.Pagos)
            .GroupBy(p => p.IdTipoPagoNavigation.Nombre)
            .Select(g => new VentasPorTipoPago { TipoPago = g.Key, Total = g.Sum(p => p.Monto) })
            .OrderByDescending(x => x.Total)
            .ToList();

            var ventasConDescuento = facturas
            .SelectMany(f => f.DetalleFacturas)
            .Where(df => df.Descuento > 0)  // Filtrar solo los detalles con descuento mayor a 0
            .GroupBy(df => df.IdProductoNavigation.Nombre + " - " + df.IdProductoNavigation.Descripcion)
            .Select(g => new VentasPorProducto
            {
                Producto = g.Key,
                Total = g.Sum(df => df.PrecioUnitario * df.Cantidad)
            })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToList();


            using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Reporte");
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
            /*    if (System.IO.File.Exists(logoPath))
                {
                    var logo = worksheet.AddPicture(logoPath)
                        .MoveTo(worksheet.Cell("A1"))
                        .Scale(0.25);
                }
                else
                {
                    Console.WriteLine("El archivo de logo no existe en la ruta especificada.");
                }

               */
                // Agregar encabezado y datos generales
                worksheet.Cells["C1"].Value = "Reporte de Ventas";
                worksheet.Cells["C2"].Value = $"Nombre de cliente: {empresa.Nombre}";
                worksheet.Cells["C3"].Value = $"RUC: {empresa.Identificacion}";
                worksheet.Cells["C4"].Value = $"Periodo: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";

            // Ventas por Día
            int currentRow = 5;
            worksheet.Cells[currentRow, 1].Value = "Ventas por Día";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                currentRow++;
            worksheet.Cells[currentRow, 1].Value = "Fecha";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 2].Value = "Total";
                worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
                currentRow++;
            foreach (var venta in ventasPorDia)
            {
                    worksheet.Cells[currentRow, 1].Value = venta.Fecha.ToString("dd/MM/yyyy");
                    worksheet.Cells[currentRow, 2].Value = venta.Total;
                currentRow++;
            }

            var ventasPorDiaChart = worksheet.Drawings.AddChart("VentasPorDiaChart", eChartType.Line);
            ventasPorDiaChart.SetPosition(4, 0, 3, 0);
            ventasPorDiaChart.SetSize(800, 300);
            ventasPorDiaChart.Series.Add(ExcelRange.GetAddress(6, 2, currentRow - 1, 2), ExcelRange.GetAddress(6, 1, currentRow - 1, 1));
            ventasPorDiaChart.Title.Text = "Ventas por Día";
            ventasPorDiaChart.Title.Font.Bold = true;
            //    currentRow += Math.Max(15, ventasPorDia.Count() + 5);
                // Ventas por Sucursal
                currentRow += 2;
            worksheet.Cells[currentRow, 1].Value = "Ventas por Sucursal";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                currentRow++;
            worksheet.Cells[currentRow, 1].Value = "Sucursal";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 2].Value = "Total";
                worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
                currentRow++;
            foreach (var venta in ventasPorSucursal)
            {
                worksheet.Cells[currentRow, 1].Value = venta.Sucursal;
                worksheet.Cells[currentRow, 2].Value = venta.Total;
                currentRow++;
            }

            var ventasPorSucursalChart = worksheet.Drawings.AddChart("VentasPorSucursalChart", eChartType.ColumnClustered);
            ventasPorSucursalChart.SetPosition(currentRow - ventasPorSucursal.Count() - 2, 0, 3, 0);
            ventasPorSucursalChart.SetSize(800, 300);
            ventasPorSucursalChart.Series.Add(ExcelRange.GetAddress(currentRow - ventasPorSucursal.Count(), 2, currentRow - 1, 2),
                                              ExcelRange.GetAddress(currentRow - ventasPorSucursal.Count(), 1, currentRow - 1, 1));
            ventasPorSucursalChart.Title.Text = "Ventas por Sucursal";
                ventasPorSucursalChart.Title.Font.Bold = true;
                currentRow += Math.Max(15, ventasPorSucursal.Count() + 5);
                // Ventas por Producto (Top 10)
                currentRow += 2;
            worksheet.Cells[currentRow, 1].Value = "Top 10 Productos más Vendidos";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                currentRow++;
            worksheet.Cells[currentRow, 1].Value = "Producto";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 2].Value = "Total";
                worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
                currentRow++;
            foreach (var venta in ventasPorProducto.Take(10))
            {
                worksheet.Cells[currentRow, 1].Value = venta.Producto;
                worksheet.Cells[currentRow, 2].Value = venta.Total;
                currentRow++;
            }

            var ventasPorProductoChart = worksheet.Drawings.AddChart("VentasPorProductoChart", eChartType.Pie);
            ventasPorProductoChart.SetPosition(currentRow - 11, 0, 3, 0);
            ventasPorProductoChart.SetSize(800, 300);
            ventasPorProductoChart.Series.Add(ExcelRange.GetAddress(currentRow - 10, 2, currentRow - 1, 2),
                                              ExcelRange.GetAddress(currentRow - 10, 1, currentRow - 1, 1));
            ventasPorProductoChart.Title.Text = "Top 10 Productos más Vendidos";
                ventasPorProductoChart.Title.Font.Bold = true;
                currentRow += Math.Max(15, ventasPorProducto.Count() + 2);
                // Ventas por Tipo de Pago
                currentRow += 2;
            worksheet.Cells[currentRow, 1].Value = "Ventas por Tipo de Pago";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                currentRow++;
            worksheet.Cells[currentRow, 1].Value = "Tipo de Pago";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 2].Value = "Total";
                worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
                currentRow++;
            foreach (var venta in ventasPorTipoPago)
            {
                worksheet.Cells[currentRow, 1].Value = venta.TipoPago;
                worksheet.Cells[currentRow, 2].Value = venta.Total;
                currentRow++;
            }

            var ventasPorTipoPagoChart = worksheet.Drawings.AddChart("VentasPorTipoPagoChart", eChartType.Doughnut);
            ventasPorTipoPagoChart.SetPosition(currentRow - ventasPorTipoPago.Count() - 2, 0, 3, 0);
            ventasPorTipoPagoChart.SetSize(800, 300);
            ventasPorTipoPagoChart.Series.Add(ExcelRange.GetAddress(currentRow - ventasPorTipoPago.Count(), 2, currentRow - 1, 2),
                                              ExcelRange.GetAddress(currentRow - ventasPorTipoPago.Count(), 1, currentRow - 1, 1));
            ventasPorTipoPagoChart.Title.Text = "Ventas por Tipo de Pago";
                ventasPorTipoPagoChart.Title.Font.Bold = true;
                currentRow += Math.Max(15, ventasPorTipoPago.Count() + 2);
                // Ventas de promociones
                currentRow += 2;
                worksheet.Cells[currentRow, 1].Value = "Venta de Promociones";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                currentRow++;
                worksheet.Cells[currentRow, 1].Value = "Producto";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 2].Value = "Total";
                worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
                currentRow++;
                foreach (var venta in ventasConDescuento)
                {
                    worksheet.Cells[currentRow, 1].Value = venta.Producto;
                    worksheet.Cells[currentRow, 2].Value = venta.Total;
                    currentRow++;
                }

                var ventasPromocionesChart = worksheet.Drawings.AddChart("VentasPromocionesChart", eChartType.Pie);
                ventasPromocionesChart.SetPosition(currentRow - 7, 0, 3, 0);
                ventasPromocionesChart.SetSize(800, 300);
                ventasPromocionesChart.Series.Add(ExcelRange.GetAddress(currentRow - ventasConDescuento.Count(), 2, currentRow - 1, 2),
                                              ExcelRange.GetAddress(currentRow - ventasConDescuento.Count(), 1, currentRow - 1, 1));
                ventasPromocionesChart.Title.Text = "Venta de Promociones";
                ventasPromocionesChart.Title.Font.Bold = true;
                currentRow += Math.Max(15, ventasConDescuento.Count() + 2);

                worksheet.Cells.AutoFitColumns();

            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte de Ventas.xlsx");
        }
    }
}
}