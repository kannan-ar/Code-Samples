using App.Services;

namespace App.Commands
{
    public static class FoodCommands
    {
        private static readonly Random _rand = new();

        public static async Task SpawnFood(Neo4jService neo4j)
        {
            // Random ID, size, and position
            var id = $"food{_rand.Next(1000, 9999)}";
            var size = _rand.Next(5, 20);   // food weight
            var x = _rand.Next(0, 50);
            var y = _rand.Next(0, 50);

            await neo4j.RunAsync(
                "CREATE (:Food {id:$id, size:$size, x:$x, y:$y})",
                new { id, size, x, y });

            Console.WriteLine($"🍎 Food {id} (size {size}) spawned at ({x},{y})");
        }
    }
}
