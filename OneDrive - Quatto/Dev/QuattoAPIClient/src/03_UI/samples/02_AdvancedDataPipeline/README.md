# Sample 2: Advanced Data Pipeline

> Pipeline complexo com múltiplas APIs, transformações e incremental load

---

## 🎯 Objetivo

Criar um SSIS package de nível Enterprise que:
- ✅ Integra múltiplas APIs
- ✅ Executa transformações complexas
- ✅ Implementa incremental load com watermark
- ✅ Trata erros gracefully
- ✅ Registra todas as operações

**Dificuldade:** Intermediate  
**Tempo:** 2 horas  
**Conceitos:** Multi-source ETL, watermark, transformations, logging

---

## 📊 Arquitetura

```
┌─────────────────────────────────────────────────────┐
│         SSIS Data Integration Pipeline              │
├─────────────────────────────────────────────────────┤
│                                                     │
│  GitHub API  ──┐                                   │
│                ├──→ Corporate API Source           │
│  GitLab API  ──┤        ↓                          │
│                ├──→ [JSON Data]                    │
│  Bitbucket ────┘        ↓                          │
│                  Data Transformation               │
│                        ↓                            │
│                  Watermark Check                   │
│                        ↓                            │
│              SQL Server Destination                │
│                        ↓                            │
│              Update Watermark Table                │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## 🗄️ Database Setup

### Tabelas Principais

```sql
USE [QuattoSamples]
GO

-- 1. Repositories from multiple sources
CREATE TABLE [dbo].[Repositories](
    [RepositoryId] [nvarchar](100) PRIMARY KEY,
    [Source] [nvarchar](50)  -- GitHub, GitLab, Bitbucket
    [Name] [nvarchar](255)
    [Description] [nvarchar](max),
    [Language] [nvarchar](50),
    [Stars] [int],
    [Watchers] [int],
    [Forks] [int],
    [LastUpdate] [datetime2],
    [CreatedAt] [datetime2],
    [LoadedAt] [datetime2] DEFAULT GETDATE(),
    CONSTRAINT PK_Repo PRIMARY KEY ([RepositoryId], [Source])
)
GO

-- 2. Watermark table for incremental load
CREATE TABLE [dbo].[WatermarkTable](
    [SourceName] [nvarchar](50) PRIMARY KEY,
    [LastLoadTime] [datetime2],
    [LastId] [nvarchar](100),
    [LoadCount] [int] DEFAULT 0,
    [UpdatedAt] [datetime2] DEFAULT GETDATE()
)
GO

-- 3. Execution audit
CREATE TABLE [dbo].[ExecutionAudit](
    [AuditId] [int] IDENTITY(1,1) PRIMARY KEY,
    [PackageName] [nvarchar](255),
    [Source] [nvarchar](50),
    [StartTime] [datetime2],
    [EndTime] [datetime2],
    [RecordsLoaded] [int],
    [Status] [nvarchar](50),  -- Success, Error, Warning
    [ErrorMessage] [nvarchar](max)
)
GO

-- 4. Initialize watermarks
INSERT INTO [dbo].[WatermarkTable] 
    ([SourceName], [LastLoadTime])
VALUES 
    ('GitHub', GETDATE()),
    ('GitLab', GETDATE()),
    ('Bitbucket', GETDATE())
GO
```

---

## 🔌 Connection Managers Setup

### Configurar 3 Conexões API

#### 1. GitHub API
```
Name:               GitHubAPI
Base URL:           https://api.github.com
Authentication:     Bearer Token
Endpoint:           /user/repos
Rate Limit:         60 requests/hour (public)
```

#### 2. GitLab API
```
Name:               GitLabAPI
Base URL:           https://gitlab.com/api/v4
Authentication:     Personal Access Token
Endpoint:           /projects
Rate Limit:         600 requests/hour
```

#### 3. Bitbucket API
```
Name:               BitbucketAPI
Base URL:           https://api.bitbucket.org/2.0
Authentication:     Basic Auth (OAuth2)
Endpoint:           /user/repositories
Rate Limit:         Unlimited (with auth)
```

---

## 📋 Package Design

### Control Flow

```
┌─ Start
│
├─ Initialize Watermarks
│  └─ Execute SQL Task: Load last watermark values
│
├─ Process GitHub
│  ├─ Data Flow: GitHub API → Repositories
│  ├─ Update Watermark: GitHub
│  └─ Log: GitHub completed
│
├─ Process GitLab
│  ├─ Data Flow: GitLab API → Repositories
│  ├─ Update Watermark: GitLab
│  └─ Log: GitLab completed
│
├─ Process Bitbucket
│  ├─ Data Flow: Bitbucket API → Repositories
│  ├─ Update Watermark: Bitbucket
│  └─ Log: Bitbucket completed
│
├─ Aggregation
│  ├─ Data Flow: Aggregate statistics
│  └─ Load: Summary tables
│
└─ Finalize
   ├─ Update Audit Table
   └─ Send Notifications
```

---

## 🔄 Incremental Load Implementation

### Watermark Strategy

```sql
-- Query with watermark
DECLARE @LastLoadTime DATETIME2 = 
    (SELECT LastLoadTime FROM WatermarkTable 
     WHERE SourceName = 'GitHub')

-- Fetch only new/updated records
-- WHERE updated_at > @LastLoadTime

-- Update watermark after load
UPDATE WatermarkTable 
SET LastLoadTime = GETDATE()
WHERE SourceName = 'GitHub'
```

### In SSIS Package

```
1. Execute SQL Task: Read watermark
   → Store in variable @LastLoadTime

2. Corporate API Source:
   → Add parameter: since=@LastLoadTime
   → Only fetch records > watermark

3. Data Flow transformation
   → Transform and load

4. Execute SQL Task: Update watermark
   → SET LastLoadTime = GETDATE()
```

---

## 🔀 Data Transformations

### Data Flow Transformations

```
JSON Data
    ↓
┌─────────────────────────────────┐
│ Derived Column Transform        │
├─────────────────────────────────┤
│ Add: Source column = "GitHub"   │
│ Add: LoadDate = GETDATE()       │
│ Add: IsActive = TRUE            │
└─────────────────────────────────┘
    ↓
┌─────────────────────────────────┐
│ Data Conversion Transform       │
├─────────────────────────────────┤
│ Convert Text → Int (Stars)      │
│ Convert Text → DateTime         │
└─────────────────────────────────┘
    ↓
┌─────────────────────────────────┐
│ Conditional Split               │
├─────────────────────────────────┤
│ Route 1: Language != NULL       │
│ Route 2: Language = NULL        │
└─────────────────────────────────┘
    ↓
Destination (SQL Server)
```

---

## 📊 Monitoring & Logging

### Implement Logging

```csharp
// In Custom Script Task
var logger = LoggerFactory.GetLogger<DataPipeline>();

using (var scope = new LogScope(logger, "LoadRepositories", correlationId))
{
    logger.LogInformation("Starting repository load");
    
    // Load data...
    
    logger.LogSuccess("LoadRepositories", 
        $"Loaded {recordCount} records from {source}");
}
```

### Log to SQL Server

```sql
-- Stored procedure to log execution
CREATE PROCEDURE sp_LogExecution
    @PackageName NVARCHAR(255),
    @Source NVARCHAR(50),
    @Status NVARCHAR(50),
    @RecordsLoaded INT,
    @ErrorMessage NVARCHAR(MAX) = NULL
AS
BEGIN
    INSERT INTO ExecutionAudit 
        (PackageName, Source, StartTime, EndTime, 
         RecordsLoaded, Status, ErrorMessage)
    VALUES 
        (@PackageName, @Source, GETDATE(), GETDATE(),
         @RecordsLoaded, @Status, @ErrorMessage)
END
GO
```

---

## ⚙️ Advanced Features

### Error Handling

```
For Each Source:
├─ TRY
│  ├─ Execute API query
│  ├─ Transform data
│  └─ Load to destination
│
├─ CATCH
│  ├─ Log error
│  ├─ Update execution audit
│  ├─ Send notification
│  └─ Continue to next source (not fail)
│
└─ FINALLY
   └─ Update watermark (if partial success)
```

### Retry Logic

```
Configuration:
├─ Max Retries: 3
├─ Retry Wait (seconds): 5
├─ Backoff Multiplier: 2x
└─ Max Wait: 60 seconds

On Error:
├─ Retry #1: wait 5 sec
├─ Retry #2: wait 10 sec
├─ Retry #3: wait 20 sec
└─ Fail: Log and continue
```

---

## 📈 Performance Optimization

### Batch Size Settings

```
GitHub:
├─ Page Size: 100
├─ Parallel Requests: 2
└─ Expected Duration: 5-10 minutes

GitLab:
├─ Page Size: 100
├─ Parallel Requests: 3
└─ Expected Duration: 3-5 minutes

Bitbucket:
├─ Page Size: 50
├─ Parallel Requests: 2
└─ Expected Duration: 2-3 minutes

Total: ~15-20 minutes for full load
```

---

## 📊 Monitoring Dashboard

### Key Metrics to Track

```
✅ Record counts by source
✅ Load duration per source
✅ Error rates
✅ Success rate %
✅ Average processing time
✅ Peak load times
✅ Data quality metrics
```

### Create Monitoring Query

```sql
-- Dashboard query
SELECT 
    Source,
    COUNT(*) as RecordCount,
    MAX(LoadedAt) as LastLoad,
    AVG(DATEDIFF(SECOND, LoadedAt, GETDATE())) as AvgAge
FROM Repositories
GROUP BY Source
ORDER BY RecordCount DESC
```

---

## 🚀 Scheduling

### Schedule in SQL Agent

```sql
-- Create job to run package nightly
EXEC msdb.dbo.sp_add_job 
    @job_name = 'QuattoSample2_Nightly'

EXEC msdb.dbo.sp_add_jobstep 
    @job_name = 'QuattoSample2_Nightly',
    @step_name = 'RunPackage',
    @command = 'dtexec /f "C:\Packages\Sample2.dtsx"'

EXEC msdb.dbo.sp_add_schedule 
    @schedule_name = 'Nightly',
    @freq_type = 4,  -- Daily
    @active_start_time = 020000  -- 2 AM
```

---

## 📋 Troubleshooting

### Common Issues

#### Issue: "Source returned no data"

**Cause:** Watermark filter too restrictive  
**Solution:**
```sql
-- Check watermark values
SELECT * FROM WatermarkTable

-- If needed, reset watermark
UPDATE WatermarkTable 
SET LastLoadTime = DATEADD(DAY, -7, GETDATE())
WHERE SourceName = 'GitHub'
```

#### Issue: "Rate limit exceeded"

**Cause:** Too many requests to API  
**Solution:**
```
1. Increase page size
2. Decrease batch frequency
3. Implement caching
4. Use higher tier API access
```

#### Issue: "Package runs slowly"

**Cause:** Inefficient transformations  
**Solution:**
```
1. Check Data Flow Profiler
2. Reduce unnecessary transformations
3. Increase buffer size
4. Run in parallel (with caution)
```

---

## 📚 Next Steps

### After Completing This Sample

```
1. ✅ Understand multi-source ETL
2. ✅ Learn watermark management
3. ✅ Implement error handling
4. ✅ Setup monitoring
5. ✅ Schedule execution

Next: Sample 3 - OAuth2 Integration
```

---

**Tempo estimado:** 2 horas  
**Nível:** Intermediate  
**Complexidade:** 📊📊📊 (3/5)

