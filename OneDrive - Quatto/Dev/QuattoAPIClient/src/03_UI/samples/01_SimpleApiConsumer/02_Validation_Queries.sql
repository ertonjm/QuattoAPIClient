-- ============================================
-- SAMPLE 1: SimpleApiConsumer
-- GitHub API Validation & Test Queries
-- ============================================

USE [QuattoSamples]
GO

-- Query 1: Check if tables exist and are empty
PRINT '========================================='
PRINT 'TABLE STRUCTURE VERIFICATION'
PRINT '========================================='

SELECT 
    TABLE_NAME,
    COUNT(*) as RecordCount
FROM INFORMATION_SCHEMA.TABLES t
LEFT JOIN sys.dm_db_partition_stats ps ON OBJECT_NAME(ps.object_id) = t.TABLE_NAME
WHERE TABLE_SCHEMA = 'dbo'
GROUP BY TABLE_NAME
ORDER BY TABLE_NAME

PRINT ''

-- Query 2: Display column information for GitHubRepositories
PRINT 'COLUMN STRUCTURE - [GitHubRepositories]:'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'GitHubRepositories'
ORDER BY ORDINAL_POSITION

PRINT ''

-- Query 3: Check data after load (will be empty initially)
PRINT '========================================='
PRINT 'DATA VERIFICATION'
PRINT '========================================='

DECLARE @Count INT = (SELECT COUNT(*) FROM [dbo].[GitHubRepositories])
PRINT 'Records in [GitHubRepositories]: ' + CAST(@Count AS VARCHAR(10))

PRINT ''

-- Query 4: Show execution logs
PRINT 'RECENT EXECUTION LOGS:'
IF EXISTS(SELECT * FROM [dbo].[ExecutionLog])
BEGIN
    SELECT TOP 10 
        [LogId],
        [PackageName],
        [ExecutionStatus],
        [StartTime],
        [EndTime],
        [RecordsLoaded],
        DATEDIFF(SECOND, [StartTime], [EndTime]) as 'Duration_Seconds'
    FROM [dbo].[ExecutionLog]
    ORDER BY [LogId] DESC
END
ELSE
BEGIN
    PRINT 'No execution logs yet. Package will create logs after first run.'
END

PRINT ''

-- Query 5: Repository statistics (after load)
IF EXISTS(SELECT * FROM [dbo].[GitHubRepositories] WHERE [Stars] > 0)
BEGIN
    PRINT '========================================='
    PRINT 'REPOSITORY STATISTICS'
    PRINT '========================================='
    
    SELECT 
        'Total Repositories' as Metric,
        COUNT(*) as Value
    FROM [dbo].[GitHubRepositories]
    
    UNION ALL
    
    SELECT 
        'Repositories with Stars',
        COUNT(*)
    FROM [dbo].[GitHubRepositories]
    WHERE [Stars] > 0
    
    UNION ALL
    
    SELECT 
        'Average Stars',
        CAST(AVG(CAST([Stars] AS DECIMAL))) AS INT)
    FROM [dbo].[GitHubRepositories]
    
    UNION ALL
    
    SELECT 
        'Max Stars',
        MAX([Stars])
    FROM [dbo].[GitHubRepositories]
    
    UNION ALL
    
    SELECT 
        'Languages Found',
        COUNT(DISTINCT [Language])
    FROM [dbo].[GitHubRepositories]
    WHERE [Language] IS NOT NULL
END
ELSE
BEGIN
    PRINT 'No data yet. Run SSIS package first.'
END

PRINT ''

-- Query 6: Language distribution
IF EXISTS(SELECT * FROM [dbo].[GitHubRepositories] WHERE [Language] IS NOT NULL)
BEGIN
    PRINT '========================================='
    PRINT 'LANGUAGE DISTRIBUTION'
    PRINT '========================================='
    
    SELECT TOP 10
        [Language],
        COUNT(*) as Count,
        AVG(CAST([Stars] AS INT)) as AvgStars
    FROM [dbo].[GitHubRepositories]
    WHERE [Language] IS NOT NULL
    GROUP BY [Language]
    ORDER BY Count DESC
END

PRINT ''
PRINT '========================================='
PRINT 'Verification Complete'
PRINT '========================================='
