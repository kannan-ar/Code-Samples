using DatabaseExporter.Models;
using DatabaseExporter.Services;
using Microsoft.Data.SqlClient;
using System.Text;

TableService tableService = new TableService();
ViewService viewService = new ViewService();
SynonymsService synonymsService = new SynonymsService();

tableService.Open();
viewService.Open();
synonymsService.Open();

Console.WriteLine("Reading the schema...");

var tableScript = new StringBuilder();
var viewScript = new StringBuilder();
var dropViewScript = new StringBuilder();
var synonymScript = new StringBuilder();
var tableCountScript = new StringBuilder();

var tableNames = await tableService.GetTableNames();
var tableColumns = new Dictionary<string, IEnumerable<TableColumn>>();
var views = await viewService.GetViewNames();
var synonyms = await synonymsService.GetSynonyms();

if (Settings.GenerateTableCount)
{
    Console.WriteLine($"Generating table count script");

    foreach (var tableName in tableNames)
    {
        tableCountScript.AppendLine(TableScriptGenerator.GetTableCountString(tableName));
    }

    FileManager.Save("table_count.sql", tableCountScript.ToString());
}

if (Settings.GenerateTable)
{
    foreach (var tableName in tableNames)
    {
        if (Settings.ExecluedTables.Contains(tableName)) continue;

        var columns = await tableService.GetTableColumns(tableName);
        var primaryKeys = await tableService.GetPrimaryKeys(tableName);
        var defaultConstraints = await tableService.GetDefaultConstraints(tableName);

        tableColumns.Add(tableName, columns);

        tableScript.AppendLine(TableScriptGenerator.GetCreateTableScript(tableName, columns, primaryKeys, defaultConstraints));
        tableScript.AppendLine();

        Console.WriteLine($"Script generated for {tableName}");

        var foreignKeys = await tableService.GetForeignKeys(tableName);

        var foreignKeyGroups = foreignKeys.GroupBy(x => x.SourceConstraint).ToList();

        foreach (var group in foreignKeyGroups)
        {
            string script = TableScriptGenerator.GetAlterTableForeignKeyScript(tableName, group);
            tableScript.AppendLine(script);
        }

        Console.WriteLine($"Foreign key script generated for {tableName}");

        FileManager.Save("tables.sql", tableScript.ToString());
    }
}
else
{
    foreach (var tableName in tableNames)
    {
        var columns = await tableService.GetTableColumns(tableName);
        tableColumns.Add(tableName, columns);
    }
}

if (Settings.GenerateView && views.Any())
{
    Console.WriteLine($"Script generating for views");

    foreach (var view in views)
    {
        viewScript.AppendLine((await viewService.GetViewDefinition(view)));
        viewScript.AppendLine();

        dropViewScript.AppendLine(ViewScriptGenerator.GetDropViewSql(view));
    }

    FileManager.Save("drop_views.sql", dropViewScript.ToString());
    FileManager.Save("views.sql", viewScript.ToString());
}

if (Settings.GenerateSynonym && synonyms.Any())
{
    Console.WriteLine($"Script generating for synonyms");
    foreach (var synonym in synonyms)
    {
        synonymScript.AppendLine(SynonymScriptGenerator.GetSynonymSql(synonym));
    }

    FileManager.Save("synonyms.sql", synonymScript.ToString());
}

if (Settings.GenerateData)
{
    foreach (var table in tableColumns)
    {
        if (Settings.ExecluedDataTables.Contains(table.Key)) continue;

        if (Settings.IncludedDataTables.Any() && !Settings.IncludedDataTables.Contains(table.Key)) continue;

        Console.WriteLine($"Reading data from {table.Key}");

        var tableCount = await tableService.GetTableCount(table.Key);
        var currentCount = await OperationService.GetTableRecordFetched(table.Key);

        for (var i = currentCount; (i * Settings.DataPageSize) < tableCount; i++)
        {
            var data = await tableService.GetData(tableService.GetPaginatedQuery(table.Key, i));

            if (!data.Any()) continue;

            Console.WriteLine($"Generating page {i} script");

            var insertScript = new StringBuilder();

            if(Settings.IdentityInsert)
            {
                insertScript.Append($"SET IDENTITY_INSERT [{table.Key}] ON");
                insertScript.AppendLine();
            }
           
            insertScript.Append(TableScriptGenerator.GetInsertDataScript(table.Key, table.Value, data));
            insertScript.AppendLine();

            if (Settings.IdentityInsert)
            {
                insertScript.Append($"SET IDENTITY_INSERT [{table.Key}] OFF");
            }

            FileManager.Save($"{table.Key}_{i}.sql", insertScript.ToString());
            await OperationService.SaveTableRecordFetched(table.Key, i);
        }
    }
}

//if (Settings.GenerateData)
//{
//    foreach (var table in tableColumns)
//    {
//        if (Settings.ExecluedDataTables.Contains(table.Key)) continue;

//        if (Settings.IncludedDataTables.Any() && !Settings.IncludedDataTables.Contains(table.Key)) continue;

//        Console.WriteLine($"Reading data from {table.Key}");

//        try
//        {
//            var data = await tableService.GetTableData(table.Key);
//            long count = data.Count();
//            decimal pageCapacity = count < Settings.DataPageSize ? 1 : count / Settings.DataPageSize;
//            var pageSize = Math.Ceiling(pageCapacity);

//            if (count == 0) continue;

//            for (int i = 1; i <= pageSize; i++)
//            {
//                Console.WriteLine($"Generating page {i} script");
//                var insertScript = new StringBuilder();

//                insertScript.Append($"SET IDENTITY_INSERT [{table.Key}] ON");
//                insertScript.AppendLine();
//                insertScript.AppendLine();

//                insertScript.Append(TableScriptGenerator.GetInsertDataScript(table.Key, i, Settings.DataPageSize, count, table.Value, data));

//                insertScript.AppendLine();
//                insertScript.AppendLine();

//                insertScript.Append($"SET IDENTITY_INSERT [{table.Key}] OFF");


//                FileManager.Save($"{table.Key}_{i}.sql", insertScript.ToString());
//            }
//        }
//        catch (SqlException ex) when (ex.Number == -2)
//        {
//            Console.WriteLine($"Timeout occured for {table.Key}");
//        }
//    }
//}



