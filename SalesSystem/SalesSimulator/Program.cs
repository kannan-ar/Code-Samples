namespace SalesSimulator
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    
    using Services;
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<HMACDelegatingHandler>()
                        .AddSingleton<IConfiguration>(configuration)
                        .AddSingleton<IProductService, ProductService>()
                        .AddSingleton<IUserService, UserService>()
                        .AddSingleton<ICarrierService, CarrierService>()
                        .AddSingleton<ISaleService, SaleService>()
                        .AddSingleton<SalesApplication>()
                        .AddHttpClient("HMAClient").AddHttpMessageHandler<HMACDelegatingHandler>();
                }).UseConsoleLifetime();

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var myService = services.GetRequiredService<SalesApplication>();
                await myService.RunAsync();
            }

            return 0;
        }
    }
}
