using ClosedXML.Excel;
using ContaFacil.Models.Dto;
using ContaFacil.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContaFacil.Models.Services
{
    public class ReporteMayorizacionService : IReporteMayorizacionService
    {
        private readonly ContableContext _context;
        private readonly string _webRootPath;

        public ReporteMayorizacionService(
            ContableContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webRootPath = webHostEnvironment.WebRootPath;
        }

        public async Task<byte[]> GenerarReporteMayorizacion(MayorizacionParametros parametros)
        {
            var contexto = await ObtenerContextoEmpresarial(parametros.UsuarioString);
            var transacciones = await ObtenerTransacciones(
                parametros.FechaInicio,
                parametros.FechaFin,
                contexto.Empresa.IdEmpresa);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Mayorización");
                ConfigurarEncabezadoReporte(worksheet, contexto.Empresa, contexto.Sucursal);

                int currentRow = ConfigurarEncabezadosTabla(worksheet);
                currentRow = await ProcesarTransacciones(worksheet, transacciones, currentRow);
                FormatearWorksheet(worksheet, currentRow);

                return GenerarArchivoExcel(workbook);
            }
        }

        public async Task<ContextoEmpresarial> ObtenerContextoEmpresarial(string idUsuario)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == int.Parse(idUsuario));

            var usuarioSucursal = await _context.UsuarioSucursals
                .FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);

            var sucursal = await _context.Sucursals
                .FirstOrDefaultAsync(s => s.IdSucursal == usuarioSucursal.IdSucursal);

            var persona = await _context.Personas
                .FirstOrDefaultAsync(p => p.IdPersona == usuario.IdPersona);

            var emisor = await _context.Emisors
                .FirstOrDefaultAsync(e => e.Ruc == persona.Identificacion);

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.Identificacion == emisor.Ruc);

            return new ContextoEmpresarial
            {
                Empresa = empresa,
                Sucursal = sucursal,
                Usuario = usuario
            };
        }

        private async Task<List<Transaccion>> ObtenerTransacciones(
            DateTime fechaInicio,
            DateTime fechaFin,
            int idEmpresa)
        {
            return await _context.Transaccions
                .Where(t => t.FechaCreacion >= fechaInicio &&
                           t.FechaCreacion <= fechaFin &&
                           t.IdEmpresa == idEmpresa)
                .Include(t => t.IdCuentaNavigation)
                .OrderBy(t => t.IdCuentaNavigation.Codigo)
                .ThenBy(t => t.FechaCreacion)
                .ToListAsync();
        }

        private void ConfigurarEncabezadoReporte(
            IXLWorksheet worksheet,
            Empresa empresa,
            Sucursal sucursal)
        {
            var logoPath = Path.Combine(_webRootPath, "img", "logo1.JPG");
            var logo = worksheet.AddPicture(logoPath)
                .MoveTo(worksheet.Cell("A1"))
                .Scale(0.25);

            worksheet.Cell("C1").Value = "MAYORIZACIÓN";
            worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
            worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
            worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";
        }

        // ... (Los demás métodos privados del servicio anterior se mantienen igual)

        private int ConfigurarEncabezadosTabla(IXLWorksheet worksheet)
        {
            int currentRow = 8;

            // Configurar encabezados
            worksheet.Cell(currentRow, 1).Value = "Fecha";
            worksheet.Cell(currentRow, 2).Value = "No. Asiento";
            worksheet.Cell(currentRow, 3).Value = "Código";
            worksheet.Cell(currentRow, 4).Value = "Cuenta";
            worksheet.Cell(currentRow, 5).Value = "Detalle";
            worksheet.Cell(currentRow, 6).Value = "Debe";
            worksheet.Cell(currentRow, 7).Value = "Haber";
            worksheet.Cell(currentRow, 8).Value = "Movimiento";

            // Dar formato a los encabezados
            var headerRange = worksheet.Range(currentRow, 1, currentRow, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            return currentRow + 1;
        }

        private async Task<int> ProcesarTransacciones(IXLWorksheet worksheet, List<Transaccion> transacciones, int startRow)
        {
            string lastCuenta = "";
            string nameCuenta = "";
            decimal totalMovimiento = 0;
            decimal saldoInicial = 0;
            int currentRow = startRow;
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
                var processingResult = ProcesarFilaTransaccion(worksheet, currentRow, transaccion, totalMovimiento, saldoInicial);
                totalMovimiento = processingResult.TotalMovimiento;
                saldoInicial = processingResult.SaldoInicial;
                cont++;
            }

            // Agregar total de la última cuenta
            currentRow++;
            worksheet.Cell(currentRow, 4).Value = $"Total {lastCuenta}" + " " + nameCuenta;
            worksheet.Cell(currentRow, 8).Value = totalMovimiento;
            worksheet.Range(currentRow, 1, currentRow, 8).Style.Font.Bold = true;

            await Task.CompletedTask;
            return currentRow;
        }
        private class TransactionProcessingResult
        {
            public decimal TotalMovimiento { get; set; }
            public decimal SaldoInicial { get; set; }
        }

        private TransactionProcessingResult ProcesarFilaTransaccion(
            IXLWorksheet worksheet,
            int currentRow,
            Transaccion transaccion,
            decimal totalMovimiento,
            decimal saldoInicial)
        {
            worksheet.Cell(currentRow, 1).Value = transaccion.FechaCreacion;
            worksheet.Cell(currentRow, 2).Value = transaccion.Descripcion.Split(' ')[0];
            worksheet.Cell(currentRow, 3).Value = transaccion.IdCuentaNavigation.Codigo;
            worksheet.Cell(currentRow, 4).Value = transaccion.IdCuentaNavigation.Nombre;
            worksheet.Cell(currentRow, 5).Value = transaccion.Descripcion;
            decimal montoAbs = Math.Abs(transaccion.Monto);
            if (transaccion.IdCuentaNavigation.Debito.GetValueOrDefault())
            {
                saldoInicial = montoAbs;
                worksheet.Cell(currentRow, 6).Value = saldoInicial;
                worksheet.Cell(currentRow, 7).Value = 0;
                worksheet.Cell(currentRow, 8).Value = saldoInicial;
                totalMovimiento = saldoInicial;
            }
            else if (transaccion.IdCuentaNavigation.Credito.GetValueOrDefault())
            {
                worksheet.Cell(currentRow, 6).Value = 0;
                worksheet.Cell(currentRow, 7).Value = montoAbs;
                worksheet.Cell(currentRow, 8).Value = -montoAbs;
                totalMovimiento -= montoAbs;
            }
            

            // Handle other cases as needed...
            return new TransactionProcessingResult
            {
                TotalMovimiento = totalMovimiento,
                SaldoInicial = saldoInicial
            };
        }

        private TransactionProcessingResult ProcesarInventario(
            IXLWorksheet worksheet,
            int currentRow,
            bool esSaldoInicial,
            bool esVenta,
            decimal montoAbs,
            decimal totalMovimiento,
            decimal saldoInicial)
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

            return new TransactionProcessingResult
            {
                TotalMovimiento = totalMovimiento,
                SaldoInicial = saldoInicial
            };
        }

        private void FormatearWorksheet(IXLWorksheet worksheet, int currentRow)
        {
            worksheet.Columns().AdjustToContents();

            var rangeNumerica = worksheet.Range(8, 6, currentRow, 8);
            rangeNumerica.Style.NumberFormat.Format = "#,##0.00";
        }

        private byte[] GenerarArchivoExcel(XLWorkbook workbook)
        {
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }

        private void ProcesarMontosTransaccion(IXLWorksheet worksheet, int currentRow, Transaccion transaccion,
            bool esInventario, bool esSaldoInicial, bool esVenta, decimal montoAbs,
            ref decimal totalMovimiento, ref decimal saldoInicial)
        {
            if (esInventario)
            {
                ProcesarInventario(worksheet, currentRow, esSaldoInicial, esVenta, montoAbs,
                    ref totalMovimiento, ref saldoInicial);
                return;
            }

          /* switch (transaccion.IdCuentaNavigation.Codigo)
            {
                case "1.1.4":
                    ProcesarCuenta114(worksheet, currentRow, transaccion.EsDebito, montoAbs, ref totalMovimiento);
                    break;
                case "2.1.2.1":
                    ProcesarCuenta2121(worksheet, currentRow, transaccion.EsDebito, montoAbs, ref totalMovimiento);
                    break;
                default:
                    ProcesarCuentaGeneral(worksheet, currentRow, transaccion, montoAbs, ref totalMovimiento);
                    break;
            }*/
        }

        private void ProcesarInventario(IXLWorksheet worksheet, int currentRow, bool esSaldoInicial,
            bool esVenta, decimal montoAbs, ref decimal totalMovimiento, ref decimal saldoInicial)
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


    }

}