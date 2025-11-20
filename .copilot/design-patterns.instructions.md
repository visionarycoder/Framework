---
applyTo: "**/*.cs"
version: 2.0.0
lastUpdated: 2025-11-20
scope: design-patterns
---

# Copilot Instructions: Design Patterns (C#)

## Purpose

Generate modern, educational design pattern examples using C# 12 / .NET 8+ features. Each pattern includes intent, structure, minimal code, usage examples, and practical notes including pitfalls and alternatives.

## Pattern Generation Standards

1. **Modern C# Syntax:** Use C# 12+ features (records, primary constructors, file-scoped namespaces, pattern matching)
2. **Minimal & Compilable:** Show complete but concise examples without extraneous code
3. **Educational Format:** Clear separation of intent, structure, code, usage, and notes
4. **Testable Design:** Prefer interfaces and dependency injection for testability
5. **Async-First:** Use async/await where asynchronous behavior is natural
6. **Avoid Legacy:** No outdated constructs (`ArrayList`, synchronous blocking of async)

## Required Output Sections

### 1. Intent
One-sentence description of the pattern's purpose

### 2. Structure
Key interfaces, classes, and their relationships

### 3. Code
Minimal, compilable C# example demonstrating the pattern

### 4. Usage
Short code snippet showing how to use the pattern

### 5. Notes
- When to use the pattern
- Common pitfalls to avoid
- Modern alternatives or implementing libraries
- Performance considerations

## Gang of Four Patterns

### Creational Patterns

Patterns for object creation mechanisms, increasing flexibility and reuse.

#### Factory Method
**Intent:** Define an interface for creating objects, letting subclasses decide which class to instantiate.

#### Abstract Factory
**Intent:** Provide an interface for creating families of related objects without specifying concrete classes.

#### Builder
**Intent:** Separate construction of complex objects from their representation.

#### Prototype
**Intent:** Create new objects by copying existing instances.

#### Singleton
**Intent:** Ensure a class has only one instance with global access point.
**Note:** Prefer dependency injection over singleton pattern for better testability.

### Structural Patterns

Patterns for composing classes and objects into larger structures.

#### Adapter
**Intent:** Convert one interface to another that clients expect.

#### Bridge
**Intent:** Decouple abstraction from implementation so both can vary independently.

#### Composite
**Intent:** Compose objects into tree structures to represent part-whole hierarchies.

#### Decorator
**Intent:** Attach additional responsibilities to objects dynamically.

#### Facade
**Intent:** Provide a simplified interface to a complex subsystem.

#### Flyweight
**Intent:** Share common state among many fine-grained objects efficiently.

#### Proxy
**Intent:** Provide a surrogate or placeholder to control access to an object.

### Behavioral Patterns

Patterns for algorithms and assignment of responsibilities between objects.

#### Chain of Responsibility
**Intent:** Pass requests along a chain of handlers until one processes it.

#### Command
**Intent:** Encapsulate requests as objects for parameterization and queuing.

#### Interpreter
**Intent:** Define a grammar and interpreter for a language.

#### Iterator
**Intent:** Access elements of a collection sequentially without exposing representation.

#### Mediator
**Intent:** Define an object that encapsulates how a set of objects interact.

#### Memento
**Intent:** Capture and restore an object's internal state without violating encapsulation.

#### Observer
**Intent:** Define a one-to-many dependency so dependents are notified of state changes.

#### State
**Intent:** Allow an object to alter behavior when its internal state changes.

#### Strategy
**Intent:** Define a family of algorithms and make them interchangeable.

#### Template Method
**Intent:** Define skeleton of an algorithm, letting subclasses override specific steps.

#### Visitor
**Intent:** Separate algorithms from objects they operate on.

## Enterprise & Modern Patterns

### Repository Pattern
**Intent:** Mediate between domain and data mapping layers using a collection-like interface.

### Unit of Work Pattern
**Intent:** Maintain a list of objects affected by a transaction and coordinate changes.

### CQRS (Command Query Responsibility Segregation)
**Intent:** Separate read and write operations for complex domains.

### Saga Pattern
**Intent:** Manage long-running transactions with compensating actions.

## Complete Pattern Examples

### Strategy Pattern

**Intent:** Select algorithm at runtime without modifying client code.

**Structure:**
```
IStrategy (interface)
  ├── ConcreteStrategyA
  └── ConcreteStrategyB
Context (uses IStrategy)
```

**Code:**
```csharp
namespace VisionaryCoder.Framework.Patterns.Strategy;

/// <summary>
/// Defines the contract for sorting algorithms.
/// </summary>
public interface ISortStrategy
{
    Task<IEnumerable<int>> SortAsync(IEnumerable<int> data, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implements ascending sort strategy.
/// </summary>
public sealed class AscendingSortStrategy : ISortStrategy
{
    public Task<IEnumerable<int>> SortAsync(IEnumerable<int> data, CancellationToken cancellationToken = default)
    {
        var sorted = data.OrderBy(x => x);
        return Task.FromResult(sorted);
    }
}

/// <summary>
/// Implements descending sort strategy.
/// </summary>
public sealed class DescendingSortStrategy : ISortStrategy
{
    public Task<IEnumerable<int>> SortAsync(IEnumerable<int> data, CancellationToken cancellationToken = default)
    {
        var sorted = data.OrderByDescending(x => x);
        return Task.FromResult(sorted);
    }
}

/// <summary>
/// Context that uses a sort strategy.
/// </summary>
public sealed class DataSorter
{
    private readonly ISortStrategy strategy;

    public DataSorter(ISortStrategy strategy)
    {
        this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public Task<IEnumerable<int>> SortAsync(IEnumerable<int> data, CancellationToken cancellationToken = default)
    {
        return strategy.SortAsync(data, cancellationToken);
    }
}
```

**Usage:**
```csharp
// Inject strategy via dependency injection
var sorter = new DataSorter(new AscendingSortStrategy());
var data = new[] { 5, 2, 8, 1, 9 };
var sorted = await sorter.SortAsync(data);
// Result: [1, 2, 5, 8, 9]

// Switch strategy at runtime
var descendingSorter = new DataSorter(new DescendingSortStrategy());
var descendingSorted = await descendingSorter.SortAsync(data);
// Result: [9, 8, 5, 2, 1]
```

**Notes:**
- **When to Use:** Multiple algorithms for same operation, need to switch at runtime
- **Alternatives:** LINQ often eliminates need for simple strategies; use for complex, pluggable algorithms
- **DI Integration:** Register strategies in `IServiceCollection` for constructor injection
- **Performance:** No significant overhead; interface dispatch is negligible

---

### Repository Pattern

**Intent:** Abstract data access logic and provide collection-like interface for domain entities.

**Structure:**
```
IRepository<T> (interface)
  └── EntityRepository (implementation)
Domain Entity
  └── Customer, Order, etc.
```

**Code:**
```csharp
namespace VisionaryCoder.Framework.Patterns.Repository;

/// <summary>
/// Generic repository interface for entity operations.
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Customer entity.
/// </summary>
public sealed record Customer(int Id, string Name, string Email);

/// <summary>
/// Customer repository implementation using Entity Framework Core.
/// </summary>
public sealed class CustomerRepository : IRepository<Customer>
{
    private readonly DbContext context;
    private readonly DbSet<Customer> dbSet;

    public CustomerRepository(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.dbSet = context.Set<Customer>();
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        var entry = await dbSet.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        dbSet.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
```

**Usage:**
```csharp
// Register in DI container
services.AddScoped<IRepository<Customer>, CustomerRepository>();

// Use in service
public sealed class CustomerService
{
    private readonly IRepository<Customer> repository;

    public CustomerService(IRepository<Customer> repository)
    {
        this.repository = repository;
    }

    public async Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken);
    }
}
```

**Notes:**
- **When to Use:** Abstract data access, enable unit testing with mocks, support multiple data sources
- **Alternatives:** Direct `DbContext` usage for simple CRUD; CQRS for complex read/write models
- **Unit of Work:** Combine with Unit of Work pattern for transaction management
- **Performance:** May hide query optimization opportunities; consider query objects for complex queries
- **Testing:** Easy to mock for unit tests using Moq or similar frameworks

---

### Decorator Pattern

**Intent:** Add responsibilities to objects dynamically without modifying their structure.

**Structure:**
```
IComponent (interface)
  ├── ConcreteComponent
  └── Decorator (base, implements IComponent)
      ├── ConcreteDecoratorA
      └── ConcreteDecoratorB
```

**Code:**
```csharp
namespace VisionaryCoder.Framework.Patterns.Decorator;

/// <summary>
/// Defines the component interface.
/// </summary>
public interface INotificationService
{
    Task SendAsync(string message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base notification service implementation.
/// </summary>
public sealed class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> logger;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        this.logger = logger;
    }

    public Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Sending email: {Message}", message);
        // Email sending logic here
        return Task.CompletedTask;
    }
}

/// <summary>
/// Decorator that adds logging functionality.
/// </summary>
public sealed class LoggingNotificationDecorator : INotificationService
{
    private readonly INotificationService inner;
    private readonly ILogger<LoggingNotificationDecorator> logger;

    public LoggingNotificationDecorator(
        INotificationService inner,
        ILogger<LoggingNotificationDecorator> logger)
    {
        this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting notification send: {Message}", message);
        
        try
        {
            await inner.SendAsync(message, cancellationToken);
            logger.LogInformation("Notification sent successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send notification");
            throw;
        }
    }
}

/// <summary>
/// Decorator that adds retry functionality.
/// </summary>
public sealed class RetryNotificationDecorator : INotificationService
{
    private readonly INotificationService inner;
    private readonly int maxRetries;

    public RetryNotificationDecorator(INotificationService inner, int maxRetries = 3)
    {
        this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        this.maxRetries = maxRetries;
    }

    public async Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await inner.SendAsync(message, cancellationToken);
                return;
            }
            catch when (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
        }
    }
}
```

**Usage:**
```csharp
// Compose decorators
INotificationService service = new EmailNotificationService(logger);
service = new LoggingNotificationDecorator(service, logger);
service = new RetryNotificationDecorator(service, maxRetries: 3);

await service.SendAsync("Hello, World!");
```

**Notes:**
- **When to Use:** Add cross-cutting concerns (logging, caching, retry) without modifying core logic
- **Alternatives:** Middleware pipeline (ASP.NET Core), aspect-oriented programming (PostSharp)
- **Composition:** Can stack multiple decorators for layered functionality
- **Performance:** Minimal overhead per decorator layer
- **Libraries:** Consider Polly for resilience patterns (retry, circuit breaker)

---

### Observer Pattern

**Intent:** Define one-to-many dependency so dependents are automatically notified of state changes.

**Structure:**
```
IObserver<T> (interface)
IObservable<T> (interface)
Subject (implements IObservable<T>)
ConcreteObserver (implements IObserver<T>)
```

**Code:**
```csharp
namespace VisionaryCoder.Framework.Patterns.Observer;

/// <summary>
/// Event data for order placement.
/// </summary>
public sealed record OrderPlacedEvent(int OrderId, decimal Amount, DateTime Timestamp);

/// <summary>
/// Observable subject that publishes order events.
/// </summary>
public sealed class OrderService
{
    private readonly List<IObserver<OrderPlacedEvent>> observers = [];

    public IDisposable Subscribe(IObserver<OrderPlacedEvent> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber(observers, observer);
    }

    public async Task PlaceOrderAsync(int orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        // Place order logic here
        await Task.Delay(100, cancellationToken); // Simulate work

        // Notify observers
        var orderEvent = new OrderPlacedEvent(orderId, amount, DateTime.UtcNow);
        foreach (var observer in observers)
        {
            observer.OnNext(orderEvent);
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<OrderPlacedEvent>> observers;
        private readonly IObserver<OrderPlacedEvent> observer;

        public Unsubscriber(List<IObserver<OrderPlacedEvent>> observers, IObserver<OrderPlacedEvent> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

        public void Dispose()
        {
            observers.Remove(observer);
        }
    }
}

/// <summary>
/// Observer that sends email notifications.
/// </summary>
public sealed class EmailNotificationObserver : IObserver<OrderPlacedEvent>
{
    private readonly ILogger<EmailNotificationObserver> logger;

    public EmailNotificationObserver(ILogger<EmailNotificationObserver> logger)
    {
        this.logger = logger;
    }

    public void OnNext(OrderPlacedEvent value)
    {
        logger.LogInformation("Sending email for order {OrderId} amount {Amount}", value.OrderId, value.Amount);
    }

    public void OnError(Exception error)
    {
        logger.LogError(error, "Error in order event stream");
    }

    public void OnCompleted()
    {
        logger.LogInformation("Order event stream completed");
    }
}
```

**Usage:**
```csharp
var orderService = new OrderService();
var emailObserver = new EmailNotificationObserver(logger);

// Subscribe to events
using var subscription = orderService.Subscribe(emailObserver);

// Place order (observers notified automatically)
await orderService.PlaceOrderAsync(orderId: 123, amount: 99.99m);
```

**Notes:**
- **When to Use:** Decoupled event notification, multiple subscribers to state changes
- **Alternatives:** C# events, `IObservable<T>` with Reactive Extensions (Rx.NET)
- **Performance:** Consider async observers for I/O-bound notifications
- **Libraries:** Use System.Reactive (Rx.NET) for complex event composition
- **Thread Safety:** Ensure observer list is thread-safe if used concurrently

---

## Pattern Selection Guide

### When to Use Each Pattern

| Pattern | Use When | Avoid When |
|---------|----------|------------|
| **Strategy** | Multiple algorithms for same task | Only one algorithm, simple LINQ suffices |
| **Repository** | Abstract data access, unit testing | Simple CRUD, direct EF Core sufficient |
| **Decorator** | Add cross-cutting concerns dynamically | Compile-time concerns, use middleware instead |
| **Observer** | Decoupled event notification | Simple callbacks, tight coupling acceptable |
| **Factory Method** | Defer instantiation to subclasses | Single concrete type, DI container handles creation |
| **Singleton** | - | Always prefer DI with appropriate lifetime scope |
| **Command** | Queue/log operations, undo functionality | Simple method calls sufficient |
| **Mediator** | Complex object interactions | Few objects, direct communication clearer |

## Testing Pattern Implementations

```csharp
[TestClass]
public sealed class StrategyPatternTests
{
    [TestMethod]
    public async Task AscendingSortStrategy_WhenGivenUnsortedData_ShouldReturnAscendingOrder()
    {
        // Arrange
        var strategy = new AscendingSortStrategy();
        var data = new[] { 5, 2, 8, 1, 9 };
        
        // Act
        var result = await strategy.SortAsync(data);
        
        // Assert
        result.Should().BeInAscendingOrder();
        result.Should().Equal(1, 2, 5, 8, 9);
    }
}
```

## Anti-Patterns (Always Reject)

❌ **Singleton with static mutable state** (use DI instead)  
❌ **God objects** implementing multiple patterns  
❌ **Deep inheritance hierarchies** (prefer composition)  
❌ **Patterns for the sake of patterns** (YAGNI principle)  
❌ **Blocking async code in patterns** (`.Result`, `.Wait()`)  
❌ **Ignoring modern alternatives** (use libraries when appropriate)  

## References

- **Code Generation:** `.copilot/code-generation.instructions.md` - Code structure and naming
- **Unit Testing:** `.copilot/unit-test.instructions.md` - Testing pattern implementations
- **Code Quality:** `.copilot/code-quality.instructions.md` - Ensuring pattern quality
- **Repository Standards:** `.copilot/repo-standards.md` - Documenting pattern usage

## External Resources

- [Gang of Four Design Patterns](https://refactoring.guru/design-patterns)
- [Microsoft Enterprise Patterns](https://learn.microsoft.com/en-us/azure/architecture/patterns/)
- [Martin Fowler's Catalog](https://martinfowler.com/eaaCatalog/)
- [C# Design Patterns (Refactoring Guru)](https://refactoring.guru/design-patterns/csharp)

---

**Version History:**
- **2.0.0 (2025-11-20):** Comprehensive pattern examples with modern C# features, testing guidance
- **1.0.0 (2025-11-20):** Initial version with basic pattern structure
