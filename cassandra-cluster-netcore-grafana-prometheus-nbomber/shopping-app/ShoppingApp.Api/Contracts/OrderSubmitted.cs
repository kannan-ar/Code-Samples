namespace ShoppingApp.Api.Contracts
{
    public class OrderSubmitted
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
