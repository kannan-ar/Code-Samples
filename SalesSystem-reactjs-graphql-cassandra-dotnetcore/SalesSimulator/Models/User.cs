namespace SalesSimulator.Models
{
    using System.Text.Json.Serialization;
    public sealed class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("interests")]
        public string[] Interests { get; set; }
    }
}