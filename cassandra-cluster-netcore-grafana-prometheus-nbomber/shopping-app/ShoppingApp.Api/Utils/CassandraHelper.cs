using Cassandra;

namespace ShoppingApp.Api.Utils
{
    public static class CassandraHelper
    {
        private static Cluster? _cluster;

        public static async Task<Cassandra.ISession> InitializeAsync(string contactPoint, int port, string keyspace)
        {
            _cluster ??= Cluster.Builder()
                            .AddContactPoint(contactPoint)
                            .WithPort(port)
                            .WithDefaultKeyspace(keyspace)
                            .Build();

            return await _cluster.ConnectAsync();
        }
    }
}
