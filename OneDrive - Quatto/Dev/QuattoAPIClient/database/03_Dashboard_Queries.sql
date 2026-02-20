/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Dashboard Queries
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
Data: Fevereiro 2026

DESCRIÇÃO:
Queries para monitoramento e análise do Quatto API Client.
Ideal para dashboards Power BI ou rotinas de alerta.

CONTEÚDO:
1. KPIs Executivos (últimas 24h)
2. Performance por Sistema
3. Alertas e Anomalias
4. Tendências (7 dias)
5. Raw Payloads Analytics
6. Watermark Status
7. Rate Limiting Status
8. Retry Analysis
9. Error Patterns

═══════════════════════════════════════════════════════════════════
*/

USE [SESCDF_DW];
GO

SET NOCOUNT ON;
GO

-- ═══════════════════════════════════════════════════════════════
-- QUERY 1: KPIs EXECUTIVOS (ÚLTIMAS 24H)
-- ═══════════════════════════════════════════════════════════════

SELECT 
    -- Contadores
    COUNT(DISTINCT SystemName) AS TotalSystems,
    COUNT(*) AS TotalExecutions,
    SUM(TotalRecords) AS TotalRecordsExtracted,
    
    -- Taxa de Sucesso
    SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) AS SuccessfulExecutions,
    SUM(CASE WHEN Status = 'FAILED' THEN 1 ELSE 0 END) AS FailedExecutions,
    CAST(SUM(CASE WHEN Status = 'SUCCESS' THEN 1.0 ELSE 0 END) * 100 / COUNT(*) AS DECIMAL(5,2)) AS SuccessRate_Pct,
    
    -- Performance
    AVG(DurationMs) / 1000.0 AS AvgDuration_Sec,
    MAX(DurationMs) / 1000.0 AS MaxDuration_Sec,
    AVG(AvgLatencyMs) AS AvgLatency_Ms,
    
    -- Requests
    SUM(TotalRequests) AS TotalRequests,
    SUM(RetriedRequests) AS TotalRetries,
    SUM(ThrottledRequests) AS TotalThrottled,
    
    -- Status Geral
    CASE 
        WHEN SUM(CASE WHEN Status = 'FAILED' THEN 1 ELSE 0 END) = 0 THEN '🟢 HEALTHY'
        WHEN CAST(SUM(CASE WHEN Status = 'SUCCESS' THEN 1.0 ELSE 0 END) * 100 / COUNT(*) AS DECIMAL(5,2)) >= 95 THEN '🟡 WARNING'
        ELSE '🔴 CRITICAL'
    END AS SystemHealth
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(HOUR, -24, SYSUTCDATETIME());

-- ═══════════════════════════════════════════════════════════════
-- QUERY 2: PERFORMANCE POR SISTEMA
-- ═══════════════════════════════════════════════════════════════

SELECT 
    SystemName,
    Endpoint,
    COUNT(*) AS Executions,
    SUM(TotalRecords) AS TotalRecords,
    AVG(TotalRecords) AS AvgRecordsPerExecution,
    
    -- Duração
    AVG(DurationMs) / 1000.0 AS AvgDuration_Sec,
    MAX(DurationMs) / 1000.0 AS MaxDuration_Sec,
    
    -- Latência
    AVG(AvgLatencyMs) AS AvgLatency_Ms,
    AVG(MaxLatencyMs) AS MaxLatency_Ms,
    
    -- Taxa de Sucesso
    CAST(SUM(CASE WHEN Status = 'SUCCESS' THEN 1.0 ELSE 0 END) * 100 / COUNT(*) AS DECIMAL(5,2)) AS SuccessRate_Pct,
    
    -- Retries e Throttling
    SUM(RetriedRequests) AS TotalRetries,
    SUM(ThrottledRequests) AS TotalThrottled,
    
    -- Última Execução
    MAX(ExecutionStartedUtc) AS LastExecutionUtc,
    DATEDIFF(MINUTE, MAX(ExecutionStartedUtc), SYSUTCDATETIME()) AS MinutesSinceLastRun
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, SYSUTCDATETIME())
GROUP BY SystemName, Endpoint
ORDER BY TotalRecords DESC;

-- ═══════════════════════════════════════════════════════════════
-- QUERY 3: ALERTAS E ANOMALIAS
-- ═══════════════════════════════════════════════════════════════

WITH SystemBaseline AS (
    SELECT 
        SystemName,
        Endpoint,
        AVG(DurationMs) AS AvgDuration,
        STDEV(DurationMs) AS StdDevDuration
    FROM dbo.API_ExecutionLog
    WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, SYSUTCDATETIME())
      AND Status = 'SUCCESS'
    GROUP BY SystemName, Endpoint
)
SELECT 
    e.SystemName,
    e.Endpoint,
    e.ExecutionStartedUtc,
    e.DurationMs / 1000.0 AS Duration_Sec,
    b.AvgDuration / 1000.0 AS BaselineAvg_Sec,
    
    -- Detecção de Anomalia
    CASE 
        WHEN e.DurationMs > b.AvgDuration + (3 * b.StdDevDuration) THEN '🔴 ANOMALY: Duration 3x StdDev'
        WHEN e.Status = 'FAILED' THEN '🔴 FAILURE'
        WHEN e.ThrottledRequests > 10 THEN '🟡 HIGH THROTTLING'
        WHEN e.RetriedRequests > e.TotalRequests * 0.5 THEN '🟡 HIGH RETRY RATE'
        ELSE '🟢 NORMAL'
    END AS AlertLevel,
    
    e.Status,
    e.ErrorMessage
FROM dbo.API_ExecutionLog e
INNER JOIN SystemBaseline b
    ON e.SystemName = b.SystemName
   AND e.Endpoint = b.Endpoint
WHERE e.ExecutionStartedUtc >= DATEADD(DAY, -1, SYSUTCDATETIME())
  AND (
      e.DurationMs > b.AvgDuration + (3 * b.StdDevDuration)
      OR e.Status = 'FAILED'
      OR e.ThrottledRequests > 10
      OR e.RetriedRequests > e.TotalRequests * 0.5
  )
ORDER BY e.ExecutionStartedUtc DESC;

-- ═══════════════════════════════════════════════════════════════
-- QUERY 4: TENDÊNCIAS (7 DIAS)
-- ═══════════════════════════════════════════════════════════════

SELECT 
    CAST(ExecutionStartedUtc AS DATE) AS ExecutionDate,
    SystemName,
    COUNT(*) AS Executions,
    SUM(TotalRecords) AS RecordsExtracted,
    AVG(DurationMs) / 1000.0 AS AvgDuration_Sec,
    AVG(AvgLatencyMs) AS AvgLatency_Ms,
    CAST(SUM(CASE WHEN Status = 'SUCCESS' THEN 1.0 ELSE 0 END) * 100 / COUNT(*) AS DECIMAL(5,2)) AS SuccessRate_Pct
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, SYSUTCDATETIME())
GROUP BY CAST(ExecutionStartedUtc AS DATE), SystemName
ORDER BY ExecutionDate DESC, SystemName;

-- ═══════════════════════════════════════════════════════════════
-- QUERY 5: RAW PAYLOADS ANALYTICS
-- ═══════════════════════════════════════════════════════════════

SELECT 
    SystemName,
    Endpoint,
    COUNT(*) AS TotalPayloads,
    SUM(ResponseSizeBytes) / 1024.0 / 1024.0 AS TotalSize_MB,
    AVG(ResponseSizeBytes) / 1024.0 AS AvgSize_KB,
    MAX(ResponseSizeBytes) / 1024.0 AS MaxSize_KB,
    
    -- Compressão
    SUM(ResponseSizeBytes) / 1024.0 / 1024.0 AS UncompressedSize_MB,
    SUM(DATALENGTH(ResponseBodyGzip)) / 1024.0 / 1024.0 AS CompressedSize_MB,
    CAST((1 - (SUM(CAST(DATALENGTH(ResponseBodyGzip) AS BIGINT)) * 1.0 / NULLIF(SUM(ResponseSizeBytes), 0))) * 100 AS DECIMAL(5,2)) AS CompressionRatio_Pct,
    
    -- Status HTTP
    COUNT(DISTINCT ResponseStatusCode) AS UniqueStatusCodes,
    SUM(CASE WHEN ResponseStatusCode = 200 THEN 1 ELSE 0 END) AS Status200_Count,
    SUM(CASE WHEN ResponseStatusCode >= 400 THEN 1 ELSE 0 END) AS Status4xx5xx_Count,
    
    -- Processamento
    SUM(CASE WHEN ProcessingStatus = 'RAW' THEN 1 ELSE 0 END) AS PendingProcessing,
    SUM(CASE WHEN ProcessingStatus = 'PROCESSED' THEN 1 ELSE 0 END) AS Processed,
    SUM(CASE WHEN ProcessingStatus = 'ERROR' THEN 1 ELSE 0 END) AS Errors
FROM dbo.API_RawPayloads
WHERE CollectedUtc >= DATEADD(DAY, -7, SYSUTCDATETIME())
GROUP BY SystemName, Endpoint
ORDER BY TotalSize_MB DESC;

-- ═══════════════════════════════════════════════════════════════
-- QUERY 6: WATERMARK STATUS
-- ═══════════════════════════════════════════════════════════════

SELECT 
    SystemName,
    Endpoint,
    Environment,
    LastWatermark,
    DATEDIFF(HOUR, LastRunUtc, SYSUTCDATETIME()) AS HoursSinceLastRun,
    LastStatus,
    TotalRecordsExtracted,
    TotalPagesExtracted,
    TotalBytesExtracted / 1024.0 / 1024.0 AS TotalExtracted_MB,
    
    -- Alerta de Staleness
    CASE 
        WHEN LastRunUtc IS NULL THEN '🔴 NEVER RUN'
        WHEN DATEDIFF(HOUR, LastRunUtc, SYSUTCDATETIME()) > 48 THEN '🔴 STALE (48h+)'
        WHEN DATEDIFF(HOUR, LastRunUtc, SYSUTCDATETIME()) > 24 THEN '🟡 STALE (24h+)'
        WHEN LastStatus = 'FAILED' THEN '🔴 LAST RUN FAILED'
        ELSE '🟢 CURRENT'
    END AS Status,
    
    UpdatedUtc
FROM dbo.API_Watermarks
ORDER BY 
    CASE 
        WHEN LastRunUtc IS NULL THEN 1
        WHEN DATEDIFF(HOUR, LastRunUtc, SYSUTCDATETIME()) > 48 THEN 2
        WHEN LastStatus = 'FAILED' THEN 3
        ELSE 4
    END,
    SystemName;

-- ═══════════════════════════════════════════════════════════════
-- QUERY 7: RATE LIMITING STATUS
-- ═══════════════════════════════════════════════════════════════

SELECT 
    SystemName,
    Environment,
    
    -- Limites Configurados
    MaxRequestsPerMinute,
    MaxRequestsPerHour,
    MaxRequestsPerDay,
    
    -- Uso Atual
    CurrentMinuteRequests,
    CurrentHourRequests,
    CurrentDayRequests,
    
    -- % de Uso
    CAST(CurrentMinuteRequests * 100.0 / NULLIF(MaxRequestsPerMinute, 0) AS DECIMAL(5,2)) AS MinuteUsage_Pct,
    CAST(CurrentHourRequests * 100.0 / NULLIF(MaxRequestsPerHour, 0) AS DECIMAL(5,2)) AS HourUsage_Pct,
    CAST(CurrentDayRequests * 100.0 / NULLIF(MaxRequestsPerDay, 0) AS DECIMAL(5,2)) AS DayUsage_Pct,
    
    -- Throttling Ativo
    IsThrottled,
    ThrottledUntil,
    DATEDIFF(SECOND, SYSUTCDATETIME(), ThrottledUntil) AS ThrottleRemaining_Sec,
    ThrottleReason,
    
    -- Status
    CASE 
        WHEN IsThrottled = 1 THEN '🔴 THROTTLED'
        WHEN CurrentMinuteRequests * 100.0 / NULLIF(MaxRequestsPerMinute, 0) > 90 THEN '🟡 HIGH USAGE'
        ELSE '🟢 NORMAL'
    END AS Status
FROM dbo.API_RateLimitControl
ORDER BY 
    CASE WHEN IsThrottled = 1 THEN 1 ELSE 2 END,
    MinuteUsage_Pct DESC;

-- ═══════════════════════════════════════════════════════════════
-- QUERY 8: RETRY ANALYSIS
-- ═══════════════════════════════════════════════════════════════

SELECT 
    SystemName,
    Endpoint,
    COUNT(*) AS Executions,
    SUM(TotalRequests) AS TotalRequests,
    SUM(RetriedRequests) AS TotalRetries,
    CAST(SUM(RetriedRequests) * 100.0 / NULLIF(SUM(TotalRequests), 0) AS DECIMAL(5,2)) AS RetryRate_Pct,
    
    -- Distribuição de Retries
    AVG(CAST(RetriedRequests AS FLOAT) / NULLIF(TotalRequests, 0)) AS AvgRetryRatio,
    MAX(CAST(RetriedRequests AS FLOAT) / NULLIF(TotalRequests, 0)) AS MaxRetryRatio,
    
    -- Análise de Falhas
    SUM(CASE WHEN Status = 'FAILED' AND RetriedRequests = TotalRequests THEN 1 ELSE 0 END) AS FailedAfterMaxRetries,
    
    -- Status
    CASE 
        WHEN CAST(SUM(RetriedRequests) * 100.0 / NULLIF(SUM(TotalRequests), 0) AS DECIMAL(5,2)) > 50 THEN '🔴 HIGH RETRY RATE'
        WHEN CAST(SUM(RetriedRequests) * 100.0 / NULLIF(SUM(TotalRequests), 0) AS DECIMAL(5,2)) > 20 THEN '🟡 MODERATE RETRIES'
        ELSE '🟢 NORMAL'
    END AS Status
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, SYSUTCDATETIME())
GROUP BY SystemName, Endpoint
HAVING SUM(RetriedRequests) > 0
ORDER BY RetryRate_Pct DESC;

-- ═══════════════════════════════════════════════════════════════
-- QUERY 9: ERROR PATTERNS
-- ═══════════════════════════════════════════════════════════════

WITH ErrorCategories AS (
    SELECT 
        SystemName,
        Endpoint,
        ErrorMessage,
        CASE 
            WHEN ErrorMessage LIKE '%401%' OR ErrorMessage LIKE '%Unauthorized%' THEN 'AUTH_ERROR'
            WHEN ErrorMessage LIKE '%429%' OR ErrorMessage LIKE '%Too Many Requests%' THEN 'RATE_LIMIT'
            WHEN ErrorMessage LIKE '%timeout%' OR ErrorMessage LIKE '%TaskCanceled%' THEN 'TIMEOUT'
            WHEN ErrorMessage LIKE '%5__' OR ErrorMessage LIKE '%Server Error%' THEN 'SERVER_ERROR'
            WHEN ErrorMessage LIKE '%404%' OR ErrorMessage LIKE '%Not Found%' THEN 'NOT_FOUND'
            WHEN ErrorMessage LIKE '%Connection%' OR ErrorMessage LIKE '%Network%' THEN 'NETWORK_ERROR'
            ELSE 'OTHER'
        END AS ErrorCategory,
        ExecutionStartedUtc
    FROM dbo.API_ExecutionLog
    WHERE Status = 'FAILED'
      AND ExecutionStartedUtc >= DATEADD(DAY, -7, SYSUTCDATETIME())
)
SELECT 
    ErrorCategory,
    COUNT(*) AS ErrorCount,
    COUNT(DISTINCT SystemName) AS AffectedSystems,
    MIN(ExecutionStartedUtc) AS FirstOccurrence,
    MAX(ExecutionStartedUtc) AS LastOccurrence,
    
    -- Top Sistemas Afetados
    (
        SELECT TOP 3 SystemName + ' (' + CAST(COUNT(*) AS VARCHAR) + ')'
        FROM ErrorCategories e2
        WHERE e2.ErrorCategory = e1.ErrorCategory
        GROUP BY SystemName
        ORDER BY COUNT(*) DESC
        FOR XML PATH('')
    ) AS TopAffectedSystems
FROM ErrorCategories e1
GROUP BY ErrorCategory
ORDER BY ErrorCount DESC;

-- ═══════════════════════════════════════════════════════════════
-- FIM DO SCRIPT
-- ═══════════════════════════════════════════════════════════════

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT 'Dashboard Queries executadas com sucesso!';
PRINT 'Total de queries: 9';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';