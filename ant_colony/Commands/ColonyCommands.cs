using App.Services;

namespace App.Commands
{
    public static class ColonyCommands
    {
        public static async Task InitColony(Neo4jService neo4j, string[] args)
        {
            int count = args.Length > 2 && int.TryParse(args[2], out var n) ? n : 5;

            for (int i = 0; i < count; i++)
            {
                await neo4j.RunAsync(
                    "CREATE (:Ant {id: $id, role:'worker', strength:1})",
                    new { id = $"ant{i + 1}" });
            }

            Console.WriteLine($"✅ Colony initialized with {count} ants");
        }

        public static async Task AddAnt(Neo4jService neo4j, string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: add ant <id> <role> [strength]");
                return;
            }

            var id = args[2];
            var role = args[3];
            var strength = args.Length > 4 && int.TryParse(args[4], out var s) ? s : 1;

            await neo4j.RunAsync(
                "CREATE (:Ant {id:$id, role:$role, strength:$strength})",
                new { id, role, strength });

            Console.WriteLine($"✅ Ant {id} ({role}) added");
        }

        public static async Task Cleanup(Neo4jService neo4j)
        {
            var cleanup = await File.ReadAllTextAsync("Scripts/cleanup.cql");
            await neo4j.RunAsync(cleanup);
            Console.WriteLine("🧹 Graph cleaned");
        }
    }

}
