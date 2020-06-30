namespace PurchaseHub.Services.Implementations
{
    using System;
    using StackExchange.Redis;

    using PurchaseHub.Models;
    public class SalesLog : ISalesLog
    {
        private readonly IDatabase db;
        public SalesLog(Lazy<IConnectionMultiplexer> redis)
        {
            this.db = redis.Value.GetDatabase();
        }
        public void Log(Purchase purchase)
        {
            HashEntry[] entries = new HashEntry[]{
                new HashEntry("user", purchase.User.Name),
                new HashEntry("seller", purchase.Product.Seller),
                new HashEntry("product", purchase.Product.Name),
                new HashEntry("amount", purchase.Product.Price * purchase.Quantity)
            };

            db.HashSet(Guid.NewGuid().ToString("N"), entries);
        }
    }
}