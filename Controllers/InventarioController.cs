using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContaFacil.Models;
using ContaFacil.Logica;
using ContaFacil.Models.ViewModel;
using SQLitePCL;

namespace ContaFacil.Controllers
{
    public class InventarioController : NotificacionClass
    {
        private readonly ContableContext _context;

        public InventarioController(ContableContext context)
        {
            _context = context;
        }

        // GET: Inventario
        public async Task<IActionResult> Index(int? idSucursal, DateTime? startDate, DateTime? endDate)
        {
            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            string idEmpresa = HttpContext.Session.GetString("_empresa");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            UsuarioSucursal usuarioSucursal= new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(s => s.IdUsuario == usuario.IdUsuario).FirstOrDefault();

            var query = _context.Inventarios
             .Include(i => i.IdProductoNavigation)
             /*.Include(i => i.SucursalInventarios)
             .Where(i => i.SucursalInventarios.Any(s => s.IdSucursal == usuarioSucursal.IdSucursal))*/
             .AsQueryable();

            if (idSucursal.HasValue)
            {
                query = query.Where(i => i.SucursalInventarios.Any(si => si.IdSucursal == idSucursal.Value));
            }

            if (startDate.HasValue)
            {
                query = query.Where(i => i.FechaMovimiento >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.FechaMovimiento <= endDate.Value);
            }

            var inventarios = await query.OrderByDescending(i => i.FechaMovimiento).ToListAsync();

            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            // Asegúrese de que esto esté configurado correctamente
            ViewBag.IdSucursal = new SelectList(_context.Sucursals, "IdSucursal", "NombreSucursal", idSucursal);
            ViewBag.SelectedSucursal = idSucursal;

            return View(inventarios);
        }

        // GET: Inventario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // GET: Inventario/Create
        public IActionResult Create()
        {

            string idUsuario = HttpContext.Session.GetString("_idUsuario");
            Usuario usuario = new Usuario();
            usuario = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idUsuario)).Include(p => p.IdPersonaNavigation).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == usuario.IdPersonaNavigation.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
            Despacho despacho = new Despacho();
            despacho = _context.Despachos.Where(e => e.IdSucursal==usuarioSucursal.IdSucursal).FirstOrDefault();
            ViewData["IdProducto"] = new SelectList(_context.Productos.Where(e=>e.IdEmpresa==empresa.IdEmpresa), "IdProducto", "Nombre");
            ViewData["IdSucursal"] = new SelectList(_context.Sucursals.Where(s => s.IdEmisor == emisor.IdEmisor & !s.NombreSucursal.Equals("Sucursal Principal")), "IdSucursal", "NombreSucursal");
            return View();
        }

        [HttpGet]
        public IActionResult ObtenerNumeroDespacho(string tipoMovimiento)
        {
            var ultimoMovimiento = _context.Inventarios
                .Where(i => i.TipoMovimiento == tipoMovimiento)
                .OrderByDescending(i => i.FechaCreacion)
                .FirstOrDefault();

            string nuevoNumeroDespacho;

            if (ultimoMovimiento != null && !string.IsNullOrEmpty(ultimoMovimiento.NumeroDespacho))
            {
                int ultimoNumero = int.Parse(ultimoMovimiento.NumeroDespacho.Split('-')[1]);
                nuevoNumeroDespacho = $"{tipoMovimiento}-{(ultimoNumero + 1).ToString("D6")}";
            }
            else
            {
                nuevoNumeroDespacho = $"{tipoMovimiento}-000001";
            }

            return Content(nuevoNumeroDespacho);
        }

        // POST: Inventario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventarioViewModel inventario)
        {
            try
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                string idEmpresa = HttpContext.Session.GetString("_empresa");
                Despacho despacho = new Despacho();
                UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
                usuarioSucursal = _context.UsuarioSucursals.Where(s => s.IdUsuario == int.Parse(idUsuario)).FirstOrDefault();
                SucursalInventario  sucursalInventario = new SucursalInventario();
                Producto producto = new Producto();
                producto=_context.Productos.Where(p => p.IdProducto==inventario.idProducto).FirstOrDefault();
                Inventario ultimoMovimiento=new Inventario();
                if (inventario.tipoMovimiento.Equals("T"))
                {
                    ultimoMovimiento = _context.Inventarios
                   .Where(i => i.TipoMovimiento == "E")
                   .OrderByDescending(i => i.FechaCreacion)
                   .FirstOrDefault();
                }
                if (inventario.tipoMovimiento.Equals("S"))
                {
                    ultimoMovimiento = _context.Inventarios
                   .Where(i => i.TipoMovimiento == "S")
                   .OrderByDescending(i => i.FechaCreacion)
                   .FirstOrDefault();
                    if (ultimoMovimiento == null)
                    {
                        ultimoMovimiento = _context.Inventarios
                  .Where(i => i.TipoMovimiento == "E")
                  .OrderByDescending(i => i.FechaCreacion)
                  .FirstOrDefault();

                    }
                }
                else
                {
                    ultimoMovimiento = _context.Inventarios
                  .Where(i => i.TipoMovimiento == inventario.tipoMovimiento)
                  .OrderByDescending(i => i.FechaCreacion)
                  .FirstOrDefault();
                }
                Inventario inv = new Inventario();
                if (inventario.tipoMovimiento.Equals("E"))
                {
                    inv.IdProducto= inventario.idProducto;
                    inv.TipoMovimiento = inventario.tipoMovimiento;
                    inv.NumeroDespacho= inventario.numeroDespacho;
                    inv.EstadoBoolean = true;
                    inv.UsuarioCreacion = int.Parse(idUsuario);
                    inv.FechaCreacion = new DateTime();
                    inv.Cantidad= inventario.cantidad;
                    inv.Descripcion = "ENTRADA";
                    if (ultimoMovimiento != null)
                    {
                        inv.Stock = ultimoMovimiento.Stock + inventario.cantidad;
                    }
                    else
                    {
                        inv.Stock = 0 + inventario.cantidad;
                    }
                    _context.Add(inv);
                    await _context.SaveChangesAsync();
                    producto.Stock=producto.Stock+inventario.cantidad;
                    sucursalInventario.EstadoBoolean = true;
                    sucursalInventario.IdInventario = inv.IdInventario;
                    sucursalInventario.IdSucursal = usuarioSucursal.IdSucursal;
                    sucursalInventario.FechaCreacion= new DateTime();
                    sucursalInventario.UsuarioCreacion= int.Parse(idUsuario);
                    _context.Add(sucursalInventario);
                    await _context.SaveChangesAsync();                   
                }
                else if(inventario.tipoMovimiento.Equals("S") && producto.Stock>=0 && producto.Stock>inventario.cantidad)
                {
                    inv.IdProducto = inventario.idProducto;
                    inv.TipoMovimiento = inventario.tipoMovimiento;
                    inv.NumeroDespacho = inventario.numeroDespacho;
                    inv.EstadoBoolean = true;
                    inv.UsuarioCreacion = int.Parse(idUsuario);
                    inv.FechaCreacion = new DateTime();
                    inv.Stock = ultimoMovimiento.Stock - inventario.cantidad;
                    inv.Cantidad = inventario.cantidad;
                    inv.Descripcion = "SALIDA";
                    _context.Add(inv);
                    await _context.SaveChangesAsync();
                    producto.Stock = producto.Stock - inventario.cantidad;
                    sucursalInventario.EstadoBoolean = true;
                    sucursalInventario.IdInventario = inv.IdInventario;
                    sucursalInventario.IdSucursal = usuarioSucursal.IdSucursal;
                    sucursalInventario.FechaCreacion = new DateTime();
                    sucursalInventario.UsuarioCreacion = int.Parse(idUsuario);
                    _context.Add(sucursalInventario);
                    await _context.SaveChangesAsync();
                }
                else if (inventario.tipoMovimiento.Equals("T") && producto.Stock >= 0 && producto.Stock > inventario.cantidad)
                {
                    inv.IdProducto = inventario.idProducto;
                    inv.TipoMovimiento = inventario.tipoMovimiento;
                    inv.NumeroDespacho = inventario.numeroDespacho;
                    inv.EstadoBoolean = true;
                    inv.UsuarioCreacion = int.Parse(idUsuario);
                    inv.FechaCreacion = new DateTime();
                    inv.Cantidad=inventario.cantidad;
                    inv.Descripcion = "TRANSFERENCIA";
                    if (ultimoMovimiento != null)
                    {
                        inv.Stock = ultimoMovimiento.Stock - inventario.cantidad;
                    }
                    else
                    {
                        inv.Stock = 0 - inventario.cantidad;
                    }
                    
                    _context.Add(inv);
                    await _context.SaveChangesAsync();
                    producto.Stock = producto.Stock - inventario.cantidad;
                    sucursalInventario.EstadoBoolean = true;
                    sucursalInventario.IdInventario = inv.IdInventario;
                    sucursalInventario.IdSucursal = inventario.sucursalDestino;
                    sucursalInventario.FechaCreacion = new DateTime();
                    sucursalInventario.UsuarioCreacion = int.Parse(idUsuario);
                    _context.Add(sucursalInventario);
                    await _context.SaveChangesAsync();
                    despacho.EstadoBoolean = true;
                    despacho.IdSucursal = usuarioSucursal.IdSucursal;
                    despacho.IdSucursalDestino=inventario.sucursalDestino;
                    despacho.IdUsuario=int.Parse(idUsuario);
                    despacho.IdEmpresa=int.Parse(idEmpresa);
                    despacho.EstadoDespacho = "PEDIENTE";
                    despacho.EstadoBoolean=true;
                    despacho.NumeroDespacho=inventario.numeroDespacho;
                    despacho.UsuarioCreacion = int.Parse(idUsuario);
                    despacho.FechaCreacion=new DateTime();  
                    _context.Add(despacho);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Notificacion("Revisar la cantidad del despacho", NotificacionTipo.Warning);
                    return RedirectToAction(nameof(Index));
                }
                _context.Update(producto);
                await _context.SaveChangesAsync();                
                Notificacion("Registro guardado con éxito", NotificacionTipo.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.idProducto);
                Notificacion("Error al guardar el Registro" + ex.Message, NotificacionTipo.Error);
                return View(inventario);
            }
        }

        // GET: Inventario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.IdProducto);
            return View(inventario);
        }

        // POST: Inventario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInventario,IdProducto,TipoMovimiento,Cantidad,FechaMovimiento,NumeroDespacho,Descripcion,EstadoBoolean,FechaCreacion,FechaModificacion,UsuarioCreacion,UsuarioModificacion")] Inventario inventario)
        {
            if (id != inventario.IdInventario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string idUsuario = HttpContext.Session.GetString("_idUsuario");
                    inventario.UsuarioModificacion = int.Parse(idUsuario);
                    inventario.FechaModificacion = new DateTime();
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                    Notificacion("Registro actualizado con éxito", NotificacionTipo.Success);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!InventarioExists(inventario.IdInventario))
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
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "IdProducto", inventario.IdProducto);
            return View(inventario);
        }

        // GET: Inventario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventarios == null)
            {
                return Problem("Entity set 'ContableContext.Inventarios'  is null.");
            }
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario != null)
            {
                string idUsuario = HttpContext.Session.GetString("_idUsuario");
                inventario.UsuarioModificacion = int.Parse(idUsuario);
                inventario.FechaModificacion = new DateTime();
                inventario.EstadoBoolean = false;
                _context.Inventarios.Update(inventario);

                Producto producto = new Producto();
                producto = _context.Productos.Where(p => p.IdProducto == inventario.IdProducto).FirstOrDefault();
                if (producto != null)
                {
                    producto.Stock = 0;
                    _context.Update(producto);
                }
            }
            
            await _context.SaveChangesAsync();
            Notificacion("Registro eliminado con éxito", NotificacionTipo.Success);
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(int id)
        {
          return (_context.Inventarios?.Any(e => e.IdInventario == id)).GetValueOrDefault();
        }
    }
}
