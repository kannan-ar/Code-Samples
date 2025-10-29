using Neo4j.Driver;

namespace App.Core;

public static class Neo4jDriverFactory
{
    private static IDriver? _driver;

    public static IDriver Create(string uri = "bolt://localhost:7687", string user = "neo4j", string password = "password")
    {
        if (_driver == null)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        return _driver;
    }

    public static async Task CloseAsync()
    {
        if (_driver != null)
        {
            await _driver.CloseAsync();
            await _driver.DisposeAsync();
            _driver = null;
        }
    }
}