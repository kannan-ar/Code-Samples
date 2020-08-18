namespace StreamClient
{
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Text.Json;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        private static async IAsyncEnumerable<DateTime> GetDate(int delay)
        {
            using (var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            })
            {
                for(int i = 0; 5 > i; i++)
                {
                    var response = await client.GetAsync($"Date?delay={delay}").ConfigureAwait(false);
                    var data = await response.Content.ReadAsStringAsync();
                    yield return JsonSerializer.Deserialize<DateTime>(data);
                }
            }
        }

        static async Task Main(string[] args)
        {
            await foreach(var date in GetDate(1000))
            {
                Console.WriteLine(date);
            }

            Console.WriteLine("Done");
        }
    }
}
