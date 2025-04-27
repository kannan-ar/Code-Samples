using CommandServiceApi.Contracts;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;

namespace CommandServiceApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var configSection = configuration.GetSection("Config");

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => { });

                x.AddRider(rider =>
                {
                    var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
                    {
                        Url = configSection.GetValue<string>("SchemaRegistry")
                    });

                    rider.AddProducer<OrderCreated>("order-events", (context, producer) =>
                    {
                        producer.SetValueSerializer(new JsonSerializer<OrderCreated>(schemaRegistry));
                    });

                    rider.UsingKafka((context, kafkaConfig) =>
                    {
                        kafkaConfig.Host(configSection.GetValue<string>("KafkaEndpoint"));
                    });
                });
            });
        }
    }
}
