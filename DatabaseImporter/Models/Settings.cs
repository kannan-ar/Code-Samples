namespace DatabaseImporter.Models;

internal class Settings
{
    public static string ConnectionString => "Server=192.168.12.56\\SOLARDEV2022,1435;Database=BMM_Budget;User Id=bmm_svc_user;Password=ZwgTPE6Jkz70CsKDA==";
    public static string TableName = "G_SCOA_SEGMENT";
    public static string SqlFilePath = "C:\\Users\\QBurst\\Desktop\\sql\\Solar\\G_SCOA_SEGMENT\\";
    public static int WaitPeriod = 10;
    public static int MaxRetryCount = 10;

    public const bool UseIdentityInsert = true;
}