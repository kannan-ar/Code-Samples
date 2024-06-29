using MassTransit;
using Messaging.Lib;
using Messaging.Lib.Consumers;

namespace ConsumerApp
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureConsumerEndPoint<T>(
            this IRabbitMqBusFactoryConfigurator cfg,
            IBusRegistrationContext context,
            string queueName)
            where T : class, IConsumer
        {
            cfg.ReceiveEndpoint(queueName, e =>
            {
                e.UseEntityFrameworkOutbox<SqlDbContext>(context);
                e.ConfigureConsumer<T>(context);
            });
        }

        public static void ConfigureConsumerEndPoint<T>(
           this IRabbitMqBusFactoryConfigurator cfg,
           IBusRegistrationContext context,
           string queueName,
           string exchangeName,
           string routingKey,
           string exchangeType)
           where T : class, IConsumer
        {
            cfg.ReceiveEndpoint(queueName, e =>
            {
                e.ConfigureConsumer<T>(context);
                e.Bind(exchangeName, e =>
                {
                    e.RoutingKey = routingKey;
                    e.ExchangeType = exchangeType;
                });
            });
        }
    }
}
