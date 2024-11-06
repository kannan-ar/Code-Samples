namespace DatabaseExporter.Services;

internal static class FileManager
{
    public static void Save(string fileName, string text)
    {
        using var streamWriter = new StreamWriter(fileName);

        streamWriter.Write(text);
    }
}