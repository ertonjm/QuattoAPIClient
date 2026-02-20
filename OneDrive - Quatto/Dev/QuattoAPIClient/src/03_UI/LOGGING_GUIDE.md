# 📊 Logging Estruturado - Guia Rápido

## 🎯 Visão Geral

O projeto Quatto API Client agora usa **Microsoft.Extensions.Logging** para logging estruturado e profissional.

### ✅ O que foi implementado

- ✅ **LoggerFactory** singleton centralizado
- ✅ Logging em **CorporateApiSourceUI.cs**
- ✅ Logging em **ApiSourceWizard.cs**
- ✅ Logging preparado em **ApiConnectionManager.cs**
- ✅ Exemplos de uso em **LoggingExamples.cs**
- ✅ NuGet packages: `Microsoft.Extensions.Logging 8.0.0`

---

## 🚀 Como Usar

### Opção 1: Logging Simples

```csharp
var logger = LoggerFactory.GetLogger<MinhaClasse>();
logger.LogInformation("Operação iniciada");
```

### Opção 2: Logging com Contexto Estruturado

```csharp
var userId = 123;
logger.LogInformation("User {UserId} logged in", userId);
```

### Opção 3: Com Escopo para Correlação

```csharp
using (var scope = new LogScope(_logger, "ProcessOrder", correlationId))
{
    logger.LogInformation("Processando pedido");
    // ... lógica
    logger.LogSuccess("ProcessOrder", "Pedido processado");
}
```

### Opção 4: Logs de Erro

```csharp
try
{
    // ... operação
}
catch (Exception ex)
{
    logger.LogOperationError("SaveData", ex);
}
```

---

## 📝 Níveis de Log

| Nível | Uso | Exemplo |
|-------|-----|---------|
| **Debug** | Informações detalhadas de debug | `logger.LogDebug("Valor: {V}", value)` |
| **Information** | Eventos normais | `logger.LogInformation("Operação OK")` |
| **Warning** | Situações potencialmente problemáticas | `logger.LogWarning("Alto uso de CPU")` |
| **Error** | Erros que precisam atenção | `logger.LogError(ex, "Erro ao salvar")` |
| **Critical** | Falhas graves | `logger.LogCritical(ex, "Sistema instável")` |

---

## 🔧 Configuração

### Padrão (Développement)

```csharp
// Console + Debug window
// LogLevel: Information
var logger = LoggerFactory.GetLogger<MyClass>();
```

### Produção (Futuro)

```csharp
// File output com Serilog
// LogLevel: Warning
// Retenção: 30 dias
```

---

## 📂 Estrutura de Arquivos

```
Logging/
├── LoggerFactory.cs          ← Classe central
├── LoggingExamples.cs        ← Exemplos de uso
└── README.md                 ← Este arquivo
```

---

## ✨ Extensões Disponíveis

```csharp
// Sucesso
logger.LogSuccess("Operação", "Mensagem");

// Erro operacional
logger.LogOperationError("Operação", ex);

// Aviso
logger.LogWarning("Mensagem");
```

---

## 🎓 Exemplos Completos

### Exemplo 1: UI Form

```csharp
public class MeuForm : Form
{
    private readonly ILogger<MeuForm> _logger;

    public MeuForm()
    {
        _logger = LoggerFactory.GetLogger<MeuForm>();
        _logger.LogInformation("Formulário criado");
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        using (var scope = new LogScope(_logger, "SaveData"))
        {
            try
            {
                _logger.LogInformation("Salvando dados");
                // ... lógica
                _logger.LogSuccess("SaveData", "Dados salvos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar: {Message}", ex.Message);
            }
        }
    }
}
```

### Exemplo 2: API Call

```csharp
public async Task<Response> FetchDataAsync(string url)
{
    _logger.LogInformation("Requisição para {Url}", url);

    try
    {
        var response = await client.GetAsync(url);
        _logger.LogInformation("Resposta: {StatusCode}", response.StatusCode);
        return await response.Content.ReadAsAsync<Response>();
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Erro ao chamar API {Url}", url);
        throw;
    }
}
```

### Exemplo 3: Validação

```csharp
private bool ValidateInput(string input)
{
    _logger.LogDebug("Validando input: {Length} chars", input.Length);

    if (string.IsNullOrEmpty(input))
    {
        _logger.LogWarning("Input vazio");
        return false;
    }

    _logger.LogDebug("Validação OK");
    return true;
}
```

---

## 🔍 Visualizando Logs

### Visual Studio - Debug Output

```
[14:30:45.123] info: QuattoAPIClient.UI.CorporateApiSourceUI
      Initialize com metadata e service provider
[14:30:46.456] info: QuattoAPIClient.UI.Forms.ApiSourceWizard
      Carregando valores de configuração atual
[14:30:47.789] info: QuattoAPIClient.UI.Forms.ApiSourceWizard
      ✓ SUCCESS: SaveValues - Todas as configurações salvas com sucesso
```

### Console Application

```csharp
// Logs aparecem no console automaticamente
```

---

## ⚙️ Configuração Avançada

### Adicionar File Output (Futuro)

```csharp
services.AddLogging(builder =>
{
    builder
        .AddConsole()
        .AddDebug()
        .AddFile("/var/logs/app.log"); // Com Serilog
});
```

### Mudar Log Level

```csharp
builder.SetMinimumLevel(LogLevel.Debug);  // Mais verboso
builder.SetMinimumLevel(LogLevel.Warning); // Menos verboso
```

---

## 📚 Boas Práticas

### ✅ BOM

```csharp
logger.LogInformation("User {UserId} logged in", userId);
logger.LogError(ex, "Failed to process order {OrderId}", orderId);
using (var scope = new LogScope(logger, "Operation")) { ... }
```

### ❌ RUIM

```csharp
logger.LogInformation($"User {userId} logged in");
logger.LogError(ex, "Error occurred");
logger.LogInformation("Processing order"); // sem OrderId
```

---

## 🔗 Referências

- [Microsoft.Extensions.Logging](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging)
- [Structured Logging](https://github.com/serilog/serilog/wiki)
- [Best Practices](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)

---

## 📞 Dúvidas?

Consulte **Logging\LoggingExamples.cs** para mais exemplos práticos!

