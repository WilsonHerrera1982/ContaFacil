using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Models.ViewModel;

namespace ContaFacil.Controllers
{
    public class ConstatacionFisicaController : Controller
    {
        private readonly ContableContext _context;

        public ConstatacionFisicaController(ContableContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            int registrosPorPagina = 10;
            var query = _context.ConstatacionFisicas
                .Include(c => c.Producto)
                .Include(c => c.Empresa)
                .AsQueryable();

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                query = query.Where(c => c.FechaCreacion >= fechaInicio && c.FechaCreacion <= fechaFin);
            }

            var totalRegistros = await query.CountAsync();
            var constataciones = await query
                .OrderByDescending(c => c.FechaCreacion)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View(constataciones);
        }

        // GET: Formulario para nueva constatación
        public async Task<IActionResult> Nueva(string searchTerm = "", int pagina = 1)
        {
            int registrosPorPagina = 10;
            var query = _context.Productos
                .Include(p => p.IdEmpresaNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Nombre.Contains(searchTerm) || p.Codigo.Contains(searchTerm));
            }

            var totalRegistros = await query.CountAsync();
            var productos = await query
                .OrderBy(p => p.Nombre)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new ConstatacionFisicaViewModel
                {
                    ProductoId = p.IdProducto,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    StockActual = p.Stock ?? 0,
                    CantidadFisica = null,
                    Descripcion = ""
                })
                .ToListAsync();
            foreach (var producto in productos)
            {
                var inventarioTask = Task.Run(() =>
                {
                    using (var newContext = new ContableContext())  // Crea un nuevo contexto
                    {
                        return newContext.Inventarios
                            .OrderByDescending(i => i.FechaCreacion)
                            .FirstOrDefault(i => i.IdProducto == producto.ProductoId);
                    }
                });

                var inventario = await inventarioTask;
               
                producto.StockActual = inventario.Stock??0m;
            }
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);
            ViewBag.SearchTerm = searchTerm;

            return View(productos);
        }

        // POST: Guardar constatación
        [HttpPost]
        public async Task<IActionResult> GuardarConstatacion([FromBody] List<ConstatacionFisicaViewModel> constataciones)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Usuario usuario = new Usuario();
                usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();

                var nuevasConstataciones = constataciones.Select(c => new ConstatacionFisica
                {
                    ProductoId = c.ProductoId,
                    ProductoStockActual = (int)c.StockActual,
                    CantidadFisica = c.CantidadFisica,
                    ConstatacionDescripcion = c.Descripcion,
                    FechaCreacion = DateTime.Now,
                    Estado = 1,
                    EmpresaId = usuario.IdEmpresa??0, // Ajustar según tu lógica de empresa
                    UsuarioCreacion = usuario.IdUsuario // Ajustar según tu lógica de usuario
                }).ToList();
                foreach (var producto in nuevasConstataciones)
                {
                    var inventarioTask = Task.Run(() =>
                    {
                        using (var newContext = new ContableContext())  // Crea un nuevo contexto
                        {
                            return newContext.Inventarios
                                .OrderByDescending(i => i.FechaCreacion)
                                .FirstOrDefault(i => i.IdProducto == producto.ProductoId);
                        }
                    });

                    var inventario = await inventarioTask;
                    
                    producto.ProductoStockActual = inventario.Stock??0;
                }
                await _context.ConstatacionFisicas.AddRangeAsync(nuevasConstataciones);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Constatación guardada exitosamente" });
            }
            catch (Exception ex)
            {
                     return Json(new { success = false, message = "Error al guardar la constatación" });
            }
        }

        // GET: ConstatacionFisica/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constatacionFisica = await _context.ConstatacionFisicas
                .Include(c => c.Empresa)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.ConstatacionId == id);
            if (constatacionFisica == null)
            {
                return NotFound();
            }

            return View(constatacionFisica);
        }

        // GET: ConstatacionFisica/Create
        public IActionResult Create()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProducto", "IdProducto");
            return View();
        }

        // POST: ConstatacionFisica/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConstatacionId,ConstatacionDescripcion,EmpresaId,ProductoId,ProductoStockActual,CantidadFisica,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,Estado")] ConstatacionFisica constatacionFisica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(constatacionFisica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", constatacionFisica.EmpresaId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", constatacionFisica.ProductoId);
            return View(constatacionFisica);
        }

        // GET: ConstatacionFisica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constatacionFisica = await _context.ConstatacionFisicas.FindAsync(id);
            if (constatacionFisica == null)
            {
                return NotFound();
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", constatacionFisica.EmpresaId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", constatacionFisica.ProductoId);
            return View(constatacionFisica);
        }

        // POST: ConstatacionFisica/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConstatacionId,ConstatacionDescripcion,EmpresaId,ProductoId,ProductoStockActual,CantidadFisica,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion,Estado")] ConstatacionFisica constatacionFisica)
        {
            if (id != constatacionFisica.ConstatacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(constatacionFisica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConstatacionFisicaExists(constatacionFisica.ConstatacionId))
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
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "IdEmpresa", "IdEmpresa", constatacionFisica.EmpresaId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", constatacionFisica.ProductoId);
            return View(constatacionFisica);
        }

        // GET: ConstatacionFisica/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constatacionFisica = await _context.ConstatacionFisicas
                .Include(c => c.Empresa)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.ConstatacionId == id);
            if (constatacionFisica == null)
            {
                return NotFound();
            }

            return View(constatacionFisica);
        }

        // POST: ConstatacionFisica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var constatacionFisica = await _context.ConstatacionFisicas.FindAsync(id);
            if (constatacionFisica != null)
            {
                _context.ConstatacionFisicas.Remove(constatacionFisica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConstatacionFisicaExists(int id)
        {
            return _context.ConstatacionFisicas.Any(e => e.ConstatacionId == id);
        }
    }
}
