namespace DatabaseExporter.Models;

internal class Settings
{
    public static string ConnectionString => "Server=192.168.12.101\\SOLARCOEQA,1435;Database=BMM_User;User Id=bbm_svc_user;Password=ZwgTPE6Jkz70CsKDA==;TrustServerCertificate=True";
    public static readonly IEnumerable<string> ExecluedTables = [];
    public static readonly IEnumerable<string> ExecluedDataTables = ["Account", "AccountHistories", "AccountMonthly", "AccountMonthlyHistories"];
    public const decimal DataPageSize = 1000;
}