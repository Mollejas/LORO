-- Script para agregar campo 'precio' a la tabla Refacciones
-- Ejecutar este script en la base de datos

USE [DAYTONAMIO]
GO

-- Verificar si el campo ya existe antes de agregarlo
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Refacciones')
    AND name = 'precio'
)
BEGIN
    ALTER TABLE dbo.Refacciones
    ADD precio DECIMAL(18, 2) NULL DEFAULT 0;

    PRINT 'Campo precio agregado exitosamente a la tabla Refacciones';
END
ELSE
BEGIN
    PRINT 'El campo precio ya existe en la tabla Refacciones';
END
GO
