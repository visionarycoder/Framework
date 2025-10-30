# SOLID Principles Authentication Usage Examples

## Overview
The VisionaryCoder.Framework.Authentication namespace now follows SOLID principles, specifically the **Dependency Inversion Principle** and **Null Object Pattern**. This ensures explicit intent in provider registration and safe fallback behavior.

## Core SOLID Principle Applied

### Dependency Inversion Principle (DIP)
- **High-level modules should not depend on low-level modules; both should depend on abstractions**
- **Abstractions should not depend on details; details should depend on abstractions**

### Implementation Strategy
1. **Null Object Pattern**: Safe fallbacks without implicit defaults
2. **Explicit Registration**: No automatic provider assumptions
3. **Interface-Based Design**: All dependencies through contracts

## Basic JWT Authentication Setup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Step 1: Register JWT authentication with null object fallbacks
    services.AddJwtAuthentication(options =>
    {
        options.Authority = "https://your-identity-provider";
        options.Audience = "your-api-audience";
        options.ClientId = "your-client-id";
    });
    
    // At this point, all providers are NULL OBJECTS that provide safe fallback behavior
    // No implicit defaults are registered - this follows SOLID DIP principles
}
```

## Explicit Provider Registration (Recommended)

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Step 1: Register JWT authentication infrastructure
    services.AddJwtAuthentication(options =>
    {
        options.Authority = "https://your-identity-provider";
        options.Audience = "your-api-audience";
        options.ClientId = "your-client-id";
    });
    
    // Step 2: EXPLICITLY register your providers (SOLID principle)
    services.ReplaceUserContextProvider<DefaultUserContextProvider>();
    services.ReplaceTenantContextProvider<DefaultTenantContextProvider>();
    services.ReplaceTokenProvider<DefaultTokenProvider>();
    
    // OR use convenience method for defaults:
    // services.UseDefaultAuthenticationProviders();
}
```

## Custom Provider Implementation

```csharp
// Step 1: Implement your custom provider
public class CustomUserContextProvider : IUserContextProvider
{
    public Task<UserContext?> GetCurrentUserAsync()
    {
        // Your custom implementation
        return Task.FromResult<UserContext?>(new UserContext
        {
            UserId = "custom-user-id",
            UserName = "custom-user"
        });
    }
}

// Step 2: Register your custom provider
public void ConfigureServices(IServiceCollection services)
{
    services.AddJwtAuthentication(options => { /* ... */ });
    
    // Replace null object with your custom implementation
    services.ReplaceUserContextProvider<CustomUserContextProvider>();
}
```

## Null Object Pattern Benefits

### Safe Fallback Behavior
```csharp
// If no explicit provider is registered, null objects provide safe behavior:

// NullUserContextProvider returns null (no exceptions)
var userContext = await userContextProvider.GetCurrentUserAsync(); // returns null

// NullTenantContextProvider returns null (no exceptions)  
var tenantContext = await tenantContextProvider.GetCurrentTenantAsync(); // returns null

// NullTokenProvider returns failed results or throws meaningful exceptions
var tokenResult = await tokenProvider.GetTokenAsync(request); // returns failed TokenResult
```

### SOLID Principle Compliance
1. **Single Responsibility**: Each provider has one clear purpose
2. **Open/Closed**: Easily extend with new providers without modifying existing code
3. **Liskov Substitution**: All implementations are substitutable through interfaces
4. **Interface Segregation**: Focused interfaces with specific responsibilities
5. **Dependency Inversion**: Depend on abstractions, not concrete implementations

## Anti-Patterns to Avoid

### ❌ Don't rely on implicit defaults
```csharp
// BAD: Assuming providers are automatically registered
services.AddJwtAuthentication(options => { /* ... */ });
// User expects DefaultUserContextProvider but gets NullUserContextProvider
```

### ❌ Don't register concrete dependencies directly
```csharp
// BAD: Bypassing the framework's registration methods
services.AddScoped<DefaultUserContextProvider>();
// This doesn't replace the null object and violates explicit intent
```

## ✅ Best Practices

### Explicit Intent Required
```csharp
// GOOD: Clear, explicit provider registration
services.AddJwtAuthentication(options => { /* ... */ });
services.UseDefaultAuthenticationProviders(); // Explicit intent to use defaults
```

### Custom Implementation with Validation
```csharp
public class ValidatedUserContextProvider : IUserContextProvider
{
    private readonly ILogger<ValidatedUserContextProvider> logger;
    
    public ValidatedUserContextProvider(ILogger<ValidatedUserContextProvider> logger)
    {
        this.logger = logger;
    }
    
    public async Task<UserContext?> GetCurrentUserAsync()
    {
        try
        {
            // Your validation logic
            var context = await GetUserFromToken();
            logger.LogInformation("User context retrieved: {UserId}", context?.UserId);
            return context;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve user context");
            return null; // Safe fallback
        }
    }
}
```

## Testing with SOLID Principles

```csharp
[TestMethod]
public async Task ShouldUseNullObjectWhenNoProviderRegistered()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddJwtAuthentication(options => { /* valid options */ });
    var provider = services.BuildServiceProvider();
    
    // Act
    var userContextProvider = provider.GetRequiredService<IUserContextProvider>();
    var userContext = await userContextProvider.GetCurrentUserAsync();
    
    // Assert
    Assert.IsNull(userContext); // Null object returns null safely
    Assert.IsInstanceOfType(userContextProvider, typeof(NullUserContextProvider));
}

[TestMethod]
public async Task ShouldUseExplicitProviderWhenRegistered()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddJwtAuthentication(options => { /* valid options */ });
    services.ReplaceUserContextProvider<DefaultUserContextProvider>();
    var provider = services.BuildServiceProvider();
    
    // Act
    var userContextProvider = provider.GetRequiredService<IUserContextProvider>();
    
    // Assert
    Assert.IsInstanceOfType(userContextProvider, typeof(DefaultUserContextProvider));
}
```

This approach ensures that:
1. **No implicit behavior** - everything must be explicitly registered
2. **Safe fallbacks** - null objects prevent runtime errors
3. **Clear intent** - developers must explicitly choose their providers
4. **Testable design** - easy to mock and verify behavior
5. **SOLID compliance** - follows dependency inversion and interface segregation principles