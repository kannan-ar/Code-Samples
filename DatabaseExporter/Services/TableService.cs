using DatabaseExporter.Models;

namespace DatabaseExporter.Services;

internal class TableService
{
    private SqlExecutor sql;

    public void Open()
    {
        sql = new SqlExecutor();
        sql.Open(Settings.ConnectionString);
    }

    public async Task<IEnumerable<string>> GetTableNames()
    {
        return await sql.Select<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'");
    }


    public async Task<IEnumerable<TableColumn>> GetTableColumns(string tableName)
    {
        return await sql.Select<TableColumn>("SELECT COLUMN_NAME ColumnName, COLUMN_DEFAULT ColumnDefault, IS_NULLABLE IsNullable, DATA_TYPE DataType, CHARACTER_MAXIMUM_LENGTH CharacterMaxLength, NUMERIC_PRECISION NumericPrecision, NUMERIC_SCALE NumericScale, DATETIME_PRECISION DatetimePrecision, COLUMNPROPERTY(object_id(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') IsIdentity, IDENT_SEED(TABLE_SCHEMA+'.'+TABLE_NAME) Seed, IDENT_INCR(TABLE_SCHEMA+'.'+TABLE_NAME) Increment FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName ORDER BY ORDINAL_POSITION", new
        {
            tableName
        });
    }

    public async Task<IEnumerable<PrimaryKey>> GetPrimaryKeys(string tableName)
    {
        return await sql.Select<PrimaryKey>("SELECT TC.CONSTRAINT_NAME ConstraintName, COLUMN_NAME ColumnName FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME WHERE TC.TABLE_NAME = @tableName ORDER BY ORDINAL_POSITION", new
        {
            tableName
        });
    }

    public async Task<IEnumerable<ForeignKey>> GetForeignKeys(string tableName)
    {
        return await sql.Select<ForeignKey>("SELECT CCU.CONSTRAINT_NAME AS SourceConstraint, CCU.COLUMN_NAME AS SourceColumn, KCU.TABLE_NAME AS TargetTable, KCU.COLUMN_NAME AS TargetColumn FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON CCU.CONSTRAINT_NAME = RC.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON KCU.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME WHERE CCU.TABLE_NAME = @tableName ORDER BY  CCU.CONSTRAINT_NAME, CCU.TABLE_NAME", new
        {
            tableName
        });
    }

    public async Task<IEnumerable<DefaultConstraint>> GetDefaultConstraints(string tableName)
    {
        return await sql.Select<DefaultConstraint>("SELECT c.name ColumnName, dc.definition [Definition] FROM sys.tables t INNER JOIN sys.default_constraints dc ON t.object_id = dc.parent_object_id INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND c.column_id = dc.parent_column_id where t.name = @tableName", new
        {
            tableName
        });
    }

    public async Task<IEnumerable<dynamic>> GetTableData(string tableName)
    {
        return await sql.Select($"SELECT * FROM [{tableName}]");
    }
}