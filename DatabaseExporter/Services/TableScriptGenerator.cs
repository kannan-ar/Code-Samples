using DatabaseExporter.Models;
using System.Text;

namespace DatabaseExporter.Services;

internal class TableScriptGenerator
{
    public static string GetCreateTableScript(
        string tableName, 
        IEnumerable<TableColumn> columns,
        IEnumerable<PrimaryKey> primaryKeys,
        IEnumerable<DefaultConstraint> defaultConstraints)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("CREATE TABLE [{0}] (", tableName);
        sb.AppendLine();

        foreach (var column in columns)
        {
            sb.Append("[" + column.ColumnName + "]");
            sb.Append(" " + column.DataType);
            sb.Append(" " + GetColumnSize(column));
            sb.Append(" " + GetIdentityString(column));
            sb.Append(" " + GetDefaultConstraintString(column, defaultConstraints));
            sb.Append(" " + GetNullString(column));
            sb.Append(',');
            sb.AppendLine();
        }

        sb.Append(GetPrimaryKeyString(primaryKeys));
        sb.AppendLine();
        sb.Append(')');
        sb.AppendLine();

        return sb.ToString();
    }

    public static string GetAlterTableForeignKeyScript(string tableName, IGrouping<string, ForeignKey> foreignKey)
    {
        var sourceColumns = string.Join(",", foreignKey.Select(x => $"[{x.SourceColumn}]"));
        var targetTable = foreignKey.Select(x => x.TargetTable).First();
        var targetColumns = string.Join(",", foreignKey.Select(x => $"[{x.TargetColumn}]"));

        return $"ALTER TABLE [{tableName}] ADD CONSTRAINT {foreignKey.Key} FOREIGN KEY ({sourceColumns}) REFERENCES [{targetTable}] ({targetColumns}){Environment.NewLine}";
    }

    public static string GetInsertDataScript(string tableName, long pageNumer, decimal pageSize, long count, IEnumerable<TableColumn> columns, IEnumerable<dynamic> data)
    {
        var pageStart = (pageNumer - 1) * pageSize;
        var pageEnd = pageStart + pageSize;
        var dataScript = new StringBuilder();

        for(; (pageStart < pageEnd && pageStart < count); pageStart++)
        {
            var item = data.ElementAt((int)pageStart);

            dataScript.Append(GetInsertString(tableName, columns, item));
            dataScript.AppendLine();
        }

        return dataScript.ToString();
    }

    public static string GetInsertDataScript(string tableName, IEnumerable<TableColumn> columns, IEnumerable<dynamic> data)
    {
        var dataScript = new StringBuilder();

        foreach (var item in data)
        {
            dataScript.Append(GetInsertString(tableName, columns, item));
            dataScript.AppendLine();
        }

        return dataScript.ToString();
    }

    public static string GetTableCountString(string tableName)
    {
        return $"SELECT COUNT(*) FROM {tableName}{Environment.NewLine}";
    }

    private static string GetColumnSize(TableColumn column)
    {
        return column.DataType switch
        {
            "decimal" or "numeric" => string.Format("({0}, {1})", column.NumericPrecision, column.NumericScale),
            "float" => string.Format("({0})", column.NumericPrecision),
            "datetime2" => string.Format("({0})", column.DatetimePrecision),
            "varchar" or "char" or "nvarchar" => string.Format("({0})", column.CharacterMaxLength == -1? "max" : column.CharacterMaxLength),
            _ => string.Empty,
        };
    }

    private static string GetIdentityString(TableColumn column)
    {
        return column.IsIdentity ? string.Format("IDENTITY({0}, {1})", column.Seed, column.Increment) : string.Empty;
    }

    private static string GetNullString(TableColumn column)
    {
        return column.IsNullable == "NO" ? "NOT NULL" : "NULL";
    }

    private static string GetDefaultConstraintString(TableColumn column, IEnumerable<DefaultConstraint> defaultConstraints)
    {
        var constraint = defaultConstraints.FirstOrDefault(c => c.ColumnName == column.ColumnName);

        return constraint == null ? string.Empty : $"DEFAULT {constraint.Definition}";
    }

    private static string GetPrimaryKeyString(IEnumerable<PrimaryKey> primaryKeys)
    {
        //if (primaryKeys.Count() > 1) throw new ArgumentException("More than one primary key");

        if (!primaryKeys.Any()) return string.Empty;

        var keyNames = string.Join(",", primaryKeys.Select(p => $"[{p.ColumnName}]"));
        var constraintName = primaryKeys.First().ConstraintName;

        return string.Format("CONSTRAINT {0} PRIMARY KEY ({1})", constraintName, keyNames);
    }

    private static string GetInsertString(string tableName, IEnumerable<TableColumn> columns, IDictionary<string, object> item)
    {
        var fieldBuilder = new StringBuilder($"INSERT INTO [{tableName}] (");
        var valueBuilder = new StringBuilder("VALUES (");
        var count = columns.Count();

        for (var i = 0; count > i; i++)
        {
            var column = columns.ElementAt(i);

            fieldBuilder.Append($"[{column.ColumnName}]");

            switch(column.DataType)
            {
                case "datetime":
                case "datetime2":
                case "varchar":
                case "char":
                case "nvarchar":
                case "uniqueidentifier":
                case "datetimeoffset":
                    valueBuilder.Append(item[column.ColumnName] == null ? "NULL" : $"'{Convert.ToString(item[column.ColumnName]).Replace("'", "''")}'");
                    break;
                case "bit":
                    valueBuilder.Append(item[column.ColumnName] == null ? "NULL" : $"{(Convert.ToBoolean(item[column.ColumnName]) ? 1 : 0)}");
                    break;
                default:
                    valueBuilder.Append(item[column.ColumnName] == null ? "NULL" : item[column.ColumnName]);
                    break;
            }

            if (i <  count - 1)
            {
                fieldBuilder.Append(", ");
                valueBuilder.Append(", ");
            }

        }

        fieldBuilder.Append(')');
        valueBuilder.Append(')');

        return $"{fieldBuilder}{valueBuilder}";
    }
}