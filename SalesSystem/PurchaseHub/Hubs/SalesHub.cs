namespace PurchaseHub.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    using PurchaseHub.Models;

    public class SalesHub : Hub
    {
        public async Task SendSalesUpdate(Purchase purchase)
        {
            await Clients.All.SendAsync("ReceiveSalesUpdate", purchase);
        }
    }
}