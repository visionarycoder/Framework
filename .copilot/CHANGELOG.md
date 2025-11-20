# Changelog

All notable changes to the Copilot instruction files will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2025-11-20

### Added
- **README.md** - Comprehensive overview of instruction files and usage
- **QUICK_REFERENCE.md** - Fast lookup guide for common scenarios
- **CHANGELOG.md** - Version tracking for instruction files
- Enhanced `applyTo` patterns with `excludePatterns` for better scoping
- Detailed examples for all design patterns (Strategy, Repository, Decorator, Observer)
- Complete EditorConfig configuration in code-quality instructions
- Advanced Moq patterns and examples in unit-test instructions
- Comprehensive CI/CD pipeline examples in repo-standards
- Security best practices across all instruction files
- Performance considerations and benchmarking guidelines
- External resource links to Microsoft documentation and industry standards

### Changed
- **copilot-instructions.md** - Enhanced with detailed precedence rules and cross-references
- **code-generation.instructions.md** - Expanded with complete naming conventions, error handling patterns, and dependency injection guidelines
- **unit-test.instructions.md** - Added advanced testing patterns, deterministic testing strategies, and theory/data-driven test examples
- **code-quality.instructions.md** - Enhanced with comprehensive analyzer configuration, nullable reference type guidance, and SARIF export instructions
- **design-patterns.instructions.md** - Complete rewrite with modern C# 12 examples, testing guidance, and pattern selection guide
- **repo-standards.md** - Expanded with branch strategies, security practices, and pre-merge checklists
- All files updated to version 2.0.0 with consistent formatting and structure
- Standardized frontmatter with `applyTo`, `version`, `lastUpdated`, and `scope` fields
- Removed `.github/copilot-instructions.md` from `.copilotignore` to ensure availability

### Fixed
- Inconsistent naming conventions across files
- Missing industry best practices
- Unclear precedence order for instruction files
- Incomplete applyTo patterns
- Missing anti-patterns section in several files

### Removed
- Redundant content between instruction files
- Outdated C# constructs in examples
- Conflicting guidance between files

## [1.1.0] - 2025-11-20

### Changed
- Updated code-generation with async patterns
- Enhanced unit-test with Moq examples
- Improved copilot-instructions with domain index

## [1.0.0] - 2025-11-19

### Added
- Initial instruction files structure
- Basic code generation rules
- Unit testing guidelines
- Code quality standards
- Design patterns examples
- Repository standards

---

**Versioning Guidelines:**
- **Major (X.0.0):** Breaking changes, paradigm shifts
- **Minor (X.Y.0):** New rules, substantial changes
- **Patch (X.Y.Z):** Clarifications, minor improvements
