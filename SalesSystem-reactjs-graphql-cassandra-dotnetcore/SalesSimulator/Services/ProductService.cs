namespace SalesSimulator.Services
{
    using System;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;

    using Models;
    public class ProductService : IProductService
    {
        private readonly IConfiguration configuration;

        public ProductService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<IList<Product>> GetProducts()
        {
            var productsApi = configuration.GetSection("APIUrls")["ProductsAPI"];
            var httpClient = new HttpClient();

            if (string.IsNullOrWhiteSpace(productsApi))
            {
                throw new ArgumentNullException("Products API url should not be empty");
            }

            return await JsonSerializer.DeserializeAsync<IList<Product>>(await httpClient.GetStreamAsync(productsApi));
        }
    }
}