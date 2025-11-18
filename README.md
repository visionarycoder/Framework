# VisionaryCoder Framework

[![Build & Test](https://github.com/visionarycoder/vc/actions/workflows/publish.yml/badge.svg)](https://github.com/visionarycoder/vc/actions/workflows/publish.yml)
[![NuGet](https://img.shields.io/nuget/v/VisionaryCoder.Framework.Core.svg)](https://www.nuget.org/packages/VisionaryCoder.Framework.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A modular, enterprise-grade framework starting from a single foundational library. This repository contains the source, documentation, samples and tests for the VisionaryCoder Framework ecosystem.

---

## 🚀 Quickstart

```bash
# Clone
git clone https://github.com/visionarycoder/Framework.git
cd Framework

# Restore
dotnet restore VisionaryCoder.Framework.sln

# Build & Test
dotnet build VisionaryCoder.Framework.sln --configuration Release
dotnet test VisionaryCoder.Framework.sln --configuration Release
```

---

## 📦 Solution Overview

This repository currently contains a single main library that aggregates foundational capabilities. The intent is to progressively decompose this monolith into smaller packages (see ADRs and roadmap in `docs/`), and this README serves as the top-level index linking to module-level READMEs and developer guidance to make that process easier.

### Projects

- `src/VisionaryCoder.Framework` — Core library and shared utilities (see `src/VisionaryCoder.Framework/README.md`)
- `tests/VisionaryCoder.Framework.Tests` — Unit tests validating framework behaviors

### Module READMEs (entry points)

- Core project README: `src/VisionaryCoder.Framework/README.md`
- Filtering subsystem: `src/VisionaryCoder.Framework/Filtering/README.md`
- Querying serialization & helpers: `src/VisionaryCoder.Framework/Querying/README.md`
- Documentation and architecture decisions: `docs/` (ADRs, best-practices, diagrams)

Use these module READMEs as the canonical documentation when splitting the project into multiple packages.

## 🗃️ Repository Structure (High-Level)

```text
/.copilot
/docs
/src/VisionaryCoder.Framework
  ├─ Filtering/
  ├─ Querying/
  └─ VisionaryCoder.Framework.csproj
/tests/VisionaryCoder.Framework.Tests
/.github
```

## 📚 Documentation & Roadmap

- Architectural Decision Records (ADRs): `docs/adr/index.md`
- Design diagrams and best-practice capsules: `docs/*`
- Roadmap notes: `docs/reviews/*`

## 🧭 How to subdivide this repo (next steps)

If you plan to split this repository into multiple packages, follow these high-level steps:

1. Identify volatility boundaries using VBD (Volatility-Based Decomposition). Good candidates: Filtering, Querying/Serialization, Execution Strategies, POCO helpers, EFCore adapters.
2. Create new projects under `src/` for each package and move code with one-class-per-file, preserving namespaces (e.g., `VisionaryCoder.Framework.Filtering.Abstractions`).
3. Keep `IFilterExecutionStrategy` and other small provider-agnostic interfaces in their own `*.Abstractions` package to avoid circular references.
4. Introduce `VisionaryCoder.Framework.*.csproj` projects with clear dependencies and update solution file.
5. Add module README files (use those in this repo as templates) and ADRs to justify the split.

## 🤝 Contributing

Contributions are welcome. Please open an issue or ADR proposal for large architectural changes. Keep PRs focused and update module READMEs when moving code.

---

This document is the canonical solution-level index. See module READMEs for implementation details and examples.

Last synchronized with solution structure: 2025-11-14
