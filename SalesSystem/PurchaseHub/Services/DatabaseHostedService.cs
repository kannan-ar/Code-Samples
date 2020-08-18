namespace PurchaseHub.Services
{
    using System;
    using Microsoft.Extensions.Hosting;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public class DatabaseHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        public DatabaseHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using(var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IDatabase>();
                await db.ConnectAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using(var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IDatabase>();
                await db.DisConnectAsync();
            }
        }
    }
}