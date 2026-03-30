# dot-net-nugets

Small collection of .NET 9.0 NuGet packages that standardize common API patterns and provide ready-to-use helpers for databases and tests.

## Packages

| Package                               | Description                                                                                  | NuGet package id                                    |
| :------------------------------------ | :------------------------------------------------------------------------------------------- | :-------------------------------------------------- |
| `EliseuBatista99.Core`                | ASP.NET Core startup builder, operation base/pattern, shared DTOs, and JWT helpers           | `eliseubatista99-dotnet-nugets-core`                |
| `EliseuBatista99.Database.PostgreSql` | EF Core (Npgsql) DI registration, generic repository, pagination helpers, byte/image helpers | `eliseubatista99-dotnet-nugets-database-postgresql` |
| `EliseuBatista99.Tests`               | Lightweight test helpers for building an `IExecutionContext` with common framework mocks     | `eliseubatista99-dotnet-nugets-tests`               |

For details on each package, see their respective `README.md` files:

[`Core`](Core/README.md), [`Database.PostgreSql`](Database.PostgreSql/README.md), and [`Tests`](Tests/README.md).
