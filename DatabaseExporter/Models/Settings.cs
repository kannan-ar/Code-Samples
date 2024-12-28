namespace DatabaseExporter.Models;

internal class Settings
{
    public static string ConnectionString => "Server=192.168.12.101\\SOLARCOEQA,1435;Database=BMM_Budget;User Id=bbm_svc_user;Password=ZwgTPE6Jkz70CsKDA==;TrustServerCertificate=True;Connect Timeout=60000";
    public static readonly IEnumerable<string> ExecluedTables = [];
    public static readonly IEnumerable<string> ExecluedDataTables = [];
    public static readonly IEnumerable<string> IncludedDataTables = ["Version"];
    public const long DataPageSize = 5000;

    public static readonly bool GenerateTableCount = false;
    public static readonly bool GenerateTable = false;
    public static readonly bool GenerateSynonym = false;
    public static readonly bool GenerateView = false;
    public static readonly bool GenerateData = true;
    public static readonly bool IdentityInsert = false;
}