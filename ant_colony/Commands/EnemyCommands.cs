using App.Services;

namespace App.Commands
{
    public static class EnemyCommands
    {
        public static async Task AddEnemy(Neo4jService neo4j, string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: add enemy <id> <strength>");
                return;
            }

            var id = args[2];
            var strength = int.TryParse(args[3], out var s) ? s : 5;

            await neo4j.RunAsync(
                "CREATE (:Enemy {id:$id, strength:$strength})",
                new { id, strength });

            Console.WriteLine($"⚔️ Enemy {id} with strength {strength} added");
        }
    }
}
