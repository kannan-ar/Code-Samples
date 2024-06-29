using MassTransit;
using Messaging.Lib.Messages;

namespace Messaging.Lib.Consumers
{
    public class RegularCustomerRegisteredConsumer : IConsumer<CustomerRegistered>
    {
        public Task Consume(ConsumeContext<CustomerRegistered> context)
        {
            return Task.CompletedTask;
        }
    }
}
