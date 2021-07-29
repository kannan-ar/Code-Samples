using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using AbstractFactoryLib;
using System.Threading;
using Unity;

namespace AbstractFactory.App
{
    public class MyApp : IHostedService
    {
        private readonly IVechicleFactory vechicleFactory;
        public MyApp(IUnityContainer container)
        {
            vechicleFactory = container.Resolve<IVechicleFactory>(nameof(VechileType.Low));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bike = vechicleFactory.CreateBike();
            var car = vechicleFactory.CreateCar();

            Console.WriteLine($"Bike price: {bike.Price}");
            Console.WriteLine($"Car price: {car.Price}");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
