# Integration Tests - Quatto API Client

> End-to-end testing com APIs reais e mock servers

---

## 📋 Overview

Integration tests para validar:
- ✅ Real API connections
- ✅ Data transformation
- ✅ Error handling
- ✅ End-to-end flows
- ✅ Database operations

---

## 🏗️ Project Structure

```
05_IntegrationTests/
├── QuattoAPIClient.IntegrationTests.csproj
├── Fixtures/
│   ├── ApiFixture.cs
│   ├── DatabaseFixture.cs
│   └── MockServerFixture.cs
├── Tests/
│   ├── ApiConnectionTests.cs
│   ├── DataFlowTests.cs
│   ├── ErrorHandlingTests.cs
│   ├── EndToEndTests.cs
│   └── PerformanceTests.cs
├── Helpers/
│   ├── TestDataGenerator.cs
│   ├── MockApiServer.cs
│   └── TestDatabaseHelper.cs
└── appsettings.json
```

---

## 🔧 Setup

### Dependencies

```xml
<ItemGroup>
  <PackageReference Include="xUnit" Version="2.6.6" />
  <PackageReference Include="Moq" Version="4.20.70" />
  <PackageReference Include="coverlet.collector" Version="6.0.0" />
  <PackageReference Include="WireMock.Net" Version="1.6.8" />
  <PackageReference Include="TestContainers" Version="3.8.0" />
  <PackageReference Include="SQLServerMocking" Version="1.0.0" />
</ItemGroup>
```

### Fixtures

```csharp
// ApiFixture.cs
public class ApiFixture : IAsyncLifetime
{
    private readonly WireMockServer _server;
    
    public async Task InitializeAsync()
    {
        _server = WireMockServer.Start();
        _server.Given(Request.Create()
            .WithPath("/user/repos")
            .UsingGet())
        .RespondWith(Response.Create()
            .WithBody("[ ... ]")
            .WithStatusCode(200));
    }
    
    public async Task DisposeAsync()
    {
        _server?.Stop();
    }
}
```

---

## 🧪 Test Categories

### 1. API Connection Tests

```csharp
[Fact]
public async Task ValidConnection_ReturnsSuccess()
{
    // Arrange
    var config = new ApiConfiguration 
    { 
        BaseUrl = "https://api.github.com",
        Timeout = 30
    };
    
    // Act
    var result = await _client.TestConnection(config);
    
    // Assert
    Assert.True(result.IsSuccess);
}

[Fact]
public async Task InvalidUrl_ReturnsError()
{
    // Arrange
    var config = new ApiConfiguration 
    { 
        BaseUrl = "https://invalid.api.com"
    };
    
    // Act
    var result = await _client.TestConnection(config);
    
    // Assert
    Assert.False(result.IsSuccess);
}

[Fact]
public async Task TimeoutExceeded_ThrowsException()
{
    // Arrange
    var config = new ApiConfiguration 
    { 
        BaseUrl = "https://slow-api.com",
        Timeout = 1  // Very short timeout
    };
    
    // Act & Assert
    await Assert.ThrowsAsync<TimeoutException>(
        () => _client.FetchData(config));
}
```

---

### 2. Data Flow Tests

```csharp
[Fact]
public async Task JsonToDatabaseFlow_LoadsDataCorrectly()
{
    // Arrange
    var apiData = new[] 
    {
        new { id = 1, name = "Repo1", stars = 100 },
        new { id = 2, name = "Repo2", stars = 200 }
    };
    _mockServer.SetupResponse(apiData);
    
    // Act
    var result = await _dataFlow.Execute();
    
    // Assert
    var dbData = _db.GetRepositories();
    Assert.Equal(2, dbData.Count);
    Assert.Equal("Repo1", dbData[0].Name);
}

[Fact]
public async Task DataTransformation_AppliesCorrectly()
{
    // Arrange
    var rawData = "{\"stars\": \"100\", \"date\": \"2025-01-01\"}";
    
    // Act
    var transformed = DataTransformer.Transform(rawData);
    
    // Assert
    Assert.Equal(100, transformed.Stars);
    Assert.Equal(new DateTime(2025, 1, 1), transformed.Date);
}

[Fact]
public async Task LargeDataSet_ProcessesWithoutError()
{
    // Arrange
    var largeDataSet = GenerateTestData(100000);
    
    // Act
    var result = await _dataFlow.ProcessBatch(largeDataSet);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(100000, result.RecordsProcessed);
}
```

---

### 3. Error Handling Tests

```csharp
[Fact]
public async Task ApiError_ReturnsWithoutCrashing()
{
    // Arrange
    _mockServer.SetupError(500, "Internal Server Error");
    
    // Act
    var result = await _client.FetchData();
    
    // Assert
    Assert.NotNull(result.Error);
    Assert.Contains("Internal Server Error", result.Error.Message);
}

[Fact]
public async Task RetryLogic_RetriesOnTransientError()
{
    // Arrange
    var callCount = 0;
    _mockServer.SetupConditional(() =>
    {
        callCount++;
        return callCount <= 2 ? 503 : 200;  // Fail first 2 times
    });
    
    // Act
    var result = await _client.FetchDataWithRetry(
        maxRetries: 3, 
        retryDelayMs: 100);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(3, callCount);
}

[Fact]
public async Task InvalidData_HandlesGracefully()
{
    // Arrange
    var invalidJson = "{ invalid json }";
    
    // Act
    var result = await _dataFlow.ProcessJson(invalidJson);
    
    // Assert
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ValidationErrors);
}

[Fact]
public async Task RateLimitExceeded_QueuesForRetry()
{
    // Arrange
    _mockServer.SetupRateLimit(429, retryAfter: 60);
    
    // Act
    var result = await _client.FetchDataWithRateLimit();
    
    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(429, result.StatusCode);
}
```

---

### 4. End-to-End Tests

```csharp
[Fact]
public async Task FullPipeline_GitHubToSqlServer()
{
    // Arrange
    var config = new PipelineConfiguration
    {
        ApiConnection = "GitHub",
        Endpoint = "/user/repos",
        DatabaseConnection = "TestDb",
        Table = "[dbo].[TestRepositories]"
    };
    
    // Act
    var result = await _pipeline.Execute(config);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.True(result.RecordsLoaded > 0);
    
    // Verify database
    var dbRecords = _testDb.GetRecords("[dbo].[TestRepositories]");
    Assert.NotEmpty(dbRecords);
}

[Fact]
public async Task MultiSourcePipeline_LoadsFromMultipleApis()
{
    // Arrange
    var config = new PipelineConfiguration
    {
        Sources = new[] 
        {
            new ApiSource { Name = "GitHub", Endpoint = "/user/repos" },
            new ApiSource { Name = "GitLab", Endpoint = "/projects" }
        }
    };
    
    // Act
    var result = await _pipeline.ExecuteMultiSource(config);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(2, result.CompletedSources);
    Assert.True(result.TotalRecords > 0);
}

[Fact]
public async Task IncrementalLoad_OnlyLoadsNewRecords()
{
    // Arrange
    var firstRun = await _pipeline.Execute();
    var initialCount = firstRun.RecordsLoaded;
    
    // Add new data to mock API
    _mockServer.AddNewRecords(10);
    
    // Act
    var secondRun = await _pipeline.ExecuteIncremental();
    
    // Assert
    Assert.Equal(10, secondRun.RecordsLoaded);  // Only new ones
}
```

---

### 5. Performance Tests

```csharp
[Fact]
public async Task FetchData_Completes_UnderThreshold()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();
    
    // Act
    var result = await _client.FetchData(10000);  // 10K records
    
    // Assert
    stopwatch.Stop();
    Assert.True(stopwatch.ElapsedMilliseconds < 5000);  // <5 seconds
    Assert.Equal(10000, result.RecordsCount);
}

[Fact]
public async Task MemoryUsage_RemainsUnder_MaxThreshold()
{
    // Arrange
    var process = Process.GetCurrentProcess();
    var initialMemory = process.WorkingSet64;
    
    // Act
    var result = await _client.FetchData(100000);  // 100K records
    
    // Assert
    var finalMemory = process.WorkingSet64;
    var memoryUsed = (finalMemory - initialMemory) / 1024 / 1024;  // MB
    Assert.True(memoryUsed < 500);  // <500 MB
}

[Fact]
public async Task Throughput_Meets_Minimum()
{
    // Arrange
    var recordCount = 50000;
    
    // Act
    var stopwatch = Stopwatch.StartNew();
    var result = await _client.FetchData(recordCount);
    stopwatch.Stop();
    
    // Assert
    var throughput = recordCount / stopwatch.Elapsed.TotalSeconds;
    Assert.True(throughput > 500);  // >500 records/sec
}
```

---

## 🚀 Running Integration Tests

### Visual Studio

```
Test Explorer → Run All Tests (in 05_IntegrationTests)
```

### Command Line

```powershell
# Run all integration tests
dotnet test 05_IntegrationTests/

# Run specific test class
dotnet test 05_IntegrationTests/ -k ApiConnectionTests

# Run with detailed output
dotnet test 05_IntegrationTests/ -v d

# Run with code coverage
dotnet test 05_IntegrationTests/ /p:CollectCoverage=true
```

---

## 🔧 Mock Server Setup

### WireMock Configuration

```csharp
public class MockApiServer
{
    private readonly WireMockServer _server;
    
    public void SetupRepositoriesEndpoint()
    {
        _server.Given(Request.Create()
            .WithPath("/user/repos")
            .WithParam("page", "*")
            .WithParam("per_page", "*")
            .UsingGet())
        .RespondWith(Response.Create()
            .WithBody(GetMockRepositories())
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json"));
    }
    
    public void SetupErrorEndpoint()
    {
        _server.Given(Request.Create()
            .WithPath("/error"))
        .RespondWith(Response.Create()
            .WithBody("{\"message\": \"Error\"}")
            .WithStatusCode(500));
    }
}
```

---

## 📊 Test Coverage

### Target Coverage

```
API Client:        >90%
Data Flow:         >85%
Error Handling:    >95%
Logging:           >80%
Transformations:   >85%

TOTAL:             >85% code coverage
```

### Running Coverage Report

```powershell
# Generate coverage report
dotnet test 05_IntegrationTests/ `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover

# View in IDE or web
# Open coverage.xml in VS Code with coverage extension
```

---

## 🐛 Debugging Tests

### Visual Studio Debugging

```
1. Set breakpoint in test method
2. Right-click test → Debug Selected Tests
3. Use debugging tools (Watch, Immediate, etc.)
```

### Logging in Tests

```csharp
[Fact]
public async Task TestWithLogging()
{
    var logger = LoggerFactory.GetLogger<MyTest>();
    
    logger.LogInformation("Test started");
    
    // Your test code
    
    logger.LogSuccess("TestWithLogging", "Test completed");
}
```

---

## 📈 Test Execution Time

```
API Connection Tests:      ~2-3 seconds
Data Flow Tests:           ~5-10 seconds
Error Handling Tests:      ~3-5 seconds
End-to-End Tests:          ~15-30 seconds
Performance Tests:         ~30-60 seconds

TOTAL:                     ~60-120 seconds
```

---

## ✅ Continuous Integration

### GitHub Actions Integration

```yaml
# In .github/workflows/integration-tests.yml
name: Integration Tests

on: [push, pull_request]

jobs:
  integration-tests:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - uses: microsoft/setup-msbuild@v1
    
    - run: dotnet test 05_IntegrationTests/ -v d
    
    - name: Upload Coverage
      uses: codecov/codecov-action@v3
```

---

## 📚 Best Practices

### ✅ Do

```
✅ Use fixtures for setup/teardown
✅ Mock external APIs
✅ Test happy path AND error cases
✅ Keep tests independent
✅ Use descriptive names
✅ One assertion per test (when possible)
✅ Clean up resources
```

### ❌ Don't

```
❌ Test implementation details
❌ Use real APIs in tests
❌ Have test interdependencies
❌ Ignore timeout issues
❌ Skip cleanup
❌ Use slow assertions
```

---

## 🎯 Integration Test Roadmap

### Current (v1.0)
```
✅ API connection tests
✅ Data flow tests
✅ Error handling tests
✅ End-to-end tests
✅ Performance tests
```

### Planned (v1.1)
```
⏳ Security tests (SQL injection, XSS)
⏳ Load tests (1M+ records)
⏳ Chaos testing (network failures)
⏳ Stress tests (sustained load)
```

---

**Last Updated:** 2026-02-20  
**Version:** 1.0.0

