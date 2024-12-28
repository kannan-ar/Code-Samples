using Microsoft.Extensions.DependencyInjection;
using Sync.Engine.Schema;
using Sync.SqlServer.Schema;

namespace Sync.SqlServer;

public static class ServiceExtensions
{
    public static void AddSqlServer(this IServiceCollection services)
    {
        services.AddScoped<SchemaImporter<ISchemaImportOptions>, SqlSchemaImporter>();
    }
}