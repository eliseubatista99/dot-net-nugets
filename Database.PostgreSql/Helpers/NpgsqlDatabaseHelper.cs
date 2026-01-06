using Database.PostgreSql.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace Database.PostgreSql.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class NpgsqlDatabaseHelper
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

        public static (NpgsqlTransaction transaction, NpgsqlCommand command) InitialzieSqlTransaction(NpgsqlConnection connection)
        {
            NpgsqlCommand command = connection.CreateCommand();
            NpgsqlTransaction transaction;

            // Start a local transaction.
            transaction = connection.BeginTransaction();

            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Parameters.Clear();
            command.Connection = connection;
            command.Transaction = transaction;
            //command.CommandText = commandText;

            return (transaction, command);
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd");
        }

        public static string FormatDateWithTime(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
        }        
    }
}
