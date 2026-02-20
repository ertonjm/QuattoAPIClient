# 🏗️ Arquitetura - Quatto API Client for SSIS

> Documentação técnica detalhada da arquitetura, componentes e fluxos de dados

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Componentes Principais](#componentes-principais)
3. [Fluxo de Dados](#fluxo-de-dados)
4. [Camadas](#camadas)
5. [Decisões de Design](#decisões-de-design)
6. [Padrões Utilizados](#padrões-utilizados)

---

## 🎯 Visão Geral

**Quatto API Client** é um componente SSIS de 4 camadas:

```
┌───────────────────────────────────────────────┐
│  Camada 1: INTERFACE (Windows Forms)          │
│  CorporateApiSourceUI + ApiSourceWizard       │
└───────────────────────────────────────────────┘
                    ↓
┌───────────────────────────────────────────────┐
│  Camada 2: COMPONENTE (Pipeline Component)    │
│  CorporateApiSource                           │
└───────────────────────────────────────────────┘
                    ↓
┌───────────────────────────────────────────────┐
│  Camada 3: CONEXÃO (Connection Manager)       │
│  ApiConnectionManager + OAuth2TokenManager    │
└───────────────────────────────────────────────┘
                    ↓
┌───────────────────────────────────────────────┐
│  Camada 4: TRANSPORTE (HTTP/HTTPS)            │
│  System.Net.Http.HttpClient                   │
└───────────────────────────────────────────────┘
```

---

## 🔧 Componentes Principais

### 1. CorporateApiSourceUI (IDtsComponentUI)

**Localização:** `src/03_UI/CorporateApiSourceUI.cs`  
**Responsabilidade:** Interface do componente no SSIS Designer  
**Implementa:** `IDtsComponentUI` interface

```csharp
public class CorporateApiSourceUI : IDtsComponentUI
{
    // Métodos principais:
    public void Initialize(object metadata, IServiceProvider provider);
    public bool Edit(IWin32Window window, object variables, object connections);
    public void New(IWin32Window window);
    public void Delete(IWin32Window window);
    public void Help(IWin32Window window);
}
```

**Responsabilidades:**
- ✅ Gerenciar lifecycle do componente
- ✅ Validar input do usuário
- ✅ Persistir configurações
- ✅ Mostrar ajuda contextual
- ✅ Registrar operações (logging)

**Padrão:** Singleton com Logger injetado

---

### 2. ApiSourceWizard (Multi-step Form)

**Localização:** `src/03_UI/Forms/ApiSourceWizard.cs`  
**Responsabilidade:** Wizard de configuração intuitivo  
**Base:** `System.Windows.Forms.Form`

```csharp
public partial class ApiSourceWizard : Form
{
    // Tabs do Wizard:
    // 1. Connection Tab - Seleção de conexão API
    // 2. Query Tab - Configuração de endpoint
    // 3. Advanced Tab - Rate limit, timeout, retry
    // 4. Watermark Tab - Configuração incremental
    
    private void SaveValues();
    private void LoadCurrentValues();
    private bool ValidateProperties();
}
```

**Features:**
- ✅ 4 abas de configuração
- ✅ Validação em tempo real
- ✅ Preview de endpoint
- ✅ Tooltips informativos
- ✅ Persistência de valores

---

### 3. CorporateApiSource (PipelineComponent)

**Localização:** `src/01_Source/Components/CorporateApiSource.cs`  
**Responsabilidade:** Componente de source do SSIS  
**Base:** `Microsoft.SqlServer.Dts.Pipeline.PipelineComponent`

```csharp
[DtsPipelineComponent(...)]
public class CorporateApiSource : PipelineComponent
{
    // Métodos principais:
    public override DTSValidationStatus Validate();
    public override void ProvideComponentProperties();
    public override void PrimeOutput(int outputs, int[] outputIDs, 
                                    PipelineBuffer[] buffers);
}
```

**Fluxo de Execução:**
1. `ProvideComponentProperties()` - Definir schema
2. `Validate()` - Validar propriedades
3. `PrimeOutput()` - Executar requisição HTTP
4. Mapear JSON → Buffer → SQL

---

### 4. ApiConnectionManager (Gerenciador de Conexão)

**Localização:** `src/02_ConnectionManager/ApiConnectionManager.cs`  
**Responsabilidade:** Gerenciar autenticação e conexão HTTP  
**Base:** `ConnectionManagerBase` (SSIS)

```csharp
[DtsConnection(...)]
public class ApiConnectionManager : ConnectionManagerBase
{
    // Métodos principais:
    public override object AcquireConnection(object transaction);
    public override void ReleaseConnection(object connection);
    public override DTSExecResult Validate(IDTSInfoEvents infoEvents);
}
```

**Funcionalidades:**
- ✅ Bearer Token authentication
- ✅ API Key headers
- ✅ OAuth2 with refresh
- ✅ Token caching
- ✅ Connection pooling

---

### 5. Helpers (Utilidades)

#### HttpHelper
```csharp
// Realiza requisições HTTP
public class HttpHelper
{
    public async Task<string> GetAsync(string url);
    public async Task<string> PostAsync(string url, string content);
}
```

#### SchemaMapper
```csharp
// Mapeia JSON schema para SSIS columns
public class SchemaMapper
{
    public static SchemaMapper FromJson(string json, IDTSComponentMetaData100 metadata);
    public void MapToBuffer(PipelineBuffer buffer, JsonElement record);
}
```

#### WatermarkManager
```csharp
// Gerencia carregamentos incrementais
public class WatermarkManager
{
    public DateTime? GetLastWatermark();
    public void UpdateWatermark(DateTime value);
}
```

---

## 📊 Fluxo de Dados

### Fluxo 1: Execução no SSIS Package

```
┌─────────────────────────┐
│  SSIS Data Flow Task    │
└─────────────────────────┘
           ↓
┌─────────────────────────┐
│  CorporateApiSource     │ (Este é nosso componente)
│  (Pipeline Component)   │
└─────────────────────────┘
           ↓
┌─────────────────────────┐
│  Transformação/Destino  │
│  (SQL, OLE DB, etc)     │
└─────────────────────────┘
```

### Fluxo 2: Dentro do CorporateApiSource

```
1. PrimeOutput() chamado pelo SSIS
            ↓
2. Obter conexão do ApiConnectionManager
            ↓
3. Montar URL: {BaseUrl}/{Endpoint}
            ↓
4. Adicionar parâmetros de paginação
            ↓
5. Executar requisição HTTP (GET)
            ↓
6. Parsear JSON response
            ↓
7. Mapear campos JSON → Colunas SSIS
            ↓
8. Popular PipelineBuffer
            ↓
9. Enviar para componente seguinte
            ↓
10. Repetir até EOF (ou max registros)
```

### Fluxo 3: Autenticação

```
┌─────────────────────────────────┐
│  Token vencido?                 │
└─────────────────────────────────┘
      ↙             ↘
    SIM              NÃO
     ↓               ↓
┌──────────┐    ┌──────────┐
│ Refresh  │    │ Use      │
│ OAuth2   │    │ Cached   │
└──────────┘    │ Token    │
     ↓          └──────────┘
┌──────────┐         ↓
│ Cache    │    ┌──────────┐
│ New      │    │ Requisição
│ Token    │    │ com Token
└──────────┘    └──────────┘
```

---

## 🏗️ Camadas

### Camada 1: Interface (UI Layer)

**Responsabilidades:**
- Interface com usuário
- Validação de input
- Persistência de propriedades
- Wizard configuration

**Componentes:**
- `CorporateApiSourceUI` - Controller principal
- `ApiSourceWizard` - Multi-step form
- `LoggerFactory` - Logging centralizado

**Dependências:**
- `System.Windows.Forms`
- `Microsoft.SqlServer.Dts.*`
- `Microsoft.Extensions.Logging`

---

### Camada 2: Pipeline Component

**Responsabilidades:**
- Processar dados do SSIS
- Orquestrar fluxo de dados
- Mapear JSON → SQL schema
- Gerenciar paginação

**Componentes:**
- `CorporateApiSource` - Component principal
- `HttpHelper` - Requisições HTTP
- `SchemaMapper` - Mapeamento JSON
- `WatermarkManager` - Incremental loads

**Dependências:**
- `Microsoft.SqlServer.Dts.Pipeline`
- `System.Net.Http`
- `System.Text.Json`

---

### Camada 3: Connection Manager

**Responsabilidades:**
- Gerenciar autenticação
- Pooling de conexões
- Refresh de tokens
- Encriptação de credentials

**Componentes:**
- `ApiConnectionManager` - Manager principal
- `OAuth2TokenManager` - Token lifecycle
- `TokenRefreshHandler` - Refresh automático

**Dependências:**
- `Microsoft.SqlServer.Dts.Runtime`
- `System.Net.Http`
- `System.Text.Json`

---

### Camada 4: Transport (HTTP)

**Responsabilidades:**
- Requisições HTTP/HTTPS
- TLS/SSL
- Retry com backoff
- Timeouts

**Componentes:**
- `System.Net.Http.HttpClient`
- Custom retry logic
- Backoff exponencial

---

## 🎨 Decisões de Design

### 1. Singleton Pattern para Logger

**Decisão:** Usar `LoggerFactory` singleton  
**Razão:** Centralizar configuração de logging  
**Implementação:** Thread-safe com lock

```csharp
public static class LoggerFactory
{
    private static ILoggerFactory? _loggerFactory;
    private static readonly object _lock = new object();
    
    public static ILoggerFactory Factory
    {
        get
        {
            if (_loggerFactory == null)
            {
                lock (_lock) { /* initialize */ }
            }
            return _loggerFactory;
        }
    }
}
```

---

### 2. OAuth2 com Caching

**Decisão:** Cache tokens com refresh automático  
**Razão:** Reduzir chamadas ao servidor de auth  
**Implementação:** In-memory com TTL

```csharp
private class TokenInfo
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public string GetToken()
{
    if (_tokenInfo?.ExpiresAt > DateTime.UtcNow)
        return _tokenInfo.Token;
    
    return RefreshToken(); // Call OAuth2 endpoint
}
```

---

### 3. Paginação Automática

**Decisão:** Iterar automaticamente sobre páginas  
**Razão:** Suportar APIs grande datasets  
**Implementação:** Loop com offset/limit

```csharp
int pageSize = properties["PageSize"];
int page = 0;

while (true)
{
    var url = BuildUrl(page, pageSize);
    var data = await httpClient.GetAsync(url);
    
    if (!data.Any()) break;
    
    yield return data;
    page++;
}
```

---

### 4. Watermark Incremental

**Decisão:** Suportar carregamentos incrementais  
**Razão:** Melhorar performance em grandes volumes  
**Implementação:** Armazenar último valor processado

```csharp
if (properties["EnableIncremental"])
{
    var lastWatermark = watermarkManager.GetLastWatermark();
    url += $"&since={lastWatermark:O}";
}

// Após processar
watermarkManager.UpdateWatermark(maxDate);
```

---

## 🔄 Padrões Utilizados

### 1. Factory Pattern

```csharp
// LoggerFactory
public static ILogger<T> GetLogger<T>() { ... }

// Connection pooling
private static Dictionary<string, HttpClient> _clientPool;
```

### 2. Strategy Pattern

```csharp
// Diferentes estratégias de autenticação
public interface IAuthStrategy
{
    Task<string> AuthenticateAsync();
}

public class BearerTokenStrategy : IAuthStrategy { }
public class ApiKeyStrategy : IAuthStrategy { }
public class OAuth2Strategy : IAuthStrategy { }
```

### 3. Repository Pattern

```csharp
// Watermark repository
public interface IWatermarkRepository
{
    DateTime? GetLastWatermark(string key);
    void SaveWatermark(string key, DateTime value);
}
```

### 4. Logging Pattern (Observer)

```csharp
// Logs observam operações
logger.LogInformation("Operation started");
logger.LogSuccess("Operation", "Completed");
logger.LogError(ex, "Operation failed");
```

---

## 🔐 Segurança

### Autenticação

✅ **Bearer Token** - Headers seguros  
✅ **API Key** - Parâmetros opcionais  
✅ **OAuth2** - Refresh tokens com TTL  

### Criptografia

✅ **HTTPS/TLS** - Obrigatório  
✅ **Password masking** - Nunca log de senhas  
✅ **Conexão segura** - SSIS Connection encryption  

### Validação

✅ **Input validation** - Todas as entradas  
✅ **Schema validation** - JSON contra schema  
✅ **SQL injection prevention** - Parameterized queries  

---

## 🧪 Testes por Camada

### Camada 1: UI (14 testes)
- CorporateApiSourceUI (Initialize, Edit, Help)
- IDtsComponentUI compliance
- Argument validation

### Camada 2: Component (22 testes)
- ValidateBaseUrl (URLs válidas/inválidas)
- ValidatePageSize (ranges)
- Watermark validation
- Parsing (Int, Bool)

### Camada 3: Logging (11 testes)
- LoggerFactory singleton
- Thread-safe operations
- Extensions (LogSuccess, LogOperationError)

---

## 📈 Performance

### Otimizações

✅ **Connection pooling** - Reusar HttpClient  
✅ **Token caching** - Reduzir auth calls  
✅ **Async/await** - Operações não-bloqueantes  
✅ **Batch processing** - Buffer size otimizado  

### Métricas Esperadas

```
API Latency: ~50-200ms (rede)
Component Overhead: ~5-10ms
Token Refresh: ~100-500ms (on-demand)
Throughput: ~1000-5000 rows/sec
```

---

## 🔗 Dependências Externas

```
Microsoft.SqlServer.Dts.* (SSIS Runtime)
System.Net.Http (HTTP client)
System.Text.Json (JSON parsing)
System.Windows.Forms (UI)
Microsoft.Extensions.Logging (Logging)
Newtonsoft.Json (JSON utilities)
```

---

## 📊 Diagrama de Classes (Simplificado)

```
┌──────────────────────────┐
│  IDtsComponentUI         │
│  (SSIS Interface)        │
└────────────┬─────────────┘
             │ implements
┌────────────▼─────────────┐
│ CorporateApiSourceUI     │
│ + Initialize()           │
│ + Edit()                 │
│ + Help()                 │
└────────────┬─────────────┘
             │ uses
       ┌─────▼─────────┬───────────────┐
       │               │               │
┌──────▼──────────┐   │   ┌───────────▼────────┐
│ApiSourceWizard  │   │   │ LoggerFactory      │
│(Multi-tab Form) │   │   │ (Singleton)        │
└─────────────────┘   │   └────────────────────┘
                      │
            ┌─────────▼──────────┐
            │ CorporateApiSource │
            │ (PipelineComponent)│
            └──────────┬─────────┘
                       │
            ┌──────────┼──────────┐
            │          │          │
       ┌────▼──────┐ ┌─▼────────┐ ┌─▼─────────┐
       │HttpHelper │ │SchemaMapper│WatermarkMgr│
       └───────────┘ └──────────┘ └────────────┘
```

---

## 🎯 Próximas Melhorias Arquiteturais

- [ ] Adicionar caching de metadata
- [ ] Implementar circuit breaker pattern
- [ ] Adicionar metrics/observability
- [ ] Suporte a graphQL queries
- [ ] Plugin system para transformações

---

**Versão:** 1.0.0  
**Última Atualização:** 2026-02-20  
**Autor:** Erton Miranda / Quatto Consultoria

