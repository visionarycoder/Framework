# VisionaryCoder.Framework

A comprehensive core framework library providing foundational features for the VisionaryCoder ecosystem.

## Overview

The `VisionaryCoder.Framework` project serves as the foundational library for the entire VisionaryCoder Framework ecosystem. It provides core services, utilities, and patterns that are used throughout all other framework components.

## Features

### Core Services

- **Framework Information Provider**: Provides metadata about the framework version, compilation time, and description
- **Correlation ID Provider**: Manages correlation IDs for distributed request tracking
- **Request ID Provider**: Manages request IDs for individual request tracking
- **Response Wrapper**: Consistent success/failure handling with `Response<T>` and `Response`

### Configuration

- **Service Collection Extensions**: Easy registration of framework services via dependency injection
- **Framework Options**: Configurable settings for framework behavior
- **Framework Constants**: Centralized constants for timeouts, headers, and logging

### Key Components

#### FrameworkConstants

Provides framework-wide constants including:

- Version information
- Default timeout values
- Common HTTP headers
- Logging configuration

#### ServiceCollectionExtensions

Extension methods for easy framework integration:

```csharp
services.AddVisionaryCoderFramework();
services.AddVisionaryCoderFramework(options => 
{
    options.EnableCorrelationId = true;
    options.DefaultHttpTimeoutSeconds = 60;
});
```

#### Response<T>

Consistent result wrapper for operations:

```csharp
var response = Response<string>.Success("Hello World");
response.Match(
    onSuccess: value => Console.WriteLine(value),
    onFailure: (error, ex) => Console.WriteLine($"Error: {error}")
);
```

## Project Structure

``` text
VisionaryCoder.Framework/
├── Abstractions.cs              # Core interfaces
├── FrameworkConstants.cs        # Framework constants
├── FrameworkResult.cs          # Result wrapper types
├── Implementations.cs          # Default implementations
├── ServiceCollectionExtensions.cs # DI extensions
└── VisionaryCoder.Framework.csproj
```

## Dependencies

- **.NET 8.0**: Target framework
- **Microsoft.Extensions.DependencyInjection.Abstractions**: For dependency injection
- **Microsoft.Extensions.Logging.Abstractions**: For logging abstractions
- **Microsoft.Extensions.Options**: For configuration options
- **VisionaryCoder.Framework.Abstractions**: Core framework abstractions

## Integration

This project is automatically included when referencing the VisionaryCoder Framework ecosystem. It provides the foundational services that other framework components depend on.

## Version

Current Version: **1.0.0**

Built with C# 12 and .NET 8.0, following Microsoft naming conventions and modern C# practices including primary constructors where applicable.
