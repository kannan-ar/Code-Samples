using App.Commands;
using App.Services;

Console.WriteLine("🐜 Ant Colony Simulation with Neo4j");

var neo4j = new Neo4jService();
await neo4j.InitializeAsync();

var handler = new CommandHandler(neo4j);

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;
    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

    await handler.HandleAsync(input);
}
