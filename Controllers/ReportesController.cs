using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using Microsoft.DiaSymReader;
using System;
using System.Text.RegularExpressions;

namespace ContaFacil.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ContableContext _context;

        public ReportesController(ContableContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExportarLibroDiarioExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            ContaFacil.Models.Sucursal sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == usuarioSucursal.IdSucursal);
            Persona persona = new Persona();
            persona = _context.Personas.Where(p => p.IdPersona == usuario.IdPersona).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == persona.Identificacion).FirstOrDefault();
            Empresa empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);

            if (fechaInicio == default || fechaFin == default)
            {
                TempData["Error"] = "Las fechas de inicio y fin son obligatorias.";
                return RedirectToAction(nameof(Index));
            }

            var transacciones = await _context.Transaccions
                .Where(t => t.FechaCreacion >= fechaInicio && t.FechaCreacion <= fechaFin && t.IdEmpresa == empresa.IdEmpresa)
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.IdTransaccion)
                .Include(t => t.IdCuentaNavigation)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Libro Diario");
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                var logo = worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.25);

                // Agregar encabezado
                worksheet.Cell("C1").Value = "LIBRO DIARIO";
                worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
                worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
                worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";

                // Encabezados de la tabla
                int currentRow = 8;
                worksheet.Cell(currentRow, 1).Value = "Fecha";
                worksheet.Cell(currentRow, 2).Value = "No. Asiento";
                worksheet.Cell(currentRow, 3).Value = "Código";
                worksheet.Cell(currentRow, 4).Value = "Cuenta";
                worksheet.Cell(currentRow, 5).Value = "Detalle";
                worksheet.Cell(currentRow, 6).Value = "Debe";
                worksheet.Cell(currentRow, 7).Value = "Haber";
                worksheet.Cell(currentRow, 8).Value = "Movimiento";

                // Estilo para encabezados
                var headerRange = worksheet.Range(currentRow, 1, currentRow, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                currentRow++;

                Cuentum cue2 = _context.Cuenta.FirstOrDefault(c => c.Nombre == "Relacionadas por pagar");
                string lastAsiento = "";
                var compras = transacciones.Where(t => !t.Descripcion.ToLower().Contains("venta") && t.IdCuenta != cue2.IdCuenta);
                var ventas = transacciones
                    .Where(t => t.Descripcion.ToLower().Contains("venta") &&
                                !t.Descripcion.ToLower().Contains("saldo inicial inventarios"))
                    .OrderBy(t => t.FechaCreacion)
                    .ToList();

                var inicial = transacciones.Where(t => t.Descripcion.ToLower().Contains("inicial")).ToList();

                // Procesar todas las transacciones en orden cronológico
                foreach (var transaccion in compras.Concat(ventas).Concat(inicial).OrderBy(t => t.FechaCreacion))
                {
                    if (transaccion.Descripcion.Split(' ')[0] != lastAsiento)
                    {
                        if (lastAsiento != "")
                        {
                            currentRow++; // Agregar fila vacía después de cada asiento completo
                        }
                        lastAsiento = transaccion.Descripcion.Split(' ')[0];
                    }

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = transaccion.FechaCreacion;
                    worksheet.Cell(currentRow, 2).Value = transaccion.Descripcion.Split(' ')[0];
                    worksheet.Cell(currentRow, 3).Value = transaccion.IdCuentaNavigation.Codigo;
                    worksheet.Cell(currentRow, 4).Value = transaccion.IdCuentaNavigation.Nombre;
                    worksheet.Cell(currentRow, 5).Value = transaccion.Descripcion;
                    // Manejo especial para Anticipos de clientes y Cuentas por cobrar
                    if (transaccion.IdCuentaNavigation.Codigo == "1.1.4" && transaccion.EsDebito)
                    {
                        worksheet.Cell(currentRow, 6).Value = 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = -Math.Abs(transaccion.Monto); // Haber
                        worksheet.Cell(currentRow, 8).Value = -Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else if (transaccion.IdCuentaNavigation.Codigo == "2.1.2.1" && transaccion.EsDebito)
                    {
                        worksheet.Cell(currentRow, 6).Value = Math.Abs(transaccion.Monto); // Debe
                        worksheet.Cell(currentRow, 7).Value = 0; // Haber
                        worksheet.Cell(currentRow, 8).Value = Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                   
                    else if (transaccion.IdCuentaNavigation.Codigo.Contains("1.1.2.")&& transaccion.EsDebito)
                    {
                        worksheet.Cell(currentRow, 6).Value = Math.Abs(transaccion.Monto); // Debe
                        worksheet.Cell(currentRow, 7).Value = 0; // Haber
                        worksheet.Cell(currentRow, 8).Value = Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else if (transaccion.IdCuentaNavigation.Codigo.Contains("1.1.2.") && !transaccion.EsDebito)
                    {
                        worksheet.Cell(currentRow, 6).Value = 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = Math.Abs(transaccion.Monto); // Haber
                        worksheet.Cell(currentRow, 8).Value = -Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else if (transaccion.IdCuentaNavigation.Codigo == "2.1.3.1")
                    {
                        worksheet.Cell(currentRow, 6).Value = 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = Math.Abs(transaccion.Monto); // Haber
                        worksheet.Cell(currentRow, 8).Value = -Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else if (transaccion.IdCuentaNavigation.Codigo == "2.1.5.1")
                    {
                        worksheet.Cell(currentRow, 6).Value = 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = Math.Abs(transaccion.Monto); // Haber
                        worksheet.Cell(currentRow, 8).Value = -Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    // Manejo especial para Anticipos de clientes y Cuentas por cobrar
                    else if (transaccion.IdCuentaNavigation.Codigo == "2.1.2.1")
                    {
                        worksheet.Cell(currentRow, 6).Value = 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = Math.Abs(transaccion.Monto); // Haber
                        worksheet.Cell(currentRow, 8).Value = -Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else if (transaccion.IdCuentaNavigation.Codigo == "1.1.4")
                    {
                        worksheet.Cell(currentRow, 6).Value = Math.Abs(transaccion.Monto); // Debe
                        worksheet.Cell(currentRow, 7).Value = 0; // Haber
                        worksheet.Cell(currentRow, 8).Value = Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else if (transaccion.IdCuentaNavigation.Codigo != "4.1.1" && transaccion.IdCuentaNavigation.Codigo.Contains("4.1."))
                    {
                        worksheet.Cell(currentRow, 6).Value = 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = Math.Abs(transaccion.Monto); // Haber
                        worksheet.Cell(currentRow, 8).Value = -Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else if (transaccion.IdCuentaNavigation.Codigo.Contains( "2.1.3.") && transaccion.EsDebito)
                    {
                        worksheet.Cell(currentRow, 6).Value = 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = Math.Abs(transaccion.Monto); // Haber
                        worksheet.Cell(currentRow, 8).Value = -Math.Abs(transaccion.Monto); // Movimiento negativo
                    }
                    else
                    {
                        // Manejo normal para otras cuentas
                        worksheet.Cell(currentRow, 6).Value = transaccion.Monto > 0 ? transaccion.Monto : 0; // Debe
                        worksheet.Cell(currentRow, 7).Value = transaccion.Monto < 0 ? Math.Abs(transaccion.Monto) : 0; // Haber
                        worksheet.Cell(currentRow, 8).Value = transaccion.Monto; // Movimiento
                    }
                }

                // Ajustar anchos de columna
                worksheet.Columns().AdjustToContents();

                // Formato para columnas numéricas
                var rangeNumerica = worksheet.Range(8, 6, currentRow, 8);
                rangeNumerica.Style.NumberFormat.Format = "#,##0.00";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LibroDiario.xlsx");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportarMayorizacionExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            ContaFacil.Models.Sucursal sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == usuarioSucursal.IdSucursal);
            Persona persona = _context.Personas.FirstOrDefault(p => p.IdPersona == usuario.IdPersona);
            Emisor emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == persona.Identificacion);
            Empresa empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);

            if (fechaInicio == default || fechaFin == default)
            {
                TempData["Error"] = "Las fechas de inicio y fin son obligatorias.";
                return RedirectToAction(nameof(Index));
            }

            var transacciones = await _context.Transaccions
                .Where(t => t.FechaCreacion >= fechaInicio && t.FechaCreacion <= fechaFin && t.IdEmpresa == empresa.IdEmpresa)
                .OrderBy(t => t.IdCuentaNavigation.Codigo)
                .ThenBy(t => t.FechaCreacion)
                .Include(t => t.IdCuentaNavigation)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Mayorización");
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                var logo = worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.25);

                worksheet.Cell("C1").Value = "MAYORIZACIÓN";
                worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
                worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
                worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";

                int currentRow = 8;
                worksheet.Cell(currentRow, 1).Value = "Fecha";
                worksheet.Cell(currentRow, 2).Value = "No. Asiento";
                worksheet.Cell(currentRow, 3).Value = "Código";
                worksheet.Cell(currentRow, 4).Value = "Cuenta";
                worksheet.Cell(currentRow, 5).Value = "Detalle";
                worksheet.Cell(currentRow, 6).Value = "Debe";
                worksheet.Cell(currentRow, 7).Value = "Haber";
                worksheet.Cell(currentRow, 8).Value = "Movimiento";

                var headerRange = worksheet.Range(currentRow, 1, currentRow, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                currentRow++;

                string lastCuenta = "";
                string nameCuenta = "";
                decimal totalMovimiento = 0;
                decimal saldoInicial = 0;
                int cont = 1;
                foreach (var transaccion in transacciones)
                {
                    if (transaccion.IdCuentaNavigation.Codigo != lastCuenta)
                    {
                        if (cont == 1)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 3).Value = transaccion.IdCuentaNavigation.Codigo;
                            worksheet.Cell(currentRow, 4).Value = transaccion.IdCuentaNavigation.Nombre;
                            worksheet.Range(currentRow, 3, currentRow, 4).Style.Font.Bold = true;
                            currentRow++;
                        }
                        if (lastCuenta != "")
                        {
                            // Agregar total de la cuenta anterior
                            currentRow++;
                            worksheet.Cell(currentRow, 4).Value = $"Total {lastCuenta}" + " " + nameCuenta;
                            worksheet.Cell(currentRow, 8).Value = totalMovimiento;
                            worksheet.Range(currentRow, 1, currentRow, 8).Style.Font.Bold = true;

                            // Agregar número de cuenta y nombre en negrita
                            currentRow += 2;
                            worksheet.Cell(currentRow, 3).Value = transaccion.IdCuentaNavigation.Codigo;
                            worksheet.Cell(currentRow, 4).Value = transaccion.IdCuentaNavigation.Nombre;
                            worksheet.Range(currentRow, 3, currentRow, 4).Style.Font.Bold = true;
                            currentRow++;
                        }
                        // Reiniciar para la nueva cuenta
                        lastCuenta = transaccion.IdCuentaNavigation.Codigo;
                        nameCuenta = transaccion.IdCuentaNavigation.Nombre;
                        totalMovimiento = 0;
                        saldoInicial = 0;
                    }

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = transaccion.FechaCreacion;
                    worksheet.Cell(currentRow, 2).Value = transaccion.Descripcion.Split(' ')[0];
                    worksheet.Cell(currentRow, 3).Value = transaccion.IdCuentaNavigation.Codigo;
                    worksheet.Cell(currentRow, 4).Value = transaccion.IdCuentaNavigation.Nombre;
                    worksheet.Cell(currentRow, 5).Value = transaccion.Descripcion;

                    bool esInventario = transaccion.IdCuentaNavigation.Codigo.StartsWith("1.1.2");
                    bool esSaldoInicial = transaccion.Descripcion.Contains("Saldo inicial");
                    bool esVenta = transaccion.Descripcion.Contains("Venta de mercadería");
                    decimal montoAbs = Math.Abs(transaccion.Monto);

                    if (esInventario)
                    {
                        if (esSaldoInicial)
                        {
                            saldoInicial = montoAbs;
                            worksheet.Cell(currentRow, 6).Value = saldoInicial;
                            worksheet.Cell(currentRow, 7).Value = 0;
                            worksheet.Cell(currentRow, 8).Value = saldoInicial;
                            totalMovimiento = saldoInicial;
                        }
                        else if (esVenta)
                        {
                            worksheet.Cell(currentRow, 6).Value = 0;
                            worksheet.Cell(currentRow, 7).Value = montoAbs;
                            worksheet.Cell(currentRow, 8).Value = -montoAbs;
                            totalMovimiento -= montoAbs;
                        }
                        else // Compra u otro movimiento positivo
                        {
                            worksheet.Cell(currentRow, 6).Value = montoAbs;
                            worksheet.Cell(currentRow, 7).Value = 0;
                            worksheet.Cell(currentRow, 8).Value = montoAbs;
                            totalMovimiento += montoAbs;
                        }
                    }
                    else
                    {
                        // Manejo especial para las cuentas 1.1.4 y 2.1.2.1
                        if (transaccion.IdCuentaNavigation.Codigo == "1.1.4")
                        {
                            if (transaccion.EsDebito)
                            {
                                worksheet.Cell(currentRow, 6).Value = 0;
                                worksheet.Cell(currentRow, 7).Value = montoAbs;
                                worksheet.Cell(currentRow, 8).Value = -montoAbs;
                                totalMovimiento -= montoAbs;
                            }
                            else
                            {
                                worksheet.Cell(currentRow, 6).Value = montoAbs;
                                worksheet.Cell(currentRow, 7).Value = 0;
                                worksheet.Cell(currentRow, 8).Value = montoAbs;
                                totalMovimiento += montoAbs;
                            }
                        }
                        else if (transaccion.IdCuentaNavigation.Codigo == "2.1.2.1")
                        {
                            if (transaccion.EsDebito)
                            {
                                worksheet.Cell(currentRow, 6).Value = montoAbs;
                                worksheet.Cell(currentRow, 7).Value = 0;
                                worksheet.Cell(currentRow, 8).Value = montoAbs;
                                totalMovimiento += montoAbs;
                            }
                            else
                            {
                                worksheet.Cell(currentRow, 6).Value = 0;
                                worksheet.Cell(currentRow, 7).Value = montoAbs;
                                worksheet.Cell(currentRow, 8).Value = -montoAbs;
                                totalMovimiento -= montoAbs;
                            }
                        }
                        else if (transaccion.IdCuentaNavigation.Codigo == "2.1.1.1" && transaccion.EsDebito)
                        {
                            worksheet.Cell(currentRow, 6).Value = montoAbs;
                            worksheet.Cell(currentRow, 7).Value = 0;
                            worksheet.Cell(currentRow, 8).Value = montoAbs;
                            totalMovimiento += montoAbs;

                        }
                        else if (transaccion.IdCuentaNavigation.Codigo.Contains("2.1.3.") && transaccion.IdCuentaNavigation.Nombre.Contains("Retención IR"))
                        {

                            worksheet.Cell(currentRow, 6).Value = 0;
                            worksheet.Cell(currentRow, 7).Value = montoAbs;
                            worksheet.Cell(currentRow, 8).Value = -montoAbs;
                            totalMovimiento -= montoAbs;

                        }
                        else if (transaccion.IdCuentaNavigation.Codigo.Contains("2.1.3.") && transaccion.IdCuentaNavigation.Nombre.Contains("Retención IVA"))
                        {

                            worksheet.Cell(currentRow, 6).Value = 0;
                            worksheet.Cell(currentRow, 7).Value = montoAbs;
                            worksheet.Cell(currentRow, 8).Value = -montoAbs;
                            totalMovimiento -= montoAbs;

                        }
                        else if (transaccion.IdCuentaNavigation.Codigo.Contains("1.1.3.") && transaccion.IdCuentaNavigation.Nombre.Contains("Retención IR"))
                        {

                            worksheet.Cell(currentRow, 6).Value = montoAbs;
                            worksheet.Cell(currentRow, 7).Value = 0;
                            worksheet.Cell(currentRow, 8).Value = montoAbs;
                            totalMovimiento += montoAbs;

                        }
                        else if (transaccion.IdCuentaNavigation.Codigo.Contains("1.1.3.") && transaccion.IdCuentaNavigation.Nombre.Contains("Retención IVA"))
                        {

                            worksheet.Cell(currentRow, 6).Value = montoAbs;
                            worksheet.Cell(currentRow, 7).Value = 0;
                            worksheet.Cell(currentRow, 8).Value = montoAbs;
                            totalMovimiento += montoAbs;

                        }
                        else
                        {
                            bool esPositivo = DeterminarSiEsPositivo(transaccion.IdCuentaNavigation);

                            if (esPositivo)
                            {
                                worksheet.Cell(currentRow, 6).Value = montoAbs;
                                worksheet.Cell(currentRow, 7).Value = 0;
                                worksheet.Cell(currentRow, 8).Value = montoAbs;
                                totalMovimiento += montoAbs;
                            }
                            else
                            {
                                worksheet.Cell(currentRow, 6).Value = 0;
                                worksheet.Cell(currentRow, 7).Value = montoAbs;
                                worksheet.Cell(currentRow, 8).Value = -montoAbs;
                                totalMovimiento -= montoAbs;
                            }
                        }
                    }
                    cont++;
                }

                // Agregar total de la última cuenta
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = $"Total {lastCuenta}" + " " + nameCuenta;
                worksheet.Cell(currentRow, 8).Value = totalMovimiento;
                worksheet.Range(currentRow, 1, currentRow, 8).Style.Font.Bold = true;

                currentRow += 2;
                worksheet.Columns().AdjustToContents();

                var rangeNumerica = worksheet.Range(8, 6, currentRow, 8);
                rangeNumerica.Style.NumberFormat.Format = "#,##0.00";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mayorizacion.xlsx");
                }
            }
        }
         [HttpPost]
        public async Task<IActionResult> ExportEstadoExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            ContaFacil.Models.Sucursal sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == usuarioSucursal.IdSucursal);
            Persona persona = _context.Personas.FirstOrDefault(p => p.IdPersona == usuario.IdPersona);
            Emisor emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == persona.Identificacion);
            Empresa empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);

            if (fechaFin == default)
            {
                TempData["Error"] = "La fecha de corte es obligatoria.";
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine($"Buscando transacciones para la empresa {empresa.IdEmpresa} entre {fechaInicio} y {fechaFin}");

            var transacciones = await _context.Transaccions
                .Where(t => t.FechaCreacion >= fechaInicio && t.FechaCreacion <= fechaFin && t.IdEmpresa == empresa.IdEmpresa)
                .Include(t => t.IdCuentaNavigation)
                .ToListAsync();

            Console.WriteLine($"Número de transacciones encontradas: {transacciones.Count}");

            if (transacciones.Count == 0)
            {
                TempData["Error"] = "No se encontraron transacciones para el período seleccionado.";
                return RedirectToAction(nameof(Index));
            }

            var balanceData = await GenerarBalanceItems(transacciones, fechaFin);

            Console.WriteLine($"Número de items en el balance: {balanceData.Count}");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Estado de Resultados");
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                var logo = worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.25);

                worksheet.Cell("C1").Value = "Estado de Resultados";
                worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
                worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
                worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";
                worksheet.Cell("C5").Value = $"Fecha de corte: {fechaFin:dd/MM/yyyy}";

                int currentRow = 7;
                

                currentRow++;
                //worksheet.Cell(currentRow, 1).Value = "Estado de resultados";
                worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Código";
                worksheet.Cell(currentRow, 2).Value = "Cuenta";
                worksheet.Cell(currentRow, 3).Value = "Movimiento";
                worksheet.Cell(currentRow, 4).Value = "Subtotal 1";
                worksheet.Cell(currentRow, 5).Value = "Total";
                //worksheet.Cell(currentRow, 6).Value = "Total";
                worksheet.Range(currentRow, 1, currentRow, 5).Style.Font.Bold = true;
                worksheet.Range(currentRow, 1, currentRow, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                currentRow++;

                var estadoResultados = balanceData.Where(item => item.Codigo.StartsWith("4") || item.Codigo.StartsWith("5")).ToList();
                decimal totalIngresos = 0;
                decimal totalGastos = 0;
                decimal sub41 = estadoResultados
                    .Where(e => e.Codigo == "4.1.3")   // Filtra por Codigo "4.1.1"
                    .Sum(e => e.Subtotal1 ?? 0); 
               decimal descuento = estadoResultados
                    .Where(e => e.Codigo == "4.1.1")   // Filtra por Codigo "4.1.1"
                    .Sum(e => e.Subtotal1 ?? 0);
                foreach (var item in estadoResultados)
                {
                    worksheet.Cell(currentRow, 1).Value = item.Codigo;
                    worksheet.Cell(currentRow, 2).Value = item.Cuenta;
                    string cadena = item.Codigo;
                    string[] partes = cadena.Split('.');
                     // Verificamos si las primeras dos partes son "4" y "1"
                    if (partes.Length == 3 && partes[0] == "4" && partes[1] == "1")
                    {
                        // Obtenemos el último número y verificamos si es 3 o mayor
                        int ultimoNumero = int.Parse(partes[2]);

                        if (ultimoNumero >= 3)
                        {
                            item.Subtotal1 = item.Subtotal1 * -1;
                            totalIngresos += item.Subtotal1 ?? 0;
                        }
                    }

                   /* if (item.Codigo.Equals("4.1.1"))
                    {
                        descuento = descuento + item.Subtotal1??0;
                    }*/
                   
                    if (item.Codigo.Equals("4.1"))
                    {
                        item.Movimiento = item.Movimiento - descuento;
                        item.Movimiento = item.Movimiento * -1;
                        item.Subtotal2 = (item.Subtotal2 - descuento);
                        item.Subtotal2 = (item.Subtotal2 - descuento) * -1;
                       // sub41 = item.Subtotal2??0;
                    }
                    if (item.Codigo.Equals("4"))
                    {
                        item.Movimiento = sub41-descuento;
                        item.Total = sub41 - descuento; 
                        item.Total=item.Total * -1;
                    }
                    var nivelCuenta = item.Codigo.Split('.').Length;

                    if (nivelCuenta == 1)
                    {
                        worksheet.Cell(currentRow, 5).Value = item.Total;
                        if (item.Codigo.StartsWith("4"))
                        {
                           // totalIngresos += item.Total ?? 0;
                        }
                        else if (item.Codigo.StartsWith("5"))
                        {
                            totalGastos += item.Total ?? 0;
                        }
                    }
                    else if (nivelCuenta == 2)
                    {
                        worksheet.Cell(currentRow, 4).Value = item.Subtotal2;
                    }
                    else if (nivelCuenta >= 3)
                    {
                        worksheet.Cell(currentRow, 2).Value = item.Movimiento;
                        worksheet.Cell(currentRow, 3).Value = item.Subtotal1;
                    }

                    if (nivelCuenta == 1 || nivelCuenta == 2 || string.IsNullOrEmpty(item.Codigo))
                    {
                        worksheet.Range(currentRow, 1, currentRow, 5).Style.Font.Bold = true;
                    }
                    
                    worksheet.Range(currentRow, 1, currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(currentRow, 2).Value = new string(' ', (nivelCuenta - 1) * 4) + item.Cuenta;

                    currentRow++;
                }

                // Calcular la utilidad
                decimal utilidad = (totalIngresos + descuento ) + Math.Abs(totalGastos); // Restando los gastos como valor absoluto


                // Agregar la utilidad al final del estado de resultados
                currentRow++;
               // worksheet.Cell(currentRow, 1).Value = "Utilidad";
                worksheet.Cell(currentRow, 2).Value = "Utilidad del ejercicio";
                worksheet.Cell(currentRow, 5).Value = utilidad;
                worksheet.Range(currentRow, 1, currentRow, 5).Style.Font.Bold = true;

                // Agregar la utilidad al final del estado de resultados
                /*currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Utilidad";
                worksheet.Cell(currentRow, 2).Value = "Utilidad del ejercicio";
                worksheet.Cell(currentRow, 6).Value = utilidad;
                worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;*/

                worksheet.Columns().AdjustToContents();
                var rangeNumerica = worksheet.Range(9, 3, currentRow - 1, 5);
                rangeNumerica.Style.NumberFormat.Format = "#,##0.00";

                Console.WriteLine("Preparando el archivo Excel para descarga");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    Console.WriteLine($"Tamaño del archivo Excel generado: {content.Length} bytes");
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EstadoResultados.xlsx");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportarBalanceExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == int.Parse(idUsuario));
            UsuarioSucursal usuarioSucursal = _context.UsuarioSucursals.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            ContaFacil.Models.Sucursal sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == usuarioSucursal.IdSucursal);
            Persona persona = _context.Personas.FirstOrDefault(p => p.IdPersona == usuario.IdPersona);
            Emisor emisor = _context.Emisors.FirstOrDefault(e => e.Ruc == persona.Identificacion);
            Empresa empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);

            if (fechaFin == default)
            {
                TempData["Error"] = "La fecha de corte es obligatoria.";
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine($"Buscando transacciones para la empresa {empresa.IdEmpresa} entre {fechaInicio} y {fechaFin}");

            var transacciones = await _context.Transaccions
                .Where(t => t.FechaCreacion >= fechaInicio && t.FechaCreacion <= fechaFin && t.IdEmpresa == empresa.IdEmpresa)
                .Include(t => t.IdCuentaNavigation)
                .ToListAsync();

            Console.WriteLine($"Número de transacciones encontradas: {transacciones.Count}");

            if (transacciones.Count == 0)
            {
                TempData["Error"] = "No se encontraron transacciones para el período seleccionado.";
                return RedirectToAction(nameof(Index));
            }

            var balanceData = await GenerarBalanceItems(transacciones, fechaFin);

            Console.WriteLine($"Número de items en el balance: {balanceData.Count}");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Estado de Situación Financiera");
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo1.JPG");
                var logo = worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.25);

                worksheet.Cell("C1").Value = "Estado de Situación Financiera";
                worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
                worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
                worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";
                worksheet.Cell("C5").Value = $"Fecha de corte: {fechaFin:dd/MM/yyyy}";

                int currentRow = 8;
                worksheet.Cell(currentRow, 1).Value = "Código";
                worksheet.Cell(currentRow, 2).Value = "Cuenta";
                worksheet.Cell(currentRow, 3).Value = "Movimiento";
                worksheet.Cell(currentRow, 4).Value = "Subtotal 1";
                worksheet.Cell(currentRow, 5).Value = "Subtotal 2";
                worksheet.Cell(currentRow, 6).Value = "Total";

                var headerRange = worksheet.Range(currentRow, 1, currentRow, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                currentRow++;
                decimal totalIngresos = 0;
                decimal totalGastos = 0;
                decimal descuento = 0;
                foreach (var item in balanceData.Where(i => !i.Codigo.StartsWith("4") && !i.Codigo.StartsWith("5")))
                {
                    worksheet.Cell(currentRow, 1).Value = item.Codigo;
                    worksheet.Cell(currentRow, 2).Value = item.Cuenta;
                    string cadena = item.Codigo;
                    string[] partes = cadena.Split('.');

                    // Verificamos si las primeras dos partes son "4" y "1"
                    if (partes.Length == 3 && partes[0] == "4" && partes[1] == "1")
                    {
                        // Obtenemos el último número y verificamos si es 3 o mayor
                        int ultimoNumero = int.Parse(partes[2]);

                        if (ultimoNumero >= 3)
                        {
                            //item.Subtotal1 = item.Subtotal1 * -1;
                            totalIngresos += item.Subtotal1 ?? 0;
                        }
                    }
                    if (item.Codigo.Equals("2.1.1"))
                    {
                        item.Subtotal1 = item.Subtotal1 * -1;
                    }
                    if (item.Codigo.Equals("2.1.1.1"))
                    {
                        item.Movimiento = item.Movimiento * -1;
                    }
                    if (item.Codigo.Contains("2.1.3.")&& item.Movimiento>0)
                    {
                        item.Movimiento= item.Movimiento * -1;
                    }
                    var nivelCuenta = item.Codigo.Split('.').Length;
                    if (item.Codigo.Equals("4.1.1"))
                    {
                        descuento = descuento + item.Subtotal1 ?? 0;
                    }
                    if (item.Codigo.StartsWith("1.1.2")) // Inventario
                    {
                        if (nivelCuenta == 3) // 1.1.2
                        {
                            worksheet.Cell(currentRow, 4).Value = item.Subtotal1;
                        }
                        else if (nivelCuenta >= 4) // 1.1.2.1, 1.1.2.2, etc.
                        {
                            worksheet.Cell(currentRow, 3).Value = item.Movimiento;
                        }
                    }
                    else if (item.Codigo.StartsWith("3")) // Patrimonio
                    {
                        if (nivelCuenta == 1) // 3
                        {
                            worksheet.Cell(currentRow, 6).Value = item.Total;
                        }
                        else if (nivelCuenta == 2) // 3.1, 3.2, 3.3
                        {
                            worksheet.Cell(currentRow, 5).Value = item.Subtotal1;
                        }
                        else if (nivelCuenta >= 3) // 3.2.1, 3.3.1, etc.
                        {
                            worksheet.Cell(currentRow, 3).Value = item.Movimiento;
                        }
                    }
                    else // Otras cuentas
                    {
                        if (nivelCuenta == 1)
                        {
                            worksheet.Cell(currentRow, 6).Value = item.Total;
                        }
                        else if (nivelCuenta == 2)
                        {
                            worksheet.Cell(currentRow, 5).Value = item.Subtotal2;
                        }
                        else if (nivelCuenta == 3)
                        {
                            worksheet.Cell(currentRow, 4).Value = item.Subtotal1;
                        }
                        else if (nivelCuenta >= 4)
                        {
                            worksheet.Cell(currentRow, 3).Value = item.Movimiento;
                        }
                    }
                    if (item.Codigo.Equals("2.1"))
                    {
                        item.Subtotal1 = item.Total;
                    }
                    if (nivelCuenta == 1 || nivelCuenta == 2 || string.IsNullOrEmpty(item.Codigo))
                    {
                        worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                    }

                    worksheet.Range(currentRow, 1, currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(currentRow, 2).Value = new string(' ', (nivelCuenta - 1) * 4) + item.Cuenta;

                    currentRow++;
                }

                /*currentRow++;
                                worksheet.Cell(currentRow, 1).Value = "Estado de resultados";
                                worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                                currentRow++;

                                worksheet.Cell(currentRow, 1).Value = "Código";
                                worksheet.Cell(currentRow, 2).Value = "Cuenta";
                                worksheet.Cell(currentRow, 3).Value = "Movimiento";
                                worksheet.Cell(currentRow, 4).Value = "Subtotal 1";
                                worksheet.Cell(currentRow, 5).Value = "Subtotal 2";
                                worksheet.Cell(currentRow, 6).Value = "Total";
                                worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                                worksheet.Range(currentRow, 1, currentRow, 6).Style.Fill.BackgroundColor = XLColor.LightGray;
                                currentRow++;
                */
                var estadoResultados = balanceData.Where(item => item.Codigo.StartsWith("4") || item.Codigo.StartsWith("5")).ToList();

                foreach (var item in estadoResultados)
                {
                    // worksheet.Cell(currentRow, 1).Value = item.Codigo;
                    // worksheet.Cell(currentRow, 2).Value = item.Cuenta;

                    var nivelCuenta = item.Codigo.Split('.').Length;

                    string cadena = item.Codigo;
                    string[] partes = cadena.Split('.');

                    // Verificamos si las primeras dos partes son "4" y "1"
                    if (partes.Length == 3 && partes[0] == "4" && partes[1] == "1")
                    {
                        // Obtenemos el último número y verificamos si es 3 o mayor
                        int ultimoNumero = int.Parse(partes[2]);

                        if (ultimoNumero >= 3)
                        {
                            //item.Subtotal1 = item.Subtotal1 * -1;
                            totalIngresos += item.Subtotal1 ?? 0;
                        }
                    }

                    if (item.Codigo.Equals("4.1.1"))
                    {
                        descuento = descuento + item.Subtotal1 ?? 0;
                    }
                    if (nivelCuenta == 1)
                    {
                        // worksheet.Cell(currentRow, 6).Value = item.Total;
                        if (item.Codigo.StartsWith("4"))
                        {
                            // totalIngresos += item.Total ?? 0;
                        }
                        else if (item.Codigo.StartsWith("5"))
                        {
                            totalGastos += item.Total ?? 0;
                        }
                    }
                    else if (nivelCuenta == 2)
                    {
                        //  worksheet.Cell(currentRow, 5).Value = item.Subtotal2;
                    }
                    else if (nivelCuenta >= 3)
                    {
                        //   worksheet.Cell(currentRow, 3).Value = item.Movimiento;
                        //  worksheet.Cell(currentRow, 4).Value = item.Subtotal1;
                    }

                    if (nivelCuenta == 1 || nivelCuenta == 2 || string.IsNullOrEmpty(item.Codigo))
                    {
                        //   worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                    }

                    /* worksheet.Range(currentRow, 1, currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                     worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                     worksheet.Cell(currentRow, 2).Value = new string(' ', (nivelCuenta - 1) * 4) + item.Cuenta;

                     currentRow++;*/
                }

                // Calcular la utilidad correctamente
                // decimal utilidad = totalIngresos + totalGastos; // Sumamos porque los gastos ya son negativos
                // Calcular la utilidad
                totalGastos = totalGastos * -1;
                decimal subt = (totalIngresos - descuento) * -1;
                decimal utilidad = subt + Math.Abs(totalGastos); // Restando los gastos como valor absoluto

                // Agregar la utilidad al final del estado de resultados
                /* currentRow++;
                 worksheet.Cell(currentRow, 1).Value = "Utilidad";
                 worksheet.Cell(currentRow, 2).Value = "Utilidad del ejercicio";
                 worksheet.Cell(currentRow, 6).Value = utilidad;
                 worksheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                */
                worksheet.Columns().AdjustToContents();
                var rangeNumerica = worksheet.Range(9, 3, currentRow - 1, 6);
                rangeNumerica.Style.NumberFormat.Format = "#,##0.00";

                Console.WriteLine("Preparando el archivo Excel para descarga");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    Console.WriteLine($"Tamaño del archivo Excel generado: {content.Length} bytes");
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EstadoSituacionFinanciera.xlsx");
                }
            }
        }
        private async Task<List<BalanceItem>> GenerarBalanceItems(List<Transaccion> transacciones, DateTime fechaFin)
        {
            var balanceItems = new Dictionary<string, BalanceItem>();

            string[] categoriasPrincipales = { "1", "2", "3", "4", "5" };
            foreach (var categoria in categoriasPrincipales)
            {
                balanceItems[categoria] = new BalanceItem
                {
                    Codigo = categoria,
                    Cuenta = ObtenerNombreCategoriaPrincipal(categoria),
                    Movimiento = 0,
                    Subtotal1 = null,
                    Subtotal2 = null,
                    Total = null
                };
            }

            var cuentasAgrupadas = transacciones
                .GroupBy(t => new {
                    Codigo = t.IdCuentaNavigation.Codigo,
                    Nombre = t.IdCuentaNavigation.Nombre
                })
                .Select(g => new {
                    g.Key.Codigo,
                    g.Key.Nombre,
                    Movimiento = g.Sum(t => t.EsDebito ? -t.Monto : t.Monto)
                })
                .OrderBy(g => g.Codigo);

            foreach (var grupo in cuentasAgrupadas)
            {
                var codigoCuenta = grupo.Codigo;
                var nombreCuenta = grupo.Nombre;
                var movimiento = grupo.Movimiento;

                if (codigoCuenta.StartsWith("1.1.2")) // Inventario
                {
                    var saldoInicial = transacciones
                        .Where(t => t.IdCuentaNavigation.Codigo == codigoCuenta && t.Descripcion.Contains("Saldo inicial"))
                        .Sum(t => t.EsDebito ? t.Monto : t.Monto);
                    var compras = transacciones
                        .Where(t => t.IdCuentaNavigation.Codigo == codigoCuenta && t.Descripcion.Contains("Compra"))
                        .Sum(t => t.EsDebito ? t.Monto : t.Monto);
                    var ventas = transacciones
                        .Where(t => t.IdCuentaNavigation.Codigo == codigoCuenta && t.Descripcion.Contains("Venta"))
                        .Sum(t => t.EsDebito ? -t.Monto : t.Monto);
                    movimiento = saldoInicial + compras - ventas;
                }
                else if (codigoCuenta.StartsWith("2.1.1.1")) // Proveedores
                {
                    var saldoInicial = transacciones
                        .Where(t => t.IdCuentaNavigation.Codigo == codigoCuenta)
                        .Sum(t => !t.EsDebito ? t.Monto : t.Monto);
                   
                    movimiento = saldoInicial;
                }
                else if (codigoCuenta.StartsWith("2.1.3.")) // Proveedores
                {
                    var saldoInicial = transacciones
                        .Where(t => t.IdCuentaNavigation.Codigo.Contains(codigoCuenta))
                        .Sum(t => !t.EsDebito ? t.Monto : t.Monto);

                    movimiento = saldoInicial;
                }
                // Invertir el signo para cuentas de Pasivo y Patrimonio
                if (codigoCuenta.StartsWith("2") || codigoCuenta.StartsWith("3"))
                {
                    movimiento = -movimiento;
                }

                AgregarOActualizarCuenta(balanceItems, codigoCuenta, nombreCuenta, movimiento);
            }

            ActualizarSubtotalesInventario(balanceItems);
            decimal descuento = 0;
            decimal totalIngresos = 0;
            foreach (var item in balanceItems.Values.OrderByDescending(i => i.Codigo.Split('.').Length))
            {
                string cadena = item.Codigo;
                string[] partes = cadena.Split('.');

                // Verificamos si las primeras dos partes son "4" y "1"
                if (partes.Length == 3 && partes[0] == "4" && partes[1] == "1")
                {
                    // Obtenemos el último número y verificamos si es 3 o mayor
                    int ultimoNumero = int.Parse(partes[2]);

                    if (ultimoNumero >= 3)
                    {
                        item.Subtotal1 = item.Movimiento * -1;
                        totalIngresos += item.Subtotal1 ?? 0;
                    }
                }
                var nivelCuenta = item.Codigo.Split('.').Length;
                if (item.Codigo.Equals("4.1.1"))
                {
                    descuento = descuento + item.Movimiento ?? 0;
                }
                if (nivelCuenta == 1)
                {
                    item.Total = item.Movimiento;
                }
                else if (nivelCuenta == 2)
                {
                    item.Subtotal2 = item.Movimiento;
                    var codigoPadre = item.Codigo.Split('.')[0];
                    if (balanceItems.ContainsKey(codigoPadre))
                    {
                        balanceItems[codigoPadre].Total += item.Subtotal2;
                    }
                }
                else if (nivelCuenta == 3)
                {
                    item.Subtotal1 = item.Movimiento;
                    item.Movimiento = null;
                    var codigoPadre = string.Join(".", item.Codigo.Split('.').Take(2));
                    if (balanceItems.ContainsKey(codigoPadre))
                    {
                        balanceItems[codigoPadre].Subtotal2 += item.Subtotal1.Value;
                    }
                }
                else
                {
                    var codigoPadre = string.Join(".", item.Codigo.Split('.').Take(3));
                    if (balanceItems.ContainsKey(codigoPadre))
                    {
                        balanceItems[codigoPadre].Subtotal1 += item.Movimiento;
                    }
                }
            }

            // Calcular la utilidad
            decimal totalGastos = balanceItems["5"].Total ?? 0;
            decimal des = totalIngresos + descuento;
            decimal utilidad = des + totalGastos;

            // Agregar las cuentas de patrimonio
            AgregarOActualizarCuenta(balanceItems, "3", "Patrimonio", 0);
            AgregarOActualizarCuenta(balanceItems, "3.1", "Capital", 0);
            AgregarOActualizarCuenta(balanceItems, "3.2", "Reserva", 0);
            AgregarOActualizarCuenta(balanceItems, "3.2.1", "Reserva legal", 0);
            AgregarOActualizarCuenta(balanceItems, "3.3", "Utilidad", utilidad);
            AgregarOActualizarCuenta(balanceItems, "3.3.1", $"Utilidad {fechaFin.Year}", utilidad);

            ActualizarTotalesPatrimonio(balanceItems);
            ActualizarTotalesPasivo(balanceItems);

            // Imprimir los valores para depuración
            foreach (var item in balanceItems.Where(i => i.Key.StartsWith("3")))
            {
                Console.WriteLine($"{item.Key}: {item.Value.Cuenta} - Movimiento: {item.Value.Movimiento}, Subtotal1: {item.Value.Subtotal1}, Subtotal2: {item.Value.Subtotal2}, Total: {item.Value.Total}");
            }
            return balanceItems.Values.OrderBy(item => item.Codigo).ToList();
        }
        private void ActualizarSubtotalesInventario(Dictionary<string, BalanceItem> balanceItems)
        {
            var cuentasInventario = balanceItems.Where(kv => kv.Key.StartsWith("1.1.2")).OrderByDescending(kv => kv.Key.Split('.').Length).ToList();

            foreach (var item in cuentasInventario)
            {
              
                var partes = item.Key.Split('.');
                if (partes.Length > 3) // Es una subcuenta de inventario
                {
                    var codigoPadre = string.Join(".", partes.Take(3));
                    if (balanceItems.TryGetValue(codigoPadre, out var cuentaPadre))
                    {
                        cuentaPadre.Subtotal1 = (cuentaPadre.Subtotal1 ?? 0) + item.Value.Movimiento;
                    }
                }
            }
        }

        private void ActualizarTotalesPasivo(Dictionary<string, BalanceItem> balanceItems)
        {
            // Actualizar Utilidad del año (3.3.1)
            if (balanceItems.TryGetValue("2.3.1", out var utilidadAnioItem))
            {
                // No cambiar el valor de Movimiento, ya que se estableció correctamente en AgregarOActualizarCuenta
            }

            // Actualizar Utilidad (3.3)
            if (balanceItems.TryGetValue("2.3", out var utilidadItem))
            {
                utilidadItem.Subtotal1 = balanceItems.Values
                    .Where(item => item.Codigo.StartsWith("2.3.") && item.Codigo.Split('.').Length == 3)
                    .Sum(item => item.Movimiento ?? 0);
            }

            // Actualizar Reserva (3.2)
            if (balanceItems.TryGetValue("2.1", out var reservaItem))
            {
                reservaItem.Subtotal2 = balanceItems.Values
                 .Where(item => item.Codigo.StartsWith("2.1.") && item.Codigo.Split('.').Length == 3)
                 .Sum(item => -(Math.Abs(item.Subtotal1 ?? 0)));
            }

            // Actualizar Patrimonio (3)
            if (balanceItems.TryGetValue("2", out var patrimonioItem))
            {
                patrimonioItem.Total = balanceItems.Values
                .Where(item => item.Codigo.StartsWith("2.1.") && item.Codigo.Split('.').Length == 3)
                .Sum(item => -(Math.Abs(item.Subtotal1 ?? 0)));
            }
        }
        private void ActualizarTotalesPatrimonio(Dictionary<string, BalanceItem> balanceItems)
        {
            // Actualizar Utilidad del año (3.3.1)
            if (balanceItems.TryGetValue("3.3.1", out var utilidadAnioItem))
            {
                // No cambiar el valor de Movimiento, ya que se estableció correctamente en AgregarOActualizarCuenta
            }

            // Actualizar Utilidad (3.3)
            if (balanceItems.TryGetValue("3.3", out var utilidadItem))
            {
                utilidadItem.Subtotal1 = balanceItems.Values
                    .Where(item => item.Codigo.StartsWith("3.3.") && item.Codigo.Split('.').Length == 3)
                    .Sum(item => item.Movimiento ?? 0);
            }

            // Actualizar Reserva (3.2)
            if (balanceItems.TryGetValue("3.2", out var reservaItem))
            {
                reservaItem.Subtotal1 = balanceItems.Values
                    .Where(item => item.Codigo.StartsWith("3.2.") && item.Codigo.Split('.').Length == 3)
                    .Sum(item => item.Movimiento ?? 0);
            }

            // Actualizar Patrimonio (3)
            if (balanceItems.TryGetValue("3", out var patrimonioItem))
            {
                patrimonioItem.Total = balanceItems.Values
                    .Where(item => item.Codigo.StartsWith("3.") && item.Codigo.Split('.').Length == 2)
                    .Sum(item => item.Subtotal1 ?? 0);
            }
        }
        private void AgregarOActualizarCuenta(Dictionary<string, BalanceItem> balanceItems, string codigo, string nombre, decimal valor)
        {
            if (!balanceItems.ContainsKey(codigo))
            {
                balanceItems[codigo] = new BalanceItem
                {
                    Codigo = codigo,
                    Cuenta = nombre,
                    Movimiento = valor,
                    Subtotal1 = null,
                    Subtotal2 = null,
                    Total = null
                };
            }
            else
            {
                balanceItems[codigo].Movimiento = valor; // Para inventario, este valor ya está calculado correctamente
            }

            var partes = codigo.Split('.');
            for (int i = 1; i < partes.Length; i++)
            {
                var codigoPadre = string.Join(".", partes.Take(i));
                if (!balanceItems.ContainsKey(codigoPadre))
                {
                    balanceItems[codigoPadre] = new BalanceItem
                    {
                        Codigo = codigoPadre,
                        Cuenta = ObtenerNombreCuentaPadre(codigoPadre),
                        Movimiento = 0,
                        Subtotal1 = null,
                        Subtotal2 = null,
                        Total = null
                    };
                }

                if (codigoPadre.StartsWith("1.1.2")) // Para cuentas de inventario
                {
                    balanceItems[codigoPadre].Movimiento += valor;
                }
                else
                {
                    // Para otras cuentas, mantenemos la lógica original
                    balanceItems[codigoPadre].Movimiento += valor;
                }
            }
        }
        private string ObtenerNombreCuentaPadre(string codigo)
        {
            Cuentum cuenta = _context.Cuenta.FirstOrDefault(c => c.Codigo == codigo);

            if (cuenta != null)
            {
                return cuenta.Nombre;
            }

            switch (codigo)
            {
                case "1.1": return "Activo Corriente";
                case "1.1.1": return "Efectivo y equivalentes de efectivo";
                case "1.1.2": return "Inventario";
                default: return $"Cuenta {codigo}";
            }
        }

        private string ObtenerNombreCategoriaPrincipal(string codigo)
        {
            switch (codigo)
            {
                case "1": return "Activo";
                case "2": return "Pasivo";
                case "3": return "Patrimonio";
                case "4": return "Ingresos";
                case "5": return "Costos y Gastos";
                default: return $"Categoría {codigo}";
            }
        }

        public class BalanceItem
        {
            public string Codigo { get; set; }
            public string Cuenta { get; set; }
            public decimal? Movimiento { get; set; }
            public decimal? Subtotal1 { get; set; }
            public decimal? Subtotal2 { get; set; }
            public decimal? Total { get; set; }
        }

        private bool DeterminarSiEsPositivo(Cuentum cuenta)
        {
            string codigoCuenta = cuenta.Codigo.Split('.')[0];
            string nombreCuenta = cuenta.Nombre.ToLower();
            if(cuenta.Nombre.Equals("Descuento en ventas"))
            {
                codigoCuenta = "5";
            }
            
            return codigoCuenta switch
            {
                "1" => true, // Activo
                "2" => false, // Pasivo
                "3" => false, // Patrimonio
                "4" => false, // Ingresos
                "5" => true, // Costos y Gastos
                _ => !(nombreCuenta.Contains("iva por pagar") ||
                       nombreCuenta.Contains("proveedores") ||
                       nombreCuenta.Contains("por pagar"))
            };
        }

        private bool DeterminarPositivoNegativo(Transaccion transaccion)
        {
            Cuentum cuenta = transaccion.IdCuentaNavigation;
            string codigoCuenta = cuenta.Codigo.Split('.')[0];
            string nombreCuenta = cuenta.Nombre.ToLower();
            if (cuenta.Nombre.Equals("Anticipo de clientes") && transaccion.EsDebito)
            {
                codigoCuenta = "5";
            }

            return codigoCuenta switch
            {
                "1" => true, // Activo
                "2" => false, // Pasivo
                "3" => false, // Patrimonio
                "4" => false, // Ingresos
                "5" => true, // Costos y Gastos
                _ => !(nombreCuenta.Contains("iva por pagar") ||
                       nombreCuenta.Contains("proveedores") ||
                       nombreCuenta.Contains("por pagar"))
            };
        }
    }
}