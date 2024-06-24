using MassTransit;
using Messaging.Lib.Messages;

namespace Messaging.Lib.Consumers
{
    public class PurchaseCompletedConsumer : IConsumer<PurchaseCompleted>
    {
        public Task Consume(ConsumeContext<PurchaseCompleted> context)
        {
            return Task.CompletedTask;
        }
    }
}
