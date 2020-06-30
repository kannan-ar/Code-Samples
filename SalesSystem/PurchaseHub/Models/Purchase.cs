namespace PurchaseHub.Models
{
    using System;
    using System.Text.Json.Serialization;
    public class Purchase
    {
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("product")]
        public Product Product { get; set; }
        [JsonPropertyName("user")]
        public User User { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("payment")]
        public ModeOfPayment Payment { get; set; }
        [JsonPropertyName("carrier")]
        public string Carrier { get; set; }
        [JsonPropertyName("purchasedon")]
        public DateTime PurchasedOn { get; set; }
    }
}