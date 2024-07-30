namespace ContaFacil.Utilities
{
    using ContaFacil.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Quartz;
    using static ContaFacil.Logica.NotificacionClass;

    public class TareaEnviarFacturacionSRI:IJob
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TareaEnviarFacturacionSRI> _logger;
        public TareaEnviarFacturacionSRI(ContableContext context, IConfiguration configuration, ILogger<TareaEnviarFacturacionSRI> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var fechaActual = DateOnly.FromDateTime(DateTime.Now);
            var facturas = await _context.Facturas
                .Where(factura => factura.Estado.Equals("Pendiente") && factura.Fecha == fechaActual)
                .ToListAsync();

            foreach (Factura factura in facturas)
            {
                await ReenviarSri(factura.IdFactura);
            }
            var facturasAutorizacion = await _context.Facturas
                .Where(factura => factura.ClaveAcceso!=null || factura.AutorizacionSri.Equals("NO AUTORIZADO") || factura.AutorizacionSri==null || factura.FechaAutorizacionSri==null)
                .ToListAsync();
            foreach (Factura factura in facturasAutorizacion)
            {
                await AutorizarSri(factura.IdFactura);
            }
            _logger.LogInformation("Tarea ejecutada en: {DateTime}", DateTime.Now);
        }

        public async Task ReenviarSri(int id)
        {
            var generator = new FacturaXmlGenerator(_configuration);
            Factura factura = await _context.Facturas
                .Where(f => f.IdFactura == id)
                .Include(f => f.IdEmisorNavigation)
                .FirstOrDefaultAsync();

            if (factura != null)
            {
                var result = await generator.EnviarXmlFirmadoYProcesarRespuesta(
                    factura.IdEmisorNavigation.TipoAmbiente,
                    factura.Xml,
                    factura.IdFactura);

                var (estado, descripcion) = result;

                factura.DescripcionSri = descripcion;
                factura.Estado = estado;
                _context.Update(factura);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AutorizarSri(int id)
        {
            var generator = new FacturaXmlGenerator(_configuration);
            Factura factura = await _context.Facturas
                .Where(f => f.IdFactura == id)
                .Include(f => f.IdEmisorNavigation)
                .FirstOrDefaultAsync();

            if (factura != null && factura.ClaveAcceso!=null)
            {
                var result = await generator.ConsultarAutorizacionAsync(factura.ClaveAcceso, factura.IdEmisorNavigation.TipoAmbiente);

                var (estado, fechaAutorizacion) = result;

                factura.AutorizacionSri = estado;
                factura.FechaAutorizacionSri = fechaAutorizacion;
                _context.Update(factura);
                await _context.SaveChangesAsync();
            }
        }
    }
}
