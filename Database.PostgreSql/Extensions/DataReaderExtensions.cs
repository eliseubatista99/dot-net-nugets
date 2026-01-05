using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.PostgreSql.Extensions
{
    public static class DataReaderExtensions
    {
        public static T? ReadColumnValue<T>(this NpgsqlDataReader reader, string columnName)
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

        public static string? ReadColumnValue(this NpgsqlDataReader sqlReader, string columnName)
        {
            return ReadColumnValue<string>(sqlReader, columnName);
        }
    }
}
