# Sample 1: Execution Checklist & Guide

> Passo-a-passo completo para executar SimpleApiConsumer sample

---

## 📋 PRÉ-EXECUÇÃO CHECKLIST

### Sistema
```
✅ Windows 10/11 ou Windows Server
✅ Visual Studio 2022 instalado
✅ SSDT instalado
✅ SQL Server 2022 instalado
✅ SSIS v17.100 disponível
```

### Quatto API Client
```
✅ Quatto API Client v1.1.0 instalado
✅ Componente aparece em SSIS Toolbox
✅ DLLs copiadas para SSIS folder
```

### GitHub
```
✅ GitHub account criada
✅ Personal Access Token gerado
✅ Token armazenado em $env:GITHUB_TOKEN
✅ Token testado com API
```

### Database
```
✅ SQL Server acessível
✅ [QuattoSamples] database criado
✅ Tabelas criadas (01_Setup_Database.sql)
✅ Permissões adequadas
```

---

## 🚀 PASSO-A-PASSO EXECUÇÃO

### FASE 1: Preparação (15 minutos)

#### 1.1 Executar Setup Database Script

```sql
-- SSMS: Execute 01_Setup_Database.sql
USE master
GO
-- ... script ...
```

**Esperado:**
```
Database [QuattoSamples] created successfully
Table [GitHubRepositories] created successfully with indexes
Table [ExecutionLog] created successfully
Table [LoadStatistics] created successfully
========================================
Setup completed successfully!
```

#### 1.2 Verificar GitHub Token

```powershell
# PowerShell (como Admin)
$token = $env:GITHUB_TOKEN

if ([string]::IsNullOrEmpty($token)) {
    Write-Host "❌ Token não configurado!"
    Write-Host "Execute:"
    Write-Host '[System.Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "seu_token", "User")'
    exit
}

Write-Host "✅ Token configurado"

# Testar API
$headers = @{
    "Authorization" = "Bearer $token"
    "Accept" = "application/vnd.github.v3+json"
}

try {
    $user = Invoke-RestMethod -Uri "https://api.github.com/user" `
        -Headers $headers
    Write-Host "✅ GitHub user: $($user.login)"
}
catch {
    Write-Host "❌ Token inválido!"
    exit
}
```

#### 1.3 Verificar SQL Server Connection

```powershell
# PowerShell
$server = "localhost"
$database = "QuattoSamples"

$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = "Server=$server;Database=$database;Trusted_Connection=true;"

try {
    $conn.Open()
    Write-Host "✅ Conexão a SQL Server OK"
    
    # Verificar tabelas
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE database_id = DB_ID()"
    $tableCount = $cmd.ExecuteScalar()
    Write-Host "✅ $tableCount tabelas encontradas"
    
    $conn.Close()
}
catch {
    Write-Host "❌ Erro ao conectar: $($_.Exception.Message)"
    exit
}
```

---

### FASE 2: Criar SSIS Package (30 minutos)

#### 2.1 Criar Novo SSIS Project

```
VS2022 → File → New → Project
Selecione "Integration Services Project"
Nome: "QuattoSample1"
Location: C:\Dev\QuattoSample1
Create
```

#### 2.2 Criar Connection Managers

**ConnectionManager 1: SQL Server**
```
Solution Explorer → Right-click "Connection Managers"
"New Connection Manager" → "OLEDB"
Name: "QuattoSamplesDB"
Server: localhost
Database: QuattoSamples
Test → OK
```

**ConnectionManager 2: GitHub API**
```
Solution Explorer → Right-click "Connection Managers"
"New Connection Manager" → "Quatto API" (ou HTTP)
Name: "GitHubAPI"
Base URL: https://api.github.com
Auth Type: Bearer Token
Token: [seu GitHub token]
Test → OK
```

#### 2.3 Criar Data Flow Task

```
Control Flow → Toolbox → "Data Flow Task"
Rename: "Load GitHub Repositories"
Double-click para abrir Data Flow
```

#### 2.4 Adicionar Components

**Corporate API Source**
```
Toolbox → Quatto Components → Corporate API Source
Configure:
├─ Connection: GitHubAPI
├─ Endpoint: /user/repos
├─ Page Size: 30
├─ Timeout: 30
└─ OK
```

**OLE DB Destination**
```
Toolbox → Destinations → OLE DB Destination
Configure:
├─ Connection: QuattoSamplesDB
├─ Table: [dbo].[GitHubRepositories]
└─ Mappings (vide passo 2.5)
```

#### 2.5 Mapear Colunas

```
OLE DB Destination → Mappings tab

GitHub API JSON    →  SQL Column
─────────────────────────────────
id                 →  RepositoryId
name               →  Name
full_name          →  FullName
description        →  Description
html_url           →  Url
stargazers_count   →  Stars
forks_count        →  Forks
language           →  Language
created_at         →  CreatedAt
updated_at         →  UpdatedAt

(Outros campos podem ser ignorados)
```

**Como mapear:**
```
1. Na aba "Mappings"
2. Coluna input apareça no lado esquerdo
3. Arraste para coluna SQL correspondente
4. Se não aparecer campo:
   a. Input Columns: click para expandir
   b. Selecione o campo
5. OK
```

---

### FASE 3: Testar Package (15 minutos)

#### 3.1 Executar Package

```
Debug → Start Debugging (F5)
ou
Right-click Package → Execute Package
```

**Monitorar:**
- Abra "Data Flow Path Execution Results"
- Veja progresso de linhas processadas
- Aguarde conclusão

#### 3.2 Verificar Execução

```powershell
# Se executado com sucesso, deve ver:
# ✅ Corporate API Source: X rows
# ✅ OLE DB Destination: X rows inserted
# ✅ Data Flow completed
```

#### 3.3 Validar Dados no SQL

```sql
-- SSMS: Execute queries de validação
USE [QuattoSamples]

-- Contar registros
SELECT COUNT(*) as TotalRecords FROM [dbo].[GitHubRepositories]

-- Ver amostra
SELECT TOP 10 Name, Stars, Language 
FROM [dbo].[GitHubRepositories]
ORDER BY Stars DESC

-- Verificar linguagens
SELECT DISTINCT Language, COUNT(*) as Count
FROM [dbo].[GitHubRepositories]
WHERE Language IS NOT NULL
GROUP BY Language
ORDER BY Count DESC
```

---

### FASE 4: Análise de Resultados (10 minutos)

#### 4.1 Executar Validation Queries

```sql
-- SSMS: Execute 02_Validation_Queries.sql
-- Vê estatísticas completas
```

#### 4.2 Analisar Resultados

```
Esperado:
✅ 20-50 repositórios carregados
✅ Múltiplas linguagens
✅ Stars distribuídos
✅ Datas válidas
✅ URLs corretas
```

#### 4.3 Exemplos de Análise

```powershell
# Analisar linguagens mais populares
$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = "Server=localhost;Database=QuattoSamples;Trusted_Connection=true;"
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
SELECT TOP 5 Language, COUNT(*) as Count, AVG(Stars) as AvgStars
FROM [dbo].[GitHubRepositories]
WHERE Language IS NOT NULL
GROUP BY Language
ORDER BY Count DESC
"@

$reader = $cmd.ExecuteReader()
Write-Host "Top Languages:"
while ($reader.Read()) {
    Write-Host "$($reader['Language']): $($reader['Count']) repos, ⭐ avg $([math]::Round($reader['AvgStars'],1))"
}

$reader.Close()
$conn.Close()
```

---

## 🎯 Próximos Passos

### Opção 1: Expandir com Transformações
```
1. Adicionar Derived Column transform
2. Adicionar Conditional Split
3. Implementar error handling
4. Schedule para execução diária
```

### Opção 2: Próximo Sample
```
1. Ir para Sample 2: Advanced Data Pipeline
2. Aprender sobre múltiplas APIs
3. Implementar incremental load
4. Setup watermark
```

### Opção 3: Performance Tuning
```
1. Aumentar page size
2. Adicionar caching
3. Implementar parallel processing
4. Benchmark e otimizar
```

---

## 🆘 Se Algo Não Funcionar

### Checklist de Troubleshooting

```
❌ Package não executa:
   → Verificar Data Flow connections (linhas vermelhas)
   → Verificar column mappings
   → Verifique error output

❌ API Connection Error:
   → Testar GitHub API manualmente (PowerShell)
   → Verificar token em variável de ambiente
   → Verificar firewall/proxy

❌ Database Insert Error:
   → Verificar tipos de dados
   → Verificar column names
   → Verificar permissões SQL

❌ No records loaded:
   → Verificar GitHub token
   → Verificar endpoint URL (/user/repos)
   → Testar API response manualmente
   → Aumentar timeout
```

---

## 📊 Resumo de Tempo

```
Preparação:         15 minutos
Criar Package:      30 minutos
Testar:             15 minutos
Análise:            10 minutos
─────────────────────────────
TOTAL:              ~70 minutos (1h 10min)
```

---

## ✅ Sucesso!

Quando você ver:
```
✅ Package executes without errors
✅ Data appears in [GitHubRepositories]
✅ Validation queries show records
✅ No duplicate IDs
✅ All columns populated
```

**Parabéns!** Sample 1 foi executado com sucesso! 🎉

---

**Próximo:** [Sample 2: Advanced Data Pipeline](../02_AdvancedDataPipeline/README.md)

