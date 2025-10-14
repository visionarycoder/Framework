# VisionaryCoder Framework Library Restructuring

## Overview

This document outlines the restructuring of the VisionaryCoder.Framework.Extensions.Configuration library into focused, single-responsibility libraries following Microsoft best practices and Volatility-Based Decomposition (VBD) principles.

## Restructured Libraries

### 1. Azure Services

#### VisionaryCoder.Framework.Azure.AppConfiguration

- **Purpose:** Dedicated Azure App Configuration integration
- **Features:**
  - Managed identity and connection string authentication
  - Environment-specific configuration with labels
  - Automatic refresh with sentinel keys
  - Service collection extensions for easy setup

#### VisionaryCoder.Framework.Azure.KeyVault

- **Purpose:** Azure Key Vault secret management with caching
- **Features:**
  - Managed identity authentication with DefaultAzureCredential
  - Memory caching with configurable TTL
  - Local development fallback support
  - Parallel secret retrieval for performance
  - Comprehensive error handling and logging

### 2. Secret Management Abstractions

#### VisionaryCoder.Framework.Secrets.Abstractions

- **Purpose:** Core secret provider abstractions
- **Features:**
  - `ISecretProvider` interface for dependency inversion
  - `NullSecretProvider` for testing scenarios
  - Async support with cancellation tokens
  - Batch secret retrieval capabilities

### 3. Proxy Interceptors (Individual Libraries)

#### VisionaryCoder.Framework.Proxy.Interceptors.Logging

- **Purpose:** Logging interceptor for operation monitoring
- **Features:**
  - Comprehensive operation lifecycle logging
  - Correlation ID tracking
  - Success/failure differentiation
  - Exception categorization

#### VisionaryCoder.Framework.Proxy.Interceptors.Caching

- **Purpose:** Response caching for performance optimization
- **Features:**
  - Configurable cache duration and key generation
  - Cache hit/miss tracking
  - Metadata-based cache control
  - Memory cache integration

#### VisionaryCoder.Framework.Proxy.Interceptors.Security

- **Purpose:** Security interceptors for authentication
- **Features:**
  - JWT Bearer token authentication
  - Integration with secret providers
  - Static token support for development
  - Automatic Authorization header injection

### 4. Data Configuration

#### VisionaryCoder.Framework.Data.Configuration

- **Purpose:** Database connection string management
- **Features:**
  - Type-safe `ConnectionString` value object
  - Integration with configuration and secret providers
  - Named connection string support
  - Service collection extensions

## Benefits of This Restructuring

### 1. Single Responsibility Principle

- Each library has a focused, well-defined purpose
- Reduced coupling between unrelated functionalities
- Easier maintenance and testing

### 2. Individual Loading & Deployment

- Interceptors can be loaded individually based on requirements
- Smaller deployment packages for specific scenarios
- Better performance through selective loading

### 3. Dependency Management

- Clear dependency hierarchies following VBD principles
- Abstractions separated from implementations
- Proper layering with contracts and implementations

### 4. Microsoft Best Practices Compliance

- Follows Microsoft naming conventions and placement guidelines
- Uses appropriate service collection extension patterns
- Implements proper configuration binding and options patterns

## Migration Guide

### From Old Configuration Library

**Before:**

```csharp
services.AddSecretProvider(configuration, options => 
{
    options.KeyVaultUri = new Uri("https://vault.vault.azure.net/");
});
```

**After:**

```csharp
services.AddAzureKeyVaultSecrets(configuration, options =>
{
    options.VaultUri = new Uri("https://vault.vault.azure.net/");
});
```

### Individual Interceptor Usage

**Logging Interceptor:**

```csharp
services.AddLoggingInterceptor();
```

**Caching Interceptor:**

```csharp
services.AddCachingInterceptor(TimeSpan.FromMinutes(10));
```

**Security Interceptor:**

```csharp
services.AddJwtBearerInterceptor("jwt-token-secret-name");
```

### Data Configuration Usage

**Connection Strings:**

```csharp
services.AddConnectionString(configuration, "DefaultConnection");
services.AddConnectionStringFromSecret("db-connection-secret");
```

## Architecture Alignment

This restructuring aligns with VBD principles:

- **Managers:** Service collection extensions and configuration setup
- **Engines:** Core interceptor logic and secret retrieval
- **Accessors:** Azure service clients and configuration sources

Each library maintains proper abstraction boundaries and follows the established proxy pattern for inter-component communication.

## Next Steps

1. **Update consuming applications** to use the new individual libraries
2. **Remove references** to the old consolidated configuration library
3. **Validate functionality** in development and testing environments
4. **Consider additional interceptors** as separate libraries (Circuit Breaker, Rate Limiting, etc.)

This restructuring provides a solid foundation for scalable, maintainable Azure integrations following enterprise architecture best practices.
