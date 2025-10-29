using Microsoft.Extensions.Configuration;
using Neo4j.Driver;

namespace App.Services
{
    public class Neo4jService : IAsyncDisposable
    {
        private readonly IDriver _driver;

        public Neo4jService()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var uri = config["Neo4j:Uri"];
            var user = config["Neo4j:User"];
            var password = config["Neo4j:Password"];

            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public async Task InitializeAsync()
        {
            await using var session = _driver.AsyncSession();
            var schema = await File.ReadAllTextAsync("Scripts/schema.cql");
            await session.RunAsync(schema);
        }

        public async Task RunAsync(string cypher, object? parameters = null)
        {
            await using var session = _driver.AsyncSession();
            await session.RunAsync(cypher, parameters);
        }

        public async Task<IResultCursor> QueryAsync(string cypher, object? parameters = null)
        {
            await using var session = _driver.AsyncSession();
            return await session.RunAsync(cypher, parameters);
        }

        public async ValueTask DisposeAsync() => await _driver.CloseAsync();
    }
}
