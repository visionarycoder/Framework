# Copilot Instructions Index

Quick navigation to all instruction files with their purpose and key topics.

---

## 📖 Overview & Getting Started

### [README.md](README.md)
**Comprehensive guide to the .copilot folder**
- Directory structure and file index
- Instruction hierarchy and precedence
- Key principles and anti-patterns
- Common commands and quick start guides
- Versioning and maintenance guidelines

### [QUICK_REFERENCE.md](QUICK_REFERENCE.md)
**Fast lookup for common scenarios**
- Service creation template
- Unit test examples
- Essential commands
- Naming quick reference
- Anti-patterns checklist

### [CHANGELOG.md](CHANGELOG.md)
**Version history and notable changes**
- Tracks updates to instruction files
- Documents breaking changes
- Records additions and improvements

---

## 🎯 Core Instruction Files

### [copilot-instructions.md](copilot-instructions.md)
**Base instructions and precedence rules**
- **ApplyTo:** `**/*` (all files)
- **Version:** 2.0.0
- **Topics:**
  - Instruction precedence order
  - Domain instruction index
  - Core principles snapshot
  - Quality gates checklist
  - Cross-references

### [code-generation.instructions.md](code-generation.instructions.md)
**C# code generation guidelines**
- **ApplyTo:** `**/*.cs` (excluding tests)
- **Version:** 2.0.0
- **Topics:**
  - Naming conventions (types, members, async)
  - File and project structure
  - API design and async patterns
  - Error handling and validation
  - Records, immutability, dependency injection
  - Complete code examples

### [unit-test.instructions.md](unit-test.instructions.md)
**Unit testing standards**
- **ApplyTo:** `**/tests/**`, `**/*.test.cs`, `**/*.spec.cs`
- **Version:** 2.0.0
- **Topics:**
  - Test naming conventions
  - Arrange/Act/Assert structure
  - Moq best practices and patterns
  - FluentAssertions usage
  - Deterministic testing strategies
  - Coverage collection and reporting
  - Advanced patterns (exception testing, data-driven tests)

### [code-quality.instructions.md](code-quality.instructions.md)
**Static analysis and quality gates**
- **ApplyTo:** `**/*.cs` (excluding build artifacts)
- **Version:** 2.0.0
- **Topics:**
  - Analyzer configuration (Roslyn, StyleCop, SonarAnalyzer)
  - Nullable reference types
  - EditorConfig settings
  - Code formatting commands
  - SARIF export
  - Quality checklist
  - CI/CD integration

### [design-patterns.instructions.md](design-patterns.instructions.md)
**Design pattern examples**
- **ApplyTo:** `**/*.cs`
- **Version:** 2.0.0
- **Topics:**
  - Gang of Four patterns (Creational, Structural, Behavioral)
  - Enterprise patterns (Repository, CQRS, Saga)
  - Complete C# 12 examples
  - Pattern selection guide
  - Testing pattern implementations

### [repo-standards.md](repo-standards.md)
**Repository hygiene and collaboration**
- **ApplyTo:** `**/*` (all files)
- **Version:** 2.0.0
- **Topics:**
  - Conventional commits
  - Repository structure
  - Documentation standards (README, ADRs)
  - Testing and quality requirements
  - CI/CD pipeline standards
  - Security best practices
  - Pre-merge checklist

---

## 🔗 External References

### Enterprise Baseline
**[.github/copilot-instructions.md](../.github/copilot-instructions.md)**
- Organization-wide architecture patterns
- Volatility-Based Decomposition (VBD)
- Security and inter-component communication
- Integration and API best practices
- Observability and monitoring
- Azure development guidelines

### Microsoft Documentation
- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [.NET Compiler Platform Analyzers](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview)
- [Nullable Reference Types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references)

---

## 📋 Quick Topic Lookup

### Need to...

**Create a new service?**
→ [code-generation.instructions.md](code-generation.instructions.md) + [QUICK_REFERENCE.md](QUICK_REFERENCE.md)

**Write unit tests?**
→ [unit-test.instructions.md](unit-test.instructions.md) + [QUICK_REFERENCE.md](QUICK_REFERENCE.md)

**Set up analyzers?**
→ [code-quality.instructions.md](code-quality.instructions.md)

**Implement a design pattern?**
→ [design-patterns.instructions.md](design-patterns.instructions.md)

**Understand naming conventions?**
→ [code-generation.instructions.md](code-generation.instructions.md#naming-conventions) + [QUICK_REFERENCE.md](QUICK_REFERENCE.md#naming-quick-reference)

**Set up CI/CD?**
→ [repo-standards.md](repo-standards.md#cicd-pipeline-standards)

**Write commit messages?**
→ [repo-standards.md](repo-standards.md#commit-message-conventions) + [QUICK_REFERENCE.md](QUICK_REFERENCE.md#git--commits)

**Understand architecture?**
→ [.github/copilot-instructions.md](../.github/copilot-instructions.md)

---

## 🎨 Instruction Precedence

When multiple instruction files apply, the precedence order is:

1. **Domain-Specific** (most specific applyTo pattern)
   - Example: `unit-test.instructions.md` for test files
2. **Base Instructions** (`copilot-instructions.md`)
3. **Enterprise Baseline** (`.github/copilot-instructions.md`)
4. **Repository Standards** (`repo-standards.md`)

---

## 🔄 Maintenance

### When to Update
- New coding standards adopted
- Architectural decisions documented (ADRs)
- Technology stack changes
- Pattern recommendations evolve

### How to Update
1. Modify specific instruction file
2. Increment version (major.minor.patch)
3. Update `lastUpdated` field
4. Document in [CHANGELOG.md](CHANGELOG.md)
5. Reference ADR in commit message

---

## 📞 Support

For questions or improvements:
1. Review relevant instruction file
2. Check [QUICK_REFERENCE.md](QUICK_REFERENCE.md)
3. Consult [.github/copilot-instructions.md](../.github/copilot-instructions.md)
4. Create issue or PR with clear rationale

---

**Last Updated:** 2025-11-20  
**Compatibility:** .NET 8+, C# 12+, forward-compatible with .NET 10 LTS
