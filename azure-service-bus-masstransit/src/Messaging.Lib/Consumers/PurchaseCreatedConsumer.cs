using MassTransit;
using Messaging.Lib.Messages;

namespace Messaging.Lib.Consumers
{
    public class PurchaseCreatedConsumer : IConsumer<PurchaseCreated>
    {
        private readonly ITopicManager _topicManager;

        public PurchaseCreatedConsumer(ITopicManager topicManager)
        {
            _topicManager = topicManager;
        }

        public async Task Consume(ConsumeContext<PurchaseCreated> context)
        {
            await _topicManager.Publish(new PurchaseCompleted
            {
                Product = context.Message.Product,
                Quantity = context.Message.Quantity
            });
        }
    }
}
