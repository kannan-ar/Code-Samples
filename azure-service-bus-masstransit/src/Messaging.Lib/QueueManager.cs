using MassTransit;

namespace Messaging.Lib
{
    public class QueueManager : IQueueManager
    {
        private readonly ISendEndpointProvider _endpointProvider;

        public QueueManager(ISendEndpointProvider endpointProvider)
        {
            _endpointProvider = endpointProvider;

        }

        public async Task SendMessage<T>(T message, string queueName)
            where T : class
        {
            var endpoint = await _endpointProvider.GetSendEndpoint(QueueUtil.GetQueueName(queueName));
            await endpoint.Send(message);
        }
    }
}
