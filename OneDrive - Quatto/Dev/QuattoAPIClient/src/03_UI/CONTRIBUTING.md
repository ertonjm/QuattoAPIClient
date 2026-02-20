# Contributing to Quatto API Client for SSIS

First off, thank you for considering contributing to Quatto API Client for SSIS! It's people like you that make this such a great tool.

## 📋 Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code.

## How Can I Contribute?

### Reporting Bugs 🐛

Before creating bug reports, please check the issue list as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

* **Use a clear and descriptive title**
* **Describe the exact steps which reproduce the problem**
* **Provide specific examples to demonstrate the steps**
* **Describe the behavior you observed after following the steps**
* **Explain which behavior you expected to see instead and why**
* **Include screenshots and animated GIFs if possible**
* **Include your environment details:**
  - OS version
  - SQL Server version
  - SSIS version
  - Visual Studio version
  - .NET Framework version

### Suggesting Enhancements ✨

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

* **Use a clear and descriptive title**
* **Provide a step-by-step description of the suggested enhancement**
* **Provide specific examples to demonstrate the steps**
* **Describe the current behavior and expected behavior**
* **Explain why this enhancement would be useful**

### Pull Requests 🔄

* Fill in the required template
* Follow the styleguides
* Include appropriate test cases
* End all files with a newline
* Avoid platform-specific code
* Document new code based on the Documentation Styleguide

## 📚 Git Workflow

Before starting, please read our complete [Git Workflow Guide](GIT_WORKFLOW.md). It covers:

* **Branch naming conventions** (feature/, bugfix/, etc)
* **Commit message format** (Conventional Commits)
* **Semantic versioning** (MAJOR.MINOR.PATCH)
* **Git Flow strategy** (main → develop → feature)

## Styleguides

### Git Commit Messages

We use **Conventional Commits** format. See [GIT_WORKFLOW.md](GIT_WORKFLOW.md#-commit-messages) for detailed examples.

**Format:**
```
<type>(<scope>): <description>

<body>

<footer>
```

**Types:**
* `feat` - New feature
* `fix` - Bug fix
* `docs` - Documentation
* `style` - Code style (formatting, missing semicolons, etc)
* `refactor` - Code refactor (no behavior change)
* `perf` - Performance improvement
* `test` - Add or modify tests
* `ci` - CI/CD changes
* `chore` - Dependencies, build, etc
* `build` - Build system changes

**Scopes:**
* `HttpHelper`, `RetryPolicy`, `HttpResponse`, `LatencyStats`
* `ConnectionManager`, `Models`, `UI`
* `Samples`, `Docs`, `CI/CD`, `Dependencies`

**Examples:**
```
feat(HttpHelper): add support for custom retry backoff strategies
fix(ConnectionManager): resolve connection leak in dispose
docs(README): add installation guide
refactor(Models): simplify response serialization
test(HttpHelper): add rate limiting tests
```

**Rules:**
* Use imperative mood ("add" not "added")
* Don't capitalize first letter
* No period (.) at the end
* First line < 50 chars
* Body < 72 chars per line
* Reference issues: "Closes #123"

### C# Styleguide

* Use PascalCase for class names, method names, and constants
* Use camelCase for local variables and parameters
* Use meaningful, descriptive names
* Follow [Microsoft C# Coding Conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
* Follow the included `.editorconfig` file
* Prefix private fields with underscore (`_privatefield`)

**Example:**

```csharp
public class HttpHelper
{
    private readonly HttpClient _httpClient;
    private readonly RetryPolicy _retryPolicy;

    /// <summary>
    /// Executes GET request with automatic retry
    /// </summary>
    public HttpResponse ExecuteGetWithRetry(string url, Guid correlationId)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentNullException(nameof(url));

        return ExecuteWithRetry(
            () => _httpClient.GetAsync(url),
            url,
            "GET",
            correlationId
        );
    }
}
```

**Code Organization:**
* One class per file
* Public members before private
* Properties before methods
* Static members first
* Order by access level: public → protected → private

### Documentation Styleguide

* Use Markdown for all documentation
* Include code examples where applicable
* Keep docs synchronized with code changes
* Use clear, concise language
* Provide step-by-step instructions for setup guides
* Use relative links to reference other docs

**XML Documentation Comments:**

Add XML docs to public methods and classes:

```csharp
/// <summary>
/// Executes HTTP request with retry policy
/// </summary>
/// <param name="url">The endpoint URL</param>
/// <param name="correlationId">Correlation ID for logging</param>
/// <returns>HTTP response with status and body</returns>
/// <exception cref="ArgumentNullException">Thrown when url is null</exception>
public HttpResponse ExecuteGetWithRetry(string url, Guid correlationId)
{
    // Implementation
}
```

### Test Styleguide

* Use AAA pattern (Arrange-Act-Assert)
* Descriptive test names: `Method_Scenario_ExpectedBehavior()`
* One assertion per test (ideally)
* Mock external dependencies
* Test behavior, not implementation
* Keep tests independent and repeatable

**Example:**

```csharp
[TestMethod]
public void ExecuteGetWithRetry_MaxRetriesExceeded_ReturnsFailureResponse()
{
    // Arrange
    var httpClient = new Mock<HttpClient>();
    var metadata = new Mock<IDTSComponentMetaData100>();
    var retryPolicy = new RetryPolicy { MaxAttempts = 3 };
    var helper = new HttpHelper(httpClient.Object, metadata.Object, retryPolicy);

    httpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
        .ThrowsAsync(new HttpRequestException("Connection failed"));

    // Act
    var result = helper.ExecuteGetWithRetry("https://api.github.com", Guid.NewGuid());

    // Assert
    Assert.IsFalse(result.Success);
    Assert.AreEqual(0, result.StatusCode);
}
```

## Development Setup

1. Clone the repository
2. Open `QuattoAPIClient.sln` in Visual Studio 2022
3. Restore NuGet packages
4. Build the solution
5. Run tests: `Ctrl+R, A` in Visual Studio

## Testing

* All changes require tests
* Run all tests before submitting PR: `Ctrl+R, A`
* Maintain or improve code coverage (currently 70%+)
* Use the Test Explorer for test management

## Documentation

* Update README if changing functionality
* Update ARCHITECTURE.md if changing design
* Update CHANGELOG.md with your changes
* Include code examples where appropriate
* Keep documentation clear and concise

## Additional Notes

### Issue and Pull Request Labels

This section lists the labels we use to help organize and categorize issues and pull requests.

* `bug` - Something isn't working
* `enhancement` - New feature or request
* `documentation` - Improvements or additions to documentation
* `good first issue` - Good for newcomers
* `help wanted` - Extra attention is needed
* `question` - Further information is requested
* `wontfix` - This will not be worked on

## Community

* Join discussions on GitHub
* Check out the documentation
* Share your experiences and ideas

## Recognition

Contributors will be recognized in:
* [AUTHORS.md](AUTHORS.md)
* Pull request comments
* Release notes

---

## Questions?

Don't hesitate to contact us:
* 📧 support@quatto.com.br
* 💬 GitHub Discussions
* 🐛 GitHub Issues

---

**Thank you for contributing!** 🎉

Last Updated: 2026-02-20

