using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace Database.PostgreSql.Helpers
{
    [ExcludeFromCodeCoverage]
    public class NpgsqlDatabaseHelper
    {
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

            return (transaction, command);
        }

        public static T? ReadColumnValue<T>(NpgsqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return default;  // retorna null para classes ou Nullable<T> para structs

            // If byte[] or other reference
            if (typeof(T) == typeof(byte[]))
            {
                return (T)(object)reader.GetFieldValue<byte[]>(ordinal);
            }

            // If string
            if (typeof(T) == typeof(string))
            {
                return (T)(object)reader.GetString(ordinal);
            }

            // If structs/values
            return reader.GetFieldValue<T>(ordinal);
        }

        public static string? ReadColumnValue(NpgsqlDataReader sqlReader, string columnName)
        {
            return ReadColumnValue<string>(sqlReader, columnName);
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
