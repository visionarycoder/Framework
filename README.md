# VisionaryCoder Framework

[![Build & Test](https://github.com/visionarycoder/vc/actions/workflows/publish.yml/badge.svg)](https://github.com/visionarycoder/vc/actions/workflows/publish.yml)
[![NuGet](https://img.shields.io/nuget/v/VisionaryCoder.Framework.Core.svg)](https://www.nuget.org/packages/VisionaryCoder.Framework.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A modular, enterprise-grade framework starting with a single foundational library (`VisionaryCoder.Framework`) and accompanying test project. The repository emphasizes **clean, reproducible, and automated development** with .NET 8 (ready for forward compatibility to .NET 10). Future packages will evolve incrementally via ADRs.

---

## 🚀 Quickstart

```bash
# Clone
git clone https://github.com/visionarycoder/Framework.git
cd vc

# Restore
dotnet restore VisionaryCoder.Framework.sln

# Build & Test
dotnet build VisionaryCoder.Framework.sln --configuration Release
dotnet test VisionaryCoder.Framework.sln --configuration Release
```

---

## 📦 Current Solution Contents

| Project                          | Type         | Description                                                                                 |
|----------------------------------|--------------|---------------------------------------------------------------------------------------------|
| `VisionaryCoder.Framework`       | Library      | Core framework primitives (configuration, results, options, providers, proxy abstractions). |
| `VisionaryCoder.Framework.Tests` | Test Project | Unit tests validating core behaviors (results, request/correlation IDs, options).           |

Planned future packages (tracked via ADRs) will be introduced gradually rather than pre-listed. See ADR index for roadmap context.

## 🗃️ Repository Structure (High-Level)

```text
/.copilot                              # Modular AI assistant instruction set (base + C# + patterns + standards)
/docs                                  # Documentation (ADRs, best-practices capsules, diagrams, reviews, onboarding)
/src/VisionaryCoder.Framework          # Core library source
/tests/VisionaryCoder.Framework.Tests  # Unit tests
/.github                               # Global Copilot instructions & workflows
```

---

## 🏗️ Architecture Overview

The framework follows **Volatility-Based Decomposition (VBD)** principles. While the current library aggregates foundational concerns, future decomposition will create distinct Manager, Engine, and Accessor component packages as volatility boundaries emerge.

Core library already enforces:

- Contract-first abstractions for providers & proxies
- Structured result + request/correlation context handling
- Dependency injection integration & options binding
- Early extensibility points for caching, security, querying

---

## 📚 Documentation & Guidance

- **ADRs**: `docs/adr/index.md` (recent: ADR-0004 modular Copilot instructions)
- **Best Practices Capsules**: `docs/best-practices/*/readme.md` (architecture, security, observability, etc.)
- **Copilot Instructions**: `.github/copilot-instructions.md` (enterprise baseline) and `.copilot/copilot-instructions.md` (modular hub)
- **Design Patterns Guidance**: `.copilot/design-patterns.instructions.md`
- **C# Generation Heuristics**: `.copilot/csharp.instructions.md`
- **Repository Standards**: `.copilot/repo-standards.md`

## 🤝 Contributing

Contributions are welcome—please open an issue or ADR proposal before large architectural changes. Align new code with:

1. Naming & layering rules (see global Copilot instructions)
2. Volatility boundaries (introduce new packages only when volatility justifies extraction)
3. Modular instruction consistency (update domain index + ADR when extending guidance)

---

## 📄 License

MIT License – see [LICENSE](LICENSE).

Copyright (c) 2025 VisionaryCoder

---
Last synchronized with solution structure: 2025-11-14
