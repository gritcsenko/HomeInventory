# GitHub Copilot Instructions for HomeInventory

## Meta-Instructions for AI Assistants

**IMPORTANT**: When a user provides:
- Requests about code patterns or practices
- Advice on how to write better code
- Hints about avoiding specific issues
- Rules that can prevent undesired results
- Corrections to mistakes you've made

**YOU MUST UPDATE THESE INSTRUCTIONS** to incorporate that guidance so future conversations benefit from the learning. Add the guidance to the appropriate section (Critical Guidelines, Examples, Patterns, etc.) with clear examples of what to do and what to avoid.

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

### General Testing Principles

1. **Unit Tests**: Test domain logic and business rules in isolation
2. **Integration Tests**: Test with real dependencies (database, external services)
3. **Acceptance Tests**: BDD scenarios using Reqnroll
4. **Test project structure**: Mirror source project structure
5. **Test naming**: `[MethodName]_[Scenario]_[ExpectedResult]`

### Test Structure Pattern

All tests follow the **Given-When-Then** pattern using `BaseTest<TGivenContext>`:

```csharp
[UnitTest]
public class MyFeatureTests() : BaseTest<MyFeatureTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void MethodName_Scenario_ExpectedResult()
    {
        Given
            .New<SomeType>(out var variable1)
            .New<OtherType>(out var variable2, static () => new OtherType("value"));

        var then = When
            .Invoked(variable1, variable2, static (v1, v2) => v1.Method(v2));

        then
            .Result(static result => 
            {
                result.Should().NotBeNull();
                result.Property.Should().Be("expected");
            });
    }
}

public sealed class MyFeatureTestsGivenContext(BaseTest test) : GivenContext<MyFeatureTestsGivenContext>(test);
```

### Critical Test Guidelines

**DO:**
- ✅ **Use expression-bodied lambdas for single statements** - `static x => x.Method()` not `static x => { x.Method(); }`
- ✅ **Use `Create<T>()` for values that don't need to be in test context** - avoids polluting context with unnecessary variables
- ✅ **Update these instructions when user provides requests, advice, hints, or rules** that can prevent undesired results
- ✅ Use `var then = When.Invoked(...);` followed by `then.Result(...)` on separate lines
- ✅ Define all test data in `Given` section using `.New<T>(out var variable)`
- ✅ Use AutoFixture to generate test data - avoid hardcoded literals/constants
- ✅ Use `static` lambdas wherever possible for performance
- ✅ Use AwesomeAssertions fluent syntax (`.Should()`)
- ✅ Use `ContainSingleSingleton<T>()`, `ContainTransient<T>()`, `ContainScoped<T>()` for service collection assertions
- ✅ Use `.ContainKey(...).WhoseValue.Should()...` for dictionary assertions
- ✅ Create a separate `GivenContext` class for each test class
- ✅ Keep test methods focused on a single behavior
- ✅ Provide meaningful assertions in module tests - verify specific services are registered

**DON'T:**
- ❌ **Use block body `{ }` for lambdas with single statement** - triggers IDE0053 warning
- ❌ **Create variables only to create other variables** - pollutes test context; use `Create<T>()` instead
- ❌ Create local variables in test methods (except for `then`)
- ❌ Capture local variables in `Invoked` or `Result` lambdas
- ❌ Chain `When.Invoked(...).Result(...)` without the `then` variable
- ❌ Use `Contain(d => d.ServiceType == typeof(T) && d.Lifetime == ...)` - use `ContainTransient<T>()` instead
- ❌ Add comments explaining what test does - test name should be self-documenting
- ❌ Duplicate literal values across tests - use AutoFixture or define in `Given` section
- ❌ Hardcode string literals, numbers, or constants in factory functions - use `Fixture.Create<T>()` or parameters
- ❌ Use `ContainKey(...)` then access dictionary - use `.ContainKey(...).WhoseValue.Should()...` pattern
- ❌ Write module tests with only `.NotBeNullOrEmpty()` - verify specific services

### AutoFixture Usage Guidelines

**Using AutoFixture for Test Data:**

The `New` method in `GivenContext` is designed to infer types automatically in most cases, eliminating the need to specify generic type parameters explicitly:

```csharp
// ✅ GOOD - Type is inferred from out parameter with explicit type
Given
    .New(out IVariable<string> fieldVar)      // Type inferred from IVariable<string>
    .New(out IVariable<int> statusVar);       // Type inferred from IVariable<int>

// ✅ GOOD - AutoFixture generates values with out var (requires type argument)
Given
    .New<string>(out var fieldVar)    // Type argument required with 'out var'
    .New<int>(out var statusVar);      // Type argument required with 'out var'

// ✅ GOOD - Type is inferred from lambda return type
Given
    .New(out var fieldVar, static () => "some value")  // Infers string from lambda
    .New(out var statusVar, static () => 400);         // Infers int from lambda

// ❌ BAD - Redundant type argument when out parameter has explicit type
Given
    .New<string>(out IVariable<string> fieldVar);  // ❌ Compiler warning: redundant type argument

// ✅ GOOD - Type is inferred from lambda with IVariable parameters
Given
    .New(out var errorVar, errorMsgVar, static msg => new NotFoundError(msg));

// ✅ GOOD - Use up to 3 IVariable parameters + lambda
Given
    .New(out var resultVar, arg1Var, arg2Var, arg3Var, static (a1, a2, a3) =>
        new MyClass { Arg1 = a1, Arg2 = a2, Arg3 = a3 });

// ❌ BAD - Hardcoded literals
Given
    .New<MyClass>(out var objectVar, static () =>
        new MyClass { Field = "hardcoded", Status = 400 });

// ✅ GOOD - For complex setups, create helper method in GivenContext
public sealed class MyTestsGivenContext(BaseTest test) : GivenContext<MyTestsGivenContext>(test)
{
    public MyTestsGivenContext ComplexSetup(
        out IVariable<ResultType> resultVar,
        out IVariable<string> field1Var,
        out IVariable<string> field2Var)
    {
        // Omit type argument - type inferred from explicit IVariable<string>
        New(out field1Var);
        New(out field2Var);
        // Use Create<T>() for values that don't need to be variables
        // Remove 'static' when using Create<T>() from base class
        New(out resultVar, field1Var, field2Var, (f1, f2) =>
            new ResultType 
            { 
                Field1 = f1, 
                Field2 = f2, 
                Temp = Create<string>()  // ✅ Don't create tempVar just to use it here
            });
        return This;
    }
}

// Usage in test
Given
    .ComplexSetup(out var resultVar, out var field1Var, out var field2Var);
```

**Key Principles:**
- The `New` method supports up to 3 `IVariable<T>` parameters followed by a factory lambda
- Type inference works when the lambda return type is explicit
- **Avoid creating variables only to create other variables** - use `Create<T>()` instead to avoid context pollution
- **Every `New` call adds a variable to test context** - only create variables that need to be referenced in assertions
- Use `.New<T>(out var)` when AutoFixture can generate the type directly AND you need to reference it
- You can add new overloads to `GivenContext<T>` if you need more than 3 parameters
- Remove `static` from lambda when using `Create<T>()` method from base class

**When to Use `Create<T>()` vs `New`:**

```csharp
// ❌ BAD - Creating variables only to use them once in factory
New<string>(out var error1Var);
New<string>(out var error2Var);
New(out var modelStateVar, field1Var, field2Var, error1Var, error2Var, static (f1, f2, e1, e2) =>
{
    var ms = new ModelStateDictionary();
    ms.AddModelError(f1, e1);  // e1 and e2 are only used here
    ms.AddModelError(f2, e2);
    return ms;
});

// ✅ GOOD - Use Create<T>() for values that don't need to be variables
New(out var modelStateVar, field1Var, field2Var, (f1, f2) =>  // Note: not 'static' when using Create<T>()
{
    var ms = new ModelStateDictionary();
    ms.AddModelError(f1, Create<string>());  // ✅ Create value directly
    ms.AddModelError(f2, Create<string>());
    return ms;
});

// Rule: Create a variable with New ONLY if:
// 1. You need to reference it in test assertions
// 2. You need to pass it to multiple method calls
// 3. You need to verify its value in .Result()

// Otherwise, use Create<T>() directly in the factory function
```

**Type Argument Guidelines:**
- **Omit type argument** when the out parameter has an explicit type: `.New(out IVariable<string> myVar)`
- **Include type argument** when using `out var`: `.New<string>(out var myVar)`
- The compiler warns about "Type argument specification is redundant" when you use `.New<T>(out IVariable<T> var)`
- Type inference relies on the declared type of the out parameter or the lambda return type

**Why AutoFixture?**
- Reduces possibility of "cheating" in production code
- Ensures code works with any valid input, not just known test values
- Makes tests more robust and less brittle
- Reveals hidden dependencies on specific values

### Examples

**❌ BAD - Local variables, chaining, and hardcoded literals:**
```csharp
[Fact]
public void MyTest()
{
    Given.New<MyClass>(out var sut);
    
    var localData = "test data";  // ❌ Local variable
    var status = 400;              // ❌ Hardcoded literal
    
    When
        .Invoked(sut, s => s.Method(localData, status))  // ❌ Capturing local variables
        .Result(result => result.Should().BeTrue());      // ❌ Chained, no 'then' variable
}
```

**✅ GOOD - Proper pattern with AutoFixture:**
```csharp
[Fact]
public void Method_WithValidData_ReturnsTrue()
{
    Given
        .New<MyClass>(out var sut)
        .New<string>(out var dataVar)      // ✅ AutoFixture generates string
        .New<int>(out var statusVar);      // ✅ AutoFixture generates int

    var then = When
        .Invoked(sut, dataVar, statusVar, static (s, data, status) => s.Method(data, status));  // ✅ No capturing

    then
        .Result(static result => result.Should().BeTrue());  // ✅ Separate 'then' variable
}
```

**❌ BAD - Dictionary assertions:**
```csharp
result.Extensions.Should().ContainKey("errorCodes");
var errorCodes = result.Extensions["errorCodes"] as string[];  // ❌ Separate access
errorCodes.Should().Contain("InvalidCredentialsError");
```

**✅ GOOD - Dictionary assertions:**
```csharp
result.Extensions.Should().ContainKey("errorCodes")
    .WhoseValue.Should().BeAssignableTo<string[]>()  // ✅ Chained assertion
    .Which.Should().Contain("InvalidCredentialsError");
```

**❌ BAD - Service assertions:**
```csharp
services.Should()
    .Contain(d => d.ServiceType == typeof(IMyService) && d.Lifetime == ServiceLifetime.Transient);
```

**✅ GOOD - Service assertions:**
```csharp
services.Should()
    .ContainTransient<IMyService>();
```

**❌ BAD - Module test with no meaningful assertions:**
```csharp
protected override void EnsureRegistered(IServiceCollection services) =>
    services.Should().NotBeNullOrEmpty();  // ❌ Too generic
```

**✅ GOOD - Module test with specific assertions:**
```csharp
protected override void EnsureRegistered(IServiceCollection services) =>
    services.Should()
        .Contain(d => d.ServiceType == typeof(HealthCheckService))
        .And.Contain(d => d.ServiceType == typeof(IHealthCheckPublisher));
```

### Module Test Pattern

```csharp
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class MyModuleTests() : BaseModuleTest<MyModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should()
            .ContainSingleSingleton<MySingletonService>()
            .And.ContainScoped<IMyScopedService>()
            .And.ContainTransient<IMyTransientService>();
}
```

### Integration Test Pattern

For tests requiring full application context or multiple modules:

```csharp
[IntegrationTest]
public class MyIntegrationTests() : BaseTest<MyIntegrationTestsGivenContext>(static t => new(t))
{
    [Fact]
    public async Task Integration_Scenario_ExpectedBehavior()
    {
        await Given
            .HttpContext(out var contextVar)
            .New<MyRequest>(out var requestVar)
            .SubstituteFor(out IVariable<IMyService> serviceVar, requestVar, (s, r) => 
                s.ProcessAsync(r, Cancellation.Token).Returns(Task.FromResult(true)))
            .InitializeHostAsync();

        var then = await When
            .InvokedAsync(serviceVar, requestVar, static (svc, req, ct) => svc.ProcessAsync(req, ct));

        then
            .Result(static result => result.Should().BeTrue());
    }
}

public sealed class MyIntegrationTestsGivenContext(BaseTest test) : GivenContext<MyIntegrationTestsGivenContext>(test);
```

### Test Data Builder Pattern

For complex test data, use factory methods in `Given`:

```csharp
Given
    .New<ComplexObject>(out var objectVar, static () => new ComplexObject
    {
        Property1 = "value1",
        Property2 = 42,
        NestedObject = new NestedObject
        {
            NestedProperty = "nested"
        }
    });
```

### Async Test Pattern

```csharp
[Fact]
public async Task AsyncMethod_Scenario_ExpectedResult()
{
    await Given
        .New<MyClass>(out var sut)
        .New<string>(out var dataVar, static () => "test")
        .InitializeAsync();  // If async setup needed

    var then = await When
        .InvokedAsync(sut, dataVar, static (s, data, ct) => s.MethodAsync(data, ct));

    then
        .Result(static result => result.Should().Be("expected"));
}
```

### Testing Error Scenarios

```csharp
[Fact]
public void Method_WithInvalidInput_ReturnsError()
{
    Given
        .New<MyService>(out var sut)
        .New<InvalidInput>(out var inputVar);

    var then = When
        .Invoked(sut, inputVar, static (s, input) => s.Process(input));

    then
        .Result(static result =>
        {
            result.IsFaulted.Should().BeTrue();
            result.Error.Should().BeOfType<ValidationError>();
        });
}
```

### Test Organization

1. Group related tests in the same file
2. Use nested classes for organizing related scenarios (when appropriate)
3. Use `[Theory]` with `[InlineData]` or `[ClassData]` for parameterized tests
4. Use `[UnitTest]`, `[IntegrationTest]`, or `[AcceptanceTest]` attributes for categorization

### Common Assertions

```csharp
// AwesomeAssertions patterns
result.Should().BeTrue();
result.Should().NotBeNull();
result.Should().Be(expected);
result.Should().BeOfType<ExpectedType>();
collection.Should().HaveCount(3);
collection.Should().Contain(item => item.Id == expectedId);
collection.Should().ContainSingle();
collection.Should().NotBeNullOrEmpty();

// Service collection assertions
services.Should().ContainSingleSingleton<IService>();
services.Should().ContainScoped<IService>();
services.Should().ContainTransient<IService>();
```

### Code Quality Warnings to Avoid

When writing tests, watch for and fix these common warnings:

1. **IDE0053: Use expression body for lambda expression**
   - Use expression-bodied lambdas when the body is a single expression
   - ✅ Good: `static x => x.Property`
   - ❌ Bad: `static x => { return x.Property; }`

2. **Type argument specification is redundant**
   - Omit type arguments when the compiler can infer from the out parameter type
   - ✅ Good: `.New(out IVariable<string> myVar)`
   - ❌ Bad: `.New<string>(out IVariable<string> myVar)`

3. **Variable is assigned but its value is never used**
   - Don't create variables that are only used to create other variables
   - Use helper methods in GivenContext instead

4. **Async method lacks 'await' operators**
   - Ensure async test methods actually await async operations
   - Use `await When.InvokedAsync(...)` not `When.Invoked(...)`

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

