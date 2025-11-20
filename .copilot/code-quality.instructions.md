---
applyTo: "**/*.cs"
version: 2.0.0
lastUpdated: 2025-11-20
scope: code-quality
excludePatterns:
  - "**/obj/**"
  - "**/bin/**"
  - "**/Migrations/**"
---

# Copilot Instructions: Code Quality & Static Analysis

## Purpose

Guide code generation toward high-quality, analyzable C# code with comprehensive static analysis. This file covers analyzer configuration, nullable reference types, code style enforcement, and diagnostic artifact generation.

## Core Quality Principles

1. **Enforce Analyzer Compliance:** Enable Roslyn NetAnalyzers and StyleCop for consistent code style
2. **Nullable Reference Types:** Eliminate null reference exceptions through compiler enforcement
3. **Warnings as Errors:** Treat warnings as errors in CI for regression detection
4. **Self-Documenting Code:** Prefer clear naming over extensive comments
5. **Single Responsibility:** Keep types small and focused on one concern
6. **Automated Formatting:** Use consistent formatting tools across team

## Analyzer Configuration

### Project-Level Settings
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Target Framework -->
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12.0</LangVersion>
    
    <!-- Nullable Reference Types -->
    <Nullable>enable</Nullable>
    
    <!-- Code Analysis -->
    <AnalysisLevel>latest</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    
    <!-- Treat Warnings as Errors -->
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningsAsErrors />
    
    <!-- Implicit Usings -->
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Generate Documentation File -->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn> <!-- Optional: Suppress missing XML comment warnings -->
  </PropertyGroup>

  <ItemGroup>
    <!-- Roslyn Analyzers -->
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    
    <!-- StyleCop Analyzers -->
    <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    
    <!-- SonarAnalyzer (Optional for enterprise) -->
    <PackageReference Include="SonarAnalyzer.CSharp" Version="9.32.0.97167">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

### EditorConfig Configuration
```ini
# .editorconfig
root = true

[*.cs]
# Indentation and spacing
indent_style = space
indent_size = 4
tab_width = 4

# New line preferences
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true

# C# Code Style Rules

# 'this.' preferences
dotnet_style_qualification_for_field = false:warning
dotnet_style_qualification_for_property = false:warning
dotnet_style_qualification_for_method = false:warning
dotnet_style_qualification_for_event = false:warning

# Language keywords vs framework type names
dotnet_style_predefined_type_for_locals_parameters_members = true:warning
dotnet_style_predefined_type_for_member_access = true:warning

# Modifier preferences
dotnet_style_require_accessibility_modifiers = always:warning
csharp_preferred_modifier_order = public,private,protected,internal,static,extern,new,virtual,abstract,sealed,override,readonly,unsafe,volatile,async:warning

# Expression-level preferences
dotnet_style_object_initializer = true:suggestion
dotnet_style_collection_initializer = true:suggestion
dotnet_style_explicit_tuple_names = true:warning
dotnet_style_prefer_inferred_tuple_names = true:suggestion
dotnet_style_prefer_inferred_anonymous_type_member_names = true:suggestion
dotnet_style_prefer_auto_properties = true:suggestion
dotnet_style_prefer_conditional_expression_over_assignment = true:suggestion
dotnet_style_prefer_conditional_expression_over_return = true:suggestion

# Null checking preferences
csharp_style_conditional_delegate_call = true:warning
dotnet_style_coalesce_expression = true:warning
dotnet_style_null_propagation = true:warning
dotnet_style_prefer_is_null_check_over_reference_equality_method = true:warning

# File header
file_header_template = unset

# Naming Conventions

# Constants are PascalCase
dotnet_naming_rule.constants_should_be_pascal_case.severity = warning
dotnet_naming_rule.constants_should_be_pascal_case.symbols = constants
dotnet_naming_rule.constants_should_be_pascal_case.style = pascal_case_style

dotnet_naming_symbols.constants.applicable_kinds = field
dotnet_naming_symbols.constants.required_modifiers = const

# Private fields are camelCase (NO underscore prefix)
dotnet_naming_rule.private_fields_should_be_camel_case.severity = warning
dotnet_naming_rule.private_fields_should_be_camel_case.symbols = private_fields
dotnet_naming_rule.private_fields_should_be_camel_case.style = camel_case_style

dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

# Async methods end with Async
dotnet_naming_rule.async_methods_end_with_async.severity = warning
dotnet_naming_rule.async_methods_end_with_async.symbols = async_methods
dotnet_naming_rule.async_methods_end_with_async.style = end_with_async_style

dotnet_naming_symbols.async_methods.applicable_kinds = method
dotnet_naming_symbols.async_methods.required_modifiers = async

dotnet_naming_style.end_with_async_style.required_suffix = Async
dotnet_naming_style.end_with_async_style.capitalization = pascal_case

# Interfaces start with I
dotnet_naming_rule.interfaces_start_with_i.severity = warning
dotnet_naming_rule.interfaces_start_with_i.symbols = interfaces
dotnet_naming_rule.interfaces_start_with_i.style = i_prefix_style

dotnet_naming_symbols.interfaces.applicable_kinds = interface

dotnet_naming_style.i_prefix_style.required_prefix = I
dotnet_naming_style.i_prefix_style.capitalization = pascal_case

# Naming styles
dotnet_naming_style.pascal_case_style.capitalization = pascal_case
dotnet_naming_style.camel_case_style.capitalization = camel_case

# CA rules configuration
dotnet_diagnostic.CA1031.severity = warning # Do not catch general exception types
dotnet_diagnostic.CA1062.severity = warning # Validate arguments of public methods
dotnet_diagnostic.CA1303.severity = none # Do not pass literals as localized parameters (optional)
dotnet_diagnostic.CA1707.severity = warning # Identifiers should not contain underscores
dotnet_diagnostic.CA1716.severity = warning # Identifiers should not match keywords
dotnet_diagnostic.CA2007.severity = none # Consider calling ConfigureAwait (optional in modern .NET)
```

## Nullable Reference Types

### Enable Project-Wide
```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

### Nullable Annotations
```csharp
// ✅ Correct nullable usage
public sealed class CustomerService
{
    private readonly ILogger<CustomerService> logger;
    
    public CustomerService(ILogger<CustomerService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    // Nullable return type
    public async Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await repository.FindAsync(id, cancellationToken);
        return customer; // Can be null
    }
    
    // Non-nullable return type
    public async Task<Customer> GetRequiredCustomerAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await repository.FindAsync(id, cancellationToken);
        return customer ?? throw new CustomerNotFoundException(id);
    }
    
    // Nullable parameter
    public IEnumerable<Customer> Search(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Array.Empty<Customer>();
        }
        
        return repository.Search(searchTerm);
    }
}

// ❌ Avoid null-forgiving operator unless compiler cannot infer
string name = customer!.Name; // Only use when you know it's not null but compiler can't prove it
```

### Null Handling Patterns
```csharp
// ✅ Null coalescing
var name = customer?.Name ?? "Unknown";

// ✅ Null propagation
var length = customer?.Name?.Length ?? 0;

// ✅ Pattern matching
if (customer is not null)
{
    ProcessCustomer(customer);
}

// ✅ Null coalescing assignment
config ??= LoadDefaultConfiguration();

// ✅ Null conditional invocation
onSuccess?.Invoke(result);
```

## Code Quality Commands

### Build with Analysis
```bash
# Standard build with all analyzers
dotnet build -c Release

# Build with verbose logging
dotnet build -c Release -v detailed

# Build specific project
dotnet build src/VisionaryCoder.Framework/VisionaryCoder.Framework.csproj -c Release
```

### Code Formatting
```bash
# Install dotnet-format tool (if not already installed)
dotnet tool install -g dotnet-format

# Format entire solution
dotnet format

# Format and verify (exit code 1 if changes needed)
dotnet format --verify-no-changes

# Format specific project
dotnet format src/VisionaryCoder.Framework/VisionaryCoder.Framework.csproj

# Format with specific severity
dotnet format --severity warn
```

### SARIF Export
```bash
# Export diagnostics to SARIF format
dotnet build -c Release /p:ErrorLog=./analysis-results.sarif

# Export with specific log format
dotnet build /p:ErrorLog="analysis-results.sarif;version=2.1"

# Export to specific directory
dotnet build /p:ErrorLog="../artifacts/analysis/code-analysis.sarif"
```

### Multiple Artifact Generation
```bash
# Build, test, and generate all artifacts
dotnet build -c Release /p:ErrorLog=./code-analysis.sarif
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:CoverageReport -reporttypes:Html
```

## Advanced Static Analysis

### SonarQube Integration
```bash
# Install SonarScanner (once)
dotnet tool install --global dotnet-sonarscanner

# Begin analysis
dotnet sonarscanner begin /k:"VisionaryCoder.Framework" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.login="<your-token>" \
  /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"

# Build and test
dotnet build -c Release
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

# End analysis and upload
dotnet sonarscanner end /d:sonar.login="<your-token>"
```

### Security Analysis
```bash
# Install security analyzers
dotnet add package Microsoft.CodeAnalysis.BannedApiAnalyzers
dotnet add package Microsoft.CodeAnalysis.PublicApiAnalyzers
dotnet add package SecurityCodeScan.VS2019

# Run security scan
dotnet build -c Release
```

## Quality Checklist

Before committing code:

1. ✅ No analyzer warnings (or documented suppressions with rationale)
2. ✅ All nullable warnings resolved (no implicit null usage)
3. ✅ Cyclomatic complexity reasonable (refactor if excessive)
4. ✅ Public APIs documented (XML comments for non-trivial members)
5. ✅ Magic numbers extracted to named constants
6. ✅ No unused using directives or dead code
7. ✅ No synchronous-over-async wrappers (`.Result`, `.Wait()`)
8. ✅ No hidden allocations in hot paths (profile before optimizing)
9. ✅ Code formatted with `dotnet format`
10. ✅ All tests pass locally

## Suppression Guidelines

### When to Suppress
- **Systemic Issues:** Use `<NoWarn>` in project file with ADR justification
- **Localized Issues:** Use `#pragma warning disable` with inline comment explaining why
- **False Positives:** Suppress with clear rationale and link to issue tracker

### Suppression Examples
```csharp
// ✅ Correct suppression with rationale
#pragma warning disable CA1031 // Do not catch general exception types
try
{
    // Third-party library may throw any exception type
    await externalService.ProcessAsync(data);
}
catch (Exception ex)
{
    // Log and translate to domain exception
    logger.LogError(ex, "External service call failed");
    throw new ExternalServiceException("Service unavailable", ex);
}
#pragma warning restore CA1031

// ❌ Incorrect - no explanation
#pragma warning disable CA1031
catch (Exception ex) { }
#pragma warning restore CA1031
```

### Project-Level Suppressions
```xml
<PropertyGroup>
  <!-- Suppress specific warnings with justification -->
  <NoWarn>$(NoWarn);CA1303;CA1308</NoWarn> <!-- Localization not required; lowercase conversion acceptable -->
</PropertyGroup>
```

## Performance Considerations

### Allocation Analysis
```bash
# Install BenchmarkDotNet for performance testing
dotnet add package BenchmarkDotNet

# Run benchmarks
dotnet run -c Release --project tests/VisionaryCoder.Framework.Benchmarks
```

### Async Best Practices
```csharp
// ✅ Avoid unnecessary async state machines
public Task<int> GetValueAsync()
{
    return repository.GetValueAsync(); // No await needed, return task directly
}

// ✅ Use ValueTask for high-frequency paths (when profiling shows benefit)
public ValueTask<int> GetCachedValueAsync(string key)
{
    if (cache.TryGetValue(key, out var value))
    {
        return new ValueTask<int>(value); // Synchronous completion
    }
    
    return new ValueTask<int>(FetchFromDatabaseAsync(key));
}

// ❌ Avoid synchronous blocking
public int GetValue()
{
    return GetValueAsync().Result; // NEVER DO THIS
}
```

## CI/CD Integration

### GitHub Actions Example
```yaml
name: Code Quality

on: [push, pull_request]

jobs:
  analyze:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build with analysis
        run: dotnet build -c Release /p:ErrorLog=./code-analysis.sarif
      
      - name: Run tests with coverage
        run: dotnet test --collect:"XPlat Code Coverage"
      
      - name: Generate coverage report
        run: |
          dotnet tool install --global dotnet-reportgenerator-globaltool
          reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:CoverageReport -reporttypes:Html
      
      - name: Upload SARIF
        uses: github/codeql-action/upload-sarif@v2
        with:
          sarif_file: code-analysis.sarif
      
      - name: Upload coverage
        uses: actions/upload-artifact@v3
        with:
          name: coverage-report
          path: CoverageReport/
```

## Metrics & Monitoring

### Code Metrics Analysis
```bash
# Install metrics tool
dotnet tool install --global dotnet-metrics

# Generate metrics report
dotnet-metrics --solution VisionaryCoder.Framework.sln --output metrics.html
```

### Complexity Monitoring
- **Cyclomatic Complexity:** Target < 10 per method
- **Lines of Code:** Target < 200 per class, < 50 per method
- **Depth of Inheritance:** Target < 4 levels
- **Class Coupling:** Target < 10 dependencies per class

## Anti-Patterns (Always Reject)

❌ Disabling analyzers without justification  
❌ Suppressing warnings globally without ADR  
❌ Ignoring nullable warnings  
❌ Using `dynamic` without strong justification  
❌ Using reflection without caching  
❌ String concatenation in loops (use `StringBuilder`)  
❌ LINQ in performance-critical paths without profiling  
❌ Catching and ignoring exceptions  

## References

- **Code Generation:** `.copilot/code-generation.instructions.md` - Code structure and naming
- **Unit Testing:** `.copilot/unit-test.instructions.md` - Test coverage and quality
- **Design Patterns:** `.copilot/design-patterns.instructions.md` - Pattern implementations
- **Repository Standards:** `.copilot/repo-standards.md`

## External Resources

- [.NET Compiler Platform Analyzers](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview)
- [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [EditorConfig Documentation](https://editorconfig.org/)
- [Nullable Reference Types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references)
- [SonarQube C# Rules](https://rules.sonarsource.com/csharp/)

---

**Version History:**
- **2.0.0 (2025-11-20):** Enhanced with comprehensive analyzer configuration, nullable guidance, CI/CD integration
- **1.0.0 (2025-11-20):** Initial version with basic quality rules
