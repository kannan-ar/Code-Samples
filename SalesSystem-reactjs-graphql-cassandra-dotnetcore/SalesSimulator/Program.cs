namespace SalesSimulator
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;
    using System.Threading.Tasks;

    using Services;
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IProductService, ProductService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<SalesManager>()
                .BuildServiceProvider())
            {
                await serviceProvider.GetRequiredService<SalesManager>().Run();
            }
        }
    }
}
