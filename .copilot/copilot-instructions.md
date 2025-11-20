---
applyTo: "**/*"
version: 2.0.0
lastUpdated: 2025-11-20
scope: copilot-base
---

# Copilot Base Instructions

## Purpose

Aggregation hub for AI-assisted code generation in the VisionaryCoder Framework. This file delegates domain-specific rules to focused instruction files and establishes precedence order for instruction application.

## Precedence Order (Most Specific Wins)

1. **Domain-Specific Instructions** (this folder) - File-specific rules (e.g., `code-generation.instructions.md`, `unit-test.instructions.md`)
2. **Base Instructions** (this file) - Project-level defaults and cross-cutting concerns
3. **Enterprise Baseline** (`.github/copilot-instructions.md`) - Organization-wide architecture and standards
4. **Repository Standards** (`repo-standards.md`) - Git hygiene, collaboration, CI/CD practices

## Usage Workflow

1. **Identify Domain:** Determine task type (code generation, testing, patterns, quality, repository)
2. **Load Instructions:** Reference the appropriate domain-specific instruction file
3. **Generate Code:** Produce minimal, maintainable artifacts adhering to naming and async conventions
4. **Add Tests:** Include unit/integration tests for all new logic (see `unit-test.instructions.md`)
5. **Document:** Update README or add XML summaries for non-trivial public APIs
6. **Cross-Reference:** Link to ADRs for architectural decisions

## Domain Instruction Index

| File | Scope | ApplyTo Pattern | Description |
|------|-------|-----------------|-------------|
| `code-generation.instructions.md` | Code Generation | `**/*.cs` | C# naming, structure, async patterns, API design, error handling |
| `unit-test.instructions.md` | Testing | `**/tests/**` | Test structure, naming conventions, coverage, determinism |
| `code-quality.instructions.md` | Quality | `**/*.cs` | Analyzers, nullable types, formatting, SARIF diagnostics |
| `design-patterns.instructions.md` | Patterns | `**/*.cs` | GoF patterns, enterprise patterns, modern C# examples |
| `repo-standards.md` | Repository | `**/*` | Git hygiene, PR practices, conventional commits, CI/CD |

## Core Principles Snapshot

### Naming Conventions
- **Public Types/Members:** PascalCase (`CustomerService`, `IOrderRepository`, `ProcessAsync`)
- **Local Variables/Parameters:** camelCase (`customerId`, `orderData`, `cancellationToken`)
- **Private Fields:** camelCase **without underscore prefix** (`logger`, `repository`, `httpClient`)
- **Async Methods:** Always suffix with `Async` (`GetDataAsync`, `SaveOrderAsync`)
- **Boolean Members:** Use `Is`, `Has`, `Can`, `Should` prefixes (`IsValid`, `HasPermission`)

### Code Structure
- One public type per file; file name matches type name
- Namespace mirrors folder structure (`src/Services/Order` → `VisionaryCoder.Framework.Services.Order`)
- Prefer composition over inheritance
- Keep public surface minimal; internal by default
- Use file-scoped namespaces (C# 10+)

### Async Patterns
- Use `async`/`await` for all I/O-bound operations
- Never block async code (`.Result`, `.Wait()`, `.GetAwaiter().GetResult()`)
- Accept `CancellationToken` for all public async APIs
- Use `Task<T>` by default; `ValueTask<T>` only when profiling proves benefit
- Propagate cancellation tokens through call chains

### Error Handling
- Validate arguments early with guard clauses (`ArgumentNullException`, `ArgumentException`)
- Use custom domain exceptions for business rule violations
- Never catch broad `Exception` unless translating to domain error
- Preserve stack traces when rethrowing (`throw;`, not `throw ex;`)
- Log exceptions with structured context (correlation IDs, operation names)

### Immutability & Nullability
- Prefer `record` for immutable data carriers
- Use `record struct` for small value types
- Enable nullable reference types (`<Nullable>enable</Nullable>`)
- Return empty collections instead of `null`
- Use null-forgiving operator (`!`) only when compiler cannot infer safety

### Dependency Injection
- Inject dependencies via constructors
- Avoid static service locator pattern
- Use strongly-typed options classes for configuration
- Validate options on startup (`IValidateOptions<T>`)
- Prefer interfaces for testability

## Anti-Patterns (Reject Immediately)

❌ **Underscore prefixes** for field names (`_logger` → use `logger`)  
❌ **Synchronous-over-async** (`.Result`, `.Wait()`)  
❌ **Global mutable state** (static fields with side effects)  
❌ **Broad catch blocks** swallowing errors without context  
❌ **Magic numbers/strings** without named constants  
❌ **Unstructured logging** or logging sensitive data  
❌ **Partially initialized objects** returned from constructors  
❌ **Deep inheritance hierarchies** (prefer Strategy/Decorator patterns)  

## Quality Gates (Before Commit)

1. ✅ Code builds without analyzer warnings
2. ✅ All tests pass locally
3. ✅ No blocking async calls (`.Result`, `.Wait()`)
4. ✅ Naming conventions followed (no underscores, proper casing)
5. ✅ Public APIs documented (XML summaries for non-trivial methods)
6. ✅ Nullable warnings resolved
7. ✅ No TODO comments or dead code
8. ✅ ADRs linked for architectural changes

## Common Commands Reference

### Code Quality
```bash
# Build with full analysis
dotnet build -c Release

# Format code
dotnet format

# Export diagnostics
dotnet build /p:ErrorLog=./code-analysis.sarif
```

### Testing
```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:CoverageReport -reporttypes:Html
```

### Repository
```bash
# Conventional commit examples
git commit -m "feat(orders): add order validation service"
git commit -m "fix(auth): resolve token refresh race condition"
git commit -m "docs: update API documentation with examples"
git commit -m "refactor(data): extract repository pattern"
```

## Evolution & Versioning

### Version Increment Guidelines
- **Major (X.0.0):** Paradigm shift, breaking changes to instruction structure
- **Minor (X.Y.0):** New rules added, substantial changes to existing guidelines
- **Patch (X.Y.Z):** Clarifications, typo fixes, minor improvements

### Change Protocol
1. Modify specific domain instruction file (keep changes atomic)
2. Increment `version` field in frontmatter
3. Update `lastUpdated` timestamp (ISO 8601 format)
4. Document in changelog for major/minor versions
5. Link to relevant ADR in commit message
6. Keep backward compatibility when possible

## Cross-References

### Internal Documentation
- **Enterprise Architecture:** `.github/copilot-instructions.md` - VBD, security, APIs, resilience patterns
- **Architectural Decisions:** `docs/adr/` - Decision records with context and consequences
- **Best Practices:** `docs/best-practices/` - Domain-specific detailed guidance

### External Standards
- **C# Coding Conventions:** [Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **Framework Design Guidelines:** [Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- **Async Best Practices:** [Stephen Cleary's Blog](https://blog.stephencleary.com/2012/02/async-and-await.html)

---

**Focused, composable instruction set—extend via new domain-specific files as needed.**

**Compatibility:** .NET 8+, C# 12+, forward-compatible with .NET 10 LTS
