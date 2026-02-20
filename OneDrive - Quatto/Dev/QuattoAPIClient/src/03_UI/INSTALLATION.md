# 🔧 Instalação e Setup - Quatto API Client for SSIS

> Guia passo-a-passo para instalar e configurar o Quatto API Client

---

## 📋 Índice

1. [Pré-requisitos](#pré-requisitos)
2. [Instalação para Desenvolvimento](#instalação-para-desenvolvimento)
3. [Instalação em Produção](#instalação-em-produção)
4. [Verificação](#verificação)
5. [Troubleshooting](#troubleshooting)

---

## ✅ Pré-requisitos

### Sistema Operacional

- ✅ Windows Server 2019+ (ou Windows 10/11 Pro)
- ✅ Administrator access

### Software Necessário

| Software | Versão | Obrigatório | Status |
|----------|--------|-----------|--------|
| **.NET Framework** | 4.7.2+ | ✅ SIM | Instalado |
| **SQL Server** | 2022 | ✅ SIM | Instalado |
| **SSIS** | v17.100 | ✅ SIM | Instalado |
| **Visual Studio** | 2022 18.3.1+ | ✅ SIM | Instalado |
| **SSDT** | Última | ✅ SIM | Instalado |
| **Git** | Qualquer | ⚠️ OPT | Para clone |

### Verificar Instalação

```powershell
# Verificar .NET Framework
Get-ChildItem "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"

# Verificar SQL Server
Get-Service "MSSQLSERVER" | Select Status

# Verificar SSIS
Get-ChildItem "HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server" | Where-Object { $_ -match "170" }

# Verificar Visual Studio
& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\devenv.exe" --version
```

---

## 🛠️ Instalação para Desenvolvimento

### Passo 1: Clonar o Repositório

```powershell
# Clone
git clone <repo-url> QuattoAPIClient
cd QuattoAPIClient/src

# Ou abra pasta existente
cd C:\Dev\QuattoAPIClient\src
```

### Passo 2: Abrir em Visual Studio

```powershell
# Opção 1: Via comando
devenv.exe QuattoAPIClient.sln

# Opção 2: Via Windows Explorer
# Duplo-clique em QuattoAPIClient.sln
```

### Passo 3: Restaurar NuGet Packages

```powershell
# Automático ao abrir VS
# Ou manual:
dotnet restore

# Ou no VS Package Manager:
# Tools → NuGet Package Manager → Package Manager Console
# PM> Update-Package -Reinstall
```

### Passo 4: Build da Solução

```powershell
# Via Visual Studio
Ctrl+Shift+B  # ou Build → Rebuild Solution

# Via CLI
dotnet build

# Esperado
# Build succeeded. 0 Warning(s), 0 Error(s), Time elapsed 15s
```

### Passo 5: Executar Testes

```powershell
# Via Visual Studio Test Explorer
Ctrl+E, T  # Open Test Explorer
# Ou Click "Run All Tests"

# Via CLI
dotnet test 04_Tests/QuattoAPIClient.Tests.csproj

# Esperado
# ===================== Test Run Successful =======================
# Total tests run: 47, Passed: 47, Failed: 0, Skipped: 0
```

### Passo 6: Verificação Final

✅ Solução compila sem erros  
✅ 47 testes passando  
✅ Sem warnings  
✅ Intellisense funciona  

**Parabéns! Desenvolvimento setup completo.** 🎉

---

## 🚀 Instalação em Produção

### Passo 1: Build Release

```powershell
# Build em Release mode
dotnet build -c Release

# Ou via VS
# Build → Configuration Manager → Selecionar "Release"
# Ctrl+Shift+B
```

### Passo 2: Localizar Assemblies

```powershell
# UI Project DLL
$uiDll = "src/03_UI/bin/Release/net472/QuattoAPIClient.UI.dll"

# ConnectionManager Project DLL
$connDll = "src/02_ConnectionManager/bin/Release/net472/QuattoAPIClient.ConnectionManager.dll"

# Verificar existência
Test-Path $uiDll
Test-Path $connDll
```

### Passo 3: Copiar para SSIS

```powershell
# Encontrar SSIS folder
$ssisBinn = "C:\Program Files\Microsoft SQL Server\160\DTS\Binn"

# Copiar DLLs
Copy-Item $uiDll -Destination $ssisBinn -Force
Copy-Item $connDll -Destination $ssisBinn -Force

# Copiar dependências (se necessário)
# Copy-Item "*.dll" -Destination $ssisBinn -Force
```

### Passo 4: Registrar no GAC (Optional)

```powershell
# Se deseja registrar no Global Assembly Cache
# Requer admin e gacutil

# Instale o Windows SDK (inclui gacutil)
# "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\gacutil.exe" ^
#   -i "QuattoAPIClient.UI.dll"
```

### Passo 5: Configurar SQL Server

```sql
-- Verificar SSIS 2022 installation
SELECT * FROM sys.dm_exec_sessions WHERE database_id = DB_ID('msdb')

-- Verificar Integration Services Catalogs
SELECT * FROM [msdb].[dbo].[ssis_catalogs]

-- Se não existir, criar
-- (Requer acesso ao SSIS Catalog)
```

### Passo 6: Testar em SSIS

1. Abra **SQL Server Data Tools** (ou SSMS)
2. Crie novo **Integration Services Project**
3. Adicione novo **Data Flow Task**
4. Arraste **"Corporate API Source"** (deve aparecer no toolbox)
5. Configure e teste

---

## ✔️ Verificação

### Verificar Instalação de Desenvolvimento

```powershell
# 1. Verificar projeto compila
dotnet build
# Esperado: Build succeeded

# 2. Verificar testes
dotnet test 04_Tests/
# Esperado: 47 passed

# 3. Verificar logging funciona
$logger = [QuattoAPIClient.Logging.LoggerFactory]::GetLogger([object])
$logger.GetType().Name
# Esperado: Logger`1

# 4. Verificar SSIS types resolvem
[QuattoAPIClient.UI.CorporateApiSourceUI]::new()
# Esperado: Sem erro
```

### Verificar Instalação de Produção

```powershell
# 1. Verificar DLLs copiadas
Test-Path "C:\Program Files\Microsoft SQL Server\160\DTS\Binn\QuattoAPIClient.UI.dll"
# Esperado: True

# 2. Verificar em SSDT toolbox
# Abrir SSDT > Toolbox > SSIS > 
# "Corporate API Source" deve aparecer

# 3. Verificar sem erros de load
# Se aparecer em toolbox = DLLs carregaram OK
```

---

## 🐛 Troubleshooting

### Erro: "Could not locate assembly 'Microsoft.SqlServer.DTSRuntimeWrap'"

**Causa:** SSIS não instalado corretamente  
**Solução:**
```powershell
# 1. Verificar SSIS versão
Get-ChildItem "C:\Program Files\Microsoft SQL Server\160\DTS\Binn"

# 2. Se vazio, instalar:
# SQL Server 2022 Installer → Modify → Integration Services

# 3. Após instalar, rebuild projeto
dotnet clean
dotnet build
```

---

### Erro: "The type or namespace name 'PipelineComponent' could not be found"

**Causa:** SSIS assemblies não carregam (dotnet CLI limitation)  
**Solução:**
```powershell
# Use Visual Studio MSBuild, NÃO dotnet CLI
# Abra em VS 2022 e compile via:
# Ctrl+Shift+B (Build Menu)
# OU
# "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe" QuattoAPIClient.sln
```

---

### Erro: "Object reference not set to an instance of an object"

**Causa:** Propriedade não inicializada  
**Solução:**
```csharp
// No Initialize method:
if (obj == null)
    throw new ArgumentNullException(nameof(obj));

// Sempre validar no Edit:
if (_metadata == null)
    throw new InvalidOperationException("Must Initialize first");
```

---

### Erro: "DLLs não aparecem em SSIS Toolbox"

**Causa:** Assembly não registrado  
**Solução:**
```powershell
# 1. Verify DLLs copiadas
ls "C:\Program Files\Microsoft SQL Server\160\DTS\Binn\QuattoAPIClient*"

# 2. Reiniciar VS completamente
# (Às vezes precisa cache flush)

# 3. Limpar componentes cache
Remove-Item "$env:APPDATA\Microsoft\DataTransformationServices" -Recurse -Force

# 4. Reopenar VS
devenv.exe /resetuserdata
```

---

### Erro: "LoggerFactory not found"

**Causa:** Namespace não importado  
**Solução:**
```csharp
// Adicionar using
using QuattoAPIClient.Logging;

// Usar
var logger = LoggerFactory.GetLogger<MyClass>();
```

---

### Erro: "Tests not running - XUnit not found"

**Causa:** Test project DLL não compila  
**Solução:**
```powershell
# 1. Restore packages
dotnet restore 04_Tests/

# 2. Build test project
dotnet build 04_Tests/

# 3. Run tests
dotnet test 04_Tests/ -v d

# Se problema persiste:
# Remove bin/obj e tente novamente
Remove-Item 04_Tests/bin, 04_Tests/obj -Recurse -Force
dotnet restore
dotnet test
```

---

## 🔄 Reinstalar Completamente

Se tudo falhar:

```powershell
# 1. Clean
Remove-Item -Path "src/*/bin" -Recurse -Force
Remove-Item -Path "src/*/obj" -Recurse -Force

# 2. Remove NuGet cache (optional)
Remove-Item "$env:USERPROFILE\.nuget\packages" -Recurse -Force

# 3. Restore
cd src
dotnet restore

# 4. Build
dotnet build

# 5. Verify
dotnet test 04_Tests/
```

---

## 📋 Checklist de Instalação

### Desenvolvimento

- [ ] .NET Framework 4.7.2 instalado
- [ ] SQL Server 2022 instalado
- [ ] SSIS v17.100 instalado
- [ ] Visual Studio 2022 18.3.1+
- [ ] Projeto clonado
- [ ] Solução compila (Ctrl+Shift+B)
- [ ] 47 testes passando
- [ ] SSDT abre com intellisense
- [ ] Pode debugar código

### Produção

- [ ] DLLs compiladas em Release
- [ ] Copiadas para SSIS Binn folder
- [ ] "Corporate API Source" aparece em SSDT Toolbox
- [ ] Teste component abre wizard sem erro
- [ ] Teste wizard salva configuração
- [ ] Package executa sem erro
- [ ] Logging funciona

---

## 🆘 Suporte

### Documentação Completa

- 📖 [MAIN_README.md](MAIN_README.md) - Visão geral
- 🏗️ [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitetura
- 🧪 [README_TESTS.md](README_TESTS.md) - Testes
- 📝 [LOGGING_GUIDE.md](LOGGING_GUIDE.md) - Logging
- 🆘 [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Mais problemas

### Contato

- 📧 support@quatto.com.br
- 🐛 Report issues
- 💬 Discussions

---

**Versão:** 1.0.0  
**Última Atualização:** 2026-02-20  
**Status:** ✅ Pronto para Instalação

