using Database.PostgreSql.Helpers;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Database.PostgreSql.Providers
{
    [ExcludeFromCodeCoverage]
    public class NpgsqlDatabaseProvider<T>
    {
        protected IConfiguration _configuration { get; }

        public NpgsqlDatabaseProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected virtual string GetConnectionString()
        {
            return string.Empty;
        }

        protected List<T> ExecuteReadMultiple(string command)
        {
            var connectionString = GetConnectionString();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                List<T> result = new List<T>();

                connection.Open();

                var (transaction, commandExecutor) = NpgsqlDatabaseHelper.InitialzieSqlTransaction(connection);

                try
                {
                    commandExecutor.CommandText = command;

                    using (var sqlReader = commandExecutor.ExecuteReader())
                    {
                        while (sqlReader!.Read())
                        {
                            var dataEntry = GetObjectFromDataReader(sqlReader)!;

                            result.Add(dataEntry);
                        }
                    }

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        protected T? ExecuteRead(string command)
        {
            var connectionString = GetConnectionString();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                T? result = default;

                connection.Open();

                var (transaction, commandExecutor) = NpgsqlDatabaseHelper.InitialzieSqlTransaction(connection);

                try
                {
                    commandExecutor.CommandText = command;

                    var sqlReader = commandExecutor.ExecuteReader();

                    if (sqlReader.HasRows)
                    {
                        sqlReader.Read();
                        result = GetObjectFromDataReader(sqlReader);

                    }

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        protected bool ExecuteWrite(string command)
        {
            var connectionString = GetConnectionString();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var (transaction, commandExecutor) = NpgsqlDatabaseHelper.InitialzieSqlTransaction(connection);

                try
                {
                    commandExecutor.CommandText = command;

                    commandExecutor.ExecuteNonQuery();

                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected virtual T? GetObjectFromDataReader(NpgsqlDataReader dataReader)
        {
            return default(T);
        }
    }

}
