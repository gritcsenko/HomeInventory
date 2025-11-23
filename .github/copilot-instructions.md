# GitHub Copilot Instructions for HomeInventory

## Project Overview

HomeInventory is a home inventory management system built with .NET 10.0, following Clean Architecture principles with a modular design pattern. The application uses ASP.NET Core for the API layer and implements Domain-Driven Design (DDD) patterns.

## Architecture & Structure

### Project Organization

The solution follows a **vertical slice/modular architecture** with clear separation of concerns:

- **HomeInventory.Api** - Entry point and API configuration
- **HomeInventory.Web[.Module]** - Carter-based HTTP endpoints (minimal APIs)
- **HomeInventory.Application[.Module]** - Application logic and use cases
- **HomeInventory.Application[.Module].Interfaces** - Public contracts for commands/queries
- **HomeInventory.Contracts[.Module]** - DTOs and request/response models
- **HomeInventory.Contracts[.Module].Validators** - FluentValidation validators for contracts
- **HomeInventory.Domain[.Module]** - Domain entities, aggregates, value objects, and domain events
- **HomeInventory.Infrastructure[.Module]** - Data access and external services
- **HomeInventory.Infrastructure.Framework** - Shared infrastructure concerns
- **HomeInventory.Core** - Shared primitives and utilities
- **HomeInventory.Modules** - Module registration and orchestration
- **HomeInventory.Tests[.Type]** - Testing projects (Unit, Integration, Acceptance)

### Key Architectural Patterns

1. **Modular Monolith**: Each feature module (e.g., UserManagement) has its own Application, Domain, Contracts, Infrastructure, and Web layers
2. **CQRS**: Command Query Responsibility Segregation for operations
3. **Domain-Driven Design**: Aggregates, Entities, Value Objects, Domain Events
4. **Vertical Slice Architecture**: Features are organized by business capability
5. **Clean Architecture**: Dependency rule - dependencies point inward toward domain

## Technology Stack

### Core Technologies
- **.NET 10.0** (see `global.json` for specific SDK version)
- **ASP.NET** for Web API
- **Carter** for minimal API endpoints
- **Entity Framework Core** for data access
- **Serilog** for structured logging

### Key Libraries
- **LanguageExt** - Functional programming primitives
- **FluentValidation** - Request validation
- **Riok.Mapperly** - Compile-time object mapping
- **Ardalis.Specification** - Repository pattern with specifications
- **BCrypt.Net-Next** - Password hashing
- **Swashbuckle** - OpenAPI/Swagger documentation
- **Scrutor** - Assembly scanning and decoration
- **Ulid** - Unique identifiers
- **System.IdentityModel.Tokens.Jwt** - JWT authentication
- **Microsoft.Extensions.Options.DataAnnotations** - Options validation
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **Microsoft.Extensions.Diagnostics.HealthChecks** - Health check infrastructure

### Testing
- **xUnit** - Testing framework
- **Reqnroll** - BDD/Acceptance testing (SpecFlow successor)
- **Expressium.LivingDoc.ReqnrollPlugin** - Living documentation
- **NSubstitute** - Mocking framework
- **AutoFixture** - Test data generation
- **AwesomeAssertions** - Fluent assertions
- **LanguageExt.UnitTesting** - Testing helpers for LanguageExt types
- **TngTech.ArchUnitNET** & **NetArchTest.Rules** - Architecture testing

## Coding Standards & Conventions

### Naming Conventions

1. **Namespaces**: Follow folder structure - `HomeInventory.[Layer][.Module][.SubFolder]`
2. **Files**: One type per file, file name matches type name
3. **Projects**: 
   - Feature modules: `HomeInventory.[Layer].[ModuleName]`
   - Framework/shared: `HomeInventory.[Layer].Framework`
4. **Private/Internal Fields**: Use underscore prefix with camelCase - `_fieldName`
5. **Interfaces**: Start with 'I' prefix - `IUserRepository`
6. **Type Parameters**: Use 'T' prefix - `TEntity`, `TIdentity`

### Code Style

1. **Use file-scoped namespaces** 
2. **Use primary constructors** where appropriate
3. **Prefer `using` declarations** over `using` statements
4. **Use implicit usings** - defined in `ImplicitUsings.cs` files
5. **Functional programming**: Leverage LanguageExt for functional patterns (Option, Either, Try, etc.)
6. **Immutability**: Prefer immutable data structures and records
7. **Prefer `extension` keyword** for static helper methods
8. **Expression-bodied members** for simple getters and methods
9. **Pattern matching** over traditional type checks and casts
10. **Static local functions** and **static anonymous functions** where possible

### Architecture Rules

1. **Modules are independent**: Each module should be self-contained
2. **Module registration**: Use `[Module]Module.cs` classes implementing module interfaces
3. **Dependency injection**: Register services via module configuration
4. **No circular dependencies** between modules
5. **Domain layer has no external dependencies** (except LanguageExt)
6. **Application layer depends only on Domain**
7. **Infrastructure implements interfaces from Application**

### Endpoint Development (Carter)

When creating new endpoints, inherit from `ApiCarterModule` (not the base `CarterModule`):

```csharp
namespace HomeInventory.Web.[ModuleName];

public sealed class [Feature]CarterModule(
    IScopeAccessor scopeAccessor, 
    IProblemDetailsFactory problemDetailsFactory,
    ContractsMapper mapper) 
    : ApiCarterModule
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;
    private readonly ContractsMapper _mapper = mapper;

    protected override string PathPrefix => "/api/[resource]";

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("[OperationName]")
            .WithValidationOf<[RequestType]>()
            .Produces<[ResponseType]>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<Results<Ok<[Response]>, ProblemHttpResult>> HandleAsync(
        [FromBody] [RequestType] request,
        [FromServices] I[Service] service,
        [FromServices] I[Repository] repository,
        [FromServices] IUnitOfWork unitOfWork,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        // Set scoped context for use in service methods
        using var scopes = new CompositeDisposable(
            _scopeAccessor.GetScope<I[Repository]>().Set(repository),
            _scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

        // Map request to command and call service
        var command = _mapper.ToCommand(request);
        var result = await service.[MethodName]Async(command, cancellationToken);
        
        // Handle result with pattern matching
        return result.Match(
            error => _problemDetailsFactory.CreateProblemResult(error, context.TraceIdentifier),
            () => TypedResults.Ok(new [Response]()));
    }
}
```

**Important Endpoint Conventions:**
- Use `ApiCarterModule` base class for automatic API versioning
- Override `PathPrefix` property to specify the base path (required)
- Inject `IScopeAccessor` for scoped dependency management
- Inject `IProblemDetailsFactory` for standardized error responses
- Inject `ContractsMapper` for mapping between DTOs and commands/queries
- Inject application services (e.g., `IUserService`) directly via `[FromServices]`
- Use `WithValidationOf<T>()` (not `WithValidation<T>()`) for validation
- Return typed results: `Results<Ok<TResponse>, ProblemHttpResult>`
- Always include `CancellationToken` parameter with `default` value
- Set scoped context before calling service methods
- Use mapper to convert requests to commands/queries

### Domain Development

1. **Entities**: Inherit from base entity classes in `HomeInventory.Domain.Primitives`
2. **Value Objects**: Use records or LanguageExt value types
3. **Domain Events**: Implement domain event notifications via `IDomainEventNotification`
4. **Aggregates**: Define aggregate roots in `Aggregates/` folder
5. **Domain Errors**: Define errors in `Errors/` folder

### Persistence & Data Access

1. **Entity Framework Core**: Use EF Core 10.0 for data access
2. **Repository Pattern**: Use Ardalis.Specification for repository pattern
   - Define specifications in `HomeInventory.Infrastructure.[Module]`
   - Inherit from `IRepository<TEntity>` or `IReadOnlyRepository<TEntity>`
3. **Unit of Work**: 
   - `IUnitOfWork` is implemented by `DatabaseContext`
   - Injected via `IScopeAccessor` at endpoint level
4. **Database Context**: 
   - One `DatabaseContext` per application
   - Module-specific configurations via `IDatabaseConfigurationApplier`
5. **Interceptors**: Domain events published via `PublishDomainEventsInterceptor`
6. **Auditing**: 
   - Use `ICreationAuditableEntity` for creation tracking
   - Use `IModificationAuditableEntity` for modification tracking
   - Timestamp from injected `TimeProvider`

### Application Layer (CQRS)

The application layer implements CQRS with the following messaging patterns:

**Commands** (write operations):
```csharp
public record RegisterCommand(
    string Email,
    string Name,
    string Password) : ICommand; // Returns Option<Error>
```

**Queries** (read operations):
```csharp
public record GetUserByEmailQuery(string Email) : IQuery<User>; // Returns Option<User>
```

**Key CQRS Conventions:**
- Commands return `Option<Error>` (success = None, failure = Some(error))
- Queries return `IQueryResult<T>` which wraps validation results
- Commands and queries are immutable records
- Define in `HomeInventory.Application.[Module].Interfaces`
- Service implementations live in `HomeInventory.Application.[Module]`

### Application Services Pattern

The solution uses a **direct service pattern** (not MediatR) where application services implement command and query methods:

**Service Interface:**
```csharp
namespace HomeInventory.Application.[Module].Interfaces;

public interface I[Module]Service
{
    // Commands return Option<Error>
    Task<Option<Error>> [CommandName]Async([Command] command, CancellationToken cancellationToken = default);
    
    // Queries return IQueryResult<T>
    Task<IQueryResult<[Result]>> [QueryName]Async([Query] query, CancellationToken cancellationToken = default);
}
```

**Service Implementation:**
```csharp
namespace HomeInventory.Application.[Module];

internal sealed class [Module]Service(
    IScopeAccessor scopeAccessor,
    // ... other dependencies
    ) : I[Module]Service
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task<Option<Error>> [CommandName]Async([Command] command, CancellationToken cancellationToken = default)
    {
        // Retrieve scoped dependencies set in endpoint
        var repository = _scopeAccessor.GetRequiredContext<I[Repository]>();
        var unitOfWork = _scopeAccessor.GetRequiredContext<IUnitOfWork>();
        
        // Implementation - business logic
        // ...
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Option<Error>.None; // Success
    }

    public async Task<IQueryResult<[Result]>> [QueryName]Async([Query] query, CancellationToken cancellationToken = default)
    {
        // Retrieve scoped dependencies set in endpoint
        var repository = _scopeAccessor.GetRequiredContext<I[Repository]>();
        
        // Implementation
        var result = await repository.GetByIdAsync(query.Id, cancellationToken);
        var validationResult = result
            .Map(entity => /* map to result */)
            .ErrorIfNone(() => new NotFoundError("Not found"));
            
        return QueryResult.From(validationResult);
    }
}
```

**Service Pattern Conventions:**
- Services are `internal sealed` classes implementing public interfaces
- Service interfaces defined in `HomeInventory.Application.[Module].Interfaces`
- Service implementations in `HomeInventory.Application.[Module]`
- Use `IScopeAccessor` to retrieve scoped dependencies (set in endpoints)
- Commands return `Option<Error>` - None for success, Some(error) for failure
- Queries return `IQueryResult<T>` wrapping `Validation<Error, T>`
- Service methods named as `[OperationName]Async` with async suffix
- Always include `CancellationToken` parameter with `default` value
- Use LanguageExt functional patterns (Option, Validation, Map, Bind, etc.)

### Validation

1. **Contract validation**: Use FluentValidation in `HomeInventory.Contracts.[Module].Validators`
2. **Validation registration**: Automatically registered via Scrutor scanning
3. **Endpoint validation**: Apply via `.WithValidationOf<T>()` extension method

### Response Mapping (Mapperly)

Use Mapperly for compile-time DTO mapping:

```csharp
namespace HomeInventory.Contracts.[Module];

[Mapper]
public static partial class [Module]Mapper
{
    // Domain to DTO
    public static partial [Response] ToResponse(this [Entity] entity);
    
    // DTO to Domain (if needed)
    public static partial [Entity] ToDomain(this [Request] request);
    
    // Collection mapping
    public static partial IEnumerable<[Response]> ToResponses(
        this IEnumerable<[Entity]> entities);
    
    // With custom mapping logic
    [MapProperty(nameof([Entity].Property), nameof([Response].MappedProperty))]
    public static partial [Response] ToResponseWithCustomMapping(this [Entity] entity);
}
```

**Mapper Conventions:**
- Place mappers in `HomeInventory.Contracts.[Module]` namespace
- Use `[Mapper]` attribute on static partial classes
- Methods must be `public static partial`
- Use extension methods for better fluent API
- Name mapper classes as `[Module]Mapper` (e.g., `UserManagementMapper`)
- Mapperly generates implementation at compile-time

### API Versioning

API versioning is built into the `ApiCarterModule` base class:

```csharp
public sealed class MyCarterModule : ApiCarterModule
{
    protected override string PathPrefix => "/api/resource";
    
    // Constructor - call MapToApiVersion to use a different version than v1 (default)
    public MyCarterModule()
    {
        MapToApiVersion(new ApiVersion(2)); // Use v2 instead of default v1
    }
    
    protected override void AddRoutes(RouteGroupBuilder group)
    {
        // Define your routes here
    }
}
```

**Versioning Conventions:**
- Default API version is 1.0
- Version is specified in query string: `/api/resource?api-version=1`
- All endpoints automatically versioned through `ApiCarterModule`
- Use `protected override string PathPrefix` to specify the base path (required)
- Call `MapToApiVersion(new ApiVersion(x))` in constructor to use a different version
- If no version is specified, defaults to v1
- Version set configured in `WebSwaggerModule`

### Logging

1. **Use Serilog** with structured logging
2. **Log context enrichment**: Thread, demystified stack traces
3. **Bootstrap logger**: Created in `LoggingModule.CreateBootstrapLogger()`
4. **Configuration**: See `appsettings.json` for Serilog settings

### Security & Authentication

1. **Password Hashing**: Use BCrypt.Net-Next for secure password storage
2. **JWT Tokens**: Use `System.IdentityModel.Tokens.Jwt` for authentication
3. **Authorization**: 
   - Use `.AllowAnonymous()` for public endpoints
   - Use `.RequireAuthorization()` for protected endpoints
   - Custom dynamic authorization in `DynamicWebAuthorizationModule`
4. **Token Generation**: Implement `IAuthenticationTokenGenerator` interface
5. **Identity**: Use ULID-based identity generation via `IIdSupplier<Ulid>`

### Error Handling

1. **Use LanguageExt types**: `Either<Error, Result>`, `Option<T>`, `Try<T>`
2. **Domain errors**: Define typed errors in domain layer
3. **Exception handling**: Use `Execute.AndCatchAsync` for top-level handling
4. **ProblemDetails**: Return RFC 7807 problem details for API errors

### Scope Accessor Pattern

The project uses a custom **Scope Accessor** pattern for managing scoped dependencies across layers:

**Setting Scope (in Endpoints):**
```csharp
// In Carter module endpoint handler
private async Task<Results<Ok<Response>, ProblemHttpResult>> HandleAsync(
    [FromBody] Request request,
    [FromServices] IUserService userService,
    [FromServices] IUserRepository repository,
    [FromServices] IUnitOfWork unitOfWork,
    HttpContext context,
    CancellationToken cancellationToken = default)
{
    // Set scoped context before calling service
    using var scopes = new CompositeDisposable(
        _scopeAccessor.GetScope<IUserRepository>().Set(repository),
        _scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

    var command = _mapper.ToCommand(request);
    var result = await userService.RegisterAsync(command, cancellationToken);
    
    // Handle result...
}
```

**Retrieving Scope (in Services):**
```csharp
// In application service implementation
internal sealed class UserService(
    IScopeAccessor scopeAccessor,
    // ... other dependencies
    ) : IUserService
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task<Option<Error>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        // Retrieve scoped dependencies that were set in endpoint
        var repository = _scopeAccessor.GetRequiredContext<IUserRepository>();
        var unitOfWork = _scopeAccessor.GetRequiredContext<IUnitOfWork>();
        
        // Use dependencies for business logic...
    }
}
```

**Why use Scope Accessor?**
- Allows passing request-scoped dependencies from Web layer to Application layer
- Avoids parameter pollution in service methods
- Maintains Clean Architecture dependency rules
- Repositories and UnitOfWork are injected at the endpoint level and accessed in services
- **Important:** Always dispose scopes using `using` or `CompositeDisposable`

## Module Development Workflow

When creating a new feature module (e.g., "Inventory"):

1. **Create project structure**:
   ```
   HomeInventory.Domain.Inventory/
   HomeInventory.Application.Inventory/
   HomeInventory.Application.Inventory.Interfaces/
   HomeInventory.Contracts.Inventory/
   HomeInventory.Contracts.Inventory.Validators/
   HomeInventory.Infrastructure.Inventory/
   HomeInventory.Web.Inventory/
   ```

2. **Create module registration class** in each layer (e.g., `InventoryModule.cs`)

3. **Register module** in `ApplicationModules.cs`:
   ```csharp
   public static class ApplicationModules
   {
       public static IReadOnlyCollection<Type> GetModuleTypes() =>
       [
           typeof(CoreModule),
           typeof(LoggingModule),
           typeof(DatabaseModule),
           typeof(UserManagementModule),
           typeof(InventoryModule), // Add new module here
           // Modules are loaded in dependency order
       ];
   }
   ```

4. **Define domain models** (Aggregates, Entities, Value Objects, Events)

5. **Define contracts** (Request/Response DTOs)

6. **Create validators** for contracts

7. **Implement application services** (service interfaces and implementations)

8. **Implement infrastructure** (repositories, external services)

9. **Create web endpoints** using Carter

10. **Write tests** (unit, integration, acceptance)

## Testing Guidelines

1. **Unit Tests**: Test domain logic and business rules in isolation
2. **Integration Tests**: Test with real dependencies (database, external services)
3. **Acceptance Tests**: BDD scenarios using Reqnroll
4. **Test project structure**: Mirror source project structure
5. **Test naming**: `[MethodName]_[Scenario]_[ExpectedResult]`

## Documentation

1. **Feature documentation**: Document features in `docs/features/[feature-name].md`
2. **API documentation**: Use XML comments for Swagger/OpenAPI generation
3. **Architecture decisions**: Document significant decisions
4. **README updates**: Keep README.md current with badges and instructions

## Development Commands

### Build
```cmd
dotnet build
```

### Test
```cmd
dotnet test
```

### Run API
```cmd
dotnet run --project src\HomeInventory\HomeInventory.Api
```

### Docker
```cmd
docker-compose up
```

## Additional Guidelines

1. **Central Package Management**: All package versions in `Directory.Packages.props`
2. **Configuration**: Use strongly-typed options pattern with validation
3. **Health Checks**: Implement health checks for all critical dependencies
4. **API Versioning**: Use `Asp.Versioning.Mvc.ApiExplorer` for versioning
5. **Authentication**: JWT bearer token authentication for secured endpoints
6. **OpenAPI**: All endpoints documented and accessible via Swagger UI
7. **Docker**: Application is containerized and can run in Docker/Kubernetes
8. **Feature Flags**: Use `Microsoft.FeatureManagement` for toggling features

## Code Quality & Analyzers

The project uses multiple analyzers to enforce code quality:

1. **SonarAnalyzer.CSharp** - Code quality and security analysis
2. **Microsoft.EntityFrameworkCore.Analyzers** - EF Core best practices
3. **NSubstitute.Analyzers.CSharp** - Proper mocking patterns
4. **AwesomeAssertions.Analyzers** - Test assertion quality
5. **EditorConfig** - Enforces consistent coding style (see `.editorconfig`)

**Key EditorConfig Rules:**
- File-scoped namespaces required
- UTF-8 encoding with CRLF line endings
- Expression-bodied members preferred
- Pattern matching preferred over traditional checks
- Static local functions/anonymous functions preferred when possible

## Common Patterns to Follow

### Options Pattern
```csharp
public sealed class [Feature]Options : IOptions
{
    public static SectionPath SectionPath => new("[Feature]");
    // Properties
}

// Validator
public sealed class [Feature]OptionsValidator : FluentOptionsValidator<[Feature]Options>
{
    // Validation rules
}
```

### Mapper Pattern
```csharp
[Mapper]
public static partial class [Feature]Mapper
{
    public static partial [Destination] Map([Source] source);
}
```

### Module Pattern
```csharp
public sealed class [Module]Module : IModule
{
    public IReadOnlyCollection<Type> Dependencies { get; } = [];
    
    public IFeatureFlag Flag => FeatureFlags.Enabled; // Or create custom flag
    
    public async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken)
    {
        // Service registration
        context.Services.AddScoped<IMyService, MyService>();
    }
    
    public async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken)
    {
        // Middleware and endpoint configuration
    }
}
```

**Module System:**
- Modules define `Dependencies` to ensure correct load order
- Use `IFeatureFlag` to enable/disable modules
- Register in `ApplicationModules.cs` in proper order
- Modules can access `IServiceCollection`, `IConfiguration`, and `IMetricsBuilder` in `AddServicesAsync`
- Modules can access `WebApplication` in `BuildAppAsync`

## Questions to Ask When Developing

1. Does this belong in Domain, Application, or Infrastructure?
2. Is this a new module or part of an existing one?
3. What validation rules apply to this contract?
4. What domain events should be raised?
5. What error cases need handling?
6. Does this need authentication/authorization?
7. What are the testable scenarios?

---

**Remember**: Always favor immutability, functional patterns, and explicit error handling. Keep modules cohesive and loosely coupled.

