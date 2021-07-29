using AbstractFactoryLib;
using Unity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Unity.Microsoft.DependencyInjection;
using System.Threading.Tasks;

namespace AbstractFactory.App
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var container = new UnityContainer();
            container.UseUnity(".", "AbstractFactoryLib.dll").RegisterTypeWithEnum<IVechicleFactory, VechileType>();

            var builder = new HostBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<MyApp>();
            }).UseUnityServiceProvider(container);

            await builder.RunConsoleAsync();

            return 0;
        }
    }
}
