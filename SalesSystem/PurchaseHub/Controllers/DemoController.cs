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
            await log.Log(new Purchase() { Carrier = "DD" });
            return Enumerable.Range(1, 5).Select(x => $"Test {x}");
        }
    }
}