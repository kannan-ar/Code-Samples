namespace PurchaseHub.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PurchaseHub.Filters;
    using PurchaseHub.Models;
    using PurchaseHub.Services;

    [HMACFilter]
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesQuery sales;
        public SalesController(ISalesQuery sales)
        {
            this.sales = sales;
        }

        [HttpGet]
        public async IAsyncEnumerable<Car> GetCars()
        {
            await foreach(var car in sales.GetCars())
            {
                yield return car;
            }
        }

        [HttpPost]
        public async Task<Purchase> Post(Purchase purchase)
        {
            //await sales.Sale(purchase);
            await Task.Delay(100);
            return purchase;
        }
    }
}