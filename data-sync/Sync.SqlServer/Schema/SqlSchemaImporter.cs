using Microsoft.SqlServer.Dac;
using Sync.Engine.Schema;

namespace Sync.SqlServer.Schema;

internal class SqlSchemaImporter : SchemaImporter<SqlSchemaImportOptions>
{
    public override string Provider => SqlConstants.Provider;

    protected override void Import(SqlSchemaImportOptions options)
    {
        Read(options);
        Write(options);
    }

    private void Read(SqlSchemaImportOptions options)
    {
        var dacServices = new DacServices(options.SourceConnectionString);

        dacServices.Extract("C:\\Test\\Database.dacpac", options.SourceDatabaseName, "MyApplication", new Version(1, 0, 0, 0));
    }

    private void Write(SqlSchemaImportOptions options)
    {
        var dacServices = new DacServices(options.TargetConnectionString);

        var deployOptions = new DacDeployOptions
        {
            CreateNewDatabase = true
        };

        dacServices.Deploy(DacPackage.Load("C:\\Test\\Database.dacpac"), options.TargetDatabaseName, true, deployOptions);
    }
}