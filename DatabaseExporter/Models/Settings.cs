namespace DatabaseExporter.Models;

internal class Settings
{
    public static string ConnectionString => "Server=192.168.12.56\\SOLARDEV2022,1435;Database=BMM_Budget;User Id=bmm_svc_user;Password=ZwgTPE6Jkz70CsKDA==;TrustServerCertificate=True;Connect Timeout=60000";
    public static readonly IEnumerable<string> ExecluedTables = [];
    public static readonly IEnumerable<string> ExecluedDataTables = [];
    public static readonly IEnumerable<string> IncludedDataTables = [];
    public const long DataPageSize = 5000;

    public static readonly bool GenerateTableCount = true;
    public static readonly bool GenerateTable = true;
    public static readonly bool GenerateSynonym = true;
    public static readonly bool GenerateView = true;
    public static readonly bool GenerateData = false;
    public static readonly bool IdentityInsert = false;
}