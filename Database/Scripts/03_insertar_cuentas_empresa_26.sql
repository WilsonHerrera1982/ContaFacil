-- ============================================
-- Script: Asociar cuentas con empresa ID 26
-- Descripción: Inserta registros en cuenta_empresa para asociar
--              todas las cuentas activas con la empresa 26
-- Autor: Sistema ContaFacil
-- Fecha: 2025-10-14
-- ============================================

USE contable;

-- Verificar que la empresa 26 existe
SELECT 'Verificando empresa...' AS mensaje;
SELECT 
    id_empresa, 
    nombre, 
    identificacion, 
    estado 
FROM empresa 
WHERE id_empresa = 26;

-- Mostrar total de cuentas activas
SELECT 
    COUNT(*) AS total_cuentas_activas,
    'Cuentas que serán asociadas a la empresa 26' AS descripcion
FROM cuenta 
WHERE estado = 1;

-- Insertar registros en cuenta_empresa para la empresa 26
-- Solo inserta si no existe ya la relación
INSERT INTO cuenta_empresa (
    id_cuenta,
    id_empresa,
    saldo_inicial,
    saldo_actual,
    estado,
    fecha_creacion,
    fecha_modificacion,
    usuario_creacion,
    usuario_modificacion
)
SELECT 
    c.id_cuenta,
    26 AS id_empresa,                    -- Empresa 26
    0.00 AS saldo_inicial,               -- Saldo inicial en 0
    0.00 AS saldo_actual,                -- Saldo actual en 0
    1 AS estado,                         -- Activo
    NOW() AS fecha_creacion,             -- Fecha actual
    NOW() AS fecha_modificacion,         -- Fecha actual
    1 AS usuario_creacion,               -- Usuario sistema (ajustar si necesario)
    NULL AS usuario_modificacion
FROM cuenta c
WHERE 
    c.estado = 1                         -- Solo cuentas activas
    AND NOT EXISTS (                     -- Evitar duplicados
        SELECT 1 
        FROM cuenta_empresa ce 
        WHERE ce.id_cuenta = c.id_cuenta 
        AND ce.id_empresa = 26
    );

-- Mostrar resumen de la inserción
SELECT 
    'Registros insertados exitosamente' AS mensaje,
    COUNT(*) AS total_relaciones_creadas
FROM cuenta_empresa
WHERE id_empresa = 26;

-- Verificar algunas cuentas principales asociadas
SELECT 
    ce.id_cuenta_empresa,
    c.codigo,
    c.nombre,
    c.tipo,
    ce.saldo_inicial,
    ce.saldo_actual,
    ce.estado,
    ce.fecha_creacion
FROM cuenta_empresa ce
INNER JOIN cuenta c ON ce.id_cuenta = c.id_cuenta
WHERE ce.id_empresa = 26
ORDER BY c.codigo
LIMIT 20;

-- Verificar estructura jerárquica (cuentas padre e hijas)
SELECT 
    'Verificando estructura jerárquica...' AS mensaje;
    
SELECT 
    CASE 
        WHEN c.id_id_cuenta IS NULL THEN 'PADRE'
        ELSE 'HIJA'
    END AS tipo_cuenta,
    COUNT(*) AS total_cuentas
FROM cuenta_empresa ce
INNER JOIN cuenta c ON ce.id_cuenta = c.id_cuenta
WHERE ce.id_empresa = 26
GROUP BY tipo_cuenta;

-- Verificar por tipo de cuenta contable
SELECT 
    tc.nombre AS tipo_cuenta,
    COUNT(ce.id_cuenta_empresa) AS total_cuentas,
    SUM(CASE WHEN c.debito = 1 THEN 1 ELSE 0 END) AS cuentas_debito,
    SUM(CASE WHEN c.credito = 1 THEN 1 ELSE 0 END) AS cuentas_credito
FROM cuenta_empresa ce
INNER JOIN cuenta c ON ce.id_cuenta = c.id_cuenta
INNER JOIN tipocuenta tc ON c.id_tipo_cuenta = tc.id_tipo_cuenta
WHERE ce.id_empresa = 26
GROUP BY tc.nombre
ORDER BY tc.nombre;

SELECT '✓ Proceso completado exitosamente' AS resultado;

-- NOTAS IMPORTANTES:
-- 
-- 1. Este script asocia TODAS las cuentas activas con la empresa 26
-- 2. Los saldos se inicializan en 0.00
-- 3. Si necesitas saldos iniciales específicos, deberás actualizarlos después
-- 4. El usuario_creacion está configurado como 1 (usuario sistema)
--    Puedes cambiarlo si tienes otro ID de usuario preferido
--
-- PARA ACTUALIZAR SALDOS POSTERIORMENTE:
-- UPDATE cuenta_empresa 
-- SET saldo_inicial = [valor], saldo_actual = [valor]
-- WHERE id_empresa = 26 AND id_cuenta = [id_cuenta_especifica];
