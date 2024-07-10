﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using ContaFacil.Models.ViewModel;

namespace ContaFacil.Controllers
{
    public class ProductoController : NotificacionClass
    {
        private readonly ContableContext _context;

        public ProductoController(ContableContext context)
        {
            _context = context;
        }

        // GET: Producto
        public async Task<IActionResult> Index()
        {
            var contableContext = _context.Productos.Include(p => p.IdCategoriaProductoNavigation).Include(p => p.IdEmpresaNavigation).Include(p => p.IdUnidadMedidaNavigation);
            return View(await contableContext.ToListAsync());
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaProductoNavigation)
                .Include(p => p.IdEmpresaNavigation)
                .Include(p => p.IdUnidadMedidaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "IdCategoriaProducto");
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "IdUnidadMedida");
            return View();
        }

        // POST: Producto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel producto)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Producto product = new Producto();
                product.Codigo = producto.Codigo;
                product.Nombre = producto.Nombre;
                product.Descripcion = producto.Descripcion;
                product.PrecioUnitario = producto.PrecioUnitario;
                product.IdCategoriaProducto = producto.IdCategoriaProducto;
                product.IdUnidadMedida = producto.IdUnidadMedida;
                product.Stock = producto.Stock;
                product.EstadoBoolean = true;
                product.FechaCreacion = new DateTime();
                product.UsuarioCreacion = int.Parse(idUsuario);
                product.IdEmpresa = int.Parse(idEmpresa);
                _context.Add(producto);

                await _context.SaveChangesAsync();
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "IdCategoriaProducto", producto.IdCategoriaProducto);
                ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", producto.IdEmpresa);
                ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "IdUnidadMedida", producto.IdUnidadMedida);
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(producto);
            }
            
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                Notificacion("Elemento no encontrado", NotificacionTipo.Warning);
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                Notificacion("Elemento no encontrado", NotificacionTipo.Warning);
                return NotFound();
            }
            ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "IdCategoriaProducto", producto.IdCategoriaProducto);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", producto.IdEmpresa);
            ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "IdUnidadMedida", producto.IdUnidadMedida);
            return View(producto);
        }

        // POST: Producto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,Codigo,Nombre,Descripcion,PrecioUnitario,IdCategoriaProducto,IdUnidadMedida,Stock,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,IdEmpresa")] Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    producto.FechaModificacion = new DateTime();
                    producto.UsuarioModificacion = int.Parse(idUsuario);
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ProductoExists(producto.IdProducto))
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
            ViewData["IdCategoriaProducto"] = new SelectList(_context.CategoriaProductos, "IdCategoriaProducto", "IdCategoriaProducto", producto.IdCategoriaProducto);
            ViewData["IdEmpresa"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", producto.IdEmpresa);
            ViewData["IdUnidadMedida"] = new SelectList(_context.UnidadMedida, "IdUnidadMedida", "IdUnidadMedida", producto.IdUnidadMedida);
            return View(producto);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaProductoNavigation)
                .Include(p => p.IdEmpresaNavigation)
                .Include(p => p.IdUnidadMedidaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'ContableContext.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.Productos?.Any(e => e.IdProducto == id)).GetValueOrDefault();
        }
    }
}