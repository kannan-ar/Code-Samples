namespace Messaging.Lib
{
    public interface ITopicManager
    {
        Task Publish<T>(T message) where T : class;
    }
}
