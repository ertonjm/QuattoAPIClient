# Guia de Instalação - Quatto API Client v1.0

## 📋 Índice

1. [Pré-Requisitos](#pré-requisitos)
2. [Preparação do Ambiente](#preparação-do-ambiente)
3. [Instalação do Database](#instalação-do-database)
4. [Compilação da Solution](#compilação-da-solution)
5. [Deploy dos Componentes](#deploy-dos-componentes)
6. [Validação](#validação)
7. [Troubleshooting](#troubleshooting)

---

## 🔧 Pré-Requisitos

### Software Obrigatório

- ✅ **SQL Server 2019+** (ou SQL Server 2017 com ajustes)
- ✅ **Visual Studio 2019+** com:
  - SQL Server Data Tools (SSDT)
  - .NET Framework 4.7.2 SDK
  - C# Workload
- ✅ **PowerShell 5.1+**

### Permissões Necessárias

- ✅ **Administrador Local** (para copiar DLLs para diretórios SSIS)
- ✅ **db_owner** no database destino
- ✅ **sysadmin** no SQL Server (para configurar SSISDB)

### Hardware Recomendado

- CPU: 2+ cores
- RAM: 8+ GB
- Disco: 2+ GB disponível

---

## 📁 Preparação do Ambiente

### Passo 1: Criar Estrutura de Diretórios
```powershell
# Criar diretório raiz
New-Item -Path "C:\Dev\QuattoAPIClient" -ItemType Directory -Force

# Criar subdiretórios
$dirs = @(
    "src\01_Source\Components",
    "src\01_Source\Helpers",
    "src\01_Source\Properties",
    "src\02_ConnectionManager\Properties",
    "src\03_UI\Forms",
    "src\03_UI\Properties",
    "database",
    "deployment",
    "examples",
    "docs"
)

foreach ($dir in $dirs) {
    New-Item -Path "C:\Dev\QuattoAPIClient\$dir" -ItemType Directory -Force
}
```

### Passo 2: Copiar Arquivos do OneDrive

Copie todos os arquivos `.txt` do OneDrive seguindo o mapeamento:
```
OneDrive/SourceCode/01_Source_CorporateApiSource.cs.txt
  → C:\Dev\QuattoAPIClient\src\01_Source\Components\CorporateApiSource.cs

OneDrive/Database/01_Tables_and_StoredProcedures.sql.txt
  → C:\Dev\QuattoAPIClient\database\01_Complete_Database_Setup.sql

... (seguir para todos os arquivos)
```

**IMPORTANTE:** Remover extensão `.txt` de todos os arquivos!

---

## 🗄️ Instalação do Database

### Passo 1: Ajustar Database Name

Editar `database/01_Complete_Database_Setup.sql`:
```sql
-- Linha 20: Ajustar conforme seu ambiente
USE [SESCDF_DW];  -- ⚠️ MODIFICAR AQUI
GO
```

### Passo 2: Executar SQL Script
```powershell
# Via SQLCMD
sqlcmd -S "SQL-SERVER\INSTANCE" -d "SESCDF_DW" `
       -i "C:\Dev\QuattoAPIClient\database\01_Complete_Database_Setup.sql"

# Ou via SQL Server Management Studio (SSMS)
# 1. Abrir SSMS
# 2. File → Open → File
# 3. Selecionar 01_Complete_Database_Setup.sql
# 4. Execute (F5)
```

### Passo 3: Validar Instalação
```sql
-- Verificar tabelas criadas
SELECT name, create_date 
FROM sys.tables 
WHERE name LIKE 'API_%'
ORDER BY name;

-- Resultado esperado:
-- API_ExecutionLog
-- API_RateLimitControl
-- API_RawPayloads
-- API_Watermarks

-- Verificar stored procedures
SELECT name, create_date 
FROM sys.procedures 
WHERE name LIKE 'usp_API_%'
ORDER BY name;

-- Resultado esperado:
-- usp_API_CheckRateLimit
-- usp_API_CleanupRawPayloads
-- usp_API_GetWatermark
-- usp_API_UpdateWatermark
```

---

## 🔨 Compilação da Solution

### Passo 1: Abrir Solution
```powershell
# Abrir Visual Studio com a solution
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\devenv.exe" `
  "C:\Dev\QuattoAPIClient\QuattoAPIClient.sln"
```

### Passo 2: Restaurar Pacotes NuGet
```
Tools → NuGet Package Manager → Restore NuGet Packages
```

### Passo 3: Ajustar Referências SSIS (se necessário)

Se aparecerem erros de referência, ajustar caminhos em `.csproj`:
```xml
<!-- Ajustar caminho conforme sua instalação -->
<Reference Include="Microsoft.SqlServer.DTSPipelineWrap">
  <HintPath>C:\Program Files\Microsoft SQL Server\150\SDK\Assemblies\Microsoft.SqlServer.DTSPipelineWrap.dll</HintPath>
</Reference>
```

**Versões do SQL Server:**
- 2017: `140`
- 2019: `150`
- 2022: `160`

### Passo 4: Build Solution
```
Build → Configuration Manager → Release → Any CPU
Build → Build Solution (Ctrl+Shift+B)
```

**Resultado esperado:**
```
Build started...
1>------ Build started: Project: QuattoAPIClient.ConnectionManager, Configuration: Release Any CPU ------
1>  QuattoAPIClient.ConnectionManager -> C:\Dev\QuattoAPIClient\src\02_ConnectionManager\bin\Release\QuattoAPIClient.ConnectionManager.dll
2>------ Build started: Project: QuattoAPIClient.Source, Configuration: Release Any CPU ------
2>  QuattoAPIClient.Source -> C:\Dev\QuattoAPIClient\src\01_Source\bin\Release\QuattoAPIClient.Source.dll
3>------ Build started: Project: QuattoAPIClient.UI, Configuration: Release Any CPU ------
3>  QuattoAPIClient.UI -> C:\Dev\QuattoAPIClient\src\03_UI\bin\Release\QuattoAPIClient.UI.dll
========== Build: 3 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========
```

---

## 🚀 Deploy dos Componentes

### Método 1: Deploy Automatizado (Recomendado)
```powershell
# Executar como Administrador
cd C:\Dev\QuattoAPIClient\deployment

.\Deploy-QuattoAPIClient.ps1 `
    -SourcePath "C:\Dev\QuattoAPIClient" `
    -TargetEnvironment DEV `
    -BuildSolution $false

# Output esperado:
# ═══════════════════════════════════════════
# QUATTO API CLIENT - AUTOMATED DEPLOYMENT
# ═══════════════════════════════════════════
# [✓] Validando pré-requisitos...
# [✓] Copiando DLLs...
# [✓] Validando deployment...
# [✓] DEPLOYMENT CONCLUÍDO COM SUCESSO
```

### Método 2: Deploy Manual

**Copiar DLLs para SSIS Designer:**
```powershell
$source = "C:\Dev\QuattoAPIClient\src\01_Source\bin\Release"
$designerPath = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\PublicAssemblies"

Copy-Item "$source\QuattoAPIClient.Source.dll" $designerPath -Force
Copy-Item "$source\QuattoAPIClient.ConnectionManager.dll" $designerPath -Force
Copy-Item "$source\QuattoAPIClient.UI.dll" $designerPath -Force
```

**Copiar DLLs para SSIS Runtime:**
```powershell
$runtimePath = "C:\Program Files\Microsoft SQL Server\150\DTS\PipelineComponents"

Copy-Item "$source\QuattoAPIClient.Source.dll" $runtimePath -Force
Copy-Item "$source\QuattoAPIClient.ConnectionManager.dll" $runtimePath -Force
```

**Copiar ConnectionManager para pasta Connections:**
```powershell
$connectionsPath = "C:\Program Files\Microsoft SQL Server\150\DTS\Connections"

Copy-Item "$source\QuattoAPIClient.ConnectionManager.dll" $connectionsPath -Force
```

---

## ✅ Validação

### Passo 1: Reiniciar Visual Studio

**⚠️ CRÍTICO:** Feche TODAS as instâncias do Visual Studio e reabra.

### Passo 2: Validar Componente na Toolbox

1. Criar novo **Integration Services Project**
2. Adicionar **Data Flow Task**
3. Abrir Data Flow
4. Verificar se **"Quatto Corporate API Source"** aparece na Toolbox:
```
SSIS Toolbox
├─ Common
├─ Other Sources
│  └─ ✓ Quatto Corporate API Source  ← DEVE APARECER AQUI
├─ Other Transforms
└─ Other Destinations
```

### Passo 3: Validar Connection Manager

1. No painel **Connection Managers**
2. Clique direito → **New Connection...**
3. Verificar se **"API"** aparece na lista

### Passo 4: Testar Componente

Criar pacote de teste simples:
```
1. Adicionar Connection Manager "API"
   - AuthType: Bearer
   - BearerToken: test_token

2. Adicionar Data Flow Task
3. Adicionar "Quatto Corporate API Source"
   - BaseUrl: https://httpbin.org
   - Endpoint: /get
   - PageSize: 10

4. Adicionar Derived Column (para visualizar)
5. NÃO executar ainda - apenas validar que não há erros de design
```

---

## 🐛 Troubleshooting

### Problema: Componente não aparece na Toolbox

**Causas possíveis:**
- Visual Studio não foi reiniciado
- DLLs não foram copiadas corretamente
- Versão do .NET Framework incompatível

**Soluções:**
```powershell
# 1. Verificar se DLLs existem
Test-Path "C:\Program Files\Microsoft SQL Server\150\DTS\PipelineComponents\QuattoAPIClient.Source.dll"

# 2. Verificar versão da DLL
[System.Reflection.Assembly]::LoadFile("C:\...\QuattoAPIClient.Source.dll").ImageRuntimeVersion

# 3. Limpar cache do Visual Studio
Remove-Item "$env:LOCALAPPDATA\Microsoft\VisualStudio\*\ComponentModelCache" -Recurse -Force

# 4. Reinstalar
.\Deploy-QuattoAPIClient.ps1 -SourcePath "C:\Dev\QuattoAPIClient" -SkipValidation
```

### Problema: Erro "Could not load file or assembly"

**Causa:** Referências SSIS não encontradas

**Solução:**
```xml
<!-- Verificar e ajustar em .csproj -->
<Reference Include="Microsoft.SqlServer.DTSPipelineWrap">
  <HintPath>C:\Program Files\Microsoft SQL Server\150\SDK\Assemblies\Microsoft.SqlServer.DTSPipelineWrap.dll</HintPath>
  <Private>False</Private> <!-- IMPORTANTE: False para SSIS -->
</Reference>
```

### Problema: Build falha com erro de Strong Name

**Causa:** AssemblyKeyFile configurado mas arquivo .snk não existe

**Solução:**
```csharp
// Comentar em AssemblyInfo.cs:
// [assembly: AssemblyKeyFile("..\\..\\..\\QuattoAPIClient.snk")]
```

### Problema: SQL Scripts falham

**Causa:** Database não existe ou permissões insuficientes

**Solução:**
```sql
-- Verificar permissões
USE SESCDF_DW;
GO

SELECT HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'CREATE TABLE') AS CanCreateTable;

-- Se retornar 0, pedir a um DBA para conceder permissões:
ALTER ROLE db_owner ADD MEMBER [SEU_USUARIO];
```

---

## 📊 Checklist de Instalação
```
☐ Pré-requisitos verificados
☐ Estrutura de diretórios criada
☐ Arquivos copiados do OneDrive
☐ Extensões .txt removidas
☐ SQL Scripts executados com sucesso
☐ 4 tabelas criadas (API_*)
☐ 4 procedures criadas (usp_API_*)
☐ Solution compilada sem erros
☐ 3 DLLs geradas em bin\Release
☐ Deploy automatizado executado
☐ Visual Studio reiniciado
☐ Componente aparece na Toolbox
☐ Connection Manager "API" disponível
☐ Pacote de teste criado sem erros
☐ Documentação revisada
```

---

## 📞 Suporte

**Problemas na instalação?**

1. Verificar logs do PowerShell em `deployment/DeploymentReport_*.txt`
2. Consultar `docs/04_TROUBLESHOOTING.md`
3. Contato: erton.miranda@quatto.com.br

---

**Próximos Passos:** [02_CONFIGURATION.md](02_CONFIGURATION.md)