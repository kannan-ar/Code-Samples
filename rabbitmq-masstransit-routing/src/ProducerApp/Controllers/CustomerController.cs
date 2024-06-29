using Messaging.Lib;
using Messaging.Lib.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProducerApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IPublishManager _publishManager;

        public CustomerController(IPublishManager publishManager)
        {
            _publishManager = publishManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerRegistered model)
        {
            await _publishManager.Publish(model);

            return Accepted();
        }
    }
}
