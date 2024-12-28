namespace DatabaseImporter.Models;

internal class Settings
{
    public static string ConnectionString => "Server=localhost,1433;Database=Solar_P_SCOA_DEMO;User Id=sa;Password=pass@word1;TrustServerCertificate=True;Connect Timeout=60000";
    public static string TableName = "G_SCOA_SEGMENT";
    public static string SqlFilePath = "C:\\Users\\QBurst\\Desktop\\sql\\Solar\\G_SCOA_SEGMENT\\";
    public static int WaitPeriod = 10;
    public static int MaxRetryCount = 10;

    public const bool UseIdentityInsert = true;
}