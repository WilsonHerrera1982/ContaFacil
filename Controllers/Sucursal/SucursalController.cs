using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;

namespace ContaFacil.Controllers.Sucursal
{
    public class SucursalController : NotificacionClass
    {
        private readonly ContableContext _context;

        public SucursalController(ContableContext context)
        {
            _context = context;
        }

        // GET: Sucursal
        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(empresa => empresa.Identificacion == emisor.Ruc).FirstOrDefault();
            var contableContext = _context.Sucursals.Where(s=>s.IdEmisor==emisor.IdEmisor).Include(s => s.IdEmisorNavigation).Include(s => s.IdUsuarioNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Sucursal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursals
                .Include(s => s.IdEmisorNavigation)
                .Include(s => s.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdSucursal == id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        // GET: Sucursal/Create
        public IActionResult Create()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(empresa => empresa.Identificacion == emisor.Ruc).FirstOrDefault();
            ViewData["IdEmisor"] = new SelectList(_context.Emisors.Where(e=>e.Ruc==empresa.Identificacion), "IdEmisor", "RazonSocial");
            return View();
        }

        // POST: Sucursal/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Sucursal sucursal)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(empresa => empresa.Identificacion == emisor.Ruc).FirstOrDefault();
                sucursal.UsuarioCreacion = int.Parse(idUsuario);
                sucursal.FechaCreacion = new DateTime();
                sucursal.IdUsuario = int.Parse(idUsuario);
                sucursal.EstadoBoolean  = true;
                sucursal.IdEmisor =emisor.IdEmisor;
                _context.Add(sucursal);
                await _context.SaveChangesAsync();
                Empresa emp = new Empresa();
                emp = empresa;
                emp.IdEmpresa = 0;
                emp.Estado = true;
                emp.UsuarioCreacion=int.Parse(idUsuario);
                emp.FechaCreacion=new DateTime();   
                _context.Empresas.Add(emp);
                await _context.SaveChangesAsync();
                Emisor emisor1= new Emisor();
                emisor1 = emisor;
                emisor1.IdEmisor = 0;
                emisor1.PuntoEmision=sucursal.PuntoEmision;
                emisor1.Secuencial=sucursal.Secuencial;
                emisor1.Direccion = sucursal.DireccionSucursal;
                emisor1.IdEmpresa=emp.IdEmpresa;
                emisor1.UsuarioCreacion=sucursal.UsuarioCreacion;
                emisor1.EstadoBoolean=true;
                emisor1.FechaCreacion=sucursal.FechaCreacion;
                _context.Emisors.Add(emisor1);
                await _context.SaveChangesAsync();
                Usuario usu= new Usuario();
                usu.Nombre = sucursal.Usuario;
                usu.Clave = sucursal.Clave;
                usu.UsuarioCreacion = int.Parse(idUsuario);
                usu.FechaCreacion=new DateTime();
                usu.IdPersona=usuario.IdPersona;
                _context.Add(usu);
                await _context.SaveChangesAsync();
                Perfil perfil= new Perfil();
                perfil = _context.Perfils.Where(p=>p.Descripcion== "Vendedor").FirstOrDefault();
                UsuarioPerfil usuarioPerfil = new UsuarioPerfil();
                usuarioPerfil.IdUsuario=usu.IdUsuario;
                usuarioPerfil.IdPerfil=perfil.IdPerfil;
                usuarioPerfil.UsuarioCreacion = int.Parse(idUsuario);
                usuarioPerfil.FechaCreacion=new DateTime();
                usuarioPerfil.Estado = true;
                _context.Add(usuarioPerfil);
                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con exito",NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdEmisor"] = new SelectList(_context.Emisors, "IdEmisor", "IdEmisor", sucursal.IdEmisor);
                ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", sucursal.IdUsuario);
                Notificacion("Error al guardar el registro",NotificacionTipo.Error);
                return View(sucursal);
            }
        }

        // GET: Sucursal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursals.FindAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }
            ViewData["IdEmisor"] = new SelectList(_context.Emisors, "IdEmisor", "IdEmisor", sucursal.IdEmisor);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", sucursal.IdUsuario);
            return View(sucursal);
        }

        // POST: Sucursal/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Models.Sucursal sucursal)
        {
            if (id != sucursal.IdSucursal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.IdSucursal))
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
            ViewData["IdEmisor"] = new SelectList(_context.Emisors, "IdEmisor", "IdEmisor", sucursal.IdEmisor);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", sucursal.IdUsuario);
            return View(sucursal);
        }

        // GET: Sucursal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursals
                .Include(s => s.IdEmisorNavigation)
                .Include(s => s.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdSucursal == id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        // POST: Sucursal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucursal = await _context.Sucursals.FindAsync(id);
            if (sucursal != null)
            {
                _context.Sucursals.Remove(sucursal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SucursalExists(int id)
        {
            return _context.Sucursals.Any(e => e.IdSucursal == id);
        }
    }
}
