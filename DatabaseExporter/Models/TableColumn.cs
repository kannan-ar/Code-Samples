namespace DatabaseExporter.Models;

internal record TableColumn
{
    public string ColumnName { get; set; }
    public string ColumnDefault { get; set; }
    public string IsNullable { get; set; }
    public string DataType { get; set; }
    public int CharacterMaxLength { get; set; }
    public int NumericPrecision { get; set; }
    public int NumericScale { get; set; }
    public int DatetimePrecision { get; set; }
    public bool IsIdentity { get; set; }
    public int Seed { get; set; }
    public int Increment { get; set; }
}