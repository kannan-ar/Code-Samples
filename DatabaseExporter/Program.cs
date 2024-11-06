using DatabaseExporter.Models;
using DatabaseExporter.Services;
using System.Text;

TableService tableService = new TableService();

tableService.Open();

Console.WriteLine("Reading the schema...");

var tableNames = await tableService.GetTableNames();
var dmlScript = new StringBuilder();
var tableColumns = new Dictionary<string, IEnumerable<TableColumn>>();

foreach (var tableName in tableNames)
{
    if (Settings.ExecluedTables.Contains(tableName)) continue;

    var columns = await tableService.GetTableColumns(tableName);
    var primaryKeys = await tableService.GetPrimaryKeys(tableName);
    var defaultConstraints = await tableService.GetDefaultConstraints(tableName);

    tableColumns.Add(tableName, columns);

    dmlScript.AppendLine(ScriptGenerator.GetCreateTableScript(tableName, columns, primaryKeys, defaultConstraints));
    dmlScript.AppendLine();

    Console.WriteLine($"Script generated for {tableName}");
}

foreach (var tableName in tableNames)
{
    if (Settings.ExecluedTables.Contains(tableName)) continue;

    var foreignKeys = await tableService.GetForeignKeys(tableName);

    foreach (var foreignKey in foreignKeys)
    {
        if (foreignKeys.Count(f => f.SourceConstraint == foreignKey.SourceConstraint) > 1) throw new InvalidOperationException("Multiple foreign key constraints");

        dmlScript.AppendLine(ScriptGenerator.GetAlterTableForeignKeyScript(tableName, foreignKey));
    }

    Console.WriteLine($"Foreign key script generated for {tableName}");
}

FileManager.Save("tables.sql", dmlScript.ToString());


foreach (var table in tableColumns)
{
    if (Settings.ExecluedDataTables.Contains(table.Key)) continue;

    Console.WriteLine($"Reading data from {table.Key}");

    var data = await tableService.GetTableData(table.Key);
    decimal count = data.Count();
    decimal pageCapacity = count < Settings.DataPageSize ? 1 : count / Settings.DataPageSize;
    var pageSize = Math.Ceiling(pageCapacity);

    if (count == 0) continue;

    for(int i = 1; i <= pageSize; i++)
    {
        Console.WriteLine($"Generating page {i} script");
        var insertScript = new StringBuilder();

        insertScript.Append($"SET IDENTITY_INSERT [{table.Key}] ON");
        insertScript.AppendLine();
        insertScript.AppendLine();

        insertScript.Append(ScriptGenerator.GetInsertDataScript(table.Key, i, Settings.DataPageSize, count, table.Value, data));

        insertScript.AppendLine();
        insertScript.AppendLine();

        insertScript.Append($"SET IDENTITY_INSERT [{table.Key}] OFF");


        FileManager.Save($"{table.Key}_{i}.sql", insertScript.ToString());
    }
    
}


