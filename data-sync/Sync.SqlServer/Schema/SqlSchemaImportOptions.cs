using Sync.Engine.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sync.SqlServer.Schema;

public class SqlSchemaImportOptions : ISchemaImportOptions
{
    public required string Provider { get; set; }
    public required string SourceDatabaseName { get; set; }
    public required string TargetDatabaseName { get; set; }
    public required string SourceConnectionString { get; set; }
    public required string TargetConnectionString { get; set; }
}