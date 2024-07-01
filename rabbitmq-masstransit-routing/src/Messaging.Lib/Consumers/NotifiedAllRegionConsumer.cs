using MassTransit;
using Messaging.Lib.Messages;

namespace Messaging.Lib.Consumers
{
    public class NotifiedAllRegionConsumer : IConsumer<NotifiedRegion>
    {
        public Task Consume(ConsumeContext<NotifiedRegion> context)
        {
            return Task.CompletedTask;
        }
    }
}
