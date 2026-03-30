# EliseuBatista99.Database.PostgreSql

`EliseuBatista99.Database.PostgreSql` provides ready-to-use EF Core helpers to work with PostgreSQL:

- Register your `DbContext` in DI using Npgsql (`AddPostgresDbContext`)
- CRUD building blocks via a generic repository (`BaseRepository<T>`)
- Pagination query helper (`ExecutePaginatedRead`)
- Utility helpers to convert `byte[]` images into Base64 data URIs

Target framework: `net9.0` (NuGet ID: `eliseubatista99-dotnet-nugets-database-postgresql`, version `0.0.2`).

## NuGet Packages

| Package                                             | Latest Version | About                                                                            |
| :-------------------------------------------------- | :------------- | :------------------------------------------------------------------------------- |
| `eliseubatista99-dotnet-nugets-database-postgresql` | `0.0.2`        | PostgreSQL EF Core DI helper, generic repository, pagination, byte/image helpers |

## Documentation

This README focuses on the main building blocks included in the nuget so you can integrate it quickly into your API.

## Quick start

To use this package in your project:

```sh
dotnet add package eliseubatista99-dotnet-nugets-database-postgresql
```

### Register your DbContext (DI)

The `WebApplicationBuilderExtensions.AddPostgresDbContext<TContext>()` extension wires Npgsql, optional query splitting, snake_case naming convention, and optional EF logging.

```csharp
using Database.PostgreSql.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddPostgresDbContext<MyDbContext>(
    connectionStringName: "DefaultConnection",
    querySplittingBehavior: QuerySplittingBehavior.SplitQuery,
    useSnakeCase: true,
    enableLogging: false
);
```

Notes:

- Connection string is read from `builder.Configuration.GetConnectionString(connectionStringName)`
- `useSnakeCase` enables `UseSnakeCaseNamingConvention()` via `EFCore.NamingConventions`

### CRUD with the generic repository

Create a repository by inheriting from `BaseRepository<T>`:

```csharp
using Database.PostgreSql.Repositories;

public class UserRepository : BaseRepository<User>
{
    public UserRepository(MyDbContext context) : base(context) { }
}
```

Then you can use it like this:

```csharp
var users = userRepository.ReadQuery(); // no tracking
var writeQuery = userRepository.WriteQuery(); // tracking-enabled (for updates)

var user = await userRepository.GetByIdAsync("some-id");
var added = await userRepository.AddAsync(new User { /* ... */ });
```

Important detail: `GetByIdAsync` takes a `string id` and uses `DbSet<T>.FindAsync(id)`. If your entity key is not a `string`, you will need to adjust/extend the repository for your key type.

### Pagination

`ExecutePaginatedRead` works on any `IQueryable<T>`:

```csharp
var (data, hasMorePages) = await userRepository
    .ReadQuery()
    .ExecutePaginatedRead(page: 1, pageSize: 20);
```

When `page` and `pageSize` are both provided and positive:

- It uses `Skip((page - 1) * pageSize)` and `Take(pageSize + 1)`
- If there is one extra element, it sets `hasMorePages = true` and returns exactly `pageSize` elements

If `page`/`pageSize` are missing or invalid, it returns `ToListAsync()` for the full query and `hasMorePages = false`.

### Image / byte helpers (Base64 data URIs)

Convert `byte[]` (png/jpg/gif/svg) to a `data:{mime};base64,...` string:

```csharp
using Database.PostgreSql.Extensions;

byte[]? imageBytes = /* ... */;
string? dataUri = imageBytes.ToBase64DataUri();
```

You can also use the static helper:

```csharp
using Database.PostgreSql.Helpers;

string? dataUri = ImagesHelper.ToBase64DataUri(imageBytes);
```

## Core Concepts

### DbContext registration (`AddPostgresDbContext`)

This extension method registers `TContext` in DI and configures:

- `UseNpgsql(connectionString, ...)`
- `UseQuerySplittingBehavior(querySplittingBehavior)`
- optional `UseSnakeCaseNamingConvention()`
- optional sensitive-data EF logging (`EnableSensitiveDataLogging()` + `LogTo(Console.WriteLine)`)

### Generic repository (`BaseRepository<T>`)

`BaseRepository<T>` implements `IRepository<T>` and provides:

- `ReadQuery()` returning `_dbSet.AsNoTracking()` for queries
- `WriteQuery()` returning `_dbSet.AsQueryable()` for modifications
- `GetByIdAsync(string id)` using `FindAsync`
- `AddAsync(...)`, `AddRangeAsync(...)`, `UpdateAsync(...)`, `DeleteAsync(...)`
- `SaveChangesAsync()`

Behavior details:

- Write methods catch exceptions and return `false` after writing details to `Console.Error`
- `UpdateAsync(filter, updateAction)` loads all matching entities into memory (`ToListAsync()`), applies the `updateAction` per entity, then saves (if `saveChanges` is `true`)
- `DeleteAsync(predicate)` uses `ExecuteDeleteAsync()` for efficient server-side deletes
