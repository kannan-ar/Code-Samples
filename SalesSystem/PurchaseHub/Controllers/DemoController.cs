using System.Diagnostics;
namespace PurchaseHub.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.SignalR;
    using PurchaseHub.Hubs;
    using PurchaseHub.Models;
    using PurchaseHub.Services;
    using PurchaseHub.Extensions;

    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ISalesLog log;


        public DemoController(ISalesLog log)
        {
            this.log = log;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            Task task = Task.Delay(1000);

            Task.Run(() => {
                log.Log(new Purchase());
            }).FireAndForget();

            await task;
            
            return Enumerable.Range(1, 5).Select(x => $"Test {x}");
        }
    }
}