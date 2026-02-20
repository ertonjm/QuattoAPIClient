# Troubleshooting - Quatto API Client v1.0

## 📋 Índice

1. [Problemas de Instalação](#problemas-de-instalação)
2. [Problemas de Autenticação](#problemas-de-autenticação)
3. [Problemas de Performance](#problemas-de-performance)
4. [Problemas de Execução](#problemas-de-execução)
5. [Problemas de Dados](#problemas-de-dados)
6. [Ferramentas de Diagnóstico](#ferramentas-de-diagnóstico)

---

## 🔧 Problemas de Instalação

### Componente não aparece na Toolbox

**Sintomas:**
- Após instalação, componente não aparece no SSIS Toolbox
- Apenas componentes padrão do SSIS visíveis

**Diagnóstico:**
```powershellVerificar se DLLs foram copiadas
$paths = @(
"C:\Program Files\Microsoft SQL Server\150\DTS\PipelineComponents\QuattoAPIClient.Source.dll",
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\PublicAssemblies\QuattoAPIClient.Source.dll"
)foreach ($path in $paths) {
if (Test-Path $path) {
Write-Host "✓ $path" -ForegroundColor Green
[System.Reflection.Assembly]::LoadFile($path).GetName().Version
} else {
Write-Host "✗ $path" -ForegroundColor Red
}
}

**Soluções:**

1. **Reiniciar Visual Studio (CRÍTICO)**
```powershellFechar TODAS as instâncias
Get-Process devenv | Stop-Process -ForceLimpar cache (opcional)
Remove-Item "$env:LOCALAPPDATA\Microsoft\VisualStudio*\ComponentModelCache" -Recurse -Force

2. **Reinstalar componente**
```powershellcd C:\Dev\QuattoAPIClient\deployment
.\Deploy-QuattoAPIClient.ps1 -SourcePath "C:\Dev\QuattoAPIClient" -SkipValidation

3. **Verificar versão do .NET Framework**
```powershellComponente requer .NET Framework 4.7.2+
Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' |
Get-ItemProperty -Name Version | Select-Object Version

---

### Erro: "Could not load file or assembly"

**Sintomas:**System.IO.FileNotFoundException: Could not load file or assembly
'Microsoft.SqlServer.DTSPipelineWrap, Version=15.0.0.0' or one of its dependencies.

**Causa:** Referências SSIS não encontradas ou versão incorreta

**Solução:**
```xml<!-- Editar QuattoAPIClient.Source.csproj -->
<Reference Include="Microsoft.SqlServer.DTSPipelineWrap">
  <!-- Ajustar caminho conforme instalação -->
  <HintPath>C:\Program Files\Microsoft SQL Server\150\SDK\Assemblies\Microsoft.SqlServer.DTSPipelineWrap.dll</HintPath>
  <Private>False</Private> <!-- IMPORTANTE -->
</Reference>
```
Mapeamento de versões:

SQL Server 2017: 140
SQL Server 2019: 150
SQL Server 2022: 160


Erro: "Strong name validation failed"
Sintomas:
Exception: Strong name validation failed for assembly 'QuattoAPIClient.Source'

**Causa:** AssemblyKeyFile configurado mas arquivo .snk não existe

**Solução:**
```csharp// Comentar em AssemblyInfo.cs:
// [assembly: AssemblyKeyFile("..\..\..\QuattoAPIClient.snk")]// OU gerar strong name key:
sn -k C:\Dev\QuattoAPIClient\QuattoAPIClient.snk

---

## 🔐 Problemas de Autenticação

### Erro 401 Unauthorized

**Sintomas:**HTTP 401: Unauthorized
ErrorMessage: Invalid or expired token

**Diagnóstico:**
```sql-- Verificar último erro na API_ExecutionLog
SELECT TOP 5
SystemName,
Endpoint,
ExecutionStartedUtc,
Status,
ErrorMessage
FROM dbo.API_ExecutionLog
WHERE Status = 'FAILED'
ORDER BY ExecutionStartedUtc DESC;

**Soluções:**

1. **Verificar token no Connection Manager**Connection Managers → GladiumAPI → Properties → BearerToken

2. **Testar token manualmente (PowerShell)**
```powershell$token = "eyJhbGci..."
$headers = @{
"Authorization" = "Bearer $token"
"Accept" = "application/json"
}Invoke-RestMethod -Uri "https://api.gladium.com/v1/orders?page=1&pageSize=10" -Headers $headers

3. **Token expirado (OAuth2)**Connection Manager → AuthType: OAuth2ClientCredentials

Verificar ClientId/ClientSecret corretos
Testar TokenEndpoint manualmente


---

### Erro 429 Too Many Requests

**Sintomas:**HTTP 429: Too Many Requests
Retry-After: 60

**Diagnóstico:**
```sql-- Verificar histórico de throttling
SELECT
SystemName,
COUNT(*) AS ThrottledRequests,
AVG(CAST(JSON_VALUE(ErrorMessage, '$.WaitSeconds') AS INT)) AS AvgWaitSec
FROM dbo.API_ExecutionLog
WHERE ErrorMessage LIKE '%429%'
AND ExecutionStartedUtc >= DATEADD(HOUR, -24, GETUTCDATE())
GROUP BY SystemName;

**Soluções:**

1. **Reduzir RateLimitRPM**Component Properties → RateLimitRPM: 60 (reduzir de 120)

2. **Implementar rate limit global**
```sql-- Atualizar limites no database
UPDATE dbo.API_RateLimitControl
SET MaxRequestsPerMinute = 60,
MaxRequestsPerHour = 2000
WHERE SystemName = 'Gladium';

3. **Distribuir carga (múltiplos pacotes)**Ao invés de: 1 pacote com PageSize=1000
Usar: 2 pacotes com PageSize=500 espaçados em 30min

---

## ⚡ Problemas de Performance

### Extração muito lenta

**Sintomas:**
- Pacote leva horas para completar
- Latências altas (>5s por request)

**Diagnóstico:**
```sql-- Análise de performance por sistema
SELECT
SystemName,
Endpoint,
COUNT(*) AS Executions,
AVG(DurationMs) / 1000.0 AS AvgDuration_Sec,
AVG(TotalRecords) AS AvgRecords,
AVG(AvgLatencyMs) AS AvgLatency_Ms,
SUM(ThrottledRequests) AS TotalThrottled
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -7, GETUTCDATE())
AND Status = 'SUCCESS'
GROUP BY SystemName, Endpoint
ORDER BY AvgDuration_Sec DESC;

**Soluções:**

1. **Aumentar PageSize**Component Properties → PageSize: 1000 (aumentar de 500)ATENÇÃO: Verificar limite da API antes

2. **Otimizar Schema Mapping**
```json// Mapear apenas campos necessários
{
"columns": [
{"name": "id", "path": "$.id", "type": "DT_WSTR", "length": 50},
{"name": "updated_at", "path": "$.updatedAt", "type": "DT_DBTIMESTAMP2"}
// Remover campos não utilizados
]
}

3. **Desabilitar Raw Storage (temporário)**Component Properties → RawStoreMode: None

4. **Paralelizar extração**Criar múltiplos pacotes:

API_Gladium_Orders_A (IDs 1-10000)
API_Gladium_Orders_B (IDs 10001-20000)
Executar em paralelo via SQL Agent Jobs

---

### Timeout frequente

**Sintomas:**TaskCanceledException: A task was canceled.
Timeout: 100s exceeded

**Diagnóstico:**
```sql-- Identificar endpoints com timeout
SELECT
SystemName,
Endpoint,
COUNT(*) AS TimeoutCount,
AVG(MaxLatencyMs) / 1000.0 AS MaxLatency_Sec
FROM dbo.API_ExecutionLog
WHERE ErrorMessage LIKE '%timeout%'
AND ExecutionStartedUtc >= DATEADD(DAY, -7, GETUTCDATE())
GROUP BY SystemName, Endpoint
ORDER BY TimeoutCount DESC;

**Soluções:**

1. **Aumentar timeout**Connection Manager → TimeoutSeconds: 300 (aumentar de 100)

2. **Reduzir PageSize**Component Properties → PageSize: 200 (reduzir de 500)

3. **Verificar conectividade**
```powershellTest-NetConnection -ComputerName api.gladium.com -Port 443

---

## 🐛 Problemas de Execução

### Erro: "Connection Manager não configurado"

**Sintomas:**ValidationError: The connection manager API is not configured properly.

**Solução:**
Abrir componente (duplo clique)
Tab "General" → Connection Manager
Selecionar "GladiumAPI" (ou criar novo)
OK → Salvar pacote


---

### Erro: "Watermark table does not exist"

**Sintomas:**SqlException: Invalid object name 'dbo.API_Watermarks'

**Solução:**
```sql-- Verificar se tabela existe
SELECT * FROM sys.tables WHERE name = 'API_Watermarks';-- Se não existir, executar:
-- C:\Dev\QuattoAPIClient\database\01_Complete_Database_Setup.sql

---

### Erro: "Schema mapping failed"

**Sintomas:**InvalidOperationException: Cannot map field '$.customer.id' - property not found in JSON

**Diagnóstico:**
```sql-- Examinar JSON bruto
SELECT TOP 1
ResponseBodyGzip,
CAST(DECOMPRESS(ResponseBodyGzip) AS NVARCHAR(MAX)) AS JsonContent
FROM dbo.API_RawPayloads
WHERE SystemName = 'Gladium'
AND Endpoint = '/v1/orders'
ORDER BY CollectedUtc DESC;

**Solução:**
```json// Corrigir path no Schema Mapping
// Se JSON é:
{ "customer": { "customerId": "123" } }// Usar:
{"name": "customer_id", "path": "$.customer.customerId", ...}

---

## 📊 Problemas de Dados

### Watermark não avança

**Sintomas:**
- Pacote executa mas sempre retorna mesmos registros
- Watermark não muda no database

**Diagnóstico:**
```sql-- Verificar histórico de watermark
SELECT
SystemName,
Endpoint,
LastWatermark,
LastRunUtc,
TotalRecordsExtracted,
UpdatedUtc
FROM dbo.API_Watermarks
WHERE SystemName = 'Gladium'
ORDER BY UpdatedUtc DESC;

**Soluções:**

1. **Verificar WatermarkColumn**Component Properties → WatermarkColumn: updatedAtATENÇÃO: Campo deve existir na API response

2. **Forçar reset de watermark (cuidado!)**
```sql-- Resetar para data específica
UPDATE dbo.API_Watermarks
SET LastWatermark = '2026-01-01T00:00:00Z',
LastRunUtc = NULL
WHERE SystemName = 'Gladium'
AND Endpoint = '/v1/orders';

---

### Duplicatas na staging

**Sintomas:**
- Registros duplicados na tabela staging
- Violação de constraint UNIQUE

**Diagnóstico:**
```sql-- Identificar duplicatas
SELECT
order_id,
COUNT() AS Occurrences
FROM stg.Gladium_Orders
GROUP BY order_id
HAVING COUNT() > 1
ORDER BY COUNT(*) DESC;

**Soluções:**

1. **Habilitar Hash em Raw Storage**Component Properties → HashRawJson: true

2. **Truncar staging antes da carga**
```sql-- Adicionar Execute SQL Task antes do Data Flow:
TRUNCATE TABLE stg.Gladium_Orders;

3. **Usar MERGE no destino**
```sqlMERGE stg.Gladium_Orders AS target
USING (SELECT * FROM #TempSource) AS source
ON target.order_id = source.order_id
WHEN MATCHED THEN UPDATE SET ...
WHEN NOT MATCHED THEN INSERT ...;

---

## 🔍 Ferramentas de Diagnóstico

### Dashboard SQL - KPIs
```sql-- Executar diariamente
WITH LastDay AS (
SELECT
SystemName,
COUNT(*) AS Executions,
SUM(TotalRecords) AS TotalRecords,
AVG(DurationMs) / 1000.0 AS AvgDuration_Sec,
SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) AS SuccessCount,
SUM(CASE WHEN Status = 'FAILED' THEN 1 ELSE 0 END) AS FailureCount
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(DAY, -1, GETUTCDATE())
GROUP BY SystemName
)
SELECT
SystemName,
Executions,
TotalRecords,
AvgDuration_Sec,
CAST(SuccessCount * 100.0 / NULLIF(Executions, 0) AS DECIMAL(5,2)) AS SuccessRate_Pct,
CASE
WHEN SuccessRate_Pct >= 95 THEN '🟢 OK'
WHEN SuccessRate_Pct >= 80 THEN '🟡 WARNING'
ELSE '🔴 CRITICAL'
END AS Status
FROM LastDay
ORDER BY SuccessRate_Pct;

### Logs Detalhados
```sql-- Ver última execução com detalhes
SELECT TOP 1
CorrelationID,
PackageName,
SystemName,
Endpoint,
ExecutionStartedUtc,
DurationMs / 1000.0 AS Duration_Sec,
TotalRequests,
SuccessfulRequests,
FailedRequests,
RetriedRequests,
ThrottledRequests,
TotalRecords,
AvgLatencyMs,
Status,
ErrorMessage
FROM dbo.API_ExecutionLog
WHERE SystemName = 'Gladium'
ORDER BY ExecutionStartedUtc DESC;

### Script de Health Check
```powershellSalvar como: C:\Dev\QuattoAPIClient\tools\HealthCheck.ps1$server = "SQL-SERVER\INSTANCE"
$database = "SESCDF_DW"$query = @"
SELECT
'Watermarks' AS CheckName,
COUNT() AS RecordCount,
CASE WHEN COUNT() > 0 THEN 'OK' ELSE 'FAIL' END AS Status
FROM dbo.API_WatermarksUNION ALLSELECT
'Raw Payloads (24h)',
COUNT(),
CASE WHEN COUNT() > 0 THEN 'OK' ELSE 'WARNING' END
FROM dbo.API_RawPayloads
WHERE CollectedUtc >= DATEADD(HOUR, -24, GETUTCDATE())UNION ALLSELECT
'Executions (24h)',
COUNT(),
CASE WHEN COUNT() > 0 THEN 'OK' ELSE 'WARNING' END
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(HOUR, -24, GETUTCDATE())UNION ALLSELECT
'Failed Executions (24h)',
COUNT(),
CASE WHEN COUNT() = 0 THEN 'OK' ELSE 'WARNING' END
FROM dbo.API_ExecutionLog
WHERE ExecutionStartedUtc >= DATEADD(HOUR, -24, GETUTCDATE())
AND Status = 'FAILED';
"@Invoke-Sqlcmd -ServerInstance $server -Database $database -Query $query | Format-Table

---

## 📞 Suporte Técnico

### Antes de abrir chamado

✅ Executar Health Check  
✅ Coletar logs de `dbo.API_ExecutionLog`  
✅ Gerar relatório de deployment  
✅ Verificar documentação  

### Informações necessárias

1. Versão do componente
2. Versão do SQL Server / SSIS
3. Mensagem de erro completa
4. Logs de execução (últimas 5 execuções)
5. Schema Mapping JSON
6. Screenshot da configuração

### Contato

**Email:** erton.miranda@quatto.com.br  
**Projeto:** SESC-DF Data Warehouse  
**SLA:** 24h úteis

---

**Documentação Relacionada:**
- [01_INSTALLATION.md](01_INSTALLATION.md)
- [02_CONFIGURATION.md](02_CONFIGURATION.md)
- [05_API_REFERENCE.md](05_API_REFERENCE.md)