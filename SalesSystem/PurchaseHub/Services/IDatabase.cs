namespace PurchaseHub.Services
{
    using System.Threading.Tasks;
    using Cassandra;
    public interface IDatabase
    {
        Task<IDatabase> ConnectAsync();
        Task<IDatabase> DisConnectAsync();
        ISession Session { get; }
    }
}