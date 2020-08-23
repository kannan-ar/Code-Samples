namespace PurchaseHub.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PurchaseHub.Models;

    public class SalesQuery : ISalesQuery
    {
        private readonly IDbQuery query;

        public SalesQuery(IDbQuery query)
        {
            this.query = query;
        }
        public async IAsyncEnumerable<Car> GetCars()
        {
            var rows = await query.ExecuteAsync("SELECT * FROM car_offers");

            foreach (var row in rows)
            {
                yield return new Car() { Brand = Convert.ToString(row["brand"]) };
            }
        }

        public async Task<Purchase> Sale(Purchase purchase)
        {
            //await query.ExecuteAsync("");
            await Task.Yield();
            return purchase;
        }
    }
}