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
    public class OpcionClientesController : NotificacionClass
    {
        private readonly ContableContext _context;

        public OpcionClientesController(ContableContext context)
        {
            _context = context;
        }

        // GET: OpcionClientes
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.OpcionClientes.Include(o => o.Empresa);
            return View(await contableContext.ToListAsync());
        }

        // GET: OpcionClientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opcionCliente = await _context.OpcionClientes
                .Include(o => o.Empresa)
                .FirstOrDefaultAsync(m => m.OpcionId == id);
            if (opcionCliente == null)
            {
                return NotFound();
            }

            return View(opcionCliente);
        }

        // GET: OpcionClientes/Create
        public IActionResult Create()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre");
            return View();
        }

        // POST: OpcionClientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OpcionCliente opcionCliente)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = new Usuario();
                usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
                opcionCliente.UsuarioCreacion = usuario.IdUsuario;
                opcionCliente.FechaResgitro = new DateTime();
                _context.Add(opcionCliente);
                await _context.SaveChangesAsync();
                Notificacion("Registro almacenado con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", opcionCliente.EmpresaId);
                Notificacion("Error al guardar el registro " +e,NotificacionTipo.Error);
                return View(opcionCliente);
            }
        }

        // GET: OpcionClientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opcionCliente = await _context.OpcionClientes.FindAsync(id);
            if (opcionCliente == null)
            {
                return NotFound();
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "Nombre", opcionCliente.EmpresaId);
            return View(opcionCliente);
        }

        // POST: OpcionClientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OpcionCliente opcionCliente)
        {
            if (id != opcionCliente.OpcionId)
            {
                return NotFound();
            }

            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = new Usuario();
                usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
                opcionCliente.UsuarioModificacion = usuario.IdUsuario;
                opcionCliente.FechaModificacion = new DateTime();
                _context.Update(opcionCliente);
                await _context.SaveChangesAsync();
                Notificacion("Registro actualizado con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException e)
            {
                ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", opcionCliente.EmpresaId);
                Notificacion("Error al actualizar el registro" + e,NotificacionTipo.Error);
                return View(opcionCliente);
            }
        }
           
        

        // GET: OpcionClientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opcionCliente = await _context.OpcionClientes
                .Include(o => o.Empresa)
                .FirstOrDefaultAsync(m => m.OpcionId == id);
            if (opcionCliente == null)
            {
                return NotFound();
            }

            return View(opcionCliente);
        }

        // POST: OpcionClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opcionCliente = await _context.OpcionClientes.FindAsync(id);
            if (opcionCliente != null)
            {
                _context.OpcionClientes.Remove(opcionCliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpcionClienteExists(int id)
        {
            return _context.OpcionClientes.Any(e => e.OpcionId == id);
        }
    }
}
