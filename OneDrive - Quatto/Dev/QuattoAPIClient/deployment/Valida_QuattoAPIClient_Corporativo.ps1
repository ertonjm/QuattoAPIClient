<#
.SYNOPSIS
  Validação automática do ambiente Quatto API Client para SSIS - versão corporativa

.DESCRIPTION
  Verifica pré-requisitos de build, deploy, execução e integrações específicas do ambiente SESC-DF DW/Quatto.
#>

Write-Host "═══════════════════════════════════════════════════════════"
Write-Host "Validação Automática do Ambiente - Quatto API Client SSIS"
Write-Host "═══════════════════════════════════════════════════════════"

# 1. Verificar DLLs SSIS (igual ao script anterior)
# ... [mantém o bloco anterior de DLLs]

# 2. Verificar .NET Framework
# ... [mantém o bloco anterior de .NET]

# 3. Verificar permissões de pasta
# ... [mantém o bloco anterior de permissões]

# 4. Verificar scripts SQL principais no banco
# ... [mantém o bloco anterior de tabelas principais]

# 5. Verificar tabelas de staging e índices
Write-Host "`n[5] Verificando tabelas de staging e índices..."
$stagingTables = @("stg.STG_Gladium_Orders", "stg.STG_Errors_Orders")
try {
    $conn = New-Object System.Data.SqlClient.SqlConnection $connectionString
    $conn.Open()
    foreach ($tbl in $stagingTables) {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SELECT OBJECT_ID('$tbl','U')"
        $result = $cmd.ExecuteScalar()
        if ($result -ne $null -and $result -gt 0) {
            Write-Host "  ✔ Tabela de staging encontrada: $tbl" -ForegroundColor Green
        } else {
            Write-Host "  ✖ Tabela de staging NÃO encontrada: $tbl" -ForegroundColor Red
        }
    }
    $conn.Close()
} catch {
    Write-Host "  ✖ Erro ao conectar ao banco para validação de staging." -ForegroundColor Red
}

# 6. Verificar procedures customizadas
Write-Host "`n[6] Verificando procedures customizadas..."
$procedures = @("dbo.usp_ApiWatermark_Get", "dbo.usp_ApiWatermark_Upsert", "dbo.usp_ApiRawJson_Insert")
try {
    $conn = New-Object System.Data.SqlClient.SqlConnection $connectionString
    $conn.Open()
    foreach ($proc in $procedures) {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SELECT OBJECT_ID('$proc','P')"
        $result = $cmd.ExecuteScalar()
        if ($result -ne $null -and $result -gt 0) {
            Write-Host "  ✔ Procedure encontrada: $proc" -ForegroundColor Green
        } else {
            Write-Host "  ✖ Procedure NÃO encontrada: $proc" -ForegroundColor Red
        }
    }
    $conn.Close()
} catch {
    Write-Host "  ✖ Erro ao conectar ao banco para validação de procedures." -ForegroundColor Red
}

# 7. Verificar parâmetros sensíveis no SSISDB (simulação)
Write-Host "`n[7] Verificando parâmetros sensíveis no SSISDB..."
$ssisParams = @("ApiToken", "OAuth2_ClientId", "OAuth2_ClientSecret", "Environment", "DW_ConnectionString")
foreach ($param in $ssisParams) {
    # Simulação: peça para o usuário validar manualmente ou integre com API do SSISDB se disponível
    Write-Host "  [ ] Verifique se o parâmetro '$param' está configurado e marcado como Sensível." -ForegroundColor Yellow
}

# 8. Checagem de conectividade com APIs externas (exemplo Gladium)
Write-Host "`n[8] Testando conectividade com API externa (exemplo Gladium)..."
try {
    $url = "https://api.gladium.com.br/v1/status"
    $response = Invoke-WebRequest -Uri $url -Method Get -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "  ✔ Conectividade OK com API Gladium." -ForegroundColor Green
    } else {
        Write-Host "  ✖ Falha ao conectar à API Gladium. Status: $($response.StatusCode)" -ForegroundColor Red
    }
} catch {
    Write-Host "  ✖ Erro ao conectar à API Gladium: $_" -ForegroundColor Red
}

# 9. Verificar arquivos de mapeamento JSON e exemplos de pacotes
Write-Host "`n[9] Verificando arquivos de mapeamento e exemplos..."
$files = @(
    "C:\Dev\QuattoAPIClient\examples\SchemaMapping_Gladium.json",
    "C:\Dev\QuattoAPIClient\examples\Sample_Package_Structure.txt"
)
foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  ✔ Arquivo encontrado: $file" -ForegroundColor Green
    } else {
        Write-Host "  ✖ Arquivo NÃO encontrado: $file" -ForegroundColor Red
    }
}

# 10. Validar grupos de disponibilidade SQL Server (Always On)
Write-Host "`n[10] Validando grupos de disponibilidade SQL Server (Always On)..."
try {
    $conn = New-Object System.Data.SqlClient.SqlConnection $connectionString
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) FROM sys.availability_groups"
    $result = $cmd.ExecuteScalar()
    if ($result -gt 0) {
        Write-Host "  ✔ Grupo de disponibilidade configurado." -ForegroundColor Green
    } else {
        Write-Host "  ✖ Nenhum grupo de disponibilidade encontrado." -ForegroundColor Red
    }
    $conn.Close()
} catch {
    Write-Host "  ✖ Erro ao validar grupos de disponibilidade." -ForegroundColor Red
}

Write-Host "`n═══════════════════════════════════════════════════════════"
Write-Host "Validação concluída. Revise os itens marcados com ✖ ou [ ]."
Write-Host "═══════════════════════════════════════════════════════════"