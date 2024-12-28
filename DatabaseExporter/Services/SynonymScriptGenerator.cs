using DatabaseExporter.Models;

namespace DatabaseExporter.Services;

public class SynonymScriptGenerator
{
    public static string GetSynonymSql(Synonym synonym)
    {
        return $"CREATE SYNONYM {synonym.Name}{Environment.NewLine}FOR {synonym.Definition};{Environment.NewLine}GO{Environment.NewLine}";
    }
}