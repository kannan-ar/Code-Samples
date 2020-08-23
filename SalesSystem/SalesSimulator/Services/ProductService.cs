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
    using Utils;

    using Models;
    public class ProductService : IProductService
    {
        private readonly IConfiguration configuration;
        private readonly Random random;
        public ProductService(IConfiguration configuration)
        {
            this.configuration = configuration;
            random = new Random();
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

        public IEnumerable<Product> GetProductsByInterests(ReadOnlyCollection<Product> allProducts, IList<string> interests)
        {
            return allProducts.Where(p => p.Categories.Any(c => interests.Contains(c)));
        }

        public IEnumerable<Product> GetProductsToBuy(IList<Product> products)
        {
            var productList = products.ToList();
            var count = productList.Count();

            var buyCount = random.Next(1, count);

            for (int i = 0; buyCount > i; i++)
            {
                yield return productList[random.Next(0, count - 1)];
            }
        }
    }
}