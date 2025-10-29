namespace App.Core;

public static class QueryLoader
{
    public static string Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Cypher file not found: {path}");

        return File.ReadAllText(path);
    }
}