namespace PurchaseHub.Services.Implementations
{
    using Cassandra;
    using System.Threading.Tasks;

    public class DbQuery: IDbQuery
    {
        private readonly IDatabase database;

        public DbQuery(IDatabase database)
        {
            this.database = database;
        }

        public async Task<RowSet> ExecuteAsync(string sql)
        {
            var query = await database.Session.PrepareAsync(sql);
            var statement = query.Bind();
            return await database.Session.ExecuteAsync(statement);
        }
    }
}