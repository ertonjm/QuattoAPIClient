# SSIS Package Setup - Sample 1

> Guia passo-a-passo para criar e configurar SSIS package

---

## 📋 Pré-requisitos

```
✅ Visual Studio 2022 com SSDT
✅ SQL Server 2022 com SSIS v17.100
✅ Quatto API Client instalado
✅ Database [QuattoSamples] criado (01_Setup_Database.sql)
✅ GitHub API token configurado (02_GitHub_API_Setup.md)
```

---

## 🔌 Passo 1: Criar Connection Manager

### Step 1.1: Abrir SSIS Project

```
1. Visual Studio → File → New → Project
2. Selecione "Integration Services Project"
3. Nome: "QuattoSamples"
4. Location: C:\Dev\QuattoSamples
5. Clique Create
```

### Step 1.2: Criar Connection Manager para SQL Server

```
1. Solution Explorer → Right-click "Connection Managers"
2. Selecione "New Connection Manager"
3. Type: OLE DB
4. Nome: "QuattoSamplesDB"
5. Configure:
   - Server: localhost (ou seu servidor SQL)
   - Database: QuattoSamples
   - Authentication: Windows Authentication (ou SQL Auth)
6. Test Connection → OK
7. Click OK
```

### Step 1.3: Criar Connection Manager para GitHub API

```
1. Solution Explorer → Right-click "Connection Managers"
2. Selecione "Quatto API Connection" (ou similar)
3. Configure:
   - Name: "GitHubAPI"
   - Connection Type: HTTP
   - Base URL: https://api.github.com
   - Authentication Type: Bearer Token
   - Token: [seu GitHub token]
   - Timeout: 30 seconds
4. Test Connection → OK
5. Click OK
```

---

## 🔄 Passo 2: Criar Control Flow

### Step 2.1: Adicionar Data Flow Task

```
1. Toolbox → Integration Services Tasks → Data Flow Task
2. Drag para Control Flow
3. Rename para "Load GitHub Repositories"
4. Double-click para abrir Data Flow
```

---

## 📊 Passo 3: Configurar Data Flow

### Step 3.1: Adicionar Corporate API Source

```
1. Data Flow → Toolbox → Quatto Components → Corporate API Source
2. Drag para Data Flow design surface
3. Name: "GitHub API Source"
4. Double-click para configurar:
   - Connection Manager: GitHubAPI
   - Base URL: https://api.github.com
   - Endpoint: /user/repos
   - Page Size: 30
   - Timeout: 30 seconds
   - Max Retries: 3
```

### Step 3.2: Configurar Data Conversion (opcional)

```
1. Toolbox → Other Transforms → Derived Column
2. Drag para Data Flow
3. Connect Corporate API Source → Derived Column
4. Configure:
   - Add LoadDate: GETDATE()
   - Add Source: "GitHub"
5. Click OK
```

### Step 3.3: Adicionar OLE DB Destination

```
1. Toolbox → Destinations → OLE DB Destination
2. Drag para Data Flow
3. Connect Derived Column → OLE DB Destination
4. Name: "Load to GitHubRepositories"
5. Double-click para configurar:
   - Connection Manager: QuattoSamplesDB
   - Table: [dbo].[GitHubRepositories]
6. Clique "Mappings"
```

### Step 3.4: Mapear Colunas

```
GitHub JSON Field    →  SQL Column
────────────────────────────────────
id                   →  RepositoryId
name                 →  Name
full_name            →  FullName
description          →  Description
html_url             →  Url
stargazers_count     →  Stars
forks_count          →  Forks
language             →  Language
created_at           →  CreatedAt
updated_at           →  UpdatedAt
(derivado) LoadDate  →  LoadedAt
(derivado) Source    →  (ignore)
```

**Como mapear:**
```
1. Na aba "Mappings"
2. Arraste do campo Input para o campo Destination
3. Se campo não aparecer:
   - Click "Create New" button
   - Nome o campo novo
4. Click OK
```

---

## 🔧 Passo 4: Adicionar Error Handling

### Step 4.1: Configurar Error Output

```
1. Corporate API Source → direita → Red output (Error)
2. Arraste para uma nova transform ou destino
3. Crie "ErrorOutput" destination para logar erros
```

### Step 4.2: Adicionar Execute SQL Task (Logging)

```
1. Control Flow → Toolbox → Execute SQL Task
2. Arrastar para Control Flow
3. Connect "Load GitHub Repositories" → Execute SQL Task
4. Configure:
   - Connection: QuattoSamplesDB
   - SQL Command:
   
   INSERT INTO [dbo].[ExecutionLog]
   ([PackageName], [ExecutionStatus], [StartTime], [RecordsLoaded])
   VALUES
   (?, ?, ?, ?)
   
5. Parameter Mapping:
   - Parameter 0: @[System::PackageName]
   - Parameter 1: "Success" ou "Failed"
   - Parameter 2: @[System::StartTime]
   - Parameter 3: ?variable para registros carregados
```

---

## 🎯 Passo 5: Testar Package

### Step 5.1: Executar Package

```
1. Debug → Start Debugging (F5)
   ou
2. Right-click package → Execute Package
```

### Step 5.2: Monitorar Execução

```
Espere por:
✅ Connection to GitHub API established
✅ Data fetched from GitHub
✅ Data transformed
✅ Data inserted into [GitHubRepositories]
✅ Execution Log updated
✅ Package completed successfully
```

### Step 5.3: Verificar Erros

```
Se houver erros:
- Veja Output window para detalhes
- Verifique Data Flow Path execution
- Check logs em [ExecutionLog]
- Review GitHub API response
```

---

## 📊 Passo 6: Validar Dados

### Step 6.1: Executar Validation Queries

```
SQL Server Management Studio:
1. Connect a [QuattoSamples]
2. Execute 02_Validation_Queries.sql
3. Verifique:
   - Table structure OK
   - Record count > 0
   - Data looks correct
```

### Step 6.2: Exemplos de Validação

```powershell
# PowerShell script para validar
$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = "Server=localhost;Database=QuattoSamples;Trusted_Connection=true;"
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM [dbo].[GitHubRepositories]"
$count = $cmd.ExecuteScalar()

Write-Host "Total repositories loaded: $count"

$cmd.CommandText = "SELECT TOP 5 Name, Stars FROM [dbo].[GitHubRepositories] ORDER BY Stars DESC"
$reader = $cmd.ExecuteReader()

while ($reader.Read()) {
    Write-Host "$($reader['Name']): ⭐ $($reader['Stars'])"
}

$reader.Close()
$conn.Close()
```

---

## 🚀 Passo 7: Schedule Package (Opcional)

### Step 7.1: Deploy Package

```
1. Solution Explorer → Right-click Package
2. Deploy
3. Selecione target server e folder
```

### Step 7.2: Schedule no SQL Agent

```
SQL Server Management Studio:
1. SQL Server Agent → Jobs → New Job
2. Name: "QuattoSample1_DailyLoad"
3. Steps → New Step:
   - Type: Integration Services Package
   - Package: [Your Package]
   - Schedule: Daily, 2 AM
4. OK
```

---

## 📋 Troubleshooting

### Erro: "Connection failed"

**Causa:** GitHub API ou SQL Server inaccessível

**Solução:**
```
1. Verificar GitHub token em variável de ambiente
2. Verificar conectividade a SQL Server
3. Reabrir VS depois de configurar variáveis
4. Testar connections manualmente
```

### Erro: "No columns found"

**Causa:** Quatto component não retornou schema

**Solução:**
```
1. Verificar GitHub endpoint (/user/repos)
2. Testar API manualmente (PowerShell)
3. Verificar token permissions
4. Check component configuration
```

### Erro: "Insert failed"

**Causa:** Conflito de tipo de dados ou constraint

**Solução:**
```
1. Verificar data types em mapping
2. Converter valores se necessário
3. Verificar duplicate IDs (RepositoryId é PK)
4. Aumentar field sizes se necessário
```

---

## ✅ Checklist Final

```
Pré-execução:
✅ VS2022 + SSDT instalado
✅ Quatto API Client instalado
✅ Database e tabelas criados
✅ GitHub token configurado
✅ Connections configuradas (2x)
✅ Data Flow completo
✅ Mapping correto

Execução:
✅ Package executa sem erros
✅ Records carregados > 0
✅ ExecutionLog preenchido
✅ Dados visíveis em SSMS

Pós-execução:
✅ Validação queries executadas
✅ Record counts OK
✅ Data quality verificada
✅ No duplicate IDs
```

---

## 📚 Próximos Passos

```
1. Adicionar transformações (Derived Column, Conditional Split)
2. Implementar incremental load (Sample 2)
3. Adicionar error handling avançado
4. Schedule para execução automática
```

---

**Tempo estimado:** 1-1.5 horas  
**Dificuldade:** Média 📊

