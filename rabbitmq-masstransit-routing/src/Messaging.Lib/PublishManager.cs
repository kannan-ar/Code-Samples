using MassTransit;

namespace Messaging.Lib
{
    public class PublishManager : IPublishManager
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly SqlDbContext _sqlDbContext;

        public PublishManager(IPublishEndpoint publishEndpoint, SqlDbContext sqlDbContext)
        {
            _publishEndpoint = publishEndpoint;
            _sqlDbContext = sqlDbContext;
        }

        public async Task Publish<T>(T message) where T : class
        {
            await _publishEndpoint.Publish(message);
            _sqlDbContext.SaveChanges();
        }
    }
}
