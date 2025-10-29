using App.Core;
using Neo4j.Driver;

namespace App.Scenarios.Logistics;

public class LogisticsScenario : ScenarioBase
{
    private readonly IDriver _driver;

    public LogisticsScenario(IDriver driver)
    {
        _driver = driver;
    }

    public override string Name => "Logistics";

    public override async Task ClearDataAsync()
    {
        var cleanup = QueryLoader.Load("Cql/Logistics/cleanup.cql");

        await using var session = _driver.AsyncSession();

        await session.RunAsync(cleanup);
    }

    public override async Task LoadDataAsync()
    {
        var queries = QueryLoader.Load("Cql/Logistics/create_data.cql")
                             .Split(';', StringSplitOptions.RemoveEmptyEntries);

        await using var session = _driver.AsyncSession();

        foreach (var q in queries)
        {
            var trimmed = q.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                await session.RunAsync(trimmed);
            }
        }
    }

    public override async Task RunQueriesAsync(string command)
    {
        var query = string.Empty;
        var hasResult = false;

        switch(command)
        {
            case "nearest_warehouse":
                query = QueryLoader.Load("Cql/Logistics/nearest-warehouse.cql");
                break;
            case "set_nearest_warehouse_of_factory":
                query = QueryLoader.Load("Cql/Logistics/set_nearest_warehouse_of_factory.cql");
                break;
            case "set_warehouse_to_warehouse_proximity":
                query = QueryLoader.Load("Cql/Logistics/set_warehouse_to_warehouse_proximity.cql");
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                break;
        }

        if (string.IsNullOrEmpty(query)) return;

        using var session = _driver.AsyncSession();
        var cursor = await session.RunAsync(query);

        if(hasResult)
        {
            var results = await cursor.ToListAsync();
        }
    }
}