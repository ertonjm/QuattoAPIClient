<#
═══════════════════════════════════════════════════════════════════
Quatto API Client - Automated Deployment Script
Versão: 1.0.0
Projeto: SESC-DF Data Warehouse
Autor: Erton Miranda / Quatto Consultoria
Data: Fevereiro 2026

DESCRIÇÃO:
Script automatizado para deploy do Quatto API Client no SSIS.
Realiza todas as etapas necessárias:
1. Validação de pré-requisitos
2. Build da solution (opcional)
3. Cópia de DLLs para diretórios SSIS
4. Registro de componentes
5. Validação pós-deploy

PARÂMETROS:
-SourcePath: Caminho da solution compilada
-TargetEnvironment: DEV, HML, PRD
-BuildSolution: $true para compilar antes de deploy
-SkipValidation: $true para pular validações
-WhatIf: Simula deploy sem executar

EXEMPLOS:
.\Deploy-QuattoAPIClient.ps1 -SourcePath "C:\Dev\QuattoAPIClient" -TargetEnvironment DEV
.\Deploy-QuattoAPIClient.ps1 -SourcePath "C:\Dev\QuattoAPIClient" -BuildSolution $true -WhatIf
.\Deploy-QuattoAPIClient.ps1 -SourcePath "C:\Dev\QuattoAPIClient" -TargetEnvironment PRD

═══════════════════════════════════════════════════════════════════
#>

[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [Parameter(Mandatory=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$SourcePath,
    
    [Parameter(Mandatory=$false)]
    [ValidateSet('DEV','HML','PRD')]
    [string]$TargetEnvironment = 'DEV',
    
    [Parameter(Mandatory=$false)]
    [switch]$BuildSolution = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipValidation = $false,
    
    [Parameter(Mandatory=$false)]
    [ValidateSet('Debug','Release')]
    [string]$BuildConfiguration = 'Release'
)

# ═══════════════════════════════════════════════════════════════
# CONFIGURAÇÕES GLOBAIS
# ═══════════════════════════════════════════════════════════════

$ErrorActionPreference = 'Stop'
$script:DeploymentLog = @()
$script:DeploymentErrors = @()
$script:DeploymentWarnings = @()

# Detectar versão do SSIS instalada
$script:SSISVersion = "150" # SQL Server 2019 (ajustar conforme ambiente)

# Caminhos padrão do SSIS
$script:SSISPaths = @{
    RuntimeComponents = "C:\Program Files\Microsoft SQL Server\$SSISVersion\DTS\PipelineComponents"
    DesignerComponents = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\PublicAssemblies"
    Connections = "C:\Program Files\Microsoft SQL Server\$SSISVersion\DTS\Connections"
}

# DLLs a serem deployadas
$script:ComponentDLLs = @(
    @{
        Name = "QuattoAPIClient.Source.dll"
        Type = "Component"
        TargetPaths = @("RuntimeComponents", "DesignerComponents")
    },
    @{
        Name = "QuattoAPIClient.ConnectionManager.dll"
        Type = "ConnectionManager"
        TargetPaths = @("RuntimeComponents", "DesignerComponents", "Connections")
    },
    @{
        Name = "QuattoAPIClient.UI.dll"
        Type = "UI"
        TargetPaths = @("DesignerComponents")
    }
)

# ═══════════════════════════════════════════════════════════════
# FUNÇÕES AUXILIARES
# ═══════════════════════════════════════════════════════════════

function Write-DeploymentLog {
    param(
        [string]$Message,
        [ValidateSet('Info','Success','Warning','Error')]
        [string]$Level = 'Info'
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "[$timestamp] [$Level] $Message"
    
    $script:DeploymentLog += $logEntry
    
    switch ($Level) {
        'Info'    { Write-Host $logEntry -ForegroundColor White }
        'Success' { Write-Host $logEntry -ForegroundColor Green }
        'Warning' { 
            Write-Host $logEntry -ForegroundColor Yellow
            $script:DeploymentWarnings += $Message
        }
        'Error'   { 
            Write-Host $logEntry -ForegroundColor Red
            $script:DeploymentErrors += $Message
        }
    }
}

function Write-SectionHeader {
    param([string]$Title)
    
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host $Title -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
}

function Test-Prerequisites {
    Write-SectionHeader "VALIDANDO PRÉ-REQUISITOS"
    
    $allValid = $true
    
    # Verificar se está executando como Administrador
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    
    if (-not $isAdmin) {
        Write-DeploymentLog "Script deve ser executado como Administrador" -Level Error
        $allValid = $false
    } else {
        Write-DeploymentLog "✓ Executando como Administrador" -Level Success
    }
    
    # Verificar se diretórios SSIS existem
    foreach ($pathKey in $script:SSISPaths.Keys) {
        $path = $script:SSISPaths[$pathKey]
        
        if (Test-Path $path) {
            Write-DeploymentLog "✓ Diretório encontrado: $pathKey" -Level Success
        } else {
            Write-DeploymentLog "✗ Diretório não encontrado: $path" -Level Error
            $allValid = $false
        }
    }
    
    # Verificar se DLLs fonte existem
    $binPath = Join-Path $SourcePath "src\01_Source\bin\$BuildConfiguration"
    
    if (-not (Test-Path $binPath)) {
        Write-DeploymentLog "✗ Diretório de binários não encontrado: $binPath" -Level Error
        $allValid = $false
    } else {
        Write-DeploymentLog "✓ Diretório de binários encontrado" -Level Success
        
        foreach ($dll in $script:ComponentDLLs) {
            $dllPath = Join-Path $binPath $dll.Name
            
            if (Test-Path $dllPath) {
                $fileInfo = Get-Item $dllPath
                Write-DeploymentLog "✓ DLL encontrada: $($dll.Name) ($($fileInfo.Length / 1KB) KB)" -Level Success
            } else {
                Write-DeploymentLog "✗ DLL não encontrada: $($dll.Name)" -Level Error
                $allValid = $false
            }
        }
    }
    
    # Verificar .NET Framework
    $dotNetVersion = (Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" -ErrorAction SilentlyContinue).Version
    
    if ($dotNetVersion) {
        Write-DeploymentLog "✓ .NET Framework instalado: $dotNetVersion" -Level Success
    } else {
        Write-DeploymentLog "⚠ Não foi possível verificar .NET Framework" -Level Warning
    }
    
    return $allValid
}

function Invoke-BuildSolution {
    Write-SectionHeader "COMPILANDO SOLUTION"
    
    $slnPath = Join-Path $SourcePath "QuattoAPIClient.sln"
    
    if (-not (Test-Path $slnPath)) {
        Write-DeploymentLog "Solution não encontrada: $slnPath" -Level Error
        return $false
    }
    
    # Localizar MSBuild
    $msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
        -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe `
        -ErrorAction SilentlyContinue | Select-Object -First 1
    
    if (-not $msbuildPath) {
        Write-DeploymentLog "MSBuild não encontrado. Instale Visual Studio Build Tools." -Level Error
        return $false
    }
    
    Write-DeploymentLog "MSBuild encontrado: $msbuildPath" -Level Info
    Write-DeploymentLog "Compilando em modo $BuildConfiguration..." -Level Info
    
    if ($PSCmdlet.ShouldProcess($slnPath, "Build Solution")) {
        try {
            $buildOutput = & $msbuildPath $slnPath `
                /p:Configuration=$BuildConfiguration `
                /p:Platform="Any CPU" `
                /t:Rebuild `
                /v:minimal `
                /nologo
            
            if ($LASTEXITCODE -eq 0) {
                Write-DeploymentLog "✓ Build concluído com sucesso" -Level Success
                return $true
            } else {
                Write-DeploymentLog "✗ Build falhou com código de saída: $LASTEXITCODE" -Level Error
                Write-DeploymentLog "Output: $($buildOutput -join "`n")" -Level Error
                return $false
            }
        }
        catch {
            Write-DeploymentLog "✗ Erro ao executar build: $_" -Level Error
            return $false
        }
    }
    
    return $true
}

function Copy-ComponentDLLs {
    Write-SectionHeader "COPIANDO DLLs PARA DIRETÓRIOS SSIS"
    
    $binPath = Join-Path $SourcePath "src\01_Source\bin\$BuildConfiguration"
    $copyCount = 0
    $errorCount = 0
    
    foreach ($dll in $script:ComponentDLLs) {
        $sourcePath = Join-Path $binPath $dll.Name
        
        if (-not (Test-Path $sourcePath)) {
            Write-DeploymentLog "Arquivo não encontrado: $sourcePath" -Level Error
            $errorCount++
            continue
        }
        
        Write-DeploymentLog "Processando: $($dll.Name)" -Level Info
        
        foreach ($targetPathKey in $dll.TargetPaths) {
            $targetDir = $script:SSISPaths[$targetPathKey]
            $targetPath = Join-Path $targetDir $dll.Name
            
            if ($PSCmdlet.ShouldProcess($targetPath, "Copy DLL")) {
                try {
                    # Backup de DLL existente
                    if (Test-Path $targetPath) {
                        $backupPath = "$targetPath.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
                        Copy-Item $targetPath $backupPath -Force
                        Write-DeploymentLog "  → Backup criado: $backupPath" -Level Info
                    }
                    
                    # Copiar nova DLL
                    Copy-Item $sourcePath $targetPath -Force
                    Write-DeploymentLog "  ✓ Copiado para: $targetPathKey" -Level Success
                    $copyCount++
                }
                catch {
                    Write-DeploymentLog "  ✗ Erro ao copiar para $targetPathKey : $_" -Level Error
                    $errorCount++
                }
            }
        }
    }
    
    Write-DeploymentLog "" -Level Info
    Write-DeploymentLog "Resumo: $copyCount cópias realizadas, $errorCount erros" -Level Info
    
    return ($errorCount -eq 0)
}

function Register-SSISComponents {
    Write-SectionHeader "REGISTRANDO COMPONENTES NO GAC (OPCIONAL)"
    
    # Nota: Registro no GAC é opcional para componentes SSIS
    # Necessário apenas se usar Strong Name Signing
    
    Write-DeploymentLog "Registro GAC não implementado (opcional)" -Level Info
    Write-DeploymentLog "Se usar Strong Name, executar: gacutil /i <dll>" -Level Info
    
    return $true
}

function Test-Deployment {
    Write-SectionHeader "VALIDANDO DEPLOYMENT"
    
    $allValid = $true
    
    foreach ($dll in $script:ComponentDLLs) {
        foreach ($targetPathKey in $dll.TargetPaths) {
            $targetDir = $script:SSISPaths[$targetPathKey]
            $targetPath = Join-Path $targetDir $dll.Name
            
            if (Test-Path $targetPath) {
                $fileInfo = Get-Item $targetPath
                $version = $fileInfo.VersionInfo.FileVersion
                Write-DeploymentLog "✓ $($dll.Name) em $targetPathKey (v$version)" -Level Success
            } else {
                Write-DeploymentLog "✗ $($dll.Name) NÃO encontrada em $targetPathKey" -Level Error
                $allValid = $false
            }
        }
    }
    
    return $allValid
}

function Export-DeploymentReport {
    Write-SectionHeader "GERANDO RELATÓRIO DE DEPLOYMENT"
    
    $reportPath = Join-Path $SourcePath "deployment\DeploymentReport_$(Get-Date -Format 'yyyyMMdd_HHmmss').txt"
    
    $report = @"
═══════════════════════════════════════════════════════════════
QUATTO API CLIENT - DEPLOYMENT REPORT
═══════════════════════════════════════════════════════════════

Data/Hora: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Ambiente: $TargetEnvironment
Source Path: $SourcePath
Build Configuration: $BuildConfiguration

═══════════════════════════════════════════════════════════════
RESUMO
═══════════════════════════════════════════════════════════════

Total de Logs: $($script:DeploymentLog.Count)
Avisos: $($script:DeploymentWarnings.Count)
Erros: $($script:DeploymentErrors.Count)

Status: $(if ($script:DeploymentErrors.Count -eq 0) { "✓ SUCESSO" } else { "✗ FALHOU" })

═══════════════════════════════════════════════════════════════
LOG COMPLETO
═══════════════════════════════════════════════════════════════

$($script:DeploymentLog -join "`n")

═══════════════════════════════════════════════════════════════
AVISOS
═══════════════════════════════════════════════════════════════

$($script:DeploymentWarnings -join "`n")

═══════════════════════════════════════════════════════════════
ERROS
═══════════════════════════════════════════════════════════════

$($script:DeploymentErrors -join "`n")

═══════════════════════════════════════════════════════════════
FIM DO RELATÓRIO
═══════════════════════════════════════════════════════════════
"@

    if ($PSCmdlet.ShouldProcess($reportPath, "Export Report")) {
        $report | Out-File -FilePath $reportPath -Encoding UTF8
        Write-DeploymentLog "✓ Relatório exportado: $reportPath" -Level Success
    }
}

function Show-PostDeploymentInstructions {
    Write-SectionHeader "PRÓXIMOS PASSOS"
    
    Write-Host "1. REINICIAR VISUAL STUDIO / SSDT" -ForegroundColor Yellow
    Write-Host "   → Feche todas as instâncias do Visual Studio" -ForegroundColor Gray
    Write-Host "   → Abra novamente para carregar os novos componentes" -ForegroundColor Gray
    Write-Host ""
    
    Write-Host "2. VERIFICAR COMPONENTE NA TOOLBOX" -ForegroundColor Yellow
    Write-Host "   → Abra um pacote SSIS" -ForegroundColor Gray
    Write-Host "   → Adicione um Data Flow Task" -ForegroundColor Gray
    Write-Host "   → Procure por 'Quatto Corporate API Source' na Toolbox" -ForegroundColor Gray
    Write-Host ""
    
    Write-Host "3. CONFIGURAR CONNECTION MANAGER" -ForegroundColor Yellow
    Write-Host "   → Connection Managers → New Connection" -ForegroundColor Gray
    Write-Host "   → Selecione 'API' na lista" -ForegroundColor Gray
    Write-Host "   → Configure AuthType, Token, etc." -ForegroundColor Gray
    Write-Host ""
    
    Write-Host "4. EXECUTAR SQL SCRIPTS" -ForegroundColor Yellow
    Write-Host "   → Execute database\01_Complete_Database_Setup.sql" -ForegroundColor Gray
    Write-Host "   → Valide criação de tabelas e procedures" -ForegroundColor Gray
    Write-Host ""
    
    Write-Host "5. CRIAR PRIMEIRO PACOTE" -ForegroundColor Yellow
    Write-Host "   → Use examples\Sample_Package_Structure.txt como guia" -ForegroundColor Gray
    Write-Host "   → Configure propriedades do componente" -ForegroundColor Gray
    Write-Host "   → Execute e valide logs" -ForegroundColor Gray
    Write-Host ""
    
    if ($script:DeploymentErrors.Count -gt 0) {
        Write-Host "⚠ ATENÇÃO: Deployment teve $($script:DeploymentErrors.Count) erro(s)" -ForegroundColor Red
        Write-Host "Verifique o relatório de deployment para detalhes" -ForegroundColor Red
    }
}

# ═══════════════════════════════════════════════════════════════
# EXECUÇÃO PRINCIPAL
# ═══════════════════════════════════════════════════════════════

function Main {
    $startTime = Get-Date
    
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "QUATTO API CLIENT - AUTOMATED DEPLOYMENT" -ForegroundColor Cyan
    Write-Host "Versão: 1.0.0" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Ambiente: $TargetEnvironment" -ForegroundColor White
    Write-Host "Source: $SourcePath" -ForegroundColor White
    Write-Host "WhatIf: $($WhatIfPreference)" -ForegroundColor White
    Write-Host ""
    
    try {
        # Etapa 1: Validar pré-requisitos
        if (-not $SkipValidation) {
            if (-not (Test-Prerequisites)) {
                throw "Pré-requisitos não atendidos. Corrija os erros e tente novamente."
            }
        } else {
            Write-DeploymentLog "⚠ Validação de pré-requisitos foi pulada (-SkipValidation)" -Level Warning
        }
        
        # Etapa 2: Build (opcional)
        if ($BuildSolution) {
            if (-not (Invoke-BuildSolution)) {
                throw "Build da solution falhou. Verifique os erros de compilação."
            }
        } else {
            Write-DeploymentLog "Build pulado (use -BuildSolution para compilar)" -Level Info
        }
        
        # Etapa 3: Copiar DLLs
        if (-not (Copy-ComponentDLLs)) {
            throw "Falha ao copiar DLLs. Verifique permissões e caminhos."
        }
        
        # Etapa 4: Registrar componentes (opcional)
        Register-SSISComponents | Out-Null
        
        # Etapa 5: Validar deployment
        if (-not $SkipValidation) {
            if (-not (Test-Deployment)) {
                throw "Validação pós-deployment falhou."
            }
        }
        
        # Etapa 6: Gerar relatório
        Export-DeploymentReport
        
        # Etapa 7: Instruções
        Show-PostDeploymentInstructions
        
        $endTime = Get-Date
        $duration = ($endTime - $startTime).TotalSeconds
        
        Write-Host ""
        Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Green
        Write-Host "✓✓✓ DEPLOYMENT CONCLUÍDO COM SUCESSO ✓✓✓" -ForegroundColor Green
        Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Green
        Write-Host "Duração: $($duration) segundos" -ForegroundColor Green
        Write-Host ""
        
        exit 0
    }
    catch {
        Write-Host ""
        Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Red
        Write-Host "✗✗✗ DEPLOYMENT FALHOU ✗✗✗" -ForegroundColor Red
        Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Red
        Write-DeploymentLog "Erro fatal: $_" -Level Error
        Write-DeploymentLog $_.ScriptStackTrace -Level Error
        
        Export-DeploymentReport
        
        Write-Host ""
        Write-Host "Verifique o relatório de deployment para detalhes." -ForegroundColor Yellow
        Write-Host ""
        
        exit 1
    }
}

# Executar
Main