namespace SalesSimulator.Models
{
    using System.Text.Json.Serialization;
    public class Carrier
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}