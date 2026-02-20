# 🎯 SAMPLE 1: SimpleApiConsumer - Complete Package

> Tudo que você precisa para executar SimpleApiConsumer sample com GitHub API

---

## 📦 Conteúdo Incluindo

```
samples/01_SimpleApiConsumer/
├─ 01_Setup_Database.sql          (Database + tabelas)
├─ 02_Validation_Queries.sql      (Queries de validação)
├─ 02_GitHub_API_Setup.md         (10 seções, troubleshooting)
├─ 03_SSIS_Package_Setup.md       (7 passos, column mapping)
├─ 04_Execution_Checklist.md      (Passo-a-passo 75 min)
└─ README.md                       (Este arquivo)

TOTAL: 5 Documentos + 2 Scripts SQL + ~2,500 linhas
```

---

## 🚀 Quick Start (75 minutos)

**Fase 1: Preparação (15 min)**
1. Executar `01_Setup_Database.sql`
2. Gerar GitHub token
3. Testar API

**Fase 2: Criar Package (30 min)**
4. VS2022: Novo SSIS project
5. 2x Connection managers
6. Data Flow + Mapping

**Fase 3: Testar (15 min)**
7. Execute Package
8. Validar dados

**Fase 4: Análise (10 min)**
9. Executar validation queries
10. Análise resultados

---

## 🎯 Objetivo

Criar um SSIS package que consome a GitHub API e carrega dados em uma tabela SQL Server.

**Dificuldade:** Beginner → Intermediate  
**Tempo Total:** 75-90 minutos  
**Conceitos:** Basic API integration, SSIS Data Flow, column mapping

---

## ✅ Pré-requisitos

- ✅ Visual Studio 2022 com SSDT
- ✅ SQL Server 2022 com SSIS v17.100
- ✅ Quatto API Client v1.1.0 instalado
- ✅ GitHub account
- ✅ .NET Framework 4.7.2+

---

## 🔑 GitHub API Setup

### Passo 1: Criar Personal Access Token

```
1. Vá a https://github.com/settings/tokens
2. Click "Generate new token (classic)"
3. Nome: "Quatto API Client"
4. Scopes: public_repo, read:user
5. Copy token (salve em local seguro!)
```

**Token Example:**
```
ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

### Passo 2: Teste o Token

```powershell
# Test GitHub API
$token = "your-token-here"
$headers = @{
    "Authorization" = "Bearer $token"
    "Accept" = "application/vnd.github.v3+json"
}

$response = Invoke-RestMethod -Uri "https://api.github.com/user" `
    -Headers $headers

Write-Host "GitHub User: $($response.login)"
```

---

## 🗄️ Database Setup

### Passo 1: Criar Database

```sql
-- Create sample database
CREATE DATABASE [QuattoSamples]
GO

USE [QuattoSamples]
GO
```

### Passo 2: Criar Tabela de Destino

```sql
-- Create GitHubRepositories table
CREATE TABLE [dbo].[GitHubRepositories](
    [RepositoryId] [bigint] PRIMARY KEY,
    [Name] [nvarchar](255) NOT NULL,
    [FullName] [nvarchar](255) NOT NULL,
    [Description] [nvarchar](max),
    [Url] [nvarchar](500),
    [Stars] [int],
    [Forks] [int],
    [Language] [nvarchar](50),
    [CreatedAt] [datetime2],
    [UpdatedAt] [datetime2],
    [LoadedAt] [datetime2] DEFAULT GETDATE()
)
GO

-- Create index
CREATE NONCLUSTERED INDEX IX_Name ON [dbo].[GitHubRepositories]([Name])
GO
```

---

## 🔌 Configurar SSIS Package

### Passo 1: Criar Connection Manager

```
1. Abra Visual Studio
2. Create new Integration Services Project
3. Right-click Connections → New Connection Manager
4. Select "Quatto API Connection"
5. Configure:
   - Name: "GitHubAPI"
   - Base URL: https://api.github.com
   - Authentication: Bearer Token
   - Token: [seu GitHub token]
```

### Passo 2: Criar Data Flow Task

```
1. Add "Data Flow Task" ao Control Flow
2. Double-click para abrir Data Flow
3. Add "Corporate API Source" component
4. Configure:
   - Connection Manager: GitHubAPI
   - Base URL: https://api.github.com
   - Endpoint: /user/repos
   - Page Size: 30
   - Timeout: 30 seconds
```

### Passo 3: Mapear Colunas

```
GitHub JSON              → SQL Column
├─ id                    → RepositoryId
├─ name                  → Name
├─ full_name             → FullName
├─ description           → Description
├─ html_url              → Url
├─ stargazers_count      → Stars
├─ forks_count           → Forks
├─ language              → Language
├─ created_at            → CreatedAt
└─ updated_at            → UpdatedAt
```

### Passo 4: Adicionar Destination

```
1. Add "OLE DB Destination" component
2. Configure:
   - Connection: SQL Server (QuattoSamples)
   - Table: [dbo].[GitHubRepositories]
3. Map columns
```

### Passo 5: Adicionar Error Handling

```
1. Add "OnError" event handler
2. Add "Execute SQL Task" para log
3. Log errors em tabela de auditoria
```

---

## 📊 Tabela de Auditoria (Optional)

```sql
-- Create audit table for errors
CREATE TABLE [dbo].[ExecutionLog](
    [LogId] [int] IDENTITY(1,1) PRIMARY KEY,
    [PackageName] [nvarchar](255),
    [ExecutionStatus] [nvarchar](50),
    [ErrorMessage] [nvarchar](max),
    [ExecutionTime] [datetime2] DEFAULT GETDATE()
)
GO
```

---

## 🚀 Executar Package

### Método 1: Visual Studio

```
1. Right-click package → Execute Package
2. Aguarde conclusão
3. Verifique status no Output window
```

### Método 2: Command Line

```powershell
# Run SSIS package via DTExec
dtexec /f "C:\path\to\package.dtsx"
```

### Esperado

```
✅ Package started
✅ Connected to GitHub API
✅ Fetched X repositories
✅ Loaded data to SQL Server
✅ Package completed successfully
```

---

## 📊 Verificar Dados

```sql
-- Check loaded data
SELECT TOP 10 * FROM [dbo].[GitHubRepositories]
ORDER BY Stars DESC

-- Check record count
SELECT COUNT(*) as RepositoryCount 
FROM [dbo].[GitHubRepositories]

-- Check by language
SELECT Language, COUNT(*) as Count
FROM [dbo].[GitHubRepositories]
WHERE Language IS NOT NULL
GROUP BY Language
ORDER BY Count DESC
```

---

## 🔍 Logs e Debugging

### Consultar Logs do SSIS

```
1. Open Visual Studio
2. View → Output (Ctrl+Alt+O)
3. Veja logs de execução
4. Procure por erros ou avisos
```

### Ativar Detailed Logging

```
1. Right-click package → Logging
2. Enable logging para todos os tasks
3. Configure log provider (Text File ou SQL Server)
4. Re-run package
5. Analise logs detalhados
```

---

## 🆘 Troubleshooting

### Erro: "Unauthorized (401)"

**Causa:** Token inválido ou expirado  
**Solução:**
```
1. Gere novo token no GitHub
2. Atualize connection manager
3. Re-run package
```

### Erro: "Connection Timeout"

**Causa:** GitHub API lento ou indisponível  
**Solução:**
```
1. Aumente timeout para 60 seconds
2. Tente mais tarde
3. Verifique GitHub status page
```

### Erro: "Rate Limit Exceeded"

**Causa:** Muitas requisições  
**Solução:**
```
1. Aumente page size (reduz requisições)
2. Aguarde rate limit reset
3. Configure retry logic
```

---

## 📈 Próximos Passos

### Melhorias Possíveis

```
1. Adicionar incremental load (Sample 2)
2. Adicionar transformação de dados
3. Adicionar error handling avançado
4. Adicionar scheduling
5. Adicionar monitoring
```

### Expandir para Múltiplas APIs

```
1. GitHub API
2. GitLab API
3. Bitbucket API
4. Outros repositórios
```

---

## 📚 Conceitos Aprendidos

- ✅ Componente básico do Quatto API Client
- ✅ Configuração de connection manager
- ✅ Mapeamento de dados JSON → SQL
- ✅ Error handling em SSIS
- ✅ Execution e debugging

---

## 🎓 Próximo Sample

Após completar este sample, avance para:

**[Sample 2: Advanced Data Pipeline](../02_AdvancedDataPipeline/README.md)**

Aprenda sobre:
- Incremental loads com watermark
- Múltiplas fontes de dados
- Transformação avançada

---

## 📝 Recursos

- [Quatto API Client Repository](https://github.com/ertonjm/QuattoAPIClient)
- [GitHub API Documentation](https://docs.github.com/en/rest)
- [SSIS Data Flow](https://learn.microsoft.com/sql/integration-services/data-flow/)
- [Quatto API Client Docs](../MAIN_README.md)

---

**Tempo estimado:** 30 minutos  
**Nível:** Beginner  
**Status:** ✅ Ready to use

