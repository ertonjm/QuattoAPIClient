/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Complete Database Setup Script
Versão: 1.0.0
Projeto: SESC-DF Data Warehouse
Autor: Erton Miranda / Quatto Consultoria
Data: Fevereiro 2026

CONTEÚDO:
1. Tabelas (Watermarks, RawPayloads, ExecutionLog, RateLimitControl)
2. Stored Procedures (GetWatermark, UpdateWatermark, CheckRateLimit, Cleanup)
3. Índices otimizados
4. Dashboard Queries

INSTRUÇÕES:
- Ajuste o nome do database conforme seu ambiente
- Execute em ordem (tabelas → procedures → índices)
- Valide a execução com as queries de teste no final

═══════════════════════════════════════════════════════════════════
*/

-- ═══════════════════════════════════════════════════════════════
-- CONFIGURAÇÃO INICIAL
-- ═══════════════════════════════════════════════════════════════

USE [SESCDF_DW]; -- ⚠️ AJUSTAR CONFORME SEU AMBIENTE
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT 'Quatto API Client v1.0 - Database Setup';
PRINT 'Iniciando em: ' + CONVERT(VARCHAR, SYSDATETIME(), 120);
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';
GO

-- ═══════════════════════════════════════════════════════════════
-- SEÇÃO 1: TABELAS
-- ═══════════════════════════════════════════════════════════════

PRINT '[1/4] Criando tabelas...';
GO

-- ───────────────────────────────────────────────────────────────
-- TABELA: dbo.API_Watermarks
-- ───────────────────────────────────────────────────────────────

IF OBJECT_ID('dbo.API_Watermarks', 'U') IS NULL
BEGIN
    PRINT '  → API_Watermarks';
    
    CREATE TABLE dbo.API_Watermarks
    (
        WatermarkID             INT IDENTITY(1,1) NOT NULL,
        SystemName              NVARCHAR(100) NOT NULL,
        Endpoint                NVARCHAR(200) NOT NULL,
        Environment             NVARCHAR(10) NOT NULL DEFAULT('PRD'),
        
        LastWatermark           DATETIMEOFFSET(3) NULL,
        LastWatermarkType       NVARCHAR(20) NULL,
        LastRunUtc              DATETIME2(3) NULL,
        LastStatus              NVARCHAR(50) NULL,
        
        TotalRecordsExtracted   BIGINT NOT NULL DEFAULT(0),
        TotalPagesExtracted     INT NOT NULL DEFAULT(0),
        TotalBytesExtracted     BIGINT NOT NULL DEFAULT(0),
        
        Notes                   NVARCHAR(4000) NULL,
        CreatedUtc              DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedUtc              DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
        RowVersion              ROWVERSION NOT NULL,
        
        CONSTRAINT PK_API_Watermarks PRIMARY KEY CLUSTERED (WatermarkID),
        CONSTRAINT UQ_API_Watermarks_SystemEndpointEnv 
            UNIQUE (SystemName, Endpoint, Environment)
    );
    
    CREATE NONCLUSTERED INDEX IX_API_Watermarks_LastRunUtc
        ON dbo.API_Watermarks(LastRunUtc DESC)
        INCLUDE (SystemName, Endpoint, LastWatermark, LastStatus);
    
    CREATE NONCLUSTERED INDEX IX_API_Watermarks_Status
        ON dbo.API_Watermarks(LastStatus, UpdatedUtc)
        WHERE LastStatus IN ('Error', 'Partial');
    
    PRINT '  ✓ API_Watermarks criada';
END
ELSE
    PRINT '  ⚠ API_Watermarks já existe (skip)';
GO

-- ───────────────────────────────────────────────────────────────
-- TABELA: dbo.API_RawPayloads
-- ───────────────────────────────────────────────────────────────

IF OBJECT_ID('dbo.API_RawPayloads', 'U') IS NULL
BEGIN
    PRINT '  → API_RawPayloads';
    
    CREATE TABLE dbo.API_RawPayloads
    (
        PayloadID               BIGINT IDENTITY(1,1) NOT NULL,
        CorrelationID           UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
        
        SystemName              NVARCHAR(100) NOT NULL,
        Endpoint                NVARCHAR(200) NOT NULL,
        Environment             NVARCHAR(10) NOT NULL DEFAULT('PRD'),
        
        HttpMethod              NVARCHAR(10) NOT NULL DEFAULT('GET'),
        RequestUrl              NVARCHAR(2000) NOT NULL,
        RequestHeaders          NVARCHAR(MAX) NULL,
        
        ResponseStatusCode      INT NOT NULL,
        ResponseHeaders         NVARCHAR(MAX) NULL,
        ResponseBodyGzip        VARBINARY(MAX) NOT NULL,
        ResponseBodyHash        CHAR(64) NOT NULL,
        ResponseSizeBytes       INT NOT NULL,
        
        PageNumber              INT NULL,
        PageSize                INT NULL,
        RecordCount             INT NULL,
        
        ElapsedMs               INT NULL,
        
        CollectedUtc            DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
        ProcessedUtc            DATETIME2(3) NULL,
        ProcessingStatus        NVARCHAR(20) NOT NULL DEFAULT('RAW'),
        ErrorMessage            NVARCHAR(MAX) NULL,
        
        CONSTRAINT PK_API_RawPayloads PRIMARY KEY CLUSTERED (PayloadID),
        CONSTRAINT CK_API_RawPayloads_Status 
            CHECK (ProcessingStatus IN ('RAW', 'PROCESSED', 'ERROR', 'QUARANTINE')),
        CONSTRAINT CK_API_RawPayloads_StatusCode
            CHECK (ResponseStatusCode BETWEEN 100 AND 599)
    );
    
    CREATE NONCLUSTERED INDEX IX_API_RawPayloads_SystemEndpoint
        ON dbo.API_RawPayloads(SystemName, Endpoint, CollectedUtc DESC)
        INCLUDE (ProcessingStatus, RecordCount, ResponseSizeBytes);
    
    CREATE NONCLUSTERED INDEX IX_API_RawPayloads_CorrelationID
        ON dbo.API_RawPayloads(CorrelationID)
        INCLUDE (SystemName, Endpoint, ResponseStatusCode);
    
    CREATE NONCLUSTERED INDEX IX_API_RawPayloads_Status
        ON dbo.API_RawPayloads(ProcessingStatus, CollectedUtc)
        WHERE ProcessingStatus = 'RAW';
    
    CREATE NONCLUSTERED INDEX IX_API_RawPayloads_Hash
        ON dbo.API_RawPayloads(ResponseBodyHash)
        INCLUDE (PayloadID, CollectedUtc);
    
    PRINT '  ✓ API_RawPayloads criada';
END
ELSE
    PRINT '  ⚠ API_RawPayloads já existe (skip)';
GO

-- ───────────────────────────────────────────────────────────────
-- TABELA: dbo.API_ExecutionLog
-- ───────────────────────────────────────────────────────────────

IF OBJECT_ID('dbo.API_ExecutionLog', 'U') IS NULL
BEGIN
    PRINT '  → API_ExecutionLog';
    
    CREATE TABLE dbo.API_ExecutionLog
    (
        LogID                   BIGINT IDENTITY(1,1) NOT NULL,
        CorrelationID           UNIQUEIDENTIFIER NOT NULL,
        
        PackageName             NVARCHAR(200) NOT NULL,
        TaskName                NVARCHAR(200) NOT NULL,
        ExecutionID             BIGINT NULL,
        
        SystemName              NVARCHAR(100) NOT NULL,
        Endpoint                NVARCHAR(200) NOT NULL,
        Environment             NVARCHAR(10) NOT NULL,
        
        ExecutionStartedUtc     DATETIME2(3) NOT NULL,
        ExecutionFinishedUtc    DATETIME2(3) NULL,
        DurationMs              INT NULL,
        
        TotalRequests           INT NOT NULL DEFAULT(0),
        SuccessfulRequests      INT NOT NULL DEFAULT(0),
        FailedRequests          INT NOT NULL DEFAULT(0),
        RetriedRequests         INT NOT NULL DEFAULT(0),
        ThrottledRequests       INT NOT NULL DEFAULT(0),
        
        TotalPages              INT NULL,
        TotalRecords            BIGINT NULL,
        TotalBytesReceived      BIGINT NULL,
        
        AvgLatencyMs            INT NULL,
        MinLatencyMs            INT NULL,
        MaxLatencyMs            INT NULL,
        
        WatermarkBefore         NVARCHAR(100) NULL,
        WatermarkAfter          NVARCHAR(100) NULL,
        
        Status                  NVARCHAR(20) NOT NULL,
        ErrorMessage            NVARCHAR(MAX) NULL,
        
        CONSTRAINT PK_API_ExecutionLog PRIMARY KEY CLUSTERED (LogID),
        CONSTRAINT CK_API_ExecutionLog_Status
            CHECK (Status IN ('SUCCESS', 'FAILED', 'PARTIAL', 'RUNNING'))
    );
    
    CREATE NONCLUSTERED INDEX IX_API_ExecutionLog_Correlation
        ON dbo.API_ExecutionLog(CorrelationID);
    
    CREATE NONCLUSTERED INDEX IX_API_ExecutionLog_Execution
        ON dbo.API_ExecutionLog(SystemName, Endpoint, ExecutionStartedUtc DESC)
        INCLUDE (Status, TotalRecords, DurationMs);
    
    CREATE NONCLUSTERED INDEX IX_API_ExecutionLog_Status
        ON dbo.API_ExecutionLog(Status, ExecutionStartedUtc DESC)
        WHERE Status IN ('FAILED', 'PARTIAL');
    
    PRINT '  ✓ API_ExecutionLog criada';
END
ELSE
    PRINT '  ⚠ API_ExecutionLog já existe (skip)';
GO

-- ───────────────────────────────────────────────────────────────
-- TABELA: dbo.API_RateLimitControl
-- ───────────────────────────────────────────────────────────────

IF OBJECT_ID('dbo.API_RateLimitControl', 'U') IS NULL
BEGIN
    PRINT '  → API_RateLimitControl';
    
    CREATE TABLE dbo.API_RateLimitControl
    (
        RateLimitID             INT IDENTITY(1,1) NOT NULL,
        SystemName              NVARCHAR(100) NOT NULL,
        Environment             NVARCHAR(10) NOT NULL DEFAULT('PRD'),
        
        MaxRequestsPerMinute    INT NOT NULL DEFAULT(60),
        MaxRequestsPerHour      INT NOT NULL DEFAULT(1000),
        MaxRequestsPerDay       INT NOT NULL DEFAULT(10000),
        
        CurrentMinuteRequests   INT NOT NULL DEFAULT(0),
        CurrentHourRequests     INT NOT NULL DEFAULT(0),
        CurrentDayRequests      INT NOT NULL DEFAULT(0),
        
        LastResetMinute         DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
        LastResetHour           DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
        LastResetDay            DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
        
        IsThrottled             BIT NOT NULL DEFAULT(0),
        ThrottledUntil          DATETIME2(3) NULL,
        ThrottleReason          NVARCHAR(200) NULL,
        
        UpdatedUtc              DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
        
        CONSTRAINT PK_API_RateLimitControl PRIMARY KEY CLUSTERED (RateLimitID),
        CONSTRAINT UQ_API_RateLimitControl_SystemEnv 
            UNIQUE (SystemName, Environment)
    );
    
    CREATE NONCLUSTERED INDEX IX_API_RateLimitControl_Throttled
        ON dbo.API_RateLimitControl(IsThrottled, ThrottledUntil)
        WHERE IsThrottled = 1;
    
    PRINT '  ✓ API_RateLimitControl criada';
END
ELSE
    PRINT '  ⚠ API_RateLimitControl já existe (skip)';
GO

PRINT '';
PRINT '✓ Todas as tabelas foram processadas';
PRINT '';
GO

-- ═══════════════════════════════════════════════════════════════
-- SEÇÃO 2: STORED PROCEDURES
-- ═══════════════════════════════════════════════════════════════

PRINT '[2/4] Criando stored procedures...';
GO

-- ───────────────────────────────────────────────────────────────
-- SP: usp_API_GetWatermark
-- ───────────────────────────────────────────────────────────────

PRINT '  → usp_API_GetWatermark';
GO

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

-- ───────────────────────────────────────────────────────────────
-- SP: usp_API_UpdateWatermark
-- ───────────────────────────────────────────────────────────────

PRINT '  → usp_API_UpdateWatermark';
GO

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

-- ───────────────────────────────────────────────────────────────
-- SP: usp_API_CheckRateLimit
-- ───────────────────────────────────────────────────────────────

PRINT '  → usp_API_CheckRateLimit';
GO

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
    
    SELECT 
        @MaxPerMinute = MaxRequestsPerMinute,
        @CurrentMinute = CurrentMinuteRequests,
        @LastResetMinute = LastResetMinute,
        @IsThrottled = IsThrottled,
        @ThrottledUntil = ThrottledUntil
    FROM dbo.API_RateLimitControl WITH (UPDLOCK, READPAST)
    WHERE SystemName = @SystemName
      AND Environment = @Environment;
    
    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.API_RateLimitControl (SystemName, Environment)
        VALUES (@SystemName, @Environment);
        
        SET @CanProceed = 1;
        SET @WaitSeconds = 0;
        RETURN;
    END
    
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
    
    IF @IsThrottled = 1 AND @ThrottledUntil > @Now
    BEGIN
        SET @CanProceed = 0;
        SET @WaitSeconds = DATEDIFF(SECOND, @Now, @ThrottledUntil);
        RETURN;
    END
    
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

-- ───────────────────────────────────────────────────────────────
-- SP: usp_API_CleanupRawPayloads
-- ───────────────────────────────────────────────────────────────

PRINT '  → usp_API_CleanupRawPayloads';
GO

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
          AND ProcessingStatus = 'PROCESSED';
        
        SET @BatchDeleted = @@ROWCOUNT;
        SET @TotalDeleted = @TotalDeleted + @BatchDeleted;
        
        IF @BatchDeleted < @BatchSize BREAK;
        
        WAITFOR DELAY '00:00:01';
    END
    
    PRINT 'Limpeza concluída: ' + CAST(@TotalDeleted AS VARCHAR) + ' registros removidos.';
END
GO

PRINT '';
PRINT '✓ Stored procedures criadas';
PRINT '';
GO

-- ═══════════════════════════════════════════════════════════════
-- SEÇÃO 3: DADOS DE TESTE (OPCIONAL)
-- ═══════════════════════════════════════════════════════════════

PRINT '[3/4] Dados de teste (opcional)...';
GO

-- Inserir sistema exemplo
IF NOT EXISTS (SELECT 1 FROM dbo.API_Watermarks WHERE SystemName = 'Gladium' AND Endpoint = '/v1/orders')
BEGIN
    PRINT '  → Inserindo watermark exemplo (Gladium)';
    
    INSERT INTO dbo.API_Watermarks 
    (SystemName, Endpoint, Environment, LastWatermark, LastRunUtc, LastStatus)
    VALUES
    ('Gladium', '/v1/orders', 'PRD', '2026-01-01T00:00:00Z', SYSUTCDATETIME(), 'SUCCESS');
END

IF NOT EXISTS (SELECT 1 FROM dbo.API_RateLimitControl WHERE SystemName = 'Gladium')
BEGIN
    PRINT '  → Inserindo rate limit exemplo (Gladium)';
    
    INSERT INTO dbo.API_RateLimitControl
    (SystemName, Environment, MaxRequestsPerMinute, MaxRequestsPerHour, MaxRequestsPerDay)
    VALUES
    ('Gladium', 'PRD', 120, 5000, 50000);
END

PRINT '✓ Dados de teste processados';
PRINT '';
GO

-- ═══════════════════════════════════════════════════════════════
-- SEÇÃO 4: VALIDAÇÃO
-- ═══════════════════════════════════════════════════════════════

PRINT '[4/4] Validando instalação...';
GO

DECLARE @TablesCount INT;
DECLARE @ProceduresCount INT;

SELECT @TablesCount = COUNT(*)
FROM sys.tables
WHERE name IN ('API_Watermarks', 'API_RawPayloads', 'API_ExecutionLog', 'API_RateLimitControl');

SELECT @ProceduresCount = COUNT(*)
FROM sys.procedures
WHERE name IN ('usp_API_GetWatermark', 'usp_API_UpdateWatermark', 'usp_API_CheckRateLimit', 'usp_API_CleanupRawPayloads');

PRINT '';
PRINT '══════════════════════════════════════════════════════════════';
PRINT 'RESUMO DA INSTALAÇÃO';
PRINT '══════════════════════════════════════════════════════════════';
PRINT 'Tabelas criadas: ' + CAST(@TablesCount AS VARCHAR) + '/4';
PRINT 'Procedures criadas: ' + CAST(@ProceduresCount AS VARCHAR) + '/4';
PRINT '';

IF @TablesCount = 4 AND @ProceduresCount = 4
BEGIN
    PRINT '✓✓✓ INSTALAÇÃO CONCLUÍDA COM SUCESSO! ✓✓✓';
END
ELSE
BEGIN
    PRINT '⚠⚠⚠ INSTALAÇÃO INCOMPLETA - VERIFICAR ERROS ⚠⚠⚠';
END

PRINT '';
PRINT 'Finalizado em: ' + CONVERT(VARCHAR, SYSDATETIME(), 120);
PRINT '══════════════════════════════════════════════════════════════';
PRINT '';
GO