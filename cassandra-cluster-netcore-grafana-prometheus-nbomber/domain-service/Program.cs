using DomainService.Infrastructure.Kafka;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.UseLocalhostClustering();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<KafkaPartitionAwareConsumerService>();
                })
                .Build();

        await host.RunAsync();
    }

    static bool IsRunningInKubernetes()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST"));
    }
}