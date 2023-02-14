namespace ApiGateway.Services
{
    public interface IQueueService
    {
        Task AddMessageAsync(string queueName, string message);
    }
}
