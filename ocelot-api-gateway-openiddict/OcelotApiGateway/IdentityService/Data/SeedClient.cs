
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace IdentityService.Data
{
    public class SeedClient : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedClient(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IOpenIddictMongoDbContext>();
            var options = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<OpenIddictMongoDbOptions>>().CurrentValue;
            var database = await context.GetDatabaseAsync(cancellationToken);

            var applications = database.GetCollection<OpenIddictMongoDbApplication>(options.ApplicationsCollectionName);
          
            var apps = await applications.FindAsync(app => app.ClientId == "postman");

            if (!(await apps.AnyAsync()))
            {
                applications.InsertOne(new OpenIddictMongoDbApplication
                {
                    ClientId = "postman",
                    ClientSecret = "postman-secret",
                    DisplayName = "Postman",
                    RedirectUris = new[] { "https://oauth.pstmn.io/v1/callback" },
                    Permissions = new[]
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,

                         OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,

                         OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                    }

                });
            }

        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
