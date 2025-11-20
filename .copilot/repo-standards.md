---
applyTo: "**/*"
version: 2.0.0
lastUpdated: 2025-11-20
scope: repository-standards
---

# Copilot Instructions: Repository Standards

## Purpose

Ensure all generated artifacts (code, documentation, automation) follow repository hygiene, structural conventions, and collaboration best practices for the VisionaryCoder Framework project.

---

## Core Repository Hygiene

### Respect Configuration Files
- **`.gitignore`:** Never commit build artifacts, user-specific files, or secrets
- **`.editorconfig`:** Follow coding style and formatting rules
- **`.copilotignore`:** Exclude files from Copilot suggestions as configured

### Commit Message Conventions
Use **Conventional Commits** format for clear, parsable commit history:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `refactor:` Code refactoring without functional changes
- `test:` Adding or updating tests
- `chore:` Maintenance tasks (dependencies, build config)
- `perf:` Performance improvements
- `style:` Code style changes (formatting, whitespace)
- `ci:` CI/CD pipeline changes
- `build:` Build system changes

**Examples:**
```bash
feat(orders): add order validation service
fix(auth): resolve token refresh race condition
docs: update API documentation with examples
refactor(data): extract repository pattern
test(customers): add integration tests for customer service
chore(deps): update Entity Framework to 8.0.11
perf(cache): optimize redis connection pooling
```

### Pull Request Guidelines
- **Small, Focused PRs:** Single concern per PR; avoid mixing refactors with features
- **Descriptive Titles:** Use conventional commit format
- **Linked Issues:** Reference related issues with `Closes #123` or `Relates to #456`
- **ADR References:** Link architectural decision records for significant changes
- **Self-Review:** Review your own PR before requesting review from others

---

## Repository Structure

### Standard Directory Layout
```
/
├── .copilot/                   # Copilot instruction files
│   ├── README.md              # This documentation
│   ├── copilot-instructions.md
│   ├── code-generation.instructions.md
│   ├── unit-test.instructions.md
│   ├── code-quality.instructions.md
│   ├── design-patterns.instructions.md
│   └── repo-standards.md
├── .github/                    # GitHub-specific configuration
│   ├── workflows/             # GitHub Actions workflows
│   ├── ISSUE_TEMPLATE/        # Issue templates
│   ├── PULL_REQUEST_TEMPLATE.md
│   └── copilot-instructions.md # Enterprise baseline
├── docs/                       # Documentation
│   ├── adr/                   # Architectural Decision Records
│   │   ├── index.md
│   │   ├── adr-0001-*.md
│   │   └── template.md
│   ├── api/                   # API documentation
│   └── guides/                # User guides and tutorials
├── src/                        # Source code
│   ├── ProjectName/
│   │   ├── Services/
│   │   ├── Models/
│   │   ├── Controllers/
│   │   └── ProjectName.csproj
│   └── ProjectName.Contracts/  # Public contracts
├── tests/                      # Test projects
│   ├── ProjectName.UnitTests/
│   └── ProjectName.IntegrationTests/
├── .editorconfig              # Editor configuration
├── .gitignore                 # Git ignore rules
├── Directory.Build.props      # Shared MSBuild properties
├── Directory.Packages.props   # Centralized package management
├── README.md                  # Project overview
└── VisionaryCoder.Framework.sln
```

### Namespace and Folder Alignment
- Source folder structure **must mirror** namespace hierarchy
- Example: `src/Services/Customers/` → `VisionaryCoder.Framework.Services.Customers`
- One public type per file; file name matches type name

---

## Documentation Standards

### README Files
Every project/module should have a `README.md` containing:

1. **Purpose:** What the component does
2. **Usage:** How to use it (with examples)
3. **Extension Points:** How to extend or customize
4. **Dependencies:** Required packages and services
5. **Configuration:** Environment variables, settings

**Template:**
```markdown
# Project Name

Brief description of the project.

## Purpose
Detailed explanation of what this project does and why it exists.

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (or Docker)

### Installation
```bash
dotnet restore
dotnet build
```

### Usage
```csharp
var service = new CustomerService(repository, logger);
var customer = await service.GetCustomerAsync(id);
```

## Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."

## Contributing
See [CONTRIBUTING.md](CONTRIBUTING.md)

## License
[License Type]
```

### Architectural Decision Records (ADRs)
Capture significant architectural decisions in `docs/adr/`:

**ADR Template:**
```markdown
# ADR-XXXX: Title

**Status:** Proposed | Accepted | Deprecated | Superseded by ADR-YYYY

**Context:**
What is the issue we're facing?

**Decision:**
What decision did we make?

**Consequences:**
What are the positive and negative consequences?

**Alternatives Considered:**
What other options did we evaluate?

**References:**
- Related ADRs
- External resources
```

### Code Documentation
- **XML Documentation:** For public APIs (see `code-generation.instructions.md`)
- **Inline Comments:** Explain "why" not "what"
- **Architecture Diagrams:** Use Mermaid or PlantUML in markdown
- **CHANGELOG.md:** Track notable changes between versions

---

## Testing & Quality Requirements

### Test Coverage
- **New Features:** Include unit tests for all new logic
- **Critical Paths:** Add integration tests for business-critical workflows
- **Bug Fixes:** Include regression tests to prevent reoccurrence
- **Coverage Monitoring:** Track coverage trends, watch for regressions

### Quality Gates
Before merging:
1. ✅ All tests pass locally
2. ✅ No analyzer warnings (or justified suppressions)
3. ✅ Code coverage stable or improved
4. ✅ Documentation updated for public API changes
5. ✅ ADR linked if architectural change
6. ✅ No TODO comments or dead code
7. ✅ Security vulnerabilities addressed

### Automated Quality Checks
```bash
# Run full quality check suite
dotnet build -c Release
dotnet test --collect:"XPlat Code Coverage"
dotnet format --verify-no-changes
```

---

## CI/CD Pipeline Standards

### Branch Strategy
- **`main`:** Protected branch, production-ready code
- **`develop`:** Integration branch for features (if using GitFlow)
- **Feature Branches:** `feat/<description>` or `feature/<ticket-id>-<description>`
- **Fix Branches:** `fix/<description>` or `bugfix/<ticket-id>-<description>`
- **Documentation:** `docs/<description>`

### Branch Protection Rules
- Require pull request reviews (minimum 1 reviewer)
- Require status checks to pass before merging
- Require up-to-date branches
- No force pushes to protected branches
- No direct commits to `main`

### Pipeline Stages
1. **Build:** Compile code with full analysis
2. **Test:** Run unit and integration tests
3. **Analyze:** Generate SARIF diagnostics, code coverage
4. **Package:** Create NuGet packages or deployment artifacts
5. **Deploy:** Deploy to appropriate environment (Dev/Test/Staging/Prod)

### GitHub Actions Example
```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build -c Release --no-restore /p:ErrorLog=./code-analysis.sarif
      
      - name: Test
        run: dotnet test -c Release --no-build --collect:"XPlat Code Coverage"
      
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

---

## Security Best Practices

### Secret Management
- **Never commit secrets:** Use `.gitignore` for local secrets
- **Production Secrets:** Store in Azure Key Vault, GitHub Secrets, or equivalent
- **Local Development:** Use .NET User Secrets (`dotnet user-secrets`)
- **CI/CD Secrets:** Use secure pipeline variable storage

### Vulnerability Scanning
- Enable Dependabot for automated dependency updates
- Run security scanners in CI pipeline
- Review and address security alerts promptly
- Keep dependencies up to date

### Code Security
- Validate all inputs (see `code-generation.instructions.md`)
- Use parameterized queries (prevent SQL injection)
- Sanitize outputs (prevent XSS)
- Implement proper authentication and authorization
- Follow OWASP Top 10 guidelines

---

## Collaboration Practices

### Code Review Guidelines
- **Be Respectful:** Provide constructive feedback
- **Be Specific:** Reference line numbers and explain reasoning
- **Be Timely:** Review within 24-48 hours
- **Check Understanding:** Ask questions if intent is unclear
- **Approve or Request Changes:** Don't leave reviews pending

### Pair Programming
- Encourage pairing for complex domain changes
- Use for knowledge transfer
- Rotate pairs regularly to spread knowledge

### Architectural Changes
- Require ADR for significant architectural decisions
- Involve relevant stakeholders early
- Require reviewer with domain expertise
- Consider impact on existing systems

---

## Anti-Patterns (Always Reject)

❌ **Large Unreviewed Dumps:** Break into small, reviewable PRs  
❌ **Merging Failing Builds:** All checks must pass  
❌ **Copy-Pasting Without Context:** Document sources and rationale  
❌ **Hidden Coupling:** Avoid implicit order dependencies  
❌ **Committing Secrets:** Use secret management tools  
❌ **Force Pushing to Main:** Never force push to protected branches  
❌ **Bypassing Reviews:** All code must be reviewed  
❌ **Ignoring Warnings:** Address or justify all warnings  

---

## Pre-Merge Checklist

Before creating a pull request:

1. ✅ **Code Quality**
   - [ ] Code builds without errors or warnings
   - [ ] All tests pass locally
   - [ ] Code formatted with `dotnet format`
   - [ ] No analyzer warnings (or justified suppressions)

2. ✅ **Testing**
   - [ ] Unit tests added for new functionality
   - [ ] Integration tests added for critical paths
   - [ ] Coverage stable or improved (no regressions)

3. ✅ **Documentation**
   - [ ] XML documentation added for public APIs
   - [ ] README updated if behavior changes
   - [ ] ADR created if architectural change
   - [ ] CHANGELOG updated (if applicable)

4. ✅ **Code Review**
   - [ ] Self-reviewed changes
   - [ ] No debug code or TODOs lingering
   - [ ] No commented-out code blocks
   - [ ] Commit messages follow conventions

5. ✅ **Security**
   - [ ] No secrets or credentials committed
   - [ ] Input validation implemented
   - [ ] Security vulnerabilities addressed
   - [ ] Dependencies up to date

---

## Dependency Management

### Centralized Package Management
Use `Directory.Packages.props` for centralized version management:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
    <PackageVersion Include="FluentAssertions" Version="7.0.0" />
    <PackageVersion Include="Moq" Version="4.20.72" />
  </ItemGroup>
</Project>
```

### Update Strategy
- Review dependency updates weekly
- Test thoroughly before updating major versions
- Use Dependabot for automated security updates
- Document breaking changes in CHANGELOG

---

## Performance & Monitoring

### Performance Testing
- Benchmark critical code paths
- Profile before optimizing
- Set performance budgets
- Monitor performance trends

### Logging & Telemetry
- Use structured logging (see enterprise guidelines)
- Include correlation IDs for distributed tracing
- Monitor error rates and performance metrics
- Set up alerts for anomalies

---

## References

### Internal Documentation
- **Code Generation:** `.copilot/code-generation.instructions.md`
- **Unit Testing:** `.copilot/unit-test.instructions.md`
- **Code Quality:** `.copilot/code-quality.instructions.md`
- **Design Patterns:** `.copilot/design-patterns.instructions.md`
- **Enterprise Baseline:** `.github/copilot-instructions.md`

### External Standards
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Keep a Changelog](https://keepachangelog.com/)
- [Semantic Versioning](https://semver.org/)
- [ADR Template](https://github.com/joelparkerhenderson/architecture-decision-record)
- [GitHub Flow](https://guides.github.com/introduction/flow/)

---

**Version History:**
- **2.0.0 (2025-11-20):** Comprehensive repository standards with CI/CD, security, and collaboration practices
- **1.0.0 (2025-11-20):** Initial version with basic hygiene and structure
