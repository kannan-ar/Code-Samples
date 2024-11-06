namespace DatabaseExporter.Models;

internal class ForeignKey
{
    public string SourceConstraint { get; set; }
    public string SourceColumn { get; set; }
    public string TargetTable { get; set; }
    public string TargetColumn { get; set; }
}