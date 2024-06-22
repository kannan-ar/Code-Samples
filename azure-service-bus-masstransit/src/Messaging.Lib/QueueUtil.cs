namespace Messaging.Lib
{
    public static class QueueUtil
    {
        public static Uri GetQueueName(string queueName)
        {
            return new Uri($"queue:{queueName}");
        }
    }
}
