using DomainService.Infrastructure.Kafka;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;

internal class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder
                        .UseLocalhostClustering();
                })
                .ConfigureServices(services =>
                {
                    services.AddOpenTelemetry()
                                .ConfigureResource(resource => resource
                                    .AddService("domain-service"))
                                .WithMetrics(metrics =>
                                {
                                    metrics
                                        //.AddRuntimeInstrumentation()
                                        .AddMeter("DomainService.Domain.OrderProcessorGrain")
                                        .AddMeter("Microsoft.Orleans")
                                        .AddPrometheusHttpListener(options =>
                                        {
                                            options.ScrapeEndpointPath = "/metrics";
                                            options.UriPrefixes = ["http://+:9464"];
                                        });
                                });

                    services.AddHostedService<KafkaPartitionAwareConsumerService>();
                })
                .Build();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during startup: {ex.Message}");
            return;
        }
        
    }

    static bool IsRunningInKubernetes()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST"));
    }
}