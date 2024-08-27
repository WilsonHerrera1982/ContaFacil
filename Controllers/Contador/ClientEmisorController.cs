using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Models.ViewModel;
namespace ContaFacil.Controllers.Contador
{
    public class ClientEmisorController : Controller
    {
        private readonly ContableContext _context;

        public ClientEmisorController(ContableContext context)
        {
            _context = context;
        }

        // GET: ClientEmisor
        public async Task<IActionResult> Index(int? IdEmisor)
        {
            // Obtener la lista de emisores para el dropdown
            IEnumerable<SelectListItem> emisores = await _context.Emisors
                .Select(e => new SelectListItem
                {
                    Value = e.IdEmisor.ToString(),
                    Text = e.NombreComercial
                })
                .ToListAsync();

            string idEmpresa = HttpContext.Session.GetString("_empresa");

            // Iniciar la consulta de clientes
            IEnumerable<Cliente> clientesQuery = await _context.Clientes
                .Where(c => c.IdEmpresa == int.Parse(idEmpresa))
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdPersonaNavigation)
                .ToListAsync();

            // Si se proporciona un IdEmisor, filtrar los clientes
            if (IdEmisor.HasValue)
            {
                var emisor = await _context.Emisors
                    .Where(e => e.IdEmisor == IdEmisor.Value)
                    .Select(e => new { e.Ruc })
                    .FirstOrDefaultAsync();

                if (emisor != null)
                {
                    Empresa empresa = await _context.Empresas
    .Where(e => e.Identificacion == emisor.Ruc)
    .Include(e => e.Clientes)
        .ThenInclude(c => c.IdPersonaNavigation)
    .FirstOrDefaultAsync();

                    clientesQuery = empresa?.Clientes ?? Enumerable.Empty<Cliente>();
                }
            }

            // Ya no es necesario ejecutar la consulta aquí, ya que clientesQuery ya es una colección en memoria
            IEnumerable<Cliente> clientes = clientesQuery;

            // Pasar tanto los clientes como la lista de emisores a la vista
            return View((clientes, emisores, selectedEmisorId: IdEmisor));
        }

        // GET: ClientEmisor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: ClientEmisor/Create
        public IActionResult Create()
        {
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            ViewData["IdEmmisor"] = new SelectList(_context.Emisors.Where(e=>e.IdEmpresa==int.Parse(idEmpresa)), "IdEmisor", "RazonSocial");
            var tiposIdentificacion = _context.TipoIdentificacions.ToList();

            if (tiposIdentificacion == null || !tiposIdentificacion.Any())
            {
                // Si no hay datos, crear una lista vacía para evitar errores
                ViewData["IdTipoIdentificacion"] = new SelectList(new List<TipoIdentificacion>());
                ViewBag.TipoIdentificacionError = "No se encontraron tipos de identificación.";
            }
            else
            {
                ViewData["IdTipoIdentificacion"] = new SelectList(tiposIdentificacion, "IdTipoIdemtificacion", "Descripcion");
            }
            return View();
        }

        // POST: ClientEmisor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteEmisorViewModel cliente)
        {
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            if (ModelState.IsValid)
            {
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e=>e.IdEmisor==cliente.idEmisor).Include(e=>e.IdEmpresaNavigation).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                Persona persona = new Persona();
                persona.Nombre = cliente.nombre;
                persona.Identificacion=cliente.identificacion;
                persona.Direccion=cliente.direccion;
                persona.Telefono=cliente.telefono;
                persona.Email=cliente.email;
                persona.IdEmpresa = empresa.IdEmpresa;
                persona.FechaCreacion = new DateTime();
                persona.UsuarioCreacion = int.Parse(idUsuario);
                persona.IdTipoIdentificacion=cliente.IdTipoIdentificacion;
                _context.Add(persona);
                _context.SaveChanges();
                Cliente clie = new Cliente();
                clie.IdPersona=persona.IdPersona;
                clie.IdEmpresa=persona.IdEmpresa;
                clie.FechaCreacion=new DateTime();
                clie.UsuarioCreacion=int.Parse(idUsuario);
                _context.Add(clie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmmisor"] = new SelectList(_context.Emisors.Where(e => e.IdEmpresa == int.Parse(idEmpresa)), "IdEmisor", "RazonSocial");

            return View(cliente);
        }

        // GET: ClientEmisor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cliente.IdEmpresa);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Direccion", cliente.IdPersona);
            return View(cliente);
        }

        // POST: ClientEmisor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,IdPersona,Estado,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.IdCliente))
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
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", cliente.IdEmpresa);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Direccion", cliente.IdPersona);
            return View(cliente);
        }

        // GET: ClientEmisor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdEmpresaNavigation)
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: ClientEmisor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.IdCliente == id);
        }
    }
}
