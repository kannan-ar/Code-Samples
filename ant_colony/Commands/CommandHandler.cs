using App.Services;

namespace App.Commands
{
    public class CommandHandler
    {
        private readonly Neo4jService _neo4j;

        public CommandHandler(Neo4jService neo4j) => _neo4j = neo4j;

        public async Task HandleAsync(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = parts[0].ToLower();

            switch (command)
            {
                case "init":
                    await ColonyCommands.InitColony(_neo4j, parts);
                    break;
                case "add":
                    if (parts.Length > 1 && parts[1].ToLower() == "ant")
                        await ColonyCommands.AddAnt(_neo4j, parts);
                    else if (parts.Length > 1 && parts[1].ToLower() == "enemy")
                        await EnemyCommands.AddEnemy(_neo4j, parts);
                    break;
                case "spawn":
                    await FoodCommands.SpawnFood(_neo4j);
                    break;
                case "nearestfood":
                    await SimulationCommands.FindNearestFood(_neo4j, parts);
                    break;
                case "cleanup":
                    await ColonyCommands.Cleanup(_neo4j);
                    break;
                default:
                    Console.WriteLine("❓ Unknown command");
                    break;
            }
        }
    }
}
