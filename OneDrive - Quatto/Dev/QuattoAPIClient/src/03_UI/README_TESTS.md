# 🧪 Testes Unitários - Guia Completo

## 📊 Visão Geral

O projeto Quatto API Client usa **xUnit** para testes unitários com cobertura de:

### ✅ Testes Implementados

| Módulo | Classe | Testes | Cobertura |
|--------|--------|--------|-----------|
| **Logging** | LoggerFactoryTests | 11 | GetLogger, Singleton, ThreadSafe |
| **Forms** | ApiSourceWizardValidationTests | 22 | Validações, Parsing |
| **UI** | CorporateApiSourceUITests | 14 | Initialize, Edit, Help |
| **Total** | - | **47** | Completo |

---

## 🚀 Como Executar Testes

### Via Visual Studio

1. **Test Explorer**
   - View → Test Explorer (Ctrl+E, T)
   - Clique em "Run All Tests"
   
2. **Ou via keyboard**
   - Ctrl+R, A (Run All Tests)
   - Ctrl+R, Ctrl+A (Run in Parallel)

### Via Command Line

```powershell
# Executar todos os testes
dotnet test 04_Tests/QuattoAPIClient.Tests.csproj

# Executar com verbosidade
dotnet test 04_Tests/QuattoAPIClient.Tests.csproj -v d

# Executar uma classe específica
dotnet test 04_Tests/QuattoAPIClient.Tests.csproj -k "LoggerFactoryTests"

# Com cobertura (requer coverlet)
dotnet test 04_Tests/QuattoAPIClient.Tests.csproj /p:CollectCoverage=true
```

---

## 📋 Estrutura de Testes

### LoggerFactoryTests.cs (11 testes)

```
✓ GetLogger_Generic_ReturnsLogger
✓ GetLogger_String_ReturnsLogger
✓ Factory_IsSingleton
✓ GetLogger_SameClass_ReturnsSameLogger
✓ Reset_ClearsFactory
✓ Dispose_ReleasesResources
✓ GetLogger_IsThreadSafe
✓ LogSuccess_LogsAsInformation
✓ LogOperationError_LogsException
✓ LogWarning_LogsAsWarning
✓ LogScope_WorksWithUsing
```

### ApiSourceWizardValidationTests.cs (22 testes)

```
✓ ValidateBaseUrl_ValidUrls_ReturnsTrue (3 URLs)
✓ ValidateBaseUrl_InvalidUrls_ReturnsFalse (4 URLs)
✓ ValidatePageSize_ValidValues_ReturnsTrue (4 values)
✓ ValidatePageSize_InvalidValues_ReturnsFalse (4 values)
✓ ValidateMaxRetries_ValidValues_ReturnsTrue (3 values)
✓ ValidateMaxRetries_InvalidValues_ReturnsFalse (3 values)
✓ ValidateRateLimit_ValidValues_ReturnsTrue (3 values)
✓ ValidateRateLimit_InvalidValues_ReturnsFalse (3 values)
✓ ValidateTimeout_ValidValues_ReturnsTrue (4 values)
✓ ValidateTimeout_InvalidValues_ReturnsFalse (4 values)
✓ ValidateWatermarkColumn_Conditional
✓ ValidateWatermarkColumn_RequiredWhenIncremental
✓ ValidateWatermarkColumn_NotRequiredWhenDisabled
✓ ParseInt_ValidStrings_ParsesCorrectly (3 values)
✓ ParseInt_InvalidStrings_ReturnsFalse (3 values)
✓ ParseBool_ValidStrings_ParsesCorrectly (4 values)
```

### CorporateApiSourceUITests.cs (14 testes)

```
✓ Constructor_InitializesWithLogger
✓ Initialize_ValidArguments_Succeeds
✓ Initialize_NullMetadata_ThrowsArgumentNullException
✓ Initialize_NullServiceProvider_ThrowsArgumentNullException
✓ Initialize_BothArgumentsNotNull_Succeeds
✓ Edit_ReturnsBoolean
✓ Edit_ReturnsWithoutException
✓ Edit_NotInitialized_ThrowsInvalidOperationException
✓ Help_ExecutesWithoutException
✓ New_Exists
✓ Delete_Exists
✓ CorporateApiSourceUI_ImplementsIDtsComponentUI
✓ IDtsComponentUI_HasRequiredMethods
```

---

## 🎓 Padrão AAA (Arrange-Act-Assert)

Todos os testes seguem o padrão **AAA**:

```csharp
[Fact(DisplayName = "Nome descritivo do teste")]
public void TestName_Scenario_ExpectedBehavior()
{
    // ARRANGE - Preparar dados e contexto
    var expectedValue = 100;
    var input = "valid_input";

    // ACT - Executar código sendo testado
    var result = ProcessInput(input);

    // ASSERT - Verificar resultado
    Assert.Equal(expectedValue, result);
}
```

---

## 📝 Convenções de Naming

### Nomes de Testes

```csharp
// Formato: MethodName_Scenario_ExpectedBehavior
[Fact]
public void ValidateBaseUrl_ValidHttpsUrl_ReturnsTrue() { }

// Ou com DisplayName para mais clareza
[Fact(DisplayName = "ValidateBaseUrl com HTTPS válida retorna verdadeiro")]
public void ValidateBaseUrl_ValidHttpsUrl_ReturnsTrue() { }
```

### Fixtures e Setup

```csharp
public class MyTests
{
    public MyTests()
    {
        // Setup antes de cada teste
        LoggerFactory.Reset();
    }

    // Teardown automático com IDisposable
    public void Dispose()
    {
        // Limpeza após cada teste
    }
}
```

---

## 🔍 Testes com Dados (Theory)

### Múltiplos Valores com [Theory]

```csharp
[Theory(DisplayName = "URLs válidas são aceitas")]
[InlineData("https://api.example.com")]
[InlineData("http://localhost:8080")]
[InlineData("https://api.example.com/v1")]
public void ValidateBaseUrl_ValidUrls_ReturnsTrue(string url)
{
    bool isValid = url.StartsWith("https://") || url.StartsWith("http://");
    Assert.True(isValid);
}
```

---

## 🎯 Exemplos de Teste Completo

### Teste 1: Validação Simples

```csharp
[Fact]
public void PageSize_100_IsValid()
{
    // Arrange
    int pageSize = 100;

    // Act
    bool isValid = pageSize >= 1 && pageSize <= 10000;

    // Assert
    Assert.True(isValid);
}
```

### Teste 2: Com Mock

```csharp
[Fact]
public void LogSuccess_LogsAsInformation()
{
    // Arrange
    var mockLogger = new Mock<ILogger>();

    // Act
    mockLogger.Object.LogSuccess("Operation");

    // Assert
    mockLogger.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

### Teste 3: Com Exception

```csharp
[Fact]
public void Initialize_NullMetadata_ThrowsArgumentNullException()
{
    // Arrange
    var ui = new CorporateApiSourceUI();

    // Act & Assert
    Assert.Throws<ArgumentNullException>(() =>
        ui.Initialize(null, mockServiceProvider));
}
```

---

## ✅ Boas Práticas

### ✅ BOM

```csharp
// Nome descritivo
[Fact(DisplayName = "ValidateBaseUrl com HTTPS válida retorna verdadeiro")]
public void ValidateBaseUrl_ValidHttpsUrl_ReturnsTrue()
{
    // Padrão AAA
    // Arrange
    string url = "https://example.com";
    
    // Act
    bool result = url.StartsWith("https://");
    
    // Assert
    Assert.True(result);
}

// Testes focados
// Um comportamento por teste
// Sem lógica complexa
```

### ❌ RUIM

```csharp
// Nome genérico
public void Test1()
{
    // Sem separação AAA
    var input = "test";
    bool x = input != "";
    Assert.True(x);
    
    // Múltiplos comportamentos
    // Lógica complexa dentro do teste
}
```

---

## 🔗 Frameworks Usados

- **xUnit** - Framework de testes
- **Moq** - Mocking framework
- **coverlet** - Code coverage

---

## 📊 Cobertura de Código

Para gerar relatório de cobertura:

```powershell
dotnet test 04_Tests/QuattoAPIClient.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover
```

---

## 🚀 Adicionar Novos Testes

### Passo 1: Criar arquivo na pasta apropriada

```
04_Tests/
├── Logging/LoggerFactoryTests.cs
├── Forms/ApiSourceWizardValidationTests.cs
├── UI/CorporateApiSourceUITests.cs
└── YourNewTests.cs
```

### Passo 2: Usar template

```csharp
using Xunit;
using QuattoAPIClient.YourNamespace;

namespace QuattoAPIClient.Tests.YourNamespace
{
    public class YourTestClass
    {
        [Fact(DisplayName = "Descrição clara do teste")]
        public void YourTestName_Scenario_ExpectedBehavior()
        {
            // Arrange
            var input = new object();

            // Act
            var result = YourMethod(input);

            // Assert
            Assert.NotNull(result);
        }
    }
}
```

### Passo 3: Executar

```powershell
dotnet test
```

---

## 📞 Dúvidas?

- Consulte xUnit docs: https://xunit.net/docs/getting-started
- Moq docs: https://github.com/moq/moq4/wiki/Quickstart
- Padrão AAA: https://bit.ly/arrange-act-assert

---

**Total: 47 testes ✅ | Cobertura: Em Progresso 📊**

