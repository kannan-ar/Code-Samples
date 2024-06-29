using MassTransit;
using Messaging.Lib.Messages;

namespace Messaging.Lib.Consumers
{
    public class PremiumCustomerRegisteredConsumer : IConsumer<CustomerRegistered>
    {
        public Task Consume(ConsumeContext<CustomerRegistered> context)
        {
            return Task.CompletedTask;
        }
    }
}
