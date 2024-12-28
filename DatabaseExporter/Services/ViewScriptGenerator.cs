namespace DatabaseExporter.Services;

internal class ViewScriptGenerator
{
    public static string GetDropViewSql(string viewName)
    {
        return $"DROP VIEW IF EXISTS {viewName};{Environment.NewLine}GO{Environment.NewLine}";
    }
}