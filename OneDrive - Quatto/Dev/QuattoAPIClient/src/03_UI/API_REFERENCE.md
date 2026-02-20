# API Reference - Quatto API Client for SSIS

> Complete reference for all public classes, methods, and properties

---

## 📋 Quick Navigation

- [UI Components](#ui-components)
- [Logging Framework](#logging-framework)
- [Data Models](#data-models)
- [Enumerations](#enumerations)
- [Exceptions](#exceptions)

---

## UI Components

### CorporateApiSourceUI

**Namespace:** `QuattoAPIClient.UI`  
**Base Class:** `IDtsComponentUI`  
**Purpose:** Main UI controller for SSIS Designer integration

#### Public Methods

```csharp
public class CorporateApiSourceUI : IDtsComponentUI
{
    /// <summary>
    /// Initialize the component UI with metadata and service provider
    /// </summary>
    public void Initialize(object metadata, IServiceProvider provider);
    
    /// <summary>
    /// Show configuration dialog for editing component properties
    /// </summary>
    /// <returns>True if configuration was saved, false if cancelled</returns>
    public bool Edit(IWin32Window window, object variables, object connections);
    
    /// <summary>
    /// Create a new instance of the component
    /// </summary>
    public void New(IWin32Window window);
    
    /// <summary>
    /// Delete the component
    /// </summary>
    public void Delete(IWin32Window window);
    
    /// <summary>
    /// Display help documentation
    /// </summary>
    public void Help(IWin32Window window);
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| MetaData | IDTSComponentMetaData100 | Component metadata |
| Connections | IDTSConnections100 | Available connections |
| Variables | Variables | SSIS variables |

#### Exceptions

- `ArgumentNullException` - If metadata or provider is null
- `InvalidOperationException` - If not initialized before calling Edit
- `ApplicationException` - For unexpected errors

---

### ApiSourceWizard

**Namespace:** `QuattoAPIClient.UI.Forms`  
**Base Class:** `Form`  
**Purpose:** Multi-step configuration wizard

#### Public Methods

```csharp
public partial class ApiSourceWizard : Form
{
    /// <summary>
    /// Initialize wizard with component metadata
    /// </summary>
    public void InitializeWizard(IDTSComponentMetaData100 metadata);
    
    /// <summary>
    /// Save all configured properties
    /// </summary>
    /// <returns>True if validation passed, false otherwise</returns>
    public bool SaveValues();
    
    /// <summary>
    /// Load current values from component
    /// </summary>
    public void LoadCurrentValues();
    
    /// <summary>
    /// Validate all properties before saving
    /// </summary>
    /// <returns>True if all properties are valid</returns>
    public bool ValidateProperties();
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| BaseUrl | string | API base URL |
| Endpoint | string | API endpoint path |
| PageSize | int | Rows per page (1-10000) |
| Timeout | int | Request timeout in seconds (10-600) |
| EnableIncremental | bool | Enable watermark loading |
| WatermarkColumn | string | Column for incremental updates |

---

## Logging Framework

### LoggerFactory

**Namespace:** `QuattoAPIClient.Logging`  
**Pattern:** Singleton  
**Purpose:** Centralized logger creation and management

#### Public Static Methods

```csharp
public static class LoggerFactory
{
    /// <summary>
    /// Get logger instance for generic type
    /// </summary>
    public static ILogger<T> GetLogger<T>();
    
    /// <summary>
    /// Get logger instance by category name
    /// </summary>
    public static ILogger GetLogger(string categoryName);
    
    /// <summary>
    /// Get the underlying ILoggerFactory
    /// </summary>
    public static ILoggerFactory Factory { get; }
    
    /// <summary>
    /// Clear and reset the logger factory
    /// </summary>
    public static void Reset();
    
    /// <summary>
    /// Dispose and cleanup resources
    /// </summary>
    public static void Dispose();
}
```

#### Thread Safety

- ✅ Thread-safe singleton implementation
- ✅ Lock-based synchronization
- ✅ Safe for concurrent access

#### Example Usage

```csharp
// Simple usage
var logger = LoggerFactory.GetLogger<MyClass>();
logger.LogInformation("Operation started");

// With structured data
logger.LogInformation("User {UserId} logged in at {Timestamp}", 
    userId, DateTime.UtcNow);

// With scope for correlation
using (var scope = new LogScope(logger, "ProcessData", correlationId))
{
    logger.LogSuccess("ProcessData", "Completed successfully");
}
```

---

### LoggerExtensions

**Namespace:** `QuattoAPIClient.Logging`  
**Purpose:** Extension methods for ILogger

#### Extension Methods

```csharp
public static class LoggerExtensions
{
    /// <summary>
    /// Log operation success at Information level
    /// </summary>
    public static void LogSuccess(this ILogger logger, string operation, string message);
    
    /// <summary>
    /// Log operation error with exception details
    /// </summary>
    public static void LogOperationError(this ILogger logger, string operation, Exception ex);
    
    /// <summary>
    /// Log warning message
    /// </summary>
    public static void LogWarning(this ILogger logger, string message);
    
    /// <summary>
    /// Log debug information (development only)
    /// </summary>
    public static void LogDebugInfo(this ILogger logger, string message, params object[] args);
}
```

#### Example Usage

```csharp
// Log success
logger.LogSuccess("FetchData", "Retrieved 1000 records");

// Log error
try 
{
    FetchData();
}
catch (HttpRequestException ex)
{
    logger.LogOperationError("FetchData", ex);
}

// Log warning
if (recordCount > 5000)
{
    logger.LogWarning("Large dataset detected: " + recordCount);
}
```

---

### LogScope

**Namespace:** `QuattoAPIClient.Logging`  
**Base Class:** `IDisposable`  
**Purpose:** Create correlated logging scope

#### Constructor

```csharp
public class LogScope : IDisposable
{
    /// <summary>
    /// Create a new logging scope
    /// </summary>
    public LogScope(ILogger logger, string operationName);
    
    /// <summary>
    /// Create scope with correlation ID
    /// </summary>
    public LogScope(ILogger logger, string operationName, string correlationId);
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| OperationName | string | Name of operation |
| CorrelationId | string | Correlation ID for tracing |
| StartTime | DateTime | When scope was created |

#### Example Usage

```csharp
// Using scope for operation tracking
using (var scope = new LogScope(logger, "ImportData", correlationId))
{
    logger.LogInformation("Starting import");
    
    // All logs within scope will be correlated
    ProcessData(data);
    
    logger.LogSuccess("ImportData", "Completed");
} // Scope disposed here
```

---

## Data Models

### ApiConfiguration

**Namespace:** `QuattoAPIClient.Models`  
**Purpose:** API configuration container

```csharp
public class ApiConfiguration
{
    public string BaseUrl { get; set; }
    public string Endpoint { get; set; }
    public int PageSize { get; set; }
    public int Timeout { get; set; }
    public bool EnableIncremental { get; set; }
    public string? WatermarkColumn { get; set; }
    public AuthenticationType AuthType { get; set; }
}
```

---

### AuthenticationCredentials

**Namespace:** `QuattoAPIClient.Models`  
**Purpose:** Credential storage

```csharp
public class AuthenticationCredentials
{
    public string? BearerToken { get; set; }
    public string? ApiKey { get; set; }
    public string? OAuth2ClientId { get; set; }
    public string? OAuth2ClientSecret { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
}
```

---

## Enumerations

### AuthenticationType

**Namespace:** `QuattoAPIClient.Models`  
**Purpose:** API authentication method selection

```csharp
public enum AuthenticationType
{
    /// <summary>Bearer Token (JWT) authentication</summary>
    BearerToken = 0,
    
    /// <summary>API Key in header</summary>
    ApiKey = 1,
    
    /// <summary>OAuth2 Client Credentials flow</summary>
    OAuth2 = 2
}
```

---

### DataType

**Namespace:** `QuattoAPIClient.Models`  
**Purpose:** Supported data types for schema mapping

```csharp
public enum DataType
{
    String = 0,
    Integer = 1,
    Decimal = 2,
    DateTime = 3,
    Boolean = 4,
    Guid = 5,
    Json = 6
}
```

---

## Exceptions

### ApiClientException

**Namespace:** `QuattoAPIClient.Exceptions`  
**Base Class:** `Exception`  
**Purpose:** API-specific exceptions

```csharp
public class ApiClientException : Exception
{
    public string? ErrorCode { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? ResponseBody { get; set; }
}
```

**Example:**

```csharp
try 
{
    // API call
}
catch (ApiClientException ex)
{
    logger.LogOperationError("FetchData", ex);
    // Handle specific error
}
```

---

### ValidationException

**Namespace:** `QuattoAPIClient.Exceptions`  
**Base Class:** `Exception`  
**Purpose:** Configuration validation failures

```csharp
public class ValidationException : Exception
{
    public List<string> Errors { get; set; }
}
```

---

## Validation Methods

### ApiSourceWizard Validators

```csharp
public class ApiSourceWizard
{
    /// <summary>
    /// Validate base URL format
    /// </summary>
    public static bool ValidateBaseUrl(string url);
    
    /// <summary>
    /// Validate page size range
    /// </summary>
    public static bool ValidatePageSize(int pageSize);
    
    /// <summary>
    /// Validate timeout range
    /// </summary>
    public static bool ValidateTimeout(int timeoutSeconds);
    
    /// <summary>
    /// Validate watermark column requirement
    /// </summary>
    public static bool ValidateWatermarkColumn(string column, bool enableIncremental);
}
```

---

## Configuration Properties

### Component Properties in SSIS

| Property | Type | Range | Default | Required |
|----------|------|-------|---------|----------|
| ConnectionManager | String | - | - | ✅ Yes |
| BaseUrl | String | - | - | ✅ Yes |
| Endpoint | String | - | - | ✅ Yes |
| PageSize | Int32 | 1-10000 | 500 | ❌ No |
| Timeout | Int32 | 10-600 | 30 | ❌ No |
| MaxRetries | Int32 | 0-10 | 3 | ❌ No |
| RateLimit | Int32 | 1-10000 | 120 | ❌ No |
| EnableIncremental | Boolean | - | False | ❌ No |
| WatermarkColumn | String | - | - | ✅* Yes* |

*Required only if EnableIncremental is true

---

## Event Handlers

### IDtsComponentUI Events

- `Initialize` - Component initialization
- `Edit` - Configuration dialog
- `Validate` - Property validation

---

## Best Practices

### Logging

```csharp
// ✅ Good
logger.LogInformation("Processing user {UserId}", userId);

// ❌ Bad
logger.LogInformation("Processing user " + userId);
```

### Error Handling

```csharp
// ✅ Good
try
{
    var data = await FetchData();
}
catch (HttpRequestException ex)
{
    logger.LogOperationError("FetchData", ex);
    throw;
}

// ❌ Bad
try
{
    var data = await FetchData();
}
catch { }  // Swallow exception
```

### Resource Management

```csharp
// ✅ Good - Using scope
using (var scope = new LogScope(logger, "Operation"))
{
    // Do work
}

// ❌ Bad - No scope
logger.LogInformation("Starting");
// Do work
logger.LogInformation("Done");
```

---

## Supported Versions

- **.NET Framework:** 4.7.2+
- **SSIS:** v17.100 (SQL Server 2022)
- **Visual Studio:** 2022 18.3.1+

---

## See Also

- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture details
- [LOGGING_GUIDE.md](LOGGING_GUIDE.md) - Logging implementation
- [README_TESTS.md](README_TESTS.md) - Test examples

---

**Last Updated:** 2026-02-20  
**Version:** 1.0.0

