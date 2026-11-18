-- ============================================
-- Script: Migrar datos de cuenta a cuenta_empresa
-- Descripción: Migra las relaciones existentes de id_empresa en cuenta
--              a la nueva tabla cuenta_empresa
-- Autor: Sistema ContaFacil
-- Fecha: 2025-10-14
-- ============================================

USE contable;

-- Verificar que la tabla cuenta_empresa existe
SELECT 'Iniciando migración de datos...' AS mensaje;

-- Migrar datos existentes de cuenta donde id_empresa NO es NULL
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
    c.id_empresa,
    c.saldo_inicial,
    c.saldo_actual,
    c.estado,
    c.fecha_creacion,
    c.fecha_modificacion,
    c.usuario_creacion,
    c.usuario_modificacion
FROM cuenta c
WHERE c.id_empresa IS NOT NULL
AND NOT EXISTS (
    -- Evitar duplicados si ya existen registros
    SELECT 1 
    FROM cuenta_empresa ce 
    WHERE ce.id_cuenta = c.id_cuenta 
    AND ce.id_empresa = c.id_empresa
);

-- Mostrar resumen de la migración
SELECT 
    COUNT(*) AS total_relaciones_migradas,
    COUNT(DISTINCT id_cuenta) AS cuentas_distintas,
    COUNT(DISTINCT id_empresa) AS empresas_distintas
FROM cuenta_empresa;

-- Verificar cuentas sin empresa asignada (quedarán como cuentas maestras)
SELECT 
    COUNT(*) AS cuentas_sin_empresa
FROM cuenta
WHERE id_empresa IS NULL;

SELECT 'Migración completada exitosamente' AS mensaje;

-- NOTA IMPORTANTE:
-- Después de verificar que la migración es correcta, puedes ejecutar:
-- 
-- ALTER TABLE cuenta DROP FOREIGN KEY (nombre_de_la_constraint_si_existe);
-- ALTER TABLE cuenta DROP COLUMN id_empresa;
--
-- Pero es recomendable mantener id_empresa por un tiempo para validación
-- o marcar como DEPRECATED en el código.
