namespace WebApp.Models
{
    public class PurchaseOrder
    {
        public Guid Id { get; set; }
        public string? Product { get; set; }
        public int Quantity { get; set; }
    }
}
