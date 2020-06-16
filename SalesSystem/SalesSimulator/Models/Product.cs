
namespace SalesSimulator.Models
{
    using System.Text.Json.Serialization;
    public sealed class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("seller")]
        public string Seller{get; set;}
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("price")]
        public double Price { get; set; }
        [JsonPropertyName("categories")]
        public string[] Categories { get; set; }
    }
}