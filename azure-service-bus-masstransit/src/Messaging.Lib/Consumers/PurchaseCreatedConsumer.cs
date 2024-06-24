using MassTransit;
using Messaging.Lib.Messages;

namespace Messaging.Lib.Consumers
{
    public class PurchaseCreatedConsumer : IConsumer<PurchaseCreated>
    {
        public Task Consume(ConsumeContext<PurchaseCreated> context)
        {
            return Task.CompletedTask;
        }
    }
}
