---
applyTo: "**/tests/**"
version: 2.0.0
lastUpdated: 2025-11-20
scope: unit-testing
applyToPatterns:
  - "**/*.test.cs"
  - "**/*.spec.cs"
  - "**/tests/**/*.cs"
---

# Copilot Instructions: Unit Testing

## Purpose

Provide focused heuristics for generating high-quality, deterministic unit tests with industry-standard practices. This file covers test structure, naming, isolation, mocking, assertions, and coverage strategies.

## Core Testing Principles

1. **One Behavioral Expectation Per Test:** Each test validates a single behavior or outcome
2. **Deterministic Tests:** Eliminate time, randomness, network, and filesystem dependencies
3. **Isolation:** No shared mutable state; tests can run in any order
4. **Fast Execution:** Unit tests should complete in milliseconds
5. **Clear Failure Messages:** Use expressive assertions that explain what went wrong
6. **Arrange/Act/Assert:** Clear separation of test setup, execution, and verification

## Test Naming Convention

**Pattern:** `MethodName_WhenCondition_ShouldOutcome`

### Examples
- `ProcessAsync_WhenRequestIsValid_ShouldPersistEntity`
- `ValidateOrder_WhenQuantityIsNegative_ShouldReturnValidationError`
- `GetCustomer_WhenCustomerNotFound_ShouldReturnNull`
- `CalculateDiscount_WhenCustomerIsPremium_ShouldApply20PercentDiscount`

### Naming Guidelines
- **MethodName:** The method under test
- **WhenCondition:** The scenario or input state being tested
- **ShouldOutcome:** The expected behavior or result

## Test Structure Template

```csharp
[TestClass]
public sealed class WidgetProcessorTests
{
    [TestMethod]
    public async Task ProcessAsync_WhenNewWidget_ShouldCreateAndReturnSuccess()
    {
        // Arrange
        var repository = new Mock<IWidgetRepository>(MockBehavior.Strict);
        repository
            .Setup(r => r.ExistsAsync("Widget-A", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository
            .Setup(r => r.CreateAsync("Widget-A", 10, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var logger = new Mock<ILogger<WidgetProcessor>>();
        var sut = new WidgetProcessor(repository.Object, logger.Object);
        var request = new WidgetRequest("Widget-A", 10);

        // Act
        var result = await sut.ProcessAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().BeNull();
        repository.VerifyAll();
    }
}
```

## Test Frameworks & Libraries

### Testing Framework
- **Primary:** MSTest (`[TestClass]`, `[TestMethod]`)
- **Alternative:** xUnit (`[Fact]`, `[Theory]`)
- **Alternative:** NUnit (`[TestFixture]`, `[Test]`)

### Assertion Library
- **Required:** FluentAssertions for expressive, readable assertions
- Provides detailed failure messages and fluent API

```csharp
// ✅ FluentAssertions - descriptive and readable
result.Should().NotBeNull();
result.Success.Should().BeTrue();
result.Items.Should().HaveCount(5);
result.Name.Should().Be("Expected Name");

// ❌ Basic assertions - less descriptive
Assert.IsNotNull(result);
Assert.IsTrue(result.Success);
Assert.AreEqual(5, result.Items.Count);
```

### Mocking Framework
- **Required:** Moq for creating test doubles and mocks
- Use `MockBehavior.Strict` for explicit verification of all interactions

## Moq Best Practices

### Mock Creation & Setup

```csharp
// ✅ Strict behavior - fails if unexpected methods called
var repository = new Mock<IWidgetRepository>(MockBehavior.Strict);

// Setup method calls with specific parameters
repository
    .Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
    .ReturnsAsync(new Widget { Id = 42, Name = "Test" });

// Setup method calls with parameter matching
repository
    .Setup(r => r.CreateAsync(It.Is<string>(s => s.Length > 0), It.IsAny<int>(), It.IsAny<CancellationToken>()))
    .Returns(Task.CompletedTask);

// Setup property access
repository.SetupGet(r => r.ConnectionString).Returns("test-connection");

// Setup with callback to capture parameters
string capturedName = null;
repository
    .Setup(r => r.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
    .Callback<string, int, CancellationToken>((name, qty, ct) => capturedName = name)
    .Returns(Task.CompletedTask);
```

### Verification Patterns

```csharp
// Verify specific method called once
repository.Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()), Times.Once);

// Verify method never called
repository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

// Verify all configured setups were called
repository.VerifyAll();

// Verify no other methods called beyond setups
repository.VerifyNoOtherCalls();
```

### Return Value Patterns

```csharp
// Simple return value
mock.Setup(m => m.GetCount()).Returns(42);

// Async return value
mock.Setup(m => m.GetDataAsync()).ReturnsAsync(new Data());

// Sequential returns
mock.SetupSequence(m => m.GetNext())
    .Returns(1)
    .Returns(2)
    .Returns(3);

// Conditional returns based on input
mock.Setup(m => m.Process(It.IsAny<int>()))
    .Returns<int>(x => x * 2);

// Throw exception
mock.Setup(m => m.GetData()).Throws<InvalidOperationException>();
mock.Setup(m => m.GetDataAsync()).ThrowsAsync(new InvalidOperationException());
```

### Mock Behavior Comparison

```csharp
// Strict - Fails on any unexpected method call (recommended for unit tests)
var strictMock = new Mock<IService>(MockBehavior.Strict);

// Loose - Returns default values for unexpected calls (use sparingly)
var looseMock = new Mock<IService>(MockBehavior.Loose);
```

## Test Organization

### Test Class Structure
```csharp
[TestClass]
public sealed class CustomerServiceTests
{
    // ✅ Test class is sealed and clearly named with "Tests" suffix
    
    // Shared test data (read-only, immutable)
    private static readonly Customer ValidCustomer = new("John Doe", "john@example.com", 30);
    
    // Helper methods for common arrangements
    private static Mock<ICustomerRepository> CreateRepositoryMock()
    {
        return new Mock<ICustomerRepository>(MockBehavior.Strict);
    }
    
    [TestMethod]
    public async Task GetCustomerAsync_WhenCustomerExists_ShouldReturnCustomer()
    {
        // Test implementation
    }
    
    [TestMethod]
    public async Task GetCustomerAsync_WhenCustomerNotFound_ShouldReturnNull()
    {
        // Test implementation
    }
}
```

### Test Data Builders
```csharp
// ✅ Use builder pattern for complex test data
public sealed class CustomerBuilder
{
    private string name = "Default Name";
    private string email = "default@example.com";
    private int age = 25;
    
    public CustomerBuilder WithName(string value)
    {
        name = value;
        return this;
    }
    
    public CustomerBuilder WithEmail(string value)
    {
        email = value;
        return this;
    }
    
    public CustomerBuilder WithAge(int value)
    {
        age = value;
        return this;
    }
    
    public Customer Build() => new(name, email, age);
}

// Usage in tests
var customer = new CustomerBuilder()
    .WithName("Jane Doe")
    .WithAge(35)
    .Build();
```

## Deterministic Testing

### Time Abstraction
```csharp
// ✅ Abstract time for testability
public interface ITimeProvider
{
    DateTime UtcNow { get; }
}

// Production implementation
public sealed class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

// Test implementation
public sealed class FakeTimeProvider : ITimeProvider
{
    public DateTime UtcNow { get; set; } = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}

// Usage in test
var timeProvider = new FakeTimeProvider { UtcNow = new DateTime(2025, 11, 20) };
var sut = new OrderService(repository, timeProvider);
```

### Random Value Abstraction
```csharp
// ✅ Abstract randomness for testability
public interface IRandomProvider
{
    int Next(int minValue, int maxValue);
}

// Test implementation
public sealed class FakeRandomProvider : IRandomProvider
{
    private readonly Queue<int> values = new();
    
    public void QueueValue(int value) => values.Enqueue(value);
    
    public int Next(int minValue, int maxValue) => 
        values.Count > 0 ? values.Dequeue() : minValue;
}
```

### No External Dependencies
```csharp
// ❌ Avoid in unit tests
await File.ReadAllTextAsync("file.txt"); // Filesystem
await httpClient.GetAsync("http://api.example.com"); // Network
await Task.Delay(1000); // Time delay
Thread.Sleep(100); // Blocking delay
var random = new Random().Next(); // Non-deterministic

// ✅ Use abstractions and mocks instead
var fileContent = await fileSystem.ReadAllTextAsync("file.txt");
var response = await httpClient.GetAsync(url);
await timeProvider.DelayAsync(duration);
var value = randomProvider.Next(0, 100);
```

## Async Testing Best Practices

### Async Method Signatures
```csharp
// ✅ Correct async test signature
[TestMethod]
public async Task ProcessAsync_WhenValidInput_ShouldComplete()
{
    // Arrange
    var sut = CreateSystemUnderTest();
    
    // Act
    var result = await sut.ProcessAsync(input, CancellationToken.None);
    
    // Assert
    result.Should().NotBeNull();
}

// ❌ Incorrect - synchronous test of async method
[TestMethod]
public void ProcessAsync_WhenValidInput_ShouldComplete()
{
    var result = sut.ProcessAsync(input, CancellationToken.None).Result; // NEVER DO THIS
}
```

### Testing Cancellation
```csharp
[TestMethod]
public async Task ProcessAsync_WhenCancellationRequested_ShouldThrowOperationCanceledException()
{
    // Arrange
    var cts = new CancellationTokenSource();
    var sut = CreateSystemUnderTest();
    cts.Cancel();
    
    // Act
    Func<Task> act = async () => await sut.ProcessAsync(input, cts.Token);
    
    // Assert
    await act.Should().ThrowAsync<OperationCanceledException>();
}

