namespace PurchaseHub.Services
{
    using PurchaseHub.Models;
    public interface ISalesLog
    {
        void Log(Purchase purchase);
    }
}