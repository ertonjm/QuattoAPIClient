# GitHub API Setup Guide - Sample 1

> Configuração completa para usar GitHub API com Quatto API Client

---

## 🔑 Passo 1: Gerar Personal Access Token

### No GitHub.com

```
1. Vá a https://github.com/settings/tokens
2. Clique em "Generate new token" → "Generate new token (classic)"
3. Configure:
   - Token name: "Quatto API Client Sample"
   - Expiration: 90 days (ou conforme sua política)
   - Scopes: 
     ✓ public_repo (acessar repositórios públicos)
     ✓ read:user (ler perfil do usuário)
4. Clique em "Generate token"
5. COPIE O TOKEN IMEDIATAMENTE (não aparecerá novamente!)
```

**Token Format:**
```
ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

### Armazenar Token com Segurança

```powershell
# ✅ MELHOR: Armazenar em variável de ambiente (sesão)
$env:GITHUB_TOKEN = "seu_token_aqui"

# ❌ NUNCA: Hardcode em scripts ou arquivos de configuração
# ❌ NUNCA: Commitar token em Git

# Para persistir em Windows (User):
[System.Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "seu_token", "User")

# Verificar:
$env:GITHUB_TOKEN
```

---

## 🧪 Passo 2: Testar GitHub API

### Teste 1: API Health Check

```powershell
# Test se a API está funcionando
$headers = @{
    "Authorization" = "Bearer seu_token_aqui"
    "Accept" = "application/vnd.github.v3+json"
}

$response = Invoke-RestMethod -Uri "https://api.github.com" `
    -Headers $headers

Write-Host "GitHub API Status: OK"
Write-Host "Rate Limit: $($response.rate_limit)"
```

### Teste 2: Validar Token

```powershell
# Verificar que o token está válido
$token = $env:GITHUB_TOKEN
$headers = @{
    "Authorization" = "Bearer $token"
    "Accept" = "application/vnd.github.v3+json"
}

try {
    $user = Invoke-RestMethod -Uri "https://api.github.com/user" `
        -Headers $headers
    
    Write-Host "✅ Token válido!"
    Write-Host "GitHub User: $($user.login)"
    Write-Host "Name: $($user.name)"
}
catch {
    Write-Host "❌ Token inválido!"
    Write-Host "Error: $($_.Exception.Message)"
}
```

### Teste 3: Listar Repositórios

```powershell
# Listar repositórios do usuário
$token = $env:GITHUB_TOKEN
$headers = @{
    "Authorization" = "Bearer $token"
    "Accept" = "application/vnd.github.v3+json"
}

$repos = Invoke-RestMethod -Uri "https://api.github.com/user/repos?page=1&per_page=5" `
    -Headers $headers

Write-Host "Repositórios encontrados: $($repos.Count)"
foreach ($repo in $repos) {
    Write-Host "- $($repo.name) ($($repo.language)) ⭐ $($repo.stargazers_count)"
}
```

---

## 📊 API Endpoints Reference

### Main Endpoint (para Sample 1)

```
GET https://api.github.com/ertonjm/QuattoAPIClient
```

**Parâmetros:**
```
page=1                    # Página (começa em 1)
per_page=30              # Registros por página (1-100)
sort=updated             # Campo para sort
order=desc               # Ordem (asc/desc)
```

**Response Example:**
```json
[
  {
    "id": 123456789,
    "name": "QuattoAPIClient",
    "full_name": "ertonjm/QuattoAPIClient",
    "description": "My awesome repository",
    "html_url": "[ertonjm/QuattoAPIClient](https://github.com/ertonjm/QuattoAPIClient)",
    "stargazers_count": 100,
    "forks_count": 10,
    "language": "Python",
    "created_at": "2025-01-01T00:00:00Z",
    "updated_at": "2025-02-20T12:34:56Z"
  }
]
```

---

## ⚡ Rate Limits

### GitHub API Rate Limits

```
Sem autenticação:
├─ 60 requisições por hora
├─ Por IP público
└─ Muito restritivo para testes

Com Token (Bearer):
├─ 5.000 requisições por hora
├─ Por usuário
└─ Adequado para testes e produção
```

### Verificar Rate Limit

```powershell
# Ver status de rate limit
$token = $env:GITHUB_TOKEN
$headers = @{
    "Authorization" = "Bearer $token"
}

$rateLimit = Invoke-RestMethod -Uri "https://api.github.com/rate_limit" `
    -Headers $headers

Write-Host "Rate Limit Remaining: $($rateLimit.rate_limit.remaining)"
Write-Host "Rate Limit Total: $($rateLimit.rate_limit.limit)"
Write-Host "Reset Time: $(Convert-UnixTime $rateLimit.rate_limit.reset)"
```

---

## 🔄 Monitorar Requisições

### Headers de Rate Limit

```
Cada resposta contém:

X-RateLimit-Limit: 5000        # Total disponível
X-RateLimit-Remaining: 4999    # Ainda disponível
X-RateLimit-Reset: 1708012345  # Timestamp Unix de reset
```

**Script para verificar:**

```powershell
$response = Invoke-WebRequest -Uri "https://api.github.com/user/repos" `
    -Headers $headers `
    -Method Get

$remaining = $response.Headers["X-RateLimit-Remaining"]
$limit = $response.Headers["X-RateLimit-Limit"]
$resetTime = [System.DateTimeOffset]::FromUnixTimeSeconds($response.Headers["X-RateLimit-Reset"]).DateTime

Write-Host "Requisições restantes: $remaining / $limit"
Write-Host "Reset em: $resetTime"
```

---

## 🆘 Troubleshooting

### Erro: "401 Unauthorized"

**Causa:** Token inválido ou expirado

**Solução:**
```powershell
# 1. Verificar se token está configurado
echo $env:GITHUB_TOKEN

# 2. Se vazio, configurar novamente
[System.Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "seu_token", "User")

# 3. Abrir novo PowerShell (para carregar var de ambiente)
# 4. Testar novamente

# 5. Se ainda não funcionar, gerar novo token
# GitHub Settings → Personal access tokens → Generate new token
```

### Erro: "403 Forbidden"

**Causa:** Rate limit excedido ou permissões insuficientes

**Solução:**
```powershell
# 1. Verificar rate limit
$rateLimit = Invoke-RestMethod -Uri "https://api.github.com/rate_limit" `
    -Headers @{"Authorization" = "Bearer $env:GITHUB_TOKEN"}

# 2. Se remaining = 0, aguardar reset
$resetTime = [System.DateTimeOffset]::FromUnixTimeSeconds($rateLimit.rate_limit.reset).DateTime
Write-Host "Requisições zeram. Aguarde até $resetTime"

# 3. Verificar scopes do token
# GitHub Settings → Personal access tokens → Selecionar token
# Verifique se tem "public_repo" e "read:user"
```

### Erro: "422 Unprocessable Entity"

**Causa:** Parâmetro de URL incorreto

**Solução:**
```powershell
# Verificar formato da URL
# Correto:   https://api.github.com/user/repos?page=1&per_page=30
# Incorreto: https://api.github.com/user/repos?page=1&perPage=30

# Testar URL manualmente:
Invoke-RestMethod -Uri "https://api.github.com/user/repos?page=1&per_page=30" `
    -Headers $headers
```

---

## 📋 Checklist de Setup

```
✅ GitHub Account criada
✅ Personal Access Token gerado
✅ Token armazenado em $env:GITHUB_TOKEN
✅ Token testado com API (Teste 2)
✅ Repositórios listados com sucesso (Teste 3)
✅ Rate limit verificado
✅ Token tem scopes corretos (public_repo, read:user)
✅ Pronto para usar em SSIS!
```

---

## 🔗 Próximo Passo

Depois que GitHub API estiver configurado e testado:

👉 **Ir para:** [03_SSIS_Package_Setup.md](03_SSIS_Package_Setup.md)

---

## 📚 Referências

- [Quatto API Client Repository](https://github.com/ertonjm/QuattoAPIClient)
- [GitHub REST API Docs](https://docs.github.com/en/rest)
- [Personal Access Tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)
- [Rate Limiting](https://docs.github.com/en/rest/overview/rate-limits-for-the-rest-api)

---

**Tempo estimado:** 10-15 minutos  
**Dificuldade:** Fácil ✅

