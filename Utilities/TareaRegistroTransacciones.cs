using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz;
using ContaFacil.Models;
using Microsoft.AspNetCore.Http;
using NuGet.Protocol.Core.Types;

namespace ContaFacil.Utilities
{
    public class TareaRegistroTransacciones : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ContableContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TareaRegistroTransacciones(IServiceProvider serviceProvider, ContableContext context, IHttpContextAccessor httpContext)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _httpContextAccessor = httpContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await RegistrarTransacciones();
        }

        public async Task RegistrarTransacciones()
        {
            var inventariosNoRegistrados = await _context.Inventarios
                .Where(i => !i.TransaccionRegistrada)
                .Include(i => i.IdProductoNavigation)
                .ToListAsync();
            foreach (var inventario in inventariosNoRegistrados)
            {
                if (inventario.TipoMovimiento == "E")
                {
                    List<CategoriaProducto> categoriaProducto = _context.CategoriaProductos.ToList();
                    List<Cuentum> listaCuentas = new List<Cuentum>();
                    foreach (var categoria in categoriaProducto)
                    {
                        Cuentum c = new Cuentum();
                        c = _context.Cuenta.FirstOrDefault(c => c.Nombre == categoria.Nombre);
                        listaCuentas.Add(c);

                    }
                    // Agrupar los inventarios por la cuenta asociada
                    var inventariosPorCuenta = inventariosNoRegistrados.GroupBy(i => listaCuentas.FirstOrDefault(c => c.Nombre == i.IdProductoNavigation.IdCategoriaProductoNavigation.Nombre));

                    // Crear una lista de listas de inventarios, una por cada cuenta
                    List<List<Inventario>> listasInventarios = new List<List<Inventario>>();
                    foreach (var group in inventariosPorCuenta)
                    {
                        foreach (var i in group)
                        {
                            i.TransaccionRegistrada = true;
                            _context.Update(i);
                        }
                        if (group.Key != null)
                        {
                            listasInventarios.Add(group.ToList());
                        }
                    }
                    foreach (var group in listasInventarios)
                    {
                        var primerInventario = group.FirstOrDefault();
                        int idCuenta = primerInventario.IdCuentaContable ?? 0;
                        int idSucursal = primerInventario.IdSucursal ?? 0;
                        Sucursal sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == idSucursal);
                        // Sumar el campo Total para cada grupo
                        decimal totalSum = group.Sum(i => i.Total ?? 0);
                        var transacciones = new List<Transaccion>();
                        Usuario usuario = _context.Usuarios.Where(u => u.Nombre == "admin").FirstOrDefault();
                        UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
                        usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                        Persona persona = new Persona();
                        persona = _context.Personas.Where(p => p.IdPersona == usuario.IdPersona).FirstOrDefault();
                        Emisor emisor = new Emisor();
                        emisor = _context.Emisors.Where(e => e.IdEmisor == sucursal.IdEmisor).FirstOrDefault();
                        Empresa empresa = new Empresa();
                        empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
                        // Buscar la cuenta de Inventarios
                        var cuentaInventarios = listaCuentas.FirstOrDefault(c => c.Nombre == "Inventarios");

                        // Buscar la cuenta Relacionadas por pagar
                        var cuentaRelacionadasPorPagar = _context.Cuenta.FirstOrDefault(c => c.Nombre == "Relacionadas por pagar");
                        // Obtener los datos necesarios de la primera entidad de la agrupación
                        var inv = group.FirstOrDefault();
                        var asientoNumero = await ObtenerSiguienteNumeroAsiento();
                        // Crear la transacción para la cuenta de Inventarios
                        var transaccionInventarios = new Transaccion
                        {
                            IdCuenta = idCuenta,
                            Fecha = DateOnly.FromDateTime(inv.FechaCreacion),
                            IdTipoTransaccion = await ObtenerIdTipoTransaccion("Saldo Inicial"),
                            Monto = totalSum, // Utilizar la suma total
                            Descripcion = asientoNumero+ $" Saldo inicial Inventarios",
                            Estado = true,
                            FechaCreacion = DateTime.Now,
                            FechaModificacion = DateTime.Now,
                            UsuarioCreacion = 1,
                            IdEmpresa = empresa.IdEmpresa,
                            IdInventario = inv.IdInventario,
                            EsDebito=true,
                        };
                        transacciones.Add(transaccionInventarios);

                        // Crear la transacción para la cuenta Relacionadas por pagar
                        var transaccionRelacionadasPorPagar = new Transaccion
                        {
                            IdCuenta = cuentaRelacionadasPorPagar.IdCuenta,
                            Fecha = DateOnly.FromDateTime(inv.FechaCreacion),
                            IdTipoTransaccion = await ObtenerIdTipoTransaccion("Saldo Inicial"),
                            Monto = totalSum, // Utilizar la suma total
                            Descripcion = asientoNumero+ $" Saldo inicial Relacionadas por pagar",
                            Estado = true,
                            FechaCreacion = DateTime.Now,
                            FechaModificacion = DateTime.Now,
                            UsuarioCreacion = 1,
                            IdEmpresa = empresa.IdEmpresa,
                            IdInventario = inv.IdInventario
                        };
                        transacciones.Add(transaccionRelacionadasPorPagar);
                        _context.Transaccions.AddRange(transacciones);
                        inventario.TransaccionRegistrada = true;
                        await _context.SaveChangesAsync(); // Guarda después de cada inventario procesado
                        return;
                    }


                }
            }
            inventariosNoRegistrados = await _context.Inventarios
                .Where(i => !i.TransaccionRegistrada)
                .Include(i => i.IdProductoNavigation)
                .ToListAsync();
            foreach (var inventario in inventariosNoRegistrados)
            {
                var transacciones = await GenerarTransacciones(inventario);
                _context.Transaccions.AddRange(transacciones);
                inventario.TransaccionRegistrada = true;
                await _context.SaveChangesAsync(); // Guarda después de cada inventario procesado
            }
        }

        private async Task<List<Transaccion>> GenerarTransacciones(Inventario inventario)
        {
            var transacciones = new List<Transaccion>();
            var producto = inventario.IdProductoNavigation;
            var asientoNumero = await ObtenerSiguienteNumeroAsiento();
            Sucursal sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == inventario.IdSucursal);
            Emisor emisor = _context.Emisors.FirstOrDefault(e => e.IdEmisor == sucursal.IdEmisor);
            Empresa empresa = _context.Empresas.FirstOrDefault(e => e.Identificacion == emisor.Ruc);
            // Función local para crear una nueva transacción
            Cuentum cuentum = _context.Cuenta.FirstOrDefault(c=>c.IdCuenta==inventario.IdCuentaContable);
            Transaccion CrearTransaccion(string codigoCuenta, decimal monto, string descripcion, string tipoTransaccion,Boolean esDebito)
            {
                return new Transaccion
                {
                    IdCuenta = ObtenerIdCuenta(codigoCuenta).Result,
                    Fecha = DateOnly.FromDateTime(inventario.FechaMovimiento),
                    IdTipoTransaccion = ObtenerIdTipoTransaccion(tipoTransaccion).Result,
                    Monto = monto,
                    Descripcion = $"{asientoNumero} {descripcion}",
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    UsuarioCreacion = 1, // Asume un usuario por defecto
                    IdEmpresa = empresa.IdEmpresa,
                    IdInventario=inventario.IdInventario
                };
            }
            if (inventario.TipoMovimiento == "E") // Compra
            {
                transacciones.Add(CrearTransaccion(cuentum.Codigo, inventario.Total ?? 0, $"Saldo de {producto.Nombre}", "Saldo",false));
            }
            if (inventario.TipoMovimiento == "C") // Compra
            {
                if (inventario.Descuento > 0)
                {
                    transacciones.Add(CrearTransaccion(cuentum.Codigo, inventario.Subtotal15 ?? 0, $"Compra de {producto.Nombre}", "Compra",false));
                }
                else
                {
                    transacciones.Add(CrearTransaccion(cuentum.Codigo, inventario.SubTotal ?? 0, $"Compra de {producto.Nombre}", "Compra",true));
                }
                
                transacciones.Add(CrearTransaccion("1.1.3.3", inventario.Iva ?? 0, $"IVA en compra de {producto.Nombre}", "Compra", false));
               
                transacciones.Add(CrearTransaccion("2.1.1.1", -(inventario.Total ?? 0), $"Pago a proveedor por {producto.Nombre}", "Compra", false));
            }
            

            return transacciones;
        }

        private async Task<int> ObtenerIdCuenta(string codigoCuenta)
        {
            var cuenta = await _context.Cuenta
                .FirstOrDefaultAsync(c => c.Codigo == codigoCuenta);
            return cuenta?.IdCuenta ?? throw new Exception($"Cuenta no encontrada para el código {codigoCuenta}");
        }

        private async Task<int> ObtenerIdTipoTransaccion(string tipoTransaccion)
        {
            var tipo = await _context.TipoTransaccions
                .FirstOrDefaultAsync(t => t.Nombre == tipoTransaccion);
            return tipo?.IdTipoTransaccion ?? throw new Exception($"Tipo de transacción no encontrado: {tipoTransaccion}");
        }

        private async Task<string> ObtenerSiguienteNumeroAsiento()
        {
            var ultimaTransaccion = await _context.Transaccions
                .OrderByDescending(t => t.IdTransaccion)
                .FirstOrDefaultAsync();

            if (ultimaTransaccion == null)
            {
                return "AS0001";
            }

            string ultimoNumeroAsiento = ultimaTransaccion.Descripcion.Split(' ')[0];
            if (ultimoNumeroAsiento.StartsWith("AS"))
            {
                int numero = int.Parse(ultimoNumeroAsiento.Substring(2)) + 1;
                return $"AS{numero:D4}";
            }
            else
            {
                return "AS0001";
            }
        }
        private async Task<List<Transaccion>> GenerarTransacciones(Inventario inventario, List<Cuentum> listaCuentas)
        {
            var transacciones = new List<Transaccion>();
            Usuario usuario = _context.Usuarios.Where(u => u.Nombre == "admin").FirstOrDefault();
            UsuarioSucursal usuarioSucursal = new UsuarioSucursal();
            usuarioSucursal = _context.UsuarioSucursals.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
            Persona persona = new Persona();
            persona = _context.Personas.Where(p => p.IdPersona == usuario.IdPersona).FirstOrDefault();
            Emisor emisor = new Emisor();
            emisor = _context.Emisors.Where(e => e.Ruc == persona.Identificacion).FirstOrDefault();
            Empresa empresa = new Empresa();
            empresa = _context.Empresas.Where(e => e.Identificacion == emisor.Ruc).FirstOrDefault();
            // Buscar la cuenta de Inventarios
            var cuentaInventarios = listaCuentas.FirstOrDefault(c => c.Nombre == "Inventarios");

            // Buscar la cuenta Relacionadas por pagar
            var cuentaRelacionadasPorPagar = listaCuentas.FirstOrDefault(c => c.Nombre == "Relacionadas por pagar");

            if (cuentaInventarios != null && cuentaRelacionadasPorPagar != null)
            {
                // Crear la transacción para la cuenta de Inventarios
                var transaccionInventarios = new Transaccion
                {
                    IdCuenta = cuentaInventarios.IdCuenta,
                    Fecha = DateOnly.FromDateTime(inventario.FechaCreacion),
                    IdTipoTransaccion = await ObtenerIdTipoTransaccion("Saldo inicial"),
                    Monto = inventario.Total??0,
                    Descripcion = $"Saldo inicial Inventarios",
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    UsuarioCreacion = 1,
                    IdEmpresa = empresa.IdEmpresa,
                    IdInventario = inventario.IdInventario
                };
                transacciones.Add(transaccionInventarios);

                // Crear la transacción para la cuenta Relacionadas por pagar
                var transaccionRelacionadasPorPagar = new Transaccion
                {
                    IdCuenta = cuentaRelacionadasPorPagar.IdCuenta,
                    Fecha = DateOnly.FromDateTime(inventario.FechaCreacion),
                    IdTipoTransaccion = await ObtenerIdTipoTransaccion("Saldo inicial"),
                    Monto = inventario.Total??0,
                    Descripcion = $"Saldo inicial Relacionadas por pagar",
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    UsuarioCreacion = 1,
                    IdEmpresa = empresa.IdEmpresa,
                    IdInventario = inventario.IdInventario
                };
                transacciones.Add(transaccionRelacionadasPorPagar);
            }

            return transacciones;
        }
    }
}