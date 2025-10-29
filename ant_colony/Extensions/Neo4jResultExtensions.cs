using Neo4j.Driver;

namespace App.Extensions
{
    public static class Neo4jResultExtensions
    {
        public static async Task<IRecord?> SingleOrDefaultAsync(this IResultCursor cursor)
        {
            var records = await cursor.ToListAsync();

            if (records.Count > 1)
            {
                throw new InvalidOperationException("Sequence contains more than one element.");
            }

            return records.SingleOrDefault();
        }
    }
}
