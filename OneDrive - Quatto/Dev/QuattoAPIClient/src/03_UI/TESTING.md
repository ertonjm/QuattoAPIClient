# 🧪 Testing Guide - Quatto API Client for SSIS

> Estratégia, padrões e melhores práticas de testes

**Versão:** 1.0.0  
**Data:** 2025-02-20  
**Escopo:** Unit tests, Integration tests, E2E tests

---

## 📊 Estratégia de Testes

### Pirâmide de Testes
```
       ▲
      /|\
     / | \  E2E Tests (10%)
    /  |  \  - Full package execution
   /   |   \ - Real database
  /    |    \
 /─────┼─────\
/      |      \ Integration Tests (30%)
/       |       \ - Component interaction
/        |        \ - Mock external APIs
─────────┼─────────
         |      Unit Tests (60%)
         |      - Individual components
         |      - Fast & isolated
         └──────────────────────────
```

### Pirâmide de Cobertura
```
Target Coverage: 80%+

Current Status: (v1.0.0)
├─ HttpHelper: 85%
├─ RetryPolicy: 90%
├─ Models: 70%
├─ Helpers: 75%
└─ Overall: 80%
```

---

## 🧪 Unit Tests

### Estrutura AAA (Arrange-Act-Assert)
```csharp
[TestClass]
public class HttpHelperTests
{
    [TestMethod]
    public void ExecuteGetWithRetry_Success_ReturnsResponse()
    {
        // ARRANGE: Setup
        var httpClient = new Mock<HttpClient>();
        var metadata = new Mock<IDTSComponentMetaData100>();
        var helper = new HttpHelper(httpClient.Object, metadata.Object);
        
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\": 1, \"name\": \"Test\"}")
        };
        
        httpClient.Setup(x => x.GetAsync("https://api.example.com/data"))
            .ReturnsAsync(expectedResponse);
        
        // ACT: Execute
        var result = helper.ExecuteGetWithRetry("https://api.example.com/data", Guid.NewGuid());
        
        // ASSERT: Verify
        Assert.IsTrue(result.Success);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body);
    }
}
```

### Padrão de Naming
```
MethodName_Scenario_ExpectedBehavior

✅ Good:
- ExecuteGetWithRetry_MaxRetriesExceeded_ReturnsFailureResponse
- CalculateBackoffDelay_ExponentialMode_ReturnsExponentialValue
- RetryPolicy_InvalidMaxAttempts_ThrowsException

❌ Bad:
- TestExecuteGetWithRetry
- Test1
- ExecuteGetWithRetry_Test
```

### O Que Testar
```
✅ Happy Path
✅ Error Conditions
✅ Boundary Conditions
✅ Exception Handling
✅ Edge Cases

❌ Não Testar
❌ Implementação interna
❌ Código de terceiros
❌ Configuração de ambiente
```

---

## 🔗 Integration Tests

### Teste com Mock HTTP
```csharp
[TestClass]
public class HttpHelperIntegrationTests
{
    private HttpClientHandler _handler;
    private HttpClient _httpClient;
    private HttpHelper _helper;
    
    [TestInitialize]
    public void Setup()
    {
        // Mock HTTP responses
        _handler = new HttpClientHandler();
        _httpClient = new HttpClient(_handler);
        _helper = new HttpHelper(_httpClient, CreateMockMetadata());
    }
    
    [TestMethod]
    public void ExecuteWithRetry_RateLimitThenSuccess_RetriesAndSucceeds()
    {
        // Setup: First call returns 429, second returns 200
        var sequence = new[]
        {
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.OK
        };
        
        // ... test logic
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _httpClient?.Dispose();
        _handler?.Dispose();
    }
}
```

### Teste com Banco de Dados Real
```csharp
[TestClass]
public class DatabaseIntegrationTests
{
    private readonly string _connectionString = 
        "Server=localhost;Database=QuattoTest;...";
    
    [TestInitialize]
    public void Setup()
    {
        // Create test database
        CreateTestDatabase();
    }
    
    [TestMethod]
    public void InsertData_ValidData_InsertsSuccessfully()
    {
        // Arrange
        var data = new[] { 
            new { id = 1, name = "Test1" },
            new { id = 2, name = "Test2" }
        };
        
        // Act
        InsertData(data);
        
        // Assert
        var count = GetRowCount();
        Assert.AreEqual(2, count);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        // Drop test database
        DropTestDatabase();
    }
}
```

---

## 🔄 End-to-End Tests

### Teste Completo de Package
```powershell
# 1. Setup
Write-Host "Setting up test environment..."
Invoke-Sqlcmd -InputFile "setup.sql" -ServerInstance "localhost"

# 2. Execute Package
Write-Host "Executing SSIS package..."
dtexec /f "C:\packages\SamplePackage.dtsx" `
  /SET "\Package.Variables[ApiUrl].Value"="https://api.example.com"

# 3. Verify Results
Write-Host "Verifying results..."
$count = Invoke-Sqlcmd "SELECT COUNT(*) FROM dbo.Results"
Assert($count -eq 100, "Expected 100 rows")

# 4. Cleanup
Write-Host "Cleaning up..."
Invoke-Sqlcmd -InputFile "cleanup.sql"
```

### Checklist de E2E
```
✅ Database preparada
✅ SSIS package configurado
✅ API mock configurado (se offline)
✅ Execute package
✅ Verificar dados inseridos
✅ Verificar logs
✅ Verificar performance
✅ Cleanup
```

---

## 🛠️ Testing Tools

### Unit Testing Framework
```
MSTest (Visual Studio built-in)

[TestClass]      // Marca classe de teste
[TestMethod]     // Marca método de teste
[TestInitialize] // Setup antes cada teste
[TestCleanup]    // Cleanup depois cada teste

Assert.AreEqual(expected, actual)
Assert.IsTrue(condition)
Assert.IsNull(object)
Assert.Throws<Exception>(() => ...)
```

### Mocking & Stubs
```csharp
// Moq library
var mock = new Mock<IHttpClient>();
mock.Setup(x => x.GetAsync("url"))
    .ReturnsAsync(response);
    
var stub = mock.Object;
// Use stub em teste
```

### Code Coverage
```powershell
# Via OpenCover
OpenCover.Console.exe -target:vstest.console.exe `
  -targetargs:"bin\Release\Tests.dll" `
  -output:coverage.xml

# Via ReportGenerator
ReportGenerator -reports:coverage.xml -targetdir:coverage
```

---

## 📋 Test Cases por Componente

### HttpHelper Tests
```
ExecuteGetWithRetry:
  ✅ Success_ReturnsResponse
  ✅ NotFound_Returns404
  ✅ RateLimit_RetriesAndSucceeds
  ✅ MaxRetriesExceeded_ReturnsFailure
  ✅ Timeout_ReturnsFailure
  ✅ ServerError_RetiesAndSucceeds
  ✅ NetworkError_RetriesAndSucceeds
  ✅ AuthenticationError_Returns401
  ✅ Throttled_RetriesWithBackoff

ExecutePostWithRetry:
  ✅ Success_ReturnsResponse
  ✅ InvalidContent_Returns400
  ✅ Similar tests as GET...

CalculateBackoffDelay:
  ✅ Exponential_ReturnsCorrectValue
  ✅ Linear_ReturnsCorrectValue
  ✅ Fixed_ReturnsFixedValue
  ✅ MaxDelay_NeverExceeds
```

### RetryPolicy Tests
```
Construction:
  ✅ Default_HasValidDefaults
  ✅ Custom_ConstructorSetsValues
  ✅ InvalidMaxAttempts_Throws
  ✅ InvalidBackoffMode_Throws
  ✅ InvalidDelays_Throws
```

### Model Tests
```
HttpResponse:
  ✅ Creation_AllPropertiesSet
  ✅ Serialization_ToJson
  ✅ Deserialization_FromJson

LatencyStats:
  ✅ Calculation_Percentiles
  ✅ EdgeCase_SingleValue
  ✅ EdgeCase_EmptyList
```

---

## 🚀 Como Rodar Testes

### Visual Studio
```
Test Explorer (Ctrl+E, T)
  ├─ Abrir Test Explorer
  ├─ Click "Run All"
  └─ Ver resultados

Ou via command line:
  vstest.console.exe bin\Release\Tests.dll
```

### Command Line
```powershell
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter TestClass=HttpHelperTests

# Run with code coverage
dotnet test /p:CollectCoverage=true

# Run only failed tests
dotnet test --filter "Category=Critical"
```

### Continuous Integration
```yaml
# GitHub Actions
- name: Run Tests
  run: dotnet test --configuration Release
  
- name: Publish Coverage
  run: |
    dotnet test /p:CollectCoverage=true
    codecov -f coverage.xml
```

---

## 📊 Test Metrics

### Cobertura Alvo por Componente
```
HttpHelper:            90%+
Models:                85%+
Helpers:               80%+
Services:              85%+
Overall Target:        80%+
```

### Sucesso Esperado
```
Unit Tests:            100% (all pass)
Integration Tests:     95%+ (some flakiness ok)
E2E Tests:             90%+ (depends on env)
Code Coverage:         80%+
```

### Performance
```
Unit tests:   < 100ms cada
Integration:  < 1s cada
E2E tests:    < 30s total
Full suite:   < 5 min
```

---

## 🐛 Debugging Testes

### Debug em Visual Studio
```
1. Right-click test → Debug Selected Tests
2. Breakpoints funcionam normalmente
3. Locals, Watch windows disponíveis
4. Step through code
```

### Troubleshooting
```
❌ Teste flaky (passa/falha aleatoriamente)
   ✅ Aumentar timeouts
   ✅ Remover dependências de timing
   ✅ Mock external calls

❌ Teste falha em CI mas passa localmente
   ✅ Verificar environment variables
   ✅ Verificar connection strings
   ✅ Verificar time zones

❌ Teste lento
   ✅ Usar mocks ao invés de APIs reais
   ✅ Usar in-memory database
   ✅ Parallelizar tests
```

---

## ✅ Checklist para Novo Código

Antes de fazer PR:

```
Funcionalidade:
  [ ] Código implementado
  [ ] Unit tests adicionados
  [ ] Testes passam localmente
  [ ] Code coverage >= 80%
  
Code Quality:
  [ ] Sem warnings do compilador
  [ ] Segue code style
  [ ] Sem código duplicado
  [ ] Sem TODO comments (ou com issue reference)
  
Documentation:
  [ ] XML docs adicionados
  [ ] README atualizado
  [ ] CHANGELOG atualizado
  [ ] Exemplos de uso

Before submitting PR:
  [ ] Todos os testes passam
  [ ] Code review completa
  [ ] CI/CD pipeline verde
```

---

## 📚 Referências

- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Integration Testing Guide](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Moq Documentation](https://github.com/moq/moq4)
- [Test Naming Conventions](https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html)
- [OWASP Testing Guide](https://owasp.org/www-project-web-security-testing-guide/)

---

**Versão:** 1.0.0  
**Data:** 2025-02-20  
**Mantido por:** @ertonjm  
**Status:** Ativo 🚀

