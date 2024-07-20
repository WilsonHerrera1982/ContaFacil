using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using Humanizer.Localisation.TimeToClockNotation;

namespace ContaFacil.Controllers.Contador
{
    public class EmisorController : NotificacionClass
    {
        private readonly ContableContext _context;

        public EmisorController(ContableContext context)
        {
            _context = context;
        }

        // GET: Emisor
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Emisors.Include(e => e.IdEmpresaNavigation).Include(e => e.IdUsuarioNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Emisor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Emisors == null)
            {
                return NotFound();
            }

            var emisor = await _context.Emisors
                .Include(e => e.IdEmpresaNavigation)
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdEmisor == id);
            if (emisor == null)
            {
                return NotFound();
            }

            return View(emisor);
        }

        // GET: Emisor/Create
        public IActionResult Create()
        {
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Emisor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Emisor emisor)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                emisor.FechaCreacion = new DateTime();
                emisor.UsuarioCreacion = int.Parse(idUsuario);
                Empresa empresa = new Empresa();
                empresa.FechaCreacion = new DateTime();
                empresa.UsuarioCreacion = int.Parse(idUsuario);
                empresa.Nombre = emisor.NombreComercial;
                empresa.Identificacion = emisor.Ruc;
                empresa.Direccion = emisor.Direccion;
                empresa.Telefono = emisor.Telefono;
                _context.Add(empresa);
                await _context.SaveChangesAsync();
                Persona persona = new Persona();
                persona.FechaCreacion = new DateTime();
                persona.UsuarioCreacion = int.Parse(idUsuario);
                persona.Email = emisor.CorreoElectronico;
                persona.Telefono = emisor.Telefono;
                persona.IdEmpresa = int.Parse(idEmpresa);
                persona.Identificacion = emisor.Ruc;
                persona.Direccion = emisor.Direccion;
                persona.Nombre = emisor.NombreComercial;
                _context.Add(persona);
                _context.SaveChanges();
                Cliente cliente = new Cliente();
                cliente.FechaCreacion = new DateTime();
                cliente.UsuarioCreacion = int.Parse(idUsuario);
                cliente.IdPersona = persona.IdPersona;
                cliente.IdEmpresa = int.Parse(idEmpresa);
                _context.Add(cliente);
                _context.SaveChanges();
                Usuario usuario = new Usuario();
                usuario.FechaCreacion = new DateTime();
                usuario.UsuarioCreacion = int.Parse(idUsuario);
                usuario.Nombre = emisor.NombreUsuario;
                usuario.Clave = emisor.Clave;
                usuario.IdPersona = persona.IdPersona;
                usuario.IdEmpresa = int.Parse(idEmpresa);
                _context.Add(usuario);
                _context.SaveChanges();
                Perfil perfil = new Perfil();
                perfil = _context.Perfils.Where(p => p.Descripcion == "Vendedor").FirstOrDefault();
                
                UsuarioPerfil usuarioPerfil = new UsuarioPerfil();
                usuarioPerfil.IdUsuario = usuario.IdUsuario;
                usuarioPerfil.IdPerfil = perfil.IdPerfil;
                usuarioPerfil.FechaCreacion = new DateTime();
                usuarioPerfil.UsuarioCreacion = int.Parse(idUsuario);
                _context.Add(usuarioPerfil);
                await _context.SaveChangesAsync();
                emisor.IdEmpresa= int.Parse(idEmpresa);
                emisor.IdUsuario= int.Parse(idUsuario);
                _context.Add(emisor);

                _context.SaveChanges();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", emisor.IdEmpresa);
                ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", emisor.IdUsuario);
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(emisor);
            }
        }

        // GET: Emisor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Emisors == null)
            {
                return NotFound();
            }

            var emisor = await _context.Emisors.FindAsync(id);
            if (emisor == null)
            {
                return NotFound();
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", emisor.IdEmpresa);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", emisor.IdUsuario);
            return View(emisor);
        }

        // POST: Emisor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmisor,IdUsuario,IdEmpresa,RazonSocial,NombreComercial,Ruc,NombreUsuario,Telefono,CorreoElectronico,Establecimiento,PuntoEmision,Secuencial,Direccion,CertificadoDigital,Clave,ObligadoContabilidad,TipoAmbiente,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Emisor emisor)
        {
            if (id != emisor.IdEmisor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    emisor.UsuarioModificacion = int.Parse(idUsuario);
                    emisor.FechaCreacion = new DateTime();
                    _context.Update(emisor);
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EmisorExists(emisor.IdEmisor))
                    {
                        return NotFound();
                    }
                    else
                    {
                        Notificacion("Error al actualizar el Registro" + ex.Message, NotificacionTipo.Error);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", emisor.IdEmpresa);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", emisor.IdUsuario);
            return View(emisor);
        }

        // GET: Emisor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Emisors == null)
            {
                return NotFound();
            }

            var emisor = await _context.Emisors
                .Include(e => e.IdEmpresaNavigation)
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdEmisor == id);
            if (emisor == null)
            {
                return NotFound();
            }

            return View(emisor);
        }

        // POST: Emisor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Emisors == null)
            {
                return Problem("Entity set 'ContableContext.Emisors'  is null.");
            }
            var emisor = await _context.Emisors.FindAsync(id);
            if (emisor != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                emisor.UsuarioModificacion = int.Parse(idUsuario);
                emisor.FechaModificacion = new DateTime();
                emisor.EstadoBoolean = false;
                _context.Emisors.Update(emisor);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool EmisorExists(int id)
        {
          return (_context.Emisors?.Any(e => e.IdEmisor == id)).GetValueOrDefault();
        }
    }
}
