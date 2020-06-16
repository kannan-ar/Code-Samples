namespace PurchaseHub.Services.Implementations
{
    using Cassandra;
    using System.Threading.Tasks;

    public class Database : IDatabase
    {
        private readonly string address;
        private readonly string keySpace;
        private Cluster cluster;
        public ISession Session { get; private set;}

        public Database(string address, string keySpace)
        {
            this.address = address;
            this.keySpace = keySpace;
        }
        public async Task<IDatabase> ConnectAsync()
        {
            cluster = Cluster.Builder().AddContactPoint(address).Build();
            Session = await cluster.ConnectAsync(keySpace);

            return this;
        }

        public async Task<IDatabase> DisConnectAsync()
        {
            await Session.ShutdownAsync();
            return this;
        }
    }
}