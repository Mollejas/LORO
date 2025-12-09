-- Script para agregar campo 'complemento' a la tabla refacciones
-- Este campo almacenará si una refacción está marcada como COMPLEMENTO

USE DaytonaDB;
GO

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'refacciones')
    AND name = 'complemento'
)
BEGIN
    ALTER TABLE refacciones
    ADD complemento BIT NOT NULL DEFAULT 0;

    PRINT 'Columna complemento agregada exitosamente a la tabla refacciones';
END
ELSE
BEGIN
    PRINT 'La columna complemento ya existe en la tabla refacciones';
END
GO

-- Verificar la estructura de la tabla
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'refacciones'
ORDER BY ORDINAL_POSITION;
GO
