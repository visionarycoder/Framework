---
applyTo: '**/*'
---

# GitHub Copilot Instructions for VisionaryCoder

**Version:** 3.0.0  
**Last Updated:** October 4, 2025  
**Compatibility:** C`#`, 12, .NET 8+, forward-compatible with .NET 10 LTS

## Changelog
### Version 3.0.0 (2025-10-04)
- **MAJOR RELEASE**: Comprehensive enterprise architecture guidelines added
- **DevOps & Bicep:** 4-tier deployment environments, Bicep-exclusive IaC, GitFlow pipelines
- **Integration & APIs:** Complete REST, gRPC, GraphQL, and messaging pattern guidance
- **Network & Security:** Zero Trust architecture, OpenID Connect, comprehensive application security
- **Resilience & Reliability:** Chaos engineering, load testing, circuit breakers, health checks
- **Observability:** Full-stack monitoring with structured logging, metrics, tracing, and AIOps
- **VBD Integration:** All patterns mapped to Volatility-Based Decomposition layers
- **Configuration Management:** Environment-specific AppSettings structure (Dev/Test/Stage/Prod)
- **Performance:** Benchmarking, profiling, and capacity planning guidelines

### Version 2.3.0 (2025-10-04)
- **MAJOR**: Added comprehensive **Security & Inter-Component Communication** section
- Enforced **NEVER use underscore prefixes** rule throughout naming conventions
- Added industry-standard security practices for authentication, authorization, and secret management
- Implemented contract-based architecture with mandatory `*.Contracts` projects
- Added performance-optimized communication protocols (in-process → gRPC → HTTP/2 → queues)
- Enhanced proxy pattern implementation with circuit breakers and monitoring
- Added strict layering rules and component isolation guidelines
- Integrated distributed caching, connection pooling, and resilience patterns

### Version 2.2.0 (2025-10-04)
- Added comprehensive **Microsoft Best Practices for Naming and Placement** section
- Enhanced C# guidelines with emphasis on following Microsoft's official conventions
- Included detailed naming standards for classes, records, methods, and libraries
- Added file organization and project structure recommendations
- Expanded documentation standards and method design principles

### Version 2.1.0 (2025-10-04)
- Added **Moq** as the preferred mocking framework for unit tests
- Included comprehensive Moq best practices and example patterns
- Enhanced unit testing guidelines with mocking strategies

### Version 2.0.0 (2025-10-04)
- **MAJOR**: Consolidated all domain-specific instructions into single file
- Added file pattern matching with `applyTo` directives
- Integrated architecture, Azure, C#, database, design patterns, Playwright, and UI guidelines
- Enhanced versioning and organization for better Copilot consumption

### Version 1.0.0 (2025-10-03)
- Initial version with C#, testing, and OpenTelemetry guidelines
- Basic technology preferences and framework selections

## Technology Preferences

- **Language:** Use `C#` for all code (client and server). Prefer the latest C# features.
- **Frameworks:**
  - **Web:** Use Blazor for web applications.
  - **Desktop:** Use Maui, WPF, or WinUI for Windows desktop apps.
  - **Backend:** Use ASP.NET Core for APIs.
  - **ORM:** Use Entity Framework Core.
- **Target Framework:** .NET 8 or latest stable.

## C# Development Guidelines
*Applies to: `**/*.cs`*

### Language & Framework Best Practices
- **Use the Latest Language Features:** Leverage `C#` 12+ features (records, pattern matching, file-scoped types, required members, primary constructors) for improved clarity and maintainability
- **Target Modern .NET:** Use .NET 8+ for all projects to benefit from performance, security, and language improvements
- **Follow Microsoft Best Practices:** Always prefer Microsoft's official naming conventions and placement guidelines when creating new records, classes, methods, and libraries
- **Naming Conventions:** Use PascalCase for public members, camelCase for local variables. **NEVER use underscore prefixes**
- **Code Quality:** Enable nullable reference types, implicit usings, analyzers, and code style enforcement
- **Immutability:** Prefer immutable types and readonly members where possible
- **Async/Await:** Use async/await for all I/O-bound and long-running operations
- **Error Handling:** Use exception filters and custom exception types for robust error management

## Microsoft Best Practices for Naming and Placement
*Applies to: `**/*.cs`, `**/src/**`*

### Naming Conventions
- **Classes:** Use PascalCase (e.g., `CustomerService`, `OrderProcessor`)
- **Records:** Use PascalCase (e.g., `CustomerData`, `OrderSummary`)
- **Interfaces:** Use PascalCase with 'I' prefix (e.g., `ICustomerService`, `IRepository<T>`)
- **Methods:** Use PascalCase with verb phrases (e.g., `GetCustomerById`, `ProcessOrderAsync`)
- **Properties:** Use PascalCase (e.g., `FirstName`, `IsActive`, `CreatedDate`)
- **Fields:** Use camelCase for private fields (e.g., `customerRepository`, `logger`). **NEVER use underscore prefixes**
- **Parameters:** Use camelCase (e.g., `customerId`, `orderData`)
- **Local Variables:** Use camelCase (e.g., `customerName`, `orderTotal`)
- **Constants:** Use PascalCase (e.g., `MaxRetryAttempts`, `DefaultTimeout`)
- **Enums:** Use PascalCase for enum and values (e.g., `OrderStatus.Pending`, `PaymentMethod.CreditCard`)

### File and Folder Organization
- **One Class Per File:** Each class should be in its own file with matching name
- **Namespace Alignment:** Folder structure should mirror namespace hierarchy
- **Project Structure:**
  - `Controllers/` - Web API controllers
  - `Services/` - Business logic services
  - `Models/` - Data transfer objects and view models
  - `Data/` - Entity Framework contexts and entities
  - `Repositories/` - Data access layer
  - `Extensions/` - Extension methods
  - `Helpers/` - Utility classes
  - `Constants/` - Application constants

### Library and Assembly Naming
- **Assembly Names:** Use company.product.component pattern (e.g., `VisionaryCoder.Core`, `VisionaryCoder.Data`)
- **Namespace Hierarchy:** Follow assembly name structure (e.g., `VisionaryCoder.Core.Services`)
- **Avoid Generic Names:** Use descriptive, domain-specific names over generic terms

### Method Design Principles
- **Single Responsibility:** Each method should have one clear purpose
- **Async Naming:** Append `Async` suffix to async methods (e.g., `GetDataAsync`)
- **Boolean Methods:** Use `Is`, `Has`, `Can`, or `Should` prefixes (e.g., `IsValid`, `HasPermission`)
- **Collection Methods:** Use clear action verbs (e.g., `AddItem`, `RemoveAll`, `FindByName`)

### Documentation Standards
- **XML Documentation:** Use `<summary>`, `<param>`, `<returns>` tags for public APIs
- **README Files:** Include clear documentation for each project/library
- **Code Comments:** Explain 'why' not 'what' - the code should be self-documenting

## Security & Inter-Component Communication
*Applies to: `**/*.cs`, `**/src/**`, `**/Contracts/**`*

### Security Best Practices
- **Authentication & Authorization:**
  - Use industry-standard protocols: OAuth 2.0, OpenID Connect, JWT
  - Implement role-based access control (RBAC) and attribute-based access control (ABAC)
  - Use Azure Active Directory, Auth0, or similar identity providers
  - Never store credentials in code or configuration files

- **Secret Management:**
  - Use Azure Key Vault, HashiCorp Vault, or AWS Secrets Manager for production
  - Use .NET Secret Manager (`dotnet user-secrets`) for local development only
  - Rotate secrets regularly with automated processes
  - Use managed identities when available (Azure, AWS IAM roles)
  - Encrypt secrets at rest and in transit

- **Communication Security:**
  - Always use TLS 1.2+ for external communication
  - Use mutual TLS (mTLS) for service-to-service communication
  - Implement certificate pinning for critical connections
  - Use API keys, bearer tokens, or client certificates for service authentication

### Inter-Component Communication Architecture
- **Contract-Based Design:**
  - Each component must expose a public `*.Contracts` project
  - Components can ONLY reference other components through their Contract projects
  - Contracts define interfaces, DTOs, and communication protocols only
  - Never reference implementation projects directly

- **Communication Protocols (Performance Priority):**
  1. **In-Process:** Direct method calls via dependency injection (fastest)
  2. **gRPC:** For high-performance service-to-service communication
  3. **HTTP/2:** For RESTful APIs with multiplexing support
  4. **Message Queues:** For asynchronous, decoupled communication
  5. **HTTP/1.1:** Only when legacy compatibility required

- **Proxy Pattern Implementation:**
  - Use proxy classes to decouple direct component dependencies
  - Implement circuit breakers and retry policies in proxies
  - Add telemetry, logging, and monitoring at proxy level
  - Support multiple communication protocols through proxy abstraction

### Layering Rules & Enforcement
- **Dependency Direction:** Dependencies must flow toward more stable layers
  - UI → Services → Business Logic → Data Access → Infrastructure
  - Higher layers can depend on lower layers, never the reverse
  - Use dependency inversion principle with interfaces

- **Layer Isolation:**
  - Each layer communicates only with adjacent layers
  - Cross-layer communication must go through defined contracts
  - Use mediator pattern for complex cross-layer operations
  - Implement architectural tests to enforce layering rules

- **Contract Project Structure:**
  ```
  Component.Contracts/
  ├── IComponentService.cs      // Service interfaces
  ├── Models/                   // Data transfer objects
  ├── Events/                   // Domain events
  └── Exceptions/               // Component-specific exceptions
  ```

- **Component Isolation:**
  - Components are self-contained with their own data stores
  - No direct database sharing between components
  - Use event-driven architecture for component coordination
  - Implement saga pattern for distributed transactions

### Performance & Resilience Patterns
- **Connection Management:**
  - Use connection pooling for all external services
  - Implement connection health checks and failover
  - Configure appropriate timeouts and retry policies

- **Caching Strategy:**
  - Use Redis for distributed caching between components
  - Implement cache-aside pattern for data consistency
  - Use in-memory caching for frequently accessed reference data

- **Monitoring & Observability:**
  - Implement distributed tracing across component boundaries
  - Use correlation IDs for request tracking
  - Monitor communication latency and error rates
  - Set up alerts for communication failures

## Integration & API Best Practices
*Applies to: `**/Controllers/**`, `**/APIs/**`, `**/Services/**`*

### API Architecture & VBD Integration
- **REST APIs:** Implement in Manager layer for workflow orchestration
- **gRPC Services:** Use for high-performance Engine-to-Engine communication
- **GraphQL:** Implement in Manager layer for complex data aggregation scenarios
- **Messaging:** Use for asynchronous Accessor-to-Manager communication

### REST API Standards
- **Resource Design:** Follow RESTful principles with clear resource hierarchies
- **HTTP Methods:** Use appropriate verbs (GET, POST, PUT, DELETE, PATCH)
- **Status Codes:** Implement comprehensive HTTP status code responses
- **Versioning:** Use header-based versioning (`Api-Version: 1.0`)
- **Documentation:** Use OpenAPI/Swagger with comprehensive examples
- **VBD Mapping:** Map REST endpoints to Manager layer operations

### gRPC Implementation
- **Service Definitions:** Define clear .proto contracts for inter-service communication
- **Performance:** Use gRPC for Engine-to-Engine high-throughput operations
- **Streaming:** Implement server/client streaming for real-time data flows
- **Error Handling:** Use gRPC status codes and detailed error messages
- **Load Balancing:** Implement client-side load balancing for Engine services

### GraphQL Architecture
- **Schema Design:** Create type-safe schemas with clear resolver patterns
- **Query Optimization:** Implement DataLoader patterns to prevent N+1 queries
- **Subscription:** Use for real-time updates in Manager layer workflows
- **Security:** Implement query complexity analysis and rate limiting
- **VBD Integration:** Map GraphQL resolvers to appropriate VBD layer operations

### Messaging Patterns
- **Event Sourcing:** Implement for Accessor layer data consistency
- **CQRS:** Separate command/query responsibilities across VBD layers
- **Pub/Sub:** Use for decoupled Manager-to-Manager communication
- **Message Queues:** Implement for reliable Accessor operations
- **Dead Letter Queues:** Handle failed message processing with retry policies

## Network & Security Architecture
*Applies to: `**/Infrastructure/**`, `**/Security/**`*

### Networking Best Practices
- **Zero Trust Architecture:** Verify every connection regardless of location
- **Network Segmentation:** Isolate VBD components in separate network segments
- **Private Endpoints:** Use for all Azure service connections
- **API Gateways:** Implement as entry points to Manager layer services
- **Load Balancers:** Distribute traffic across VBD component instances
- **CDN:** Use for static content delivery and edge caching

### Identity & Access Management
- **OpenID Connect:** Implement for federated identity across all environments
- **OAuth 2.0:** Use for API authorization with appropriate scopes
- **JWT Tokens:** Implement with proper validation and refresh mechanisms
- **Role-Based Access:** Map roles to VBD component access patterns
- **Attribute-Based Access:** Implement fine-grained permissions per operation
- **Multi-Factor Authentication:** Enforce for all administrative access

### Application Security
- **Input Validation:** Implement comprehensive validation in Manager layer
- **Output Encoding:** Sanitize all responses to prevent injection attacks
- **SQL Injection Prevention:** Use parameterized queries in Accessor layer
- **XSS Protection:** Implement Content Security Policy and input sanitization
- **CSRF Protection:** Use anti-forgery tokens for state-changing operations
- **Secrets Management:** Never store secrets in code; use Key Vault integration

## Resilience & Reliability
*Applies to: `**/*.cs`, `**/Services/**`*

### Resilience Patterns
- **Circuit Breakers:** Implement in proxy classes between VBD components
- **Retry Policies:** Use exponential backoff for Accessor layer operations
- **Bulkheads:** Isolate critical Manager operations from non-critical ones
- **Timeouts:** Set appropriate timeouts for each VBD layer interaction
- **Fallback Mechanisms:** Provide degraded functionality when services fail
- **Health Checks:** Implement comprehensive health monitoring per component

### Chaos Engineering
- **Failure Injection:** Test component failures in non-production environments
- **Service Degradation:** Validate fallback mechanisms across VBD layers
- **Network Partitioning:** Test Manager-Engine-Accessor communication failures
- **Load Testing:** Validate performance under stress for each component type
- **Disaster Recovery:** Test complete system recovery procedures
- **Game Days:** Regular chaos engineering exercises with team participation

### Performance & Benchmarking
- **Load Testing:** Use tools like NBomber, K6, or Artillery for comprehensive testing
- **Stress Testing:** Validate system behavior under extreme load conditions
- **Benchmarking:** Establish performance baselines for each VBD component
- **Profiling:** Regular performance profiling of critical code paths
- **Capacity Planning:** Monitor and predict scaling requirements per layer
- **Performance Budgets:** Set and enforce performance thresholds

## Observability & Monitoring
*Applies to: `**/*.cs`, `**/Logging/**`*

### Comprehensive Logging
- **Structured Logging:** Use consistent JSON format across all VBD components
- **Correlation IDs:** Track requests across Manager → Engine → Accessor flows
- **Log Levels:** Implement appropriate levels (Trace, Debug, Info, Warn, Error, Fatal)
- **Sensitive Data:** Never log passwords, tokens, or personal information
- **Log Aggregation:** Centralize logs from all environments and components
- **VBD Context:** Include component type (Manager/Engine/Accessor) in all logs

### Metrics & Monitoring
- **Business Metrics:** Track KPIs relevant to each Manager workflow
- **Technical Metrics:** Monitor performance, errors, and resource usage per layer
- **Custom Metrics:** Implement domain-specific metrics for Engine operations
- **Real-time Dashboards:** Create role-specific monitoring dashboards
- **Alerting:** Set up proactive alerts based on metric thresholds
- **SLA Monitoring:** Track service level objectives across component boundaries

### Distributed Tracing
- **OpenTelemetry:** Implement comprehensive tracing across all components
- **Trace Context:** Propagate trace context through VBD layer boundaries
- **Span Annotations:** Add meaningful annotations for business operations
- **Performance Analysis:** Use traces to identify bottlenecks in workflows
- **Error Correlation:** Link errors across distributed component calls
- **Dependency Mapping:** Visualize component relationships and dependencies

### APM & Anomaly Detection
- **Application Performance Monitoring:** Implement full-stack APM solutions
- **Baseline Establishment:** Create performance baselines for normal operations
- **Anomaly Detection:** Use machine learning for automated issue detection
- **Root Cause Analysis:** Implement tools for rapid issue diagnosis
- **Predictive Analytics:** Use historical data for capacity and failure prediction
- **AIOps Integration:** Leverage AI for operational insights and automation

## Database Guidelines
*Applies to: `**/*.cs`, `**/migrations/**`, `**/Data/**`*

### Development Environment
- **Local Development:** Use SQLite or SQL Server LocalDB for lightweight, local development
- **Containerized Development:** Use Docker for SQL Server, PostgreSQL instances
- **Configuration:** Store connection strings in `AppSettings.Development.json` or environment variables
- **Secrets Management:** Use .NET Secret Manager (`dotnet user-secrets`) to keep credentials secure
- **Schema Management:** Use EF Core migrations to sync local and production schemas

### Production Environment
- **Production Database:** Use SQL Server for production deployments
- **Cloud Deployments:** Prefer Azure SQL Database, Azure Cosmos DB for managed services
- **Security:** Store connection strings in Azure Key Vault, use managed identities
- **Performance:** Enable geo-redundancy, automated backups, monitor with Azure Monitor

## Testing Guidelines
*Applies to: `**/tests/**`, `**/*.test.cs`, `**/*.spec.cs`*

### Unit Testing
- **Framework:** Use **MSTest**, **xUnit**, or **VSTest** for unit tests
- **Assertions:** Use **FluentAssertions** for expressive, readable assertions
- **Mocking:** Use **Moq** for creating test doubles and mocks
- **Structure:** Place unit tests in `tests/UnitTests/{projectName}.UnitTests`
- **Isolation:** Write unit tests that are fast, reliable, and independent

#### Moq Best Practices
- **Mock Creation:** Use `Mock<T>` for interfaces and virtual methods
- **Setup Behavior:** Use `Setup()` for method calls, `SetupProperty()` for properties
- **Verification:** Use `Verify()` to assert method calls, `VerifyAll()` for comprehensive verification
- **Returns:** Use `Returns()` for simple values, `ReturnsAsync()` for async methods
- **Callbacks:** Use `Callback()` for complex setup or to capture parameters
- **Mock Behavior:** Use `MockBehavior.Strict` for strict mocks, `MockBehavior.Loose` for lenient mocks
- **Example Pattern:**
  ```csharp
  var mockRepository = new Mock<IRepository>();
  mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
              .ReturnsAsync(new Entity { Id = 1 });
  
  var service = new EntityService(mockRepository.Object);
  var result = await service.ProcessAsync(1);
  
  mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
  ```

### Integration Testing
- **Framework:** Use **xUnit**, **VSTest**, or **Playwright** for integration tests
- **ASP.NET Core:** Use `Microsoft.AspNetCore.Mvc.Testing` for web API testing
- **Entity Framework:** Use `Microsoft.EntityFrameworkCore.InMemory` for database testing
- **Structure:** Place integration tests in `tests/IntegrationTests/{projectName|Solution}.IntegrationTests`

### UI & End-to-End Testing
*Applies to: `**/*.spec.ts`, `**/e2e/**`, `**/playwright/**`*

#### Playwright Best Practices
- **Test Structure:** Organize by feature/workflow, use descriptive names and `describe` blocks
- **Performance:** Run tests in parallel, use built-in tracing and reporting
- **Selectors:** Prefer data-test attributes, avoid brittle CSS/XPath selectors
- **Assertions:** Verify UI state, network responses, and accessibility compliance
- **Environment:** Use clean, isolated environments with environment variables
- **Cross-Browser:** Test across Chromium, Firefox, and WebKit
- **CI Integration:** Include in CI pipelines, fail builds on test failures
- **Accessibility:** Use Playwright's accessibility snapshots and assertions

## Validation
- Use **FluentValidation** for model and business rule validation.
- Prefer FluentValidation over DataAnnotations for complex validation scenarios.

## Observability
- Use **OpenTelemetry** for distributed tracing, metrics, and logging.
- Integrate OpenTelemetry with ASP.NET Core and other services for end-to-end observability.

## Architecture Guidelines
*Applies to: `**/*.cs`, `**/src/**`*

### Volatility-Based Decomposition
- **Organize components by volatility:** Separate workflows (Managers), business logic (Engines), and data access (Accessors)
- **Component Relationships:**
  - **Clients → 1..* Managers:** Each client interacts with managers via Contract interfaces only
  - **Managers → 0..* Engines | Accessors:** Managers coordinate workflows through proxy components
  - **Engines → 0..* Accessors:** Engines perform business logic using accessor contract interfaces
  - **Accessors → 1..* Resources:** Accessors interact directly with data resources (databases, external services)
- **Communication Rules:** 
  - All communication must go through Contract interfaces and proxy implementations
  - Use fastest appropriate protocol: in-process DI → gRPC → HTTP/2 → message queues
  - Prevent direct sibling communication; Use parent component; If manager use message bus;
  - Implement circuit breakers and retry policies in all communications
- **Composition over Inheritance:** Compose behaviors to keep components focused and testable

### Cross-Cutting Concerns
- **Logging:** Use ApplicationInsights
- **Security:** Use ASP.NET Core Identity/OAuth
- **Error Handling:** Use middleware for global exception handling
- **Configuration:** Use `AppSettings.json` and environment variables
- **Caching:** Use Redis or `IMemoryCache`
- **Validation:** Use FluentValidation (prefer over DataAnnotations)
- **Localization:** Use resource files and localization middleware
- **Concurrency:** Use optimistic concurrency in EF Core
- **Auditing:** Implement audit logs
- **Transactions:** Use EF Core transactions
- **Observability:** Use OpenTelemetry for tracing and metrics

## Azure Development Guidelines
*Applies to: `**/azure/**`, `**/*.bicep`, `**/ARM/**`*

### Resource Management
- Use Azure Resource Manager (ARM) templates or **Bicep** for infrastructure as code
- Group related resources in resource groups for logical management

### Security Best Practices
- Use **managed identities** for secure service-to-service authentication
- Store secrets in **Azure Key Vault**; never hard-code credentials
- Enforce **role-based access control (RBAC)** for all resources
- Enable Azure Security Center and Defender for threat protection

### Networking & Scalability
- Use private endpoints and virtual networks to isolate resources
- Use Azure App Service, Azure Functions, or AKS for scalable compute
- Enable autoscaling and geo-redundancy for critical workloads

### DevOps & CI/CD Best Practices
- **Infrastructure as Code:** Use **Bicep** exclusively for all Azure deployments
- **Deployment Environments:** Implement 4-tier deployment structure:
  - **Development:** Local/sandbox environment for feature development
  - **Testing:** Automated testing and QA validation environment
  - **Staging:** Production-like environment for final validation
  - **Production:** Live production environment
- **Configuration Management:** Use environment-specific `AppSettings.json` files:
  - `AppSettings.json` (base configuration)
  - `AppSettings.Local.json` (local environment)
  - `AppSettings.Development.json` (development environment)
  - `AppSettings.Testing.json` (testing environment)
  - `AppSettings.Staging.json` (staging environment)
  - `AppSettings.Production.json` (production environment)
- **Pipeline Strategy:** Implement GitFlow with automated deployments
- **Deployment Patterns:** Use blue-green or canary deployments for zero-downtime releases
- **VBD Pipeline Organization:** Structure pipelines by volatility (Manager → Engine → Accessor deployment order)

### Bicep Deployment Standards
- **Modular Design:** Create reusable Bicep modules for common resources
- **Parameter Files:** Use environment-specific parameter files for each deployment tier
- **Resource Naming:** Follow consistent naming conventions across all environments
- **Security:** Use Key Vault references for secrets in Bicep templates
- **Validation:** Implement Bicep linting and What-If deployments in pipelines
- **Version Control:** Tag and version Bicep templates with semantic versioning

### Monitoring & Observability
- **Logging:** Implement structured logging with correlation IDs across VBD layers
- **Metrics:** Collect performance metrics for Managers, Engines, and Accessors separately
- **Tracing:** Use distributed tracing to track requests across component boundaries
- **APM:** Implement Application Performance Monitoring with anomaly detection
- **Dashboards:** Create layer-specific monitoring dashboards (Manager/Engine/Accessor views)
- Integrate with **Azure Monitor**, **Application Insights**, and **Log Analytics**

### Cost & Compliance
- Use Azure Cost Management to monitor and optimize spending
- Use Azure Policy to enforce compliance and governance
- Implement cost allocation tags aligned with VBD component structure

## UI Development Guidelines
*Applies to: `**/*.razor`, `**/*.html`, `**/*.css`, `**/*.scss`*

### Design System
- Use **IBM Carbon Design System** for all UI components and styling
- Maintain consistent spacing, typography, and iconography with Carbon standards

### Performance & Optimization
- Minimize DOM nodes and component nesting
- Lazy load heavy resources and components
- Optimize images and assets for web delivery
- Use memoization and virtualization to avoid unnecessary re-renders

### Accessibility & Standards
- Follow **WCAG guidelines** and Carbon accessibility standards
- Ensure keyboard navigation and screen reader support
- Use semantic HTML and ARIA attributes

### Responsive Design
- Design **mobile-first**, then scale up for larger screens
- Use Carbon grid and layout utilities for adaptive layouts

### User Experience
- Prioritize essential content and actions (minimalism)
- Provide immediate feedback for user actions
- Use Carbon skeletons and loading indicators for async operations
- Avoid blocking UI; use non-blocking notifications and dialogs

## Design Patterns Guidelines
*Applies to: `**/*.cs`*

### Pattern Implementation Standards
- Generate **modern C# 12/.NET 8+** syntax, forward-compatible with .NET 10 LTS
- Create **reproducible and isolated** examples with minimal boilerplate
- Provide **educational format** with clear separation of pattern intent, structure, and usage
- Follow **Gang of Four (GoF) design pattern principles** and classifications where applicable

### Gang of Four Pattern Categories & Guidelines
- **Creational Patterns:** Factory Method, Abstract Factory, Builder, Prototype, Singleton
  - Show DI-friendly implementations (e.g., `IServiceCollection` integration)
- **Structural Patterns:** Adapter, Bridge, Composite, Decorator, Facade, Flyweight, Proxy
  - Emphasize composition over inheritance, use `record` types for immutable values
- **Behavioral Patterns:** Chain of Responsibility, Command, Interpreter, Iterator, Mediator, Memento, Observer, State, Strategy, Template Method, Visitor
  - Show real-world .NET scenarios (e.g., `MediatR` for Mediator, `IAsyncEnumerable` for Iterator)

### Enterprise & Modern Patterns
- **Unit of Work:** Implement for transaction management and data consistency
- **Saga:** Use for long-running transactions with compensating actions
- **Repository:** Use with Entity Framework Core for data access abstraction
- **CQRS:** Separate read and write models for complex domains

### Implementation Requirements
- Always explain **when to use** a pattern, not just how
- Prefer **interfaces and records** where appropriate
- Use **async/await** for concurrency-related patterns
- Show **unit-testable examples** (xUnit style)
- Avoid outdated constructs (e.g., `ArrayList`, `Task.Result`)

### Example Output Format
1. **Intent:** One-sentence purpose
2. **Structure:** Key classes/interfaces
3. **Code:** Minimal, compilable C# example
4. **Usage:** Short demo snippet
5. **Notes:** Pitfalls, modern alternatives, or implementing libraries

---

*For Azure-specific requests, follow Azure best practices and use appropriate tools.*
