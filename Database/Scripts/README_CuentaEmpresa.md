# Implementación Tabla Intermedia: cuenta_empresa

## 📋 Descripción General

Se ha implementado una tabla intermedia `cuenta_empresa` para establecer una **relación muchos a muchos** entre las tablas `cuenta` y `empresa`. Esto permite que:

- ✅ Una cuenta puede ser utilizada por múltiples empresas
- ✅ Una empresa puede tener múltiples cuentas
- ✅ Cada relación cuenta-empresa mantiene sus propios saldos independientes
- ✅ Se puede activar/desactivar cuentas por empresa sin afectar a otras

## 🗂️ Estructura de la Tabla

```sql
CREATE TABLE cuenta_empresa (
    id_cuenta_empresa INT AUTO_INCREMENT PRIMARY KEY,
    id_cuenta INT NOT NULL,
    id_empresa INT NOT NULL,
    saldo_inicial DECIMAL(15,2) DEFAULT 0.00,
    saldo_actual DECIMAL(15,2) DEFAULT 0.00,
    estado TINYINT(1) DEFAULT 1,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    usuario_creacion INT NOT NULL,
    usuario_modificacion INT,
    
    UNIQUE KEY uk_cuenta_empresa (id_cuenta, id_empresa),
    FOREIGN KEY (id_cuenta) REFERENCES cuenta(id_cuenta) ON DELETE CASCADE,
    FOREIGN KEY (id_empresa) REFERENCES empresa(id_empresa) ON DELETE CASCADE
);
```

## 📦 Archivos Creados

### 1. Scripts SQL
- **`01_crear_tabla_cuenta_empresa.sql`**: Crea la tabla en la base de datos
- **`02_migrar_datos_cuenta_empresa.sql`**: Migra los datos existentes del campo `id_empresa` en la tabla `cuenta`

### 2. Modelos C#
- **`Models/CuentaEmpresa.cs`**: Modelo de la entidad CuentaEmpresa

### 3. Actualizaciones
- **`Models/ContableContext.cs`**: 
  - Agregado `DbSet<CuentaEmpresa> CuentaEmpresas`
  - Configuración de la entidad en `OnModelCreating`
- **`Models/Cuentum.cs`**: 
  - Agregada colección `ICollection<CuentaEmpresa> CuentaEmpresas`
- **`Models/Empresa.cs`**: 
  - Agregada colección `ICollection<CuentaEmpresa> CuentaEmpresas`

## 🚀 Pasos de Implementación

### 1. Crear la tabla en la base de datos

```bash
# En MySQL Workbench o línea de comandos:
mysql -u root -p contable < 01_crear_tabla_cuenta_empresa.sql
```

### 2. Migrar los datos existentes

```bash
mysql -u root -p contable < 02_migrar_datos_cuenta_empresa.sql
```

### 3. Verificar la migración

```sql
-- Verificar total de relaciones migradas
SELECT COUNT(*) FROM cuenta_empresa;

-- Ver relaciones por empresa
SELECT 
    e.nombre AS empresa,
    COUNT(ce.id_cuenta) AS total_cuentas
FROM cuenta_empresa ce
JOIN empresa e ON ce.id_empresa = e.id_empresa
GROUP BY e.nombre;

-- Ver cuentas con múltiples empresas
SELECT 
    c.codigo,
    c.nombre,
    COUNT(ce.id_empresa) AS total_empresas
FROM cuenta_empresa ce
JOIN cuenta c ON ce.id_cuenta = c.id_cuenta
GROUP BY c.codigo, c.nombre
HAVING COUNT(ce.id_empresa) > 1;
```

## 💡 Casos de Uso

### Ejemplo 1: Asignar una cuenta existente a una nueva empresa

```csharp
var cuentaEmpresa = new CuentaEmpresa
{
    IdCuenta = 13, // ID de la cuenta "Activo"
    IdEmpresa = 25, // ID de la empresa
    SaldoInicial = 0,
    SaldoActual = 0,
    Estado = true,
    FechaCreacion = DateTime.Now,
    UsuarioCreacion = 1
};

_context.CuentaEmpresas.Add(cuentaEmpresa);
await _context.SaveChangesAsync();
```

### Ejemplo 2: Obtener todas las cuentas de una empresa

```csharp
var cuentasEmpresa = await _context.CuentaEmpresas
    .Include(ce => ce.IdCuentaNavigation)
    .Where(ce => ce.IdEmpresa == 25 && ce.Estado == true)
    .Select(ce => new
    {
        ce.IdCuentaNavigation.Codigo,
        ce.IdCuentaNavigation.Nombre,
        ce.SaldoActual,
        ce.IdCuentaNavigation.Debito,
        ce.IdCuentaNavigation.Credito
    })
    .ToListAsync();
```

### Ejemplo 3: Obtener empresas que usan una cuenta específica

```csharp
var empresasUsandoCuenta = await _context.CuentaEmpresas
    .Include(ce => ce.IdEmpresaNavigation)
    .Where(ce => ce.IdCuenta == 13 && ce.Estado == true)
    .Select(ce => new
    {
        ce.IdEmpresaNavigation.Nombre,
        ce.IdEmpresaNavigation.Identificacion,
        ce.SaldoActual
    })
    .ToListAsync();
```

### Ejemplo 4: Actualizar saldo de una cuenta para una empresa específica

```csharp
var cuentaEmpresa = await _context.CuentaEmpresas
    .FirstOrDefaultAsync(ce => ce.IdCuenta == 13 && ce.IdEmpresa == 25);

if (cuentaEmpresa != null)
{
    cuentaEmpresa.SaldoActual += 1000;
    cuentaEmpresa.FechaModificacion = DateTime.Now;
    cuentaEmpresa.UsuarioModificacion = 1;
    
    await _context.SaveChangesAsync();
}
```

## 🔄 Ventajas del Nuevo Diseño

### 1. **Plan de Cuentas Compartido**
- Permite tener un catálogo maestro de cuentas que todas las empresas pueden usar
- Reduce duplicación de cuentas estándar (Activo, Pasivo, etc.)

### 2. **Personalización por Empresa**
- Cada empresa puede activar solo las cuentas que necesita
- Puede agregar cuentas específicas para su negocio

### 3. **Saldos Independientes**
- Los saldos se manejan de forma independiente por empresa
- No hay riesgo de mezclar información contable entre empresas

### 4. **Flexibilidad**
- Fácil agregar/quitar empresas sin afectar el catálogo de cuentas
- Permite análisis consolidados o individuales

## ⚠️ Consideraciones Importantes

### 1. **Campo `id_empresa` en tabla `cuenta`**

El campo `id_empresa` en la tabla `cuenta` **aún existe** por compatibilidad. Se recomienda:

- **Corto plazo**: Mantenerlo para validación y rollback si es necesario
- **Mediano plazo**: Marcar como `[Obsolete]` en el código C#
- **Largo plazo**: Eliminar después de validar que todo funciona correctamente

```sql
-- Cuando estés listo para eliminar (NO ejecutar todavía):
-- ALTER TABLE cuenta DROP FOREIGN KEY empresa_cuenta;
-- ALTER TABLE cuenta DROP COLUMN id_empresa;
```

### 2. **Actualizar Servicios Existentes**

Los servicios que consultan cuentas deben actualizarse para usar `CuentaEmpresa`:

```csharp
// ❌ Forma antigua
var cuentas = await _context.Cuenta
    .Where(c => c.IdEmpresa == empresaId)
    .ToListAsync();

// ✅ Forma nueva
var cuentas = await _context.CuentaEmpresas
    .Include(ce => ce.IdCuentaNavigation)
    .Where(ce => ce.IdEmpresa == empresaId && ce.Estado == true)
    .Select(ce => ce.IdCuentaNavigation)
    .ToListAsync();
```

### 3. **Índices y Rendimiento**

La tabla cuenta_empresa tiene índices optimizados:
- `idx_cuenta_empresa_cuenta`: Para búsquedas por cuenta
- `idx_cuenta_empresa_empresa`: Para búsquedas por empresa
- `idx_cuenta_empresa_estado`: Para filtros por estado
- `uk_cuenta_empresa`: Evita duplicados cuenta-empresa

## 📊 Consultas Útiles

### Reporte de uso de cuentas por empresa

```sql
SELECT 
    e.nombre AS empresa,
    c.codigo,
    c.nombre AS cuenta,
    ce.saldo_actual,
    ce.estado
FROM cuenta_empresa ce
JOIN cuenta c ON ce.id_cuenta = c.id_cuenta
JOIN empresa e ON ce.id_empresa = e.id_empresa
WHERE ce.estado = 1
ORDER BY e.nombre, c.codigo;
```

### Cuentas sin empresas asignadas (maestras)

```sql
SELECT c.codigo, c.nombre
FROM cuenta c
LEFT JOIN cuenta_empresa ce ON c.id_cuenta = ce.id_cuenta
WHERE ce.id_cuenta_empresa IS NULL
ORDER BY c.codigo;
```

### Verificar integridad de datos

```sql
-- Verificar que no haya duplicados
SELECT id_cuenta, id_empresa, COUNT(*)
FROM cuenta_empresa
GROUP BY id_cuenta, id_empresa
HAVING COUNT(*) > 1;

-- Verificar referencias huérfanas
SELECT ce.*
FROM cuenta_empresa ce
LEFT JOIN cuenta c ON ce.id_cuenta = c.id_cuenta
LEFT JOIN empresa e ON ce.id_empresa = e.id_empresa
WHERE c.id_cuenta IS NULL OR e.id_empresa IS NULL;
```

## 🎯 Próximos Pasos

1. ✅ Ejecutar los scripts SQL para crear la tabla y migrar datos
2. ⏳ Actualizar servicios existentes para usar `CuentaEmpresa`
3. ⏳ Crear controlador y vistas para gestionar relaciones cuenta-empresa
4. ⏳ Implementar lógica de negocio para asignación automática de cuentas
5. ⏳ Validar que los reportes contables usen la nueva estructura
6. ⏳ Después de validación completa, considerar eliminación de `id_empresa` de `cuenta`

## 📝 Notas Adicionales

- La eliminación en cascada está habilitada: si eliminas una cuenta o empresa, se eliminarán automáticamente sus relaciones
- El campo `estado` permite desactivar una cuenta para una empresa sin eliminar la relación
- Los triggers y stored procedures que usen la tabla `cuenta` deben revisarse

---

**Autor**: Sistema ContaFacil  
**Fecha**: 2025-10-14  
**Versión**: 1.0
