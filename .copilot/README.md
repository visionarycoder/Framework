# Copilot Instructions Directory

**Purpose:** Domain-specific AI-assisted code generation instructions for the VisionaryCoder Framework project.

## Overview

This directory contains focused, composable instruction files that guide GitHub Copilot in generating high-quality code, tests, and documentation aligned with enterprise best practices and the VisionaryCoder coding standards.

## Instruction Hierarchy

Instructions are applied in order of specificity (most specific wins):

1. **Domain-Specific Instructions** (this folder) - Context-aware rules for code generation, testing, quality
2. **Enterprise Baseline** (`.github/copilot-instructions.md`) - Organization-wide standards and architecture patterns
3. **Repository Standards** (`repo-standards.md`) - Project structure, hygiene, and collaboration practices

## Instruction Files

| File | Scope | ApplyTo Pattern | Description |
|------|-------|-----------------|-------------|
| `copilot-instructions.md` | Base | `**/*` | Aggregation hub and precedence rules |
| `code-generation.instructions.md` | Code | `**/*.cs` | C# naming, structure, async patterns, API design |
| `unit-test.instructions.md` | Testing | `**/tests/**` | Unit test structure, naming, coverage, determinism |
| `code-quality.instructions.md` | Quality | `**/*.cs` | Analyzers, nullable types, SARIF artifacts |
| `design-patterns.instructions.md` | Patterns | `**/*.cs` | GoF and enterprise pattern examples |
| `repo-standards.md` | Repository | `**/*` | Git hygiene, PR practices, CI/CD checklist |

## Quick Start

### For Code Generation
1. Review `code-generation.instructions.md` for naming and async rules
2. Use PascalCase for public types, camelCase for locals
3. Never use underscore prefixes
4. Always include `CancellationToken` for async I/O operations

### For Testing
1. Review `unit-test.instructions.md` for test structure
2. Use naming pattern: `MethodName_WhenCondition_ShouldOutcome`
3. Follow Arrange/Act/Assert structure
4. Use Moq for mocking, FluentAssertions for assertions

### For Quality
1. Review `code-quality.instructions.md` for analyzer setup
2. Enable nullable reference types and treat warnings as errors
3. Run `dotnet format` before committing
4. Generate coverage reports with `dotnet test --collect:"XPlat Code Coverage"`

## Key Principles

### Naming Conventions
- **Public Types/Members:** PascalCase (`CustomerService`, `ProcessAsync`)
- **Local Variables/Parameters:** camelCase (`customerId`, `orderData`)
- **Private Fields:** camelCase **without underscore prefix** (`logger`, `repository`)
- **Async Methods:** Suffix with `Async` (`GetDataAsync`, `ProcessOrderAsync`)

### Code Structure
- One public type per file
- Namespace mirrors folder structure
- Immutability preferred (records, readonly)
- Dependency injection via constructors

### Async Patterns
- Use `async`/`await` for all I/O-bound operations
- Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`
- Accept `CancellationToken` for all async public APIs
- Use `Task<T>` for async operations; `ValueTask<T>` only when profiling shows benefit

### Testing Standards
- One behavioral expectation per test
- Deterministic tests (no time, randomness, network, filesystem dependencies)
- Use strict mocks (`MockBehavior.Strict`) to verify interactions
- All async tests use `async Task` return type

### Quality Gates
- No analyzer warnings in builds
- Nullable reference types enabled
- Code coverage monitored for regressions
- XML documentation for non-trivial public APIs

## Anti-Patterns (Always Reject)

❌ Underscore prefixes for field names  
❌ Synchronous-over-async (`.Result`, `.Wait()`)  
❌ Global mutable state  
❌ Broad catch blocks swallowing errors  
❌ Magic numbers without named constants  
❌ Static service locator pattern  
❌ Logging sensitive data (passwords, tokens, PII)  

## Common Commands

### Code Quality
```bash
# Build with analyzers
dotnet build -c Release

# Format code
dotnet format

# Export SARIF diagnostics
dotnet build -c Release /p:ErrorLog=./code-analysis.sarif
```

### Testing & Coverage
```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:CoverageReport -reporttypes:Html
```

### Repository Hygiene
```bash
# Check for uncommitted changes
git status

# Conventional commit format
git commit -m "feat: add customer processing feature"
git commit -m "fix: resolve null reference in order validation"
git commit -m "docs: update API documentation"
```

## Versioning

Each instruction file uses semantic versioning (`major.minor.patch`):

- **Major:** Paradigm shift (e.g., new architecture pattern)
- **Minor:** New rules or substantial changes
- **Patch:** Clarifications or minor corrections

Update the `version` and `lastUpdated` fields in the frontmatter when modifying files.

## Evolution & Maintenance

### When to Update
- New coding standards adopted
- Architectural decisions documented (ADRs)
- Technology stack changes (.NET versions, libraries)
- Pattern recommendations evolve

### How to Update
1. Modify specific instruction file
2. Increment version according to change scope
3. Update `lastUpdated` timestamp
4. Document in changelog if major/minor version
5. Reference related ADR in commit message
6. Keep changes atomic and focused

### Adding New Instructions
1. Create new file: `{topic}.instructions.md`
2. Include YAML frontmatter with `applyTo`, `version`, `lastUpdated`, `scope`
3. Reference from `copilot-instructions.md` index
4. Update this README with new file entry
5. Follow single-responsibility principle per file

## Cross-References

### Enterprise Guidance
- **Architecture:** `.github/copilot-instructions.md` - VBD, security, APIs, resilience
- **Technology Stack:** `.github/copilot-instructions.md` - .NET 8+, C# 12, Blazor, EF Core

### Repository Documentation
- **ADRs:** `docs/adr/` - Architectural Decision Records
- **Best Practices:** `docs/best-practices/` - Domain-specific guidance

## Support & Contribution

### Questions or Clarifications
- Review relevant instruction file for your domain
- Check ADRs for architectural context
- Consult `.github/copilot-instructions.md` for enterprise standards

### Contributing Improvements
1. Propose changes via PR with clear rationale
2. Include examples demonstrating the improvement
3. Reference industry standards or Microsoft documentation
4. Update version and changelog
5. Ensure backward compatibility or document breaking changes

---

**Last Updated:** 2025-11-20  
**Maintainers:** VisionaryCoder Framework Team  
**Compatibility:** .NET 8+, C# 12+, forward-compatible with .NET 10 LTS
