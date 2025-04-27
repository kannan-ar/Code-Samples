namespace CommandServiceApi.Contracts
{
    public interface IOrderEvent
    {
        string EventType { get; }
        Guid OrderId { get; set; }
    }
}
