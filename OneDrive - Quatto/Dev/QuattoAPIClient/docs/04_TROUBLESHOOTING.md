# Guia de Uso - Quatto API Client v1.0

## 📋 Índice

1. [Primeiro Pacote](#primeiro-pacote)
2. [Configuração Básica](#configuração-básica)
3. [Casos de Uso Comuns](#casos-de-uso-comuns)
4. [Padrões Recomendados](#padrões-recomendados)
5. [Integração com Pipelines](#integração-com-pipelines)
6. [Monitoramento e Alertas](#monitoramento-e-alertas)

---

## 🚀 Primeiro Pacote

### Passo a Passo: Criar Pacote Gladium Orders

#### 1. Criar Novo Projeto SSIS
```
File → New → Project
Template: Integration Services Project
Name: SESCDF_DataWarehouse_APIs
Location: C:\Projects\SESCDF_DW
```

#### 2. Configurar Parâmetros de Projeto
```
Solution Explorer → Project.params → Botão direito → Parameters

Adicionar:
├─ GladiumApiBaseUrl (String) = "https://api.gladium.com"
├─ GladiumApiToken (String, Sensitive) = "eyJhbGci..."
├─ DW_ConnectionString (String) = "Data Source=SQL-PRD;Initial Catalog=SESCDF_DW;..."
└─ Environment (String) = "PRD"
```

#### 3. Criar Connection Managers

**Connection Manager 1: API Connection**
```
Connection Managers → New Connection → API

Name: GladiumAPI
Properties:
├─ AuthType: Bearer
├─ BearerToken: @[$Project::GladiumApiToken]
├─ TimeoutSeconds: 100
└─ DefaultHeadersJson: {"Accept":"application/json","User-Agent":"SESCDF-DW/1.0"}
```

**Connection Manager 2: OLE DB Connection**
```
Connection Managers → New Connection → OLE DB

Name: DW
ConnectionString: @[$Project::DW_ConnectionString]
```

#### 4. Criar Variáveis do Pacote
```
View → Other Windows → Variables

Adicionar:
├─ TotalRecords (Int32) = 0
├─ ExecutionStatus (String) = "RUNNING"
└─ ErrorCount (Int32) = 0
```

#### 5. Control Flow
```
Toolbox → Control Flow Items

[1] Execute SQL Task: Truncate Staging
    └─ SQL: TRUNCATE TABLE stg.Gladium_Orders;
    
[2] Data Flow Task: Extract Gladium Orders
    └─ (configurar abaixo)
    
[3] Execute SQL Task: Update Statistics
    └─ SQL: UPDATE STATISTICS stg.Gladium_Orders;
```

#### 6. Data Flow

**Componente 1: Quatto Corporate API Source**
```
Toolbox → SSIS Toolbox → Other Sources → Quatto Corporate API Source

Duplo clique → Configurar:

Tab "General":
├─ Connection: GladiumAPI
├─ BaseUrl: @[$Project::GladiumApiBaseUrl]
├─ Endpoint: /v1/orders
├─ HttpMethod: GET
├─ QueryTemplate: ?page={Page}&pageSize={PageSize}&updatedSince={Watermark}
└─ PageSize: 500

Tab "Pagination":
├─ PaginationType: Offset
├─ StartPage: 1
└─ MaxPages: 0 (ilimitado)

Tab "Incremental":
├─ EnableIncremental: ✓
├─ WatermarkColumn: updatedAt
├─ WatermarkStore: dbo.API_Watermarks
├─ SourceSystem: Gladium
└─ Environment: @[$Project::Environment]

Tab "Storage":
├─ RawStoreMode: SqlVarbinary
├─ RawStoreTarget: dbo.API_RawPayloads
├─ CompressRawJson: ✓
└─ HashRawJson: ✓

Tab "Advanced":
├─ MaxRetries: 5
├─ BackoffMode: Exponential
├─ BaseDelayMs: 1000
├─ RateLimitRPM: 120
└─ TimeoutSeconds: 100
```

**Schema Mapping (JSON):**
```json
{
  "columns": [
    {"name": "order_id", "path": "$.id", "type": "DT_WSTR", "length": 50},
    {"name": "order_number", "path": "$.orderNumber", "type": "DT_WSTR", "length": 100},
    {"name": "customer_id", "path": "$.customer.id", "type": "DT_WSTR", "length": 50},
    {"name": "customer_name", "path": "$.customer.name", "type": "DT_WSTR", "length": 200},
    {"name": "updated_at", "path": "$.updatedAt", "type": "DT_DBTIMESTAMP2"},
    {"name": "total_amount", "path": "$.amount.total", "type": "DT_NUMERIC", "precision": 18, "scale": 2},
    {"name": "status", "path": "$.status", "type": "DT_WSTR", "length": 50}
  ]
}
```

**Componente 2: Derived Column**
```
Toolbox → Common → Derived Column

Adicionar colunas:
├─ LoadDate = GETDATE()
├─ SourceSystem = "Gladium"
├─ BatchID = @[System::TaskID]
└─ Environment = @[$Project::Environment]
```

**Componente 3: OLE DB Destination**
```
Connection Manager: DW
Table: stg.Gladium_Orders
Access Mode: Table or view - fast load

Mappings: (automático)
```

**Componente 4: Error Output → Flat File Destination**
```
Configure Error Output no API Source:
├─ Error: Redirect Row
└─ Truncation: Redirect Row

Flat File Connection:
├─ Name: ErrorLog
├─ Format: Delimited
└─ File: C:\SSIS_Logs\Errors\Gladium_Orders_{ExecutionID}.txt
```

#### 7. Event Handlers (Opcional)

**OnError:**
```
Send Mail Task
├─ To: dba-team@sescdf.com.br
├─ Subject: "[ERRO] API Gladium Orders - " + @[System::PackageName]
└─ MessageSource: @[System::ErrorDescription]
```

**OnPostExecute:**
```
Execute SQL Task: Log Summary
SQL:
INSERT INTO dbo.ETL_PackageLog 
(PackageName, StartTime, EndTime, RowsRead, Status)
VALUES 
(@[System::PackageName], @[System::StartTime], GETDATE(), 
 @[User::TotalRecords], @[User::ExecutionStatus]);
```

#### 8. Executar e Validar
```
Debug → Start Debugging (F5)

Verificar:
✓ Data Flow mostra registros processados
✓ Nenhum erro no Output
✓ Tabela stg.Gladium_Orders populada
✓ Watermark atualizado em dbo.API_Watermarks
✓ JSON armazenado em dbo.API_RawPayloads
✓ Telemetria em dbo.API_ExecutionLog
```

---

## ⚙️ Configuração Básica

### Connection Manager Patterns

#### Pattern 1: Bearer Token (Gladium)
```
AuthType: Bearer
BearerToken: @[$Project::GladiumApiToken]
```

**Uso:** APIs que fornecem token JWT de longa duração.

#### Pattern 2: API Key (Portal SESC)
```
AuthType: ApiKey
ApiKeyHeader: x-api-key
ApiKeyValue: @[$Project::PortalSESCApiKey]
```

**Uso:** APIs que usam chave fixa em header.

#### Pattern 3: OAuth2 (Easydental)
```
AuthType: OAuth2ClientCredentials
TokenEndpoint: https://auth.easydental.com.br/token
ClientId: @[$Project::EasydentalClientId]
ClientSecret: @[$Project::EasydentalClientSecret]
Scope: api.read api.write
```

**Uso:** APIs que requerem OAuth2 flow.

### Query Templates Comuns

#### Template 1: Offset Pagination
```
?page={Page}&pageSize={PageSize}&updatedSince={Watermark}
```

#### Template 2: Offset com Filtros
```
?offset={Page}&limit={PageSize}&modifiedAfter={Watermark}&status=active
```

#### Template 3: Cursor Pagination
```
?cursor={Cursor}&pageSize={PageSize}&since={Watermark}
```

#### Template 4: Date Range
```
?startDate={Watermark}&endDate={CurrentDate}&pageSize={PageSize}
```

**Placeholders disponíveis:**
- `{Page}` - Número da página (paginação offset)
- `{PageSize}` - Tamanho da página
- `{Watermark}` - Último watermark (incremental)
- `{Cursor}` - Cursor atual (paginação cursor)
- `{CurrentDate}` - Data/hora atual UTC

---

## 💡 Casos de Uso Comuns

### Caso 1: Extração Full (Primeira Carga)

**Cenário:** Carregar todos os dados históricos da API.

**Configuração:**
```
EnableIncremental: false  ← DESABILITAR
MaxPages: 0 (ilimitado)
PageSize: 1000 (máximo permitido pela API)
```

**Atenção:**
- Pode levar horas dependendo do volume
- Considerar executar fora do horário comercial
- Monitorar uso de disco (Raw JSON)

**SQL para limpar watermark antes:**
```sql
DELETE FROM dbo.API_Watermarks
WHERE SystemName = 'Gladium' 
  AND Endpoint = '/v1/orders';
```

---

### Caso 2: Extração Incremental (Delta)

**Cenário:** Carregar apenas registros novos/alterados.

**Configuração:**
```
EnableIncremental: true
WatermarkColumn: updatedAt
QueryTemplate: ?updatedSince={Watermark}&page={Page}&pageSize={PageSize}
```

**Fluxo:**
1. Componente lê último watermark do DB
2. API retorna apenas registros com `updatedAt > watermark`
3. Componente observa maior `updatedAt` nos registros
4. Salva novo watermark ao final

**SQL para verificar watermark:**
```sql
SELECT 
    SystemName,
    Endpoint,
    LastWatermark,
    LastRunUtc,
    TotalRecordsExtracted
FROM dbo.API_Watermarks
WHERE SystemName = 'Gladium';
```

---

### Caso 3: Reprocessamento de Erros

**Cenário:** Reprocessar registros que falharam.

**SQL para identificar erros:**
```sql
SELECT 
    CorrelationID,
    SystemName,
    Endpoint,
    ErrorMessage,
    CollectedUtc
FROM dbo.API_RawPayloads
WHERE ProcessingStatus = 'ERROR'
  AND SystemName = 'Gladium'
ORDER BY CollectedUtc DESC;
```

**Reprocessar:**

1. Copiar JSON do campo `ResponseBodyGzip` (descomprimir)
2. Criar pacote temporário que lê JSON de arquivo
3. Processar manualmente
4. Atualizar `ProcessingStatus = 'PROCESSED'`

**OU usar stored procedure:**
```sql
EXEC dbo.usp_API_ReprocessErrors 
    @SystemName = 'Gladium',
    @Endpoint = '/v1/orders',
    @MaxRecords = 100;
```

---

### Caso 4: Extração com Rate Limiting Agressivo

**Cenário:** API com limite muito baixo (ex: 30 RPM).

**Configuração:**
```
RateLimitRPM: 30
PageSize: 100 (reduzir para evitar timeout)
MaxRetries: 10 (aumentar)
BaseDelayMs: 2000 (aumentar delay inicial)
```

**Cálculo de tempo estimado:**
```
Total de registros: 100.000
Page size: 100
Total de páginas: 1.000
RPM: 30
Tempo estimado: 1.000 / 30 = 33 minutos
```

**Dica:** Executar em horários de baixa demanda.

---

### Caso 5: APIs com Dados Aninhados Complexos

**Cenário:** API retorna JSON com arrays e objetos nested.

**Exemplo de JSON:**
```json
{
  "order": {
    "id": "123",
    "customer": {
      "id": "456",
      "name": "John Doe",
      "addresses": [
        {"type": "billing", "city": "São Paulo"},
        {"type": "shipping", "city": "Rio de Janeiro"}
      ]
    },
    "items": [
      {"sku": "ABC", "quantity": 2},
      {"sku": "XYZ", "quantity": 1}
    ]
  }
}
```

**Solução v1.0 (Flat Fields):**
```json
{
  "columns": [
    {"name": "order_id", "path": "$.order.id", "type": "DT_WSTR", "length": 50},
    {"name": "customer_id", "path": "$.order.customer.id", "type": "DT_WSTR", "length": 50},
    {"name": "customer_name", "path": "$.order.customer.name", "type": "DT_WSTR", "length": 200}
    // ⚠️ Arrays não suportados - extrair apenas primeiro elemento
  ]
}
```

**Solução v2.0 (Planejado):**
- Normalização automática de arrays
- Múltiplos outputs (Orders, Items, Addresses)

**Workaround atual:**

1. Armazenar JSON completo em Raw Storage
2. Processar arrays em pacote separado usando Script Task:
```csharp
// Ler de dbo.API_RawPayloads
// Parsear JSON
// Gerar tabelas normalizadas (Orders, Items)
```

---

## 📐 Padrões Recomendados

### Pattern 1: Staging → ODS → DW
```
┌─────────────────────────────────────────────────────────────┐
│ SSIS Package 1: API → Staging                               │
├─────────────────────────────────────────────────────────────┤
│ [Quatto API Source] → [Staging Table]                       │
│ - Carga bruta da API                                        │
│ - Sem transformações                                        │
│ - Todas as colunas da API                                   │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ SSIS Package 2: Staging → ODS                               │
├─────────────────────────────────────────────────────────────┤
│ [Staging] → [Transformações] → [ODS]                        │
│ - Limpeza de dados                                          │
│ - Conversões de tipo                                        │
│ - Business rules                                            │
│ - SCD Type 1 ou Type 2                                      │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ SSIS Package 3: ODS → DW                                    │
├─────────────────────────────────────────────────────────────┤
│ [ODS] → [Aggregations] → [Fact/Dimension Tables]           │
│ - Modelagem dimensional                                     │
│ - Agregações                                                │
│ - Slow Changing Dimensions                                  │
└─────────────────────────────────────────────────────────────┘
```

### Pattern 2: Error Handling Strategy
```
Data Flow:

[API Source]
    │
    ├─→ (Success Output) → [Transformations] → [Destination]
    │
    └─→ (Error Output) → [Error Table/File]

Error Table Structure:

CREATE TABLE dbo.API_ErrorLog
(
    ErrorID         BIGINT IDENTITY PRIMARY KEY,
    CorrelationID   UNIQUEIDENTIFIER,
    SystemName      NVARCHAR(100),
    Endpoint        NVARCHAR(200),
    ErrorCode       INT,
    ErrorMessage    NVARCHAR(MAX),
    RawJson         NVARCHAR(MAX),
    ErrorDate       DATETIME2 DEFAULT GETUTCDATE(),
    Reprocessed     BIT DEFAULT 0
);
```

### Pattern 3: Idempotent Loads

**Problema:** Pacote executado 2x por engano → duplicatas

**Solução:** MERGE ao invés de INSERT
```sql
-- No OLE DB Destination, usar SQL Command:

MERGE stg.Gladium_Orders AS target
USING (SELECT * FROM ##TempSource) AS source
ON target.order_id = source.order_id
WHEN MATCHED AND source.updated_at > target.updated_at THEN
    UPDATE SET
        order_number = source.order_number,
        customer_name = source.customer_name,
        total_amount = source.total_amount,
        status = source.status,
        updated_at = source.updated_at,
        LoadDate = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (order_id, order_number, customer_name, total_amount, status, updated_at, LoadDate)
    VALUES (source.order_id, source.order_number, source.customer_name, 
            source.total_amount, source.status, source.updated_at, GETDATE());
```

### Pattern 4: Logging and Auditing

**Execute SQL Task (PreExecute):**
```sql
DECLARE @ExecutionID BIGINT;

INSERT INTO dbo.ETL_ExecutionLog 
(PackageName, TaskName, StartTime, Status)
VALUES 
(@PackageName, @TaskName, GETDATE(), 'RUNNING');

SET @ExecutionID = SCOPE_IDENTITY();
SELECT @ExecutionID AS ExecutionID;

-- Armazenar em variável @[User::ExecutionID]
```

**Execute SQL Task (PostExecute):**
```sql
UPDATE dbo.ETL_ExecutionLog
SET 
    EndTime = GETDATE(),
    RowsRead = ?,
    RowsWritten = ?,
    Status = ?
WHERE ExecutionID = ?;

-- Parameters:
-- 0: @[User::TotalRecords]
-- 1: @[User::TotalRecords]
-- 2: @[User::ExecutionStatus]
-- 3: @[User::ExecutionID]
```

---

## 🔄 Integração com Pipelines

### SQL Server Agent Job

**Job 1: Daily Incremental**
```sql
USE [msdb];
GO

EXEC msdb.dbo.sp_add_job
    @job_name = N'API_Gladium_Orders_Daily',
    @enabled = 1,
    @description = N'Extração incremental diária de pedidos Gladium';

EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'API_Gladium_Orders_Daily',
    @step_name = N'Execute SSIS Package',
    @subsystem = N'SSIS',
    @command = N'/ISSERVER "\SSISDB\SESCDF_DW\API_Gladium_Orders.dtsx" /SERVER "SQL-PRD" /Par "$Project::Environment(String)";"PRD" /CALLERINFO SQLAGENT';

EXEC msdb.dbo.sp_add_jobschedule
    @job_name = N'API_Gladium_Orders_Daily',
    @name = N'Daily at 2AM',
    @freq_type = 4, -- Daily
    @freq_interval = 1,
    @active_start_time = 020000;

EXEC msdb.dbo.sp_add_jobserver
    @job_name = N'API_Gladium_Orders_Daily';
GO
```

**Job 2: Weekly Full Refresh**
```sql
-- Executar limpeza completa + carga full semanalmente
-- Útil para detectar registros deletados na API

EXEC msdb.dbo.sp_add_job
    @job_name = N'API_Gladium_Orders_WeeklyFull';

-- Step 1: Clear watermark
EXEC msdb.dbo.sp_add_jobstep
    @step_name = N'Clear Watermark',
    @subsystem = N'TSQL',
    @command = N'DELETE FROM dbo.API_Watermarks WHERE SystemName = ''Gladium'' AND Endpoint = ''/v1/orders'';';

-- Step 2: Execute package
EXEC msdb.dbo.sp_add_jobstep
    @step_name = N'Execute Package',
    @subsystem = N'SSIS',
    @command = N'...';

-- Schedule: Sundays at 1AM
EXEC msdb.dbo.sp_add_jobschedule
    @freq_type = 8, -- Weekly
    @freq_interval = 1, -- Sunday
    @active_start_time = 010000;
```

### Azure Data Factory Integration

**Linked Service: SSIS-IR**
```json
{
  "name": "SSIS_IntegrationRuntime",
  "properties": {
    "type": "SsisIntegrationRuntime",
    "typeProperties": {
      "catalogInfo": {
        "catalogServerEndpoint": "sql-prd.database.windows.net",
        "catalogAdminUserName": "ssisadmin",
        "catalogAdminPassword": {
          "type": "SecureString",
          "value": "**********"
        }
      }
    }
  }
}
```

**Pipeline:**
```json
{
  "name": "Pipeline_Gladium_Orders",
  "properties": {
    "activities": [
      {
        "name": "Execute_SSIS_Package",
        "type": "ExecuteSSISPackage",
        "typeProperties": {
          "packageLocation": {
            "packagePath": "/SSISDB/SESCDF_DW/API_Gladium_Orders.dtsx"
          },
          "runtime": "x64",
          "loggingLevel": "Basic",
          "environmentPath": "/SSISDB/SESCDF_DW/Environments/PRD",
          "parameters": {
            "Environment": "PRD"
          }
        }
      }
    ]
  }
}
```

---

## 📊 Monitoramento e Alertas

### Dashboard Power BI

**Query 1: KPIs em Tempo Real**
```sql
-- Última execução por sistema
SELECT 
    SystemName,
    MAX(ExecutionStartedUtc) AS LastRun,
    DATEDIFF(MINUTE, MAX(ExecutionStartedUtc), GETUTCDATE()) AS MinutesAgo,
    SUM(CASE WHEN ExecutionStartedUtc >= DATEADD(DAY, -1, GETUTCDATE()) THEN TotalRecords ELSE 0 END) AS Records_24h
FROM dbo.API_ExecutionLog
GROUP BY SystemName;
```

**Query 2: Taxa de Sucesso (7 dias)**
```sql
SELECT 
    CAST(ExecutionStartedUtc AS DATE) AS Date,
    SystemName,
    COUNT(*) AS Executions,
    SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS SuccessRate_Pct
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, GETUTCDATE())
GROUP BY CAST(ExecutionStartedUtc AS DATE), SystemName
ORDER BY Date DESC;
```

### Alertas via SQL Server Agent

**Alert 1: Falha de Execução**
```sql
USE [msdb];
GO

EXEC msdb.dbo.sp_add_alert
    @name = N'API_ExecutionFailure',
    @message_id = 0,
    @severity = 16,
    @enabled = 1,
    @delay_between_responses = 300,
    @include_event_description_in = 1,
    @database_name = N'SESCDF_DW',
    @event_description_keyword = N'API_ExecutionLog',
    @job_name = N'Send_Alert_Email';
GO
```

**Alert 2: Watermark Stale (24h)**
```sql
-- Job que roda a cada 6 horas

IF EXISTS (
    SELECT 1 
    FROM dbo.API_Watermarks
    WHERE LastRunUtc < DATEADD(HOUR, -24, GETUTCDATE())
      AND LastStatus = 'SUCCESS'
)
BEGIN
    EXEC msdb.dbo.sp_send_dbmail
        @profile_name = 'DBA_Profile',
        @recipients = 'dba-team@sescdf.com.br',
        @subject = '[ALERTA] Watermark desatualizado há mais de 24h',
        @body = 'Um ou mais sistemas não executaram nas últimas 24 horas. Verificar SQL Agent Jobs.';
END
```

### Grafana Dashboard (Opcional)

**Datasource:** SQL Server  
**Queries:**
```sql
-- Panel 1: Total Records Extracted (Last 30 days)
SELECT 
    CAST(ExecutionStartedUtc AS DATE) AS time,
    SUM(TotalRecords) AS value
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -30, GETUTCDATE())
GROUP BY CAST(ExecutionStartedUtc AS DATE)
ORDER BY time;

-- Panel 2: Average Latency by System
SELECT 
    SystemName AS metric,
    AVG(AvgLatencyMs) AS value
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, GETUTCDATE())
GROUP BY SystemName;

-- Panel 3: Error Rate
SELECT 
    SystemName,
    SUM(CASE WHEN Status = 'FAILED' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS ErrorRate_Pct
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, GETUTCDATE())
GROUP BY SystemName;
```

---

## 🎓 Boas Práticas

### ✅ DO's

1. **Sempre use parâmetros SSISDB para valores sensíveis**
2. **Configure ErrorOutput para capturar falhas**
3. **Use Raw Storage para auditoria e debug**
4. **Monitore dashboard diariamente**
5. **Teste em DEV antes de PRD**
6. **Documente schema mappings**
7. **Configure alertas de falha**
8. **Mantenha PageSize entre 100-1000**
9. **Use MERGE para idempotência**
10. **Valide watermarks periodicamente**

### ❌ DON'Ts

1. **Não hardcode tokens no pacote**
2. **Não ignore ErrorOutput**
3. **Não desabilite Raw Storage em PRD**
4. **Não use PageSize > limite da API**
5. **Não execute Full Load em horário comercial**
6. **Não reutilize Connection Manager entre ambientes**
7. **Não ignore logs de throttling**
8. **Não modifique watermarks manualmente sem backup**
9. **Não use mesmo BatchID para execuções diferentes**
10. **Não exponha logs com tokens em dashboards**

---

## 📞 Próximos Passos

- [01_INSTALLATION.md](01_INSTALLATION.md) - Instalação
- [02_CONFIGURATION.md](02_CONFIGURATION.md) - Configuração
- [03_TROUBLESHOOTING.md](03_TROUBLESHOOTING.md) - Problemas comuns
- [05_API_REFERENCE.md](05_API_REFERENCE.md) - Referência técnica

**Suporte:** erton.miranda@quatto.com.br
