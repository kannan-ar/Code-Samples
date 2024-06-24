namespace Messaging.Lib.Messages
{
    public class PurchaseCompleted
    {
        public string? Product { get; init; }
        public int Quantity { get; init; }
    }
}
