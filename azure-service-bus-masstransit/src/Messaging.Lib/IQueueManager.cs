namespace Messaging.Lib
{
    public interface IQueueManager
    {
        Task SendMessage<T>(T message, string queueName) where T : class;
    }
}
