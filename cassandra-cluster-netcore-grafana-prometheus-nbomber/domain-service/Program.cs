using DomainService.Infrastructure.Kafka;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Grafana.Loki;

internal class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var config = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();

            var applicationName = config["Logging:Loki:ApplicationName"] ?? "domain-service";
            var lokiUrl = config["Logging:Loki:LokiUrl"]
                ?? throw new ArgumentNullException("Logging:Loki:LokiUrl");

            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .Enrich.WithSpan()
                                .Enrich.With<ActivityEnricher>()
                                .Enrich.WithProperty("Application", applicationName)
                                .WriteTo.GrafanaLoki(lokiUrl)
                                .CreateLogger();

            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
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
                                .WithTracing(tracerProviderBuilder =>
                                {
                                    tracerProviderBuilder
                                        .AddSource(applicationName)
                                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName))
                                        .AddOtlpExporter(options =>
                                        {
                                            options.Endpoint = new Uri(config["Tempo:OtlpEndpoint"]
                                                ?? throw new ArgumentNullException("Tempo OtlpEndpoint"));
                                        });
                                })
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