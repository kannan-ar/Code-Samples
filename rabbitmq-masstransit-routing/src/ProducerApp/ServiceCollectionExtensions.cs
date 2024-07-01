using MassTransit;
using Messaging.Lib;
using Messaging.Lib.Messages;
using Microsoft.EntityFrameworkCore;

namespace ProducerApp
{
    public static class ServiceCollectionExtensions
    {
        public static void InitDatabase(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var dbcotext = serviceProvider.GetRequiredService<SqlDbContext>();
            dbcotext.Database.Migrate();
        }

        public static void ConfigExchange<T>(
            this IRabbitMqBusFactoryConfigurator cfg,
            Func<SendContext<T>, string> formatter,
            string entityName,
            string exchangeType)
            where T : class
        {
            cfg.Send<T>(x =>
            {
                x.UseRoutingKeyFormatter(formatter);
            });

            cfg.Message<T>(x => x.SetEntityName(entityName));
            cfg.Publish<T>(x => x.ExchangeType = exchangeType);
        }
    }
}
