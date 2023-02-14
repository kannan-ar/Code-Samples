using Functions;
using Functions.Extensions;
using Functions.Modals;
using Functions.Services;
using Functions.Services.Implementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddConfiguration();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.Configure<QueueNameSettings>(configuration.GetSection("QueueNameSettings"));
            builder.Services.Configure<TableNameSettings>(configuration.GetSection("TableNameSettings"));

            builder.Services.AddAzureClients(builder =>
            {
                builder.AddQueueServiceClient(configuration.GetConnectionString("QueueStorage"));
                builder.AddTableServiceClient(configuration.GetConnectionString("TableStorage"));
            });

            builder.Services.AddSingleton<ITableService, TableService>();
        }
    }
}
