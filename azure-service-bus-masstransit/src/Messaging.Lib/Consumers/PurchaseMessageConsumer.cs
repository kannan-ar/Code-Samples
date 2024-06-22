using MassTransit;
using Messaging.Lib.QueueMessages;

namespace Messaging.Lib.Consumers
{
    public class PurchaseMessageConsumer : IConsumer<PurchaseCreated>
    {
        public Task Consume(ConsumeContext<PurchaseCreated> context)
        {
            return Task.CompletedTask;
        }
    }
}
