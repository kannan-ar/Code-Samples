namespace DatabaseExporter.Models;

internal record PrimaryKey
{
    public required string ConstraintName { get; set; }
    public required string ColumnName { get; set; }
}