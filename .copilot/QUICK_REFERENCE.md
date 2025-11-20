# Quick Reference Guide

**Purpose:** Fast lookup for common code generation scenarios and commands.

## Common Scenarios

### Creating a New Service

```csharp
// 1. Define interface
public interface ICustomerService
{
    Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken = default);
}

// 2. Implement service
public sealed class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> repository;
    private readonly ILogger<CustomerService> logger;
    
    public CustomerService(IRepository<Customer> repository, ILogger<CustomerService> logger)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(id, cancellationToken);
    }
}

// 3. Register in DI
services.AddScoped<ICustomerService, CustomerService>();
```

**References:**
- Full guidelines: `code-generation.instructions.md`
- Naming rules: No underscore prefixes, PascalCase public, camelCase private

---

### Writing Unit Tests

```csharp
[TestClass]
public sealed class CustomerServiceTests
{
    [TestMethod]
    public async Task GetCustomerAsync_WhenCustomerExists_ShouldReturnCustomer()
    {
        // Arrange
        var repository = new Mock<IRepository<Customer>>(MockBehavior.Strict);
        repository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer(1, "John Doe", "john@example.com"));
        
        var logger = new Mock<ILogger<CustomerService>>();
        var sut = new CustomerService(repository.Object, logger.Object);
        
        // Act
        var result = await sut.GetCustomerAsync(1, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("John Doe");
        repository.VerifyAll();
    }
}
```

**References:**
- Full guidelines: `unit-test.instructions.md`
- Pattern: `MethodName_WhenCondition_ShouldOutcome`
- Use Moq (strict) + FluentAssertions

---

## Essential Commands

### Build & Quality
```bash
# Full build with analysis
dotnet build -c Release

# Format code
dotnet format

# Export diagnostics
dotnet build /p:ErrorLog=./code-analysis.sarif

# Verify formatting
dotnet format --verify-no-changes
```

### Testing & Coverage
```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:CoverageReport -reporttypes:Html

# Run specific test
dotnet test --filter "FullyQualifiedName~CustomerServiceTests"
```

### Git & Commits
```bash
# Conventional commit examples
git commit -m "feat(customers): add customer search service"
git commit -m "fix(auth): resolve token expiration issue"
git commit -m "docs: update API documentation"
git commit -m "test(orders): add integration tests"
git commit -m "refactor(data): extract repository pattern"

# Create feature branch
git checkout -b feat/customer-search

# Update branch from main
git fetch origin
git rebase origin/main
```

---

## Naming Quick Reference

| Element | Convention | Example | Notes |
|---------|-----------|---------|-------|
| **Public Types** | PascalCase | `CustomerService` | Classes, interfaces, records |
| **Interfaces** | PascalCase + I | `ICustomerService` | Always prefix with 'I' |
| **Private Fields** | camelCase | `logger`, `repository` | **NO underscore prefix** |
| **Parameters** | camelCase | `customerId`, `cancellationToken` | Descriptive names |
| **Local Variables** | camelCase | `customerName`, `orderTotal` | Descriptive names |
| **Constants** | PascalCase | `MaxRetryAttempts` | Use for magic numbers |
| **Async Methods** | PascalCase + Async | `GetCustomerAsync` | Always suffix |
| **Boolean Members** | Is/Has/Can/Should | `IsValid`, `HasPermission` | Clear intent |

---

## Async Patterns Checklist

- ✅ Suffix async methods with `Async`
- ✅ Accept `CancellationToken` parameter (default: `default`)
- ✅ Return `Task` or `Task<T>`
- ✅ Use `await` for I/O operations
- ❌ Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`
- ❌ Never return `null` tasks (use `Task.CompletedTask`)

---

## Common Anti-Patterns to Avoid

| Anti-Pattern | Why | Fix |
|--------------|-----|-----|
| `_logger` (underscore prefix) | Violates naming convention | Use `logger` |
| `await task.Result` | Deadlocks, blocks threads | Use `await task` |
| `catch (Exception) { }` | Swallows all errors | Log and rethrow |
| `if (obj != null)` | Verbose | Use `if (obj is not null)` |
| `var x = 5;` (magic number) | Unclear intent | `const int MaxRetries = 5;` |
| Static service locator | Hard to test | Use constructor injection |
| Deep inheritance | Brittle, hard to maintain | Use composition |

---

## Project Structure Template

```
src/ProjectName/
├── Controllers/          # API controllers
├── Services/             # Business logic
│   ├── ICustomerService.cs
│   └── CustomerService.cs
├── Repositories/         # Data access
│   ├── ICustomerRepository.cs
│   └── CustomerRepository.cs
├── Models/              # DTOs
│   ├── CustomerRequest.cs
│   └── CustomerResponse.cs
├── Entities/            # Domain entities
│   └── Customer.cs
├── Validators/          # FluentValidation
│   └── CustomerRequestValidator.cs
├── Extensions/          # Extension methods
│   └── ServiceCollectionExtensions.cs
└── ProjectName.csproj

tests/ProjectName.UnitTests/
├── Services/
│   └── CustomerServiceTests.cs
└── ProjectName.UnitTests.csproj
```

---

## Quality Gates Quick Check

Before creating a PR:

1. ✅ Code builds without warnings
2. ✅ All tests pass
3. ✅ Code formatted (`dotnet format`)
4. ✅ No underscore prefixes
5. ✅ XML docs for public APIs
6. ✅ No TODO/dead code
7. ✅ Conventional commit message

---

## File References

| Topic | File | Description |
|-------|------|-------------|
| **Overview** | `README.md` | Complete guide to .copilot folder |
| **Base Rules** | `copilot-instructions.md` | Precedence and workflow |
| **Code Gen** | `code-generation.instructions.md` | Naming, structure, async |
| **Testing** | `unit-test.instructions.md` | Test structure, Moq, FluentAssertions |
| **Quality** | `code-quality.instructions.md` | Analyzers, nullable types |
| **Patterns** | `design-patterns.instructions.md` | Design pattern examples |
| **Repository** | `repo-standards.md` | Git, PRs, CI/CD |

---

## Getting Help

1. **For Code Generation:** See `code-generation.instructions.md`
2. **For Testing:** See `unit-test.instructions.md`
3. **For Quality Issues:** See `code-quality.instructions.md`
4. **For Pattern Examples:** See `design-patterns.instructions.md`
5. **For Repository Questions:** See `repo-standards.md`
6. **For Architecture:** See `.github/copilot-instructions.md`

---

**Last Updated:** 2025-11-20  
**Compatibility:** .NET 8+, C# 12+
