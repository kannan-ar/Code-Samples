namespace SalesSimulator.Models
{
    using System;
    public class Purchase
    {
        public Product Product { get; set; }
        public User User { get; set; }
        public int Quantity { get; set; }
        public ModeOfPayment Payment { get; set; }
        public string Carrier { get; set; }
        public DateTime PurchasedOn { get; set; }
    }
}