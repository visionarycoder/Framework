---
# 🔧 Copilot Instruction Metadata
version: 1.0.0
schema: 1
updated: 2025-10-03
owner: Platform/IDL
stability: stable
domain: csharp
# version semantics: MAJOR / MINOR / PATCH
---

# C# / .NET Instructions

## Scope
Applies to `.cs`, `.razor`, `.csproj`, and `.sln` files.

## Language Conventions
- **Multi-target**: Projects target both net8.0 and net10.0 (WinUI targets net8.0-windows10.0.19041.0 and net10.0-windows10.0.19041.0).
- Use primary constructors and collection expressions.
- Prefer `ref readonly` for public APIs.
- Avoid `dynamic`; use strong typing.
- Use PascalCase for types and camelCase for locals.
- Prefer expression-bodied members for simple logic.
- Use `var` only when type is obvious.
- Never use `_` prefix for private fields.
- Avoid `Thread.Sleep`; use `Task.Delay` or proper async patterns.
- Use `using` alias directives for verbose types.
- Enable nullable reference types (`<Nullable>enable</Nullable>`).
- Enable implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`).
- Use latest C# language version (`<LangVersion>latest</LangVersion>`).

## Build & Format
- Build with `dotnet build` (uses `HardwareMonitor.sln` which excludes .wapproj).
- Format with `dotnet format`.
- XML documentation warnings (CS1570, CS1591) are informational.
- Address nullable reference warnings (CS8602) with proper null checks.

## Project Structure
- **Central Package Management**: Currently enabled (`ManagePackageVersionsCentrally=true`).
- **Solution structure**: Standard .sln file excludes WinUI packaging project (.wapproj).
- **Dependencies**: `Directory.Build.props` defines common properties and version variables.
- **Package versions**: Orleans (latest), Aspire (latest), MSTest (latest), FluentAssertions (latest).

## Extension Methods Pattern
- Create static extension classes in dedicated `Model.Extensions/` project.
- All extension methods must be `public static`.
- Implement health assessment with component-specific thresholds.
- Use descriptive method names: `IsHealthy()`, `GetFormattedTemperature()`, `GetStatus()`.
- Extension methods enable testability without modifying core models.

## Testing
- Use MSTest (latest) for all unit and integration tests.
- Use `[TestMethod]` with `[DataRow(...)]` for data-driven tests.
- Use FluentAssertions (latest) for readable assertions.
- Use `[TestInitialize]` and `[TestCleanup]` for setup/teardown.
- Follow Arrange-Act-Assert structure.
- Organize tests with `#region` blocks for logical grouping.
- Multi-target tests: Ensure coverage on both net8.0 and net10.0.

## 📝 Changelog
### 1.0.0 (2025-10-03)
- Added metadata header (initial versioning schema).

### 1.1.0 (2025-10-10)
- Established C#/.NET coding standards and best practices.
- Improved project structure and organization.
