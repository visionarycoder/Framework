---
applyTo: "**/*.cs"
version: 2.0.0
lastUpdated: 2025-11-20
scope: code-generation
excludePatterns:
  - "**/tests/**"
  - "**/*.test.cs"
  - "**/*.spec.cs"
---

# Copilot Instructions: Code Generation (C#)

## Purpose

Focused heuristics for generating clean, modern, testable C# code (C# 12 / .NET 8+). This file covers code-level generation including naming, structure, API design, and error handling. See other instruction files for testing, quality, patterns, and repository guidance.

## Guiding Principles

1. **Clarity Over Cleverness:** Explicit intent beats terse code
2. **Modern Language Features:** Leverage records, primary constructors, pattern matching, file-scoped namespaces
3. **Composition Over Inheritance:** Prefer small, composable types with minimal public surface
4. **Async All the Way:** All I/O APIs must be truly async with `CancellationToken` support
5. **Fail Fast:** Validate inputs early; throw exceptions with clear messages
6. **Immutability by Default:** Use readonly, records, and init-only properties

## Naming Conventions

### Type Naming
- **Public Types:** PascalCase (`CustomerService`, `OrderProcessor`, `ProductRepository`)
- **Interfaces:** PascalCase with 'I' prefix (`ICustomerService`, `IRepository<T>`)
- **Records:** PascalCase, descriptive noun phrases (`CustomerData`, `OrderSummary`)
- **Enums:** PascalCase for type and values (`OrderStatus.Pending`, `PaymentMethod.CreditCard`)
- **Generic Type Parameters:** Single uppercase letter or PascalCase with 'T' prefix (`T`, `TEntity`, `TKey`)

### Member Naming
- **Public Methods:** PascalCase, verb phrases (`GetCustomerById`, `ProcessOrderAsync`)
- **Properties:** PascalCase, noun phrases (`FirstName`, `IsActive`, `CreatedDate`)
- **Private Fields:** camelCase **without underscore prefix** (`logger`, `repository`, `httpClient`)
- **Parameters:** camelCase, descriptive (`customerId`, `orderData`, `cancellationToken`)
- **Local Variables:** camelCase, descriptive (`customerName`, `orderTotal`, `validationResult`)
- **Constants:** PascalCase (`MaxRetryAttempts`, `DefaultTimeout`, `ApiVersion`)

### Async Naming
- **Async Methods:** Always suffix with `Async` (`GetDataAsync`, `SaveOrderAsync`, `ValidateAsync`)
- **Async Event Handlers:** Suffix with `Async` (`OnClickAsync`, `HandleSubmitAsync`)

### Boolean Naming
- **Properties/Methods:** Use `Is`, `Has`, `Can`, `Should` prefixes (`IsValid`, `HasPermission`, `CanExecute`, `ShouldRetry`)
- **Avoid negative names:** `IsEnabled` instead of `IsNotDisabled`

## File and Structure Organization

### File Organization
- **One Public Type Per File:** File name must match the type name (`CustomerService.cs` contains `CustomerService`)
- **Namespace Alignment:** Folder structure mirrors namespace hierarchy
  - `src/Services/Customers/` → `VisionaryCoder.Framework.Services.Customers`
- **File-Scoped Namespaces:** Use C# 10+ file-scoped namespaces for cleaner code
- **Nested Types:** Only when tightly coupled and used exclusively by parent

### Project Structure
```
src/
├── Controllers/          # Web API controllers
├── Services/             # Business logic services
├── Repositories/         # Data access layer
├── Models/              # DTOs and view models
├── Entities/            # Domain entities
├── Extensions/          # Extension methods
├── Validators/          # FluentValidation validators
└── Constants/           # Application constants
```

## API Design & Async Patterns

### Async Requirements
- **Suffix:** All async methods must end with `Async`
- **Return Type:** Use `Task` or `Task<T>`; use `ValueTask<T>` only when profiling shows benefit
- **Cancellation:** Accept `CancellationToken` for all public async APIs (default parameter: `default`)
- **No Blocking:** Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`
- **Propagation:** Pass cancellation tokens through entire call chain

### Method Signatures
```csharp
// ✅ Correct async signature
public async Task<OrderResult> ProcessOrderAsync(
    OrderRequest request, 
    CancellationToken cancellationToken = default)
{
    // Implementation
}

// ❌ Incorrect - missing Async suffix
public async Task<OrderResult> ProcessOrder(OrderRequest request) { }

// ❌ Incorrect - missing CancellationToken
public async Task<OrderResult> ProcessOrderAsync(OrderRequest request) { }

// ❌ Incorrect - blocking async
public OrderResult ProcessOrder(OrderRequest request) 
{
    return ProcessOrderAsync(request).Result; // NEVER DO THIS
}
```

## Error Handling & Validation

### Guard Clauses
- Validate all public method inputs early
- Throw appropriate exceptions with clear messages
- Use helper methods for complex validations

```csharp
public async Task<WidgetResult> ProcessAsync(WidgetRequest request, CancellationToken cancellationToken)
{
    ArgumentNullException.ThrowIfNull(request);
    ArgumentException.ThrowIfNullOrWhiteSpace(request.Name, nameof(request.Name));
    
    if (request.Quantity <= 0)
        throw new ArgumentException("Quantity must be positive", nameof(request.Quantity));

    // Implementation
}
```

### Exception Handling
- **Avoid Broad Catches:** Never catch `Exception` unless translating to domain error
- **Preserve Stack Trace:** Use `throw;` to rethrow, not `throw ex;`
- **Custom Exceptions:** Create domain-specific exceptions for business rules
- **Log Exceptions:** Always log with structured context

```csharp
// ✅ Correct exception handling
try
{
    await ProcessOrderAsync(order, cancellationToken);
}
catch (OrderValidationException ex)
{
    logger.LogWarning(ex, "Order validation failed for {OrderId}", order.Id);
    return new OrderResult(false, ex.Message);
}
catch (Exception ex)
{
    logger.LogError(ex, "Unexpected error processing order {OrderId}", order.Id);
    throw; // Rethrow to let caller handle
}

// ❌ Incorrect - swallows all errors
try
{
    await ProcessOrderAsync(order, cancellationToken);
}
catch { } // NEVER DO THIS
```
## Records & Immutability

### When to Use Records
- **Data Transfer Objects (DTOs):** Immutable data carriers
- **Value Objects:** Domain primitives with value equality
- **API Requests/Responses:** Immutable contract definitions

```csharp
// ✅ Correct record usage
public sealed record CustomerRequest(string Name, string Email, int Age);

public sealed record CustomerResponse(int Id, string Name, string Email)
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

// ✅ Record struct for small value types
public readonly record struct Money(decimal Amount, string Currency);

// ✅ Class for services with behavior
public sealed class CustomerService
{
    private readonly ICustomerRepository repository;
    private readonly ILogger<CustomerService> logger;

    public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }
}
```

### Immutability Patterns
- Use `readonly` for fields that never change
- Use `init` for properties that set once during construction
- Avoid mutable static state
- Use `ImmutableArray<T>`, `ImmutableList<T>` for collections

## Dependency Injection

### Constructor Injection
- **Primary Pattern:** Inject all dependencies via constructor
- **Field Assignment:** Use field initialization or assignment in constructor body
- **Validation:** Throw `ArgumentNullException` for null dependencies

```csharp
// ✅ Correct dependency injection
public sealed class OrderProcessor
{
    private readonly IOrderRepository repository;
    private readonly ILogger<OrderProcessor> logger;
    private readonly IValidator<Order> validator;

    public OrderProcessor(
        IOrderRepository repository, 
        ILogger<OrderProcessor> logger,
        IValidator<Order> validator)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
}

// ✅ Alternative with C# 11+ null validation
public sealed class OrderProcessor(
    IOrderRepository repository, 
    ILogger<OrderProcessor> logger,
    IValidator<Order> validator)
{
    private readonly IOrderRepository repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ILogger<OrderProcessor> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IValidator<Order> validator = validator ?? throw new ArgumentNullException(nameof(validator));
}
```

### Options Pattern
- Use strongly-typed options classes
- Validate on startup with `IValidateOptions<T>`
- Never read environment variables directly in business logic

```csharp
public sealed class EmailOptions
{
    public const string SectionName = "Email";
    
    public string SmtpHost { get; init; } = string.Empty;
    public int SmtpPort { get; init; }
    public string FromAddress { get; init; } = string.Empty;
}

// Registration
services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();
```

## Documentation

### XML Documentation
- Provide `<summary>`, `<param>`, `<returns>` for **non-trivial public APIs**
- Omit redundant comments that restate obvious code
- Document exceptions with `<exception>` tags
- Use `<remarks>` for additional context

```csharp
/// <summary>
/// Processes a customer order asynchronously with validation and persistence.
/// </summary>
/// <param name="request">The order request containing customer and item details.</param>
/// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
/// <returns>A task representing the asynchronous operation with the order result.</returns>
/// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
/// <exception cref="ValidationException">Thrown when request validation fails.</exception>
public async Task<OrderResult> ProcessOrderAsync(
    OrderRequest request, 
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### Code Comments
- Explain **why**, not **what** (code should be self-documenting)
- Add context for non-obvious decisions
- Reference tickets/ADRs for architectural choices

## Result Types & Nullability

### Nullable Reference Types
- Always enable: `<Nullable>enable</Nullable>`
- Never return `null` collections—return empty sequences
- Use `?` for nullable reference types explicitly
- Use null-forgiving operator (`!`) sparingly, only when compiler cannot infer

### Result Wrappers
- Use result types for operations needing diagnostics beyond simple return values
- Include success indicator, data, and error messages

```csharp
public sealed record OperationResult<T>(bool Success, T? Data, string? ErrorMessage)
{
    public static OperationResult<T> SuccessResult(T data) => new(true, data, null);
    public static OperationResult<T> FailureResult(string errorMessage) => new(false, default, errorMessage);
}

// Usage
public async Task<OperationResult<Customer>> GetCustomerAsync(int id, CancellationToken cancellationToken)
{
    var customer = await repository.FindByIdAsync(id, cancellationToken);
    
    return customer is not null 
        ? OperationResult<Customer>.SuccessResult(customer)
        : OperationResult<Customer>.FailureResult("Customer not found");
}
```

## Complete Example

```csharp
namespace VisionaryCoder.Framework.Services.Widgets;

/// <summary>
/// Processes widget operations with validation and persistence.
/// </summary>
public interface IWidgetProcessor
{
    /// <summary>
    /// Processes a widget request asynchronously.
    /// </summary>
    Task<WidgetResult> ProcessAsync(WidgetRequest request, CancellationToken cancellationToken = default);
}

public sealed record WidgetRequest(string Name, int Quantity);

public sealed record WidgetResult(bool Success, string? Message);

public sealed class WidgetProcessor : IWidgetProcessor
{
    private readonly IWidgetRepository repository;
    private readonly ILogger<WidgetProcessor> logger;
    private const int MaxQuantity = 1000;

    public WidgetProcessor(IWidgetRepository repository, ILogger<WidgetProcessor> logger)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WidgetResult> ProcessAsync(WidgetRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name, nameof(request.Name));
        
        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(request.Quantity));
            
        if (request.Quantity > MaxQuantity)
            throw new ArgumentException($"Quantity cannot exceed {MaxQuantity}", nameof(request.Quantity));

        var exists = await repository.ExistsAsync(request.Name, cancellationToken);
        
        if (!exists)
        {
            await repository.CreateAsync(request.Name, request.Quantity, cancellationToken);
            logger.LogInformation("Created widget {WidgetName} with quantity {Quantity}", request.Name, request.Quantity);
        }

        return new WidgetResult(true, null);
    }
}
```

## Output Checklist

Before completing code generation, verify:

1. ✅ File name matches type name
2. ✅ Namespace mirrors folder structure
3. ✅ Async methods have `Async` suffix and `CancellationToken` parameter
4. ✅ No blocking async calls (`.Result`, `.Wait()`)
5. ✅ Guard clauses for public method inputs
6. ✅ No magic numbers/strings—use named constants
7. ✅ Public APIs documented if non-trivial
8. ✅ Nullable warnings resolved
9. ✅ Dependencies injected via constructor
10. ✅ No underscore prefixes on field names

## Anti-Patterns (Always Reject)

❌ Static service locator usage  
❌ Deep inheritance for behavior variation (use Strategy/Decorator)  
❌ Broad catch blocks suppressing failures  
❌ Unbounded recursion or retries  
❌ Mixing sync & async flows arbitrarily  
❌ Partially initialized objects returned from constructors  
❌ Underscore prefixes on private fields  
❌ Public mutable state  

## References

- **Enterprise Baseline:** `.github/copilot-instructions.md` - Architecture, security, VBD patterns
- **Testing:** `.copilot/unit-test.instructions.md` - Test structure and coverage
- **Quality:** `.copilot/code-quality.instructions.md` - Analyzers and diagnostics
- **Patterns:** `.copilot/design-patterns.instructions.md` - Design pattern examples
- **Repository:** `.copilot/repo-standards.md` - Git hygiene and collaboration

## External Standards

- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [Async Best Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)

---

**Version History:**
- **2.0.0 (2025-11-20):** Enhanced naming conventions, added industry best practices, improved examples
- **1.1.0 (2025-11-20):** Added async patterns and error handling guidance
- **1.0.0 (2025-11-19):** Initial version with basic code generation rules
