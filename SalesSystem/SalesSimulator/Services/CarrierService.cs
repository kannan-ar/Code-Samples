namespace SalesSimulator.Services
{
    using System;
    using Microsoft.Extensions.Configuration;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Linq;

    using SalesSimulator.Models;
    public class CarrierService : ICarrierService
    {
        private readonly IConfiguration configuration;
        private readonly Random random;
        public CarrierService(IConfiguration configuration)
        {
            this.configuration = configuration;
            random = new Random();
        }
        public async Task<IList<Carrier>> GetCarriers()
        {
            var carrierApi = configuration.GetSection("APIUrls")["CarriersAPI"];
            var httpClient = new HttpClient();

            if (string.IsNullOrWhiteSpace(carrierApi))
            {
                throw new ArgumentNullException("Carrier API url should not be empty");
            }

            return await JsonSerializer.DeserializeAsync<IList<Carrier>>(await httpClient.GetStreamAsync(carrierApi));
        }

        public Carrier GetCarrier(ReadOnlyCollection<Carrier> allCarriers)
        {
            var randomCount = random.Next(0, allCarriers.Count());
            return allCarriers[randomCount];
        }
    }
}