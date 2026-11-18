# Actualización: Filtrado de Cuentas por Empresa

## 📋 Resumen

Se actualizó el **CuentumController** para que el **Plan de Cuentas** se filtre automáticamente según la empresa del usuario logueado, utilizando la nueva tabla intermedia `cuenta_empresa`.

---

## 🔄 Cambios Implementados

### 1. **Método Index** - Filtrado por Empresa

**Antes:**
```csharp
public async Task<IActionResult> Index()
{
    var cuentas = await _context.Cuenta.Where(c=>c.Estado==true)
        .Include(c => c.IdEmpresaNavigation)
        .Include(c => c.IdTipoCuentaNavigation).OrderBy(c=>c.Codigo)
        .ToListAsync();

    var viewModel = new CuentumIndexViewModel
    {
        Cuentas = cuentas
    };
    return View(viewModel);
}
```

**Después:**
```csharp
public async Task<IActionResult> Index()
{
    // Obtener el id de empresa del usuario logueado
    string idEmpresaStr = HttpContext.Session.GetString("_empresa");
    
    if (string.IsNullOrEmpty(idEmpresaStr))
    {
        // Si no hay empresa en sesión, redirigir al login
        return RedirectToAction("Login", "Account");
    }

    int idEmpresa = int.Parse(idEmpresaStr);

    // Obtener las cuentas asociadas a la empresa usando cuenta_empresa
    var cuentas = await _context.CuentaEmpresas
        .Where(ce => ce.IdEmpresa == idEmpresa && ce.Estado == true)
        .Include(ce => ce.IdCuentaNavigation)
            .ThenInclude(c => c.IdTipoCuentaNavigation)
        .Select(ce => ce.IdCuentaNavigation)
        .Where(c => c.Estado == true)
        .OrderBy(c => c.Codigo)
        .ToListAsync();

    var viewModel = new CuentumIndexViewModel
    {
        Cuentas = cuentas
    };
    return View(viewModel);
}
```

**Mejoras:**
- ✅ Filtra cuentas por `id_empresa` del usuario en sesión
- ✅ Usa la tabla intermedia `cuenta_empresa`
- ✅ Valida que exista empresa en sesión
- ✅ Solo muestra cuentas activas para esa empresa

---

### 2. **Método Create** - Asociar Cuenta a Empresa

**Antes:**
```csharp
[HttpPost]
[IgnoreAntiforgeryToken]
public async Task<IActionResult> Create([FromForm] Cuentum cuentum)
{
    try
    {
        string idUsuario = HttpContext.Session.GetString("_idUsuario");
        Empresa empresa = _context.Empresas.FirstOrDefault(e=>e.Identificacion=="0401253950001");
        // ... guardar cuenta ...
        cuentum.IdEmpresa = empresa.IdEmpresa;
        _context.Add(cuentum);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    catch (Exception e)
    {
        return View();
    }
}
```

**Después:**
```csharp
[HttpPost]
[IgnoreAntiforgeryToken]
public async Task<IActionResult> Create([FromForm] Cuentum cuentum)
{
    try
    {
        string idUsuario = HttpContext.Session.GetString("_idUsuario");
        string idEmpresaStr = HttpContext.Session.GetString("_empresa");
        
        if (string.IsNullOrEmpty(idEmpresaStr))
        {
            Notificacion("No se pudo obtener la empresa del usuario", NotificacionTipo.Error);
            return View();
        }

        int idEmpresa = int.Parse(idEmpresaStr);
        
        // ... configurar cuenta ...
        
        // Guardar la cuenta
        _context.Add(cuentum);
        await _context.SaveChangesAsync();

        // Crear la relación en cuenta_empresa
        var cuentaEmpresa = new CuentaEmpresa
        {
            IdCuenta = cuentum.IdCuenta,
            IdEmpresa = idEmpresa,
            SaldoInicial = 0,
            SaldoActual = 0,
            Estado = true,
            FechaCreacion = DateTime.Now,
            UsuarioCreacion = int.Parse(idUsuario)
        };

        _context.CuentaEmpresas.Add(cuentaEmpresa);
        await _context.SaveChangesAsync();

        Notificacion("Registro guardado correctamente", NotificacionTipo.Success);
        return RedirectToAction("Index");
    }
    catch (Exception e)
    {
        Notificacion($"Error al guardar el registro: {e.Message}", NotificacionTipo.Error);
        return View();
    }
}
```

**Mejoras:**
- ✅ Usa `_empresa` de sesión en lugar de identificación hardcodeada
- ✅ Crea registro en `cuenta_empresa` automáticamente
- ✅ Valida que exista empresa en sesión
- ✅ Mejor manejo de errores con mensajes descriptivos

---

### 3. **Método ExportToExcel** - Exportar Solo Cuentas de la Empresa

**Antes:**
```csharp
public IActionResult ExportToExcel()
{
    var cuentas = _context.Cuenta.Where(c=>c.Estado==true)
        .OrderBy(c => c.Codigo).ToList();
    // ... generar Excel ...
}
```

**Después:**
```csharp
public IActionResult ExportToExcel()
{
    // Obtener el id de empresa del usuario logueado
    string idEmpresaStr = HttpContext.Session.GetString("_empresa");
    
    if (string.IsNullOrEmpty(idEmpresaStr))
    {
        return RedirectToAction("Login", "Account");
    }

    int idEmpresa = int.Parse(idEmpresaStr);

    // Obtener cuentas de la empresa usando cuenta_empresa
    var cuentas = _context.CuentaEmpresas
        .Where(ce => ce.IdEmpresa == idEmpresa && ce.Estado == true)
        .Include(ce => ce.IdCuentaNavigation)
        .Select(ce => ce.IdCuentaNavigation)
        .Where(c => c.Estado == true)
        .OrderBy(c => c.Codigo)
        .ToList();
    
    // ... generar Excel ...
}
```

**Mejoras:**
- ✅ Exporta solo cuentas de la empresa del usuario
- ✅ Usa la tabla intermedia `cuenta_empresa`
- ✅ Valida sesión antes de exportar

---

## 🎯 Flujo de Trabajo Actualizado

### Escenario 1: Usuario Lista Cuentas

1. Usuario navega a `/Cuentum/Index`
2. Sistema obtiene `_empresa` de sesión
3. Sistema consulta `cuenta_empresa` filtrando por `id_empresa`
4. Sistema carga las cuentas relacionadas de la tabla `cuenta`
5. Vista muestra solo las cuentas de esa empresa

### Escenario 2: Usuario Crea Nueva Cuenta

1. Usuario crea una nueva cuenta
2. Sistema guarda la cuenta en tabla `cuenta`
3. Sistema crea registro en `cuenta_empresa` asociando cuenta con empresa del usuario
4. La cuenta queda disponible solo para esa empresa

### Escenario 3: Usuario Exporta a Excel

1. Usuario hace clic en "Exportar a Excel"
2. Sistema filtra cuentas por empresa del usuario
3. Excel generado contiene solo las cuentas de esa empresa

---

## ✅ Ventajas de los Cambios

### 1. **Aislamiento de Datos**
- Cada empresa solo ve sus propias cuentas
- No hay riesgo de ver información de otras empresas

### 2. **Seguridad**
- Validación de sesión en cada método
- Redirección automática si no hay empresa en sesión

### 3. **Escalabilidad**
- Preparado para múltiples empresas/clientes
- Fácil agregar más empresas sin modificar código

### 4. **Consistencia**
- Todos los métodos usan el mismo patrón de filtrado
- Centralizado en la tabla intermedia `cuenta_empresa`

---

## 📝 Consideraciones Importantes

### 1. **Sesión Requerida**

Todos los métodos ahora requieren que exista `_empresa` en sesión. Asegúrate de que el login establezca esta variable:

```csharp
// En el controlador de Login
HttpContext.Session.SetString("_empresa", usuario.IdEmpresa.ToString());
```

### 2. **Migración de Datos Existentes**

Si ya tienes datos en producción, debes:
1. Ejecutar el script `02_migrar_datos_cuenta_empresa.sql`
2. Verificar que todas las cuentas tengan relación con su empresa
3. Validar que los usuarios tengan `_empresa` en sesión al loguearse

### 3. **Vista Index.cshtml**

La vista **no requiere cambios** porque:
- Recibe el mismo `CuentumIndexViewModel`
- El filtrado se hace en el controlador
- La vista simplemente muestra las cuentas que recibe

---

## 🧪 Pruebas Recomendadas

### 1. **Prueba de Filtrado**
- [ ] Login con usuario de empresa A
- [ ] Verificar que solo se muestren cuentas de empresa A
- [ ] Login con usuario de empresa B
- [ ] Verificar que solo se muestren cuentas de empresa B

### 2. **Prueba de Creación**
- [ ] Crear nueva cuenta estando logueado con empresa A
- [ ] Verificar que la cuenta aparezca en cuenta_empresa con id_empresa A
- [ ] Login con empresa B
- [ ] Verificar que la cuenta NO aparezca para empresa B

### 3. **Prueba de Exportación**
- [ ] Exportar Excel con usuario de empresa A
- [ ] Verificar que Excel contenga solo cuentas de empresa A
- [ ] Exportar Excel con usuario de empresa B
- [ ] Verificar que Excel contenga solo cuentas de empresa B

### 4. **Prueba de Sesión Inválida**
- [ ] Limpiar sesión (logout)
- [ ] Intentar acceder a `/Cuentum/Index`
- [ ] Verificar redirección al login

---

## 🔧 Próximos Pasos Sugeridos

1. **Actualizar otros controladores** que usen la tabla `cuenta` para filtrar por empresa
2. **Crear interfaz de administración** para asignar cuentas a empresas
3. **Implementar reporte de uso** de cuentas por empresa
4. **Agregar logs de auditoría** para cambios en cuenta_empresa

---

## 📚 Archivos Modificados

- ✅ `Controllers/CuentumController.cs` - Métodos Index, Create y ExportToExcel
- ✅ `Views/Cuentum/Index.cshtml` - Sin cambios (funciona con el filtrado del controlador)

---

**Autor**: Sistema ContaFacil  
**Fecha**: 2025-10-14  
**Versión**: 1.0
