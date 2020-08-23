namespace SalesSimulator.Services
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;

    using SalesSimulator.Models;

    public class SaleService : ISaleService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpFactory;
        private readonly ConsoleLogger logger;
        private readonly Random random;


        public SaleService(
            IConfiguration configuration,
            IHttpClientFactory httpFactory,
            ConsoleLogger logger)
        {
            this.configuration = configuration;
            this.httpFactory = httpFactory;
            this.logger = logger;

            random = new Random();
        }

        public ModeOfPayment GetModeOfPayment()
        {
            Type type = typeof(ModeOfPayment);

            var names = Enum.GetNames(type);
            return (ModeOfPayment)Enum.Parse(type, names[random.Next(0, names.Length)]);
        }

        public async Task Buy(Purchase purchase)
        {
            string apiBaseAddress = configuration.GetSection("APIUrls")["SalesAPI"];

            HttpClient client = httpFactory.CreateClient("HMAClient");

            HttpResponseMessage response = await client.PostAsJsonAsync(apiBaseAddress + "Sales", purchase).ConfigureAwait(false);

            string responseString = await response.Content.ReadAsStringAsync();
            logger.Add(responseString);
        }
    }
}