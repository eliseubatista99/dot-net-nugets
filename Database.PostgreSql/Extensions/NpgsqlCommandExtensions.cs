using Database.PostgreSql.Models;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace Database.PostgreSql.Extensions
{
    public static class NpgsqlCommandExtensions
    {
        public static NpgsqlCommand BuildInsertCommand(this NpgsqlCommand command, string tableName, TableField[] fields)
        {
            var cmdText = $"INSERT INTO {tableName} (";

            for (int i = 0; i < fields.Length; i++)
            {
                if (i > 0)
                {
                    cmdText += ", ";
                }

                cmdText += $"{fields[i].FieldName}";
            }

            cmdText += $") VALUES (";

            for (int i = 0; i < fields.Length; i++)
            {
                if (i > 0)
                {
                    cmdText += ", ";
                }

                cmdText += $"@{fields[i].FieldName}";

            }

            cmdText += ")";

            command.Parameters.Clear();
            command.CommandText = cmdText;

            for (int i = 0; i < fields.Length; i++)
            {
                command.Parameters.AddWithValue(fields[i].FieldName, fields[i].DataType, fields[i].FieldValue ?? (object)DBNull.Value);
            }

            return command;
        }
    }
}
