using App.Extensions;
using App.Services;
using Neo4j.Driver;

namespace App.Commands
{
    public static class SimulationCommands
    {
        // Find nearest food to a given ant
        public static async Task FindNearestFood(Neo4jService neo4j, string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: nearestfood <antId>");
                return;
            }

            var antId = args[1];
            var query = @"
            MATCH (a:Ant {id:$antId}), (f:Food)
            WITH f, point({x:f.x, y:f.y}) AS foodPos,
                 point({x:a.x, y:a.y}) AS antPos
            RETURN f.id AS id, f.size AS size,
                   distance(foodPos, antPos) AS dist
            ORDER BY dist ASC
            LIMIT 1
        ";

            var cursor = await neo4j.QueryAsync(query, new { antId });
            var record = await cursor.SingleOrDefaultAsync();

            if (record == null)
            {
                Console.WriteLine("❌ No food found");
                return;
            }

            Console.WriteLine($"✅ Nearest food for {antId} is {record["id"]} (size {record["size"]}) at distance {Math.Round((double)record["dist"], 2)}");
        }

        // Defense: find nearest N ants to defend against enemy
        public static async Task DefendAgainstEnemy(Neo4jService neo4j, string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: defend <enemyId> <count>");
                return;
            }

            var enemyId = args[1];
            var count = int.TryParse(args[2], out var n) ? n : 3;

            var query = @"
            MATCH (e:Enemy {id:$enemyId}), (a:Ant)
            WITH e, a,
                 point({x:e.x, y:e.y}) AS enemyPos,
                 point({x:a.x, y:a.y}) AS antPos
            RETURN a.id AS antId,
                   distance(enemyPos, antPos) AS dist
            ORDER BY dist ASC
            LIMIT $count
        ";

            var cursor = await neo4j.QueryAsync(query, new { enemyId, count });
            var ants = await cursor.ToListAsync();

            if (!ants.Any())
            {
                Console.WriteLine("❌ No ants available to defend");
                return;
            }

            Console.WriteLine($"🛡️ Nearest {count} ants to defend {enemyId}:");
            foreach (var ant in ants)
                Console.WriteLine($"   - {ant["antId"]} (distance {Math.Round((double)ant["dist"], 2)})");
        }

        // Cooperation: find ants to help carry heavy food
        public static async Task CarryFood(Neo4jService neo4j, string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: carry <foodId>");
                return;
            }

            var foodId = args[1];
            var query = @"
            MATCH (f:Food {id:$foodId}), (a:Ant {role:'worker'})
            WITH f, a,
                 point({x:f.x, y:f.y}) AS foodPos,
                 point({x:a.x, y:a.y}) AS antPos
            RETURN a.id AS antId,
                   distance(foodPos, antPos) AS dist,
                   f.size AS size
            ORDER BY dist ASC
        ";

            var cursor = await neo4j.QueryAsync(query, new { foodId });
            var records = await cursor.ToListAsync();

            if (!records.Any())
            {
                Console.WriteLine("❌ No workers found to carry food");
                return;
            }

            var size = (int)records[0]["size"];
            int needed = Math.Max(1, size / 5); // 1 ant per 5 size units

            Console.WriteLine($"💪 Food {foodId} (size {size}) needs {needed} workers");
            foreach (var record in records.Take(needed))
                Console.WriteLine($"   - {record["antId"]} (distance {Math.Round((double)record["dist"], 2)})");
        }
    }
}
