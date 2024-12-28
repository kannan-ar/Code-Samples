using Microsoft.Extensions.DependencyInjection;
using Sync.Client;
using Sync.SqlServer;

namespace Sync;

public static class ServiceExtensions
{
    public static void AddDataSync(this IServiceCollection services)
    {
        services.AddSqlServer();

        services.AddScoped<SchemaImport>();
    }
}