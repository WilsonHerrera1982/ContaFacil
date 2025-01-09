using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;

namespace ContaFacil.Controllers
{
    public class OpcionesClientesController : NotificacionClass
    {
        private readonly ContableContext _context;
        string idUsuario = "";
        string idEmpresa = "";
        public OpcionesClientesController(ContableContext context)
        {
            _context = context;
        }

        // GET: OpcionesClientes
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.OpcionesClientes.Include(o => o.Empresa).Include(o => o.Opcion);
            return View(await contableContext.ToListAsync());
        }

        // GET: OpcionesClientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opcionesCliente = await _context.OpcionesClientes
                .Include(o => o.Empresa)
                .Include(o => o.Opcion)
                .FirstOrDefaultAsync(m => m.OpcId == id);
            if (opcionesCliente == null)
            {
                return NotFound();
            }

            return View(opcionesCliente);
        }

        // GET: OpcionesClientes/Create
        public IActionResult Create()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre");
            ViewData["OpcionId"] = new SelectList(_context.OpcionClientes, "OpcionId", "OpcionNombre");
            return View();
        }

        // POST: OpcionesClientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OpcionesCliente opcionesCliente)
        {
            try
            {
                idUsuario = HttpContext.Session.GetString("_idUsuario");
                idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = new Usuario();
                usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
                opcionesCliente.UsuarioCreacion = usuario.UsuarioCreacion;
                opcionesCliente.FechaCreacion = DateTime.Now;
                _context.Add(opcionesCliente);
                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con exito",NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre", opcionesCliente.EmpresaId);
                ViewData["OpcionId"] = new SelectList(_context.OpcionClientes, "OpcionId", "OpcionNombre", opcionesCliente.OpcionId);
                Notificacion("Error al guardar el archivo" + e, NotificacionTipo.Error);
                return View(opcionesCliente);
            }
          
        }

        // GET: OpcionesClientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opcionesCliente = await _context.OpcionesClientes.FindAsync(id);
            if (opcionesCliente == null)
            {
                return NotFound();
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre", opcionesCliente.EmpresaId);
            ViewData["OpcionId"] = new SelectList(_context.OpcionClientes, "OpcionId", "OpcionNombre", opcionesCliente.OpcionId);
            return View(opcionesCliente);
        }

        // POST: OpcionesClientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OpcionesCliente opcionesCliente)
        {
            idUsuario = HttpContext.Session.GetString("_idUsuario");
            idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
           
            if (id != opcionesCliente.OpcId)
            {
                return NotFound();
            }

           
                try
                {
                opcionesCliente.FechaCreacion = DateTime.Now;
                opcionesCliente.UsuarioCreacion = usuario.UsuarioCreacion;
                    _context.Update(opcionesCliente);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
            {
                ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", opcionesCliente.EmpresaId);
                ViewData["OpcionId"] = new SelectList(_context.OpcionClientes, "OpcionId", "OpcionNombre", opcionesCliente.OpcionId);
                return View(opcionesCliente);
            }
                
           
        }

        // GET: OpcionesClientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opcionesCliente = await _context.OpcionesClientes
                .Include(o => o.Empresa)
                .Include(o => o.Opcion)
                .FirstOrDefaultAsync(m => m.OpcId == id);
            if (opcionesCliente == null)
            {
                return NotFound();
            }

            return View(opcionesCliente);
        }

        // POST: OpcionesClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opcionesCliente = await _context.OpcionesClientes.FindAsync(id);
            if (opcionesCliente != null)
            {
                _context.OpcionesClientes.Remove(opcionesCliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpcionesClienteExists(int id)
        {
            return _context.OpcionesClientes.Any(e => e.OpcId == id);
        }
    }
}
