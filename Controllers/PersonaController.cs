﻿using System;
using System.Collections.Generic;  using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using System.Diagnostics;

namespace ContaFacil.Controllers
{
    public class PersonaController : NotificacionClass
    {
        private readonly ContableContext _context;

        public PersonaController(ContableContext context)
        {
            _context = context;
        }
        public IActionResult PrincipalCliente()
        {
            // Aquí puedes agregar cualquier lógica adicional que necesites antes de devolver la vista
            // Por ejemplo, podrías cargar algunos datos desde la base de datos y pasarlos a la vista

            return View(); // Esto devolverá la vista PrincipalProducto.cshtml
        }
        // GET: Persona
        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(empresa => empresa.Identificacion == emisor.Ruc).FirstOrDefault();
            var contableContext = _context.Personas.Include(p => p.IdEmpresaNavigation).Where(p=>p.IdEmpresa==empresa.IdEmpresa);
            return View(await contableContext.ToListAsync());
        }

        // GET: Persona/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas
                .Include(p => p.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // GET: Persona/Create
        // GET: Persona/Create
        public IActionResult Create()
        {
            try
            {
                // Asegúrate de que _context no sea nulo y esté correctamente inicializado
                if (_context == null)
                {
                    throw new InvalidOperationException("Database context is not initialized.");
                }

                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "NombreEmpresa");
                ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION EN LA FUENTE").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                // Suponiendo que tienes una tabla TipoIdentificacion con IdTipoIdentificacion y Descripcion
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
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Create action: {ex.Message}");
                // You might want to log this to a file or database in a real application

                // Return an error view
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        // POST: Persona/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Persona persona)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                Usuario usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(u => u.IdPersonaNavigation).FirstOrDefault();
                Emisor emisor = new Emisor();
                emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
                Empresa empresa = new Empresa();
                empresa = _context.Empresas.Where(empresa => empresa.Identificacion == emisor.Ruc).FirstOrDefault();
                persona.FechaCreacion = new DateTime();
                persona.UsuarioCreacion = int.Parse(idUsuario);
                persona.IdEmpresa =empresa.IdEmpresa;
                _context.Add(persona);
                Cliente cliente = new Cliente();
                cliente.IdPersonaNavigation = persona;
                cliente.Estado = true;
                cliente.FechaCreacion = persona.FechaCreacion;
                cliente.UsuarioCreacion = 1;
                cliente.IdEmpresa= empresa.IdEmpresa;
                
                _context.Add(cliente);

                await _context.SaveChangesAsync();
                Notificacion("Registro guardardo con exito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION EN LA FUENTE").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", persona.IdEmpresa);
                Notificacion("Error al guardar el Registro " + ex.Message, NotificacionTipo.Error);
                return View(persona);
            }
        }

        // GET: Persona/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
            {
                IdImpuesto = i.Porcentaje,
                NombrePorcentaje = i.Nombre
            }), "IdImpuesto", "NombrePorcentaje");
            ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION EN LA FUENTE").Select(i => new
            {
                IdImpuesto = i.Porcentaje,
                NombrePorcentaje = i.Nombre
            }), "IdImpuesto", "NombrePorcentaje");
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", persona.IdEmpresa);
            return View(persona);
        }

        // POST: Persona/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Persona persona)
        {
            if (id != persona.IdPersona)
            {
                return NotFound();
            }
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    persona.FechaModificacion = new DateTime();
                    persona.UsuarioModificacion = int.Parse(idUsuario);
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException ex)
                {
                    
                        Notificacion("Error al actualizar el Registro" + ex.Message, NotificacionTipo.Error);
                ViewData["IdImpuesto"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION IVA").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdRetencionF"] = new SelectList(_context.Impuestos.Where(i => i.Tipo == "RETENCION EN LA FUENTE").Select(i => new
                {
                    IdImpuesto = i.Porcentaje,
                    NombrePorcentaje = i.Nombre
                }), "IdImpuesto", "NombrePorcentaje");
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", persona.IdEmpresa);
                return View(persona);
            }
               
            
          
        }

        // GET: Persona/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas
                .Include(p => p.IdEmpresaNavigation)
                .FirstOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // POST: Persona/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Personas == null)
            {
                return Problem("Entity set 'ContableContext.Personas'  is null.");
            }
            var persona = await _context.Personas.FindAsync(id);
            if (persona != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                persona.UsuarioModificacion = int.Parse(idUsuario);
                persona.FechaModificacion = new DateTime();
                persona.Estado = false;
                _context.Personas.Update(persona);

                Cliente cliente = _context.Clientes.Where(c => c.IdPersona == id).FirstOrDefault();
                if (cliente != null)
                {
                    cliente.UsuarioModificacion = int.Parse(idUsuario);
                    cliente.FechaModificacion = new DateTime();
                    cliente.Estado = false;
                    _context.Clientes.Update(cliente);
                }                
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
          return (_context.Personas?.Any(e => e.IdPersona == id)).GetValueOrDefault();
        }
    }
}
