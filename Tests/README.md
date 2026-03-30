# EliseuBatista99.Tests

`EliseuBatista99.Tests` is a lightweight testing helper package for projects built on top of `EliseuBatista99.Core`.

It provides:

- `TestsBuilder`: a simple mock factory that builds an `IExecutionContext` populated with common framework dependencies (AutoMapper + `IHttpContextAccessor`)
- `ExecutionContextMock`: an `IExecutionContext` implementation backed by a dictionary of dependencies
- `HttpContextMock`: an `IHttpContextAccessor` mock that allows setting the current user (`ClaimsPrincipal`)
- `OperationTestBase`: a base class to set up the `ExecutionContext` for unit tests

Target framework: `net9.0` (NuGet ID: `eliseubatista99-dotnet-nugets-tests`, version `0.0.1`).

## NuGet Packages

| Package | Latest Version | About |
| :------ | :-------------- | :---- |
| `eliseubatista99-dotnet-nugets-tests` | `0.0.1` | Test helpers: builders, mocks, and a base class for operation unit tests |

## Documentation

This README focuses on the main building blocks included in the nuget, so you can wire up `EliseuBatista99.Core` operations in your unit tests.

## Quick start

To use this package in your test project:

```sh
dotnet add package eliseubatista99-dotnet-nugets-tests
```

### Use `OperationTestBase`

Inherit from `EliseuBatista99.Tests.OperationTestBase` to get a prepared `IExecutionContext` during construction.

```csharp
using EliseuBatista99.Tests;
using Xunit;

public class MyOperationTests : OperationTestBase
{
    [Fact]
    public void Example()
    {
        // ExecutionContext is already available
    }
}
```

### Customize mocks with `TestsBuilder`

If your tests need extra services in DI (for example, repositories, external clients, or custom AutoMapper profiles), derive from `TestsBuilder` and override `AddMockClasses()` and/or `GetMapperProfiles()`.

```csharp
using AutoMapper;
using EliseuBatista99.Tests;
using Moq;

public class MyTestsBuilder : TestsBuilder
{
    protected override void AddMockClasses()
    {
        AddMock<IMyRepository>();
    }

    protected override Profile[] GetMapperProfiles()
    {
        return new Profile[] { new MyMappingProfile() };
    }
}
```

Then in your test base you can swap the builder:

```csharp
using EliseuBatista99.Tests;

public class MyOperationTestBase : OperationTestBase
{
    protected override TestsBuilder CreateBuilder()
    {
        return new MyTestsBuilder();
    }
}
```

### Access mocks from the builder

`TestsBuilder` implements `ITestsBuilder` and exposes:

- `GetMock<T>()` returning the mocked instance (if you registered it)
- `Build()` returning an `IExecutionContext` backed by the registered dependencies

## Core Concepts

### `TestsBuilder`

`TestsBuilder` is responsible for assembling an `IExecutionContext` used by `EliseuBatista99.Core.Operations.OperationBase`.

It sets up framework mocks:

- AutoMapper: creates an `IMapper` from `GetMapperProfiles()`
- `IHttpContextAccessor`: provided by `HttpContextMock`

When you call `Build()`, it:

- adds framework mocks
- calls `AddMockClasses()` so you can register your own mocks/instances
- returns an `ExecutionContextMock` built from the internal dependency dictionary

### `OperationTestBase`

`OperationTestBase` is a convenience base for unit tests.

In its constructor, it:

- creates a builder via `CreateBuilder()` (default: `new TestsBuilder()`)
- builds an `IExecutionContext` and stores it in `ExecutionContext`
- calls `BeforeAll()` (virtual) so you can run setup logic once per test instance

### `HttpContextMock`

`HttpContextMock` implements `IHttpContextAccessor` via `Moq`.

It includes a helper to set the authenticated user:

- `User(ClaimsPrincipal principal)` sets `_context.User` and updates the accessor mock

This is important because `OperationBase` reads `HttpContext.User` (when available).

## Notes

- This package targets `net9.0` and includes `Moq`, and `xunit` packages only as dependencies for your test projects.
- The exact services you can mock depend on what your `OperationBase` implementations resolve from `IExecutionContext`.

