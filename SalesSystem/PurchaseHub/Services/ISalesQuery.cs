namespace PurchaseHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using PurchaseHub.Models;

    public interface ISalesQuery
    {
        IAsyncEnumerable<Car> GetCars();
        Task<Purchase> Sale(Purchase purchase);
    }
}