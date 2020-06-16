namespace PurchaseHub.Models
{
    using System;

    public class Purchase
    {
        public string Location { get; set; }
        public string UserName { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public ModeOfPayment ModeOfPayment { get; set; }
        public string Carrier { get; set; }
        public DateTime PurchasedOn { get; set; }
    }
}