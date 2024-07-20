using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using ContaFacil.Utilities;
using ContaFacil.Models.ViewModel;

namespace ContaFacil.Controllers.Contador
{
    public class FacturacionContadorController : NotificacionClass
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        string idUsuario = "";
        string idEmpresa = "";
        public FacturacionContadorController(ContableContext context, IConfiguration configuration)
        {
            _configuration = configuration;

            _context = context;
        }

        // GET: FacturacionContador
        public async Task<IActionResult> Index(int? IdEmisor)
        {
            idEmpresa = HttpContext.Session.GetString("_empresa");
            var contableContext = _context.Facturas
                .Where(f => f.IdEmisorNavigation.IdEmpresa == int.Parse(idEmpresa));

            if (IdEmisor != null)
            {
                contableContext = contableContext.Where(f => f.IdEmisor == IdEmisor);
            }

            // Apply the Include methods after all Where clauses
            contableContext = contableContext
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.IdEmisorNavigation);

            var emisores = await _context.Emisors
                .Where(e => e.IdEmpresa == int.Parse(idEmpresa))
                .Select(e => new SelectListItem { Value = e.IdEmisor.ToString(), Text = e.RazonSocial })
                .ToListAsync();

            var viewModel = new FacturacionContadorViewModel
            {
                Facturas = await contableContext.ToListAsync(),
                Emisores = emisores,
                SelectedEmisorId = IdEmisor
            };

            return View(viewModel);
        }
        // GET: FacturacionContador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        public IActionResult Create()
        {
            idEmpresa = HttpContext.Session.GetString("_empresa");
            ViewData["IdEmisor"] = new SelectList(_context.Emisors.Where(e => e.IdEmpresa == int.Parse(idEmpresa)), "IdEmisor", "NombreComercial");
            ViewData["IdTipoPago"] = new SelectList(_context.TipoPagos,"IdTipoPago", "Nombre");
            ViewBag.Productos = _context.Productos.ToList();
            return View();
        }
        // POST: FacturacionContador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Factura factura,List<DetalleFactura> detalles)
        {
            try
            {
                idUsuario = HttpContext.Session.GetString("_idUsuario");
                idEmpresa = HttpContext.Session.GetString("_empresa");
                factura.Fecha = DateOnly.FromDateTime(DateTime.Now);
                factura.FechaCreacion = DateTime.Now;
                factura.UsuarioCreacion = int.Parse(idUsuario);
                factura.Estado = "Pendiente";
                factura.EstadoBoolean = true;
                _context.Facturas.Add(factura);
                _context.SaveChanges();
                foreach (var detalle in detalles)
                {
                    Producto producto = new Producto();
                    //producto = _context.Productos.FirstOrDefault(p => p.IdProducto==detalle.
                    detalle.IdFactura = factura.IdFactura;
                    detalle.FechaCreacion = DateTime.Now;
                    detalle.UsuarioCreacion = int.Parse(idUsuario);
                    detalle.Estado = true;
                    _context.DetalleFacturas.Add(detalle);
                }

                _context.SaveChanges();
                Cliente cliente = new Cliente();
                cliente = _context.Clientes.Where(c => c.IdCliente == factura.IdCliente).Include(c => c.IdPersonaNavigation).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e=>e.IdEmisor==factura.IdEmisor).FirstOrDefault();
                var generator = new FacturaXmlGenerator(_configuration);
                var xmlDocument = generator.GenerateXml(factura, cliente.IdPersonaNavigation, emisor);

                // Para guardar el XML en un archivo:
                xmlDocument.Save("factura.xml");

                // Para obtener el XML como string:
                string xmlString = xmlDocument.ToString();
                string xmlFirmado=generator.FirmarXml(xmlString,emisor.CertificadoDigital,emisor.Clave);
                var (estado, descripcion) = await generator.EnviarXmlFirmadoYProcesarRespuesta(emisor.TipoAmbiente,xmlFirmado, factura.IdFactura);
                factura.DescripcionSri = descripcion;
                factura.Estado = estado;
                _context.Update(factura);
                _context.SaveChanges();
                Notificacion("Registro guardado con exito", NotificacionTipo.Success);
                ViewData["IdEmisor"] = new SelectList(_context.Emisors.Where(e => e.IdEmpresa == int.Parse(idEmpresa)), "IdEmisor", "NombreComercial");
                ViewData["IdTipoPago"] = new SelectList(_context.TipoPagos, "IdTipoPago", "Nombre");
                ViewBag.Productos = _context.Productos.ToList();
                return RedirectToAction("Index", "FacturacionContador");
            }
            catch (Exception ex)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Productos = _context.Productos.ToList();
                Notificacion("Error al guardar el registro", NotificacionTipo.Error);
                return View(factura);
            }
        }

        // GET: FacturacionContador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", factura.IdCliente);
            return View(factura);
        }

        // POST: FacturacionContador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFactura,IdCliente,Fecha,MontoTotal,Estado,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Factura factura)
        {
            if (id != factura.IdFactura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.IdFactura))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", factura.IdCliente);
            return View(factura);
        }

        // GET: FacturacionContador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // POST: FacturacionContador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Facturas == null)
            {
                return Problem("Entity set 'ContableContext.Facturas'  is null.");
            }
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
          return (_context.Facturas?.Any(e => e.IdFactura == id)).GetValueOrDefault();
        }
        [HttpGet]
        public IActionResult GetClientesPorEmisor(int idEmisor)
        {
            var emisor = _context.Emisors.Find(idEmisor);
            if (emisor == null)
                return Json(new SelectList(Enumerable.Empty<SelectListItem>()));

            var empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);
            if (empresa == null)
                return Json(new SelectList(Enumerable.Empty<SelectListItem>()));

            var clientes = _context.Clientes
                .Where(c => c.IdEmpresa == empresa.IdEmpresa)
                .Select(c => new { c.IdCliente, c.IdPersonaNavigation.Nombre })
                .ToList();

            return Json(new SelectList(clientes, "IdCliente", "Nombre"));
        }
    }
}
