```
═══════════════════════════════════════════════════════════════════
QUATTO API CLIENT - MAPEAMENTO DE ARQUIVOS
═══════════════════════════════════════════════════════════════════

Este arquivo mapeia TODOS os 32 arquivos do OneDrive para a 
estrutura local do projeto. Use como guia durante a cópia.

═══════════════════════════════════════════════════════════════════
COMO USAR ESTE GUIA
═══════════════════════════════════════════════════════════════════

1. Crie a estrutura de diretórios (veja seção abaixo)
2. Para cada arquivo listado:
   - Copie do OneDrive para o caminho de destino
   - REMOVA a extensão .txt
3. Valide que todos os 32 arquivos foram copiados

═══════════════════════════════════════════════════════════════════
MAPEAMENTO COMPLETO (32 ARQUIVOS)
═══════════════════════════════════════════════════════════════════

CÓDIGO FONTE C# (14 ARQUIVOS):
--------------------------------

OneDrive: SourceCode/01_Source_CorporateApiSource.cs.txt
Destino:  src/01_Source/Components/CorporateApiSource.cs

OneDrive: SourceCode/02_Source_HttpHelper.cs.txt
Destino:  src/01_Source/Helpers/HttpHelper.cs

OneDrive: SourceCode/03_Source_PaginationEngine.cs.txt
Destino:  src/01_Source/Helpers/PaginationEngine.cs

OneDrive: SourceCode/04_Source_WatermarkManager.cs.txt
Destino:  src/01_Source/Helpers/WatermarkManager.cs

OneDrive: SourceCode/05_Source_RawStorageManager.cs.txt
Destino:  src/01_Source/Helpers/RawStorageManager.cs

OneDrive: SourceCode/06_Source_SchemaMapper.cs.txt
Destino:  src/01_Source/Helpers/SchemaMapper.cs

OneDrive: SourceCode/07_Source_AssemblyInfo.cs.txt
Destino:  src/01_Source/Properties/AssemblyInfo.cs

OneDrive: SourceCode/08_ConnectionManager_ApiConnectionManager.cs.txt
Destino:  src/02_ConnectionManager/ApiConnectionManager.cs

OneDrive: SourceCode/09_ConnectionManager_OAuth2TokenManager.cs.txt
Destino:  src/02_ConnectionManager/OAuth2TokenManager.cs

OneDrive: SourceCode/10_ConnectionManager_AssemblyInfo.cs.txt
Destino:  src/02_ConnectionManager/Properties/AssemblyInfo.cs

OneDrive: SourceCode/11_UI_CorporateApiSourceUI.cs.txt
Destino:  src/03_UI/CorporateApiSourceUI.cs

OneDrive: SourceCode/12_UI_ApiSourceWizard.cs.txt
Destino:  src/03_UI/Forms/ApiSourceWizard.cs

OneDrive: SourceCode/13_UI_ApiSourceWizard_Designer.cs.txt
Destino:  src/03_UI/Forms/ApiSourceWizard.Designer.cs

OneDrive: SourceCode/14_UI_AssemblyInfo.cs.txt
Destino:  src/03_UI/Properties/AssemblyInfo.cs

ARQUIVOS DE PROJETO (5 ARQUIVOS):
----------------------------------

OneDrive: SourceCode/15_ConnectionManager_csproj.txt
Destino:  src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj

OneDrive: SourceCode/16_UI_csproj.txt
Destino:  src/03_UI/QuattoAPIClient.UI.csproj

OneDrive: SourceCode/17_Solution.txt
Destino:  QuattoAPIClient.sln

OneDrive: SourceCode/18_Source_csproj.txt
Destino:  src/01_Source/QuattoAPIClient.Source.csproj

DATABASE (2 ARQUIVOS):
----------------------

OneDrive: Database/01_Tables_and_StoredProcedures.sql.txt
Destino:  database/01_Complete_Database_Setup.sql

OneDrive: Database/03_Dashboard_Queries.sql.txt
Destino:  database/03_Dashboard_Queries.sql

DEPLOYMENT (1 ARQUIVO):
-----------------------

OneDrive: Deployment/02_Deploy_Component.ps1.txt
Destino:  deployment/Deploy-QuattoAPIClient.ps1

EXEMPLOS (4 ARQUIVOS):
----------------------

OneDrive: Examples/SchemaMapping_Gladium.json.txt
Destino:  examples/SchemaMapping_Gladium.json

OneDrive: Examples/SchemaMapping_PortalSESC.json.txt
Destino:  examples/SchemaMapping_PortalSESC.json

OneDrive: Examples/SSISDB_Parameters.json.txt
Destino:  examples/SSISDB_Parameters.json

OneDrive: Examples/Sample_Package_Structure.txt
Destino:  examples/Sample_Package_Structure.txt

DOCUMENTAÇÃO (6 ARQUIVOS):
--------------------------

OneDrive: Documentation/00_README.txt
Destino:  README.md

OneDrive: Documentation/01_INSTALLATION.md.txt
Destino:  docs/01_INSTALLATION.md

OneDrive: Documentation/02_CONFIGURATION.md.txt
Destino:  docs/02_CONFIGURATION.md

OneDrive: Documentation/03_TROUBLESHOOTING.md.txt
Destino:  docs/03_TROUBLESHOOTING.md

OneDrive: Documentation/04_USAGE.md.txt
Destino:  docs/04_USAGE.md

OneDrive: 00_LEIA_PRIMEIRO.txt
Destino:  README_FIRST.txt

OneDrive: 01_ESTRUTURA_DO_PROJETO.txt
Destino:  PROJECT_STRUCTURE.txt (este arquivo)

═══════════════════════════════════════════════════════════════════
COMANDO POWERSHELL PARA CRIAR ESTRUTURA
═══════════════════════════════════════════════════════════════════

$baseDir = "C:\Dev\QuattoAPIClient"
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

New-Item -Path $baseDir -ItemType Directory -Force
foreach ($dir in $dirs) {
    New-Item -Path "$baseDir\$dir" -ItemType Directory -Force
}

Write-Host "✓ Estrutura de diretórios criada em: $baseDir" -ForegroundColor Green

═══════════════════════════════════════════════════════════════════
VALIDAÇÃO (CHECKLIST)
═══════════════════════════════════════════════════════════════════

Total esperado: 32 arquivos

☐ 14 arquivos C# copiados
☐ 5 arquivos de projeto copiados
☐ 2 arquivos SQL copiados
☐ 1 arquivo PowerShell copiado
☐ 4 arquivos de exemplo copiados
☐ 6 arquivos de documentação copiados

☐ TODAS as extensões .txt removidas
☐ Estrutura de diretórios validada

COMANDO PARA VALIDAR:
Get-ChildItem -Path "C:\Dev\QuattoAPIClient" -Recurse -File | 
    Where-Object { $_.Extension -eq ".txt" } | 
    Select-Object FullName

⚠️ Se o comando acima retornar arquivos, você esqueceu de remover .txt!

═══════════════════════════════════════════════════════════════════