═══════════════════════════════════════════════════════════════════
QUATTO API CLIENT v1.0 - LEIA PRIMEIRO[R E A D M E F I R S T]()
Projeto: SESC-DF Data Warehouse
Autor: Erton Miranda / Quatto Consultoria
Data: Fevereiro 2026
═══════════════════════════════════════════════════════════════════

BEM-VINDO AO QUATTO API CLIENT!

Este pacote contém TODOS os arquivos necessários para implementar
o componente SSIS customizado de consumo de APIs REST.

═══════════════════════════════════════════════════════════════════
📦 CONTEÚDO DO PACOTE (32 ARQUIVOS)
═══════════════════════════════════════════════════════════════════

✅ 5 ARQUIVOS PRINCIPAIS:
   [1] CorporateApiSource.cs (1.150 linhas) - Componente principal
   [2] ApiConnectionManager.cs (400 linhas) - Autenticação
   [3] Complete Database Setup.sql (600 linhas) - Banco de dados
   [4] Deploy-QuattoAPIClient.ps1 (550 linhas) - Deploy automatizado
   [5] README.md (700 linhas) - Documentação completa

✅ 9 HELPERS C#:
   - HttpHelper.cs - Retry e backoff
   - PaginationEngine.cs - Paginação automática
   - WatermarkManager.cs - Controle incremental
   - RawStorageManager.cs - Armazenamento de JSON
   - SchemaMapper.cs - Mapeamento de campos
   - OAuth2TokenManager.cs - Gerenciamento de tokens

✅ 3 UI C#:
   - CorporateApiSourceUI.cs - Controlador de UI
   - ApiSourceWizard.cs - Wizard de configuração
   - ApiSourceWizard.Designer.cs - Designer partial

✅ 6 ARQUIVOS DE PROJETO:
   - QuattoAPIClient.sln
   - QuattoAPIClient.Source.csproj
   - QuattoAPIClient.ConnectionManager.csproj
   - QuattoAPIClient.UI.csproj
   - 3x AssemblyInfo.cs

✅ 8 DOCUMENTAÇÃO E EXEMPLOS:
   - 01_INSTALLATION.md
   - 02_CONFIGURATION.md
   - 03_TROUBLESHOOTING.md
   - Schema Mappings (Gladium, Portal SESC)
   - SSISDB Parameters template
   - Sample Package Structure
   - Dashboard Queries SQL

═══════════════════════════════════════════════════════════════════
🚀 INÍCIO RÁPIDO (5 PASSOS)
═══════════════════════════════════════════════════════════════════

PASSO 1: Criar estrutura de diretórios
---------------------------------------
cd C:\
mkdir Dev\QuattoAPIClient
cd Dev\QuattoAPIClient

PASSO 2: Copiar arquivos do OneDrive
------------------------------------
Copie todos os arquivos .txt deste pacote seguindo o mapeamento:

SourceCode/*.cs.txt → src/01_Source/Components/*.cs
Database/*.sql.txt → database/*.sql
... (ver ESTRUTURA_DO_PROJETO.txt)

⚠️ IMPORTANTE: Remover extensão .txt de TODOS os arquivos!

PASSO 3: Executar SQL Scripts
------------------------------
sqlcmd -S "SQL-SERVER" -d "SESCDF_DW" -i database\01_Complete_Database_Setup.sql

PASSO 4: Compilar Solution
---------------------------
Abrir QuattoAPIClient.sln no Visual Studio
Build → Build Solution (Ctrl+Shift+B)

PASSO 5: Deploy componente
---------------------------
Executar PowerShell como Administrador:

cd deployment
.\Deploy-QuattoAPIClient.ps1 -SourcePath "C:\Dev\QuattoAPIClient" -TargetEnvironment DEV

═══════════════════════════════════════════════════════════════════
📁 ESTRUTURA DE DIRETÓRIOS ESPERADA
═══════════════════════════════════════════════════════════════════

C:\Dev\QuattoAPIClient\
│
├── QuattoAPIClient.sln
├── README.md
├── README_FIRST.txt (este arquivo)
│
├── src\
│   ├── 01_Source\
│   │   ├── Components\
│   │   │   └── CorporateApiSource.cs
│   │   ├── Helpers\
│   │   │   ├── HttpHelper.cs
│   │   │   ├── PaginationEngine.cs
│   │   │   ├── WatermarkManager.cs
│   │   │   ├── RawStorageManager.cs
│   │   │   └── SchemaMapper.cs
│   │   ├── Properties\
│   │   │   └── AssemblyInfo.cs
│   │   └── QuattoAPIClient.Source.csproj
│   │
│   ├── 02_ConnectionManager\
│   │   ├── ApiConnectionManager.cs
│   │   ├── OAuth2TokenManager.cs
│   │   ├── Properties\
│   │   │   └── AssemblyInfo.cs
│   │   └── QuattoAPIClient.ConnectionManager.csproj
│   │
│   └── 03_UI\
│       ├── Forms\
│       │   ├── ApiSourceWizard.cs
│       │   └── ApiSourceWizard.Designer.cs
│       ├── CorporateApiSourceUI.cs
│       ├── Properties\
│       │   └── AssemblyInfo.cs
│       └── QuattoAPIClient.UI.csproj
│
├── database\
│   ├── 01_Complete_Database_Setup.sql
│   └── 03_Dashboard_Queries.sql
│
├── deployment\
│   └── Deploy-QuattoAPIClient.ps1
│
├── examples\
│   ├── SchemaMapping_Gladium.json
│   ├── SchemaMapping_PortalSESC.json
│   ├── SSISDB_Parameters.json
│   └── Sample_Package_Structure.txt
│
└── docs\
    ├── 01_INSTALLATION.md
    ├── 02_CONFIGURATION.md
    └── 03_TROUBLESHOOTING.md

═══════════════════════════════════════════════════════════════════
⚙️ PRÉ-REQUISITOS
═══════════════════════════════════════════════════════════════════

SOFTWARE:
✅ SQL Server 2019+ (ou 2017 com ajustes)
✅ Visual Studio 2019+ com SSDT
✅ .NET Framework 4.7.2+
✅ PowerShell 5.1+

PERMISSÕES:
✅ Administrador Local (para copiar DLLs)
✅ db_owner no database (para criar tabelas)
✅ sysadmin no SQL Server (para SSISDB)

HARDWARE:
✅ CPU: 2+ cores
✅ RAM: 8+ GB
✅ Disco: 2+ GB disponível

═══════════════════════════════════════════════════════════════════
📊 O QUE VOCÊ VAI TER APÓS INSTALAÇÃO
═══════════════════════════════════════════════════════════════════

✅ Componente "Quatto Corporate API Source" na Toolbox do SSIS
✅ Connection Manager "API" disponível
✅ 4 tabelas no database (API_Watermarks, API_RawPayloads, etc.)
✅ 4 stored procedures (usp_API_GetWatermark, etc.)
✅ Dashboard queries para monitoramento
✅ Exemplos prontos para Gladium e Portal SESC

BENEFÍCIOS:
✅ Extração incremental automática
✅ Retry com backoff exponencial
✅ Paginação automática
✅ JSON bruto armazenado (auditoria)
✅ Telemetria detalhada
✅ Governança centralizada

═══════════════════════════════════════════════════════════════════
🆘 PRECISA DE AJUDA?
═══════════════════════════════════════════════════════════════════

DOCUMENTAÇÃO:
📖 docs/01_INSTALLATION.md - Guia passo a passo
📖 docs/02_CONFIGURATION.md - Todas as propriedades
📖 docs/03_TROUBLESHOOTING.md - Problemas comuns

EXEMPLOS:
📁 examples/Sample_Package_Structure.txt - Estrutura de pacote
📁 examples/SchemaMapping_Gladium.json - Schema mapping

SUPORTE:
📧 erton.miranda@quatto.com.br
🏢 Quatto Consultoria
📦 Projeto: SESC-DF Data Warehouse

═══════════════════════════════════════════════════════════════════
✅ CHECKLIST DE INSTALAÇÃO
═══════════════════════════════════════════════════════════════════

☐ Arquivos copiados do OneDrive
☐ Extensões .txt removidas
☐ SQL Scripts executados
☐ 4 tabelas criadas (API_*)
☐ 4 procedures criadas (usp_API_*)
☐ Solution compilada sem erros
☐ Deploy PowerShell executado
☐ Visual Studio reiniciado
☐ Componente aparece na Toolbox
☐ Connection Manager "API" disponível
☐ Pacote de teste criado
☐ Documentação revisada

═══════════════════════════════════════════════════════════════════
🎯 PRÓXIMOS PASSOS
═══════════════════════════════════════════════════════════════════

1. Seguir docs/01_INSTALLATION.md para instalação completa
2. Criar primeiro pacote usando examples/Sample_Package_Structure.txt
3. Configurar parâmetros SSISDB usando examples/SSISDB_Parameters.json
4. Executar Dashboard Queries para validar funcionamento
5. Compartilhar feedback com equipe Quatto

═══════════════════════════════════════════════════════════════════

Boa implementação! 🚀

Erton Miranda
Quatto Consultoria
Fevereiro 2026

═══════════════════════════════════════════════════════════════════
```