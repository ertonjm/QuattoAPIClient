/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Stored Procedures v1.0
═══════════════════════════════════════════════════════════════════
*/

USE [SESCDF_DW];
GO

PRINT '═══════════════════════════════════════════════════════════════';
PRINT 'Criando Stored Procedures...';
PRINT '═══════════════════════════════════════════════════════════════';
GO

-- ═══════════════════════════════════════════════════════════════════
-- SP: usp_API_GetWatermark
-- Propósito: Obter último watermark para extração incremental
-- ═══════════════════════════════════════════════════════════════════

CREATE OR ALTER PROCEDURE dbo.usp_API_GetWatermark
    @SystemName     NVARCHAR(100),
    @Endpoint       NVARCHAR(200),
    @Environment    NVARCHAR(10) = 'PRD',
    @Watermark      NVARCHAR(100) OUTPUT,
    @LastRunUtc     DATETIME2(3) OUTPUT,
    @LastStatus     NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        @Watermark = CAST(LastWatermark AS NVARCHAR(100)),
        @LastRunUtc = LastRunUtc,
        @LastStatus = LastStatus
    FROM dbo.API_Watermarks WITH (READCOMMITTED)
    WHERE SystemName = @SystemName
      AND Endpoint = @Endpoint
      AND Environment = @Environment;
    
    -- Se não existe, retorna valores padrão
    IF @Watermark IS NULL
    BEGIN
        SET @Watermark = '1900-01-01T00:00:00Z';
        SET @LastRunUtc = '1900-01-01';
        SET @LastStatus = 'NEVER_RUN';
    END
END
GO

-- ═══════════════════════════════════════════════════════════════════
-- SP: usp_API_UpdateWatermark
-- Propósito: Atualizar controle de extração incremental (thread-safe)
-- ═══════════════════════════════════════════════════════════════════

CREATE OR ALTER PROCEDURE dbo.usp_API_UpdateWatermark
    @SystemName         NVARCHAR(100),
    @Endpoint           NVARCHAR(200),
    @Environment        NVARCHAR(10) = 'PRD',
    @NewWatermark       DATETIMEOFFSET(3) = NULL,
    @RecordsExtracted   BIGINT = 0,
    @PagesExtracted     INT = 0,
    @BytesExtracted     BIGINT = 0,
    @Status             NVARCHAR(50) = 'SUCCESS',
    @ErrorMessage       NVARCHAR(4000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRANSACTION;
    
    MERGE dbo.API_Watermarks WITH (HOLDLOCK) AS target
    USING (
        SELECT 
            @SystemName AS SystemName, 
            @Endpoint AS Endpoint, 
            @Environment AS Environment
    ) AS source
    ON target.SystemName = source.SystemName 
       AND target.Endpoint = source.Endpoint
       AND target.Environment = source.Environment
    WHEN MATCHED THEN
        UPDATE SET
            LastWatermark = COALESCE(@NewWatermark, target.LastWatermark),
            LastRunUtc = SYSUTCDATETIME(),
            LastStatus = @Status,
            TotalRecordsExtracted = target.TotalRecordsExtracted + @RecordsExtracted,
            TotalPagesExtracted = target.TotalPagesExtracted + @PagesExtracted,
            TotalBytesExtracted = target.TotalBytesExtracted + @BytesExtracted,
            Notes = CASE 
                        WHEN @ErrorMessage IS NOT NULL THEN @ErrorMessage
                        ELSE target.Notes
                    END,
            UpdatedUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (
            SystemName, Endpoint, Environment,
            LastWatermark, LastRunUtc, LastStatus,
            TotalRecordsExtracted, TotalPagesExtracted, TotalBytesExtracted,
            Notes
        )
        VALUES (
            @SystemName, @Endpoint, @Environment,
            @NewWatermark, SYSUTCDATETIME(), @Status,
            @RecordsExtracted, @PagesExtracted, @BytesExtracted,
            @ErrorMessage
        );
    
    COMMIT TRANSACTION;
END
GO

-- ═══════════════════════════════════════════════════════════════════
-- SP: usp_API_CheckRateLimit
-- Propósito: Verificar se pode fazer requisição (rate limit control)
-- ═══════════════════════════════════════════════════════════════════

CREATE OR ALTER PROCEDURE dbo.usp_API_CheckRateLimit
    @SystemName     NVARCHAR(100),
    @Environment    NVARCHAR(10) = 'PRD',
    @CanProceed     BIT OUTPUT,
    @WaitSeconds    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Now DATETIME2(3) = SYSUTCDATETIME();
    DECLARE @MaxPerMinute INT, @CurrentMinute INT, @LastResetMinute DATETIME2(3);
    DECLARE @IsThrottled BIT, @ThrottledUntil DATETIME2(3);
    
    -- Obter configuração
    SELECT 
        @MaxPerMinute = MaxRequestsPerMinute,
        @CurrentMinute = CurrentMinuteRequests,
        @LastResetMinute = LastResetMinute,
        @IsThrottled = IsThrottled,
        @ThrottledUntil = ThrottledUntil
    FROM dbo.API_RateLimitControl WITH (UPDLOCK, READPAST)
    WHERE SystemName = @SystemName
      AND Environment = @Environment;
    
    -- Se não existe, criar com valores padrão
    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.API_RateLimitControl (SystemName, Environment)
        VALUES (@SystemName, @Environment);
        
        SET @CanProceed = 1;
        SET @WaitSeconds = 0;
        RETURN;
    END
    
    -- Resetar contadores se passou 1 minuto
    IF DATEDIFF(SECOND, @LastResetMinute, @Now) >= 60
    BEGIN
        UPDATE dbo.API_RateLimitControl
        SET CurrentMinuteRequests = 0,
            LastResetMinute = @Now,
            IsThrottled = 0,
            ThrottledUntil = NULL,
            UpdatedUtc = @Now
        WHERE SystemName = @SystemName
          AND Environment = @Environment;
        
        SET @CurrentMinute = 0;
        SET @IsThrottled = 0;
    END
    
    -- Verificar throttling ativo
    IF @IsThrottled = 1 AND @ThrottledUntil > @Now
    BEGIN
        SET @CanProceed = 0;
        SET @WaitSeconds = DATEDIFF(SECOND, @Now, @ThrottledUntil);
        RETURN;
    END
    
    -- Verificar limite por minuto
    IF @CurrentMinute >= @MaxPerMinute
    BEGIN
        SET @CanProceed = 0;
        SET @WaitSeconds = 60 - DATEDIFF(SECOND, @LastResetMinute, @Now);
        
        UPDATE dbo.API_RateLimitControl
        SET IsThrottled = 1,
            ThrottledUntil = DATEADD(SECOND, @WaitSeconds, @Now),
            ThrottleReason = 'Max requests per minute exceeded',
            UpdatedUtc = @Now
        WHERE SystemName = @SystemName
          AND Environment = @Environment;
    END
    ELSE
    BEGIN
        SET @CanProceed = 1;
        SET @WaitSeconds = 0;
        
        -- Incrementar contador
        UPDATE dbo.API_RateLimitControl
        SET CurrentMinuteRequests = CurrentMinuteRequests + 1,
            CurrentHourRequests = CurrentHourRequests + 1,
            CurrentDayRequests = CurrentDayRequests + 1,
            UpdatedUtc = @Now
        WHERE SystemName = @SystemName
          AND Environment = @Environment;
    END
END
GO

-- ═══════════════════════════════════════════════════════════════════
-- SP: usp_API_CleanupRawPayloads
-- Propósito: Limpeza por retenção (em lotes para não inchar log)
-- ═══════════════════════════════════════════════════════════════════

CREATE OR ALTER PROCEDURE dbo.usp_API_CleanupRawPayloads
    @RetentionDays  INT = 90,
    @BatchSize      INT = 5000,
    @Environment    NVARCHAR(10) = NULL,
    @SystemName     NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @cutoff DATETIME2(3) = DATEADD(DAY, -@RetentionDays, SYSUTCDATETIME());
    DECLARE @TotalDeleted INT = 0;
    DECLARE @BatchDeleted INT;
    
    PRINT 'Iniciando limpeza de payloads com mais de ' + CAST(@RetentionDays AS VARCHAR) + ' dias...';
    
    WHILE 1=1
    BEGIN
        DELETE TOP (@BatchSize) 
        FROM dbo.API_RawPayloads WITH (READPAST)
        WHERE CollectedUtc < @cutoff
          AND (@Environment IS NULL OR Environment = @Environment)
          AND (@SystemName IS NULL OR SystemName = @SystemName)
          AND ProcessingStatus = 'PROCESSED'; -- Só remove processados
        
        SET @BatchDeleted = @@ROWCOUNT;
        SET @TotalDeleted = @TotalDeleted + @BatchDeleted;
        
        IF @BatchDeleted < @BatchSize BREAK;
        
        WAITFOR DELAY '00:00:01'; -- Pausa para não saturar log
    END
    
    PRINT 'Limpeza concluída: ' + CAST(@TotalDeleted AS VARCHAR) + ' registros removidos.';
END
GO

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '✓ Stored Procedures criadas com sucesso!';
PRINT '═══════════════════════════════════════════════════════════════';
GO
