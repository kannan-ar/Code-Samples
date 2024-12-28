using Microsoft.Extensions.DependencyInjection;
using Sync;
using Sync.Client;

var serviceCollection = new ServiceCollection();

serviceCollection.AddDataSync();

var serviceProvider = serviceCollection.BuildServiceProvider();

var schemaImport = serviceProvider.GetRequiredService<SchemaImport>();

schemaImport.Import(new Dictionary<string, object>
{
    { "Provider", "SqlServer" },
    { "SourceDatabaseName", "BMM_Budget" },
    { "TargetDatabaseName", "BMM_Bud" },
    { "SourceConnectionString", "Server=localhost,1433;User Id=sa;Password=pass@word1;TrustServerCertificate=True;" },
    { "TargetConnectionString", "Server=localhost,1433;User Id=sa;Password=pass@word1;TrustServerCertificate=True;" }
});