# VisionaryCoder Framework

[![Build & Test](https://github.com/visionarycoder/vc/actions/workflows/publish.yml/badge.svg)](https://github.com/visionarycoder/vc/actions/workflows/publish.yml)
[![NuGet](https://img.shields.io/nuget/v/VisionaryCoder.Framework.Core.svg)](https://www.nuget.org/packages/VisionaryCoder.Framework.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A collection of enterprise-grade libraries and infrastructure components designed for **clean, reproducible, and automated development**.  
This framework is built with **multi‑targeting (.NET 8 + .NET 10)**, **Nerdbank.GitVersioning (NBGV)**, and a **CI/CD pipeline** that publishes prereleases to GitHub Packages and stable releases to NuGet.org.

---

## 🚀 Quickstart

```bash
# Clone
git clone https://github.com/visionarycoder/vc.git
cd vc

# Restore
dotnet restore VisionaryCoder.Framework.sln

# Build & Test
dotnet build VisionaryCoder.Framework.sln --configuration Release
dotnet test VisionaryCoder.Framework.sln --configuration Release
```

---

## 📦 Framework Components

The VisionaryCoder Framework includes the following components:

### Core Libraries

- **VisionaryCoder.Framework.Core** - Base entity classes and core abstractions
- **VisionaryCoder.Framework.Abstractions** - Core interfaces and contracts

### Extensions

- **VisionaryCoder.Framework.Extensions** - General utility extensions
- **VisionaryCoder.Framework.Extensions.Configuration** - Configuration helpers and providers
- **VisionaryCoder.Framework.Extensions.Logging** - Enhanced logging capabilities
- **VisionaryCoder.Framework.Extensions.Pagination** - Pagination support for collections
- **VisionaryCoder.Framework.Extensions.Querying** - Advanced querying capabilities
- **VisionaryCoder.Framework.Extensions.Security** - Security utilities and helpers

### Platform-Specific Extensions

- **VisionaryCoder.Framework.Extensions.Primitives** - Primitive type extensions
- **VisionaryCoder.Framework.Extensions.Primitives.AspNetCore** - ASP.NET Core integration
- **VisionaryCoder.Framework.Extensions.Primitives.EFCore** - Entity Framework Core integration

### Proxy & Service Architecture

- **VisionaryCoder.Framework.Proxy** - Proxy pattern implementations
- **VisionaryCoder.Framework.Proxy.Abstractions** - Proxy abstractions and contracts
- **VisionaryCoder.Framework.Proxy.Caching** - Caching proxy implementations
- **VisionaryCoder.Framework.Proxy.DependencyInjection** - DI container integration
- **VisionaryCoder.Framework.Proxy.Interceptors** - Method interception capabilities

### Data Access

- **VisionaryCoder.Framework.Data.Abstractions** - Data access abstractions
- **VisionaryCoder.Framework.Services.Abstractions** - Service layer abstractions
- **VisionaryCoder.Framework.Services.FileSystem** - File system services

### Examples

- **VisionaryCoder.Framework.Example** - Usage examples and demonstrations

---

## 🏗️ Architecture

The VisionaryCoder Framework follows **Volatility-Based Decomposition (VBD)** principles:

- **Managers**: Workflow orchestration and business process coordination
- **Engines**: Core business logic and domain operations  
- **Accessors**: Data access and external service integration

All components communicate through contract interfaces and proxy implementations, enabling clean separation of concerns and excellent testability.

---

## 🤝 Contributing

Contributions are welcome! Please read our contributing guidelines and submit pull requests for any improvements.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2025 VisionaryCoder
