-- Script para verificar si la columna 'complemento' existe en la tabla refacciones
-- Ejecuta este script PRIMERO para diagnosticar el problema

USE DaytonaDB;
GO

PRINT '========================================';
PRINT 'VERIFICACIÓN DE COLUMNA COMPLEMENTO';
PRINT '========================================';
PRINT '';

-- Verificar si la tabla existe
IF OBJECT_ID(N'refacciones', N'U') IS NULL
BEGIN
    PRINT '❌ ERROR: La tabla refacciones NO EXISTE';
END
ELSE
BEGIN
    PRINT '✓ La tabla refacciones existe';
    PRINT '';

    -- Verificar si la columna existe
    IF EXISTS (
        SELECT * FROM sys.columns
        WHERE object_id = OBJECT_ID(N'refacciones')
        AND name = 'complemento'
    )
    BEGIN
        PRINT '✓ La columna complemento YA EXISTE';
        PRINT '';
        PRINT 'Detalles de la columna:';
        SELECT
            COLUMN_NAME as Columna,
            DATA_TYPE as Tipo,
            IS_NULLABLE as Permite_Nulos,
            COLUMN_DEFAULT as Valor_Por_Defecto
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'refacciones'
        AND COLUMN_NAME = 'complemento';
    END
    ELSE
    BEGIN
        PRINT '❌ La columna complemento NO EXISTE';
        PRINT '';
        PRINT '👉 SOLUCIÓN: Ejecuta el script db_add_complemento_field.sql';
        PRINT '   para agregar la columna a la base de datos.';
    END
END

PRINT '';
PRINT '========================================';
GO
