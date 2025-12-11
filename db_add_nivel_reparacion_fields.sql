USE DaytonaDB;
GO

-- Agregar campos de Nivel Reparación a tabla refacciones
-- Solo se agregan si no existen

-- Nivel Reparación: L, M, F
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'refacciones')
    AND name = 'nivel_rep_l'
)
BEGIN
    ALTER TABLE refacciones
    ADD nivel_rep_l BIT NOT NULL DEFAULT 0;
    PRINT 'Columna nivel_rep_l agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna nivel_rep_l ya existe';
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'refacciones')
    AND name = 'nivel_rep_m'
)
BEGIN
    ALTER TABLE refacciones
    ADD nivel_rep_m BIT NOT NULL DEFAULT 0;
    PRINT 'Columna nivel_rep_m agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna nivel_rep_m ya existe';
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'refacciones')
    AND name = 'nivel_rep_f'
)
BEGIN
    ALTER TABLE refacciones
    ADD nivel_rep_f BIT NOT NULL DEFAULT 0;
    PRINT 'Columna nivel_rep_f agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna nivel_rep_f ya existe';
END
GO

-- Nivel Reparación Pintura: L, M, F
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'refacciones')
    AND name = 'nivel_rep_pint_l'
)
BEGIN
    ALTER TABLE refacciones
    ADD nivel_rep_pint_l BIT NOT NULL DEFAULT 0;
    PRINT 'Columna nivel_rep_pint_l agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna nivel_rep_pint_l ya existe';
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'refacciones')
    AND name = 'nivel_rep_pint_m'
)
BEGIN
    ALTER TABLE refacciones
    ADD nivel_rep_pint_m BIT NOT NULL DEFAULT 0;
    PRINT 'Columna nivel_rep_pint_m agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna nivel_rep_pint_m ya existe';
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'refacciones')
    AND name = 'nivel_rep_pint_f'
)
BEGIN
    ALTER TABLE refacciones
    ADD nivel_rep_pint_f BIT NOT NULL DEFAULT 0;
    PRINT 'Columna nivel_rep_pint_f agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna nivel_rep_pint_f ya existe';
END
GO

PRINT 'Script completado. Todos los campos de Nivel Reparación han sido procesados.';
GO
