namespace Messaging.Lib
{
    public interface IPublishManager
    {
        Task Publish<T>(T message) where T : class;
    }
}
