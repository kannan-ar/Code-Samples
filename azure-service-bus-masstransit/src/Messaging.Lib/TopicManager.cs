
using MassTransit;

namespace Messaging.Lib
{
    public class TopicManager : ITopicManager
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public TopicManager(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Publish<T>(T message) where T : class
        {
            await _publishEndpoint.Publish(message);
        }
    }
}
