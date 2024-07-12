using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using ContaFacil.Utilities;
using System.Configuration;

namespace ContaFacil.Controllers
{
    public class FacturaController : NotificacionClass
    {
        private readonly ContableContext _context;
        private readonly IConfiguration _configuration;
        string idUsuario="";
        string idEmpresa="";
        public FacturaController(ContableContext context, IConfiguration configuration)
        {            
            _context = context;
            _configuration = configuration;
        }
        // GET: Factura
        public async Task<IActionResult> Index()
        {
            idUsuario = HttpContext.Session.GetString("_idUsuario");
            idEmpresa = HttpContext.Session.GetString("_empresa");
            var contableContext = _context.Facturas.Include(f => f.IdClienteNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Factura/Details/5
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
            ViewBag.Clientes = _context.Clientes.Include(p=>p.IdPersonaNavigation).ToList();
            ViewBag.Productos = _context.Productos.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Factura factura, List<DetalleFactura> detalles)
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
                cliente=_context.Clientes.Where(c=>c.IdCliente==factura.IdCliente).Include(c=>c.IdPersonaNavigation).FirstOrDefault();
                var generator = new FacturaXmlGenerator(_configuration);
                var xmlDocument = generator.GenerateXml(factura, cliente.IdPersonaNavigation, cliente.IdPersonaNavigation);

                // Para guardar el XML en un archivo:
                xmlDocument.Save("factura.xml");

                // Para obtener el XML como string:
                string xmlString = xmlDocument.ToString();
                generator.FirmarXml(xmlString);
                Notificacion("Registro guardado con exito", NotificacionTipo.Success);
                return RedirectToAction("Index", "Factura");
            }
            catch (Exception ex)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Productos = _context.Productos.ToList();
                Notificacion("Error al guardar el registro",NotificacionTipo.Error);
                return View(factura);
            }
        }

       
        // GET: Factura/Delete/5
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

        // POST: Factura/Delete/5
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
        public IActionResult GetProductoDetails(int idProducto)
        {
            var producto = _context.Productos
                .Where(p => p.IdProducto == idProducto)
                .Select(p => new
                {
                    p.IdProducto,
                    p.Codigo,
                    p.Nombre,
                    p.Descripcion,
                    p.PrecioUnitario,
                    p.Stock
                })
                .FirstOrDefault();

            if (producto == null)
            {
                return NotFound();
            }

            return Json(producto);
        }
    }
}