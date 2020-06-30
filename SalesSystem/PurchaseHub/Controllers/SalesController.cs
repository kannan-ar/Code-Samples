namespace PurchaseHub.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PurchaseHub.Filters;
    using PurchaseHub.Models;
    using PurchaseHub.Services;
    using PurchaseHub.Extensions;

    [HMACFilter]
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesQuery sales;
        private readonly ISalesLog log;
        public SalesController(ISalesQuery sales, ISalesLog log)
        {
            this.sales = sales;
            this.log = log;
        }

        [HttpGet]
        public async IAsyncEnumerable<Car> GetCars()
        {
            await foreach (var car in sales.GetCars())
            {
                yield return car;
            }
        }

        [HttpPost]
        public async Task<Purchase> Post(Purchase purchase)
        {
            Task salesTask = sales.Sale(purchase);
            
            Task.Run(() => {
                log.Log(purchase);
            }).FireAndForget();

            await salesTask.ConfigureAwait(false);
            
            return purchase;
        }
    }

    
}