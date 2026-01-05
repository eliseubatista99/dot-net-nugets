using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.PostgreSql.Models
{
    public class TableField
    {
        public required string FieldName { get; set; }

        public required NpgsqlTypes.NpgsqlDbType DataType { get; set; }

        public required object? FieldValue { get; set; }
    }
}
