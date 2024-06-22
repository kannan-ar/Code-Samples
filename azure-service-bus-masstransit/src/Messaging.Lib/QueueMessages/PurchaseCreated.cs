using MassTransit;

namespace Messaging.Lib.QueueMessages
{
    public record PurchaseCreated
    {
        public Guid Id { get; init; }
        public string? Product { get; init; }
        public int Quantity { get; init; }
    }
}
