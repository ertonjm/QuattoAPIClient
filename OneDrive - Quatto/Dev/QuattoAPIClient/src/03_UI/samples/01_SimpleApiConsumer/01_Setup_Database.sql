-- ============================================
-- SAMPLE 1: SimpleApiConsumer
-- Database Setup Script
-- GitHub API Integration
-- ============================================

-- Step 1: Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'QuattoSamples')
BEGIN
    CREATE DATABASE [QuattoSamples]
    PRINT 'Database [QuattoSamples] created successfully'
END
ELSE
BEGIN
    PRINT 'Database [QuattoSamples] already exists'
END

GO

USE [QuattoSamples]
GO

-- Step 2: Create Main Table for GitHub Repositories
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GitHubRepositories')
BEGIN
    CREATE TABLE [dbo].[GitHubRepositories](
        [RepositoryId] [bigint] PRIMARY KEY NOT NULL,
        [Name] [nvarchar](255) NOT NULL,
        [FullName] [nvarchar](255) NOT NULL,
        [Description] [nvarchar](max),
        [Url] [nvarchar](500),
        [Stars] [int] DEFAULT 0,
        [Forks] [int] DEFAULT 0,
        [Language] [nvarchar](50),
        [CreatedAt] [datetime2],
        [UpdatedAt] [datetime2],
        [LoadedAt] [datetime2] DEFAULT GETDATE(),
        [IsActive] [bit] DEFAULT 1
    )
    
    CREATE NONCLUSTERED INDEX IX_Name ON [dbo].[GitHubRepositories]([Name])
    CREATE NONCLUSTERED INDEX IX_Language ON [dbo].[GitHubRepositories]([Language])
    CREATE NONCLUSTERED INDEX IX_Stars ON [dbo].[GitHubRepositories]([Stars])
    
    PRINT 'Table [GitHubRepositories] created successfully with indexes'
END
ELSE
BEGIN
    PRINT 'Table [GitHubRepositories] already exists'
END

GO

-- Step 3: Create Audit Table for Execution Logs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExecutionLog')
BEGIN
    CREATE TABLE [dbo].[ExecutionLog](
        [LogId] [int] IDENTITY(1,1) PRIMARY KEY,
        [PackageName] [nvarchar](255),
        [ExecutionStatus] [nvarchar](50),
        [StartTime] [datetime2],
        [EndTime] [datetime2],
        [RecordsLoaded] [int],
        [ErrorMessage] [nvarchar](max),
        [ExecutionTime] [datetime2] DEFAULT GETDATE()
    )
    
    PRINT 'Table [ExecutionLog] created successfully'
END
ELSE
BEGIN
    PRINT 'Table [ExecutionLog] already exists'
END

GO

-- Step 4: Create Summary Statistics Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoadStatistics')
BEGIN
    CREATE TABLE [dbo].[LoadStatistics](
        [StatId] [int] IDENTITY(1,1) PRIMARY KEY,
        [LoadDate] [date] DEFAULT CAST(GETDATE() AS date),
        [TotalRepositories] [int],
        [RepositoriesByLanguage] [nvarchar](max),  -- JSON format
        [AverageStars] [decimal](10,2),
        [AverageForks] [decimal](10,2),
        [UpdatedAt] [datetime2] DEFAULT GETDATE()
    )
    
    PRINT 'Table [LoadStatistics] created successfully'
END
ELSE
BEGIN
    PRINT 'Table [LoadStatistics] already exists'
END

GO

-- Step 5: Verify table creation
PRINT '========================================='
PRINT 'Database Setup Summary:'
PRINT '========================================='
PRINT 'Database: QuattoSamples'
PRINT 'Tables created:'
SELECT 
    TABLE_NAME,
    TABLE_SCHEMA,
    'Created' AS Status
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_CATALOG = 'QuattoSamples'
ORDER BY TABLE_NAME

PRINT ''
PRINT '========================================='
PRINT 'Setup completed successfully!'
PRINT '========================================='
