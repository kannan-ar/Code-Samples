
using DatabaseImporter.Models;
using DatabaseImporter.Services;
using Microsoft.Data.SqlClient;

var totalRecords = 0;
var retryCount = 0;

var sql = new SqlExecutor(Settings.ConnectionString);
sql.Open();

var currentCount = await OperationService.GetTableRecordAffected(Settings.TableName);

while (true)
{

    var fileName = $"{Settings.TableName}_{currentCount}.sql";
    var filePath = $"{Settings.SqlFilePath}{fileName}";
    SqlTransaction transaction = default;

    if (!File.Exists(filePath))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{fileName} does not exist. Exiting...");
        break;
    }

    try
    {
        Console.WriteLine($"{fileName} executing...");

        using StreamReader reader = new(filePath);
        var sqlString = await reader.ReadToEndAsync();

        (int recordsAffected, transaction, bool success) = await sql.Execute(sqlString);

        if(success)
        {
            await transaction.CommitAsync();

            await OperationService.SaveTableRecordAffected(Settings.TableName, currentCount);

            currentCount++;
            totalRecords += recordsAffected;
            retryCount = 0;

            Console.WriteLine($"{recordsAffected} records inserted into {Settings.TableName}. Waiting {Settings.WaitPeriod} seconds");
        }
        else
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }

            throw new InvalidOperationException("Error occured transaction rolled back");
        }

        Thread.Sleep(Settings.WaitPeriod * 1000);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
        Console.ForegroundColor = ConsoleColor.White;

        if (retryCount == Settings.MaxRetryCount)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Maximum retry count reached for {Settings.TableName}. Exiting...");
            break;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Timeout occured on {fileName} retrying of {retryCount}/{Settings.MaxRetryCount}. Wating 10 seconds...");
        Console.ForegroundColor = ConsoleColor.White;

        Thread.Sleep(10000);

        retryCount++;
    }
}

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine($"Total {totalRecords} records inserted into {Settings.TableName}");