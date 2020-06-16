namespace PurchaseHub.Services
{
    using Cassandra;
    using System.Threading.Tasks;
    public interface IDbQuery
    {
        Task<RowSet> ExecuteAsync(string sql);
    }
}