namespace SalesSimulator
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(dataAsString);
        }
    }
}