using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database.PostgreSql.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class WebApplicationBuilderExtensions
    {
        public static void AddPostgresDbContext<TContext>(
        this WebApplicationBuilder builder,
        string connectionStringName = "DefaultConnection",
        QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SplitQuery,
        bool useSnakeCase = true,
        bool enableLogging = false
    )
        where TContext : DbContext
        {
            var connectionString = builder.Configuration.GetConnectionString(connectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{connectionStringName}' not found."
                );

            builder.Services.AddDbContext<TContext>(options =>
            {
                // Configura o provider Postgres e query splitting
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseQuerySplittingBehavior(querySplittingBehavior);
                });

                // Snake case opcional
                if (useSnakeCase)
                {
                    options.UseSnakeCaseNamingConvention();
                }

                // Logging de debug opcional
                if (enableLogging)
                {
                    options.EnableSensitiveDataLogging();
                    options.LogTo(Console.WriteLine);
                }
            });
        }
    }
}
