═══════════════════════════════════════════════════════════════════
QUATTO API CLIENT - EXEMPLO DE PACOTE SSIS
Arquivo: API_Gladium_Orders.dtsx
═══════════════════════════════════════════════════════════════════

ESTRUTURA DO PACOTE:

┌─────────────────────────────────────────────────────────────────┐
│ CONTROL FLOW                                                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  [1] Execute SQL Task: Validate Database Objects                │
│       SQL: SELECT COUNT(*) FROM sys.tables WHERE name LIKE 'API_%'
│       ↓                                                          │
│  [2] Data Flow Task: Extract Gladium Orders                     │
│       ↓                                                          │
│  [3] Execute SQL Task: Log Execution Summary                    │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════════
VARIÁVEIS DO PACOTE
═══════════════════════════════════════════════════════════════════

User::LastWatermark (String)
  - Escopo: Package
  - Valor: (vazio - será carregado dinamicamente)

User::TotalRecordsExtracted (Int32)
  - Escopo: Package
  - Valor: 0

User::ExecutionStatus (String)
  - Escopo: Package
  - Valor: RUNNING

═══════════════════════════════════════════════════════════════════
CONNECTION MANAGERS
═══════════════════════════════════════════════════════════════════

[1] DW (OLE DB Connection)
  - ConnectionString: @[$Project::DW_ConnectionString]
  - Uso: Destination e controle de watermark

[2] GladiumAPI (API Connection)
  - AuthType: Bearer
  - BearerToken: @[$Project::GladiumApiToken]
  - TimeoutSeconds: 100
  - SandboxMode: false

═══════════════════════════════════════════════════════════════════
DATA FLOW: Extract Gladium Orders
═══════════════════════════════════════════════════════════════════

┌─────────────────────────────────────────────────────────────────┐
│                                                                  │
│  ┌──────────────────────────────────────────┐                  │
│  │ Quatto Corporate API Source              │                  │
│  │                                           │                  │
│  │ Connection: GladiumAPI                   │                  │
│  │ BaseUrl: @[$Project::GladiumApiBaseUrl]  │                  │
│  │ Endpoint: /v1/orders                     │                  │
│  │ PageSize: 500                            │                  │
│  │ EnableIncremental: true                  │                  │
│  │ WatermarkColumn: updatedAt               │                  │
│  │ SourceSystem: Gladium                    │                  │
│  │ Environment: @[$Project::Environment]    │                  │
│  │ RawStoreMode: SqlVarbinary               │                  │
│  │ RawStoreTarget: dbo.API_RawPayloads      │                  │
│  └──────────────────┬───────────────────────┘                  │
│                     │                                           │
│           ┌─────────┴─────────┐                                │
│           │                   │                                 │
│      (Output)           (ErrorOutput)                           │
│           │                   │                                 │
│           ↓                   ↓                                 │
│  ┌────────────────┐  ┌────────────────────┐                   │
│  │ Derived Column │  │ Flat File Dest     │                   │
│  │                │  │                    │                   │
│  │ Add ETL cols:  │  │ File: errors_      │                   │
│  │ - LoadDate     │  │ gladium_orders.txt │                   │
│  │ - SourceSystem │  │                    │                   │
│  │ - BatchID      │  └────────────────────┘                   │
│  └────────┬───────┘                                            │
│           │                                                     │
│           ↓                                                     │
│  ┌────────────────┐                                            │
│  │ OLE DB Dest    │                                            │
│  │                │                                            │
│  │ Table: stg.    │                                            │
│  │ Gladium_Orders │                                            │
│  │                │                                            │
│  │ Access Mode:   │                                            │
│  │ Table or View  │                                            │
│  │ Fast Load      │                                            │
│  └────────────────┘                                            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════════
DERIVED COLUMN TRANSFORMATIONS
═══════════════════════════════════════════════════════════════════

Column Name       | Expression
------------------+--------------------------------------------------
LoadDate          | GETDATE()
SourceSystem      | "Gladium"
BatchID           | (DT_I4)@[System::TaskID]
Environment       | @[$Project::Environment]

═══════════════════════════════════════════════════════════════════
STAGING TABLE STRUCTURE
═══════════════════════════════════════════════════════════════════

CREATE TABLE stg.Gladium_Orders
(
    StagingID        BIGINT IDENTITY(1,1) PRIMARY KEY,
    
    -- Campos da API
    order_id         NVARCHAR(50) NOT NULL,
    order_number     NVARCHAR(100),
    customer_id      NVARCHAR(50),
    customer_name    NVARCHAR(200),
    customer_email   NVARCHAR(100),
    order_date       DATETIME2(3),
    updated_at       DATETIME2(3),
    status           NVARCHAR(50),
    total_amount     DECIMAL(18,2),
    currency         NVARCHAR(3),
    payment_method   NVARCHAR(50),
    is_paid          BIT,
    
    -- Campos de controle ETL
    LoadDate         DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    SourceSystem     NVARCHAR(50) NOT NULL,
    BatchID          INT,
    Environment      NVARCHAR(10)
);

CREATE INDEX IX_Gladium_Orders_OrderID 
    ON stg.Gladium_Orders(order_id);

CREATE INDEX IX_Gladium_Orders_UpdatedAt 
    ON stg.Gladium_Orders(updated_at);

═══════════════════════════════════════════════════════════════════
EXECUTE SQL TASKS
═══════════════════════════════════════════════════════════════════

[Task 1] Validate Database Objects
----------------------------------
SQLStatement:
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'API_Watermarks')
    RAISERROR('Tabela API_Watermarks não existe. Execute scripts de setup.', 16, 1);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'API_RawPayloads')
    RAISERROR('Tabela API_RawPayloads não existe. Execute scripts de setup.', 16, 1);

[Task 3] Log Execution Summary
-------------------------------
SQLStatement:
DECLARE @RecordCount INT = ?;
DECLARE @Status NVARCHAR(20) = ?;

INSERT INTO dbo.ETL_ExecutionLog 
(PackageName, TaskName, RecordCount, Status, ExecutionDate)
VALUES 
('API_Gladium_Orders', 'Extract Orders', @RecordCount, @Status, SYSUTCDATETIME());

Parameters:
  0 = User::TotalRecordsExtracted (Input)
  1 = User::ExecutionStatus (Input)

═══════════════════════════════════════════════════════════════════
EXPRESSIONS E PARÂMETROS
═══════════════════════════════════════════════════════════════════

Connection Manager "GladiumAPI":
  - BearerToken: @[$Project::GladiumApiToken]

Data Flow Task:
  - [Quatto API Source].BaseUrl: @[$Project::GladiumApiBaseUrl]
  - [Quatto API Source].Environment: @[$Project::Environment]

Flat File Connection (errors):
  - ConnectionString: @[User::$PackageLocation] + "\\errors\\gladium_orders_" 
                      + (DT_WSTR,8)DATEPART("year", GETDATE())
                      + RIGHT("0" + (DT_WSTR,2)DATEPART("month", GETDATE()), 2)
                      + RIGHT("0" + (DT_WSTR,2)DATEPART("day", GETDATE()), 2) + ".txt"

═══════════════════════════════════════════════════════════════════
EVENT HANDLERS (OPCIONAL)
═══════════════════════════════════════════════════════════════════

OnError:
  - Send Mail Task → Alertar equipe DBA
  - Execute SQL Task → Registrar erro detalhado

OnPostExecute:
  - Execute SQL Task → Atualizar estatísticas de execução

═══════════════════════════════════════════════════════════════════
CONFIGURAÇÃO DE LOGGING
═══════════════════════════════════════════════════════════════════

Log Provider: SSIS log provider for SQL Server
Connection: DW
Events to Log:
  - OnError
  - OnWarning
  - OnInformation
  - OnPreExecute
  - OnPostExecute
  - OnProgress

═══════════════════════════════════════════════════════════════════
DEPLOYMENT NOTES
═══════════════════════════════════════════════════════════════════

1. Criar folder SSISDB: SESCDF_DW
2. Deploy projeto com modo Project Deployment
3. Configurar environments (DEV, HML, PRD)
4. Associar parâmetros aos environments
5. Criar SQL Agent Job:
   - Frequency: Daily, 2:00 AM
   - Retry: 3 attempts, 5 minutes apart
   - Notifications: On failure

═══════════════════════════════════════════════════════════════════