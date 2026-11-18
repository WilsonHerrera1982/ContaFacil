-- ============================================
-- Script: Crear tabla cuenta_empresa
-- Descripción: Tabla intermedia para relación M:M entre cuenta y empresa
-- Autor: Sistema ContaFacil
-- Fecha: 2025-10-14
-- ============================================

USE contable;

-- Crear tabla cuenta_empresa
CREATE TABLE IF NOT EXISTS cuenta_empresa (
    id_cuenta_empresa INT AUTO_INCREMENT PRIMARY KEY COMMENT 'ID único de la relación cuenta-empresa',
    id_cuenta INT NOT NULL COMMENT 'ID de la cuenta',
    id_empresa INT NOT NULL COMMENT 'ID de la empresa',
    saldo_inicial DECIMAL(15,2) NOT NULL DEFAULT 0.00 COMMENT 'Saldo inicial de la cuenta para esta empresa',
    saldo_actual DECIMAL(15,2) NOT NULL DEFAULT 0.00 COMMENT 'Saldo actual de la cuenta para esta empresa',
    estado TINYINT(1) NOT NULL DEFAULT 1 COMMENT '1=Activo, 0=Inactivo',
    fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Fecha de creación del registro',
    fecha_modificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Fecha de última modificación',
    usuario_creacion INT NOT NULL COMMENT 'ID del usuario que creó el registro',
    usuario_modificacion INT COMMENT 'ID del usuario que modificó el registro',
    
    -- Índices
    INDEX idx_cuenta_empresa_cuenta (id_cuenta),
    INDEX idx_cuenta_empresa_empresa (id_empresa),
    INDEX idx_cuenta_empresa_estado (estado),
    
    -- Clave única para evitar duplicados
    UNIQUE KEY uk_cuenta_empresa (id_cuenta, id_empresa),
    
    -- Llaves foráneas
    CONSTRAINT fk_cuenta_empresa_cuenta 
        FOREIGN KEY (id_cuenta) REFERENCES cuenta(id_cuenta)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    
    CONSTRAINT fk_cuenta_empresa_empresa 
        FOREIGN KEY (id_empresa) REFERENCES empresa(id_empresa)
        ON DELETE CASCADE
        ON UPDATE CASCADE
        
) ENGINE=InnoDB 
  DEFAULT CHARSET=utf8mb4 
  COLLATE=utf8mb4_0900_ai_ci 
  COMMENT='Relación muchos a muchos entre cuentas y empresas. Permite que una cuenta pueda ser usada por múltiples empresas con saldos independientes';

-- Verificar creación
SELECT 'Tabla cuenta_empresa creada exitosamente' AS mensaje;
