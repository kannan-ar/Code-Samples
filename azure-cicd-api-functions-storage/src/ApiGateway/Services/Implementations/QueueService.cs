using Azure.Storage.Queues;

namespace ApiGateway.Services.Implementations
{
    public class QueueService : IQueueService
    {
        private readonly QueueServiceClient queueServiceClient;

        public QueueService(QueueServiceClient queueServiceClient)
        {
            this.queueServiceClient = queueServiceClient;
        }

        public async Task AddMessageAsync(string queueName, string message)
        {
            var queue = queueServiceClient.GetQueueClient(queueName);
            _ = await queue.CreateIfNotExistsAsync();
            await queue.SendMessageAsync(message);
        }
    }
}
