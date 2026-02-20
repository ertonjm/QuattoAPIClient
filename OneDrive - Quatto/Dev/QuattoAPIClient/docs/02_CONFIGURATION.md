# Guia de Configuração - Quatto API Client v1.0

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Connection Manager](#connection-manager)
3. [Propriedades do Componente](#propriedades-do-componente)
4. [Schema Mapping](#schema-mapping)
5. [Parâmetros SSISDB](#parâmetros-ssisdb)
6. [Exemplos Práticos](#exemplos-práticos)

---

## 🎯 Visão Geral

O Quatto API Client possui 3 níveis de configuração:

1. **Connection Manager** - Autenticação e configurações HTTP
2. **Component Properties** - Endpoint, paginação, incremental, storage
3. **SSISDB Parameters** - Valores por ambiente (DEV/HML/PRD)

---

## 🔐 Connection Manager

### Criar Connection Manager

1. No painel **Connection Managers**, clique direito → **New Connection**
2. Selecione **API** na lista
3. Clique **Add**

### Propriedades

#### AuthType: Bearer (Token Fixo)
```
AuthType: Bearer
BearerToken: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
TimeoutSeconds: 100
DefaultHeadersJson: {"Accept":"application/json"}
SandboxMode: false
```

**Uso:** APIs que fornecem token de longa duração

#### AuthType: ApiKey (Header Customizado)
```
AuthType: ApiKey
ApiKeyHeader: x-api-key
ApiKeyValue: abc123xyz789...
TimeoutSeconds: 100
DefaultHeadersJson: {"Accept":"application/json"}
```

**Uso:** APIs que usam API Key em header customizado

#### AuthType: OAuth2ClientCredentials
```
AuthType: OAuth2ClientCredentials
TokenEndpoint: https://api.exemplo.com/oauth/token
ClientId: my_client_id
ClientSecret: my_client_secret_here
Scope: api.read api.write
TimeoutSeconds: 100
DefaultHeadersJson: {"Accept":"application/json"}
```

**Uso:** APIs que usam OAuth2 client credentials flow

### Parâmetros Sensíveis

**⚠️ NUNCA HARDCODE TOKENS!**

Use parâmetros SSISDB:
```
BearerToken: @[$Project::GladiumApiToken]
ApiKeyValue: @[$Project::PortalSESCApiKey]
ClientSecret: @[$Project::EasydentalClientSecret]
```

---

## ⚙️ Propriedades do Componente

### Categoria: Geral (Endpoint Configuration)

| Propriedade | Tipo | Descrição | Exemplo |
|-------------|------|-----------|---------|
| **BaseUrl** | String | URL base da API | `https://api.gladium.com` |
| **Endpoint** | String | Path do endpoint | `/v1/orders` |
| **HttpMethod** | String | Método HTTP | `GET` ou `POST` |
| **QueryTemplate** | String | Template de query string | `?page={Page}&pageSize={PageSize}&since={Watermark}` |
| **PageSize** | Int | Registros por página | `500` (típico: 100-1000) |

**QueryTemplate Placeholders:**
- `{Page}` - Número da página atual
- `{PageSize}` - Tamanho da página
- `{Watermark}` - Valor do watermark (incremental)

### Categoria: Paginação

| Propriedade | Tipo | Descrição | Valores |
|-------------|------|-----------|---------|
| **PaginationType** | String | Tipo de paginação | `Offset`, `Cursor`, `Link`, `None` |
| **StartPage** | Int | Página inicial | `1` (ou `0` conforme API) |
| **MaxPages** | Int | Limite de páginas | `0` = ilimitado |

**Tipos de Paginação:**

**Offset:**
```
?page=1&pageSize=500
?page=2&pageSize=500
```

**Cursor:**
```
?cursor=abc123&pageSize=500
Response: { "next_cursor": "xyz789", ... }
```

**Link:**
```
Response: { "links": { "next": "https://api.../orders?page=2" }, ... }
```

### Categoria: Incremental

| Propriedade | Tipo | Descrição | Exemplo |
|-------------|------|-----------|---------|
| **EnableIncremental** | Boolean | Habilitar incremental | `true` |
| **WatermarkColumn** | String | Campo de controle | `updatedAt`, `id`, `version` |
| **WatermarkStore** | String | Tabela de controle | `dbo.API_Watermarks` |
| **SourceSystem** | String | Nome do sistema | `Gladium`, `PortalSESC` |
| **Environment** | String | Ambiente | `DEV`, `HML`, `PRD` |

**Como funciona:**
1. Componente lê último watermark de `dbo.API_Watermarks`
2. Adiciona `since={Watermark}` na query
3. Observa maior watermark nos registros extraídos
4. Salva novo watermark no `PostExecute`

### Categoria: Raw Storage

| Propriedade | Tipo | Descrição | Valores |
|-------------|------|-----------|---------|
| **RawStoreMode** | String | Modo de armazenamento | `SqlVarbinary`, `FileSystem`, `None` |
| **RawStoreTarget** | String | Tabela ou caminho | `dbo.API_RawPayloads` ou `C:\ApiRaw\{system}\{yyyy}\{MM}` |
| **CompressRawJson** | Boolean | Comprimir com GZIP | `true` (recomendado) |
| **HashRawJson** | Boolean | Gerar SHA256 | `true` (recomendado) |

**SqlVarbinary:**
- JSON armazenado em tabela SQL
- GZIP + SHA256
- Fácil consulta e reprocesso

**FileSystem:**
- JSON em arquivos `.json.gz`
- Arquivo `.meta` com metadados
- Melhor para volumes muito altos

### Categoria: Avançado

| Propriedade | Tipo | Descrição | Padrão |
|-------------|------|-----------|--------|
| **MaxRetries** | Int | Tentativas em caso de falha | `5` |
| **BackoffMode** | String | Modo de backoff | `Exponential`, `Linear`, `Fixed` |
| **BaseDelayMs** | Int | Delay base (ms) | `1000` |
| **RateLimitRPM** | Int | Requisições por minuto | `120` |
| **TimeoutSeconds** | Int | Timeout HTTP | `100` |
| **ErrorBehavior** | String | Comportamento em erro | `RedirectRow`, `FailComponent`, `IgnoreFailure` |
| **MaxErrorsBeforeAbort** | Int | Máximo de erros | `100` |

**Backoff Modes:**

**Exponential:** `delay = baseDelay * 2^attempt`
```
Attempt 1: 1s
Attempt 2: 2s
Attempt 3: 4s
Attempt 4: 8s
Attempt 5: 16s
```

**Linear:** `delay = baseDelay * attempt`
```
Attempt 1: 1s
Attempt 2: 2s
Attempt 3: 3s
```

**Fixed:** `delay = baseDelay`
```
Todos: 1s
```

---

## 📊 Schema Mapping

### Estrutura JSON
```json
{
  "columns": [
    {
      "name": "column_name",
      "path": "$.json.path",
      "type": "DT_WSTR",
      "length": 100,
      "precision": 18,
      "scale": 2,
      "defaultValue": ""
    }
  ]
}
```

### Tipos SSIS Suportados

| Tipo SSIS | Tipo .NET | Exemplo JSON |
|-----------|-----------|--------------|
| `DT_WSTR` | String | `"John Doe"` |
| `DT_I4` | Int32 | `42` |
| `DT_I8` | Int64 | `9999999999` |
| `DT_R8` | Double | `3.14159` |
| `DT_NUMERIC` | Decimal | `123.45` |
| `DT_BOOL` | Boolean | `true` / `false` |
| `DT_DBTIMESTAMP2` | DateTime | `"2026-02-04T10:30:00Z"` |
| `DT_DATE` | Date | `"2026-02-04"` |
| `DT_GUID` | Guid | `"550e8400-e29b-41d4-a716-446655440000"` |

### JSONPath Básico
```json
// API Response
{
  "id": "123",
  "customer": {
    "name": "John Doe",
    "email": "john@example.com"
  },
  "amount": {
    "total": 150.00,
    "currency": "BRL"
  }
}
```
```json
// Schema Mapping
{
  "columns": [
    {"name": "order_id", "path": "$.id", "type": "DT_WSTR", "length": 50},
    {"name": "customer_name", "path": "$.customer.name", "type": "DT_WSTR", "length": 200},
    {"name": "customer_email", "path": "$.customer.email", "type": "DT_WSTR", "length": 100},
    {"name": "total_amount", "path": "$.amount.total", "type": "DT_NUMERIC", "precision": 18, "scale": 2},
    {"name": "currency", "path": "$.amount.currency", "type": "DT_WSTR", "length": 3}
  ]
}
```

---

## 🗄️ Parâmetros SSISDB

### Criar Parâmetros de Projeto
```sql
-- No SSISDB, navegar até:
-- Integration Services Catalogs → SSISDB → [Pasta do Projeto] → Configure

-- Parâmetros sugeridos:
```

| Parâmetro | Tipo | Sensitive | Valor (PRD) |
|-----------|------|-----------|-------------|
| `GladiumApiBaseUrl` | String | ❌ | `https://api.gladium.com` |
| `GladiumApiToken` | String | ✅ | `eyJhbGci...` |
| `PortalSESCApiBaseUrl` | String | ❌ | `https://api.portalsesc.com.br` |
| `PortalSESCApiKey` | String | ✅ | `abc123...` |
| `DW_ConnectionString` | String | ❌ | `Data Source=SQL-PRD;...` |
| `Environment` | String | ❌ | `PRD` |

### Criar Environments
```sql
-- 1. No SSISDB, criar Environments:
-- - DEV_Environment
-- - HML_Environment
-- - PRD_Environment

-- 2. Em cada environment, criar variáveis correspondentes

-- 3. Associar projeto aos environments:
--    Clique direito no projeto → Configure → References → Add
```

---

## 💡 Exemplos Práticos

### Exemplo 1: Gladium Orders (Bearer Token)

**Connection Manager:**
```
Name: GladiumAPI
AuthType: Bearer
BearerToken: @[$Project::GladiumApiToken]
```

**Component Properties:**
```
BaseUrl: @[$Project::GladiumApiBaseUrl]
Endpoint: /v1/orders
QueryTemplate: ?page={Page}&pageSize={PageSize}&updatedSince={Watermark}
PageSize: 500
EnableIncremental: true
WatermarkColumn: updatedAt
SourceSystem: Gladium
Environment: @[$Project::Environment]
```

### Exemplo 2: Portal SESC (API Key)

**Connection Manager:**
```
Name: PortalSESCAPI
AuthType: ApiKey
ApiKeyHeader: x-api-key
ApiKeyValue: @[$Project::PortalSESCApiKey]
```

**Component Properties:**
```
BaseUrl: @[$Project::PortalSESCApiBaseUrl]
Endpoint: /api/users
QueryTemplate: ?offset={Page}&limit={PageSize}&modifiedAfter={Watermark}
PageSize: 1000
PaginationType: Offset
EnableIncremental: true
WatermarkColumn: updatedAt
```

### Exemplo 3: Easydental (OAuth2)

**Connection Manager:**
```
Name: EasydentalAPI
AuthType: OAuth2ClientCredentials
TokenEndpoint: https://auth.easydental.com.br/token
ClientId: @[$Project::EasydentalClientId]
ClientSecret: @[$Project::EasydentalClientSecret]
Scope: api.read
```

**Component Properties:**
```
BaseUrl: https://api.easydental.com.br
Endpoint: /v2/appointments
PageSize: 200
RateLimitRPM: 60
```

---

## 📞 Próximos Passos

- [03_TROUBLESHOOTING.md](03_TROUBLESHOOTING.md) - Problemas comuns
- [04_USAGE.md](04_USAGE.md) - Como usar no SSIS
- [05_API_REFERENCE.md](05_API_REFERENCE.md) - Referência completa

**Suporte:** erton.miranda@quatto.com.br