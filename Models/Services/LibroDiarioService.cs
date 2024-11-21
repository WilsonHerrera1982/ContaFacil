using ClosedXML.Excel;
using ContaFacil.Models.Dto;
using ContaFacil.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContaFacil.Models.Services
{
    public class LibroDiarioService : ILibroDiarioService
    {
        private readonly ContableContext _context;
        private readonly string _webRootPath;

        public LibroDiarioService(
            ContableContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webRootPath = webHostEnvironment.WebRootPath;
        }

        public async Task<byte[]> GenerarLibroDiario(LibroDiarioParametros parametros)
        {
            var contexto = await ObtenerContextoEmpresarial(parametros.UsuarioString);
            var transacciones = await ObtenerTransacciones(
                parametros.FechaInicio,
                parametros.FechaFin,
                contexto.Empresa.IdEmpresa);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Libro Diario");
                ConfigurarEncabezadoReporte(worksheet, contexto.Empresa, contexto.Sucursal);

                int currentRow = ConfigurarEncabezadosTabla(worksheet);
                currentRow = await ProcesarTransacciones(worksheet, transacciones, currentRow);
                FormatearWorksheet(worksheet, currentRow);

                return GenerarArchivoExcel(workbook);
            }
        }

        private async Task<ContextoEmpresarial> ObtenerContextoEmpresarial(string idUsuario)
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
            var cue2 = await _context.Cuenta
                .FirstOrDefaultAsync(c => c.Nombre == "Relacionadas por pagar");

            var transacciones = await _context.Transaccions
                .Where(t => t.FechaCreacion >= fechaInicio &&
                           t.FechaCreacion <= fechaFin &&
                           t.IdEmpresa == idEmpresa)
                .Include(t => t.IdCuentaNavigation)
                .ToListAsync();

            var compras = transacciones
                .Where(t => !t.Descripcion.ToLower().Contains("venta") &&
                           t.IdCuenta != cue2.IdCuenta);

            var ventas = transacciones
                .Where(t => t.Descripcion.ToLower().Contains("venta") &&
                           !t.Descripcion.ToLower().Contains("saldo inicial inventarios"))
                .OrderBy(t => t.FechaCreacion);

            var inicial = transacciones
                .Where(t => t.Descripcion.ToLower().Contains("inicial"));

            return compras.Concat(ventas).Concat(inicial)
                .OrderBy(t => t.FechaCreacion)
                .ToList();
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

            worksheet.Cell("C1").Value = "LIBRO DIARIO";
            worksheet.Cell("C2").Value = $"Nombre de cliente: {empresa.Nombre}";
            worksheet.Cell("C3").Value = $"RUC: {empresa.Identificacion}";
            worksheet.Cell("C4").Value = $"Sucursal: {sucursal.NombreSucursal}";
        }

        private int ConfigurarEncabezadosTabla(IXLWorksheet worksheet)
        {
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

            return currentRow;
        }

        private async Task<int> ProcesarTransacciones(
            IXLWorksheet worksheet,
            List<Transaccion> transacciones,
            int currentRow)
        {
            string lastAsiento = "";

            foreach (var transaccion in transacciones)
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
                ProcesarFilaTransaccion(worksheet, currentRow, transaccion);
            }

            await Task.CompletedTask;
            return currentRow;
        }

        private void ProcesarFilaTransaccion(
            IXLWorksheet worksheet,
            int currentRow,
            Transaccion transaccion)
        {
            worksheet.Cell(currentRow, 1).Value = transaccion.FechaCreacion;
            worksheet.Cell(currentRow, 2).Value = transaccion.Descripcion.Split(' ')[0];
            worksheet.Cell(currentRow, 3).Value = transaccion.IdCuentaNavigation.Codigo;
            worksheet.Cell(currentRow, 4).Value = transaccion.IdCuentaNavigation.Nombre;
            worksheet.Cell(currentRow, 5).Value = transaccion.Descripcion;

            var montoAbs = Math.Abs(transaccion.Monto);

            // Nueva lógica basada en el servicio de mayorización
            if (transaccion.IdCuentaNavigation.Debito.GetValueOrDefault() && transaccion.EsDebito == true)
            {
                worksheet.Cell(currentRow, 6).Value = montoAbs; // Debe
                worksheet.Cell(currentRow, 7).Value = 0; // Haber
                worksheet.Cell(currentRow, 8).Value = montoAbs; // Movimiento positivo
            }
            else if (transaccion.IdCuentaNavigation.Credito.GetValueOrDefault() || transaccion.EsDebito == false)
            {
                worksheet.Cell(currentRow, 6).Value = 0; // Debe
                worksheet.Cell(currentRow, 7).Value = montoAbs; // Haber
                worksheet.Cell(currentRow, 8).Value = -montoAbs; // Movimiento negativo
            }
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
    }
}
