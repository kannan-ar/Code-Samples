namespace PurchaseHub.Services.Implementations
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    using PurchaseHub.Models;
    using PurchaseHub.Hubs;
    public class SalesLog : ISalesLog
    {
        private readonly IHubContext<SalesHub> salesHub;

        public SalesLog(IHubContext<SalesHub> salesHub)
        {
            this.salesHub = salesHub;
        }
        public async Task Log(Purchase purchase)
        {
            await salesHub.Clients.All.SendAsync("ReceiveSalesUpdate", purchase);
        }
    }
}