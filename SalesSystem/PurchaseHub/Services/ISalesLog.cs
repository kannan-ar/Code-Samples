namespace PurchaseHub.Services
{
    using System.Threading.Tasks;
    using PurchaseHub.Models;
    public interface ISalesLog
    {
        Task Log(Purchase purchase);
    }
}