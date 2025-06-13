# GitHub Copilot Instructions for Blazor Web Applications and Desktop Development

## Technology Preferences

### Frameworks & Languages
- **Frontend:** Use Blazor for web apps.
- **Desktop:** Use Maui, WPF or WinUI for Windows desktop apps.
- **Backend:** Use ASP.NET Core for APIs.
- **Database:** Use Entity Framework Core for ORM.
- **Language:** Use C# for all code (client and server).  Use the latest C# features.
- **Testing:** Use xUnit, VSTest, Playwright for unit and integration tests.
- **Target:** .NET 8 or latest stable.

### Database Preferences
- **Development/Testing:** Use SQLite.
- **Production:** Use SQL Server.
- Store connection strings in `AppSettings.json`.

### Unit Testing
- Use xUnit, VSTest, or Playwright for unit tests.
- Place tests in `tests/UnitTests/{projectName}.UnitTests`.

### Integration Testing
- Use xUnit, VSTest, or Playwright for integration tests.
- Use `Microsoft.AspNetCore.Mvc.Testing` for ASP.NET Core integration tests.
- Use in-memory databases for testing purposes.
- Use `Microsoft.EntityFrameworkCore.InMemory` for EF Core integration tests.
- Place tests in `tests/IntegrationTests/({projectName}|Solution).IntgrationTests`.

## Architecture Patterns

### Volatility-Based Decomposition
- **Manager:** Workflow activities (called by clients).
- **Engine:** Business logic, aggregations, transformations, etc. (called by managers).
- **Access:** Data persistence (called by managers/engines).

**Communication Rules:**
- Clients → Managers
- Managers → Engines, Accessors
- Engines → Accessors
- Accessors → Data only
- No communication between sibling components: 
  - Clients do not call other clients.
  - Managers do not call other managers.
  - Engines do not call each other engines.
  - Engines do not call managers.
  - Accessors do not call each other accessors.
  - Accessors do not call managers or engines.
 - When a manager needs to communicatw with another manager, it should leverage a message bus.

### Cross-Cutting Concerns
- **Logging:** Use ApplicationInsights.
- **Security:** Use ASP.NET Core Identity/OAuth.
- **Error Handling:** Use middleware for global exception handling.
- **Configuration:** Use `AppSettings.json` and environment variables.  
- **Caching:** Use Redis or `IMemoryCache`.
- **Validation:** Use DataAnnotations/FluentValidation.
- **Localization:** Use resource files and localization middleware.
- **Concurrency:** Use optimistic concurrency in EF Core.
- **Auditing:** Implement audit logs.
- **Transactions:** Use EF Core transactions.

### Patterns

#### Unit of Work
- Implement Unit of Work for transaction management and data consistency.

#### Saga
- Use Saga pattern for long-running transactions with compensating actions.

---

*For Azure-specific requests, follow Azure best practices and use appropriate tools.*
