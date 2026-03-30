# EliseuBatista99.Core

`EliseuBatista99.Core` is a small set of ASP.NET Core building blocks designed to standardize application startup, a consistent "operation" pattern, common DTO conventions, and JWT-related helper utilities.

Target framework: `net9.0` (NuGet ID: `eliseubatista99-dotnet-nugets-core`, version `0.0.5`).

## NuGet Packages

| Package                              | Latest Version | About                                                                                          |
| :----------------------------------- | :------------- | :--------------------------------------------------------------------------------------------- |
| `eliseubatista99-dotnet-nugets-core` | `0.0.5`        | Core abstractions (startup builder, operation base, DTOs, JWT helpers, Swagger schema filters) |

## Documentation

This README gives a quick overview of the main building blocks so you can integrate the nuget with your API project. The examples focus on the most common extension points.

## Quick start

To use `EliseuBatista99.Core`, add the package to your project:

```sh
dotnet add package eliseubatista99-dotnet-nugets-core
```

### Create your own program builder

Derive from `EliseuBatista99.Core.BaseProgramBuilder` and enable the features you want (authentication, authorization, AutoMapper profiles, and DB hooks).

```csharp
using EliseuBatista99.Core;
using AutoMapper;

public class ProgramBuilder : BaseProgramBuilder
{
    protected override bool UseAuthentication => true;
    protected override bool UseAuthorization => true;

    protected override Profile[] GetMapperProfiles() =>
        new[] { new MyMappingProfile() };

    protected override void ConfigureDatabase()
    {
        // configure your DB
    }

    protected override void SeedDatabase()
    {
        // optional seeding
    }
}
```

Then in `Program.cs`:

```csharp
var builder = new ProgramBuilder();
builder.Build(args);
builder.Run();
```

### Implement an operation

Create input/output DTOs and inherit from `EliseuBatista99.Core.Operations.OperationBase<TInput, TOutput>`.

```csharp
using EliseuBatista99.Core.Operations;
using EliseuBatista99.Core;

public class MyInput : OperationInputDto
{
    public string Name { get; set; } = "";
}

public class MyOutput : OperationOutputDto
{
    public string Result { get; set; } = "";
}

public class MyOperation : OperationBase<MyInput, MyOutput>
{
    public MyOperation(IExecutionContext executionContext) : base(executionContext) { }

    protected override Task HandleExecution()
    {
        output.Data = new MyOutput { Result = input!.Name };
        return Task.CompletedTask;
    }
}
```

Your `Execute(...)` returns an `OperationResponseDto<TOutput>` with:

- `Data` (the operation output)
- `Metadata` (`Success`, and optional `Errors`)
- `StatusCode` (defaults to `200`)

## Core Concepts

### Base startup (`BaseProgramBuilder`)

`BaseProgramBuilder` provides a `Build(args)` method that wires up the application pipeline:

- Loads environment variables via `DotNetEnv` (`Env.Load()`) and `AddEnvironmentVariables()`
- Registers `IExecutionContext` (scoped) and provides access to DI services
- Adds controllers with JSON enum serialization as strings (`JsonStringEnumConverter`)
- Adds a default CORS policy named `CorsPolicy` (localhost/client origins + credentials)
- Configures Swagger/OpenAPI:
  - `EnumSchemaFilter` (enums as `string` + enum names)
  - `ForceAllDtosDocumentFilter` (generates schemas for all types derived from `Dto`)
  - Swagger annotations enabled
- If authentication is enabled (`UseAuthentication`), configures JWT Bearer using the `Jwt` section
- If authorization is enabled (`UseAuthorization`), adds `UseAuthorization()`
- In development, maps and enables OpenAPI/Swagger endpoints

Override these protected members in your derived builder:

- `UseAuthentication` (default: `false`)
- `UseAuthorization` (default: `false`)
- `corsOrigins` (default: localhost/client origins)
- `GetMapperProfiles()` (AutoMapper profiles)
- `ConfigureDatabase()` and `SeedDatabase()` (app hooks)
- `InjectDependencies()` and `ConfigureControllers()` (customization hooks)

### Operation pattern (`OperationBase`)

`OperationBase<TInput, TOutput>` standardizes how you execute operations and build responses:

- Accepts `IExecutionContext` and resolves `IMapper` from DI
- Reads `HttpContext.User` when available
- Calls your overridden `HandleExecution()` method
- Returns an `OperationResponseDto<TOutput>` envelope

Included base DTO types:

- `OperationInputDto`
- `OperationOutputDto`

Pagination DTOs:

- `OperationPaginatedInputDto` (`Page`, `PageSize`)
- `OperationPaginatedOutputDto` (`HasMorePages`)

### Authentication helpers (`JwtSettings`, `AuthenticationHelper`)

The package includes:

- `JwtSettings`:
  - `Key`, `Issuer`, `Audience`
  - `AccessTokenExpirationMinutes`
  - `RefreshTokenExpirationDays`
- `AuthenticationHelper`:
  - Password hashing/verification (`EncryptPassword`, `DecryptPassword`)
  - Token generation:
    - `GenerateAccessToken(claims, settings)`
    - `GenerateRefreshToken()`

When `UseAuthentication` is enabled, `BaseProgramBuilder` expects:

- a `Jwt` section in configuration that can be bound to `JwtSettings`

## Configuration

### JWT configuration (`UseAuthentication`)

Example `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your-symmetric-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  }
}
```

## Swagger behavior notes

Swagger schema behavior is driven by:

- `EnumSchemaFilter` (enums as strings)
- `ForceAllDtosDocumentFilter` (generates schemas for all `Dto`-derived types)

The package also defines marker attributes (`ExcludeFromSwagger`, `ExcludeDtoIfUnusedAttribute`) that you can use as extension points in your own Swagger customization.
