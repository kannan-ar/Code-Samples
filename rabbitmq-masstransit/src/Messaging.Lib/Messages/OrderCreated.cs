namespace Messaging.Lib.Messages
{
    public class OrderCreated
    {
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
