namespace DatabaseExporter.Models;

internal class TableQueryInfo
{
    public string TableName { get; set; }
    public long RecordFetched { get; set; }
}

internal class OperationInfo
{
    public List<TableQueryInfo> DataInfo { get; set; }
}